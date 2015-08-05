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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using HRApplicationServices.Contracts;
using System.Net.Mail;
using System.Web.Configuration;
namespace HRApplicationServices.Activities
{

    public sealed class NotifyApplicant : CodeActivity
    {
        public InArgument<bool> Hire { get; set; }
        public InArgument<bool> Cancel { get; set; }
        public InArgument<ApplicantResume> Resume { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var hire = Hire.Get(context);
            var resume = Resume.Get(context);
            var baseURI = WebConfigurationManager.AppSettings["BaseURI"];

            if (string.IsNullOrEmpty(baseURI))
                throw new InvalidOperationException("No baseURI appSetting found in web.config");


            string mailText;
            string heading;

            if (Cancel.Get(context))
            {
                mailText = ServiceResources.CancelText;
                heading = ServiceResources.CancelHeading;
            }
            else
            {
                mailText = hire ? ServiceResources.OfferText : ServiceResources.NoHireText;
                heading = hire ? ServiceResources.HireHeading : ServiceResources.NoHireHeading;                
            }

            string htmlMailText = string.Format(ServiceResources.GenericMailTemplate, heading, string.Format(mailText, resume.Name), baseURI);
            var smtpClient = new SmtpClient();

            var msg = new MailMessage("admin@contoso.com", "appfabricdemo@hotmail.com",
                string.Format(ServiceResources.ApplicationMailSubject), htmlMailText);
            msg.IsBodyHtml = true;

            smtpClient.Send(msg);
        }
    }
}