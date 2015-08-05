// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SilverlightFaultBehavior.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The silverlight fault behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.ServiceModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// The silverlight fault behavior.
    /// </summary>
    public class SilverlightFaultBehavior : Attribute, IServiceBehavior
    {
        #region Implemented Interfaces

        #region IServiceBehavior

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        /// <param name="endpoints">
        /// The endpoints.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                endpoint.Behaviors.Add(new SilverlightFaultEndpointBehavior());
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion

        #endregion

        /// <summary>
        /// The silverlight fault endpoint behavior.
        /// </summary>
        private class SilverlightFaultEndpointBehavior : IEndpointBehavior
        {
            #region Implemented Interfaces

            #region IEndpointBehavior

            /// <summary>
            /// The add binding parameters.
            /// </summary>
            /// <param name="endpoint">
            /// The endpoint.
            /// </param>
            /// <param name="bindingParameters">
            /// The binding parameters.
            /// </param>
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            /// <summary>
            /// The apply client behavior.
            /// </summary>
            /// <param name="endpoint">
            /// The endpoint.
            /// </param>
            /// <param name="clientRuntime">
            /// The client runtime.
            /// </param>
            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            /// <summary>
            /// The apply dispatch behavior.
            /// </summary>
            /// <param name="endpoint">
            /// The endpoint.
            /// </param>
            /// <param name="endpointDispatcher">
            /// The endpoint dispatcher.
            /// </param>
            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SilverlightFaultMessageInspector());
            }

            /// <summary>
            /// The validate.
            /// </summary>
            /// <param name="endpoint">
            /// The endpoint.
            /// </param>
            public void Validate(ServiceEndpoint endpoint)
            {
            }

            #endregion

            #endregion

            /// <summary>
            /// The silverlight fault message inspector.
            /// </summary>
            private class SilverlightFaultMessageInspector : IDispatchMessageInspector
            {
                #region Implemented Interfaces

                #region IDispatchMessageInspector

                /// <summary>
                /// The after receive request.
                /// </summary>
                /// <param name="request">
                /// The request.
                /// </param>
                /// <param name="channel">
                /// The channel.
                /// </param>
                /// <param name="instanceContext">
                /// The instance context.
                /// </param>
                /// <returns>
                /// The object.
                /// </returns>
                public object AfterReceiveRequest(
                    ref Message request, IClientChannel channel, InstanceContext instanceContext)
                {
                    return null;
                }

                /// <summary>
                /// The before send reply.
                /// </summary>
                /// <param name="reply">
                /// The reply.
                /// </param>
                /// <param name="correlationState">
                /// The correlation state.
                /// </param>
                public void BeforeSendReply(ref Message reply, object correlationState)
                {
                    if ((reply != null) && reply.IsFault)
                    {
                        var property = new HttpResponseMessageProperty { StatusCode = HttpStatusCode.OK };
                        reply.Properties[HttpResponseMessageProperty.Name] = property;
                    }
                }

                #endregion

                #endregion
            }
        }
    }
}