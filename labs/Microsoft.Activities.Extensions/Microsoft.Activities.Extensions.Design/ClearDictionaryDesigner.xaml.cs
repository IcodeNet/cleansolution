// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearDictionaryDesigner.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System;
    using System.Activities.Presentation.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;

    using Microsoft.Activities.Extensions.Statements;

    /// <summary>
    ///   The load activity designer
    /// </summary>
    public partial class ClearDictionaryDesigner
    {
        #region Static Fields

        /// <summary>
        ///   The DictionaryType property
        /// </summary>
        public static readonly DependencyProperty DictionaryTypeProperty = DependencyProperty.Register(
            "DictionaryType", typeof(Type), typeof(ClearDictionaryDesigner), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="ClearDictionaryDesigner" /> class.
        /// </summary>
        public ClearDictionaryDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DictionaryType.
        /// </summary>
        private Type DictionaryType
        {
// ReSharper disable UnusedMember.Local
            get
// ReSharper restore UnusedMember.Local
            {
                return (Type)this.GetValue(DictionaryTypeProperty);
            }

            set
            {
                this.SetValue(DictionaryTypeProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The register metadata.
        /// </summary>
        /// <param name="builder">
        /// The builder. 
        /// </param>
        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(ClearDictionary<,>), 
                new DesignerAttribute(typeof(ClearDictionaryDesigner)), 
                new ToolboxBitmapAttribute("ClearDict.bmp"), 
                new DescriptionAttribute(
                    Properties.Resources.ClearDictionaryDesigner_RegisterMetadata_Clears_the_Dictionary));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The model item changed handler
        /// </summary>
        /// <param name="newItem">
        /// The new item. 
        /// </param>
        protected override void OnModelItemChanged(object newItem)
        {
            base.OnModelItemChanged(newItem);
            var modelItemType = this.ModelItem.ItemType;
            var types = modelItemType.GetGenericArguments();
            this.DictionaryType = typeof(IDictionary<,>).MakeGenericType(types);
        }

        #endregion
    }
}