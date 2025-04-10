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
    public partial class Cases : System.Web.UI.Page
    {
        public string id = "";
        public string pid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            id = Request.QueryString["id"].ToString();
            if (Request.QueryString["pid"] != null) pid = Request.QueryString["pid"].ToString();
            #endregion
            List1();
        }
        private void List1()
        {
            string id = eParameters.QueryString("id");
            string pid = eParameters.QueryString("pid");
            eList list = new eList("eWeb_Cases");
            list.Fields.Add("CaseID,bt,jj,xtp");
            list.Where.Add("ColumnID=" + id);
            if (pid.Length > 0) list.Where.Add("ClassID in (" + pid + ",(select ClassID from eWeb_Class where ParentClassID=" + pid + " and delTag=0))");
            list.Where.Add("delTag=0 and show=1");
            list.OrderBy.Add("ZD desc,px,CaseID desc");
            list.Bind(RepList, ePageControl1);
        }
        private void List()
        {
            //eDataView1.Where.Add("ColumnID='" + id + "'");
            //eDataView1.Where.Add("SiteID='" + Pub.getSiteID() + "'");
            //if (pid.Length > 0) eDataView1.Where.Add("ClassID in (" + pid + ",(select ClassID from eWeb_Class where ParentClassID='" + pid + "' and delTag=0))");
        }
    }
}