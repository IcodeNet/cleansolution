// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowCanceledException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when the workflow is canceled
    /// </summary>
    public class WorkflowCanceledException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCanceledException"/> class.
        /// </summary>
        public WorkflowCanceledException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCanceledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public WorkflowCanceledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCanceledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public WorkflowCanceledException(
            string message, 
            Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCanceledException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public WorkflowCanceledException(
            SerializationInfo info, 
            StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}