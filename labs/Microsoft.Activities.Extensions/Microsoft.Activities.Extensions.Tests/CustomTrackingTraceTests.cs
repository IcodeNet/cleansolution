// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTrackingTraceTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for custom tracking
    /// </summary>
    [TestClass]
    public class CustomTrackingTraceTests
    {
        #region Constants

        /// <summary>
        /// The test value
        /// </summary>
        public const int TestValue = 1;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A Custom Tracking Record
        ///   When
        ///   * ToFormattedString is invoked
        ///   Then
        ///   * The custom formatter is invoked
        /// </summary>
        [TestMethod]
        public void CustomTraceShouldBeInvoked()
        {
            var activity = new TestCustomActivity();
            var host = WorkflowInvokerTest.Create(activity);
            try
            {
                host.TestActivity(Constants.Timeout);
                host.Tracking.Assert.Exists<TestCustomTrackingRecord>(
                    r => r.GetType() == typeof(TestCustomTrackingRecord), 0);
                foreach (var record in host.Tracking.Records)
                {
                    record.ToFormattedString();
                }

                Assert.IsTrue(TestCustomTrackingRecord.FormatInvoked);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}