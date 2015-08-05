// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateRecordEnumerableTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Tracking;
    using System.Linq;

    using Microsoft.Activities.Extensions.Linq;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Tests for Microsoft.Activities.Extensions.Linq extensions
    /// </summary>
    [TestClass]
    public class ActivityStateRecordEnumerableTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * The result will be false
        /// </summary>
        [TestMethod]
        public void AnyActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Faulted);

                // Assert
                Assert.IsFalse(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The result will be true
        /// </summary>
        [TestMethod]
        public void AnyActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Closed);

                // Assert
                Assert.IsTrue(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing, a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The result will be true
        /// </summary>
        [TestMethod]
        public void AnyDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsTrue(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The result will be true
        /// </summary>
        [TestMethod]
        public void AnyDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsTrue(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing, a displayName of Assign, Id of 1.9 and a startIndex of 5
        ///   Then
        ///   * The result will return 1 record
        ///   6: Activity [1.9] "Assign" is Executing
        ///   {
        ///   Arguments
        ///   Value: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void AnyDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const string Id = "1.9";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Executing, DisplayName, Id, StartRecordNumber);

                // Assert
                Assert.IsTrue(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * The result will return 0 records
        /// </summary>
        [TestMethod]
        public void AnyDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsFalse(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing and an Id of 1
        ///   Then
        ///   * The result will be true
        /// </summary>
        [TestMethod]
        public void AnyIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var any = tracking.Records.Any(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                Assert.IsTrue(any);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Any is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void AnyNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.Any(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Any is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void AnyThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.Any(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            host.RunEpisode();

            // Act / Assert
            AssertHelper.Throws<InvalidOperationException>(() => tracking.Records.First(ActivityInstanceState.Faulted));
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.First(ActivityInstanceState.Closed);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Closed, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.First(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.First(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.First(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(
                    () =>
                    tracking.Records.First(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.First(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * First is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void FirstNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.First(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            host.RunEpisode();

            // Act
            var record = tracking.Records.FirstOrDefault(ActivityInstanceState.Faulted);

            // Assert
            Assert.IsNull(record);
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(ActivityInstanceState.Closed);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Closed, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNull(record);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.FirstOrDefault(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * FirstOrDefault is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.FirstOrDefault(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * FirstOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstOrDefaultThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.FirstOrDefault(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * First is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void FirstThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.First(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void LastActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            host.RunEpisode();

            // Act / Assert
            AssertHelper.Throws<InvalidOperationException>(() => tracking.Records.Last(ActivityInstanceState.Faulted));
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Last(ActivityInstanceState.Closed);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Closed, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Executing a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Last(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Last(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Last(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void LastDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(
                    () =>
                    tracking.Records.Last(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Last is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void LastNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.Last(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void LastOrDefaultActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            host.RunEpisode();

            // Act
            var record = tracking.Records.LastOrDefault(ActivityInstanceState.Faulted);

            // Assert
            Assert.IsNull(record);
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastOrDefaultActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.LastOrDefault(ActivityInstanceState.Closed);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Closed, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastOrDefaultDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.LastOrDefault(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void LastOrDefaultDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.LastOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void LastOrDefaultDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.LastOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNull(record);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * LastOrDefault is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void LastOrDefaultNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.LastOrDefault(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * LastOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void LastOrDefaultThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.LastOrDefault(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Last is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void LastThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.Last(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Single is invoked with a ActivityInstanceState.Executing a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Single(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Single is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Single(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Single is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Single(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Single is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void SingleDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(
                    () =>
                    tracking.Records.Single(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Single is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.Single(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Single is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void SingleNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.Single(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            host.RunEpisode();

            // Act
            var record = tracking.Records.SingleOrDefault(ActivityInstanceState.Faulted);

            // Assert
            Assert.IsNull(record);
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * An InvalidOperationException is thrown because there is more than one Closed record
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultActivityInstanceStateThrowsOnClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<InvalidOperationException>(
                    () => tracking.Records.SingleOrDefault(ActivityInstanceState.Closed));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of AddToNumOrThrow and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultDisplayNameIdMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.SingleOrDefault(ActivityInstanceState.Executing, DisplayName, Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.SingleOrDefault(ActivityInstanceState.Executing, DisplayName);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.SingleOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * An InvalidOperationException will be thrown
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.SingleOrDefault(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                Assert.IsNull(record);
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing and an Id of 1
        ///   Then
        ///   * The resulting record will not be null
        ///   * The record state will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var record = tracking.Records.SingleOrDefault(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                Assert.IsNotNull(record);
                Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * SingleOrDefault is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.SingleOrDefault(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * SingleOrDefault is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void SingleOrDefaultThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.SingleOrDefault(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing and an ActivityId of 1
        ///   Then
        ///   * The result will return 1 record
        ///   2: Activity [1] "AddToNumOrThrow" is Executing
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   }
        /// </summary>
        [TestMethod]
        public void WhereActivityIdMatch()
        {
            // Arrange
            const string Id = "1";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(ActivityInstanceState.Executing, activityId: Id);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var activityStateRecord = list.FirstOrDefault();
                Assert.IsNotNull(activityStateRecord);
                Assert.AreEqual(Id, activityStateRecord.Activity.Id);
                Assert.AreEqual(ActivityInstanceState.Executing, activityStateRecord.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Faulted
        ///   Then
        ///   * The result will include 0 records
        /// </summary>
        [TestMethod]
        public void WhereActivityInstanceStateDoesNotFindFaulted()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(ActivityInstanceState.Faulted);

                // Assert
                Assert.AreEqual(0, query.Count());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Closed
        ///   Then
        ///   * The result will include 3 records
        ///   * The result will include only records of type ActivityStateRecord 
        ///   * The state of all the records will be ActivityInstanceState.Closed
        /// </summary>
        [TestMethod]
        public void WhereActivityInstanceStateFindsClosed()
        {
            // Arrange
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(ActivityInstanceState.Closed);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(4, list.Count());
                Assert.IsFalse(
                    list.Any(record => record != null && record.GetInstanceState() != ActivityInstanceState.Closed), 
                    "Found record where the state is not closed");
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, a displayName of Assign, Id of 1.9 and a startIndex of 5
        ///   Then
        ///   * The result will return 1 record
        ///   6: Activity [1.9] "Assign" is Executing
        ///   {
        ///   Arguments
        ///   Value: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WhereDisplayNameIdAndStartRecordNumber()
        {
            // Arrange
            const string Id = "1.9";
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(ActivityInstanceState.Executing, DisplayName, Id, StartRecordNumber);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var activityStateRecord = list.FirstOrDefault();
                Assert.IsNotNull(activityStateRecord);
                Assert.AreEqual(DisplayName, activityStateRecord.Activity.Name);
                Assert.AreEqual(Id, activityStateRecord.Activity.Id);
                Assert.AreEqual(ActivityInstanceState.Executing, activityStateRecord.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing and a displayName of AddToNumOrThrow
        ///   Then
        ///   * The result will return 1 record
        ///   2: Activity [1] "AddToNumOrThrow" is Executing
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   }
        /// </summary>
        [TestMethod]
        public void WhereDisplayNameMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(ActivityInstanceState.Executing, DisplayName);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var activityStateRecord = list.FirstOrDefault();
                Assert.IsNotNull(activityStateRecord);
                Assert.AreEqual(DisplayName, activityStateRecord.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Executing, activityStateRecord.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 5
        ///   Then
        ///   * The result will return 1 record
        ///   6: Activity [1.9] "Assign" is Executing
        ///   {
        ///   Arguments
        ///   Value: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WhereDisplayNameMatchWithStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var activityStateRecord = list.FirstOrDefault();
                Assert.IsNotNull(activityStateRecord);
                Assert.AreEqual(DisplayName, activityStateRecord.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Executing, activityStateRecord.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * The result will return 0 records because Assign occurs before record number 8
        /// </summary>
        [TestMethod]
        public void WhereDisplayNameNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(
                    ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(0, list.Count());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, Id of 1.9 and a startIndex of 5
        ///   Then
        ///   * The result will return 1 record
        ///   6: Activity [1.9] "Assign" is Executing
        ///   {
        ///   Arguments
        ///   Value: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WhereIdAndStartRecordNumber()
        {
            // Arrange
            const string Id = "1.9";
            const long StartRecordNumber = 5;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(
                    ActivityInstanceState.Executing, activityId: Id, startRecordNumber: StartRecordNumber);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var activityStateRecord = list.FirstOrDefault();
                Assert.IsNotNull(activityStateRecord);
                Assert.AreEqual(Id, activityStateRecord.Activity.Id);
                Assert.AreEqual(ActivityInstanceState.Executing, activityStateRecord.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of 8
        ///   Then
        ///   * The result will return 0 records
        /// </summary>
        [TestMethod]
        public void WhereIdNoMatchPastStartRecordNumber()
        {
            // Arrange
            const string Id = "Assign";
            const long StartRecordNumber = 9;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.Where(
                    ActivityInstanceState.Executing, activityId: Id, startRecordNumber: StartRecordNumber);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(0, list.Count());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Where is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void WhereNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(
                () => ActivityStateRecordEnumerable.Where(null, ActivityInstanceState.Closed));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * Where is invoked with a ActivityInstanceState.Executing, a displayName of Assign and a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void WhereThrowsNegStartRecordNumber()
        {
            // Arrange
            const string DisplayName = "Assign";
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.Where(
                        ActivityInstanceState.Executing, DisplayName, startRecordNumber: StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithArgument is invoked with an argument name of "Result"
        ///   Then
        ///   * The result will return 1 record
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithArgumentMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                // Find all the ActivityStateRecords that have an argument named Result
                var query = tracking.Records.OfType<ActivityStateRecord>().WithArgument("Result");

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Closed, first.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Any is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void WithArgumentNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(() => ActivityStateRecordEnumerable.WithArgument(null, "BAD"));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * WithArgument is invoked with a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void WithArgumentThrowsNegStartRecordNumber()
        {
            // Arrange
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.OfType<ActivityStateRecord>().WithArgument(
                        "BAD", StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Any is invoked with a null source
        ///   Then
        ///   * An ArgumentNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void WithArgumentValNullSourceShouldThrowArgumentNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(() => ActivityStateRecordEnumerable.WithArgument(null, "BAD", 1));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithArgument is invoked with an argument name of "Num" and a value of 2
        ///   Then
        ///   * The result will return 2 records
        ///   2: Activity [1] "AddToNumOrThrow" is Executing
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   }
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithArgumentValueMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.OfType<ActivityStateRecord>().WithArgument("Num", 2);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(2, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Executing, first.GetInstanceState());
                var last = list.Last();
                Assert.IsNotNull(last);
                Assert.AreEqual(DisplayName, last.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Closed, last.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithArgument is invoked with an argument name of "Num", a value of 2 and starting record of 7
        ///   Then
        ///   * The result will return 1 records
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Arguments
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithArgumentValueStartMatch()
        {
            // Arrange
            const string DisplayName = "AddToNumOrThrow";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                // Find all ActivityStateRecords that have an argument named "Num" starting with record 7
                var query = tracking.Records.OfType<ActivityStateRecord>().WithArgument("Num", 2, 7);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Closed, first.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * WithArgument{T} is invoked with a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void WithArgumentValueThrowsNegStartRecordNumber()
        {
            // Arrange
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.OfType<ActivityStateRecord>().WithArgument(
                        "BAD", 1, StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithVariable is invoked with an variable name of "Result"
        ///   Then
        ///   * The result will return 1 record
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Variables
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithVariableMatch()
        {
            // Arrange
            const string DisplayName = "Sequence";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                // Find all the ActivityStateRecords that have an variable named Result
                var query = tracking.Records.OfType<ActivityStateRecord>().WithVariable("VarStr");

                // Assert
                var list = query.ToList();
                Assert.AreEqual(2, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Executing, first.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Any is invoked with a null source
        ///   Then
        ///   * An VariableNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void WithVariableNullSourceShouldThrowVariableNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(() => ActivityStateRecordEnumerable.WithVariable(null, "BAD"));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * WithVariable is invoked with a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void WithVariableThrowsNegStartRecordNumber()
        {
            // Arrange
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () =>
                    tracking.Records.OfType<ActivityStateRecord>().WithVariable(
                        "BAD", StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * Any is invoked with a null source
        ///   Then
        ///   * An VariableNull exception is thrown.
        /// </summary>
        [TestMethod]
        public void WithVariableValNullSourceShouldThrowVariableNull()
        {
            // Arrange

            // Act
            AssertHelper.Throws<ArgumentNullException>(() => ActivityStateRecordEnumerable.WithVariable(null, "BAD", 1));

            // Assert
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithVariable is invoked with an variable name of "Num" and a value of 2
        ///   Then
        ///   * The result will return 2 records
        ///   2: Activity [1] "AddToNumOrThrow" is Executing
        ///   {
        ///   Variables
        ///   Num: 2
        ///   }
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Variables
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithVariableValueMatch()
        {
            // Arrange
            const string DisplayName = "Sequence";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                var query = tracking.Records.OfType<ActivityStateRecord>().WithVariable("VarNum", 1);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(2, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Executing, first.GetInstanceState());
                var last = list.Last();
                Assert.IsNotNull(last);
                Assert.AreEqual(DisplayName, last.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Closed, last.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}.OfType{ActivityStateRecord}
        ///   When
        ///   * WithVariable is invoked with an variable name of "Num", a value of 2 and starting record of 7
        ///   Then
        ///   * The result will return 1 records
        ///   9: Activity [1] "AddToNumOrThrow" is Closed
        ///   {
        ///   Variables
        ///   Num: 2
        ///   Result: 3
        ///   }
        /// </summary>
        [TestMethod]
        public void WithVariableValueStartMatch()
        {
            // Arrange
            const string DisplayName = "Sequence";
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act
                // Find all ActivityStateRecords that have an variable named "Num" starting with record 7
                var query = tracking.Records.OfType<ActivityStateRecord>().WithVariable("VarNum", 1, 7);

                // Assert
                var list = query.ToList();
                Assert.AreEqual(1, list.Count());
                var first = list.First();
                Assert.IsNotNull(first);
                Assert.AreEqual(DisplayName, first.Activity.Name);
                Assert.AreEqual(ActivityInstanceState.Closed, first.GetInstanceState());
            }
            finally
            {
                tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * An IEnumerable{TrackingRecord}
        ///   When
        ///   * WithVariable{T} is invoked with a startIndex of -1
        ///   Then
        ///   * an ArgumentOutOfRangeException will be thrown
        /// </summary>
        [TestMethod]
        public void WithVariableValueThrowsNegStartRecordNumber()
        {
            // Arrange
            const long StartRecordNumber = -1;
            var activity = new AddToNumOrThrow();
            dynamic args = new WorkflowArguments();
            args.Num = 2;
            var host = new WorkflowApplication(activity, args);
            var tracking = new ListTrackingParticipant();
            host.Extensions.Add(tracking);

            try
            {
                host.RunEpisode();

                // Act / Assert
                AssertHelper.Throws<ArgumentOutOfRangeException>(
                    () => tracking.Records.OfType<ActivityStateRecord>().WithVariable("BAD", 1, StartRecordNumber));
            }
            finally
            {
                tracking.Trace();
            }
        }

        #endregion
    }
}