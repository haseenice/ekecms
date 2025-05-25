using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Plugins
{
    public partial class CompanySelect : System.Web.UI.Page
    {        
        public string obj = eParameters.QueryString("obj");
        public string area = eParameters.QueryString("area");
        string siteid="";
        string comcode = "";
        string usertype = "";
        bool change = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            siteid = eBase.getSiteID();// eConfig.SiteID();  
            if (area.Length > 0)
            {
                eUser user = new eUser(area);
                if (user.Logined)
                {
                    if (user["siteid"].ToString().Length > 0) siteid = user["siteid"].ToString();
                    if (user["comcode"].ToString().Length > 0) comcode = user["comcode"].ToString();
                    if (user["usertype"].ToString().Length > 0) usertype = user["usertype"].ToString();
                }
            }
            eTable etb = new eTable("Organizationals");
            change = etb.ColumnCollection.ContainsKey("change");
            litBody.Text = getTree("");
        }
        private string getTree(string ParentID)
        {
            StringBuilder sb = new StringBuilder();
            string sql = "select OrganizationalID,ParentID,MC,Code,IsCorp from Organizationals where DelTag=0 and IsCorp=1";
           // sql += (ParentID.Length == 0 ? " and ParentID IS NULL" : " and ParentID='" + ParentID + "'");
            if (ParentID.Length == 0)
            {
                if (comcode.Length > 0 && usertype!="1")
                {
                    sql += " and code ='" + comcode + "'";// and IsCorp=1
                }
                else
                {
                    sql += " and ParentID IS NULL";// and IsCorp=1
                }
            }
            else
            {
                sql += " and ParentID='" + ParentID + "'";
            }
            sql += " and " + (siteid.Length > 0 ? "(SiteID=0 or SiteID=" + siteid + ")" : "SiteID=0");
            sql += " and show=1";
            if (change) sql += " and change=0";
            sql += " Order by IsCorp,PX,addTime";
            //eBase.Writeln(sql);
            DataTable tb = eBase.DataBase.getDataTable(sql);
            if (ParentID.Length == 0 && tb.Rows.Count == 0)
            {
                sb.Append("<div style=\"margin:20px;text-align:center;color:#666;\">暂无相关数据!<div>");
            }
            else
            {

                if (ParentID.Length == 0)
                {
                    sb.Append("<ul id=\"etree\" class=\"etree\" PID=\"NULL\">\r\n");
                }
                else
                {
                    sb.Append("<ul PID=\"" + ParentID + "\">\r\n");
                }
                foreach (DataRow dr in tb.Rows)
                {
                    string ct = eBase.DataBase.getValue("select count(*) from  Organizationals where SiteID=" + siteid + " and DelTag=0 and ParentID='" + dr["OrganizationalID"].ToString() + "'");

                    //if (ct == "0") continue;
                    sb.Append("<li dataid=\"" + dr["OrganizationalID"].ToString() + "\" ");// oncontextmenu=\"return false;\"
                    sb.Append(" class=\"\">");
                    sb.Append("<div>");// oncontextmenu=\"return false;\"
                    sb.Append("<a href=\"javascript:;\" onclick=\"setDepartment('" + dr["OrganizationalID"].ToString() + "','" + dr["MC"].ToString() + "');\" oncontextmenu=\"return false;\">");
                    sb.Append(dr["MC"].ToString());
                    sb.Append("</a>"); // (" + ct + ")
                    sb.Append("</div>");

                    sb.Append(getTree(dr["OrganizationalID"].ToString()));

                    sb.Append("</li>\r\n");
                }
                sb.Append("</ul>\r\n");
            }
            return sb.ToString();
        }
    }
}