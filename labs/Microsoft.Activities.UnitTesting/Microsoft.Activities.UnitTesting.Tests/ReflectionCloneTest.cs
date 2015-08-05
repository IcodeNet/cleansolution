// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionCloneTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The reflection clone test.
    /// </summary>
    [TestClass]
    public class ReflectionCloneTest
    {
        #region Public Methods

        /// <summary>
        /// Verify you can clone a string.
        /// </summary>
        [TestMethod]
        public void CloneArrayOfStringWorks()
        {
            var source = new[] { "Test1", "Test2" };

            var dest = ReflectionClone.DeepCopy(source);

            CollectionAssert.AreEquivalent(source, dest);
        }

        /// <summary>
        /// Verify you can clone a class.
        /// </summary>
        [TestMethod]
        public void CloneClassWorks()
        {
            var source = new Level1();
            for (int i = 0; i < 3; i++)
            {
                var l2 = new Level2();
                for (int j = 0; j < 5; j++)
                {
                    l2.Level3s.Add(new Level3 { Value = j });
                }

                source.Level2s.Add(l2);
            }

            var dest = ReflectionClone.DeepCopy(source);


            Assert.AreEqual(source.Name, dest.Name);
            Assert.AreEqual(source.Num, dest.Num);

            for (int i = 0; i < source.Level2s.Count; i++)
            {
                Assert.AreEqual(source.Level2s[i].SomeField, dest.Level2s[i].SomeField);
                for (int j = 0; j < source.Level2s[i].Level3s.Count; j++)
                {
                    Assert.AreEqual(source.Level2s[i].Level3s[j].Value, dest.Level2s[i].Level3s[j].Value);
                }
            }
        }

        /// <summary>
        /// Verify you can clone an int.
        /// </summary>
        [TestMethod]
        public void CloneIntValueTypeWorks()
        {
            const int Source = 123;

            var dest = ReflectionClone.DeepCopy(Source);

            Assert.AreEqual(Source, dest);
        }

        /// <summary>
        /// Verify you can clone a string.
        /// </summary>
        [TestMethod]
        public void CloneStringWorks()
        {
            const string Source = "Test String";

            var dest = ReflectionClone.DeepCopy(Source);

            Assert.AreEqual(Source, dest);
        }

        #endregion

        /// <summary>
        /// The level 1.
        /// </summary>
        private class Level1
        {
            #region Constants and Fields

            /// <summary>
            /// The level 2 s.
            /// </summary>
            private readonly IList<Level2> level2S = new List<Level2>();

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets Level2s.
            /// </summary>
            public IList<Level2> Level2s
            {
                get
                {
                    return this.level2S;
                }
            }

            /// <summary>
            /// Gets or sets Name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets Num.
            /// </summary>
            public int Num { get; set; }

            #endregion
        }

        /// <summary>
        /// The level 2.
        /// </summary>
        private class Level2
        {
            #region Constants and Fields

            /// <summary>
            /// The level 3 s.
            /// </summary>
            private readonly IList<Level3> level3S = new List<Level3>();

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets Level3s.
            /// </summary>
            public IList<Level3> Level3s
            {
                get
                {
                    return this.level3S;
                }
            }

            /// <summary>
            /// Gets or sets SomeField.
            /// </summary>
            public string SomeField { get; set; }

            #endregion
        }

        /// <summary>
        /// The level 3.
        /// </summary>
        private class Level3
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets Value.
            /// </summary>
            public int Value { get; set; }

            #endregion
        }
    }
}