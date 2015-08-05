// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationExceptionTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The initialization exception test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WFCollections.Activities.Tests
{
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The initialization exception test.
    /// </summary>
    [TestClass]
    public class InitializationExceptionTest
    {
        #region Public Methods

        /// <summary>
        /// The should handle initialization exception.
        /// </summary>
        [TestMethod]
        public void ShouldHandleInitializationException()
        {
            var activity = new UninitializedCollection();
            var host = new WorkflowInvokerTest(activity);
            try
            {
                host.TestActivity();
                host.AssertOutArgument.IsTrue("CaughtException");
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}