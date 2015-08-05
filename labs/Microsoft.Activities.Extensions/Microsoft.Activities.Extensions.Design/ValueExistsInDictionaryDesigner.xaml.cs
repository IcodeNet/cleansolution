// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueExistsInDictionaryDesigner.xaml.cs" company="Microsoft">
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
    /// The load activity designer
    /// </summary>
    public partial class ValueExistsInDictionaryDesigner
    {
        #region Constants and Fields

        /// <summary>
        /// The DictionaryType property
        /// </summary>
        public static readonly DependencyProperty DictionaryTypeProperty = DependencyProperty.Register(
            "DictionaryType", typeof(Type), typeof(ValueExistsInDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        /// The ValueType property
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register(
            "ValueType", typeof(Type), typeof(ValueExistsInDictionaryDesigner), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ValueExistsInDictionaryDesigner" /> class.
        /// </summary>
        public ValueExistsInDictionaryDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets DictionaryType.
        /// </summary>
        private Type DictionaryType
        {
            // ReSharper disable UnusedMember.Local
            // XAML uses this
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

        /// <summary>
        /// Gets or sets ValueType.
        /// </summary>
        private Type ValueType
        {
            // ReSharper disable UnusedMember.Local
            // XAML uses this
            get
            // ReSharper restore UnusedMember.Local
            {
                return (Type)this.GetValue(ValueTypeProperty);
            }

            set
            {
                this.SetValue(ValueTypeProperty, value);
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
            builder.AddCustomAttributes(
                typeof(ValueExistsInDictionary<,>), 
                new DesignerAttribute(typeof(ValueExistsInDictionaryDesigner)), 
                new ToolboxBitmapAttribute("ValExistsDict.bmp"), 
                new DescriptionAttribute(
                    Properties.Resources.ValueExistsInDictionaryDesigner_RegisterMetadata_Determines_if_a_value_exists_in_the_dictionary));
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
            Type modelItemType = this.ModelItem.ItemType;
            Type[] types = modelItemType.GetGenericArguments();
            this.ValueType = types[1];
            this.DictionaryType = typeof(IDictionary<,>).MakeGenericType(types);
        }

        #endregion
    }
}