// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.StateMachine.Tests
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Diagnostics;
    using System.Xaml;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Test states
    /// </summary>
    public enum TestState
    {
        /// <summary>
        ///   One Value
        /// </summary>
        One,

        /// <summary>
        ///   Two Value
        /// </summary>
        Two,

        /// <summary>
        ///   Three Value
        /// </summary>
        Three
    }

    /// <summary>
    ///   Test Triggers
    /// </summary>
    public enum TestTrigger
    {
        /// <summary>
        ///   Trigger A
        /// </summary>
        A,

        /// <summary>
        ///   Trigger B
        /// </summary>
        B,

        /// <summary>
        ///   Trigger C
        /// </summary>
        C
    }

    /// <summary>
    ///   Tests for CodeStateMachine
    /// </summary>
    [TestClass]
    public class StateMachineTest
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets TestContext.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Demonstrates how you can use variables and activities
        /// </summary>
        /// <remarks>
        ///   Given * A state machine
        /// </remarks>
        [TestMethod]
        public void AddTwoNumbersAndStoreInVariable()
        {
            // Create a state machine that uses ints to describe states and triggers
            // Using an int as the type requires a conversion function
            var target = new CodeStateMachine<TestState, TestTrigger>();

            var tracking = new MemoryTrackingParticipant();
            target.AddTracking(tracking);

            try
            {
                // Setup arguments that can be passed into the workflow
                // var arg1 = target.CreateInArgument<int>("Num1", 1);

                // Create a variable
                var sum = target.CreateVariable("Sum", typeof(int));

                // TODO: Think more about state, storing it, load it and passing it to activities

                // Access a variable from an expression
                target[TestState.One].Exit(
                    new Assign { To = new OutArgument<int>(sum), Value = new InArgument<int>(3) }).When(
                        TestTrigger.B, TestState.Two);

                // Will run the state machine and fire the trigger when ready
                // then wait for it to complete
                target.Fire(TestTrigger.B).WaitForComplete();

                AssertState.OccursInOrder(target.DisplayName, tracking.Records, TestState.One, TestState.Two);
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(target.Xaml);
            }
        }

        /// <summary>
        ///   Create and run a state machine synchronously
        /// </summary>
        [TestMethod]
        public void CreateAndRunStateIntSync()
        {
            // Create a state machine that uses ints to describe states and triggers
            var target = new CodeStateMachine<int, int>();

            var tracking = new MemoryTrackingParticipant();
            target.AddTracking(tracking);

            try
            {
                // State 1 will wait for trigger 2 which will transition to state 2
                target.AddState(1).When(2, 2);

                // State 2 will be the final state
                target.AddState(2);

                // Will run the state machine and fire the trigger when ready
                target.Fire(2).WaitForComplete();

                AssertState.OccursInOrder(target.DisplayName, tracking.Records, 1, 2);
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(XamlServices.Save(target.WorkflowStateMachine));
            }
        }

        /// <summary>
        ///   Create and run a state machine with a string and enum
        /// </summary>
        [TestMethod]
        public void CreateAndRunStateStringEnum()
        {
            // Create a state machine that uses ints to describe states and triggers
            // Using an int as the type requires a conversion function
            var target = new CodeStateMachine<TestState, TestTrigger>();

            var tracking = new MemoryTrackingParticipant();
            target.AddTracking(tracking);

            try
            {
                // State One will wait for trigger B which will transition to state Two
                target[TestState.One].When(TestTrigger.B, TestState.Two);

                // Will run the state machine and fire the trigger when ready
                // then wait for it to complete
                target.Fire(TestTrigger.B).WaitForComplete();

                AssertState.OccursInOrder(target.DisplayName, tracking.Records, TestState.One, TestState.Two);
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(XamlServices.Save(target.WorkflowStateMachine));
            }
        }

        /// <summary>
        ///   Create and run a state machine sync
        /// </summary>
        [TestMethod]
        public void CreateAndRunStateStringSync()
        {
            // Create a state machine that uses ints to describe states and triggers
            // Using an int as the type requires a conversion function
            var target = new CodeStateMachine();

            var tracking = new MemoryTrackingParticipant();
            target.AddTracking(tracking);

            try
            {
                // State 1 will wait for trigger 2 which will transition to state 2
                target.AddState("One").When("Two", "Two");

                // State 2 will be the final state
                target.AddState("Two");

                // Will run the state machine and fire the trigger when ready
                target.Fire("Two").WaitForComplete();

                AssertState.OccursInOrder(target.DisplayName, tracking.Records, "One", "Two");
            }
            finally
            {
                tracking.Trace();

                Trace.WriteLine(string.Empty);
                Trace.WriteLine("*** Workflow Definition ***");
                Trace.WriteLine(XamlServices.Save(target.WorkflowStateMachine));
            }
        }

        #endregion
    }
}