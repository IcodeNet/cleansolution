// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationCache.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    /// <summary>
    ///   Provides a cache for WorkflowApplication
    /// </summary>
    /// <remarks>
    ///   The cache keeps only WorkflowApplication instances that are active.  
    ///   Once they abort, terminate, complete etc. they are removed from the cache
    ///   User Stories
    ///   * Add to cache
    ///   * Get from cache
    /// </remarks>
    public class WorkflowApplicationCache
    {
        #region Fields

        /// <summary>
        ///   The dictionary
        /// </summary>
        private readonly ConcurrentDictionary<Guid, WorkflowApplication> dictionary =
            new ConcurrentDictionary<Guid, WorkflowApplication>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a new WorkflowApplication to the cache
        /// </summary>
        /// <param name="application">
        /// The application. 
        /// </param>
        /// <returns>
        /// The WorkflowApplication that was added or updated 
        /// </returns>
        public WorkflowApplication Add(WorkflowApplication application)
        {
            Debug.Assert(this.dictionary != null, "dictionary != null");
            var observer = new WorkflowApplicationObserver(application) { Aborted = this.Aborted };
            return this.dictionary.AddOrUpdate(
                application.Id, guid => application, (guid, workflowApplication) => application);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="workflowApplicationAbortedEventArgs">
        /// The workflow application aborted event args. 
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void Aborted(WorkflowApplicationAbortedEventArgs workflowApplicationAbortedEventArgs)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}