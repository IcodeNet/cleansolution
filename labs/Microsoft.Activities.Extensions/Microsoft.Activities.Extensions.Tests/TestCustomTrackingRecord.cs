// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCustomTrackingRecord.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities.Tracking;
    using System.Diagnostics;
    using System.Globalization;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The test custom tracking record.
    /// </summary>
    public class TestCustomTrackingRecord : CustomTrackingRecord, ICustomTrackingTrace
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCustomTrackingRecord"/> class.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        public TestCustomTrackingRecord(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCustomTrackingRecord"/> class.
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="level">
        /// The level. 
        /// </param>
        public TestCustomTrackingRecord(string name, TraceLevel level)
            : base(name, level)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCustomTrackingRecord"/> class.
        /// </summary>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="level">
        /// The level. 
        /// </param>
        public TestCustomTrackingRecord(Guid instanceId, string name, TraceLevel level)
            : base(instanceId, name, level)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCustomTrackingRecord"/> class.
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        protected TestCustomTrackingRecord(CustomTrackingRecord record)
            : base(record)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether format invoked.
        /// </summary>
        public static bool FormatInvoked { get; set; }

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        public int Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns a formatted string
        /// </summary>
        /// <param name="option">
        /// The tracking options 
        /// </param>
        /// <param name="tabs">
        /// The tabs. 
        /// </param>
        /// <returns>
        /// The formatted string 
        /// </returns>
        public string ToFormattedString(TrackingOption option = TrackingOption.Default, int tabs = 0)
        {
            FormatInvoked = true;
            return this.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Trace your tracking record
        /// </summary>
        /// <param name="option">
        /// The tracking options 
        /// </param>
        /// <param name="source">
        /// The trace source 
        /// </param>
        public void Trace(TrackingOption option, TraceSource source)
        {
            WorkflowTrace.Information(((TrackingRecord)this).ToFormattedString(option));
        }

        #endregion
    }
}