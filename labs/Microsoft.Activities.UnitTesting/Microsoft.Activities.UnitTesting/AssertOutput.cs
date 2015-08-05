// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertOutput.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Asserts aspects of an output dictionary
    /// </summary>
    public class AssertOutput
    {
        #region Constants and Fields

        /// <summary>
        ///   The output dictionary
        /// </summary>
        private readonly IDictionary<string, object> output;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertOutput"/> class.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        internal AssertOutput(IDictionary<string, object> output)
        {
            this.output = output;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies the arguments are equal
        /// </summary>
        /// <param name="name">
        /// The output name.
        /// </param>
        /// <param name="expected">
        /// The expected value.
        /// </param>
        /// <param name="message">
        /// Failure message.
        /// </param>
        /// <typeparam name="T">
        /// The expectedType of the out argument
        /// </typeparam>
        [DebuggerStepThrough]
        public void AreEqual<T>(string name, T expected, string message = null)
        {
            AssertOutArgument.AreEqual(this.output, name, expected, message);
        }

        /// <summary>
        /// Verifies the arguments are not equal
        /// </summary>
        /// <param name="name">
        /// The output name.
        /// </param>
        /// <param name="expected">
        /// The expected value.
        /// </param>
        /// <param name="message">
        /// Failure message.
        /// </param>
        /// <typeparam name="T">
        /// The expectedType of the out argument
        /// </typeparam>
        [DebuggerStepThrough]
        public void AreNotEqual<T>(string name, T expected, string message = null)
        {
            AssertOutArgument.AreNotEqual(this.output, name, expected, message);
        }

        /// <summary>
        /// Verifies that the argument is false
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <param name="message">
        /// The fail message.
        /// </param>
        [DebuggerStepThrough]
        public void IsFalse(string name, string message = null)
        {
            AssertOutArgument.IsFalse(this.output, name, message);
        }

        /// <summary>
        /// The is instance of type.
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <param name="expectedType">
        /// The expected type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        [DebuggerStepThrough]
        public void IsInstanceOfType(string name, Type expectedType, string message = null)
        {
            AssertOutArgument.IsInstanceOfType(this.output, name, expectedType, message);
        }

        /// <summary>
        /// Verifies that the argument is not null
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <param name="message">
        /// Failure message.
        /// </param>
        [DebuggerStepThrough]
        public void IsNotNull(string name, string message = null)
        {
            AssertOutArgument.IsNotNull(this.output, name, message);
        }

        /// <summary>
        /// Verifies that the argument is null.
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <param name="message">
        /// Failure message.
        /// </param>
        [DebuggerStepThrough]
        public void IsNull(string name, string message = null)
        {
            AssertOutArgument.IsNull(this.output, name, message);
        }

        /// <summary>
        /// Verifies that the argument is true
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <param name="message">
        /// The fail message.
        /// </param>
        [DebuggerStepThrough]
        public void IsTrue(string name, string message = null)
        {
            AssertOutArgument.IsTrue(this.output, name, message);
        }

        #endregion
    }
}