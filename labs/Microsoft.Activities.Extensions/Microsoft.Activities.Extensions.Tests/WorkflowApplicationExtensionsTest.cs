// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationExtensionsTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.Activities.UnitTesting.Persistence;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Parallel = System.Activities.Statements.Parallel;

    /// <summary>
    ///   Unit tests for WorkflowApplicationExtensions
    /// </summary>
    [TestClass]
    public class WorkflowApplicationExtensionsTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication 
        ///   * An activity that creates a bookmark and an activity that creates an extension
        ///   * two singleton extensions
        ///   When 
        ///   * GetExtensions is invoked
        ///   Then
        ///   * a read only collection is returned
        ///   * The collection contains the two extensions
        ///   * The extension created by the activity does not appear in the collection
        /// </summary>
        [TestMethod]
        public void GetExtensionsReturnsExtensionsCollection()
        {
            const string BookmarkName = "Test";

            var activity = new Sequence
                {
                    Activities =
                        {
                            new ActivityExtensionTest { AddExtensionProvider = true }, 
                            new TestBookmark<int> { BookmarkName = BookmarkName }, 
                            new ActivityExtensionTest { AddExtensionProvider = true }, 
                        }
                };

            var traceTrackingParticipant = new TraceTrackingParticipant();
            var listTrackingParticipant = new ListTrackingParticipant();

            var workflowApplication = new WorkflowApplication(activity);

            // Add a couple of singleton extensions
            workflowApplication.Extensions.Add(traceTrackingParticipant);
            workflowApplication.Extensions.Add(listTrackingParticipant);

            // foreach (var extension in workflowApplication.Extensions)
            // {
            // Doh! this won't work
            // foreach statement cannot operate on variables of type 
            // 'System.Activities.Hosting.WorkflowInstanceExtensionManager' 
            // because 'System.Activities.Hosting.WorkflowInstanceExtensionManager' 
            // does not contain a public definition for 'GetEnumerator'	
            // }

            // Run it so that the activity will create an extension
            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            // Resume and run to completion
            workflowApplication.ResumeEpisodeBookmark(BookmarkName, 1);

            // Now I can get the Singleton Extensions as a collection
            var extensions = workflowApplication.GetSingletonExtensions();
            Assert.IsNotNull(extensions);
            Assert.AreEqual(2, extensions.Count);

            // Note: Extensions created by AddDefaultExtensionProvider will not appear in the collection
            Assert.IsTrue(extensions.Contains(traceTrackingParticipant));
            Assert.IsTrue(extensions.Contains(listTrackingParticipant));

            foreach (var extension in extensions)
            {
                Debug.WriteLine("Found singleton extension " + extension);
            }
        }

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication
        ///   When 
        ///   * it is created
        ///   Then
        ///   * WorkflowApplication.IsHandlerThread should be false
        /// </summary>
        [TestMethod]
        public void InHandlerThreadDetectsHandlerThread()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };
            var inHandler = false;
            WorkflowApplication workflowApplication = null;
            workflowApplication = new WorkflowApplication(activity)
                {
                    // ReSharper disable AccessToModifiedClosure
                    Idle = args =>
                        {
                            if (workflowApplication != null)
                            {
                                inHandler =
                                    workflowApplication.
                                        IsHandlerThread();

                                // workflowApplication.Cancel();
                                // Doh! Can't to this in a handler
                                // System.InvalidOperationException was unhandled by user code
                                // Message=WorkflowApplication operations cannot be performed from within event handlers.
                            }
                        }, 
                    
                    // ReSharper restore AccessToModifiedClosure
                };

            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            Assert.IsTrue(inHandler);
            Assert.IsFalse(workflowApplication.IsHandlerThread());
        }

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication
        ///   When 
        ///   * it is created
        ///   Then
        ///   * WorkflowApplication.IsReadOnly should be false
        ///   When
        ///   * it is run
        ///   Then
        ///   * Attempting to add an extension results in an InvalidOperationException
        ///   * WorkflowApplication.IsReadOnly should be true
        /// </summary>
        [TestMethod]
        public void IsReadOnlyDetectsReadOnly()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };
            var workflowApplication = new WorkflowApplication(activity);

            // Before you run it, the application is not readonly
            var readOnly = workflowApplication.IsReadOnly();
            Assert.IsFalse(readOnly);

            // Once you run the WorkflowApplication it becomes read only
            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            // This will cause an exception
            // System.InvalidOperationException was unhandled by user code
            // Message=WorkflowInstanceExtensionsManager cannot be modified once it has been associated with a WorkflowInstance.
            AssertHelper.Throws<InvalidOperationException>(
                () => workflowApplication.Extensions.Add(new TraceTrackingParticipant()));

            readOnly = workflowApplication.IsReadOnly();
            Assert.IsTrue(readOnly);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * ResumeUntilBookmark is invoked with no bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void ResumeUntilBookmarkShouldResumeUntilIdleAnyBookmark()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            workflowApplication.RunUntilBookmark(ExpectedBookmarkName1);

            // Act
            var result = workflowApplication.ResumeUntilBookmark(ExpectedBookmarkName1, 1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * ResumeUntilBookmark is invoked with the bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void ResumeUntilBookmarkShouldResumeUntilIdleWithBookmarkName()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            workflowApplication.RunUntilBookmark(ExpectedBookmarkName1);

            // Act
            var result = workflowApplication.ResumeUntilBookmark(ExpectedBookmarkName1, 1, ExpectedBookmarkName2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * ResumeUntilBookmark is invoked with the bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void ResumeUntilBookmarkShouldResumeUntilIdleWithBookmarkNameAndTimeout()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            workflowApplication.RunUntilBookmark(ExpectedBookmarkName1);

            // Act
            var result = workflowApplication.ResumeUntilBookmark(
                ExpectedBookmarkName1, 1, Constants.Timeout, ExpectedBookmarkName2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * ResumeUntilBookmark is invoked with no bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void ResumeUntilBookmarkShouldTimeoutOnBadName()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            workflowApplication.RunUntilBookmark(ExpectedBookmarkName1);

            // Act / Assert
            AssertHelper.Throws<TimeoutException>(
                () =>
                workflowApplication.ResumeUntilBookmark(ExpectedBookmarkName1, 1, TimeSpan.FromMilliseconds(100), "BAD"));
        }

        /// <summary>
        ///   Given 
        ///   * A Parallel with two branches that contain an activity which creates a bookmark
        ///   When
        ///   * RunUntilBookmark is invoked with two bookmark names
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void RunUntilBookmark2BookmarksRunUntilIdleWithBookmarkName()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Parallel
                        {
                            Branches =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            // Act
            WorkflowEpisode.DefaultTimeout = Constants.Timeout;

            var result = workflowApplication.RunUntilBookmark(ExpectedBookmarkName1, ExpectedBookmarkName2);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * A Parallel with two branches that contain an activity which creates a bookmark
        ///   When
        ///   * RunUntilBookmark is invoked with no bookmark names
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void RunUntilBookmarkNoMatchThrowsTimeout()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Parallel
                        {
                            Branches =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });
            WorkflowEpisode.DefaultTimeout = TimeSpan.FromMilliseconds(100);

            // Act / Assert
            AssertHelper.Throws<TimeoutException>(() => workflowApplication.RunUntilBookmark("BAD"));
        }

        /// <summary>
        ///   Given 
        ///   * A Parallel with two branches that contain an activity which creates a bookmark
        ///   When
        ///   * RunUntilBookmark is invoked with no bookmark names
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void RunUntilBookmarkNoNameRunUntilIdleWithAnyBookmarkName()
        {
            // Arrange
            const string ExpectedBookmarkName1 = "Test1";
            const string ExpectedBookmarkName2 = "Test2";
            var workflowApplication =
                new WorkflowApplication(
                    new Parallel
                        {
                            Branches =
                                {
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName1 }, 
                                    new TestBookmark<int> { BookmarkName = ExpectedBookmarkName2 }, 
                                }
                        });

            // Act
            WorkflowEpisode.DefaultTimeout = Constants.Timeout;

            var result = workflowApplication.RunUntilBookmark();

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * RunUntilBookmark is invoked with the bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void RunUntilBookmarkShouldRunUntilIdleWithBookmarkName()
        {
            // Arrange
            const string ExpectedBookmarkName = "Test";
            var workflowApplication =
                new WorkflowApplication(new TestBookmark<int> { BookmarkName = ExpectedBookmarkName });

            // Act
            WorkflowEpisode.DefaultTimeout = Constants.Timeout;

            var result = workflowApplication.RunUntilBookmark(ExpectedBookmarkName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   When
        ///   * RunUntilBookmark is invoked with the bookmark name
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * With a state of Executing
        /// </summary>
        [TestMethod]
        public void RunUntilBookmarkShouldRunUntilIdleWithBookmarkNameAndTimeout()
        {
            // Arrange
            const string ExpectedBookmarkName = "Test";
            var workflowApplication =
                new WorkflowApplication(new TestBookmark<int> { BookmarkName = ExpectedBookmarkName });

            // Act
            var result = workflowApplication.RunUntilBookmark(Constants.Timeout, ExpectedBookmarkName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given 
        ///   * an activity with bookmarks
        ///   When 
        ///   * it is run and then aborted
        ///   Then
        ///   * WorkflowApplication.IsAborted should be true
        /// </summary>
        [TestMethod]
        public void WhenAbortedIsAbortedIsTrue()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };

            var workflowApplication = new WorkflowApplication(activity);

            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);
            workflowApplication.Abort();

            Assert.IsTrue(workflowApplication.IsAborted());
        }

        /// <summary>
        ///   Given an activity with delays when RunEpisodeAsync is called with a cancellation token the caller can cancel via the token
        /// </summary>
        [TestMethod]
        public void WhenActivityDelayIsRunAsyncCancelationTokenCanCancel()
        {
            // The WorkflowRuntime is not aware of the cancellation token and will not cancel the workflow
            // The WorkflowEpisode will cancel when the workflow becomes idle
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence { Activities = { new Delay { Duration = TimeSpan.FromMilliseconds(10) }, } });

            var tokenSource = new CancellationTokenSource();

            // Run the activity
            var task = workflowApplication.RunEpisodeAsync(tokenSource.Token);

            // Immediately cancel 
            tokenSource.Cancel();

            // Exception is thrown when Wait() or Result is accessed
            AssertHelper.Throws<TaskCanceledException>(task);
        }

        /// <summary>
        ///   Given an activity when Run is called the caller can specify to run until tracking reports an activity state is reached
        /// </summary>
        [TestMethod]
        public void WhenActivityGoesIdleEpisodeContinuesUntilTrackingReportsActivityState()
        {
            const string WaitForBookmarkName = "TestBookmark";

            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new TestBookmark<int> { BookmarkName = WaitForBookmarkName }
                                }
                        });

            // Run through three idle events and end the episode when idle with a bookmark "TestBookmark" 
            var result = workflowApplication.RunEpisodeAsync(WaitForBookmarkName, Constants.Timeout).Result;

            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given an activity when Run is called the caller can specify to run until tracking reports an activity state is reached
        /// </summary>
        [TestMethod]
        public void WhenActivityRunsToTargetResumeContinuesUntilTrackingReportsActivityState()
        {
            const string WaitForBookmarkName = "TestBookmark";

            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new TestBookmark<int> { BookmarkName = WaitForBookmarkName }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new TestBookmark<int> { BookmarkName = WaitForBookmarkName }, 
                                }
                        });

            // Run through three idle events and end the episode when idle with a bookmark "TestBookmark" 
            Assert.AreEqual(
                ActivityInstanceState.Executing, 
                workflowApplication.RunEpisode(WaitForBookmarkName, Constants.Timeout).State);

            // Assert.AreEqual(ActivityInstanceState.Executing, workflowApplication.RunEpisode(WaitForBookmarkName, TimeSpan.FromHours(1)).State);
            Debug.WriteLine("Running to idle second time TestBookmark");

            // var result = workflowApplication.ResumeEpisodeBookmark(WaitForBookmarkName, 1, WaitForBookmarkName, this.DefaultTimeout);
            var result = workflowApplication.ResumeEpisodeBookmark(
                WaitForBookmarkName, 1, WaitForBookmarkName, TimeSpan.FromHours(1));

            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
        }

        /// <summary>
        ///   Given an activity that throws an exception and the caller specifies UnhandledExceptionAction.Cancel the workflow should throw an OperationCanceledException
        /// </summary>
        [TestMethod]
        public void WhenActivityThrowsAndUnhandledCancelRequestedEpisodeShouldCancel()
        {
            var workflowApplication =
                new WorkflowApplication(
                    new Throw { Exception = new InArgument<Exception>(ctx => new InvalidOperationException()) });

            AssertHelper.Throws<TaskCanceledException>(
                workflowApplication.RunEpisodeAsync(UnhandledExceptionAction.Cancel));
        }

        /// <summary>
        ///   Given an activity that throws an exception and the caller specifies UnhandledExceptionAction.Terminate the workflow should throw the terminating exception
        /// </summary>
        [TestMethod]
        public void WhenActivityThrowsAndUnhandledTerminateRequestedEpisodeShouldTerminate()
        {
            var workflowApplication =
                new WorkflowApplication(
                    new Throw { Exception = new InArgument<Exception>(ctx => new InvalidOperationException()) });

            AssertHelper.Throws<InvalidOperationException>(
                workflowApplication.RunEpisodeAsync(UnhandledExceptionAction.Terminate));
        }

        /// <summary>
        ///   Given an activity that throws an exception when Run it should throw an AggregateException with the terminating exception contained within
        /// </summary>
        [TestMethod]
        public void WhenActivityThrowsRunShouldThrowAggregateWithInnerSetToTerminatingException()
        {
            var workflowApplication =
                new WorkflowApplication(
                    new Throw { Exception = new InArgument<Exception>(ctx => new InvalidOperationException()) });

            AssertHelper.Throws<InvalidOperationException>(workflowApplication.RunEpisodeAsync(Constants.Timeout));
        }

        /// <summary>
        ///   Given 
        ///   * an activity that will go idle with a bookmark 
        ///   When 
        ///   * RunEpisodeAsync is called with the name of the bookmark and timeout
        ///   Then
        ///   * it should return a WorkflowIdleEpisodeResult
        ///   * The state should be Executing
        ///   * There should be one bookmark
        /// </summary>
        [TestMethod]
        public void WhenActivityWithBookmarkAndPersistenceGoesIdleRunAsyncShouldReturnWorkflowIdleEpisodeResult()
        {
            var activity = new TestBookmark<int> { BookmarkName = "Test" };
            var workflowApplication = new WorkflowApplication(activity) { InstanceStore = new MemoryStore() };

            var result = workflowApplication.RunEpisodeAsync("Test", Constants.Timeout).Result;

            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));

            var completedResult = (WorkflowIdleEpisodeResult)result;
            Assert.AreEqual(ActivityInstanceState.Executing, completedResult.State);
            Assert.AreEqual(1, completedResult.IdleArgs.Bookmarks.Count);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   * that will complete when Resumed 
        ///   When
        ///   * ResumeBookmarkAsync is called with the bookmark name and data
        ///   Then
        ///   * it should return an WorkflowCompletedEpisodeResult 
        ///   * with Output arguments 
        ///   * and State Closed
        /// </summary>
        [TestMethod]
        public void WhenActivityWithBookmarkGoesIdleResumeBookmarkAsyncShouldReturnWorkflowCompletedEpisodeResult()
        {
            var workflowApplication = new WorkflowApplication(new TestBookmark<int> { BookmarkName = "Test" });

            // Run to the idle
            Assert.AreEqual(
                ActivityInstanceState.Executing, workflowApplication.RunEpisode("Test", Constants.Timeout).State);

            // Resume and complete
            var result = workflowApplication.ResumeEpisodeBookmarkAsync("Test", 1).Result;

            Assert.IsInstanceOfType(result, typeof(WorkflowCompletedEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Closed, result.State);
            AssertOutArgument.AreEqual(((WorkflowCompletedEpisodeResult)result).Outputs, "Result", 1);
        }

        /// <summary>
        ///   Given 
        ///   * an activity that will go idle with a bookmark 
        ///   When 
        ///   * RunEpisodeAsync is called with the name of the bookmark and timeout
        ///   Then
        ///   * it should return a WorkflowIdleEpisodeResult
        ///   * The state should be Executing
        ///   * There should be one bookmark
        /// </summary>
        [TestMethod]
        public void WhenActivityWithBookmarkGoesIdleRunAsyncShouldReturnWorkflowIdleEpisodeResult()
        {
            var workflowApplication = new WorkflowApplication(new TestBookmark<int> { BookmarkName = "Test" });

            var result = workflowApplication.RunEpisodeAsync("Test", Constants.Timeout).Result;

            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));

            var idleEpisodeResult = (WorkflowIdleEpisodeResult)result;
            Assert.AreEqual(ActivityInstanceState.Executing, idleEpisodeResult.State);
            Assert.AreEqual(1, idleEpisodeResult.IdleArgs.Bookmarks.Count);
        }

        /// <summary>
        ///   Given an activity with a bookmark when ResumeEpisodeBookmarkAsync is called with an invalid bookmark name a BookmarkResumptionException will be thrown
        /// </summary>
        [TestMethod]
        public void WhenActivityWithBookmarkIsResumedAsyncWithWrongNameExceptionIsThrown()
        {
            const string WaitForBookmarkName = "TestBookmark";

            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new TestBookmark<int>
                                        {
                                           DisplayName = WaitForBookmarkName, BookmarkName = WaitForBookmarkName 
                                        }
                                }
                        });

            // Run through three idle events and end the episode when idle with a bookmark named "TestBookmark"
            Assert.AreEqual(ActivityInstanceState.Executing, workflowApplication.RunEpisode(WaitForBookmarkName).State);

            var exception =
                AssertHelper.Throws<BookmarkResumptionException>(
                    workflowApplication.ResumeEpisodeBookmarkAsync("WrongName", null));
            Assert.AreEqual("WrongName", exception.BookmarkName);
            Assert.AreEqual(BookmarkResumptionResult.NotFound, exception.Result);
            exception.Trace();
        }

        /// <summary>
        ///   Given an activity with a bookmark when ResumeEpisodeBookmark is called with an invalid bookmark name a BookmarkResumptionException will be thrown
        /// </summary>
        [TestMethod]
        public void WhenActivityWithBookmarkIsResumedWithWrongNameExceptionIsThrown()
        {
            const string WaitForBookmarkName = "TestBookmark";

            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(1) }, 
                                    new TestBookmark<int>
                                        {
                                           DisplayName = WaitForBookmarkName, BookmarkName = WaitForBookmarkName 
                                        }
                                }
                        });

            // Run through three idle events and end the episode when idle with a bookmark named "TestBookmark"
            Assert.AreEqual(ActivityInstanceState.Executing, workflowApplication.RunEpisode(WaitForBookmarkName).State);

            var exception =
                AssertHelper.Throws<BookmarkResumptionException>(
                    workflowApplication.ResumeEpisodeBookmarkAsync("WrongName", null));
            Assert.AreEqual("WrongName", exception.BookmarkName);
            Assert.AreEqual(BookmarkResumptionResult.NotFound, exception.Result);
        }

        /// <summary>
        ///   Given an activity with delays when RunEpisodeAsync is called with a cancellation token the caller can cancel via the token
        /// </summary>
        [TestMethod]
        public void WhenActivityWithMultipleDelayIsRunAsyncCancelationTokenCanCancel()
        {
            const string WaitForBookmarkName = "TestBookmark";

            // The WorkflowRuntime is not aware of the cancellation token and will not cancel the workflow
            // The WorkflowEpisode will cancel when the workflow becomes idle
            var workflowApplication =
                new WorkflowApplication(
                    new Sequence
                        {
                            Activities =
                                {
                                    new Delay { Duration = TimeSpan.FromMilliseconds(100) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(100) }, 
                                    new Delay { Duration = TimeSpan.FromMilliseconds(100) }, 
                                    new TestBookmark<int> { BookmarkName = WaitForBookmarkName }
                                }
                        });

            var tokenSource = new CancellationTokenSource();

            // Run the activity
            var task = workflowApplication.RunEpisodeAsync(WaitForBookmarkName, tokenSource.Token);

            // Immediately cancel 
            tokenSource.Cancel();

            // Exception is thrown when Wait() or Result is accessed
            AssertHelper.Throws<TaskCanceledException>(task);
        }

        /// <summary>
        ///   Given 
        ///   * an activity with bookmarks
        ///   When 
        ///   * it is run 
        ///   Then
        ///   * WorkflowApplication.IsInitialized should be true
        /// </summary>
        [TestMethod]
        public void WhenInitializedIsInitializedIsTrue()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };

            var workflowApplication = new WorkflowApplication(activity);

            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            Assert.IsTrue(workflowApplication.IsInitialized());
        }

        /// <summary>
        ///   Given 
        ///   * an activity with no bookmarks 
        ///   When 
        ///   * it is run Run asynchronously
        ///   Then
        ///   * it should return a task 
        ///   * and a result with Output arguments 
        ///   * and State Closed
        /// </summary>
        [TestMethod]
        public void WhenNoBookmarksAndOutArgRunAsyncShouldCompleteWithOutArgs()
        {
            const int Expected = 1;
            var workflowApplication = new WorkflowApplication(new EchoArg<int> { Value = Expected });

            var task = workflowApplication.RunEpisodeAsync(Constants.Timeout);

            Assert.IsNotNull(task);
            var result = task.Result;

            Assert.IsInstanceOfType(result, typeof(WorkflowCompletedEpisodeResult));

            var completedResult = (WorkflowCompletedEpisodeResult)result;
            Assert.IsNotNull(completedResult);
            Assert.AreEqual(ActivityInstanceState.Closed, completedResult.State);
            AssertOutArgument.AreEqual(completedResult.Outputs, "Result", Expected);
        }

        /// <summary>
        ///   Given 
        ///   * an activity with no bookmarks 
        ///   When 
        ///   * RunEpisode is invoked
        ///   Then
        ///   * The calling thread is blocked until the workflow completes
        ///   * and returns a result object with Output arguments 
        ///   * and State Closed
        /// </summary>
        [TestMethod]
        public void WhenNoBookmarksAndOutArgRunShouldCompleteWithOutArgs()
        {
            const int Expected = 1;
            var workflowApplication = new WorkflowApplication(new EchoArg<int> { Value = Expected });

            // To run it synchronously
            var result = workflowApplication.RunEpisode(Constants.Timeout);

            // Or asynchronously using a Task
            // var result = workflowApplication.RunEpisodeAsync(this.DefaultTimeout).Result;
            Assert.IsInstanceOfType(result, typeof(WorkflowCompletedEpisodeResult));

            var completedResult = (WorkflowCompletedEpisodeResult)result;
            Assert.AreEqual(ActivityInstanceState.Closed, completedResult.State);
            AssertOutArgument.AreEqual(completedResult.Outputs, "Result", Expected);
        }

        /// <summary>
        ///   Given 
        ///   * an activity with bookmarks
        ///   When 
        ///   * it is run 
        ///   Then
        ///   * WorkflowApplication.IsAborted should be false
        /// </summary>
        [TestMethod]
        public void WhenNotAbortedIsAbortedIsFalse()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };

            var workflowApplication = new WorkflowApplication(activity);

            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            Assert.IsFalse(workflowApplication.IsAborted());
        }

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication
        ///   When 
        ///   * it is created
        ///   Then
        ///   * WorkflowApplication.IsInitialized should be false
        /// </summary>
        [TestMethod]
        public void WhenNotInitializedIsInitializedIsFalse()
        {
            const string BookmarkName = "Test";

            var activity = new TestBookmark<int> { BookmarkName = BookmarkName };

            var workflowApplication = new WorkflowApplication(activity);

            Assert.IsFalse(workflowApplication.IsInitialized());
        }

        /// <summary>
        ///   Given 
        ///   * An instance store with a persisted instance
        ///   * that will complete when Resumed 
        ///   When
        ///   * ResumeEpisodeBookmark is called with the bookmark name and value 1
        ///   Then
        ///   * it should return a WorkflowCompletedEpisodeResult 
        ///   * with state = ActivityInstanceState.Closed
        ///   * and Output argument Result = 1
        /// </summary>
        [TestMethod]
        public void WhenPersistedInstanceLoadedRunEpisodeShouldComplete()
        {
            var activity = new TestBookmark<int> { BookmarkName = "Test" };

            var workflowApplication = new WorkflowApplication(activity)
                {
                   InstanceStore = new MemoryStore(), PersistableIdle = args => PersistableIdleAction.Unload, 
                };

            // Episodes can end with until unloaded, aborted, completed, timeout
            // Run episode until unloaded because of persistable idle event
            var workflowIdleEpisodeResult =
                workflowApplication.RunEpisode(Constants.Timeout) as WorkflowIdleEpisodeResult;

            Assert.IsNotNull(workflowIdleEpisodeResult);

            // Cannot resume the same WorkflowApplication - it will cause a System.InvalidOperationException
            // Message=WorkflowInstance (guid) cannot be modified after it has started running.
            var workflowApplicationResume = new WorkflowApplication(activity) { InstanceStore = new MemoryStore(), };

            // Load the instance with a new WorkflowApplication
            workflowApplicationResume.Load(workflowIdleEpisodeResult.InstanceId);

            // Resume and complete
            var result = workflowApplicationResume.ResumeEpisodeBookmark("Test", 1);

            Assert.IsInstanceOfType(result, typeof(WorkflowCompletedEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Closed, result.State);
            AssertOutArgument.AreEqual(((WorkflowCompletedEpisodeResult)result).Outputs, "Result", 1);
        }

        /// <summary>
        ///   Given 
        ///   * an activity which creates a bookmark
        ///   * that will complete when Resumed 
        ///   * and an InstanceStore is set on the WorkflowApplication
        ///   * and the PersistableIdle func returns PersistableIdleAction.Unload
        ///   When
        ///   * RunEpisode is called with the default timeout
        ///   Then
        ///   * it should return a WorkflowIdleEpisodeResult 
        ///   * with state = ActivityInstanceState.Executing
        ///   * and Unloaded = true
        /// </summary>
        [TestMethod]
        public void WhenstringleWithPersistenceUnloadRunShouldReturnIdleUnloaded()
        {
            var activity = new TestBookmark<int> { BookmarkName = "Test" };

            var workflowApplication = new WorkflowApplication(activity)
                {
                   InstanceStore = new MemoryStore(), PersistableIdle = args => PersistableIdleAction.Unload, 
                };

            // Episodes can end with until unloaded, aborted, completed, timeout
            // Run episode until unloaded because of persistable idle event
            var workflowIdleEpisodeResult =
                workflowApplication.RunEpisode(Constants.Timeout) as WorkflowIdleEpisodeResult;

            Assert.IsNotNull(workflowIdleEpisodeResult);
            Assert.AreEqual(ActivityInstanceState.Executing, workflowIdleEpisodeResult.State);
            Assert.IsTrue(workflowIdleEpisodeResult.Unloaded);
        }

        #endregion
    }
}