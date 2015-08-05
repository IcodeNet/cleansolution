// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The enum helper.
    /// </summary>
    public static class ListHelper
    {
        #region Public Methods

        /// <summary>
        /// Creates a comma delimited list of enum values
        /// </summary>
        /// <param name="objects">
        /// A collection of object values
        /// </param>
        /// <typeparam name="T">
        /// The type of objects
        /// </typeparam>
        /// <returns>
        /// a comma delimited list of enum values
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static string ToDelimitedList<T>(IEnumerable<T> objects)
        {

            var result = new StringBuilder();

            foreach (var e in objects)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }

                result.AppendFormat("{0}", e);
            }

            return result.ToString();
        }

        #endregion
    }
}