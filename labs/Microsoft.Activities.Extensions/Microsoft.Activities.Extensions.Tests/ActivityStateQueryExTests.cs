// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityStateQueryExTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   ActivityStateQueryEx Tests
    /// </summary>
    [TestClass]
    public class ActivityStateQueryExTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * An ActivityScheduleQuery 
        ///   When
        ///   * ToFormattedString is invoked
        ///   Then
        ///   * The formatted string should be returned
        /// </summary>
        [TestMethod]
        public void ActivityStateQueryToFormattedShouldFormat()
        {
            const string Expected = @"ActivityStateQuery
{
	ActivityName: ActivityName
	Arguments
	{
		Arg1
		Arg2
	}
	QueryAnnotations
	{
		Annotation1: Annotation 1
		Annotation2: Annotation 2
	}
	States
	{
		Canceled
		Closed
	}
	Variables
	{
		Var1
		Var2
	}
}
";

            // Arrange
            var asq = new ActivityStateQuery
                {
                    ActivityName = "ActivityName", 
                    Arguments = {
                                   "Arg1", "Arg2" 
                                }, 
                    QueryAnnotations = {
                                          { "Annotation1", "Annotation 1" }, { "Annotation2", "Annotation 2" } 
                                       }, 
                    States = {
                                ActivityInstanceStates.Canceled, ActivityInstanceStates.Closed 
                             }, 
                    Variables = {
                                   "Var1", "Var2" 
                                }
                };

            // Act
            var actual = asq.ToFormattedString();

            // Assert
            Assert.AreEqual(Expected, actual);
        }

        #endregion
    }
}