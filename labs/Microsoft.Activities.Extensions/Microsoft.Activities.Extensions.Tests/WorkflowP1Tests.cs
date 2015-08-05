// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTests.cs" company="Microsoft">
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

    using Microsoft.Activities.Extensions.Linq;
    using Microsoft.Activities.Extensions.Prototype;
    using Microsoft.Activities.Extensions.Prototype1;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WorkflowResult = Microsoft.Activities.Extensions.Prototype.WorkflowResult;

    /// <summary>
    ///   Tests for the Workflow class
    /// </summary>
    [TestClass]
    public class WorkflowP1Tests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Abort method is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void AbortShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.Abort());
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A CancellationTokenSource
        ///   * A Workflow with a Delay activity
        ///   When
        ///   * The token is canceled
        ///   Then
        ///   * The workflow is canceled
        /// </summary>
        [TestMethod]
        public void CancelWithTokenSource()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Delay { Duration = TimeSpan.FromMilliseconds(200) };
            var workflow = new WorkflowP1(activity)
                {
                    Timeout = Constants.Timeout,
                    Extensions =
                        {
                            new ListTrackingParticipant()
                        }
                };
            var delayExecuting = new AutoResetEvent(false);
            var source = new CancellationTokenSource(Constants.Timeout);

            // When the workflow is idle set an event to trigger the test to cancel
            workflow.When.Idle += (sender, args) => delayExecuting.Set();

            try
            {
                // Act
                TestTrace.Act();

                // Start the workflow async
                var task = workflow.RunAsync(source.Token);

                TestTrace.Write("Waiting for Delay activity to start executing");
                var delayIsExecuting = delayExecuting.WaitOne(Constants.Timeout);


                if (delayIsExecuting)
                {
                    TestTrace.Write("Cancelling workflow via token source");
                    source.Cancel();
                }


                // Assert
                TestTrace.Assert();
                Assert.IsTrue(delayIsExecuting);

                AssertHelper.Throws<AggregateException>(task.Wait, typeof(TaskCanceledException));

                // Canceled tasks can run to completion
                Assert.IsTrue(task.IsCanceled, "The task was not canceled");
                Assert.IsTrue(task.IsCompleted, "Task was not completed, task status is " + task.Status.ToString());
            }
            finally
            {
                TestTrace.Finally();

                workflow.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow which runs a long running TaskAsyncActivity
        ///   When
        ///   * The AsyncSpinWaiter is Scheduled
        ///   Then
        ///   * The test detects this and cancels the workflow before AsyncSpinWaiter blocks
        /// </summary>
        [TestMethod]
        public void CancelWithAsyncActivityWillCancel()
        {
            // Arrange
            TestTrace.Arrange();
            var spinExecuting = new AutoResetEvent(false);
            var notify = new SpinNotify { LoopComplete = (loops, iterations) => spinExecuting.Set() };
            var cancellationHandlerClosed = new AutoResetEvent(false);
            var activity = CreateSequenceWithSpinWaiter(typeof(AsyncSpinWaiter));

            var tracking = new ListTrackingParticipant();

            var workflow = new WorkflowP1(activity)
            {
                Timeout = Constants.Timeout,
                Extensions =
                        {
                            tracking, 
                            notify
                        }
            };

            workflow.When.Activity.Name("CancellationHandler").Closed +=
                (sender, args) => cancellationHandlerClosed.Set();

            var source = new CancellationTokenSource(Constants.Timeout);

            try
            {
                // Act
                TestTrace.Act();
                workflow.Until.Idle.RunAsync(source.Token);

                TestTrace.Write("Waiting for AsyncSpinWaiter to start executing");
                var spinWait = spinExecuting.WaitOne(TimeSpan.FromSeconds(10));

                TestTrace.Write("Cancelling workflow");
                source.Cancel();

                var cancelInvoked = cancellationHandlerClosed.WaitOne(Constants.Timeout);

                // Assert
                TestTrace.Assert();
                Assert.IsTrue(spinWait, "The spinExecuting event was not set");
                Assert.IsTrue(cancelInvoked);
                Assert.IsTrue(
                    tracking.Records.Any(ActivityInstanceState.Closed, "CancellationHandler"),
                    "The cancellation handler was not invoked");
            }
            finally
            {
                TestTrace.Finally();

                workflow.Trace();
            }
        }



        /// <summary>
        ///   Given
        ///   * A Workflow which runs a long running activity
        ///   When
        ///   * The workflow is canceled
        ///   Then
        ///   * The result state is Canceled
        /// * Workflow.IsCompleted is true
        /// * Workflow.IsCanceled is true
        /// </summary>
        [TestMethod]
        public void CancelWithTokenSimple()
        {
            var activity = new Delay() { Duration = TimeSpan.FromSeconds(10) };
            var workflow = new WorkflowP1(activity);
            var cancellationTokenSource = new CancellationTokenSource(Constants.Timeout);

            // Run until idle
            var task = workflow.Until.Idle.RunAsync(cancellationTokenSource.Token);

            // Cancel with the token source
            cancellationTokenSource.Cancel();

            // If you try to do anything with the task, you get an exception
            AssertHelper.Throws<AggregateException>(
                task.Wait,
                typeof(TaskCanceledException),
                "The task should be canceled");
            Assert.IsTrue(task.IsCanceled);
        }



        /// <summary>
        ///   Given
        ///   * A Workflow which runs a long running TaskAsyncActivity
        ///   When
        ///   * The AsyncSpinWaiter is Scheduled
        ///   Then
        ///   * The test detects this and cancels the workflow before AsyncSpinWaiter blocks
        /// </summary>
        [TestMethod]
        public void CancelTaskAsyncActivityViaToken()
        {
            // Arrange
            TestTrace.Arrange();
            var spinExecuting = new ManualResetEvent(false);
            var completedEvent = new ManualResetEvent(false);
            var notify = new SpinNotify { LoopComplete = (loops, iterations) => spinExecuting.Set() };
            var cancellationHandlerClosed = new AutoResetEvent(false);
            var activity = CreateSequenceWithSpinWaiter(typeof(TaskSpinWaiter));

            var tracking = new ListTrackingParticipant();

            var workflow = new WorkflowP1(activity)
            {
                Timeout = Constants.Timeout,
                Extensions =
                        {
                            tracking, 
                            notify
                        }
            };

            workflow.When.Activity.Name("CancellationHandler").Closed +=
                (sender, args) => cancellationHandlerClosed.Set();

            workflow.When.Completed += (sender, args) => completedEvent.Set();

            var source = new CancellationTokenSource(Constants.Timeout);

            try
            {
                // Act
                TestTrace.Act();
                var task = workflow.RunAsync(source.Token);

                TestTrace.Write("Waiting for AsyncSpinWaiter to start executing");
                Assert.IsTrue(spinExecuting.WaitOne(Constants.Timeout));

                TestTrace.Write("Cancelling workflow with token");
                source.Cancel();

                TestTrace.Write("Waiting for workflow to execute cancellation handler");
                Assert.IsTrue(cancellationHandlerClosed.WaitOne(Constants.Timeout));

                TestTrace.Write("Waiting for workflow");

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<AggregateException>(task.Wait, typeof(TaskCanceledException));
                Assert.IsTrue(
                    tracking.Records.Any(ActivityInstanceState.Closed, "CancellationHandler"),
                    "The cancellation handler was not invoked");
                Assert.IsTrue(task.IsCanceled);
                Assert.IsTrue(task.IsCompleted);
            }
            finally
            {
                TestTrace.Finally();

                workflow.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow which runs a long running activity named AsyncSpinWaiter
        ///   When
        ///   * The AsyncSpinWaiter is Scheduled
        ///   Then
        ///   * The test detects this and cancels the workflow before AsyncSpinWaiter blocks
        /// </summary>
        [TestMethod]
        public void CancelWorkflowApp()
        {
            // Arrange
            TestTrace.Arrange();
            var spinExecuting = new AutoResetEvent(false);
            var completed = new AutoResetEvent(false);
            var notify = new SpinNotify { LoopComplete = (loops, iterations) => spinExecuting.Set() };
            var activity = CreateSequenceWithSpinWaiter(typeof(SpinWaiter));

            WorkflowApplicationCompletedEventArgs completedArgs = null;

            var workflowApplication = new WorkflowApplication(activity)
                {
                    Completed = args =>
                        {
                            completedArgs = args;
                            WorkflowTrace.Verbose(
                                "Completed state: {0} fault {1}", args.CompletionState, args.TerminationException);
                            completed.Set();
                        },
                    Aborted = args => WorkflowTrace.Verbose("Aborted {0}", args.Reason),
                };

            var source = new CancellationTokenSource(Constants.Timeout);

            var tracking = new ListTrackingParticipant();
            workflowApplication.Extensions.Add(tracking);
            workflowApplication.Extensions.Add(notify);
            var activitySource = new ActivityCancellationToken(source.Token);
            workflowApplication.Extensions.Add(activitySource);

            try
            {
                // Act
                TestTrace.Act();

                workflowApplication.Run();

                TestTrace.Write("Waiting for AsyncSpinWaiter to start executing");
                var spinWait = spinExecuting.WaitOne(TimeSpan.FromSeconds(10));

                if (spinWait)
                {
                    source.Token.Register(workflowApplication.Cancel);

                    TestTrace.Write("Cancelling workflow");
                    source.Cancel();

                    // workflow.Cancel();
                }
                else
                {
                    TestTrace.Write("spinWait timeout");
                }

                var isComplete = completed.WaitOne(Constants.Timeout);

                // Assert
                TestTrace.Assert();

                Assert.IsTrue(isComplete);
                Assert.IsNotNull(completedArgs);
                Assert.AreEqual(ActivityInstanceState.Canceled, completedArgs.CompletionState);
            }
            finally
            {
                TestTrace.Finally();
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Extensions property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void ExtensionsGetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => AssertHelper.GetProperty(workflow.Extensions));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Extensions property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void ExtensionsSetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.Extensions = null);
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A Disposed Workflow
        ///   When
        ///   * the Id property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void IdShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => AssertHelper.GetProperty(workflow.Id));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Load method is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void LoadShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.Load(Guid.Empty));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The PersistableIdle property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void PersistableIdleGetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => AssertHelper.GetProperty(workflow.PersistableIdle));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The PersistableIdle property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void PersistableIdleSetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(
                    () => workflow.PersistableIdle = PersistableIdleAction.Persist);
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The ResumeAsync method is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void ResumeAsyncShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.ResumeAsync("Foo").Wait());
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The ResumeAsync method is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void ResumeAsyncWithTokenShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.ResumeAsync("Foo").Wait());
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Resume method is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void ResumeShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.Resume("Foo"));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with a Sequence activity
        ///   When
        ///   * Workflow.RunAsync is invoked
        ///   Then
        ///   * The workflow runs to completion
        /// </summary>
        [TestMethod]
        public void RunAsyncSimple()
        {
            var activity = new Sequence();

            // Workflow owns disposable members
            using (var workflow = new WorkflowP1(activity))
            {
                workflow.RunAsync();
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with a Sequence activity
        ///   When
        ///   * Workflow.RunAsync is invoked
        ///   Then
        ///   * The workflow runs to completion
        /// </summary>
        [TestMethod]
        public void RunAsyncWithAwatMethod()
        {
            // Invoke a method using the async keyword
            var result = RunSequenceAsync(5).Result as WorkflowCompletedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Outputs.Result);
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with a Sequence activity
        ///   When
        ///   * Workflow.Run is invoked
        ///   Then
        ///   * The workflow runs to completion
        /// </summary>
        /// <remarks>
        ///   As a Workflow user
        ///   I want to run a workflow to completion
        ///   So that I have a very simple way to run a workflow
        /// </remarks>
        [TestMethod]
        public void RunShouldRunWorkflowToCompletion()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            using (var workflow = new WorkflowP1(activity) { Timeout = Constants.Timeout })
            {
                try
                {
                    // Act
                    TestTrace.Act();
                    var result = workflow.RunAsync().Result as WorkflowCompletedResult;

                    // Assert
                    TestTrace.Assert();
                    Assert.IsNotNull(result);
                }
                finally
                {
                    TestTrace.Finally();
                    workflow.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with a Sequence activity which has been disposed
        ///   When
        ///   * Workflow.Run is invoked
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void RunShouldThrowIfDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            using (var workflow = new WorkflowP1(activity) { Timeout = Constants.Timeout })
            {
                Exception exception = null;
                try
                {
                    // Act
                    TestTrace.Act();
                    workflow.Dispose();

                    // ReSharper disable AccessToDisposedClosure
                    // Testing dispose behavior
                    exception = AssertHelper.Throws<ObjectDisposedException>(() => workflow.RunAsync().Wait());

                    // ReSharper restore AccessToDisposedClosure

                    // Assert
                    TestTrace.Assert();
                }
                finally
                {
                    TestTrace.Finally();
                    exception.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with a Sequence activity
        ///   When
        ///   * Workflow.Run is invoked
        ///   Then
        ///   * The workflow runs to completion
        /// </summary>
        /// <remarks>
        ///   As a Workflow user
        ///   I want to run a workflow to completion
        ///   So that I have a very simple way to run a workflow
        /// </remarks>
        [TestMethod]
        public void RunSimple()
        {
            var activity = new Sequence();

            // Workflow owns disposable members
            using (var workflow = new WorkflowP1(activity))
            {
                workflow.RunAsync().Wait();
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with an EchoArg activity
        ///   * A WorkflowArguments
        ///   When
        ///   * Workflow.Run is invoked with the arguments
        ///   Then
        ///   * The workflow runs to completion
        ///   * The Output.Result value is the value passed to EchoArg
        /// </summary>
        /// <remarks>
        ///   As a Workflow user
        ///   I want to run a workflow to completion
        ///   So that I have a very simple way to run a workflow
        /// </remarks>
        [TestMethod]
        public void RunWithArgsSimple()
        {
            var activity = new EchoArg<int>();

            var workflow = new WorkflowP1(activity);

            // Setup EchoArg.Value in argument
            workflow.Inputs.Value = 123;

            var result = workflow.Until.Complete.Run();

            // Access EchoArg.Result out argument
            Assert.AreEqual(123, result.Outputs.Result);
        }


        /// <summary>
        /// Given
        /// * Arrange
        /// When
        /// * Act
        /// Then
        /// * Assert
        /// </summary>
        [TestMethod]
        public void RunTwiceShouldThrow()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);

            try
            {
                // Act / Assert
                TestTrace.Act();
                workflow.RunAsync().Wait();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<AggregateException>(() => workflow.RunAsync().Wait(), typeof(WorkflowApplicationCompletedException));
            }
            finally
            {
                TestTrace.Finally();

                // Trace objects here
            }
        }

        /// <summary>
        ///   Given
        ///   * A Workflow with an EchoArg activity
        ///   * A WorkflowArguments with an invalid argument
        ///   When
        ///   * Workflow.Run is invoked with the arguments
        ///   Then
        ///   * Throw
        /// </summary>
        [TestMethod]
        public void RunWithInvalidArgShouldThrow()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new EchoArg<int>();
            using (var workflow = new WorkflowP1(activity) { Extensions = { new ListTrackingParticipant() } })
            {
                // Intentionally used wrong name
                workflow.Inputs.Input1 = 123;

                try
                {
                    // Act 
                    TestTrace.Act();

                    // Assert
                    TestTrace.Assert();
                    AssertHelper.Throws<ArgumentException>(workflow.RunAsync());
                }
                finally
                {
                    TestTrace.Finally();
                    workflow.Trace();
                }
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The TraceOptions property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void TraceOptionsGetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => AssertHelper.GetProperty(workflow.TraceOptions));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The TraceOptions property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void TraceOptionsSetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.TraceOptions = WorkflowTraceOptions.Debug);
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The UnhandledException property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void UnhandledExceptionGetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(
                    () => AssertHelper.GetProperty(workflow.UnhandledException));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The UnhandledException property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void UnhandledExceptionSetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => workflow.UnhandledException = null);
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        /// <summary>
        ///   Given
        ///   * A disposed Workflow
        ///   When
        ///   * The Until property is accessed
        ///   Then
        ///   * An ObjectDisposedException is thrown
        /// </summary>
        [TestMethod]
        public void UntilGetShouldThrowObjectDisposed()
        {
            // Arrange
            TestTrace.Arrange();
            var activity = new Sequence();
            var workflow = new WorkflowP1(activity);
            workflow.RunAsync().Wait();

            try
            {
                // Act
                TestTrace.Act();
                workflow.Dispose();

                // Assert
                TestTrace.Assert();
                AssertHelper.Throws<ObjectDisposedException>(() => AssertHelper.GetProperty(workflow.Until));
            }
            finally
            {
                TestTrace.Finally();

                // Trace things here
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create sequence with spin waiter.
        /// </summary>
        /// <param name="spinType">
        /// The spin type.
        /// </param>
        /// <returns>
        /// The System.Activities.Statements.Sequence.
        /// </returns>
        private static Sequence CreateSequenceWithSpinWaiter(Type spinType)
        {
            var ctor = spinType.GetConstructor(new Type[0]);
            Debug.Assert(ctor != null, "ctor != null");

            var spinWaiter = (Activity)ctor.Invoke(new object[0]);
            spinType.GetProperty("Iterations").SetValue(spinWaiter, 10000);
            spinType.GetProperty("Loops").SetValue(spinWaiter, 10);

            return new Sequence
                {
                    Activities =
                        {
                            new CancellationScope
                                {
                                    Body = new Sequence
                                        {
                                            DisplayName = "Inner Sequence", 
                                            Activities =
                                                {
                                                    new WriteLine { Text = "Before async spin wait" }, 
                                                    spinWaiter, 
                                                    new WriteLine { Text = "After async spin wait" }, 
                                                }
                                        }, 
                                    CancellationHandler =
                                        new WriteLine
                                            {
                                               Text = "Cancellation Handler", DisplayName = "CancellationHandler" 
                                            }, 
                                }, 
                            new Delay { Duration = TimeSpan.FromMilliseconds(500) }, 
                            new WriteLine { Text = "I'm done" }
                        }
                };
        }

        /// <summary>
        /// A sample method using async
        /// </summary>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <returns>
        /// A task 
        /// </returns>
        private static async Task<WorkflowResult> RunSequenceAsync(int value)
        {
            var activity = new EchoArg<int> { Value = value };

            return await Workflow.RunAsync(activity);
        }

        #endregion
    }
}