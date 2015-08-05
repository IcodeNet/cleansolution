// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The workflow application extensions.
    /// </summary>
    public static class WorkflowApplicationExtensions
    {
        #region Enums

        /// <summary>
        ///   The workflow application state.
        /// </summary>
        internal enum WorkflowApplicationState : byte
        {
            /// <summary>
            ///   The aborted.
            /// </summary>
            Aborted = 3, 

            /// <summary>
            ///   The paused.
            /// </summary>
            Paused = 0, 

            /// <summary>
            ///   The runnable.
            /// </summary>
            Runnable = 1, 

            /// <summary>
            ///   The unloaded.
            /// </summary>
            Unloaded = 2
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Async Task to Cancel the workflow
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// A Task to Cancel the workflow 
        /// </returns>
        public static Task CancelAsync(this WorkflowApplication workflowApplication)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            Debug.Assert(Task.Factory != null, "Task.Factory != null");
            var task = Task.Factory.FromAsync(workflowApplication.BeginCancel, workflowApplication.EndCancel, null);
            Debug.Assert(task != null, "task != null");

            // ReSharper disable PossibleNullReferenceException
            return task.ContinueWith(t => t.Wait());

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Async Task to Cancel the workflow
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A Task to Cancel the workflow 
        /// </returns>
        public static Task CancelAsync(this WorkflowApplication workflowApplication, TimeSpan timeout)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            Debug.Assert(Task.Factory != null, "Task.Factory != null");
            var task = Task.Factory.FromAsync(
                workflowApplication.BeginCancel, workflowApplication.EndCancel, timeout, null);
            Debug.Assert(task != null, "task != null");
            return task.ContinueWith(
                t =>
                    {
                        Debug.Assert(t != null, "t != null");
                        t.Wait();
                    });
        }

        /// <summary>
        /// Extension method which determines if the bookmark collection contains a bookmark with the bookmark name
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The name of the bookmark 
        /// </param>
        /// <returns>
        /// true if there is a matching bookmark 
        /// </returns>
        public static bool ContainsBookmark<T>(this WorkflowApplication workflowApplication, T bookmarkName)
        {
            Contract.Requires(!IsNullOrEmptyBookmark(bookmarkName));
            if (IsNullOrEmptyBookmark(bookmarkName))
            {
                throw new ArgumentNullException("bookmarkName");
            }

            return workflowApplication.GetBookmarks().Any(info => info.BookmarkName == bookmarkName.ToString());
        }

        /// <summary>
        /// Extension method which determines if the bookmark collection contains a bookmark with the bookmark name
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="timeout">
        /// The interval in which this method must complete before the operation is canceled and a TimeoutException is thrown. 
        /// </param>
        /// <returns>
        /// true if there is a bookmark with <paramref name="bookmarkName"/> 
        /// </returns>
        public static bool ContainsBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, TimeSpan timeout)
        {
            Contract.Requires(!IsNullOrEmptyBookmark(bookmarkName));
            if (IsNullOrEmptyBookmark(bookmarkName))
            {
                throw new ArgumentNullException("bookmarkName");
            }

            return workflowApplication.GetBookmarks(timeout).Any(info => info.BookmarkName == bookmarkName.ToString());
        }

        /// <summary>
        /// Gets a list of bookmark names
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// The list of bookmarks 
        /// </returns>
        public static IEnumerable<string> GetBookmarkNames(this WorkflowApplication workflowApplication)
        {
            Debug.Assert(workflowApplication != null, "workflowApplication != null");
            return workflowApplication.GetBookmarks().Select(bi => bi.BookmarkName).ToList();
        }

        /// <summary>
        /// Gets a list of bookmark names
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="timeout">
        /// The interval in which this method must complete before the operation is canceled and a TimeoutException is thrown. 
        /// </param>
        /// <returns>
        /// The list of bookmarks 
        /// </returns>
        public static IEnumerable<string> GetBookmarkNames(
            this WorkflowApplication workflowApplication, TimeSpan timeout)
        {
            Debug.Assert(workflowApplication != null, "workflowApplication != null");

            // ReSharper disable PossibleNullReferenceException
            return workflowApplication.GetBookmarks(timeout).Select(bi => bi.BookmarkName).ToList();

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Gets a readonly collection of singleton extensions
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// a readonly collection of singleton extensions 
        /// </returns>
        public static ICollection<object> GetSingletonExtensions(this WorkflowApplication workflowApplication)
        {
            if (workflowApplication == null)
            {
                return new Collection<object>();
            }

            dynamic wfapp = new ReflectionObject(workflowApplication);

            var o = wfapp.extensions;
            if (o == null)
            {
                return new Collection<object>();
            }

            dynamic extensions = new ReflectionObject(o);

            return extensions != null && extensions.SingletonExtensions != null
                       ? (ICollection<object>)new ReadOnlyCollection<object>(extensions.SingletonExtensions)
                       : new Collection<object>();
        }

        /// <summary>
        /// Gets a value that determines if a workflow has aborted
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// true if the workflow has aborted, false if not 
        /// </returns>
        public static bool IsUnloaded(this WorkflowApplication workflowApplication)
        {
            dynamic wfapp = new ReflectionObject(workflowApplication);

            var state = (WorkflowApplicationState)wfapp.state;
            return state == WorkflowApplicationState.Unloaded;
        }

        /// <summary>
        /// Gets a value that determines if a workflow has aborted
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// true if the workflow has aborted, false if not 
        /// </returns>
        public static bool IsAborted(this WorkflowApplication workflowApplication)
        {
            dynamic wfapp = new ReflectionObject(workflowApplication);

            var state = (WorkflowApplicationState)wfapp.state;
            return state == WorkflowApplicationState.Aborted;
        }

        /// <summary>
        /// Gets a value that determines if a workflow is in a handler thread
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// true if in a handler thread 
        /// </returns>
        public static bool IsHandlerThread(this WorkflowApplication workflowApplication)
        {
            dynamic wfapp = new ReflectionObject(workflowApplication);

            return wfapp.IsHandlerThread;
        }

        /// <summary>
        /// Gets a value that determines if a WorkflowApplication is initialized
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// true if the workflow has aborted, false if not 
        /// </returns>
        public static bool IsInitialized(this WorkflowApplication workflowApplication)
        {
            dynamic wfapp = new ReflectionObject(workflowApplication);

            return wfapp.isInitialized;
        }

        /// <summary>
        /// Gets a value that determines if a workflow is in a handler thread
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// true if in a handler thread 
        /// </returns>
        public static bool IsReadOnly(this WorkflowApplication workflowApplication)
        {
            dynamic wfapp = new ReflectionObject(workflowApplication);

            return wfapp.IsReadOnly;
        }

        /// <summary>
        /// Wraps the BeginLoad / EndLoad async methods in a task
        /// </summary>
        /// <param name="host">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="instanceId">
        /// The instance ID 
        /// </param>
        /// <param name="timeout">
        /// The load timeout 
        /// </param>
        /// <returns>
        /// A Task that will load the WorkflowApplication from the instance store 
        /// </returns>
        public static Task LoadAsync(this WorkflowApplication host, Guid instanceId, TimeSpan timeout)
        {
            return Task.Factory.FromAsync(host.BeginLoad, host.EndLoad, instanceId, timeout, null);
        }

        /// <summary>
        /// Wraps the BeginLoad / EndLoad async methods in a task
        /// </summary>
        /// <param name="host">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="instanceId">
        /// The instance ID 
        /// </param>
        /// <returns>
        /// A Task that will load the WorkflowApplication from the instance store 
        /// </returns>
        public static Task LoadAsync(this WorkflowApplication host, Guid instanceId)
        {
            return Task.Factory.FromAsync(host.BeginLoad, host.EndLoad, instanceId, null);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName)
        {
            return ResumeEpisodeBookmark(workflowApplication, bookmarkName, null);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, TimeSpan timeout)
        {
            return ResumeEpisodeBookmark(workflowApplication, bookmarkName, null, timeout);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, string waitForBookmarkName)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value, waitForBookmarkName);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            string waitForBookmarkName, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value, waitForBookmarkName);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value, idleEventCallback);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.ResumeEpisodeBookmark(bookmarkName, value, idleEventCallback);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, T bookmarkName)
        {
            return ResumeEpisodeBookmarkAsync(workflowApplication, bookmarkName, null);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { CancellationToken = token };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, token);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, idleEventCallback);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, idleEventCallback);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The token. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, idleEventCallback, token);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// Run the episode until completed, aborted or an idle with a bookmark matching this name 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, string waitForBookmarkName)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, waitForBookmarkName);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// Run the episode until completed, aborted or an idle with a bookmark matching this name 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            string waitForBookmarkName, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { CancellationToken = token };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, waitForBookmarkName, token);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// Run the episode until completed, aborted or an idle with a bookmark matching this name 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            string waitForBookmarkName, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, waitForBookmarkName);
        }

        /// <summary>
        /// Creates a task that will resume a bookmark and run the workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="bookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmark with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// Run the episode until completed, aborted or an idle with a bookmark matching this name 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            string waitForBookmarkName, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.ResumeEpisodeBookmarkAsync(bookmarkName, value, waitForBookmarkName, token);
        }

        /// <summary>
        /// Resume the workflow and run until idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarkName">
        /// The name of the bookmark to resume will be converted to string 
        /// </param>
        /// <param name="value">
        /// The value to pass to the activity 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeUntilAnyBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, TimeSpan timeout)
        {
            WorkflowTrace.Information(
                "ResumeUntilBookmark resuming bookmark {0} with value {1} and will run until idle with any bookmark", 
                bookmarkName, 
                value);

            return workflowApplication.ResumeEpisodeBookmark(
                bookmarkName, value, (args, s) => args.Bookmarks.Any(), timeout);
        }

        /// <summary>
        /// Resume the workflow and run until idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarkName">
        /// The name of the bookmark to resume will be converted to string 
        /// </param>
        /// <param name="value">
        /// The value to pass to the activity 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeUntilAnyBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value)
        {
            WorkflowTrace.Information(
                "ResumeUntilBookmark resuming bookmark {0} with value {1} and will run until idle with any bookmark", 
                bookmarkName, 
                value);

            return workflowApplication.ResumeEpisodeBookmark(bookmarkName, value, (args, s) => args.Bookmarks.Any());
        }

        /// <summary>
        /// Resume the workflow and run until idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarkName">
        /// The name of the bookmark to resume will be converted to string 
        /// </param>
        /// <param name="value">
        /// The value to pass to the activity 
        /// </param>
        /// <param name="bookmarks">
        /// A list of bookmarks to wait for. If the list is empty any bookmark will suffice 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeUntilBookmark<T>(
            this WorkflowApplication workflowApplication, T bookmarkName, object value, params T[] bookmarks)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            Contract.Requires(typeof(T) != typeof(TimeSpan));
            if (typeof(T) == typeof(TimeSpan))
            {
                throw new InvalidOperationException(
                    "bookmarks of type TimeSpan detected.  To run until any bookmark with a timeout use RunUntilAnyBookmark");
            }

            TraceResumeUntil(bookmarkName, value, bookmarks);

            return workflowApplication.ResumeEpisodeBookmark(
                bookmarkName, 
                value, 
                (args, s) => // If no bookmarks provided
                bookmarks.Length == 0
                    ? // Any bookmark will do
                args.Bookmarks.Count > 0
                    : // Otherwise, check for the intersection of the two sets
                IntersectAny(bookmarks, args));
        }

        /// <summary>
        /// Resume the workflow and run until idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark name 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarkName">
        /// The name of the bookmark to resume 
        /// </param>
        /// <param name="value">
        /// The value to pass to the activity 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <param name="bookmarks">
        /// A list of bookmarks to wait for. If the list is empty any bookmark will suffice 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult ResumeUntilBookmark<T>(
            this WorkflowApplication workflowApplication, 
            T bookmarkName, 
            object value, 
            TimeSpan timeout, 
            params T[] bookmarks)
        {
            TraceResumeUntil(bookmarkName, value, bookmarks);

            return workflowApplication.ResumeEpisodeBookmark(
                bookmarkName, 
                value, 
                (args, s) => // If no bookmarks provided
                bookmarks.Length == 0
                    ? // Any bookmark will do
                args.Bookmarks.Count > 0
                    : // Otherwise, check for the intersection of the two sets
                IntersectAny(bookmarks, args), 
                timeout);
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(this WorkflowApplication workflowApplication)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisode();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(this WorkflowApplication workflowApplication, TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.RunEpisode();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandledExceptionAction. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, UnhandledExceptionAction unhandledExceptionAction)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction 
                };
            return workflowEpisode.RunEpisode();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandledExceptionAction. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout 
                };
            return workflowEpisode.RunEpisode();
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, string waitForBookmarkName)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisode(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction 
                };
            return workflowEpisode.RunEpisode(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, string waitForBookmarkName, TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.RunEpisode(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout 
                };
            return workflowEpisode.RunEpisode(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisode(idleEventCallback);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunEpisode(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.RunEpisode(idleEventCallback);
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <returns>
        /// A task that will run the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(this WorkflowApplication workflowApplication)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisodeAsync();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task that will run the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.RunEpisodeAsync();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandledExceptionAction. 
        /// </param>
        /// <returns>
        /// A task that will run the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, UnhandledExceptionAction unhandledExceptionAction)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction 
                };
            return workflowEpisode.RunEpisodeAsync();
        }

        /// <summary>
        /// Runs a workflow until the activity is Closed, Faulted or Timeout
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandledExceptionAction. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task that will run the workflow 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout 
                };
            return workflowEpisode.RunEpisodeAsync();
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, string waitForBookmarkName)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction 
                };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, string waitForBookmarkName, TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { Timeout = timeout };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout 
                };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, string waitForBookmarkName, CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { CancellationToken = token };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The bookmark name. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            string waitForBookmarkName, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(waitForBookmarkName, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication);
            return workflowEpisode.RunEpisodeAsync(idleEventCallback);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            UnhandledExceptionAction unhandledExceptionAction)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction 
                };
            return workflowEpisode.RunEpisodeAsync(idleEventCallback);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout 
                };
            return workflowEpisode.RunEpisodeAsync(idleEventCallback);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { CancellationToken = token };
            return workflowEpisode.RunEpisodeAsync(idleEventCallback, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            UnhandledExceptionAction unhandledExceptionAction, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(idleEventCallback, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(idleEventCallback, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication) { CancellationToken = token };
            return workflowEpisode.RunEpisodeAsync(token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            UnhandledExceptionAction unhandledExceptionAction, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="unhandledExceptionAction">
        /// The unhandled Exception Action. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the workflow episode 
        /// </returns>
        public static Task<WorkflowEpisodeResult> RunEpisodeAsync(
            this WorkflowApplication workflowApplication, 
            UnhandledExceptionAction unhandledExceptionAction, 
            TimeSpan timeout, 
            CancellationToken token)
        {
            var workflowEpisode = new WorkflowEpisode(workflowApplication)
                {
                   UnhandledExceptionAction = unhandledExceptionAction, Timeout = timeout, CancellationToken = token 
                };
            return workflowEpisode.RunEpisodeAsync(token);
        }

        /// <summary>
        /// Run the workflow until it becomes idle with any bookmark
        /// </summary>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunUntilAnyBookmark(this WorkflowApplication workflowApplication)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            WorkflowTrace.Information("RunUntilBookmark will run until idle with any bookmark");

            return workflowApplication.RunEpisode((args, s) => args.Bookmarks.Any());
        }

        /// <summary>
        /// Run the workflow until it becomes idle with any bookmark
        /// </summary>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunUntilAnyBookmark(
            this WorkflowApplication workflowApplication, TimeSpan timeout)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            WorkflowTrace.Information("RunUntilBookmark will run until idle with any bookmark");

            return workflowApplication.RunEpisode((args, s) => args.Bookmarks.Any(), timeout);
        }

        /// <summary>
        /// Run the workflow until it becomes idle with one of the bookmarks
        /// </summary>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarks">
        /// The bookmarks 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunUntilBookmark(
            this WorkflowApplication workflowApplication, params object[] bookmarks)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            TraceRunUntil(bookmarks);
            return workflowApplication.RunEpisode(
                (args, s) => // If no bookmarks provided
                bookmarks.Length == 0
                    ? // Any bookmark will do
                args.Bookmarks.Count > 0
                    : // Otherwise, check for the intersection of the two sets
                IntersectAny(bookmarks, args));
        }

        /// <summary>
        /// Run the workflow until it becomes idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmarks 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="bookmarks">
        /// The bookmarks 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunUntilBookmark<T>(
            this WorkflowApplication workflowApplication, params T[] bookmarks)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            Contract.Requires(typeof(T) != typeof(TimeSpan));
            if (typeof(T) == typeof(TimeSpan))
            {
                throw new InvalidOperationException(
                    "bookmarks of type TimeSpan detected.  To run until any bookmark with a timeout use RunUntilAnyBookmark");
            }

            TraceRunUntil(bookmarks);
            return workflowApplication.RunEpisode(
                (args, s) => // If no bookmarks provided
                bookmarks.Length == 0
                    ? // Any bookmark will do
                args.Bookmarks.Count > 0
                    : // Otherwise, check for the intersection of the two sets
                IntersectAny(bookmarks, args));
        }

        /// <summary>
        /// Run the workflow until it becomes idle with one of the bookmarks
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmarks 
        /// </typeparam>
        /// <param name="workflowApplication">
        /// The WorkflowApplication 
        /// </param>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <param name="bookmarks">
        /// The bookmarks 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        public static WorkflowEpisodeResult RunUntilBookmark<T>(
            this WorkflowApplication workflowApplication, TimeSpan timeout, params T[] bookmarks)
        {
            Contract.Requires(workflowApplication != null);
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            TraceRunUntil(bookmarks);

            return workflowApplication.RunEpisode((args, s) => IntersectAny(bookmarks, args), timeout);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the intersection of two sets results in any items
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmarks values 
        /// </typeparam>
        /// <param name="bookmarks">
        /// The bookmarks 
        /// </param>
        /// <param name="args">
        /// The idle args 
        /// </param>
        /// <returns>
        /// true if a value exists in both 
        /// </returns>
        private static bool IntersectAny<T>(IEnumerable<T> bookmarks, WorkflowApplicationIdleEventArgs args)
        {
            return
                args.Bookmarks.Select(b => b.BookmarkName).Intersect(
                    bookmarks.Select(arg => !typeof(T).IsValueType && Equals(arg, default(T)) ? null : arg.ToString())).
                    Any();
        }

        /// <summary>
        /// Determines if the bookmarkName is null or empty
        /// </summary>
        /// <typeparam name="T">
        /// The type of the bookmark 
        /// </typeparam>
        /// <param name="bookmarkName">
        /// The bookmarkj name 
        /// </param>
        /// <returns>
        /// true if null or empty, false if not 
        /// </returns>
        private static bool IsNullOrEmptyBookmark<T>(T bookmarkName)
        {
            return typeof(T) == typeof(string)
                       ? string.IsNullOrWhiteSpace(bookmarkName as string)
                   
                   
                   
                   // ReSharper disable CompareNonConstrainedGenericWithNull
                   // If T is a value type, bookmark name will never be null which is ok in this method
                       : bookmarkName == null;

            // ReSharper restore CompareNonConstrainedGenericWithNull
        }

        /// <summary>
        /// The trace resume until.
        /// </summary>
        /// <param name="bookmark">
        /// The bookmark. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        /// <param name="bookmarks">
        /// The bookmarks. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the bookmarks 
        /// </typeparam>
        [Conditional("TRACE")]
        private static void TraceResumeUntil<T>(T bookmark, object value, T[] bookmarks)
        {
            if (bookmarks.Length == 0)
            {
                WorkflowTrace.Information(
                    "ResumeUntilBookmark resuming bookmark {0} with value \"{1}\" and will run until idle with any bookmark", 
                    bookmark, 
                    value);
            }
            else
            {
                WorkflowTrace.Information(
                    "ResumeUntilBookmark resuming bookmark {0} with value \"{1}\" and will run until idle with a bookmark in the set [{2}]", 
                    bookmark, 
                    value, 
                    bookmarks.ToDelimitedList());
            }
        }

        /// <summary>
        /// Traces a RunUntil condition
        /// </summary>
        /// <typeparam name="T">
        /// The bookmark type 
        /// </typeparam>
        /// <param name="bookmarks">
        /// The bookmarks 
        /// </param>
        [Conditional("TRACE")]
        private static void TraceRunUntil<T>(T[] bookmarks)
        {
            if (bookmarks.Length == 0)
            {
                WorkflowTrace.Information("RunUntilBookmark will run until idle with any bookmark");
            }
            else
            {
                WorkflowTrace.Information(
                    "RunUntilBookmark will run until idle with a bookmark in the set [{0}]", bookmarks.ToDelimitedList());
            }
        }

        #endregion
    }
}