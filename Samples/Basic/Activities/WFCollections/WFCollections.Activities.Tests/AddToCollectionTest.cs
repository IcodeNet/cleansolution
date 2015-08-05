// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddToCollectionTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   This is a test class for AddToCollectionTest and is intended
//   to contain all AddToCollectionTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities.Tests
{
    using System.Collections.Generic;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AddToCollectionTest and is intended
    ///   to contain all AddToCollectionTest Unit Tests
    /// </summary>
    [TestClass]
    public class AddToCollectionTest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given
        ///   * A workflow with an out argument Result of type List(Of Customer)
        ///   When
        ///   * The AddToCollection activity is invoked
        ///   Then
        ///   * The customer is added to the collection
        /// </summary>
        [TestMethod]
        public void ShouldAddACustomerToTheCollection()
        {
            var activity = new AddCustomerToCollectionActivity();
            var host = new WorkflowInvokerTest(activity);

            try
            {
                var output = host.TestActivity();

                host.AssertOutArgument.IsNotNull("Result");

                var collection = output["Result"] as List<Customer>;
                Assert.IsNotNull(collection);
                Assert.AreEqual(1, collection.Count);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Given
        ///   * A workflow with an out argument Result of type List(Of Customer)
        ///   When
        ///   * The AddToCollection activity is invoked twice
        ///   * with the same customer
        ///   Then
        ///   * The customer is added to the collection twice
        /// </summary>
        [TestMethod]
        public void WhatHappensWhenAddToCollectionAddsADuplicate()
        {
            var activity = new AddDuplicateCustomerToCollectionActivity();
            var host = new WorkflowInvokerTest(activity);

            try
            {
                var output = host.TestActivity();

                host.AssertOutArgument.IsNotNull("Result");

                var collection = output["Result"] as List<Customer>;

                Assert.IsNotNull(collection);
                Assert.AreEqual(2, collection.Count);

                this.TestContext.WriteLine("Customer list count {0}", collection.Count);
                foreach (var customer in collection)
                {
                    this.TestContext.WriteLine("Customer {0}", customer.Name);
                }
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}