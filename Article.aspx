<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Article.aspx.cs" Inherits="EKECMS.Article" %>
<%@ Register Src="~/Controls/Comment.ascx" TagPrefix="uc1" TagName="Comment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="content">
<h1><%=siteinfo.Data["BT"].ToString()%></h1>
<h2>作者.<%=siteinfo.Data["ZZ"].ToString()%>&nbsp;&nbsp;日期.<%= String.Format("{0:yyyy-MM-dd}", siteinfo.Data["addtime"])%>&nbsp;&nbsp;来源.<%=siteinfo.Data["LY"].ToString()%>&nbsp;&nbsp;浏览.<%=siteinfo.Data["CKCS"].ToString()%>
                        <script src="Count.aspx?lb=2&id=<%=siteinfo.ID%>"></script></h2>
<hr />
<%if(siteinfo.Data["BT"].ToString().Length>0){%><div id="remark">
&nbsp;&nbsp;&nbsp;&nbsp;摘&nbsp;要：<%=siteinfo.Data["BT"].ToString()%>
</div><%}%>
<span class="minheight">
<%=nr%>
</span>
<hr />
<ul>
          <li><b>上一篇：</b><%=syp%></li>
          <li><b>下一篇：</b><%=xyp%></li>
        </ul>
		<div class="navbox"><a href="javascript:history.back();">【返回】</a>&nbsp;<a href="#">【顶部】</a>&nbsp;<a href="javascript:window.close();">【关闭】</a></div>	
<%if(siteinfo.State.Length>0){%>
<hr />
<div id="state">
<strong>免责声明：</strong><br>
<%=siteinfo.State%>
</div>
<%}%>	
</div>
<uc1:Comment runat="server" ID="Comment" />
</asp:Content>