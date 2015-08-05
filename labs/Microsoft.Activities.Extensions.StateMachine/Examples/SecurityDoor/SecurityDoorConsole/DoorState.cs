// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoorState.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoorConsole
{
    /// <summary>
    ///   The door states
    /// </summary>
    /// <remarks>
    ///   TODO (01) Create an enum that defines your states
    /// </remarks>
    public enum DoorState
    {
        /// <summary>
        ///   The door is closed and locked
        /// </summary>
        ClosedLocked, 

        /// <summary>
        ///   The door is closed an unlocked
        /// </summary>
        ClosedUnlocked, 

        /// <summary>
        ///   The door is open
        /// </summary>
        Open, 

        /// <summary>
        ///   The door has been open too long
        /// </summary>
        IntrusionDetect, 

        /// <summary>
        ///   The door requires investigation by security
        /// </summary>
        Alert
    }
}