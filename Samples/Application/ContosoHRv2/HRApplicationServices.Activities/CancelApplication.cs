using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace HRApplicationServices.Activities
{
    public class CancelApplication : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            context.MarkCanceled();
        }
    }
}
