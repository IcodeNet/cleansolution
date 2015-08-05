// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTrackerSurrogated.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The state tracker surrogated.
    /// </summary>
    [DataContract(Name = "StateTracker", Namespace = Constants.Namespace)]
    public class StateTrackerSurrogated
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateTrackerSurrogated"/> class.
        /// </summary>
        /// <param name="stateTracker">
        /// The state tracker.
        /// </param>
        public StateTrackerSurrogated(StateTracker stateTracker)
        {
            this.StateMachines = new StateMachineList(stateTracker.StateMachines);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the state machines.
        /// </summary>
        [DataMember]
        public StateMachineList StateMachines { get; set; }

        #endregion
    }
}