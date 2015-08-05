// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmMachine.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AtmStateMachine.Activities
{
    using System.Collections.Generic;

    using Microsoft.Activities.Extensions.StateMachine;

    /// <summary>
    ///   The ATM Machine object
    /// </summary>
    public class AtmMachine : CodeStateMachine<AtmState, AtmTrigger>
    {
        #region Fields

        /// <summary>
        /// The list of displayed prompts
        /// </summary>
        private readonly List<string> displayedPrompts = new List<string>();

        /// <summary>
        /// The list of transitions
        /// </summary>
        private readonly List<AtmTrigger> transitions = new List<AtmTrigger>();

        /// <summary>
        /// The atmStateMachine
        /// </summary>
        private CodeStateMachine<AtmState, AtmTrigger> atmStateMachine;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AtmMachine"/> class.
        /// </summary>
        public AtmMachine()
        {
            this.CardReaderResults = new List<CardReaderResult>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets CardReaderResults.
        /// </summary>
        public List<CardReaderResult> CardReaderResults { get; private set; }

        /// <summary>
        /// Gets ClearCount.
        /// </summary>
        public int ClearCount { get; private set; }

        /// <summary>
        /// Gets DisplayedPrompts.
        /// </summary>
        public List<string> DisplayedPrompts
        {
            get
            {
                return this.displayedPrompts;
            }
        }

        /// <summary>
        /// Gets StateMachine.
        /// </summary>
        public CodeStateMachine<AtmState, AtmTrigger> StateMachine
        {
            get
            {
                return this.atmStateMachine ?? this.CreateStateMachine();
            }
        }

        /// <summary>
        /// Gets Transitions.
        /// </summary>
        public IEnumerable<AtmTrigger> Transitions
        {
            get
            {
                return this.transitions;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether CardValid.
        /// </summary>
        protected bool CardValid { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Accept a Pin
        /// </summary>
        /// <param name="pin">
        /// The pin.
        /// </param>
        public void AcceptPin(string pin)
        {
            this.StateMachine.Fire(AtmTrigger.KeypadEnter, pin);

            // Record the transitions
            this.transitions.Add(AtmTrigger.KeypadEnter);
        }

        /// <summary>
        /// Clear the view
        /// </summary>
        public void ClearView()
        {
            this.ClearCount++;
        }

        /// <summary>
        /// Insert a card
        /// </summary>
        /// <param name="validCard">
        /// The valid card.
        /// </param>
        public void InsertCard(bool validCard)
        {
            this.CardValid = validCard;
            this.CardReaderResults.Add(
                new CardReaderResult
                    {
                        CardStatus = validCard ? CardStatus.Valid : CardStatus.Invalid, 
                        Event = CardReaderEvent.CardInserted
                    });

            // TODO: We should be able to pick up the validCard value from the condition func
            // Is that really necessary to pass it through the bookmark?

            // Will block until the CardInserted trigger is enabled
            this.StateMachine.Fire(AtmTrigger.CardInserted, validCard);

            // Record the transitions
            this.transitions.Add(AtmTrigger.CardInserted);
        }

        /// <summary>
        /// Prompt the user
        /// </summary>
        /// <param name="prompt">
        /// The prompt.
        /// </param>
        public void Prompt(string prompt)
        {
            this.displayedPrompts.Add(prompt);
        }

        /// <summary>
        /// Turn the power off
        /// </summary>
        public void TurnPowerOff()
        {
            this.StateMachine.Fire(AtmTrigger.PowerOff);

            // Record the transitions
            this.transitions.Add(AtmTrigger.PowerOff);
        }

        /// <summary>
        /// Turn the power on
        /// </summary>
        public void TurnPowerOn()
        {
            // Record the transitions
            this.transitions.Add(AtmTrigger.PowerOn);

            // Starts the state machine
            this.StateMachine.StartAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the state machine
        /// </summary>
        /// <returns>
        /// The state machine
        /// </returns>
        private CodeStateMachine<AtmState, AtmTrigger> CreateStateMachine()
        {
            // Fluent interface for declaring the state machine
            // Enums define the states and transitions
            this.atmStateMachine = new CodeStateMachine<AtmState, AtmTrigger> { DisplayName = "ATM StateMachine" };

            // Use an indexer to refer to the state you want
            // Non workflow developers can use lambda expressions or Action to provide implementation - no need to learn WF
            // AutoTrigger is a null transition
            this.atmStateMachine[AtmState.Initialize]
                .Entry(this.Prompt, Prompts.PleaseWait)
                .Exit(this.ClearView)
                .AutoTrigger(AtmState.InsertCard);

            // When defines a transition that is implemented with a bookmark
            // Add multiple transitions with multiple When statements
            this.atmStateMachine[AtmState.InsertCard]
                .Entry(this.Prompt, Prompts.InsertCard)
                .When(AtmTrigger.PowerOff, AtmState.PowerOff)
                .When(AtmTrigger.CardInserted, AtmState.EnterPin, condition: this.IsCardValid, action: this.TurnCameraOn)
                .When(AtmTrigger.CardInserted, AtmState.RemoveCard, condition: this.IsCardInvalid, action: this.TurnCameraOn);

            this.atmStateMachine[AtmState.EnterPin]
                .Entry(this.Prompt, Prompts.EnterYourPin)
                .Exit(this.ClearView)
                .When(AtmTrigger.KeypadEnter, AtmState.InsertCard);

            this.atmStateMachine[AtmState.RemoveCard]
                .Entry(this.Prompt, Prompts.ErrRemoveCard)
                .Exit(this.ClearView)
                .When(AtmTrigger.CardRemoved, AtmState.InsertCard, action: this.TurnCameraOff);

            this.atmStateMachine[AtmState.PowerOff].Entry(this.ClearView);

            return this.atmStateMachine;
        }

        /// <summary>
        /// Determines if a card is invalid
        /// </summary>
        /// <returns>
        /// true if the card is invalid.
        /// </returns>
        private bool IsCardInvalid()
        {
            return !this.CardValid;
        }

        /// <summary>
        /// Determines if the card is valid
        /// </summary>
        /// <returns>
        /// true if the card is valid.
        /// </returns>
        private bool IsCardValid()
        {
            return this.CardValid;
        }

        /// <summary>
        /// Turns the camera off
        /// </summary>
        private void TurnCameraOff()
        {
        }

        /// <summary>
        /// Turns the camera on
        /// </summary>
        private void TurnCameraOn()
        {
            // TODO
        }

        #endregion
    }
}