// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestExtension.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   A test extension
    /// </summary>
    public sealed class TestExtension : IDisposable, ITraceable
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the invoke count.
        /// </summary>
        public int InvokeCount { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Disposes of the extension
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("Disposing TestExtension");
        }

        /// <summary>
        ///   Sample method
        /// </summary>
        public void DoSomething()
        {
            Debug.WriteLine("TestExtension.DoSomething()");
        }

        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="tabs">
        /// the tabs The tabs. 
        /// </param>
        /// <returns>
        /// The System.String. 
        /// </returns>
        public string ToFormattedString(int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs);
            tsb.AppendTitle(this);
            using (tsb.IndentBlock())
            {
                tsb.AppendProperty("InvokeCount", this.InvokeCount);
            }

            return tsb.ToString();
        }

        /// <summary>
        /// The trace.
        /// </summary>
        public void Trace()
        {
            WorkflowTrace.Information(this.ToFormattedString());
        }

        #endregion
    }
}