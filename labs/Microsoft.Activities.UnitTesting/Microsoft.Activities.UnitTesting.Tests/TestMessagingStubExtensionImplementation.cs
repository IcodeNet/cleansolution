// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestMessagingStubExtensionImplementation.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;

    using Microsoft.Activities.UnitTesting.Stubs;

    /// <summary>
    /// The test receive stub.
    /// </summary>
    public class TestMessagingStubExtensionImplementation : MessagingStubExtension
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether Operation1Invoked.
        /// </summary>
        public bool Operation1Invoked { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether Operation2Invoked.
        /// </summary>
        public bool Operation2Invoked { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The get implementation.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// The implementation
        /// </returns>
        public override Action GetImplementation(Activity activity)
        {
            var receive = activity as ReceiveStub;
            if (receive != null && receive.OperationName == "Operation1")
            {
                return () => this.Operation1Invoked = true;
            }

            if (receive != null && receive.OperationName == "Operation2")
            {
                return () => this.Operation2Invoked = true;
            }

            return null;
        }

        #endregion
    }
}