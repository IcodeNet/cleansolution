// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertOutArgumentTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AssertOutArgumentTest and is intended
    ///   to contain all AssertOutArgumentTest Unit Tests
    /// </summary>
    [TestClass]
    public class AssertOutArgumentTest
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
        /// The are equal test should fail when not equal.
        /// </summary>
        [TestMethod]
        public void AreEqualTestShouldFailWhenNotEqual()
        {
            const string userName = "Test User";
            const string expectedExceptionMessage =
                "AssertOutArgument.AreEqual failed. Expected:<No Match>. Actual:<Hello Test User from Workflow 4>.";

            var output = WorkflowInvoker.Invoke(new SayHelloWithThread { UserName = userName });

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertOutArgument.AreEqual(output, "Greeting", "No Match"), 
                expectedExceptionMessage, 
                "AssertOutArgument Did not throw WorkflowAssertFailedException when AreEqual should fail");
        }

        /// <summary>
        /// The are not equal test should fail when equal.
        /// </summary>
        [TestMethod]
        public void AreNotEqualTestShouldFailWhenEqual()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);

            const string expectedExceptionMessage =
                "AssertOutArgument.AreNotEqual failed. Expected any value except:<Hello Test User from Workflow 4>. Actual:<Hello Test User from Workflow 4>.";

            var output = WorkflowInvoker.Invoke(new SayHelloWithThread { UserName = userName });

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertOutArgument.AreNotEqual(output, "Greeting", expectedGreeting), 
                expectedExceptionMessage, 
                "AssertOutArgument Did not throw WorkflowAssertFailedException when AreNotEqual should fail");
        }

        /// <summary>
        /// Verifies that the test will fail when an OutArgument does not exist
        /// </summary>
        [TestMethod]
        public void AssertOutArgumentFailsTestWhenOutArgDoesNotExist()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new SayHello { UserName = "Test User" });

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());

            const string expectedExceptionMessage =
                "AssertOutArgument.AreNotEqual failed. Output does not contain an argument named <WorkflowThread>.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                AssertOutArgument.AreNotEqual(
                    host.Results.Output, "WorkflowThread", Thread.CurrentThread.ManagedThreadId), 
                expectedExceptionMessage, 
                "AssertOutArgument Did not throw WorkflowAssertFailedException for missing out argument");
        }

        /// <summary>
        /// Verifies that the test will fail when an OutArgument does not exist
        /// </summary>
        [TestMethod]
        public void AssertOutArgumentFailsTestWhenOutArgIsWrongType()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new SayHelloWithStringThread { UserName = "Test User" });

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());

            const string expectedExceptionMessage =
                "AssertOutArgument.AreNotEqual failed. Wrong type for OutArgument <WorkflowThread>. Expected Type: <System.Int32>. Actual Type: <System.String>.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                AssertOutArgument.AreNotEqual(
                    host.Results.Output, "WorkflowThread", Thread.CurrentThread.ManagedThreadId), 
                expectedExceptionMessage, 
                "AssertOutArgument Did not throw WorkflowAssertFailedException on wrong type");
        }

        /// <summary>
        /// The assert out argument should fail on null output.
        /// </summary>
        [TestMethod]
        public void AssertOutArgumentShouldFailOnNullArg()
        {
            const string expectedExceptionMessage = "AssertOutArgument.IsNotNull failed.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertOutArgument.IsNotNull(new Dictionary<string, object> { { "name", null } }, "name"), 
                expectedExceptionMessage, 
                "AssertOutArgument did not throw WorkflowAssertFailedException when arg is null");
        }

        /// <summary>
        /// The assert out argument should fail on null output.
        /// </summary>
        [TestMethod]
        public void AssertOutArgumentShouldFailOnNullOutput()
        {
            const string expectedExceptionMessage = "AssertOutArgument.IsNotNull failed. output is null.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertOutArgument.IsNotNull(null, "name"), 
                expectedExceptionMessage, 
                "AssertOutArgument did not throw WorkflowAssertFailedException when output is null");
        }

        /// <summary>
        /// The should fail on non null out arg.
        /// </summary>
        [TestMethod]
        public void ShouldFailOnNonNullOutArg()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new WorkflowWithNonNullOutArgument());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());

            const string expectedExceptionMessage = "AssertOutArgument.IsNull failed.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertOutArgument.IsNull(host.Results.Output, "NonNullOutArg"), 
                expectedExceptionMessage, 
                "AssertOutArgument.IsNull Did not throw on non null out argument");
        }

        /// <summary>
        /// The should fail on null out arg.
        /// </summary>
        [TestMethod]
        public void ShouldFailOnNullOutArg()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new WorkflowWithNullOutArgument());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());

            const string expectedExceptionMessage = "AssertOutArgument.IsNotNull failed.";

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.AssertOutArgument.IsNotNull("NullOutArg"), 
                expectedExceptionMessage, 
                "AssertOutArgument.IsNotNull Did not throw on null out argument");
        }

        /// <summary>
        /// The should work on null out arg.
        /// </summary>
        [TestMethod]
        public void ShouldWorkOnNullOutArg()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new WorkflowWithNullOutArgument());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());
            host.AssertOutArgument.IsNull("NullOutArg");
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

            AssertOutArgument.IsFalse(host.Output, "Result");
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

            AssertHelper.Throws<WorkflowAssertFailedException>(() => AssertOutArgument.IsTrue(host.Output, "Result"));
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

            AssertHelper.Throws<WorkflowAssertFailedException>(() => AssertOutArgument.IsFalse(host.Output, "Result"));
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

            AssertOutArgument.IsTrue(host.Output, "Result");
        }

        #endregion
    }
}