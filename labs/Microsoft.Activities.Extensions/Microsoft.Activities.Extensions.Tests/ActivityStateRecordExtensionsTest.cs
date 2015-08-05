// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateRecordExtensionsTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Tracking;
    using System.Collections.ObjectModel;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The activity state record extensions test.
    /// </summary>
    [TestClass]
    public class ActivityStateRecordExtensionsTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * An ActivityStateRecord
        ///   When
        ///   * ToFormattedString is invoked with TrackingOption.TypeName
        ///   Then
        ///   * The fullname of the activity type is appended to the string inside paranthesis
        /// </summary>
        [TestMethod]
        public void ToFormattedStringIsFormatted()
        {
            // Arrange
            const string Expected = @"2: Activity [1] ""EchoArg<Int32>"" is Executing
{
	Arguments
	{
		Value: 123
	}
}
";

            WorkflowTrace.Information("Arrange");
            var activity = new EchoArg<int> { Value = new InArgument<int>(123) };
            var tracker = new ActivityStateTracker();
            var workflow = WorkflowApplicationTest.Create(activity);
            workflow.Extensions.Add(tracker);

            // Act
            try
            {
                WorkflowTrace.Information("Act");

                // Run until idle with the Test extension
                workflow.TestWorkflowApplication.RunEpisode();
                var actual = tracker.Records[0].ToFormattedString();

                // Assert
                Assert.AreEqual(Environment.NewLine + Expected, Environment.NewLine + actual);
            }
            finally
            {
                workflow.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An ActivityStateRecord
        ///   When
        ///   * ToFormattedString is invoked with TrackingOption.TypeName
        ///   Then
        ///   * The fullname of the activity type is appended to the string inside paranthesis
        /// </summary>
        [TestMethod]
        public void TypeNameOptionAddsTypeFullName()
        {
            var activity = new Sequence();
            var host = WorkflowInvokerTest.Create(activity);
            var tracker = new ActivityStateTracker();
            host.Extensions.Add(tracker);

            host.TestActivity();

            var record = tracker.Records[0];

            var actual = record.ToFormattedString(TrackingOption.Default | TrackingOption.TypeName);
            var index = actual.IndexOf("(System.Activities.Statements.Sequence)", StringComparison.Ordinal);
            Assert.AreEqual(40, index);
        }

        #endregion

        /// <summary>
        ///   Tracking participant to capture activity tracking records
        /// </summary>
        public class ActivityStateTracker : TypedTrackingParticipant, ITraceable
        {
            #region Fields

            /// <summary>
            ///   The records.
            /// </summary>
            private readonly Collection<ActivityStateRecord> records = new Collection<ActivityStateRecord>();

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets the records.
            /// </summary>
            public ReadOnlyCollection<ActivityStateRecord> Records
            {
                get
                {
                    return new ReadOnlyCollection<ActivityStateRecord>(this.records);
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
                tsb.AppendCollection("Records", this.Records, (record, i) => record.ToFormattedString(tabs: i));
                return tsb.ToString();
            }

            #endregion

            #region Methods

            /// <summary>
            /// The track.
            /// </summary>
            /// <param name="record">
            /// The record. 
            /// </param>
            /// <param name="timeout">
            /// The timeout. 
            /// </param>
            protected override void Track(ActivityStateRecord record, TimeSpan timeout)
            {
                this.records.Add(record);
            }

            #endregion
        }
    }
}