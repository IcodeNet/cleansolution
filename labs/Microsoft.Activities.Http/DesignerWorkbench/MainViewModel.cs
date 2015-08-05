namespace DesignerWorkbench
{
    using System;
    using System.Activities;
    using System.Activities.Core.Presentation;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Metadata;
    using System.Activities.Presentation.Toolbox;
    using System.Activities.Statements;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    using DesignerWorkbench.Properties;

    using Microsoft.Activities.Http.Design;
    using Microsoft.Win32;

    public class MainViewModel : INotifyPropertyChanged
    {
        #region Constants and Fields

        private const string TemplateXaml = "template.xaml";

        private const string UntitledXAML = "Untitled.xaml";

        private string fileName;

        private string status;

        private string title = Resources.Workflow_Designer_Workbench;

        private WorkflowDesigner workflowDesigner;

        #endregion

        #region Constructors and Destructors

        internal MainViewModel()
        {
            (new DesignerMetadata()).Register();
            HttpWorkflowServicesMetadata.RegisterAll();
            LoadToolboxIconsForBuiltInActivities();
            this.ToolboxPanel = this.CreateToolbox();
            this.ExitCommand = new RelayCommand(this.ExecuteExit, this.CanExecuteExit);
            this.OpenCommand = new RelayCommand(this.ExecuteOpen, this.CanExecuteOpen);
            this.NewCommand = new RelayCommand(this.ExecuteNew, this.CanExecuteNew);
            this.SaveCommand = new RelayCommand(this.ExecuteSave, this.CanExecuteSave);
            this.SaveAsCommand = new RelayCommand(this.ExecuteSaveAs, this.CanExecuteSaveAs);
            this.ExecuteNew(null);
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public ICommand ExitCommand { get; set; }

        public string FileName
        {
            get
            {
                return this.fileName;
            }
            private set
            {
                this.fileName = value;
                this.Title = string.Format("{0} - {1}", Resources.Workflow_Designer_Workbench, this.FileName);
            }
        }

        public ICommand NewCommand { get; set; }

        public ICommand OpenCommand { get; set; }

        public ICommand SaveAsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

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

        public string Title
        {
            get
            {
                return this.title;
            }
            private set
            {
                this.title = value;
                this.NotifyChanged("Title");
            }
        }

        public object ToolboxPanel { get; private set; }

        public WorkflowDesigner WorkflowDesigner
        {
            get
            {
                return this.workflowDesigner;
            }
            private set
            {
                this.workflowDesigner = value;
                this.NotifyChanged("WorkflowDesignerPanel");
                this.NotifyChanged("WorkflowPropertyPanel");
            }
        }

        public object WorkflowDesignerPanel
        {
            get
            {
                return this.WorkflowDesigner.View;
            }
        }

        public object WorkflowPropertyPanel
        {
            get
            {
                return this.WorkflowDesigner.PropertyInspectorView;
            }
        }

        public string XAML
        {
            get
            {
                if (this.WorkflowDesigner.Text != null)
                {
                    this.WorkflowDesigner.Flush();
                    return this.WorkflowDesigner.Text;
                }
                return null;
            }
        }

        #endregion

        #region Public Methods

        public void ViewClosed(object sender, EventArgs e)
        {
        }

        public void ViewClosing(object sender, CancelEventArgs e)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Notify that a property has changed
        /// </summary>
        /// <param name = "property">
        ///   The property that changed
        /// </param>
        internal void NotifyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private static void CreateToolboxBitmapAttributeForActivity(AttributeTableBuilder builder, ResourceReader resourceReader, Type builtInActivityType)
        {
            var bitmap = ExtractBitmapResource(
                resourceReader, builtInActivityType.IsGenericType ? builtInActivityType.Name.Split('`')[0] : builtInActivityType.Name);

            if (bitmap == null)
            {
                return;
            }

            var tbaType = typeof(ToolboxBitmapAttribute);

            var imageType = typeof(Image);

            var constructor = tbaType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { imageType, imageType }, null);

            var tba = constructor.Invoke(new object[] { bitmap, bitmap }) as ToolboxBitmapAttribute;

            builder.AddCustomAttributes(builtInActivityType, tba);
        }

        private static Bitmap ExtractBitmapResource(ResourceReader resourceReader, string bitmapName)
        {
            var dictEnum = resourceReader.GetEnumerator();

            Bitmap bitmap = null;

            while (dictEnum.MoveNext())
            {
                if (Equals(dictEnum.Key, bitmapName))
                {
                    bitmap = dictEnum.Value as Bitmap;

                    if (bitmap != null)
                    {
                        var pixel = bitmap.GetPixel(bitmap.Width - 1, 0);

                        bitmap.MakeTransparent(pixel);
                    }

                    break;
                }
            }

            return bitmap;
        }

        private static void LoadToolboxIconsForBuiltInActivities()
        {
            var builder = new AttributeTableBuilder();

            var sourceAssembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), @"Lib\Microsoft.VisualStudio.Activities.dll"));

            if (sourceAssembly != null)
            {
                var stream = sourceAssembly.GetManifestResourceStream("Microsoft.VisualStudio.Activities.Resources.resources");
                if (stream != null)
                {
                    var resourceReader = new ResourceReader(stream);

                    foreach (var type in typeof(Activity).Assembly.GetTypes().Where(t => t.Namespace == "System.Activities.Statements"))
                    {
                        CreateToolboxBitmapAttributeForActivity(builder, resourceReader, type);
                    }
                }
            }

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        private bool CanExecuteExit(object obj)
        {
            return true;
        }

        private bool CanExecuteNew(object obj)
        {
            return true;
        }

        private bool CanExecuteOpen(object obj)
        {
            return true;
        }

        private bool CanExecuteSave(object obj)
        {
            return true;
        }

        private bool CanExecuteSaveAs(object obj)
        {
            return true;
        }

        private ToolboxControl CreateToolbox()
        {
            var toolboxControl = new ToolboxControl();

            toolboxControl.Categories.Add(
                new ToolboxCategory("Control Flow")
                    {
                        new ToolboxItemWrapper(typeof(DoWhile)),
                        new ToolboxItemWrapper(typeof(ForEach<>)),
                        new ToolboxItemWrapper(typeof(If)),
                        new ToolboxItemWrapper(typeof(Parallel)),
                        new ToolboxItemWrapper(typeof(ParallelForEach<>)),
                        new ToolboxItemWrapper(typeof(Pick)),
                        new ToolboxItemWrapper(typeof(PickBranch)),
                        new ToolboxItemWrapper(typeof(Sequence)),
                        new ToolboxItemWrapper(typeof(Switch<>)),
                        new ToolboxItemWrapper(typeof(While)),
                    });

            toolboxControl.Categories.Add(
                new ToolboxCategory("Primitives")
                    {
                        new ToolboxItemWrapper(typeof(Assign)),
                        new ToolboxItemWrapper(typeof(Delay)),
                        new ToolboxItemWrapper(typeof(InvokeMethod)),
                        new ToolboxItemWrapper(typeof(WriteLine)),
                    });

            toolboxControl.Categories.Add(
                new ToolboxCategory("Error Handling")
                    {
                        new ToolboxItemWrapper(typeof(Rethrow)),
                        new ToolboxItemWrapper(typeof(Throw)),
                        new ToolboxItemWrapper(typeof(TryCatch)),
                    });

            toolboxControl.Categories.Add(
                new ToolboxCategory("HTTP")
                    {
                        new ToolboxItemWrapper(typeof(HttpReceiveFactory), "HttpReceive"),
                        new ToolboxItemWrapper(typeof(HttpWorkflowServiceFactory), "HttpWorkflowService"),
                    });

            return toolboxControl;
        }

        private void ExecuteExit(object obj)
        {
            Application.Current.Shutdown();
        }

        private void ExecuteNew(object obj)
        {
            this.WorkflowDesigner = new WorkflowDesigner();
            this.WorkflowDesigner.ModelChanged += this.WorkflowDesignerModelChanged;
            this.WorkflowDesigner.Load(TemplateXaml);
            this.WorkflowDesigner.Flush();
            this.FileName = UntitledXAML;
            this.Status = string.Format("Created new workflow from template {0}", TemplateXaml);
        }

        void WorkflowDesignerModelChanged(object sender, EventArgs e)
        {
            this.WorkflowDesigner.Flush();
        }

        private void ExecuteOpen(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                this.LoadWorkflow(openFileDialog.FileName);
            }
        }

        private void ExecuteSave(object obj)
        {
            if (this.FileName == UntitledXAML)
            {
                this.ExecuteSaveAs(obj);
            }
            else
            {
                this.Save();
            }
        }

        private void Save()
        {
            this.workflowDesigner.Save(this.FileName);
            this.Status = string.Format("Saved workflow file {0}", this.FileName);
        }

        private void ExecuteSaveAs(object obj)
        {
            var saveFileDialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    DefaultExt = "xaml",
                    FileName = this.FileName,
                    Filter =
                        "xaml files (*.xaml) | *.xaml;*.xamlx| All Files | *.*"
                };

            if (saveFileDialog.ShowDialog().Value)
            {
                this.FileName = saveFileDialog.FileName;
                this.Save();
            }
        }

        private void LoadWorkflow(string name)
        {
            this.ResolveImportedAssemblies(name);
            this.FileName = name;
            this.WorkflowDesigner = new WorkflowDesigner();
            this.WorkflowDesigner.Load(name);
        }

        private void Locate(XamlClrRef xamlClrRef)
        {
            this.Status = string.Format("Locate referenced assembly {0}", xamlClrRef.CodeBase);
            var openFileDialog = new OpenFileDialog
                {
                    FileName = xamlClrRef.CodeBase,
                    CheckFileExists = true,
                    Filter = "Assemblies (*.dll;*.exe)|*.dll;*.exe|All Files|*.*", 
                    Title = this.Status
                };

            if (openFileDialog.ShowDialog(Application.Current.MainWindow).Value)
            {
                if (!xamlClrRef.Load(openFileDialog.FileName))
                {
                    MessageBox.Show("Error loading assembly");
                }
            }
        }

        private void ResolveImportedAssemblies(string name)
        {
            var references = XamlClrReferences.Load(name);

            var query = from reference in references.References where !reference.Loaded select reference;
            foreach (var xamlClrRef in query)
            {
                this.Locate(xamlClrRef);
            }
        }

        #endregion
    }
}