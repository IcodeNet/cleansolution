// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dictionary.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Collections.Generic;

    /// <summary>
    /// Static dictionary helpers
    /// </summary>
    internal static class DictionaryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns true if the dictionary is null or empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <typeparam name="TKey">
        /// The key type
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The value type
        /// </typeparam>
        /// <returns>
        /// true if the dictionary is null or empty.
        /// </returns>
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        /// <summary>
        /// Returns true if the dictionary is null or empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <typeparam name="TKey">
        /// The key type
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The value type
        /// </typeparam>
        /// <returns>
        /// true if the dictionary is null or empty.
        /// </returns>
        public static bool IsNullOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        #endregion
    }
}