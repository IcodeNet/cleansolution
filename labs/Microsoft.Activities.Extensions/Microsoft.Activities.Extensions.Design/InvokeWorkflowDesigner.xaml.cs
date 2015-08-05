namespace Microsoft.Activities.Extensions.Design
{
    using System.Activities.Presentation.Metadata;
    using System.ComponentModel;
    using System.Drawing;

    using Microsoft.Activities.Extensions.Statements;

    // Interaction logic for InvokeWorkflowDesigner.xaml
    public partial class InvokeWorkflowDesigner
    {
        #region Constructors and Destructors

        public InvokeWorkflowDesigner()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The register metadata.
        /// </summary>
        /// <param name = "builder">
        ///   The builder.
        /// </param>
        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(typeof(InvokeWorkflow), new DesignerAttribute(typeof(InvokeWorkflowDesigner)));
            builder.AddCustomAttributes(
                typeof(InvokeWorkflow),
                new DescriptionAttribute(Properties.Resources.Invokes_an_Activity_using_WorkflowInvoker));
            builder.AddCustomAttributes(typeof(InvokeWorkflow), new ToolboxBitmapAttribute("InvokeWorkflow.bmp"));
        }

        #endregion
    }
}