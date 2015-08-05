// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultPropagationRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extensions for the FaultPropagationRecord class
    /// </summary>
    public static class FaultPropagationRecordExtensions
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
            this FaultPropagationRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);
            tsb.AppendFormat(
                "{0}Fault source {1}", record.GetRecordNumber(option), record.FaultSource.ToFormattedString());

            TrackingRecordExtensions.AppendInstance(
                tsb, option, record.InstanceId, record.Annotations, record.EventTime);
            return tsb.ToString();
        }

        /// <summary>
        /// Provides tracing for a FaultPropagation record
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
            this FaultPropagationRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}