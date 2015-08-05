// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncrementServiceTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MathServiceLibrary.Tests
{
    using System.Activities;

    using MathServiceLibrary.Tests.IncrementService;

    using Microsoft.Activities;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The increment service test.
    /// </summary>
    [TestClass]
    public class IncrementServiceTest
    {
        #region Constants

        /// <summary>
        ///   The MathServiceLibrary folder
        /// </summary>
        private const string MathServiceLibrary = Labs.UnitTestingExamples + @"MathServiceLibrary\MathServiceLibrary\";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   The increment service should increment data.
        /// </summary>
        /// <remarks>
        ///   // You must deploy the XAMLX file to the test directory
        /// </remarks>
        [TestMethod]
        [DeploymentItem(MathServiceLibrary + "IncrementService.xamlx")]
        public void IncrementServiceShouldIncrementData()
        {
            // Arrange
            const int InitialData = 1;
            const int ExpectedData = 2;

            WorkflowServiceTestHost host = null;
            try
            {
                var address = ServiceTest.GetUniqueEndpointAddress();
                using (host = WorkflowServiceTestHost.Open("IncrementService.xamlx", address))
                {
                    var proxy = new ServiceClient(ServiceTest.Pipe, address);
                    int? value = InitialData;
                    proxy.Increment(ref value);
                    Assert.AreEqual(ExpectedData, value, "Increment did not correctly increment the value");
                }

                // The host must be closed before asserting tracking
                // Explicitly call host.Close or exit the using block to do this.

                // Assert that the Assign activity was executed with an argument named "Value" which contains the value 2
                host.Tracking.Assert.ExistsArgValue("Assign", ActivityInstanceState.Closed, "Value", 2);
            }
            finally
            {
                if (host != null)
                {
                    host.Tracking.Trace();
                }
            }
        }

        #endregion
    }
}