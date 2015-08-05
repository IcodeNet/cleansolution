// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateRecordEnumerable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Linq
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Linq Extensions for IEnumerable{ActivityStateRecord}
    /// </summary>
    public static class ActivityStateRecordEnumerable
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id 
        /// </param>
        /// <param name="startRecordNumber">
        /// The start index. 
        /// </param>
        /// <returns>
        /// true if any elements in the source sequence pass the test in the specified predicate; otherwise, false 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The source is null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The startRecordNumber is out of rane
        /// </exception>
        public static bool Any(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            using (var enumerator = source.Where(state, displayName, activityId, startRecordNumber).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the first element in a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// The first element in the sequence that passes the test in the specified predicate function. 
        /// </returns>
        public static ActivityStateRecord First(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().First(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// default(TSource) if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate. 
        /// </returns>
        public static ActivityStateRecord FirstOrDefault(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().FirstOrDefault(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// The last element in the sequence that passes the test in the specified predicate function. 
        /// </returns>
        public static ActivityStateRecord Last(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().Last(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// default(TSource) if the sequence is empty or if no elements pass the test in the predicate function; otherwise, the last element that passes the test in the predicate function. 
        /// </returns>
        public static ActivityStateRecord LastOrDefault(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().LastOrDefault(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// The single element of the input sequence that satisfies a condition. 
        /// </returns>
        public static ActivityStateRecord Single(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().Single(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity Id. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting index for the search 
        /// </param>
        /// <returns>
        /// The single element of the input sequence that satisfies the condition, or default(TSource) if no such element is found. 
        /// </returns>
        public static ActivityStateRecord SingleOrDefault(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().SingleOrDefault(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Filters a sequence of values based on the state, display number.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="state">
        /// The state. 
        /// </param>
        /// <param name="displayName">
        /// The display name. 
        /// </param>
        /// <param name="activityId">
        /// The activity id 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting record number for the search 
        /// </param>
        /// <returns>
        /// An IEnumerable{T} that contains elements from the input sequence that satisfy the condition. 
        /// </returns>
        public static IEnumerable<ActivityStateRecord> Where(
            this IEnumerable<TrackingRecord> source, 
            ActivityInstanceState state, 
            string displayName = null, 
            string activityId = null, 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return
                source.OfType<ActivityStateRecord>().Where(
                    record =>
                    record != null && (record.RecordNumber >= startRecordNumber)
                    && (string.IsNullOrEmpty(displayName) || record.Activity.Name == displayName)
                    && (string.IsNullOrEmpty(activityId) || record.Activity.Id == activityId)
                    && record.GetInstanceState() == state);
        }

        /// <summary>
        /// Filters a sequence of ActivityStateRecords based on the record containing an argument with the specified name and value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The value of the argument 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting record number for the search 
        /// </param>
        /// <returns>
        /// An IEnumerable{T} that contains elements from the input sequence that satisfy the condition. 
        /// </returns>
        public static IEnumerable<ActivityStateRecord> WithArgument<T>(
            this IEnumerable<ActivityStateRecord> source, 
            string argumentName, 
            T value = default(T), 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return source.Where(record => HasMatchingArgument(record, argumentName, value, startRecordNumber));
        }

        /// <summary>
        /// Filters a sequence of ActivityStateRecords based on the record containing an argument with the specified name and value.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting record number for the search 
        /// </param>
        /// <returns>
        /// An IEnumerable{T} that contains elements from the input sequence that satisfy the condition. 
        /// </returns>
        public static IEnumerable<ActivityStateRecord> WithArgument(
            this IEnumerable<ActivityStateRecord> source, string argumentName, long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return source.Where(record => HasMatchingArgument<object>(record, argumentName, null, startRecordNumber));
        }

        /// <summary>
        /// Filters a sequence of ActivityStateRecords based on the record containing an argument with the specified name and value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="value">
        /// The value of the argument 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting record number for the search 
        /// </param>
        /// <returns>
        /// An IEnumerable{T} that contains elements from the input sequence that satisfy the condition. 
        /// </returns>
        public static IEnumerable<ActivityStateRecord> WithVariable<T>(
            this IEnumerable<ActivityStateRecord> source, 
            string argumentName, 
            T value = default(T), 
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return source.Where(record => HasMatchingVariable(record, argumentName, value, startRecordNumber));
        }

        /// <summary>
        /// Filters a sequence of ActivityStateRecords based on the record containing an argument with the specified name and value.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument 
        /// </param>
        /// <param name="startRecordNumber">
        /// The zero-based starting record number for the search 
        /// </param>
        /// <returns>
        /// An IEnumerable{T} that contains elements from the input sequence that satisfy the condition. 
        /// </returns>
        public static IEnumerable<ActivityStateRecord> WithVariable(
            this IEnumerable<ActivityStateRecord> source,
            string argumentName,
            long startRecordNumber = 0)
        {
            Contract.Requires(source != null);
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Contract.Requires(startRecordNumber >= 0);
            if (startRecordNumber < 0)
            {
                throw new ArgumentOutOfRangeException("startRecordNumber");
            }

            return source.Where(record => HasMatchingVariable<object>(record, argumentName, null, startRecordNumber));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The argument value match.
        /// </summary>
        /// <param name="activityStateRecord">
        /// The activity state record. 
        /// </param>
        /// <param name="argumentName">
        /// The argument name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        private static bool ArgumentValueMatch<T>(ActivityStateRecord activityStateRecord, string argumentName, T value)
        {
            return object.Equals(value, default(T)) || object.Equals(activityStateRecord.GetArgument<T>(argumentName), value);
        }

        /// <summary>
        /// The predicate.
        /// </summary>
        /// <param name="activityStateRecord">
        /// The activity state record. 
        /// </param>
        /// <param name="argumentName">
        /// The argument name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The start record number. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        private static bool HasMatchingArgument<T>(
            ActivityStateRecord activityStateRecord, string argumentName, T value, long startRecordNumber)
        {
            return activityStateRecord != null
                   &&
                   activityStateRecord.Arguments.Any(
                       pair =>
                       activityStateRecord.RecordNumber >= startRecordNumber && NameMatchesKey(argumentName, pair)
                       && ArgumentValueMatch(activityStateRecord, argumentName, value));
        }

        /// <summary>
        /// The predicate.
        /// </summary>
        /// <param name="activityStateRecord">
        /// The activity state record. 
        /// </param>
        /// <param name="variableName">
        /// The argument name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="startRecordNumber">
        /// The start record number. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        private static bool HasMatchingVariable<T>(
            ActivityStateRecord activityStateRecord, string variableName, T value, long startRecordNumber)
        {
            return activityStateRecord != null
                   &&
                   activityStateRecord.Variables.Any(
                       pair =>
                       activityStateRecord.RecordNumber >= startRecordNumber && NameMatchesKey(variableName, pair)
                       && VariableValueMatch(activityStateRecord, variableName, value));
        }

        /// <summary>
        /// The argument name match.
        /// </summary>
        /// <param name="argumentName">
        /// The argument name. 
        /// </param>
        /// <param name="pair">
        /// The pair. 
        /// </param>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        private static bool NameMatchesKey(string argumentName, KeyValuePair<string, object> pair)
        {
            return pair.Key == argumentName;
        }

        /// <summary>
        /// The argument value match.
        /// </summary>
        /// <param name="activityStateRecord">
        /// The activity state record. 
        /// </param>
        /// <param name="variableName">
        /// The argument name. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument value 
        /// </typeparam>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        private static bool VariableValueMatch<T>(ActivityStateRecord activityStateRecord, string variableName, T value)
        {
            return object.Equals(value, default(T)) || object.Equals(activityStateRecord.GetVariable<T>(variableName), value);
        }

        #endregion
    }
}