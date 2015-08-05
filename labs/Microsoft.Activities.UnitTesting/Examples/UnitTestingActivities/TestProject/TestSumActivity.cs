// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSumActivity.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TestProject
{
    using System.Activities;
    using System.Collections.Generic;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using UnitTestingActivities;

    /// <summary>
    /// Tests the sum activities
    /// </summary>
    [TestClass]
    public class TestSumActivity
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The should add bad sum arg name.
        /// </summary>
        [TestMethod]
        public void ShouldAddBadSumArgName()
        {
            var output = WorkflowInvoker.Invoke(new BadSumArgName { x = 1, y = 2 });

            AssertHelper.Throws<KeyNotFoundException>(
                () => Assert.AreEqual(3, output["sum"]), "The given key was not present in the dictionary.");
        }

        /// <summary>
        /// The should add bad sum arg name assert out argument.
        /// </summary>
        [TestMethod]
        public void ShouldAddBadSumArgNameAssertOutArgument()
        {
            var sut = new WorkflowInvokerTest(new BadSumArgName { x = 1, y = 2 });
            sut.TestActivity();

            // How to handle an expected exception
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => sut.AssertOutArgument.AreEqual("sum", 3), 
                "AssertOutArgument.AreEqual failed. Output does not contain an argument named <sum>.");
        }

        /// <summary>
        /// The should add bad sum wrong type.
        /// </summary>
        [TestMethod]
        public void ShouldAddBadSumWrongType()
        {
            var output = WorkflowInvoker.Invoke(new BadSumWrongType { x = 1, y = 2 });

            AssertHelper.Throws<WorkflowAssertFailedException>(() => AssertOutArgument.AreEqual(output, "sum", 3));
        }

        /// <summary>
        /// The should add bad sum wrong type assert out argument.
        /// </summary>
        [TestMethod]
        public void ShouldAddBadSumWrongTypeAssertOutArgument()
        {
            var sut = new WorkflowInvokerTest(new BadSumWrongType { x = 1, y = 2 });
            sut.TestActivity();

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => sut.AssertOutArgument.AreEqual("sum", 3), 
                "AssertOutArgument.AreEqual failed. Wrong type for OutArgument <sum>. Expected Type: <System.Int32>. Actual Type: <System.String>.");
        }

        /// <summary>
        /// The should add good sum.
        /// </summary>
        [TestMethod]
        public void ShouldAddGoodSum()
        {
            var output = WorkflowInvoker.Invoke(new GoodSum { x = 1, y = 2 });
            Assert.AreEqual(3, output["sum"]);
        }

        /// <summary>
        /// The should add good sum assert out argument.
        /// </summary>
        [TestMethod]
        public void ShouldAddGoodSumAssertOutArgument()
        {
            var sut = new WorkflowInvokerTest(new GoodSum { x = 1, y = 2 });
            sut.TestActivity();

            sut.AssertOutArgument.AreEqual("sum", 3);
        }

        #endregion
    }
}