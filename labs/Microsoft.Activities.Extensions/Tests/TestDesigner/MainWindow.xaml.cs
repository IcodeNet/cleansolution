// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDesigner
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Toolbox;
    using System.Activities.Statements;
    using System.Windows.Controls;

    using Microsoft.Activities.Extensions.Statements;

    using DesignerMetadata = System.Activities.Core.Presentation.DesignerMetadata;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants and Fields

        /// <summary>
        ///   The workflow designer.
        /// </summary>
        private WorkflowDesigner workflowDesigner = new WorkflowDesigner();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            RegisterMetadata();
            this.AddDesigner();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create toolbox control.
        /// </summary>
        /// <returns>
        /// The toolbox control
        /// </returns>
        private static ToolboxControl CreateToolboxControl()
        {
            return new ToolboxControl
                {
                    Categories =
                        {
                            new ToolboxCategory("Test Workflow")
                                {
                                    Tools =
                                        {
                                            new ToolboxItemWrapper(typeof(Assign)), 
                                            new ToolboxItemWrapper(typeof(Sequence)), 
                                            new ToolboxItemWrapper(typeof(TryCatch)), 
                                            new ToolboxItemWrapper(typeof(DelayUntilTime)), 
                                            new ToolboxItemWrapper(typeof(InvokeWorkflow)), 
                                            new ToolboxItemWrapper(typeof(LoadAndInvokeWorkflow)), 
                                            new ToolboxItemWrapper(typeof(LoadActivity)),
                                            new ToolboxItemWrapper(typeof(LoadAssembly)),
                                            new ToolboxItemWrapper(typeof(AddToDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(ClearDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(GetFromDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(KeyExistsInDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(RemoveFromDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(ValueExistsInDictionary<,>)),
                                            new ToolboxItemWrapper(typeof(DelayUntilDateTime)),
                                        }
                                }
                        }
                };
        }

        /// <summary>
        /// The register metadata.
        /// </summary>
        private static void RegisterMetadata()
        {
            var metaData = new DesignerMetadata();
            metaData.Register();
            Microsoft.Activities.Extensions.Design.DesignerMetadata.RegisterAll();
        }

        /// <summary>
        /// The add designer.
        /// </summary>
        private void AddDesigner()
        {
            // Create an instance of WorkflowDesigner class
            this.workflowDesigner = new WorkflowDesigner();

            // Place the WorkflowDesigner in the middle column of the grid
            Grid.SetColumn(this.workflowDesigner.View, 1);

            // Show the XAML when the model changes
            this.workflowDesigner.ModelChanged += this.ShowWorkflowXaml;

            // Load a new Sequence as default.
            this.workflowDesigner.Load(new Sequence());

            // Add the WorkflowDesigner to the grid
            this.grid1.Children.Add(this.workflowDesigner.View);

            // Add the Property Inspector
            Grid.SetColumn(this.workflowDesigner.PropertyInspectorView, 2);
            this.grid1.Children.Add(this.workflowDesigner.PropertyInspectorView);

            // Add the toolbox
            ToolboxControl tc = CreateToolboxControl();
            Grid.SetColumn(tc, 0);
            this.grid1.Children.Add(tc);

            // Show the initial XAML
            this.ShowWorkflowXaml(null, null);
        }

        /// <summary>
        /// The show workflow xaml.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event args.
        /// </param>
        private void ShowWorkflowXaml(object sender, EventArgs e)
        {
            this.workflowDesigner.Flush();
            this.textXAML.Text = this.workflowDesigner.Text;
        }

        #endregion
    }
}