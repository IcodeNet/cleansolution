// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingWorkflowTask
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Prototype;
    using Microsoft.Activities.UnitTesting.Activities;

    /// <summary>
    ///   The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        ///   The timeout.
        /// </summary>
        private static readonly TimeSpan Timeout =
            TimeSpan.FromMilliseconds(1000);

        #endregion

        #region Methods

        /// <summary>
        /// The echo arg.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        private static string EchoArg(string message)
        {
            return message;
        }

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private static void Main(string[] args)
        {
            WriteThread(
                "Invoking WorkflowTask.Run to get result");
            TaskCancel();

            WriteThread("Complete - Press any key");
            Console.ReadKey(true);
        }

        /// <summary>
        /// The task get result.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void TaskGetResult(string message)
        {
            var task = Task.Run(() => EchoArg(message));
            WriteThread(task.Result);
        }

        /// <summary>
        /// The task get result.
        /// </summary>
        private static void TaskCancel()
        {
            var source = new CancellationTokenSource();
            var task = Task.Run(() => CountTo(100, source.Token), source.Token);
            source.CancelAfter(100);

            try
            {
                // task.Wait();
                Thread.Sleep(200);
                WriteThread("Was the task canceled? " + task.IsCanceled);
                WriteThread("Was the task completed? " + task.IsCompleted);
            }
            catch (AggregateException exception)
            {
                WriteThread("Caught exception " + exception.InnerException.Message);
            }
        }

        private static void CountTo(int countTo, CancellationToken token)
        {
            for (int i = 0; i < countTo; i++)
            {
                WriteThread("Count: {0}", i);
                Thread.Sleep(i);

                // You can terminate the operation by using one of these options:

                // By simply returning from the delegate. In many scenarios this is sufficient; 
                // however, a task instance that is "canceled" in this way transitions to the RanToCompletion state, 
                // not to the Canceled state.

                //if (token.IsCancellationRequested)
                //{
                //    WriteThread("Canceling after {0} iterations", i + 1);
                //    break;
                //}


                // By throwing a OperationCanceledException and passing it the token 
                // on which cancellation was requested. 
                // The preferred way to do this is to use the ThrowIfCancellationRequested method. 
                // A task that is canceled in this way transitions to the Canceled state, 
                // which the calling code can use to verify that the task responded to its cancellation request.

                //token.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// The task get result.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void TaskGetResultContinueWith(
            string message)
        {
            // Fire and forget and then write result
            Task.Run(() => EchoArg(message)).ContinueWith(
                task => WriteThread(task.Result));
        }

        /// <summary>
        /// The write to console with task.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void TaskWriteToConsoleFireAndForget(
            string message)
        {
            // Fire and forget
            Task.Run(() => WriteThread(message));
        }

        /// <summary>
        /// The write to console with task and wait.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void
            TaskWriteToConsoleWithTaskAndWait(
            string message)
        {
            // Get the task and wait for it to complete
            var task = Task.Run(() => WriteThread(message));
            task.Wait();
        }

        /// <summary>
        /// The workflow task get result.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void WorkflowTaskGetResult(
            string message)
        {
            var workflowTask =
                WorkflowTask.Run(
                    new EchoArg<string> { Value = message });

            WriteThread(workflowTask.Result);
        }

        /// <summary>
        /// The task get result.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void
            WorkflowTaskGetResultContinueWith(
            string message)
        {
            // Fire and forget and then write result
            WorkflowTask.Run(new EchoArg<string> { Value = message })
                .ContinueWith(
                    workflowTask =>
                    WriteThread(workflowTask.Result));
        }

        /// <summary>
        /// The write thread.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        private static void WriteThread(
            string message, params object[] args)
        {
            Console.WriteLine(
                "[{0,2:00}] {1}",
                Thread.CurrentThread.ManagedThreadId,
                string.Format(message, args));
        }

        /// <summary>
        /// The write to console fire and forget.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void WriteToConsoleFireAndForget(
            string message)
        {
            // Fire and forget using a WriteThread activity
            WorkflowTask.Run(
                new WriteLineThread { Message = message });
        }

        /// <summary>
        /// The write to console fire and forget.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        private static void WriteToConsoleWithTaskAndWait(
            string message)
        {
            // Get the workflow and wait for it to complete
            var workflow =
                WorkflowTask.Run(
                    new WriteLineThread
                        {
                            Message = message
                        });
            workflow.WaitWorkflow();
        }

        #endregion
    }
}