// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmTrigger.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AtmStateMachine.Activities
{
    /// <summary>
    /// Triggers that cause the state transition
    /// </summary>
    public enum AtmTrigger
    {
        /// <summary>
        /// Insert Card
        /// </summary>
        InsertCard, 

        /// <summary>
        /// The power was turned on
        /// </summary>
        PowerOn, 

        /// <summary>
        /// The power was turned off
        /// </summary>
        PowerOff, 

        /// <summary>
        /// </summary>
        CardInserted, 

        /// <summary>
        /// </summary>
        KeypadEnter, 

        /// <summary>
        /// </summary>
        CardRemoved
    }
}