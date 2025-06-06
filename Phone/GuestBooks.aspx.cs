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
    public partial class GuestBooks : System.Web.UI.Page
    {
        public string id = "";
        public string pid = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPhone(); //不是手机、ipod 访问则跳转到PC版

            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("default.aspx", true);
            id = Request.QueryString["id"].ToString();
            if (Request.QueryString["pid"] != null) pid = Request.QueryString["pid"].ToString();
            #endregion

            ((EKECMS.Master.Phone)Master).SiteInfo = siteinfo;
            List();
        }
        private void List()
        {
            if (pid.Length > 0) eDataView1.Where.Add("ClassID in (" + pid + ",(select ClassID from eWeb_Class where ParentClassID='" + pid + "' and delTag=0))");
            eDataView1.Where.Add("ColumnID='" + id + "'");
            eDataView1.Where.Add("WebCode='" + eBase.getWebCode() + "'");


        }
    }
}