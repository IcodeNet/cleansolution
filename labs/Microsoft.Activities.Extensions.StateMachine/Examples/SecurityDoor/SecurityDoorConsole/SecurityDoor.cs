// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityDoor.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoorConsole
{
    using System;

    using Microsoft.Activities.Extensions.StateMachine;

    /// <summary>
    ///   Class that encapsulates the security door state machine
    /// </summary>
    public class SecurityDoor
    {
        #region Fields

        /// <summary>
        ///   The state machine
        /// </summary>
        private readonly CodeStateMachine<DoorState, Triggers> doorStateMachine =
            new CodeStateMachine<DoorState, Triggers>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="SecurityDoor" /> class.
        /// </summary>
        public SecurityDoor()
        {
            // TODO (03) Create your state machine using a fluent interface
            this.doorStateMachine.AddState(DoorState.ClosedLocked)
                .When(Triggers.AuthorizeKey, DoorState.ClosedUnlocked, this.AuthorizedKey)
                .When(Triggers.AuthorizeKey, DoorState.IntrusionDetect, this.UnauthorizedKey);

            this.doorStateMachine.AddState(DoorState.ClosedUnlocked)
                .When(Triggers.OpenTimeout, DoorState.ClosedLocked)
                .When(Triggers.DoorOpened, DoorState.Open);

            this.doorStateMachine.AddState(DoorState.Open).Entry(this.OnDoorOpen)
                .When(Triggers.DoorClosed, DoorState.ClosedLocked)
                .When(Triggers.OpenTimeout, DoorState.Alert);

            this.doorStateMachine.AddTracking(new ConsoleTrackingParticipant());
        }

        private void OnDoorOpen()
        {
            System.Diagnostics.Trace.WriteLine("The door is now open");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets State.
        /// </summary>
        public DoorState State { get; private set; }

        /// <summary>
        ///   Gets or sets Timeout for workflow operations
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return this.doorStateMachine.Timeout;
            }

            set
            {
                this.doorStateMachine.Timeout = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets KeyGuid.
        /// </summary>
        protected Guid KeyGuid { get; set; }

        /// <summary>
        ///   Gets or sets UnlockAttempts.
        /// </summary>
        protected int UnlockAttempts { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Insert a key into the lock
        /// </summary>
        /// <param name="guid">
        /// The guid. 
        /// </param>
        public void InsertKey(Guid guid)
        {
            this.KeyGuid = guid;
            this.doorStateMachine.Fire(Triggers.AuthorizeKey, null)
                .Wait(DoorState.ClosedUnlocked, DoorState.IntrusionDetect);
        }

        /// <summary>
        /// </summary>
        public void Trace()
        {
            System.Diagnostics.Trace.WriteLine("SecurityDoor Trace");
            System.Diagnostics.Trace.WriteLine(string.Format("Current State: {0}", this.State));
            System.Diagnostics.Trace.WriteLine("*** Workflow Definition ***");
            System.Diagnostics.Trace.WriteLine(this.doorStateMachine.Xaml);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Determines if the key is authorized
        /// </summary>
        /// <returns> true if the key is valid </returns>
        private bool AuthorizedKey()
        {
            if (this.KeyGuid != Guid.Empty)
            {
                System.Diagnostics.Trace.WriteLine("The key is authorized");
                this.UnlockAttempts = 1;
                return true;
            }

            System.Diagnostics.Trace.WriteLine("The key is Unauthorized");
            return false;
        }

        /// <summary>
        ///   Determines if the key is unauthorized
        /// </summary>
        /// <returns> true if the key is unauthorized </returns>
        private bool UnauthorizedKey()
        {
            return !this.AuthorizedKey();
        }

        #endregion
    }
}