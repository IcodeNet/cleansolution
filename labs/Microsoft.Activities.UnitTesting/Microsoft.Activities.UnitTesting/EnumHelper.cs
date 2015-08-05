// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumHelper.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;

    /// <summary>
    ///   The enum helper.
    /// </summary>
    public static class EnumHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Creates a comma delimited list of all values for an enum type
        /// </summary>
        /// <typeparam name="TEnum"> The type of the enum </typeparam>
        /// <returns> a comma delimited list of all values for an enum type </returns>
        /// <exception cref="ArgumentException">The type is not an enum</exception>
        public static string ToDelimitedList<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }

            return ListHelper.ToDelimitedList(Enum.GetNames(typeof(TEnum)));
        }

        #endregion
    }
}