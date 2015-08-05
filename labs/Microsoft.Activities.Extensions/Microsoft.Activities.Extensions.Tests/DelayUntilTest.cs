// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.Statements;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This is a test class for DelayUntilTest and is intended
    ///   to contain all DelayUntilTest Unit Tests
    /// </summary>
    [TestClass]
    public class DelayUntilTest
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   - A workflow that needs to wait until a specific time
        ///   - An activity that will delay until that time
        ///   When
        ///   - The activity is executed
        ///   Then
        ///   - The workflow delays until that time
        /// </summary>
        [TestMethod]
        public void DelayUntilTimeNowPlus1Day1Sec()
        {
            DateTime until;

            DateTime.TryParse(DateTime.Now.ToShortTimeString(), out until);

            var days = new List<DayOfWeek>
                {
                   DateTime.Now.AddDays(1).DayOfWeek, DateTime.Now.DayOfWeek, DateTime.Now.AddDays(-1).DayOfWeek 
                };

            var activity = new DelayUntilTime { OccurenceDays = days };
            dynamic input = new WorkflowArguments();
            input.Time = DateTime.Now.AddSeconds(1).TimeOfDay;

            var host = new WorkflowInvokerTest(activity);

            try
            {
                host.TestActivity(input, Constants.Timeout);

                host.Tracking.Assert.Exists(
                    "DelayUntilTime", ActivityInstanceState.Closed, "DelayUntilTime activity failed to close");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   - A workflow that needs to wait until a specific time
        ///   - An activity that will delay until that time
        ///   When
        ///   - The activity is executed
        ///   Then
        ///   - The workflow delays until that time
        /// </summary>
        [TestMethod]
        public void DelayUntilTimeNowPlus1Sec()
        {
            var activity = new DelayUntilDateTime { UntilDate = DateTime.Now + TimeSpan.FromSeconds(1) };

            var host = new WorkflowInvokerTest(activity);

            try
            {
                host.TestActivity(Constants.Timeout);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given 
        ///   * A XAML activity with a DelayUntilTime activity
        ///   * And all days are selected
        ///   * And a WriteLine after it
        ///   When
        ///   * The XAML activity is invoked
        ///   * with the current time plus one second
        ///   Then
        ///   * The activity should delay for about 1 second (slightly less)
        ///   * The WriteLine should complete
        /// </summary>
        [TestMethod]
        public void DelayUntilTimeXamlNowPlus1Day1Sec()
        {
            var time = DateTime.Now.AddSeconds(2).TimeOfDay;
            var activity = new DelayUntilTimeTest { UntilTime = time };
            var host = new WorkflowInvokerTest(activity);
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                host.TestActivity(Constants.Timeout);
                stopwatch.Stop();
            }
            finally
            {
                host.Tracking.Trace();
            }

            Assert.IsTrue(stopwatch.ElapsedMilliseconds > 900);
        }

        #endregion
    }
}