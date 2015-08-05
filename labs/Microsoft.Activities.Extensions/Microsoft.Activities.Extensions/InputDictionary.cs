// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputDictionary.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   The input dictionary.
    /// </summary>
    public static class InputDictionary
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a dictionary of input arguments
        /// </summary>
        /// <param name="values">
        /// The values. 
        /// </param>
        /// <returns>
        /// A dicationary of input arguments 
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Values must contain an even number of key value pairs
        /// </exception>
        public static IDictionary<string, object> Create(params object[] values)
        {
            var input = new Dictionary<string, object>();
            if (values.Length % 2 != 0)
            {
                throw new ArgumentException("Values must contain an even number of key / value pairs");
            }

            for (var i = 0; i < values.Length; i = i + 2)
            {
                if (values[i] is string)
                {
                    input.Add(values[i].ToString(), values[i + 1]);
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("Keys must be of type string, values[{0}] is type {1}", i, values[i].GetType()));
                }
            }

            return input;
        }

        #endregion
    }
}