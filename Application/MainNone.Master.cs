using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Application
{
    public partial class MainNone : System.Web.UI.MasterPage
    {
        public string AppItem = eParameters.Request("AppItem");
        public string appName = "";
        protected void Page_Init(object sender, EventArgs e)
        {
            eUser user = new eUser("Application");
            user.Check();//检测用户是否登录,未登录则跳转到登录页
        }
        protected void Page_Load(object sender, EventArgs e)
        { 
            //appName = eBase.DataBase.getValue("select mc from a_eke_sysApplications where ApplicationID='" + AppId + "'");
        }
    }
}