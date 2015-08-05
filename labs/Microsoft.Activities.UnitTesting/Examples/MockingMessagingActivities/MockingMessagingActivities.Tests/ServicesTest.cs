// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServicesTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MockingMessagingActivities.Tests
{
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.Xml.Linq;

    using Microsoft.Activities;
    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Sample tests that demonstrate mocking of send/receive activities
    /// </summary>
    [TestClass]
    public class ServicesTest
    {
        #region Constants

        /// <summary>
        ///   The backup response
        /// </summary>
        private const string BackupResponse = "Backup 123";

        /// <summary>
        /// Path to the MockingTests
        /// </summary>
        private const string MockingTestsDir = Labs.UnitTestingExamples + @"MockingMessagingActivities\MockingMessagingActivities.Tests\";

        private const string MockingMessagingActivities = "MockingMessagingActivities";

        private const string Service1Xamlx = "Service1.xamlx";

        #endregion

        #region Fields

        /// <summary>
        ///   The service contract name.
        /// </summary>
        private readonly XName serviceContractName = XName.Get("{http://tempuri.org/}IService");

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * The default WorkflowService implementation GetData
        ///   When
        ///   * The service is invoked using mocked Send/Receive
        ///   Then
        ///   * The service can be invoked without sending a WCF message
        ///   * The mock implementations are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(MockingTestsDir + Service1Xamlx, MockingMessagingActivities)]
        public void CanMockReceiveAndSendReply()
        {
            // Arrange
            const int Expected = 123;
            var xamlInjector = new XamlInjector(MockingMessagingActivities + "\\" + Service1Xamlx);
            
            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", Expected);
            host.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // The reply should be "123"
                Assert.AreEqual(Expected.ToString(CultureInfo.InvariantCulture), stubExtension.Messages[1].Content);
            }
            finally
            {
                Trace.WriteLine("*** Messaging Stub Dump");
                stubExtension.Trace();

                Trace.WriteLine("\r\n*** Workflow Tracking Records");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService with a GetData Operation which accepts two parameters value1 and value2 and returns a composite response with string parameters data1 and data2
        ///   When
        ///   * The service is invoked using mocked Send/Receive with parameters value1=123 and value2=456
        ///   Then
        ///   * The service can be invoked without sending a WCF message
        ///   * The mock implementations are used
        ///   * The response parameters are data1="123" and data2="456"
        /// </summary>
        [TestMethod]
        [DeploymentItem(MockingTestsDir + "Service1Parameters.xamlx", MockingMessagingActivities)]
        public void CanMockReceiveAndSendReplyWithParameters()
        {
            // Arrange
            const int Value1 = 123;
            const int Value2 = 456;
            var xamlInjector = new XamlInjector(@"MockingMessagingActivities\Service1Parameters.xamlx");

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            dynamic arguments = new WorkflowArguments();
            arguments.value1 = Value1;
            arguments.value2 = Value2;

            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", arguments);
            host.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // The first reply message parameter data1 should be "123"
                Assert.AreEqual(Value1.ToString(CultureInfo.InvariantCulture), stubExtension.Messages[1].Parameter("data1"));

                // The first reply message parameter data2 should be "456"
                Assert.AreEqual(Value2.ToString(CultureInfo.InvariantCulture), stubExtension.Messages[1].Parameter("data2"));
            }
            finally
            {
                Trace.WriteLine("*** Messaging Stub Dump");
                stubExtension.Trace();

                Trace.WriteLine("\r\n*** Workflow Tracking Records");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * The default WorkflowService implementation GetData
        ///   * A second method GetData2 which continues the workflow, adds 2 to the data and returns a string
        ///   When
        ///   * The service is invoked using mocked Send/Receive with a value of 5 sent to the first receive
        ///   and a value of 6 to the second receive
        ///   Then
        ///   * The first reply is "5"
        ///   * The second reply is "8"
        /// </summary>
        [TestMethod]
        [DeploymentItem(MockingTestsDir + "Service2.xamlx", MockingMessagingActivities)]
        public void CanMockTwoReceiveAndSendReply()
        {
            // Arrange
            var xamlInjector = new XamlInjector(@"MockingMessagingActivities\Service2.xamlx");

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", 5);
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData2", 6);
            host.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // Assert
                // The first reply is "5"
                Assert.AreEqual("5", stubExtension.Messages[1].Content);

                // The second reply is "8"
                Assert.AreEqual("8", stubExtension.Messages[3].Content);
            }
            finally
            {
                Trace.WriteLine("*** Messaging Stub Dump");
                stubExtension.Trace();

                Trace.WriteLine("\r\n*** Workflow Tracking Records");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService with operation GetData which accepts two parameters value1 and value2
        ///   * A second method GetData2 which continues the workflow, adds 1 to value1 and 2 to value2 to the data and returns strings data1 and data2
        ///   When
        ///   * The service is invoked using mocked Send/Receive with value1=5 and value2=4
        ///   Then
        ///   * The first reply is data1="5" and data2="4"
        ///   When
        ///   * The service is invoked using mocked Send/Receive with value1=6 and value2=3
        ///   Then
        ///   * The second reply is data1="7" and data2="5"
        /// </summary>
        [TestMethod]
        [DeploymentItem(MockingTestsDir + "Service2Parameters.xamlx", MockingMessagingActivities)]
        public void CanMockTwoReceiveAndSendReplyWithParameters()
        {
            // Arrange
            var xamlInjector = new XamlInjector(@"MockingMessagingActivities\Service2Parameters.xamlx");

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();

            // Setup the first message
            dynamic parameterValues1 = new WorkflowArguments();
            parameterValues1.value1 = 5;
            parameterValues1.value2 = 4;
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", parameterValues1);

            // Setup the second message
            dynamic parameterValues2 = new WorkflowArguments();
            parameterValues2.value1 = 6;
            parameterValues2.value2 = 3;
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData2", parameterValues2);

            host.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // Assert
                // The first reply parameter "data1" is "5"
                Assert.AreEqual("5", stubExtension.Messages[1].Parameter("data1"));

                // The first reply parameter "data1" is "4"
                Assert.AreEqual("4", stubExtension.Messages[1].Parameter("data2"));

                // The second reply parameter "data1" is "7"
                Assert.AreEqual("7", stubExtension.Messages[3].Parameter("data1"));

                // The second reply parameter "data1" is "5"
                Assert.AreEqual("5", stubExtension.Messages[3].Parameter("data2"));
            }
            finally
            {
                Trace.WriteLine("*** Messaging Stub Dump");
                stubExtension.Trace();

                Trace.WriteLine("\r\n*** Workflow Tracking Records");
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A WorkflowService which sends a message to another service in a Try/Catch block
        ///   * If the primary service is down an EndpointNotFoundException is caught and it tries to call a backup service
        ///   When
        ///   * The service is invoked with value 123 and the endpoint is not available
        ///   Then
        ///   * The reply is "Backup 123"
        /// </summary>
        [TestMethod]
        [DeploymentItem(MockingTestsDir + "ServiceWithSend.xamlx", MockingMessagingActivities)]
        public void CanSimulateSendFailure()
        {
            // Arrange
            var xamlInjector = new XamlInjector(@"MockingMessagingActivities\ServiceWithSend.xamlx");

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));
            xamlInjector.ReplaceAll(typeof(ReceiveReply), typeof(ReceiveReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();

            // The first receive will start the process
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", 123);

            // The first send will result in an exception
            stubExtension.SetImplementation("Send Primary", () => { throw new EndpointNotFoundException(); });

            // The second send should succeed 
            // The next receive reply should simulate data from the backup service
            stubExtension.EnqueueReceiveReply(this.serviceContractName, "GetData", BackupResponse);
            host.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // Assert
                // The final reply is "Backup 123"
                Assert.AreEqual(BackupResponse, stubExtension.Messages[4].Content);
            }
            finally
            {
                Trace.WriteLine("*** Messaging Stub Dump");
                stubExtension.Trace();

                Trace.WriteLine("\r\n*** Workflow Tracking Records");
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}