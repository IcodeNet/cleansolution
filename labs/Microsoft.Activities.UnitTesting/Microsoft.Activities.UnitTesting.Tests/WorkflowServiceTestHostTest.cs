// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowServiceTestHostTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Activities.Description;

    using Microsoft.Activities.UnitTesting.Persistence;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This test demonstrates the WorkflowServiceTestHost class
    /// </summary>
    [TestClass]
    public class WorkflowServiceTestHostTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Demonstrates how to test a service with correlation
        /// </summary>
        /// <remarks>
        ///   Be sure to enable deployment - the xamlx file must be deployed
        /// </remarks>
        [TestMethod]
        [DeploymentItem(Constants.TestActivitiesDir + "ServiceWithCorrelation.xamlx")]
        public void ShouldCorrelateServiceCalls()
        {
            var serviceAddress = ServiceTest.GetUniqueEndpointAddress();
            using (var host = new WorkflowServiceTestHost("ServiceWithCorrelation.xamlx", serviceAddress.Uri))
            {
                // Add an idle behavior to unload as soon as idle is detected
                host.Host.Description.Behaviors.Add(new WorkflowIdleBehavior { TimeToUnload = TimeSpan.Zero });
                host.Open();

                var client = ChannelFactory<IServiceWithCorrelation>.CreateChannel(ServiceTest.Pipe, serviceAddress);
                Trace.WriteLine("Test Client: Sending GetData(1)");
                var response = client.GetData(1);
                Assert.AreEqual("1", response.Text);

                Trace.WriteLine(string.Format("Test Client: Received GetData response {0} with key {1}", response.Text, response.Key));

                // Wait for unload
                Assert.IsTrue(host.WaitForInstanceUnloaded(2000), "Timeout waiting for instance unloaded");

                // If you want to see what is in the memory store, dump it
                // MemoryStore.Dump();
                Trace.WriteLine(string.Format("Test Client:  Sending GetMoreData(2, {0})", response.Key));
                var secondResponse = client.GetMoreData(2, response.Key);
                Assert.AreEqual("2", secondResponse.Text);

                Trace.WriteLine(string.Format("Test Client: Received GetMoreData response {0} with key {1}", secondResponse.Text, secondResponse.Key));

                host.WaitForInstanceDeleted();
                host.Close();
                MemoryStore.DisplayCommandCounts();
                host.WaitForHostClosed();

                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Verifies that the host throws an exception and faults the channel
        /// </summary>
        /// <remarks>
        ///   Be sure to enable deployment - the xamlx file must be deployed
        /// </remarks>
        [TestMethod]
        [DeploymentItem(Constants.TestActivitiesDir + "TestService.xamlx")]
        public void ShouldFaultProxyWhenLessThanZero()
        {
            var serviceAddress = ServiceTest.GetUniqueEndpointAddress();
            using (var testHost = new WorkflowServiceTestHost("TestService.xamlx", serviceAddress))
            {
                testHost.Open();
                var client = ChannelFactory<ITestService>.CreateChannel(ServiceTest.Pipe, serviceAddress);
                AssertHelper.Throws<FaultException>(() => client.GetData(-1));

                testHost.Close();
            }
        }

        /// <summary>
        ///   Verifies that the WorkflowServiceTestHost hosts a service and that the service receives and sends a reply
        /// </summary>
        /// <remarks>
        ///   Be sure to enable deployment - the xamlx file must be deployed
        /// </remarks>
        [TestMethod]
        [DeploymentItem(Constants.TestActivitiesDir + "TestService.xamlx")]
        public void ShouldHostService()
        {
            var trackingProfile = new TrackingProfile { Queries = { new ActivityStateQuery { ActivityName = "ReceiveRequest", States = { "Executing" }, }, new ActivityStateQuery { ActivityName = "SendResponse", States = { "Executing" }, }, } };

            var serviceAddress = ServiceTest.GetUniqueEndpointAddress();
            using (var testHost = WorkflowServiceTestHost.Open("TestService.xamlx", serviceAddress))
            {
                testHost.TrackingProfile = trackingProfile;
                var client = ChannelFactory<ITestService>.CreateChannel(ServiceTest.Pipe, serviceAddress);
                var response = client.GetData(1);
                Assert.AreEqual("1", response);

                testHost.Close();

                // Find the tracking records for the ReceiveRequest and SendResponse

                // Activity <ReceiveRequest> state is Executing
                testHost.Tracking.Assert.ExistsAt(0, "ReceiveRequest", ActivityInstanceState.Executing);

                // Activity <SendResponse> state is Executing
                testHost.Tracking.Assert.ExistsAt(1, "SendResponse", ActivityInstanceState.Executing);
            }
        }

        #endregion
    }
}