// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
#if NET401_OR_GREATER
    using System.Activities.Statements.Tracking;
#endif
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.ServiceModel.Activities.Tracking;
    using System.Threading;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   The tracking record extensions.
    /// </summary>
    public static class TrackingRecordExtensions
    {
        #region Constants

        /// <summary>
        ///   The event time format
        /// </summary>
        public const string EventTimeFormat = "hh:mm:ss.ffff";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the record number if the tracking options indicate it should be included
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="option">
        /// The options. 
        /// </param>
        /// <returns>
        /// A string with the record number if the TrackingOptions.RecordNumber flag is used, otherwise an empty string 
        /// </returns>
        public static string GetRecordNumber(this TrackingRecord record, TrackingOption option = TrackingOption.Default)
        {
            var tsb = new TraceStringBuilder();
            if (option.HasFlag(TrackingOption.RecordNumber))
            {
                tsb.AppendFormat("{0}: ", record.RecordNumber);
            }

            return tsb.ToString();
        }

        /// <summary>
        /// Traces a tracking record
        /// </summary>
        /// <param name="record">
        /// The record to trace 
        /// </param>
        /// <param name="option">
        /// The tracking options to use, if not provided will use the TrackingOptions.Default value 
        /// </param>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        /// <remarks>
        /// The default ToString() method of tracking records produces difficult to read text
        ///   The extensions provide formatted indented text designed to make it more readable
        /// </remarks>
        /// <returns>
        /// The formatted string. 
        /// </returns>
        public static string ToFormattedString(
            this TrackingRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            if (option.HasFlag(TrackingOption.None))
            {
                return record.ToString();
            }

            var activityStateRecord = record as ActivityStateRecord;
            if (activityStateRecord != null)
            {
                return activityStateRecord.ToFormattedString(option, tabs);
            }

            var activityScheduledRecord = record as ActivityScheduledRecord;
            if (activityScheduledRecord != null)
            {
                return activityScheduledRecord.ToFormattedString(option, tabs);
            }

            var bookmarkResumptionRecord = record as BookmarkResumptionRecord;
            if (bookmarkResumptionRecord != null)
            {
                return bookmarkResumptionRecord.ToFormattedString(option, tabs);
            }

            var cancelRequestedRecord = record as CancelRequestedRecord;
            if (cancelRequestedRecord != null)
            {
                return cancelRequestedRecord.ToFormattedString(option, tabs);
            }

#if NET401_OR_GREATER
            var stateMachineStateRecord = record as StateMachineStateRecord;
            if (stateMachineStateRecord != null)
            {
                return stateMachineStateRecord.ToFormattedString(option, tabs);
            }

#endif
            var receiveMessageRecord = record as ReceiveMessageRecord;
            if (receiveMessageRecord != null)
            {
                return receiveMessageRecord.ToFormattedString(option, tabs);
            }

            var sendMessageRecord = record as SendMessageRecord;
            if (sendMessageRecord != null)
            {
                return sendMessageRecord.ToFormattedString(option, tabs);
            }

            var trackingTrace = record as ICustomTrackingTrace;
            if (trackingTrace != null)
            {
                return trackingTrace.ToFormattedString(option, tabs);
            }

            var customTrackingRecord = record as CustomTrackingRecord;
            if (customTrackingRecord != null)
            {
                return customTrackingRecord.ToFormattedString(option, tabs);
            }

            var faultPropagationRecord = record as FaultPropagationRecord;
            if (faultPropagationRecord != null)
            {
                return faultPropagationRecord.ToFormattedString(option, tabs);
            }

            var workflowInstanceAbortedRecord = record as WorkflowInstanceAbortedRecord;
            if (workflowInstanceAbortedRecord != null)
            {
                return workflowInstanceAbortedRecord.ToFormattedString(option, tabs);
            }

            var workflowInstanceSuspendedRecord = record as WorkflowInstanceSuspendedRecord;
            if (workflowInstanceSuspendedRecord != null)
            {
                return workflowInstanceSuspendedRecord.ToFormattedString(option, tabs);
            }

            var workflowInstanceTerminatedRecord = record as WorkflowInstanceTerminatedRecord;
            if (workflowInstanceTerminatedRecord != null)
            {
                return workflowInstanceTerminatedRecord.ToFormattedString(option, tabs);
            }

            var workflowInstanceUnhandledExceptionRecord = record as WorkflowInstanceUnhandledExceptionRecord;
            if (workflowInstanceUnhandledExceptionRecord != null)
            {
                return workflowInstanceUnhandledExceptionRecord.ToFormattedString(option, tabs);
            }

            var workflowInstanceRecord = record as WorkflowInstanceRecord;
            if (workflowInstanceRecord != null)
            {
                return workflowInstanceRecord.ToFormattedString(option, tabs);
            }

            return record.ToString();
        }

        /// <summary>
        /// Traces a tracking record
        /// </summary>
        /// <param name="record">
        /// The record to trace 
        /// </param>
        /// <param name="option">
        /// The tracking options to use, if not provided will use the TrackingOptions.Default value 
        /// </param>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        /// <remarks>
        /// The default ToString() method of tracking records produces difficult to read text
        ///   The extensions provide formatted indented text designed to make it more readable
        /// </remarks>
        public static void Trace(
            this TrackingRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Traces additional data about a tracking record
        /// </summary>
        /// <param name="tsb">
        /// The stringBuilder. 
        /// </param>
        /// <param name="option">
        /// A TrackingOptions value which specifies data to include in the trace 
        /// </param>
        /// <param name="instanceId">
        /// The instance ID of the tracking record 
        /// </param>
        /// <param name="annotations">
        /// The annotations dictionary 
        /// </param>
        /// <param name="arguments">
        /// The arguments dictionary 
        /// </param>
        /// <param name="variables">
        /// The variables dictionary 
        /// </param>
        /// <param name="data">
        /// The data dictionary 
        /// </param>
        /// <param name="eventTime">
        /// The event time 
        /// </param>
        internal static void AppendInstance(
            TraceStringBuilder tsb, 
            TrackingOption option, 
            Guid instanceId, 
            IDictionary<string, string> annotations, 
            IDictionary<string, object> arguments, 
            IDictionary<string, object> variables, 
            IDictionary<string, object> data, 
            DateTime eventTime)
        {
            if (!AnythingToTrace(option, tsb.Options, annotations, arguments, variables, data))
            {
                return;
            }

            tsb.AppendLine();

            using (tsb.IndentBlock())
            {
                if (option.HasFlag(TrackingOption.InstanceId))
                {
                    tsb.AppendProperty("InstanceId", instanceId);
                }

                if (option.HasFlag(TrackingOption.Time))
                {
                    tsb.AppendProperty("Event Time", eventTime.ToString(EventTimeFormat));
                }

                if (option.HasFlag(TrackingOption.Annotations))
                {
                    tsb.AppendDictionary("Annotations", annotations);
                }

                if (option.HasFlag(TrackingOption.Arguments))
                {
                    tsb.AppendDictionary("Arguments", arguments);
                }

                if (option.HasFlag(TrackingOption.Variables))
                {
                    tsb.AppendDictionary("Variables", variables);
                }

                if (option.HasFlag(TrackingOption.Data))
                {
                    tsb.AppendDictionary("Data", data);
                }
            }
        }

        /// <summary>
        /// Traces additional data about a tracking record
        /// </summary>
        /// <param name="tsb">
        /// The string builder. 
        /// </param>
        /// <param name="option">
        /// A TrackingOptions value which specifies data to include in the trace 
        /// </param>
        /// <param name="instanceId">
        /// The instance ID of the tracking record 
        /// </param>
        /// <param name="annotations">
        /// The annotations dictionary 
        /// </param>
        /// <param name="eventTime">
        /// The event time 
        /// </param>
        internal static void AppendInstance(
            TraceStringBuilder tsb, 
            TrackingOption option, 
            Guid instanceId, 
            IDictionary<string, string> annotations, 
            DateTime eventTime)
        {
            AppendInstance(
                tsb, option, instanceId, annotations, arguments: null, variables: null, data: null, eventTime: eventTime);
        }

        /// <summary>
        /// The anything to trace.
        /// </summary>
        /// <param name="option">
        /// The options. 
        /// </param>
        /// <param name="workflowTraceOptions">
        /// The trace options 
        /// </param>
        /// <param name="annotations">
        /// The annotations. 
        /// </param>
        /// <param name="arguments">
        /// The arguments. 
        /// </param>
        /// <param name="variables">
        /// The variables. 
        /// </param>
        /// <param name="data">
        /// The data. 
        /// </param>
        /// <returns>
        /// true if there is anything to trace. 
        /// </returns>
        private static bool AnythingToTrace(
            TrackingOption option, 
            WorkflowTraceOptions workflowTraceOptions, 
            IDictionary<string, string> annotations, 
            IDictionary<string, object> arguments, 
            IDictionary<string, object> variables, 
            IDictionary<string, object> data)
        {
            return option.HasFlag(TrackingOption.InstanceId) 
                   || option.HasFlag(TrackingOption.Time)
                   || workflowTraceOptions.HasFlag(WorkflowTraceOptions.ShowEmptyCollections)
                   || (option.HasFlag(TrackingOption.Annotations) && !annotations.IsNullOrEmpty())
                   || (option.HasFlag(TrackingOption.Arguments) && !arguments.IsNullOrEmpty())
                   || (option.HasFlag(TrackingOption.Variables) && !variables.IsNullOrEmpty())
                   || (option.HasFlag(TrackingOption.Data) && !data.IsNullOrEmpty());
        }

        #endregion
    }
}