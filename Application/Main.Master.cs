
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;


namespace eFrameWork.Application
{
    public partial class Main : System.Web.UI.MasterPage
    {
        public string AppItem = eParameters.Request("AppItem");
        public eUser user;
        public string sysNameorLogo = "";
        public ApplicationMenu appmenu;
        public string appNames = "";
        private string _top_fixed;
        public bool TopFixed
        {
            get
            {
                if (_top_fixed == null)
                {
                    string sname = "Application_TopFixed";
                    if (Session[sname] == null)
                    {

                        string tmp = eBase.UserInfoDB.getValue("select parValue from a_eke_sysUserCustoms where ParName='application_top_fixed' and UserID='" + user.ID + "'");
                        if (tmp.Length == 0) tmp = "true";
                        Session[sname] = tmp;
                        _top_fixed = tmp;
                    }
                    else
                    {
                        _top_fixed = Session[sname].ToString();
                    }
                }
                return eBase.parseBool(_top_fixed);
            }
        }
        private string _left_fixed;
        public bool LeftFixed
        {
            get
            {
                if (_left_fixed == null)
                {
                    string sname = "Application_LeftFixed";
                    if (Session[sname] == null)
                    {
                        string tmp = eBase.UserInfoDB.getValue("select parValue from a_eke_sysUserCustoms where ParName='application_left_fixed' and UserID='" + user.ID + "'");
                        if (tmp.Length == 0) tmp = "true";
                        Session[sname] = tmp;
                        _left_fixed = tmp;
                    }
                    else
                    {
                        _left_fixed = Session[sname].ToString();
                    }
                }
                return eBase.parseBool(_left_fixed);
            }
        }
        public DataRow row_pass;
        public DataRow row_info;
        public DataRow row_acc;
        protected void Page_Init(object sender, EventArgs e)
        {
            user = new eUser("Application");
            user.Check();//检测用户是否登录,未登录则跳转到登录页
        }
        protected void Page_Load(object sender, EventArgs e)
        {           
            sysNameorLogo = eConfig.ApplicationLogo(user["SiteID"].ToString());            
            appmenu = new ApplicationMenu(user,"1");

            #region 多应用
            if (appmenu.Applications.Rows.Count == 0)
            {
                Response.Write("没有权限!");
                Response.End();
            }
            else
            {
                if (appmenu.Applications.Rows.Count > 1)//两个及以上数量应用显示
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow dr in appmenu.Applications.Rows)
                    {
                        sb.Append("<a title=\"" + dr["MC"].ToString() + "\"" + (appmenu.ApplicationID == dr["ApplicationID"].ToString() ? " class=\"cur\"" : "") + " href=\"" + (appmenu.ApplicationID == dr["ApplicationID"].ToString() ? "javascript:;" : dr["href"].ToString() + (eRequest.QueryString("debug") == null ? "" : "&debug=1")) + "\">");
                        string iconhtml = dr["IconHTML"].ToString();
                        if (iconhtml.Length > 10)
                        {
                            if (dr.Contains("IconSize") && Convert.ToInt32(dr["IconSize"]) > 0)
                            {
                                iconhtml = iconhtml.Replace("<i", "<i style=\"font-size:" + dr["IconSize"].ToString() + "px;\"");
                            }
                            sb.Append(iconhtml);
                        }
                        else
                        {
                            string iconpath = dr["Icon"].ToString().Length > 10 ? dr["Icon"].ToString() : "../images/none.gif";
                            string iconactivepath = dr["IconActive"].ToString().Length > 0 ? dr["IconActive"].ToString() : iconpath;
                            sb.Append("<img");
                            if (dr.Contains("IconWidth") && dr.Contains("IconHeight") && (Convert.ToInt32(dr["IconWidth"]) > 0 || Convert.ToInt32(dr["IconHeight"]) > 0))
                            {
                                sb.Append(" style=\"" + (Convert.ToInt32(dr["IconWidth"]) > 0 ? "width:" + dr["IconWidth"].ToString() + "px;" : "") + (Convert.ToInt32(dr["IconHeight"]) > 0 ? "height:" + dr["IconHeight"].ToString() + "px;" : "") + "\"");
                            }
                            sb.Append(" class=\"def\" src=\"" + iconpath + "\">");// onerror=\"this.src='../images/none.gif';\"
                            sb.Append("<img");
                            if (dr.Contains("IconWidth") && dr.Contains("IconHeight") && (Convert.ToInt32(dr["IconWidth"]) > 0 || Convert.ToInt32(dr["IconHeight"]) > 0))
                            {
                                sb.Append(" style=\"" + (Convert.ToInt32(dr["IconWidth"]) > 0 ? "width:" + dr["IconWidth"].ToString() + "px;" : "") + (Convert.ToInt32(dr["IconHeight"]) > 0 ? "height:" + dr["IconHeight"].ToString() + "px;" : "") + "\"");
                            }
                            sb.Append(" class=\"cur\" src=\"" + iconactivepath + "\">");
                        }
                        sb.Append("<p>" + dr["mc"].ToString() + "</p>");
                        sb.Append("</a>");
                    }
                    appNames = sb.ToString();
                }
            }
            #endregion

            DataRow[] rows = appmenu.UserApps.Select("ModelID='13d1ceba-eef8-4471-8abe-c617fc8838ea'");//修改密码  and ApplicationID='" + appmenu.ApplicationID + "'
            if (rows.Length > 0) row_pass = rows[0];

            rows = appmenu.UserApps.Select("ModelID='c76750c3-1bc5-4bdb-9805-640a62faab64'"); //个人信息  and ApplicationID='" + appmenu.ApplicationID + "'
            if (rows.Length > 0) row_info = rows[0];

            rows = appmenu.UserApps.Select("ModelID='f1578584-713e-48e2-a392-2ed0d5cb83bc'"); //切换帐号  and ApplicationID='" + appmenu.ApplicationID + "'
            if (rows.Length > 0) row_acc = rows[0];
        }
    }
}