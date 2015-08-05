// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyExistsInDictionary.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The KeyExistsInDictionary activity determines if a key exists in a dictionary
    /// </summary>
    /// <typeparam name="TKey">
    /// Type of the key 
    /// </typeparam>
    /// <typeparam name="TValue">
    /// Type of the value 
    /// </typeparam>
    public sealed class KeyExistsInDictionary<TKey, TValue> : CodeActivity<bool>
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Dictionary.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IDictionary<TKey, TValue>> Dictionary { get; set; }

        /// <summary>
        ///   Gets or sets Key.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<TKey> Key { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Dictionary is null
        /// </exception>
        /// <returns>
        /// True if the key exists in the dictionary 
        /// </returns>
        protected override bool Execute(CodeActivityContext context)
        {
            var dictionary = this.Dictionary.Get(context);

            if (dictionary == null)
            {
                throw new InvalidOperationException(SR.The_Dictionary_has_not_been_initialized);
            }

            var key = this.Key.Get(context);

            return dictionary.ContainsKey(key);
        }

        #endregion
    }
}