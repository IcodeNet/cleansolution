// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationTestTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.Activities.UnitTesting.Persistence;
    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the WorkflowApplicationTest class
    /// </summary>
    [TestClass]
    public class WorkflowApplicationTestTests
    {
        #region Properties

        /// <summary>
        ///   Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The aborted event should remain signaled.
        /// </summary>
        [TestMethod]
        public void AbortedEventShouldRemainSignaled()
        {
            var expectedException = new InvalidWorkflowException(this.TestContext.TestName);

            var inputs = new Dictionary<string, object> { { "AbortException", expectedException } };

            var host = WorkflowApplicationTest.Create(new ActivityThatAborts(), inputs);

            // Act
            host.TestActivity();

            // Wait for the real aborted event
            Assert.IsTrue(host.WaitForAbortedEvent());

            // Should not block because AbortedEvent remains signaled
            Assert.IsTrue(host.WaitForAbortedEvent());
        }

        /// <summary>
        /// The CompletedEvent wait handle should remain signaled even if it happened before you wait for it.
        /// </summary>
        [TestMethod]
        public void CompletedEventShouldRemainSignaled()
        {
            // Arrange
            var userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var host = WorkflowApplicationTest.Create(new SayHelloWithThread { UserName = userName });

            // Act
            host.TestActivity();

            // Waits for the real completed event
            Assert.IsTrue(host.WaitForCompletedEvent(), "Workflow did not complete before timeout");

            // Assert

            // This should not block because the CompletedEvent should remain signaled
            Assert.IsTrue(host.WaitForCompletedEvent(), "Workflow CompletedEvent is not signaled");
        }

        /// <summary>
        /// Verifies that an episode ended with abort
        /// </summary>
        [TestMethod]
        public void EpisodeShouldEndInAbort()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(GetSequenceThatAbortsAfterAsync());

            // Act
            try
            {
                // Run the activity until it aborts, the activity will go idle once 
                // because of the TestAsync activity.
                AssertHelper.Throws<InvalidOperationException>(() => host.TestWorkflowApplication.RunEpisode());
            }
            finally
            {
                // Track the tracking records to the test results
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Verifies that an episode of work went idle and then completed
        /// </summary>
        [TestMethod]
        public void EpisodeShouldRunToCompleteAfterIdle()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(
                new Sequence { Activities = { new TestAsync(), new WriteLine() } });

            // Act
            try
            {
                // Run the activity until completed
                Assert.AreEqual(ActivityInstanceState.Closed, host.TestWorkflowApplication.RunEpisode().State);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// The should cancel activity in cancellation scope.
        /// </summary>
        [TestMethod]
        public void ShouldCancelActivityInCancellationScope()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new ActivityWithCancelScope());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForIdleEvent());

            host.TestWorkflowApplication.Cancel();

            Assert.IsTrue(host.WaitForCompletedEvent());
            Assert.IsTrue(host.IsCanceled);
        }

        /// <summary>
        /// The should capture aborted args when activity aborts.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureAbortedArgsWhenActivityAborts()
        {
            var expectedException = new InvalidWorkflowException(this.TestContext.TestName);

            var inputs = new Dictionary<string, object> { { "AbortException", expectedException } };

            var host = WorkflowApplicationTest.Create(new ActivityThatAborts(), inputs);

            // Act
            host.TestActivity();

            Assert.IsTrue(host.WaitForAbortedEvent());

            Assert.IsNotNull(host.Results.AbortedArgs);
            Assert.IsInstanceOfType(host.Results.AbortedArgs.Reason, expectedException.GetType());
            Assert.AreSame(expectedException, host.Results.AbortedArgs.Reason);
        }

        /// <summary>
        /// The should capture text lines.
        /// </summary>
        [TestMethod]
        public void ShouldCaptureTextLines()
        {
            // Arrange
            const string expected = "Resumed Bookmark Value";
            var host = WorkflowApplicationTest.Create(new WorkflowWriteLine());

            // Act
            host.TestActivity();

            // Wait for the bookmark
            Assert.IsTrue(host.WaitForIdleEvent());

            // Check the lines
            Assert.AreEqual(2, host.TextLines.Length);

            // Should be waiting for a bookmark named Bookmark1
            Assert.IsTrue(host.Bookmarks.Contains("Bookmark1"));

            Assert.AreEqual(
                BookmarkResumptionResult.Success, host.TestWorkflowApplication.ResumeBookmark("Bookmark1", expected));

            Assert.IsTrue(host.WaitForCompletedEvent());
            Assert.AreEqual(3, host.TextLines.Length);
            Assert.AreEqual(expected, host.TextLines[1]);
        }

        /// <summary>
        /// Test to ensure that the WorkflowApplicationTest.Aborted delegate is invoked
        /// </summary>
        [TestMethod]
        public void ShouldInvokeAbortedDelegateWhenActivityAborts()
        {
            var expectedException = new InvalidWorkflowException(this.TestContext.TestName);

            var inputs = new Dictionary<string, object> { { "AbortException", expectedException } };
            WorkflowApplicationAbortedEventArgs abortArgs = null;

            var host = WorkflowApplicationTest.Create(new ActivityThatAborts(), inputs);

            host.Aborted = (args) => abortArgs = args;

            // Act
            host.TestActivity();

            Assert.IsTrue(host.WaitForAbortedEvent());
            Assert.IsNotNull(abortArgs);
        }

        /// <summary>
        /// Test to ensure that the WorkflowApplicationTest.Completed delegate is invoked
        /// </summary>
        [TestMethod]
        public void ShouldInvokeCompletedDelegateWhenActivityCompletes()
        {
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var host = WorkflowApplicationTest.Create(new SayHelloWithThread { UserName = userName });

            WorkflowApplicationCompletedEventArgs completedArgs = null;

            host.Completed = (args) => completedArgs = args;

            // Act
            host.TestActivity();

            Assert.IsTrue(host.WaitForCompletedEvent());
            Assert.IsNotNull(completedArgs);
        }

        /// <summary>
        /// Test to ensure that the WorkflowApplicationTest.Idle delegate is invoked
        /// </summary>
        [TestMethod]
        public void ShouldInvokeIdleDelegateWhenActivityIsIdle()
        {
            // Arrange
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);

            WorkflowApplicationIdleEventArgs idleArgs = null;
            host.Idle = (args) => idleArgs = args;

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForIdleEvent());

            Assert.AreEqual(
                BookmarkResumptionResult.Success, host.TestWorkflowApplication.ResumeBookmark("Test", "value"));

            Assert.IsTrue(host.WaitForCompletedEvent());

            // Assert
            Assert.IsNotNull(idleArgs);
        }

        /// <summary>
        /// The should invoke persistable idle delegate.
        /// </summary>
        [TestMethod]
        public void ShouldInvokePersistableIdleDelegate()
        {
            // Arrange
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);
            host.TestWorkflowApplication.InstanceStore = new MemoryStore();

            WorkflowApplicationIdleEventArgs idleArgs = null;
            host.PersistableIdle = (args) =>
                {
                    idleArgs = args;
                    return PersistableIdleAction.None;
                };

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForPersistableIdleEvent());

            Assert.AreEqual(
                BookmarkResumptionResult.Success, host.TestWorkflowApplication.ResumeBookmark("Test", "value"));

            Assert.IsTrue(host.WaitForCompletedEvent());

            // Assert
            Assert.IsNotNull(idleArgs);
        }

        /// <summary>
        /// Test to ensure that the WorkflowApplicationTest.Aborted delegate is invoked
        /// </summary>
        [TestMethod]
        public void ShouldInvokeUnhandledExceptionDelegate()
        {
            var expectedException = new InvalidWorkflowException(this.TestContext.TestName);

            var inputs = new Dictionary<string, object> { { "Exception", expectedException } };
            WorkflowApplicationUnhandledExceptionEventArgs unhandledExceptionEventArgs = null;

            var host = WorkflowApplicationTest.Create(new Throw(), inputs);

            host.OnUnhandledException = (args) =>
                {
                    unhandledExceptionEventArgs = args;
                    return UnhandledExceptionAction.Terminate;
                };

            // Act
            host.TestActivity();

            Assert.IsTrue(host.WaitForUnhandledExceptionEvent());
            Assert.IsNotNull(unhandledExceptionEventArgs);
        }

        /// <summary>
        /// The should invoke unloaded delegate.
        /// </summary>
        [TestMethod]
        public void ShouldInvokeUnloadedDelegate()
        {
            // Arrange
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);
            host.TestWorkflowApplication.InstanceStore = new MemoryStore();

            WorkflowApplicationEventArgs applicationEventArgs = null;

            host.Unloaded = (wargs) => applicationEventArgs = wargs;

            host.PersistableIdle = (args) => { return PersistableIdleAction.Unload; };

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForUnloadedEvent());
            Assert.IsNotNull(applicationEventArgs);
        }

        /// <summary>
        /// The should return completion state.
        /// </summary>
        [TestMethod]
        public void ShouldReturnCompletionState()
        {
            var activity = new Sequence { Activities = { new WriteLine { Text = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());

            Assert.IsNotNull(host.CompletionState);
        }

        /// <summary>
        /// The should say hello with thread on different thread.
        /// </summary>
        [TestMethod]
        public void ShouldSayHelloWithThreadOnDifferentThread()
        {
            // Arrange
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var host = WorkflowApplicationTest.Create(new SayHelloWithThread { UserName = userName });

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());
            host.AssertOutArgument.AreEqual("Greeting", expectedGreeting);
            host.AssertOutArgument.AreNotEqual("WorkflowThread", Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Verifies that OutArguments return values when they exist
        /// </summary>
        [TestMethod]
        public void OutArgumentsShouldReturnValues()
        {
            // Arrange
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var host = WorkflowApplicationTest.Create(new SayHelloWithThread { UserName = userName });

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());
            Assert.AreEqual(expectedGreeting, host.OutArguments.Greeting);
            Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, host.OutArguments.WorkflowThread);
        }

        ///<summary>
        ///  Verifies that the OutArguments property is not null when there are no arguments
        ///</summary>
        [TestMethod]
        public void OutArgumentsShouldNotBeNull()
        {
            Activity activity = new Sequence();
            var target = WorkflowApplicationTest.Create(activity);
            target.TestActivity();
            Assert.IsNotNull(target.OutArguments);
        }

        /// <summary>
        /// The should test activities using input dict.
        /// </summary>
        [TestMethod]
        public void ShouldTestActivitiesUsingInputDict()
        {
            // Arrange
            const string userName = "Test User";
            var expectedGreeting = string.Format("Hello {0} from Workflow 4", userName);
            var input = new Dictionary<string, object> { { "UserName", userName } };
            var host = WorkflowApplicationTest.Create(new SayHelloWithThread(), input);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForCompletedEvent());
            host.AssertOutArgument.AreEqual("Greeting", expectedGreeting);
            host.AssertOutArgument.AreNotEqual("WorkflowThread", Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// The should test activity with bookmarks.
        /// </summary>
        [TestMethod]
        public void ShouldTestActivityWithBookmarks()
        {
            // Arrange
            var host = WorkflowApplicationTest.Create(new ActivityWithBookmarks());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.WaitForIdleEvent());

            Assert.IsTrue(host.Bookmarks.Contains("GetNumber2"));
            Assert.AreEqual(
                BookmarkResumptionResult.Success, host.TestWorkflowApplication.ResumeBookmark("GetNumber2", 2));

            Assert.IsTrue(host.WaitForIdleEvent());
            Assert.IsTrue(host.Bookmarks.Contains("GetNumber3"));
            Assert.AreEqual(
                BookmarkResumptionResult.Success, host.TestWorkflowApplication.ResumeBookmark("GetNumber3", 3));

            Assert.IsTrue(host.WaitForCompletedEvent());
            host.AssertOutArgument.AreEqual("Number", 6);
        }

        /// <summary>
        /// The should throw when completion state is not available yet.
        /// </summary>
        [TestMethod]
        public void ShouldThrowWhenCompletionStateIsNotAvailableYet()
        {
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            ActivityInstanceState state;

            AssertHelper.Throws<InvalidOperationException>(() => state = host.CompletionState);
        }

        /// <summary>
        /// The should throw when output is not available yet.
        /// </summary>
        [TestMethod]
        public void ShouldThrowWhenOutputIsNotAvailableYet()
        {
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<InvalidOperationException>(() => host.AssertOutArgument.AreEqual("test", "test"));
        }

        /// <summary>
        /// Verifies that accessing the OutArguments property before the workflow has run will throw
        /// </summary>
        [TestMethod]
        public void OutArgumentsShouldThrowWhenNotSetYet()
        {
            var activity = new Sequence { Activities = { new TestBookmark<string> { BookmarkName = "Test" } } };

            var host = WorkflowApplicationTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<InvalidOperationException>(() => { var value = host.OutArguments.test; });
        }

        /// <summary>
        /// Test to ensure that the WorkflowApplicationTest.Aborted delegate is invoked
        /// </summary>
        [TestMethod]
        public void UnhandledExceptionEventShouldRemainSignaled()
        {
            var expectedException = new InvalidWorkflowException(this.TestContext.TestName);

            var inputs = new Dictionary<string, object> { { "Exception", expectedException } };

            var host = WorkflowApplicationTest.Create(new Throw(), inputs);

            // Act
            host.TestActivity();

            // Wait for the real unhandled exception event
            Assert.IsTrue(host.WaitForUnhandledExceptionEvent());

            // Should not block because the event remains signaled
            Assert.IsTrue(host.WaitForUnhandledExceptionEvent());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a sequence that aborts after TestAsync activity
        /// </summary>
        /// <returns>
        /// The sequence
        /// </returns>
        private static Sequence GetSequenceThatAbortsAfterAsync()
        {
            return new Sequence
                {
                    Activities =
                        {
                            new TestAsync(), 
                            new Throw
                                {
                                    Exception =
                                        new InArgument<Exception>(ctx => new InvalidOperationException("Test Exception"))
                                }
                        }
                };
        }

        /// <summary>
        /// Gets a sequence with three TestAsync activities
        /// </summary>
        /// <param name="sleep">
        /// The number of milliseconds to sleep.
        /// </param>
        /// <returns>
        /// The sequence 
        /// </returns>
        private static Sequence GetSequenceWithThreeTestAsync(int sleep)
        {
            return new Sequence
                {
                    Activities =
                        {
                            new TestAsync { Sleep = sleep }, 
                            new TestAsync { Sleep = sleep }, 
                            new TestAsync { Sleep = sleep }, 
                        }
                };
        }

        #endregion
    }
}