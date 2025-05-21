<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WeChatScanLogin.aspx.cs" Inherits="eFrameWork.Plugins.WeChatScanLogin" %>
<%@ Import Namespace="EKETEAM.FrameWork" %>
<!DOCTYPE html>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>微信扫码登录</title>
     <script src="//res.wx.qq.com/connect/zh_CN/htmledition/js/wxLogin.js"></script>
     <script src="<%=eBase.getAbsolutePath() %>Scripts/jquery.js"></script>
</head>
 <style>
.impowerBox .qrcode {width: 200px;}
.impowerBox .title {display: none;}
.impowerBox .info {width: 200px;}
.status_icon {display: none  !important}
.impowerBox .status {text-align: center;}

</style>
<body>
     <div id="login_container"></div>
</body>
</html>
<script>
    $(function () {
        //开放平台申请 网站应用才可以扫码登录，公众号不支持。
        var obj = new WxLogin({
            self_redirect:  <%=(fromURL.ToLower().IndexOf("bindwechat.aspx")>-1 ? "true" : "false")%>,//true：手机点击确认登录后可以在 iframe 内跳转到 redirect_uri，false：手机点击确认登录后可以在 top window 跳转到 redirect_uri。默认为 false。
        "id": "login_container",//div的id
        "appid": "<%=eBase.WeChatAccount.getValue("OpenAppID") %>",
        "scope": "snsapi_login",
            //"redirect_uri": "<%=Request.Url.PathAndFile().UrlEncode()%>",//回调地址
        "redirect_uri": "<%=HttpUtility.UrlEncode(Request.Url.AbsoluteUri)%>",  
        "state": "<%=HttpUtility.UrlEncode(HttpUtility.UrlEncode(fromURL))%>", //参数，可带可不带
        "style": "",//样式  提供"black"、"white"可选，默认为黑色文字描述
        "href": "" //自定义样式链接，第三方可根据实际需求覆盖默认样式。 
    });
});
</script>