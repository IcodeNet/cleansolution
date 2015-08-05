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
using System.Collections.Generic;

namespace Microsoft.ApplicationServer.Caching.Activities
{
    public sealed class DataCacheAdd<T> : CodeActivity<DataCacheItemVersion>
    {
        // Define an activity input argument of type string
        public InArgument<string> Key { get; set; }
        public InArgument<T> Value { get; set; }
        public InArgument<string> CacheName { get; set; }
        public InArgument<string> Region { get; set; }
        public InArgument<TimeSpan> Timeout { get; set; }
        public InArgument<IEnumerable<DataCacheTag>> Tags { get; set; }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Will use config to obtain cache information
            metadata.RequireExtension(typeof (DataCacheFactory));
            metadata.AddDefaultExtensionProvider(() => new DataCacheFactory());

            metadata.AddArgument(new RuntimeArgument("Key", typeof (string), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("CacheName", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Region", typeof (string), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Timeout", typeof (TimeSpan), ArgumentDirection.In, false));
            metadata.AddArgument(new RuntimeArgument("Tags", typeof (IEnumerable<DataCacheTag>), ArgumentDirection.In,
                                                     false));
            metadata.AddArgument(new RuntimeArgument("Value", typeof (T), ArgumentDirection.In, true));
        }

        protected override DataCacheItemVersion Execute(CodeActivityContext context)
        {
            var key = Key.Get(context);

            // Cannot store with a null key
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("Key");

            // Can store any value including null if you want

            var cacheName = CacheName.Get(context);

            // Get the factory
            var dcf = context.GetExtension<DataCacheFactory>();

            // Get the DataCache
            var dataCache = string.IsNullOrWhiteSpace(cacheName) ? dcf.GetDefaultCache() : dcf.GetCache(cacheName);

            var tags = Tags.Get(context);
            var timeout = Timeout.Get(context);
            var region = Region.Get(context);
            var value = Value.Get(context);

            // Add the value - working our way through the overloads)
            if (timeout != default(TimeSpan))
            {
                return dataCache.Add(key, value, timeout, tags, region);
            }
            else if (tags != null)
            {
                return dataCache.Add(key, value, tags, region);
            }
            else if (region != null)
            {
                return dataCache.Add(key, value, region);
            }
            else
            {
                return dataCache.Add(key, value);
            }
        }
    }
}