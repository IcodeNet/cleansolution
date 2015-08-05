namespace SecurityDoor.Tests
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using System.Threading;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SecurityDoor.Tests.SecurityWeb;
    using SecurityDoor.Web;

    /// <summary>
    ///   Tests for the SecurityDoorWorkflowService
    /// </summary>
    [TestClass]
    public class SecurityDoorServiceTests
    {
        #region Constants and Fields

        public const int TestIntrusionThreshold = 2;

        public readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(1);

        private readonly NetNamedPipeBinding binding = new NetNamedPipeBinding();

        /// <summary>
        ///   The service uri.
        /// </summary>
        private readonly EndpointAddress serviceAddress = new EndpointAddress(
            "net.pipe://localhost/securityDoorService");

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Given
        ///   - A security door in the Open state
        ///   When
        ///   - The close notification is received
        ///   Then
        ///   - The security door should transition to the Closed/Locked state
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void ClosedDoorShouldReturnToClosedLocked()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    // Arrange
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    // Authorize and unlock the door
                    var response =
                        proxy.AuthorizeKey(100, Guid.NewGuid(), TestTimeout, TestIntrusionThreshold);

                    // Notify that the door is opened and unlocked
                    proxy.NotifyDoorStatus(100, true, false);

                    // Notify that the door is closed and locked
                    proxy.NotifyDoorStatus(100, false, true);

                    proxy.Close();

                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify the authorize was successful
                    Assert.IsTrue(response.Authorized);

                    // Verify the timeout occured by the states that occur in the tracking
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.ClosedUnlocked,
                        States.Open,
                        States.ClosedLocked);

                    // Verify that the Closed transition was triggered
                    host.Tracking.Assert.Exists(Transitions.ReceiveDoorClosed, ActivityInstanceState.Closed);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Verify that an invalid key will not open the door
        /// </summary>
        /// <remarks>
        ///   Given
        ///   - A closed / locked door
        ///   - An invalid card key
        ///   When
        ///   - The AuthorizeKey message is sent
        ///   Then
        ///   - The response will be false
        ///   - The door will remain in the Closed / Locked State
        /// </remarks>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void InvalidKeyShouldNotAuthorize()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    var response = proxy.AuthorizeKey(100, Guid.Empty,this.TestTimeout, TestIntrusionThreshold);

                    proxy.Close();
                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify that the door was unlocked
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine, host.Tracking.Records, States.ClosedLocked);

                    // Verify the key was not authorized
                    Assert.IsFalse(response.Authorized);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   - A security door in the Open state
        ///   When
        ///   - The open timeout is exceeded
        ///   Then
        ///   - The security door should transition to the Alert state
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void OpenedDoorShouldTimeoutIfNotClosed()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    // Arrange
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    // Authorize and unlock the door
                    var response = proxy.AuthorizeKey(100, Guid.NewGuid(),this.TestTimeout, TestIntrusionThreshold);

                    // Notify that the door is opened and unlocked
                    proxy.NotifyDoorStatus(100, true, false);

                    // Wait for the open timeout
                    Thread.Sleep(this.TestTimeout.Milliseconds + 1000);

                    proxy.Close();

                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify the authorize was successful
                    Assert.IsTrue(response.Authorized);

                    // Verify the timeout occured by the states that occur in the tracking
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.ClosedUnlocked,
                        States.Open,
                        States.Alert);

                    // Verify that the Open Timeout transition was triggered
                    host.Tracking.Assert.Exists(Transitions.OpenTimeout, ActivityInstanceState.Closed);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   - A security door in the closed/unlocked state
        ///   When
        ///   - The open notification is received
        ///   Then
        ///   - The security door should transition to the opened state
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void OpenedDoorShouldTransitionOnNotify()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    // Arrange
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    // Authorize and unlock the door
                    var response = proxy.AuthorizeKey(100, Guid.NewGuid(),this.TestTimeout, TestIntrusionThreshold);

                    // Notify that the door is opened and unlocked
                    proxy.NotifyDoorStatus(100, true, false);

                    proxy.Close();

                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify the authorize was successful
                    Assert.IsTrue(response.Authorized);

                    // Verify the timeout occured by the states that occur in the tracking
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.ClosedUnlocked,
                        States.Open);

                    // Verify that the ReceiveDoorOpen transition was triggered
                    host.Tracking.Assert.Exists(Transitions.ReceiveDoorOpen, ActivityInstanceState.Closed);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Verify that repeated attempts to use an invalid key will alert staff
        /// </summary>
        /// <remarks>
        ///   Given
        ///   - A closed / locked door
        ///   - An invalid card key
        ///   When
        ///   - The AuthorizeKey message is sent
        ///   Then
        ///   - The response will be false
        ///   - The door will remain in the Closed / Locked State
        /// </remarks>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void RepeatedInvalidKeyShouldAlertIntrusion()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    for (var i = 0; i < TestIntrusionThreshold; i++)
                    {
                        proxy.AuthorizeKey(100, Guid.Empty,TimeSpan.FromMilliseconds(1), TestIntrusionThreshold);
                    }

                    proxy.Close();
                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify that the door was unlocked
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.IntrusionDetect,
                        States.ClosedLocked,
                        States.IntrusionDetect,
                        States.Alert);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Verify that a valid card key will unlock the door
        /// </summary>
        /// <remarks>
        ///   Given
        ///   - A closed / locked door
        ///   - A valid card key
        ///   When
        ///   - The AuthorizeKey message is sent
        ///   Then
        ///   - The response will be true
        ///   - The door will be in the Closed / Unlocked State
        /// </remarks>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void ValidKeyShouldAuthorize()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    var response =
                        proxy.AuthorizeKey(100, Guid.NewGuid(), TestTimeout, TestIntrusionThreshold);

                    proxy.Close();
                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify that the door was unlocked
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.ClosedUnlocked);

                    // Verify the key was authorized
                    Assert.IsTrue(response.Authorized);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   - A security door in the closed/locked state
        ///   When
        ///   - The open timeout expires
        ///   Then
        ///   - The security door should return to the closed/locked state
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SecurityDoor.Web\SecurityDoorService.xamlx")]
        public void VerifyTimeoutIfNotOpened()
        {
            using (var host = WorkflowServiceTestHost.Open("SecurityDoorService.xamlx", this.serviceAddress))
            {
                try
                {
                    // Arrange
                    var proxy = new SecurityDoorServiceClient(this.binding, this.serviceAddress);

                    // Authorize and unlock the door
                    var response =
                        proxy.AuthorizeKey(100, Guid.NewGuid(), TestTimeout, TestIntrusionThreshold);
                    proxy.Close();

                    // Wait for timeout
                    Thread.Sleep(this.TestTimeout.Add(TimeSpan.FromSeconds(1)));

                    // By this time the timeout should have occurred.
                    host.Close();

                    // Wait for the host to close before checking the tracking data
                    Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed(1000));

                    // Verify the authorize was successful
                    Assert.IsTrue(response.Authorized);

                    // Verify the timeout occured by the states that occur in the tracking
                    AssertState.OccursInOrder(
                        Constants.SecurityDoorStateMachine,
                        host.Tracking.Records,
                        States.ClosedLocked,
                        States.ClosedUnlocked,
                        States.ClosedLocked);

                    // Verify that the UnlockedTimeout transition was triggered
                    host.Tracking.Assert.Exists(Transitions.UnlockedTimeout, ActivityInstanceState.Closed);
                }
                finally
                {
                    host.Tracking.Trace();
                }
            }
        }

        #endregion
    }
}