// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using HRApplicationServices.Activities;
using HRApplicationServices.Contracts;
using System.Net.Mail;

namespace HRServices.Tests.Activities
{

    public sealed class MockRequestHumanScreening : CodeActivity
    {

        // Define an activity input argument of type string
        public InArgument<SubmitJobApplicationRequest> ApplicationRequest { get; set; }
        public InArgument<int> ApplicationID { get; set; }
        public InArgument<int> RetryCount { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            // Do nothing
        }
    }
}