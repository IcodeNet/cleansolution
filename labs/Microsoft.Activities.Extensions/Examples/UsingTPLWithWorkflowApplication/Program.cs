// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingTPLWithWorkflowApplication
{
    using System;
    using System.Activities;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Enums

        /// <summary>
        /// The no or yes.
        /// </summary>
        private enum NoOrYes
        {
            /// <summary>
            ///   The no value.
            /// </summary>
            No,

            /// <summary>
            ///   The yes value.
            /// </summary>
            Yes,
        }

        #endregion

        #region Methods

        /// <summary>
        /// The invoke with standard workflow application.
        /// </summary>
        private static void InvokeWithStandardWorkflowApplication()
        {
            var shouldThrow = PromptEnum<NoOrYes>("Should the workflow throw an exception") == NoOrYes.Yes;
            var workflowBusy = new AutoResetEvent(false);
            var workflowApplication = new WorkflowApplication(new Workflow1 { ShouldThrow = shouldThrow });
            var workflowFaulted = false;

            workflowApplication.OnUnhandledException = args =>
                {
                    Console.WriteLine("Workflow Unhandled Exception {0}", args.UnhandledException.Message);
                    return PromptEnum<UnhandledExceptionAction>("Select return action");
                };

            workflowApplication.Idle = (WorkflowApplicationIdleEventArgs obj) =>
                {
                    Console.WriteLine("Workflow is idle");
                    workflowBusy.Set();
                };

            workflowApplication.Aborted = args =>
                {
                    Console.WriteLine("Workflow aborted reason {0}", args.Reason.Message);
                    workflowFaulted = true;
                    workflowBusy.Set();
                };

            workflowApplication.Completed = args =>
                {
                    Console.WriteLine("Workflow completed state is {0}", args.CompletionState);
                    switch (args.CompletionState)
                    {
                        case ActivityInstanceState.Closed:
                            Console.WriteLine("Number out argument is {0}", args.Outputs["Value"]);
                            break;
                        case ActivityInstanceState.Faulted:
                            Console.WriteLine(
                                "Workflow faulted termination exception {0}", args.TerminationException.Message);
                            workflowFaulted = true;
                            break;
                    }

                    workflowBusy.Set();
                };

            Console.WriteLine("Starting workflow");
            workflowApplication.Run();

            WaitIfNotFaulted(workflowBusy, workflowFaulted);

            if (!workflowFaulted)
            {
                switch (PromptEnum<MenuAction>("Workflow waiting"))
                {
                    case MenuAction.Resume:
                        var value = PromptForValue();
                        Console.WriteLine("Main thread resuming bookmark Test with value {0}", value);
                        workflowApplication.ResumeBookmark("Test", value);
                        WaitIfNotFaulted(workflowBusy, workflowFaulted);
                        break;
                    case MenuAction.Cancel:
                        Console.WriteLine("Main thread cancelling workflow");
                        workflowApplication.Cancel();
                        break;
                    case MenuAction.Abort:
                        Console.WriteLine("Main thread aborting workflow");
                        workflowApplication.Abort();
                        break;
                    case MenuAction.Terminate:
                        Console.WriteLine("Main thread terminating workflow");
                        workflowApplication.Terminate(new ApplicationException("Main thread terminate"));
                        break;
                }
            }
            else
            {
                Console.WriteLine("Main thread detected workflow faulted, not resuming");
            }
        }

        /// <summary>
        /// The invoke with standard workflow application.
        /// </summary>
        private static void InvokeWithTasks()
        {
            var shouldThrow = PromptEnum<NoOrYes>("Should the workflow throw an exception") == NoOrYes.Yes;

            var workflowApplication = new WorkflowApplication(new Workflow1 { ShouldThrow = shouldThrow });
            Console.WriteLine("Starting workflow");
            WorkflowEpisodeResult result = null;

            try
            {
                result = workflowApplication.RunEpisodeAsync("Test").Result;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

            if (result != null && result.State != ActivityInstanceState.Faulted)
            {
                switch (PromptEnum<MenuAction>("Workflow waiting"))
                {
                    case MenuAction.Resume:

                        var value = PromptForValue();

                        Console.WriteLine("Main thread resuming bookmark Test with value {0}", value);
                        var completedResult =
                            workflowApplication.ResumeEpisodeBookmarkAsync("Test", value).Result as
                            WorkflowCompletedEpisodeResult;
                        if (completedResult != null)
                        {
                            Console.WriteLine("Workflow completed state is {0}", completedResult.State);

                            switch (result.State)
                            {
                                case ActivityInstanceState.Closed:
                                    Console.WriteLine("Number out argument is {0}", completedResult.Outputs["Value"]);
                                    break;
                                case ActivityInstanceState.Faulted:
                                    Console.WriteLine(
                                        "Workflow faulted termination exception {0}",
                                        completedResult.TerminationException.Message);

                                    break;
                            }
                        }

                        break;
                    case MenuAction.Cancel:
                        Console.WriteLine("Main thread cancelling workflow");
                        workflowApplication.Cancel();
                        break;
                    case MenuAction.Abort:
                        Console.WriteLine("Main thread aborting workflow");
                        workflowApplication.Abort();
                        break;
                    case MenuAction.Terminate:
                        Console.WriteLine("Main thread terminating workflow");
                        workflowApplication.Terminate(new ApplicationException("Main thread terminate"));
                        break;
                }
            }
            else
            {
                Console.WriteLine("Main thread detected workflow faulted, not resuming");
            }
        }

        /// <summary>
        /// The main method.
        /// </summary>
        private static void Main()
        {
            var numChoice = 0;

            while (numChoice != 9)
            {
                Console.Clear();

                var validChoice = false;

                while (!validChoice)
                {
                    Console.WriteLine("Select option");
                    Console.WriteLine("1. Run WorkflowApplication API");
                    Console.WriteLine("2. Run Tasks API ");
                    Console.WriteLine("3. Run and Resume");
                    Console.WriteLine("4. Task Run and Resume");
                    Console.WriteLine("9. Exit");
                    char choice = Console.ReadKey(true).KeyChar;

                    validChoice = int.TryParse(choice.ToString(CultureInfo.InvariantCulture), out numChoice);
                }

                Console.Clear();
                switch (numChoice)
                {
                    case 1:
                        InvokeWithStandardWorkflowApplication();
                        break;
                    case 2:
                        InvokeWithTasks();
                        break;
                    case 3:
                        RunAndResume();
                        break;
                    case 4:
                        TaskRunAndResume();
                        break;
                }

                if (numChoice != 9)
                {
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey(true);
                }
            }
        }

        /// <summary>
        /// The prompt enum.
        /// </summary>
        /// <param name="prompt">
        /// The prompt.
        /// </param>
        /// <typeparam name="T">
        /// The type of the enum
        /// </typeparam>
        /// <returns>
        /// The enum value chosen by the user
        /// </returns>
        /// <exception cref="InvalidEnumArgumentException">
        /// An argument was invalid
        /// </exception>
        private static T PromptEnum<T>(string prompt) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidEnumArgumentException();
            }

            var names = Enum.GetNames(typeof(T));
            var choice = -1;
            while (choice < 0 || choice > names.Length)
            {
                Console.WriteLine(prompt);
                for (var i = 0; i < names.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, names[i]);
                }

                if (!int.TryParse(Console.ReadKey(true).KeyChar.ToString(CultureInfo.InvariantCulture), out choice))
                {
                    choice = -1;
                }
            }

            var value = Enum.GetValues(typeof(T)).GetValue(choice);
            return value != null ? (T)value : default(T);
        }

        /// <summary>
        /// The prompt for value.
        /// </summary>
        /// <returns>
        /// Returns the value entered by the user
        /// </returns>
        private static int PromptForValue()
        {
            int value;

            Console.Write("What integer value do you want to resume with (use -1 to force an exception)? ");
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.WriteLine("Please enter an integer value");
            }

            return value;
        }

        /// <summary>
        /// Runs and resumes the workflow using WorkflowApplication API
        /// </summary>
        private static void RunAndResume()
        {
            var shouldThrow = PromptEnum<NoOrYes>("Should the workflow throw an exception") == NoOrYes.Yes;

            // Developer needs to understand synchronization events
            var workflowBusy = new AutoResetEvent(false);
            var workflowApplication = new WorkflowApplication(new Workflow1 { ShouldThrow = shouldThrow });

            // Developer has to keep track of the faulted state
            var workflowFaulted = false;
            Exception faultException = null;
            var number = 0;

            // Developer has to try and figure out which "Idle" is the correct one 
            // This code is brittle - AsyncCodeActivity can cause idle, Delay can cause idle
            // Release the main thread at the wrong point and you have a problem
            workflowApplication.Idle = obj => workflowBusy.Set();

            // Developer has to understand completion state and how it differs between workflow abort, terminate and cancel
            workflowApplication.Completed = args =>
                {
                    Debug.Assert(args != null, "args != null");
                    switch (args.CompletionState)
                    {
                        case ActivityInstanceState.Closed:
                            Debug.Assert(args.Outputs != null, "args.Outputs != null");
                            var o = args.Outputs["Value"];
                            if (o != null)
                            {
                                number = (int)o;
                            }

                            break;
                        case ActivityInstanceState.Faulted:
                            faultException = args.TerminationException;
                            workflowFaulted = true;
                            break;
                    }

                    workflowBusy.Set();
                };

            workflowApplication.Run();

            // Block this thread until the workflow is ready
            workflowBusy.WaitOne();

            // Developer has to check to see if the workflow faulted every time before waiting      
            if (!workflowFaulted)
            {
                var value = PromptForValue();

                if (workflowApplication.ResumeBookmark("Test", value) == BookmarkResumptionResult.Success)
                {
                    if (!workflowFaulted)
                    {
                        workflowBusy.WaitOne();
                        if (!workflowFaulted)
                        {
                            Console.WriteLine("Workflow completed, number is {0}", number);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error resuming bookmark Test");
                }
            }

            if (workflowFaulted)
            {
                if (faultException != null)
                {
                    Console.WriteLine("Workflow faulted {0}", faultException.Message);
                }
            }
        }

        /// <summary>
        /// Runs and resumes the workflow using a task
        /// </summary>
        private static void TaskRunAndResume()
        {
            var shouldThrow = PromptEnum<NoOrYes>("Should the workflow throw an exception") == NoOrYes.Yes;
            var workflowApplication = new WorkflowApplication(new Workflow1 { ShouldThrow = shouldThrow });

            try
            {
                // Run until there is an idle event with a bookmark named "Test"
                // Can use the sync API for a WorkflowInvoker like experience
                workflowApplication.RunEpisode("Test");

                // or Async API for a task
                // workflowApplication.RunEpisodeAsync("Test").ContinueWith(t => Console.WriteLine("Waiting for bookmark"));

                var value = PromptForValue();

                // Resume and run until complete regardless of idle events
                // Developer does not have to provide synchronization
                // Can use Async API for a Task
                var result = workflowApplication.ResumeEpisodeBookmarkAsync("Test", value).Result;

                var completedResult = (WorkflowCompletedEpisodeResult)result;
                Debug.Assert(completedResult != null, "completedResult != null");
                Debug.Assert(completedResult.Outputs != null, "completedResult.Outputs != null");
                var o = completedResult.Outputs["Value"];
                if (o != null)
                {
                    var number = (int)o;
                    Console.WriteLine("Workflow completed, number is {0}", number);
                }
            }
            catch (AggregateException aggregateException)
            {
                // Tasks use AggregateException
                Debug.Assert(aggregateException.InnerException != null, "aggregateException.InnerException != null");
                Console.WriteLine("Workflow faulted {0}", aggregateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                // Sync API will throw other exceptions
                Console.WriteLine("Workflow faulted {0}", ex.Message);
            }
        }

        /// <summary>
        /// The wait if not faulted.
        /// </summary>
        /// <param name="workflowBusy">
        /// The workflow busy.
        /// </param>
        /// <param name="workflowFaulted">
        /// The workflow faulted.
        /// </param>
        private static void WaitIfNotFaulted(AutoResetEvent workflowBusy, bool workflowFaulted)
        {
            if (!workflowFaulted)
            {
                Console.WriteLine("Main thread waiting for workflow");
                Debug.Assert(workflowBusy != null, "workflowBusy != null");
                workflowBusy.WaitOne();
            }
            else
            {
                Console.WriteLine("Main thread detected workflow faulted, not waiting");
            }
        }

        #endregion
    }
}