// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWorkflowInstanceStoreManagerTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Data.SqlClient;

    using Microsoft.Activities.Extensions.DurableInstancing;
    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Contains tests for SqlWorkflowInstanceStoreExtensions
    /// </summary>
    [TestClass]
    public class SqlWorkflowInstanceStoreManagerTests
    {
        #region Constants

        /// <summary>
        ///   Attacker SQLInjection string
        /// </summary>
        private const string EvilDatabase = "TestDatabase; DROP Database Master";

        /// <summary>
        ///   A database name that does not exist
        /// </summary>
        private const string NonExistentDatabase = "NonExistentDatabase";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A connection string
        ///   When
        ///   * ConfigureInstanceStore is invoked
        ///   Then
        ///   * The schema and logic should be applied to the database
        /// </summary>
        [TestMethod]
        public void ConfigureInstanceStoreShouldConfigure()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Create a plain database without running the scripts
                testdb.Create();

                // Act
                // Configure the database with the scripts
                SqlWorkflowInstanceStoreManager.ConfigureInstanceStore(
                    testdb.DatabaseName, 
                    testdb.ConnectionString);

                // Assert
                testdb.AssertDatabaseIsConfigured();
            }
        }

        /// <summary>
        ///   Given
        ///   * An evil database name
        ///   When
        ///   * ConfigureInstanceStore is invoked
        ///   Then
        ///   * An ArgumentException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigureInstanceStoreWithEvilDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Create a plain database without running the scripts
                testdb.Create();

                // Act
                SqlWorkflowInstanceStoreManager.ConfigureInstanceStore(
                    EvilDatabase, testdb.ConnectionString);

                // Assert
                testdb.AssertDatabaseIsConfigured();
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid connection string
        ///   When
        ///   * ConfigureInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigureInstanceStoreWithInvalidConnShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.ConfigureInstanceStore(
                    testdb.DatabaseName, string.Empty);

                // Assert
                testdb.AssertDatabaseIsConfigured();
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid database name
        ///   When
        ///   * ConfigureInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConfigureInstanceStoreWithInvalidDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.ConfigureInstanceStore(
                    string.Empty, testdb.ConnectionString);

                // Assert
                testdb.AssertDatabaseIsConfigured();
            }
        }

        /// <summary>
        ///   Given
        ///   * An evil database name
        ///   When
        ///   * CreateInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateInstanceStoreWithEvilDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                    EvilDatabase, testdb.ConnectionString);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid connection string
        ///   When
        ///   * CreateInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceStoreWithInvalidConnShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                    testdb.DatabaseName, string.Empty);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid database name
        ///   When
        ///   * CreateInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceStoreWithInvalidDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                    string.Empty, testdb.ConnectionString);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * A SQL Server
        ///   When
        ///   * Create is invoked
        ///   Then
        ///   * The InstanceStore is created
        /// </summary>
        [TestMethod]
        public void CreateShouldCreateInstanceStore()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.CreateInstanceStore(
                    testdb.DatabaseName, 
                    testdb.ConnectionString, 
                    true);

                // Assert
                Assert.IsTrue(testdb.Exists(), "Database does not exist");
                testdb.AssertDatabaseIsConfigured();
            }
        }

        /// <summary>
        ///   Given
        ///   * An evil database name
        ///   When
        ///   * DropInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DropInstanceStoreWithEvilDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.DropInstanceStore(
                    EvilDatabase, testdb.ConnectionString);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid connection string
        ///   When
        ///   * DropInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DropInstanceStoreWithInvalidConnShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.DropInstanceStore(
                    testdb.DatabaseName, string.Empty);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * An invalid database name
        ///   When
        ///   * DropInstanceStore is invoked
        ///   Then
        ///   * An ArgumentNullException is thrown
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DropInstanceStoreWithInvalidDbShouldThrow()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                SqlWorkflowInstanceStoreManager.DropInstanceStore(
                    string.Empty, testdb.ConnectionString);

                // Assert
            }
        }

        /// <summary>
        ///   Given
        ///   * A database
        ///   * A connection to the database
        ///   When
        ///   * DropInstanceStore is invoked
        ///   Then
        ///   * Connections are closed
        ///   * The database is dropped
        /// </summary>
        [TestMethod]
        public void DropShouldCloseConnections()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.Create();

                // Act
                using (var connection = new SqlConnection(testdb.ConnectionString))
                {
                    // Open a connection and keep it open
                    connection.Open();

                    // DropInstanceStore will force the connection closed and drop the database
                    SqlWorkflowInstanceStoreManager.DropInstanceStore(
                        testdb.DatabaseName, 
                        testdb.ConnectionString);
                }

                // Assert
                Assert.IsFalse(testdb.Exists());
            }
        }

        /// <summary>
        ///   Given
        ///   * An InstanceStore Database
        ///   When
        ///   * Drop is invoked
        ///   Then
        ///   * The database is dropped
        /// </summary>
        [TestMethod]
        public void DropShouldDropDatabase()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                SqlWorkflowInstanceStoreManager.DropInstanceStore(
                    testdb.DatabaseName, 
                    testdb.ConnectionString);

                // Assert
                Assert.IsFalse(testdb.Exists());
            }
        }

        /// <summary>
        ///   Given
        ///   * A connection string and database name for a database that does not exist
        ///   When
        ///   * Exists is invoked
        ///   Then
        ///   * Exists returns false
        /// </summary>
        [TestMethod]
        public void ExistsFalseForNonExistentDatabase()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange

                // Act
                var actual = SqlWorkflowInstanceStoreManager.InstanceStoreExists(
                    NonExistentDatabase, testdb.ConnectionString);

                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A connection string and database name for a database that does exist
        ///   When
        ///   * Exists is invoked
        ///   Then
        ///   * Exists returns false
        /// </summary>
        [TestMethod]
        public void ExistsTrueForExistentDatabase()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                var actual = SqlWorkflowInstanceStoreManager.InstanceStoreExists(
                    testdb.DatabaseName, 
                    testdb.ConnectionString);

                // Assert
                Assert.IsTrue(actual);
            }
        }

        #endregion
    }
}