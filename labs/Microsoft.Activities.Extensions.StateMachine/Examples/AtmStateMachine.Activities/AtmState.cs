// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmState.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AtmStateMachine.Activities
{
    /// <summary>
    /// States for the ATM machine
    /// </summary>
    public enum AtmState
    {
        /// <summary>
        /// Waiting to insert a card
        /// </summary>
        InsertCard, 

        /// <summary>
        /// Power off
        /// </summary>
        PowerOff, 

        /// <summary>
        /// Initializing at startup
        /// </summary>
        Initialize, 

        /// <summary>
        /// Enter a PIN
        /// </summary>
        EnterPin, 

        /// <summary>
        /// Remove card
        /// </summary>
        RemoveCard
    }
}