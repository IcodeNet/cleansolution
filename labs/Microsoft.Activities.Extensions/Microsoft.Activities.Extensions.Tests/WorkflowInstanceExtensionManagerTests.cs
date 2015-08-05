// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstanceExtensionManagerTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Activities.Statements;
    using System.Diagnostics;
    using System.ServiceModel.Activities;

    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the WorkflowInstanceExtensionManagerEx class
    /// </summary>
    [TestClass]
    public class WorkflowInstanceExtensionManagerTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication 
        ///   * An activity that creates a bookmark and an activity that creates an extension
        ///   * two singleton extensions
        ///   When 
        ///   * GetSingletonExtensions is invoked
        ///   Then
        ///   * a read only collection is returned
        ///   * The collection contains the two extensions
        ///   * The extension created by the activity does not appear in the collection
        /// </summary>
        [TestMethod]
        public void WorkflowApplicationReturnsExtensionsCollection()
        {
            const string BookmarkName = "Test";

            var activity = new Sequence { Activities = { new ActivityExtensionTest { AddExtensionProvider = true }, new TestBookmark<int> { BookmarkName = BookmarkName }, new ActivityExtensionTest { AddExtensionProvider = true }, } };

            var traceTrackingParticipant = new TraceTrackingParticipant();
            var listTrackingParticipant = new ListTrackingParticipant();

            var workflowApplication = new WorkflowApplication(activity);

            // Add a couple of singleton extensions
            workflowApplication.Extensions.Add(traceTrackingParticipant);
            workflowApplication.Extensions.Add(listTrackingParticipant);

            // foreach (var extension in workflowApplication.Extensions)
            // {
            // Doh! this won't work
            // foreach statement cannot operate on variables of type 
            // 'System.Activities.Hosting.WorkflowInstanceExtensionManager' 
            // because 'System.Activities.Hosting.WorkflowInstanceExtensionManager' 
            // does not contain a public definition for 'GetEnumerator'	
            // }

            // Run it so that the activity will create an extension
            workflowApplication.RunEpisode(BookmarkName, Constants.Timeout);

            // Resume and run to completion
            workflowApplication.ResumeEpisodeBookmark(BookmarkName, 1);

            // Now I can get the Singleton Extensions as a collection
            var extensions = workflowApplication.Extensions.GetSingletonExtensions();
            Assert.IsNotNull(extensions);
            Assert.AreEqual(2, extensions.Count);

            // Note: Extensions created by AddDefaultExtensionProvider will not appear in the collection
            Assert.IsTrue(extensions.Contains(traceTrackingParticipant));
            Assert.IsTrue(extensions.Contains(listTrackingParticipant));

            foreach (var extension in extensions)
            {
                Debug.WriteLine("Found singleton extension " + extension);
            }
        }

        /// <summary>
        ///   Given 
        ///   * A WorkflowApplication 
        ///   * An activity that creates a bookmark and an activity that creates an extension
        ///   * two singleton extensions
        ///   When 
        ///   * GetSingletonExtensions is invoked
        ///   Then
        ///   * a read only collection is returned
        ///   * The collection contains the two extensions
        ///   * The extension created by the activity does not appear in the collection
        /// </summary>
        [TestMethod]
        public void WorkflowServiceHostReturnsExtensionsCollection()
        {
            var traceTrackingParticipant = new TraceTrackingParticipant();
            var listTrackingParticipant = new ListTrackingParticipant();

            var service = new WorkflowService() { Body = new Sequence() };
            var host = new WorkflowServiceHost(service);

            // Add a couple of singleton extensions
            host.WorkflowExtensions.Add(traceTrackingParticipant);
            host.WorkflowExtensions.Add(listTrackingParticipant);

            // Now I can get the Singleton Extensions as a collection
            var extensions = host.WorkflowExtensions.GetSingletonExtensions();
            Assert.IsNotNull(extensions);
            Assert.AreEqual(2, extensions.Count);

            Assert.IsTrue(extensions.Contains(traceTrackingParticipant));
            Assert.IsTrue(extensions.Contains(listTrackingParticipant));
        }
        #endregion
    }
}