// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Tracking.Windows
{
    using System;
    using System.Activities;
    using System.Activities.DurableInstancing;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Microsoft.Activities.Extensions.Tracking;

    using Tracking.Windows.Activities;

    /// <summary>
    ///   The WorkflowModel class invokes the workflows
    /// </summary>
    public sealed class WorkflowModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        #region Constants

        /// <summary>
        ///   The unknown state
        /// </summary>
        internal const string Unknown = "Unknown";

        /// <summary>
        ///   The connection string
        /// </summary>
        private const string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=" + SampleInstanceStore
            + ";Integrated Security=True;Asynchronous Processing=True";

        /// <summary>
        ///   The name of the database
        /// </summary>
        private const string SampleInstanceStore = "SampleInstanceStore";

        #endregion

        #region Fields

        /// <summary>
        ///   The view.
        /// </summary>
        private readonly IWorkflowView view;

        /// <summary>
        ///   The instance store
        /// </summary>
        private SqlWorkflowInstanceStore instanceStore;

        /// <summary>
        ///   Determines if the instance store has been checked
        /// </summary>
        private bool instanceStoreChecked;

        /// <summary>
        ///   The selected index.
        /// </summary>
        private int selectedIndex = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowModel"/> class.
        /// </summary>
        /// <param name="view">
        /// The view. 
        /// </param>
        internal WorkflowModel(IWorkflowView view)
        {
            this.view = view;
            this.Workflows = new ObservableCollection<WorkflowInstance>();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        ///   The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets CurrentInstance.
        /// </summary>
        public WorkflowInstance CurrentInstance
        {
            get
            {
                Debug.Assert(this.Workflows != null, "Workflows is null");
                return this.SelectedIndex != -1 ? this.Workflows[this.SelectedIndex] : null;
            }
        }

        /// <summary>
        ///   Gets CurrentState.
        /// </summary>
        public string CurrentState
        {
            get
            {
                return this.CurrentStateTracker != null ? this.CurrentStateTracker.CurrentState : null;
            }
        }

        /// <summary>
        ///   Gets CurrentStateMachineName.
        /// </summary>
        public string CurrentStateMachineName
        {
            get
            {
                return this.CurrentStateTracker != null && this.CurrentStateTracker.CurrentStateMachine != null
                           ? this.CurrentStateTracker.CurrentStateMachine.Name
                           : Unknown;
            }
        }

        /// <summary>
        ///   Gets DatabaseName.
        /// </summary>
        public string DatabaseName
        {
            get
            {
                return SampleInstanceStore;
            }
        }

        /// <summary>
        ///   Gets or sets the instance store
        /// </summary>
        public SqlWorkflowInstanceStore InstanceStore
        {
            get
            {
                return this.instanceStore ?? this.CreateInstanceStore();
            }

            set
            {
                this.instanceStore = value;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsLoaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                var workflowInstance = this.CurrentInstance;
                return workflowInstance != null && (this.SelectedIndex != -1 && workflowInstance.IsLoaded);
            }
        }

        /// <summary>
        ///   Gets or sets SelectedIndex.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///   Gets the state history
        /// </summary>
        public IList<string> StateHistory
        {
            get
            {
                return this.CurrentStateTracker != null ? this.CurrentStateTracker.StateHistory : new string[0];
            }
        }

        /// <summary>
        ///   Gets Transitions.
        /// </summary>
        public ICollection<string> Transitions
        {
            get
            {
                return this.CurrentStateTracker != null ? this.CurrentStateTracker.Transitions : null;
            }
        }

        /// <summary>
        ///   Gets Workflows.
        /// </summary>
        public ObservableCollection<WorkflowInstance> Workflows { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets CurrentStateTracker.
        /// </summary>
        private StateTracker CurrentStateTracker
        {
            get
            {
                if (this.SelectedIndex
                    == -1)
                {
                    return null;
                }

                return this.Workflows[this.SelectedIndex].StateTracker;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if a trigger can be executed
        /// </summary>
        /// <param name="trigger">
        /// The trigger 
        /// </param>
        /// <returns>
        /// true if the trigger can be executed 
        /// </returns>
        public bool CanExecute(StateTrigger trigger)
        {
            return this.Transitions != null && this.Transitions.Any(t => t == trigger.ToString());
        }

        /// <summary>
        ///   The load.
        /// </summary>
        /// <returns> True if loaded, false if not </returns>
        public bool Load()
        {
            var instance = this.CurrentInstance;
            if (instance != null)
            {
                instance.Load();
                this.NotifyChanged("Workflows");
                instance.Run();
                return true;
            }

            return false;
        }

        /// <summary>
        /// A Workflow has aborted
        /// </summary>
        /// <param name="args">
        /// The args 
        /// </param>
        public void OnAbort(WorkflowApplicationAbortedEventArgs args)
        {
            this.Refresh();
        }

        /// <summary>
        /// A Workflow has completed
        /// </summary>
        /// <param name="workflowApplicationCompletedEventArgs">
        /// The workflow Application Completed Event Args. 
        /// </param>
        public void OnComplete(WorkflowApplicationCompletedEventArgs workflowApplicationCompletedEventArgs)
        {
            var index = this.SelectedIndex;
            this.SelectedIndex = this.Workflows.Count - 2;
            this.view.RemoveWorkflow(index);
            this.Refresh();
        }

        /// <summary>
        /// A Workflow has become idle
        /// </summary>
        /// <param name="args">
        /// The args 
        /// </param>
        public void OnIdle(WorkflowApplicationIdleEventArgs args)
        {
            this.Refresh();
        }

        /// <summary>
        /// Called when a Workflow is unloaded
        /// </summary>
        /// <param name="args">
        /// The arguments 
        /// </param>
        public void OnUnload(WorkflowApplicationEventArgs args)
        {
            this.Refresh();
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="instanceId">
        /// The instance id. 
        /// </param>
        public void Open(Guid instanceId)
        {
            Debug.Assert(this.view != null, "View is null");
            var sm = StateTracker.LoadInstance(
                instanceId, WorkflowInstance.StateMachineExampleDefintion, ConnectionString);
            if (sm == null)
            {
                return;
            }

            Debug.Assert(this.Workflows != null, "Workflows is null");
            this.Workflows.Add(new WorkflowInstance(this, sm));

            this.NotifyChanged("Workflows");

            Debug.Assert(this.Workflows != null, "Workflows is null");
            this.selectedIndex = this.Workflows.Count - 1;

            Debug.Assert(this.view != null, "View is null");
            this.NotifyChanged("SelectedIndex");
            this.NotifyChanged("StateHistory");
        }

        /// <summary>
        /// Resume a bookmark
        /// </summary>
        /// <param name="bookmark">
        /// The bookmark. 
        /// </param>
        public void ResumeBookmark(string bookmark)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(bookmark));
            if (string.IsNullOrWhiteSpace(bookmark))
            {
                throw new ArgumentNullException("bookmark");
            }

            if (!this.IsLoaded)
            {
                if (!this.Load())
                {
                    return;
                }
            }

            this.CurrentInstance.Resume(bookmark);
            this.NotifyChanged("StateHistory");
        }

        /// <summary>
        /// Resume a bookmark
        /// </summary>
        /// <param name="trigger">
        /// The trigger to resume 
        /// </param>
        public void ResumeBookmark(StateTrigger trigger)
        {
            this.ResumeBookmark(trigger.ToString());
        }

        /// <summary>
        ///   The unload.
        /// </summary>
        public void Unload()
        {
            var instance = this.CurrentInstance;
            if (instance != null)
            {
                instance.Unload();
                this.NotifyChanged("Workflows");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the instances
        /// </summary>
        /// <param name="workflowView">
        /// The view. 
        /// </param>
        internal void LoadInstances(IWorkflowView workflowView)
        {
            Contract.Requires(workflowView != null);
            if (workflowView == null)
            {
                throw new ArgumentNullException("workflowView");
            }

            this.EnsureInstanceStoreExists();
            var instances = StateTracker.LoadInstances(WorkflowInstance.StateMachineExampleDefintion, ConnectionString);

            if (instances != null)
            {
                foreach (var stateMachineStateTracker in instances)
                {
                    this.Workflows.Add(new WorkflowInstance(this, stateMachineStateTracker));
                }

                WorkflowTrace.Information(
                    string.Format(
                        "Loaded {0} instance{1} from instance store", 
                        this.Workflows.Count, 
                        this.Workflows.Count != 1 ? "s" : string.Empty));
            }
            else
            {
                WorkflowTrace.Information("Error loading instances");
            }
        }

        /// <summary>
        ///   Creates a new workflow and runs until it becomes idle
        /// </summary>
        /// <returns> The instance ID </returns>
        internal Guid New()
        {
            var instance = new WorkflowInstance(this);
            this.Workflows.Add(instance);
            this.SelectedIndex = this.Workflows.Count - 1;
            instance.New();

            instance.Run();

            return instance.Id;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name. 
        /// </param>
        internal void NotifyChanged(string propertyName)
        {
            var onPropertyChanged = this.PropertyChanged;
            if (onPropertyChanged != null)
            {
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name. 
        /// </param>
        internal void NotifyCollectionChanged(string propertyName)
        {
            var onPropertyChanged = this.CollectionChanged;
            if (onPropertyChanged != null)
            {
                onPropertyChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            this.NotifyChanged(propertyName);
        }

        /// <summary>
        ///   Creates an instance store
        /// </summary>
        /// <returns> The instance store </returns>
        private SqlWorkflowInstanceStore CreateInstanceStore()
        {
            this.instanceStore = new SqlWorkflowInstanceStore(ConnectionString);
            this.EnsureInstanceStoreExists();

            this.InstanceStore = new SqlWorkflowInstanceStore(ConnectionString);

            StateTracker.Promote(this.InstanceStore);

            using (var handle = new DisposableInstanceHandle(this.instanceStore))
            {
                var ownerView = this.InstanceStore.Execute(handle, new CreateWorkflowOwnerCommand(), Globals.Timeout);
                this.InstanceStore.DefaultInstanceOwner = ownerView.InstanceOwner;
            }

            return this.instanceStore;
        }

        /// <summary>
        /// The ensure instance store exists.
        /// </summary>
        private void EnsureInstanceStoreExists()
        {
            if (!this.instanceStoreChecked)
            {
                WorkflowTrace.Information("Verifying instance store " + this.DatabaseName);
                if (!SqlWorkflowInstanceStoreManager.InstanceStoreExists(this.DatabaseName, ConnectionString))
                {
                    WorkflowTrace.Information("Creating instance store " + this.DatabaseName);
                    SqlWorkflowInstanceStoreManager.CreateInstanceStore(this.DatabaseName, ConnectionString);
                }

                this.instanceStoreChecked = true;
            }
        }

        /// <summary>
        ///   Refresh the view
        /// </summary>
        private void Refresh()
        {
            this.NotifyChanged("CurrentState");
            this.NotifyChanged("CurrentStateMachineName");
            this.NotifyCollectionChanged("Model.StateHistory");
            this.NotifyChanged("SelectedIndex");
            this.view.Refresh();
        }

        #endregion
    }
}