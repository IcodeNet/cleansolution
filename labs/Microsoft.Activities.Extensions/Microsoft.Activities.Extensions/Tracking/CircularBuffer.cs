// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A circular buffer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the buffer 
    /// </typeparam>
    internal class CircularBuffer<T> : IProducerConsumerCollection<T>
    {
        #region Fields

        /// <summary>
        ///   The queue.
        /// </summary>
        private readonly ConcurrentQueue<T> queue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="capacity">
        /// The capacity. 
        /// </param>
        public CircularBuffer(int capacity)
        {
            this.Capacity = capacity;
            this.queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="items">
        /// The items. 
        /// </param>
        /// <param name="capacity">
        /// The capacity. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The
        ///   <paramref name="items"/>
        ///   argument was null/&gt;
        /// </exception>
        public CircularBuffer(IEnumerable<T> items, int capacity)
        {
            Contract.Requires(items != null);
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.Capacity = capacity;
            this.queue = new ConcurrentQueue<T>(items);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the capacity.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        ///   Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.queue.Count;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether is synchronized.
        /// </summary>
        /// <exception cref="NotSupportedException">Set is not supported</exception>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets the sync root.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attempts to add an object to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> . 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The
        ///   <paramref name="item"/>
        ///   was invalid for this collection.
        /// </exception>
        public void Add(T item)
        {
            lock (this.queue)
            {
                if (this.queue.Count >= this.Capacity)
                {
                    T result;

                    if (!this.queue.TryDequeue(out result))
                    {
                        throw new InvalidOperationException("Unable to remove item from circular buffer");
                    }

                    var disposable = result as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                this.queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/> . The <see cref="T:System.Array"/> must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array"/> at which copying begins. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/>
        ///   is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/>
        ///   is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/>
        ///   is multidimensional.-or- The number of elements in the source
        ///   <see cref="T:System.Collections.ICollection"/>
        ///   is greater than the available space from
        ///   <paramref name="index"/>
        ///   to the end of the destination
        ///   <paramref name="array"/>
        ///   .
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source
        ///   <see cref="T:System.Collections.ICollection"/>
        ///   cannot be cast automatically to the type of the destination
        ///   <paramref name="array"/>
        ///   .
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            this.queue.ToArray().CopyTo(array, index);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> to an <see cref="T:System.Array"/>, starting at a specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> . The array must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array"/> at which copying begins. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/>
        ///   is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/>
        ///   is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="index"/>
        ///   is equal to or greater than the length of the
        ///   <paramref name="array"/>
        ///   -or- The number of elements in the source
        ///   <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/>
        ///   is greater than the available space from
        ///   <paramref name="index"/>
        ///   to the end of the destination
        ///   <paramref name="array"/>
        ///   .
        /// </exception>
        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            this.queue.CopyTo(array, index);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection. </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return this.queue.GetEnumerator();
        }

        /// <summary>
        ///   Copies the elements contained in the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1" /> to a new array.
        /// </summary>
        /// <returns> A new array containing the elements copied from the 
        /// <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1" />. 
        /// </returns>
        public T[] ToArray()
        {
            return this.queue.ToArray();
        }

        /// <summary>
        /// Attempts to add an object to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if the object was added successfully; otherwise, false. 
        /// </returns>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/> . 
        /// </param>
        /// <exception cref="T:System.ArgumentException">
        /// The
        ///   <paramref name="item"/>
        ///   was invalid for this collection.
        /// </exception>
        public bool TryAdd(T item)
        {
            this.Add(item);
            return true;
        }

        /// <summary>
        /// Attempts to remove and return an object from the <see cref="T:System.Collections.Concurrent.IProducerConsumerCollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if an object was removed and returned successfully; otherwise, false. 
        /// </returns>
        /// <param name="item">
        /// When this method returns, if the object was removed and returned successfully, <paramref name="item"/> contains the removed object. If no object was available to be removed, the value is unspecified. 
        /// </param>
        public bool TryTake(out T item)
        {
            return this.queue.TryDequeue(out item);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///   The get enumerator.
        /// </summary>
        /// <returns> The System.Collections.IEnumerator. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}