// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingStubTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.Xml.Linq;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Tests of the MessagingStubExtension class
    /// </summary>
    [TestClass]
    public class MessagingStubTest
    {
        #region Fields

        /// <summary>
        ///   The service contract XName.
        /// </summary>
        private readonly XName serviceContractName = XName.Get("{http://tempuri.org/}IService");

        #endregion

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
        ///   * A WorkflowService which has a receive which accepts an integer and returns it as an argument
        ///   * A second Receive/SendReply pair which receives a number to increment the first number by.
        ///   * An assign activity which adds the first number and the second and returns the sum
        ///   * A MessagingStubExtension.OnIdle action receives the response and enqueues a second message to be received containing the first number plus the increment
        ///   When
        ///   * The service is invoked using mocked Receive/SendReply
        ///   Then
        ///   * The service can be invoked without sending a WCF message
        ///   * The mock implementations are used
        ///   * The messaging stub OnIdle is invoked allowing a second message to be enqueued
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ServicewithtworeceivesandincrementXamlxPath)]
        public void CanImplementMessagingProtocol()
        {
            // Arrange
            const int Expected = 123;
            const int Increment = 3;
            var xamlInjector = new XamlInjector(Constants.ServicewithtworeceivesandincrementXamlx);

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();
            host.Extensions.Add(stubExtension);

            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", Expected);
            stubExtension.OnIdle = s =>
                {
                    // If processing the reply to the initial message
                    if (s.Messages.Count == 2 && s.QueueCount == 0)
                    {
                        // Access the content and add the increment
                        s.EnqueueReceive(
                            this.serviceContractName, "IncrementData", ((int)s.Messages[1].Content) + Increment);
                    }
                };

            try
            {
                // Act
                host.TestActivity();

                // Assert
                Assert.AreEqual(Expected + Expected + Increment, stubExtension.Messages[3].Content);
                Assert.AreEqual(0, stubExtension.QueueCount);
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
        ///   When
        ///   * The service is invoked using mocked Send/Receive
        ///   Then
        ///   * The service can be invoked without sending a WCF message
        ///   * The mock implementations are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.DefaultServiceXamlxPath)]
        public void CanMockReceiveAndSendReply()
        {
            // Arrange
            const int Expected = 123;
            var xamlInjector = new XamlInjector(Constants.DefaultServiceXamlx);

            // Setup the XamlInjector to replace the receive / send activities
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));

            // Access the workflow service Body activity for testing with WorkflowInvoker
            var host = WorkflowInvokerTest.Create(xamlInjector.GetWorkflowService().Body);

            // Setup the extension
            var stubExtension = new MessagingStubExtension();
            host.Extensions.Add(stubExtension);

            stubExtension.EnqueueReceive(this.serviceContractName, "GetData", 123);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                Assert.AreEqual(Expected.ToString(), stubExtension.Messages[1].Content);
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
        ///   * A WorkflowService which contains a Parallel activity with three branches each of which contains a Receive/SendReply for operations GetData1, GetData2 and GetData3
        ///   When
        ///   * XamlInjector replaces messaging activities
        ///   * Messages are enqueued in the order 3, 1, 2
        ///   Then
        ///   * The service will receive the messages in order 3, 1, 2
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ServiceWithParallelThreeReceivesXamlxPath)]
        public void CanSendToAnyParallelBranch()
        {
            var xamlInjector = new XamlInjector(Constants.ServiceWithParallelThreeReceivesXamlx);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));
            var host = new WorkflowInvokerTest(xamlInjector.GetWorkflowService().Body);
            var stubExtension = new MessagingStubExtension();
            host.Invoker.Extensions.Add(stubExtension);

            stubExtension.EnqueueReceive(this.serviceContractName, "GetData3", 3);
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData1", 1);
            stubExtension.EnqueueReceive(this.serviceContractName, "GetData2", 2);

            try
            {
                host.TestActivity();

                Assert.AreEqual(6, stubExtension.Messages.Count);
                Assert.AreEqual("3", stubExtension.Messages[1].Content);
                Assert.AreEqual("1", stubExtension.Messages[3].Content);
                Assert.AreEqual("2", stubExtension.Messages[5].Content);
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
        ///   * An activity which contains two Receive activities
        ///   * and a custom stubExtension implementation
        ///   When
        ///   * XamlInjector replaces the Receive activity with the ReceiveStub activity
        ///   * And the activity is invoked
        ///   Then
        ///   * The custom stubExtension implementation will be invoked for both operations
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithTwoReceivesXamlPath)]
        public void CustomStubShouldBeInvoked()
        {
            // Arrange
            var xamlInjector = new XamlInjector(Constants.ActivityWithTwoReceivesXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            var sut = xamlInjector.GetActivity();
            var host = WorkflowInvokerTest.Create(sut);
            var stubExtension = new TestMessagingStubExtensionImplementation();
            host.Extensions.Add(stubExtension);
            stubExtension.EnqueueReceive(this.serviceContractName, "Operation1", null);
            stubExtension.EnqueueReceive(this.serviceContractName, "Operation2", null);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                Assert.IsTrue(stubExtension.Operation1Invoked);
                Assert.IsTrue(stubExtension.Operation2Invoked);
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
        ///   * An activity which contains a Receive activity
        ///   * and the receive accepts one integers x as message content
        ///   * and assigns the result of x+7 to an out argument named Sum
        ///   When
        ///   * XamlInjector replaces the Receive activity with the MessagingStubExtension activity
        ///   * And the activity is run
        ///   * with an extension of type IMessagingStub
        ///   Then
        ///   * The activity will invoke the IMessagingStubImplemtation.Implementation action
        ///   * and idle
        ///   * and return Sum = (x+7)
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithReceiveMessageXamlPath)]
        public void MessagingStubImplementationShouldSupplyMessageResults()
        {
            // Arrange
            const int Expected = 12;
            var xamlInjector = new XamlInjector(Constants.ActivityWithReceiveMessageXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            var host = WorkflowInvokerTest.Create(xamlInjector.GetActivity());

            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(this.serviceContractName, "Sum", 5);
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                host.Tracking.Assert.Exists("ReceiveStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
                host.AssertOutArgument.AreEqual("sum", Expected);
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
        ///   * An activity which contains a Receive activity
        ///   * and the receive accepts two integers x and y as parameters
        ///   * and assigns the result to an out argument named Sum
        ///   When
        ///   * XamlInjector replaces the Receive activity with the MessagingStubExtension activity
        ///   * And the activity is run
        ///   * with an extension of type IMessagingStub
        ///   Then
        ///   * The activity will invoke the IMessagingStubImplemtation.Implementation action
        ///   * and idle
        ///   * and return Sum = (x+y)
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithReceiveXamlPath)]
        public void MessagingStubImplementationShouldSupplyResults()
        {
            // Arrange
            const int Expected = 12;
            var xamlInjector = new XamlInjector(Constants.ActivityWithReceiveXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            var host = WorkflowInvokerTest.Create(xamlInjector.GetActivity());

            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(
                this.serviceContractName, "Sum", new Dictionary<string, object> { { "x", 5 }, { "y", 7 } });
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                host.Tracking.Assert.Exists("ReceiveStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
                host.AssertOutArgument.AreEqual("sum", Expected);
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
        ///   * An activity which contains a ReceiveAndSendReply activity
        ///   * and the receive accepts two integers x and y as parameters
        ///   * and Send returns the sum
        ///   When
        ///   * XamlInjector replaces the Send / Receive activities with stubs
        ///   * And the activity is run
        ///   * with an extension of type IMessagingStub
        ///   Then
        ///   * The activity will invoke the IMessagingStubImplemtation.Implementation action
        ///   * and replace the parameter values with 5 and 7
        ///   * and idle
        ///   * and SendStub will do nothing
        ///   * at completion outArgument Sum should be 12
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithReceiveAndSendReplyXamlPath)]
        public void ReceiveAndSendReplyStubImplementationShouldSetSum()
        {
            // Arrange
            const int Expected = 12;
            var xamlInjector = new XamlInjector(Constants.ActivityWithReceiveAndSendReplyXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));
            var host = WorkflowInvokerTest.Create(xamlInjector.GetActivity());

            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(
                this.serviceContractName, "Sum", new Dictionary<string, object> { { "x", 5 }, { "y", 7 } });
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                host.Tracking.Assert.Exists("ReceiveStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists("SendReplyToReceive", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed);
                host.AssertOutArgument.AreEqual("sum", Expected);
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
        ///   * An activity which contains a Receive activity
        ///   * and the receive accepts two integers x and y
        ///   * and assigns the result to an out argument named Sum
        ///   When
        ///   * XamlInjector replaces the Receive activity with the ReceiveStub activity
        ///   * And the activity is run until idle with the bookmark "{http://tempuri.org/}IService|Sum")
        ///   Then
        ///   * The activity will idle
        ///   * and the ReceiveStub activity will be Executing
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithReceiveXamlPath)]
        public void ReceiveStubShouldGoIdleWithBookmark()
        {
            // Arrange
            var xamlInjector = new XamlInjector(Constants.ActivityWithReceiveXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            var host = WorkflowApplicationTest.Create(xamlInjector.GetActivity());
            var stubExtension = new MessagingStubExtension();
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                // Run until idle with this bookmark
                host.TestWorkflowApplication.RunEpisode("{http://tempuri.org/}IService|Sum");

                // Assert
                host.Tracking.Assert.Exists("ReceiveStub", ActivityInstanceState.Executing);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
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
        ///   * An activity which contains a SendAndReceiveReply activity
        ///   * and the send accepts two integers x and y as parameters
        ///   * and Receive receives the sum as message content
        ///   When
        ///   * XamlInjector replaces the Send / Receive activities with stubs
        ///   * And the activity is run
        ///   * with an extension of type IMessagingMessageStub
        ///   Then
        ///   * The activity will invoke the IMessagingMessageStub.Implementation action
        ///   * and replace the message values with 12
        ///   * and idle
        ///   * and SendStub will do nothing
        ///   * at completion outArgument Sum should be 12
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithSendAndReceiveReplyXamlPath)]
        public void SendAndReceiveReplyStubMessageImplementationShouldSetSum()
        {
            // Arrange
            const int Expected = 12;
            var xamlInjector = new XamlInjector(Constants.ActivityWithSendAndReceiveReplyXaml);

            // Replace the messaging activities with stubs
            xamlInjector.ReplaceAll(typeof(ReceiveReply), typeof(ReceiveReplyStub));
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));

            var host = WorkflowInvokerTest.Create(xamlInjector.GetActivity());

            // Setup the extension
            var stubExtension = new MessagingStubExtension();

            // For the reply use the name of the contract / operation from the matching Send activity
            stubExtension.EnqueueReceiveReply(
                this.serviceContractName, "Sum", new Dictionary<string, object> { { "SumResult", Expected } });
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity(Constants.Timeout);

                // Assert
                host.Tracking.Assert.Exists("SendStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists("ReceiveReplyForSend", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed);
                host.AssertOutArgument.AreEqual("sum", Expected);

                Assert.AreEqual(2, stubExtension.Messages.Count);
                Assert.AreEqual(5, stubExtension.Messages[0].Parameter("x"));
                Assert.AreEqual(7, stubExtension.Messages[0].Parameter("y"));
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
        ///   * An activity which contains a SendAndReceiveReply activity
        ///   * and the send accepts two integers x and y as parameters
        ///   * and Receive receives the sum as message content
        ///   When
        ///   * XamlInjector replaces the Send / Receive activities with stubs
        ///   * And the activity is run
        ///   * with an extension of type IMessagingMessageStub
        ///   Then
        ///   * The activity will invoke the IMessagingMessageStub.Implementation action
        ///   * and replace the message values with 12
        ///   * and idle
        ///   * and SendStub will do nothing
        ///   * at completion outArgument Sum should be 12
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithReceiveAndSendReplyAndCorrXamlPath)]
        public void StubCorrelationActivities()
        {
            // Arrange
            const int Expected = 12;
            var xamlInjector = new XamlInjector(Constants.ActivityWithReceiveAndSendReplyAndCorrXaml);
            xamlInjector.ReplaceAll(typeof(Receive), typeof(ReceiveStub));
            xamlInjector.ReplaceAll(typeof(SendReply), typeof(SendReplyStub));
            xamlInjector.ReplaceAll(typeof(InitializeCorrelation), typeof(InitializeCorrelationStub));
            var host = WorkflowInvokerTest.Create(xamlInjector.GetActivity());

            var stubExtension = new MessagingStubExtension();
            stubExtension.EnqueueReceive(
                this.serviceContractName, "Sum", new Dictionary<string, object> { { "x", 5 }, { "y", 7 } });
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                host.Tracking.Assert.Exists("ReceiveStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists("SendReplyToReceive", ActivityInstanceState.Closed);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Idle);
                host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed);
                host.AssertOutArgument.AreEqual("sum", Expected);
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
        ///   * A XAML file which contains a Send activity 
        ///   * and the Send is in a Try Catch
        ///   * and you want to test a communication exception scenario
        ///   When
        ///   * XamlInjector replaces the Send activity with a StubSend activity
        ///   * And the test Xaml is runDeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting
        ///   Then
        ///   * The StubSend activity throws a communication exception
        ///   * The catch is invoked
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithSendCatchCommExceptionXamlPath)]
        public void WhenSendStubInvokedSimulatesCommunicationException()
        {
            // Arrange
            var xamlInjector = new XamlInjector(Constants.ActivityWithSendCatchCommExceptionXaml);
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));
            var sut = xamlInjector.GetActivity();
            var host = new WorkflowInvokerTest(sut);

            var stubExtension = new MessagingStubExtension();

            // Add an implementation that will simulate a bad URI for the activity 
            // Note: If the DisplayName is not set, the name will be the name of the SendStub activity
            stubExtension.SetImplementation("SendStub", () => { throw new EndpointNotFoundException(); });
            host.Extensions.Add(stubExtension);

            try
            {
                // Act
                host.TestActivity();

                // Assert
                // Shows that the SendStub activity was used
                host.Tracking.Assert.Exists("SendStub", ActivityInstanceState.Executing);
                host.Tracking.Assert.DoesNotExist("Send", ActivityInstanceState.Closed);
                host.AssertOutArgument.IsTrue("CatchHandled");
                host.AssertOutArgument.IsInstanceOfType("CaughtException", typeof(EndpointNotFoundException));
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
        ///   * A XAML file which contains two Send activities with message content
        ///   When
        ///   * XamlInjector replaces the Send activities with StubSend activities
        ///   * And the test Xaml is run
        ///   Then
        ///   * The StubSend activities are invoked instead of the Send activities
        ///   * The sent message content is captured by the send stub implementation
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithSendMessageContentXamlPath)]
        public void WhenSendWithContentStubInvoked()
        {
            var xamlInjector = new XamlInjector(Constants.ActivityWithSendMessageContentXaml);
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));
            var sut = xamlInjector.GetActivity();
            var host = new WorkflowInvokerTest(sut);
            var stubExtension = new MessagingStubExtension();
            host.Invoker.Extensions.Add(stubExtension);

            dynamic arguments = new WorkflowArguments();
            arguments.data = "Test";
            try
            {
                host.TestActivity(arguments);

                // Shows that the SendStub activity was used
                host.Tracking.Assert.Exists("SendStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.DoesNotExist("Send", ActivityInstanceState.Closed);

                Assert.AreEqual(2, stubExtension.Messages.Count);
                Assert.AreEqual("Test 1", stubExtension.Messages[0].Content);
                Assert.AreEqual("Test 2", stubExtension.Messages[1].Content);
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
        ///   * A XAML file which contains a Send activity with no message content
        ///   When
        ///   * XamlInjector replaces the Send activity with a StubSend activity
        ///   * And the test Xaml is run
        ///   Then
        ///   * The StubSend activity is invoked instead of the Send activity
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithSendXamlPath)]
        public void WhenSendWithNoContentStubInvoked()
        {
            var xamlInjector = new XamlInjector(Constants.ActivityWithSendXaml);
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));
            var sut = xamlInjector.GetActivity();
            var host = new WorkflowInvokerTest(sut);
            var stubExtension = new MessagingStubExtension();
            host.Invoker.Extensions.Add(stubExtension);
            try
            {
                host.TestActivity();

                // Shows that the SendStub activity was used
                host.Tracking.Assert.Exists("SendStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.DoesNotExist("Send", ActivityInstanceState.Closed);
                Assert.AreEqual(1, stubExtension.Messages.Count);
                Assert.AreEqual(1, stubExtension.Messages[0].Content);
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
        ///   * A XAML file which contains two Send activities with parameters content
        ///   When
        ///   * XamlInjector replaces the Send activities with StubSend activities
        ///   * And the test Xaml is run
        ///   Then
        ///   * The StubSend activities are invoked instead of the Send activities
        ///   * The sent parameters content is captured by the send stub implementation
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityWithSendParametersContentXamlPath)]
        public void WhenSendWithParametersContentStubInvoked()
        {
            var xamlInjector = new XamlInjector(Constants.ActivityWithSendParametersContentXaml);
            xamlInjector.ReplaceAll(typeof(Send), typeof(SendStub));
            var sut = xamlInjector.GetActivity();
            var host = new WorkflowInvokerTest(sut);
            var stubExtension = new MessagingStubExtension();
            host.Invoker.Extensions.Add(stubExtension);

            try
            {
                host.TestActivity();

                // Shows that the SendStub activity was used
                host.Tracking.Assert.Exists("SendStub", ActivityInstanceState.Closed);
                host.Tracking.Assert.DoesNotExist("Send", ActivityInstanceState.Closed);

                // There should be two send activities
                Assert.AreEqual(2, stubExtension.Messages.Count);

                // Verify the parameters content from the first send
                Assert.AreEqual(1, stubExtension.Messages[0].Parameter("num"));
                Assert.AreEqual("test1", stubExtension.Messages[0].Parameter("test"));

                // Verify the parameters content from the second send
                Assert.AreEqual(2, stubExtension.Messages[1].Parameter("num"));
                Assert.AreEqual("test2", stubExtension.Messages[1].Parameter("test"));
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