// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionActivityTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The collection activity tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities.Tests
{
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WFCollections.Console;

    /// <summary>
    /// The collection activity tests.
    /// </summary>
    [TestClass]
    public class CollectionActivityTests
    {
        #region Public Methods

        /// <summary>
        /// The verify collection activity.
        /// </summary>
        [TestMethod]
        public void VerifyCollectionActivity()
        {
            var activity = new CollectionActivities();
            var host = new WorkflowInvokerTest(activity);
            try
            {
                host.TestActivity();
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}