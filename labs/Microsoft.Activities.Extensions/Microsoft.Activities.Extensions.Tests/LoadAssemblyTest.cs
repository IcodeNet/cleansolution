// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadAssemblyTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using Microsoft.Activities.Extensions.Statements;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for LoadAssemblyTest and is intended
    /// to contain all LoadAssemblyTest Unit Tests
    /// </summary>
    [TestClass]
    public class LoadAssemblyTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given 
        /// * an assembly that exists on disk
        /// When
        ///  * Load assembly is invoked with the correct path
        /// Then
        /// * The assembly should be loaded
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.ActivityV1)]
        public void LoadAssemblyShouldLoadAnAssembly()
        {
            var activity = new LoadAssembly();
            var host = WorkflowInvokerTest.Create(activity);
            dynamic arguments = new WorkflowArguments();
            arguments.Path = "ActivityLibrary.dll";
            try
            {
                host.TestActivity(arguments);
                Assert.IsNotNull(host.OutArguments.Assembly);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}