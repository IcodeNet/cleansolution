// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExtensionsBehaviorTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;

    using Microsoft.Activities.Extensions.Tests.ServiceReferences;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This is a test class for WorkflowExtensionsBehaviorTest and is intended
    ///   to contain all WorkflowExtensionsBehaviorTest Unit Tests
    /// </summary>
    [TestClass]
    public class WorkflowExtensionsBehaviorTest
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
        ///   A test for ApplyDispatchBehavior
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestProjectDir + "ServiceExtensionTest.xamlx")]
        public void WorkflowExtensionsBehaviorAddsExtension()
        {
            WorkflowServiceTestHost host = null;

            // TODO: Test with multiple extensions
            // TODO: Test with bad config file entries
            var serviceEndpoint1 = ServiceTest.GetUniqueEndpointAddress();
            using (host = WorkflowServiceTestHost.Open("ServiceExtensionTest.xamlx", serviceEndpoint1))
            {
                try
                {
                    var proxy = new ServiceClient(ServiceTest.Pipe, serviceEndpoint1);
                    try
                    {
                        proxy.GetData(1);
                        proxy.Close();
                    }
                    catch (Exception)
                    {
                        proxy.Abort();
                        throw;
                    }
                }
                finally
                {
                    if (host != null)
                    {
                        host.Tracking.Trace();
                    }
                }
            }
        }

        #endregion
    }
}