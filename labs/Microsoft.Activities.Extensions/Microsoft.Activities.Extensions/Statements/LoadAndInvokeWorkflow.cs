// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadAndInvokeWorkflow.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Statements
{
    using System;
    using System.Activities;
    using System.Activities.Expressions;
    using System.Activities.Statements;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The load and invoke workflow.
    /// </summary>
    public sealed class LoadAndInvokeWorkflow : Activity<IDictionary<string, object>>
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadAndInvokeWorkflow" /> class.
        /// </summary>
        public LoadAndInvokeWorkflow()
        {
            this.Implementation = this.CreateImplementation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Extensions
        /// </summary>
        /// <remarks>
        /// Use this property to pass extensions to the child workflow
        /// </remarks>
        public InArgument<IEnumerable<object>> Extensions { get; set; }

        /// <summary>
        ///   Gets or sets Inputs.
        /// </summary>
        /// <remarks>
        ///   The input arguments you want to pass to the other workflow
        /// </remarks>
        public InArgument<IDictionary<string, object>> Input { get; set; }

        /// <summary>
        /// Gets or sets LocalAssembly.
        /// </summary>
        public InArgument<Assembly> LocalAssembly { get; set; }

        /// <summary>
        ///   Gets or sets Path.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> Path { get; set; }

        /// <summary>
        ///   Gets or sets Timeout.
        /// </summary>
        public InArgument<TimeSpan> Timeout { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the workflow implementation
        /// </summary>
        /// <returns>
        /// An activity
        /// </returns>
        private Activity CreateImplementation()
        {
            var activity = new Variable<Activity>();
            return new Sequence
                {
                    Variables = { activity },
                    Activities =
                        {
                            new LoadActivity
                                {
                                    Path = new InArgument<string>(ctx => this.Path.Get(ctx)), 
                                    Result = new VariableReference<Activity>(activity), 
                                    LocalAssembly = new InArgument<Assembly>(ctx => this.LocalAssembly.Get(ctx))
                                }, 
                            new InvokeWorkflow
                                {
                                    Activity = new InArgument<Activity>(activity), 
                                    Timeout =
                                        this.Timeout != null && this.Timeout.Expression != null
                                            ? new InArgument<TimeSpan>(ctx => this.Timeout.Get(ctx))
                                            : null, 
                                    Input = new InArgument<IDictionary<string, object>>(ctx => this.Input.Get(ctx)), 
                                    Extensions = new InArgument<IEnumerable<object>>(ctx => this.Extensions.Get(ctx)), 
                                    Result = new ArgumentReference<IDictionary<string, object>>("Result"), 
                                }
                        }
                };
        }

        #endregion
    }
}