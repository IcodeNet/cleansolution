namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;

    public class TestCustomActivity : CodeActivity
    {
        #region Methods

        protected override void Execute(CodeActivityContext context)
        {
            context.Track(new TestCustomTrackingRecord("MyRecord") { Value = CustomTrackingTraceTests.TestValue });
        }

        #endregion
    }
}