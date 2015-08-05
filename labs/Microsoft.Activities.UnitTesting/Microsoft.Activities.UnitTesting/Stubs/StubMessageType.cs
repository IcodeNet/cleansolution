// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StubMessageType.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    /// <summary>
    /// The stub message type.
    /// </summary>
    public enum StubMessageType
    {
        /// <summary>
        ///   The received.
        /// </summary>
        Receive, 

        /// <summary>
        ///   The replied.
        /// </summary>
        ReceiveReply, 

        /// <summary>
        ///   The sent.
        /// </summary>
        Send,

        /// <summary>
        /// A sent reply
        /// </summary>
        SendReply,
    }
}