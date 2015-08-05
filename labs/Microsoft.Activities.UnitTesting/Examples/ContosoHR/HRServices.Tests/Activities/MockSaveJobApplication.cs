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
using HRApplicationServices.Activities;
using HRApplicationServices.Contracts;

namespace HRServices.Tests.Activities
{
    public class MockSaveJobApplication : CodeActivity<SubmitJobApplicationResponse>
    {
        private const int AppId = 123;
        public InArgument<SubmitJobApplicationRequest> AppRequest { get; set; }

        protected override SubmitJobApplicationResponse Execute(CodeActivityContext context)
        {
            SubmitJobApplicationRequest request = AppRequest.Get(context);

            var response = new SubmitJobApplicationResponse
                               {
                                   ApplicationID = AppId,
                                   ApplicantName = request.Resume.Name,
                                   ResponseText =
                                       string.Format(ServiceResources.JobApplicationProcessing, request.Resume.Name,
                                                     AppId)
                               };


            return response;
        }
    }
}