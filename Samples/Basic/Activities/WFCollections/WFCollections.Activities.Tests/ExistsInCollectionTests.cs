// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExistsInCollectionTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The exists in collection tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities.Tests
{
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The exists in collection tests.
    /// </summary>
    [TestClass]
    public class ExistsInCollectionTests
    {
        #region Public Methods

        /// <summary>
        /// Given 
        ///   * A list of numbers with 1 and 3
        ///   When
        ///   * The ExistsInCollection is executed with item=2
        ///   Then
        ///   * It should return false
        /// </summary>
        [TestMethod]
        public void ShouldNotFind2InList()
        {
            var activity = new CheckNumberExistsInCollection { CheckNumber = 2 };
            var host = new WorkflowInvokerTest(activity);
            try
            {
                host.TestActivity();
                host.AssertOutArgument.IsFalse("NumberExists");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}