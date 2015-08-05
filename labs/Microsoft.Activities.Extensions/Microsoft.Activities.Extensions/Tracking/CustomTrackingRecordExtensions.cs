// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTrackingRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extension methods for CustomTrackingRecordExtensions
    /// </summary>
    public static class CustomTrackingRecordExtensions
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
        /// <param name="tabs"> the tabs
        /// The tabs. 
        /// </param>
        /// <returns>
        /// The formatted tracking record 
        /// </returns>
        public static string ToFormattedString(
            this CustomTrackingRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendFormat(
                "{0}CustomTrackingRecord [{1}] \"{2}\"", 
                record.GetRecordNumber(option), 
                record.Activity.GetId(), 
                record.Name ?? Constants.Null);

            TrackingRecordExtensions.AppendInstance(
                tsb, 
                option, 
                instanceId: record.InstanceId, 
                annotations: record.Annotations, 
                arguments: null, 
                variables: null, 
                data: record.Data, 
                eventTime: record.EventTime);

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
        /// <param name="tabs"> the tabs
        /// The tabs. 
        /// </param>
        public static void Trace(
            this CustomTrackingRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}