<%@ Page Language="C#" MasterPageFile="~/Master/Home.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="EKECMS.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HomeBody" runat="server">
<div id="container">
<div id="left">
<div id="news">
<%if(articles!=null){%>
<h1><span><a href="Articles.aspx?id=<%=articles["ColumnID"]%>"><img class="more" src="images/none.gif"></a></span><a href="Articles.aspx?id=<%=articles["ColumnID"]%>"><%=articles["mc"]%></a></h1>
<asp:Repeater id="RepArticles" runat="server" >
<HeaderTemplate><ul></HeaderTemplate>
<ItemTemplate><li><a href="Article.aspx?id=<%# Eval("ArticleID")%>" title="<%# Eval("BT")%>"><%# Eval("BT")%></a></li></ItemTemplate>
<FooterTemplate></ul></FooterTemplate>
</asp:Repeater>
<%}%>
</div>
</div>
<div id="right">
<%if(content!=null){%>
<div id="introduce">
<h1><span><a href="Content.aspx?id=<%=content["ColumnID"]%>"><img class="more" src="images/none.gif"></a></span><%=content["mc"]%></h1>
<div><span><IMG height=129 src="<%=siteinfo.Site["Figure"].ToString() %>" width=177 border="0" ></span>
<asp:Literal ID="LitIntroduce" runat="server" /> ... <a href="Content.aspx?id=<%=content["ColumnID"]%>"><font color="#B61737">[更多]</font></a></div>
</div>
<%}%>
<%if(products!=null){%>
<div id="iproducts">
<div id="products">
<h1><span><a href="Products.aspx?id=<%=products["ColumnID"]%>"><img class="more" src="images/none.gif"></a></span><%=products["mc"]%></h1>
<asp:Repeater id="RepProducts" runat="server" >
<ItemTemplate><dl>
<dt><a href="Product.aspx?id=<%# Eval("ProductID")%>" title="<%# Eval("CPMC")%>"><img src="<%# Eval("XTP")%>" width="160" height="130" border="0" alt="<%# Eval("CPMC")%>"></a></dt>
<dd><a href="Product.aspx?id=<%# Eval("ProductID")%>" title="<%# Eval("CPMC")%>"><%# Eval("CPMC")%></a>
</dd>
</dl></ItemTemplate>
</asp:Repeater>
</div>
</div>
<%}%>
</div>
</div>
<%if(cases!=null){%>
<div id="icases">
<h1><span><a href="Cases.aspx?id=<%=cases["ColumnID"]%>"><img class="more" src="images/none.gif"></a></span><%=cases["mc"]%></h1>
<asp:Repeater id="RepCases" runat="server" >
<HeaderTemplate><div id="quee">
<table cellpadding="0" cellspacing="0" border="0">
            <tr>
            <td id="queel" valign="top">
            <table cellpadding="0" cellspacing="0" border="0">
            <tr align="center"></HeaderTemplate>
			<ItemTemplate><td><a href="Case.aspx?id=<%# Eval("CaseID")%>" title="<%# Eval("BT")%>"><img src="<%# Eval("XTP")%>"></a><span><a href="Case.aspx?id=<%# Eval("CaseID")%>" title="<%# Eval("BT")%>"><%# Eval("BT")%></a></span></td></ItemTemplate>
			<FooterTemplate></tr>
            </table>
            </td>
            <td id="queer" valign="top"></td>
            </tr>
            </table>
</div>
<script>
    var speed = 30;
    queer.innerHTML = queel.innerHTML;
    function Marquee() {
        if (queer.offsetWidth - quee.scrollLeft <= 0) { quee.scrollLeft -= queel.offsetWidth; }
        else { quee.scrollLeft++ }
    };
    var qt = setInterval(Marquee, speed);
    quee.onmouseover = function () { clearInterval(qt) };
    quee.onmouseout = function () { qt = setInterval(Marquee, speed) };
</script>
</FooterTemplate>
</asp:Repeater>
</div>
<%}%>
<div id="friendLink">
<h1>友情连接</h1>
<asp:Repeater id="RepTextLinks" runat="server" >
<ItemTemplate><a href="<%# Eval("URL")%>" title="<%# Eval("MC")%>" target="_blank"><%# Eval("MC")%></a></ItemTemplate>
</asp:Repeater>

<asp:Repeater id="RepPicLinks" runat="server" >
<HeaderTemplate><ul></HeaderTemplate>
<ItemTemplate><li><a href="<%# Eval("URL")%>" title="<%# Eval("MC")%>" target="_blank"><img src="<%# Eval("Pic")%>" width="150" height="50"></a></li></ItemTemplate>
<FooterTemplate></ul></FooterTemplate>
</asp:Repeater>
</div>
</asp:Content>