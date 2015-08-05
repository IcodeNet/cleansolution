// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateMachineInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The StateMachineInfo interface.
    /// </summary>
    public interface IStateMachineInfo
    {
        #region Public Properties

        /// <summary>
        ///   Gets the last known state of the current state machine
        /// </summary>
        string CurrentState { get; }

        /// <summary>
        ///   Gets the instance ID of the state machine
        /// </summary>
        Guid InstanceId { get; }

        /// <summary>
        ///   Gets the last known instance state
        /// </summary>
        ActivityInstanceState InstanceState { get; }

        /// <summary>
        ///   Gets the name of the state machine
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   Gets the possible transitions
        /// </summary>
        ReadOnlyCollection<string> PossibleTransitions { get; }

        /// <summary>
        ///   Gets the previous state
        /// </summary>
        string PreviousState { get; }

        /// <summary>
        ///   Gets the state history
        /// </summary>
        ReadOnlyCollection<string> StateHistory { get; }

        #endregion
    }
}