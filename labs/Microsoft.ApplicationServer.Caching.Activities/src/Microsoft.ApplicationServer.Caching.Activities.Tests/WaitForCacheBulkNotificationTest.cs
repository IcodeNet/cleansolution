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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowTestHelper;

namespace Microsoft.ApplicationServer.Caching.Activities.Tests
{
    [TestClass]
    public class WaitForCacheBulkNotificationTest
    {
        // TODO: When testing be sure to create a cache using the PowerShell command
        // New-Cache NotificationCache -NotificationsEnabled true
        private const string NotificationCacheName = "NotificationCache";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WaitForItemNotificationShouldGetBulkNotification()
        {
            var expectedKey = "Key" + TestContext.TestName;
            var expectedOldValue = "oldValue" + TestContext.TestName;
            var expectedNewValue = "newValue" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetCache(NotificationCacheName);

            // Create a workflow that waits for a notification
            var sut = WorkflowApplicationTest.Create(new WaitForCacheBulkNotification
                                                         {
                                                             CacheName = NotificationCacheName,
                                                         });

            // Will catch if the workflow aborts
            sut.Aborted = (args) => TestContext.WriteLine("Workflow Aborted {0}", args.Reason);

            sut.TestActivity();

            // Wait for the workflow to go idle
            Assert.IsTrue(sut.WaitForIdleEvent(), "WaitForCacheBulkNotification did not go idle");

            dc.Remove(expectedKey);
            dc.Add(expectedKey, expectedOldValue);

            // Update the item
            var expectedVersion = dc.Put(expectedKey, expectedNewValue);

            Assert.IsTrue(sut.WaitForCompletedEvent(2000), "WaitForCacheBulkNotification did not complete");

            // Assert Out Arguments
            sut.AssertOutArgument.IsNotNull("NotificationDescriptor");
            sut.AssertOutArgument.IsNotNull("Operations");

            var operations = (IEnumerable<DataCacheOperationDescriptor>) sut.Results.Output["Operations"];

            // Notifications are timing sensitive - the RemoveItem may or may not be included in the bulk get
            //Assert.AreEqual(DataCacheOperations.RemoveItem, operations.ElementAt(0).OperationType);
            //Assert.AreEqual(DataCacheOperations.AddItem, operations.ElementAt(1).OperationType);
            //Assert.AreEqual(DataCacheOperations.ReplaceItem, operations.ElementAt(2).OperationType);

            sut.Tracking.Trace();
        }
    }
}