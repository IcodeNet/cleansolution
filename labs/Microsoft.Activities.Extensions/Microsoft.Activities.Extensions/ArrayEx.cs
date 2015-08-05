// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    /// <summary>
    /// The array ex.
    /// </summary>
    internal static class ArrayEx
    {
        #region Methods

        /// <summary>
        /// The is null or empty.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        internal static bool IsNullOrEmpty(this object[] array)
        {
            return array == null || array.Length == 0;
        }

        #endregion
    }
}