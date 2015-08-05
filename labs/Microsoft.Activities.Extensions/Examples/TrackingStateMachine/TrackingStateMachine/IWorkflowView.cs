namespace TrackingStateMachine
{
    using System;

    /// <summary>
    ///   The workflow view interface.
    /// </summary>
    public interface IWorkflowView
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Indicates that the view should refresh
        /// </summary>
        void Refresh();

        /// <summary>
        ///   The write exception.
        /// </summary>
        /// <param name="exception"> The exception. </param>
        /// <param name="error"> The error. </param>
        void WriteException(Exception exception, string error);

        #endregion

        void RemoveWorkflow(int i);
    }
}