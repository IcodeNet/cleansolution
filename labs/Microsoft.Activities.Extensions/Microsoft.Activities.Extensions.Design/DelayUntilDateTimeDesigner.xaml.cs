// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilDateTimeDesigner.xaml.cs" company="Microsoft">
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
    /// The delay until date time designer.
    /// </summary>
    public partial class DelayUntilDateTimeDesigner
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayUntilDateTimeDesigner"/> class.
        /// </summary>
        public DelayUntilDateTimeDesigner()
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
                typeof(DelayUntilDateTime), 
                new DesignerAttribute(typeof(DelayUntilDateTimeDesigner)), 
                new DescriptionAttribute(Properties.Resources.DelayUntilDateTimeDesigner_RegisterMetadata_Delay_until_a_specific_date_time), 
                new ToolboxBitmapAttribute("DelayCheck.bmp"));
        }

        #endregion
    }
}