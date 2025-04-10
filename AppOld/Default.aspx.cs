using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace eFrameWork.AppOld
{
    public partial class Default : System.Web.UI.Page
    {
        public eUser user;
        public ApplicationMenu appmenu;
        public string appTitle = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser("Application");
            user.Check(); 
           
            appTitle = eConfig.ApplicationTitle(user["SiteID"].ToString());
            appmenu = new ApplicationMenu(user,"1");

            //eBase.PrintDataTable(appmenu.UserApps);
            //eBase.PrintDataTable(appmenu.UserApps.Select("ApplicationID,ApplicationItemID,ModelID,MC,ModelName", "MC='内容管理'", ""));
            if (appmenu.Applications.Rows.Count == 0)
            {
                //Response.Write("没有权限!&nbsp;&nbsp;<a href=\"Login.aspx\">重新登录</a>");
                litMsg.Text = eBase.getTipMsg(new eTipMsg() { Title = "没有权限!", Body = "请与管理员联系，开通相应权限。", Icon = "warning", Text = "关闭", Href = "javascript:window.close();" });
                return;
            }
            if (appmenu.Applications.Rows.Count == 1) //直接跳转应用
            {
                Response.Redirect(appmenu.Applications.Rows[0]["href"].ToString(), true);
            }
            else //用户选择应用
            {
                //Rep.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
                Rep.DataSource = appmenu.Applications;
                Rep.DataBind();
            }
        }
        
        protected void Rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Control ctrl = e.Item.Controls[0];
                Literal lit = (Literal)ctrl.FindControl("LitTags");
                if (lit != null)
                {
                    //lit.Text = DataBinder.Eval(e.Item.DataItem, "ModelTabID").ToString();
                }
            }
        }
    }
}