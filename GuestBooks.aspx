<%@ Page Language="C#" MasterPageFile="~/Master/Site.Master" AutoEventWireup="true" CodeFile="GuestBooks.aspx.cs" Inherits="EKECMS.GuestBooks" %>
<%@ Register Src="~/Controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>
<asp:Content ID="Content1" ContentPlaceHolderID="SiteBody" runat="server">
<div id="guestbooks">
<span class="minheight"><ev:eDataView ID="eDataView1" DataID="04607456-ad31-4ae4-be6c-171cf6eb6ff8" runat="server" /></span>
<div id="pages"><ev:ePageControl ID="ePagerCtrl1" PageSize="10" PageNum="9" runat="server" /></div>  
<div class="note">&nbsp;↓我要留言&nbsp;<span>Send  Message</span></div>
<uc1:Message runat="server" id="Message" />
</div>
</asp:Content>