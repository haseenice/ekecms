<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true" CodeFile="CompareOnline.aspx.cs" Inherits="eFrameWork.Manage.CompareOnline" %>
<%@ Register Src="GroupMenu.ascx" TagPrefix="uc1" TagName="GroupMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
ul,li{margin:0px;padding:0px;list-style-type:none;}
.close{background:url(../images/close.gif) no-repeat scroll 8px 8px transparent;padding-left:23px; cursor:pointer;}
.open{background:url(../images/open.gif) no-repeat scroll 8px 8px transparent;padding-left:23px; cursor:pointer;}
</style>
<script>
    function show(obj)
    {
        var evt = window.event || arguments[0];
        var src = evt.srcElement || evt.target;
        if (src.tagName == "A") { return;}
        if(obj.className.toLowerCase()=="open")
        {
            obj.className="close";
            //$(obj).next("ul").hide();
            $(obj).next("ul").slideUp();
        }
        else
        {
            obj.className="open";
            //$(obj).next("ul").show();
            $(obj).next("ul").slideDown();
        }
    };
    function addField(obj,table,field)
    {
        var url = "?act=add&ajax=true&table=" + table + "&code=" + field;
        $.ajax({
            type: 'get',
            url: url,
            dataType: "html",
            success: function (data) {
                var li = $(obj).parent();
                $(obj).remove();
                li.attr("style", "");
            }
        });
    };
    function createTable(obj, table)
    {
        var url = "?act=create&ajax=true&table=" + table;
        $.ajax({
            type: 'get',
            url: url,
            dataType: "html",
            success: function (data) {
                var div = $(obj).parent();
                div.attr("style", "");
                var ul = div.next();
                ul.find("li").attr("style", "");
                $(obj).remove();
            }
        });
    };
</script>
<uc1:GroupMenu runat="server" ID="GroupMenu" />    
<div class="nav">您当前位置：<a href="Default.aspx">首页</a> -> 库对比(在线)</div>
<%if (eRegisterInfo.Base == 0 && eRegisterInfo.Loaded)
  { %>
    <div style="margin:6px;line-height:25px;font-size:13px;">
<div class="tips" style="margin-bottom:6px;"><b>未授权提示</b><br><a href="http://frame.eketeam.com/getSerialNumber.html" style="color:#ff0000;" target="_blank">申请临时授权</a>,享更多功能。</div>
   </div>
 <%} %>
    <asp:Literal id="litBody2" runat="server" />
     <asp:Literal id="litBody3" runat="server" />
<div style="margin:6px;line-height:25px;font-size:13px;">



    <form id="form1" name="form1" method="post" action="">
对比方式：
      <select name="pattern" id="pattern" style="height:26px;">
    <option value="1"<%=(pattern=="1" ? " selected=\"true\"" : "") %>>简单</option>
    <option value="2"<%=(pattern=="2" ? " selected=\"true\"" : "") %>>详细</option>
  </select>

  <input type="submit" name="Submit" value="开始比较" style="padding:3px 10px 3px 10px;margin-left:10px;" />
</form>



 <asp:Literal ID="LitBody" runat="server" />
</div>
</asp:Content>
