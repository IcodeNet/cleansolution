// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryList.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The history list.
    /// </summary>
    [CollectionDataContract(Name = "StateHistory", ItemName = "State", Namespace = Constants.Namespace)]
    public class HistoryList : List<string>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryList"/> class.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        public HistoryList(IEnumerable<string> enumerable)
            : base(enumerable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryList"/> class.
        /// </summary>
        public HistoryList()
        {
        }

        #endregion
    }
}