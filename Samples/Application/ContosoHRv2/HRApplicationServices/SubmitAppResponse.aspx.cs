using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HRApplicationServices.Contracts;

namespace HRApplicationServices
{
    public partial class SubmitAppResponse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var response = Session["SubmitAppResponse"] as SubmitJobApplicationResponse;

            if (response == null)
            {
                Response.Redirect(GetRedirectString("HRMessage.aspx?MsgID=ErrNoAppResponse"),true);
            }

            if (response != null) this.lableResponse.Text = response.ResponseText;
        }

        private string GetRedirectString(string uriFormat, params object[] args)
        {
            return Server.UrlPathEncode(string.Format(uriFormat, args));
        }

    }
}