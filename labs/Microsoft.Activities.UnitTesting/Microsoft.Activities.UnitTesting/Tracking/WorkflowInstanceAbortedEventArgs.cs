// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceAbortedEventArgs.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities.Tracking;

    /// <summary>
    /// The workflow instance aborted args.
    /// </summary>
    public class WorkflowInstanceAbortedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceAbortedEventArgs"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        public WorkflowInstanceAbortedEventArgs(TrackingRecord record)
        {
            this.Record = (WorkflowInstanceAbortedRecord)record;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Record.
        /// </summary>
        public WorkflowInstanceAbortedRecord Record { get; set; }

        #endregion
    }
}