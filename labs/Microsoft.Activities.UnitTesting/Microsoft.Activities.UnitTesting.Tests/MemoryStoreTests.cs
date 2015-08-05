// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryStoreTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Diagnostics;
    using System.Runtime.DurableInstancing;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Persistence;
    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Tests for the MemoryStore
    /// </summary>
    [TestClass]
    public class MemoryStoreTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication
        ///   * A MemoryStore as the InstanceStore
        ///   When
        ///   * The Workflow becomes idle
        ///   * The code tries to load the workflow
        ///   Then
        ///   * The workflow should load
        /// </summary>
        [TestMethod]
        public void MemoryStoreShouldSupportLoad()
        {
            var instanceStore = new MemoryStore();
            var host1 = CreateTestHost(instanceStore);
            var host2 = CreateTestHost(instanceStore);

            try
            {
                Debug.Assert(host1 != null, "host1 != null");
                host1.TestWorkflowApplication.RunEpisode();
                Assert.IsTrue(host1.WaitForUnloadedEvent(Constants.Timeout));

                Debug.Assert(host2 != null, "host2 != null");
                Debug.Assert(host2.TestWorkflowApplication != null, "host2.TestWorkflowApplication != null");
                Debug.Assert(host1.TestWorkflowApplication != null, "host1.TestWorkflowApplication != null");
                host2.TestWorkflowApplication.Load(host1.TestWorkflowApplication.Id);
                host2.TestWorkflowApplication.Run();
                Assert.IsTrue(host2.WaitForUnloadedEvent(Constants.Timeout));
            }
            finally
            {
                Trace.WriteLine("*** Test Host 1 Tracking ***");
                Debug.Assert(host1 != null, "host1 != null");
                Debug.Assert(host1.Tracking != null, "host1.Tracking != null");
                host1.Tracking.Trace();
                Trace.WriteLine("*** Test Host 2 Tracking ***");
                Debug.Assert(host2 != null, "host2 != null");
                Debug.Assert(host2.Tracking != null, "host2.Tracking != null");
                host2.Tracking.Trace();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a test host
        /// </summary>
        /// <param name="instanceStore">
        /// The instance store. 
        /// </param>
        /// <returns>
        /// The test host 
        /// </returns>
        private static WorkflowApplicationTest<ActivityWithDelay> CreateTestHost(InstanceStore instanceStore)
        {
            var host = WorkflowApplicationTest.Create(new ActivityWithDelay());
            Debug.Assert(host != null, "host != null");
            host.InstanceStore = instanceStore;
            host.PersistableIdle += args => PersistableIdleAction.Unload;
            return host;
        }

        #endregion
    }
}