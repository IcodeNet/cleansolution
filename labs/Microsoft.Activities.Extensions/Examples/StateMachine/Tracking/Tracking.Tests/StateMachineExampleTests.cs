// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineExampleTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Tests
{
    using System.Activities;
    using System.Linq;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.DurableInstancing;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Tracking.Windows.Activities;

    /// <summary>
    ///   The state machine example tests.
    /// </summary>
    [TestClass]
    public class StateMachineExampleTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// Initializes the test class
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <remarks>
        /// Since this class uses databases, this initialization
        ///   will attempt to remove databases leftover from previous
        ///   test runs
        /// </remarks>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            SqlDatabaseTest.TryDropDatabasesWithPrefix();
        }

        /// <summary>
        ///   Given
        ///   * A StateMachineExample
        ///   When
        ///   * Started
        ///   Then
        ///   * The StateMachine will become idle waiting for transitions T1 or T2
        /// </summary>
        [TestMethod]
        public void StateMachineWillIdleWithT1OrT2()
        {
            // Arrange
            WorkflowTestTrace.Arrange();
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var stateTracker = StateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                // Act
                WorkflowTestTrace.Act();
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                // Assert
                WorkflowTestTrace.Assert();
                Assert.IsTrue(stateTracker.PossibleTransitions.Any(s => s == "T1"));
            }
            finally
            {
                WorkflowTestTrace.Finally();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A state machine example workflow
        ///   * An instance store 
        ///   * A host configured to unload on persistable idle
        ///   * A state tracker attached to the host and instance store
        ///   When
        ///   * The workflow is run to idle
        ///   Then
        ///   * The host will persist and unload the workflow
        ///   * The state tracker will persist the state machine info
        ///   * The state tracker can then load the persisted info which will contain the possible transition
        /// </summary>
        [TestMethod]
        public void StateTrackerCanLoadPersistedInstance()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var host = CreateInstanceAndPersist(testdb);

                // Load the state tracker from the database
                var loadTracker = StateTracker.LoadInstance(host.Id, testdb.ConnectionString);

                try
                {
                    // Act
                    WorkflowTestTrace.Act();
                    var host2 = WorkflowApplicationTest.Create(new StateMachineExample());
                    host2.InstanceStore = testdb.InstanceStore;
                    var tracker = StateTracker.Attach(host2.TestWorkflowApplication, testdb.InstanceStore);
                    host2.Load(loadTracker.InstanceId);
                    try
                    {
                        host2.TestWorkflowApplication.ResumeUntilAnyBookmark(StateTrigger.T2, null);

                        // Assert
                        WorkflowTestTrace.Assert();
                        Assert.IsTrue(tracker.PossibleTransitions.Any(s => s == "T4"));
                    }
                    finally
                    {
                        WorkflowTestTrace.Finally("Host 2");
                        host2.Tracking.Trace();                        
                    }
                }
                finally
                {
                    WorkflowTestTrace.Finally("Host 1");
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   * A state machine example workflow
        ///   * An instance store 
        ///   * A host configured to unload on persistable idle
        ///   * A state tracker attached to the host and instance store
        ///   When
        ///   * The workflow is run to idle
        ///   Then
        ///   * The host will persist and unload the workflow
        ///   * The state tracker will persist the state machine info
        ///   * The state tracker can then load the persisted info which will contain the possible transition
        /// </summary>
        [TestMethod]
        public void StateTrackerCanLoadPersistedStateInfo()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var host = CreateInstanceAndPersist(testdb);

                try
                {
                    // Act
                    WorkflowTestTrace.Act();

                    // Load the state tracker from the database
                    var loadTracker = StateTracker.LoadInstance(host.Id, testdb.ConnectionString);

                    // Assert
                    WorkflowTestTrace.Assert();
                    Assert.IsTrue(loadTracker.PossibleTransitions.Any(s => s == "T1"));
                }
                finally
                {
                    WorkflowTestTrace.Finally();
                    host.Tracking.Trace();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create instance and persist.
        /// </summary>
        /// <param name="testdb">
        /// The test database.
        /// </param>
        /// <returns>
        /// The <see cref="WorkflowApplicationTest"/>.
        /// </returns>
        private static WorkflowApplicationTest<StateMachineExample> CreateInstanceAndPersist(
            SqlWorkflowInstanceStoreTest testdb)
        {
            WorkflowTestTrace.Arrange();
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);

            var instanceStore = SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                testdb.DatabaseName, testdb.ConnectionString, true);
            host.InstanceStore = instanceStore;
            host.PersistableIdle += args => PersistableIdleAction.Unload;
            var stateTracker = StateTracker.Attach(host.TestWorkflowApplication, instanceStore);

            // Act
            host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

            Assert.IsTrue(host.WaitForUnloadedEvent());
            return host;
        }

        #endregion
    }
}