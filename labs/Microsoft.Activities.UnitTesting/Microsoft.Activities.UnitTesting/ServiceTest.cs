// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.ServiceModel;

    /// <summary>
    ///   Helper methods for testing services
    /// </summary>
    public static class ServiceTest
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="ServiceTest"/> class.
        /// </summary>
        static ServiceTest()
        {
            Pipe = new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.FromSeconds(5), SendTimeout = TimeSpan.FromSeconds(5) };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the binding for local testing
        /// </summary>
        public static NetNamedPipeBinding Pipe { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Gets a unique service endpoint
        /// </summary>
        /// <returns> The service endpoint </returns>
        public static EndpointAddress GetUniqueEndpointAddress()
        {
            return new EndpointAddress(GetUniqueUri());
        }

        /// <summary>
        ///   Gets a unique service endpoint
        /// </summary>
        /// <returns> The service endpoint </returns>
        public static Uri GetUniqueUri()
        {
            return new Uri("net.pipe://localhost/" + Guid.NewGuid());
        }

        #endregion
    }
}