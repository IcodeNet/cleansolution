// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extension methods for WorkflowInstanceRecord
    /// </summary>
    public static class WorkflowInstanceRecordExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Converts the string Record state into a WorkflowInstanceRecordState
        /// </summary>
        /// <param name="record">
        /// The WorklfowInstanceRecord 
        /// </param>
        /// <returns>
        /// a WorkflowInstanceRecordState 
        /// </returns>
        public static WorkflowInstanceRecordState GetState(this WorkflowInstanceRecord record)
        {
            return (WorkflowInstanceRecordState)Enum.Parse(typeof(WorkflowInstanceRecordState), record.State);
        }

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
            this WorkflowInstanceRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendFormat(
                "{0}WorkflowInstance \"{1}\" is {2}", 
                record.GetRecordNumber(option), 
                record.ActivityDefinitionId, 
                record.State);

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
            this WorkflowInstanceRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}