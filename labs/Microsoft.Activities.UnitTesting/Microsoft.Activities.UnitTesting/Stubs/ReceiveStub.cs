// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiveStub.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Stubs
{
    using System;
    using System.Activities;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Net.Security;
    using System.ServiceModel;
    using System.ServiceModel.Activities;
    using System.ServiceModel.XamlIntegration;
    using System.Windows.Markup;
    using System.Xml.Linq;

    /// <summary>
    /// The receive stub.
    /// </summary>
    [ContentProperty("Content")]
    public sealed class ReceiveStub : ReceiveStubBase
    {
        #region Constants and Fields

        /// <summary>
        ///   The correlates on.
        /// </summary>
        private MessageQuerySet correlatesOn = new MessageQuerySet();

        /// <summary>
        ///   The correlation initializers collection.
        /// </summary>
        private Collection<CorrelationInitializer> correlationInitializers = new Collection<CorrelationInitializer>();

        /// <summary>
        ///   The known types.
        /// </summary>
        private Collection<Type> knownTypes = new Collection<Type>();

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether CanCreateInstance.
        /// </summary>
        [DefaultValue(false)]
        public bool CanCreateInstance { get; set; }

        /// <summary>
        ///   Gets or sets CorrelatesOn.
        /// </summary>
        public MessageQuerySet CorrelatesOn
        {
            get
            {
                return this.correlatesOn;
            }

            set
            {
                this.correlatesOn = value;
            }
        }

        /// <summary>
        ///   Gets or sets CorrelatesWith.
        /// </summary>
        [DefaultValue(null)]
        public InArgument<CorrelationHandle> CorrelatesWith { get; set; }

        /// <summary>
        ///   Gets or sets CorrelationInitializers.
        /// </summary>
        public Collection<CorrelationInitializer> CorrelationInitializers
        {
            get
            {
                return this.correlationInitializers;
            }

            set
            {
                this.correlationInitializers = value;
            }
        }

        /// <summary>
        ///   Gets or sets KnownTypes.
        /// </summary>
        public Collection<Type> KnownTypes
        {
            get
            {
                return this.knownTypes;
            }

            set
            {
                this.knownTypes = value;
            }
        }

        /// <summary>
        ///   Gets or sets OperationName.
        /// </summary>
        [DefaultValue(null)]
        public string OperationName { get; set; }

        /// <summary>
        ///   Gets or sets ProtectionLevel.
        /// </summary>
        [DefaultValue(null)]
        public ProtectionLevel? ProtectionLevel { get; set; }

        /// <summary>
        ///   Gets or sets SerializerOption.
        /// </summary>
        [DefaultValue(0)]
        public SerializerOption SerializerOption { get; set; }

        /// <summary>
        ///   Gets or sets ServiceContractName.
        /// </summary>
        [TypeConverter(typeof(ServiceXNameTypeConverter))]
        [DefaultValue(null)]
        public XName ServiceContractName { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The should serialize correlates on.
        /// </summary>
        /// <returns>
        /// The should serialize correlates on property.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeCorrelatesOn()
        {
            return false;
        }

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
            return string.Format("{0}|{1}", this.ServiceContractName, this.OperationName);
        }

        #endregion
    }
}