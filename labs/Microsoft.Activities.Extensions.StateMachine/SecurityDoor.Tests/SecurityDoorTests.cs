// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityDoorTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoor.Tests
{
    using System;

    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SecurityDoorConsole;

    /// <summary>
    /// </summary>
    [TestClass]
    public class SecurityDoorTests
    {
        #region Fields

        /// <summary>
        /// Test timeout
        /// </summary>
        /// <remarks>
        /// Use a longer value when debugging
        /// </remarks>
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(30);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Verify that a valid card key will unlock the door
        /// </summary>
        /// <remarks>
        ///   Given
        ///   - A closed / locked door
        ///   - A valid card key
        ///   When
        ///   - The AuthorizeKey message is sent
        ///   Then
        ///   - The door will be in the Closed / Unlocked State
        /// </remarks>
        [TestMethod]
        public void ValidKeyShouldAuthorize()
        {
            // Arrange
            var securityDoor = new SecurityDoor { Timeout = this.timeout };
            var tracking = new MemoryTrackingParticipant();
            try
            {
                // Act
                // Insert a valid key
                securityDoor.InsertKey(Guid.NewGuid());

                // Assert
                Assert.AreEqual(DoorState.ClosedUnlocked, securityDoor.State);
            }
            finally
            {
                tracking.Trace();
                securityDoor.Trace();
            }
        }

        #endregion
    }
}