// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTrackerPersistenceProviderTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
#if NET401_OR_GREATER
    using System;
    using System.Activities;
    using System.Activities.DurableInstancing;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.DurableInstancing;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Tests the StateTrackerPersistence
    /// </summary>
    [TestClass]
    public class StateTrackerPersistenceTests
    {
        /// <summary>
        ///   Initializes the test class
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <remarks>
        /// Since this class uses databases, this initialization
        /// will attempt to remove databases leftover from previous
        /// test runs
        /// </remarks>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            SqlDatabaseTest.TryDropDatabasesWithPrefix();
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        public void TrackerShouldLoadInstancesWithActivity()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();
                var id = this.RunSampleStateMachine(testdb, StateTrigger.T1, StateTrigger.T3);

                // Act
                var instance = StateTracker.LoadInstance(id, testdb.ConnectionString);

                // Assert
                Assert.IsNotNull(instance);
                Assert.AreEqual(id, instance.InstanceId);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        public void LoadInstancesLoadsTwo()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Create a couple of state machines in the persistence store
                this.RunSampleStateMachine(testdb, StateTrigger.T1, StateTrigger.T3);
                this.RunSampleStateMachine(testdb, StateTrigger.T1, StateTrigger.T3, StateTrigger.T2);

                var instances = StateTracker.LoadInstances(testdb.ConnectionString);

                Assert.IsNotNull(instances);
                Assert.AreEqual(2, instances.Count);

                foreach (var stateTracker in instances)
                {
                    stateTracker.Trace();
                }
            }
        }
        /// <summary>
        ///   Runs the sample state machine through a number of triggers and leaves a persisted instance
        /// </summary>
        /// <param name="testdb"> The test database </param>
        /// <param name="triggers"> The triggers </param>
        /// <returns> The workflow instance ID </returns>
        private Guid RunSampleStateMachine(SqlWorkflowInstanceStoreTest testdb, params StateTrigger[] triggers)
        {
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var sqlWorkflowInstanceStore = new SqlWorkflowInstanceStore(testdb.ConnectionString);
            host.InstanceStore = sqlWorkflowInstanceStore;
            StateTracker.Attach(host.TestWorkflowApplication, sqlWorkflowInstanceStore);

            using (host)
            {
                // Start the workflow and run until it becomes idle with at least one bookmark
                host.TestWorkflowApplication.RunUntilAnyBookmark(Constants.Timeout);

                // Play each of the triggers
                foreach (var trigger in triggers)
                {
                    // Resume the workflow and run until any bookmark
                    host.TestWorkflowApplication.ResumeUntilBookmark(trigger, null, Constants.Timeout);
                }

                host.Unload(Constants.Timeout);
            }

            return host.Id;
        }
        /// <summary>
        ///   The test database
        /// </summary>
        private static readonly SqlWorkflowInstanceStoreTest TestInstanceStore = new SqlWorkflowInstanceStoreTest();

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A WorkflowApplication with a StateMachine and SqlWorkflowInstanceStore
        ///   When
        ///   * StateTracker is invoked
        ///   Then
        ///   * A StateTracker is added to the extensions collection
        ///   * A StateTrackerPersistence is added to the extensions collection
        ///   * The instance store is set as the instance store on the WorkflowApplication
        /// </summary>
        [TestMethod]
        public void AttachShouldAttachToApplication()
        {
            var activity = new StateMachineExample();
            Debug.Assert(TestInstanceStore != null, "TestInstanceStore != null");
            var sqlStore = new SqlWorkflowInstanceStore(TestInstanceStore.ConnectionString);

            // Setup the host
            var host = CreateHost(activity, sqlStore);

            // Setup the tracker
            Debug.Assert(host != null, "host != null");

            var tracker = StateTracker.Attach(host.TestWorkflowApplication, sqlStore);

            try
            {
                // Using Microsoft.Activities.Extensions run the workflow until a bookmark named "T1"
                var result = host.TestWorkflowApplication.RunEpisode("T1", TimeSpan.FromSeconds(60));

                Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
                Assert.IsTrue(host.WaitForUnloadedEvent(Constants.Timeout), "Host did not unload");

                Debug.Assert(result != null, "result != null");
                var instance = StateTracker.LoadInstance(result.InstanceId, TestInstanceStore.ConnectionString);

                Assert.IsNotNull(instance, "Failed to load instance");
                Assert.IsNotNull(tracker);
                Assert.AreEqual(
                    tracker.CurrentState,
                    instance.CurrentState,
                    "State read from database does not match current state from tracker");
            }
            finally
            {
                if (tracker != null)
                {
                    tracker.Trace();
                }

                if (host.Tracking != null)
                {
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowApplication with a StateMachine and null SqlWorkflowInstanceStore
        ///   When
        ///   * StateTracker ctor is invoked
        ///   Then
        ///   * ArgumentNullException
        /// </summary>
        [TestMethod]
        public void NullInstanceStoreShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(
                () => new StateTrackerPersistence(new StateTracker(), null));
        }

        /// <summary>
        ///   Given
        ///   * A null SqlWorkflowInstanceStore
        ///   When
        ///   * StateTracker.Promote is invoked
        ///   Then
        ///   * ArgumentNullException
        /// </summary>
        [TestMethod]
        public void NullInstanceStoreShouldThrowOnPromote()
        {
            AssertHelper.Throws<ArgumentNullException>(() => StateTrackerPersistence.Promote(null));
        }

        /// <summary>
        ///   Given
        ///   * nothing
        ///   When
        ///   * StateTracker ctor is invoked with null tracker
        ///   Then
        ///   * ArgumentNullException
        /// </summary>
        [TestMethod]
        public void NullTrackerShouldThrow()
        {
            // Setup the tracker
            AssertHelper.Throws<ArgumentNullException>(() => new StateTrackerPersistence(null));
        }


        /// <summary>
        ///   Test initialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            Debug.Assert(TestInstanceStore != null, "TestInstanceStore != null");
            SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                TestInstanceStore.DatabaseName, TestInstanceStore.ConnectionString, true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the workflow application test host
        /// </summary>
        /// <param name="activity">
        /// The activity. 
        /// </param>
        /// <param name="sqlStore">
        /// The sql store. 
        /// </param>
        /// <returns>
        /// A configured host 
        /// </returns>
        private static WorkflowApplicationTest<StateMachineExample> CreateHost(
            StateMachineExample activity, SqlWorkflowInstanceStore sqlStore)
        {
            var host = WorkflowApplicationTest.Create(activity);
            if (sqlStore != null)
            {
                Debug.Assert(host != null, "host != null");
                host.InstanceStore = sqlStore;
            }

            Debug.Assert(host != null, "host != null");
            host.PersistableIdle += args => PersistableIdleAction.Unload;
            return host;
        }

        #endregion
    }
#endif
}