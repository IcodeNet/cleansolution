// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineState.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;

    /// <summary>
    /// A state of a state machine
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state
    /// </typeparam>
    /// <typeparam name="TTrigger">
    /// The type of the trigger
    /// </typeparam>
    public class StateMachineState<TState, TTrigger>
    {
        #region Fields

        /// <summary>
        /// The state machine
        /// </summary>
        private readonly CodeStateMachine<TState, TTrigger> codeStateMachine;

        /// <summary>
        /// The key
        /// </summary>
        private readonly TState key;

        /// <summary>
        /// The workflow state
        /// </summary>
        private readonly State workflowState;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineState{TState,TTrigger}"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="codeStateMachine">
        /// The code state machine.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public StateMachineState(TState key, CodeStateMachine<TState, TTrigger> codeStateMachine, string name = null)
        {
            this.Transitions = new List<StateMachineTransition<TState, TTrigger>>();

            this.codeStateMachine = codeStateMachine;
            this.key = key;
            this.workflowState = new State { DisplayName = name ?? key.ToString() };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets DisplayName.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.workflowState.DisplayName;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether InitialState.
        /// </summary>
        public bool InitialState { get; set; }

        /// <summary>
        /// Gets Key.
        /// </summary>
        public TState Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// Gets or sets Transitions.
        /// </summary>
        public List<StateMachineTransition<TState, TTrigger>> Transitions { get; set; }

        /// <summary>
        /// Gets WorkflowState.
        /// </summary>
        public State WorkflowState
        {
            get
            {
                return this.workflowState;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an auto trigger transition
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> AutoTrigger(
            TState state, string name = null, Action action = null, Func<bool> condition = null)
        {
            // if the destination state does not exist yet, this will add it.
            var destination = this.codeStateMachine[state];

            this.Transitions.Add(
                new StateMachineTransition<TState, TTrigger>
                    {
                        Action = action, 
                        AutoTrigger = true, 
                        Condition = condition, 
                        Source = this, 
                        Destination = destination, 
                        Name = name
                    });

            return this;
        }

        /// <summary>
        /// Creates an entry action
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the action
        /// </typeparam>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> Entry<T1>(Action<T1> action, T1 arg1, string name = null)
        {
            this.workflowState.Entry = new WorkflowAction<T1>
                {
                   Action = action, Arg1 = arg1, DisplayName = name ?? "Entry" 
                };
            return this;
        }

        /// <summary>
        /// Adds an entry action
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> Entry(Action action, string name = null)
        {
            this.workflowState.Entry = new WorkflowAction { Action = action, DisplayName = name ?? "Entry" };
            return this;
        }

        /// <summary>
        /// Adds an exit action
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> Exit(Action action, string name = null)
        {
            this.workflowState.Exit = new WorkflowAction { Action = action, DisplayName = name ?? "Exit" };
            return this;
        }

        /// <summary>
        /// Adds an exit action
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> Exit(Activity activity)
        {
            this.workflowState.Exit = activity;
            return this;
        }

        /// <summary>
        /// Marks the state as the final state
        /// </summary>
        /// <param name="isFinal">
        /// The is final.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> IsFinal(bool isFinal)
        {
            this.WorkflowState.IsFinal = isFinal;
            return this;
        }

        /// <summary>
        /// Marks a state as the start state
        /// </summary>
        /// <param name="isStart">
        /// The is start.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> IsStart(bool isStart)
        {
            this.InitialState = true;
            return this;
        }

        /// <summary>
        /// Creates a transition from the state
        /// </summary>
        /// <param name="trigger">
        /// The trigger.
        /// </param>
        /// <param name="gotoState">
        /// The goto state.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The state
        /// </returns>
        public StateMachineState<TState, TTrigger> When(
            TTrigger trigger, 
            TState gotoState, 
            Func<bool> condition = null, 
            Action action = null, 
            string name = null)
        {
            // Will add the destination if it does not exist yet
            var destination = this.codeStateMachine[gotoState];

            this.Transitions.Add(
                new StateMachineTransition<TState, TTrigger>
                    {
                        Action = action, 
                        Condition = condition, 
                        Source = this, 
                        Destination = destination, 
                        Trigger = trigger, 
                        Name = name
                    });

            return this;
        }

        #endregion
    }
}