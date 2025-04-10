using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using EKETEAM.UserControl;


namespace EKECMS
{
    public partial class Parents : System.Web.UI.Page
    {
        public string id = "";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            id = Request.QueryString["id"].ToString();
            #endregion
            ((EKECMS.Master.Site)Master).SiteInfo = siteinfo;
            StringBuilder sb = new StringBuilder();
            eDataView dataview;
            string sql = "select ColumnID,mc,Type,url from " + Pub.getTablePrefix() + "Columns where ParentColumnID=" + id + " and WebCode='" + eBase.getWebCode() + "'";
            sql += " and show=1 and delTag=0 order by px,ColumnID";
            DataTable tb = eBase.DataBase.getDataTable(sql);
            foreach (DataRow dr in tb.Rows)
            {
                sb.Append("<h1><span><a href=\"" + siteinfo.gethref(dr["ColumnID"].ToString(), "", dr["Type"].ToString(), dr["url"].ToString()) + "\"><img class=\"more\" src=\"images/none.gif\"></a></span>" + dr["mc"].ToString() + "</h1>\r\n");
                switch (dr["Type"].ToString())
                {

                    case "1":
                        #region 单页文章
                        sb.Append("<div id=\"content\">\r\n");
                        sb.Append(eBase.DataBase.getValue("select jj from " + Pub.getTablePrefix() + "Columns where ColumnID=" + dr["ColumnID"].ToString()));
                        sb.Append("</div>\r\n");
                        #endregion
                        break;
                    case "2":
                        #region 文章列表
                        dataview = new eDataView();
                        dataview.DataID = "16caab84-d249-402e-850d-b6306f62fa18";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");

                        sb.Append("<div id=\"articles\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                    case "3":
                        #region 产品栏目
                        dataview = new eDataView();
                        dataview.DataID = "b544e877-3767-4a0b-920b-e47d5f0e5421";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");
                        sb.Append("<div id=\"products\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                    case "4":
                        #region 图片栏目
                        dataview = new eDataView();
                        dataview.DataID = "44755eb9-9c68-4c3f-8e87-bdda5333337b";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");
                        sb.Append("<div id=\"pictures\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                    case "5":
                        #region 案例栏目
                        dataview = new eDataView();
                        dataview.DataID = "36b4e43d-2b46-41fe-b0ad-d672bee7d7a7";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");
                        sb.Append("<div id=\"cases\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                    case "6":
                        #region 视频栏目
                        dataview = new eDataView();
                        dataview.DataID = "cb189507-bc1c-4995-946f-68b764b5ffb6";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");
                        sb.Append("<div id=\"videos\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                    case "7":
                        #region 留言栏目
                        dataview = new eDataView();
                        dataview.DataID = "d9f13f71-8656-4885-a904-36708684d1d1";
                        dataview.Where.Add("ColumnID='" + dr["ColumnID"].ToString() + "'");
                        dataview.Where.Add("WebCode='" + eBase.getWebCode() + "'");
                        sb.Append("<div id=\"guestbooks\">" + dataview.getControlHTML() + "</div>");
                        #endregion
                        break;
                }
            }
            LitBody.Text = sb.ToString();
        }
    }
}