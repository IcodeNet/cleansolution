// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CreateGuid
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows;

    using CreateGuid.Properties;

    using Microsoft.Win32;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        ///   The title
        /// </summary>
        private const string CreateGuidNet = "Create Guid.NET";

        /// <summary>
        ///   The external tools for DEV10
        /// </summary>
        private const string VisualStudio10ExternalTools = @"Software\Microsoft\VisualStudio\10.0\External Tools";

        /// <summary>
        ///   The external tools for DEV11
        /// </summary>
        private const string VisualStudio11ExternalTools = @"Software\Microsoft\VisualStudio\11.0\External Tools";

        #endregion

        #region Fields

        /// <summary>
        ///   The guid format
        /// </summary>
        private GuidFormat format;

        /// <summary>
        ///   The status bar text
        /// </summary>
        private string status;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            Enum.TryParse(Settings.Default.Format, true, out this.format);
            this.Guid = Guid.NewGuid();
            this.InitializeComponent();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///   A property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        ///// <summary>
        ///// Gets a value indicating whether can install key.
        ///// </summary>
        // public Visibility CanInstallKey
        // {
        // get
        // {
        // return CanWriteKey(VisualStudio11ExternalTools) ? Visibility.Visible : Visibility.Hidden;
        // }
        // }
        #region Public Properties

        /// <summary>
        ///   Gets or sets the guid format.
        /// </summary>
        public GuidFormat Format
        {
            get
            {
                return this.format;
            }

            set
            {
                this.format = value;
                Settings.Default.Format = this.format.ToString();
                Settings.Default.Save();
            }
        }

        /// <summary>
        ///   Gets or sets the guid.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///   Gets the guid b.
        /// </summary>
        public string GuidB
        {
            get
            {
                return this.Guid.ToString("B").ToUpperInvariant();
            }
        }

        /// <summary>
        ///   Gets the guid d.
        /// </summary>
        public string GuidD
        {
            get
            {
                return this.Guid.ToString("D").ToUpperInvariant();
            }
        }

        /// <summary>
        ///   Gets the guid n.
        /// </summary>
        public string GuidN
        {
            get
            {
                return this.Guid.ToString("N").ToUpperInvariant();
            }
        }

        /// <summary>
        ///   Gets the guid p.
        /// </summary>
        public string GuidP
        {
            get
            {
                return this.Guid.ToString("P").ToUpperInvariant();
            }
        }

        /// <summary>
        ///   Gets the guid x.
        /// </summary>
        public string GuidX
        {
            get
            {
                return this.Guid.ToString("X").ToUpperInvariant();
            }
        }

        /// <summary>
        ///   Gets or sets the status.
        /// </summary>
        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                this.NotifyChanged("Status");
            }
        }

        #endregion

        ///// <summary>
        ///// </summary>
        ///// <param name="key">
        ///// The key.
        ///// </param>
        ///// <returns>
        ///// The System.Boolean.
        ///// </returns>
        // public static bool CanWriteKey(string key)
        // {
        // try
        // {
        // var r = new RegistryPermission(RegistryPermissionAccess.Write, key);
        // r.Demand();
        // return true;
        // }
        // catch (SecurityException)
        // {
        // return false;
        // }
        // }
        #region Methods

        /// <summary>
        /// Adds the tool to Visual Studio
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void ButtonInstallClick(object sender, RoutedEventArgs e)
        {
            var dev10Added = false;
            var dev11Added = false;
            var path = new UriBuilder(Assembly.GetExecutingAssembly().EscapedCodeBase).Path.Replace('/', '\\');
            using (var visualStudio11Key = Registry.CurrentUser.OpenSubKey(VisualStudio11ExternalTools, true))
            {
                dev11Added = this.InstallToolVs(visualStudio11Key, path);
            }

            using (var visualStudio10Key = Registry.CurrentUser.OpenSubKey(VisualStudio10ExternalTools, true))
            {
                dev10Added = this.InstallToolVs(visualStudio10Key, path);
            }

            if (!dev10Added && !dev11Added)
            {
                MessageBox.Show("Unable to add to Visual Studio");
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0}\r\nAdded to the following products - Restart Visual Studio to see it.\r\n", path);
                if (dev10Added)
                {
                    sb.AppendLine("Visual Studio 2010");
                }

                if (dev11Added)
                {
                    sb.AppendLine("Visual Studio 2012");
                }

                MessageBox.Show(this, sb.ToString());
            }
        }

        /// <summary>
        /// The new button click handler
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void ButtonNewGuidClick(object sender, RoutedEventArgs e)
        {
            this.GenerateGuid();
        }

        /// <summary>
        ///   copy the selected Guid to the clipboard
        /// </summary>
        private void CopyGuid()
        {
            string text;
            switch (this.Format)
            {
                case GuidFormat.B:
                    text = this.GuidB;
                    break;
                case GuidFormat.D:
                    text = this.GuidD;
                    break;
                case GuidFormat.N:
                    text = this.GuidN;
                    break;
                case GuidFormat.P:
                    text = this.GuidP;
                    break;
                case GuidFormat.X:
                    text = this.GuidX;
                    break;
                default:
                    text = this.GuidB;
                    break;
            }

            Debug.Assert(text != null, "text != null");
            Clipboard.SetText(text);

            this.Status = string.Format("Copied {0} to clipboard", text);
        }

        /// <summary>
        ///   Generates a new Guid
        /// </summary>
        private void GenerateGuid()
        {
            this.Guid = Guid.NewGuid();
            this.NotifyChanged("GuidD");
            this.NotifyChanged("GuidB");
            this.NotifyChanged("GuidN");
            this.NotifyChanged("GuidP");
            this.NotifyChanged("GuidX");
        }

        /// <summary>
        /// Install into DEV10 external tools
        /// </summary>
        /// <param name="visualStudioKey">
        /// The visual studio key. 
        /// </param>
        /// <param name="path">
        /// The path. 
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool InstallToolVs(RegistryKey visualStudioKey, string path)
        {
            if (visualStudioKey == null)
            {
                return false;
            }

            // Get the next avilable tool slot
            try
            {
                var nextKey = int.Parse(visualStudioKey.GetValue("ToolNumKeys", 0).ToString());
                var nextKeyStr = nextKey.ToString(CultureInfo.InvariantCulture);
                var installKey = nextKey;
                for (int i = 0; i < nextKey; i++)
                {
                    if (visualStudioKey.GetValue("ToolTitle" + i).ToString() == Title)
                    {
                        installKey = i;
                        break;
                    }
                }

                visualStudioKey.SetValue("ToolCmd" + installKey, path);
                visualStudioKey.SetValue("ToolTitle" + installKey, this.Title);
                visualStudioKey.SetValue("ToolOpt" + installKey, 17);

                if (installKey == nextKey)
                {
                    // Added new tool
                    visualStudioKey.SetValue("ToolNumKeys", nextKey + 1);
                }
                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }

        /// <summary>
        /// Install into DEV11 external tools
        /// </summary>
        /// <param name="visualStudioKey">
        /// The visual studio key. 
        /// </param>
        /// <param name="path">
        /// The path. 
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool InstallToolVs2012(RegistryKey visualStudioKey, string path)
        {
            if (visualStudioKey == null)
            {
                return false;
            }

            try
            {
                // Remove the previous key if exists
                if (visualStudioKey.OpenSubKey(CreateGuidNet) != null)
                {
                    visualStudioKey.DeleteSubKeyTree(CreateGuidNet);
                }

                using (var key = visualStudioKey.CreateSubKey(CreateGuidNet))
                {
                    Debug.Assert(key != null, "key != null");
                    key.SetValue("ToolArg", string.Empty);
                    key.SetValue("ToolCmd", path);
                    key.SetValue("ToolDir", string.Empty);
                    key.SetValue("ToolOpt", 1, RegistryValueKind.DWord);
                    key.SetValue("ToolTitle", CreateGuidNet);
                }

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }
        }

        /// <summary>
        /// Notify that a property has changed
        /// </summary>
        /// <param name="prop">
        /// The name of the property 
        /// </param>
        private void NotifyChanged(string prop)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                propChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        /// <summary>
        /// The radio button is checked
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void RadioGuidBChecked(object sender, RoutedEventArgs e)
        {
            this.CopyGuid();
        }

        /// <summary>
        /// The radio button is checked
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void RadioGuidDChecked(object sender, RoutedEventArgs e)
        {
            this.CopyGuid();
        }

        /// <summary>
        /// The radio button is checked
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void RadioGuidNChecked(object sender, RoutedEventArgs e)
        {
            this.CopyGuid();
        }

        /// <summary>
        /// The radio button is checked
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void RadioGuidPChecked(object sender, RoutedEventArgs e)
        {
            this.CopyGuid();
        }

        /// <summary>
        /// The radio button is checked
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e. 
        /// </param>
        private void RadioGuidXChecked(object sender, RoutedEventArgs e)
        {
            this.CopyGuid();
        }

        #endregion
    }
}