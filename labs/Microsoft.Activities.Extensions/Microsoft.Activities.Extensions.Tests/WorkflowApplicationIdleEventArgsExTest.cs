// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationIdleEventArgsExTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;

    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The book mark test.
    /// </summary>
    [TestClass]
    public class WorkflowApplicationIdleEventArgsExTest
    {
        #region Public Methods

        /// <summary>
        /// Given a workflow with an activity that creates a bookmark, when Idle ContainsBookmark should return false for a non-matching bookmark name
        /// </summary>
        [TestMethod]
        public void WhenIdleWithBookmarkContainsBookmarkShouldReturnFalseOnNoMatch()
        {
            const string Expected = "TestBookmark";
            var found = false;

            var activity = new Sequence
                {
                   Activities = {
                                     new TestBookmark<int> { BookmarkName = Expected }, new WriteLine() 
                                 } 
                };

            var host = new WorkflowApplication(activity) { Idle = args => found = args.ContainsBookmark("NoMatch") };

            // Run the workflow until idle with the bookmark
            var result = host.RunEpisode(Expected, TimeSpan.FromMilliseconds(100));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
            Assert.IsFalse(found);
        }

        /// <summary>
        /// Given a workflow with an activity that creates a bookmark, when Idle ContainsBookmark should return true for a matching bookmark name
        /// </summary>
        [TestMethod]
        public void WhenIdleWithBookmarkContainsBookmarkShouldReturnTrueOnMatch()
        {
            const string Expected = "TestBookmark";
            var found = false;

            var activity = new Sequence
                {
                   Activities = {
                                     new TestBookmark<int> { BookmarkName = Expected }, new WriteLine() 
                                 } 
                };

            var host = new WorkflowApplication(activity) { Idle = args => found = args.ContainsBookmark(Expected) };

            // Run the workflow until idle with the bookmark
            var result = host.RunEpisode(Expected, TimeSpan.FromMilliseconds(100));

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(WorkflowIdleEpisodeResult));
            Assert.AreEqual(ActivityInstanceState.Executing, result.State);
            Assert.IsTrue(found);
        }

        /// <summary>
        /// Given a workflow with an activity that goes idle but does not create bookmarks, when Idle ContainsBookmark should return false for any bookmark name
        /// </summary>
        [TestMethod]
        public void WhenIdleWithNoBookmarksContainsBookmarkShouldReturnFalseOnNoMatch()
        {
            var found = false;

            var activity = new Sequence { Activities = { new TestAsync { Sleep = 10 }, new WriteLine() } };

            var host = new WorkflowApplication(activity)
                {
                   Idle = args => found = args.ContainsBookmark("No Bookmarks Should Match") 
                };

            host.RunEpisode(TimeSpan.FromMilliseconds(50000));

            Assert.IsFalse(found);
        }

        /// <summary>
        /// Given a workflow with an activity that goes idle but does not create bookmarks, when Idle ContainsBookmark(null) should return false
        /// </summary>
        [TestMethod]
        public void WhenIdleWithNoBookmarksContainsBookmarkShouldReturnFalseOnNull()
        {
            var found = false;

            var activity = new Sequence { Activities = { new TestAsync { Sleep = 10 }, new WriteLine() } };

            var host = new WorkflowApplication(activity) { Idle = args => found = args.ContainsBookmark(null) };

            host.RunEpisode(TimeSpan.FromMilliseconds(100));

            Assert.IsFalse(found);
        }

        #endregion
    }
}