// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    /// <summary>
    ///   Interface to provide key semantics for the configuration element
    /// </summary>
    public interface IConfigurationElement
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Gets the key for the element
        /// </summary>
        /// <returns> A key </returns>
        object GetKey();

        #endregion
    }
}