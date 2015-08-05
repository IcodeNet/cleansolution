#region copyright

// ----------------------------------------------------------------------------------
// Microsoft
//   
// Copyright (c) Microsoft Corporation. All rights reserved.
//   
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

#endregion

using System;
using System.Web.UI;
using CacheAsideExample.ServiceReferences;

namespace CacheAsideExample
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            var proxy = new ServiceClient();
            try
            {
                var sampleResponse = proxy.GetData(TextBoxKey.Text);
                LabelValue.Text = sampleResponse.Data.Value;
                RadioButtonHit.Checked = sampleResponse.CacheHit;
                RadioButtonMiss.Checked = !sampleResponse.CacheHit;
                proxy.Close();
            }
            catch (Exception)
            {
                proxy.Abort();
                throw;
            }
        }
    }
}