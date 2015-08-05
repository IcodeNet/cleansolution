using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Threading;

    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;

    [TestClass]
    public class WorkflowObserverTests
    {

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * A WorkflowObserverCtor is invoked with null
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        public void TestName()
        {
            AssertHelper.Throws<ArgumentNullException>(() => new WorkflowApplicationObserver(null));
        }

        /// <summary>
        /// Given
        /// * A WorkflowApplication with an Idleed handler
        /// * A WorkflowApplicationObserver which chains an aborted handler
        /// When
        /// * The WorkflowApplicaiton Idles
        /// Then
        /// * Both aborted handlers are invoked
        /// </summary>
        [TestMethod]
        public void ObserverShouldObserveIdle()
        {
            // Arrange
            var activity = new TestBookmark<int> { BookmarkName = "B1" };
            var applicationIdle = false;
            var observerIdle = false;
            var completedEvent = new AutoResetEvent(false);
            var host = new WorkflowApplication(activity)
            {
                Idle = args => applicationIdle = true,
            };
            WorkflowApplicationObserver.Attach(host).ObserveIdle(args => observerIdle = true).ObserveComplete(args => completedEvent.Set());

            // Act
            // Run the workflow until idle with bookmark "B1"
            host.RunEpisode("B1");
            host.Cancel();

            completedEvent.WaitOne(Constants.Timeout);

            // Assert
            Assert.IsTrue(applicationIdle, "Application abort handler was not invoked");
            Assert.IsTrue(observerIdle, "Observer abort handler was not invoked");
        }


        /// <summary>
        /// Given
        /// * A WorkflowApplication with an Aborted handler
        /// * A WorkflowApplicationObserver which chains an aborted handler
        /// When
        /// * The WorkflowApplicaiton Aborts
        /// Then
        /// * Both aborted handlers are invoked
        /// </summary>
        [TestMethod]
        public void ObserverShouldObserveAbort()
        {
            // Arrange
            var activity = new TestBookmark<int> { BookmarkName = "B1" };
            var applicationAbort = false;
            var observerAbort = false;
            var completedEvent = new AutoResetEvent(false);
            var host = new WorkflowApplication(activity)
                {
                    Aborted = args => applicationAbort = true,
                    Completed = args => completedEvent.Set()
                };
            var observer = new WorkflowApplicationObserver(host) { Aborted = args => observerAbort = true };

            // Act
            // Run the workflow until idle with bookmark "B1"
            host.RunEpisode("B1");
            host.Abort();

            completedEvent.WaitOne(Constants.Timeout);

            // Assert
            Assert.IsTrue(applicationAbort, "Application abort handler was not invoked");
            Assert.IsTrue(observerAbort, "Observer abort handler was not invoked");
        }

        /// <summary>
        /// Given
        /// * A WorkflowApplication with an Completed handler
        /// * A WorkflowApplicationObserver which chains a completed handler
        /// When
        /// * The WorkflowApplicaiton Completes
        /// Then
        /// * Both completed handlers are invoked
        /// </summary>
        [TestMethod]
        public void ObserverShouldObserveComplete()
        {
            // Arrange
            var activity = new TestBookmark<int> { BookmarkName = "B1" };
            var applicationComplete = false;
            var observerComplete = false;
            var completedEvent = new AutoResetEvent(false);
            var host = new WorkflowApplication(activity)
            {
                Completed = args =>
                    {
                        applicationComplete = true;
                        completedEvent.Set();
                    }
            };

            var observer = new WorkflowApplicationObserver(host) { Completed = args => observerComplete = true };

            // Act
            // Run the workflow until idle with bookmark "B1"
            host.RunEpisode("B1");
            host.ResumeEpisodeBookmark("B1", 1);

            completedEvent.WaitOne(Constants.Timeout);

            // Assert
            Assert.IsTrue(applicationComplete, "Application abort handler was not invoked");
            Assert.IsTrue(observerComplete, "Observer abort handler was not invoked");
        }

        /// <summary>
        /// Given
        /// * A WorkflowApplication with an Aborted handler
        /// * A WorkflowApplicationObserver which chains an aborted handler
        /// When
        /// * The WorkflowApplicaiton Aborts
        /// Then
        /// * Both aborted handlers are invoked
        /// </summary>
        [TestMethod]
        public void TwoObserversShouldObserveAbort()
        {
            // Arrange
            var activity = new TestBookmark<int> { BookmarkName = "B1" };
            var applicationAbort = false;
            var observer1Abort = false;
            var observer2Abort = false;
            var completedEvent = new AutoResetEvent(false);
            var host = new WorkflowApplication(activity)
            {
                Aborted = args => applicationAbort = true,
                Completed = args => completedEvent.Set()
            };
            var observer1 = new WorkflowApplicationObserver(host) { Aborted = args => observer1Abort = true };
            var observer2 = new WorkflowApplicationObserver(host) { Aborted = args => observer2Abort = true };

            // Act
            // Run the workflow until idle with bookmark "B1"
            host.RunEpisode("B1");
            host.Abort();

            completedEvent.WaitOne(Constants.Timeout);

            // Assert
            Assert.IsTrue(applicationAbort, "Application abort handler was not invoked");
            Assert.IsTrue(observer1Abort, "Observer1 abort handler was not invoked");
            Assert.IsTrue(observer2Abort, "Observer2 abort handler was not invoked");
        }

    }
}
