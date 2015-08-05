// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EpisodeEndedWith.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System.Threading;

    /// <summary>
    /// Indicates how an episode of work ended
    /// </summary>
    public enum EpisodeEndedWith
    {
        /// <summary>
        /// Unknown result
        /// </summary>
        Unknown,

        /// <summary>
        /// Episode timeout
        /// </summary>
        Timeout = WaitHandle.WaitTimeout,

        /// <summary>
        /// Episode ended when the activity aborted
        /// </summary>
        Aborted = 0, 

        /// <summary>
        /// Episode ended when the activity completed
        /// </summary>
        Completed = 1, 

        /// <summary>
        /// Episode ended with an Idle
        /// </summary>
        Idle = 2, 
    }
}