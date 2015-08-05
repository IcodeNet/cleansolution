// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowApplicationObserver.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions
{
    using System;
    using System.Activities;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///   Observes a WorkflowApplication
    /// </summary>
    public class WorkflowApplicationObserver
    {
        #region Fields

        /// <summary>
        ///   The innerious Aborted handler
        /// </summary>
        private readonly Action<WorkflowApplicationAbortedEventArgs> innerAborted;

        /// <summary>
        ///   The innerious Completed handler
        /// </summary>
        private readonly Action<WorkflowApplicationCompletedEventArgs> innerCompleted;

        /// <summary>
        ///   The innerious Idle handler
        /// </summary>
        private readonly Action<WorkflowApplicationIdleEventArgs> innerIdle;

        /// <summary>
        ///   The inner OnUnhandledException handler
        /// </summary>
        private readonly Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction> innerOnUnhandledException;

        /// <summary>
        ///   The workflow application
        /// </summary>
        private readonly WorkflowApplication workflowApplication;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowApplicationObserver"/> class.
        /// </summary>
        /// <param name="application">
        /// The application. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The application is null
        /// </exception>
        public WorkflowApplicationObserver(WorkflowApplication application)
        {
            Contract.Requires(application != null);
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.workflowApplication = application;

            if (this.workflowApplication.Aborted != null)
            {
                this.innerAborted = this.workflowApplication.Aborted;
            }

            this.workflowApplication.Aborted = this.InternalAborted;

            if (this.workflowApplication.Completed != null)
            {
                this.innerCompleted = this.workflowApplication.Completed;
            }

            this.workflowApplication.Completed = this.InternalCompleted;

            if (this.workflowApplication.Idle != null)
            {
                this.innerIdle = this.workflowApplication.Idle;
            }

            this.workflowApplication.Idle = this.InternalIdle;

            if (this.workflowApplication.OnUnhandledException != null)
            {
                this.innerOnUnhandledException = this.workflowApplication.OnUnhandledException;
            }

            this.workflowApplication.OnUnhandledException = this.InternalOnUnhandledException;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the aborted handler.
        /// </summary>
        public Action<WorkflowApplicationAbortedEventArgs> Aborted { get; set; }

        /// <summary>
        ///   Gets or sets the completed handler.
        /// </summary>
        public Action<WorkflowApplicationCompletedEventArgs> Completed { get; set; }

        /// <summary>
        ///   Gets or sets the idle handler.
        /// </summary>
        public Action<WorkflowApplicationIdleEventArgs> Idle { get; set; }

        /// <summary>
        ///   Gets or sets the OnUnhandledException handler
        /// </summary>
        public Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction, UnhandledExceptionAction> OnUnhandledException { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Attach a WorkflowObserver to a WorkflowApplication
        /// </summary>
        /// <param name="host">
        /// The WorkflowApplication 
        /// </param>
        /// <returns>
        /// The Observer 
        /// </returns>
        public static WorkflowApplicationObserver Attach(WorkflowApplication host)
        {
            return new WorkflowApplicationObserver(host);
        }

        /// <summary>
        /// Attaches an action to the OnUnhandledException handler
        /// </summary>
        /// <param name="func">
        /// The handler. 
        /// </param>
        /// <returns>
        /// The WorkflowApplicationObserver. 
        /// </returns>
        /// <remarks>
        /// The hook function will receive as a parameter the value returned from the previous hook
        /// </remarks>
        public WorkflowApplicationObserver HookOnUnhandledException(
            Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction, UnhandledExceptionAction> func)
        {
            this.OnUnhandledException = func;
            return this;
        }

        /// <summary>
        /// Attaches an action to the complete handler
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        /// <returns>
        /// The Microsoft.Activities.Extensions.WorkflowApplicationObserver. 
        /// </returns>
        public WorkflowApplicationObserver ObserveComplete(Action<WorkflowApplicationCompletedEventArgs> args)
        {
            this.Completed = args;
            return this;
        }

        /// <summary>
        /// Attaches an action to the idle handler
        /// </summary>
        /// <param name="action">
        /// The action. 
        /// </param>
        /// <returns>
        /// The WorkflowApplicationObserver. 
        /// </returns>
        public WorkflowApplicationObserver ObserveIdle(Action<WorkflowApplicationIdleEventArgs> action)
        {
            this.Idle = action;
            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Internal Aborted Handler
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private void InternalAborted(WorkflowApplicationAbortedEventArgs args)
        {
            if (this.Aborted != null)
            {
                var action = this.innerAborted;
                if (action != null)
                {
                    action(args);
                }

                this.Aborted(args);
            }
        }

        /// <summary>
        /// Internal Completed Handler
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private void InternalCompleted(WorkflowApplicationCompletedEventArgs args)
        {
            if (this.Completed != null)
            {
                var action = this.innerCompleted;
                if (action != null)
                {
                    action(args);
                }

                this.Completed(args);
            }
        }

        /// <summary>
        /// Internal Idle Handler
        /// </summary>
        /// <param name="args">
        /// The args. 
        /// </param>
        private void InternalIdle(WorkflowApplicationIdleEventArgs args)
        {
            if (this.Idle != null)
            {
                var action = this.innerIdle;
                if (action != null)
                {
                    action(args);
                }

                this.Idle(args);
            }
        }

        /// <summary>
        /// The Internal Unhandled Exception Handler
        /// </summary>
        /// <param name="args">
        /// The arg. 
        /// </param>
        /// <returns>
        /// The UnhandledExceptionAction. 
        /// </returns>
        private UnhandledExceptionAction InternalOnUnhandledException(
            WorkflowApplicationUnhandledExceptionEventArgs args)
        {
            var result = UnhandledExceptionAction.Abort;
            if (this.OnUnhandledException != null)
            {
                var func = this.innerOnUnhandledException;
                if (func != null)
                {
                    result = func(args);
                }

                result = this.OnUnhandledException(args, result);
            }

            return result;
        }

        #endregion
    }
}