// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityScheduledQueryEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   ActivityScheduledQuery Extensions
    /// </summary>
    public static class ActivityScheduledQueryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="query">
        /// The query. 
        /// </param>
        /// <param name="tabs"> the tabsThe tabs count</param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this ActivityScheduledQuery query, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendLine(query.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("ActivityName: {0}", query.ActivityName);
                tsb.AppendLine("ChildActivityName: {0}", query.ChildActivityName);
                tsb.AppendDictionary("QueryAnnotations", query.QueryAnnotations);
            }

            return tsb.ToString();
        }

        #endregion
    }
}