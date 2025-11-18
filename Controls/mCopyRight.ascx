<%@ Control Language="C#" AutoEventWireup="true" CodeFile="mCopyRight.ascx.cs" Inherits="EKECMS.Controls.mCopyRight" %>
<%if(SiteInfo.Site["ContentTel"].ToString().Length>0){%>
<div class="Mobile"><a href="tel:<%=SiteInfo.Site["ContentTel"].ToString().Replace("-","")%>">点击免费电话咨询:<%=SiteInfo.Site["ContentTel"].ToString()%></a></div>
<%}%>
<div id="copyright" style="margin-top:15px;">
<div class="bottom_navigation"></div>
<div class="bottom_copyright"><%=SiteInfo.Site["mFooter"].ToString()%></div>
</div>
<div id="tool">
<ul class="toollist">
<%=Tools%>
</ul>
</div>