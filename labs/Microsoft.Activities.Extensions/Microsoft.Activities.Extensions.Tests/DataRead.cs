// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRead.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Prototype;

    /// <summary>
    /// The data read.
    /// </summary>
    public class DataRead : TaskActivity<bool>
    {
        public InArgument<SqlDataReader> Reader { get; set; }

        public override Task<bool> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken token)
        {
            return this.Reader.Get(context).ReadAsync(token);
        }
    }
}