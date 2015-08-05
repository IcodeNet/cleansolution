// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowEpisode.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Tracking;

    /// <summary>
    ///   The workflow episode.
    /// </summary>
    public class WorkflowEpisode : IDisposable
    {
        #region Fields

        /// <summary>
        ///   The workflow busy.
        /// </summary>
        private readonly ManualResetEvent workflowBusy = new ManualResetEvent(false);

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        ///   The episode result.
        /// </summary>
        private WorkflowEpisodeResult episodeResult;

        /// <summary>
        ///   The handler thread id.
        /// </summary>
        private int handlerThreadId = -1;

        /// <summary>
        ///   Function to determine if the idleEpisode is over
        /// </summary>
        private Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventHandler;

        /// <summary>
        ///   The is in handler.
        /// </summary>
        private bool isInHandler;

        /// <summary>
        ///   The target activity.
        /// </summary>
        private string targetBookmark;

        /// <summary>
        ///   The timeout.
        /// </summary>
        private TimeSpan timeout = Debugger.IsAttached ? DefaultDebugTimeout : DefaultTimeout;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="WorkflowEpisode" /> class.
        /// </summary>
        static WorkflowEpisode()
        {
            DefaultDebugTimeout = TimeSpan.FromMinutes(3);
            DefaultTimeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEpisode"/> class.
        /// </summary>
        /// <param name="activity">
        /// The activity. 
        /// </param>
        public WorkflowEpisode(Activity activity)
        {
            this.WorkflowApplication = new WorkflowApplication(activity);
            this.SaveDelegates();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEpisode"/> class.
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        public WorkflowEpisode(WorkflowApplication workflowApplication)
        {
            this.WorkflowApplication = workflowApplication;
            this.SaveDelegates();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the default debug timeout.
        /// </summary>
        public static TimeSpan DefaultDebugTimeout { get; set; }

        /// <summary>
        ///   Gets or sets The default timeout.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        ///   Gets or sets CancellationToken.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        ///   Gets or sets Timeout.
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                this.timeout = value;
            }
        }

        /// <summary>
        ///   Gets or sets UnhandledExceptionAction.
        /// </summary>
        public UnhandledExceptionAction UnhandledExceptionAction { get; set; }

        /// <summary>
        ///   Gets WorkflowApplication.
        /// </summary>
        public WorkflowApplication WorkflowApplication { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Aborted.
        /// </summary>
        protected Action<WorkflowApplicationAbortedEventArgs> Aborted { get; set; }

        /// <summary>
        ///   Gets or sets Completed.
        /// </summary>
        protected Action<WorkflowApplicationCompletedEventArgs> Completed { get; set; }

        /// <summary>
        ///   Gets or sets Idle.
        /// </summary>
        protected Action<WorkflowApplicationIdleEventArgs> Idle { get; set; }

        /// <summary>
        ///   Gets or sets UnhandledException.
        /// </summary>
        protected Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction> OnUnhandledException { get; set; }

        /// <summary>
        ///   Gets or sets PersistableIdle.
        /// </summary>
        protected Func<WorkflowApplicationIdleEventArgs, PersistableIdleAction> PersistableIdle { get; set; }

        /// <summary>
        ///   Gets or sets the unloaded delegate.
        /// </summary>
        protected Action<WorkflowApplicationEventArgs> Unloaded { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Function that will end the episode when an idle with the target bookmarkName name is reached
        /// </summary>
        /// <param name="args">
        /// The idle event args. 
        /// </param>
        /// <param name="targetBookmarkName">
        /// The target Bookmark. 
        /// </param>
        /// <returns>
        /// true if the idle arguments contains a bookmarkName with the target bookmarkName name 
        /// </returns>
        public bool IdleWithBookmark(WorkflowApplicationIdleEventArgs args, string targetBookmarkName)
        {
            return args.ContainsBookmark(targetBookmarkName);
        }

        /// <summary>
        /// Resumes the workflow bookmarkName and runs until the workflow is completed, aborted or timed out
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        /// <exception cref="BookmarkResumptionException">
        /// Failed to resume bookmarkName
        /// </exception>
        public WorkflowEpisodeResult ResumeEpisodeBookmark<T>(T bookmarkName, object value)
        {
            return this.InternalResumeBookmark(bookmarkName, value, null, null);
        }

        /// <summary>
        /// Resumes the workflow bookmarkName and runs until the workflow is completed, aborted, timed out or idle with a specific bookmarkName
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        /// <exception cref="BookmarkResumptionException">
        /// Failed to resume bookmarkName
        /// </exception>
        public WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            T bookmarkName, object value, string waitForBookmarkName)
        {
            return this.InternalResumeBookmark(bookmarkName, value, waitForBookmarkName, null);
        }

        /// <summary>
        /// Resumes the workflow bookmarkName and runs until the workflow is completed, aborted, timed out or idle where the callback indicates the episode should end
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <returns>
        /// The <see cref="WorkflowEpisodeResult"/> that ended the episode 
        /// </returns>
        /// <exception cref="BookmarkResumptionException">
        /// Failed to resume bookmarkName
        /// </exception>
        public WorkflowEpisodeResult ResumeEpisodeBookmark<T>(
            T bookmarkName, object value, Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            Contract.Requires(idleEventCallback != null);

            if (idleEventCallback == null)
            {
                throw new ArgumentNullException("idleEventCallback");
            }

            return this.InternalResumeBookmark(bookmarkName, value, null, idleEventCallback);
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted or timed out
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(T bookmarkName, object value)
        {
            Debug.Assert(Task<WorkflowEpisodeResult>.Factory != null, "Task<WorkflowEpisodeResult>.Factory != null");
            return Task<WorkflowEpisodeResult>.Factory.StartNew(() => this.ResumeEpisodeBookmark(bookmarkName, value));
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted or timed out
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            T bookmarkName, object value, CancellationToken token)
        {
            Debug.Assert(Task<WorkflowEpisodeResult>.Factory != null, "Task<WorkflowEpisodeResult>.Factory != null");
            return Task<WorkflowEpisodeResult>.Factory.StartNew(
                () => this.ResumeEpisodeBookmark(bookmarkName, value), token);
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted, timed out or idle with a specific bookmarkName
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            T bookmarkName, object value, string waitForBookmarkName)
        {
            Debug.Assert(Task<WorkflowEpisodeResult>.Factory != null, "Task<WorkflowEpisodeResult>.Factory != null");
            return
                Task<WorkflowEpisodeResult>.Factory.StartNew(
                    () => this.ResumeEpisodeBookmark(bookmarkName, value, waitForBookmarkName));
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted, timed out or idle with a specific bookmarkName
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            T bookmarkName, object value, string waitForBookmarkName, CancellationToken token)
        {
            return
                Task<WorkflowEpisodeResult>.Factory.StartNew(
                    () => this.ResumeEpisodeBookmark(bookmarkName, value, waitForBookmarkName), token);
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted, timed out or idle where the callback indicates the episode should end
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            T bookmarkName, object value, Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            return
                Task<WorkflowEpisodeResult>.Factory.StartNew(
                    () => this.ResumeEpisodeBookmark(bookmarkName, value, idleEventCallback));
        }

        /// <summary>
        /// Returns a task that will resume a bookmarkName and run until the workflow is completed, aborted, timed out or idle where the callback indicates the episode should end
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task that will resume the bookmarkName 
        /// </returns>
        public Task<WorkflowEpisodeResult> ResumeEpisodeBookmarkAsync<T>(
            T bookmarkName, 
            object value, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, 
            CancellationToken token)
        {
            return
                Task<WorkflowEpisodeResult>.Factory.StartNew(
                    () => this.ResumeEpisodeBookmark(bookmarkName, value, idleEventCallback), token);
        }

        /// <summary>
        /// Runs an episode synchronously
        /// </summary>
        /// <param name="idleEventCallback">
        /// Function that determines if a given idle event should end the episode 
        /// </param>
        /// <returns>
        /// An episode result 
        /// </returns>
        /// <exception cref="TimeoutException">
        /// The episode did not complete before timeout
        /// </exception>
        public WorkflowEpisodeResult RunEpisode(Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            Contract.Requires(idleEventCallback != null);

            if (idleEventCallback == null)
            {
                throw new ArgumentNullException("idleEventCallback");
            }

            return this.InternalRunEpisode(null, idleEventCallback);
        }

        /// <summary>
        /// Runs an episode synchronously
        /// </summary>
        /// <param name="waitForBookmarkName">
        /// The workflow will run until an idle event which contains this bookmarkName name. 
        /// </param>
        /// <returns>
        /// An episode result 
        /// </returns>
        /// <exception cref="TimeoutException">
        /// The episode did not complete before timeout
        /// </exception>
        public WorkflowEpisodeResult RunEpisode(string waitForBookmarkName)
        {
            return this.InternalRunEpisode(waitForBookmarkName, this.IdleWithBookmark);
        }

        /// <summary>
        ///   Runs an episode synchronously
        /// </summary>
        /// <returns> An episode result </returns>
        /// <exception cref="TimeoutException">The episode did not complete before timeout</exception>
        public WorkflowEpisodeResult RunEpisode()
        {
            return this.InternalRunEpisode(null, null);
        }

        /// <summary>
        ///   Returns a task that runs a workflow episode
        /// </summary>
        /// <returns> A task for running the activity async </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync()
        {
            return Task<WorkflowEpisodeResult>.Factory.StartNew(this.RunEpisode);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="token">
        /// The cancellation token. 
        /// </param>
        /// <returns>
        /// A task for running the activity async 
        /// </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync(CancellationToken token)
        {
            return Task<WorkflowEpisodeResult>.Factory.StartNew(this.RunEpisode, token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="waitForBookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <returns>
        /// A task for running the workflow until an idle event with the given bookmarkName occurs 
        /// </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync(string waitForBookmarkName)
        {
            return Task<WorkflowEpisodeResult>.Factory.StartNew(() => this.RunEpisode(waitForBookmarkName));
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="waitForBookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow until an idle event with the given bookmarkName occurs 
        /// </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync(string waitForBookmarkName, CancellationToken token)
        {
            return Task<WorkflowEpisodeResult>.Factory.StartNew(() => this.RunEpisode(waitForBookmarkName), token);
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <returns>
        /// A task for running the workflow until an idle event with the given bookmarkName occurs 
        /// </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync(
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            Contract.Requires(idleEventCallback != null);

            if (idleEventCallback == null)
            {
                throw new ArgumentNullException("idleEventCallback");
            }

            return Task<WorkflowEpisodeResult>.Factory.StartNew(() => this.RunEpisode(idleEventCallback));
        }

        /// <summary>
        /// Returns a task that runs a workflow episode
        /// </summary>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <param name="token">
        /// The cancellation token 
        /// </param>
        /// <returns>
        /// A task for running the workflow until an idle event with the given bookmarkName occurs 
        /// </returns>
        public Task<WorkflowEpisodeResult> RunEpisodeAsync(
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback, CancellationToken token)
        {
            Contract.Requires(idleEventCallback != null);

            if (idleEventCallback == null)
            {
                throw new ArgumentNullException("idleEventCallback");
            }

            return Task<WorkflowEpisodeResult>.Factory.StartNew(() => this.RunEpisode(idleEventCallback), token);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes of resources
        /// </summary>
        /// <param name="disposing">
        /// The disposing. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                    this.workflowBusy.Close();
                }

                // free native resources
                this.disposed = true;
            }
        }

        /// <summary>
        ///   The check for cancel.
        /// </summary>
        private void CheckForCancel()
        {
            if (this.CancellationToken.IsCancellationRequested)
            {
                if (!this.IsInHandler())
                {
                    this.WorkflowApplication.Cancel(this.Timeout);
                }

                this.CancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        ///   The enter handler.
        /// </summary>
        private void EnterHandler()
        {
            this.isInHandler = true;
            this.handlerThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        ///   The exit handler.
        /// </summary>
        private void ExitHandler()
        {
            this.isInHandler = false;
            this.handlerThreadId = -1;
        }

        /// <summary>
        /// The on abort.
        /// </summary>
        /// <param name="abortedEventArgs">
        /// The abort event args. 
        /// </param>
        private void InternalOnAborted(WorkflowApplicationAbortedEventArgs abortedEventArgs)
        {
            try
            {
                this.EnterHandler();
                this.episodeResult = new WorkflowAbortedEpisodeResult(abortedEventArgs, this.WorkflowApplication.Id);

                if (this.Aborted != null)
                {
                    this.Aborted(abortedEventArgs);
                }
            }
            finally
            {
                this.ExitHandler();
                this.workflowBusy.Set();
            }
        }

        /// <summary>
        /// The Completed Delegate
        /// </summary>
        /// <param name="completedEventArgs">
        /// The event args. 
        /// </param>
        private void InternalOnCompleted(WorkflowApplicationCompletedEventArgs completedEventArgs)
        {
            try
            {
                this.EnterHandler();
                this.episodeResult = new WorkflowCompletedEpisodeResult(completedEventArgs, this.WorkflowApplication.Id);

                if (this.Completed != null)
                {
                    this.Completed(completedEventArgs);
                }
            }
            finally
            {
                this.ExitHandler();
                this.workflowBusy.Set();
            }
        }

        /// <summary>
        /// Handles the Idle event
        /// </summary>
        /// <param name="idleEventArgs">
        /// The event args. 
        /// </param>
        private void InternalOnIdle(WorkflowApplicationIdleEventArgs idleEventArgs)
        {
            var episodeCompleted = false;

            try
            {
                this.EnterHandler();
#if DEBUG
                WorkflowTrace.Information(
                    "\r\nWorkflow Idle: {0} [{1}]", 
                    this.WorkflowApplication.WorkflowDefinition.DisplayName, 
                    this.WorkflowApplication.Id);
                WorkflowTrace.Information(
                    "Pending Bookmarks: {0}\r\n", 
                    idleEventArgs.Bookmarks.Select(info => info.BookmarkName).ToDelimitedList());
#endif

                // If there is an idle event handler, the episode can end on idle depending on the handler result
                if (this.idleEventHandler != null && this.idleEventHandler(idleEventArgs, this.targetBookmark))
                {
                    // The idle handler exists and indicates the episode should complete
                    this.episodeResult = new WorkflowIdleEpisodeResult(idleEventArgs, this.WorkflowApplication.Id);
                    episodeCompleted = true;
                }

                if (this.Idle != null)
                {
                    this.Idle(idleEventArgs);
                }

                if (this.CancellationToken.IsCancellationRequested)
                {
                    episodeCompleted = true;
                }
            }
            finally
            {
                this.ExitHandler();

                if (episodeCompleted)
                {
                    this.workflowBusy.Set();
                }
            }
        }

        /// <summary>
        /// The internal on persistable idle.
        /// </summary>
        /// <param name="idleEventArgs">
        /// The idle Event Args. 
        /// </param>
        /// <returns>
        /// A persistable idle action 
        /// </returns>
        private PersistableIdleAction InternalOnPersistableIdle(WorkflowApplicationIdleEventArgs idleEventArgs)
        {
            var result = PersistableIdleAction.None;
            try
            {
                this.EnterHandler();

                if (this.PersistableIdle != null)
                {
                    result = this.PersistableIdle(idleEventArgs);
                }

                if (result == PersistableIdleAction.Unload)
                {
                    // If unloading the episode is complete
                    this.episodeResult = new WorkflowIdleEpisodeResult(idleEventArgs, this.WorkflowApplication.Id);
                }
            }
            finally
            {
                this.ExitHandler();
            }

            return result;
        }

        /// <summary>
        /// Returns an action that tells the workflow runtime what to do when an unhandeled exception is caught
        /// </summary>
        /// <param name="unhandledExceptionEventArgs">
        /// The unhandledExceptionEventArgs. 
        /// </param>
        /// <returns>
        /// The unhandeled exception action 
        /// </returns>
        private UnhandledExceptionAction InternalOnUnhandledException(
            WorkflowApplicationUnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                this.EnterHandler();

                return this.OnUnhandledException != null
                           ? this.OnUnhandledException(unhandledExceptionEventArgs)
                           : this.UnhandledExceptionAction;
            }
            finally
            {
                this.ExitHandler();
            }
        }

        /// <summary>
        /// The internal on unloaded.
        /// </summary>
        /// <param name="workflowApplicationEventArgs">
        /// The workflowApplicationEventArgs. 
        /// </param>
        private void InternalOnUnloaded(WorkflowApplicationEventArgs workflowApplicationEventArgs)
        {
            // Unloaded gets called when completed or when Persistable Idle func returns Unload
            var workflowIdleEpisodeResult = this.episodeResult as WorkflowIdleEpisodeResult;
            if (workflowIdleEpisodeResult != null)
            {
                // Indicates that the idle ended with an unload
                workflowIdleEpisodeResult.Unloaded = true;
            }

            if (this.Unloaded != null)
            {
                this.Unloaded(workflowApplicationEventArgs);
            }

            this.workflowBusy.Set();
        }

        /// <summary>
        /// The internal resume bookmarkName.
        /// </summary>
        /// <param name="bookmarkName">
        /// The bookmarkName name. 
        /// </param>
        /// <param name="value">
        /// The value to resume the bookmarkName with 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <exception cref="BookmarkResumptionException">
        /// Unable to resume the bookmarkName
        /// </exception>
        /// <returns>
        /// The episode result 
        /// </returns>
        private WorkflowEpisodeResult InternalResumeBookmark<T>(
            T bookmarkName, 
            object value, 
            string waitForBookmarkName, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            return this.InternalRunEpisode(
                () =>
                    {
                        var result = this.WorkflowApplication.ResumeBookmark(bookmarkName.ToString(), value);
                        if (result != BookmarkResumptionResult.Success)
                        {
                            throw new BookmarkResumptionException(
                                bookmarkName.ToString(), 
                                result, 
                                value);
                        }
                    }, 
                waitForBookmarkName, 
                idleEventCallback);
        }

        /// <summary>
        /// The internal run episode.
        /// </summary>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        private WorkflowEpisodeResult InternalRunEpisode(
            string waitForBookmarkName, Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            return this.InternalRunEpisode(this.WorkflowApplication.Run, waitForBookmarkName, idleEventCallback);
        }

        /// <summary>
        /// The internal run episode.
        /// </summary>
        /// <param name="runAction">
        /// The run action. 
        /// </param>
        /// <param name="waitForBookmarkName">
        /// The wait For Bookmark Name. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle Event Callback. 
        /// </param>
        /// <returns>
        /// The episode result 
        /// </returns>
        private WorkflowEpisodeResult InternalRunEpisode(
            Action runAction, 
            string waitForBookmarkName, 
            Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            try
            {
                this.PrepareEpisode(waitForBookmarkName, idleEventCallback);
                runAction();
                this.WaitForWorkflow();
                return this.episodeResult;
            }
            finally
            {
                this.RestoreDelegates();
            }
        }

        /// <summary>
        ///   The is in handler.
        /// </summary>
        /// <returns> True if the thread is called from a handler </returns>
        private bool IsInHandler()
        {
            return this.isInHandler && (this.handlerThreadId == Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// Prepares the WorkflowEpisode to start a new episode
        /// </summary>
        /// <param name="waitForBookmarkName">
        /// The wait for bookmarkName name. 
        /// </param>
        /// <param name="idleEventCallback">
        /// The idle event callback. 
        /// </param>
        private void PrepareEpisode(
            string waitForBookmarkName, Func<WorkflowApplicationIdleEventArgs, string, bool> idleEventCallback)
        {
            this.targetBookmark = waitForBookmarkName;

            // If you provide a targetBookmark the idleEventCallback will not be used
            this.idleEventHandler = !string.IsNullOrWhiteSpace(this.targetBookmark)
                                        ? this.IdleWithBookmark
                                        : idleEventCallback;

            this.workflowBusy.Reset();
        }

        /// <summary>
        ///   Restores the delegates of WorkflowApplication
        /// </summary>
        private void RestoreDelegates()
        {
            this.WorkflowApplication.Aborted = this.Aborted;
            this.WorkflowApplication.Idle = this.Idle;
            this.WorkflowApplication.Completed = this.Completed;
            this.WorkflowApplication.OnUnhandledException = this.OnUnhandledException;
        }

        /// <summary>
        ///   Saves the delegates
        /// </summary>
        private void SaveDelegates()
        {
            this.Aborted = this.WorkflowApplication.Aborted;
            this.WorkflowApplication.Aborted = this.InternalOnAborted;

            this.Completed = this.WorkflowApplication.Completed;
            this.WorkflowApplication.Completed = this.InternalOnCompleted;

            this.Idle = this.WorkflowApplication.Idle;
            this.WorkflowApplication.Idle = this.InternalOnIdle;

            this.PersistableIdle = this.WorkflowApplication.PersistableIdle;
            this.WorkflowApplication.PersistableIdle = this.InternalOnPersistableIdle;

            this.OnUnhandledException = this.WorkflowApplication.OnUnhandledException;
            this.WorkflowApplication.OnUnhandledException = this.InternalOnUnhandledException;

            this.Unloaded = this.WorkflowApplication.Unloaded;
            this.WorkflowApplication.Unloaded = this.InternalOnUnloaded;
        }

        /// <summary>
        ///   Waits for a workflow episode
        /// </summary>
        /// <exception cref="TimeoutException">The workflow did not resume in time</exception>
        private void WaitForWorkflow()
        {
            this.CheckForCancel();

            Debug.Assert(this.workflowBusy != null, "this.workflowBusy != null");

            if (!this.workflowBusy.WaitOne(this.Timeout))
            {
                throw new TimeoutException();
            }

            this.CheckForCancel();

            // Special handling for completed and aborted results
            if (this.episodeResult is WorkflowCompletedEpisodeResult)
            {
                var workflowCompletedEpisodeResult = (WorkflowCompletedEpisodeResult)this.episodeResult;

                if (this.episodeResult.State == ActivityInstanceState.Canceled)
                {
                    throw new TaskCanceledException(
                        "Activity Canceled", workflowCompletedEpisodeResult.TerminationException);
                }

                // UnhandledExceptionAction.Terminate can cause this
                if (this.episodeResult.State == ActivityInstanceState.Faulted)
                {
                    throw workflowCompletedEpisodeResult.TerminationException;
                }
            }
            else if (this.episodeResult is WorkflowAbortedEpisodeResult)
            {
                // If an exception occured during the episode throw it on the calling thread
                throw ((WorkflowAbortedEpisodeResult)this.episodeResult).Reason;
            }

            // No special handling required for idle results
        }

        #endregion
    }
}