// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddToDictionaryDesigner.xaml.cs" company="Microsoft">
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
    using System.Runtime.InteropServices;
    using System.Windows;

    using Microsoft.Activities.Extensions.Statements;

    /// <summary>
    ///   The load activity designer
    /// </summary>
    [ComVisible(false)]
    public partial class AddToDictionaryDesigner
    {
        #region Static Fields

        /// <summary>
        ///   The DictionaryType property
        /// </summary>
        public static readonly DependencyProperty DictionaryTypeProperty = DependencyProperty.Register(
            "DictionaryType", typeof(Type), typeof(AddToDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        ///   The KeyType property
        /// </summary>
        public static readonly DependencyProperty KeyTypeProperty = DependencyProperty.Register(
            "KeyType", typeof(Type), typeof(AddToDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        ///   The ValueType property
        /// </summary>
        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register(
            "ValueType", typeof(Type), typeof(AddToDictionaryDesigner), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="AddToDictionaryDesigner" /> class.
        /// </summary>
        public AddToDictionaryDesigner()
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
                typeof(AddToDictionary<,>), 
                new DesignerAttribute(typeof(AddToDictionaryDesigner)), 
                new ToolboxBitmapAttribute("AddToDict.bmp"), 
                new DescriptionAttribute(
                    Properties.Resources.LoadActivityDesigner_RegisterMetadata_Loads_an_activity_from_XAML));
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
            Contract.Assume(types.Length == 2);
            this.KeyType = types[0];
            this.ValueType = types[1];
            this.DictionaryType = typeof(IDictionary<,>).MakeGenericType(types);
        }

        #endregion
    }
}