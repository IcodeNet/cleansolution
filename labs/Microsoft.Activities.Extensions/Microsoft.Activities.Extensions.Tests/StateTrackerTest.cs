// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateTrackerTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Tests the StateTracker
    /// </summary>
    [TestClass]
    public class StateTrackerTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run until State1 
        ///   Then
        ///   * The CurrentState returns State1
        /// </summary>
        [TestMethod]
        public void CurrentStateIsState2()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var actual = tracker.CurrentState;

                // Assert
                Assert.AreEqual(StateMachineExample.State1, actual);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * Two WorkflowApplication hosts run
        ///   When
        ///   * CurrentState is accessed
        ///   Then
        ///   * An InvalidOperationException is thrown
        /// </summary>
        [TestMethod]
        public void CurrentStateWithTwoHostsThrows()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host1 = WorkflowApplicationTest.Create(activity);
            var host2 = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host1.Extensions.Add(tracker);
            host2.Extensions.Add(tracker);

            try
            {
                // Act / Assert
                host1.TestWorkflowApplication.RunUntilBookmark();
                host2.TestWorkflowApplication.RunUntilBookmark();
                host1.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1.ToString(), 1);
                host2.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1.ToString(), 1);

                AssertHelper.Throws<InvalidOperationException>(() => AssertHelper.GetProperty(tracker.CurrentState));
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                WorkflowTrace.Information("Host1");
                host1.Tracking.Trace();
                WorkflowTrace.Information("Host2");
                host2.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine is run
        ///   When
        ///   * The InstanceId property is accessed
        ///   Then
        ///   * The guid returned will be the same as the host instance Id
        /// </summary>
        [TestMethod]
        public void InstanceIdIsHostInstanceId()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var expected = host.Id;
                var actual = tracker.InstanceId;

                // Assert
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Canceled
        /// </summary>
        [TestMethod]
        public void InstanceStateIsCanceledWhenCanceled()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                host.Cancel();
                var actual = tracker.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Canceled, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Closed
        /// </summary>
        [TestMethod]
        public void InstanceStateIsClosedWhenClosed()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                host.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1, 1, StateTrigger.T5);
                host.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T5, 1);
                var actual = tracker.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Closed, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run
        ///   Then
        ///   * The InstanceState returns the instance state of the StateMachine
        /// </summary>
        [TestMethod]
        public void InstanceStateIsExecutingWhenExecuting()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark();
                var actual = tracker.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Executing, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run until the state machine is complete
        ///   Then
        ///   * The InstanceState returns Faulted
        /// </summary>
        [TestMethod]
        public void InstanceStateIsFaultedWhenFaulted()
        {
            // Arrange
            var activity = CreateStateMachineThatFaults();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                AssertHelper.Throws<InvalidOperationException>(() => host.TestWorkflowApplication.RunUntilBookmark());
                var actual = tracker.InstanceState;

                // Assert
                Assert.AreEqual(ActivityInstanceState.Faulted, actual);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The Name property is access
        ///   Then
        ///   * The Name returns StateMachine
        /// </summary>
        [TestMethod]
        public void NameIsStateMachine()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var actual = tracker.Name;

                // Assert
                Assert.AreEqual(StateMachineExample.Name, actual);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateMachine which contains another state machine
        ///   * A StateTracker attached to the host
        ///   * Both state machines are run
        ///   When
        ///   * The StateMachines.Count property is access
        ///   Then
        ///   * There are two tracked state machines
        /// </summary>
        [TestMethod]
        public void NestedStateMachineIsTracked()
        {
            var activity = new NestedStateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);

            try
            {
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1, Constants.Timeout);

                // Run until bookmark "NT1" from nested state machine
                host.TestWorkflowApplication.ResumeUntilBookmark("T1", null, "NT1");

                Assert.AreEqual(2, tracker.StateMachines.Count);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker which has tracked a StateMachine and has been saved to a string
        ///   When
        ///   * Parse is invoked with the string returning a second tracker
        ///   Then
        ///   * The first tracker and the second tracker have the same data
        /// </summary>
        [TestMethod]
        public void ParseShouldDeserialize()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker1 = new StateTracker();
            StateTracker tracker2 = null;
            host.Extensions.Add(tracker1);

            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var xml = tracker1.ToXml();
                tracker2 = StateTracker.Parse(xml);

                // Assert
                Assert.AreEqual(tracker1.StateMachines.Count, tracker2.StateMachines.Count);

                for (var i = 0; i < tracker1.StateMachines.Count; i++)
                {
                    AssertEquivalent(tracker1.StateMachines[i], tracker2.StateMachines[i]);
                }
            }
            finally
            {
                tracker1.Trace();
                if (tracker2 != null)
                {
                    tracker2.Trace();
                }

                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * The StateMachineExample run to state T1
        ///   When
        ///   * The PossibleTransitions collection is read
        ///   Then
        ///   * There are two possible transitions
        ///   * T1 and T2
        /// </summary>
        [TestMethod]
        public void PossibleTransitionsAreTwo()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var actual = tracker.PossibleTransitions;

                // Assert
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(StateTrigger.T1.ToString(), actual.First());
                Assert.AreEqual(StateTrigger.T2.ToString(), actual.Last());
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * An activity with one state machine
        ///   When
        ///   * The activity is run until State1 and again until State2
        ///   Then
        ///   * The CurrentState returns the name of the last known name state
        /// </summary>
        [TestMethod]
        public void PreviousStateIsState1()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);
                host.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1, 1);

                var actual = tracker.PreviousState;

                // Assert
                Assert.AreEqual(StateMachineExample.State1, actual);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker with a max history of 10
        ///   * A StateMachine with 20 states
        ///   When
        ///   * The StateMachine is run to completion
        ///   Then
        ///   * The StateHistory will show only the last 10 states
        /// </summary>
        [TestMethod]
        public void StateHistoryEnforcesMaxHistory()
        {
            // Arrange
            const int ExpectedHistory = 10;
            var activity = CreateLargeStateMachine(20);
            var host = WorkflowInvokerTest.Create(activity);
            var tracker = new StateTracker(maxHistory: ExpectedHistory);
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestActivity();

                var actual = tracker.StateHistory;

                // Assert
                Assert.AreEqual(ExpectedHistory, actual.Count());
                Assert.AreEqual("State11", actual.First());
                Assert.AreEqual("State20", actual.Last());
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker which has tracked a StateMachine
        ///   When
        ///   * ToXml is invoked
        ///   Then
        ///   * Xml is returned
        /// </summary>
        [TestMethod]
        public void ToXmlShouldSerialize()
        {
            const string Expected =
                @"<StateTracker xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/2012/07/Microsoft.Activities.Extensions"">
  <StateMachines xmlns:d2p1=""http://schemas.datacontract.org/2004/07/Microsoft.Activities.Extensions.Tracking"">
    <StateMachine>
      <CurrentState>State1</CurrentState>
      <InstanceId>{0}</InstanceId>
      <InstanceState>Executing</InstanceState>
      <MaxHistory>1000</MaxHistory>
      <Name>StateMachine</Name>
      <PossibleTransitions>
        <Transition>T1</Transition>
        <Transition>T2</Transition>
      </PossibleTransitions>
      <PreviousState i:nil=""true"" />
      <StateHistory>
        <State>State1</State>
      </StateHistory>
    </StateMachine>
  </StateMachines>
</StateTracker>";

            // Arrange
            var activity = new StateMachineExample();
            var host = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host.Extensions.Add(tracker);
            try
            {
                // Act
                host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                var actual = tracker.ToXml();

                // Assert
                Assert.AreEqual(string.Format(Expected, host.Id), actual);
            }
            finally
            {
                tracker.Trace();
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A StateTracker
        ///   * Two WorkflowApplication hosts
        ///   When
        ///   * The activity is run on both hosts
        ///   Then
        ///   * There are two StateMachines tracked
        /// </summary>
        [TestMethod]
        public void TrackerTracksMoreThanOneInstance()
        {
            // Arrange
            var activity = new StateMachineExample();
            var host1 = WorkflowApplicationTest.Create(activity);
            var host2 = WorkflowApplicationTest.Create(activity);
            var tracker = new StateTracker();
            host1.Extensions.Add(tracker);
            host2.Extensions.Add(tracker);

            try
            {
                // Act / Assert
                host1.TestWorkflowApplication.RunUntilBookmark();
                host2.TestWorkflowApplication.RunUntilBookmark();
                host1.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1.ToString(), 1);
                host2.TestWorkflowApplication.ResumeUntilBookmark(StateTrigger.T1.ToString(), 1);
                Assert.AreEqual(2, tracker.StateMachines.Count);
            }
            catch (Exception exception)
            {
                exception.Trace();
                throw;
            }
            finally
            {
                tracker.Trace();
                WorkflowTrace.Information("Host1");
                host1.Tracking.Trace();
                WorkflowTrace.Information("Host2");
                host2.Tracking.Trace();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a large StateMachine
        /// </summary>
        /// <param name="stateCount">
        /// The state Count. 
        /// </param>
        /// <returns>
        /// A StateMachine 
        /// </returns>
        internal static Activity CreateLargeStateMachine(int stateCount)
        {
            var initialState = new State { DisplayName = "State1", };

            var largeStateMachine = new StateMachine { InitialState = initialState, States = { initialState } };

            var prevState = initialState;
            for (var i = 1; i < stateCount; i++)
            {
                var state = new State { DisplayName = string.Format("State{0}", i + 1), IsFinal = i == stateCount - 1 };
                largeStateMachine.States.Add(state);
                prevState.Transitions.Add(new Transition { DisplayName = string.Format("T{0}", i), To = state });
                prevState = state;
            }

            return largeStateMachine;
        }

        /// <summary>
        ///   Creates a StateMachine that will fault
        /// </summary>
        /// <returns> A StateMachine </returns>
        internal static Activity CreateStateMachineThatFaults()
        {
            var finalState = new State { DisplayName = "FinalState", IsFinal = true };
            var initialState = new State
                {
                    DisplayName = "Initial State", 
                    Entry = new Throw { Exception = new InArgument<Exception>(ctx => new InvalidOperationException()) }, 
                    Transitions = {
                                     new Transition { To = finalState } 
                                  }
                };
            return new StateMachine { InitialState = initialState, States = { initialState, finalState } };
        }

        /// <summary>
        /// Asserts that two state machine infos are equivalent by matching fields
        /// </summary>
        /// <param name="info1">
        /// The first info 
        /// </param>
        /// <param name="info2">
        /// The second info 
        /// </param>
        private static void AssertEquivalent(IStateMachineInfo info1, IStateMachineInfo info2)
        {
            Assert.AreEqual(info1.CurrentState, info2.CurrentState);
            Assert.AreEqual(info1.InstanceId, info2.InstanceId);
            Assert.AreEqual(info1.InstanceState, info2.InstanceState);
            Assert.AreEqual(info1.Name, info2.Name);
            Assert.AreEqual(info1.PreviousState, info2.PreviousState);
            AssertHelper.AreEqual(info1.PossibleTransitions, info2.PossibleTransitions);
            AssertHelper.AreEqual(info1.StateHistory, info2.StateHistory);
        }

        #endregion
    }
}