// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationElementCollection.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System.Configuration;

    /// <summary>
    /// Generic class for creating configuration elements
    /// </summary>
    /// <typeparam name="T">
    /// The type of the configuration element 
    /// </typeparam>
    public class ConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, IConfigurationElement, new()
    {
        #region Public Properties

        /// <summary>
        ///   Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <returns> The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection. </returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Indexer for accessing configuration elements
        /// </summary>
        /// <param name="index">
        /// The element to access 
        /// </param>
        /// <returns>
        /// The element 
        /// </returns>
        public T this[int index]
        {
            get
            {
                return (T)this.BaseGet(index);
            }

            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Indexer for accessing an element
        /// </summary>
        /// <param name="name">
        /// The element to access 
        /// </param>
        /// <returns>
        /// The T.
        /// </returns>
        public new T this[string name]
        {
            get
            {
                return (T)BaseGet(name);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an element
        /// </summary>
        /// <param name="element">
        /// The element to add 
        /// </param>
        public void Add(T element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        ///   Clears all the elements
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }

        /// <summary>
        /// Returns the index of an element
        /// </summary>
        /// <param name="element">
        /// The element 
        /// </param>
        /// <returns>
        /// The index 
        /// </returns>
        public int IndexOf(T element)
        {
            return this.BaseIndexOf(element);
        }

        /// <summary>
        /// Removes an element
        /// </summary>
        /// <param name="element">
        /// The element to remove 
        /// </param>
        public void Remove(T element)
        {
            if (this.BaseIndexOf(element) >= 0)
            {
                this.BaseRemove(element.GetKey());
            }
        }

        /// <summary>
        /// Removes an element
        /// </summary>
        /// <param name="name">
        /// The element to remove 
        /// </param>
        public void Remove(string name)
        {
            this.BaseRemove(name);
        }

        /// <summary>
        /// Removes an element at the index
        /// </summary>
        /// <param name="index">
        /// The index to remove at 
        /// </param>
        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="element">
        /// The <see cref="T:System.Configuration.ConfigurationElement"/> to add. 
        /// </param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            this.BaseAdd(element, false);
        }

        /// <summary>
        ///   When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns> A new <see cref="T:System.Configuration.ConfigurationElement" /> . </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/> . 
        /// </returns>
        /// <param name="element">
        /// The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. 
        /// </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IConfigurationElement)element).GetKey();
        }

        #endregion
    }
}