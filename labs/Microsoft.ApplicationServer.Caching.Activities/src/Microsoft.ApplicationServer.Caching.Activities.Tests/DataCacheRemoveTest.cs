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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationServer.Caching.Activities.Tests
{
    /// <summary>
    ///This is a test class for DataCacheGetTest and is intended
    ///to contain all DataCacheGetTest Unit Tests
    ///</summary>
    [TestClass]
    public class DataCacheRemoveTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        [TestMethod]
        public void ShouldRemoveFromCache()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            dc.Add(key, expectedValue);

            var sut = new DataCacheRemove {Key = key};

            WorkflowInvoker.Invoke(sut);

            var value = dc.Get(key);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldRemoveFromCacheRegion()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.CreateRegion(region);
            dc.Remove(key, region);

            // Add the value
            dc.Add(key, expectedValue, region);

            var sut = new DataCacheRemove {Key = key, Region = region};

            WorkflowInvoker.Invoke(sut);

            var value = dc.Get(key, region);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldRemoveFromCacheItemVersion()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            var itemVersion = dc.Add(key, expectedValue);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"ItemVersion", itemVersion},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var value = dc.Get(key);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldRemoveFromCacheItemVersionRegion()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.CreateRegion(region);
            dc.Remove(key, region);

            // Add the value
            var itemVersion = dc.Add(key, expectedValue, region);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Region", region},
                                {"ItemVersion", itemVersion},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var value = dc.Get(key, region);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldFailRemoveFromCacheItemVersionWithUpdate()
        {
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            var itemVersion = dc.Add(key, expectedOldValue);
            // Will update the item version
            var newItemVersion = dc.Put(key, expectedNewValue, itemVersion);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"ItemVersion", itemVersion},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var item = dc.GetCacheItem(key);

            // Assert
            Assert.IsNotNull(item);
            Assert.AreEqual(newItemVersion, item.Version);
        }

        [TestMethod]
        public void ShouldRemoveFromCacheLockHandle()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            var itemVersion = dc.Add(key, expectedValue);
            DataCacheLockHandle lockHandle;
            dc.GetAndLock(key, TimeSpan.FromMinutes(5), out lockHandle);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"LockHandle", lockHandle},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var value = dc.Get(key);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldRemoveFromCacheLockHandleRegion()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var region = "Region" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.CreateRegion(region);
            dc.Remove(key, region);

            // Add the value
            var itemVersion = dc.Add(key, expectedValue, region);
            DataCacheLockHandle lockHandle;
            dc.GetAndLock(key, TimeSpan.FromMinutes(5), out lockHandle, region);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"Region", region},
                                {"LockHandle", lockHandle},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var value = dc.Get(key, region);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldFailRemoveFromCacheLockHandleWithUpdate()
        {
            var expectedOldValue = TestContext.TestName;
            var expectedNewValue = "Updated " + TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            var itemVersion = dc.Add(key, expectedOldValue);
            // Will update the item version
            var newItemVersion = dc.Put(key, expectedNewValue, itemVersion);

            DataCacheLockHandle lockHandle;
            dc.GetAndLock(key, TimeSpan.FromMinutes(5), out lockHandle);

            var sut = new DataCacheRemove();

            // Using input arguments
            var input = new Dictionary<string, object>
                            {
                                {"Key", key},
                                {"LockHandle", lockHandle},
                            };

            WorkflowInvoker.Invoke(sut, input);

            var item = dc.GetCacheItem(key);

            // Assert

            // Version does not matter when removing with a lock handle
            Assert.IsNull(item);
        }
    }
}