// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   The _ default.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XamlAssemblyResolutionWeb
{
    using System;
    using System.Web.UI;

    using XamlAssemblyResolutionWeb.ServiceReference;

    /// <summary>
    /// The _ default.
    /// </summary>
    public partial class _Default : Page
    {
        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// The button test w s_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void buttonTestWS_Click(object sender, EventArgs e)
        {
            var proxy = new ServiceClient();
            try
            {
                var resolutionInfo = proxy.GetAssemblyNames();
                this.labelWebAssembly.Text = resolutionInfo.WebAssembly.Name;
                this.labelWebPath.Text = resolutionInfo.WebAssembly.Path;
                this.labelWebVersion.Text = resolutionInfo.WebAssembly.Version;
                this.labelActivityAssembly.Text = resolutionInfo.ActivityAssembly.Name;
                this.labelActivityPath.Text = resolutionInfo.ActivityAssembly.Path;
                this.labelActivityVersion.Text = resolutionInfo.ActivityAssembly.Version;
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }

        #endregion
    }
}