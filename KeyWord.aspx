<%@ Page Language="C#" AutoEventWireup="true" CodeFile="KeyWord.aspx.cs" Inherits="EKECMS.KeyWord" %><!DOCTYPE html>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title><%=Title%></title>
   <link rel="stylesheet" type="text/css" href="<%=eBase.getAbsolutePath() %>style.css?ver=<%=Common.Version %>">
</head>
<style>
body {background:none;}
</style>
<body>
<div class="keywordbody">
<div class="title"><%=Title%></div>
<div class="line"></div>
<asp:Literal ID="LitBody" runat="server" /> 
</div>
</body>
</html>