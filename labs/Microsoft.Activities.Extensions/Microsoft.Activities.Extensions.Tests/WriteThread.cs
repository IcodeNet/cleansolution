namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Diagnostics;

    /// <summary>
    /// The write thread.
    /// </summary>
    public class WriteThread : CodeActivity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            WorkflowTrace.Options = TraceOptions.ThreadId;
            WorkflowTrace.Information(this.Message);
        }

        #endregion
    }
}