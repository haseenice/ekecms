using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace EKECMS.Phone
{
    public partial class Content : System.Web.UI.Page
    {
        public string nr = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPhone(); //不是手机、ipod 访问则跳转到PC版

            ((EKECMS.Master.Phone)Master).SiteInfo = siteinfo;
            nr = siteinfo.Data["NR"].ToString().replaceKeyWord(siteinfo.KeyWord).replaceAbsPath().handlePlayer();
        }
    }
}