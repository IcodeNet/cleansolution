// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListTrackingParticipant.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Threading;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   The memory tracking participant.
    /// </summary>
    public class ListTrackingParticipant : TrackingParticipant, ITraceable
    {
        #region Fields

        /// <summary>
        ///   The records.
        /// </summary>
        private readonly List<TrackingRecord> records = new List<TrackingRecord>();

        /// <summary>
        /// The sync lock.
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        ///   The options.
        /// </summary>
        private TrackingOption option = TrackingOption.Default;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the options.
        /// </summary>
        public TrackingOption Option
        {
            get
            {
                return this.option;
            }

            set
            {
                this.option = value;
            }
        }

        /// <summary>
        ///   Gets Records.
        /// </summary>
        public ReadOnlyCollection<TrackingRecord> Records
        {
            get
            {
                lock (this.syncLock)
                {
                    return new ReadOnlyCollection<TrackingRecord>(this.records);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

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
            tsb.AppendTitle(this.GetType().Name);
            using (tsb.IndentBlock())
            {
                var trackingProfile = this.TrackingProfile;
                if (trackingProfile != null)
                {
                    tsb.AppendLine(trackingProfile.ToFormattedString(tabs));
                }

                var trackingRecords = this.Records;
                if (trackingRecords != null)
                {
                    lock (this.syncLock)
                    {
                        foreach (var trackingRecord in trackingRecords)
                        {
                            tsb.AppendLine(trackingRecord.ToFormattedString(this.Option, tsb.Tabs).TrimStart());
                        }
                    }
                }
                else
                {
                    tsb.AppendLine(SR.No_tracking_records_to_trace);
                }
            }

            return tsb.ToString();
        }

        /// <summary>
        ///   Traces all the tracking records
        /// </summary>
        public void Trace()
        {
            WorkflowTrace.Information(this.ToFormattedString());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Stores a tracking record.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            lock (this.syncLock)
            {
                this.records.Add(record);
            }
        }

        #endregion
    }
}