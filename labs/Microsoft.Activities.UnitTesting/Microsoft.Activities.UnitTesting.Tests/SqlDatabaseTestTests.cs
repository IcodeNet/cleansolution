// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDatabaseTestTests.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.Tests
{
    using System;
    using System.Data.SqlClient;

    using Microsoft.Activities.UnitTesting.DurableInstancing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   Test for the SqlDatabaseTest class
    /// </summary>
    [TestClass]
    public class SqlDatabaseTestTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   Given
        ///   * A SqlDatabaseTest 
        ///   When
        ///   * The connection string property is read
        ///   Then
        ///   * A valid connection string is returned
        /// </summary>
        [TestMethod]
        public void ConnectionStringIsValid()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();

            // Act
            var actual = testdb.ConnectionString;

            // Assert
            Assert.IsTrue(IsValidConnectionString(actual));
        }

        /// <summary>
        ///   Given
        ///   * A database name
        ///   When
        ///   * You construct a SqlDatabaseTest with a database name argument
        ///   Then
        ///   * The DatabaseName property is the name passed to the ctor
        ///   * The DataSource is set to (local)\SQLExpress
        /// </summary>
        [TestMethod]
        public void CtorDatabaseNameArg()
        {
            const string Expected = "CtorDatabaseNameArg";
            using (var testdb = new SqlDatabaseTest(Expected))
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
        ///   * You construct a SqlDatabaseTest with a database name argument
        ///   Then
        ///   * The DatabaseName property is the name passed to the ctor
        ///   * The DataSource is set to (local)\SQLExpress
        /// </summary>
        [TestMethod]
        public void CtorDatabaseNameDataSourceArg()
        {
            const string DatabaseName = "DatabaseName";
            const string DataSource = "ServerName";
            using (var testdb = new SqlDatabaseTest(DatabaseName, DataSource))
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
        ///   Given
        ///   * Nothing
        ///   When
        ///   * You construct a SqlDatabaseTest with no arguments
        ///   Then
        ///   * You get a unique database name that begins with the DefaultPrefix
        ///   * The DataSource is set to (local)\SQLExpress
        /// </summary>
        [TestMethod]
        public void CtorNoArgs()
        {
            using (var testdb = new SqlDatabaseTest())
            {
                // Arrange

                // Act
                var actual = testdb.DatabaseName;
                var prefix = SqlDatabaseTest.DefaultPrefix;
                var server = testdb.DataSource;

                // Assert
                Assert.IsNotNull(actual);
                Assert.IsNotNull(prefix);
                Assert.IsTrue(actual.StartsWith(prefix));
                Assert.AreEqual(Constants.LocalSqlExpress, server);
            }
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * DefaultPrefix is checked
        ///   Then
        ///   * It is TestDatabase
        /// </summary>
        [TestMethod]
        public void DefaultDataSourceIsSet()
        {
            // Arrange

            // Act
            var actual = SqlDatabaseTest.DefaultDataSource;

            // Assert
            Assert.AreEqual(Constants.LocalSqlExpress, actual);
        }

        /// <summary>
        ///   Given
        ///   * Nothing
        ///   When
        ///   * DefaultPrefix is checked
        ///   Then
        ///   * It is TestDatabase
        /// </summary>
        [TestMethod]
        public void DefaultPrefixIsSet()
        {
            // Arrange

            // Act
            var actual = SqlDatabaseTest.DefaultPrefix;

            // Assert
            Assert.AreEqual(Constants.DefaultTestDatabaseName, actual);
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The connection string property is read
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessConnectionStringThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException>(() => { var actual = testdb.ConnectionString; });
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Create method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessCreateThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException>(() => testdb.Create());
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Drop method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessDropThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException>(testdb.Drop);
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Exists method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessExistsThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException, bool>(testdb.Exists);
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Exists method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessProcedureExistsThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException, bool>(() => testdb.ProcedureExists("Foo"));
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server connection string property is read
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessServerConnectionStringThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException>(() => { var actual = testdb.ServerConnectionString; });
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Exists method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessTableExistsThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException, bool>(() => testdb.TableExists("Foo"));
        }

        /// <summary>
        ///   Given
        ///   * A Disposed SqlDatabaseTest 
        ///   When
        ///   * The server Exists method is invoked
        ///   Then
        ///   * An ObjectDisposed exception is thrown
        /// </summary>
        [TestMethod]
        public void DisposedAccessViewExistsThrows()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();
            testdb.Dispose();

            // Act
            AssertHelper.Throws<ObjectDisposedException, bool>(() => testdb.ViewExists("Foo"));
        }

        /// <summary>
        ///   Given
        ///   * A configured SqlWorkflowInstanceStore database
        ///   When
        ///   * ProcedureExists is invoked with a sproc name that does exist
        ///   Then
        ///   * ProcedureExists returns false
        /// </summary>
        [TestMethod]
        public void ProcedureExistsReturnsTrueWhenProcDoesExist()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                var actual = testdb.ProcedureExists("LoadInstance");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * An empty database
        ///   When
        ///   * ProcedureExists is invoked with a sproc name that does not exist
        ///   Then
        ///   * ProcedureExists returns false
        /// </summary>
        [TestMethod]
        public void ProcedureExistsReturnsTrueWhenProcDoesNotExist()
        {
            using (var testdb = new SqlDatabaseTest())
            {
                // Arrange
                testdb.Create();

                // Act
                var actual = testdb.ProcedureExists("NonExistent");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A SqlDatabaseTest 
        ///   When
        ///   * The connection string property is read
        ///   Then
        ///   * A valid connection string is returned
        ///   * The connection string does not have the InitialCatalog value
        /// </summary>
        [TestMethod]
        public void ServerConnectionStringIsValid()
        {
            // Arrange
            var testdb = new SqlDatabaseTest();

            // Act
            var actual = testdb.ServerConnectionString;

            // Assert
            Assert.IsTrue(IsValidConnectionString(actual));
            var builder = new SqlConnectionStringBuilder(actual);
            Assert.IsTrue(string.IsNullOrEmpty(builder.InitialCatalog));
        }

        /// <summary>
        ///   Given
        ///   * A new SqlDatabaseTest
        ///   When
        ///   * Create is invoked
        ///   Then
        ///   * A new empty database is created
        /// </summary>
        [TestMethod]
        public void SqlDatabaseTestShouldCreate()
        {
            using (var testdb = new SqlDatabaseTest())
            {
                // Arrange

                // Act
                testdb.Create();

                // Assert
                Assert.IsTrue(testdb.Exists(), "Create did not create test datbase");
            }
        }

        /// <summary>
        ///   Given
        ///   * A configured SqlWorkflowInstanceStore database
        ///   When
        ///   * TableExists is invoked with a name that does exist
        ///   Then
        ///   * TableExists returns false
        /// </summary>
        [TestMethod]
        public void TableExistsReturnsTrueWhenTableDoesExist()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                var actual = testdb.TableExists("InstancesTable");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * An empty database
        ///   When
        ///   * TableExists is invoked with a name that does not exist
        ///   Then
        ///   * TableExists returns false
        /// </summary>
        [TestMethod]
        public void TableExistsReturnsTrueWhenTableDoesNotExist()
        {
            using (var testdb = new SqlDatabaseTest())
            {
                // Arrange
                testdb.Create();

                // Act
                var actual = testdb.TableExists("NonExistent");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * A configured SqlWorkflowInstanceStore database
        ///   When
        ///   * ViewExists is invoked with a name that does exist
        ///   Then
        ///   * ViewExists returns false
        /// </summary>
        [TestMethod]
        public void ViewExistsReturnsTrueWhenViewDoesExist()
        {
            using (var testdb = new SqlWorkflowInstanceStoreTest())
            {
                // Arrange
                testdb.CreateInstanceStore();

                // Act
                var actual = testdb.ViewExists("Instances");

                // Assert
                Assert.IsTrue(actual);
            }
        }

        /// <summary>
        ///   Given
        ///   * An empty database
        ///   When
        ///   * ViewExists is invoked with a name that does not exist
        ///   Then
        ///   * ViewExists returns false
        /// </summary>
        [TestMethod]
        public void ViewExistsReturnsTrueWhenViewDoesNotExist()
        {
            using (var testdb = new SqlDatabaseTest())
            {
                // Arrange
                testdb.Create();

                // Act
                var actual = testdb.ViewExists("NonExistent");

                // Assert
                Assert.IsFalse(actual);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if a connection string is valid
        /// </summary>
        /// <param name="actual">
        /// The actual. 
        /// </param>
        /// <returns>
        /// true if valid, false if not 
        /// </returns>
        private static bool IsValidConnectionString(string actual)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(actual);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}