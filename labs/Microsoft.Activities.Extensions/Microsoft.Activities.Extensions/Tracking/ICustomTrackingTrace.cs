// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomTrackingTrace.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    /// <summary>
    ///   Implement this interface to output a custom tracking trace
    /// </summary>
    public interface ICustomTrackingTrace
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns a formatted string
        /// </summary>
        /// <param name="option">
        /// The tracking options 
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs.
        /// </param>
        /// <returns>
        /// The formatted string 
        /// </returns>
        string ToFormattedString(TrackingOption option = TrackingOption.Default, int tabs = 0);

        #endregion
    }
}