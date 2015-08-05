// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityInstanceStates.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    /// <summary>
    /// The activity instance states.
    /// </summary>
    public static class ActivityInstanceStates
    {
        #region Constants

        /// <summary>
        ///   Specifies that the activity is in a canceled state.
        /// </summary>
        public const string Canceled = "Canceled";

        /// <summary>
        ///   Specifies that the activity is in a closed state.
        /// </summary>
        public const string Closed = "Closed";

        /// <summary>
        ///   Specifies that the activity is in an executing state.
        /// </summary>
        public const string Executing = "Executing";

        /// <summary>
        ///   Specifies that the activity is in a faulted state.
        /// </summary>
        public const string Faulted = "Faulted";

        #endregion
    }
}