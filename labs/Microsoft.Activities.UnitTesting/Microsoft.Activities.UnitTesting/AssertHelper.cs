// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Activities.UnitTesting.Properties;

    /// <summary>
    ///   The assert helper.
    /// </summary>
    public static class AssertHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Verifies that all elements in two collections are equal
        /// </summary>
        /// <param name="expectedCollection">
        /// The expected collection. 
        /// </param>
        /// <param name="actualCollection">
        /// The actual collection. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the elements 
        /// </typeparam>
        /// <exception cref="WorkflowAssertFailedException">
        /// The collections are not equal
        /// </exception>
        public static void AreEqual<T>(
            IEnumerable<T> expectedCollection, IEnumerable<T> actualCollection, string failMessage = null)
        {
            if (expectedCollection == null && actualCollection != null)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        "AssertHelper.AreEqual failed. Expected is null. Actual is not null.", failMessage));
            }

            if (expectedCollection != null && actualCollection == null)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        "AssertHelper.AreEqual failed. Expected is not null. Actual is null.", failMessage));
            }

            var expected = expectedCollection.ToList();
            var actual = actualCollection.ToList();

            if (expected.Count != actual.Count)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        "AssertHelper.AreEqual failed. Expected count is <{0}>. Actual count is <{1}>", 
                        failMessage, 
                        expected.Count, 
                        actual.Count));
            }

            for (var i = 0; i < expected.Count; i++)
            {
                if (!Equals(expected[i], actual[i]))
                {
                    throw new WorkflowAssertFailedException(
                        FormatExceptionText(
                            "AssertHelper.AreEqual failed. Expected[{0}] is <{1}>. Actual[{2}]is <{3}>", 
                            failMessage, 
                            expected.Count, 
                            expected[i], 
                            actual.Count, 
                            actual[i]));
                }
            }
        }

        /// <summary>
        /// Asserts that a collection(of T) contains all values passed in args
        /// </summary>
        /// <param name="collection">
        /// The colllection to test 
        /// </param>
        /// <param name="args">
        /// The values to test for 
        /// </param>
        /// <typeparam name="T">
        /// The type of the collection 
        /// </typeparam>
        [DebuggerStepThrough]
        public static void Contains<T>(IEnumerable<T> collection, params T[] args)
        {
            var badValue = default(T);

            if (args.Any(
                value =>
                    {
                        // If the collection does not contain the value
                        if (!collection.Contains(value))
                        {
                            // Save the bad value
                            badValue = value;
                            return true;
                        }

                        return false;
                    }))
            {
                throw new WorkflowAssertFailedException(
                    string.Format(Resources.ContainsValueNotFound, badValue, ListHelper.ToDelimitedList(collection)));
            }
        }

        /// <summary>
        /// Access a property value
        /// </summary>
        /// <typeparam name="T">
        /// The type of the roperty 
        /// </typeparam>
        /// <param name="value">
        /// The property value 
        /// </param>
        /// <remarks>
        /// Use with Throws{T} to verify that a property access throws
        /// </remarks>
        /// <returns>
        /// The value. 
        /// </returns>
        public static T GetProperty<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Determines if a collection of values occurs in exactly the same order as the expected collection
        /// </summary>
        /// <param name="actual">
        /// The actual. 
        /// </param>
        /// <param name="expected">
        /// The expected. 
        /// </param>
        /// <typeparam name="T">
        /// The type of value 
        /// </typeparam>
        [DebuggerStepThrough]
        public static void OccursInOrder<T>(IEnumerable<T> actual, params T[] expected)
        {
            Contract.Requires(actual != null);
            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            Contract.Requires(expected != null && expected.Length != 0);
            if (expected == null || expected.Length == 0)
            {
                throw new ArgumentNullException("expected");
            }

            OccursInOrder(actual.ToString(), actual, expected);
        }

        /// <summary>
        /// Determines if a collection of values occurs in exactly the same order as the expected collection
        /// </summary>
        /// <param name="collectionName">
        /// The collection Name. 
        /// </param>
        /// <param name="actual">
        /// The actual. 
        /// </param>
        /// <param name="expected">
        /// The expected. 
        /// </param>
        /// <typeparam name="T">
        /// The type of value 
        /// </typeparam>
        [DebuggerStepThrough]
        public static void OccursInOrder<T>(string collectionName, IEnumerable<T> actual, params T[] expected)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(collectionName));
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentNullException("collectionName");
            }

            Contract.Requires(actual != null);
            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            Contract.Requires(expected != null && expected.Length != 0);
            if (expected == null || expected.Length == 0)
            {
                throw new ArgumentNullException("expected");
            }

            var actualArray = actual.ToArray();

            // Verify that there are at least count records
            if (expected.Count() > actualArray.Count())
            {
                throw new WorkflowAssertFailedException(
                    string.Format(
                        Resources.OccursInOrderExpectedCollectionLargerThanActual, 
                        collectionName, 
                        ListHelper.ToDelimitedList(expected), 
                        ListHelper.ToDelimitedList(actualArray)));
            }

            // Verify that the items occured in the expected order
            for (var i = 0; i < expected.Count(); i++)
            {
                if (!expected[i].Equals(actualArray.ElementAt(i)))
                {
                    throw new WorkflowAssertFailedException(
                        string.Format(
                            Resources.OccursInOrderExpectedNotActual, 
                            expected[i], 
                            actualArray.ElementAt(i), 
                            collectionName, 
                            i, 
                            ListHelper.ToDelimitedList(expected), 
                            ListHelper.ToDelimitedList(actualArray)));
                }
            }
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Action action) where TException : Exception
        {
            return Throws<TException>(action, null, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Action action, string exceptionMessage) where TException : Exception
        {
            return Throws<TException>(action, exceptionMessage, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Action action, string exceptionMessage, string failMessage)
            where TException : Exception
        {
            return Throws<TException>(action, exceptionMessage, null, failMessage);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Action action, Type expectedInnerExceptionType)
            where TException : Exception
        {
            return Throws<TException>(action, expectedInnerExceptionType, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Action action, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            return Throws<TException>(action, null, expectedInnerExceptionType, failMessage);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(
            Action action, string exceptionMessage, Type expectedInnerExceptionType) where TException : Exception
        {
            return Throws<TException>(action, exceptionMessage, expectedInnerExceptionType, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="func">
        /// The func. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException, TResult>(Func<TResult> func) where TException : Exception
        {
            return Throws<TException, TResult>(func, null, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="func">
        /// The func. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException, TResult>(Func<TResult> func, string exceptionMessage)
            where TException : Exception
        {
            return Throws<TException, TResult>(func, exceptionMessage, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="func">
        /// The func. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException, TResult>(
            Func<TResult> func, string exceptionMessage, Type expectedInnerExceptionType) where TException : Exception
        {
            return Throws<TException, TResult>(func, exceptionMessage, expectedInnerExceptionType, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="func">
        /// The func. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the result 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException, TResult>(
            Func<TResult> func, string exceptionMessage, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            var didNotThrow = false;
            TException caughtException;

            try
            {
                func();

                didNotThrow = true;
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.ActionDidNotThrow, failMessage, typeof(TException)));
            }
            catch (TException exception)
            {
                // If the type is WorkflowAssertFailedException you have to throw it again
                // Otherwise this catch will mask the error
                if (typeof(TException) == typeof(WorkflowAssertFailedException))
                {
                    if (didNotThrow)
                    {
                        throw;
                    }
                }

                caughtException = exception;
            }

            AssertCaughtException(caughtException, exceptionMessage, expectedInnerExceptionType, failMessage);
            return caughtException;
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the action
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(
            Action action, string exceptionMessage, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            var didNotThrow = false;
            TException caughtException;

            try
            {
                action();

                didNotThrow = true;
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.ActionDidNotThrow, failMessage, typeof(TException)));
            }
            catch (TException exception)
            {
                // If the type is WorkflowAssertFailedException you have to throw it again
                // Otherwise this catch will mask the error
                if (typeof(TException) == typeof(WorkflowAssertFailedException))
                {
                    if (didNotThrow)
                    {
                        throw;
                    }
                }

                caughtException = exception;
            }

            AssertCaughtException(caughtException, exceptionMessage, expectedInnerExceptionType, failMessage);
            return caughtException;
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task) where TException : Exception
        {
            return Throws<TException>(task, null, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task, string exceptionMessage) where TException : Exception
        {
            return Throws<TException>(task, exceptionMessage, null, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task, Type expectedInnerExceptionType)
            where TException : Exception
        {
            return Throws<TException>(task, null, expectedInnerExceptionType);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail Message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            return Throws<TException>(task, null, expectedInnerExceptionType, failMessage);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task, string exceptionMessage, Type expectedInnerExceptionType)
            where TException : Exception
        {
            return Throws<TException>(task, exceptionMessage, expectedInnerExceptionType, null);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
        [DebuggerStepThrough]
        public static TException Throws<TException>(Task task, string exceptionMessage, string failMessage)
            where TException : Exception
        {
            return Throws<TException>(task, exceptionMessage, null, failMessage);
        }

        /// <summary>
        /// Verifies that an exception of the correct type is thrown by the task
        /// </summary>
        /// <param name="task">
        /// The task to verify. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of exception that is expected 
        /// </typeparam>
        /// <returns>
        /// The caught exception if it passes the asserts 
        /// </returns>
         [DebuggerStepThrough]
        public static TException Throws<TException>(
            Task task, string exceptionMessage, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            var didNotThrow = false;
            TException caughtException;
            try
            {
                // Wait for the task to complete
                task.Wait();

                didNotThrow = true;
                throw new WorkflowAssertFailedException(
                    string.Format(Resources.ActionDidNotThrow, typeof(TException), failMessage));
            }
            catch (AggregateException aggException)
            {
                if (typeof(TException) == typeof(WorkflowAssertFailedException))
                {
                    if (didNotThrow)
                    {
                        throw;
                    }
                }

                if (aggException.InnerException.GetType() != typeof(TException))
                {
                    throw new WorkflowAssertFailedException(
                        FormatExceptionText(
                            Resources.AggregateExceptionDoesNotContainExpectedException, 
                            failMessage,
                            typeof(TException).Name,
                            aggException),
                        aggException);
                }

                caughtException = (TException)aggException.InnerException;
            }

            // Verify that the exception that was caught contains what we expected
            AssertCaughtException(caughtException, exceptionMessage, expectedInnerExceptionType, failMessage);
            return caughtException;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asserts that a condition is true
        /// </summary>
        /// <param name="condition">
        /// The condition. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The condition was fale
        /// </exception>
        internal static void IsTrue(Func<bool> condition, string message, params object[] args)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (!condition())
            {
                throw new WorkflowAssertFailedException(message, args);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="condition">
        /// The condition. 
        /// </param>
        /// <param name="arg1">
        /// The arg 1. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="WorkflowAssertFailedException">
        /// </exception>
        internal static void IsTrue<T>(Func<T, bool> condition, T arg1, string message, params object[] args)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            if (!condition(arg1))
            {
                throw new WorkflowAssertFailedException(message, args);
            }
        }

        /// <summary>
        /// Checks the caught exception
        /// </summary>
        /// <param name="caughtException">
        /// The exception. 
        /// </param>
        /// <param name="exceptionMessage">
        /// The exception message. 
        /// </param>
        /// <param name="expectedInnerExceptionType">
        /// The expected inner exception type. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <typeparam name="TException">
        /// The type of the exception 
        /// </typeparam>
        [DebuggerStepThrough]
        private static void AssertCaughtException<TException>(
            TException caughtException, string exceptionMessage, Type expectedInnerExceptionType, string failMessage)
            where TException : Exception
        {
            if (caughtException == null)
            {
                throw new WorkflowAssertFailedException(failMessage);
            }

            // If there is an exception message to match, check it
            if (!string.IsNullOrWhiteSpace(exceptionMessage))
            {
                if (exceptionMessage != caughtException.Message)
                {
                    throw new WorkflowAssertFailedException(
                        FormatExceptionText(
                            Resources.ExpectedExceptionMessageMismatch, 
                            failMessage, 
                            exceptionMessage, 
                            caughtException.Message));
                }
            }

            // If there is an inner exception type to match, check it
            if (expectedInnerExceptionType != null
                && !expectedInnerExceptionType.IsInstanceOfType(caughtException.InnerException))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.AssertCaughtExceptionInnerExceptionInstanceOfTypeFailed, 
                        failMessage, 
                        expectedInnerExceptionType, 
                        caughtException.InnerException.GetType()));
            }
        }

        /// <summary>
        /// Formats exception text
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="failMessage">
        /// The fail message. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <returns>
        /// The formatted exception text. 
        /// </returns>
        private static string FormatExceptionText(string format, string failMessage, params object[] args)
        {
            var sb = new StringBuilder(string.Format(format, args));

            if (!string.IsNullOrWhiteSpace(failMessage))
            {
                sb.AppendFormat(" {0}", failMessage);
            }

            return sb.ToString();
        }

        #endregion
    }
}