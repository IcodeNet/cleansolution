﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineTrackingBehavior.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System.Activities.DurableInstancing;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml.Linq;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   A Service Behavior which will add Workflow extensions
    /// </summary>
    public class StateMachineTrackingBehavior : IServiceBehavior
    {
        /// <summary>
        /// The max history
        /// </summary>
        private readonly int maxHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineTrackingBehavior"/> class
        /// </summary>
        /// <param name="maxHistory">The maximum number of state history entries</param>
        public StateMachineTrackingBehavior(int maxHistory = StateMachineInfo.DefaultMaxHistory)
        {
            this.maxHistory = maxHistory;
        }

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

            StateTracker.Attach(workflowServiceHost);
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