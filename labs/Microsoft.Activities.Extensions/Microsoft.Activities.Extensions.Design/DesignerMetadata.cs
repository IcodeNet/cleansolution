// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignerMetadata.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System.Activities.Presentation.Metadata;

    /// <summary>
    /// The designer metadata.
    /// </summary>
    public sealed class DesignerMetadata : IRegisterMetadata
    {
        #region Public Methods

        /// <summary>
        /// Registers designer metadata
        /// </summary>
        public static void RegisterAll()
        {
            var dm = new DesignerMetadata();
            dm.Register();
        }

        /// <summary>
        /// The register.
        /// </summary>
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            DelayUntilTimeDesigner.RegisterMetadata(builder);
            InvokeWorkflowDesigner.RegisterMetadata(builder);
            LoadActivityDesigner.RegisterMetadata(builder);
            LoadAndInvokeWorkflowDesigner.RegisterMetadata(builder);
            LoadAssemblyDesigner.RegisterMetadata(builder);
            AddToDictionaryDesigner.RegisterMetadata(builder);
            ClearDictionaryDesigner.RegisterMetadata(builder);
            GetFromDictionaryDesigner.RegisterMetadata(builder);
            KeyExistsInDictionaryDesigner.RegisterMetadata(builder);
            RemoveFromDictionaryDesigner.RegisterMetadata(builder);
            ValueExistsInDictionaryDesigner.RegisterMetadata(builder);
            DelayUntilDateTimeDesigner.RegisterMetadata(builder);
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        #endregion
    }
}