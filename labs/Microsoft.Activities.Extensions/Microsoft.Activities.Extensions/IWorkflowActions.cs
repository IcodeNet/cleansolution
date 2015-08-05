// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowApplicationObserver.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;

    /// <summary>
    /// The WorkflowApplicationObserver interface.
    /// </summary>
    public interface IWorkflowActions
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the aborted handler.
        /// </summary>
        Action<WorkflowApplicationAbortedEventArgs> Aborted { get; set; }

        /// <summary>
        ///   Gets or sets the completed handler.
        /// </summary>
        Action<WorkflowApplicationCompletedEventArgs> Completed { get; set; }

        /// <summary>
        ///   Gets or sets the idle handler.
        /// </summary>
        Action<WorkflowApplicationIdleEventArgs> Idle { get; set; }

        #endregion
    }
}