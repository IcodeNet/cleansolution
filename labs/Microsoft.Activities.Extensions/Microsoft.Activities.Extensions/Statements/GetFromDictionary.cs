// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFromDictionary.cs" company="Microsoft">
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
    /// The GetFromDictionary activity will add a key value pair to a dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    /// Type of the key
    /// </typeparam>
    /// <typeparam name="TValue">
    /// Type of the value
    /// </typeparam>
    public sealed class GetFromDictionary<TKey, TValue> : CodeActivity<bool>
    {
        #region Properties

        /// <summary>
        /// Gets or sets Dictionary.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IDictionary<TKey, TValue>> Dictionary { get; set; }

        /// <summary>
        /// Gets or sets Key.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<TKey> Key { get; set; }

        /// <summary>
        /// Gets or sets Value.
        /// </summary>
        public OutArgument<TValue> Value { get; set; }

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
        /// True if the value was found, false if not
        /// </returns>
        protected override bool Execute(CodeActivityContext context)
        {
            var dictionary = this.Dictionary.Get(context);

            if (dictionary == null)
            {
                throw new InvalidOperationException(SR.The_Dictionary_has_not_been_initialized);
            }

            var key = this.Key.Get(context);
            TValue value;

            var result = dictionary.TryGetValue(key, out value);

            // Even if the value is not found we set the out argument
            // to be consistent with the TryGetValue api
            this.Value.Set(context, value);

            return result;
        }

        #endregion
    }
}