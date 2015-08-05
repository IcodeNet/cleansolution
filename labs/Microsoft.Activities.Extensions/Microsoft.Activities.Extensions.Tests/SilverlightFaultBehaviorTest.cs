// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SilverlightFaultBehaviorTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.ServiceModel;

    using Microsoft.Activities.Extensions.Tests.ServiceReferences;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This is a test class for SilverlightFaultBehaviorTest and is intended to contain all SilverlightFaultBehaviorTest Unit Tests
    /// </summary>
    [TestClass]
    public class SilverlightFaultBehaviorTest
    {
        #region Fields

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A config file that adds the SilverlightFaultBehavior
        ///   When
        ///   * The service is hosted
        ///   Then
        ///   * The fault behavior is added
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.DefaultServiceXamlxPath)]
        public void BehaviorCanBeAddedViaConfig()
        {
            // Arrange
            var serviceEndpoint = ServiceTest.GetUniqueEndpointAddress();
            using (WorkflowServiceTestHost.Open(Constants.DefaultServiceXamlx, serviceEndpoint))
            {
                var proxy = new ServiceClient(ServiceTest.Pipe, serviceEndpoint);
                try
                {
                    // Act
                    var result = proxy.GetData(123);
                    proxy.Close();

                    // Assert
                    Assert.AreEqual("123", result);
                }
                catch 
                {
                    proxy.Abort();
                    throw;
                }
            }
        }

        #endregion
    }
}