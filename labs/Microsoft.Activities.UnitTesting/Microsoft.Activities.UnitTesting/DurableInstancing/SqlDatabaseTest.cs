// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDatabaseTest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.UnitTesting.DurableInstancing
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Win32;

    /// <summary>
    ///   Class for creating test databases
    /// </summary>
    public class SqlDatabaseTest : IDisposable
    {
        #region Constants

        /// <summary>
        ///   The drop databases prefix sql.
        /// </summary>
        private const string DropDatabasesPrefixSql = @"
declare @dbnames nvarchar(max) = '';
declare @statement nvarchar(max) = '';
select @dbnames = @dbnames + ',[' + name + ']' from sys.databases where name like '{0}%';
if @dbnames IS NULL
    begin
		RAISERROR (N'No database to drop', 1, 1) WITH NOWAIT;
    end
else
    begin
		set @statement = 'drop database ' + substring(@dbnames, 2, len(@dbnames));
		print @statement;
		exec sp_executesql @statement;
    end";

        /// <summary>
        ///   The drop if exists sql
        /// </summary>
        private const string DropIfExistsSql = @"
IF EXISTS (SELECT 1 FROM sys.databases WHERE Name = '{0}') 
BEGIN 
    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
    USE [MASTER]; 
    DROP DATABASE [{0}]
END";

        /// <summary>
        ///   LocalDb connection
        /// </summary>
        private const string LocalDb = @"(LocalDB)\v11.0";

        /// <summary>
        ///   SQLExpress connection
        /// </summary>
        private const string SqlExpress = @"(local)\SQLExpress";

        #endregion

        #region Fields

        /// <summary>
        ///   The connection string builder
        /// </summary>
        private readonly SqlConnectionStringBuilder builder;

        /// <summary>
        ///   The sync lock
        /// </summary>
        private readonly object syncLock = new object();

        /// <summary>
        ///   The disposed flag
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="SqlDatabaseTest" /> class. 
        /// </summary>
        static SqlDatabaseTest()
        {
            DefaultPrefix = "TestDatabase";

            DefaultDataSource =
                Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\11.0", 
                    "InstanceAPIPath", 
                    null) != null
                    ? LocalDb
                    : SqlExpress;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlDatabaseTest" /> class.
        /// </summary>
        public SqlDatabaseTest()
            : this(GenerateUniqueName(DefaultPrefix), DefaultDataSource)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseTest"/> class.
        /// </summary>
        /// <param name="databaseName">
        /// The name. 
        /// </param>
        public SqlDatabaseTest(string databaseName)
            : this(databaseName, DefaultDataSource)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseTest"/> class.
        /// </summary>
        /// <param name="databaseName">
        /// The name. 
        /// </param>
        /// <param name="dataSource">
        /// The server. 
        /// </param>
        public SqlDatabaseTest(string databaseName, string dataSource)
        {
            this.DatabaseName = databaseName;
            this.builder = new SqlConnectionStringBuilder
                {
                   InitialCatalog = this.DatabaseName, IntegratedSecurity = true, Pooling = false, ConnectTimeout = 1 
                };

            if (dataSource != null)
            {
                this.DataSource = dataSource;
                this.builder.DataSource = this.DataSource;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the default data source.
        /// </summary>
        public static string DefaultDataSource { get; set; }

        /// <summary>
        ///   Gets or sets the DefaultPrefix
        /// </summary>
        public static string DefaultPrefix { get; set; }

        /// <summary>
        ///   Gets ConnectionString.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                Debug.Assert(this.builder != null, "sqlConnectionStringBuilder != null");
                return this.builder.ConnectionString;
            }
        }

        /// <summary>
        ///   Gets or sets Server.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        ///   Gets the test database name
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        ///   Gets ServerConnectionString.
        /// </summary>
        public string ServerConnectionString
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                Debug.Assert(this.builder != null, "sqlConnectionStringBuilder != null");
                var serverBuilder = new SqlConnectionStringBuilder(this.builder.ConnectionString);
                serverBuilder.Remove("Initial Catalog");
                return serverBuilder.ConnectionString;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Drops all databases with specified prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix. 
        /// </param>
        /// <param name="dataSource">
        /// The name of the server 
        /// </param>
        public static void DropDatabasesWithPrefix(string prefix = null, string dataSource = null)
        {
            var builder = new SqlConnectionStringBuilder
                {
                   DataSource = dataSource ?? DefaultDataSource, IntegratedSecurity = true, InitialCatalog = "Master" 
                };
            using (var sqlConnection = new SqlConnection(builder.ToString()))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText = string.Format(DropDatabasesPrefixSql, prefix ?? DefaultPrefix);
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Generates a unique name starting with the prefix
        /// </summary>
        /// <param name="prefix">
        /// The prefix. 
        /// </param>
        /// <returns>
        /// The generate unique name. 
        /// </returns>
        public static string GenerateUniqueName(string prefix)
        {
            return prefix + Guid.NewGuid().ToString().Replace('-', '_');
        }

        /// <summary>
        /// The try drop databases with prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix. 
        /// </param>
        /// <param name="dataSource">
        /// The data source. 
        /// </param>
        /// <returns>
        /// The System.Boolean. 
        /// </returns>
        public static bool TryDropDatabasesWithPrefix(string prefix = null, string dataSource = null)
        {
            try
            {
                DropDatabasesWithPrefix(prefix, dataSource);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="dropIfExists">
        /// The drop If Exists. 
        /// </param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", 
            Justification = "Cannot use a parameter for database name in DDL")]
        public void Create(bool dropIfExists = true)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ServerConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();

                if (dropIfExists)
                {
                    sqlCommand.CommandText =
                        string.Format(
                            "IF EXISTS (SELECT 1 FROM sys.databases WHERE Name = @DatabaseName) DROP DATABASE [{0}]", 
                            this.DatabaseName);
                    sqlCommand.Parameters.AddWithValue("@DatabaseName", this.DatabaseName);
                    sqlCommand.ExecuteNonQuery();
                }

                sqlCommand.CommandText = string.Format("CREATE DATABASE [{0}]", this.DatabaseName);
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!this.disposed)
            {
                lock (this.syncLock)
                {
                    this.Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        ///   Drops the database
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", 
            Justification = "Cannot use parameter with DDL")]
        public void Drop()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ServerConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText = string.Format(DropIfExistsSql, this.DatabaseName);
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///   Determines if a database exists
        /// </summary>
        /// <returns> True if the database exists. </returns>
        public bool Exists()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ServerConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText = "SELECT 1 FROM sys.databases WHERE Name = @DatabaseName";
                sqlCommand.Parameters.AddWithValue("@DatabaseName", this.DatabaseName);
                var result = sqlCommand.ExecuteScalar();
                return result != null && (int)result == 1;
            }
        }

        /// <summary>
        /// Determines if a stored procedure exists
        /// </summary>
        /// <param name="sproc">
        /// The name of the stored procedure 
        /// </param>
        /// <returns>
        /// true if it exists 
        /// </returns>
        public bool ProcedureExists(string sproc)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText = "select 1 from sys.procedures where name = @SprocName";
                sqlCommand.Parameters.AddWithValue("@SprocName", sproc);
                var result = sqlCommand.ExecuteScalar();
                return result != null && (int)result == 1;
            }
        }

        /// <summary>
        /// Determines if a table exists
        /// </summary>
        /// <param name="table">
        /// The name of the table 
        /// </param>
        /// <returns>
        /// true if it exists 
        /// </returns>
        public bool TableExists(string table)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText =
                    "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'System.Activities.DurableInstancing' AND  TABLE_NAME = @TableName";
                sqlCommand.Parameters.AddWithValue("@TableName", table);
                var result = sqlCommand.ExecuteScalar();
                return result != null && (int)result == 1;
            }
        }

        /// <summary>
        /// Determines if a view exists
        /// </summary>
        /// <param name="view">
        /// The name of the view 
        /// </param>
        /// <returns>
        /// true if it exists 
        /// </returns>
        public bool ViewExists(string view)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            using (var sqlConnection = new SqlConnection(this.ConnectionString))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();
                sqlCommand.CommandText =
                    "SELECT 1 FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = 'System.Activities.DurableInstancing' AND  TABLE_NAME = @ViewName";
                sqlCommand.Parameters.AddWithValue("@ViewName", view);
                var result = sqlCommand.ExecuteScalar();
                return result != null && (int)result == 1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose of the resource
        /// </summary>
        /// <param name="disposing">
        /// The disposing flag. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                Debug.Assert(this.syncLock != null, "this.syncLock != null");
                lock (this.syncLock)
                {
                    if (disposing)
                    {
                        try
                        {
                            if (this.Exists())
                            {
                                this.Drop();
                            }
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine("Exception during Dispose: {0}", exception);
                        }

                        this.disposed = true;
                    }
                }
            }
        }

        #endregion
    }
}