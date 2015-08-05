// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangedEventArgs.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine
{
    using System;

    /// <summary>
    /// The StateChangedEvent arguments
    /// </summary>
    /// <typeparam name="TState">
    /// The type of the state
    /// </typeparam>
    public class StateChangedEventArgs<TState> : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        public TState State { get; set; }

        #endregion
    }
}