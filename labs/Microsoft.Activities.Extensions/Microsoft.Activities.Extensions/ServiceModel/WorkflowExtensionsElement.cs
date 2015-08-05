// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExtensionsElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Configuration;

    /// <summary>
    ///   Configuration element for Workflow Extensions
    /// </summary>
    public class WorkflowExtensionsElement : BehaviorExtensionElement
    {
        #region Public Properties

        /// <summary>
        ///   Gets the type of behavior.
        /// </summary>
        /// <returns> A <see cref="T:System.Type" /> . </returns>
        public override Type BehaviorType
        {
            get
            {
                return typeof(WorkflowExtensionsBehavior);
            }
        }

        /// <summary>
        ///   Gets The extensions collection
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<WorkflowExtensionConfigElement>), 
            AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ConfigurationElementCollection<WorkflowExtensionConfigElement> Extensions
        {
            get
            {
                var elementCollection =
                    (ConfigurationElementCollection<WorkflowExtensionConfigElement>)base[string.Empty];
                return elementCollection;
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
            return new WorkflowExtensionsBehavior(this.Extensions);
        }

        #endregion
    }
}