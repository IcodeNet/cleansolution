// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineTransition.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;
    using System.Activities.Statements;

    /// <summary>
    /// A state machine transition
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state
    /// </typeparam>
    /// <typeparam name="TTrigger">
    /// The type of the trigger
    /// </typeparam>
    public class StateMachineTransition<TState, TTrigger>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets Action.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AutoTrigger.
        /// </summary>
        public bool AutoTrigger { get; set; }

        /// <summary>
        /// Gets or sets Condition.
        /// </summary>
        public Func<bool> Condition { get; set; }

        /// <summary>
        /// Gets or sets Destination.
        /// </summary>
        public StateMachineState<TState, TTrigger> Destination { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets Source.
        /// </summary>
        public StateMachineState<TState, TTrigger> Source { get; internal set; }

        /// <summary>
        /// Gets or sets Trigger.
        /// </summary>
        public TTrigger Trigger { get; set; }

        /// <summary>
        /// Gets WorkflowTransition.
        /// </summary>
        public Transition WorkflowTransition { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates a transition
        /// </summary>
        /// <returns>
        /// The transition
        /// </returns>
        public Transition CreateTransition()
        {
            // Create a new transition or use an existing one which already has a WorkflowTransition
            var foundTransition =
                this.Source.Transitions.Find(
                    t => t.Trigger.Equals(this.Trigger) && t.WorkflowTransition != null && t != this);

            // If a transition was found with the same trigger, use the same trigger activity as a shared trigger
            this.WorkflowTransition = new Transition
                {
                    DisplayName = this.Name ?? "To " + this.Destination.DisplayName, 
                    Trigger = this.AutoTrigger
                                  ? null
                                  : foundTransition != null
                                        ? foundTransition.WorkflowTransition.Trigger
                                        : new BookmarkTrigger
                                            {
                                                BookmarkName = this.Trigger.ToString(), 
                                                DisplayName = "Trigger " + this.Trigger
                                            }, 
                    To = this.Destination.WorkflowState, 
                    Action = this.Action != null
                                 ? new WorkflowAction { Action = this.Action }
                                 : null, 
                    Condition = this.Condition != null
                                    ? new WorkflowCondition { Condition = this.Condition }
                                    : null
                };

            return this.WorkflowTransition;
        }

        #endregion
    }
}