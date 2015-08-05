// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceTrackingParticipant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Diagnostics
{
    using System;
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Outputs tracking records to System.Diagnostics.Trace
    /// </summary>
    public class TraceTrackingParticipant : TrackingParticipant
    {
        #region Fields

        /// <summary>
        ///   The tracking options
        /// </summary>
        private readonly TrackingOption option;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TraceTrackingParticipant" /> class.
        /// </summary>
        public TraceTrackingParticipant()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceTrackingParticipant"/> class.
        /// </summary>
        /// <param name="option">
        /// The tracking options 
        /// </param>
        public TraceTrackingParticipant(TrackingOption option)
        {
            this.option = option;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Track the record
        /// </summary>
        /// <param name="record">
        /// The record to track 
        /// </param>
        /// <param name="timeout">
        /// The timeout value 
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            record.Trace(this.option);
        }

        #endregion
    }
}