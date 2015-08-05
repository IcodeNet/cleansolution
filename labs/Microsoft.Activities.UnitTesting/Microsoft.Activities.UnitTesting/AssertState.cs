// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertState.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable EmptyNamespace
namespace Microsoft.Activities.UnitTesting
// ReSharper restore EmptyNamespace
{
#if NET401_OR_GREATER
    using System.Activities.Statements;
    using System.Activities.Statements.Tracking;
    using System.Activities.Tracking;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Tracking;

    /// <summary>
    /// The assert state.
    /// </summary>
    public static class AssertState
    {
        #region Public Methods

        /// <summary>
        /// The occurred in order.
        /// </summary>
        /// <param name="stateMachineName">
        /// The name of the State Machine activity
        /// </param>
        /// <param name="trackingRecords">
        /// The tracking records.
        /// </param>
        /// <param name="expectedStates">
        /// The expected states in order.
        /// </param>
        public static void OccursInOrder(string stateMachineName, TrackingRecordsList trackingRecords, params object[] expectedStates)
        {
            // Find all CustomTrackingRecords that indicate a state
            var records = trackingRecords.FindAll<StateMachineStateRecord>(r => r.StateMachineName == stateMachineName).ToList();

            // Verify that there are at least count records
            if (expectedStates.Count() > records.Count())
            {
                throw new WorkflowAssertFailedException(
                    string.Format(
                        "Expected collection of states is larger than actual states found collection, expected <{0}> found <{1}>",
                        ListHelper.ToDelimitedList(expectedStates),
                        records.ToDelimitedStateList()));
            }
            

            // Verify that the states occurred in the expected order
            for (var i = 0; i < expectedStates.Count(); i++)
            {
                if (!expectedStates[i].ToString().Equals(records.ElementAt(i).StateName))
                {
                    throw new WorkflowAssertFailedException(
                        string.Format("Expected state <{0}> Actual state <{1}> found at index {2} expected states <{3}> found states <{4}>", 
                        expectedStates[i],
                        ((StateMachineStateRecord)records.ElementAt(i)).StateName,
                        i, 
                        ListHelper.ToDelimitedList(expectedStates), 
                        records.ToDelimitedStateList()));
                }
            }
        }

        #endregion
    }

#endif
}