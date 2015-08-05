// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookmarkInfoEnumerable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Linq
{
    using System;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The bookmark info enumerable.
    /// </summary>
    public static class BookmarkInfoEnumerable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the name 
        /// </typeparam>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name 
        /// </param>
        /// <returns>
        /// true if any elements in the source sequence pass the test in the specified predicate; otherwise, false 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The source is null
        /// </exception>
        public static bool Any<T>(this IEnumerable<BookmarkInfo> source, T bookmarkName)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // ReSharper disable CompareNonConstrainedGenericWithNull
            // If bookmarkName is a value type, comparisons with null always return false which is ok in this situation
            if (bookmarkName == null)
            {
                // ReSharper restore CompareNonConstrainedGenericWithNull
                throw new ArgumentNullException("bookmarkName");
            }

            return source.Any(info => info.BookmarkName == bookmarkName.ToString());
        }

        #endregion
    }
}