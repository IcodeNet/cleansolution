// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessagingStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Implement this interface to provide an implementation for a receive stub
    /// </summary>
    public interface IMessagingStub : IWorkflowInstanceExtension
    {
        #region Public Properties

        /// <summary>
        ///   Gets the received messages.
        /// </summary>
        /// <returns>
        ///   Returns messages received
        /// </returns>
        IList<StubMessage> Messages { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueue a message to be received
        /// </summary>
        /// <param name="contract">
        /// The contract.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        void EnqueueReceive(XName contract, string operation, object messageContent);

        /// <summary>
        /// Enqueue a message to be received
        /// </summary>
        /// <param name="contract">
        /// The contract.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        void EnqueueReceive(XName contract, string operation, IDictionary<string, object> parametersContent);

        /// <summary>
        /// Enqueue a reply to a send
        /// </summary>
        /// <param name="fromContract">
        /// The from contract.
        /// </param>
        /// <param name="fromOperation">
        /// The from operation.
        /// </param>
        /// <param name="messageContent">
        /// The message content.
        /// </param>
        void EnqueueReceiveReply(XName fromContract, string fromOperation, object messageContent);

        /// <summary>
        /// Enqueue a reply to a send
        /// </summary>
        /// <param name="fromContract">
        /// The from contract.
        /// </param>
        /// <param name="fromOperation">
        /// The from operation.
        /// </param>
        /// <param name="parametersContent">
        /// The parameters content.
        /// </param>
        void EnqueueReceiveReply(XName fromContract, string fromOperation, IDictionary<string, object> parametersContent);

        /// <summary>
        /// The get implementation.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// An action to be invoked when the stubbed activity is run
        /// </returns>
        Action GetImplementation(Activity activity);

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="stubMessage">
        /// The stub message.
        /// </param>
        void Send(StubMessage stubMessage);

        #endregion
    }
}