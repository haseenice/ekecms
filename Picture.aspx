<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Picture.aspx.cs" Inherits="EKECMS.Picture" %>
<%@ Register Src="~/Controls/Comment.ascx" TagPrefix="uc1" TagName="Comment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="content">
<h1><%=siteinfo.Data["bt"].ToString()%></h1>
<hr />
<span class="minheight">
<%=nr%>
</span>
<hr />
</div>
<uc1:Comment runat="server" ID="Comment" />
</asp:Content>