namespace Microsoft.Activities.Extensions.Tests
{
    //using System;
    //using System.Linq;

    //using Microsoft.Activities.Extensions.Windows;
    //using Microsoft.Activities.UnitTesting;
    //using Microsoft.VisualStudio.TestTools.UnitTesting;

    //using TrackingStateMachine.Activities;

    ///// <summary>
    /////   Tests for the ObservableStateTracker
    ///// </summary>
    //[TestClass]
    //public class ObservableStateTrackerTest
    //{
    //    #region Public Methods and Operators

    //    /// <summary>
    //    ///   Given
    //    ///   * A StateTracker
    //    ///   * An activity with a state machine
    //    ///   When
    //    ///   * The workflow is run
    //    ///   Then
    //    ///   * At least one PropertyChanged notification is raised for the "CurrentStateMachine" property
    //    /// </summary>
    //    [TestMethod]
    //    public void CurrentStateMachineRaisesPropertyChanged()
    //    {
    //        // Arrange
    //        var activity = new StateMachineExample();
    //        var host = WorkflowApplicationTest.Create(activity);
    //        var tracker = new ObservableStateTracker(activity);
    //        host.Extensions.Add(tracker);
    //        var observer = new StateTrackerObserver(tracker);
    //        try
    //        {
    //            // Act
    //            host.TestWorkflowApplication.RunUntilBookmark();

    //            // Assert
    //            Assert.IsTrue(
    //                observer.PropertyChangedArgs.Any(args => args.PropertyName == "CurrentStateMachine"),
    //                "Could not find property changed notification for CurrentStateMachine");
    //        }
    //        catch (Exception exception)
    //        {
    //            exception.Trace();

    //            observer.Trace();

    //            // Trace when the test fails
    //            host.Tracking.Trace();
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    ///   Given
    //    ///   * A StateTracker
    //    ///   * An activity with a state machine
    //    ///   When
    //    ///   * The workflow is run
    //    ///   Then
    //    ///   * At least one PropertyChanged notification is raised for the "CurrentState" property
    //    /// </summary>
    //    [TestMethod]
    //    public void CurrentStateRaisesPropertyChanged()
    //    {
    //        // Arrange
    //        var activity = new StateMachineExample();
    //        var host = WorkflowApplicationTest.Create(activity);
    //        var tracker = new ObservableStateTracker(activity);
    //        host.Extensions.Add(tracker);
    //        var observer = new StateTrackerObserver(tracker);
    //        try
    //        {
    //            // Act
    //            host.TestWorkflowApplication.RunUntilBookmark();

    //            // Assert
    //            Assert.IsTrue(observer.PropertyChangedArgs.Any(args => args.PropertyName == "CurrentState"));
    //        }
    //        finally
    //        {
    //            host.Tracking.Trace();
    //        }
    //    }

    //    #endregion
    //}
}