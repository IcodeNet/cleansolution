// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearDictionary.cs" company="Microsoft">
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
    /// The ClearDictionary will clear a dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    /// Type of the key 
    /// </typeparam>
    /// <typeparam name="TValue">
    /// Type of the value 
    /// </typeparam>
    public sealed class ClearDictionary<TKey, TValue> : CodeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets Dictionary.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IDictionary<TKey, TValue>> Dictionary { get; set; }

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
        protected override void Execute(CodeActivityContext context)
        {
            var dictionary = this.Dictionary.Get(context);

            if (dictionary == null)
            {
                throw new InvalidOperationException(SR.The_Dictionary_has_not_been_initialized);
            }

            dictionary.Clear();
        }

        #endregion
    }
}