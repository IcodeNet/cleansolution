// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFromDictionaryDesigner.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Design
{
    using System;
    using System.Activities.Presentation.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Windows;

    using Microsoft.Activities.Extensions.Statements;

    /// <summary>
    ///   The load activity designer
    /// </summary>
    public partial class GetFromDictionaryDesigner
    {
        #region Static Fields

        /// <summary>
        ///   The DictionaryType property
        /// </summary>
        public static readonly DependencyProperty DictionaryTypeProperty = DependencyProperty.Register(
            "DictionaryType", typeof(Type), typeof(GetFromDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        ///   The KeyType property
        /// </summary>
        public static readonly DependencyProperty KeyTypeProperty = DependencyProperty.Register(
            "KeyType", typeof(Type), typeof(GetFromDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        ///   The ValueType property
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register(
            "ValueType", typeof(Type), typeof(GetFromDictionaryDesigner), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="GetFromDictionaryDesigner" /> class.
        /// </summary>
        public GetFromDictionaryDesigner()
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
        ///   Gets or sets KeyType.
        /// </summary>
        private Type KeyType
        {
            // ReSharper disable UnusedMember.Local
            // XAML uses this
            get
            // ReSharper restore UnusedMember.Local
            {
                return (Type)this.GetValue(KeyTypeProperty);
            }

            set
            {
                this.SetValue(KeyTypeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets ValueType.
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
                typeof(GetFromDictionary<,>), 
                new DesignerAttribute(typeof(GetFromDictionaryDesigner)), 
                new ToolboxBitmapAttribute("DictGet.bmp"), 
                new DescriptionAttribute(
                    Properties.Resources.GetFromDictionaryDesigner_RegisterMetadata_Gets_a_value_from_the_Dictionary));
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
            Contract.Ensures(this.ModelItem.ItemType.GetGenericArguments().Length == 2);

            base.OnModelItemChanged(newItem);
            var modelItemType = this.ModelItem.ItemType;
            var types = modelItemType.GetGenericArguments();
            this.KeyType = types[0];
            this.ValueType = types[1];
            this.DictionaryType = typeof(IDictionary<,>).MakeGenericType(types);
        }

        #endregion
    }
}