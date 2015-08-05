// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputDictionary.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The output dictionary.
    /// </summary>
    public class OutputDictionary
    {
        #region Constants and Fields

        /// <summary>
        ///   The dictionary.
        /// </summary>
        private readonly IDictionary<string, object> dictionary;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public OutputDictionary(IDictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a value from the output dictionary
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument
        /// </typeparam>
        /// <returns>
        /// The dictionary value
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No value was found for name
        /// </exception>
        public static T Get<T>(IDictionary<string, object> dictionary, string name)
        {
            var od = new OutputDictionary(dictionary);
            return od.Get<T>(name);
        }

        /// <summary>
        /// Gets a value from the output dictionary
        /// </summary>
        /// <param name="name">
        /// The argument name.
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument
        /// </typeparam>
        /// <returns>
        /// The dictionary value
        /// </returns>
        /// <exception cref="ArgumentException">
        /// No value was found for name
        /// </exception>
        public T Get<T>(string name)
        {
            object value;

            if (this.dictionary.TryGetValue(name, out value))
            {
                return value is T ? (T)value : default(T);
            }

            throw new ArgumentException(SR.No_output_argument_was_found, "name");
        }

        #endregion
    }
}