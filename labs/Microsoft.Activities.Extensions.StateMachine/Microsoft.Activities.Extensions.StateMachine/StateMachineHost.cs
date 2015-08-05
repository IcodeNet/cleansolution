// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineHost.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The state machine host class
    /// </summary>
    public sealed class StateMachineHost : NativeActivity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineHost"/> class.
        /// </summary>
        public StateMachineHost()
        {
            this.Variables = new Collection<Variable>();
            this.DisplayName = "StateMachineHost";
            this.Arguments = new Dictionary<string, InArgument>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Arguments.
        /// </summary>
        public Dictionary<string, InArgument> Arguments { get; set; }

        /// <summary>
        /// Gets or sets StateMachine.
        /// </summary>
        public StateMachine StateMachine { get; set; }

        /// <summary>
        /// Gets or sets Variables.
        /// </summary>
        public Collection<Variable> Variables { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates and validates a description of the activity’s arguments, variables, child activities, and activity delegates.
        /// </summary>
        /// <param name="metadata">The activity’s metadata that encapsulates the activity’s arguments, variables, child activities, and activity delegates.</param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            foreach (var kvp in this.Arguments)
            {
                metadata.AddArgument(new RuntimeArgument(kvp.Key, kvp.Value.ArgumentType, kvp.Value.Direction));
            }

            metadata.SetVariablesCollection(this.Variables);
            metadata.AddChild(this.StateMachine);
        }

        /// <summary>
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">The execution context in which the activity executes.</param>
        protected override void Execute(NativeActivityContext context)
        {
            context.ScheduleActivity(this.StateMachine);
        }

        #endregion
    }
}