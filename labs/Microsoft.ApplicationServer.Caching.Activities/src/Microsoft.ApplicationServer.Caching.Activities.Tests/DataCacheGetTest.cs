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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationServer.Caching.Activities.Tests
{
    /// <summary>
    ///This is a test class for DataCacheGetTest and is intended
    ///to contain all DataCacheGetTest Unit Tests
    ///</summary>
    [TestClass]
    public class DataCacheGetTest
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
        public void ShouldGetFromCache()
        {
            var expectedValue = TestContext.TestName;
            var key = "Key" + TestContext.TestName;
            var dcf = new DataCacheFactory();
            var dc = dcf.GetDefaultCache();

            dc.Remove(key);

            // Add the value
            dc.Add(key, expectedValue);

            var sut = new DataCacheGet<string> {Key = key};

            var value = WorkflowInvoker.Invoke(sut);

            // Assert
            Assert.AreEqual(expectedValue, value);
        }

        [TestMethod]
        public void ShouldGetFromCacheRegion()
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

            var sut = new DataCacheGet<string> {Key = key, Region = region};

            var value = WorkflowInvoker.Invoke(sut);

            // Assert
            Assert.AreEqual(expectedValue, value);
        }
    }
}