using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.UserControl;
using EKETEAM.FrameWork;


    public partial class Customs_Default_Home : System.Web.UI.Page
    {
        public string UserArea = "Application";
        public eModel model;
        public eUser user;       
        public string AppId = "";
        public string AppItem = eParameters.Request("AppItem");
        public string ModelID = "";
        string sql = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser(eBase.getUserArea(UserArea));
            user.Check();
            /*
            if (AppItem.Length > 0)
            {
                DataRow[] appRows = eBase.a_eke_sysApplicationItems.Select("ApplicationItemID='" + AppItem + "'");
                if (appRows.Length == 0) Response.End();

                AppId = appRows[0]["ApplicationID"].ToString();
                ModelID = appRows[0]["ModelID"].ToString();
                model = new eModel(AppItem, ModelID, user);
            }
            else
            {
                model = new eModel(ModelID, user);
            }
            */
            model = new eModel();

            StringBuilder sb = new StringBuilder();

            foreach (string key in user.Keys)
            {
                //sb.Append(key + " = " + user[key].ToString() + "<br>");
            }
            //eBase.Print(user);

            sb.Append("欢迎登录" + eConfig.ApplicationTitle(user["SiteID"].ToString()) + "!<br>");
            sb.Append("姓名：" + user["Name"] + "<br>");
            sb.Append("用户ID：" + user.ID + "<br>");
            sb.Append("用户UserType：" + user["UserType"] + "<br>");
            sb.Append("用户SiteID：" + user["SiteID"] + "<br>");
            sb.Append("用户RoleID：" + user["RoleID"] + "<br>");
            sb.Append("用户RoleIDS：" + user["RoleIDS"] + "<br>");
            sb.Append("用户PostLevel：" + user["PostLevel"] + "<br>");
            sb.Append("用户Roledatatype：" + user["roledatatype"] + "<br>");
            
            sb.Append("用户OrgCode：" + user["OrgCode"] + "<br>");
            sb.Append("用户ComCode：" + user["ComCode"] + "<br>");
            sb.Append("用户UserCode：" + user["UserCode"] + "<br>");
            sb.Append("用户WebCode：" + user["WebCode"] + "<br>");


            DataTable tb = eBase.UserInfoDB.getDataTable("select LoginCount,LastLoginTime from a_eke_sysUsers where UserID='" + user["ID"].ToString() + "'");
            if (tb.Rows.Count > 0)
            {
                string logincount = tb.Rows[0]["LoginCount"].ToString();
                string lastlt = string.Format("{0:yyyy-MM-dd HH:mm:ss}", tb.Rows[0]["LastLoginTime"]);

                sb.Append("登录次数：" + logincount + "<br>");
                sb.Append("上次登录时间：" + lastlt + "<br>");
            }
            //sb.Append("默认密码非常不安全,点此<a href=\"ModifyPass.aspx?AppItem=" + AppItem + "\">修改密码</a>");
            sb.Append("默认密码非常不安全,请及时修改密码</a>");
            litBody.Text = sb.ToString();
           
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Master == null) return;
            //Literal lit = (Literal)Master.FindControl("LitTitle");
           // if (lit != null) lit.Text = eConfig.ApplicationTitle(user["SiteID"].ToString());// model.ModelInfo["mc"].ToString() + " - " + 



        }
    }
