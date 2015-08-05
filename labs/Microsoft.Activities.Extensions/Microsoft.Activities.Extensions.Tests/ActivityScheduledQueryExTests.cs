// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityScheduledQueryExTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities.Tracking;

    using Microsoft.Activities.Extensions.Tracking;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   The activity scheduled query ex tests.
    /// </summary>
    [TestClass]
    public class ActivityScheduledQueryExTests
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
        public void ActivityScheduledQueryToFormattedShouldFormat()
        {
            const string Expected = @"ActivityScheduledQuery
{
	ActivityName: ActivityName
	ChildActivityName: ChildActivityName
	QueryAnnotations
	{
		Annotation1: Annotation 1
		Annotation2: Annotation 2
	}
}
";

            // Arrange
            var asq = new ActivityScheduledQuery
                {                    
                    ActivityName = "ActivityName", 
                    ChildActivityName = "ChildActivityName", 
                    QueryAnnotations = {
                                          { "Annotation1", "Annotation 1" }, { "Annotation2", "Annotation 2" } 
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