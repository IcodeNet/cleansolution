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

using System.Collections.Concurrent;

namespace CacheAsideExample
{
    public class SampleDataRepository
    {
        private static readonly ConcurrentDictionary<string, SampleData> RepositoryData =
            new ConcurrentDictionary<string, SampleData>();

        static SampleDataRepository()
        {
            for (var i = 0; i <= 20; i++)
            {
                RepositoryData.AddOrUpdate(i.ToString(),
                                           (key) =>
                                           new SampleData
                                               {
                                                   Name = string.Format("Name {0}", i),
                                                   Value = string.Format("Value {0}", i)
                                               },
                                           (key, existing) => existing);
            }
        }

        public static SampleData Get(string name)
        {
            SampleData result;

            if (RepositoryData.TryGetValue(name, out result))
            {
                return result;
            }

            return null;
        }
    }
}