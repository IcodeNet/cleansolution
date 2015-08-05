// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryStoreElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Persistence
{
    using System;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// The memory store element.
    /// </summary>
    public class MemoryStoreElement : BehaviorExtensionElement
    {
        #region Properties

        /// <summary>
        /// Gets BehaviorType.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(MemoryStoreBehavior);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create behavior.
        /// </summary>
        /// <returns>
        /// The create behavior.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new MemoryStoreBehavior();
        }

        #endregion
    }
}