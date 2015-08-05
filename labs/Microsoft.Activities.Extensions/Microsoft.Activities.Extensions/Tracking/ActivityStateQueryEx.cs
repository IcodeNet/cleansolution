// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateQueryEx.cs" company="Microsoft">
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
    public static class ActivityStateQueryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="query">
        /// The query. 
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs.
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this ActivityStateQuery query, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendLine(query.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("ActivityName: {0}", query.ActivityName);
                tsb.AppendCollection("Arguments", query.Arguments);
                tsb.AppendDictionary("QueryAnnotations", query.QueryAnnotations);
                tsb.AppendCollection("States", query.States);
                tsb.AppendCollection("Variables", query.Variables);
            }

            return tsb.ToString();
        }

        #endregion
    }
}