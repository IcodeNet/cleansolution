// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookmarkResumptionRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extension methods for BookmarkResumptionRecord
    /// </summary>
    public static class BookmarkResumptionRecordExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Returns the record as a formatted string
        /// </summary>
        /// <param name="record">
        /// The record to trace. 
        /// </param>
        /// <param name="option">
        /// The tracking options to use, if not provided will use the TrackingOptions.Default value 
        /// </param>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        /// <returns>
        /// The formatted tracking record 
        /// </returns>
        public static string ToFormattedString(
            this BookmarkResumptionRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var traceablePayload = record.Payload as ITraceable;

            var tsb = new TraceStringBuilder(tabs);
            tsb.AppendFormat(
                "{0}Bookmark \"{1}\" resumed with payload <{2}> owner {3}", 
                record.GetRecordNumber(option), 
                record.BookmarkName ?? Constants.Null, 
                traceablePayload != null ? traceablePayload.ToFormattedString() : record.Payload ?? Constants.Null, 
                record.Owner.ToFormattedString());

            TrackingRecordExtensions.AppendInstance(
                tsb, option, record.InstanceId, record.Annotations, record.EventTime);
            return tsb.ToString();
        }

        /// <summary>
        /// Provides a human readable trace
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="option">
        /// The tracking options to use, if not provided will use the TrackingOptions.Default value 
        /// </param>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        public static void Trace(
            this BookmarkResumptionRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}