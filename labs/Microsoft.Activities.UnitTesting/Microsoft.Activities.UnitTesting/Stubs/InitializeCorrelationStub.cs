// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializeCorrelationStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System.Activities;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ServiceModel.Activities;

    /// <summary>
    /// Stub for the InitializeCorrelation activity
    /// </summary>
    public class InitializeCorrelationStub : NativeActivity
    {
        #region Constants and Fields

        /// <summary>
        /// The correlation data.
        /// </summary>
        private readonly IDictionary<string, InArgument<string>> correlationData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeCorrelationStub"/> class.
        /// </summary>
        public InitializeCorrelationStub()
        {
            this.correlationData = new Dictionary<string, InArgument<string>>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the System.ServiceModel.Activities.CorrelationHandle that references
        ///     the correlation.
        /// </summary>
        [DefaultValue("")]
        public InArgument<CorrelationHandle> Correlation { get; set; }

        /// <summary>
        ///     Gets a dictionary of correlation data that relates messages to this
        ///     workflow instance.
        /// </summary>
        public IDictionary<string, InArgument<string>> CorrelationData
        {
            get
            {
                return this.correlationData;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute method.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(NativeActivityContext context)
        {
            // Stub - do nothing
        }

        #endregion
    }
}