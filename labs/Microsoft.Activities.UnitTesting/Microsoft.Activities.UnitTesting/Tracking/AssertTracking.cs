// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertTracking.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Properties;

    /// <summary>
    ///   Asserts aspects of tracking record lists
    /// </summary>
    public static class AssertTracking
    {
        #region Public Methods and Operators

        /// <summary>
        /// Verifies that a record does not exist
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
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
        public static void DoesNotExist(
            TrackingRecordsList records, string displayName, ActivityInstanceState state, string message = null)
        {
            DoesNotExist(records, displayName, state, 0, message);
        }

        /// <summary>
        /// Verifies that a record does not exist
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
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
        public static void DoesNotExist(
            TrackingRecordsList records, 
            string displayName, 
            ActivityInstanceState state, 
            long startIndex, 
            string message = null)
        {

            if (records.OfType<ActivityStateRecord>().Where((record, i) => i >= startIndex && record.GetInstanceState() == state && record.Activity.Name == displayName).Any())
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordExists, message, "DoesNotExist", displayName, state));
            }
        }

        /// <summary>
        /// Asserts that a Workflow instance state record does not exist for the given state after the start index
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="instanceRecordState">
        /// The instance record state 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void DoesNotExist(
            TrackingRecordsList records, WorkflowInstanceRecordState instanceRecordState, string message = null)
        {
            DoesNotExist(records, instanceRecordState, 0, message);
        }

        /// <summary>
        /// Asserts that a Workflow instance state record does not exist for the given state after the start index
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="instanceRecordState">
        /// The instance record state 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void DoesNotExist(
            TrackingRecordsList records, 
            WorkflowInstanceRecordState instanceRecordState, 
            long startIndex, 
            string message = null)
        {
            if (records.Exists(instanceRecordState, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.WorkflowInstanceStateNotFoundIndex, 
                        message, 
                        "DoesNotExist", 
                        instanceRecordState, 
                        startIndex));
            }
        }

        /// <summary>
        /// Verifies that a record does not exist
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="id">
        /// The activity id. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void DoesNotExistId(TrackingRecordsList records, string id, ActivityInstanceState state)
        {
            DoesNotExistId(records, id, state, 0);
        }

        /// <summary>
        /// Verifies that a record does not exist
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
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
        public static void DoesNotExistId(
            TrackingRecordsList records, string id, ActivityInstanceState state, string message)
        {
            DoesNotExistId(records, id, state, 0, message);
        }

        /// <summary>
        /// Verifies that a record does not exist
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
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
        public static void DoesNotExistId(
            TrackingRecordsList records, string id, ActivityInstanceState state, long startIndex, string message = null)
        {
            if (records.ExistsId(id, state, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.ActivityStateRecordExists, message, "DoesNotExistId", id, state));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void Exists(TrackingRecordsList records, string displayName, ActivityInstanceState state)
        {
            Exists(records, displayName, state, 0, null);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index for the search 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void Exists(
            TrackingRecordsList records, string displayName, ActivityInstanceState state, long startIndex)
        {
            Exists(records, displayName, state, 0, null);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index for the search 
        /// </param>
        /// <param name="message">
        /// The fail message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void Exists(
            TrackingRecordsList records, 
            string displayName, 
            ActivityInstanceState state, 
            long startIndex, 
            string message)
        {
            if (!records.Exists(displayName, state, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordDoesNotExist, message, "Exists", displayName, state));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists for the given predicate
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record you are looking for 
        /// </typeparam>
        /// <exception cref="WorkflowAssertFailedException">
        /// The record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void Exists<TRecord>(TrackingRecordsList records, Predicate<TRecord> predicate)
            where TRecord : TrackingRecord
        {
            Exists(records, predicate, null);
        }

        /// <summary>
        /// Verifies that a tracking record exists for the given predicate
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void Exists<TRecord>(
            TrackingRecordsList records, Predicate<TRecord> predicate, string message)
            where TRecord : TrackingRecord
        {
            if (!records.Exists(predicate))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.TrackingRecordDoesNotExist, message, "Exists"));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists for the given predicate
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void Exists<TRecord>(
            TrackingRecordsList records, Predicate<TRecord> predicate, long startIndex, string message = null)
            where TRecord : TrackingRecord
        {
            if (!records.Exists(predicate, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.TrackingRecordDoesNotExist, message, "Exists"));
            }
        }

        /// <summary>
        /// Asserts that a WorkflowInstance Record exists
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="instanceState">
        /// The instance state. 
        /// </param>
        public static void Exists(TrackingRecordsList records, WorkflowInstanceRecordState instanceState)
        {
            Exists(records, instanceState, 0, null);
        }

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="instanceState">
        /// The instance state. 
        /// </param>
        /// <param name="startIndex">
        /// The starting index
        /// </param>
        public static void Exists(
            TrackingRecordsList records, WorkflowInstanceRecordState instanceState, long startIndex)
        {
            Exists(records, instanceState, startIndex, null);
        }

        /// <summary>
        /// Asserts that a WorkflowInstanceRecordState exists
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="instanceState">
        /// The instance state. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        public static void Exists(
            TrackingRecordsList records, WorkflowInstanceRecordState instanceState, string message)
        {
            Exists(records, instanceState, 0, message);
        }

        /// <summary>
        /// Asserts that a WorkflowInstanceRecordState exists
        /// </summary>
        /// <param name="records">
        /// The records. 
        /// </param>
        /// <param name="instanceState">
        /// The instance state. 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        public static void Exists(
            TrackingRecordsList records, 
            WorkflowInstanceRecordState instanceState, 
            long startIndex, 
            string message)
        {
            if (!records.Exists(instanceState, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.WorkflowInstanceStateNotFound, message, "Exists", instanceState));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="list">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
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
        public static void ExistsArgMatch(
            TrackingRecordsList list, 
            string displayName, 
            ActivityInstanceState state, 
            string argumentName, 
            string pattern, 
            string message = null)
        {
            ExistsArgMatch(list, displayName, state, argumentName, pattern, 0, message);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="list">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
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
        /// The zero-based starting index for the search 
        /// </param>
        /// <param name="message">
        /// The fail message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void ExistsArgMatch(
            TrackingRecordsList list, 
            string displayName, 
            ActivityInstanceState state, 
            string argumentName, 
            string pattern, 
            long startIndex, 
            string message = null)
        {
            if (!list.ExistsArgMatch(displayName, state, argumentName, pattern, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordDoesNotMatchArg, 
                        message, 
                        "Exists", 
                        displayName, 
                        state, 
                        argumentName, 
                        pattern, 
                        startIndex));
            }
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value
        /// </typeparam>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        public static void ExistsArgValue<T>(
            TrackingRecordsList records, 
            string displayName, 
            ActivityInstanceState state, 
            string argumentName, 
            T value, 
            string message = null)
        {
            ExistsArgValue(records, displayName, state, argumentName, value, 0, message);
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value
        /// </typeparam>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        public static void ExistsArgValue<T>(
            TrackingRecordsList records, 
            string displayName, 
            ActivityInstanceState state, 
            string argumentName, 
            T value, 
            long startIndex, 
            string message = null)
        {
            ExistsArgMatch(records, displayName, state, argumentName, value.ToString(), startIndex, message);
        }

        /// <summary>
        /// Verifies that a tracking record exists at the given index
        /// </summary>
        /// <param name="list">
        /// The tracking record list. 
        /// </param>
        /// <param name="index">
        /// The index. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <param name="message">
        /// The failure message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void ExistsAt(
            TrackingRecordsList list, int index, string displayName, ActivityInstanceState state, string message = null)
        {
            if (!list.ExistsAt(index, displayName, state))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordDoesNotExistAt, message, "ExistsAt", displayName, state, index));
            }
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="beforeActivityName">
        /// The name of the before activity 
        /// </param>
        /// <param name="afterActivityName">
        /// The name of the after activity 
        /// </param>
        /// <param name="state">
        /// The ActivityInstanceState 
        /// </param>
        /// <param name="message">
        /// The assertion failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBefore(
            TrackingRecordsList records, 
            string beforeActivityName, 
            string afterActivityName, 
            ActivityInstanceState state, 
            string message = null)
        {
            ExistsBefore(records, beforeActivityName, afterActivityName, state, 0, message);
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="beforeActivityName">
        /// The name of the before activity 
        /// </param>
        /// <param name="beforeState">
        /// The activity state of the before record 
        /// </param>
        /// <param name="afterActivityName">
        /// The name of the after activity 
        /// </param>
        /// <param name="afterState">
        /// The ActivityInstanceState 
        /// </param>
        /// <param name="message">
        /// The assertion failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBefore(
            TrackingRecordsList records, 
            string beforeActivityName, 
            ActivityInstanceState beforeState, 
            string afterActivityName, 
            ActivityInstanceState afterState, 
            string message = null)
        {
            ExistsBefore(records, beforeActivityName, beforeState, afterActivityName, afterState, 0, message);
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="beforeActivityName">
        /// The name of the before activity 
        /// </param>
        /// <param name="beforeState">
        /// The activity state of the before record 
        /// </param>
        /// <param name="afterActivityName">
        /// The name of the after activity 
        /// </param>
        /// <param name="afterState">
        /// The ActivityInstanceState 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <param name="message">
        /// The assertion failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBefore(
            TrackingRecordsList records, 
            string beforeActivityName, 
            ActivityInstanceState beforeState, 
            string afterActivityName, 
            ActivityInstanceState afterState, 
            long startIndex, 
            string message = null)
        {
            var beforeIndex = records.FindIndex(beforeActivityName, beforeState, startIndex);

            if (beforeIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", beforeActivityName, beforeState));
            }

            var afterIndex = records.FindIndex(afterActivityName, afterState, startIndex);

            if (afterIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", afterActivityName, afterState));
            }

            if (beforeIndex >= afterIndex)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotBeforeState, 
                        message, 
                        "ExistsBefore", 
                        beforeActivityName, 
                        afterState, 
                        beforeIndex, 
                        afterActivityName, 
                        afterState, 
                        afterIndex));
            }
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
        /// <param name="beforeActivityName">
        /// The name of the before activity 
        /// </param>
        /// <param name="afterActivityName">
        /// The name of the after activity 
        /// </param>
        /// <param name="state">
        /// The ActivityInstanceState 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <param name="message">
        /// The assertion failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBefore(
            TrackingRecordsList records, 
            string beforeActivityName, 
            string afterActivityName, 
            ActivityInstanceState state, 
            long startIndex, 
            string message = null)
        {
            var beforeIndex = records.FindIndex(beforeActivityName, state, startIndex);
            if (beforeIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", beforeActivityName, state));
            }

            var afterIndex = records.FindIndex(afterActivityName, state, startIndex);

            if (afterIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", afterActivityName, state));
            }

            if (beforeIndex >= afterIndex)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotBefore, 
                        message, 
                        "ExistsBefore", 
                        beforeActivityName, 
                        state, 
                        beforeIndex, 
                        afterActivityName, 
                        afterIndex));
            }
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
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
        /// The assert failed
        /// </exception>
        public static void ExistsBeforeArgMatch(
            TrackingRecordsList records, 
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
            ExistsBeforeArgMatch(
                records, 
                beforeDisplayName, 
                beforeState, 
                beforeArgumentName, 
                beforePattern, 
                afterDisplayName, 
                afterState, 
                afterArgumentName, 
                afterPattern, 
                0, 
                message);
        }

        /// <summary>
        /// Asserts that a tracking record exists prior to another tracking record
        /// </summary>
        /// <param name="records">
        /// The tracking records 
        /// </param>
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
        /// The assert failed
        /// </exception>
        public static void ExistsBeforeArgMatch(
            TrackingRecordsList records, 
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
            var beforeIndex = records.FindIndexArgMatch(
                beforeDisplayName, beforeState, beforeArgumentName, beforePattern, startIndex);

            if (beforeIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", beforeDisplayName, beforeState));
            }

            var afterIndex = records.FindIndexArgMatch(
                afterDisplayName, afterState, afterArgumentName, afterPattern, startIndex);

            if (afterIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", afterDisplayName, afterState));
            }

            if (beforeIndex >= afterIndex)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotBeforeState, 
                        message, 
                        "ExistsBefore", 
                        beforeDisplayName, 
                        afterState, 
                        beforeIndex, 
                        afterDisplayName, 
                        afterState, 
                        afterIndex));
            }
        }

        /// <summary>
        /// Verifies that an ActivityStateRecord exists before another
        /// </summary>
        /// <param name="records">
        /// The record list 
        /// </param>
        /// <param name="beforestring">
        /// The id of the before activity 
        /// </param>
        /// <param name="afterstring">
        /// The id of the after activity 
        /// </param>
        /// <param name="state">
        /// The state of the activity 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBeforeId(
            TrackingRecordsList records, 
            string beforestring, 
            string afterstring, 
            ActivityInstanceState state, 
            string message = null)
        {
            ExistsBeforeId(records, beforestring, afterstring, state, 0, message);
        }

        /// <summary>
        /// Verifies that an ActivityStateRecord exists before another
        /// </summary>
        /// <param name="records">
        /// The record list 
        /// </param>
        /// <param name="beforestring">
        /// The id of the before activity 
        /// </param>
        /// <param name="afterstring">
        /// The id of the after activity 
        /// </param>
        /// <param name="state">
        /// The state of the activity 
        /// </param>
        /// <param name="startIndex">
        /// The zero-based starting index of the search. 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ExistsBeforeId(
            TrackingRecordsList records, 
            string beforestring, 
            string afterstring, 
            ActivityInstanceState state, 
            long startIndex, 
            string message = null)
        {
            var beforeIndex = records.FindIndexId(beforestring, state, startIndex);
            if (beforeIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", beforestring, state));
            }

            var afterIndex = records.FindIndexId(afterstring, state, startIndex);

            if (afterIndex == -1)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotFound, message, "ExistsBefore", afterstring, state));
            }

            if (beforeIndex >= afterIndex)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.TrackingRecordNotBefore, 
                        message, 
                        "ExistsBefore", 
                        beforestring, 
                        state, 
                        beforeIndex, 
                        afterstring, 
                        afterIndex));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="displayName">
        /// The activity display name. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <param name="count">
        /// The count of records to match. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void ExistsCount(
            TrackingRecordsList records, string displayName, ActivityInstanceState state, int count)
        {
            ExistsCount(records, displayName, state, count, null);
        }

        /// <summary>
        /// Verifies that a count of matching tracking records exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsCount(
            TrackingRecordsList records, string displayName, ActivityInstanceState state, int count, string message)
        {
            if (!records.ExistsCount(displayName, state, count))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordDoesNotExist, message, "Exists", displayName, state));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists for the given predicate
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="predicate">
        /// The predicate. 
        /// </param>
        /// <param name="count">
        /// The count. 
        /// </param>
        /// <typeparam name="TRecord">
        /// The type of tracking record you are looking for 
        /// </typeparam>
        /// <exception cref="WorkflowAssertFailedException">
        /// The record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void ExistsCount<TRecord>(TrackingRecordsList records, Predicate<TRecord> predicate, int count)
            where TRecord : TrackingRecord
        {
            ExistsCount(records, predicate, count, null);
        }

        /// <summary>
        /// Verifies that a tracking record exists for the given predicate
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsCount<TRecord>(
            TrackingRecordsList records, Predicate<TRecord> predicate, int count, string message)
            where TRecord : TrackingRecord
        {
            if (!records.ExistsCount(predicate, count))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.TrackingRecordDoesNotExist, message, "Exists"));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsId(
            TrackingRecordsList records, string id, ActivityInstanceState state, string message = null)
        {
            ExistsId(records, id, state, 0, message);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsId(
            TrackingRecordsList records, string id, ActivityInstanceState state, long startIndex, string message = null)
        {
            if (!records.ExistsId(id, state, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordIdDoesNotExist, message, "ExistsId", id, state, startIndex));
            }
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsIdArgMatch(
            TrackingRecordsList records, 
            string id, 
            ActivityInstanceState state, 
            string argumentName, 
            string pattern, 
            string message = null)
        {
            ExistsIdArgMatch(records, id, state, argumentName, pattern, 0, message);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsIdArgMatch(
            TrackingRecordsList records, 
            string id, 
            ActivityInstanceState state, 
            string argumentName, 
            string pattern, 
            long startIndex, 
            string message = null)
        {
            if (!records.ExistsIdArgMatch(id, state, argumentName, pattern, startIndex))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ActivityStateRecordDoesNotMatchArg, 
                        message, 
                        "ExistsId", 
                        id, 
                        state, 
                        argumentName, 
                        pattern, 
                        startIndex));
            }
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value 
        /// </typeparam>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        public static void ExistsIdArgValue<T>(
            TrackingRecordsList records, 
            string id, 
            ActivityInstanceState state, 
            string argumentName, 
            T value, 
            string message = null)
        {
            ExistsIdArgValue(records, id, state, argumentName, value, 0, message);
        }

        /// <summary>
        /// Asserts the an ActivityStateRecord with the given argument name and a value matching the value pattern exists
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value 
        /// </typeparam>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        public static void ExistsIdArgValue<T>(
            TrackingRecordsList records, 
            string id, 
            ActivityInstanceState state, 
            string argumentName, 
            T value, 
            long startIndex, 
            string message = null)
        {
            ExistsIdArgMatch(records, id, state, argumentName, value.ToString(), startIndex, message);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
        /// <param name="id">
        /// The id of the activity. 
        /// </param>
        /// <param name="state">
        /// The activity state. 
        /// </param>
        /// <param name="count">
        /// The count of records to match. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// Thrown if the record does not exist
        /// </exception>
        [DebuggerStepThrough]
        public static void ExistsIdCount(TrackingRecordsList records, string id, ActivityInstanceState state, int count)
        {
            ExistsIdCount(records, id, state, count, null);
        }

        /// <summary>
        /// Verifies that a tracking record exists
        /// </summary>
        /// <param name="records">
        /// The tracking record list. 
        /// </param>
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
        public static void ExistsIdCount(
            TrackingRecordsList records, string id, ActivityInstanceState state, int count, string message)
        {
            if (!records.ExistsIdCount(id, state, count))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.stringStateRecordDoesNotExistCount, 
                        message, 
                        "ExistsId", 
                        id, 
                        state, 
                        count, 
                        records.FindAllId(id, state).Count()));
            }
        }

        /// <summary>
        /// Asserts that a record was scheduled
        /// </summary>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        public static void Scheduled(
            TrackingRecordsList records, string parentDisplayName, string childDisplayName, string message = null)
        {
            Scheduled(records, parentDisplayName, childDisplayName, 0, message);
        }

        /// <summary>
        /// Asserts that a record was scheduled
        /// </summary>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
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
        /// The assert failed
        /// </exception>
        [DebuggerStepThrough]
        public static void Scheduled(
            TrackingRecordsList records, 
            string parentDisplayName, 
            string childDisplayName, 
            int startRecord, 
            string message = null)
        {
            if (records.FindScheduled(parentDisplayName, childDisplayName, startRecord) == null)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ScheduledRecordNotFound, message, "Scheduled", parentDisplayName, childDisplayName));
            }
        }

        /// <summary>
        /// Asserts that a record was scheduled
        /// </summary>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
        /// <param name="parentDisplayNameId">
        /// The parent activity 
        /// </param>
        /// <param name="childDisplayNameId">
        /// The child activity 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ScheduledId(
            TrackingRecordsList records, string parentDisplayNameId, string childDisplayNameId, string message = null)
        {
            ScheduledId(records, parentDisplayNameId, childDisplayNameId, 0, message);
        }

        /// <summary>
        /// Asserts that a record was scheduled
        /// </summary>
        /// <param name="records">
        /// The tracking records collection 
        /// </param>
        /// <param name="parentDisplayNameId">
        /// The parent activity 
        /// </param>
        /// <param name="childDisplayNameId">
        /// The child activity 
        /// </param>
        /// <param name="startIndex">
        /// The staring record number of the search 
        /// </param>
        /// <param name="message">
        /// The failure message 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        public static void ScheduledId(
            TrackingRecordsList records, 
            string parentDisplayNameId, 
            string childDisplayNameId, 
            long startIndex, 
            string message = null)
        {
            if (records.FindScheduledId(parentDisplayNameId, childDisplayNameId, startIndex) == null)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.ScheduledIdRecordNotFound, 
                        message, 
                        "ScheduledId", 
                        parentDisplayNameId, 
                        childDisplayNameId));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats exception text
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="args">
        /// optional args. 
        /// </param>
        /// <returns>
        /// The formatted exception text. 
        /// </returns>
        private static string FormatExceptionText(string format, string message, params object[] args)
        {
            var sb = new StringBuilder(string.Format(format, args));

            if (!string.IsNullOrWhiteSpace(message))
            {
                sb.AppendFormat(" {0}", message);
            }

            return sb.ToString();
        }

        #endregion
    }
}