// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UsingTypedTracking
{
    using System.Activities;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Example program that demonstrates how to use a TypedTrackingParticipant
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        ///   The Workflow Definition
        /// </summary>
        private static readonly Activity Workflow1Definition = new Workflow1();

        #endregion

        #region Methods

        /// <summary>
        /// The Main program
        /// </summary>
        private static void Main()
        {
            var invoker = new WorkflowInvoker(Workflow1Definition);
            invoker.Extensions.Add(new WriteLineTracker());
            invoker.Extensions.Add(new FileTracker("Tracking.txt"));
            invoker.Invoke();
        }

        #endregion
    }
}