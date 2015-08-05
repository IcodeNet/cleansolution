// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting
{
    using System;
    using System.Activities;
    using System.Activities.Hosting;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.DurableInstancing;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.UnitTesting.Properties;
    using Microsoft.Activities.UnitTesting.Tracking;

    /// <summary>
    ///   Non Generic Overload to provide factory method
    /// </summary>
    // ReSharper disable ClassNeverInstantiated.Global
    public static class WorkflowApplicationTest
    {
        // ReSharper restore ClassNeverInstantiated.Global
        #region Public Methods and Operators

        /// <summary>
        /// Creates a WorkflowApplicationTest instance using a previously created WorkflowApplication
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The WorkflowApplication is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The type of the activity associated with the WorkflowApplication does not match the type parameter T
        /// </exception>
        public static WorkflowApplicationTest<T> Attach<T>(WorkflowApplication workflowApplication) where T : Activity
        {
            VerifyWorkflowApplicationParameter<T>(workflowApplication);

            return new WorkflowApplicationTest<T>(workflowApplication);
        }

        /// <summary>
        /// Creates a WorkflowApplicationTest instance using a previously created WorkflowApplication
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <param name="defaultTimeout">
        /// The default Timeout. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The WorkflowApplication is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The type of the activity associated with the WorkflowApplication does not match the type parameter T
        /// </exception>
        public static WorkflowApplicationTest<T> Attach<T>(
            WorkflowApplication workflowApplication, TimeSpan defaultTimeout) where T : Activity
        {
            VerifyWorkflowApplicationParameter<T>(workflowApplication);

            return new WorkflowApplicationTest<T>(workflowApplication) { DefaultTimeout = defaultTimeout };
        }

        /// <summary>
        /// Provides support for factory construction with inference
        /// </summary>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <param name="target">
        /// The target activity 
        /// </param>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        public static WorkflowApplicationTest<T> Create<T>(T target) where T : Activity
        {
            return Create(target, null);
        }

        /// <summary>
        /// Provides support for factory construction with inference
        /// </summary>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <param name="target">
        /// The target activity 
        /// </param>
        /// <param name="input">
        /// Optional input arguments 
        /// </param>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        public static WorkflowApplicationTest<T> Create<T>(T target, IDictionary<string, object> input)
            where T : Activity
        {
            Contract.Ensures(null != Contract.Result<WorkflowApplicationTest<T>>());
            return new WorkflowApplicationTest<T>(target, input);
        }

        /// <summary>
        /// Provides support for factory construction with inference
        /// </summary>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <param name="target">
        /// The target activity 
        /// </param>
        /// <param name="defaultTimeout">
        /// The default Timeout. 
        /// </param>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        public static WorkflowApplicationTest<T> Create<T>(T target, TimeSpan defaultTimeout) where T : Activity
        {
            return Create(target, null, defaultTimeout);
        }

        /// <summary>
        /// Provides support for factory construction with inference
        /// </summary>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <param name="target">
        /// The target activity 
        /// </param>
        /// <param name="input">
        /// Optional input arguments 
        /// </param>
        /// <param name="defaultTimeout">
        /// The default Timeout. 
        /// </param>
        /// <returns>
        /// A new instance of the test host 
        /// </returns>
        public static WorkflowApplicationTest<T> Create<T>(
            T target, IDictionary<string, object> input, TimeSpan defaultTimeout) where T : Activity
        {
            return new WorkflowApplicationTest<T>(target, input) { DefaultTimeout = defaultTimeout };
        }

        /// <summary>
        /// Creates a new host and loads an instance
        /// </summary>
        /// <param name="instanceStore">The instance store </param>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the workflow 
        /// </typeparam>
        /// <returns>
        /// The <see cref="WorkflowApplicationTest"/> . 
        /// </returns>
        public static WorkflowApplicationTest<T> Load<T>(InstanceStore instanceStore, Guid instanceId) where T : Activity, new()
        {
            var workflowApplicationTest = new WorkflowApplicationTest<T>(new T()) { InstanceStore = instanceStore };
            workflowApplicationTest.Load(instanceId);
            return workflowApplicationTest;
        }

        /// <summary>
        /// Creates a new host and loads an instance
        /// </summary>
        /// <param name="instanceStore">
        /// The instance Store.
        /// </param>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the workflow 
        /// </typeparam>
        /// <returns>
        /// The <see cref="WorkflowApplicationTest"/> . 
        /// </returns>
        public static WorkflowApplicationTest<T> Load<T>(InstanceStore instanceStore, Guid instanceId, TimeSpan timeout) where T : Activity, new()
        {
            var workflowApplicationTest = new WorkflowApplicationTest<T>(new T()) { InstanceStore = instanceStore };
            workflowApplicationTest.Load(instanceId, timeout);
            return workflowApplicationTest;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verifies the WorkflowApplication parameter
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        /// <typeparam name="T">
        /// The type of the activity to test 
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// The WorkflowApplication is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The type of the activity associated with the WorkflowApplication does not match the type parameter T
        /// </exception>
        private static void VerifyWorkflowApplicationParameter<T>(WorkflowApplication workflowApplication)
        {
            if (workflowApplication == null)
            {
                throw new ArgumentNullException("workflowApplication");
            }

            if (workflowApplication.WorkflowDefinition.GetType()
                != typeof(T))
            {
                throw new ArgumentException(
                    string.Format("Workflow definition must be of type {0}", typeof(T).Name), "workflowApplication");
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides a wrapper around WorkflowApplication for unit testing activities
    /// </summary>
    /// <typeparam name="T">
    /// The type of activity to test 
    /// </typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification =
            "The non generic version WorkflowApplicationTest is used to provide a factory with type inference")]
    public class WorkflowApplicationTest<T> : IDisposable
        where T : Activity
    {
        #region Constants

        /// <summary>
        ///   The default limit for the number of loops to make while waiting for an idle event
        /// </summary>
        public const int DefaultIdleLoopLimit = 20;

        #endregion

        #region Fields

        /// <summary>
        ///   The aborted event.
        /// </summary>
        private readonly ManualResetEvent abortedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The completed event.
        /// </summary>
        private readonly ManualResetEvent completedEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The idle event.
        /// </summary>
        private readonly AutoResetEvent idleEvent = new AutoResetEvent(false);

        /// <summary>
        ///   The persistable idle event.
        /// </summary>
        private readonly AutoResetEvent persistableIdleEvent = new AutoResetEvent(false);

        /// <summary>
        ///   The target activity.
        /// </summary>
        private readonly T target;

        /// <summary>
        ///   The unhandled exception event.
        /// </summary>
        private readonly ManualResetEvent unhandledExceptionEvent = new ManualResetEvent(false);

        /// <summary>
        ///   The unloaded event.
        /// </summary>
        private readonly AutoResetEvent unloadedEvent = new AutoResetEvent(false);

        /// <summary>
        ///   The default timeout
        /// </summary>
        private TimeSpan defaultTimeout = Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(1);

        /// <summary>
        ///   The out argument helper.
        /// </summary>
        private AssertOutput outArgument;

        /// <summary>
        ///   The WorkflowApplication being tested
        /// </summary>
        private WorkflowApplication testWorkflowApplication;

        /// <summary>
        ///   Captures lines of text written during the test run.
        /// </summary>
        private string[] textLines;

        /// <summary>
        ///   The memory tracking participant
        /// </summary>
        private MemoryTrackingParticipant tracking;

        /// <summary>
        ///   The length or the writer.
        /// </summary>
        private int writerLength;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowApplicationTest{T}"/> class. 
        ///   Construct with the Create method
        /// </summary>
        /// <param name="target">
        /// The target activity 
        /// </param>
        /// <param name="arguments">
        /// Optional input paramters 
        /// </param>
        public WorkflowApplicationTest(T target, IDictionary<string, object> arguments = null)
            : this()
        {
            this.target = target;
            this.TestWorkflowApplication = arguments != null
                                               ? new WorkflowApplication(this.target, arguments)
                                               : new WorkflowApplication(this.target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowApplicationTest{T}"/> class.
        /// </summary>
        /// <param name="workflowApplication">
        /// The workflow application. 
        /// </param>
        public WorkflowApplicationTest(WorkflowApplication workflowApplication)
            : this()
        {
            this.TestWorkflowApplication = workflowApplication;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="WorkflowApplicationTest"/> class from being created. 
        ///   Initializes a new instance of the <see cref="WorkflowApplicationTest{T}"/> class.
        /// </summary>
        private WorkflowApplicationTest()
        {
            this.Results = new WorkflowTestResults();
            this.Writer = new StringWriter();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Aborted action.
        /// </summary>
        public Action<WorkflowApplicationAbortedEventArgs> Aborted { get; set; }

        /// <summary>
        ///   Gets the AbortedEvent.
        /// </summary>
        public ManualResetEvent AbortedEvent
        {
            get
            {
                return this.abortedEvent;
            }
        }

        /// <summary>
        ///   Gets the AssertOuput object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the output is not available yet</exception>
        public AssertOutput AssertOutArgument
        {
            get
            {
                if (this.outArgument == null)
                {
                    this.CheckResultsOutput();

                    this.outArgument = new AssertOutput(this.Results.Output);
                }

                return this.outArgument;
            }
        }

        /// <summary>
        ///   Gets the last known set of Bookmark names.
        /// </summary>
        public IEnumerable<string> Bookmarks
        {
            get
            {
                var bookmarks = this.TestWorkflowApplication.GetBookmarks();
                return bookmarks.Select(bi => bi.BookmarkName).ToList();
            }
        }

        /// <summary>
        ///   Gets or sets Completed action.
        /// </summary>
        public Action<WorkflowApplicationCompletedEventArgs> Completed { get; set; }

        /// <summary>
        ///   Gets CompletedEvent.
        /// </summary>
        public ManualResetEvent CompletedEvent
        {
            get
            {
                return this.completedEvent;
            }
        }

        /// <summary>
        ///   Gets CompletionState.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the state is not available yet</exception>
        public ActivityInstanceState CompletionState
        {
            get
            {
                if (this.Results.CompletedArgs != null)
                {
                    return this.Results.CompletedArgs.CompletionState;
                }

                throw new InvalidOperationException("CompletionState is not available yet");
            }
        }

        /// <summary>
        ///   Gets or sets DefaultTimeout.
        /// </summary>
        public TimeSpan DefaultTimeout
        {
            get
            {
                return this.defaultTimeout;
            }

            set
            {
                this.defaultTimeout = value;
            }
        }

        /// <summary>
        ///   Gets the Extensions manager
        /// </summary>
        public WorkflowInstanceExtensionManager Extensions
        {
            get
            {
                return this.TestWorkflowApplication.Extensions;
            }
        }

        /// <summary>
        ///   Gets the Workflow Instance ID
        /// </summary>
        public Guid Id
        {
            get
            {
                return this.TestWorkflowApplication.Id;
            }
        }

        /// <summary>
        ///   Gets or sets Idle action.
        /// </summary>
        public Action<WorkflowApplicationIdleEventArgs> Idle { get; set; }

        /// <summary>
        ///   Gets IdleEvent.
        /// </summary>
        public AutoResetEvent IdleEvent
        {
            get
            {
                return this.idleEvent;
            }
        }

        /// <summary>
        ///   Gets or sets InstanceStore.
        /// </summary>
        public InstanceStore InstanceStore
        {
            get
            {
                return this.TestWorkflowApplication.InstanceStore;
            }

            set
            {
                this.TestWorkflowApplication.InstanceStore = value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsCanceled.
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                return this.Results.CompletedArgs.CompletionState == ActivityInstanceState.Canceled;
            }
        }

        /// <summary>
        ///   Gets or sets UnhandledException action.
        /// </summary>
        public Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction> OnUnhandledException { get; set; }

        /// <summary>
        ///   A dynamic object for accessing the out arguments of the activity
        /// </summary>
        public dynamic OutArguments
        {
            get
            {
                this.CheckResultsOutput();
                return WorkflowArguments.FromDictionary(this.Results.Output);
            }
        }

        /// <summary>
        ///   Gets or sets PersistableIdle.
        /// </summary>
        public Func<WorkflowApplicationIdleEventArgs, PersistableIdleAction> PersistableIdle { get; set; }

        /// <summary>
        ///   Gets PersistableIdleEvent.
        /// </summary>
        public AutoResetEvent PersistableIdleEvent
        {
            get
            {
                return this.persistableIdleEvent;
            }
        }

        /// <summary>
        ///   Gets or sets Results.
        /// </summary>
        public WorkflowTestResults Results { get; protected set; }

        /// <summary>
        ///   Gets or sets TestWorkflowApplication.
        /// </summary>
        public WorkflowApplication TestWorkflowApplication
        {
            get
            {
                return this.testWorkflowApplication;
            }

            protected set
            {
                this.testWorkflowApplication = value;
                this.testWorkflowApplication.Extensions.Add(this.Writer);
                this.testWorkflowApplication.Extensions.Add(this.Tracking);
                this.InitializeDelegates();
            }
        }

        /// <summary>
        ///   Gets TextLines.
        /// </summary>
        public string[] TextLines
        {
            get
            {
                var strWriter = this.Writer.ToString();

                // If the buffer has changed, split it
                if (this.writerLength
                    != strWriter.Length)
                {
                    this.writerLength = strWriter.Length;
                    this.textLines = Regex.Split(strWriter, Environment.NewLine);
                }

                return this.textLines;
            }
        }

        /// <summary>
        ///   Gets Tracking.
        /// </summary>
        public MemoryTrackingParticipant Tracking
        {
            get
            {
                return this.tracking ?? (this.tracking = new MemoryTrackingParticipant());
            }
        }

        /// <summary>
        ///   Gets UnhandledExceptionEvent.
        /// </summary>
        public ManualResetEvent UnhandledExceptionEvent
        {
            get
            {
                return this.unhandledExceptionEvent;
            }
        }

        /// <summary>
        ///   Gets or sets Unloaded action.
        /// </summary>
        public Action<WorkflowApplicationEventArgs> Unloaded { get; set; }

        /// <summary>
        ///   Gets UnloadedEvent.
        /// </summary>
        public AutoResetEvent UnloadedEvent
        {
            get
            {
                return this.unloadedEvent;
            }
        }

        /// <summary>
        ///   Gets or sets Writer.
        /// </summary>
        public StringWriter Writer { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Cancels the workflow and waits for the workflow to complete
        /// </summary>
        public void Cancel()
        {
            Debug.Assert(this.TestWorkflowApplication != null, "workflowApplication != null");
            this.TestWorkflowApplication.Cancel();
            this.WaitForCompletedEvent();
        }

        /// <summary>
        ///   Cancels the workflow and waits for the workflow to complete
        /// </summary>
        /// <returns> A task for cancelling the workflow </returns>
        public Task CancelAsync()
        {
            return Task.Factory.StartNew(
                () =>
                    {
                        this.TestWorkflowApplication.Cancel();
                        this.WaitForCompletedEvent();
                    });
        }

        /// <summary>
        ///   Disposes of the workflow application
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        public void Load(Guid instanceId)
        {
            this.Load(instanceId, this.DefaultTimeout);
        }

        /// <summary>
        /// </summary>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        public void Load(Guid instanceId, TimeSpan timeout)
        {
            this.TestWorkflowApplication.Load(instanceId, timeout);
        }

        /// <summary>
        ///   Invokes Persist on the TestWorkflowApplication
        /// </summary>
        public void Persist()
        {
            this.TestWorkflowApplication.Persist();
        }

        /// <summary>
        /// Invokes Persist on the TestWorkflowApplication
        /// </summary>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        public void Persist(TimeSpan timeout)
        {
            this.TestWorkflowApplication.Persist(timeout);
        }

        /// <summary>
        ///   Runs an activity but does not wait for it
        /// </summary>
        /// <remarks>
        ///   This method exists for legacy support.  It does the same thing as Run()
        /// </remarks>
        public void TestActivity()
        {
            this.TestWorkflowApplication.Run();
        }

        /// <summary>
        ///   Unload the application
        /// </summary>
        public void Unload()
        {
            this.TestWorkflowApplication.Unload();
        }

        /// <summary>
        /// Unload the application
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        public void Unload(TimeSpan timeout)
        {
            this.TestWorkflowApplication.Unload(timeout);
        }

        /// <summary>
        /// Waits for the target to abort
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForAbortedEvent(TimeSpan timeout)
        {
            Debug.Assert(this.AbortedEvent != null, "manualResetEvent != null ");
            return this.AbortedEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   Waits for the target to abort
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForAbortedEvent()
        {
            return this.WaitForAbortedEvent(this.DefaultTimeout);
        }

        /// <summary>
        /// Waits for the target to complete
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForCompletedEvent(TimeSpan timeout)
        {
            Debug.Assert(this.CompletedEvent != null, "this.CompletedEvent != null");
            return this.CompletedEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   Waits for the target to complete
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForCompletedEvent()
        {
            return this.WaitForCompletedEvent(this.DefaultTimeout);
        }

        /// <summary>
        /// The wait for idle event.
        /// </summary>
        /// <param name="timeout">
        /// The timeout 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForIdleEvent(TimeSpan timeout)
        {
            Debug.Assert(this.IdleEvent != null, "this.IdleEvent != null");
            return this.IdleEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   The wait for idle event.
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForIdleEvent()
        {
            return this.WaitForIdleEvent(this.DefaultTimeout);
        }

        /// <summary>
        ///   The wait for persistable idle event.
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForPersistableIdleEvent()
        {
            return this.WaitForPersistableIdleEvent(this.DefaultTimeout);
        }

        /// <summary>
        /// The wait for persistable idle event.
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForPersistableIdleEvent(TimeSpan timeout)
        {
            Debug.Assert(this.PersistableIdleEvent != null, "this.PersistableIdleEvent != null");
            return this.PersistableIdleEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   The wait for unhandled exception event.
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForUnhandledExceptionEvent()
        {
            return this.WaitForUnhandledExceptionEvent(this.DefaultTimeout);
        }

        /// <summary>
        /// The wait for unhandled exception event.
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForUnhandledExceptionEvent(TimeSpan timeout)
        {
            Debug.Assert(this.UnhandledExceptionEvent != null, "this.UnhandledExceptionEvent != null");

            return this.UnhandledExceptionEvent.WaitOne(timeout);
        }

        /// <summary>
        ///   The wait for unloaded event.
        /// </summary>
        /// <returns> true if the event occured, false if timeout </returns>
        public bool WaitForUnloadedEvent()
        {
            return this.WaitForUnloadedEvent(this.DefaultTimeout);
        }

        /// <summary>
        /// The wait for unloaded event.
        /// </summary>
        /// <param name="timeout">
        /// The timeout. 
        /// </param>
        /// <returns>
        /// true if the event occured, false if timeout 
        /// </returns>
        public bool WaitForUnloadedEvent(TimeSpan timeout)
        {
            Debug.Assert(this.UnloadedEvent != null, "this.UnloadedEvent != null");
            return this.UnloadedEvent.WaitOne(timeout);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes of resources
        /// </summary>
        /// <param name="disposing">
        /// True if disposing. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (this.testWorkflowApplication != null)
                {
                    // Cancel the workflow if in progress.
                    // If not running this call is harmless
                    this.testWorkflowApplication.Cancel();
                }
            }

            // free native resources
        }

        /// <summary>
        ///   The check results output.
        /// </summary>
        /// <exception cref="InvalidOperationException">No output is set yet</exception>
        private void CheckResultsOutput()
        {
            // If already completed, results can be obtained
            if (this.WaitForCompletedEvent())
            {
                return;
            }

            if (this.Results == null
                || this.Results.Output == null)
            {
                throw new InvalidOperationException(
                    Resources.WorkflowApplicationTest_CheckResultsOutput_Results_Output_is_not_set_yet);
            }
        }

        /// <summary>
        ///   Initializes the delegates for the workflow application
        /// </summary>
        private void InitializeDelegates()
        {
            Debug.Assert(this.TestWorkflowApplication != null, "this.TestWorkflowApplication != null");

            this.Aborted = this.TestWorkflowApplication.Aborted;
            this.TestWorkflowApplication.Aborted = this.InternalOnAborted;

            this.Completed = this.TestWorkflowApplication.Completed;
            this.TestWorkflowApplication.Completed = this.InternalOnCompleted;

            this.Idle = this.TestWorkflowApplication.Idle;
            this.TestWorkflowApplication.Idle = this.InternalOnIdle;

            this.PersistableIdle = this.TestWorkflowApplication.PersistableIdle;
            this.TestWorkflowApplication.PersistableIdle = this.InternalOnPersistableIdle;

            this.OnUnhandledException = this.TestWorkflowApplication.OnUnhandledException;
            this.TestWorkflowApplication.OnUnhandledException = this.InternalOnUnhandledException;

            this.Unloaded = this.TestWorkflowApplication.Unloaded;
            this.TestWorkflowApplication.Unloaded = this.InternalOnUnloaded;
        }

        /// <summary>
        /// Handles the Aborted event
        /// </summary>
        /// <param name="e">
        /// The event args 
        /// </param>
        private void InternalOnAborted(WorkflowApplicationAbortedEventArgs e)
        {
            Debug.Assert(this.Results != null, "this.Results != null");

            this.Results.AbortedArgs = e;

            if (this.Aborted != null)
            {
                this.Aborted(e);
            }

            Debug.Assert(this.abortedEvent != null, "this.abortedEvent != null");

            this.abortedEvent.Set();
        }

        /// <summary>
        /// Handles the Completed event
        /// </summary>
        /// <param name="e">
        /// The event args. 
        /// </param>
        private void InternalOnCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            Debug.Assert(this.Results != null, "this.Results != null");

            this.Results.CompletedArgs = e;

            if (this.Completed != null)
            {
                this.Completed(e);
            }

            Debug.Assert(this.completedEvent != null, "this.completedEvent != null");

            this.completedEvent.Set();
        }

        /// <summary>
        /// Handles the idle event
        /// </summary>
        /// <param name="e">
        /// The event args. 
        /// </param>
        private void InternalOnIdle(WorkflowApplicationIdleEventArgs e)
        {
            Debug.Assert(this.Results != null, "this.Results != null");
            Debug.Assert(this.Results.IdleEventArgs != null, "this.Results.IdleEventArgs != null");

            this.Results.IdleEventArgs.Add(e);

            if (this.Idle != null)
            {
                this.Idle(e);
            }

            Debug.Assert(this.idleEvent != null, "this.idleEvent != null");
            this.idleEvent.Set();
        }

        /// <summary>
        /// The internal on persistable idle.
        /// </summary>
        /// <param name="e">
        /// The event args. 
        /// </param>
        /// <returns>
        /// The action to take on a persistable idle 
        /// </returns>
        /// <remarks>
        /// To provide a return value, create a PersistableIdle delegate and return the desired value
        /// </remarks>
        private PersistableIdleAction InternalOnPersistableIdle(WorkflowApplicationIdleEventArgs e)
        {
            try
            {
                Debug.Assert(this.Results != null, "this.Results != null");
                Debug.Assert(this.Results.IdleEventArgs != null, "this.Results.IdleEventArgs != null");

                this.Results.IdleEventArgs.Add(e);
                var result = default(PersistableIdleAction);

                if (this.PersistableIdle != null)
                {
                    result = this.PersistableIdle(e);
                }

                return result;
            }
            finally
            {
                Debug.Assert(this.persistableIdleEvent != null, "this.persistableIdleEvent != null");
                this.persistableIdleEvent.Set();
            }
        }

        /// <summary>
        /// The internal on unhandled exception.
        /// </summary>
        /// <param name="e">
        /// The event args. 
        /// </param>
        /// <returns>
        /// The unhandled exception action 
        /// </returns>
        private UnhandledExceptionAction InternalOnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                Debug.Assert(this.Results != null, "this.Results != null");
                this.Results.UnhandledExceptionArgs = e;
                var result = default(UnhandledExceptionAction);

                if (this.OnUnhandledException != null)
                {
                    result = this.OnUnhandledException(e);
                }

                return result;
            }
            finally
            {
                Debug.Assert(this.unhandledExceptionEvent != null, "this.unhandledExceptionEvent != null");
                this.unhandledExceptionEvent.Set();
            }
        }

        /// <summary>
        /// The internal on unloaded.
        /// </summary>
        /// <param name="e">
        /// The event args. 
        /// </param>
        private void InternalOnUnloaded(WorkflowApplicationEventArgs e)
        {
            Debug.Assert(this.Results != null, "this.Results != null");
            this.Results.UnloadedArgs = e;

            if (this.Unloaded != null)
            {
                this.Unloaded(e);
            }

            Debug.Assert(this.unloadedEvent != null, "this.unloadedEvent != null");
            this.unloadedEvent.Set();
        }

        #endregion
    }
}