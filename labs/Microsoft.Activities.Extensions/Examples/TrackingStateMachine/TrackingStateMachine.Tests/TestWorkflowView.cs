namespace TrackingStateMachine.Tests
{
    using System;
    using System.Activities.DurableInstancing;
    using System.Threading.Tasks;

    /// <summary>
    ///   The test workflow view.
    /// </summary>
    public class TestWorkflowView : IWorkflowView
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the TestWorkflowView
        /// </summary>
        /// <param name="instanceStore">
        /// The instance Store.
        /// </param>
        public TestWorkflowView(SqlWorkflowInstanceStore instanceStore)
        {
            this.Model = new WorkflowModel(this) { InstanceStore = instanceStore };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The model
        /// </summary>
        public WorkflowModel Model { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Indicates that the view should refresh
        /// </summary>
        public void Refresh()
        {
            // Do nothing
        }

        /// <summary>
        ///  Remove a workflow from the collection
        /// </summary>
        /// <param name="i">The index</param>
        public void RemoveWorkflow(int i)
        {
            Task.Factory.StartNew(() => this.Model.Workflows.RemoveAt(i));
        }

        /// <summary>
        ///   The write exception.
        /// </summary>
        /// <param name="exception"> The exception. </param>
        /// <param name="error"> The error. </param>
        public void WriteException(Exception exception, string error)
        {
            // Do nothing
        }

        #endregion
    }
}