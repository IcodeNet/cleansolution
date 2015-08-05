namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the StateMachineInfo class
    /// </summary>
    [TestClass]
    public class StateMachineInfoTest
    {

        /// <summary>
        ///   Given
        ///   * A StateTracker
        /// * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Closed
        /// </summary>
        [TestMethod]
        public void InstanceStateIsClosedWhenClosed()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                host.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1, 1, StateTrigger.T5);
                host.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T5, 1);
                var stateMachineInfo = tracker.StateMachines[0];
                var actual = stateMachineInfo.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Closed, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }


        /// <summary>
        ///   Given
        ///   * A StateTracker
        /// * An activity with one state machine
        ///   When
        ///   * The activity is run
        ///   Then
        ///   * The InstanceState returns the instance state of the StateMachine
        /// </summary>
        [TestMethod]
        public void InstanceStateIsExecutingWhenExecuting()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                var stateMachineInfo = tracker.StateMachines[0];
                var actual = stateMachineInfo.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Executing, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        /// * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Faulted
        /// </summary>
        [TestMethod]
        public void InstanceStateIsFaultedWhenFaulted()
        {
            // Arrange
            var activity = StateTrackerTest.CreateStateMachineThatFaults();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                AssertHelper.Throws<InvalidOperationException>(() => host.TestWorkflowApplication.RunUntilBookmark());
                var stateMachineInfo = tracker.StateMachines[0];
                var actual = stateMachineInfo.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Faulted, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        /// * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Canceled
        /// </summary>
        [TestMethod]
        public void InstanceStateIsCanceledWhenCanceled()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                host.Cancel();
                var stateMachineInfo = tracker.StateMachines[0];
                var actual = stateMachineInfo.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Canceled, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }        
    }
}