// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertHelperTest.cs" company="">
//   
// </copyright>
// <summary>
//   This is a test class for AssertHelperTest and is intended
//   to contain all AssertHelperTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AssertHelperTest and is intended
    ///   to contain all AssertHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class AssertHelperTest
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
        /// The should capture with action.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWorkflowAssertFailedExceptionWhenActionDoesNotThrow()
        {
            var didNotThrow = false;

            // Assert that an action should throw an WorkflowAssertFailedException
            try
            {
                // Empty action will not throw
                AssertHelper.Throws<WorkflowAssertFailedException>(() => { });

                // If we get here AssertHelper did not work
                didNotThrow = true;
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException)
            {
                // This catch occurs when the test passes or fails
                if (didNotThrow)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void ContainsReturnsTrueWhenAllItemsContained()
        {
            var list = new List<int> { 1, 2, 3, 4 };

            AssertHelper.Contains(list, 2, 3);
        }

        [TestMethod]
        public void ContainsReturnsFalseWhenItemsNotContained()
        {
            var list = new List<int> { 1, 2, 3, 4 };

            AssertHelper.Throws<WorkflowAssertFailedException>(() => AssertHelper.Contains(list, 2, 3, 8));
        }

        /// <summary>
        /// The should capture with action.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithAction()
        {
            AssertHelper.Throws<ArgumentException>(ThrowArgException);
        }

        /// <summary>
        /// The should capture with action message and inner exception.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionAndInnerException()
        {
            var expected = this.TestContext.TestName;

            AssertHelper.Throws<InvalidOperationException>(
                () => ThrowInvalidOperationExceptionWithInner(expected), typeof(ArgumentNullException));
        }

        /// <summary>
        /// The should capture with action and message.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionAndMessage()
        {
            var expected = this.TestContext.TestName;

            AssertHelper.Throws<InvalidOperationException>(() => ThrowInvalidOperationException(expected), expected);
        }

        /// <summary>
        /// The should capture with action message fail and inner exception.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionFailAndInnerException()
        {
            var expected = this.TestContext.TestName;

            AssertHelper.Throws<InvalidOperationException>(
                () => ThrowInvalidOperationExceptionWithInner(expected), 
                typeof(ArgumentNullException), 
                "Failed to match inner exception");
        }

        /// <summary>
        /// The should capture with action message and fail.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionMessageAndFail()
        {
            var expected = this.TestContext.TestName;
            var failMessage = "Fail " + this.TestContext.TestName;
            var expectedMessage =
                "Action did not throw exception of type System.NotImplementedException. " + failMessage;

            try
            {
                AssertHelper.Throws<NotImplementedException>(DontThrow, expected, failMessage);
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException afx)
            {
                Assert.AreEqual(expectedMessage, afx.Message);
            }
        }

        /// <summary>
        /// The should capture with action message and inner exception.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionMessageAndInnerException()
        {
            var expected = this.TestContext.TestName;

            AssertHelper.Throws<InvalidOperationException>(
                () => ThrowInvalidOperationExceptionWithInner(expected), expected, typeof(ArgumentNullException));
        }

        /// <summary>
        /// The should capture with action message fail and inner exception.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureWithActionMessageFailAndInnerException()
        {
            var expected = this.TestContext.TestName;

            AssertHelper.Throws<InvalidOperationException>(
                () => ThrowInvalidOperationExceptionWithInner(expected), 
                expected, 
                typeof(ArgumentNullException), 
                "Failed to match inner exception");
        }

        /// <summary>
        /// The should capture with action.
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldCaptureWorkflowAssertFailedExceptionWhenActionDoesNotThrow()
        {
            var didNotThrow = false;

            // Assert that an action should throw an WorkflowAssertFailedException
            try
            {
                // Empty action will not throw
                AssertHelper.Throws<WorkflowAssertFailedException>(Task<int>.Factory.StartNew(() => 0));

                // If we get here AssertHelper did not work
                didNotThrow = true;
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException)
            {
                // This catch occurs when the test passes or fails
                if (didNotThrow)
                {
                    Assert.Fail();
                }
            }
        }

        /// <summary>
        /// Verifies that if a task throws a different exception than expected 
        ///   the assert will fail
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldFailIfWrongException()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationException(expected));
            try
            {
                AssertHelper.Throws<ArgumentException>(task);
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldFailThrowException()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => 0);

            try
            {
                // This will fail
                AssertHelper.Throws<InvalidOperationException>(task);
            }
            catch (WorkflowAssertFailedException)
            {
                // do nothing
            }
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowException()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task);
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionMessageInnerAndFail()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(
                task, expected, typeof(ArgumentNullException), "Fail Message");
        }

        /// <summary>
        /// Verifies that a task throws an exception and reports the fail message
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionWithFailMessages()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected, "Task did not throw expected exception");
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionWithInner()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, typeof(ArgumentNullException));
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionWithInnerAndExpected()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected, typeof(ArgumentNullException));
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionWithInnerAndFail()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, typeof(ArgumentNullException), "Fail Message");
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskOfTShouldThrowExceptionWithText()
        {
            var expected = this.TestContext.TestName;
            var task = Task<int>.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected);
        }

        /// <summary>
        /// The should capture with action.
        /// </summary>
        [TestMethod]
        public void TaskShouldCaptureWorkflowAssertFailedExceptionWhenActionDoesNotThrow()
        {
            var didNotThrow = false;

            // Assert that an action should throw an WorkflowAssertFailedException
            try
            {
                // Empty action will not throw
                AssertHelper.Throws<WorkflowAssertFailedException>(Task.Factory.StartNew(() => { }));

                // If we get here AssertHelper did not work
                didNotThrow = true;
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException)
            {
                // This catch occurs when the test passes or fails
                if (didNotThrow)
                {
                    Assert.Fail();
                }
            }
        }

        /// <summary>
        /// Verifies that if a task throws a different exception than expected 
        ///   the assert will fail
        /// </summary>
        [TestMethod]
        public void TaskShouldFailIfWrongException()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            try
            {
                AssertHelper.Throws<ArgumentException>(task);
                Assert.Fail();
            }
            catch (WorkflowAssertFailedException)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldFailThrowException()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => { });

            try
            {
                // This will fail
                AssertHelper.Throws<InvalidOperationException>(task);
            }
            catch (WorkflowAssertFailedException)
            {
                // do nothing
            }
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowException()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task);
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionMessageInnerAndFail()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(
                task, expected, typeof(ArgumentNullException), "Fail Message");
        }

        /// <summary>
        /// Verifies that a task throws an exception and reports the fail message
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionWithFailMessages()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected, "Task did not throw expected exception");
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionWithInner()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, typeof(ArgumentNullException));
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionWithInnerAndExpected()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected, typeof(ArgumentNullException));
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionWithInnerAndFail()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationExceptionWithInner(expected));

            AssertHelper.Throws<InvalidOperationException>(task, typeof(ArgumentNullException), "Fail Message");
        }

        /// <summary>
        /// Verifies that a task throws an exception
        /// </summary>
        [TestMethod]
        public void TaskShouldThrowExceptionWithText()
        {
            var expected = this.TestContext.TestName;
            var task = Task.Factory.StartNew(() => ThrowInvalidOperationException(expected));

            AssertHelper.Throws<InvalidOperationException>(task, expected);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dont throw.
        /// </summary>
        private static void DontThrow()
        {
            return;
        }

        /// <summary>
        /// The throw arg exception.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        private static void ThrowArgException()
        {
            throw new ArgumentException();
        }

        /// <summary>
        /// The throw invalid operation exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <returns>
        /// nothing really
        /// </returns>
        private static int ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// The throw invalid operation exception with inner.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// An invalid operation exception with inner.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private static int ThrowInvalidOperationExceptionWithInner(string message)
        {
            throw new InvalidOperationException(message, new ArgumentNullException());
        }

        #endregion
    }
}