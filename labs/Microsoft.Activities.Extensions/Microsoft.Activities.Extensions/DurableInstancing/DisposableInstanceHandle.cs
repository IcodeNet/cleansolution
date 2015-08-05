// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableInstanceHandle.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.DurableInstancing
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.DurableInstancing;

    /// <summary>
    /// The disposable instance handle.
    /// </summary>
    public class DisposableInstanceHandle : IDisposable, IEquatable<DisposableInstanceHandle>
    {
        #region Fields

        /// <summary>
        /// The instance handle.
        /// </summary>
        private readonly InstanceHandle instanceHandle;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableInstanceHandle"/> class.
        /// </summary>
        /// <param name="instanceStore">
        /// The instance store.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The instance store is null
        /// </exception>
        public DisposableInstanceHandle(InstanceStore instanceStore)
        {
            Contract.Requires(instanceStore != null);
            if (instanceStore == null)
            {
                throw new ArgumentNullException("instanceStore");
            }

            this.instanceHandle = instanceStore.CreateInstanceHandle();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The ==.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// true if equal
        /// </returns>
        public static bool operator ==(DisposableInstanceHandle left, DisposableInstanceHandle right)
        {
            return left == right;
        }

        /// <summary>
        /// The op_ implicit.
        /// </summary>
        /// <param name="disposableInstanceHandle">
        /// The disposable instance handle.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The instance handle is null
        /// </exception>
        public static implicit operator InstanceHandle(DisposableInstanceHandle disposableInstanceHandle)
        {
            return disposableInstanceHandle != null ? disposableInstanceHandle.instanceHandle : null;
        }

        /// <summary>
        /// The !=.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// true if not equal
        /// </returns>
        public static bool operator !=(DisposableInstanceHandle left, DisposableInstanceHandle right)
        {
            return left != right;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false. 
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object. 
        /// </param>
        public bool Equals(DisposableInstanceHandle other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.instanceHandle == other.instanceHandle;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/> ; otherwise, false. 
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/> . 
        /// </param>
        /// <filterpriority>2</filterpriority>
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((DisposableInstanceHandle)obj);
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns> A hash code for the current <see cref="T:System.Object" /> . </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.instanceHandle != null ? this.instanceHandle.GetHashCode() : 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes of resources
        /// </summary>
        /// <param name="disposing">
        /// The disposing flag 
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.instanceHandle != null)
                {
                    this.instanceHandle.Free();
                }
            }
        }

        #endregion
    }
}