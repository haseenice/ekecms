using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.UserControl;

namespace EKECMS.Phone
{
    public partial class Jobs : System.Web.UI.Page
    {
        public string id = "";
        public string pid = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {

            Pub.clearPageCache();

            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("default.aspx", true);
            id = Request.QueryString["id"].ToString();
            if (Request.QueryString["pid"] != null) pid = Request.QueryString["pid"].ToString();
            #endregion


            List();
        }
        private void List()
        {


            string sql = "Select * from eWeb_Jobs";
            sql += " where ColumnID='" + id + "' and delTag=0 and show=1";
            sql += " order by JobID desc";
            //DataTable tb = eOleDB.getDataTable(sql);
            //Rep.DataSource = tb;
            //Rep.DataBind();
            eList list = new eList("eWeb_Articles");
            list.SQL = sql;
            list.Bind(Rep, ePageControl1);
            if (ePageControl1.PageCount < 2) ePageControl1.Visible = false;

        }
    }
}