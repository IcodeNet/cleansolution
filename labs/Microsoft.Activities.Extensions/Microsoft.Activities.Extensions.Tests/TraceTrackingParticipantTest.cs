// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceTrackingParticipantTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This is a test class for TraceTrackingParticipantTest and is intended
    ///   to contain all TraceTrackingParticipantTest Unit Tests
    /// </summary>
    [TestClass]
    public class TraceTrackingParticipantTest
    {
        #region Public Properties

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Given
        /// * A TraceTrackingParticipant
        /// When
        /// * The TraceTrackingParticipant is added to the Workflow extensions 
        /// * And the workflow is run
        /// Then
        /// * The TraceTrackingParticipant will emit records that can be picked up by a trace listener
        /// </summary>
        [TestMethod]
        public void TraceTrackingParticipantShouldTrace()
        {
            // Arrange
            var target = new TraceTrackingParticipant();
            var host = new WorkflowApplication(new Sequence());
            host.Extensions.Add(target);
            var memoryListener = new MemoryListener();
            Trace.Listeners.Add(memoryListener);

            // Act
            host.RunEpisode();

            Assert.IsTrue(memoryListener.Records.Count > 0);
        }

        /// <summary>
        /// Given
        /// * A TraceTrackingParticipant
        /// When
        /// * The TraceTrackingParticipant is added to the Workflow extensions 
        /// * And the workflow is run
        /// Then
        /// * The TraceTrackingParticipant will emit records that can be picked up by a trace listener
        /// </summary>
        [TestMethod]
        public void TraceTrackingParticipantShouldTraceOptions()
        {
            // Arrange
            var memoryListener = new MemoryListener();
            Trace.Listeners.Add(memoryListener);
            var target = new TraceTrackingParticipant(TrackingOption.All);
            var host = WorkflowApplicationTest.Create(new Sequence());
            host.Extensions.Add(target);

            // Act
            try
            {
                host.TestWorkflowApplication.RunEpisode(Constants.Timeout);

                Assert.AreEqual(5, memoryListener.Records.Count);
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}