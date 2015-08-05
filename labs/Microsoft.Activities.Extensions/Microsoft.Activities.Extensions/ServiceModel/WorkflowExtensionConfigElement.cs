// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExtensionConfigElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System.Configuration;
    using System.Text;

    /// <summary>
    ///   Configuration element to allow WorkflowExtensions to be added
    /// </summary>
    public class WorkflowExtensionConfigElement : ConfigurationElement, IConfigurationElement
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowExtensionConfigElement"/> class.
        /// </summary>
        /// <param name="key">
        /// The key. 
        /// </param>
        public WorkflowExtensionConfigElement(string key)
        {
            this.ExtensionType = key;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="WorkflowExtensionConfigElement" /> class.
        /// </summary>
        public WorkflowExtensionConfigElement()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the fully qualified type name for this extension
        /// </summary>
        [ConfigurationProperty("extension", DefaultValue = null, IsRequired = true, IsKey = true)]
        public string ExtensionType
        {
            get
            {
                return (string)this["extension"];
            }

            set
            {
                this["extension"] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the key for this config element
        /// </summary>
        /// <returns>
        /// The System.Object.
        /// </returns>
        public object GetKey()
        {
            return this.ExtensionType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get assembly.
        /// </summary>
        /// <returns>
        /// The System.String.
        /// </returns>
        internal string GetAssembly()
        {
            var parts = this.ExtensionType.Split(',');
            var sb = new StringBuilder();

            // Skip the first part - it is the type name
            for (var i = 1; i < parts.Length; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                sb.Append(parts[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        ///   Returns the name of the type from config
        /// </summary>
        /// <returns> The name of the type </returns>
        internal string GetTypeName()
        {
            var parts = this.ExtensionType.Split(',');
            return parts[0];
        }

        #endregion
    }
}