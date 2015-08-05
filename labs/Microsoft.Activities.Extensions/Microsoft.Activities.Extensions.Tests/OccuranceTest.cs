// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OccuranceTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for OccuranceTest and is intended
    ///   to contain all OccuranceTest Unit Tests
    /// </summary>
    [TestClass]
    public class OccuranceTest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given
        ///   * A from date time of 1/10/2011 12:00am
        ///   * A next occurance time of 5:00pm
        ///   * on the next Wed
        ///   When 
        ///   * Occurance.Interval is invoked
        ///   Then
        ///   * The time to next occurance should be a timespan of 2 hours
        /// </summary>
        [TestMethod]
        public void GetNextDate()
        {
            var fromTime = new DateTime(2011, 1, 10);
            var time = TimeSpan.FromHours(17);
            var days = new List<DayOfWeek> { DayOfWeek.Wednesday };
            var expected = new DateTime(2011, 1, 12, 17, 0, 0);
            var actual = Occurance.Next(fromTime, time, days);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Given
        ///   * From 1/23/2011 13:00
        ///   * A next occurance time of 2:00
        ///   * No days of the week
        ///   When 
        ///   * Occurance.Interval is invoked
        ///   Then
        ///   * The time to next occurance should be a timespan of 13 hours
        /// </summary>
        [TestMethod]
        public void TimeToNext2Am()
        {
            var fromTime = new DateTime(2011, 1, 23, 13, 00, 0);
            var time = TimeSpan.FromHours(2);
            var expected = TimeSpan.FromHours(13);
            var actual = Occurance.Interval(fromTime, time);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Given
        ///   * A from date time of 1/1/2011 12:00am
        ///   * A next occurance time of 2:00am
        ///   * on the next Friday
        ///   When 
        ///   * Occurance.Interval is invoked
        ///   Then
        ///   * The time to next occurance should be a timespan of 2 hours
        /// </summary>
        [TestMethod]
        public void TimeToNext2AmOnNextFriday()
        {
            var fromTime = new DateTime(2011, 1, 1);
            var time = TimeSpan.FromHours(2);
            var days = new List<DayOfWeek> { DayOfWeek.Friday };
            var expected = TimeSpan.FromHours(2) + TimeSpan.FromDays(6);
            var actual = Occurance.Interval(fromTime, time, days);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Given
        ///   * A from date time of 1/1/2011 2:00
        ///   * A next occurance time of 2:00
        ///   * No days of the week
        ///   When 
        ///   * Occurance.Interval is invoked
        ///   Then
        ///   * The time to next occurance should be zero
        /// </summary>
        [TestMethod]
        public void TimeToNext2AmOnSameDay()
        {
            var fromTime = new DateTime(2011, 1, 1, 2, 0, 0);
            var time = TimeSpan.FromHours(2);
            var expected = TimeSpan.Zero;
            var actual = Occurance.Interval(fromTime, time, null);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Given
        /// * Day of the week is Sunday
        /// * Allowed days include Sunday, Wed
        /// When 
        /// * Occurance.Next is invoked
        /// </summary>
        [TestMethod]
        public void NextOccurranceOnSameDay17()
        {
            var fromTime = new DateTime(2011, 1, 23, 12, 0, 0);
            var time = TimeSpan.FromHours(17);
            var expected = new DateTime(2011, 1, 23, 17, 0, 0);
            var days = new List<DayOfWeek> { DayOfWeek.Sunday, DayOfWeek.Thursday };
            var actual = Occurance.Next(fromTime, time, days);
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}