// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquatableCustomer.cs" company="Microsoft">
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
    /// To use a complex type like a EquatableCustomer in a generic collection
    ///   such as ICollection(Of T) you should implement the IEquatable(Of T) 
    ///   interface and also override the object.Equals and object.GetHashCode
    ///   interfaces to ensure that your objects will work correctly with 
    ///   collections.
    ///   You must decide on the property that will represent the unique
    ///   key for your object and use it to test equality as shown in this example
    ///   See the IEquatable(Of T).Equals method documentation for more information
    ///   http://msdn.microsoft.com/en-us/library/ms131190.aspx
    /// </remarks>
    public class EquatableCustomer : Customer, IEquatable<EquatableCustomer>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableCustomer"/> class.
        /// </summary>
        public EquatableCustomer()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableCustomer"/> class.
        /// </summary>
        /// <param name="other">
        /// The other customer.
        /// </param>
        public EquatableCustomer(Customer other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            this.Id = other.Id;
            this.Name = other.Name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The equals method.
        /// </summary>
        /// <param name="obj">
        /// The obj to test for equality.
        /// </param>
        /// <returns>
        /// true if the objects are equal
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(EquatableCustomer))
            {
                return false;
            }

            return this.Equals((EquatableCustomer)obj);
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion

        #region Implemented Interfaces

        #region IEquatable<EquatableCustomer>

        /// <summary>
        /// Tests for equality with another customer
        /// </summary>
        /// <param name="other">
        /// The other customer
        /// </param>
        /// <returns>
        /// true if the customers are equal
        /// </returns>
        public bool Equals(EquatableCustomer other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Id.Equals(this.Id);
        }

        #endregion

        #endregion
    }
}