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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationServer.Caching.Activities.Tests
{
    [TestClass]
    public class DataCachePutTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void PuKeyValueShouldAddToCache()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            var sut = new DataCachePut<string> {Key = key, Value = expectedValue};

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut);

            // Get it from the cache
            var value = dc.Get(key) as string;

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public void PuKeyValueShouldUpdateCache()
        {
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);
            dc.Add(key, expectedOldValue);

            var sut = new DataCachePut<string> {Key = key, Value = expectedNewValue};

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut);

            // Get it from the cache
            var value = dc.Get(key) as string;

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedNewValue, value);
        }

        [TestMethod]
        public void PuKeyValueRegionShouldAddToCache()
        {
            var expectedValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            var sut = new DataCachePut<string> {Key = key, Value = expectedValue, Region = region};

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut);

            // Get it from the cache
            var value = dc.Get(key, region) as string;

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public void PuKeyValueRegionShouldUpdateCache()
        {
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;

            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);

            dc.Remove(key, region);
            dc.Add(key, expectedOldValue, region);

            var sut = new DataCachePut<string> {Key = key, Value = expectedNewValue, Region = region};

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut);

            // Get it from the cache
            var value = dc.Get(key, region) as string;

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedNewValue, value);
        }

        [TestMethod]
        public void PutKeyValueRegionTagsShouldAddToCache()
        {
            const int expectedCount = 1;
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var tags = new List<DataCacheTag>
                           {
                               new DataCacheTag("tag1" + TestContext.TestName),
                               new DataCacheTag("tag2" + TestContext.TestName),
                               new DataCacheTag("tag3" + TestContext.TestName)
                           };
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            var sut = new DataCachePut<string>();
            // To use Object Initialization syntax you would do the following
            // { Key = key, Value = expectedValue, Region = region, Tags = new LambdaValue<IEnumerable<DataCacheTag>>((env) =>tags)};

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Value", expectedValue},
                                {"Region", region},
                                {"Tags", tags},
                            };

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut, input);

            // Get it from the cache
            var values = dc.GetObjectsByAllTags(tags, region);

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.AreEqual(expectedValue, values.First().Value);
        }


        [TestMethod]
        public void PutKeyValueRegionTagsShouldUpdateCache()
        {
            const int expectedCount = 1;
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var tags = new List<DataCacheTag>
                           {
                               new DataCacheTag("tag1" + TestContext.TestName),
                               new DataCacheTag("tag2" + TestContext.TestName),
                               new DataCacheTag("tag3" + TestContext.TestName)
                           };
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            // The old value had no tags
            dc.Add(key, expectedOldValue, region);

            var sut = new DataCachePut<string>();
            // To use Object Initialization syntax you would do the following
            // { Key = key, Value = expectedValue, Region = region, Tags = new LambdaValue<IEnumerable<DataCacheTag>>((env) =>tags)};

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Value", expectedNewValue},
                                {"Region", region},
                                {"Tags", tags},
                            };

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut, input);

            // Get it from the cache
            var values = dc.GetObjectsByAllTags(tags, region);

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.AreEqual(expectedNewValue, values.First().Value);
        }

        [TestMethod]
        public void PutKeyValueRegionTagsTimeoutShouldAddToCache()
        {
            const int expectedCount = 1;
            var expectedTimeout = TimeSpan.FromMinutes(20);
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var tags = new List<DataCacheTag>
                           {
                               new DataCacheTag("tag1" + TestContext.TestName),
                               new DataCacheTag("tag2" + TestContext.TestName),
                               new DataCacheTag("tag3" + TestContext.TestName)
                           };
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            var sut = new DataCachePut<string>();
            // To use Object Initialization syntax you would do the following
            // { Key = key, Value = expectedValue, Region = region, Tags = new LambdaValue<IEnumerable<DataCacheTag>>((env) =>tags)};

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Value", expectedValue},
                                {"Region", region},
                                {"Tags", tags},
                                {"Timeout", expectedTimeout}
                            };

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut, input);

            // Get it from the cache
            var values = dc.GetObjectsByAllTags(tags, region);
            var item = dc.GetCacheItem(key, region);

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.AreEqual(expectedValue, item.Value);
            Assert.IsTrue(item.Timeout > TimeSpan.MinValue && item.Timeout < expectedTimeout);
        }


        [TestMethod]
        public void PutKeyValueRegionTagsTimeoutShouldUpdateCache()
        {
            const int expectedCount = 1;
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var expectedOldTimeout = TimeSpan.FromMinutes(20);
            var expectedNewTimeout = TimeSpan.FromMinutes(30);

            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var tags = new List<DataCacheTag>
                           {
                               new DataCacheTag("tag1" + TestContext.TestName),
                               new DataCacheTag("tag2" + TestContext.TestName),
                               new DataCacheTag("tag3" + TestContext.TestName)
                           };
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            dc.Add(key, expectedOldValue, expectedOldTimeout, region);

            var sut = new DataCachePut<string>();
            // To use Object Initialization syntax you would do the following
            // { Key = key, Value = expectedValue, Region = region, Tags = new LambdaValue<IEnumerable<DataCacheTag>>((env) =>tags)};

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Value", expectedNewValue},
                                {"Region", region},
                                {"Tags", tags},
                                {"Timeout", expectedNewTimeout}
                            };

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut, input);

            // Get it from the cache
            var values = dc.GetObjectsByAllTags(tags, region);
            var item = dc.GetCacheItem(key, region);

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.AreEqual(expectedNewValue, item.Value);

            // The timeout counts down so it should be greater than the old timeout
            Assert.IsTrue(item.Timeout > expectedOldTimeout);
        }

        [TestMethod]
        public void PutKeyValueRegionTagsTimeoutOldVersionShouldUpdateCache()
        {
            const int expectedCount = 1;
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var expectedOldTimeout = TimeSpan.FromMinutes(20);
            var expectedNewTimeout = TimeSpan.FromMinutes(30);

            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var tags = new List<DataCacheTag>
                           {
                               new DataCacheTag("tag1" + TestContext.TestName),
                               new DataCacheTag("tag2" + TestContext.TestName),
                               new DataCacheTag("tag3" + TestContext.TestName)
                           };
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            // Clean up previous entries
            dc.CreateRegion(region);
            dc.Remove(key, region);

            var oldVersion = dc.Add(key, expectedOldValue, expectedOldTimeout, region);

            var sut = new DataCachePut<string>();
            // To use Object Initialization syntax you would do the following
            // { Key = key, Value = expectedValue, Region = region, Tags = new LambdaValue<IEnumerable<DataCacheTag>>((env) =>tags)};

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Value", expectedNewValue},
                                {"Region", region},
                                {"Tags", tags},
                                {"Timeout", expectedNewTimeout},
                                {"OldVersion", oldVersion}
                            };

            // Put an item in the cache using the activity
            var itemVersion = WorkflowInvoker.Invoke(sut, input);

            // Get it from the cache
            var values = dc.GetObjectsByAllTags(tags, region);
            var item = dc.GetCacheItem(key, region);

            // Assert
            Assert.IsNotNull(itemVersion);
            Assert.AreEqual(expectedCount, values.Count());
            Assert.AreEqual(expectedNewValue, item.Value);

            // The timeout counts down so it should be greater than the old timeout
            Assert.IsTrue(item.Timeout > expectedOldTimeout);
        }
    }
}