// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowInstance.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrackingStateMachine
{
    using System;
    using System.Activities;
    using System.ComponentModel;
    using System.Diagnostics;

    using Microsoft.Activities.Extensions;
    using Microsoft.Activities.Extensions.Diagnostics;
    using Microsoft.Activities.Extensions.Tracking;

    using TrackingStateMachine.Activities;

    /// <summary>
    ///   The WorkflowInstance
    /// </summary>
    public class WorkflowInstance : INotifyPropertyChanged
    {
        #region Static Fields

        /// <summary>
        ///   The workflow 1 definition.
        /// </summary>
        internal static readonly StateMachineExample StateMachineExampleDefintion = new StateMachineExample();

        #endregion

        #region Fields

        /// <summary>
        ///   The view.
        /// </summary>
        private readonly WorkflowModel model;

        /// <summary>
        ///   The is loaded.
        /// </summary>
        private bool isLoaded;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="WorkflowInstance" /> class.
        /// </summary>
        /// <param name="model"> The view. </param>
        /// <param name="stateTracker"> The StateTracker (optional) </param>
        public WorkflowInstance(WorkflowModel model, StateTracker stateTracker = null)
        {
            this.model = model;
            this.StateTracker = stateTracker ?? new StateTracker();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets Host.
        /// </summary>
        public WorkflowApplication Host { get; private set; }

        /// <summary>
        ///   Gets Id.
        /// </summary>
        public Guid Id
        {
            get
            {
                Debug.Assert(this.StateTracker != null, "this.StateTracker != null");
                return this.Host == null ? this.StateTracker.InstanceId : this.Host.Id;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether a workflow is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }

            private set
            {
                this.isLoaded = value;
                this.NotifyChanged("IsLoaded");
            }
        }

        /// <summary>
        ///   Gets the StateTracker.
        /// </summary>
        public StateTracker StateTracker { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   The load.
        /// </summary>
        public void Load()
        {
            this.CreateWorkflowApplication();
            this.Host.Load(this.StateTracker.InstanceId);
            this.IsLoaded = true;
        }

        /// <summary>
        ///   The new.
        /// </summary>
        public void New()
        {
            this.CreateWorkflowApplication();
            this.IsLoaded = true;
        }

        /// <summary>
        ///   Resume the workflow until it becomes idle with any bookmark
        /// </summary>
        /// <param name="trigger"> The name of the bookmark </param>
        public void Resume(StateTrigger trigger)
        {
            this.Resume(trigger.ToString());
        }

        /// <summary>
        ///   Resume the bookmark
        /// </summary>
        /// <param name="bookmark"> The bookmark name </param>
        public void Resume(string bookmark)
        {
            this.Host.ResumeUntilBookmark(bookmark, null);
            this.model.NotifyChanged("StateHistory");
        }

        /// <summary>
        ///   The run.
        /// </summary>
        public void Run()
        {
            // Run until idle with a bookmark
            this.Host.RunUntilBookmark();
            this.model.NotifyChanged("StateHistory");
        }

        /// <summary>
        ///   The unload.
        /// </summary>
        public void Unload()
        {
            if (this.IsLoaded)
            {
                this.Host.Unload();
                this.IsLoaded = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The create workflow application.
        /// </summary>
        private void CreateWorkflowApplication()
        {
            Debug.Assert(this.model != null, "this.view != null");
            Debug.Assert(StateMachineExampleDefintion != null, "StateMachineExampleDefintion != null");
            this.Host = new WorkflowApplication(StateMachineExampleDefintion)
                {
                    Idle = this.model.OnIdle,
                    Aborted = this.model.OnAbort,
                    Completed = this.model.OnComplete,
                    OnUnhandledException = args => UnhandledExceptionAction.Abort,
                    PersistableIdle = args => PersistableIdleAction.Persist,
                    Unloaded = this.OnUnload
                };

            StateTracker.Attach(this.Host, tracker: this.StateTracker, instanceStore: this.model.InstanceStore);

            // Setup tracking in UI
            this.Host.Extensions.Add(new TraceTrackingParticipant());
        }

        /// <summary>
        ///   The on property changed.
        /// </summary>
        /// <param name="propertyName"> The property name. </param>
        private void NotifyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///   The on unload.
        /// </summary>
        /// <param name="args"> The args. </param>
        private void OnUnload(WorkflowApplicationEventArgs args)
        {
            this.IsLoaded = false;
            Debug.Assert(this.model != null, "this.view != null");
            this.model.OnUnload(args);
        }

        #endregion
    }
}