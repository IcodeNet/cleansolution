// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmMachineTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AtmStateMachine.Tests
{
    using System.Diagnostics;
    using System.Xaml;

    using AtmStateMachine.Activities;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the AtmMachine 
    /// </summary>
    [TestClass]
    public class AtmMachineTest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given * The ATM Machine * In the Insert Card state When * An valid card is inserted Then * the camera is turned on * The StateMachine enters the Enter Pin state * and displays prompt "Enter your pin"
        /// </summary>
        /// <remarks>
        ///   Test scenario completed in Exercise 2
        /// </remarks>
        [TestMethod]
        public void InsertValidCard()
        {
            // Works with tracking
            var tracking = new MemoryTrackingParticipant();

            var atm = new AtmMachine();

            // Create a new state machine using enums to define the states I want
            var atmStateMachine = atm.StateMachine;

            // Add my tracking provider
            atmStateMachine.WorkflowApplication.Extensions.Add(tracking);

            try
            {
                // Turn the power on an wait for the CardInserted transition to enable
                atm.TurnPowerOn();

                // Insert a valid card and wait for the KeypadEnter transition
                atm.InsertCard(true);

                // Enter a pin
                atm.AcceptPin("1234");

                // Turn off the ATM
                atm.TurnPowerOff();

                // Verify the first three states occurred in the correct order.
                // Make sure you set the state name correctly
                AssertState.OccursInOrder(
                    "ATM StateMachine", tracking.Records, AtmState.Initialize, AtmState.InsertCard, AtmState.EnterPin);

                // Verify that you added an InitializeAtm activity to the Entry actions of the Initialize State
                AssertHelper.OccursInOrder(
                    "Transitions", 
                    atm.Transitions, 
                    AtmTrigger.PowerOn, 
                    AtmTrigger.CardInserted, 
                    AtmTrigger.KeypadEnter, 
                    AtmTrigger.PowerOff);

                // Verify the prompts
                AssertHelper.OccursInOrder(
                    "Prompts", atm.DisplayedPrompts, Prompts.PleaseWait, Prompts.InsertCard, Prompts.EnterYourPin);

                // Verify the valid card
                Assert.AreEqual(1, atm.CardReaderResults.Count);
                Assert.AreEqual(CardStatus.Valid, atm.CardReaderResults[0].CardStatus);

                // Verify the exit action cleared the screen at least twice
                Assert.IsTrue(
                    atm.ClearCount >= 2, 
                    "Verify that you added a ClearView activity to the Exit Action of the Initialize State");

                // Verify the camera control
                // AssertHelper.OccursInOrder("CameraControl", testViewModel.CameraControl, true);
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(XamlServices.Save(atmStateMachine.WorkflowStateMachine));
            }
        }

        /// <summary>
        ///   Given * A Powered Off ATM When * The Power Is Turned On Then * The ATM displays a please wait message. * and initializes the hardware * After initialization it prompts the user to insert a card
        /// </summary>
        /// <remarks>
        ///   Test scenario completed in Exercise 1
        /// </remarks>
        [TestMethod]
        public void PowerOnAtmScenario()
        {
            // Works with tracking
            var tracking = new MemoryTrackingParticipant();

            var atm = new AtmMachine();

            // Create a new state machine using enums to define the states I want
            var atmStateMachine = atm.StateMachine;

            // Add my tracking provider
            atmStateMachine.WorkflowApplication.Extensions.Add(tracking);

            try
            {
                // Start the state machine until it goes idle with the PowerOff trigger enabled
                atmStateMachine.Start(AtmTrigger.PowerOff);

                // Validate the current state is what you think it is
                Assert.AreEqual(AtmState.InsertCard, atmStateMachine.State);

                // Will block until the transition can fire (or timeout)
                atmStateMachine.Fire(AtmTrigger.PowerOff);

                // WaitForComplete for the atmStateMachine workflow to complete
                atmStateMachine.WaitForComplete();

                // Verify the first three states occurred in the correct order.
                // Make sure you set the state name correctly
                AssertState.OccursInOrder(
                    "ATM StateMachine", tracking.Records, AtmState.Initialize, AtmState.InsertCard);

                // Verify the prompts
                AssertHelper.OccursInOrder("Prompts", atm.DisplayedPrompts, Prompts.PleaseWait, Prompts.InsertCard);

                // Verify the exit action on Initialize and Entry action on PowerOff cleared the screen
                Assert.AreEqual(2, atm.ClearCount);
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(XamlServices.Save(atmStateMachine.WorkflowStateMachine));
            }
        }

        #endregion
    }
}