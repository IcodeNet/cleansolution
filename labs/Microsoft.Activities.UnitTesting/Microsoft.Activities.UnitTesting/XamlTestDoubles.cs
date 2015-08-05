// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlTestDoubles.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The xaml test doubles.
    /// </summary>
    internal class XamlTestDoubles
    {
        #region Constants and Fields

        /// <summary>
        ///   The testdoubles.
        /// </summary>
        private readonly Dictionary<Type, List<XamlTestDouble>> testdoubles =
            new Dictionary<Type, List<XamlTestDouble>>();

        /// <summary>
        ///   The type index.
        /// </summary>
        private readonly Dictionary<Type, int> typeIndex = new Dictionary<Type, int>();

        #endregion

        #region Methods

        /// <summary>
        /// The add double.
        /// </summary>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <param name="replaceType">
        /// The replace type.
        /// </param>
        /// <param name="replaceAll">
        /// The replace all.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        internal void AddDouble(Type searchType, Type replaceType, bool replaceAll = true, int index = -1)
        {
            List<XamlTestDouble> doubleList;
            var testDouble = new XamlTestDouble { Index = index, ReplaceAll = replaceAll, ReplaceWith = replaceType };

            if (!this.testdoubles.TryGetValue(searchType, out doubleList))
            {
                doubleList = new List<XamlTestDouble>();
                this.testdoubles.Add(searchType, doubleList);
            }

            doubleList.Add(testDouble);
        }

        /// <summary>
        /// The try get double.
        /// </summary>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <param name="replaceType">
        /// The replace type.
        /// </param>
        /// <returns>
        /// true if a double was found
        /// </returns>
        internal bool TryGetDouble(Type searchType, out Type replaceType)
        {
            List<XamlTestDouble> testDouble;

            // If there is a test double defined for this type
            if (this.testdoubles.TryGetValue(searchType, out testDouble))
            {
                // Record the index of each type with a double that was found
                var occurence = 0;
                int index;
                if (!this.typeIndex.TryGetValue(searchType, out index))
                {
                    this.typeIndex.Add(searchType, occurence);
                }
                else
                {
                    this.typeIndex[searchType]++;
                    occurence = this.typeIndex[searchType];
                }

                var found = testDouble.Find(td => td.ReplaceAll || td.Index == occurence);

                replaceType = found != null ? found.ReplaceWith : null;

                // if (replaceType != null)
                // {
                // Debug.WriteLine(string.Format("XamlInjector replacing type {0} with type {1}", searchType, replaceType));
                // }
                return replaceType != null;
            }

            replaceType = null;
            return false;
        }

        #endregion
    }
}