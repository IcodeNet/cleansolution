#region copyright

//  ----------------------------------------------------------------------------------
//  Microsoft
//  
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//  ----------------------------------------------------------------------------------
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
//  ----------------------------------------------------------------------------------

#endregion

using System.Activities;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Activities.UnitTesting;

namespace UnitTestingBookmarks.Tests
{
    /// <summary>
    ///   Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTestBookmarks
    {
        #region TestContext

        ///<summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void ShouldOutputGreeting()
        {
            // Arrange
            const string expectedFirstName = "Test";
            const string expectedLastName = "User";
            var expectedGreeting = string.Format("Hello {0} {1}", expectedFirstName, expectedLastName);
            var sut = WorkflowApplicationTest.Create(new TestReadLine());

            // Act

            // Run the workflow
            sut.TestActivity();

            // Wait for the first idle event - prompt for First Name
            // will return false if the activity does not go idle within the
            // timeout (default 1 sec)
            Assert.IsTrue(sut.WaitForIdleEvent());

            // Should have a bookmark named "FirstName"
            Assert.IsTrue(sut.Bookmarks.Contains("FirstName"));

            Assert.AreEqual(BookmarkResumptionResult.Success,
                            sut.TestWorkflowApplication.ResumeBookmark("FirstName", expectedFirstName));

            // Wait for the second idle event - prompt for Last Name
            Assert.IsTrue(sut.WaitForIdleEvent());

            // Should have a bookmark named "LastName"
            Assert.IsTrue(sut.Bookmarks.Contains("LastName"));

            Assert.AreEqual(BookmarkResumptionResult.Success,
                            sut.TestWorkflowApplication.ResumeBookmark("LastName", expectedLastName));

            // Wait for the workflow to complete
            Assert.IsTrue(sut.WaitForCompletedEvent());

            // Assert
            // WorkflowApplicationTest.TextLines returns an array of strings
            // that contains strings written by the WriteLine activity
            Assert.AreEqual(4, sut.TextLines.Length);
            Assert.AreEqual(expectedGreeting, sut.TextLines[2]);
        }
    }
}