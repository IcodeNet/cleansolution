// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumHelperTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for EnumHelperTest and is intended
    ///   to contain all EnumHelperTest Unit Tests
    /// </summary>
    [TestClass]
    public class EnumHelperTest
    {
        #region Enums

        /// <summary>
        /// The empty enum.
        /// </summary>
        private enum EmptyEnum
        {
        }

        /// <summary>
        /// The five enum.
        /// </summary>
        private enum FiveEnum
        {
            /// <summary>
            /// The one.
            /// </summary>
            One, 

            /// <summary>
            /// The two.
            /// </summary>
            Two, 

            /// <summary>
            /// The three.
            /// </summary>
            Three, 

            /// <summary>
            /// The four.
            /// </summary>
            Four, 

            /// <summary>
            /// The five.
            /// </summary>
            Five
        }

        /// <summary>
        /// The one enum.
        /// </summary>
        private enum OneEnum
        {
            /// <summary>
            ///   The one.
            /// </summary>
            One, 
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies that an empty enum returns an empty string
        /// </summary>
        [TestMethod]
        public void EmptyEnumShouldReturnEmptyString()
        {
            Assert.AreEqual(String.Empty, EnumHelper.ToDelimitedList<EmptyEnum>());
        }

        /// <summary>
        /// Verifies that an enum with 1 member returns only that member
        /// </summary>
        [TestMethod]
        public void OneEnumShouldReturnStringWithOneMember()
        {
            Assert.AreEqual("One", EnumHelper.ToDelimitedList<OneEnum>());
        }

        /// <summary>
        /// Verifies that an enum with five members returns a comma delimited string with five members
        /// </summary>
        [TestMethod]
        public void EnumShouldReturnDelimitedStringWithAllMembers()
        {
            Assert.AreEqual("One, Two, Three, Four, Five", EnumHelper.ToDelimitedList<FiveEnum>());
        }

        /// <summary>
        /// Verifies that an IEnumerable(of TEnum) returns a comma delimited string with the members
        /// </summary>
        [TestMethod]
        public void EnumShouldReturnDelimitedStringWithSomeMembers()
        {
            List<FiveEnum> enums = new List<FiveEnum>() { FiveEnum.One, FiveEnum.Three, FiveEnum.Five };

            // Invoke using extension method
            Assert.AreEqual("One, Three, Five", ListHelper.ToDelimitedList(enums));
        }

        #endregion
    }
}