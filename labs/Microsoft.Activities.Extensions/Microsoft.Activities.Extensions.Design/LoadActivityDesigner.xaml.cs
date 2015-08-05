// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadActivityDesigner.xaml.cs" company="Microsoft">
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
    /// The load activity designer
    /// </summary>
    public partial class LoadActivityDesigner
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadActivityDesigner" /> class.
        /// </summary>
        public LoadActivityDesigner()
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
                typeof(LoadActivity), 
                new DesignerAttribute(typeof(LoadActivityDesigner)), 
                new ToolboxBitmapAttribute("dbGreenCheck16.png"), 
                new DescriptionAttribute(
                    Properties.Resources.LoadActivityDesigner_RegisterMetadata_Loads_an_activity_from_XAML));
        }

        #endregion
    }
}