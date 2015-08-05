// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowAssertFailedException.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The workflow assert failed exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The workflow assert failed exception.
    /// </summary>
    [Serializable]
    public sealed class WorkflowAssertFailedException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAssertFailedException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public WorkflowAssertFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAssertFailedException"/> class.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="args">parameters to be inserted into the message</param>
        public WorkflowAssertFailedException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAssertFailedException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public WorkflowAssertFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAssertFailedException"/> class.
        /// </summary>
        public WorkflowAssertFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowAssertFailedException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private WorkflowAssertFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}