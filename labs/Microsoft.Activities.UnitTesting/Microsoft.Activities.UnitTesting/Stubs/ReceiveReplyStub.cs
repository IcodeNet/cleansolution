// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveReplyStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System.Activities;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ServiceModel.Activities;

    /// <summary>
    /// The receive reply stub.
    /// </summary>
    public class ReceiveReplyStub : ReceiveStubBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The correlation initializers.
        /// </summary>
        private readonly Collection<CorrelationInitializer> correlationInitializers = new Collection<CorrelationInitializer>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets CorrelationInitializers.
        /// </summary>
        public Collection<CorrelationInitializer> CorrelationInitializers
        {
            get
            {
                return this.correlationInitializers;
            }
        }

        /// <summary>
        ///   Gets or sets Request.
        /// </summary>
        [DefaultValue(null)]
        public Activity Request { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The get bookmark name.
        /// </summary>
        /// <returns>
        /// The bookmark name.
        /// </returns>
        protected override string GetBookmarkName()
        {
            var send = (SendStub)this.Request;

            return string.Format("{0}|{1}", send.ServiceContractName, send.OperationName);
        }

        #endregion
    }
}