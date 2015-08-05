// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowEpisodeResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The episode state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;

    /// <summary>
    /// Base class for workflow episodes
    /// </summary>
    public abstract class WorkflowEpisodeResult
    {
        #region Properties

        /// <summary>
        ///   Gets or sets InstanceId.
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        ///   Gets State
        /// </summary>
        public ActivityInstanceState State { get; internal set; }

        #endregion
    }
}