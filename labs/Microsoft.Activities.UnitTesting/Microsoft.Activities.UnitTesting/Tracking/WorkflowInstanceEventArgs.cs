// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceEventArgs.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The workflow instance event args.
    /// </summary>
    public class WorkflowInstanceEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceEventArgs"/> class.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        public WorkflowInstanceEventArgs(TrackingRecord record)
        {
            this.Record = (WorkflowInstanceRecord)record;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Record.
        /// </summary>
        public WorkflowInstanceRecord Record { get; set; }

        /// <summary>
        ///   Gets the State.
        /// </summary>
        public WorkflowInstanceRecordState State
        {
            get
            {
                return this.Record.GetState();
            }
        }

        #endregion
    }
}