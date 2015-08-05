// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendReplyStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System.Activities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ServiceModel.Activities;
    using System.Windows.Markup;

    /// <summary>
    /// The send reply stub.
    /// </summary>
    [ContentProperty("Content")]
    public class SendReplyStub : SendStubBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The correlation initializers.
        /// </summary>
        private readonly Collection<CorrelationInitializer> correlationInitializers = new Collection<CorrelationInitializer>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Action.
        /// </summary>
        [DefaultValue(null)]
        public string Action { get; set; }

        /// <summary>
        ///   Gets CorrelationInitializers.
        /// </summary>
        [DefaultValue(null)]
        public Collection<CorrelationInitializer> CorrelationInitializers
        {
            get
            {
                return this.correlationInitializers;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether PersistBeforeSend.
        /// </summary>
        [DefaultValue(false)]
        public bool PersistBeforeSend { get; set; }

        /// <summary>
        ///   Gets or sets Request.
        /// </summary>
        [DefaultValue(null)]
        public Activity Request { get; set; }

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
            var receive = (ReceiveStub)this.Request;
            stub.Send(
                new StubMessage(StubMessageType.SendReply) { Content = messageContent, Contract = receive.ServiceContractName.ToString(), Operation = receive.OperationName });
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
            var receive = (ReceiveStub)this.Request;
            stub.Send(
                new StubMessage(StubMessageType.SendReply) { Content = parametersContent, Contract = receive.ServiceContractName.ToString(), Operation = receive.OperationName });
        }

        #endregion
    }
}