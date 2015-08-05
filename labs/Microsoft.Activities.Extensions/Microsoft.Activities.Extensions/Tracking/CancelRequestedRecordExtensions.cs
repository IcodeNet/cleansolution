// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelRequestedRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extensions for the CancelRequestedRecord class
    /// </summary>
    public static class CancelRequestedRecordExtensions
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
            this CancelRequestedRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var owner = record.Activity == null ? "Host" : record.Activity.ToFormattedString();

            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendFormat(
                "{0}{1} requests cancel of activity [{2}] \"{3}\"", 
                record.GetRecordNumber(option), 
                owner, 
                record.Child != null ? record.Child.Id : Constants.Null, 
                record.Child != null ? record.Child.Name : Constants.Null);

            TrackingRecordExtensions.AppendInstance(tsb, option, record.InstanceId, record.Annotations, record.EventTime);
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
            this CancelRequestedRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}