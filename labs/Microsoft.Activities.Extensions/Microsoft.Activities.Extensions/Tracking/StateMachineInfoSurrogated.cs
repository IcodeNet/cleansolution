// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineInfoSurrogated.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Runtime.Serialization;

    /// <summary>
    /// The state machine info surrogated.
    /// </summary>
    [DataContract(Name = "StateMachine", Namespace = Constants.Namespace)]
    public class StateMachineInfoSurrogated
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineInfoSurrogated"/> class.
        /// </summary>
        /// <param name="stateMachineInfo">
        /// The state machine info.
        /// </param>
        public StateMachineInfoSurrogated(StateMachineInfo stateMachineInfo)
        {
            this.CurrentState = stateMachineInfo.CurrentState;
            this.InstanceId = stateMachineInfo.InstanceId;
            this.InstanceState = stateMachineInfo.InstanceState;
            this.Name = stateMachineInfo.Name;
            this.PossibleTransitions = new TransitionList(stateMachineInfo.PossibleTransitions);
            this.PreviousState = stateMachineInfo.PreviousState;
            this.StateHistory = new HistoryList(stateMachineInfo.StateHistory);
            this.MaxHistory = stateMachineInfo.MaxHistory;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets or sets the last known state of the current state machine
        /// </summary>
        [DataMember]
        public string CurrentState { get; set; }

        /// <summary>
        ///   Gets or sets the instance ID of the state machine
        /// </summary>
        [DataMember]
        public Guid InstanceId { get; set; }

        /// <summary>
        ///   Gets or sets the last known instance state
        /// </summary>
        [DataMember]
        public ActivityInstanceState InstanceState { get; set; }

        /// <summary>
        ///   Gets or sets the maximum number of history entries
        /// </summary>
        [DataMember]
        public int MaxHistory { get; set; }

        /// <summary>
        ///   Gets or sets the name of the state machine
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///   Gets or sets the possible transitions
        /// </summary>
        [DataMember]
        public TransitionList PossibleTransitions { get; set; }

        /// <summary>
        ///   Gets or sets the previous state
        /// </summary>
        [DataMember]
        public string PreviousState { get; set; }

        /// <summary>
        ///   Gets or sets the state history
        /// </summary>
        [DataMember]
        public HistoryList StateHistory { get; set; }

        #endregion
    }
}