// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlInjectorTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Tracking;
    using System.IO;
    using System.ServiceModel;
    using System.Xaml;
    using System.Xml;

    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.Activities.UnitTesting.Tests.MockActivities;
    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The xaml injector tests.
    /// </summary>
    [TestClass]
    public class XamlInjectorTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Verifies that the WorkflowServiceTestHost hosts a service and that the service receives and sends a reply
        /// </summary>
        /// <remarks>
        ///   Be sure to enable deployment - the xamlx file must be deployed
        /// </remarks>
        [TestMethod]
        [DeploymentItem(Constants.TestActivitiesDir + "TestSumService.xamlx")]
        public void ShouldHostService()
        {
            var trackingProfile = new TrackingProfile { Queries = { new ActivityStateQuery { ActivityName = "ReceiveRequest", States = { "Executing" }, }, new ActivityStateQuery { ActivityName = "SendResponse", States = { "Executing" }, }, } };

            var xamlInjector = new XamlInjector("TestSumService.xamlx");

            // The first TestActivity1 will not be replaced - will add 1 to sum

            // Replace the second TestActivity1 with TestActivity2 - will add 2 to sum
            xamlInjector.ReplaceAt(1, typeof(TestActivity1), typeof(TestActivity2));

            // Replace third TestActivity1 with TestActivity3 - will add 3 to sum
            xamlInjector.ReplaceAt(2, typeof(TestActivity1), typeof(TestActivity3));

            // Replace all (2) TestActivity4 with TestActivity5 - will add 10 to sum
            xamlInjector.ReplaceAll(typeof(TestActivity4), typeof(TestActivity5));

            // Response will be (data=1)+1+2+3+10 = 17
            var serviceAddress = ServiceTest.GetUniqueEndpointAddress();
            using (var testHost = new WorkflowServiceTestHost(xamlInjector.GetWorkflowService(), serviceAddress))
            {
                testHost.Tracking.TrackingProfile = trackingProfile;
                testHost.Open();

                var client = ChannelFactory<ITestService>.CreateChannel(ServiceTest.Pipe, serviceAddress);
                var response = client.GetData(1);
                Assert.AreEqual("17", response);

                testHost.Close();

                // Find the tracking records for the ReceiveRequest and SendResponse

                // Activity <ReceiveRequest> state is Executing
                AssertTracking.ExistsAt(testHost.Tracking.Records, 0, "ReceiveRequest", ActivityInstanceState.Executing);

                // Activity <SendResponse> state is Executing
                AssertTracking.ExistsAt(testHost.Tracking.Records, 1, "SendResponse", ActivityInstanceState.Executing);
            }
        }

        /// <summary>
        ///   Given a XAML file with activities and a Default value for a variable
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInjectWithDefault.xaml")]
        public void ShouldReplaceTypesInXamlWhenDefaultValue()
        {
            this.VerifyInjection(new XamlInjector(new XamlXmlReader("TestInjectWithDefault.xaml")));
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void ShouldReplaceTypesInXamlWithFileName()
        {
            this.VerifyInjection(new XamlInjector("TestInject.xaml"));
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void ShouldReplaceTypesInXamlWithStream()
        {
            this.VerifyInjection(new XamlInjector(new FileStream("TestInject.xaml", FileMode.Open, FileAccess.Read)));
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void ShouldReplaceTypesInXamlWithTextReader()
        {
            this.VerifyInjection(new XamlInjector(new StreamReader("TestInject.xaml")));
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void ShouldReplaceTypesInXamlWithXamlXmlReader()
        {
            this.VerifyInjection(new XamlInjector(new XamlXmlReader("TestInject.xaml")));
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector is used to inject replacements
        ///   Then the replacements are used
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void ShouldReplaceTypesInXamlWithXmlReader()
        {
            this.VerifyInjection(new XamlInjector(new XmlTextReader("TestInject.xaml")));
        }

        /// <summary>
        ///   Verifies that the WriteLine activity can be replaced
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestActivitiesDir + "TestInjectWriteLine.xaml")]
        public void ShouldReplaceWriteLineActivity()
        {
            // Arrange
            var xamlInjector = new XamlInjector("TestInjectWriteLine.xaml", typeof(TestActivity1).Assembly);

            xamlInjector.ReplaceAll(typeof(TestActivity1), typeof(TestActivity2));

            xamlInjector.ReplaceAll(typeof(WriteLine), typeof(MockWriteLine));
            var activity = xamlInjector.GetActivity();

            var invokerTest = new WorkflowInvokerTest(activity);

            try
            {
                // Act
                invokerTest.TestActivity();

                // Assert
                invokerTest.Tracking.Assert.Exists("MockWriteLine", ActivityInstanceState.Closed);
            }
            finally
            {
                invokerTest.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given a XAML file with activities
        ///   When the XamlInjector.GetActivity method is called more than once
        ///   Then it returns the same activity each time
        /// </summary>
        [TestMethod]
        [DeploymentItem(Labs.Dir + @"\Microsoft.Activities.UnitTesting\Microsoft.Activities.UnitTesting.Tests\TestInject.xaml")]
        public void WhenGetActivityIsCalledTwiceSameActivityIsReturned()
        {
            var injector = new XamlInjector("TestInject.xaml");
            Assert.AreSame(injector.GetActivity(), injector.GetActivity());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The verify injection.
        /// </summary>
        /// <param name="xamlInjector">
        /// The xaml injector. 
        /// </param>
        private void VerifyInjection(XamlInjector xamlInjector)
        {
            // The first TestActivity1 will not be replaced - will add 1 to sum

            // Replace the second TestActivity1 with TestActivity2 - will add 2 to sum
            xamlInjector.ReplaceAt(1, typeof(TestActivity1), typeof(TestActivity2));

            // Replace third TestActivity1 with TestActivity3 - will add 3 to sum
            xamlInjector.ReplaceAt(2, typeof(TestActivity1), typeof(TestActivity3));

            // Replace all (2) TestActivity4 with TestActivity5 - will add 10 to sum
            xamlInjector.ReplaceAll(typeof(TestActivity4), typeof(TestActivity5));

            // Debug.WriteLine(string.Format("Invoking Injected XAML activity {0}", activity.GetType()));
            var host = new WorkflowInvokerTest(xamlInjector.GetActivity());

            try
            {
                // Act
                host.TestActivity();

                // Total should be 1+2+3+10=16
                host.AssertOutArgument.AreEqual("sum", 16);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}