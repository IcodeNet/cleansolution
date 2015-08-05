// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowTaskTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using Microsoft.Activities.Extensions.Prototype;
    using Microsoft.Activities.Extensions.Prototype1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The workflow task tests.
    /// </summary>
    [TestClass]
    public class WorkflowTaskTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The test method 1.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            const string message = "Hello World";

            // Fire and forget
            WorkflowTask.Run(new WriteThread { Message = message });
        }

        #endregion
    }
}