<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Articles.aspx.cs" Inherits="EKECMS.Articles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="articles">
<span class="minheight">
<asp:Repeater id="RepList" runat="server">
<headertemplate><ul></headertemplate>
<itemtemplate>
<li><span><%# Eval("addTime","{0:yyyy-MM-dd}")%></span><a href="<%# Eval("url").ToString().Length > 0 ? Eval("url").ToString() : "Article.aspx?id=" + Eval("ArticleID").ToString() %>" target="<%# Eval("url").ToString().Length > 0 ? "_blank" : "_self" %>" title="<%# Eval("BT").ToString()%>"><%# Eval("BT").ToString()%></a><%# Eval("newimg").ToString()%></li>
</itemtemplate>
<footertemplate></ul></footertemplate>
</asp:Repeater>	
<div id="pages"><ev:ePageControl ID="ePageControl1" PageSize="10" PageNum="9" runat="server" /></div>   
</span>
</div>
</asp:Content>