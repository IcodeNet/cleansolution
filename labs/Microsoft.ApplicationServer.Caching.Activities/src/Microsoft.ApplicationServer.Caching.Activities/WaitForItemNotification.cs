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

using System;
using System.Activities;

namespace Microsoft.ApplicationServer.Caching.Activities
{
    public sealed class WaitForItemNotification : NativeActivity
    {
        public const string BookmarkName = "CacheNotification_{0}_{1}";
        private readonly Variable<NoPersistHandle> _noPersistHandle = new Variable<NoPersistHandle>();

        private BookmarkCallback _notificationBookmarkCallback;

        public InArgument<string> Key { get; set; }
        public InArgument<string> CacheName { get; set; }
        public InArgument<string> Region { get; set; }
        public InArgument<DataCacheOperations> Filter { get; set; }

        public OutArgument<DataCacheItemVersion> Version { get; set; }
        public OutArgument<DataCacheOperations> CacheOperation { get; set; }
        public OutArgument<DataCacheNotificationDescriptor> NotificationDescriptor { get; set; }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public BookmarkCallback NotificationBookmarkCallback
        {
            get
            {
                return _notificationBookmarkCallback ??
                       (_notificationBookmarkCallback = new BookmarkCallback(OnNotificationCallback));
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Will use config to obtain cache information
            metadata.RequireExtension(typeof (DataCacheFactory));
            metadata.AddDefaultExtensionProvider(() => new DataCacheFactory());

            metadata.RequireExtension(typeof (DataCacheNotificationExtension));
            metadata.AddDefaultExtensionProvider(() => new DataCacheNotificationExtension());

            metadata.AddArgument(new RuntimeArgument("Key", typeof (string), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("CacheName", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Region", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Filter", typeof (DataCacheOperations), ArgumentDirection.In,
                                                     true));
            metadata.AddArgument(new RuntimeArgument("CacheOperation", typeof (DataCacheOperations),
                                                     ArgumentDirection.Out, false));
            metadata.AddArgument(new RuntimeArgument("NotificationDescriptor", typeof (DataCacheNotificationDescriptor),
                                                     ArgumentDirection.Out, false));
            metadata.AddArgument(new RuntimeArgument("Version", typeof (DataCacheItemVersion), ArgumentDirection.Out,
                                                     false));

            metadata.AddImplementationVariable(_noPersistHandle);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var key = Key.Get(context);
            var filter = Filter.Get(context);
            var cacheName = CacheName.Get(context);
            var region = Region.Get(context);

            var dcf = context.GetExtension<DataCacheFactory>();
            var notificationExtension = context.GetExtension<DataCacheNotificationExtension>();

            var dataCache = string.IsNullOrWhiteSpace(cacheName) ? dcf.GetDefaultCache() : dcf.GetCache(cacheName);
            var nd = string.IsNullOrWhiteSpace(region)
                         ? dataCache.AddItemLevelCallback(key, filter, notificationExtension.CacheNotificationCallback)
                         : dataCache.AddItemLevelCallback(key, filter, notificationExtension.CacheNotificationCallback,
                                                          region);

            // Enter a no persist zone to pin this activity to memory since we are setting up a delegate to receive a callback
            var handle = _noPersistHandle.Get(context);
            handle.Enter(context);
            context.CreateBookmark(GetNotificationBookmarkName(nd), NotificationBookmarkCallback);
        }

        internal static string GetNotificationBookmarkName(DataCacheNotificationDescriptor nd)
        {
            return string.Format(BookmarkName, nd.CacheName, nd.DelegateId);
        }

        internal void OnNotificationCallback(NativeActivityContext context, Bookmark bookmark, Object value)
        {
            var notification = (ItemNotification) value;

            Version.Set(context, notification.Version);
            CacheOperation.Set(context, notification.CacheOperation);
            NotificationDescriptor.Set(context, notification.NotificationDescriptor);

            // Exit the no persist zone 
            var handle = _noPersistHandle.Get(context);
            handle.Exit(context);
        }
    }
}