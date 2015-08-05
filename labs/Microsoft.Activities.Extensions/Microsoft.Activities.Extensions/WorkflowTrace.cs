// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTrace.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Provides tracing from the Microsoft.Activities.Extensions
    /// </summary>
    public static class WorkflowTrace
    {
        #region Constants

        /// <summary>
        ///   The source name
        /// </summary>
        public const string SourceName = "Microsoft.Activities.Extensions";

        #endregion

        #region Static Fields

        /// <summary>
        ///   The debugger attached source.
        /// </summary>
        private static readonly TraceSource DebuggerAttachedSource = new TraceSource(
            SourceName, SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning | SourceLevels.Information);

        /// <summary>
        ///   The no debugger attached source.
        /// </summary>
        private static readonly TraceSource NoDebuggerSource = new TraceSource(SourceName, SourceLevels.Information);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="WorkflowTrace" /> class.
        /// </summary>
        static WorkflowTrace()
        {
            UseGlobalTrace = true;
            Level = TraceLevel.Info;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the trace level to control publishing of events with global tracing
        /// </summary>
        public static TraceLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public static TraceOptions Options { get; set; }

        /// <summary>
        ///   Gets the source.
        /// </summary>
        public static TraceSource Source
        {
            get
            {
                return Debugger.IsAttached ? DebuggerAttachedSource : NoDebuggerSource;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether use global trace.
        /// </summary>
        public static bool UseGlobalTrace { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        public static void Information(string format, params object[] args)
        {
            if (UseGlobalTrace)
            {
                if (Level >= TraceLevel.Info)
                {
                    WriteTrace(format, args);
                }
            }
            else
            {
                if (!args.IsNullOrEmpty())
                {
                    Source.TraceInformation(format, args);
                }
                else
                {
                    Source.TraceInformation(format);
                }
            }
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="traceStringBuilder">
        /// The trace string builder. 
        /// </param>
        public static void Information(TraceStringBuilder traceStringBuilder)
        {
            Information(traceStringBuilder.ToString());
        }

        /// <summary>
        /// The new line.
        /// </summary>
        public static void NewLine()
        {
            Trace.WriteLine(string.Empty);
        }

        /// <summary>
        /// Trace a Error event
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        public static void Error(string format, params object[] args)
        {
            if (UseGlobalTrace)
            {
                if (Level >= TraceLevel.Error)
                {
                    WriteTrace(format, args);
                }
            }
            else
            {
                if (!args.IsNullOrEmpty())
                {
                    Source.TraceEvent(TraceEventType.Error, 0, format, args);
                }
                else
                {
                    Source.TraceEvent(TraceEventType.Error, 0, format);
                }
            }
        }

        /// <summary>
        /// Trace a Verbose event
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        public static void Verbose(string format, params object[] args)
        {
            if (UseGlobalTrace)
            {
                if (Level >= TraceLevel.Verbose)
                {
                    WriteTrace(format, args);
                }
            }
            else
            {
                if (!args.IsNullOrEmpty())
                {
                    Source.TraceEvent(TraceEventType.Verbose, 0, format, args);
                }
                else
                {
                    Source.TraceEvent(TraceEventType.Verbose, 0, format);
                }
            }
        }

        /// <summary>
        /// Trace a Warning event
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        public static void Warning(string format, params object[] args)
        {
            if (UseGlobalTrace)
            {
                if (Level >= TraceLevel.Warning)
                {
                    WriteTrace(format, args);
                }
            }
            else
            {
                if (!args.IsNullOrEmpty())
                {
                    Source.TraceEvent(TraceEventType.Warning, 0, format, args);
                }
                else
                {
                    Source.TraceEvent(TraceEventType.Warning, 0, format);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The write trace.
        /// </summary>
        /// <param name="format">
        /// The format. 
        /// </param>
        /// <param name="args">
        /// The args. 
        /// </param>
        private static void WriteTrace(string format, object[] args)
        {
            var message = !args.IsNullOrEmpty() ? string.Format(format, args) : format;
            if (Options.HasFlag(TraceOptions.ThreadId))
            {
                Trace.WriteLine(string.Format("[{0,2:00}] {1}", Thread.CurrentThread.ManagedThreadId, message));
            }
            else
            {
                Trace.WriteLine(message);
            }
        }

        #endregion

        public static void Error(Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }
}