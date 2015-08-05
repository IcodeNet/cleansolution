// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InArgumentExtensionsTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests extension methods for InArgumentExtensions
    /// </summary>
    [TestClass]
    public class InArgumentExtensionsTest
    {
        #region Public Methods

        /// <summary>
        /// When only the required arg is provided the activity should use the default value
        /// </summary>
        [TestMethod]
        public void WhenOnlyRequiredArgIsProvidedOptionalArgShouldUseDefault()
        {
            var host = new WorkflowInvokerTest(new ActivityWithOptionalArgs { RequiredArg = "Required Value" });

            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual(
                "Result", string.Format("Required Value: {0}", ActivityWithOptionalArgs.DefaultOptionalValue));
        }

        /// <summary>
        /// When only the required arg is provided the activity should use the default value
        /// </summary>
        [TestMethod]
        public void WhenOnlyRequiredArgIsProvidedOptionalStringArgShouldUseDefault()
        {
            var host = new WorkflowInvokerTest(new ActivityWithOptionalStringArg { RequiredArg = "Required Value" });

            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual(
                "Result", string.Format("Required Value: {0}", ActivityWithOptionalStringArg.DefaultOptionalValue));
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalStringArgShouldUseOptionalValue()
        {
            const string Expected = "optional";
            var host =
                new WorkflowInvokerTest(
                    new ActivityWithOptionalStringArg { RequiredArg = "Required Value", OptionalArg = Expected });

            try
            {
                host.TestActivity(Constants.Timeout);
                host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));

            }
            finally
            {
                host.Tracking.Trace();
            } 
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalArgShouldUseOptionalValue()
        {
            const int Expected = 2;
            var host =
                new WorkflowInvokerTest(
                    new ActivityWithOptionalArgs { RequiredArg = "Required Value", OptionalArg = Expected });

            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));
            host.Tracking.Trace();
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value even if it is equal to default(T)
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalArgShouldUseOptionalValueNull()
        {
            const string Expected = null;
            var host =
                new WorkflowInvokerTest(
                    new ActivityWithOptionalStringArg { RequiredArg = "Required Value", OptionalArg = Expected });

            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));
            host.Tracking.Trace();
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value even if it is equal to default(T)
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalArgShouldUseOptionalValue0()
        {
            const int Expected = 0;
            var host =
                new WorkflowInvokerTest(
                    new ActivityWithOptionalArgs { RequiredArg = "Required Value", OptionalArg = Expected });

            host.TestActivity(Constants.Timeout);
            host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));
            host.Tracking.Trace();
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalStringArgShouldUseOptionalValueInputs()
        {
            const string Expected = "optional";
            var host = new WorkflowInvokerTest(new ActivityWithOptionalStringArg());

            host.TestActivity(GetInputs("Required Value", Expected));
            host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));
            host.Tracking.Trace();
        }

        /// <summary>
        /// When optional and required args are provided the activity should use the provided optional value
        /// </summary>
        [TestMethod]
        public void WhenOptionalAndRequiredArgIsProvidedOptionalArgShouldUseOptionalValueInputs()
        {
            const int Expected = 2;
            var host = new WorkflowInvokerTest(new ActivityWithOptionalArgs());

            host.TestActivity(GetInputs("Required Value", Expected));
            host.AssertOutArgument.AreEqual("Result", string.Format("Required Value: {0}", Expected));
            host.Tracking.Trace();
        }

        /// <summary>
        /// When the required arg is not provided the host should throw an ArgumentException
        /// </summary>
        [TestMethod]
        public void WhenRequiredArgIsNotProvidedShouldThrowArgumentException()
        {
            var host = new WorkflowInvokerTest(new ActivityWithOptionalArgs());

            // Required argument was not supplied, this should throw an exception
            AssertHelper.Throws<ArgumentException>(
                () => host.TestActivity(), 
                string.Empty, 
                "The activity should fail because the RequiredArg was not provided");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an input dictionary with the required and optional arguments
        /// </summary>
        /// <param name="text">
        /// The required argument text.
        /// </param>
        /// <param name="optional">
        /// The optional argument.
        /// </param>
        /// <returns>
        /// an input dictionary with the required and optional arguments
        /// </returns>
        private static IDictionary<string, object> GetInputs(string text, string optional)
        {
            return new Dictionary<string, object> { { "RequiredArg", text }, { "OptionalArg", optional } };
        }

        /// <summary>
        /// Returns an input dictionary with the required and optional arguments
        /// </summary>
        /// <param name="text">
        /// The required argument text.
        /// </param>
        /// <param name="optional">
        /// The optional argument.
        /// </param>
        /// <returns>
        /// an input dictionary with the required and optional arguments
        /// </returns>
        private static IDictionary<string, object> GetInputs(string text, int optional)
        {
            return new Dictionary<string, object> { { "RequiredArg", text }, { "OptionalArg", optional } };
        }

        #endregion
    }
}