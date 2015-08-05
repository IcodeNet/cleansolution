// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureCustomerIsInListTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The ensure customer is in list test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The ensure customer is in list test.
    /// </summary>
    [TestClass]
    public class EnsureCustomerIsInListTest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given
        ///   * A workflow with a variable of type List(Of Customer)
        ///   * And an InArgument(Of Customer) 
        ///   * That does not exist in the list
        ///   When
        ///   * The activity is invoked
        ///   Then
        ///   * The customer is added to the list
        /// </summary>
        [TestMethod]
        public void AddCustomerIfNotInList()
        {
            var collection = new Collection<Customer>();
            var customer = new Customer();
            var activity = new EnsureCustomerIsInList();
            var host = new WorkflowInvokerTest(activity);

            var inputs = new Dictionary<string, object> { { "CustomerCollection", collection }, { "Customer", customer } };

            try
            {
                host.TestActivity(inputs);
                Assert.IsTrue(collection.Contains(customer));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Demonstrates how reference based equality can cause problems for 
        ///   workflow authors
        /// </summary>
        /// <remarks>
        /// The EnsureCustomerIsInList activity tries to find a customer object
        ///   If the customer is not found, it adds the customer to the collection
        ///   This uses the Object.Equals notion of equality which means the objects
        ///   are equal if the reference is equal.
        ///   In this case we don't want reference equivalence but rather
        ///   semantic equivalence.  We have to ask the question 
        ///   what makes two customers equal?
        ///   In this sample customers are equal if the Customer.Id property is equal.  
        ///   This test illustrates the problem of reference equivalence because
        ///   the activity will not find a match and add a second copy of the activity
        /// </remarks>
        [TestMethod]
        public void WhatHappensIfCopiedCustomerExists()
        {
            var collection = new Collection<Customer>();
            var customer1 = new Customer();

            // Add the customer to the list
            collection.Add(customer1);

            // Create a copy of the same customer
            var customer2 = new Customer(customer1);

            var activity = new EnsureCustomerIsInList();
            var host = new WorkflowInvokerTest(activity);

            // Pass the copy to the activity
            var inputs = new Dictionary<string, object> { { "CustomerCollection", collection }, { "Customer", customer2 } };

            try
            {
                host.TestActivity(inputs);

                this.TestContext.WriteLine("Customer collection count {0}", collection.Count);
                foreach (var cust in collection)
                {
                    this.TestContext.WriteLine("Customer ID {0}", cust.Id);
                }

                // The copy will be added because the reference is not equal
                // This is probably not be what you want
                Assert.IsTrue(collection.Contains(customer1));
                Assert.IsTrue(collection.Contains(customer2));

                // Both customer and customer2 are in the collection
                Assert.AreEqual(2, collection.Count);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        /// Demonstrates how reference based equality can cause problems for 
        ///   workflow authors
        /// </summary>
        /// <remarks>
        /// The EnsureCustomerIsInList activity tries to find a customer object
        ///   If the customer is not found, it adds the customer to the collection
        ///   This uses the Object.Equals notion of equality which means the objects
        ///   are equal if the reference is equal.
        ///   In this case we don't want reference equivalence but rather
        ///   semantic equivalence.  We have to ask the question 
        ///   what makes two customers equal?
        ///   In this sample customers are equal if the Customer.Id property is equal.  
        ///   This test illustrates how to solve the problem of reference equivalence because
        ///   the activity will now find a match and not add a second copy of the activity
        /// </remarks>
        [TestMethod]
        public void WhatHappensIfCopiedEquatableCustomerExists()
        {
            var collection = new Collection<Customer>();
            var customer1 = new EquatableCustomer();

            // Add the customer to the list
            collection.Add(customer1);

            // Create a copy of the same customer
            var customer2 = new EquatableCustomer(customer1);

            var activity = new EnsureCustomerIsInList();
            var host = new WorkflowInvokerTest(activity);

            // Pass the copy to the activity
            var inputs = new Dictionary<string, object> { { "CustomerCollection", collection }, { "Customer", customer2 } };

            try
            {
                host.TestActivity(inputs);

                this.TestContext.WriteLine("Customer collection count {0}", collection.Count);
                foreach (var cust in collection)
                {
                    this.TestContext.WriteLine("Customer ID {0}", cust.Id);
                }

                // The copy will not be added because the reference is not equal
                // collection.Contains will return true for both because of the IEquatable interface
                Assert.IsTrue(collection.Contains(customer1));
                Assert.IsTrue(collection.Contains(customer2));

                // Only 1 customer is in the collection
                Assert.AreEqual(1, collection.Count);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}