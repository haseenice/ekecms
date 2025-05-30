<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Menu.ascx.cs" Inherits="eFrameWork.Manage.Menu" %>
<ul>
<li><a href="Default.aspx"<% =(aspxfile.IndexOf("default.aspx") > -1? " class=\"cur\"" :"") %>>首页</a></li>
<li><a href="Config.aspx"<% =("config.aspx,cache.aspx,alldomain.aspx,services.aspx,datacontents.aspx,dataviews.aspx,reports.aspx,reportitems.aspx,devusers.aspx,labels.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>初始</a></li>
<li><a href="DataSources.aspx"<% =("datasources.aspx,compare.aspx,tableinfo.aspx,query.aspx,dbmanage.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>数据库</a></li>
<li><a href="Models.aspx"<% =("models.aspx,modelitems.aspx,applications.aspx,applicationitems.aspx,sites.aspx,approles.aspx,appusers.aspx,runmodel.aspx".IndexOf(aspxfile) > -1? " class=\"cur\"" :"") %>>开发</a></li>
<li><a href="FileManage.aspx"<% =("filemanage.aspx,tokens.aspx,update.aspx".IndexOf(aspxfile) > -1 ? " class=\"cur\"" :"") %>>维护</a></li>
</ul>