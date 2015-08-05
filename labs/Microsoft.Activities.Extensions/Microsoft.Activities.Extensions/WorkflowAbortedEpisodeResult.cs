// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowAbortedEpisodeResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;

    /// <summary>
    /// The workflow episode result.
    /// </summary>
    public class WorkflowAbortedEpisodeResult : WorkflowEpisodeResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAbortedEpisodeResult"/> class.
        /// </summary>
        /// <param name="args">
        /// The event args.
        /// </param>
        /// <param name="instanceId">
        /// The instance Id.
        /// </param>
        public WorkflowAbortedEpisodeResult(WorkflowApplicationAbortedEventArgs args, Guid instanceId)
        {
            this.State = ActivityInstanceState.Faulted;
            this.Reason = args.Reason;
            this.InstanceId = instanceId;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Reason.
        /// </summary>
        public Exception Reason { get; internal set; }

        #endregion
    }
}