using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities.Statements;

    using Microsoft.Activities.Extensions.Prototype;
    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;

    [TestClass]
    public class TaskActivityTest
    {

        /// <summary>
        /// Given
        /// * A Text File
        /// * A FileReadToEnd activity
        /// When
        /// * The activity is run
        /// Then
        /// * The activity will read to end and return the result
        /// </summary>
        [TestMethod]
        [DeploymentItem(Constants.Workflow1Path)]
        public void TestMethod1()
        {
            // Arrange
            WorkflowTestTrace.Arrange();
            var activity = new FileReadToEnd() { FileName = Constants.Workflow1Xaml };
            var tracking = new ListTrackingParticipant();
            var workflow = new Workflow(activity) { Tracking = tracking };

            try
            {
                // Act
                WorkflowTestTrace.Act();
                var result = workflow.Start().Result.Output.Result;
                
                // Assert
                WorkflowTestTrace.Assert();
                
                Assert.AreEqual(2113, result.Length);
            }
            finally
            {
                WorkflowTestTrace.Finally();
                workflow.Trace();
            }
        }

    }
}
