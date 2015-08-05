// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTestTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// </summary>
    [TestClass]
    public class ServiceTestTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * GetUniqueUri is invoked 3 times
        ///   Then
        ///   * All three Uris are different
        /// </summary>
        [TestMethod]
        public void GetUniqueUriReturnsUniqueUri()
        {
            // Arrange
            var duplicate = false;

            var uris = new List<Uri>();

            // Act
            for (var i = 0; i < 3; i++)
            {
                var uri = ServiceTest.GetUniqueUri();
                if (!duplicate)
                {
                    duplicate = uris.Contains(uri);
                }

                uris.Add(uri);
            }

            // Assert
            Assert.IsFalse(duplicate);
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * GetUniqueEndpointAddress is invoked 3 times
        ///   Then
        ///   * All three EndpointAddresss are different
        /// </summary>
        [TestMethod]
        public void GetUniqueEndpointAddressReturnsUniqueEndpointAddress()
        {
            // Arrange
            var duplicate = false;

            var uris = new List<EndpointAddress>();

            // Act
            for (var i = 0; i < 3; i++)
            {
                var uri = ServiceTest.GetUniqueEndpointAddress();
                if (!duplicate)
                {
                    duplicate = uris.Contains(uri);
                }

                uris.Add(uri);
            }

            // Assert
            Assert.IsFalse(duplicate);
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * ServiceTest.Pipe is invoked
        ///   Then
        ///   * It returns a NetNamedPipeBinding
        /// </summary>
        [TestMethod]
        public void ServiceTestReturnsPipeBinding()
        {
            // Arrange

            // Act
            var actual = ServiceTest.Pipe;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(NetNamedPipeBinding));
        }

        #endregion
    }
}