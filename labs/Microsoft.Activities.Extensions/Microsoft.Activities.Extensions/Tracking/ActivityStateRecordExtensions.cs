// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extension methods for CustomTrackingRecordExtensions
    /// </summary>
    public static class ActivityStateRecordExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets an argument
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="name">
        /// The argument name. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument 
        /// </typeparam>
        /// <returns>
        /// The argument 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The record is null
        /// </exception>
        [DebuggerStepThrough]
        public static T GetArgument<T>(this ActivityStateRecord record, string name)
        {
            Contract.Requires(record != null);
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            object value;

            if (record.Arguments.TryGetValue(name, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        /// <summary>
        /// Converts the string representation of the ActivityStateRecord.State member to an ActivityInstanceState
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// An ActivityInstanceState 
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The string cannot be converted to the enum value
        /// </exception>
        public static ActivityInstanceState GetInstanceState(this ActivityStateRecord record)
        {
            Contract.Requires(record != null);
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            ActivityInstanceState activityInstanceState;
            if (Enum.TryParse(record.State, out activityInstanceState))
            {
                return activityInstanceState;
            }

            throw new ArgumentOutOfRangeException("record");
        }

        /// <summary>
        /// Gets a variable from the record
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="name">
        /// The variable name. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the variable 
        /// </typeparam>
        /// <returns>
        /// The variable 
        /// </returns>
        [DebuggerStepThrough]
        public static T GetVariable<T>(this ActivityStateRecord record, string name)
        {
            Contract.Requires(record != null);
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            object value;

            if (record.Variables.TryGetValue(name, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        /// <summary>
        /// Determines if the <see cref="ActivityStateRecord"/> was in a closed state
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// true if the activity was closed 
        /// </returns>
        public static bool IsClosed(this ActivityStateRecord record)
        {
            return GetInstanceState(record) == ActivityInstanceState.Closed;
        }

        /// <summary>
        /// Determines if the <see cref="ActivityStateRecord"/> was in an Executing state
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// true if the activity was Executing 
        /// </returns>
        public static bool IsExecuting(this ActivityStateRecord record)
        {
            Contract.Requires(record != null);
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            return GetInstanceState(record) == ActivityInstanceState.Executing;
        }

        /// <summary>
        /// Determines if the state matches
        /// </summary>
        /// <param name="record">
        /// The tracking record 
        /// </param>
        /// <param name="state">
        /// The state 
        /// </param>
        /// <returns>
        /// true if a match, false if not 
        /// </returns>
        public static bool StateIs(this ActivityStateRecord record, ActivityInstanceState state)
        {
            Contract.Requires(record != null);
            if (record == null)
            {
                throw new ArgumentNullException("record");
            }

            return state == GetInstanceState(record);
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
        /// <param name="tabs">
        /// the tabs 
        /// </param>
        /// <returns>
        /// The formatted tracking record 
        /// </returns>
        public static string ToFormattedString(
            this ActivityStateRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);
            tsb.AppendFormat(
                "{0}{1} is {2}{3}", 
                record.GetRecordNumber(option), 
                record.Activity.ToFormattedString(), 
                record.State, 
                option.HasFlag(TrackingOption.TypeName) ? string.Format(" ({0})", record.Activity.TypeName) : null);

            TrackingRecordExtensions.AppendInstance(
                tsb, 
                option, 
                record.InstanceId, 
                record.Annotations, 
                record.Arguments, 
                record.Variables, 
                null, 
                record.EventTime);
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
            this ActivityStateRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }
}