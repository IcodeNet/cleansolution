// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoDbOperationTask.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System;
    using System.Activities;
    using System.Data.SqlClient;
    using System.Threading;

    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    ///   The do db operation.
    /// </summary>
    public class DoDbOperationTask : TaskActivity
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        ///   Gets or sets the customer id.
        /// </summary>
        public InArgument<int> CustomerId { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   The execute.
        /// </summary>
        protected override void Execute()
        {
            WorkflowTrace.Verbose("DoDbOperationTask Executing");

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                WorkflowTrace.Verbose("DoDbOperationTask Opening Database");

                // Pass the this.CancellationToken to all async operations
                connection.OpenAsync(this.CancellationToken).Wait();
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE CUST_ID = @CustomerId", connection);

                // Access arguments setup in the BeforeExecute method
                cmd.Parameters.AddWithValue("@CustomerId", this.Inputs.CustomerId);

                WorkflowTrace.Verbose("DoDbOperationTask Executing Reader");

                // Pass the this.CancellationToken to all async operations
                using (var reader = cmd.ExecuteReaderAsync(this.CancellationToken).Result)
                {
                    // Pass the this.CancellationToken to all async operations
                    while (reader.ReadAsync(this.CancellationToken).Result)
                    {
                        // Inside of a loop, check the cancellation this.CancellationToken
                        this.CancellationToken.ThrowIfCancellationRequested();

                        // Pass the this.CancellationToken to other classes and methods
                        DoSomethingWithCustomer(reader, this.CancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// The do something with customer.
        /// </summary>
        /// <param name="reader">
        /// The reader. 
        /// </param>
        /// <param name="cancellationToken">
        /// The token 
        /// </param>
        /// <exception cref="NotImplementedException">
        /// This is just a stub
        /// </exception>
        private static void DoSomethingWithCustomer(SqlDataReader reader, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}