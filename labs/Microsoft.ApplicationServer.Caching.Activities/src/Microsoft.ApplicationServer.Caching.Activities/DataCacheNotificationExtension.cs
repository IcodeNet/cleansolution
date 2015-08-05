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

using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;

namespace Microsoft.ApplicationServer.Caching.Activities
{
    internal class DataCacheNotificationExtension : IWorkflowInstanceExtension
    {
        private WorkflowInstanceProxy _instance;

        #region IWorkflowInstanceExtension Members

        public IEnumerable<object> GetAdditionalExtensions()
        {
            return null;
        }

        public void SetInstance(WorkflowInstanceProxy instance)
        {
            _instance = instance;
        }

        #endregion

        internal void CacheNotificationCallback(
            string cacheName,
            string regionName,
            string key,
            DataCacheItemVersion version,
            DataCacheOperations cacheOperation,
            DataCacheNotificationDescriptor nd)
        {
            var notification = new ItemNotification
                                   {
                                       CacheName = cacheName,
                                       RegionName = regionName,
                                       Key = key,
                                       Version = version,
                                       CacheOperation = cacheOperation,
                                       NotificationDescriptor = nd
                                   };

            _instance.BeginResumeBookmark(
                new Bookmark(WaitForItemNotification.GetNotificationBookmarkName(nd)),
                notification,
                (asr) => _instance.EndResumeBookmark(asr),
                null);
        }

        internal void NotificationFailureCallback(string cacheName, DataCacheNotificationDescriptor nd)
        {
            var notification = new NotificationFailure
                                   {
                                       CacheName = cacheName,
                                       NotificationDescriptor = nd
                                   };

            _instance.BeginResumeBookmark(
                new Bookmark(WaitForItemNotification.GetNotificationBookmarkName(nd)),
                notification,
                (asr) => _instance.EndResumeBookmark(asr),
                null);
        }


        internal void BulkNotificationCallback(
            string cacheName,
            IEnumerable<DataCacheOperationDescriptor> operations,
            DataCacheNotificationDescriptor nd)
        {
            var notification = new BulkNotification
                                   {
                                       CacheName = cacheName,
                                       Operations = operations,
                                       NotificationDescriptor = nd
                                   };

            _instance.BeginResumeBookmark(
                new Bookmark(WaitForItemNotification.GetNotificationBookmarkName(nd)),
                notification,
                (asr) => _instance.EndResumeBookmark(asr),
                null);
        }
    }
}