using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace EKECMS.Controls
{
    public partial class Top : System.Web.UI.UserControl
    {
        private eSiteInfo _SiteInfo;
        public eSiteInfo SiteInfo
        {
            get { return _SiteInfo; }
            set { _SiteInfo = value; }
        }
        public DataRow row_content;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteInfo == null) return;
            LitMenu.Text = SiteInfo.Menu;
            LitMoveAds.Text = SiteInfo.MoveAds;
            LitTopAds.Text = SiteInfo.TopAds;
            LitMiddleAds.Text = SiteInfo.MiddleAds;
            LitServiceOnline.Text = SiteInfo.ServiceOnline;

            DataRow[] rows = SiteInfo.Columns.Select("Type=1 and mc like '%系我%' and show=1");
            if (rows.Length > 0) row_content = rows[0];
        }
    }
}