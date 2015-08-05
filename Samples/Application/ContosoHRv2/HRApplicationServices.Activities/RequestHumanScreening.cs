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

using System;
using System.Activities;
using System.Net.Mail;
using System.Web.Configuration;
using HRApplicationServices.Contracts;

namespace HRApplicationServices.Activities
{
    public sealed class RequestHumanScreening : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<SubmitJobApplicationRequest> ApplicationRequest { get; set; }
        public InArgument<int> ApplicationID { get; set; }
        public InArgument<int> RetryCount { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var baseUri = WebConfigurationManager.AppSettings["BaseURI"];

            if (string.IsNullOrEmpty(baseUri))
                throw new InvalidOperationException("No baseURI appSetting found in web.config");

            var smtpClient = new SmtpClient();

            var applicant = ApplicationRequest.Get(context).Resume.Name;
            var mailTemplate = RetryCount.Get(context) > 0
                                      ? ServiceResources.NagMailTemplate
                                      : ServiceResources.ReviewMailTemplate;
            var htmlMailText = string.Format(mailTemplate, applicant,
                                                ApplicationID.Get(context), baseUri);
            var msg = new MailMessage("admin@contoso.com", "appfabricdemo@hotmail.com",
                                      string.Format(ServiceResources.HumanScreenSubject, applicant), htmlMailText) { IsBodyHtml = true };
            smtpClient.Send(msg);
        }
    }
}