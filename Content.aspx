<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Content.aspx.cs" Inherits="EKECMS.Content" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="content">
<h1><%=siteinfo.Data["MC"].ToString()%></h1>
<hr />
<span class="minheight"><%=nr%></span>
</div>
</asp:Content>