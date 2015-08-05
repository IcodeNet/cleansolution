namespace SecurityDoorConsole
{
    /// <summary>
    /// The triggering events for the security door
    /// </summary>
    /// <remarks>
    /// TODO (02) Create a list of triggering events
    /// </remarks>
    internal enum Triggers
    {
        /// <summary>
        /// A key was inserted into the lock
        /// </summary>
        AuthorizeKey, 

        /// <summary>
        /// The door was unlocked but failed to open before timeout
        /// </summary>
        UnlockTimeout, 

        /// <summary>
        /// The door was opened
        /// </summary>
        DoorOpened, 

        /// <summary>
        /// The door was closed
        /// </summary>
        DoorClosed, 

        /// <summary>
        /// The door was open too long
        /// </summary>
        OpenTimeout, 

        /// <summary>
        /// A user failed to open the door with an unauthorized key too many times
        /// </summary>
        Warning, 

        /// <summary>
        /// A user failed to open the door with an unauthorized key
        /// </summary>
        Retry, 

        /// <summary>
        /// The door has been reset after an alert
        /// </summary>
        Reset, 
    }
}