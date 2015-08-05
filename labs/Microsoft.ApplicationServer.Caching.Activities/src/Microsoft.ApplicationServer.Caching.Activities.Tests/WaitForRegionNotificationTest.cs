#region copyright

// ----------------------------------------------------------------------------------
// Microsoft
//   
// Copyright (c) Microsoft Corporation. All rights reserved.
//   
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowTestHelper;

namespace Microsoft.ApplicationServer.Caching.Activities.Tests
{
    [TestClass]
    public class WaitForRegionNotificationTest
    {
        private const string NotificationCacheName = "NotificationCache";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        /// 
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WaitForCacheNotificationShouldGetNotificationOnReplaceWithRegion()
        {
            var expectedKey = "Key" + TestContext.TestName;
            var expectedOldValue = "oldValue" + TestContext.TestName;
            var expectedNewValue = "newValue" + TestContext.TestName;
            var expectedRegion = "Region" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetCache(NotificationCacheName);

            dc.CreateRegion(expectedRegion);
            dc.Remove(expectedKey, expectedRegion);
            dc.Add(expectedKey, expectedOldValue, expectedRegion);


            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForRegionNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                             Filter = DataCacheOperations.ReplaceItem,
                                                             Region = expectedRegion
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForIdleEvent(), "WaitForRegionNotification did not go idle");

            // Update the item
            var expectedVersion = dc.Put(expectedKey, expectedNewValue, expectedRegion);

            Assert.IsTrue(sut.WaitForCompletedEvent(2000), "WaitForRegionNotification did not complete");

            // Assert Out Arguments
            sut.AssertOutArgument.IsNotNull("NotificationDescriptor");
            sut.AssertOutArgument.AreEqual("Version", expectedVersion);
            sut.AssertOutArgument.AreEqual("CacheOperation", DataCacheOperations.ReplaceItem);
            sut.Tracking.Trace();
        }
    }
}