// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultPropagationQueryEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   FaultPropagationQuery Extensions
    /// </summary>
    public static class FaultPropagationQueryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="query">
        /// The query. 
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs indent level.
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this FaultPropagationQuery query, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendLine(query.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("FaultHandlerActivityName: {0}", query.FaultHandlerActivityName);
                tsb.AppendLine("FaultSourceActivityName: {0}", query.FaultSourceActivityName);
                tsb.AppendDictionary("QueryAnnotations", query.QueryAnnotations);
            }

            return tsb.ToString();
        }

        #endregion
    }
}