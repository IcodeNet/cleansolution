namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Threading;

    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The test task async.
    /// </summary>
    public class TestTaskNativeActivity : TaskNativeActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the int result.
        /// </summary>
        public OutArgument<int> IntResult { get; set; }

        /// <summary>
        ///   Gets or sets the int value.
        /// </summary>
        public InArgument<int> IntValue { get; set; }

        /// <summary>
        ///   Gets or sets the string result.
        /// </summary>
        public OutArgument<string> StringResult { get; set; }

        /// <summary>
        ///   Gets or sets the string value.
        /// </summary>
        public InArgument<string> StringValue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        protected override void Execute(NativeActivityContext context, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            this.StringResult.Set(context, this.StringValue.Get(context));
            this.IntResult.Set(context, this.IntValue.Get(context));
        }

        #endregion
    }
}