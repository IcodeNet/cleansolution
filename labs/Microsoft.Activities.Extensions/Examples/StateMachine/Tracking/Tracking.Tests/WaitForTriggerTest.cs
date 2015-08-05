// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForTriggerTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Tests
{
    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Linq;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Tracking.Windows.Activities;

    /// <summary>
    ///   Tests the WaitForTrigger activity
    /// </summary>
    [TestClass]
    public class WaitForTriggerTest
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A WaitForTrigger Activity with a trigger of StateTrigger.T1
        ///   When
        ///   * The activity is invoked
        ///   Then
        ///   * The workflow will become idle with a bookmark named "T1"
        /// </summary>
        [TestMethod]
        public void BecomesIdleWithTriggerBookmark()
        {
            // Arrange
            var activity = new WaitForTrigger { Trigger = StateTrigger.T1 };
            var host = WorkflowApplicationTest.Create(activity);
            try
            {
                // Act
                var result = host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

                // Assert
                Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
                var idleResult = result as WorkflowIdleEpisodeResult;
                Assert.IsTrue(idleResult.IdleArgs.Bookmarks.Any(StateTrigger.T1));
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        /// <summary>
        ///   Given
        ///   * A WaitForTrigger Activity with a trigger of StateTrigger.T1
        ///   * The activity has been invoked and is now idle
        ///   When
        ///   * The activity is resumed with the bookmark name StateTrigger.T1
        ///   Then
        ///   * The workflow will complete
        /// </summary>
        [TestMethod]
        public void CompletesWhenResumedWithTrigger()
        {
            // Arrange
            var activity = new WaitForTrigger { Trigger = StateTrigger.T1 };
            var host = WorkflowApplicationTest.Create(activity);
            host.TestWorkflowApplication.RunUntilBookmark(StateTrigger.T1);

            try
            {
                // Act
                var result = host.TestWorkflowApplication.ResumeEpisodeBookmark(StateTrigger.T1);

                // Assert
                Assert.IsInstanceOfType(result, typeof(WorkflowCompletedEpisodeResult));
                var idleResult = result as WorkflowCompletedEpisodeResult;
            }
            finally
            {
                host.Tracking.Trace();
            }
        }

        #endregion
    }
}