// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListTrackingParticipantTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Activities.Statements;
    using System.Windows;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.Activities.UnitTesting;
    using Microsoft.Activities.UnitTesting.Activities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The list tracking participant tests.
    /// </summary>
    [TestClass]
    public class ListTrackingParticipantTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list tracing should format string.
        /// </summary>
        [TestMethod]
        public void ListTracingShouldFormatString()
        {
            // Arrange
            const string Expected = @"ListTrackingParticipant
{
	0: WorkflowInstance ""Sequence"" is Started
	1: Activity [null] ""null"" scheduled child activity [1] ""Sequence""
	2: Activity [1] ""Sequence"" is Executing
	3: Activity [1] ""Sequence"" scheduled child activity [4] ""EchoArg<Int32>""
	4: Activity [4] ""EchoArg<Int32>"" is Executing
	{
		Arguments
		{
			Value: 123
		}
	}

	5: Activity [4] ""EchoArg<Int32>"" is Closed
	{
		Arguments
		{
			Value: 123
			Result: 123
		}
	}

	6: Activity [1] ""Sequence"" scheduled child activity [2] ""WaitForBookmark""
	7: Activity [2] ""WaitForBookmark"" is Executing
	{
		Arguments
		{
			BookmarkName: Test
		}
	}

	8: WorkflowInstance ""Sequence"" is Idle
}
";

            WorkflowTrace.Information("Arrange");
            var activity = new Sequence
                {
                    Activities =
                        {
                            new EchoArg<int> { Value = new InArgument<int>(123) },
                            new WaitForBookmark { BookmarkName = "Test" }, 
                        }
                };
            var listTrackingParticipant = new ListTrackingParticipant();
            var workflow = WorkflowApplicationTest.Create(activity);
            workflow.Extensions.Add(listTrackingParticipant);
                // Act
                WorkflowTrace.Information("Act");

                // Run until idle with the Test extension
                workflow.TestWorkflowApplication.RunUntilBookmark("Test");
                var actual = listTrackingParticipant.ToFormattedString();

                // Assert
                Assert.AreEqual(Environment.NewLine + Expected, Environment.NewLine + actual);
        }

        #endregion
    }
}