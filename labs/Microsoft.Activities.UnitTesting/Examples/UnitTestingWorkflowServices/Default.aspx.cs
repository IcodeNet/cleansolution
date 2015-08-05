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
using System.Web.UI;
using WorkflowExtensionExample.ServiceReference1;

namespace WorkflowExtensionExample
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LinkButtonTest_Click(object sender, EventArgs e)
        {
            var proxy = new ServiceClient();

            try
            {
                int num;

                if (!Int32.TryParse(TextBoxNum.Text, out num))
                {
                    LabelResult.Text = "Text cannot be converted to a number";
                }
                else
                {
                    LabelResult.Text = proxy.GetData(num);
                    proxy.Close();
                }
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            int num;

            if (!Int32.TryParse(TextBoxNum.Text, out num))
            {
                LabelResult.Text = "Text cannot be converted to a number";
            }
            else
            {
                LabelResult.Text = WorkflowInvoker.Invoke(new GetAverage {Number = num});
            }
        }
    }
}