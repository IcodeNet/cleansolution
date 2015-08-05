// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadAndInvokeWorkflowDesigner.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System.Activities.Presentation.Metadata;
    using System.ComponentModel;
    using System.Drawing;

    using Microsoft.Activities.Extensions.Statements;

    /// <summary>
    /// The load and invoke workflow designer.
    /// </summary>
    public partial class LoadAndInvokeWorkflowDesigner
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadAndInvokeWorkflowDesigner" /> class.
        /// </summary>
        public LoadAndInvokeWorkflowDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The register metadata.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(LoadAndInvokeWorkflow), new DesignerAttribute(typeof(LoadAndInvokeWorkflowDesigner)));
            builder.AddCustomAttributes(
                typeof(LoadAndInvokeWorkflow), 
                new DescriptionAttribute(Properties.Resources.Loads_and_invokes_a_workflow));
            builder.AddCustomAttributes(typeof(LoadAndInvokeWorkflow), new ToolboxBitmapAttribute("InvokeWorkflow.bmp"));
        }

        #endregion
    }
}