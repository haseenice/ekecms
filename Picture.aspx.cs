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
    public partial class Picture : System.Web.UI.Page
    {
        public string nr = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            ((EKECMS.Master.Site)Master).SiteInfo = siteinfo;
            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            string id = Request.QueryString["id"].ToString();
            #endregion

            siteinfo.BT = siteinfo.Data["BT"].ToString() + "-";

            nr = siteinfo.Data["XXNR"].ToString().replaceKeyWord(siteinfo.KeyWord).replaceAbsPath().handlePlayer();


            eBase.DataBase.Execute("update " + Pub.getTablePrefix() + "Pictures set ckcs=ckcs+1 where PictureID=" + id);
        }
        
    }
}