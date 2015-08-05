// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowArguments.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Dynamic object support for declaring input arguments for a workflow
    /// </summary>
    [Serializable]
    public class WorkflowArguments : DynamicObject, IDictionary<string, object>, ISerializable
    {
        #region Fields

        /// <summary>
        ///   The arguments.
        /// </summary>
        private readonly IDictionary<string, object> arguments = new Dictionary<string, object>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="WorkflowArguments" /> class. 
        ///   Creates an instance of WorkflowArguments
        /// </summary>
        public WorkflowArguments()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowArguments"/> class. 
        ///   Creates an instance of WorkflowArguments
        /// </summary>
        /// <param name="args">
        /// A dictionary to pre-populate the arguments with 
        /// </param>
        public WorkflowArguments(IDictionary<string, object> args)
        {
            this.arguments = args;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowArguments"/> class.
        /// </summary>
        /// <param name="info">
        /// The info. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        protected WorkflowArguments(SerializationInfo info, StreamingContext context)
        {
            foreach (var entry in info)
            {
                this.arguments.Add(entry.Name, entry.Value);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the number of elements contained in the arguments collection />.
        /// </summary>
        /// <returns> The number of elements contained in the arguments collection /> . </returns>
        public int Count
        {
            get
            {
                return this.arguments.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the arguments collection is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns> true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false. </returns>
        public bool IsReadOnly
        {
            get
            {
                return this.arguments.IsReadOnly;
            }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" /> . </returns>
        public ICollection<string> Keys
        {
            get
            {
                return this.arguments.Keys;
            }
        }

        /// <summary>
        ///   Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see
        ///    cref="T:System.Collections.Generic.IDictionary`2" /> . </returns>
        public ICollection<object> Values
        {
            get
            {
                return this.arguments.Values;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key. 
        /// </returns>
        /// <param name="key">
        /// The key of the element to get or set. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/>
        ///   is null.
        /// </exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        /// The property is retrieved and
        ///   <paramref name="key"/>
        ///   is not found.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The property is set and the
        ///   <see cref="T:System.Collections.Generic.IDictionary`2"/>
        ///   is read-only.
        /// </exception>
        public object this[string key]
        {
            get
            {
                return this.arguments[key];
            }

            set
            {
                this.arguments[key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates WorkflowArguments from a Dictionary
        /// </summary>
        /// <param name="args">
        /// The dictionary to create from 
        /// </param>
        /// <returns>
        /// an initialized WorkflowArguments instance 
        /// </returns>
        public static dynamic FromDictionary(IDictionary<string, object> args)
        {
            return new WorkflowArguments(args);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/> . 
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// The
        ///   <see cref="T:System.Collections.Generic.ICollection`1"/>
        ///   is read-only.
        /// </exception>
        public void Add(KeyValuePair<string, object> item)
        {
            this.arguments.Add(item);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add. 
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/>
        ///   is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the
        ///   <see cref="T:System.Collections.Generic.IDictionary`2"/>
        ///   .
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The
        ///   <see cref="T:System.Collections.Generic.IDictionary`2"/>
        ///   is read-only.
        /// </exception>
        public void Add(string key, object value)
        {
            this.arguments.Add(key, value);
        }

        /// <summary>
        ///   Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The
        ///   <see cref="T:System.Collections.Generic.ICollection`1" />
        ///   is read-only.</exception>
        public void Clear()
        {
            this.arguments.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/> ; otherwise, false. 
        /// </returns>
        /// <param name="item">
        /// The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/> . 
        /// </param>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.arguments.Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false. 
        /// </returns>
        /// <param name="key">
        /// The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/> . 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/>
        ///   is null.
        /// </exception>
        public bool ContainsKey(string key)
        {
            return this.arguments.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/> . The <see cref="T:System.Array"/> must have zero-based indexing. 
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/>
        ///   is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/>
        ///   is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/>
        ///   is multidimensional.-or-The number of elements in the source
        ///   <see cref="T:System.Collections.Generic.ICollection`1"/>
        ///   is greater than the available space from
        ///   <paramref name="arrayIndex"/>
        ///   to the end of the destination
        ///   <paramref name="array"/>
        ///   .-or-Type cannot be cast automatically to the type of the destination
        ///   <paramref name="array"/>
        ///   .
        /// </exception>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this.arguments.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns> A sequence that contains dynamic member names. </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.arguments.Keys;
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection. </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.arguments.GetEnumerator();
        }

        /// <summary>
        /// The get object data.
        /// </summary>
        /// <param name="info">
        /// The info. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var kvp in this.arguments)
            {
                info.AddValue(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/> ; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/> . 
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/> . 
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// The
        ///   <see cref="T:System.Collections.Generic.ICollection`1"/>
        ///   is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.arguments.Remove(item);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/> . 
        /// </returns>
        /// <param name="key">
        /// The key of the element to remove. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/>
        ///   is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The
        ///   <see cref="T:System.Collections.Generic.IDictionary`2"/>
        ///   is read-only.
        /// </exception>
        public bool Remove(string key)
        {
            return this.arguments.Remove(key);
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.) 
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive. 
        /// </param>
        /// <param name="result">
        /// The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/> . 
        /// </param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.arguments.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false. 
        /// </returns>
        /// <param name="key">
        /// The key whose value to get. 
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized. 
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/>
        ///   is null.
        /// </exception>
        public bool TryGetValue(string key, out object value)
        {
            return this.arguments.TryGetValue(key, out value);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.) 
        /// </returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive. 
        /// </param>
        /// <param name="value">
        /// The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test". 
        /// </param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.arguments[binder.Name] = value;

            return true;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns> An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection. </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.arguments.GetEnumerator();
        }

        #endregion
    }
}