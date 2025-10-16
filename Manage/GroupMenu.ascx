<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupMenu.ascx.cs" Inherits="Manage_GroupMenu" %>
<div class="esearchgroup">
<%     
    //初始
    if ("config.aspx,cache.aspx,alldomain.aspx,services.aspx,datacontents.aspx,dataviews.aspx,reports.aspx,reportitems.aspx,devusers.aspx,labels.aspx".IndexOf(aspxfile) > -1)
   {%>
    <a href="Config.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("config.aspx") > -1 ? " class=\"cur\"" :"") %>>配置</a>
    <a href="Labels.aspx" onfocus="this.blur();"<% =("labels.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>标签</a>
    <a href="DevUsers.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("devusers.aspx") > -1 ? " class=\"cur\"" :"") %>>开发用户</a>
   <!--
    <a href="AllDomain.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("alldomain.aspx") > -1 ? " class=\"cur\"" :"") %>>域名授权</a>
    <a href="Services.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("services.aspx") > -1 ? " class=\"cur\"" :"") %>>服务商</a>
       -->
    <a href="DataContents.aspx"<% =(aspxfile.IndexOf("datacontents.aspx") > -1 ? " class=\"cur\"" :"") %>>静态内容</a>
    <a href="DataViews.aspx"<% =(aspxfile.IndexOf("dataviews.aspx") > -1 ? " class=\"cur\"" :"") %>>动态数据</a>
        <a href="Reports.aspx" onfocus="this.blur();"<% =("reports.aspx,reportitems.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>报表</a>
    <a href="Cache.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("cache.aspx") > -1 ? " class=\"cur\"" :"") %>>缓存</a>
  
<%} %>

<%     
    //数据库
    if ("datasources.aspx,compare.aspx,compareonline.aspx,tableinfo.aspx,query.aspx,dbmanage.aspx".IndexOf(aspxfile) > -1)
   {%>
<a href="DataSources.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("datasources.aspx") > -1 ? " class=\"cur\"" :"") %>>数据源</a>
<a href="Compare.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("compare.aspx") > -1 ? " class=\"cur\"" :"") %>>库对比</a>
<a href="CompareOnline.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("compareonline.aspx") > -1 ? " class=\"cur\"" :"") %>>库对比(在线)</a>

<a href="TableInfo.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("tableinfo.aspx") > -1 ? " class=\"cur\"" :"") %>>结构</a>
<!--
<a href="Query.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("query.aspx") > -1 ? " class=\"cur\"" :"") %>>查询</a>
-->
<a href="DBManage.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("dbmanage.aspx") > -1 ? " class=\"cur\"" :"") %>>备份还原</a>
<%} %>

<%
    //开发
    if ("models.aspx,modelitems.aspx,applications.aspx,applicationitems.aspx,applicationitemsn.aspx,sites.aspx,approles.aspx,appusers.aspx,runmodel.aspx,accounts.aspx".IndexOf(aspxfile) > -1)
   {%>
 
    <a href="Models.aspx" onfocus="this.blur();"<% =("models.aspx,modelitems.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>模块</a>
    <a href="Applications.aspx" onfocus="this.blur();"<% =("applications.aspx,applicationitems.aspx,applicationitemsn.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>应用</a>
    <a href="Sites.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("sites.aspx") > -1 ? " class=\"cur\"" :"") %>>企业</a>
    <a href="Accounts.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("accounts.aspx") > -1 ? " class=\"cur\"" :"") %>>第三方帐号</a>
    <a href="AppRoles.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("approles.aspx") > -1 ? " class=\"cur\"" :"") %>>角色</a>
    <a href="AppUsers.aspx" onfocus="this.blur();"<% =(aspxfile.IndexOf("appusers.aspx") > -1 ? " class=\"cur\"" :"") %>>用户</a>


<%} %>

<%
     //维护
    if ("filemanage.aspx,tokens.aspx,update.aspx".IndexOf(aspxfile) > -1)
   {%>
    <a href="FileManage.aspx"<% =("filemanage.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>文件库</a>
    <a href="ToKens.aspx"<% =(aspxfile.IndexOf("tokens.aspx") > -1 ? " class=\"cur\"" :"") %>>令牌</a>
    <a href="Update.aspx"<% =(aspxfile.IndexOf("update.aspx") > -1 ? " class=\"cur\"" :"") %>>更新</a>
    <%} %>
</div>
