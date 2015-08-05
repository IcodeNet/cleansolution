#region copyright

//  ----------------------------------------------------------------------------------
//  Microsoft
//  
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//  ----------------------------------------------------------------------------------
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
//  ----------------------------------------------------------------------------------

#endregion

using System.Activities;
using System.Diagnostics;

namespace Microsoft.Activities.UnitTesting.Tests.MockActivities
{
    public sealed class TestActivityTakesArgs : CodeActivity<int>
    {
        public InArgument<string> Arg1 { get; set; }
        public InArgument<int> Arg2 { get; set; }

        public string Arg1Processed { get; set; }
        public int Arg2Processed { get; set; }

        protected override int Execute(CodeActivityContext context)
        {
            Trace.WriteLine(GetType());

            Arg1Processed = Arg1.Get(context);
            Arg2Processed = Arg2.Get(context);

            return 1;
        }
    }
}
