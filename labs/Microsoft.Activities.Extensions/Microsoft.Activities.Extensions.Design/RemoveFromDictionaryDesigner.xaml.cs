// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveFromDictionaryDesigner.xaml.cs" company="Microsoft">
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
    public partial class RemoveFromDictionaryDesigner
    {
        #region Constants and Fields

        /// <summary>
        /// The DictionaryType property
        /// </summary>
        public static readonly DependencyProperty DictionaryTypeProperty = DependencyProperty.Register(
            "DictionaryType", typeof(Type), typeof(RemoveFromDictionaryDesigner), new UIPropertyMetadata(null));

        /// <summary>
        /// The KeyType property
        /// </summary>
        public static readonly DependencyProperty KeyTypeProperty = DependencyProperty.Register(
            "KeyType", typeof(Type), typeof(RemoveFromDictionaryDesigner), new UIPropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RemoveFromDictionaryDesigner" /> class.
        /// </summary>
        public RemoveFromDictionaryDesigner()
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
        /// Gets or sets KeyType.
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
                typeof(RemoveFromDictionary<,>), 
                new DesignerAttribute(typeof(RemoveFromDictionaryDesigner)), 
                new ToolboxBitmapAttribute("RemoveDict.bmp"), 
                new DescriptionAttribute(
                    Properties.Resources.RemoveFromDictionaryDesigner_RegisterMetadata_Removes_a_key_from_the_Dictionary));
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
            this.KeyType = types[0];
            this.DictionaryType = typeof(IDictionary<,>).MakeGenericType(types);
        }

        #endregion
    }
}