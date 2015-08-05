#region copyright

//  ----------------------------------------------------------------------------------
//  Microsoft
//  
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//  ----------------------------------------------------------------------------------
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
//  ----------------------------------------------------------------------------------

#endregion

using System;
using System.Activities;
using System.ServiceModel;
using System.ServiceModel.Channels;
using HRApplicationServices.Activities;
using HRApplicationServices.Application;
using HRApplicationServices.Contracts;
using HRServices.Tests.Activities;
using Microsoft.Activities.UnitTesting.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Activities.UnitTesting;
using Microsoft.Activities.UnitTesting.Tracking;
using SubmitJobApplicationRequest = HRApplicationServices.Contracts.SubmitJobApplicationRequest;
using SubmitJobApplicationResponse = HRApplicationServices.Contracts.SubmitJobApplicationResponse;
using System.Activities.Statements;

namespace HRServices.Tests
{
    /// <summary>
    ///   Tests the SubmitApplication Workflow
    /// </summary>
    [TestClass]
    public class SubmitApplicationTest
    {
        #region Test Context

        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        private readonly Binding _binding = new NetNamedPipeBinding();

        /// <summary>
        ///   The endpoint address to be used by the test host
        /// </summary>
        private readonly EndpointAddress _serviceAddress = new EndpointAddress("net.pipe://localhost/SubmitApplication");


        [TestMethod]
        [DeploymentItem(@"HRApplicationServices\SubmitApplication.xamlx")]
        public void ShouldNotHireEductionLevelNone()
        {
            const string expectedName = "test";
            const int expectedId = 123;
            var expectedResponse = string.Format(ServiceResources.JobApplicationProcessing, expectedName, expectedId);

            var xamlInjector = new XamlInjector("SubmitApplication.xamlx");
            xamlInjector.ReplaceAll(typeof(NotifyApplicant), typeof(MockNotifyApplicant));
            xamlInjector.ReplaceAll(typeof(RequestHumanScreening), typeof(MockRequestHumanScreening));
            xamlInjector.ReplaceAll(typeof(SaveJobApplication), typeof(MockSaveJobApplication));
            xamlInjector.ReplaceAll(typeof(UpdateHireApproved), typeof(MockUpdateHireApproved));

            SubmitJobApplicationResponse response = null;

            // Host the service);
            using (var testHost = WorkflowServiceTestHost.Open(xamlInjector.GetWorkflowService(), _serviceAddress))
            {
                // Setup a proxy to use named pipes
                var proxy = new ApplicationServiceClient(_binding, _serviceAddress);

                try
                {
                    // Submit an application
                    response = proxy.SubmitJobApplication(new SubmitJobApplicationRequest
                                                              {
                                                                  RequestID = Guid.NewGuid(),
                                                                  Resume = new ApplicantResume
                                                                               {
                                                                                   Education = "None",
                                                                                   Email = "test@test.org",
                                                                                   Name = "test",
                                                                                   NumReferences = 0
                                                                               }
                                                              });

                    proxy.Close();
                }
                catch (Exception)
                {
                    //testHost.Tracking.Trace();
                    proxy.Abort();
                    throw;
                }


                // The last thing to happen is for the Workflow Instance to be deleted from
                // the persistence store.
                // Wait for this before asserting tracking records
                Assert.IsTrue(WorkflowServiceTestHost.WaitForInstanceDeleted());

                // Close the host
                testHost.Close();

                // Assert

                // Check that we got a response message
                Assert.AreEqual(expectedResponse, response.ResponseText);

                testHost.Tracking.Trace();

                // Assert that the Auto Screen Education activity returned EducationPassed = false

                // Find this tracking record and assert the arguments
                // Activity <Auto Screen Education> state is Closed
                // {
                // Arguments
                // EducationPassed: False
                // Education: None
                // }

                // After the autoscreen the EducationPassed argument should be false
                var autoScreenEducation = testHost.Tracking.Records.FindActivityState("Auto Screen Education",
                                                                                      ActivityInstanceState.Closed);
                Assert.IsFalse(autoScreenEducation.GetArgument<bool>("EducationPassed"));

                // Assert that the Update No Hire activity was invoked

                // Find this activity state record
                //Activity <Update No Hire> state is Closed
                //{
                //    Arguments
                //        ApplicantID: 123
                //        HireApproved: False
                //}

                var updateNoHire = testHost.Tracking.Records.FindActivityState("Update No Hire",
                                                                               ActivityInstanceState.Closed);
                Assert.IsFalse(updateNoHire.GetArgument<bool>("HireApproved"));

                // Assert that the no-hire email notification was sent

                // Find this activity state record
                //Activity <MockNotifyApplicant> state is Closed
                //{
                //    Arguments
                //        Hire: False
                //        Resume: HRApplicationServices.Contracts.ApplicantResume
                //}

                var notifyApplicant = testHost.Tracking.Records.FindActivityState("MockNotifyApplicant",
                                                                                  ActivityInstanceState.Closed);
                Assert.IsFalse(notifyApplicant.GetArgument<bool>("Hire"));
            }
        }

        [TestMethod]
        [DeploymentItem(@"HRApplicationServices\SubmitApplication.xamlx")]
        public void ShouldHireEducationLevelMasters()
        {
            const string expectedName = "test";
            const int expectedId = 123;
            var expectedResponse = string.Format(ServiceResources.JobApplicationProcessing, expectedName, expectedId);


            var xamlInjector = new XamlInjector("SubmitApplication.xamlx");
            xamlInjector.ReplaceAll(typeof(NotifyApplicant), typeof(MockNotifyApplicant));
            xamlInjector.ReplaceAll(typeof(RequestHumanScreening), typeof(MockRequestHumanScreening));
            xamlInjector.ReplaceAll(typeof(SaveJobApplication), typeof(MockSaveJobApplication));
            xamlInjector.ReplaceAll(typeof(UpdateHireApproved), typeof(MockUpdateHireApproved));

            SubmitJobApplicationResponse response = null;

            // Host the service);
            using (var testHost = WorkflowServiceTestHost.Open(xamlInjector.GetWorkflowService(), _serviceAddress))
            {
                // Setup a proxy to use named pipes
                var proxy = new ApplicationServiceClient(_binding, _serviceAddress);

                try
                {
                    // Submit an application
                    response = proxy.SubmitJobApplication(new SubmitJobApplicationRequest
                                                              {
                                                                  RequestID = Guid.NewGuid(),
                                                                  Resume = new ApplicantResume
                                                                               {
                                                                                   Education = "Masters",
                                                                                   Email = "test@test.org",
                                                                                   Name = "test",
                                                                                   NumReferences = 0
                                                                               }
                                                              });

                    // Check that we got a response message
                    Assert.AreEqual(expectedResponse, response.ResponseText);

                    var result =
                        proxy.HumanScreeningCompleted(new HumanScreeningResult { AppID = expectedId, HiringApproved = true });
                    Assert.IsTrue(result.HasValue && result.Value);

                    proxy.Close();
                }
                catch (Exception)
                {
                    //testHost.Tracking.Trace();
                    proxy.Abort();
                    throw;
                }

                // The last thing to happen is for the Workflow Instance to be deleted from
                // the persistence store.
                // Wait for this before asserting tracking records
                Assert.IsTrue(WorkflowServiceTestHost.WaitForInstanceDeleted());

                // Close the host
                testHost.Close();
                Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed());

                // Assert

                testHost.Tracking.Trace(1000);

                // Assert that the Auto Screen Education activity returned EducationPassed = false

                // Find this tracking record and assert the arguments
                //Activity <Auto Screen Education> state is Closed
                //{
                //    Arguments
                //        EducationPassed: True
                //        Education: Masters
                //}

                // After the autoscreen the EducationPassed argument should be true
                var autoScreenEducation = testHost.Tracking.Records.FindActivityState("Auto Screen Education",
                                                                                      ActivityInstanceState.Closed);
                Assert.IsTrue(autoScreenEducation.GetArgument<bool>("EducationPassed"));

                // Assert that the Request Human Screening activity was invoked
                //Activity <Request Human Screening> state is Closed
                //{
                //    Arguments
                //        ApplicationRequest: HRApplicationServices.Contracts.SubmitJobApplicationRequest
                //        ApplicationID: 123
                //}

                var requestHumanScreening = testHost.Tracking.Records.FindActivityState("Request Human Screening",
                                                                                        ActivityInstanceState.Closed);

                Assert.IsNotNull(requestHumanScreening, "Could not find Request Human Screening");
                Assert.AreEqual(expectedId, requestHumanScreening.GetArgument<int>("ApplicationID"));


                // Assert that the Update Hire activity was invoked

                // Find this activity state record
                //Activity <Update Hire Approved> state is Executing
                //{
                //    Arguments
                //        ApplicantID: 123
                //        HireApproved: True
                //}

                var updateHire = testHost.Tracking.Records.FindActivityState("Update Hire Approved",
                                                                             ActivityInstanceState.Closed);
                Assert.IsTrue(updateHire.GetArgument<bool>("HireApproved"));
                Assert.AreEqual(expectedId, updateHire.GetArgument<int>("ApplicantID"));

                // Assert that the hire email notification was sent

                // Find this activity state record
                //Activity <MockNotifyApplicant> state is Closed
                //{
                //    Arguments
                //        Hire: True
                //        Resume: HRApplicationServices.Contracts.ApplicantResume
                //}

                //var notifyApplicant = testHost.Tracking.Records.FindActivityState("MockNotifyApplicant",
                //                                                                  ActivityInstanceState.Closed);
                //Assert.IsNotNull(notifyApplicant);
                //Assert.IsTrue(notifyApplicant.GetArgument<bool>("Hire"));
                //Assert.AreEqual("test", notifyApplicant.GetArgument<ApplicantResume>("Resume").Name);
            }
        }

        [TestMethod]
        [DeploymentItem(@"HRApplicationServices\SubmitApplication.xamlx")]
        public void ShouldCancelAfter2Nags()
        {
            
            const string expectedName = "test";
            const int expectedId = 123;
            var expectedResponse = string.Format(ServiceResources.JobApplicationProcessing, expectedName, expectedId);


            var xamlInjector = new XamlInjector("SubmitApplication.xamlx");
            xamlInjector.ReplaceAll(typeof(NotifyApplicant), typeof(MockNotifyApplicant));
            xamlInjector.ReplaceAll(typeof(RequestHumanScreening), typeof(MockRequestHumanScreening));
            xamlInjector.ReplaceAll(typeof(SaveJobApplication), typeof(MockSaveJobApplication));
            xamlInjector.ReplaceAll(typeof(Delay), typeof(FakeDelay));
            xamlInjector.ReplaceAll(typeof(UpdateHireApproved), typeof(MockUpdateHireApproved));

            SubmitJobApplicationResponse response = null;

            // Host the service);
            using (var testHost = WorkflowServiceTestHost.Open(xamlInjector.GetWorkflowService(), _serviceAddress))
            {
                // Setup a proxy to use named pipes
                var proxy = new ApplicationServiceClient(_binding, _serviceAddress);

                try
                {
                    // Submit an application
                    response = proxy.SubmitJobApplication(new SubmitJobApplicationRequest
                    {
                        RequestID = Guid.NewGuid(),
                        Resume = new ApplicantResume
                        {
                            Education = "Masters",
                            Email = "test@test.org",
                            Name = "test",
                            NumReferences = 0
                        }
                    });

                    // Check that we got a response message
                    Assert.AreEqual(expectedResponse, response.ResponseText);

                    // Take no further action - application should cancel

                    proxy.Close();
                }
                catch (Exception)
                {
                    testHost.Tracking.Trace();
                    proxy.Abort();
                    throw;
                }

                // The last thing to happen is for the Workflow Instance to be deleted from
                // the persistence store.
                // Wait for this before asserting tracking records
                Assert.IsTrue(WorkflowServiceTestHost.WaitForInstanceDeleted());

                // Close the host
                testHost.Close();
                Assert.IsTrue(WorkflowServiceTestHost.WaitForHostClosed());

                // Assert

                testHost.Tracking.Trace();

                // Find the cancel notification
                //Activity <MockNotifyApplicant> state is Closed
                //{
                //    Arguments
                //        Cancel: True
                //        Hire: False
                //        Resume: HRApplicationServices.Contracts.ApplicantResume
                //}

                var notifyApplicant = testHost.Tracking.Records.FindActivityState("MockNotifyApplicant",
                                                                                  ActivityInstanceState.Closed);
                Assert.IsNotNull(notifyApplicant);
                Assert.IsFalse(notifyApplicant.GetArgument<bool>("Hire"));
                Assert.IsTrue(notifyApplicant.GetArgument<bool>("Cancel"));
            }
        }
    }
}