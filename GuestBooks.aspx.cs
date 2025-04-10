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
    public partial class GuestBooks : System.Web.UI.Page
    {
        public string id = "";
        public string pid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            #region 系统参数及安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            id = Request.QueryString["id"].ToString();
            if (Request.QueryString["pid"] != null) pid = Request.QueryString["pid"].ToString();
            #endregion
            List();
        }
        private void List()
        {
            eDataView1.Where.Add("ColumnID=" + id);
            eDataView1.Where.Add("WebCode='" + eBase.getWebCode() + "'");
            if (pid.Length > 0) eDataView1.Where.Add("ClassID in (" + pid + ",(select ClassID from eWeb_Class where ParentClassID=" + pid + " and delTag=0))");

        }
    }
}