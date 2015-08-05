// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileReadToEnd.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Activities.Extensions.Tests
{
    using System.Activities;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Activities.Extensions.Prototype;

    public class FileReadToEnd : TaskActivity<string>
    {
        public InArgument<string> FileName { get; set; }

        public override Task<string> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken token)
        {
            var stream = File.OpenText(this.FileName.Get(context));
            return stream.ReadToEndAsync().ContinueWith(
                task =>
                {
                    stream.Close();
                    return task.Result;
                });
        }
    }
}