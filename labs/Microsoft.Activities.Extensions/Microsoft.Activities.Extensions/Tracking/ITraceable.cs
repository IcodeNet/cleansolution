// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormattable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    /// <summary>
    /// The Formattable interface.
    /// </summary>
    public interface ITraceable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="tabs"> the tabs
        /// The tabs.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        string ToFormattedString(int tabs = 0);

        #endregion
    }
}