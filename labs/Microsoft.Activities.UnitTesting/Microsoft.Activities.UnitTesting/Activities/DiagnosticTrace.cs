// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticTrace.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Activities
{
    using System.Activities;
    using System.Activities.Tracking;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Markup;

    using Microsoft.Activities.Extensions;

    /// <summary>
    /// An activity that will output a System.Diagnostics.Trace message as well as a custom tracking record
    /// </summary>
    [Designer("Microsoft.Activities.UnitTesting.Activities.Designers.DiagnosticTraceDesigner, Microsoft.Activities.UnitTesting.Activities")]
    public sealed class DiagnosticTrace : CodeActivity
    {
        #region Constants and Fields

        /// <summary>
        ///   The default record name.
        /// </summary>
        private const string DefaultRecordName = "Diagnostic Trace";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DiagnosticTrace" /> class.
        /// </summary>
        public DiagnosticTrace()
        {
            this.Level = TraceLevel.Off;
            this.Category = DefaultRecordName;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the category of the trace
        /// </summary>
        /// <remarks>
        ///   The category name is also used as the record name for 
        ///   the CustomTrackingRecord
        /// </remarks>
        [DependsOn("Text")]
        [DefaultValue(null)]
        public string Category { get; set; }

        /// <summary>
        ///   Gets or sets the trace level for this message
        /// </summary>
        [DependsOn("Category")]
        [DefaultValue("Off")]
        public TraceLevel Level { get; set; }

        /// <summary>
        ///   Gets or sets the text of the message
        /// </summary>
        [DefaultValue(null)]
        public InArgument<string> Text { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The cache metadata.
        /// </summary>
        /// <param name="metadata">
        /// The metadata.
        /// </param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            var textArg = new RuntimeArgument("Text", typeof(string), ArgumentDirection.In, true);
            metadata.AddArgument(textArg);

            if (string.IsNullOrEmpty(this.Category))
            {
                metadata.AddValidationError("Category must not be empty");
            }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void Execute(CodeActivityContext context)
        {
            var text = context.GetValue(this.Text);

            // Debug.WriteLine(text);
            switch (this.Level)
            {
                case TraceLevel.Error:
                    Trace.TraceError(text);
                    break;
                case TraceLevel.Info:
                    Trace.TraceInformation(text);
                    break;
                case TraceLevel.Verbose:
                    WorkflowTrace.Information(text, this.Category);
                    break;
                case TraceLevel.Warning:
                    Trace.TraceWarning(text);
                    break;
            }

            if (this.Level != TraceLevel.Off)
            {
                var trackRecord = new CustomTrackingRecord(this.Category, this.Level);
                trackRecord.Data.Add("Text", text);
                trackRecord.Data.Add("Category", this.Category);

                context.Track(trackRecord);
            }
        }

        #endregion
    }
}