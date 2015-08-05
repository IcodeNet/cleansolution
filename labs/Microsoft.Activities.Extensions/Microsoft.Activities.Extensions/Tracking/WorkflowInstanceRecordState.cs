// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceRecordState.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    /// <summary>
    /// The workflow instance states.
    /// </summary>
    public enum WorkflowInstanceRecordState
    {
        /// <summary>
        ///   The aborted.
        /// </summary>
        Aborted, 

        /// <summary>
        ///   The canceled.
        /// </summary>
        Canceled, 

        /// <summary>
        ///   The completed.
        /// </summary>
        Completed, 

        /// <summary>
        ///   The deleted.
        /// </summary>
        Deleted, 

        /// <summary>
        ///   The idle.
        /// </summary>
        Idle, 

        /// <summary>
        ///   The persisted.
        /// </summary>
        Persisted, 

        /// <summary>
        ///   The resumed.
        /// </summary>
        Resumed, 

        /// <summary>
        ///   The started.
        /// </summary>
        Started, 

        /// <summary>
        ///   The suspended.
        /// </summary>
        Suspended, 

        /// <summary>
        ///   The terminated.
        /// </summary>
        Terminated, 

        /// <summary>
        ///   The unhandled exception.
        /// </summary>
        UnhandledException, 

        /// <summary>
        ///   The unloaded.
        /// </summary>
        Unloaded, 

        /// <summary>
        ///   The unsuspended.
        /// </summary>
        Unsuspended, 
    }
}