// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryTrackingParticipant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
#if NET401_OR_GREATER
    using System.Activities.Statements.Tracking;
#endif
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    /// The memory tracking participant.
    /// </summary>
#if NET401_OR_GREATER
    public partial class MemoryTrackingParticipant : TrackingParticipant
#else
    public class MemoryTrackingParticipant : TrackingParticipant
#endif
    {
        #region Constants and Fields

        /// <summary>
        ///   The records.
        /// </summary>
        private readonly TrackingRecordsList records = new TrackingRecordsList();

        /// <summary>
        ///   The signal instance state.
        /// </summary>
        private readonly Dictionary<WorkflowInstanceRecordState, List<EventWaitHandle>> signalInstanceState =
            new Dictionary<WorkflowInstanceRecordState, List<EventWaitHandle>>();

        /// <summary>
        ///   The assert tracking.
        /// </summary>
        private AssertHostTracking assertTracking;

        #endregion

        #region Public Events

        /// <summary>
        ///   The Workflow aborted.
        /// </summary>
        public event EventHandler<WorkflowInstanceAbortedEventArgs> WorkflowAborted;

        /// <summary>
        ///   The workflow instance event handler.
        /// </summary>
        public event EventHandler<WorkflowInstanceEventArgs> WorkflowIdle;

        /// <summary>
        ///   The workflow instance event handler.
        /// </summary>
        public event EventHandler<WorkflowInstanceEventArgs> WorkflowInstanceEvent;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Assert.
        /// </summary>
        public AssertHostTracking Assert
        {
            get
            {
                return this.assertTracking ?? (this.assertTracking = new AssertHostTracking(this.records));
            }
        }

        /// <summary>
        ///   Gets or sets OnTrack.
        /// </summary>
        public Action<TrackingRecord, TimeSpan> OnTrack { get; set; }

        /// <summary>
        ///   Gets Records.
        /// </summary>
        public TrackingRecordsList Records
        {
            get
            {
                return this.records;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the tracking records
        /// </summary>
        public void Clear()
        {
            this.records.Clear();
        }

        /// <summary>
        /// Trace a tracking record
        /// </summary>
        /// <param name="trackingOption">
        /// The tracking record options
        /// </param>
        public void Trace(TrackingOption trackingOption = TrackingOption.Default)
        {
            var trackingProfile = this.TrackingProfile;
            if (trackingProfile != null)
            {
                trackingProfile.Trace();
            }

            foreach (var t in this.records)
            {
                t.Trace(trackingOption);
            }
        }

        /// <summary>
        /// The wait for workflow instance record.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        public void WaitForWorkflowInstanceRecord(WorkflowInstanceRecordState state, EventWaitHandle handle)
        {
            List<EventWaitHandle> handles;
            if (!this.signalInstanceState.TryGetValue(state, out handles))
            {
                handles = new List<EventWaitHandle>();
                this.signalInstanceState.Add(state, handles);
            }

            handles.Add(handle);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on raise aborted event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnRaiseAbortedEvent(WorkflowInstanceAbortedEventArgs args)
        {
            this.OnRaiseEvent(args, this.WorkflowAborted);
        }

        /// <summary>
        /// The on raise event.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="eventHandler">
        /// The event handler.
        /// </param>
        /// <typeparam name="T">
        /// The type of the event args
        /// </typeparam>
        protected virtual void OnRaiseEvent<T>(T args, EventHandler<T> eventHandler) where T : EventArgs
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised
            var handler = eventHandler;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// The workflow is idle
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnRaiseWorkflowIdleInstanceEvent(WorkflowInstanceEventArgs args)
        {
            this.OnRaiseEvent(args, this.WorkflowIdle);
        }

        /// <summary>
        /// There is a workflow instance event
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnRaiseWorkflowInstanceEvent(WorkflowInstanceEventArgs args)
        {
            this.OnRaiseEvent(args, this.WorkflowInstanceEvent);
        }

        /// <summary>
        /// The track.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            this.records.Add(record);

            if (record is WorkflowInstanceAbortedRecord)
            {
                this.OnRaiseAbortedEvent(new WorkflowInstanceAbortedEventArgs(record));
            }
            else if (record is WorkflowInstanceRecord)
            {
                this.OnRaiseWorkflowInstanceEvent(new WorkflowInstanceEventArgs(record));

                var state = (record as WorkflowInstanceRecord).GetState();

                if (state == WorkflowInstanceRecordState.Idle)
                {
                    this.OnRaiseWorkflowIdleInstanceEvent(new WorkflowInstanceEventArgs(record));
                }

                List<EventWaitHandle> handles;

                if (this.signalInstanceState.TryGetValue(state, out handles))
                {
                    foreach (var handle in handles)
                    {
                        handle.Set();
                    }
                }
            }

#if NET401_OR_GREATER
            var smr = record as StateMachineStateRecord;
            if (smr != null)
            {
                this.OnStateMachineState(smr);
            }

#endif
            if (this.OnTrack != null)
            {
                this.OnTrack(record, timeout);
            }
        }

        #endregion

        /// <summary>
        /// The assert host tracking.
        /// </summary>
        public class AssertHostTracking
        {
            #region Constants and Fields

            /// <summary>
            ///   The records.
            /// </summary>
            private readonly TrackingRecordsList records = new TrackingRecordsList();

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AssertHostTracking"/> class.
            /// </summary>
            /// <param name="records">
            /// The records.
            /// </param>
            internal AssertHostTracking(TrackingRecordsList records)
            {
                this.records = records;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// The does not exist.
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void DoesNotExist(string displayName, ActivityInstanceState state, string message = null)
            {
                this.DoesNotExist(displayName, state, 0, message);
            }

            /// <summary>
            /// The does not exist.
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void DoesNotExist(string displayName, ActivityInstanceState state, long startIndex, string message = null)
            {
                AssertTracking.DoesNotExist(this.records, displayName, state, startIndex, message);
            }

            /// <summary>
            /// Verifies that a WorkflowInstanceRecordState record does not exist
            /// </summary>
            /// <param name="instanceRecordState">
            /// The state to search for
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            [DebuggerStepThrough]
            public void DoesNotExist(WorkflowInstanceRecordState instanceRecordState, string message = null)
            {
                this.DoesNotExist(instanceRecordState, 0, message);
            }

            /// <summary>
            /// Verifies that a WorkflowInstanceRecordState record does not exist
            /// </summary>
            /// <param name="instanceRecordState">
            /// The state to search for
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            [DebuggerStepThrough]
            public void DoesNotExist(WorkflowInstanceRecordState instanceRecordState, long startIndex, string message = null)
            {
                AssertTracking.DoesNotExist(this.records, instanceRecordState, startIndex, message);
            }

            /// <summary>
            /// Verifies that a record does not exist
            /// </summary>
            /// <param name="id">
            /// The activity id.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The assert failed
            /// </exception>
            public void DoesNotExistId(string id, ActivityInstanceState state, string message = null)
            {
                this.DoesNotExistId(id, state, 0, message);
            }

            /// <summary>
            /// Verifies that a record does not exist
            /// </summary>
            /// <param name="id">
            /// The activity id.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The assert failed
            /// </exception>
            public void DoesNotExistId(string id, ActivityInstanceState state, long startIndex, string message = null)
            {
                AssertTracking.DoesNotExistId(this.records, id, state, startIndex, message);
            }

            /// <summary>
            /// Verifies that a record exists
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The state.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void Exists(string displayName, ActivityInstanceState state, string message = null)
            {
                AssertTracking.Exists(this.records, displayName, state, 0, message);
            }

            /// <summary>
            /// Verifies that a record exists
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The state.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void Exists(string displayName, ActivityInstanceState state, int count, string message = null)
            {
                AssertTracking.ExistsCount(this.records, displayName, state, count, message);
            }

            /// <summary>
            /// The exists.
            /// </summary>
            /// <param name="instanceState">
            /// The instance state.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void Exists(WorkflowInstanceRecordState instanceState, string message = null)
            {
                AssertTracking.Exists(this.records, instanceState);
            }

            /// <summary>
            /// Verifies that a WorkflowInstanceRecord with the given state exists at the start index
            /// </summary>
            /// <param name="instanceState">
            /// The instance state
            /// </param>
            /// <param name="startIndex">
            /// The start index
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            [DebuggerStepThrough]
            public void Exists(WorkflowInstanceRecordState instanceState, long startIndex, string message = null)
            {
                AssertTracking.Exists(this.records, instanceState, startIndex);
            }

            /// <summary>
            /// Verifies that a tracking record exists for the given predicate
            /// </summary>
            /// <param name="predicate">
            /// The predicate.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <typeparam name="TRecord">
            /// The type of tracking record you are looking for
            /// </typeparam>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void Exists<TRecord>(Predicate<TRecord> predicate, string message = null) where TRecord : TrackingRecord
            {
                this.Exists(predicate, 0, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists for the given predicate
            /// </summary>
            /// <param name="predicate">
            /// The predicate.
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index for the search
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <typeparam name="TRecord">
            /// The type of tracking record you are looking for
            /// </typeparam>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void Exists<TRecord>(Predicate<TRecord> predicate, long startIndex, string message = null) where TRecord : TrackingRecord
            {
                AssertTracking.Exists(this.records, predicate, startIndex, message);
            }

            /// <summary>
            /// Verifies that a record exists with a given argument and value matching a regular expression
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The state.
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="pattern">
            /// The regular expression to match
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void ExistsArgMatch(string displayName, ActivityInstanceState state, string argumentName, string pattern, string message = null)
            {
                this.ExistsArgMatch(displayName, state, argumentName, pattern, 0, message);
            }

            /// <summary>
            /// Verifies that a record exists with a given argument and value matching a regular expression
            /// </summary>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The state.
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="pattern">
            /// The regular expression to match
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void ExistsArgMatch(
                string displayName, ActivityInstanceState state, string argumentName, string pattern, long startIndex, string message = null)
            {
                AssertTracking.ExistsArgMatch(this.records, displayName, state, argumentName, pattern, startIndex, message);
            }

            /// <summary>
            /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
            /// </summary>
            /// <typeparam name="T">
            /// The type of the value
            /// </typeparam>
            /// <param name="displayName">
            /// The display name of the activity
            /// </param>
            /// <param name="state">
            /// The activity instance state
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="value">
            /// The regular expression pattern to match
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            public void ExistsArgValue<T>(string displayName, ActivityInstanceState state, string argumentName, T value, string message = null)
            {
                this.ExistsArgValue(displayName, state, argumentName, value, 0, message);
            }

            /// <summary>
            /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
            /// </summary>
            /// <typeparam name="T">
            /// The type of the value
            /// </typeparam>
            /// <param name="displayName">
            /// The display name of the activity
            /// </param>
            /// <param name="state">
            /// The activity instance state
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="value">
            /// The regular expression pattern to match
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            public void ExistsArgValue<T>(string displayName, ActivityInstanceState state, string argumentName, T value, long startIndex, string message = null)
            {
                AssertTracking.ExistsArgValue(this.records, displayName, state, argumentName, value, startIndex, message);
            }

            /// <summary>
            /// Verifies that a record exists at the index.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="displayName">
            /// The activity name.
            /// </param>
            /// <param name="state">
            /// The state.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            [DebuggerStepThrough]
            public void ExistsAt(int index, string displayName, ActivityInstanceState state, string message = null)
            {
                AssertTracking.ExistsAt(this.records, index, displayName, state, message);
            }

            /// <summary>
            /// Asserts that an activity with the given name exists before another activity with the given state
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBefore(string beforeDisplayName, string afterDisplayName, ActivityInstanceState state, string message = null)
            {
                this.ExistsBefore(beforeDisplayName, afterDisplayName, state, 0, message);
            }

            /// <summary>
            /// Asserts that an activity with the given name exists before another activity with the given state
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="beforeState">
            /// The activity state of the before record
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="afterState">
            /// The activity state of the after record
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBefore(
                string beforeDisplayName, ActivityInstanceState beforeState, string afterDisplayName, ActivityInstanceState afterState, string message = null)
            {
                this.ExistsBefore(beforeDisplayName, beforeState, afterDisplayName, afterState, 0, message);
            }

            /// <summary>
            /// Asserts that an activity with the given name exists before another activity with the given state
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="beforeState">
            /// The activity state of the before record
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="afterState">
            /// The activity state of the after record
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBefore(
                string beforeDisplayName, 
                ActivityInstanceState beforeState, 
                string afterDisplayName, 
                ActivityInstanceState afterState, 
                long startIndex, 
                string message = null)
            {
                AssertTracking.ExistsBefore(this.records, beforeDisplayName, afterDisplayName, afterState, startIndex, message);
            }

            /// <summary>
            /// Asserts that an activity with the given name exists before another activity with the given state
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="state">
            /// The activity state
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBefore(string beforeDisplayName, string afterDisplayName, ActivityInstanceState state, long startIndex, string message = null)
            {
                AssertTracking.ExistsBefore(this.records, beforeDisplayName, afterDisplayName, state, startIndex, message);
            }

            /// <summary>
            /// Asserts that a tracking record exists prior to another tracking record
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the before activity
            /// </param>
            /// <param name="beforeState">
            /// The activity state of the before record
            /// </param>
            /// <param name="beforeArgumentName">
            /// The name of the before argument
            /// </param>
            /// <param name="beforePattern">
            /// The regular expression to match the before argument value to
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the after activity
            /// </param>
            /// <param name="afterState">
            /// The ActivityInstanceState
            /// </param>
            /// <param name="afterArgumentName">
            /// The name of the after argument
            /// </param>
            /// <param name="afterPattern">
            /// The regular expression to match the after argument value to
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            public void ExistsBeforeArgMatch(
                string beforeDisplayName, 
                ActivityInstanceState beforeState, 
                string beforeArgumentName, 
                string beforePattern, 
                string afterDisplayName, 
                ActivityInstanceState afterState, 
                string afterArgumentName, 
                string afterPattern, 
                string message = null)
            {
                this.ExistsBeforeArgMatch(
                    beforeDisplayName, beforeState, beforeArgumentName, beforePattern, afterDisplayName, afterState, afterArgumentName, afterPattern, 0, message);
            }

            /// <summary>
            /// Asserts that a tracking record exists prior to another tracking record
            /// </summary>
            /// <param name="beforeDisplayName">
            /// The name of the before activity
            /// </param>
            /// <param name="beforeState">
            /// The activity state of the before record
            /// </param>
            /// <param name="beforeArgumentName">
            /// The name of the before argument
            /// </param>
            /// <param name="beforePattern">
            /// The regular expression to match the before argument value to
            /// </param>
            /// <param name="afterDisplayName">
            /// The name of the after activity
            /// </param>
            /// <param name="afterState">
            /// The ActivityInstanceState
            /// </param>
            /// <param name="afterArgumentName">
            /// The name of the after argument
            /// </param>
            /// <param name="afterPattern">
            /// The regular expression to match the after argument value to
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            public void ExistsBeforeArgMatch(
                string beforeDisplayName, 
                ActivityInstanceState beforeState, 
                string beforeArgumentName, 
                string beforePattern, 
                string afterDisplayName, 
                ActivityInstanceState afterState, 
                string afterArgumentName, 
                string afterPattern, 
                long startIndex, 
                string message = null)
            {
                AssertTracking.ExistsBeforeArgMatch(
                    this.records, 
                    beforeDisplayName, 
                    beforeState, 
                    beforeArgumentName, 
                    beforePattern, 
                    afterDisplayName, 
                    afterState, 
                    afterArgumentName, 
                    afterPattern, 
                    startIndex, 
                    message);
            }

            /// <summary>
            /// Asserts that an activity with the given Id exists before another activity with the given state
            /// </summary>
            /// <param name="beforestring">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="afterstring">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBeforeId(string beforestring, string afterstring, ActivityInstanceState state, string message = null)
            {
                this.ExistsBefore(beforestring, afterstring, state, 0, message);
            }

            /// <summary>
            /// Asserts that an activity with the given name exists before another activity with the given state
            /// </summary>
            /// <param name="beforestring">
            /// The name of the activity which should be before
            /// </param>
            /// <param name="afterstring">
            /// The name of the activity which should be after
            /// </param>
            /// <param name="state">
            /// The activity state
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ExistsBeforeId(string beforestring, string afterstring, ActivityInstanceState state, long startIndex, string message = null)
            {
                AssertTracking.ExistsBeforeId(this.records, beforestring, afterstring, state, startIndex, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists for the given predicate
            /// </summary>
            /// <param name="predicate">
            /// The predicate.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <typeparam name="TRecord">
            /// The type of tracking record you are looking for
            /// </typeparam>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsCount<TRecord>(Predicate<TRecord> predicate, int count, string message = null) where TRecord : TrackingRecord
            {
                AssertTracking.ExistsCount(this.records, predicate, count, message);
            }

            /// <summary>
            /// Verifies that a count of matching tracking records exists
            /// </summary>
            /// <param name="displayName">
            /// The activity display name.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="count">
            /// The count of records to match.
            /// </param>
            /// <param name="message">
            /// The failure message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsCount(string displayName, ActivityInstanceState state, int count, string message = null)
            {
                AssertTracking.ExistsCount(this.records, displayName, state, count, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists
            /// </summary>
            /// <param name="id">
            /// The id of the activity.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="message">
            /// The fail message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsId(string id, ActivityInstanceState state, string message = null)
            {
                this.ExistsId(id, state, 0, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists
            /// </summary>
            /// <param name="id">
            /// The id of the activity.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The fail message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsId(string id, ActivityInstanceState state, long startIndex, string message = null)
            {
                AssertTracking.ExistsId(this.records, id, state, startIndex, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists
            /// </summary>
            /// <param name="id">
            /// The id of the activity.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="pattern">
            /// The regular expression to match
            /// </param>
            /// <param name="message">
            /// The fail message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsIdArgMatch(string id, ActivityInstanceState state, string argumentName, string pattern, string message = null)
            {
                this.ExistsIdArgMatch(id, state, argumentName, pattern, 0, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists
            /// </summary>
            /// <param name="id">
            /// The id of the activity.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="pattern">
            /// The regular expression to match
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The fail message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsIdArgMatch(string id, ActivityInstanceState state, string argumentName, string pattern, long startIndex, string message = null)
            {
                AssertTracking.ExistsIdArgMatch(this.records, id, state, argumentName, pattern, startIndex, message);
            }

            /// <summary>
            /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
            /// </summary>
            /// <typeparam name="T">
            /// The type of the value
            /// </typeparam>
            /// <param name="id">
            /// The id of the activity
            /// </param>
            /// <param name="state">
            /// The activity instance state
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="value">
            /// The regular expression pattern to match
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            public void ExistsIdArgValue<T>(string id, ActivityInstanceState state, string argumentName, T value, string message = null)
            {
                this.ExistsIdArgValue(id, state, argumentName, value, 0, message);
            }

            /// <summary>
            /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
            /// </summary>
            /// <typeparam name="T">
            /// The type of the value
            /// </typeparam>
            /// <param name="id">
            /// The id of the activity
            /// </param>
            /// <param name="state">
            /// The activity instance state
            /// </param>
            /// <param name="argumentName">
            /// The name of the argument
            /// </param>
            /// <param name="value">
            /// The regular expression pattern to match
            /// </param>
            /// <param name="startIndex">
            /// The zero-based starting index of the search.
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            public void ExistsIdArgValue<T>(string id, ActivityInstanceState state, string argumentName, T value, long startIndex, string message = null)
            {
                AssertTracking.ExistsIdArgValue(this.records, id, state, argumentName, value, 0, message);
            }

            /// <summary>
            /// Verifies that a tracking record exists
            /// </summary>
            /// <param name="id">
            /// The id of the activity.
            /// </param>
            /// <param name="state">
            /// The activity state.
            /// </param>
            /// <param name="count">
            /// The count of records to match.
            /// </param>
            /// <param name="message">
            /// The failure message.
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// Thrown if the record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void ExistsIdCount(string id, ActivityInstanceState state, int count, string message = null)
            {
                AssertTracking.ExistsIdCount(this.records, id, state, count, message);
            }

            /// <summary>
            /// Asserts that a record was scheduled
            /// </summary>
            /// <param name="parentDisplayName">
            /// The parent activity
            /// </param>
            /// <param name="childDisplayName">
            /// The child activity
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The assert failed
            /// </exception>
            [DebuggerStepThrough]
            public void Scheduled(string parentDisplayName, string childDisplayName, string message = null)
            {
                this.Scheduled(parentDisplayName, childDisplayName, 0, message);
            }

            /// <summary>
            /// Asserts that a record was scheduled
            /// </summary>
            /// <param name="parentDisplayName">
            /// The parent activity
            /// </param>
            /// <param name="childDisplayName">
            /// The child activity
            /// </param>
            /// <param name="startRecord">
            /// The staring record number of the search
            /// </param>
            /// <param name="message">
            /// The failure message
            /// </param>
            /// <exception cref="WorkflowAssertFailedException">
            /// The record does not exist
            /// </exception>
            [DebuggerStepThrough]
            public void Scheduled(string parentDisplayName, string childDisplayName, int startRecord, string message = null)
            {
                AssertTracking.Scheduled(this.records, parentDisplayName, childDisplayName, startRecord, message);
            }

            /// <summary>
            /// Asserts that a child activity was scheduled by a parent activity
            /// </summary>
            /// <param name="parentDisplayNameId">
            /// The parent activity display name
            /// </param>
            /// <param name="childDisplayNameId">
            /// The child activity display name
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ScheduledId(string parentDisplayNameId, string childDisplayNameId, string message = null)
            {
                this.ScheduledId(parentDisplayNameId, childDisplayNameId, 0, message);
            }

            /// <summary>
            /// Asserts that a child activity was scheduled by a parent activity
            /// </summary>
            /// <param name="parentDisplayNameId">
            /// The parent activity display name
            /// </param>
            /// <param name="childDisplayNameId">
            /// The child activity display name
            /// </param>
            /// <param name="startRecord">
            /// The starting record number of the search
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ScheduledId(string parentDisplayNameId, string childDisplayNameId, int startRecord, string message = null)
            {
                AssertTracking.ScheduledId(this.records, parentDisplayNameId, childDisplayNameId, startRecord, message);
            }

            /// <summary>
            /// Asserts that a child activity was scheduled by a parent activity
            /// </summary>
            /// <param name="parentDisplayName">
            /// The parent activity display name
            /// </param>
            /// <param name="childDisplayName">
            /// The child activity display name
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ScheduledName(string parentDisplayName, string childDisplayName, string message = null)
            {
                this.ScheduledName(parentDisplayName, childDisplayName, 0, message);
            }

            /// <summary>
            /// Asserts that a child activity was scheduled by a parent activity
            /// </summary>
            /// <param name="parentDisplayName">
            /// The parent activity display name
            /// </param>
            /// <param name="childDisplayName">
            /// The child activity display name
            /// </param>
            /// <param name="startRecord">
            /// The starting record number of the search
            /// </param>
            /// <param name="message">
            /// The assertion failure message
            /// </param>
            [DebuggerStepThrough]
            public void ScheduledName(string parentDisplayName, string childDisplayName, int startRecord, string message = null)
            {
                AssertTracking.Scheduled(this.records, parentDisplayName, childDisplayName, startRecord, message);
            }

            #endregion
        }
    }
}