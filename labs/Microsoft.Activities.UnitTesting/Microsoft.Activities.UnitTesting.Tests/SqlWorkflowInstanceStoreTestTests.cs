// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWorkflowInstanceStoreTestTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the SqlWorkflowInstanceStoreTest class
    /// </summary>
    [TestClass]
    public class SqlWorkflowInstanceStoreTestTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * A SqlWorkflowInstanceStoreTest is created using the default ctor
        ///   Then
        ///   * No exception is thrown
        /// </summary>
        [TestMethod]
        public void CtorNoArg()
        {
            // Arrange

            // Act
            var actual = new SqlWorkflowInstanceStoreTest();

            // Assert
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///   Given
        ///   * A database name
        ///   When
        ///   * You construct a SqlWorkflowInstanceStoreTest with a database name argument
        ///   Then
        ///   * The DatabaseName property is the name passed to the ctor
        ///   * The DataSource is set to (local)\SQLExpress
        /// </summary>
        [TestMethod]
        public void CtorDatabaseNameArg()
        {
            const string Expected = "CtorDatabaseNameArg";
            using (var testdb = new SqlWorkflowInstanceStoreTest(Expected))
            {
                // Arrange

                // Act
                var actual = testdb.DatabaseName;
                var dataSource = testdb.DataSource;

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(Expected, actual);
                Assert.AreEqual(Constants.LocalSqlExpress, dataSource);
            }
        }

        /// <summary>
        ///   Given
        ///   * A database name
        ///   When
        ///   * You construct a SqlWorkflowInstanceStoreTest with a database name argument
        ///   Then
        ///   * The DatabaseName property is the name passed to the ctor
        ///   * The DataSource is set to (local)\SQLExpress
        /// </summary>
        [TestMethod]
        public void CtorDatabaseNameDataSourceArg()
        {
            const string DatabaseName = "DatabaseName";
            const string DataSource = "ServerName";
            using (var testdb = new SqlWorkflowInstanceStoreTest(DatabaseName, DataSource))
            {
                // Arrange

                // Act
                var actual = testdb.DatabaseName;
                var dataSource = testdb.DataSource;

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(DatabaseName, actual);
                Assert.AreEqual(DataSource, dataSource);
            }
        }

        /// <summary>
        /// Given
        /// * An empty database
        /// When
        /// * Configure is invoked
        /// Then
        /// * The database is configured as a valid instance store
        /// </summary>        
        [TestMethod]
        public void ConfigureConfiguresInstanceStore()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.Create();

                // Act
                testdb.ConfigureInstanceStore();

                // Assert
                testdb.AssertDatabaseIsConfigured();
            }
        }

        #endregion
    }
}