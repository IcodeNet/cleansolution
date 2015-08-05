// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestAsyncTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for TestAsyncTest and is intended
    ///   to contain all TestAsyncTest Unit Tests
    /// </summary>
    [TestClass]
    public class TestAsyncTest
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
        /// Verifies that the TestAsync activity sleeps for the specified interval
        /// </summary>
        [TestMethod]
        public void TestAsyncShouldSleepForTheSpeciedInterval()
        {
            const int SleepFor = 100;
            var host = WorkflowApplicationTest.Create(new TestAsync { Sleep = SleepFor });

            try
            {
                var s = new Stopwatch();
                s.Start();
                var result = host.TestWorkflowApplication.RunEpisode();
                s.Stop();

                var errorMessage = string.Format(
                    "Expected TestAsync to sleep for more than {0} milliseconds actual {1}", 
                    SleepFor, 
                    s.ElapsedMilliseconds);

                // Note: I have found that it may be off by as  much as 10%
                Assert.IsTrue(s.ElapsedMilliseconds + 10 >= SleepFor, errorMessage);

                Assert.AreEqual(ActivityInstanceState.Closed, result.State);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}