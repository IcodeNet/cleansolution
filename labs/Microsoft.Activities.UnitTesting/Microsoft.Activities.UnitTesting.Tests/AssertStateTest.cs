// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertStateTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities.Statements;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for AssertStateTest and is intended
    ///   to contain all AssertStateTest Unit Tests
    /// </summary>
    [TestClass]
    public class AssertStateTest
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies that states occur in the specified order
        /// </summary>
        [TestMethod]
        public void OccursInOrderDoesNotThrowWhenInOrder()
        {
            StateMachine stateMachine = CreateStateMachine();
            WorkflowInvokerTest host = WorkflowInvokerTest.Create(stateMachine);

            host.TestActivity();
            host.Tracking.Trace();
            AssertState.OccursInOrder("StateMachine", host.Tracking.Records, "state1", "state2", "state3");
        }

        /// <summary>
        /// Verifies that states occur in the specified order
        /// </summary>
        [TestMethod]
        public void OccursInOrderDoesThrowWhenOuyOfOrder()
        {
            StateMachine stateMachine = CreateStateMachine();
            WorkflowInvokerTest host = WorkflowInvokerTest.Create(stateMachine);

            host.TestActivity();
            host.Tracking.Trace();
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => AssertState.OccursInOrder("StateMachine", host.Tracking.Records, "state2", "state1", "state3"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create state machine.
        /// </summary>
        /// <returns>
        /// </returns>
        private static StateMachine CreateStateMachine()
        {
            var state1 = new State { DisplayName = "state1" };
            var state2 = new State { DisplayName = "state2" };
            var state3 = new State { IsFinal = true, DisplayName = "state3" };
            state1.Transitions.Add(new Transition { To = state2 });
            state2.Transitions.Add(new Transition { To = state3 });

            var stateMachine = new StateMachine { InitialState = state1, States = { state1, state2, state3 } };

            return stateMachine;
        }

        #endregion
    }
}