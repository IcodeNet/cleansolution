// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Security;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.XamlIntegration;
    using System.Windows.Markup;
    using System.Xml.Linq;

    /// <summary>
    /// A stubbed send activity
    /// </summary>
    [ContentProperty("Content")]
    public sealed class SendStub : SendStubBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The correlation initializers.
        /// </summary>
        private readonly Collection<CorrelationInitializer> correlationInitializers = new Collection<CorrelationInitializer>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SendStub" /> class.
        /// </summary>
        public SendStub()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendStub"/> class.
        /// </summary>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        public SendStub(Func<Activity> implementation)
        {
            this.Implementation = implementation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Action.
        /// </summary>
        [DefaultValue(null)]
        public string Action { get; set; }

        /// <summary>
        ///   Gets or sets CorrelatesWith.
        /// </summary>
        [DefaultValue(null)]
        public InArgument<CorrelationHandle> CorrelatesWith { get; set; }

        /// <summary>
        ///   Gets CorrelationInitializers.
        /// </summary>
        public Collection<CorrelationInitializer> CorrelationInitializers
        {
            get
            {
                return this.correlationInitializers;
            }
        }

        /// <summary>
        ///   Gets or sets Endpoint.
        /// </summary>
        [DefaultValue(null)]
        public Endpoint Endpoint { get; set; }

        /// <summary>
        ///   Gets or sets EndpointAddress.
        /// </summary>
        [DefaultValue(null)]
        public InArgument<Uri> EndpointAddress { get; set; }

        /// <summary>
        ///   Gets or sets EndpointConfigurationName.
        /// </summary>
        [DefaultValue(null)]
        public string EndpointConfigurationName { get; set; }

        /// <summary>
        ///   Gets or sets KnownTypes.
        /// </summary>
        public Collection<Type> KnownTypes { get; set; }

        /// <summary>
        ///   Gets or sets OperationName.
        /// </summary>
        [DefaultValue(null)]
        public string OperationName { get; set; }

        /// <summary>
        ///   Gets or sets ProtectionLevel.
        /// </summary>
        [DefaultValue(null)]
        public ProtectionLevel? ProtectionLevel { get; set; }

        /// <summary>
        ///   Gets or sets SerializerOption.
        /// </summary>
        [DefaultValue(0)]
        public SerializerOption SerializerOption { get; set; }

        /// <summary>
        ///   Gets or sets ServiceContractName.
        /// </summary>
        [DefaultValue(null)]
        [TypeConverter(typeof(ServiceXNameTypeConverter))]
        public XName ServiceContractName { get; set; }

        /// <summary>
        ///   Gets or sets TokenImpersonationLevel.
        /// </summary>
        [DefaultValue(2)]
        public TokenImpersonationLevel TokenImpersonationLevel { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stub">
        /// The stub.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        protected override void Send(IMessagingStub stub, object messageContent)
        {
            stub.Send(new StubMessage(StubMessageType.Send) { Content = messageContent, Contract = this.ServiceContractName.ToString(), Operation = this.OperationName });
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stub">
        /// The stub.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        protected override void Send(IMessagingStub stub, Dictionary<string, object> parametersContent)
        {
            stub.Send(new StubMessage(StubMessageType.Send) { Content = parametersContent, Contract = this.ServiceContractName.ToString(), Operation = this.OperationName });
        }

        #endregion
    }
}