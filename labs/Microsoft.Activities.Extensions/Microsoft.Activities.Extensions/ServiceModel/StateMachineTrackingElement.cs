// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineTrackingElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Configuration;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Configuration element for Workflow Extensions
    /// </summary>
    public class StateMachineTrackingElement : BehaviorExtensionElement
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the max history
        /// </summary>
        [ConfigurationProperty("MaxHistory", DefaultValue = StateMachineInfo.DefaultMaxHistory)]
        public int MaxHistory { get; set; }

        /// <summary>
        ///   Gets the type of behavior.
        /// </summary>
        /// <returns> A <see cref="T:System.Type" /> . </returns>
        public override Type BehaviorType
        {
            get
            {
                return typeof(StateMachineTrackingBehavior);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns> The behavior extension. </returns>
        protected override object CreateBehavior()
        {
            return new StateMachineTrackingBehavior(this.MaxHistory);
        }

        #endregion
    }
}