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

using System;
using System.Activities;

namespace Microsoft.ApplicationServer.Caching.Activities
{
    public sealed class DataCacheRemove : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> Key { get; set; }
        public InArgument<string> CacheName { get; set; }
        public InArgument<string> Region { get; set; }
        public InArgument<DataCacheLockHandle> LockHandle { get; set; }
        public InArgument<DataCacheItemVersion> ItemVersion { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Will use config to obtain cache information
            metadata.RequireExtension(typeof (DataCacheFactory));
            metadata.AddDefaultExtensionProvider(() => new DataCacheFactory());

            metadata.AddArgument(new RuntimeArgument("Key", typeof (string), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("CacheName", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Region", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("LockHandle", typeof (DataCacheLockHandle), ArgumentDirection.In,
                                                     false));
            metadata.AddArgument(new RuntimeArgument("ItemVersion", typeof (DataCacheItemVersion), ArgumentDirection.In,
                                                     false));
        }

        protected override void Execute(CodeActivityContext context)
        {
            var key = Key.Get(context);
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("Key");

            var cacheName = CacheName.Get(context);

            // Get the factory
            var dcf = context.GetExtension<DataCacheFactory>();

            // Get the DataCache
            var dataCache = string.IsNullOrWhiteSpace(cacheName) ? dcf.GetDefaultCache() : dcf.GetCache(cacheName);

            var region = Region.Get(context);
            var lockHandle = LockHandle.Get(context);
            var itemVersion = ItemVersion.Get(context);

            if (lockHandle != null)
            {
                if (region != null)
                    dataCache.Remove(key, lockHandle, region);
                else
                    dataCache.Remove(key, lockHandle);
            }
            else if (itemVersion != null)
            {
                if (region != null)
                    dataCache.Remove(key, itemVersion, region);
                else
                    dataCache.Remove(key, itemVersion);
            }
            else
            {
                if (region != null)
                    dataCache.Remove(key, region);
                else
                    dataCache.Remove(key);
            }
        }
    }
}