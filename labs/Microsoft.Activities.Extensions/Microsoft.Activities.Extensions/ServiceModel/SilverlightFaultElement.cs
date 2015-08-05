// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SilverlightFaultElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The silverlight fault element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// The silverlight fault element.
    /// </summary>
    public class SilverlightFaultElement : BehaviorExtensionElement
    {
        #region Properties

        /// <summary>
        /// Gets BehaviorType.
        /// </summary>
        public override Type BehaviorType
        {
            get
            {
                return typeof(SilverlightFaultBehavior);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create behavior.
        /// </summary>
        /// <returns>
        /// The created behavior.
        /// </returns>
        protected override object CreateBehavior()
        {
            return new SilverlightFaultBehavior();
        }

        #endregion
    }
}