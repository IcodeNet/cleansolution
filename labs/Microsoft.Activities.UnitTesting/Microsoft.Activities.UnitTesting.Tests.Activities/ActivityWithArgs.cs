using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Microsoft.Activities.UnitTesting.Tests.Activities
{

    public sealed class ActivityWithArgs : CodeActivity
    {
        public InArgument<bool> BoolArg { get; set; }
        public InArgument<string> StringArg { get; set; }
        public InArgument<int> IntArg { get; set; }
        public InArgument<TimeSpan> TimeSpanArg { get; set; }
        public InArgument<DateTime> DateTimeArg { get; set; }
        public InArgument<DataResponse> DataResponseArg { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

        }
    }
}
