// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowCompletedEpisodeResult.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Collections.Generic;

    /// <summary>
    ///   The workflow episode result.
    /// </summary>
    public class WorkflowCompletedEpisodeResult : WorkflowEpisodeResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowCompletedEpisodeResult"/> class.
        /// </summary>
        /// <param name="args">
        /// The event args. 
        /// </param>
        /// <param name="instanceId">
        /// The instance Id. 
        /// </param>
        public WorkflowCompletedEpisodeResult(WorkflowApplicationCompletedEventArgs args, Guid instanceId)
        {
            this.State = args.CompletionState;
            this.InstanceId = instanceId;

            switch (this.State)
            {
                case ActivityInstanceState.Closed:
                    this.Outputs = args.Outputs;
                    break;
                case ActivityInstanceState.Faulted:
                    this.TerminationException = args.TerminationException;
                    break;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Outputs.
        /// </summary>
        public IDictionary<string, object> Outputs { get; internal set; }

        /// <summary>
        ///   Gets TerminationException.
        /// </summary>
        public Exception TerminationException { get; internal set; }

        #endregion
    }
}