// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingRecordExtensionsTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities.Statements;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The tracking record extensions test.
    /// </summary>
    [TestClass]
    public class TrackingRecordExtensionsTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A TrackingRecord
        ///   When
        ///   * ToFormattedString is invoked with the RecordNumber option
        ///   Then
        ///   * The string returned starts with "0: "
        /// </summary>
        [TestMethod]
        public void GetRecordNumberIncludedWhenOptionRecordNumber()
        {
            // Arrange
            var activity = new Sequence();
            var listTrackingParticipant = new ListTrackingParticipant();
            var host = new WorkflowInvokerTest(activity);
            host.Extensions.Add(listTrackingParticipant);
            host.TestActivity();
            var record = listTrackingParticipant.Records.First();

            try
            {
                // Act
                var actual = record.ToFormattedString(TrackingOption.RecordNumber);

                // Assert
                Assert.IsTrue(actual.StartsWith("0: "));
            }
            finally
            {
                listTrackingParticipant.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A TrackingRecord
        ///   When
        ///   * ToFormattedString is invoked with the RecordNumber option
        ///   Then
        ///   * The string returned starts with "0: "
        /// </summary>
        [TestMethod]
        public void GetRecordNumberNotIncludedWhenNoOptionRecordNumber()
        {
            // Arrange
            var activity = new Sequence();
            var host = WorkflowApplicationTest.Create(activity);
            host.TestWorkflowApplication.RunEpisode();
            var record = host.Tracking.Records.First();
            string actual = null;

            try
            {
                // Act
                actual = record.ToFormattedString(TrackingOption.None);

                // Assert
                Assert.IsFalse(actual.StartsWith("0: "));
            }
            finally
            {
                WorkflowTrace.Information("Actual is <{0}>", actual);
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}