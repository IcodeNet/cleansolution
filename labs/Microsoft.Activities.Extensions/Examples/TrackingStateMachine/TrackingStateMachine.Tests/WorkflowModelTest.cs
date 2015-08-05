namespace TrackingStateMachine.Tests
{
    using System.Threading;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TrackingStateMachine.Activities;


    /// <summary>
    ///   Tests the WorkflowModel
    /// </summary>
    [TestClass]
    public class WorkflowModelTest
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

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * CurrentState property is accessed
        ///   Then
        ///   * The CurrentState will be null
        /// </summary>
        [TestMethod]
        public void CurrentStateIsNullWhenNew()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                var state = model.CurrentState;

                // Assert
                Assert.IsNull(state);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * The Workflow is run
        ///   Then
        ///   * The CurrentState will report the current state
        /// </summary>
        [TestMethod]
        public void CurrentStateIsReported()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                model.New();

                // Assert
                Assert.AreEqual(StateMachineExample.State1, model.CurrentState);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * CurrentStateMachineName property is accessed
        ///   Then
        ///   * The CurrentStateMachineName will be null
        /// </summary>
        [TestMethod]
        public void CurrentStateMachineNameIsNullWhenNew()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                var stateMachineName = model.CurrentStateMachineName;

                // Assert
                Assert.AreEqual(WorkflowModel.Unknown, stateMachineName);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * The Workflow is run
        ///   Then
        ///   * The CurrentStateMachineName will report the current state
        /// </summary>
        [TestMethod]
        public void CurrentStateMachineNameIsReported()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                model.New();

                // Assert
                Assert.AreEqual(StateMachineExample.Name, model.CurrentStateMachineName);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * The WorkflowModel is constructed
        ///   Then
        ///   * The IsLoaded property will be false
        /// </summary>
        [TestMethod]
        public void IsLoadedFalseAfterCreate()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                var actual = model.IsLoaded;

                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * The Workflow is run
        ///   Then
        ///   * The IsLoaded property will be true
        /// </summary>
        [TestMethod]
        public void IsLoadedTrueWhenRun()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                model.New();

                // Act
                var actual = model.IsLoaded;

                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A new WorkflowModel
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * Load will return false because there is no current instance
        /// </summary>
        [TestMethod]
        public void LoadReturnsFalseWhenNoCurrentInstance()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                var actual = model.Load();

                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel with a new workflow that has been unloaded
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
                var model = view.Model;

                // Create and run the workflow
                model.New();
                model.Unload();

                // Act
                model.Load();
                var isLoaded = model.IsLoaded;

                // Assert
                Assert.IsTrue(isLoaded);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel
        ///   When
        ///   * The Workflow is run
        ///   Then
        ///   * The StateHistory is not null or empty
        /// </summary>
        [TestMethod]
        public void ModelReturnsStateHistory()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;

                // Act
                model.New();

                // Assert
                Assert.AreEqual(1, model.StateHistory.Count);
            }
        }

        /// <summary>
        ///   Given
        ///   * A new WorkflowModel
        ///   When
        ///   * Load is invoked
        ///   Then
        ///   * Load will return false because there is no current instance
        /// </summary>
        [TestMethod]
        public void OnCompleteRemovesCurrent()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;
                var completedEvent = new AutoResetEvent(false);
                model.Workflows.CollectionChanged += (sender, args) => completedEvent.Set();
                model.New();

                // Act
                model.ResumeBookmark(StateTrigger.T1);
                var index1 = model.SelectedIndex;
                var count1 = model.Workflows.Count;

                // Complete the workflow
                model.ResumeBookmark(StateTrigger.T5);
                completedEvent.WaitOne(Globals.Timeout);

                var index2 = model.SelectedIndex;
                var count2 = model.Workflows.Count;

                // Assert
                Assert.AreEqual(1, count1);
                Assert.AreEqual(0, index1);
                Assert.AreEqual(0, count2);
                Assert.AreEqual(-1, index2);
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowModel that is run
        ///   When
        ///   * The TransitionsProperty is accessed
        ///   Then
        ///   * A collection of 2 transitions will be returned
        /// </summary>
        [TestMethod]
        public void TransitionsReturnCollectionWhenRun()
        {
            // Arrange
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                var view = new TestWorkflowView(testdb.CreateInstanceStore());
                var model = view.Model;
                model.New();

                // Act
                var actual = model.Transitions;

                // Assert
                Assert.AreEqual(2, actual.Count);
                AssertHelper.OccursInOrder(actual, StateTrigger.T1.ToString(), StateTrigger.T2.ToString());
            }
        }

        #endregion
    }
}