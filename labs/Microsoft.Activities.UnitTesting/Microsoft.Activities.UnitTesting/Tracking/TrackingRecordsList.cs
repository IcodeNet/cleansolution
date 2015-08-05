// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingRecordsList.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Provides a list of TrackingRecord with helpers for LINQ
    /// </summary>
    public class TrackingRecordsList : IEnumerable<TrackingRecord>
    {
        #region Fields

        /// <summary>
        ///   The tracking record list
        /// </summary>
        private readonly List<TrackingRecord> records = new List<TrackingRecord>();

        /// <summary>
        ///   The lock
        /// </summary>
        private readonly object syncLock = new object();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the count of tracking records
        /// </summary>
        public int Count
        {
            get
            {
                return this.records.Count;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///   Gets the tracking record at the given index
        /// </summary>
        /// <param name="index"> The index of the tracking record </param>
        /// <returns> The tracking record </returns>
        public TrackingRecord this[int index]
        {
            get
            {
                return this.records.ElementAt(index);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Clears the tracking records
        /// </summary>
        public void Clear()
        {
            lock (this.syncLock)
            {
                this.records.Clear();
            }
        }

        /// <summary>
        /// Returns true if a record matching the predicate is found
        /// </summary>
        /// <param name="predicate">
        /// The predicate 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// true if the record exists 
        /// </returns>
        public bool Exists<TRecord>(Predicate<TRecord> predicate) where TRecord : TrackingRecord
        {
            return this.Exists(predicate, 0);
        }

        /// <summary>
        /// Returns true if a record matching the predicate is found
        /// </summary>
        /// <param name="predicate">
        /// The predicate 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// true if the record exists 
        /// </returns>
        public bool Exists<TRecord>(Predicate<TRecord> predicate, long startIndex) where TRecord : TrackingRecord
        {
            Contract.Requires(predicate != null);
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            Contract.Requires(startIndex >= 0);
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return this.Find(predicate, startIndex) != null;
        }

        /// <summary>
        /// Determines if an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool Exists(string displayName, ActivityInstanceState state)
        {
            return this.Exists(displayName, state, 0);
        }

        /// <summary>
        /// Determines if an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool Exists(string displayName, ActivityInstanceState state, long startIndex)
        {
            return this.Find(displayName, state, startIndex) != null;
        }

        /// <summary>
        /// Determines if a WorkflowInstanceRecord exists for the given record state
        /// </summary>
        /// <param name="instanceState">
        /// The instance record state. 
        /// </param>
        /// <returns>
        /// true if a record with the state exists, false if not 
        /// </returns>
        public bool Exists(WorkflowInstanceRecordState instanceState)
        {
            return this.Exists(instanceState, 0);
        }

        /// <summary>
        /// Determines if a WorkflowInstanceRecord exists for the given record state
        /// </summary>
        /// <param name="instanceState">
        /// The instance record state. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// true if a record with the state exists, false if not 
        /// </returns>
        public bool Exists(WorkflowInstanceRecordState instanceState, long startIndex)
        {
            return
                this.Exists<WorkflowInstanceRecord>(
                    rec => rec != null && rec.RecordNumber >= startIndex && rec.State == instanceState.ToString());
        }

        /// <summary>
        /// Determines if an activity state record with the given name, state and argument matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsArgMatch(string displayName, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.ExistsArgMatch(displayName, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Determines if an activity state record with the given name, state and argument matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsArgMatch(
            string displayName, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            return this.FindArgMatch(displayName, state, argumentName, pattern, startIndex) != null;
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsArgValue<T>(string displayName, ActivityInstanceState state, string argumentName, T value)
        {
            return this.ExistsArgValue(displayName, state, argumentName, value, 0);
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsArgValue<T>(
            string displayName, ActivityInstanceState state, string argumentName, T value, long startIndex)
        {
            return this.FindArgValue(displayName, state, argumentName, value) != null;
        }

        /// <summary>
        /// Returns true if an activity state record with the name and index exists at the specified index
        /// </summary>
        /// <param name="index">
        /// The index. 
        /// </param>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <returns>
        /// true if an activity state record with the name and index exists at the specified index 
        /// </returns>
        public bool ExistsAt(int index, string displayName, ActivityInstanceState state)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            var record = this.records.ElementAt(index) as ActivityStateRecord;

            return record != null && IsMatchingDisplayName(displayName, record) && IsMatchingState(state, record);
        }

        /// <summary>
        /// Returns true if an activity state record with the name and index exists at the specified index
        /// </summary>
        /// <param name="index">
        /// The index. 
        /// </param>
        /// <param name="id">
        /// The activity id. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <returns>
        /// true if an activity state record with the name and index exists at the specified index 
        /// </returns>
        public bool ExistsAtId(int index, string id, ActivityInstanceState state)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            var activityStateRecord = this.records.ElementAt(index) as ActivityStateRecord;

            return activityStateRecord != null && IsMatchingId(id, activityStateRecord)
                   && IsMatchingState(state, activityStateRecord);
        }

        /// <summary>
        /// Returns true if the specified number of matching records exist
        /// </summary>
        /// <param name="predicate">
        /// The predicate 
        /// </param>
        /// <param name="count">
        /// The count. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// true if the record exists 
        /// </returns>
        public bool ExistsCount<TRecord>(Predicate<TRecord> predicate, int count) where TRecord : TrackingRecord
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this.FindAll(predicate).Count() == count;
        }

        /// <summary>
        /// Determines if an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="count">
        /// The count. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsCount(string displayName, ActivityInstanceState state, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this.FindAll(displayName, state).Count() == count;
        }

        /// <summary>
        /// Determines if an activity state record with the given id and state
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsId(string id, ActivityInstanceState state)
        {
            return this.ExistsId(id, state, 0);
        }

        /// <summary>
        /// Determines if an activity state record with the given id and state
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsId(string id, ActivityInstanceState state, long startIndex)
        {
            return this.FindId(id, state, startIndex) != null;
        }

        /// <summary>
        /// Determines if an activity state record with the given name, state and argument matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsIdArgMatch(string id, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.ExistsIdArgMatch(id, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Determines if an activity state record with the given name, state and argument matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsIdArgMatch(
            string id, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            return this.FindIdArgMatch(id, state, argumentName, pattern, startIndex) != null;
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="id">
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsIdArgValue<T>(string id, ActivityInstanceState state, string argumentName, T value)
        {
            return this.ExistsIdArgValue(id, state, argumentName, value, 0);
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="id">
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
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsIdArgValue<T>(
            string id, ActivityInstanceState state, string argumentName, T value, long startIndex)
        {
            return this.FindIdArgValue(id, state, argumentName, value) != null;
        }

        /// <summary>
        /// Determines if a count of activity state record with the given id and state exists
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="count">
        /// The count. 
        /// </param>
        /// <returns>
        /// True if the record exists, false if not 
        /// </returns>
        public bool ExistsIdCount(string id, ActivityInstanceState state, int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this.FindAllId(id, state).Count() == count;
        }

        /// <summary>
        /// Finds a tracking record
        /// </summary>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// The tracking record cast to the type TRecord 
        /// </returns>
        public TRecord Find<TRecord>(Predicate<TRecord> predicate) where TRecord : TrackingRecord
        {
            return this.Find(predicate, 0);
        }

        /// <summary>
        /// Finds a tracking record
        /// </summary>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// The tracking record cast to the type TRecord 
        /// </returns>
        public TRecord Find<TRecord>(Predicate<TRecord> predicate, long startIndex) where TRecord : TrackingRecord
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return
                this.records.Find(rec => rec is TRecord && rec.RecordNumber >= startIndex && predicate((TRecord)rec)) as
                TRecord;
        }

        /// <summary>
        /// Finds an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord Find(string displayName, ActivityInstanceState state)
        {
            return this.Find(displayName, state, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord Find(string displayName, ActivityInstanceState state, long startIndex)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            return
                this.Find<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingDisplayName(displayName, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public IEnumerable<ActivityStateRecord> FindAll(string displayName, ActivityInstanceState state)
        {
            return
                this.FindAll<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingDisplayName(displayName, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord));
        }

        /// <summary>
        /// Finds all tracking records selected by the predicate
        /// </summary>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// A list of tracking records cast to the type of TRecord 
        /// </returns>
        public IEnumerable<TRecord> FindAll<TRecord>(Predicate<TRecord> predicate) where TRecord : TrackingRecord
        {
            return this.records.FindAll(rec => rec is TRecord && predicate((TRecord)rec)).ConvertAll(c => (TRecord)c);
        }

        /// <summary>
        /// Finds an activity state record with the given id and state
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public IEnumerable<ActivityStateRecord> FindAllId(string id, ActivityInstanceState state)
        {
            return
                this.FindAll<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingId(id, activityStateRecord) && IsMatchingState(state, activityStateRecord));
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindArgMatch(
            string displayName, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.FindArgMatch(displayName, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindArgMatch(
            string displayName, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentNullException("pattern");
            }

            return
                this.Find<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingDisplayName(displayName, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord)
                    && IsArgumentValueMatch(argumentName, pattern, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The argument value you want to find 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindArgValue<T>(
            string displayName, ActivityInstanceState state, string argumentName, T value)
        {
            return this.FindArgValue(displayName, state, argumentName, value, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The argument value you want to find 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindArgValue<T>(
            string displayName, ActivityInstanceState state, string argumentName, T value, long startIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException("displayName");
            }

            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            return
                this.Find<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingDisplayName(displayName, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord)
                    && IsArgumentValueEqual(argumentName, value, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindIdArgMatch(
            string id, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.FindIdArgMatch(id, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindIdArgMatch(
            string id, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            return
                this.Find<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingId(id, activityStateRecord) && IsMatchingState(state, activityStateRecord)
                    && IsArgumentValueMatch(argumentName, pattern, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The argument value you want to find 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindIdArgValue<T>(
            string id, ActivityInstanceState state, string argumentName, T value)
        {
            return this.FindIdArgValue(id, state, argumentName, value, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given id, state and argument with value matching a Regular Expression
        /// </summary>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The argument value you want to find 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// An activity state record if found 
        /// </returns>
        public ActivityStateRecord FindIdArgValue<T>(
            string id, ActivityInstanceState state, string argumentName, T value, long startIndex)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            return this.Find<ActivityStateRecord>(
                activityStateRecord =>
                IsMatchingId(id, activityStateRecord) && IsMatchingState(state, activityStateRecord)
                && IsArgumentValueEqual(argumentName, value, activityStateRecord), 
                startIndex);
        }

        /// <summary>
        /// Finds the index of a tracking record
        /// </summary>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndex<TRecord>(Predicate<TRecord> predicate) where TRecord : TrackingRecord
        {
            return this.FindIndex(predicate, 0);
        }

        /// <summary>
        /// Finds the index of a tracking record
        /// </summary>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <param name="startIndex">
        /// The starting index 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record 
        /// </typeparam>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndex<TRecord>(Predicate<TRecord> predicate, long startIndex) where TRecord : TrackingRecord
        {
            return
                this.records.FindIndex(
                    rec => rec is TRecord && rec.RecordNumber >= startIndex && predicate((TRecord)rec));
        }

        /// <summary>
        /// Finds the index of an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndex(string displayName, ActivityInstanceState state)
        {
            return this.FindIndex(displayName, state, 0);
        }

        /// <summary>
        /// Finds the index of an activity state record with the given name and state
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndex(string displayName, ActivityInstanceState state, long startIndex)
        {
            return this.FindIndex<ActivityStateRecord>(
                activityStateRecord =>
                activityStateRecord.Activity != null && activityStateRecord.Activity.Name == displayName
                && activityStateRecord.State == state.ToString(), 
                startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexArgMatch(
            string displayName, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.FindIndexArgMatch(displayName, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="displayName">
        /// The display name of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexArgMatch(
            string displayName, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            return
                this.FindIndex<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingDisplayName(displayName, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord)
                    && IsArgumentValueMatch(argumentName, pattern, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds the index of an activity state record with the given name and state
        /// </summary>
        /// <param name="id">
        /// The Id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexId(string id, ActivityInstanceState state)
        {
            return this.FindIndexId(id, state, 0);
        }

        /// <summary>
        /// Finds the index of an activity state record with the given name and state
        /// </summary>
        /// <param name="id">
        /// The Id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based index of the starting record 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexId(string id, ActivityInstanceState state, long startIndex)
        {
            return
                this.FindIndex<ActivityStateRecord>(
                    activityStateRecord =>
                    activityStateRecord.Activity != null && activityStateRecord.Activity.Id == id
                    && activityStateRecord.State == state.ToString(), 
                    startIndex);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="pattern">
        /// The regular expression to match 
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexIdArgMatch(string id, ActivityInstanceState state, string argumentName, string pattern)
        {
            return this.FindIndexIdArgMatch(id, state, argumentName, pattern, 0);
        }

        /// <summary>
        /// Finds an activity state record with the given name, state and argument with value matching a Regular Expression
        /// </summary>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The state you are searching for. 
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
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1. 
        /// </returns>
        public int FindIndexIdArgMatch(
            string id, ActivityInstanceState state, string argumentName, string pattern, long startIndex)
        {
            return
                this.FindIndex<ActivityStateRecord>(
                    activityStateRecord =>
                    IsMatchingId(id, activityStateRecord) && IsMatchingState(state, activityStateRecord)
                    && IsArgumentValueMatch(argumentName, pattern, activityStateRecord), 
                    startIndex);
        }

        /// <summary>
        /// Finds an ActivityScheduledRecord for the given parent and child activities
        /// </summary>
        /// <param name="parentDisplayName">
        /// The display name of the parent activity 
        /// </param>
        /// <param name="childDisplayName">
        /// The display name of the child activity 
        /// </param>
        /// <returns>
        /// The first matching record if found, null if not 
        /// </returns>
        public ActivityScheduledRecord FindScheduled(string parentDisplayName, string childDisplayName)
        {
            return this.FindScheduled(parentDisplayName, childDisplayName, 0);
        }

        /// <summary>
        /// Finds an ActivityScheduledRecord for the given parent and child activities
        /// </summary>
        /// <param name="parentDisplayName">
        /// The display name of the parent activity 
        /// </param>
        /// <param name="childDisplayName">
        /// The display name of the child activity 
        /// </param>
        /// <param name="startIndex">
        /// The staring record number of the search 
        /// </param>
        /// <returns>
        /// The first matching record if found, null if not 
        /// </returns>
        public ActivityScheduledRecord FindScheduled(string parentDisplayName, string childDisplayName, long startIndex)
        {
            if (string.IsNullOrWhiteSpace(parentDisplayName))
            {
                throw new ArgumentNullException("parentDisplayName");
            }

            if (string.IsNullOrWhiteSpace(childDisplayName))
            {
                throw new ArgumentNullException("childDisplayName");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return
                this.Find<ActivityScheduledRecord>(
                    activityStateRecord =>
                    activityStateRecord.RecordNumber >= startIndex
                    && IsMatchingDisplayName(parentDisplayName, childDisplayName, activityStateRecord));
        }

        /// <summary>
        /// Finds an ActivityScheduledRecord for the given parent and child activities
        /// </summary>
        /// <param name="parentstring">
        /// The Id of the parent activity 
        /// </param>
        /// <param name="childstring">
        /// The Id of the child activity 
        /// </param>
        /// <returns>
        /// The first matching record if found, null if not 
        /// </returns>
        public ActivityScheduledRecord FindScheduledId(string parentstring, string childstring)
        {
            return this.FindScheduledId(parentstring, childstring, 0);
        }

        /// <summary>
        /// Finds an ActivityScheduledRecord for the given parent and child activities
        /// </summary>
        /// <param name="parentstring">
        /// The Id of the parent activity 
        /// </param>
        /// <param name="childstring">
        /// The Id of the child activity 
        /// </param>
        /// <param name="startIndex">
        /// The staring record number of the search 
        /// </param>
        /// <returns>
        /// The first matching record if found, null if not 
        /// </returns>
        public ActivityScheduledRecord FindScheduledId(string parentstring, string childstring, long startIndex)
        {
            if (string.IsNullOrWhiteSpace(parentstring))
            {
                throw new ArgumentNullException("parentstring");
            }

            if (string.IsNullOrWhiteSpace(childstring))
            {
                throw new ArgumentNullException("childstring");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            return
                this.Find<ActivityScheduledRecord>(
                    activityStateRecord =>
                    activityStateRecord.RecordNumber >= startIndex
                    && IsMatchingId(parentstring, childstring, activityStateRecord));
        }

        /// <summary>
        ///   The get enumerator.
        /// </summary>
        /// <returns> A ReadOnlyCollection for enumeration </returns>
        /// <remarks>
        ///   The workflow may be running an adding more records to the list while you are enumerating the collection causing an exception.  To avoid this we copy the records to an array before returning the enumerator giving you a snapshot of the collection at the time.
        /// </remarks>
        public IEnumerator<TrackingRecord> GetEnumerator()
        {
            lock (this.syncLock)
            {
                return new SynchronizedReadOnlyCollection<TrackingRecord>(this.syncLock, this.records).GetEnumerator();
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///   The get enumerator.
        /// </summary>
        /// <returns> A ReadOnlyCollection for enumeration </returns>
        /// <remarks>
        ///   The workflow may be running an adding more records to the list while you are enumerating the collection causing an exception.  To avoid this we copy the records to an array before returning the enumerator giving you a snapshot of the collection at the time.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.syncLock)
            {
                return new SynchronizedReadOnlyCollection<TrackingRecord>(this.syncLock, this.records).GetEnumerator();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a record to the tracking list
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        internal void Add(TrackingRecord record)
        {
            lock (this.syncLock)
            {
                this.records.Add(record);
            }
        }

        /// <summary>
        /// The is argument value equal.
        /// </summary>
        /// <param name="argumentName">
        /// The argument name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="activityStateRecord">
        /// The activityStateRecord. 
        /// </param>
        /// <typeparam name="T">
        /// The record type 
        /// </typeparam>
        /// <returns>
        /// true if the argument value is equal. 
        /// </returns>
        private static bool IsArgumentValueEqual<T>(
            string argumentName, T value, ActivityStateRecord activityStateRecord)
        {
            // Arguments contains argumentName
            object argValue;
            if (!activityStateRecord.Arguments.TryGetValue(argumentName, out argValue))
            {
                return false;
            }

            // The argument value is null, and value is the default
            if (argValue == null && EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return true;
            }

            // The value is a string, the argument is null and the value string is null, empty or whitespace
            if (value is string && argValue == null && string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            // Argument value is not null and the string value is equal
            return argValue != null && argValue.ToString().Equals(value.ToString());
        }

        /// <summary>
        /// The is argument value match.
        /// </summary>
        /// <param name="argumentName">
        /// The argument name. 
        /// </param>
        /// <param name="pattern">
        /// The pattern. 
        /// </param>
        /// <param name="activityStateRecord">
        /// The activityStateRecord. 
        /// </param>
        /// <returns>
        /// True if the argument value matches. 
        /// </returns>
        private static bool IsArgumentValueMatch(
            string argumentName, string pattern, ActivityStateRecord activityStateRecord)
        {
            object argValue;
            return activityStateRecord.Arguments.TryGetValue(argumentName, out argValue) && argValue != null
                   && Regex.IsMatch(argValue.ToString(), pattern);
        }

        /// <summary>
        /// Determines if the record matches the display name.
        /// </summary>
        /// <param name="parentDisplayName">
        /// The parent display name. 
        /// </param>
        /// <param name="childDisplayName">
        /// The child display name. 
        /// </param>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The is matching display name. 
        /// </returns>
        private static bool IsMatchingDisplayName(
            string parentDisplayName, string childDisplayName, ActivityScheduledRecord record)
        {
            return record != null && record.Activity != null && record.Activity.Name == parentDisplayName
                   && record.Child.Name == childDisplayName;
        }

        /// <summary>
        /// Determines if the record matches the display name.
        /// </summary>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The is matching display name. 
        /// </returns>
        private static bool IsMatchingDisplayName(string displayName, ActivityStateRecord record)
        {
            return record != null && record.Activity != null && record.Activity.Name == displayName;
        }

        /// <summary>
        /// Determines if the record matches the id.
        /// </summary>
        /// <param name="parentstring">
        /// The parent activity id. 
        /// </param>
        /// <param name="childstring">
        /// The child activity id. 
        /// </param>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The is matching id. 
        /// </returns>
        private static bool IsMatchingId(
            string parentstring, string childstring, ActivityScheduledRecord record)
        {
            return record != null && record.Activity != null && record.Activity.Id == parentstring
                   && record.Child.Id == childstring;
        }

        /// <summary>
        /// Determines if the record matches the id.
        /// </summary>
        /// <param name="id">
        /// The id. 
        /// </param>
        /// <param name="activityStateRecord">
        /// The activityStateRecord. 
        /// </param>
        /// <returns>
        /// The is matching id. 
        /// </returns>
        private static bool IsMatchingId(string id, ActivityStateRecord activityStateRecord)
        {
            return activityStateRecord != null && activityStateRecord.Activity != null
                   && activityStateRecord.Activity.Id == id;
        }

        /// <summary>
        /// Determines if the record is a matching state.
        /// </summary>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <returns>
        /// The is matching state. 
        /// </returns>
        private static bool IsMatchingState(ActivityInstanceState state, ActivityStateRecord record)
        {
            return record != null && record.GetInstanceState() == state;
        }

        /// <summary>
        /// The find id.
        /// </summary>
        /// <param name="id">
        /// The id. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="startIndex">
        /// The start index. 
        /// </param>
        /// <returns>
        /// The activity state record 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The id is null or invalid
        /// </exception>
        private ActivityStateRecord FindId(string id, ActivityInstanceState state, long startIndex)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            return
                this.Find<ActivityStateRecord>(
                    activityStateRecord =>
                    activityStateRecord != null && IsMatchingId(id, activityStateRecord)
                    && IsMatchingState(state, activityStateRecord), 
                    startIndex);
        }

        #endregion
    }
}