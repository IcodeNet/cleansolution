// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// Helper for writing debug messages
    /// </summary>
    public static class TraceHelper
    {
        #region Public Methods

        /// <summary>
        /// Outputs a debug message with the thread
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// optional args.
        /// </param>
        public static void Write(string format, params object[] args)
        {
            WorkflowTrace.Information(
                string.Format("{0,3:0} {1}", Thread.CurrentThread.ManagedThreadId, string.Format(format, args)));
        }

        #endregion
    }
}