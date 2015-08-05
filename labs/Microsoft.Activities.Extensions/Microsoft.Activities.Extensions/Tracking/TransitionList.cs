// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionList.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The transition list.
    /// </summary>
    [CollectionDataContract(ItemName = "Transition", Namespace = Constants.Namespace)]
    public class TransitionList : List<string>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionList"/> class.
        /// </summary>
        public TransitionList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionList"/> class.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        public TransitionList(IEnumerable<string> enumerable)
            : base(enumerable)
        {
        }

        #endregion
    }
}