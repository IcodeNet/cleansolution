// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingQueryEx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    using DiagTrace = System.Diagnostics.Trace;

    /// <summary>
    ///   The tracking query ex.
    /// </summary>
    public static class TrackingQueryEx
    {
        #region Public Methods and Operators

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="trackingQuery">
        /// The tracking query. 
        /// </param>
        /// <param name="tabs"> the tabs
        /// The number of tabs 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public static string ToFormattedString(this TrackingQuery trackingQuery, int tabs = 0)
        {
            var activityScheduledQuery = trackingQuery as ActivityScheduledQuery;
            if (activityScheduledQuery != null)
            {
                return activityScheduledQuery.ToFormattedString(tabs);
            }

            var activityStateQuery = trackingQuery as ActivityStateQuery;
            if (activityStateQuery != null)
            {
                return activityStateQuery.ToFormattedString(tabs);
            }

            var bookmarkResumptionQuery = trackingQuery as BookmarkResumptionQuery;
            if (bookmarkResumptionQuery != null)
            {
                return bookmarkResumptionQuery.ToFormattedString(tabs);
            }

            var cancelRequestedQuery = trackingQuery as CancelRequestedQuery;
            if (cancelRequestedQuery != null)
            {
                return cancelRequestedQuery.ToFormattedString(tabs);
            }

            var customTrackingQuery = trackingQuery as CustomTrackingQuery;
            if (customTrackingQuery != null)
            {
                return customTrackingQuery.ToFormattedString(tabs);
            }

            var faultPropagationQuery = trackingQuery as FaultPropagationQuery;
            if (faultPropagationQuery != null)
            {
                return faultPropagationQuery.ToFormattedString(tabs);
            }

            var workflowInstanceQuery = trackingQuery as WorkflowInstanceQuery;
            if (workflowInstanceQuery != null)
            {
                return workflowInstanceQuery.ToFormattedString(tabs);
            }

            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendLine(trackingQuery.GetType().Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendDictionary("QueryAnnotations", trackingQuery.QueryAnnotations);
            }

            return tsb.ToString();
        }

        /// <summary>
        /// The trace.
        /// </summary>
        /// <param name="trackingQuery">
        /// The tracking query.
        /// </param>
        /// <param name="tabs"> the tabs
        /// The tabs.
        /// </param>
        public static void Trace(this TrackingQuery trackingQuery, int tabs = 0)
        {
            WorkflowTrace.Information(ToFormattedString(trackingQuery));
        }

        #endregion
    }
}