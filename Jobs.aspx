<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Jobs.aspx.cs" Inherits="EKECMS.Jobs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
		<div class="right_body">
            <asp:Repeater id="Rep" runat="server" >
            <HeaderTemplate><ul class="joblist"></HeaderTemplate>
            <ItemTemplate>
			<li>
			<table class="job_table">
			<tr><th  class="t1">职位名称</th>
			<td  colspan="2"  class="t2"><%# Eval("MC")%></td><td><div class="operation"><a href="Resume.aspx?id=<%# Eval("JobID")%>" class="btn" >应聘此岗位</a></div></td>
			</tr><tr><th class="">招聘人数</th><td class="t2"><%# Eval("RS")%></td><th class="t3">有效期至</th><td class="t4"><%# Eval("YXQ")%></td></tr>
			<tr><th class="">学历要求</th><td class="t2"><%# Eval("XL")%></td><th class="t3">性别要求</th><td class="t4"><%# Eval("XB")%></td></tr>
			<tr><th class="">年龄要求</th><td class="t2"><%# Eval("NL")%></td><th class="t3">薪金待遇</th><td class="t4"><%# Eval("GZ")%></td><tr>
			<tr><th class="">语言要求</th><td class="t2"><%# Eval("YY")%></td><th class="t3">工作地点</th><td class="t4"><%# Eval("DD")%></td></tr>
			<tr><th>详细要求</th>
			<td colspan="3" class="Requirement"><div class="cc_m" style="height: 100%"><%# Eval("Info").ToString().Replace("\n","<br>")%></div></td>
			</tr>
			</table>
			</li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
            <div style="clear:both;text-align:center;padding-top:8px;"><ev:ePageControl ID="ePageControl1" PageSize="5" PageNum="9" runat="server" /></div>
		</div>    
</asp:Content>