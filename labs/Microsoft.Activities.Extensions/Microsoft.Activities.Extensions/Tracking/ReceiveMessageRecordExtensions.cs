// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveMessageRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Diagnostics;
    using System.ServiceModel.Activities.Tracking;
    using System.Text;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extensions to the ReceiveMessageRecord class
    /// </summary>
    public static class ReceiveMessageRecordExtensions
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
        /// <returns>
        /// The formatted tracking record 
        /// </returns>
        public static string ToFormattedString(
            this ReceiveMessageRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var sb =
                new TraceStringBuilder(
                    tabs,
                        "{0}Activity [{1}] \"{2}\" received Message ID {3}, E2E Activity ID {4}", 
                        record.GetRecordNumber(option), 
                        record.Activity != null ? record.Activity.Id : Constants.Null, 
                        record.Activity != null ? record.Activity.Name : Constants.Null, 
                        record.MessageId, 
                        record.E2EActivityId);

            TrackingRecordExtensions.AppendInstance(sb, option, record.InstanceId, record.Annotations, record.EventTime);
            return sb.ToString();
        }

        /// <summary>
        /// Provides a human readable trace
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="option">
        /// The tracking options to use, if not provided will use the TrackingRecordExtensions.Options value 
        /// </param>
        /// <param name="source">
        /// The source. 
        /// </param>
        public static void Trace(
            this ReceiveMessageRecord record, 
            TrackingOption option = TrackingOption.Default, int tabs = 0
            )
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}