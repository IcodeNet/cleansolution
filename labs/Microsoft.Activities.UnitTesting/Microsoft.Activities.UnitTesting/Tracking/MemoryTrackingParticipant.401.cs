// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryTrackingParticipant.401.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET401_OR_GREATER

namespace Microsoft.Activities.UnitTesting.Tracking
{
    using System;
    using System.Activities.Statements.Tracking;
    using System.Collections.Generic;

    /// <summary>
    /// The memory tracking participant.
    /// </summary>
    public partial class MemoryTrackingParticipant
    {
        #region Constants and Fields

        /// <summary>
        ///   The state actions.
        /// </summary>
        private readonly Dictionary<string, List<Action<StateMachineStateRecord>>> stateActions =
            new Dictionary<string, List<Action<StateMachineStateRecord>>>();

        #endregion

        #region Public Methods

        /// <summary>
        /// The when state.
        /// </summary>
        /// <param name="stateName">
        /// The state name.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public void WhenState(string stateName, Action<StateMachineStateRecord> action)
        {
            lock (this.stateActions)
            {
                List<Action<StateMachineStateRecord>> actions;

                if (!this.stateActions.TryGetValue(stateName, out actions))
                {
                    actions = new List<Action<StateMachineStateRecord>>();
                    this.stateActions.Add(stateName, actions);
                }

                actions.Add(action);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on state machine state.
        /// </summary>
        /// <param name="smr">
        /// The smr.
        /// </param>
        private void OnStateMachineState(StateMachineStateRecord smr)
        {
            lock (this.stateActions)
            {
                List<Action<StateMachineStateRecord>> actions;

                if (this.stateActions.TryGetValue(smr.StateName, out actions))
                {
                    foreach (var action in actions)
                    {
                        action(smr);
                    }
                }
            }
        }

        #endregion
    }
}

#endif