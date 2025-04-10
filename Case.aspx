<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="Case.aspx.cs" Inherits="EKECMS.Case" %>
<%@ Register Src="~/Controls/Comment.ascx" TagPrefix="uc1" TagName="Comment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<script src="Scripts/jquery.jcarousellite.min.js"></script>
<div id="content">
<div class="caseBox">
<h1><%=siteinfo.Data["BT"].ToString()%><span>（<span class="current">1</span>/<%=ct%>）</span></h1>
<hr>
<%if(siteinfo.Data["jj"].ToString().Length>0){%><div id="remark" style="border:none; background:none;">
&nbsp;&nbsp;&nbsp;&nbsp;<strong>摘&nbsp;要：</strong><%=siteinfo.Data["jj"].ToString()%>
</div><%}%>
<div class="OriginalPicBorder">
		<div id="OriginalPic">
			<div id="aPrev" class="CursorL"></div>
			<div id="aNext" class="CursorR"></div>
			<%=bigimg%>
		</div>
	</div>
	<div class="ThumbPicBorder">
		<img src="images/ArrowL.jpg" id="btnPrev" />
		<div class="Lite">
			<ul id="ThumbPic"><%=smimg%></ul>
		</div>
		<img src="images/ArrowR.jpg" id="btnNext" />

	</div>
</div>
<script type="text/javascript">
    //缩略图滚动事件
    $(".Lite").jCarouselLite({
        btnNext: "#btnNext",
        btnPrev: "#btnPrev",
        scroll: 1,
        speed: 240,
        circular: false,
        visible: 4
    });
</script>
<script src="Scripts/case.js?ver=<%=Common.Version %>"></script>
<h3 style="">案例简介</h3>
<%=nr%>
</div>
<uc1:Comment runat="server" ID="Comment" />
</asp:Content>