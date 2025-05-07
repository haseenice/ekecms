using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using EKETEAM.Tencent.WxWork;
using LitJson;

namespace eFrameWork.Plugins
{
    public partial class BindWxWorkCode : System.Web.UI.Page
    {
        public string UserArea = "Application";
        eUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["mode"] != null) return;
            user = new eUser(UserArea);

            string code = Request["code"];
            if (string.IsNullOrEmpty(code)) return;

            JsonData json = WxWorkHelper.getUserInfo(code);

            string qyuserid = json.getValue("UserId"); //非企业成员 返回OpenId ，这里为空
            string userticket = json.getValue("user_ticket");//扫码不会返回 user_ticket
            if (qyuserid.Length == 0)
            {
                Response.Write("登录失败,请与企业微信管理员联系!");
                Response.End();
            }
            JsonData jd = WxWorkHelper.getUser(qyuserid);
            //Response.Write(json.ToJson());
            //Response.Write(jd.ToJson());

            //Response.End();

            eTable etb = new eTable("a_eke_sysUsers", user);
            etb.DataBase = eBase.UserInfoDB;
            etb.Fields.Add("qyUserID", qyuserid);
            etb.Fields.Add("name", jd.GetValue("name"));
            etb.Fields.Add("gender", jd.GetValue("gender"));
            string headimgurl = jd.GetValue("avatar").Replace("\\", "");
            string face = WxWorkHelper.downHeadImage(user.ID, qyuserid,headimgurl);
            etb.Fields.Add("avatar", headimgurl);
            etb.Fields.Add("face", face);
            etb.Where.Add("UserID='" + user.ID + "'");
            etb.Update();
            //Response.Write("<script>top.CloseWxWork();</script>");//AppFrame无效
            Response.Write("<script>if(top.frames.length==1){top.CloseWxWork();}else{parent.CloseWxWork();}</script>");
            Response.End();
            

            
        }
        
    }
}