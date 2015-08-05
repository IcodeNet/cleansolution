// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoorConsole
{
    using System;
    using System.Diagnostics;

    using CmdLine;

    /// <summary>
    ///   Sample program for illustrating CodeStateMachine
    /// </summary>
    internal class Program
    {
        #region Constants

        /// <summary>
        /// </summary>
        private const string Title = "Windows Workflow Foundation - Coded StateMachine Example";

        #endregion

        #region Methods

        /// <summary>
        /// Display a menu
        /// </summary>
        /// <returns>
        /// The display menu.
        /// </returns>
        private static char DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("--- Menu --- ");
            Console.WriteLine("[a] Insert Authorized Key");
            Console.WriteLine("[u] Insert Unauthorized Key");
            Console.WriteLine("[x] Exit");
            return CommandLine.PromptKey("Enter command", 'a', 'u', 'x');
        }

        /// <summary>
        /// The main program
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private static void Main(string[] args)
        {
            Console.Title = Title;
            CommandLine.WriteLineColor(ConsoleColor.Yellow, Title);
            Trace.Listeners.Add(new ConsoleTraceListener());
            var door = new SecurityDoor();
            var done = false;
            while (!done)
            {
                CommandLine.WriteLineColor(ConsoleColor.Magenta, "\r\nThe door is now {0}", door.State);

                switch (DisplayMenu())
                {
                    case 'a':
                        door.InsertKey(Guid.NewGuid());
                        break;
                    case 'u':
                        door.InsertKey(Guid.Empty);
                        break;
                    case 'x':
                        done = true;
                        break;
                }
            }

            CommandLine.Pause("Sample complete. Press any key to exit");
        }

        #endregion
    }
}