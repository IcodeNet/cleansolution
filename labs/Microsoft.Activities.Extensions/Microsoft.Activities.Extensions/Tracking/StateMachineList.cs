// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineList.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tracking
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The state machine list.
    /// </summary>
    [CollectionDataContract(Name = "StateMachines", ItemName = "StateMachine", Namespace = Constants.Namespace)]
    public class StateMachineList : List<StateMachineInfo>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineList"/> class.
        /// </summary>
        public StateMachineList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineList"/> class.
        /// </summary>
        /// <param name="stateMachines">
        /// The state machines.
        /// </param>
        public StateMachineList(IEnumerable<StateMachineInfo> stateMachines)
            : base(stateMachines)
        {
        }

        #endregion
    }
}