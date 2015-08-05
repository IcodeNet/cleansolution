// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryTrackingParticipantTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Diagnostics;
    using System.Threading;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for MemoryTrackingParticipantTest and is intended
    ///  to contain all MemoryTrackingParticipantTest Unit Tests
    /// </summary>
    [TestClass]
    public class MemoryTrackingParticipantTest
    {
        #region Constants and Fields

        /// <summary>
        /// The default timeout.
        /// </summary>
        public readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(100);

        #endregion

        #region Public Methods

        /// <summary>
        /// The assert will return an assert host tracking.
        /// </summary>
        [TestMethod]
        public void AssertWillReturnAnAssertHostTracking()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowApplication(activity);
            var target = new MemoryTrackingParticipant();
            host.Extensions.Add(target);

            host.RunEpisode();

            Assert.IsNotNull(target.Assert);
            Assert.IsInstanceOfType(target.Assert, typeof(MemoryTrackingParticipant.AssertHostTracking));
        }

        /// <summary>
        /// Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ClearWillClearRecords()
        {
            // Arrange
            var host = new WorkflowInvokerTest(CreateTestActivity());

            // Act
            host.TestActivity();

            host.Tracking.Clear();

            // Assert
            Assert.AreEqual(0, host.Tracking.Records.Count);
        }

        /// <summary>
        /// Given
        ///   * An activity
        ///   * A host with a MemoryTrackingParticipant added to the extensions
        ///   * An OnTrack Delegate is provided
        ///   When
        ///   * The activity is run
        ///   Then
        ///   * The OnTrack delegate is invoked
        /// </summary>
        [TestMethod]
        public void OnTrackIsCalledWhenAdded()
        {
            var trackInvoked = false;
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            host.Tracking.OnTrack = (r, t) => { trackInvoked = true; };

            host.TestActivity();
            Assert.IsTrue(trackInvoked);
        }

        /// <summary>
        /// The records will return tracking record list.
        /// </summary>
        [TestMethod]
        public void RecordsWillReturnTrackingRecordList()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowApplication(activity);
            var target = new MemoryTrackingParticipant();
            host.Extensions.Add(target);

            host.RunEpisode();

            Assert.IsNotNull(target.Records);
            Assert.IsInstanceOfType(target.Records, typeof(TrackingRecordsList));
        }

        /// <summary>
        /// The trace will trace records.
        /// </summary>
        [TestMethod]
        public void TraceWillTraceRecords()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowApplication(activity);
            var target = new MemoryTrackingParticipant();
            host.Extensions.Add(target);
            var listener = new TestTraceListener();

            Trace.Listeners.Add(listener);

            // Act
            host.RunEpisode();
            target.Trace();

            // Assert
            Assert.IsTrue(listener.WriteLineCount > 0);
        }

        /// <summary>
        /// The wait for workflow instance record will signal wait handle when run.
        /// </summary>
        [TestMethod]
        public void WaitForWorkflowInstanceRecordWillSignalWaitHandleWhenRun()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowApplication(activity);
            var target = new MemoryTrackingParticipant();
            host.Extensions.Add(target);
            var completed = new AutoResetEvent(false);

            // Add a handle to wait for the completed event
            target.WaitForWorkflowInstanceRecord(WorkflowInstanceRecordState.Completed, completed);

            host.Run();

            // Should signal 
            Assert.IsTrue(completed.WaitOne(this.DefaultTimeout));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a test activity
        /// </summary>
        /// <returns>
        /// The activity
        /// </returns>
        private static Activity CreateTestActivity()
        {
            return new Sequence
                {
                    Activities =
                        {
                            new WriteLine { DisplayName = "WriteLine1", Text = "WriteLine1" }, 
                            new WriteLine { DisplayName = "WriteLine2", Text = "WriteLine2" }, 
                            new WriteLine { DisplayName = "WriteLine3", Text = "WriteLine3" }, 
                            new WriteLine { DisplayName = "Don't Find Me", Text = "Don't Find Me" }, 
                            new WriteLine { DisplayName = "TwoOfThese", Text = "TwoOfThese" }, 
                            new WriteLine { DisplayName = "TwoOfThese", Text = "TwoOfThese" }, 
                        }
                };
        }

        #endregion

        /// <summary>
        /// The test trace listener.
        /// </summary>
        internal class TestTraceListener : TraceListener
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets WriteCount.
            /// </summary>
            public int WriteCount { get; set; }

            /// <summary>
            /// Gets or sets WriteLineCount.
            /// </summary>
            public int WriteLineCount { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// The write.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public override void Write(string message)
            {
                this.WriteCount++;
            }

            /// <summary>
            /// The write line.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public override void WriteLine(string message)
            {
                this.WriteLineCount++;
            }

            #endregion
        }
    }
}