// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowIdleEpisodeResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;

    /// <summary>
    /// Result when a workflow episode ends with idle
    /// </summary>
    public class WorkflowIdleEpisodeResult : WorkflowEpisodeResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowIdleEpisodeResult"/> class.
        /// </summary>
        /// <param name="args">
        /// The event args.
        /// </param>
        /// <param name="instanceId">
        /// The instance Id.
        /// </param>
        public WorkflowIdleEpisodeResult(WorkflowApplicationIdleEventArgs args, Guid instanceId)
        {
            this.IdleArgs = args;
            this.InstanceId = instanceId;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets IdleArgs.
        /// </summary>
        public WorkflowApplicationIdleEventArgs IdleArgs { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether Unloaded.
        /// </summary>
        public bool Unloaded { get; set; }

        #endregion
    }
}