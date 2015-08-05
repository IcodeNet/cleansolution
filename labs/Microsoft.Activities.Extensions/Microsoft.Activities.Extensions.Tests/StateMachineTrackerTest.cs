namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.DurableInstancing;
    using System.Activities.Statements;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.Activities.Description;

    using Microsoft.Activities.Extensions.ServiceModel;
    using Microsoft.Activities.Extensions.Tests.ServiceReference1;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TrackingStateMachine.Activities;

    /// <summary>
    ///   This is a test class for StateMachineTrackerTest and is intended
    ///   to contain all StateMachineTrackerTest Unit Tests
    /// </summary>
    /// <remarks>
    ///   TODO: What happens when tracked activity is versioned? Can dynamic update work?
    ///   TODO: What happens if more than one PersistenceParticipant is used?
    /// </remarks>
    [TestClass]
    public class StateMachineTrackerTest
    {
        #region Public Methods and Operators

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
        ///   * A WorkflowApplication
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * A StateMachineStateTracker is returned
        ///   * The InstanceStore is added to the WorkflowApplicaiton
        ///   * A StateMachineStateTracker is added to the Extensions
        ///   * A StateTrackerPersistence is created and added to the extensions
        /// </summary>
        [TestMethod]
        public void AttachNullWorkflowApplicationShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var store = new SqlWorkflowInstanceStore(testdb.ConnectionString);

                // Act / Assert
                AssertHelper.Throws<ArgumentNullException>(
                    () => StateMachineStateTracker.Attach((WorkflowApplication)null, store));
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowServiceHost
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * A StateMachineStateTracker is returned
        ///   * The InstanceStore is added to the WorkflowApplicaiton
        ///   * A StateMachineStateTracker is added to the Extensions
        ///   * A StateTrackerPersistence is created and added to the extensions
        /// </summary>
        [TestMethod]
        public void AttachNullWorkflowServiceHostShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var store = new SqlWorkflowInstanceStore(testdb.ConnectionString);

                // Act / Assert
                AssertHelper.Throws<ArgumentNullException>(
                    () => StateMachineStateTracker.Attach((WorkflowServiceHost)null, store));
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowApplication
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * A StateMachineStateTracker is returned
        ///   * The InstanceStore is added to the WorkflowApplicaiton
        ///   * A StateMachineStateTracker is added to the Extensions
        ///   * A StateTrackerPersistence is created and added to the extensions
        /// </summary>
        [TestMethod]
        public void AttachShouldAttachToWorkflowApplication()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var host = new WorkflowApplication(new Sequence());
                var store = testdb.CreateInstanceStore();

                // Act
                var tracker = StateMachineStateTracker.Attach(host, store);

                // Assert
                Assert.IsNotNull(tracker);
                Assert.AreEqual(store, host.InstanceStore);
                Assert.IsTrue(
                    host.Extensions.GetSingletonExtensions().Contains(tracker),
                    "The tracker was not added to the extensions");
                Assert.IsTrue(
                    host.Extensions.GetSingletonExtensions().Any(o => o.GetType() == typeof(StateTrackerPersistence)));
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowServiceHost
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * A StateMachineStateTracker is returned
        ///   * The InstanceStore is added to the WorkflowApplicaiton
        ///   * A StateMachineStateTracker is added to the Extensions
        ///   * A StateTrackerPersistence is created and added to the extensions
        /// </summary>
        [TestMethod]
        public void AttachShouldAttachToWorkflowServiceHost()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var host = new WorkflowServiceHost(new Sequence());
                var store = testdb.CreateInstanceStore();

                // Act
                var tracker = StateMachineStateTracker.Attach(host, store);

                // Assert
                Assert.IsNotNull(tracker);
                Assert.AreEqual(store, host.DurableInstancingOptions.InstanceStore);
                Assert.IsTrue(
                    host.WorkflowExtensions.GetSingletonExtensions().Contains(tracker),
                    "The tracker was not added to the extensions");
                Assert.IsTrue(
                    host.WorkflowExtensions.GetSingletonExtensions().Any(
                        o => o.GetType() == typeof(StateTrackerPersistence)));
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowApplication that has run
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * An exception is thrown
        /// </summary>
        [TestMethod]
        public void AttachToInitializedWorkflowApplicationShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var host = new WorkflowApplication(new TestBookmark<int> { BookmarkName = "B1" });
                var store = testdb.CreateInstanceStore();
                host.RunEpisode("B1");

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(() => StateMachineStateTracker.Attach(host, store));
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowServiceHost that has run
        ///   * A SqlWorkflowInstanceStore
        ///   When
        ///   * Attach is invoked
        ///   Then
        ///   * An exception is thrown
        /// </summary>
        [TestMethod]
        public void AttachToInitializedWorkflowServiceHostShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var store = testdb.CreateInstanceStore();
                var host = new WorkflowServiceHost(
                    new TestBookmark<int> { BookmarkName = "B1" }, ServiceTest.GetUniqueUri());
                host.DurableInstancingOptions.InstanceStore = store;
                host.Open();

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(() => StateMachineStateTracker.Attach(host, store));
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * StateMachineTracker.CurrentState is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void CurrentStateThrowWithMoreThanOneStateMachine()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
                AssertHelper.Throws<InvalidOperationException>(() => AssertHelper.GetProperty(tracker.CurrentState));
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker with no CurrentStateMachine
        ///   When
        ///   * Get CurrentState is invoked
        ///   Then
        ///   * It should return null
        /// </summary>
        [TestMethod]
        public void CurrentStateWithNoCurrentShouldReturnNull()
        {
            // Arrange
            var tracker = new StateMachineStateTracker(new Sequence());

            // Act / Assert
            Assert.IsNull(tracker.CurrentState);
        }

        /// <summary>
        ///   Given
        ///   * A StateMachineExampleActivity
        ///   * A StateMachineStateTracker set to a max history of 3
        ///   When
        ///   * The fourth state transition occurs
        ///   Then
        ///   * The first state in the history is dropped from the buffer and the fourth is added to the end
        /// </summary>
        [TestMethod]
        public void HistoryIsCircularBuffer()
        {
            // Arrange
            const int MaxHistory = 3;
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateMachineStateTracker(activity, MaxHistory);
            host.Extensions.Add(tracker);

            try
            {
                // Start the workflow - run to State1 with bookmark "T1"
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // History
                // State1
                CollectionAssert.AreEqual(tracker.StateHistory.ToList(), new[] { "State1" });

                // Run until State2 with bookmark "T3" 
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "T3");

                // History
                // State1
                // State2
                CollectionAssert.AreEqual(tracker.StateHistory.ToList(), new[] { "State1", "State2" });

                // Run until State1 with bookmark "T1"
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T3", null, "T1");

                // History
                // State1
                // State2
                // State1
                CollectionAssert.AreEqual(tracker.StateHistory.ToList(), new[] { "State1", "State2", "State1" });

                // Run until State2 with bookmark "T3" 
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "T3");

                // History
                // State1 <- Dropped from buffer
                // State2
                // State1
                // State2
                Assert.AreEqual(MaxHistory, tracker.StateHistory.Count());
                CollectionAssert.AreEqual(tracker.StateHistory.ToList(), new[] { "State2", "State1", "State2" });
            }
            finally
            {
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   A null WorkflowServiceHost should throw with a valid instance store
        /// </summary>
        [TestMethod]
        public void InstanceStoreAndNullHostShouldThrow()
        {
            // Arrange 
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var store = new SqlWorkflowInstanceStore(testdb.ConnectionString);

                // Act / Assert
                AssertHelper.Throws<ArgumentNullException>(
                    () => StateMachineStateTracker.Attach((WorkflowServiceHost)null, store));
            }
        }

        /// <summary>
        ///   LoadInstance with an empty connection string should throw
        /// </summary>
        [TestMethod]
        public void LoadInstanceEmptyConnStringShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(
                () => StateMachineStateTracker.LoadInstance(Guid.NewGuid(), new Sequence(), string.Empty));
        }

        /// <summary>
        ///   LoadInstance with a null activity should throw
        /// </summary>
        [TestMethod]
        public void LoadInstanceNullActivityShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var conn = testdb.ConnectionString;
                AssertHelper.Throws<ArgumentNullException>(
                    () => StateMachineStateTracker.LoadInstance(Guid.NewGuid(), null, conn));
            }
        }

        /// <summary>
        ///   LoadInstance with a bad extension should throw
        /// </summary>
        [TestMethod]
        public void LoadInstancesBadNameExtShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var conn = testdb.ConnectionString;
                AssertHelper.Throws<ArgumentException>(
                    () => StateMachineStateTracker.LoadInstances("BadName.bad", "foo", conn));
            }
        }

        /// <summary>
        ///   LoadInstance with a bad file name no extension should throw
        /// </summary>
        [TestMethod]
        public void LoadInstancesBadNameXamlShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var conn = testdb.ConnectionString;
                AssertHelper.Throws<ArgumentException>(
                    () => StateMachineStateTracker.LoadInstances("BadName", "foo", conn));
            }
        }

        /// <summary>
        ///   LoadInstance with an empty connection string should throw
        /// </summary>
        [TestMethod]
        public void LoadInstancesEmptyConnStringShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                StateMachineStateTracker.LoadInstances(Constants.StateMachineServiceExampleXamlx, "foo", string.Empty));
        }

        /// <summary>
        ///   LoadInstance with an empty displayName should throw
        /// </summary>
        [TestMethod]
        public void LoadInstancesEmptyDisplayNameShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var conn = testdb.ConnectionString;
                AssertHelper.Throws<ArgumentNullException>(
                    () =>
                    StateMachineStateTracker.LoadInstances(
                        Constants.StateMachineServiceExampleXamlx, string.Empty, conn));
            }
        }

        /// <summary>
        ///   LoadInstance with an empty xaml should throw
        /// </summary>
        [TestMethod]
        public void LoadInstancesEmptyXamlShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var conn = testdb.ConnectionString;
                AssertHelper.Throws<ArgumentNullException>(
                    () => StateMachineStateTracker.LoadInstances(string.Empty, "foo", conn));
            }
        }

        /// <summary>
        ///   A null activity should throw
        /// </summary>
        [TestMethod]
        public void NullActivityShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(() => new StateMachineStateTracker(null));
        }

        /// <summary>
        ///   A null WorkflowServiceHost and null instnace store should throw
        /// </summary>
        [TestMethod]
        public void NullInstanceStoreAndHostShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(
                () => StateMachineStateTracker.Attach((WorkflowServiceHost)null, null));
        }

        /// <summary>
        ///   A null WorkflowApplication should throw
        /// </summary>
        [TestMethod]
        public void NullWorkflowApplicationShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(() => StateMachineStateTracker.Attach((WorkflowApplication)null));
        }

        /// <summary>
        ///   A null WorkflowServiceHost should throw
        /// </summary>
        [TestMethod]
        public void NullWorkflowServiceHostShouldThrow()
        {
            AssertHelper.Throws<ArgumentNullException>(() => StateMachineStateTracker.Attach((WorkflowServiceHost)null));
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * StateMachineTracker.PossibleTransitions is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void PossibleTransitionsThrowWithMoreThanOneStateMachine()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
                AssertHelper.Throws<InvalidOperationException>(
                    () => AssertHelper.GetProperty(tracker.PossibleTransitions));
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker with no CurrentStateMachine
        ///   When
        ///   * Get PossibleTransitions is invoked
        ///   Then
        ///   * A null string is returned
        /// </summary>
        [TestMethod]
        public void PossibleTransitionsWithNoCurrentShouldReturnNull()
        {
            // Arrange
            var tracker = new StateMachineStateTracker(new Sequence());

            // Act / Assert
            Assert.IsNull(tracker.PossibleTransitions);
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * StateMachineTracker.PreviousState is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void PreviousStateThrowWithMoreThanOneStateMachine()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
                AssertHelper.Throws<InvalidOperationException>(() => AssertHelper.GetProperty(tracker.PreviousState));
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * StateMachineTracker.StateHistory is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void StateHistoryThrowWithMoreThanOneStateMachine()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
                AssertHelper.Throws<InvalidOperationException>(() => AssertHelper.GetProperty(tracker.StateHistory));
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A newly created StateMachineTracker
        ///   When
        ///   * The TrackedStateMachines property is read
        ///   Then
        ///   * An empty dictionary is returned
        /// </summary>
        [TestMethod]
        public void TrackedStateMachinesShouldBeEmptyOnCreate()
        {
            // Arrange
            var tracker = new StateMachineStateTracker(new StateMachine());

            // Act
            var trackedMachines = tracker.TrackedStateMachines;

            // Assert
            Assert.IsNotNull(trackedMachines);
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService configured to use the StateMachineStateTracker
        ///   When
        ///   * StateMachineTracker.Attach is invoked
        ///   Then
        ///   * The list of StateMachineStateTracker instances is returned with 2 instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineServiceExamplePath)]
        public void TrackerShouldAttachWorkflowServiceHost()
        {
            // Arrange
            var serviceEndpoint = ServiceTest.GetUniqueEndpointAddress();

            using (
                var testHost = new WorkflowServiceTestHost(Constants.StateMachineServiceExampleXamlx, serviceEndpoint))
            {
                testHost.Host.Description.Behaviors.Add(
                    new WorkflowIdleBehavior { TimeToPersist = TimeSpan.Zero, TimeToUnload = TimeSpan.Zero });

                // Act
                var tracker = StateMachineStateTracker.Attach(testHost.Host);
                testHost.Open();
                this.RunSampleStateMachineService(testHost.EndpointAddress, ExampleTrigger.T1, ExampleTrigger.T2);

                // Assert
                Assert.IsNotNull(tracker);
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension and run through a couple of transitions
        ///   When
        ///   * The state tracker is converted to XML and back again
        ///   Then
        ///   * Two state trackers are equal
        /// </summary>
        [TestMethod]
        public void TrackerShouldConvertToAndFromXml()
        {
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker1 = new StateMachineStateTracker(activity);
            Debug.Assert(host != null, "host != null");
            Debug.Assert(host.Extensions != null, "host.Extensions != null");
            host.Extensions.Add(tracker1);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "T3");

                var xml = tracker1.ToXml();

                WorkflowTrace.Information("*** Tracker1***");
                WorkflowTrace.Information(xml);
                WorkflowTrace.Information(Environment.NewLine);

                var tracker2 = StateMachineStateTracker.Parse(activity, xml);
                WorkflowTrace.Information("*** Tracker2***");
                WorkflowTrace.Information(tracker2.ToXml());
                WorkflowTrace.Information(Environment.NewLine);

                Assert.AreEqual(tracker1.CurrentState, tracker2.CurrentState);
                Assert.AreEqual(tracker1.CurrentStateMachine, tracker2.CurrentStateMachine);
                Assert.AreEqual(tracker1.InstanceId, tracker2.InstanceId);
                Assert.AreEqual(tracker1.PossibleTransitions, tracker2.PossibleTransitions);
                Assert.AreEqual(tracker1.PreviousState, tracker2.PreviousState);
                Assert.AreEqual(tracker1.PreviousStateMachine, tracker2.PreviousStateMachine);
                Assert.AreEqual(tracker1.RootActivity, tracker2.RootActivity);
                AssertHelper.AreEqual(tracker1.StateHistory, tracker2.StateHistory);
                AssertHelper.AreEqual(tracker1.Transitions, tracker2.Transitions);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineExamplePath)]
        public void TrackerShouldLoadInstancesWithActivity()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();
                var id = this.RunSampleStateMachine(testdb, ExampleTrigger.T1, ExampleTrigger.T3);

                // Act
                var instance = StateMachineStateTracker.LoadInstance(
                    id, new StateMachineExample(), testdb.ConnectionString);

                // Assert
                Assert.IsNotNull(instance);
                Assert.AreEqual(id, instance.InstanceId);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        /// * A StateMachineTracker which has previously accumulated history
        ///   When
        ///   * The StateMachine is loaded and run again
        ///   Then
        ///   * The history is accumulated
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineExamplePath)]
        public void TrackerAccumulatesHistoryAcrossLoad()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange                
                var triggers = new[] { ExampleTrigger.T1 };
                var activity = new StateMachineExample();
                var host = WorkflowApplicationTest.Create(activity);
                Guid id;
                StateMachineStateTracker.Attach(host.TestWorkflowApplication, testdb.CreateInstanceStore());

                // Act
                using (host)
                {
                    try
                    {
                        // Start the workflow and run until it becomes idle with at least one bookmark
                        host.TestWorkflowApplication.RunEpisode(AnyBookmark, Constants.Timeout);

                        // Play each of the triggers
                        foreach (var trigger in triggers)
                        {
                            // Resume the workflow and run until any bookmark
                            host.TestWorkflowApplication.ResumeEpisodeBookmark(
                                trigger.ToString(), Constants.Timeout, AnyBookmark);
                        }

                        host.Persist(Constants.Timeout);
                        host.Unload(Constants.Timeout);
                        id = host.Id;

                    }
                    finally
                    {
                        host.Tracking.Trace();
                    }
                }

                var tracker = StateMachineStateTracker.LoadInstance(id, activity, testdb.ConnectionString);
                var host2 = WorkflowApplicationTest.Create(activity);
                StateMachineStateTracker.Attach(host2.TestWorkflowApplication, testdb.InstanceStore, tracker: tracker);
                host2.Load(id, Constants.Timeout);
                using (host2)
                {
                    try
                    {
                        // Resume the workflow and run until any bookmark
                        host2.TestWorkflowApplication.ResumeEpisodeBookmark(
                            ExampleTrigger.T3.ToString(), Constants.Timeout, AnyBookmark);

                    }
                    finally
                    {
                        host.Tracking.Trace();
                    }
                }

                // Assert
                Assert.IsNotNull(tracker);
                Assert.AreEqual(3, tracker.StateHistory.Count);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineExamplePath)]
        public void TrackerShouldLoadInstancesWithXaml()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Create a couple of state machines in the persistence store
                this.RunSampleStateMachine(testdb, ExampleTrigger.T1, ExampleTrigger.T3);
                this.RunSampleStateMachine(testdb, ExampleTrigger.T1, ExampleTrigger.T3, ExampleTrigger.T2);

                var instances = StateMachineStateTracker.LoadInstances(
                    Constants.StateMachineExampleXaml, Constants.StateMachineExample, testdb.ConnectionString);

                Assert.IsNotNull(instances);
                Assert.AreEqual(2, instances.Count);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineServiceExamplePath)]
        public void TrackerShouldLoadInstancesWithXamlx()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Create a couple of state machines in the persistence store
                RunSampleStateMachineService(testdb, ExampleTrigger.T1, ExampleTrigger.T2);
                RunSampleStateMachineService(testdb, ExampleTrigger.T1, ExampleTrigger.T2, ExampleTrigger.T5);

                // Get the root activity of the workflow service
                var activity = XamlHelper.Load(Constants.StateMachineServiceExampleXamlx);

                var instances = StateMachineStateTracker.LoadInstances(activity, testdb.ConnectionString);

                Assert.IsNotNull(instances);
                Assert.AreEqual(2, instances.Count);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * LoadInstances should load the instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineExamplePath)]
        public void TrackerShouldNotLoadInstancesWithBadId()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                var instance = StateMachineStateTracker.LoadInstance(
                    Guid.NewGuid(), new StateMachineExample(), testdb.ConnectionString);

                Assert.IsNull(instance);
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * The StateMachineStateTracker correctly reports the current state
        ///   * and the possible transitions
        /// </summary>
        [TestMethod]
        public void CurrentStateReportedWhenRun()
        {
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateMachineStateTracker(activity);
            Debug.Assert(host != null, "host != null");
            Debug.Assert(host.Extensions != null, "host.Extensions != null");
            host.Extensions.Add(tracker);

            try
            {
                // Using Microsoft.Activities.Extensions run the workflow until a bookmark named "T1"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout), typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("First Idle");
                tracker.Trace();

                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
                var stateTrackerInfo = tracker.TrackedStateMachines.Values.ElementAt(0);
                Debug.Assert(stateTrackerInfo != null, "stateTrackerInfo != null");
                Assert.AreEqual("State1", stateTrackerInfo.CurrentState);
                Assert.AreEqual("State1", tracker.CurrentState);
                Assert.AreEqual(tracker.CurrentState, stateTrackerInfo.CurrentState);
                Debug.Assert(stateTrackerInfo.Transitions != null, "stateTrackerInfo.Transitions != null");
                Assert.AreEqual(2, stateTrackerInfo.Transitions.Count);
                Assert.AreEqual("T1, T2", stateTrackerInfo.PossibleTransitions);

                // Run until bookmark "T3"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "T3"),
                    typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("Second Idle");
                tracker.Trace();

                // Use the tracker operations that are forwarded to the current state
                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
                Assert.AreEqual("State2", tracker.CurrentState);
                Debug.Assert(tracker.Transitions != null, "tracker.Transitions != null");
                Assert.AreEqual(2, tracker.Transitions.Count);
                Assert.AreEqual("T3, T5", tracker.PossibleTransitions);

                // Resume bookmark T5 and run until complete
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T5", null),
                    typeof(WorkflowCompletedEpisodeResult));

                // Should be no remaining state machines tracked after completion
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
            }
            finally
            {
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * The StateMachineStateTracker correctly reports the current state
        ///   * and the possible transitions
        /// </summary>
        [TestMethod]
        public void TrackerShouldReportPreviousState()
        {
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateMachineStateTracker(activity);
            Debug.Assert(host != null, "host != null");
            Debug.Assert(host.Extensions != null, "host.Extensions != null");
            host.Extensions.Add(tracker);

            try
            {
                // Using Microsoft.Activities.Extensions run the workflow until a bookmark named "T1"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout), typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("First Idle");
                tracker.Trace();

                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
                var stateTrackerInfo = tracker.TrackedStateMachines.Values.ElementAt(0);
                Debug.Assert(stateTrackerInfo != null, "stateTrackerInfo != null");
                Assert.AreEqual(null, stateTrackerInfo.PreviousState);
                Assert.AreEqual(null, tracker.PreviousState);
                Assert.AreEqual(tracker.PreviousState, stateTrackerInfo.PreviousState);
                Debug.Assert(stateTrackerInfo.Transitions != null, "stateTrackerInfo.Transitions != null");
                Assert.AreEqual(2, stateTrackerInfo.Transitions.Count);
                Assert.AreEqual("T1, T2", stateTrackerInfo.PossibleTransitions);

                // Run until bookmark "T3"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "T3"),
                    typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("Second Idle");
                tracker.Trace();

                // Use the tracker operations that are forwarded to the current state
                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
                Assert.AreEqual("State1", tracker.PreviousState);
                Debug.Assert(tracker.Transitions != null, "tracker.Transitions != null");
                Assert.AreEqual(2, tracker.Transitions.Count);
                Assert.AreEqual("T3, T5", tracker.PossibleTransitions);

                // Resume bookmark T5 and run until complete
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T5", null),
                    typeof(WorkflowCompletedEpisodeResult));

                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
            }
            finally
            {
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * The StateMachine is run and becomes idle
        ///   Then
        ///   * The StateMachineStateTracker correctly reports the current state
        ///   * and the possible transitions
        /// </summary>
        [TestMethod]
        public void TrackerWithNestedState()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            Debug.Assert(host != null, "host != null");
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                // Using Microsoft.Activities.Extensions run the workflow until a bookmark named "T1"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout), typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("First Idle");
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();

                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(1, tracker.TrackedStateMachines.Count);
                var stateTrackerInfo = tracker.TrackedStateMachines.Values.ElementAt(0);
                Debug.Assert(stateTrackerInfo != null, "stateTrackerInfo != null");
                Assert.AreEqual("State1", stateTrackerInfo.CurrentState);
                Debug.Assert(stateTrackerInfo.Transitions != null, "stateTrackerInfo.Transitions != null");
                Assert.AreEqual(2, stateTrackerInfo.Transitions.Count);
                Assert.AreEqual("T1, T6", stateTrackerInfo.PossibleTransitions);

                // Run until bookmark "NT1" from nested state machine
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1"),
                    typeof(WorkflowIdleEpisodeResult));

                WorkflowTrace.Information("Second Idle");
                tracker.Trace();

                Debug.Assert(tracker.TrackedStateMachines != null, "tracker.TrackedStateMachines != null");
                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);

                // Verify the parent state machine is already in State2
                stateTrackerInfo = tracker.TrackedStateMachines.Values.ElementAt(0);
                Debug.Assert(stateTrackerInfo != null, "stateTrackerInfo != null");
                Assert.AreEqual("State2", stateTrackerInfo.CurrentState);
                Debug.Assert(stateTrackerInfo.Transitions != null, "stateTrackerInfo.Transitions != null");
                Assert.AreEqual(3, stateTrackerInfo.Transitions.Count);
                Assert.AreEqual("T2, T3, T7", stateTrackerInfo.PossibleTransitions);

                // The nested state machine
                var nestedStateTrackerInfo = tracker.TrackedStateMachines.Values.ElementAt(1);
                Debug.Assert(nestedStateTrackerInfo != null, "nestedStateTrackerInfo != null");
                Assert.AreEqual("Nested State1", nestedStateTrackerInfo.CurrentState);
                Debug.Assert(nestedStateTrackerInfo.Transitions != null, "nestedStateTrackerInfo.Transitions != null");
                Assert.AreEqual(1, nestedStateTrackerInfo.Transitions.Count);
                Assert.AreEqual("NT1", nestedStateTrackerInfo.PossibleTransitions);

                // Resume bookmark "NT1" from nested state machine and run until bookmark "T7"
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("NT1", null, "T7"),
                    typeof(WorkflowIdleEpisodeResult));

                // Resume bookmark T7 and run until complete
                Assert.IsInstanceOfType(
                    host.TestWorkflowApplication.ResumeEpisodeBookmark("T7", null),
                    typeof(WorkflowCompletedEpisodeResult));

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine
        ///   * A WorkflowApplication with the StateMachineStateTracker added as an extension
        ///   When
        ///   * StateMachineTracker.Transitions is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void TransitionsThrowWithMoreThanOneStateMachine()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = StateMachineStateTracker.Attach(host.TestWorkflowApplication);

            try
            {
                host.TestWorkflowApplication.RunEpisode("T1", Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeEpisodeBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.TrackedStateMachines.Count);
                AssertHelper.Throws<InvalidOperationException>(() => AssertHelper.GetProperty(tracker.Transitions));
            }
            finally
            {
                Debug.Assert(tracker != null, "tracker != null");
                tracker.Trace();
                Debug.Assert(host.Tracking != null, "host.Tracking != null");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker with no CurrentStateMachine
        ///   When
        ///   * Get Transitions is invoked
        ///   Then
        ///   * A null is returned
        /// </summary>
        [TestMethod]
        public void TransitionsWithNoCurrentShouldReturnNull()
        {
            // Arrange
            var tracker = new StateMachineStateTracker(new Sequence());

            // Act / Assert
            Assert.IsNull(tracker.Transitions);
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService configured to use the StateMachineStateTracker
        ///   * An instance store with 2 persisted instances with state tracker information
        ///   When
        ///   * LoadInstances is called using the xamlx file
        ///   Then
        ///   * The list of StateMachineStateTracker instances is returned with 2 instances
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.StateMachineServiceExamplePath)]
        public void XamlXFileShouldLoadInstances()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            using (
                var host = WorkflowServiceTestHost.Open(
                    Constants.StateMachineServiceExampleXamlx,
                    ServiceTest.GetUniqueUri(),
                    testdb.CreateInstanceStore(),
                    new StateMachineTrackingBehavior(),
                    new WorkflowIdleBehavior { TimeToPersist = TimeSpan.Zero, TimeToUnload = TimeSpan.Zero }))
            {
                // Arrange
                this.RunSampleStateMachineService(host.EndpointAddress, ExampleTrigger.T1, ExampleTrigger.T2);
                this.RunSampleStateMachineService(
                    host.EndpointAddress, ExampleTrigger.T1, ExampleTrigger.T2, ExampleTrigger.T3);

                // Act
                var instances = StateMachineStateTracker.LoadInstances(
                    Constants.StateMachineServiceExampleXamlx, Constants.StateMachine, testdb.ConnectionString);

                // Assert
                Assert.IsNotNull(instances);
                Assert.AreEqual(2, instances.Count);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Determines if there are any bookmarks
        /// </summary>
        /// <param name="args"> The args. </param>
        /// <param name="targetBookmark"> The s. </param>
        /// <returns> The any bookmark. </returns>
        private static bool AnyBookmark(WorkflowApplicationIdleEventArgs args, string targetBookmark)
        {
            return args != null && args.Bookmarks.Any();
        }

        private static void LoadAndRunSampleStateMachine(
            Guid id, SqlWorkflowInstanceStoreTest testdb, params ExampleTrigger[] triggers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService configured to use the StateMachineStateTracker
        ///   * An instance store with 2 persisted instances with state tracker information
        ///   When
        ///   * LoadInstances is called using the xamlx file
        ///   Then
        ///   * The list of StateMachineStateTracker instances is returned with 2 instances
        /// </summary>
        /// <summary>
        ///   Runs the sample service through the list of triggers
        /// </summary>
        /// <param name="testdb"> The test database </param>
        /// <param name="exampleTriggers"> The triggers. </param>
        private static void RunSampleStateMachineService(
            SqlWorkflowInstanceStoreTest testdb, params ExampleTrigger[] exampleTriggers)
        {
            Debug.Assert(testdb != null, "TestInstanceStore != null");
            var instanceStore = new SqlWorkflowInstanceStore(testdb.ConnectionString);

            WorkflowServiceTestHost host = null;
            var serviceEndpoint = ServiceTest.GetUniqueEndpointAddress();

            var proxy = new ServiceClient(ServiceTest.Pipe, serviceEndpoint);
            var instanceId = Guid.Empty;
            try
            {
                using (
                    host =
                    WorkflowServiceTestHost.Open(
                        Constants.StateMachineServiceExampleXamlx,
                        serviceEndpoint,
                        instanceStore,
                        new StateMachineTrackingBehavior(),
                        new WorkflowIdleBehavior { TimeToPersist = TimeSpan.Zero, TimeToUnload = TimeSpan.Zero }))
                {
                    Debug.Assert(exampleTriggers != null, "exampleTriggers != null");
                    foreach (var trigger in exampleTriggers)
                    {
                        switch (trigger)
                        {
                            case ExampleTrigger.T1:
                                instanceId = proxy.T1().GetValueOrDefault();
                                break;

                            case ExampleTrigger.T2:
                                proxy.T2(instanceId);
                                break;

                            case ExampleTrigger.T3:
                                proxy.T3(instanceId);
                                break;

                            case ExampleTrigger.T4:
                                proxy.T4(instanceId);
                                break;
                            case ExampleTrigger.T5:
                                proxy.T5(instanceId);
                                break;
                            case ExampleTrigger.T6:
                                proxy.T6(instanceId);
                                break;
                            case ExampleTrigger.T7:
                                proxy.T7(instanceId);
                                break;
                        }
                    }

                    proxy.Close();
                }
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
            finally
            {
                if (host != null)
                {
                    Debug.Assert(host.Tracking != null, "host.Tracking != null");
                    host.Tracking.Trace();
                }
            }
        }

        /// <summary>
        ///   Runs the sample state machine through a number of triggers and leaves a persisted instance
        /// </summary>
        /// <param name="testdb"> The test database </param>
        /// <param name="triggers"> The triggers </param>
        /// <returns> The workflow instance ID </returns>
        private Guid RunSampleStateMachine(SqlWorkflowInstanceStoreTest testdb, params ExampleTrigger[] triggers)
        {
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            Debug.Assert(host != null, "host != null");
            Debug.Assert(testdb != null, "TestInstanceStore != null");
            StateMachineStateTracker.Attach(
                host.TestWorkflowApplication, new SqlWorkflowInstanceStore(testdb.ConnectionString));
            using (host)
            {
                try
                {
                    // Start the workflow and run until it becomes idle with at least one bookmark
                    Assert.IsInstanceOfType(
                        host.TestWorkflowApplication.RunEpisode(AnyBookmark, Constants.Timeout),
                        typeof(WorkflowIdleEpisodeResult));

                    // Play each of the triggers
                    Debug.Assert(triggers != null, "triggers != null");
                    foreach (var trigger in triggers)
                    {
                        // Resume the workflow and run until any bookmark
                        host.TestWorkflowApplication.ResumeEpisodeBookmark(
                            trigger.ToString(), Constants.Timeout, AnyBookmark);
                    }

                    host.Persist(Constants.Timeout);
                    host.Unload(Constants.Timeout);
                }
                finally
                {
                    Debug.Assert(host.Tracking != null, "host.Tracking != null");
                    host.Tracking.Trace();
                }
            }

            return host.Id;
        }

        /// <summary>
        ///   Runs the sample service through the list of triggers
        /// </summary>
        /// <param name="address"> The host address </param>
        /// <param name="exampleTriggers"> The triggers. </param>
        private void RunSampleStateMachineService(EndpointAddress address, params ExampleTrigger[] exampleTriggers)
        {
            var proxy = new ServiceClient(ServiceTest.Pipe, address);
            var instanceId = Guid.Empty;
            try
            {
                Debug.Assert(exampleTriggers != null, "exampleTriggers != null");
                foreach (var trigger in exampleTriggers)
                {
                    switch (trigger)
                    {
                        case ExampleTrigger.T1:
                            instanceId = proxy.T1().GetValueOrDefault();
                            break;

                        case ExampleTrigger.T2:
                            proxy.T2(instanceId);
                            break;

                        case ExampleTrigger.T3:
                            proxy.T3(instanceId);
                            break;

                        case ExampleTrigger.T4:
                            proxy.T4(instanceId);
                            break;
                        case ExampleTrigger.T5:
                            proxy.T5(instanceId);
                            break;
                        case ExampleTrigger.T6:
                            proxy.T6(instanceId);
                            break;
                        case ExampleTrigger.T7:
                            proxy.T7(instanceId);
                            break;
                    }
                }

                proxy.Close();
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }

        #endregion
    }
}