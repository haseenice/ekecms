using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Application
{
    public partial class LoginOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            eUser user = new eUser("Application");
            if (user.Logined) eFHelper.UserLoginOutLog(user);//用户退出日志
            user.Remove();
            Response.Redirect("Login.aspx", true);
        }
    }
}