// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertOutArgument.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;

    using Microsoft.Activities.UnitTesting.Properties;

    /// <summary>
    ///   Provides a simple way to assert an out argument
    /// </summary>
    public static class AssertOutArgument
    {
        #region Public Methods and Operators

        /// <summary>
        /// Verifies an OutArgument returned in the output collection from a Workflow
        /// </summary>
        /// <typeparam name="T">
        /// The expectedType of the OutArgument 
        /// </typeparam>
        /// <param name="output">
        /// The output collection 
        /// </param>
        /// <param name="name">
        /// The name of the OutArgument 
        /// </param>
        /// <param name="expected">
        /// The expected value 
        /// </param>
        /// <param name="message">
        /// A message to display if the assertion fails. This message can be seen in the unit test results. 
        /// </param>
        [DebuggerStepThrough]
        public static void AreEqual<T>(
            IDictionary<string, object> output, string name, T expected, string message = null)
        {
            var actual = VerifyOutArgument<T>(output, name, "AreEqual");

            if (!object.Equals(actual, expected))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgAreEqualFailMsg, message, expected, actual));
            }
        }

        /// <summary>
        /// Verifies an OutArgument returned in the output collection from a Workflow
        /// </summary>
        /// <typeparam name="T">
        /// The expectedType of the OutArgument 
        /// </typeparam>
        /// <param name="output">
        /// The output collection 
        /// </param>
        /// <param name="name">
        /// The name of the OutArgument 
        /// </param>
        /// <param name="expected">
        /// The expected value 
        /// </param>
        /// <param name="message">
        /// A message to display if the assertion fails. This message can be seen in the unit test results. 
        /// </param>
        [DebuggerStepThrough]
        public static void AreNotEqual<T>(
            IDictionary<string, object> output, string name, T expected, string message = null)
        {
            var actual = VerifyOutArgument<T>(output, name, "AreNotEqual");

            if (object.Equals(actual, expected))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgAreNotEqualFailMsg, message, expected, actual));
            }
        }

        /// <summary>
        /// Verifies that an out argument is false
        /// </summary>
        /// <param name="output">
        /// The output dictionary. 
        /// </param>
        /// <param name="name">
        /// The name of the argument. 
        /// </param>
        /// <param name="message">
        /// The fail message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The out argument is true
        /// </exception>
        [DebuggerStepThrough]
        public static void IsFalse(IDictionary<string, object> output, string name, string message = null)
        {
            var actual = VerifyOutArgument<bool>(output, name, "IsFalse");

            if (actual)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgBoolFailMsg, message, "IsFalse"));
            }
        }

        /// <summary>
        /// The is instance of expectedType.
        /// </summary>
        /// <param name="output">
        /// The output. 
        /// </param>
        /// <param name="name">
        /// The argument name. 
        /// </param>
        /// <param name="expectedType">
        /// The expected expectedType. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        [DebuggerStepThrough]
        public static void IsInstanceOfType(IDictionary<string, object> output, string name, Type expectedType)
        {
            IsInstanceOfType(output, name, expectedType, null);
        }

        /// <summary>
        /// The is instance of expectedType.
        /// </summary>
        /// <param name="output">
        /// The output. 
        /// </param>
        /// <param name="name">
        /// The argument name. 
        /// </param>
        /// <param name="expectedType">
        /// The expected expectedType. 
        /// </param>
        /// <param name="message">
        /// The fail message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        [DebuggerStepThrough]
        public static void IsInstanceOfType(
            IDictionary<string, object> output, string name, Type expectedType, string message)
        {
            Contract.Requires(expectedType != null);
            if (expectedType == null)
            {
                throw new ArgumentNullException("expectedType");
            }

            var actual = VerifyOutArgument<object>(output, name, "IsInstanceOfType");

            if (actual == null || !expectedType.IsInstanceOfType(actual))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(
                        Resources.IsInstanceOfTypeFailed, 
                        message, 
                        expectedType, 
                        actual != null ? actual.GetType() : null));
            }
        }

        /// <summary>
        /// Verifies that an OutArgument exists and is not null
        /// </summary>
        /// <param name="output">
        /// The output collection 
        /// </param>
        /// <param name="name">
        /// The name of the OutArgument 
        /// </param>
        /// <param name="message">
        /// A message to display if the assertion fails. This message can be seen in the unit test results. 
        /// </param>
        [DebuggerStepThrough]
        public static void IsNotNull(IDictionary<string, object> output, string name, string message = null)
        {
            var actual = VerifyOutArgument<object>(output, name, "IsNotNull");

            if (actual == null)
            {
                throw new WorkflowAssertFailedException(FormatExceptionText(Resources.OutArgIsNotNull, message));
            }
        }

        /// <summary>
        /// Verifies that an OutArgument exists and is null
        /// </summary>
        /// <param name="output">
        /// The output collection 
        /// </param>
        /// <param name="name">
        /// The name of the OutArgument 
        /// </param>
        /// <param name="message">
        /// A message to display if the assertion fails. This message can be seen in the unit test results. 
        /// </param>
        [DebuggerStepThrough]
        public static void IsNull(IDictionary<string, object> output, string name, string message = null)
        {
            var actual = VerifyOutArgument<object>(output, name, "IsNull");

            if (actual != null)
            {
                throw new WorkflowAssertFailedException(FormatExceptionText(Resources.OutArgIsNull, message));
            }
        }

        /// <summary>
        /// Verifies that an out argument is true
        /// </summary>
        /// <param name="output">
        /// The output dictionary. 
        /// </param>
        /// <param name="name">
        /// The name of the argument. 
        /// </param>
        /// <param name="message">
        /// The fail message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The out argument is not true
        /// </exception>
        [DebuggerStepThrough]
        public static void IsTrue(IDictionary<string, object> output, string name, string message = null)
        {
            var actual = VerifyOutArgument<bool>(output, name, "IsTrue");

            if (!actual)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgBoolFailMsg, message, "IsTrue"));
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
        /// optiona exception args. 
        /// </param>
        /// <returns>
        /// the formatted exception text 
        /// </returns>
        [DebuggerStepThrough]
        private static string FormatExceptionText(string format, string message, params object[] args)
        {
            var sb = new StringBuilder(string.Format(format, args));

            if (!string.IsNullOrWhiteSpace(message))
            {
                sb.AppendFormat(" {0}", message);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Verifies that the argument is not null
        /// </summary>
        /// <param name="argument">
        /// The argument. 
        /// </param>
        /// <param name="assertName">
        /// The assert name. 
        /// </param>
        /// <param name="internalMessage">
        /// The internal message. 
        /// </param>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The assert failed
        /// </exception>
        [DebuggerStepThrough]
        // ReSharper disable UnusedParameter.Local
        private static void VerifyNotNull(object argument, string assertName, string internalMessage, string message)
        {
            // ReSharper restore UnusedParameter.Local
            if (argument == null)
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgIsNotNullInternal, message, assertName, internalMessage));
            }
        }

        /// <summary>
        /// Verifies the out argument exists and is of the correct expectedType
        /// </summary>
        /// <param name="output">
        /// The output dictionary. 
        /// </param>
        /// <param name="name">
        /// The name of the argument. 
        /// </param>
        /// <param name="assertName">
        /// The name of the assert method. 
        /// </param>
        /// <param name="message">
        /// The fail message message. 
        /// </param>
        /// <typeparam name="T">
        /// The expectedType of the argument 
        /// </typeparam>
        /// <returns>
        /// The out argument 
        /// </returns>
        [DebuggerStepThrough]
        private static T VerifyOutArgument<T>(
            IDictionary<string, object> output, string name, string assertName, string message = null)
        {

            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            Contract.Requires(!string.IsNullOrWhiteSpace(assertName));
            if (string.IsNullOrWhiteSpace(assertName))
            {
                throw new ArgumentNullException("assertName");
            }

            // Verify that the output dictionary is not null
            VerifyNotNull(output, assertName, Resources.OutputIsNull, message);

            // Verify that the output dictionary contains the argument
            VerifyOutputContainsArgument(output, name, assertName, message);

            // Get the object from the output
            var actualObject = output[name];

            // Verify that it is the correct expectedType
            VerifyOutArgumentType<T>(name, assertName, message, actualObject);

            // Cast and return it
            return actualObject == null ? default(T) : (T)actualObject;
        }

        /// <summary>
        /// Verifies that the argument is of the correct expectedType
        /// </summary>
        /// <param name="name">
        /// The name of the argument 
        /// </param>
        /// <param name="assertName">
        /// The name of the assert 
        /// </param>
        /// <param name="message">
        /// The failure message. 
        /// </param>
        /// <param name="actualObject">
        /// The actual object to verify. 
        /// </param>
        /// <typeparam name="T">
        /// The expected expectedType 
        /// </typeparam>
        /// <exception cref="WorkflowAssertFailedException">
        /// The argument is not of the correct expectedType
        /// </exception>
        [DebuggerStepThrough]
        private static void VerifyOutArgumentType<T>(
            string name, string assertName, string message, object actualObject)
        {
            if (actualObject != null)
            {
                var expectedType = typeof(T);

                if (!expectedType.IsInstanceOfType(actualObject))
                {
                    throw new WorkflowAssertFailedException(
                        FormatExceptionText(
                            Resources.OutArgWrongType, message, assertName, name, expectedType, actualObject.GetType()));
                }
            }
        }

        /// <summary>
        /// Verifies that the output contains an argument
        /// </summary>
        /// <param name="output">
        /// The output dictionary. 
        /// </param>
        /// <param name="name">
        /// The argument name. 
        /// </param>
        /// <param name="assertName">
        /// The assert name. 
        /// </param>
        /// <param name="message">
        /// The failure message. 
        /// </param>
        /// <exception cref="WorkflowAssertFailedException">
        /// The output does not contain the argument
        /// </exception>
        [DebuggerStepThrough]
        private static void VerifyOutputContainsArgument(
            IDictionary<string, object> output, string name, string assertName, string message)
        {
            Contract.Requires(output != null);
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (!output.ContainsKey(name))
            {
                throw new WorkflowAssertFailedException(
                    FormatExceptionText(Resources.OutArgDoesNotContain, message, assertName, name));
            }
        }

        #endregion
    }
}