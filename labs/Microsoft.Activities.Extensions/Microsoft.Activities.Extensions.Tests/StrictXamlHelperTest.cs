// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StrictXamlHelperTest.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.IO;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the StrictXamlHelper class
    /// </summary>
    [TestClass]
    public class StrictXamlHelperTest
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
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V1
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * Workflow should load and return version 1.0.0.0
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestAssembly, Constants.WorkflowV1ActivityV1)]
        [DeploymentItem(Constants.ActivityV1, Constants.WorkflowV1ActivityV1)]
        public void WorkflowV1RefActivityV1DeployedActivityV1ShouldLoad()
        {
            var domain = this.CreateWorkerDomain(Constants.WorkflowV1ActivityV1);
            try
            {
                CreateTestWorker(domain).WorkflowV1RefActivityV1DeployedActivityV1ShouldLoad();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Given 
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V1 (Unsigned)
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * FileLoadException should be thrown
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestAssembly, Constants.WorkflowV1ActivityV1Unsigned)]
        [DeploymentItem(Constants.ActivityV1Unsigned, Constants.WorkflowV1ActivityV1Unsigned)]
        public void WorkflowV1RefActivityV1DeployedActivityV1UnsignedShouldThrow()
        {
            var domain = this.CreateWorkerDomain(Constants.WorkflowV1ActivityV1Unsigned);
            try
            {
                CreateTestWorker(domain).WorkflowV1RefActivityV1DeployedActivityV1UnsignedShouldThrow();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Given 
        ///   * The following components are deployed
        ///   * Workflow (compiled) V1
        ///   * ActivityLibrary.dll V2
        ///   When 
        ///   * Workflow (compiled) is constructed using reference to Activity V1
        ///   Then
        ///   * FileLoadException is thrown
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestAssembly, Constants.WorkflowV1ActivityV2)]
        [DeploymentItem(Constants.ActivityV2, Constants.WorkflowV1ActivityV2)]
        public void WorkflowV1RefActivityV1DeployedActivityV2ShouldThrow()
        {
            var domain = this.CreateWorkerDomain(Constants.WorkflowV1ActivityV2);
            try
            {
                CreateTestWorker(domain).WorkflowV1RefActivityV1DeployedActivityV2ShouldThrow();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Given
        ///   * Workflow.xaml deployed
        ///   * Activity V1 (signed) deployed
        ///   When
        ///   * ActivityLoad with ref to ActivityLibrary V1 (signed)
        ///   Then
        ///   * the workflow should return V1
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestAssembly, Constants.WorkflowXamlActivityV1)]
        [DeploymentItem(Constants.ActivityV1, Constants.WorkflowXamlActivityV1)]
        [DeploymentItem(Constants.WorkflowXaml, Constants.WorkflowXamlActivityV1)]
        public void WorkflowXamlRefActivityV1DeployedActivityV1ShouldLoad()
        {
            var domain = this.CreateWorkerDomain(Constants.WorkflowXamlActivityV1);
            try
            {
                CreateTestWorker(domain).WorkflowXamlRefActivityV1DeployedActivityV1ShouldLoad();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Given 
        ///   * Workflow.xaml deployed
        ///   * ActivityLibrary (V2) deployed
        ///   When 
        ///   * ActivityLoad with reference to ActivityLibrary V1
        ///   Then 
        ///   * should throw FileLoadException because cannot load ActivityLibrary V1
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.TestAssembly, Constants.WorkflowXamlActivityV2)]
        [DeploymentItem(Constants.WorkflowXaml, Constants.WorkflowXamlActivityV2)]
        [DeploymentItem(Constants.ActivityV2, Constants.WorkflowXamlActivityV2)]
        public void WorkflowXamlRefActivityV1DeployedActivityV2ShouldThrow()
        {
            var domain = this.CreateWorkerDomain(Constants.WorkflowXamlActivityV2);
            try
            {
                CreateTestWorker(domain).WorkflowXamlRefActivityV1DeployedActivityV2ShouldThrow();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a test worker in another domain
        /// </summary>
        /// <param name="domain">
        /// The domain.
        /// </param>
        /// <returns>
        /// A StaticXamlTestWorker
        /// </returns>
        private static StaticXamlTestWorker CreateTestWorker(AppDomain domain)
        {
            domain.Load(Assembly.GetExecutingAssembly().GetName().FullName);
            var worker =
                (StaticXamlTestWorker)
                domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, "Microsoft.Activities.Extensions.Tests.StaticXamlTestWorker");

            return worker;
        }

        /// <summary>
        /// The create worker domain.
        /// </summary>
        /// <param name="workerPath">
        /// The worker path.
        /// </param>
        /// <returns>
        /// Returns an appdomain
        /// </returns>
        private AppDomain CreateWorkerDomain(string workerPath)
        {
            return AppDomain.CreateDomain(
                this.TestContext.TestName, null, new AppDomainSetup { ApplicationBase = Path.Combine(this.TestContext.DeploymentDirectory, workerPath) });
        }

        #endregion
    }
}