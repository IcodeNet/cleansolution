// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertOutputTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Collections.Generic;

    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AssertOutputTest and is intended
    ///   to contain all AssertOutputTest Unit Tests
    /// </summary>
    [TestClass]
    public class AssertOutputTest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies that the AreEqual method does not throw when equal
        /// </summary>
        [TestMethod]
        public void WhenEqualAreEqualShouldNotThrow()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);
            host.AssertOutArgument.AreEqual("Result", expected);
        }

        /// <summary>
        /// Verifies that AreNotEqual throws when equal
        /// </summary>
        [TestMethod]
        public void WhenEqualAreNotEqualThrows()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.AssertOutArgument.AreNotEqual("Result", expected));
        }

        /// <summary>
        /// Verifies that when an out argument is false, IsFalse does not throw an exception
        /// </summary>
        [TestMethod]
        public void WhenFalseIsFalseDoesNotThrow()
        {
            const bool expected = false;
            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoArg<bool>());
            host.TestActivity(input);

            host.AssertOutArgument.IsFalse("Result");
        }

        /// <summary>
        /// Verifies that when an out argument is false, IsTrue throws an exception
        /// </summary>
        [TestMethod]
        public void WhenFalseIsTrueDoesThrow()
        {
            const bool expected = false;
            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoArg<bool>());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.AssertOutArgument.IsTrue("Result"));
        }

        /// <summary>
        /// Verifies that the AreEqual method does throw when not equal
        /// </summary>
        [TestMethod]
        public void WhenNotEqualAreEqualShouldThrow()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.AssertOutArgument.AreEqual("Result", string.Empty));
        }

        /// <summary>
        /// Verifies that AreNotEqual throws when equal
        /// </summary>
        [TestMethod]
        public void WhenNotEqualAreNotEqualDoesNotThrow()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            host.AssertOutArgument.AreNotEqual("Result", string.Empty);
        }

        /// <summary>
        /// Verifies that IsNotNull does not throw when the argument is not null
        /// </summary>
        [TestMethod]
        public void WhenNotNullIsNotNullDoesNotThrow()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            host.AssertOutArgument.IsNotNull("Result");
        }

        /// <summary>
        /// When Not Null IsNull Does Throw
        /// </summary>
        [TestMethod]
        public void WhenNotNullIsNullDoesThrow()
        {
            var expected = this.TestContext.TestName;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.AssertOutArgument.IsNull("Result"));
        }

        /// <summary>
        /// Verifies that IsNotNull does throw when the argument is null
        /// </summary>
        [TestMethod]
        public void WhenNullIsNotNullThrow()
        {
            string expected = null;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.AssertOutArgument.IsNotNull("Result"));
        }

        /// <summary>
        /// When Null IsNull Does Not Throw
        /// </summary>
        [TestMethod]
        public void WhenNullIsNullDoesNotThrow()
        {
            string expected = null;

            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoStringArg());
            host.TestActivity(input);

            host.AssertOutArgument.IsNull("Result");
        }

        /// <summary>
        /// Verifies that when an out argument is true, IsFalse throws an exception
        /// </summary>
        [TestMethod]
        public void WhenTrueIsFalseDoesThrow()
        {
            const bool expected = true;
            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoArg<bool>());
            host.TestActivity(input);

            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.AssertOutArgument.IsFalse("Result"));
        }

        /// <summary>
        /// Verifies that when an out argument is true, IsTrue does not throw an exception
        /// </summary>
        [TestMethod]
        public void WhenTrueIsTrueDoesNotThrow()
        {
            const bool expected = true;
            var input = new Dictionary<string, object> { { "Value", expected } };
            var host = new WorkflowInvokerTest(new EchoArg<bool>());
            host.TestActivity(input);

            host.AssertOutArgument.IsTrue("Result");
        }

        #endregion
    }
}