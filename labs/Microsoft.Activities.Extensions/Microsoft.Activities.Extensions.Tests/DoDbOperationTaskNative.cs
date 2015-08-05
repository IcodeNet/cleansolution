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
    public class DoDbOperationTaskNative : TaskNativeActivity
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
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        protected override void Execute(NativeActivityContext context, CancellationToken token)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.OpenAsync(token);
                var cmd = new SqlCommand("SELECT * FROM Customers WHERE CUST_ID = @CustomerId", connection);
                cmd.Parameters.AddWithValue("@CustomerId", this.CustomerId.Get(context));

                using (var reader = cmd.ExecuteReaderAsync(token).Result)
                {
                    while (reader.ReadAsync(token).Result)
                    {
                        // Inside of a loop, check the cancellation token
                        token.ThrowIfCancellationRequested();
                        DoSomethingWithCustomer(reader, token);
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