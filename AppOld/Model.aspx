<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true" CodeFile="Model.aspx.cs" Inherits="eFrameWork.AppOld.Model" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%if (!model.Ajax){ %>
<div class="nav">您当前位置：<a href="<%=eBase.getApplicationHomeURL() %>">首页</a><%=model.ModelLink %>
<%=model.ActionBottons %></div>
<%}%>
<%= model.StartHTML  %>
<%= model.Tip  %>
<asp:Literal ID="LitBody" runat="server" />
</asp:Content>