// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowView.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Windows
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
        /// The remove workflow.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        void RemoveWorkflow(int i);

        /// <summary>
        /// The write exception.
        /// </summary>
        /// <param name="exception">
        /// The exception. 
        /// </param>
        /// <param name="error">
        /// The error. 
        /// </param>
        void WriteException(Exception exception, string error);

        #endregion
    }
}