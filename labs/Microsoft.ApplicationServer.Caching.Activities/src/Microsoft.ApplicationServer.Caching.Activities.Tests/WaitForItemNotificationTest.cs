﻿#region copyright

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
    public class WaitForItemNotificationTest
    {
        private const string NotificationCacheName = "NotificationCache";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WaitForItemNotificationShouldGetNotificationOnReplace()
        {
            var expectedKey = "Key" + TestContext.TestName;
            var expectedOldValue = "oldValue" + TestContext.TestName;
            var expectedNewValue = "newValue" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetCache(NotificationCacheName);

            dc.Remove(expectedKey);
            dc.Add(expectedKey, expectedOldValue);


            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForItemNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                             Key = expectedKey,
                                                             Filter = DataCacheOperations.ReplaceItem
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForIdleEvent(), "WaitForItemNotification did not go idle");

            // Update the item
            var expectedVersion = dc.Put(expectedKey, expectedNewValue);

            Assert.IsTrue(sut.WaitForCompletedEvent(2000), "WaitForItemNotification did not complete");

            // Assert Out Arguments
            sut.AssertOutArgument.IsNotNull("NotificationDescriptor");
            sut.AssertOutArgument.AreEqual("Version", expectedVersion);
            sut.AssertOutArgument.AreEqual("CacheOperation", DataCacheOperations.ReplaceItem);
            sut.Tracking.Trace();
        }

        [TestMethod]
        public void WaitForItemNotificationShouldGetNotificationOnReplaceWithRegion()
        {
            var expectedKey = "Key" + TestContext.TestName;
            var expectedRegion = "Region" + TestContext.TestName;
            var expectedOldValue = "oldValue" + TestContext.TestName;
            var expectedNewValue = "newValue" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetCache(NotificationCacheName);

            dc.CreateRegion(expectedRegion);
            dc.Remove(expectedKey, expectedRegion);
            dc.Add(expectedKey, expectedOldValue, expectedRegion);


            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForItemNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                             Key = expectedKey,
                                                             Region = expectedRegion,
                                                             Filter = DataCacheOperations.ReplaceItem
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForIdleEvent(), "WaitForItemNotification did not go idle");

            // Update the item
            var expectedVersion = dc.Put(expectedKey, expectedNewValue, expectedRegion);

            Assert.IsTrue(sut.WaitForCompletedEvent(2000), "WaitForItemNotification did not complete");

            // Assert
            sut.AssertOutArgument.IsNotNull("NotificationDescriptor");
            sut.AssertOutArgument.AreEqual("Version", expectedVersion);
            sut.AssertOutArgument.AreEqual("CacheOperation", DataCacheOperations.ReplaceItem);
            sut.Tracking.Trace();
        }


        [TestMethod]
        public void WaitForItemNotificationShouldAbortOnNullKey()
        {
            string expectedKey = null;

            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForItemNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                             Key = expectedKey,
                                                             Filter = DataCacheOperations.ReplaceItem
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForAbortedEvent(), "WaitForItemNotification did not abort");

            // Assert
            Assert.IsNotNull(sut.Results.AbortedArgs);
        }

        [TestMethod]
        public void WaitForItemNotificationShouldAbortOnCacheNameDoesNotExist()
        {
            var expectedKey = "Key" + TestContext.TestName;

            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForItemNotification
                                                         {
                                                             CacheName = "BadCacheName",
                                                             Key = expectedKey,
                                                             Filter = DataCacheOperations.ReplaceItem
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForAbortedEvent(), "WaitForItemNotification did not abort");

            // Assert
            Assert.IsNotNull(sut.Results.AbortedArgs);
        }

        [TestMethod]
        public void WaitForItemNotificationShouldAbortOnInvalidFilter()
        {
            var expectedKey = "Key" + TestContext.TestName;

            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForItemNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                             Key = expectedKey,
                                                             // Don't set the filter
                                                             Filter = default(DataCacheOperations)
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForAbortedEvent(), "WaitForItemNotification did not abort");

            // Assert
            Assert.IsNotNull(sut.Results.AbortedArgs);
        }
    }
}