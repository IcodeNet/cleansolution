// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayUntilTimeDesigner.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System.Activities.Presentation.Metadata;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;

    using Microsoft.Activities.Extensions.Statements;

    /// <summary>
    /// The delay until time designer.
    /// </summary>
    public partial class DelayUntilTimeDesigner
    {
        #region Constants and Fields

        /// <summary>
        ///   The view model property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DelayUntilTimeViewModel), typeof(DelayUntilTimeDesigner));

        /// <summary>
        ///   The occurence days.
        /// </summary>
        internal const string OccurenceDays = "OccurenceDays";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelayUntilTimeDesigner" /> class.
        /// </summary>
        public DelayUntilTimeDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets ViewModel.
        /// </summary>
        public DelayUntilTimeViewModel ViewModel
        {
            get
            {
                return (DelayUntilTimeViewModel)this.GetValue(ViewModelProperty);
            }

            set
            {
                this.SetValue(ViewModelProperty, value);
            }
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
            builder.AddCustomAttributes(typeof(DelayUntilTime), new DesignerAttribute(typeof(DelayUntilTimeDesigner)));
            builder.AddCustomAttributes(typeof(DelayUntilTime), OccurenceDays, new BrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DelayUntilTime), new DescriptionAttribute(Properties.Resources.An_activity_that_will_delay_until_a_specific_time_on_certain_days));
            builder.AddCustomAttributes(typeof(DelayUntilTime), new ToolboxBitmapAttribute("DelayCheck.bmp"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on model item changed.
        /// </summary>
        /// <param name="newItem">
        /// The new item.
        /// </param>
        protected override void OnModelItemChanged(object newItem)
        {
            this.ViewModel = new DelayUntilTimeViewModel(this.ModelItem);
        }

        #endregion
    }
}