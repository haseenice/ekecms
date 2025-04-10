<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Cases.aspx.cs" Inherits="EKECMS.Cases" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="cases">
<span class="minheight">
    <!--
    <ev:eDataView ID="eDataView1" DataID="d99bc7e7-8854-4ff6-b50b-69aaa572e7d3" runat11111="server" />
    -->
<asp:Repeater id="RepList" runat="server">
<headertemplate><ul></headertemplate>
<itemtemplate>
<li>
<dl>
<dt><a href="Case.aspx?id=<%# Eval("CaseID").ToString()%>" title="<%# Eval("bt").ToString()%>"><%# Eval("bt").ToString()%></a></dt>
<dd><%# Eval("jj").ToString()%></dd>
</dl>
<span>
<a href="Case.aspx?id=<%# Eval("CaseID").ToString()%>" title="<%# Eval("bt").ToString()%>"><img src="<%# Eval("xtp").ToString()%>" width="130" height="90" border="0" alt="<%# Eval("bt").ToString()%>"></a>
</span>
</li>
</itemtemplate>
<footertemplate></ul></footertemplate>
</asp:Repeater>	
<div style="margin-top:8px;margin-bottom:8px;clear:both;">
<div id="pages"><ev:ePageControl ID="ePageControl1" PageSize="10" PageNum="9" runat="server" /></div>   
</div>
</span>
</div>
</asp:Content>