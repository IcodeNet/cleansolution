// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExtensionsBehavior.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    /// <summary>
    ///   A Service Behavior which will add Workflow extensions
    /// </summary>
    public class WorkflowExtensionsBehavior : IServiceBehavior
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowExtensionsBehavior"/> class. 
        ///   Initializes the WorkflowExtensionsBehavior
        /// </summary>
        /// <param name="extensionConfigElements">
        /// The extension config elements 
        /// </param>
        public WorkflowExtensionsBehavior(
            ConfigurationElementCollection<WorkflowExtensionConfigElement> extensionConfigElements)
        {
            this.ExtensionConfigElements = extensionConfigElements;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the extension configuration elements
        /// </summary>
        protected ConfigurationElementCollection<WorkflowExtensionConfigElement> ExtensionConfigElements { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description of the service. 
        /// </param>
        /// <param name="serviceHostBase">
        /// The host of the service. 
        /// </param>
        /// <param name="endpoints">
        /// The service endpoints. 
        /// </param>
        /// <param name="bindingParameters">
        /// Custom objects to which binding elements have access. 
        /// </param>
        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description. 
        /// </param>
        /// <param name="serviceHostBase">
        /// The host that is currently being built. 
        /// </param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var workflowServiceHost = serviceHostBase as WorkflowServiceHost;
            if (workflowServiceHost == null)
            {
                return;
            }

            foreach (
                var extension in
                    from WorkflowExtensionConfigElement extensionConfigElement in this.ExtensionConfigElements
                    select
                        AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
                            extensionConfigElement.GetAssembly(), extensionConfigElement.GetTypeName()))
            {
                workflowServiceHost.WorkflowExtensions.Add(extension);
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description. 
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host that is currently being constructed. 
        /// </param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}