// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeStateMachine.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Statements.Tracking;
    using System.Activities.Tracking;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Xaml;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   Initializes a new instance of CodeStateMachine
    /// </summary>
    public class CodeStateMachine : CodeStateMachine<string, string>
    {
    }

    // TODO: Design Note - The interface works hard to avoid forcing you to declare all states before hooking them together 
    // This can result in some unexpected behavior like an indexer adding a state.

    /// <summary>
    /// A state machine with a fluent interface
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state 
    /// </typeparam>
    /// <typeparam name="TTrigger">
    /// The type of the trigger 
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Reviewed. Suppression is OK here.")]
    public class CodeStateMachine<TState, TTrigger> : TypedTrackingParticipant
    {
        #region Fields

        /// <summary>
        ///   The completed event
        /// </summary>
        private readonly ManualResetEvent completedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   A list of state events
        /// </summary>
        private readonly Dictionary<TState, ManualResetEvent> stateEvents = new Dictionary<TState, ManualResetEvent>();

        // private readonly Dictionary<TState, ManualResetEvent> stateEvents = new Dictionary<TState, ManualResetEvent>();

        /// <summary>
        ///   Maps the display name of the state to a TState
        /// </summary>
        private readonly Dictionary<string, TState> stateKeyMap = new Dictionary<string, TState>();

        /// <summary>
        ///   The actual state machine
        /// </summary>
        private readonly StateMachine stateMachine =
            new StateMachine();

        /// <summary>
        ///   The code state machine
        /// </summary>
        private readonly StateMachineHost stateMachineHost = new StateMachineHost();

        /// <summary>
        ///   The list of states
        /// </summary>
        private readonly Dictionary<TState, StateMachineState<TState, TTrigger>> states =
            new Dictionary<TState, StateMachineState<TState, TTrigger>>();

        /// <summary>
        ///   A lock object
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        ///   A list of triggering events
        /// </summary>
        private readonly Dictionary<TTrigger, ManualResetEvent> triggerEvents =
            new Dictionary<TTrigger, ManualResetEvent>();

        /// <summary>
        ///   The trigger map
        /// </summary>
        private readonly Dictionary<string, StateMachineTransition<TState, TTrigger>> triggerMap =
            new Dictionary<string, StateMachineTransition<TState, TTrigger>>();

        /// <summary>
        ///   The workflow application
        /// </summary>
        private readonly WorkflowApplication workflowApplication;

        /// <summary>
        ///   Initialized flag
        /// </summary>
        private bool initialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="CodeStateMachine{TState,TTrigger}" /> class.
        /// </summary>
        public CodeStateMachine()
        {
            this.Timeout = TimeSpan.FromSeconds(5);

            this.Arguments = new WorkflowArguments();
            this.Variables = new Dictionary<string, Type>();

            this.workflowApplication = new WorkflowApplication(this.stateMachineHost, this.Arguments);
            this.workflowApplication.Extensions.Add(this);
            this.WorkflowApplication.Idle = this.OnIdle;
            this.WorkflowApplication.Completed = this.OnComplete;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Arguments.
        /// </summary>
        public dynamic Arguments { get; set; }

        /// <summary>
        ///   Gets or sets DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.WorkflowStateMachine.DisplayName;
            }

            set
            {
                // TODO - Throw if workflow has already started
                if (this.IsRunning)
                {
                    throw new InvalidOperationException("Cannot modify once the workflow is running");
                }

                this.WorkflowStateMachine.DisplayName = value;
            }
        }

        /// <summary>
        ///   Gets or sets Output.
        /// </summary>
        public dynamic Output { get; set; }

        /// <summary>
        ///   Gets State.
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        ///   Gets or sets the stateChanged event
        /// </summary>
        public EventHandler<StateChangedEventArgs<TState>> StateChanged { get; set; }

        /// <summary>
        ///   Gets or sets StateFromString.
        /// </summary>
        public Converter<string, TState> StateFromString { get; set; }

        /// <summary>
        ///   Gets States.
        /// </summary>
        public IDictionary<TState, StateMachineState<TState, TTrigger>> States
        {
            get
            {
                return this.states;
            }
        }

        /// <summary>
        ///   Gets or sets Timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        ///   Gets or sets Tracking.
        /// </summary>
        public TrackingParticipant Tracking { get; set; }

        /// <summary>
        ///   Gets or sets TriggerFromString.
        /// </summary>
        public Converter<string, TTrigger> TriggerFromString { get; set; }

        /// <summary>
        ///   Gets Variables.
        /// </summary>
        public IDictionary<string, Type> Variables { get; private set; }

        /// <summary>
        ///   Gets WorkflowApplication.
        /// </summary>
        public WorkflowApplication WorkflowApplication
        {
            get
            {
                return this.workflowApplication;
            }
        }

        /// <summary>
        ///   Gets WorkflowStateMachine.
        /// </summary>
        public StateMachine WorkflowStateMachine
        {
            get
            {
                return this.stateMachine;
            }
        }

        /// <summary>
        ///   Gets Xaml.
        /// </summary>
        public string Xaml
        {
            get
            {
                return XamlServices.Save(this.stateMachineHost);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsRunning.
        /// </summary>
        protected bool IsRunning
        {
            get
            {
                // Check to see if the completed event is set with no timeout
                return this.completedEvent.WaitOne(0);
            }
        }

        #endregion

        // TODO: An indexer that adds an item is an unexpected behavior - is that too weird?
        #region Public Indexers

        /// <summary>
        ///   Indexer for states
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <returns> The state </returns>
        public StateMachineState<TState, TTrigger> this[TState index]
        {
            get
            {
                StateMachineState<TState, TTrigger> state;

                // Add the state if it does not exist yet, otherwise return it
                return !this.States.TryGetValue(index, out state) ? this.AddState(index) : state;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a state
        /// </summary>
        /// <param name="stateKey">
        /// The state key. 
        /// </param>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <returns>
        /// The state 
        /// </returns>
        public StateMachineState<TState, TTrigger> AddState(TState stateKey, string name = null)
        {
            StateMachineState<TState, TTrigger> state;

            // Note: It is not an error to try and add a state that already exists
            // Because we are trying not to force creation order, the state may
            // be implicitly created before the caller thinks it is.
            if (!this.States.TryGetValue(stateKey, out state))
            {
                state = new StateMachineState<TState, TTrigger>(stateKey, this, name);

                this.stateKeyMap.Add(state.DisplayName, stateKey);
                this.states.Add(stateKey, state);
            }

            return state;
        }

        /// <summary>
        /// Adds a tracking participant
        /// </summary>
        /// <param name="trackingParticipant">
        /// The tracking participant. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Cannot add a tracking participant if already running
        /// </exception>
        public void AddTracking(TrackingParticipant trackingParticipant)
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.workflowApplication.Extensions.Add(trackingParticipant);
        }

        /// <summary>
        /// Creates an InArgument
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="defaultValue">
        /// The default value. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the argument 
        /// </typeparam>
        /// <returns>
        /// The InArgument 
        /// </returns>
        public InArgument<T> CreateInArgument<T>(string name, T defaultValue)
        {
            var arg = new InArgument<T>(defaultValue);
            this.stateMachineHost.Arguments.Add(name, arg);
            return arg;
        }

        /// <summary>
        /// Creates a variable
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <param name="type">
        /// The type. 
        /// </param>
        /// <returns>
        /// A variable 
        /// </returns>
        public Variable CreateVariable(string name, Type type)
        {
            var variable = Variable.Create(name, type, VariableModifiers.None);
            this.stateMachineHost.Variables.Add(variable);
            return variable;
        }

        ///// <summary>
        ///// Fires a trigger async
        ///// </summary>
        ///// <param name="trigger">
        ///// The trigger. 
        ///// </param>
        ///// <param name="value">
        ///// The value. 
        ///// </param>
        ///// <returns>
        ///// The state machine 
        ///// </returns>
        ///// <exception cref="InvalidOperationException">
        ///// The statemachine is not waiting for the trigger
        ///// </exception>
        ///// <exception cref="ArgumentOutOfRangeException">
        ///// An unknown resumption result
        ///// </exception>
        // public CodeStateMachine<TState, TTrigger> Fire(TTrigger trigger, object value = null)
        // {
        // return this.Fire(trigger, value).WaitForComplete();
        // }

        /// <summary>
        /// Fires a trigger async
        /// </summary>
        /// <param name="trigger">
        /// The trigger. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <returns>
        /// The state machine 
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The statemachine is not waiting for the trigger
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// An unknown resumption result
        /// </exception>
        public CodeStateMachine<TState, TTrigger> Fire(TTrigger trigger, object value = null)
        {
            // If the workflow is not running, start it and wait for the trigger
            if (!this.IsRunning)
            {
                this.Start(trigger);
            }

            // WaitForComplete for the trigger, then fire
            this.Wait(trigger);

            // ReSharper disable AssignNullToNotNullAttribute
            switch (this.WorkflowApplication.ResumeBookmark(trigger.ToString(), value))
            {
                    // ReSharper restore AssignNullToNotNullAttribute
                case BookmarkResumptionResult.Success:
                    break;
                case BookmarkResumptionResult.NotFound:
                    throw new InvalidOperationException("CodeStateMachine is not waiting for trigger " + trigger);
                case BookmarkResumptionResult.NotReady:
                    throw new InvalidOperationException("CodeStateMachine is not ready for a trigger right now");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return this;
        }

        ///// <summary>
        ///// Runs the state machine
        ///// </summary>
        // public void Start()
        // {
        // this.Initialize();

        // // Use Microsoft.Activities extension to run the state machine to completion, abort or timeout
        // this.WorkflowApplication.RunEpisode(this.Timeout);
        // }

        /// <summary>
        /// Runs until a trigger is ready
        /// </summary>
        /// <param name="triggers">
        /// The triggers you will run until. 
        /// </param>
        /// <remarks>
        /// Specify a list of triggers
        /// </remarks>
        public void Start(params TTrigger[] triggers)
        {
            this.Initialize();

            if (triggers.Length > 0)
            {
                var triggerStrings = triggers.Select(t => t.ToString());

                // Use Microsoft.Activities extension to run the state machine until an idle with one of the triggers as a bookmark name
                this.WorkflowApplication.RunEpisode(
                    (args, s) => args.Bookmarks.Select(b => b.BookmarkName).Intersect(triggerStrings).Any(), 
                    this.Timeout);
            }
            else
            {
                // Use Microsoft.Activities extension to run the state machine to completion, abort or timeout
                this.WorkflowApplication.RunEpisode((args, s) => args.Bookmarks.Any(), this.Timeout);
            }
        }

        /// <summary>
        ///   Runs the state machine async
        /// </summary>
        public void StartAsync()
        {
            this.Initialize();

            this.WorkflowApplication.Run();
        }

        /// <summary>
        /// Waits for the trigger
        /// </summary>
        /// <param name="triggers">
        /// Triggers you want to wait on 
        /// </param>
        /// <exception cref="TimeoutException">
        /// The trigger did not occur within the timeout
        /// </exception>
        public void Wait(params TTrigger[] triggers)
        {
            var handles = triggers.Select(trigger => this.triggerEvents[trigger]).ToArray();

            // ReSharper disable CoVariantArrayConversion
            if (WaitHandle.WaitAny(handles, this.Timeout) == WaitHandle.WaitTimeout)
            {
                // ReSharper restore CoVariantArrayConversion
                throw new TimeoutException(
                    "Timeout waiting for state transition to state(s) " + this.states.ToDelimitedList());
            }

            // if (!this.triggerEvents[waitForTrigger].WaitOne(this.Timeout))
            // {
            // throw new TimeoutException("Timeout waiting for trigger " + waitForTrigger);
            // }
        }

        /// <summary>
        /// Waits for the workflow to reach a certain state
        /// </summary>
        /// <param name="states">
        /// The states. 
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void Wait(params TState[] states)
        {
            var handles = states.Select(state => this.stateEvents[state]).ToArray();

            // ReSharper disable CoVariantArrayConversion
            if (WaitHandle.WaitAny(handles) == WaitHandle.WaitTimeout)
            {
                // ReSharper restore CoVariantArrayConversion
                throw new TimeoutException(
                    "Timeout waiting for state transition to state(s) " + states.ToDelimitedList());
            }
        }

        /// <summary>
        ///   Waits for the workflow to complete
        /// </summary>
        /// <exception cref="TimeoutException">Workflow did not complete before timeout</exception>
        public void WaitForComplete()
        {
            if (!this.completedEvent.WaitOne(this.Timeout))
            {
                throw new TimeoutException("Timeout waiting for workflow to complete");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tracks a StateMachineState Record
        /// </summary>
        /// <param name="record">
        /// The record. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// An unknown state was encountered
        /// </exception>
        protected override void Track(StateMachineStateRecord record, TimeSpan timeout)
        {
            if (!this.TrySetState(record.StateName))
            {
                throw new InvalidOperationException("Unknown state " + record.StateName);
            }
        }

        /// <summary>
        /// Adds a trigger
        /// </summary>
        /// <param name="transition">
        /// The transition. 
        /// </param>
        private void AddTrigger(StateMachineTransition<TState, TTrigger> transition)
        {
            this.triggerMap.Add(transition.Trigger.ToString(), transition);
            this.triggerEvents.Add(transition.Trigger, new ManualResetEvent(false));
        }

        /// <summary>
        ///   Initialize the state machine
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot initialilze if running</exception>
        private void Initialize()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Cannot initialize while CodeStateMachine is running");
            }

            lock (this.syncLock)
            {
                this.ResetTriggerEvents();
                this.completedEvent.Reset();

                // Return if the state machine has already been created
                if (this.initialized)
                {
                    return;
                }

                // Pass 1 resolve the states
                // Add all the states to the state machine
                foreach (var state in this.States.Values)
                {
                    this.WorkflowStateMachine.States.Add(state.WorkflowState);

                    // If the state was marked as initial state explicitly
                    if (state.InitialState)
                    {
                        this.WorkflowStateMachine.InitialState = state.WorkflowState;
                    }

                    this.stateEvents[state.Key] = new ManualResetEvent(false);
                }

                // If no other state was marked as the initial state
                if (this.WorkflowStateMachine.InitialState == null)
                {
                    // The first state becomes the initial state
                    this.WorkflowStateMachine.InitialState = this.States.Values.First().WorkflowState;
                }

                // Pass 2 - resolve transitions
                foreach (var state in this.States.Values)
                {
                    foreach (var transition in state.Transitions)
                    {
                        // Create a transition
                        state.WorkflowState.Transitions.Add(transition.CreateTransition());

                        // Map triggers to strings so we can reverse the process later
                        StateMachineTransition<TState, TTrigger> trigger;
                        if (!this.triggerMap.TryGetValue(transition.Trigger.ToString(), out trigger))
                        {
                            // The trigger map enables conversion from string to TTrigger
                            // This is required when resuming bookmarks where the name of the bookmark
                            this.AddTrigger(transition);
                        }
                    }

                    // States with no transitions are marked as final
                    state.IsFinal(state.WorkflowState.Transitions.Count == 0);
                }

                this.stateMachineHost.StateMachine = this.stateMachine;
                this.initialized = true;
            }
        }

        /// <summary>
        /// Called when the workflow complete
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <exception cref="WorkflowCanceledException">
        /// The workflow was canceled
        /// </exception>
        /// <exception cref="WorkflowApplicationTerminatedException">
        /// The workflow terminated
        /// </exception>
        private void OnComplete(WorkflowApplicationCompletedEventArgs args)
        {
            try
            {
                switch (args.CompletionState)
                {
                    case ActivityInstanceState.Closed:
                        this.Output = WorkflowArguments.FromDictionary(args.Outputs);
                        break;
                    case ActivityInstanceState.Canceled:
                        throw new WorkflowCanceledException("Workflow Canceled");
                    case ActivityInstanceState.Faulted:
                        throw new WorkflowApplicationTerminatedException("Workflow Faulted", args.TerminationException);
                }
            }
            finally
            {
                // Release any threads waiting for completion
                this.completedEvent.Set();
            }
        }

        /// <summary>
        /// Called when the workflow is idle
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private void OnIdle(WorkflowApplicationIdleEventArgs args)
        {
            lock (this.syncLock)
            {
                // Reset all the triggers
                this.ResetTriggerEvents();

                // Signal events that match a bookmark name
                foreach (var bookmark in args.Bookmarks)
                {
                    // Try to set the trigger, it may not match
                    this.TrySetTrigger(bookmark.BookmarkName);
                }
            }
        }

        /// <summary>
        ///   Resets the trigger events
        /// </summary>
        private void ResetTriggerEvents()
        {
            lock (this.syncLock)
            {
                foreach (var triggerEvent in this.triggerEvents.Values)
                {
                    triggerEvent.Reset();
                }
            }
        }

        /// <summary>
        /// Tries to set a trigger
        /// </summary>
        /// <param name="triggerName">
        /// The trigger name. 
        /// </param>
        private void SetTrigger(string triggerName)
        {
            var transition = this.triggerMap[triggerName];
            this.triggerEvents[transition.Trigger].Set();
        }

        /// <summary>
        /// Try to set the state
        /// </summary>
        /// <param name="stateName">
        /// The state name. 
        /// </param>
        /// <returns>
        /// true if the state was set 
        /// </returns>
        private bool TrySetState(string stateName)
        {
            TState state;

            if (!this.stateKeyMap.TryGetValue(stateName, out state))
            {
                return false;
            }

            // Set the current state
            this.State = state;

            // Reset all state events
            foreach (var stateEvent in this.stateEvents.Values)
            {
                stateEvent.Reset();
            }

            // Set the state event for the current state
            this.stateEvents[state].Set();

            if (this.StateChanged != null)
            {
                this.StateChanged(this, new StateChangedEventArgs<TState> { State = this.State });
            }

            return true;
        }

        /// <summary>
        /// Tries to set a trigger
        /// </summary>
        /// <param name="triggerName">
        /// The trigger name. 
        /// </param>
        /// <returns>
        /// true if the trigger was set 
        /// </returns>
        private bool TrySetTrigger(string triggerName)
        {
            StateMachineTransition<TState, TTrigger> transition;

            return this.triggerMap.TryGetValue(triggerName, out transition)
                   && this.triggerEvents[transition.Trigger].Set();
        }

        #endregion
    }
}