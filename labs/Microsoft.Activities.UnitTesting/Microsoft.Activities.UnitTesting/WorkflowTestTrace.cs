// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTrace.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System.Diagnostics;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   The test trace builder.
    /// </summary>
    public class WorkflowTestTrace : TraceStringBuilder
    {
        #region Public Methods and Operators

        /// <summary>
        ///   The act.
        /// </summary>
        public static void Act()
        {
            WorkflowTrace.Verbose(new string('-', 80));
            WorkflowTrace.NewLine();
            WorkflowTrace.Verbose("Act");
        }

        /// <summary>
        ///   The arrange.
        /// </summary>
        public static void Arrange()
        {
            WorkflowTrace.Level = TraceLevel.Verbose;
            WorkflowTrace.Options = TraceOptions.ThreadId;
            WorkflowTrace.Verbose("Arrange");
        }

        /// <summary>
        ///   The act.
        /// </summary>
        public static void Assert()
        {
            WorkflowTrace.Verbose(new string('-', 80));
            WorkflowTrace.NewLine();
            WorkflowTrace.Verbose("Assert");
        }

        /// <summary>
        /// The finally.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        public static void Finally(string msg = "Finally")
        {
            WorkflowTrace.Verbose(new string('-', 80));
            WorkflowTrace.Verbose(msg);
            WorkflowTrace.Options = TraceOptions.None;
            WorkflowTrace.NewLine();
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        public static void Write(string format, params object[] args)
        {
            WorkflowTrace.Verbose(format, args);
        }

        #endregion
    }
}