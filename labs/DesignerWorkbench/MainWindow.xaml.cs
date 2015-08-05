namespace DesignerWorkbench
{
    using System.Activities.Core.Presentation;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Toolbox;
    using System.Activities.Statements;
    using System.Windows;

    using Microsoft.Win32;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants and Fields

        private readonly MainViewModel mainViewModel;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();
            this.mainViewModel = new MainViewModel();
            this.DataContext = this.mainViewModel;
            this.Closing += this.mainViewModel.ViewClosing;
            this.Closed += this.mainViewModel.ViewClosed;
        }

        //private string WorkflowFilePathName
        //{
        //    get
        //    {
        //        return this.workflowFilePathName;
        //    }
        //    set
        //    {
        //        this.workflowFilePathName = value;
        //        this.Title = "Workflow Designer Workbench - " + this.WorkflowFilePathName;
        //    }
        //}

        #endregion

        #region Methods

        //private void AddDesigner()
        //{
        //    this.wd = new WorkflowDesigner();
        //    this.workflowDesignerPanel.Content = this.wd.View;
        //}

        //private void AddPropertyInspector()
        //{
        //    if (this.wd == null)
        //    {
        //        return;
        //    }
        //    this.WorkflowPropertyPanel.Content = this.wd.PropertyInspectorView;
        //}


        //private void LoadWorkflowFromFile(string fileName)
        //{
        //    this.WorkflowFilePathName = fileName;
        //    this.workflowDesignerPanel.Content = null;
        //    this.WorkflowPropertyPanel.Content = null;
        //    this.wd = new WorkflowDesigner();
        //    this.wd.Load(this.WorkflowFilePathName);
        //    this.workflowDesignerPanel.Content = this.wd.View;
        //    this.WorkflowPropertyPanel.Content = this.wd.PropertyInspectorView;
        //}

        //private void MenuItemClickLoadWorkflow(
        //    object sender,
        //    RoutedEventArgs e)
        //{
        //    var openFileDialog = new OpenFileDialog();
        //    if (openFileDialog.ShowDialog(this).Value)
        //    {
        //        this.WorkflowFilePathName = openFileDialog.FileName;
        //        this.LoadWorkflowFromFile(this.WorkflowFilePathName);
        //    }
        //}

        //private void MenuItemClickNewWorkflow(
        //    object sender,
        //    RoutedEventArgs e)
        //{
        //    this.WorkflowFilePathName = "template.xaml";
        //    this.LoadWorkflowFromFile(this.WorkflowFilePathName);
        //    this.WorkflowFilePathName = "temp.xaml";
        //}

        //private void MenuItemClickRunWorkflow(
        //    object sender,
        //    RoutedEventArgs e)
        //{
        //}

        //private void MenuItemClickSave(
        //    object sender,
        //    RoutedEventArgs e)
        //{
        //    this.Save();
        //}

        //private void MenuItemClickSaveAs(
        //    object sender,
        //    RoutedEventArgs e)
        //{
        //    this.SaveAs();
        //}

        //private void Save()
        //{
        //    if (this.WorkflowFilePathName == "temp.xaml")
        //    {
        //        this.SaveAs();
        //    }
        //    else
        //    {
        //        this.wd.Save(this.WorkflowFilePathName);
        //        statusText.Text = string.Format("Saved workflow to {0}", this.WorkflowFilePathName);
        //    }
        //    this.LoadWorkflowFromFile(this.WorkflowFilePathName);
        //}

        //private void SaveAs()
        //{
        //    var saveFileDialog =
        //        new SaveFileDialog()
        //            {
        //                AddExtension = true,
        //                DefaultExt = "*.xaml",
        //                Filter = "XAML Files (*.xaml)|*.xaml|All Files|*.*",                        
        //            };
        //    if (saveFileDialog.ShowDialog(this).Value)
        //    {
        //        this.WorkflowFilePathName = saveFileDialog.FileName;
        //        this.wd.Save(this.WorkflowFilePathName);
        //        this.statusText.Text = string.Format("Saved workflow to {0}", this.WorkflowFilePathName);
        //    }
        //}

        private void TabItemGotFocusRefreshXamlBox(object sender, RoutedEventArgs e)
        {
            mainViewModel.NotifyChanged("XAML");
        }

        #endregion
    }
}