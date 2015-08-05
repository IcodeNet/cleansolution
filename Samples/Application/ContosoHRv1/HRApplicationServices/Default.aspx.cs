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
using System.Web.UI;
using HRApplicationServices.Application;
using HRApplicationServices.Contracts;
using SubmitJobApplicationRequest = HRApplicationServices.Contracts.SubmitJobApplicationRequest;

namespace HRApplicationServices
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                SetFieldDefaults();
        }

        protected void ButtonApplyClick(object sender, EventArgs e)
        {
            var request = new SubmitJobApplicationRequest
                              {
                                  Resume = new ApplicantResume
                                               {
                                                   Name = txtName.Text,
                                                   Email = txtEmail.Text,
                                                   Education = ddEducation.Text,
                                               },
                                  RequestID = Guid.NewGuid()
                              };

            var proxy = new ApplicationServiceClient();
            var response = proxy.SubmitJobApplication(request);
            Session["SubmitAppResponse"] = response;
            Response.Redirect("SubmitAppResponse.aspx");
        }

        private void SetFieldDefaults()
        {
            var r = new Random();

            txtName.Text = r.Next()%2 == 0 ? "John Smith" : "Jane Smith";
            txtEmail.Text = "JSmith@hotmail.com";
            ddEducation.Text = r.Next()%2 == 0 ? "Bachelors" : "Masters";
        }
    }
}