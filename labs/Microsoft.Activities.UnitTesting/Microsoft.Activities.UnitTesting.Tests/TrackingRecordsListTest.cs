// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackingRecordsListTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Tracking;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Tests.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   This is a test class for TrackingRecordsListTest and is intended
    ///   to contain all TrackingRecordsListTest Unit Tests
    /// </summary>
    [TestClass]
    public class TrackingRecordsListTest
    {
        #region Constants and Fields

        private const string ManyArgId = "2";

        private const string NoArgId = "9";

        private const int TrackingRecordCount = 29;

        private const string TwoOfTheseId = "9";

        private const string WriteLine1Id = "20";

        private const string WriteLine3Id = "16";

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the test context which provides
        ///   information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void CountPropertyHasNumberOfRecords()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(TrackingRecordCount, host.Tracking.Records.Count);
        }

        /// <summary>
        ///   Verifies that ExistsCount will return false when there is no matching record
        /// </summary>
        [TestMethod]
        public void ExisitsCountWillReturnFalseWhenNoMatchingRecordExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(host.Tracking.Records.ExistsCount("No Record", ActivityInstanceState.Executing, 2));
        }

        /// <summary>
        ///   Verifies that Exists will return false when there is no matching record
        /// </summary>
        [TestMethod]
        public void ExisitsWillReturnFalseWhenNoMatchingRecordExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(host.Tracking.Records.Exists("No Record", ActivityInstanceState.Executing));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ExistsArgValueFindsAMatchingArgument()
        {
            // Arrange
            var host = new WorkflowInvokerTest(CreateTestActivity());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsArgValue("ManyArgs", ActivityInstanceState.Executing, "BoolArg", true));            
            Assert.IsTrue(host.Tracking.Records.ExistsArgValue("ManyArgs", ActivityInstanceState.Executing, "IntArg", 1));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ExistsIdArgValueFindsAMatchingArgument()
        {
            // Arrange
            var host = new WorkflowInvokerTest(CreateTestActivity());

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsIdArgValue("2", ActivityInstanceState.Executing, "BoolArg", true));
            Assert.IsTrue(host.Tracking.Records.ExistsIdArgValue("2", ActivityInstanceState.Executing, "IntArg", 1));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ClearWillClearRecords()
        {
            // Arrange
            var host = new WorkflowInvokerTest(CreateTestActivity());

            // Act
            host.TestActivity();

            host.Tracking.Records.Clear();

            // Assert
            Assert.AreEqual(0, host.Tracking.Records.Count);            
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ExistsArgMatchFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [19] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsTrue(
                host.Tracking.Records.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d"));
            Assert.IsTrue(
                host.Tracking.Records.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0));
        }

        /// <summary>
        ///   Verifies that ExistsAtId will return false when a record is found at the index but the id does not match
        /// </summary>
        [TestMethod]
        public void ExistsAtIdWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(
                host.Tracking.Records.ExistsAtId(host.Tracking.Records.Count() - 2, "-1", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAtId will return false when a record is found at the index but the record type is not ActivityStateRecord
        /// </summary>
        [TestMethod]
        public void ExistsAtIdWillReturnFalseWhenWrongRecordType()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            // Last record is wrong type
            Assert.IsFalse(
                host.Tracking.Records.ExistsAtId(host.Tracking.Records.Count() - 1, "-1", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAtId will return true when a matching record is found at the index
        /// </summary>
        [TestMethod]
        public void ExistsAtIdWillReturnTrueWhenMatchingRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsAtId(5, WriteLine1Id, ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAtId will throw when an invalid Id is used
        /// </summary>
        [TestMethod]
        public void ExistsAtIdWillThrowWhenInvalidId()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.ExistsAtId(1, "", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAtId will throw when an invalid index is used
        /// </summary>
        [TestMethod]
        public void ExistsAtIdWillThrowWhenInvalidIndex()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.ExistsAtId(-1, WriteLine1Id, ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will return false when a record is found at the index but the name does not match
        /// </summary>
        [TestMethod]
        public void ExistsAtWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(
                host.Tracking.Records.ExistsAt(
                    host.Tracking.Records.Count() - 2, "TwoOfThese", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will return false when a record is found at the index but the record type is not ActivityStateRecord
        /// </summary>
        [TestMethod]
        public void ExistsAtWillReturnFalseWhenWrongRecordType()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            // Last record is wrong type
            Assert.IsFalse(
                host.Tracking.Records.ExistsAt(
                    host.Tracking.Records.Count() - 1, "TwoOfThese", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will return true when a matching record is found at the index
        /// </summary>
        [TestMethod]
        public void ExistsAtWillReturnTrueWhenMatchingRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsAt(5, "WriteLine1", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will throw when an invalid DisplayName is used
        /// </summary>
        [TestMethod]
        public void ExistsAtWillThrowWhenInvalidDisplayName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.ExistsAt(1, "", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will throw when an invalid index is used
        /// </summary>
        [TestMethod]
        public void ExistsAtWillThrowWhenInvalidIndex()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.ExistsAt(-1, "WriteLine1", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that Exists will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void ExistsCountPredicateWillReturnFalseWhenCountDoesNotMatch()
        {
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(
                host.Tracking.Records.ExistsCount<ActivityStateRecord>(
                    r => r.Activity.Name == "WriteLine1" && r.GetInstanceState() == ActivityInstanceState.Closed, 2));
        }

        /// <summary>
        ///   Verifies that ExistsCount will return false when there is not a matching record
        /// </summary>
        [TestMethod]
        public void ExistsCountPredicateWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(
                host.Tracking.Records.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == "No Record", 1));
        }

        /// <summary>
        ///   Verifies that Exists will return true when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsCountPredicateWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(
                host.Tracking.Records.ExistsCount<ActivityStateRecord>(
                    r => r.Activity.Name == "TwoOfThese" && r.GetInstanceState() == ActivityInstanceState.Closed, 2));
        }

        /// <summary>
        ///   Verifies that Exists will return true when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsCountPredicateWillThrowWhenInvalidCount()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () =>
                host.Tracking.Records.ExistsCount<ActivityStateRecord>(
                    r => r.Activity.Name == "TwoOfThese" && r.GetInstanceState() == ActivityInstanceState.Closed, -1));
        }

        /// <summary>
        ///   Verifies that ExistsCount will return false when there are no records
        /// </summary>
        [TestMethod]
        public void ExistsCountWillReturnFalseWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.IsFalse(host.Tracking.Records.ExistsCount("No Record", ActivityInstanceState.Executing, 2));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void ExistsCountWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(host.Tracking.Records.ExistsCount("WriteLine1", ActivityInstanceState.Closed, 2));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return true when a matching records
        /// </summary>
        [TestMethod]
        public void ExistsCountWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsCount("TwoOfThese", ActivityInstanceState.Closed, 2));
        }

        /// <summary>
        ///   Verifies that ExistsCount will throw when using an invalid count
        /// </summary>
        [TestMethod]
        public void ExistsCountWillThrowWhenInvalidCount()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.ExistsCount("TwoOfThese", ActivityInstanceState.Closed, -1));
        }

        /// <summary>
        ///   Verifies that you can Exists an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void ExistsIdArgMatchExistssAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // 4: Activity [19] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsTrue(
                host.Tracking.Records.ExistsIdArgMatch(
                    WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine\\d"));
            Assert.IsTrue(
                host.Tracking.Records.ExistsIdArgMatch(
                    WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0));
        }

        /// <summary>
        ///   Verifies that ExistsIdCount will return false when there are no records
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillReturnFalseWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.IsFalse(host.Tracking.Records.ExistsIdCount("No Record", ActivityInstanceState.Executing, 2));
        }

        /// <summary>
        ///   Verifies that ExistsIdCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(host.Tracking.Records.ExistsIdCount("WriteLine1", ActivityInstanceState.Closed, 2));
        }

        /// <summary>
        ///   Verifies that ExistsIdCount with name and state will return true when a matching records
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsIdCount(TwoOfTheseId, ActivityInstanceState.Closed, 1));
        }

        /// <summary>
        ///   Verifies that ExistsIdCount will throw when using an invalid count
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillThrowWhenInvalidCount()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.ExistsIdCount(TwoOfTheseId, ActivityInstanceState.Closed, -1));
        }

        /// <summary>
        ///   Verifies that ExistsId with name and state will return true when a matching records
        /// </summary>
        [TestMethod]
        public void ExistsIdWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.ExistsId(TwoOfTheseId, ActivityInstanceState.Closed));
            Assert.IsTrue(host.Tracking.Records.ExistsId(TwoOfTheseId, ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that ExistsId with name and state will return true when a matching records
        /// </summary>
        [TestMethod]
        public void ExistsIdWillThrowWhenInvalidId()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.ExistsId("", ActivityInstanceState.Closed));
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.ExistsId("", ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that Exists will return false when there are no records
        /// </summary>
        [TestMethod]
        public void ExistsPredicateWillReturnFalseWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.IsFalse(host.Tracking.Records.Exists<ActivityStateRecord>(r => false));
        }

        /// <summary>
        ///   Verifies that Exists will return false when there is not a matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateWillReturnFalseWhenNoMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsFalse(host.Tracking.Records.Exists<ActivityStateRecord>(r => r.Activity.Name == "No Record"));
        }

        /// <summary>
        ///   Verifies that Exists will return true when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.Exists<ActivityStateRecord>(r => r.Activity.Name == "WriteLine2"));
        }

        /// <summary>
        ///   Verifies that Exists will return false when there are no records
        /// </summary>
        [TestMethod]
        public void ExistsWillReturnFalseWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.IsFalse(host.Tracking.Records.Exists("No Record", ActivityInstanceState.Executing));
        }

        /// <summary>
        ///   Verifies that Exists with name and state will return true when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsWillReturnTrueWhenMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsTrue(host.Tracking.Records.Exists("WriteLine2", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies Exists(WorkflowInstanceRecordState instanceState, long startIndex) will find a matching record
        /// </summary>
        [TestMethod]
        public void ExistsWorkflowInstanceFindsMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 0: WorkflowInstance "Sequence" is Started at 04:18:41.3324
            // 28: WorkflowInstance "Sequence" is Completed at 04:18:41.3394
            Assert.IsTrue(host.Tracking.Records.Exists(WorkflowInstanceRecordState.Started));

            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started);

            // Should not find after index 0
            Assert.IsFalse(host.Tracking.Records.Exists(WorkflowInstanceRecordState.Started, 5));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started, 5));

            // No index
            Assert.IsTrue(host.Tracking.Records.Exists(WorkflowInstanceRecordState.Completed));

            // Again with index
            Assert.IsTrue(host.Tracking.Records.Exists(WorkflowInstanceRecordState.Completed, 20));

            // Verify that the workflow did not go idle
            Assert.IsFalse(host.Tracking.Records.Exists(WorkflowInstanceRecordState.Idle));
        }

        /// <summary>
        ///   Verifies that FindAll will find no matching records
        /// </summary>
        [TestMethod]
        public void FindAllPredicateWillReturnEmptyCollectionWhenNoMatchingRecords()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(0, host.Tracking.Records.FindAll<ActivityStateRecord>(r => false).Count());
        }

        /// <summary>
        ///   Verifies that FindAll will find all matching records
        /// </summary>
        [TestMethod]
        public void FindAllPredicateWillReturnThreeRecordsWhenThreeExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Find all records that start with WriteLine and are in the Closed state
            var records =
                host.Tracking.Records.FindAll<ActivityStateRecord>(
                    r => r.Activity.Name.StartsWith("WriteLine") && r.GetInstanceState() == ActivityInstanceState.Closed);

            // Assert
            Assert.IsNotNull(records, "Did not find records");
            Assert.AreEqual(3, records.Count());
        }

        /// <summary>
        ///   Verifies that FindAll will find no matching records
        /// </summary>
        [TestMethod]
        public void FindAllWillReturnEmptyCollectionWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.AreEqual(0, host.Tracking.Records.FindAll<ActivityStateRecord>(r => false).Count());
        }

        /// <summary>
        ///   Verifies that FindAll will find all matching records
        /// </summary>
        [TestMethod]
        public void FindAllWillReturnTwoRecordsWhenTwoExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Find all records that start with WriteLine and are in the Closed state
            var records = host.Tracking.Records.FindAll("TwoOfThese", ActivityInstanceState.Closed);

            // Assert
            Assert.IsNotNull(records, "Did not find records");
            Assert.AreEqual(2, records.Count());
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgMatchFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [19] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindArgMatch(
                "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d");
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual("WriteLine1", found.GetArgument<string>("Text"));

            found = host.Tracking.Records.FindArgMatch(
                "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0);
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual("WriteLine1", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that an invalid display name will throw
        /// </summary>
        [TestMethod]
        public void FindArgMatchWillThrowOnInvalidArgs()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Invalid Display Name
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindArgMatch(
                    string.Empty, ActivityInstanceState.Executing, "Text", "WriteLine\\d"));
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindArgMatch(
                    string.Empty, ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0));

            // Invalid Argument Name
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindArgMatch("WriteLine1", ActivityInstanceState.Executing, "", "WriteLine\\d"));
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindArgMatch("WriteLine1", ActivityInstanceState.Executing, "", "WriteLine\\d", 0));

            // Invalid Pattern
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.FindArgMatch("WriteLine1", ActivityInstanceState.Executing, "Text", ""));
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.FindArgMatch("WriteLine1", ActivityInstanceState.Executing, "Text", "", 0));

            // Invalid Start Index
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () =>
                host.Tracking.Records.FindArgMatch(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d", -1));
        }

        /// <summary>
        ///   Verifies that if the record you are looking for is after the start index
        ///   No record will be found
        /// </summary>
        [TestMethod]
        public void FindArgMatchWithStartIndexDoesNotFindEarlierRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following starting at Index 20
            // 11: Activity [8] "WriteLine3" is Closed at 02:45:04.9032
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindArgMatch(
                    WriteLine3Id, ActivityInstanceState.Closed, "Text", "WriteLine\\d", 20));
        }

        /// <summary>
        ///   Verifies that FindArgValue will throw an exception when the ArgumentName is whitespace
        /// </summary>
        [TestMethod]
        public void FindArgValueBadArgNameShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindArgValue(
                    "WriteLine3", ActivityInstanceState.Executing, "    ", "WriteLine3", 1));
        }

        /// <summary>
        ///   Verifies that FindArgValue will throw an exception when the id is null
        /// </summary>
        [TestMethod]
        public void FindArgValueBadIdShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.FindArgValue(null, ActivityInstanceState.Executing, "Text", "WriteLine3", 1));
        }

        /// <summary>
        ///   Verifies that FindArgValue will throw an exception on an index less than zero
        /// </summary>
        [TestMethod]
        public void FindArgValueBadIndexShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () =>
                host.Tracking.Records.FindArgValue(
                    "WriteLine3", ActivityInstanceState.Executing, "Text", "WriteLine3", -1));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueDoesNotFindANonMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindArgValue("WriteLine1", ActivityInstanceState.Executing, "Text", "Bad Value"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindArgValue(
                "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine1");
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual("WriteLine1", found.GetArgument<string>("Text"));
        }


        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void GetEnumeratorWillCopyRecords()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Forces a call to GetEnumerator
            foreach (var rec in host.Tracking.Records)
            {
                Assert.IsNotNull(rec);
            }
        }


        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueDoesNotFindAnArgumentThatDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsNull(host.Tracking.Records.FindArgValue("WriteLine1", ActivityInstanceState.Executing, "Does Not Exist", "WriteLine1"));
            Assert.IsNull(host.Tracking.Records.FindArgValue("WriteLine1", ActivityInstanceState.Executing, "Does Not Exist", "WriteLine1", 0));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a null argument value
        /// </summary>
        [TestMethod]
        public void FindArgValueFindsANullArgumentValue()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following with a null value for the Text argument
            // 22: Activity [2] "NoArg" is Executing at 03:15:34.1018
            // {
            //     Arguments
            //         Text: 
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindArgValue<string>(
                "NoArg", ActivityInstanceState.Executing, "Text", null);
            Assert.IsNotNull(found);
            Assert.AreEqual("NoArg", found.Activity.Name);
            Assert.IsNull(found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a whitespace argument value
        /// </summary>
        [TestMethod]
        public void FindArgValueFindsAWhitespaceArgumentValue()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            // Find the following with a null value for the Text argument
            // 22: Activity [8] "NoArg" is Executing at 03:15:34.1018
            // {
            //     Arguments
            //         Text: 
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindArgValue("NoArg", ActivityInstanceState.Executing, "Text", "   ");
            Assert.IsNotNull(found);
            Assert.AreEqual("NoArg", found.Activity.Name);
            Assert.IsNull(found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueFindsManyArguments()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 26: Activity [2] "ManyArgs" is Closed at 03:59:21.6831
            // {
            //     Arguments
            //         DataResponseArg: DataResponse (Key = 1, Text = Text)
            //         IntArg: 1
            //         StringArg: StringArg Value
            //         TimeSpanArg: 01:03:00
            //         DateTimeArg: 7/1/2011 9:00:00 AM
            // }                
            // Assert that you can find each of these by argument value
            Assert.IsNotNull(
                host.Tracking.Records.FindArgValue(
                    "ManyArgs",
                    ActivityInstanceState.Executing,
                    "DataResponseArg",
                    new DataResponse { Key = 1, Text = "Text" }));
            Assert.IsNotNull(
                host.Tracking.Records.FindArgValue("ManyArgs", ActivityInstanceState.Executing, "IntArg", 1));
            Assert.IsNotNull(
                host.Tracking.Records.FindArgValue(
                    "ManyArgs", ActivityInstanceState.Executing, "StringArg", "StringArg Value"));
            Assert.IsNotNull(
                host.Tracking.Records.FindArgValue(
                    "ManyArgs", ActivityInstanceState.Executing, "TimeSpanArg", TimeSpan.FromMinutes(63)));
            Assert.IsNotNull(
                host.Tracking.Records.FindArgValue(
                    "ManyArgs", ActivityInstanceState.Executing, "DateTimeArg", new DateTime(2011, 7, 1, 9, 0, 0)));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueIndexDoesNotFindANonMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindArgValue(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "Bad Value", 3));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindArgValueIndexFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            // Find the following
            // 10: Activity [8] "WriteLine3" is Executing at 03:00:11.2150
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindArgValue(
                "WriteLine3", ActivityInstanceState.Executing, "Text", "WriteLine3", 8);
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine3", found.Activity.Name);
            Assert.AreEqual("WriteLine3", found.Activity.Name);
            Assert.AreEqual("WriteLine3", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgMatchFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert
            // Find the following
            // 4: Activity [19] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgMatch(
                WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine\\d");
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual(WriteLine1Id, found.Activity.Id);
            Assert.AreEqual("WriteLine1", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that if the record you are looking for is after the start index
        ///   No record will be found
        /// </summary>
        [TestMethod]
        public void FindIdArgMatchWithStartIndexDoesNotFindEarlierRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following starting at Index 20
            // 11: Activity [8] "WriteLine3" is Closed at 02:45:04.9032
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindIdArgMatch(
                    WriteLine3Id, ActivityInstanceState.Closed, "Text", "WriteLine\\d", 20));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgMatchWithStartIndexFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following starting at Index 10
            // 11: Activity [15] "WriteLine3" is Closed at 02:45:04.9032
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgMatch(
                WriteLine3Id, ActivityInstanceState.Closed, "Text", "WriteLine\\d", 10);
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine3", found.Activity.Name);
            Assert.AreEqual(WriteLine3Id, found.Activity.Id);
            Assert.AreEqual("WriteLine3", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that FindIdArgValue will throw an exception when the ArgumentName is whitespace
        /// </summary>
        [TestMethod]
        public void FindIdArgValueBadArgNameShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindIdArgValue(
                    WriteLine3Id, ActivityInstanceState.Executing, "    ", "WriteLine3", 1));
        }

        /// <summary>
        ///   Verifies that FindIdArgValue will throw an exception when the id is null
        /// </summary>
        [TestMethod]
        public void FindIdArgValueBadIdShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(
                () =>
                host.Tracking.Records.FindIdArgValue(null, ActivityInstanceState.Executing, "Text", "WriteLine3", 1));
        }

        /// <summary>
        ///   Verifies that FindIdArgValue will throw an exception on an index less than zero
        /// </summary>
        [TestMethod]
        public void FindIdArgValueBadIndexShouldThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () =>
                host.Tracking.Records.FindIdArgValue(
                    WriteLine3Id, ActivityInstanceState.Executing, "Text", "WriteLine3", -1));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgValueDoesNotFindANonMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindIdArgValue(WriteLine1Id, ActivityInstanceState.Executing, "Text", "Bad Value"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgValueFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgValue(
                WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine1");
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine1", found.Activity.Name);
            Assert.AreEqual(WriteLine1Id, found.Activity.Id);
            Assert.AreEqual("WriteLine1", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a null argument value
        /// </summary>
        [TestMethod]
        public void FindIdArgValueFindsANullArgumentValue()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following with a null value for the Text argument
            // 22: Activity [2] "NoArg" is Executing at 03:15:34.1018
            // {
            //     Arguments
            //         Text: 
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgValue<string>(
                NoArgId, ActivityInstanceState.Executing, "Text", null);
            Assert.IsNotNull(found);
            Assert.AreEqual("NoArg", found.Activity.Name);
            Assert.AreEqual(NoArgId, found.Activity.Id);
            Assert.IsNull(found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a whitespace argument value
        /// </summary>
        [TestMethod]
        public void FindIdArgValueFindsAWhitespaceArgumentValue()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            host.Tracking.Trace();

            // Assert

            // Find the following with a null value for the Text argument
            // 22: Activity [8] "NoArg" is Executing at 03:15:34.1018
            // {
            //     Arguments
            //         Text: 
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgValue(NoArgId, ActivityInstanceState.Executing, "Text", "   ");
            Assert.IsNotNull(found);
            Assert.AreEqual("NoArg", found.Activity.Name);
            Assert.AreEqual(NoArgId, found.Activity.Id);
            Assert.IsNull(found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgValueFindsManyArguments()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 26: Activity [2] "ManyArgs" is Closed at 03:59:21.6831
            // {
            //     Arguments
            //         DataResponseArg: DataResponse (Key = 1, Text = Text)
            //         IntArg: 1
            //         StringArg: StringArg Value
            //         TimeSpanArg: 01:03:00
            //         DateTimeArg: 7/1/2011 9:00:00 AM
            // }                
            // Assert that you can find each of these by argument value
            Assert.IsNotNull(
                host.Tracking.Records.FindIdArgValue(
                    ManyArgId,
                    ActivityInstanceState.Executing,
                    "DataResponseArg",
                    new DataResponse { Key = 1, Text = "Text" }));
            Assert.IsNotNull(
                host.Tracking.Records.FindIdArgValue(ManyArgId, ActivityInstanceState.Executing, "IntArg", 1));
            Assert.IsNotNull(
                host.Tracking.Records.FindIdArgValue(
                    ManyArgId, ActivityInstanceState.Executing, "StringArg", "StringArg Value"));
            Assert.IsNotNull(
                host.Tracking.Records.FindIdArgValue(
                    ManyArgId, ActivityInstanceState.Executing, "TimeSpanArg", TimeSpan.FromMinutes(63)));
            Assert.IsNotNull(
                host.Tracking.Records.FindIdArgValue(
                    ManyArgId, ActivityInstanceState.Executing, "DateTimeArg", new DateTime(2011, 7, 1, 9, 0, 0)));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgValueIndexDoesNotFindANonMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [12] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.IsNull(
                host.Tracking.Records.FindIdArgValue(
                    WriteLine1Id, ActivityInstanceState.Executing, "Text", "Bad Value", 3));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIdArgValueIndexFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            // Find the following
            // 10: Activity [8] "WriteLine3" is Executing at 03:00:11.2150
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            var found = host.Tracking.Records.FindIdArgValue(
                WriteLine3Id, ActivityInstanceState.Executing, "Text", "WriteLine3", 8);
            Assert.IsNotNull(found);
            Assert.AreEqual("WriteLine3", found.Activity.Name);
            Assert.AreEqual(WriteLine3Id, found.Activity.Id);
            Assert.AreEqual("WriteLine3", found.GetArgument<string>("Text"));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIndexArgMatchFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [19] "WriteLine1" is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.AreEqual(
                4,
                host.Tracking.Records.FindIndexArgMatch(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d"));
            Assert.AreEqual(
                4,
                host.Tracking.Records.FindIndexArgMatch(
                    "WriteLine1", ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0));
        }

        /// <summary>
        ///   Verifies that if the record you are looking for is after the start index
        ///   No record will be found
        /// </summary>
        [TestMethod]
        public void FindIndexArgMatchWithStartIndexDoesNotFindEarlierRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following starting at Index 20
            // 11: Activity [8] "WriteLine3" is Closed at 02:45:04.9032
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            Assert.AreEqual(
                -1,
                host.Tracking.Records.FindIndexArgMatch(
                    WriteLine3Id, ActivityInstanceState.Closed, "Text", "WriteLine\\d", 20));
        }

        /// <summary>
        ///   Verifies that you can find an ActivityStateRecord by Id with a matching argument
        /// </summary>
        [TestMethod]
        public void FindIndexIdArgMatchFindsAMatchingArgument()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following
            // 4: Activity [19] WriteLine1Id is Executing at 02:42:24.1300
            // {
            //     Arguments
            //         Text: WriteLine1
            //         TextWriter: 
            // }
            Assert.AreEqual(
                4,
                host.Tracking.Records.FindIndexIdArgMatch(
                    WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine\\d"));
            Assert.AreEqual(
                4,
                host.Tracking.Records.FindIndexIdArgMatch(
                    WriteLine1Id, ActivityInstanceState.Executing, "Text", "WriteLine\\d", 0));
        }

        /// <summary>
        ///   Verifies that if the record you are looking for is after the start index
        ///   No record will be found
        /// </summary>
        [TestMethod]
        public void FindIndexIdArgMatchWithStartIndexDoesNotFindEarlierRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            // Find the following starting at Index 20
            // 11: Activity [8] "WriteLine3" is Closed at 02:45:04.9032
            // {
            //     Arguments
            //         Text: WriteLine3
            //         TextWriter: 
            // }
            Assert.AreEqual(
                -1,
                host.Tracking.Records.FindIndexIdArgMatch(
                    WriteLine3Id, ActivityInstanceState.Closed, "Text", "WriteLine\\d", 20));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void FindIndexIdReturnsIndexWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(5, host.Tracking.Records.FindIndexId(WriteLine1Id, ActivityInstanceState.Closed));
            Assert.AreEqual(5, host.Tracking.Records.FindIndexId(WriteLine1Id, ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that FindIndexId will return -1 when the display name does not exist
        /// </summary>
        [TestMethod]
        public void FindIndexIdReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(-1, host.Tracking.Records.FindIndexId("-1", ActivityInstanceState.Closed));
            Assert.AreEqual(-1, host.Tracking.Records.FindIndexId("-1", ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void FindIndexPredicateReturnsIndexWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(28, host.Tracking.Records.FindIndex<WorkflowInstanceRecord>(rec => rec.RecordNumber > 0));
            Assert.AreEqual(28, host.Tracking.Records.FindIndex<WorkflowInstanceRecord>(rec => rec.RecordNumber > 0, 1));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void FindIndexReturnsIndexWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(5, host.Tracking.Records.FindIndex("WriteLine1", ActivityInstanceState.Closed));
            Assert.AreEqual(5, host.Tracking.Records.FindIndex("WriteLine1", ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that FindIndex will return -1 when the display name does not exist
        /// </summary>
        [TestMethod]
        public void FindIndexReturnsNotFoundWhenNotExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual(-1, host.Tracking.Records.FindIndex("Bad Name", ActivityInstanceState.Closed));
            Assert.AreEqual(-1, host.Tracking.Records.FindIndex("Bad Name", ActivityInstanceState.Closed, 0));
        }

        /// <summary>
        ///   Verifies that Find will find a matching record
        /// </summary>
        [TestMethod]
        public void FindPredicateWillFindWhenMatchingRecordExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            const string expected = "WriteLine2";
            var record = host.Tracking.Records.Find<ActivityStateRecord>(r => r.Activity.Name == expected);

            // Assert
            Assert.IsNotNull(record, "Did not find record");
            Assert.AreEqual(expected, record.Activity.Name);
        }

        /// <summary>
        ///   Verifies that Find will not find a record when the name does not exist
        /// </summary>
        [TestMethod]
        public void FindPredicateWillReturnNullWhenNoMatchingRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.Find<ActivityStateRecord>(r => false));
        }

        /// <summary>
        ///   Verifies that Find will throw with an invalid start index
        /// </summary>
        [TestMethod]
        public void FindPredicateWillThrowWhenStartIndexInvalid()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.Find<ActivityStateRecord>(r => r.Activity.Name == "WriteLine2", -1));
        }























        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillFindScheduledIdParentChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert

            // 3: Activity [1] "1" scheduled child activity [19] WriteLine1Id at 03:16:05.8526
            Assert.IsNotNull(host.Tracking.Records.FindScheduledId("1", WriteLine1Id));
            Assert.IsNotNull(host.Tracking.Records.FindScheduledId("1", WriteLine1Id, 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillNotFindBadParentBadChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.FindScheduledId("-1", "-1"));
            Assert.IsNull(host.Tracking.Records.FindScheduledId("-1", "-1", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillNotFindScheduledIdParentBadChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.FindScheduledId("1", "-1"));
            Assert.IsNull(host.Tracking.Records.FindScheduledId("1", "-1", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillThrowInvalidChildName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduledId("1", ""));
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduledId("1", "", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillThrowInvalidParentName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduledId("", "-1"));
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduledId("", "-1", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledIdWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledIdWillThrowInvalidStartIndex()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.FindScheduledId("1", WriteLine1Id, -1));
        }



























        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillFindScheduledParentChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert

            // 3: Activity [1] "Sequence" scheduled child activity [19] "WriteLine1" at 03:16:05.8526
            Assert.IsNotNull(host.Tracking.Records.FindScheduled("Sequence", "WriteLine1"));
            Assert.IsNotNull(host.Tracking.Records.FindScheduled("Sequence", "WriteLine1", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillNotFindBadParentBadChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.FindScheduled("Bad Name", "Bad Name"));
            Assert.IsNull(host.Tracking.Records.FindScheduled("Bad Name", "Bad Name", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillNotFindScheduledParentBadChild()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.FindScheduled("Sequence", "Bad Name"));
            Assert.IsNull(host.Tracking.Records.FindScheduled("Sequence", "Bad Name", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillThrowInvalidChildName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduled("Sequence", ""));
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduled("Sequence", "", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillThrowInvalidParentName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduled("", "Bad Name"));
            AssertHelper.Throws<ArgumentNullException>(() => host.Tracking.Records.FindScheduled("", "Bad Name", 0));
        }

        /// <summary>
        ///   Verifies that FindScheduledWillFind when the parent and child records exist
        /// </summary>
        [TestMethod]
        public void FindScheduledWillThrowInvalidStartIndex()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<ArgumentOutOfRangeException>(
                () => host.Tracking.Records.FindScheduled("Sequence", "WriteLine1", -1));
        }

        /// <summary>
        ///   Verifies that Find will find a record in the given state
        /// </summary>
        [TestMethod]
        public void FindWillFindWhenMatchingRecordExists()
        {
            // Arrange
            const string expected = "WriteLine3";
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            var record = host.Tracking.Records.Find(expected, ActivityInstanceState.Executing);

            // Assert
            Assert.IsNotNull(record, "Did not find record");
            Assert.AreEqual(expected, record.Activity.Name);
            Assert.AreEqual(ActivityInstanceState.Executing, record.GetInstanceState());
        }

        /// <summary>
        ///   Verifies that Find will not find a record when there are no records
        /// </summary>
        [TestMethod]
        public void FindWillReturnNullWhenEmptyCollection()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            // Do nothing

            // Assert
            Assert.IsNull(host.Tracking.Records.Find<ActivityStateRecord>(r => false));
        }

        /// <summary>
        ///   Verifies that Find will return null when there is no matching record
        /// </summary>
        [TestMethod]
        public void FindWillReturnNullWhenNoMatchingRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.IsNull(host.Tracking.Records.Find("No Record", ActivityInstanceState.Executing));
        }

        /// <summary>
        ///   Verifies that Find will throw with an invalid display name
        /// </summary>
        [TestMethod]
        public void FindWillThrowWhenInvalidDisplayName()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.Find(string.Empty, ActivityInstanceState.Executing));
            AssertHelper.Throws<ArgumentNullException>(
                () => host.Tracking.Records.Find(string.Empty, ActivityInstanceState.Executing, 0));
        }

        /// <summary>
        ///   Verifies that ExistsCount with name and state will return false when the count does not match
        /// </summary>
        [TestMethod]
        public void TrackingIndexerReturnsRecord()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            Assert.AreEqual("WriteLine1", ((ActivityStateRecord)host.Tracking.Records[4]).Activity.Name);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Creates a test activity
        /// </summary>
        /// <returns>
        ///   The activity
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
                            new WriteLine { DisplayName = "NoArg" },
                            new ActivityWithArgs
                                {
                                    DisplayName = "ManyArgs",
                                    BoolArg = true,
                                    IntArg = 1,
                                    DataResponseArg =
                                        new InArgument<DataResponse>(ctx => new DataResponse { Key = 1, Text = "Text" }),
                                    DateTimeArg = new DateTime(2011, 7, 1, 9, 0, 0),
                                    TimeSpanArg = TimeSpan.FromMinutes(63),
                                    StringArg = "StringArg Value"
                                }
                        }
                };
        }

        #endregion
    }
}