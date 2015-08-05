// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryListener.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    ///   A memory based trace listener
    /// </summary>
    public class MemoryListener : TraceListener
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly StringBuilder buffer = new StringBuilder();

        /// <summary>
        ///   The lock
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        ///   The traces
        /// </summary>
        private readonly List<string> traces = new List<string>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Traces.
        /// </summary>
        public SynchronizedReadOnlyCollection<string> Records
        {
            get
            {
                return new SynchronizedReadOnlyCollection<string>(this.syncLock, this.traces);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   When overridden in a derived class, flushes the output buffer.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
        {
            lock (this.syncLock)
            {
                if (this.buffer.Length > 0)
                {
                    this.traces.Add(this.buffer.ToString());
                    this.buffer.Clear();
                }
            }
        }

        /// <summary>
        /// Traces the class
        /// </summary>
        public void Trace()
        {
            WorkflowTrace.Information("MemoryListener has {0} record(s)", this.Records.Count);
            foreach (var t in this.Records)
            {
                WorkflowTrace.Information(t);
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">
        /// A message to write. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override void Write(string message)
        {
            lock (this.syncLock)
            {
                this.buffer.Append(message);
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">
        /// A message to write. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override void WriteLine(string message)
        {
            lock (this.syncLock)
            {
                this.Write(message);
                this.Flush();
            }
        }

        #endregion
    }
}