// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookmarkResumptionQueryEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   BookmarkResumptionQuery Extensions
    /// </summary>
    public static class BookmarkResumptionQueryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="query">
        /// The query. 
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tab indent level.
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this BookmarkResumptionQuery query, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendLine(query.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("Name: {0}", query.Name);
                tsb.AppendDictionary("QueryAnnotations", query.QueryAnnotations);
            }

            return tsb.ToString();
        }

        #endregion
    }
}