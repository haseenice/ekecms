using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using LitJson;
using EKETEAM.Tencent.WeChat;

namespace eFrameWork.Plugins
{
    public partial class BindWeChat : System.Web.UI.Page
    {
        public string UserArea = "Application";
        public eUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser(UserArea);
            if (Request.QueryString["data"] != null)
            {
                string data = Request.QueryString["data"].ToString();
                string base64 = Base64.Decode(data);
                if (base64.Length > 5 && base64.StartsWith("\"") && base64.EndsWith("\"")) base64 = base64.Substring(1, base64.Length - 2);
                base64 = base64.Replace("\\\\/", "\\/");
                base64 = base64.Replace("\\\"", "\"");
                if (base64.StartsWith("{"))
                {
                    JsonData json = JsonMapper.ToObject(base64);
                    string openid = json.GetValue("openid");
                    string unionid = json.GetValue("unionid");
                    //eBase.Print(json);
                    eTable etb = new eTable("a_eke_sysUsers", user);
                    etb.DataBase = eBase.UserInfoDB;
                    etb.Fields.Add("openid", openid);
                    if (unionid.Length > 0) etb.Fields.Add("unionid", unionid);
                    etb.Fields.Add("nickname", json.GetValue("nickname"));
                    etb.Fields.Add("sex", json.GetValue("sex"));

                    string headimgurl = json.GetValue("headimgurl").Replace("\\", "");
                    string face = WeChatHelper.downHeadImage(user.ID, headimgurl);

                    etb.Fields.Add("headimgurl", headimgurl);
                    etb.Fields.Add("face", face);
                    etb.Fields.Add("country", json.GetValue("country"));
                    etb.Fields.Add("province", json.GetValue("province"));
                    etb.Fields.Add("city", json.GetValue("city"));
                    etb.Where.Add("UserID='" + user.ID + "'");
                    etb.Update();
                    //Response.Write("<script>top.CloseWeChat();</script>");//AppFrame无效
                    Response.Write("<script>if(top.frames.length==1){top.CloseWeChat();}else{parent.parent.CloseWeChat();}</script>");
                }
                eBase.End();
            }
            if (eBase.WeChatAccount.getValue("Proxy").Length ==0 && (eBase.WeChatAccount.getValue("OpenAppID").Length == 0 || eBase.WeChatAccount.getValue("OpenAppSecret").Length == 0))
            {
                litBody.Text = "系统没有绑定微信帐号!";
                return;
            }

            string _openid = eBase.UserInfoDB.getValue("select unionid from a_eke_sysUsers where UserID='" + user.ID + "'");
            if (_openid.Length == 0) _openid = eBase.UserInfoDB.getValue("select openid from a_eke_sysUsers where UserID='" + user.ID + "'");
            if (_openid.Length > 0)
            {
                litBody.Text = "已绑定!";
            }
            else
            {
                litBody.Text = "<a class=\"button btnprimary\" href=\"javascript:;\" onclick=\"BindWeChat();\"><span style=\"letter-spacing:1px;\"><i class=\"\">授权绑定</i></span></a>";
            }
        }
    }
}