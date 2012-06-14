using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VoteR
{
    public partial class Default : System.Web.UI.Page
    {
        public bool IsAdmin
        {
            get
            {
                bool isAdmin;

                if (bool.TryParse(Request.QueryString["isAdmin"], out isAdmin))
                {
                    return isAdmin;
                }

                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}