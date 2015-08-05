// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Console
{
    using System;
    using System.Activities;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The main method.
        /// </summary>
        private static void Main()
        {
            const string SampleTitle = "Collection Activities Sample";

            Console.Title = SampleTitle;
            Console.WriteLine("Windows Workflow Foundation (WF4)");
            Console.WriteLine(SampleTitle);
            Console.WriteLine();

            WorkflowInvoker.Invoke(new CollectionActivities());

            Console.WriteLine();
            Console.WriteLine("Sample Completed");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        #endregion
    }
}