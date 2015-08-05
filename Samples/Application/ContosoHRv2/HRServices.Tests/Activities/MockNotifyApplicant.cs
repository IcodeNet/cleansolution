//  ----------------------------------------------------------------------------------
//  Microsoft Developer & Platform Evangelism
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
using System.Activities;
using System.Collections.Generic;
using System.Net.Mail;
using HRApplicationServices.Activities;
using HRApplicationServices.Contracts;

namespace HRServices.Tests.Activities
{
    public sealed class MockNotifyApplicant : CodeActivity
    {

        public InArgument<bool> Hire { get; set; }
        public InArgument<bool> Cancel { get; set; }
        public InArgument<ApplicantResume> Resume { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Do nothing
        }
    }
}