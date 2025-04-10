using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;


namespace EKECMS
{
    public partial class Content : System.Web.UI.Page
    {
        public string nr = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            ((EKECMS.Master.Site)Master).SiteInfo = siteinfo;

            nr = siteinfo.Data["NR"].ToString().replaceKeyWord(siteinfo.KeyWord).replaceAbsPath().handlePlayer();
        }
    }
}