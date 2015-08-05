// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAverageTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WorkflowExtensionExample.Tests
{
    using System;
    using System.Activities;

    using Microsoft.Activities;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WorkflowExtensionExample.ServiceReference1;

    /// <summary>
    ///   This is a test class for GetAverageTest and is intended
    ///   to contain all GetAverageTest Unit Tests
    /// </summary>
    [TestClass]
    public class GetAverageTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Tests the GetAverage Activity by invoking it in a WorkflowService multiple times
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.UnitTestingExamples + @"UnitTestingWorkflowServices\Service1.xamlx")]
        public void ShouldInvokeAverageExtensionWithService()
        {
            const string Expected1 = "Stored 33, Average:33";
            const string Expected2 = "Stored 44, Average:38.5";
            const string Expected3 = "Stored 55, Average:44";

            // Reset the static collection
            AverageExtension.Reset();

            // Self-Host Service1.xamlx using Named Pipes
            var address = ServiceTest.GetUniqueEndpointAddress();
            using (WorkflowServiceTestHost.Open("Service1.xamlx", address))
            {
                string result1;
                string result2;
                string result3;

                // Use the generated proxy with named pipes 
                var proxy = new ServiceClient(ServiceTest.Pipe, address);

                try
                {
                    result1 = proxy.GetData(33);
                    result2 = proxy.GetData(44);
                    result3 = proxy.GetData(55);
                    proxy.Close();
                }
                catch (Exception)
                {
                    proxy.Abort();
                    throw;
                }

                Assert.AreEqual(Expected1, result1);
                Assert.AreEqual(Expected2, result2);
                Assert.AreEqual(Expected3, result3);
            }
        }

        /// <summary>
        ///   Verify the GetAverageExtention
        /// </summary>
        [TestMethod]
        public void ShouldInvokeGetAverageExtension()
        {
            const string Expected1 = "Stored 33, Average:33";
            const string Expected2 = "Stored 44, Average:38.5";
            const string Expected3 = "Stored 55, Average:44";

            // Reset the static collection
            AverageExtension.Reset();

            var result1 = WorkflowInvoker.Invoke(new GetAverage { Number = 33 });
            var result2 = WorkflowInvoker.Invoke(new GetAverage { Number = 44 });
            var result3 = WorkflowInvoker.Invoke(new GetAverage { Number = 55 });

            Assert.AreEqual(Expected1, result1);
            Assert.AreEqual(Expected2, result2);
            Assert.AreEqual(Expected3, result3);
        }

        #endregion
    }
}