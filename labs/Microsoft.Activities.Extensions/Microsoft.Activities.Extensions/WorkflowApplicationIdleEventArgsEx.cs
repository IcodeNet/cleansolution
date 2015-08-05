// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationIdleEventArgsEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System.Activities;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The workflow application idle event args ex.
    /// </summary>
    public static class WorkflowApplicationIdleEventArgsEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// Extension method which determines if the bookmarkName collection contains a bookmarkName with the bookmarkName name
        /// </summary>
        /// <param name="args">
        /// The idle event args. 
        /// </param>
        /// <param name="bookmarkName">
        /// One or more bookmarkName names to search for 
        /// </param>
        /// <returns>
        /// true if there is a matching bookmarkName 
        /// </returns>
        public static bool ContainsBookmark(this WorkflowApplicationIdleEventArgs args, string bookmarkName)
        {
            return !string.IsNullOrWhiteSpace(bookmarkName)
                   && args.Bookmarks.Any(info => info.BookmarkName == bookmarkName);
        }

        /// <summary>
        /// Extension method which determines if the bookmarkName collection contains a bookmarkName with the bookmarkName name
        /// </summary>
        /// <typeparam name="T">
        /// The type of bookmark name 
        /// </typeparam>
        /// <param name="args">
        /// The idle event args. 
        /// </param>
        /// <param name="bookmarkName">
        /// One or more bookmarkName names to search for 
        /// </param>
        /// <returns>
        /// true if there is a matching bookmarkName 
        /// </returns>
        public static bool ContainsBookmark<T>(this WorkflowApplicationIdleEventArgs args, T bookmarkName)
        {
            return args.Bookmarks.Any(info => info.BookmarkName == bookmarkName.ToString());
        }

        /// <summary>
        /// The get bookmarkName names.
        /// </summary>
        /// <param name="args">
        /// The event args. 
        /// </param>
        /// <returns>
        /// The list of bookmarks 
        /// </returns>
        public static IEnumerable<string> GetBookmarkNames(this WorkflowApplicationIdleEventArgs args)
        {
            return args.Bookmarks.Select(bi => bi.BookmarkName).ToList();
        }

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string ToFormattedString(this WorkflowApplicationIdleEventArgs args)
        {
            var tsb = new TraceStringBuilder() { Options = WorkflowTraceOptions.ShowCollectionCount };
            tsb.AppendQuotedList("Bookmarks", args.Bookmarks.Select(info => info.BookmarkName));
            return tsb.ToString();
        }

        #endregion
    }
}