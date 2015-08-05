// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingExtensionsTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities.Statements;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Test for the tracking extensions
    /// </summary>
    [TestClass]
    public class TrackingExtensionsTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * A sequence
        ///   * A tracking participant that calls ToFormattedString
        ///   When
        ///   * The workflow is invoked
        ///   Then
        ///   * The tracking participant will invoke ToFormattedString
        /// </summary>
        [TestMethod]
        public void FormatStringShouldBeInvoked()
        {
            const string Expected = "0: WorkflowInstance \"Sequence\" is Started";

            var activity = new Sequence();
            var host = WorkflowInvokerTest.Create(activity);
            var observer = new Observer();
            host.Extensions.Add(observer);
            WorkflowTrace.Options = TraceOptions.ThreadId;
            host.Extensions.Add(new TraceTrackingParticipant());

            try
            {
                host.TestActivity(Constants.Timeout);
                Assert.AreEqual(5, observer.Records.Count);
                Assert.AreEqual(Expected, observer.Records[0]);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion

        /// <summary>
        ///   An observer to watch the tracking records
        /// </summary>
        private class Observer : TrackingParticipant
        {
            #region Fields

            /// <summary>
            ///   The list of records
            /// </summary>
            private readonly List<string> records = new List<string>();

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets Records.
            /// </summary>
            public List<string> Records
            {
                get
                {
                    return this.records;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// When implemented in a derived class, used to synchronously process the tracking record.
            /// </summary>
            /// <param name="record">
            /// The generated tracking record. 
            /// </param>
            /// <param name="timeout">
            /// The time period after which the provider aborts the attempt. 
            /// </param>
            protected override void Track(TrackingRecord record, TimeSpan timeout)
            {
                this.Records.Add(record.ToFormattedString());
            }

            #endregion
        }
    }
}