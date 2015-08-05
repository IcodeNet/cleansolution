// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTestResults.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System.Activities;
    using System.Activities.Tracking;
    using System.Collections.Generic;

    /// <summary>
    /// The workflow test results.
    /// </summary>
    public class WorkflowTestResults
    {
        #region Constants and Fields

        /// <summary>
        ///   The idle event args.
        /// </summary>
        private readonly List<WorkflowApplicationIdleEventArgs> idleEventArgs =
            new List<WorkflowApplicationIdleEventArgs>();

        /// <summary>
        ///   The tracking records.
        /// </summary>
        private readonly List<TrackingRecord> trackingRecords = new List<TrackingRecord>();

        #endregion

        #region Properties

        /// <summary>
        ///   Gets AbortedArgs.
        /// </summary>
        public WorkflowApplicationAbortedEventArgs AbortedArgs { get; internal set; }

        /// <summary>
        ///   Gets CompletedArgs.
        /// </summary>
        public WorkflowApplicationCompletedEventArgs CompletedArgs { get; internal set; }

        /// <summary>
        ///   Gets EpisodeResult.
        /// </summary>
        public EpisodeEndedWith EpisodeResult { get; internal set; }

        /// <summary>
        ///   Gets ExceptionMessage.
        /// </summary>
        public string ExceptionMessage
        {
            get
            {
                return this.UnhandledExceptionArgs != null
                           ? this.UnhandledExceptionArgs.UnhandledException.Message
                           : null;
            }
        }

        /// <summary>
        ///   Gets IdleEventArgs.
        /// </summary>
        public List<WorkflowApplicationIdleEventArgs> IdleEventArgs
        {
            get
            {
                return this.idleEventArgs;
            }
        }

        /// <summary>
        ///   Gets Output.
        /// </summary>
        public IDictionary<string, object> Output
        {
            get
            {
                return this.CompletedArgs != null ? this.CompletedArgs.Outputs : null;
            }
        }

        /// <summary>
        ///   Gets TrackingRecords.
        /// </summary>
        public List<TrackingRecord> TrackingRecords
        {
            get
            {
                return this.trackingRecords;
            }
        }

        /// <summary>
        ///   Gets UnhandledExceptionArgs.
        /// </summary>
        public WorkflowApplicationUnhandledExceptionEventArgs UnhandledExceptionArgs { get; internal set; }

        /// <summary>
        ///   Gets UnloadedArgs.
        /// </summary>
        public WorkflowApplicationEventArgs UnloadedArgs { get; internal set; }

        #endregion
    }
}