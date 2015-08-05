// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingTraceTrackingParticipant
{
    using System;
    using System.Activities;
    using System.Diagnostics;

    using CmdLine;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    /// The sample program
    /// </summary>
    internal class Program
    {
        #region Constants

        /// <summary>
        /// App Title
        /// </summary>
        private const string Title = "Windows Workflow Foundation - TraceTrackingParticipant Example";

        #endregion

        #region Static Fields

        /// <summary>
        ///   The workflow definition
        /// </summary>
        private static readonly Workflow1 Workflow1Definition = new Workflow1();

        #endregion

        #region Methods

        /// <summary>
        /// The main program
        /// </summary>
        private static void Main()
        {
            Console.Title = Title;
            CommandLine.WriteLineColor(ConsoleColor.Yellow, Title);
            CommandLine.WriteLineColor(ConsoleColor.Cyan, "Demonstrates how you can output tracking to a TraceListener");

            // Setup the trace listener
            var consoleTraceListener = new ConsoleTraceListener();

            // Setup the trace source
            Trace.Listeners.Add(consoleTraceListener);

            // Setup the workflow host
            var invoker = new WorkflowInvoker(Workflow1Definition);
            invoker.Extensions.Add(new TraceTrackingParticipant());

            // As the workflow is invoked, the tracking data will be output to the console
            invoker.Invoke();

            CommandLine.Pause();

            Console.WriteLine("Sample Complete");
            Console.ReadKey(true);
        }

        #endregion
    }
}