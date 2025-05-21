using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;

namespace EKECMS.Controls
{
    public partial class Head : System.Web.UI.UserControl
    {
        private eSiteInfo _SiteInfo;
        public eSiteInfo SiteInfo
        {
            get { return _SiteInfo; }
            set { _SiteInfo = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteInfo == null) return;
            LitTitle.Text = SiteInfo.Title;
            if (SiteInfo.KeyWords.Length > 0)
            {
                LitKeyWords.Text = "<meta name=\"Keywords\" content=\"" + SiteInfo.KeyWords + "\" />\r\n";
            }
            if (SiteInfo.Description.Length > 0)
            {
                LitDescription.Text = "<meta name=\"Description\" content=\"" + SiteInfo.Description + "\" />\r\n";
            }
        }
    }
}