namespace TrackingStateMachine.Tests
{
    using System;
    using System.Activities.Hosting;

    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TrackingStateMachine.Activities;

    /// <summary>
    ///   Tests the WorkflowInstance class
    /// </summary>
    [TestClass]
    public class WorkflowInstanceTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Initialize the class
        /// </summary>
        /// <param name="context"> The context. </param>
        [ClassInitialize]
        public static void TestInitialize(TestContext context)
        {
            SqlDatabaseTest.TryDropDatabasesWithPrefix();
        }

        /// <summary>
        ///   Given
        ///   * An IWorkflowView
        ///   When
        ///   * A WorkflowInstance is constructed with the view
        ///   Then
        ///   * A new StateTracker will be created
        /// </summary>
        [TestMethod]
        public void CtorShouldCreateTracker()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);

                // Act
                var wi = new WorkflowInstance(model);

                // Assert
                Assert.IsNotNull(wi.StateTracker);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * WorkflowInstance.New is invoked
        ///   Then
        ///   * The Host property is not null
        /// </summary>
        [TestMethod]
        public void HostIsNotNull()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);

                // Act
                wi.New();

                // Assert
                Assert.IsNotNull(wi.Host);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * WorkflowInstance.New is invoked
        ///   Then
        ///   * The Id property is a Guid that is the same as the Host.Id
        /// </summary>
        [TestMethod]
        public void IdIsHostId()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);

                // Act
                wi.New();
                var hostid = wi.Host.Id;
                var id = wi.Id;

                // Assert
                Assert.AreEqual(hostid, id);
            }
        }

        /// <summary>
        ///   Given
        ///   * A Persisted, Tracked Instance
        ///   When
        ///   * WorkflowInstance.Load is invoked
        ///   Then
        ///   * The Id property is a Guid that is the same as the StateTracker.InstanceId
        /// </summary>
        [TestMethod]
        public void IdIsLoadedTrackerId()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);

                // Create and run the workflow
                var id = model.New();

                // Act

                // Load the instances from the datbase
                model.LoadInstances(view);

                // Get the id from the state tracker
                var hostid = model.CurrentInstance.Id;

                // Assert
                Assert.AreEqual(hostid, id);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * A WorkflowInstance is constructed
        ///   Then
        ///   * The Id property is Guid.Empty
        /// </summary>
        [TestMethod]
        public void IdIsNotEmptyGuid()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);

                // Act
                var id = wi.Id;

                // Assert
                Assert.AreEqual(Guid.Empty, id);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * WorkflowInstance.New is invoked
        ///   Then
        ///   * The PropertyChanged event is fired for property IsLoaded
        /// </summary>
        [TestMethod]
        public void IsLoadedFiresPropertyChanged()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);
                var propChanged = false;

                wi.PropertyChanged += (sender, args) =>
                    {
                        if (!propChanged)
                        {
                            propChanged = args.PropertyName == "IsLoaded";
                        }
                    };

                // Act
                wi.New();

                // Assert
                Assert.IsTrue(propChanged);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * WorkflowInstance.New is invoked
        ///   Then
        ///   * The IsLoaded returns true
        /// </summary>
        [TestMethod]
        public void IsLoadedReturnsTrueOnNew()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);
                wi.New();

                // Act
                var actual = wi.IsLoaded;

                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * a WorkflowInstance with an InstanceId associated with it
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * The WorkflowApplication is loaded from the instance store
        ///   * IsLoaded returns true
        /// </summary>
        [TestMethod]
        public void LoadShouldLoadWorkflowApp()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);

                // Create and run the workflow
                model.New();
                model.Unload();

                // Act
                // Load the instances from the datbase
                var workflowInstance = model.CurrentInstance;
                workflowInstance.Load();
                var isLoaded = workflowInstance.IsLoaded;

                // Assert
                Assert.IsTrue(isLoaded);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowInstance
        ///   When
        ///   * WorkflowInstance.New is invoked
        ///   Then
        ///   * The IsLoaded returns true
        /// </summary>
        [TestMethod]
        public void ResumeShouldResume()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);
                var wi = new WorkflowInstance(model);
                wi.New();

                // Act
                wi.Resume(StateTrigger.T1);

                // Assert
                Assert.AreEqual(StateMachineExample.State2, wi.StateTracker.CurrentState);
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * The AppDomain starts
        ///   Then
        ///   * StateMachineExampleDefintion is constructed
        /// </summary>
        [TestMethod]
        public void StateMachineExampleDefintionIsNotNull()
        {
            // Arrange

            // Act
            var actual = WorkflowInstance.StateMachineExampleDefintion;

            // Assert
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///   Given
        ///   * a WorkflowInstance with an InstanceId associated with it
        ///   When
        ///   * Unload is invoked
        ///   Then
        ///   * The WorkflowApplication is loaded from the instance store
        ///   * IsLoaded returns false
        /// </summary>
        [TestMethod]
        public void UnloadShouldUnloadWorkflowApp()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = new WorkflowModel(view);

                // Create and run the workflow
                model.New();

                // Act
                // Load the instances from the datbase
                var workflowInstance = model.CurrentInstance;
                workflowInstance.Unload();
                var isLoaded = workflowInstance.IsLoaded;

                // Assert
                Assert.IsFalse(isLoaded);
            }
        }

        #endregion
    }
}