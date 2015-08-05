// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineInfo.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System;
    using System.Activities;
    using System.Activities.Statements.Tracking;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Microsoft.Activities.Extensions.Diagnostics;

    /// <summary>
    ///   Contains information about a tracked state machine
    /// </summary>
    public class StateMachineInfo : ITraceable, IStateMachineInfo
    {
        #region Constants

        /// <summary>
        ///   The default max history
        /// </summary>
        public const int DefaultMaxHistory = 1000;

        #endregion

        #region Fields

        /// <summary>
        ///   The state history
        /// </summary>
        private readonly CircularBuffer<string> stateHistory;

        /// <summary>
        ///   The synchronization object
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        ///   The current state
        /// </summary>
        private string currentState;

        /// <summary>
        ///   The possible transitions
        /// </summary>
        private IList<string> possibleTransitions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineInfo"/> class /&gt;
        /// </summary>
        /// <param name="maxHistory">
        /// The maximum number of history entries 
        /// </param>
        public StateMachineInfo(int maxHistory = DefaultMaxHistory)
        {
            this.MaxHistory = maxHistory;
            this.stateHistory = new CircularBuffer<string>(this.MaxHistory);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineInfo"/> class.
        /// </summary>
        /// <param name="surrogate">
        /// The surrogate 
        /// </param>
        internal StateMachineInfo(StateMachineInfoSurrogated surrogate)
        {
            Contract.Requires(surrogate != null);
            if (surrogate == null)
            {
                throw new ArgumentNullException("surrogate");
            }

            this.currentState = surrogate.CurrentState;
            this.InstanceId = surrogate.InstanceId;
            this.InstanceState = surrogate.InstanceState;
            this.MaxHistory = surrogate.MaxHistory;
            this.Name = surrogate.Name;
            this.possibleTransitions = new List<string>(surrogate.PossibleTransitions);
            this.PreviousState = surrogate.PreviousState;
            this.stateHistory = new CircularBuffer<string>(surrogate.StateHistory, this.MaxHistory);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the last known state of the current state machine
        /// </summary>
        public string CurrentState
        {
            get
            {
                return this.currentState;
            }

            private set
            {
                lock (this.syncLock)
                {
                    this.PreviousState = this.currentState;
                    this.currentState = value;
                    if (!string.IsNullOrWhiteSpace(this.currentState))
                    {
                        this.stateHistory.Add(this.currentState);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets the instance ID of the state machine
        /// </summary>
        public Guid InstanceId { get; internal set; }

        /// <summary>
        ///   Gets the last known instance state
        /// </summary>
        public ActivityInstanceState InstanceState { get; internal set; }

        /// <summary>
        ///   Gets the maximum number of state history entries
        /// </summary>
        public int MaxHistory { get; private set; }

        /// <summary>
        ///   Gets the name of the state machine
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///   Gets the possible transitions
        /// </summary>
        public ReadOnlyCollection<string> PossibleTransitions
        {
            get
            {
                return new ReadOnlyCollection<string>(this.possibleTransitions ?? new List<string>());
            }
        }

        /// <summary>
        ///   Gets the previous state
        /// </summary>
        public string PreviousState { get; private set; }

        /// <summary>
        ///   Gets the state history
        /// </summary>
        public ReadOnlyCollection<string> StateHistory
        {
            get
            {
                return new ReadOnlyCollection<string>(this.stateHistory.ToArray());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Invoked when the tracked state machine is aborted
        /// </summary>
        /// <param name="record">
        /// The aborted record 
        /// </param>
        public void Abort(WorkflowInstanceAbortedRecord record)
        {
            this.InstanceState = ActivityInstanceState.Faulted;
            this.CurrentState = null;
            this.possibleTransitions = null;
        }

        /// <summary>
        /// Creates a formatted string for tracing
        /// </summary>
        /// <param name="tabs"> the tabs
        /// The tab count 
        /// </param>
        /// <returns>
        /// A string 
        /// </returns>
        public string ToFormattedString(int tabs = 0)
        {
            var tsb = new TraceStringBuilder(tabs) { Options = WorkflowTraceOptions.ShowCollectionCount };
            tsb.AppendLine("State Machine: {0}", this.Name);
            using (tsb.IndentBlock())
            {
                tsb.AppendLine("InstanceId: {0}", this.InstanceId);
                tsb.AppendLine("Instance State: {0}", this.InstanceState);
                tsb.AppendLine("Current State: {0}", this.CurrentState);
                tsb.AppendLine("Previous State: {0}", this.PreviousState);
                tsb.AppendCollection("Possible Transitions", this.PossibleTransitions);
                tsb.AppendCollection("State History", this.StateHistory);
            }

            return tsb.ToString();
        }

        /// <summary>
        /// Update the state from a state machine record
        /// </summary>
        /// <param name="record">
        /// The record 
        /// </param>
        public void UpdateState(StateMachineStateRecord record)
        {
            this.CurrentState = record.StateName;
            this.possibleTransitions =
                record.GetTransitions().Select(
                    transition =>
                    string.IsNullOrWhiteSpace(transition.DisplayName)
                        ? transition.GetType().Name
                        : transition.DisplayName).ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the state info based on the record
        /// </summary>
        /// <param name="record">
        /// The activity state record 
        /// </param>
        internal void UpdateState(ActivityStateRecord record)
        {
            this.InstanceState = record.GetInstanceState();
            switch (this.InstanceState)
            {
                case ActivityInstanceState.Executing:
                    break;
                case ActivityInstanceState.Closed:
                    this.CurrentState = null;
                    this.possibleTransitions = null;

                    break;
                case ActivityInstanceState.Canceled:
                    this.CurrentState = null;
                    this.possibleTransitions = null;
                    break;
                case ActivityInstanceState.Faulted:
                    this.CurrentState = null;
                    this.possibleTransitions = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}