// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogOpen.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrackingStateMachine
{
    using System.Windows;

    /// <summary>
    ///   Interaction logic for DialogOpen.xaml
    /// </summary>
    public partial class DialogOpen : Window
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogOpen"/> class.
        /// </summary>
        public DialogOpen()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}