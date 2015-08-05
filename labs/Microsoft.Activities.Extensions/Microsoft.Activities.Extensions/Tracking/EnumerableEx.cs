// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enumerable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///   Static dictionary helpers
    /// </summary>
    internal static class EnumerableEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns true if the enumerable is null or empty.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enumerable 
        /// </typeparam>
        /// <param name="enumerable">
        /// The enumerable. 
        /// </param>
        /// <returns>
        /// true if the enumerable is null or empty 
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Converts a list of objects to a comma delimited string
        /// </summary>
        /// <param name="enumerable">
        /// The records. 
        /// </param>
        /// <typeparam name="T">
        /// The type 
        /// </typeparam>
        /// <returns>
        /// The delimited list. 
        /// </returns>
        public static string ToDelimitedList<T>(this IEnumerable<T> enumerable)
        {
            var result = new StringBuilder();

            foreach (var o in enumerable)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }

                result.Append(o);
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts a list of objects to a comma delimited string
        /// </summary>
        /// <param name="enumerable">
        /// The records. 
        /// </param>
        /// <typeparam name="T">
        /// The type 
        /// </typeparam>
        /// <returns>
        /// The delimited list. 
        /// </returns>
        public static string ToQuotedList<T>(this IEnumerable<T> enumerable)
        {
            var result = new StringBuilder();

            foreach (var o in enumerable)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }

                result.AppendFormat("\"{0}\"", o);
            }

            return result.ToString();
        }

        #endregion
    }
}