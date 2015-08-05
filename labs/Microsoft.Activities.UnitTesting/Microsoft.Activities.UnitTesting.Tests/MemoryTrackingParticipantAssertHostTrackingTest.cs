namespace Microsoft.Activities.UnitTesting.Tests
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Activities.Tracking;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    ///<summary>
    ///  This is a test class for MemoryTrackingParticipant_AssertHostTrackingTest and is intended
    ///  to contain all MemoryTrackingParticipant_AssertHostTrackingTest Unit Tests
    ///</summary>
    [TestClass]
    public class MemoryTrackingParticipantAssertHostTrackingTest
    {
        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///   Verifies that ExistsAt will throw when it finds an ActivityStateRecord with the wrong name
        /// </summary>
        [TestMethod]
        public void ExistsAtWillThrowWhenDisplayNameDoesNotMatch()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsAt(host.Tracking.Records.Count() - 2, expected, ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has an ID of 12
        ///   When
        ///   * The workflow is run
        ///   Then
        ///   * Assert.ExistsId will find the tracking record with the state and ID
        /// </summary>
        [TestMethod]
        public void ExistsIdWillNotThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            host.Tracking.Assert.ExistsId("12", ActivityInstanceState.Closed);
            host.Tracking.Assert.ExistsId("12", ActivityInstanceState.Closed, 0);
            host.Tracking.Assert.ExistsId("12", ActivityInstanceState.Closed, "fail message");
            host.Tracking.Assert.ExistsId("12", ActivityInstanceState.Closed, 0, "fail message");
        }

        /// <summary>
        ///   Given
        ///   * An Activity ID of -1
        ///   When
        ///   * The workflow is run
        ///   Then
        ///   * Assert.ExistsId will not find a tracking record with the state and ID
        ///   * A WorkflowAssertFailed exception will be thrown
        /// </summary>
        [TestMethod]
        public void ExistsIdWithNoArgumentMatchWillThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsId("-1", ActivityInstanceState.Closed));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsId("-1", ActivityInstanceState.Closed, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has a Text argument with the value "WriteLine1"
        ///   When
        ///   * The workflow is run with Memory Tracking
        ///   Then
        ///   * Assert.Exists will find the tracking record with the text argument WriteLine1
        /// </summary>
        [TestMethod]
        public void ExistsIdArgMatchWillNotThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            host.Tracking.Assert.ExistsIdArgMatch("12", ActivityInstanceState.Closed, "Text", "WriteLine\\d");
            host.Tracking.Assert.ExistsIdArgMatch(
                "12", ActivityInstanceState.Closed, "Text", "WriteLine\\d", "fail message");
            host.Tracking.Assert.ExistsIdArgMatch("12", ActivityInstanceState.Closed, "Text", "WriteLine\\d", 0);
            host.Tracking.Assert.ExistsIdArgMatch(
                "12", ActivityInstanceState.Closed, "Text", "WriteLine\\d", 0, "fail message");
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has a Text argument with the value "WriteLine1"
        ///   When
        ///   * The workflow is run with Memory Tracking
        ///   Then
        ///   * Assert.Exists will not find a tracking record with the text argument "Bad Argument"
        ///   * A WorkflowAssertFailed exception will be thrown
        /// </summary>
        [TestMethod]
        public void ExistsIdArgMatchWithNoArgumentMatchWillThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsIdArgMatch("12", ActivityInstanceState.Closed, "Text", "Bad Argument"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsIdArgMatch(
                    "12", ActivityInstanceState.Closed, "Text", "Bad Argument", "fail message"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsIdArgMatch("12", ActivityInstanceState.Closed, "Text", "Bad Argument", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsIdArgMatch(
                    "12", ActivityInstanceState.Closed, "Text", "Bad Argument", 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has a Text argument with the value "WriteLine1"
        ///   When
        ///   * The workflow is run with Memory Tracking
        ///   Then
        ///   * Assert.Exists will find the tracking record with the text argument WriteLine1
        /// </summary>
        [TestMethod]
        public void ExistsArgMatchWillNotThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            host.Tracking.Assert.ExistsArgMatch("WriteLine1", ActivityInstanceState.Closed, "Text", "WriteLine\\d");
            host.Tracking.Assert.ExistsArgMatch(
                "WriteLine1", ActivityInstanceState.Closed, "Text", "WriteLine\\d", "fail message");
            host.Tracking.Assert.ExistsArgMatch("WriteLine1", ActivityInstanceState.Closed, "Text", "WriteLine\\d", 0);
            host.Tracking.Assert.ExistsArgMatch(
                "WriteLine1", ActivityInstanceState.Closed, "Text", "WriteLine\\d", 0, "fail message");
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has a Text argument with the value "WriteLine1"
        ///   When
        ///   * The workflow is run with Memory Tracking
        ///   Then
        ///   * Assert.Exists will not find a tracking record with the text argument "Bad Argument"
        ///   * A WorkflowAssertFailed exception will be thrown
        /// </summary>
        [TestMethod]
        public void ExistsArgMatchWithNoArgumentMatchWillThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsArgMatch("WriteLine1", ActivityInstanceState.Closed, "Text", "Bad Argument"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Closed, "Text", "Bad Argument", "fail message"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Closed, "Text", "Bad Argument", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Closed, "Text", "Bad Argument", 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with an activity that does occur before
        ///   Then
        ///   * Assert.ExistsBefore does not throw
        /// </summary>
        [TestMethod]
        public void ExistBeforeWillNotThrowWhenBefore()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            host.Tracking.Assert.ExistsBefore("WriteLine1", "WriteLine2", ActivityInstanceState.Closed);
            host.Tracking.Assert.ExistsBefore("WriteLine1", "WriteLine2", ActivityInstanceState.Closed, "fail message");
            host.Tracking.Assert.ExistsBefore("WriteLine1", "WriteLine2", ActivityInstanceState.Closed, 0);
            host.Tracking.Assert.ExistsBefore(
                "WriteLine1", "WriteLine2", ActivityInstanceState.Closed, 0, "fail message");

            host.Tracking.Assert.ExistsBefore(
                "WriteLine1", ActivityInstanceState.Closed, "WriteLine2", ActivityInstanceState.Executing);

            host.Tracking.Assert.ExistsBefore(
                "WriteLine1", ActivityInstanceState.Closed, "WriteLine2", ActivityInstanceState.Executing, 0);

            host.Tracking.Assert.ExistsBefore(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "WriteLine2",
                ActivityInstanceState.Executing,
                "fail message");

            host.Tracking.Assert.ExistsBefore(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "WriteLine2",
                ActivityInstanceState.Executing,
                0,
                "fail message");
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with an activity that does not occur before
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeWillThrowWhenNotBefore()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("WriteLine2", "WriteLine1", ActivityInstanceState.Closed));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2", "WriteLine1", ActivityInstanceState.Closed, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("WriteLine2", "WriteLine1", ActivityInstanceState.Closed, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2", "WriteLine1", ActivityInstanceState.Closed, 0, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2", ActivityInstanceState.Executing, "WriteLine1", ActivityInstanceState.Closed));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2",
                    ActivityInstanceState.Executing,
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2", ActivityInstanceState.Executing, "WriteLine1", ActivityInstanceState.Closed, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine2",
                    ActivityInstanceState.Executing,
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    0,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeId is invoked with an activity that does not occur before
        ///   Then
        ///   * Assert.ExistsBeforeId does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeIdWillThrowWhenNotBeforeId()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("2", "1", ActivityInstanceState.Executing));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("2", "1", ActivityInstanceState.Executing, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("2", "1", ActivityInstanceState.Executing, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("2", "1", ActivityInstanceState.Executing, 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with an before activity that does exist
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeDoesNotExistWillThrow()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("Bad Record", "WriteLine1", ActivityInstanceState.Closed));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "Bad Record", "WriteLine1", ActivityInstanceState.Closed, "fail message"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("Bad Record", "WriteLine1", ActivityInstanceState.Closed, 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "Bad Record", "WriteLine1", ActivityInstanceState.Closed, 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with a before activity that does exist
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistBeforeWillThrowWhenBeforeDoesNotExist()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("Bad Record", "WriteLine2", ActivityInstanceState.Closed));

            // Overload with startIndex
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("Bad Record", "WriteLine2", ActivityInstanceState.Closed, 1));

            // Overload with state
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "Bad Record", ActivityInstanceState.Closed, "WriteLine2", ActivityInstanceState.Closed));

            // Overload with state and startIndex
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "Bad Record", ActivityInstanceState.Closed, "WriteLine2", ActivityInstanceState.Closed, 1));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with an after activity that does exist
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistBeforeWillThrowWhenAfterDoesNotExist()
        {
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            host.TestActivity();
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("WriteLine1", "Bad Record", ActivityInstanceState.Closed));

            // Overload with startIndex
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("WriteLine1", "Bad Record", ActivityInstanceState.Closed, 1));

            // Overload with state and startIndex
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine1", ActivityInstanceState.Closed, "Bad Record", ActivityInstanceState.Closed));

            // Overload with state and startIndex
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBefore(
                    "WriteLine1", ActivityInstanceState.Closed, "Bad Record", ActivityInstanceState.Closed, 1));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with the same name for before and after
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchAndAfterSameWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with an invalid after DisplayName
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadAfterDisplayNameWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with an invalid before DisplayName
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadBeforeDisplayNameWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "Bad Name",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with an invalid after argument name
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadAferArgumentWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with an invalid before argument name
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadBeforeArgumentWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Bad Arg",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with an invalid before DisplayName
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadBeforePatternWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    1,
                    "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with an invalid after pattern
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWithBadAfterPatternWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    1,
                    "fail message"));

            // Overload with startIndex 
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsBeforeArgMatch(
                    "WriteLine1",
                    ActivityInstanceState.Closed,
                    "Text",
                    "WriteLine\\d",
                    "WriteLine2",
                    ActivityInstanceState.Closed,
                    "Text",
                    "Bad Pattern",
                    1));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeArgMatch is invoked with the a before argument that is before 
        ///   Then
        ///   * Assert.ExistsBeforeArgMatch does not throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeArgMatchWhenBeforeWillNotThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert

            host.Tracking.Assert.ExistsBeforeArgMatch(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                "WriteLine2",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d");

            host.Tracking.Assert.ExistsBeforeArgMatch(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                "WriteLine2",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                "fail message");

            // Overload with startIndex 
            host.Tracking.Assert.ExistsBeforeArgMatch(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                "WriteLine2",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                1);

            // Overload with startIndex 
            host.Tracking.Assert.ExistsBeforeArgMatch(
                "WriteLine1",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                "WriteLine2",
                ActivityInstanceState.Closed,
                "Text",
                "WriteLine\\d",
                1,
                "fail message");
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeId is invoked with an After Id that does not exist
        ///   Then
        ///   * Assert.ExistsBeforeId does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeIdWillThrowWhenAfterIdDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "-1", ActivityInstanceState.Closed));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "-1", ActivityInstanceState.Closed, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "-1", ActivityInstanceState.Closed, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "-1", ActivityInstanceState.Closed, 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBeforeId is invoked with an Id that does not exist
        ///   Then
        ///   * Assert.ExistsBeforeId does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeIdWillThrowWhenBeforeIdDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("-1", "1", ActivityInstanceState.Closed));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("-1", "1", ActivityInstanceState.Closed, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("-1", "1", ActivityInstanceState.Closed, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("-1", "1", ActivityInstanceState.Closed, 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with the same name for before and after
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeIdWillThrowWhenBeforeAndAfterSame()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "1", ActivityInstanceState.Closed));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "1", ActivityInstanceState.Closed, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "1", ActivityInstanceState.Closed, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBeforeId("1", "1", ActivityInstanceState.Closed, 0, "fail message"));
        }

        /// <summary>
        ///   Given
        ///   * An activity with several child activities
        ///   When
        ///   * Assert.ExistsBefore is invoked with the same name for before and after
        ///   Then
        ///   * Assert.ExistsBefore does throw
        /// </summary>
        [TestMethod]
        public void ExistsBeforeWillThrowWhenBeforeAndAfterTheSame()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsBefore("WriteLine1", "WriteLine1", ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Given
        ///   * Workflow with a WriteLine1 activity which has a Text argument with the value "WriteLine1"
        ///   When
        ///   * The workflow is run with Memory Tracking
        ///   Then
        ///   * Assert.Exists will not find a tracking record searching for an argument named "Does Not Exist"
        ///   * A WorkflowAssertFailed exception will be thrown
        /// </summary>
        [TestMethod]
        public void ExistsArgMatchWithNoArgumentWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = new WorkflowInvokerTest(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsArgMatch(
                    "WriteLine1", ActivityInstanceState.Closed, "Does Not Exist", "pattern"));
        }

        /// <summary>
        ///   Verifies assertion of WorkflowInstanceRecords for Exist and DoesNotExist with and without indexes
        /// </summary>
        [TestMethod]
        public void ExistsWorkflowInstanceWillNotThrow()
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

            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started);

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started, 5));

            // No index
            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed);

            // Again with index
            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed, 20);

            // No index, fail message
            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed, "fail message");

            // Again with index and fail message
            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Completed, 20, "fail message");
        }

        /// <summary>
        ///   Verifies assertion of WorkflowInstanceRecords for Exist and DoesNotExist with and without indexes
        /// </summary>
        [TestMethod]
        public void DoesNotExistWorkflowInstanceWillNotThrow()
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

            host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started);

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started, 5));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists(WorkflowInstanceRecordState.Started, 5, "fail message"));

            // Verify that the workflow did not go idle
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Idle);

            // No started after 2
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started, 2);

            // No started after 2
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started, 2, "fail message");
        }

        /// <summary>
        ///   Verifies that ExistsAt will throw when a record is found at the index but the record type is not ActivityStateRecord
        /// </summary>
        [TestMethod]
        public void ExistsAtIndexOfWrongTypeExistsAtMessageWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";
            const string expectedMessage =
                "AssertTracking.ExistsAt failed. Cannot find ActivityStateRecord for Activity \"TwoOfThese\" with state <Closed> at index <22>. fail message";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsAt(
                    host.Tracking.Records.Count() - 1, expected, ActivityInstanceState.Closed, "fail message"),
                expectedMessage);
        }

        /// <summary>
        ///   Verifies that ExistsAt will throw when a record is found at the index but the record type is not ActivityStateRecord
        /// </summary>
        [TestMethod]
        public void ExistsAtIndexOfWrongTypeExistsAtWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsAt(host.Tracking.Records.Count() - 1, expected, ActivityInstanceState.Closed));
        }

        /// <summary>
        ///   Verifies that ExistsAt will not throw if the record exists
        /// </summary>
        [TestMethod]
        public void ExistsAtIndexWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            // The third to last record should be the close of a TwoOfThese
            host.Tracking.Assert.ExistsAt(host.Tracking.Records.Count() - 3, expected, ActivityInstanceState.Closed);
        }

        /// <summary>
        ///   Verifies that ExistsAt will throw when it finds an ActivityStateRecord with the wrong name
        /// </summary>
        [TestMethod]
        public void ExistsAtIndexMessageOfWrongNameExistsAtWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";
            const string expectedMessage =
               "AssertTracking.ExistsAt failed. Cannot find ActivityStateRecord for Activity \"TwoOfThese\" with state <Closed> at index <21>. fail message";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsAt(
                    host.Tracking.Records.Count() - 2, expected, ActivityInstanceState.Closed, "fail message"),
                expectedMessage);
        }

        /// <summary>
        ///   Verifies that ExistsAt will not throw if the record exists
        /// </summary>
        [TestMethod]
        public void ExistsAtIndexMessageWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            // The third to last record should be the close of a TwoOfThese
            host.Tracking.Assert.ExistsAt(
                host.Tracking.Records.Count() - 3, expected, ActivityInstanceState.Closed, "fail message");
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateCountMessageWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";
            const string expectedMessage =
               "AssertTracking.Exists failed. Cannot find TrackingRecord with predicate. fail message";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsCount<ActivityStateRecord>(
                    r => r.Activity.Name == expected, 4, "fail message"),
                expectedMessage);
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateCountWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == expected, 2));
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsCountWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsCount(expected, ActivityInstanceState.Executing, 2));
        }

        /// <summary>
        ///   Verifies that Exists will not throw when there are count matching records
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.ExistsIdCount("1", ActivityInstanceState.Executing, 1);
            host.Tracking.Assert.ExistsIdCount("1", ActivityInstanceState.Executing, 1, "fail message");
        }

        /// <summary>
        ///   Verifies that ScheduledId will not throw when a matching record is scheduled
        /// </summary>
        [TestMethod]
        public void ScheduledIdWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert
            host.Tracking.Assert.ScheduledId("1", "12");
            host.Tracking.Assert.ScheduledId("1", "12", 0);
            host.Tracking.Assert.ScheduledId("1", "12", 0, "fail message");
        }

        /// <summary>
        ///   Verifies that ScheduledId will not throw when a matching record is not found
        /// </summary>
        [TestMethod]
        public void ScheduledIdWillThrowWhenNotExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert

            // Bad child activity Id
            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.Tracking.Assert.ScheduledId("1", "-1"));
            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.Tracking.Assert.ScheduledId("1", "-1", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ScheduledId("1", "-1", 0, "fail message"));

            // Bad parent activity Id
            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.Tracking.Assert.ScheduledId("-1", "1"));
            AssertHelper.Throws<WorkflowAssertFailedException>(() => host.Tracking.Assert.ScheduledId("-1", "1", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ScheduledId("-1", "1", 0, "fail message"));
        }

        /// <summary>
        ///   Verifies that Scheduled will not throw when a matching record is scheduled
        /// </summary>
        [TestMethod]
        public void ScheduledWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert
            host.Tracking.Assert.Scheduled("Sequence", "WriteLine1");
            host.Tracking.Assert.Scheduled("Sequence", "WriteLine1", 0);
            host.Tracking.Assert.Scheduled("Sequence", "WriteLine1", 0, "fail message");
        }

        /// <summary>
        ///   Verifies that Scheduled will not throw when a matching record is not found
        /// </summary>
        [TestMethod]
        public void ScheduledWillThrowWhenNotExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();
            host.Tracking.Trace();

            // Assert

            // Bad child activity Id
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Sequence", "Bad Name"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Sequence", "Bad Name", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Sequence", "Bad Name", 0, "fail message"));

            // Bad parent activity Id
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Bad Name", "Sequence"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Bad Name", "Sequence", 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Scheduled("Bad Name", "Sequence", 0, "fail message"));
        }

        /// <summary>
        ///   Verifies that Exists will not throw when there are count matching records
        /// </summary>
        [TestMethod]
        public void ExistsIdCountWillThrowWhenWrongCount()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsIdCount("1", ActivityInstanceState.Executing, 99));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsIdCount("1", ActivityInstanceState.Executing, 99, "fail message"));
        }

        /// <summary>
        ///   Verifies that ExistsCount will not throw when there are count matching records
        /// </summary>
        [TestMethod]
        public void ExistsCountWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.ExistsCount(expected, ActivityInstanceState.Executing, 2);
            host.Tracking.Assert.ExistsCount(expected, ActivityInstanceState.Executing, 2, "fail message");
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateMessageWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";
            const string expectedMessage =
               "AssertTracking.Exists failed. Cannot find TrackingRecord with predicate. fail message";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected, "fail message"),
                expectedMessage);
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected));

            // Overload with start index
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected, 1));

            // Overload with message
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected, "fail message"));

            // Overload with start index and message
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected, 1, "fail message"));
        }

        /// <summary>
        ///   Verifies that Exists will throw when no matching record
        /// </summary>
        [TestMethod]
        public void ExistsCountPredicateWillThrowWhenCountNotMet()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "No Record";

            // Act
            host.TestActivity();

            // Assert

            // Overload with count
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == expected, 99));

            // Overload with count and message
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.ExistsCount<ActivityStateRecord>(
                    r => r.Activity.Name == expected, 99, "fail message"));
        }

        /// <summary>
        ///   Verifies that Exists will not throw when a record exists
        /// </summary>
        [TestMethod]
        public void ExistsWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.Exists("WriteLine1", ActivityInstanceState.Executing);
            host.Tracking.Assert.Exists("WriteLine1", ActivityInstanceState.Executing, 1);
            host.Tracking.Assert.Exists("WriteLine1", ActivityInstanceState.Executing, 1, "fail message");
        }

        /// <summary>
        ///   Verifies that Exists will not throw when a record exists
        /// </summary>
        [TestMethod]
        public void ExistsWillThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists("Bad", ActivityInstanceState.Executing));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists("Bad", ActivityInstanceState.Executing, 1));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.Exists("Bad", ActivityInstanceState.Executing, 1, "fail message"));
        }

        /// <summary>
        ///   Verifies that Exists will throw when wrong count
        /// </summary>
        [TestMethod]
        public void ExistsWrongCountMessageWillThrow()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert

            // Only one of these
            const string expected = "WriteLine1";
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == expected, 99));
        }

        /// <summary>
        ///   Verifies that Exists will not throw when a matching record and count
        /// </summary>
        [TestMethod]
        public void ExistsPredicateCountMessageWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == expected, 4, "fail message");
        }

        /// <summary>
        ///   Verifies that Exists (count) will not throw when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateCountWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "TwoOfThese";

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.ExistsCount<ActivityStateRecord>(r => r.Activity.Name == expected, 4);
        }

        /// <summary>
        ///   Verifies that Exists will not throw when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateMessageWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "WriteLine2";
            const string expectedMessage = "fail message";

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected, expectedMessage);
        }

        /// <summary>
        ///   Verifies that Exists will not throw when a matching record
        /// </summary>
        [TestMethod]
        public void ExistsPredicateWillNotThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);
            const string expected = "WriteLine2";

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.Exists<ActivityStateRecord>(r => r.Activity.Name == expected);
        }

        /// <summary>
        ///   DoesNotExist should not throw when the record does not exist
        /// </summary>
        [TestMethod]
        public void DoesNotExistWillNotThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.DoesNotExist("No Record", ActivityInstanceState.Executing);
        }

        /// <summary>
        ///   DoesNotExist should throw when the record exists
        /// </summary>
        [TestMethod]
        public void DoesNotExistWillThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist("WriteLine1", ActivityInstanceState.Executing));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist("WriteLine1", ActivityInstanceState.Executing, 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist("WriteLine1", ActivityInstanceState.Executing, "fail message"));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () =>
                host.Tracking.Assert.DoesNotExist("WriteLine1", ActivityInstanceState.Executing, 0, "fail message"));
        }

        /// <summary>
        ///   DoesNotExistId should not throw when the record does not exist
        /// </summary>
        [TestMethod]
        public void DoesNotExistIdWillNotThrowWhenDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.DoesNotExistId("No Id", ActivityInstanceState.Executing);
            host.Tracking.Assert.DoesNotExistId("No Id", ActivityInstanceState.Executing, "Fail message");
            host.Tracking.Assert.DoesNotExistId("No Id", ActivityInstanceState.Executing, 0);
            host.Tracking.Assert.DoesNotExistId("No Id", ActivityInstanceState.Executing, 0, "Fail message");
        }

        /// <summary>
        ///   DoesNotExistId should throw when the record exists
        /// </summary>
        [TestMethod]
        public void DoesNotExistIdWillThrowWhenExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExistId("1", ActivityInstanceState.Executing));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExistId("1", ActivityInstanceState.Executing, 0));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExistId("1", ActivityInstanceState.Executing, 0, "fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExistId("1", ActivityInstanceState.Executing, "Fail message"));
        }

        /// <summary>
        ///   DoesNotExist should not throw when the record does not exist
        /// </summary>
        [TestMethod]
        public void DoesNotExistWillNotThrowWhenWorkflowInstanceDoesNotExist()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Idle);
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Idle, 1);
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Idle, "Fail message");
            host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Idle, 1, "Fail message");
        }

        /// <summary>
        ///   DoesNotExist should throw when the record exists
        /// </summary>
        [TestMethod]
        public void DoesNotExistWillThrowWhenWorkflowInstanceExists()
        {
            // Arrange
            var activity = CreateTestActivity();
            var host = WorkflowInvokerTest.Create(activity);

            // Act
            host.TestActivity();

            // Assert
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started, 0));
            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started, "Fail message"));

            AssertHelper.Throws<WorkflowAssertFailedException>(
                () => host.Tracking.Assert.DoesNotExist(WorkflowInstanceRecordState.Started, 0, "Fail message"));
        }

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
                        }
                };
        }
    }
}