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
using System.Linq;
using System.Web.UI;
using HRApplicationServices.Application;
using HRApplicationServices.Contracts;
using HRApplicationServices.Data;

namespace HRApplicationServices
{
    public partial class HireApproval : Page
    {
        private int _appId = -1;
        private Applicant _applicant;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetApplicant();
            }
        }

        private void SendHumanScreenComplete(bool hire)
        {
            var proxy = new ApplicationServiceClient();
            try
            {
                // Create the result
                var result = new HumanScreeningResult
                                 {
                                     AppID = Convert.ToInt32(LabelAppID.Text),
                                     HiringApproved = hire,
                                 };

                // Wrap it in a request
                var request =
                    new HumanScreeningCompletedRequest(result);

                // Call the workflow service
                proxy.HumanScreeningCompleted(result);

                proxy.Close();
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }

        private void GetApplicant()
        {
            _appId = GetAppIdFromRequest();

            if (_appId == -1)
            {
                var s = GetRedirectString("HRMessage.aspx?MsgID=ErrInvalidAppID");
                Response.Redirect(s, true);
            }
            else
            {
                using (var ctx = new HRApplicationDataEntities())
                {
                    var query = from a in ctx.Applicants
                                where a.ApplicationID == _appId
                                select a;

                    _applicant = query.FirstOrDefault();

                    if (_applicant == null)
                        Response.Redirect(GetRedirectString("HRMessage.aspx?MsgID=ErrUnknownAppID"), true);
                    else
                    {
                        LabelAppID.Text = _appId.ToString();
                        LabelName.Text = _applicant.ApplicantName;
                        LabelEducation.Text = _applicant.Education;
                    }
                }
            }
        }

        protected void ButtonHireClick(object sender, EventArgs e)
        {
            SendHumanScreenComplete(true);

            // Redirect to message page
            Response.Redirect(
                GetRedirectString(
                    "HRMessage.aspx?MsgID=AppIDStatusUpdated&AppID={0}&Status={1}",
                    LabelAppID.Text, "Hire"),
                true);
        }

        protected void ButtonNoHireClick(object sender, EventArgs e)
        {
            SendHumanScreenComplete(false);

            // Redirect to message page
            Response.Redirect(
                GetRedirectString(
                    "HRMessage.aspx?MsgID=AppIDStatusUpdated&AppID={0}&Status={1}",
                    LabelAppID.Text, "Hire"),
                true);
        }

        private string GetRedirectString(string uriFormat, params object[] args)
        {
            return Server.UrlPathEncode(string.Format(uriFormat, args));
        }

        private int GetAppIdFromRequest()
        {
            var appIdArg = Request.QueryString["AppID"];

            if (string.IsNullOrEmpty(appIdArg))
                return -1;
            else
            {
                try
                {
                    return Convert.ToInt32(appIdArg);
                }
                catch (FormatException)
                {
                    return -1;
                }
            }
        }
    }
}