// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   A customer object ready to use with collections
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities
{
    using System;

    /// <summary>
    /// A customer object ready to use with collections
    /// </summary>
    /// <remarks>
    /// This customer will be considered equal only if it is the same reference
    /// </remarks>
    public class Customer
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Customer" /> class.
        /// </summary>
        public Customer()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="other">
        /// The other customer
        /// </param>
        /// <remarks>
        /// The copy constructor
        /// </remarks>
        public Customer(Customer other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            this.Id = other.Id;
            this.Name = other.Name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///   Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}