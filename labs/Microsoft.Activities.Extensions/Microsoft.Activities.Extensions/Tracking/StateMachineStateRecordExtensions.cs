// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineStateRecordExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    // ReSharper restore EmptyNamespace
#if NET401_OR_GREATER
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Statements.Tracking;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Extension methods for CustomTrackingRecordExtensions
    /// </summary>
    public static class StateMachineStateRecordExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get transitions.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="rootActivity">
        /// The root activity. 
        /// </param>
        /// <returns>
        /// A collection of possible transitions 
        /// </returns>
        public static List<string> GetTransitionNames(this StateMachineStateRecord record, Activity rootActivity)
        {
            var transitions = GetTransitions(record, rootActivity);
            return transitions != null
                       ? transitions.Select(transition => transition.DisplayName).ToList()
                       : new List<string>();
        }

        /// <summary>
        /// The get transitions.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="rootActivity">
        /// The root activity. 
        /// </param>
        /// <returns>
        /// A collection of possible transitions 
        /// </returns>
        public static IList<Transition> GetTransitions(this StateMachineStateRecord record, Activity rootActivity)
        {
            var activity = WorkflowInspectionServices.Resolve(rootActivity, record.Activity.Id);
            dynamic internalState = new ReflectionObject(activity);
            dynamic transitions = new ReflectionObject(internalState.Transitions);
            return ((ReflectionObject)transitions).Inner as IList<Transition>;
        }

        /// <summary>
        /// The get transitions.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// A collection of possible transitions 
        /// </returns>
        public static IList<Transition> GetTransitions(this StateMachineStateRecord record)
        {
            dynamic internalState = new ReflectionObject(record.Activity.GetActivity());
            dynamic transitions = new ReflectionObject(internalState.Transitions);
            return ((ReflectionObject)transitions).Inner as IList<Transition>;
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
            this StateMachineStateRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);

            tsb.AppendFormat(
                "{0}StateMachineStateRecord [{1}] \"{2}\" CurrentStateMachine State <{3}>", 
                record.GetRecordNumber(option), 
                record.Activity.GetId(), 
                record.StateMachineName ?? Constants.Null, 
                record.StateName);

            TrackingRecordExtensions.AppendInstance(
                tsb, option, record.InstanceId, record.Annotations, null, null, record.Data, record.EventTime);

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
            this StateMachineStateRecord record, TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            WorkflowTrace.Information(record.ToFormattedString(option, tabs));
        }

        #endregion
    }

#endif
}