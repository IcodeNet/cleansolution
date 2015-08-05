// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWorkflowInstanceStoreManager.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.DurableInstancing
{
    using System;
    using System.Activities.DurableInstancing;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   Provides operations for Managing a SqlWorkflowInstanceStore Database
    /// </summary>
    public static class SqlWorkflowInstanceStoreManager
    {
        #region Constants

        /// <summary>
        ///   Schema name for SqlWorkflowInstanceStore
        /// </summary>
        public const string SchemaName = "System.Activities.DurableInstancing";

        /// <summary>
        ///   Create database SQL
        /// </summary>
        private const string CreateDatabaseSql = "CREATE DATABASE [{0}]";

        /// <summary>
        ///   Database exists SQL
        /// </summary>
        private const string DatabaseExistsSql = "SELECT 1 FROM sys.databases WHERE Name=" + DatabaseNameParameter;

        /// <summary>
        ///   Database name parameter
        /// </summary>
        private const string DatabaseNameParameter = "@DatabaseName";

        /// <summary>
        ///   Whitelist for T-SQL Identifiers
        /// </summary>
        private const string DbIdentifierWhiteList =
            @"(?x)^[a-zA-Z._@#] # First char must be a letter, underscore, at sign or number sign
[a-zA-Z0-9._@#]     #remaining chars can include numbers
{1,122}$            # up to 123 chars total";

        /// <summary>
        ///   Drop if exists SQL
        /// </summary>
        private const string DropIfExistsSql = @"
IF EXISTS (SELECT 1 FROM sys.databases WHERE Name = @DatabaseName) 
BEGIN 
    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
    USE [MASTER]; 
    DROP DATABASE [{0}]
END";

        /// <summary>
        ///   The Initial Catalog keyword
        /// </summary>
        private const string InitialCatalog = "Initial Catalog";

        /// <summary>
        ///   Format string for script files
        /// </summary>
        private const string SqlWorkflowInstanceStoreSql = @"SQL\en\SqlWorkflowInstanceStore{0}.sql";

        #endregion

        #region Static Fields

        /// <summary>
        ///   Reqex to split the script
        /// </summary>
        private static readonly Regex SqlReqex = new Regex(
            @"^\s*GO\s*\r?$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        /// <summary>
        ///   Cached logic script
        /// </summary>
        private static string[] logicScript;

        /// <summary>
        ///   Cached schema script
        /// </summary>
        private static string[] schemaScript;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Runs the schema and logic scripts on an existing database
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database 
        /// </param>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        public static void ConfigureInstanceStore(string databaseName, string connectionString)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            CheckNameWhitelist(databaseName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            InvokeScript(SqlWorkflowInstanceStoreScript.Schema, databaseName, connectionString);
            InvokeScript(SqlWorkflowInstanceStoreScript.Logic, databaseName, connectionString);
        }

        /// <summary>
        /// Creates an instance store database
        /// </summary>
        /// <param name="databaseName">
        /// The database name. 
        /// </param>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        /// <param name="dropIfExists">
        /// Drop the database if it exists 
        /// </param>
        /// <returns>
        /// A SqlWorkflowInstanceStore.
        /// </returns>
        public static SqlWorkflowInstanceStore CreateInstanceStore(string databaseName, string connectionString, bool dropIfExists = false)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            CheckNameWhitelist(databaseName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            CreateDatabase(databaseName, connectionString, dropIfExists);
            ConfigureInstanceStore(databaseName, connectionString);
            return new SqlWorkflowInstanceStore(connectionString);
        }

        /// <summary>
        /// Drops a database
        /// </summary>
        /// <param name="databaseName">
        /// The database name. 
        /// </param>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", 
            Justification = "databaseName checked against Whitelist")]
        public static void DropInstanceStore(string databaseName, string connectionString)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            CheckNameWhitelist(databaseName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var serverConnection = ConfigureConnection(connectionString);
            using (var sqlConnection = new SqlConnection(serverConnection))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();

                sqlCommand.CommandText = string.Format(DropIfExistsSql, databaseName);
                sqlCommand.Parameters.AddWithValue(DatabaseNameParameter, databaseName);
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Determines if the database exists or not
        /// </summary>
        /// <param name="databaseName">
        /// The database name. 
        /// </param>
        /// <param name="connectionString">
        /// The connection string. 
        /// </param>
        /// <returns>
        /// true if the database exists 
        /// </returns>
        public static bool InstanceStoreExists(string databaseName, string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.Remove(InitialCatalog);
            using (var conn = new SqlConnection(builder.ConnectionString))
            using (var command = new SqlCommand(DatabaseExistsSql, conn))
            {
                conn.Open();
                command.Parameters.AddWithValue(DatabaseNameParameter, databaseName);
                var result = command.ExecuteScalar();
                return result != null && (int)result == 1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks a name against a whitelist of allowed characters for identifiers in SQL Server
        /// </summary>
        /// <param name="name">
        /// The name. 
        /// </param>
        /// <exception cref="ArgumentException">
        /// The name is not a valid match for the whitelist
        /// </exception>
        internal static void CheckNameWhitelist(string name)
        {
            if (!Regex.IsMatch(name, DbIdentifierWhiteList))
            {
                throw new ArgumentException(string.Format("Invalid Database Identifier {0}", name));
            }
        }

        /// <summary>
        /// Removes the Initial Catalog setting from the connection string
        /// </summary>
        /// <param name="connectionString">
        /// The connection string 
        /// </param>
        /// <param name="initialCatalog">
        /// The catalog to connect to 
        /// </param>
        /// <returns>
        /// A connection string without the InitialCatalog 
        /// </returns>
        private static string ConfigureConnection(string connectionString, string initialCatalog = null)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.Remove(InitialCatalog);
            if (!string.IsNullOrWhiteSpace(initialCatalog))
            {
                builder.InitialCatalog = initialCatalog;
            }

            builder.Remove("Pooling");
            builder.Pooling = false;
            return builder.ConnectionString;
        }

        /// <summary>
        /// Creates a database
        /// </summary>
        /// <param name="databaseName">
        /// The database name. 
        /// </param>
        /// <param name="connectionString">
        /// The connection string 
        /// </param>
        /// <param name="dropIfExists">
        /// If true and the database exists, it will be dropped 
        /// </param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", 
            Justification = "databaseName is checked against WhiteList prior to this method")]
        private static void CreateDatabase(string databaseName, string connectionString, bool dropIfExists)
        {
            var serverConnection = ConfigureConnection(connectionString);
            using (var sqlConnection = new SqlConnection(serverConnection))
            using (var sqlCommand = new SqlCommand { Connection = sqlConnection })
            {
                sqlConnection.Open();

                if (dropIfExists)
                {
                    sqlCommand.CommandText = string.Format(DropIfExistsSql, databaseName);
                    sqlCommand.Parameters.AddWithValue(DatabaseNameParameter, databaseName);
                    sqlCommand.ExecuteNonQuery();
                }

                sqlCommand.CommandText = string.Format(CreateDatabaseSql, databaseName);
                sqlCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Caches and returns a script loaded from file
        /// </summary>
        /// <param name="script">
        /// The script. 
        /// </param>
        /// <returns>
        /// The get script. 
        /// </returns>
        private static IEnumerable<string> GetScript(SqlWorkflowInstanceStoreScript script)
        {
            switch (script)
            {
                case SqlWorkflowInstanceStoreScript.Logic:
                    return logicScript
                           ??
                           (logicScript = ReadScript(SqlWorkflowInstanceStoreScript.Logic));

                case SqlWorkflowInstanceStoreScript.Schema:
                    return schemaScript
                           ??
                           (schemaScript = ReadScript(SqlWorkflowInstanceStoreScript.Schema));
            }

            return null;
        }

        /// <summary>
        /// Gets the path to the SQL Script
        /// </summary>
        /// <param name="script">
        /// The script type. 
        /// </param>
        /// <returns>
        /// The script path. 
        /// </returns>
        private static string GetScriptPath(SqlWorkflowInstanceStoreScript script)
        {
            return Path.Combine(
                RuntimeEnvironment.GetRuntimeDirectory(), string.Format(SqlWorkflowInstanceStoreSql, script));
        }

        /// <summary>
        /// Invokes a SqlWorkflowInstanceStore Script
        /// </summary>
        /// <param name="script">
        /// The script. 
        /// </param>
        /// <param name="databaseName">
        /// The database name 
        /// </param>
        /// <param name="connectionString">
        /// The connection string 
        /// </param>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", 
            Justification = "Internal method runs only scripts for creating instance store")]
        private static void InvokeScript(
            SqlWorkflowInstanceStoreScript script, string databaseName, string connectionString)
        {
            using (var sqlConnection = new SqlConnection(ConfigureConnection(connectionString, databaseName)))
            using (var command = sqlConnection.CreateCommand())
            {
                sqlConnection.Open();
                foreach (var sqlStatement in GetScript(script).Where(sqlStatement => sqlStatement.Length > 0))
                {
                    command.CommandText = sqlStatement;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Reads the script from disk
        /// </summary>
        /// <param name="script">
        /// The script. 
        /// </param>
        /// <returns>
        /// The script split into an array of strings. 
        /// </returns>
        private static string[] ReadScript(SqlWorkflowInstanceStoreScript script)
        {
            return SqlReqex.Split(File.ReadAllText(GetScriptPath(script)));
        }

        #endregion
    }
}