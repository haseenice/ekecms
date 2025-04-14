<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PivotConfig.aspx.cs" Inherits="eFrameWork.Plugins.PivotConfig" %>
<!DOCTYPE html>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>透视设置</title>

    <link href="../Plugins/eControls/default/style.css?ver=<%=Common.Version %>" rel="stylesheet" type="text/css" />  
  	<link href="../Plugins/Theme/default/style.css?ver=<%=Common.Version %>" rel="stylesheet" type="text/css" />  
</head>
<script>var ModelID = "<%=ModelID%>";</script>
<script src="../Scripts/Init.js?ver=<%=Common.Version %>"></script>
<style>
body {overflow:auto;margin-top:0px;box-sizing: border-box;-moz-box-sizing: border-box;-webkit-box-sizing: border-box;}
</style>
    <script>
        function saveConfigs(index)
        {
            var json = "[";
            $("#eDataTable").find("tbody tr").each(function (i, elem) {
                var item = '{"ModelItemID":"' + $(this).attr("edata") + '"';
                if ($(this).find("select[name='parentid']").length == 0) {
                    //item += ',"extend":0';
                    item += ',"parentid":""';
                }
                else {
                    //item += ',"extend":1';
                    item += ',"parentid":"' + $(this).find("select[name='parentid']").val() + '"';
                }
                item += ',"mc":"' + $(this).find("input[type='text']:eq(0)").val() + '"';
                item += ',"show":' + ($(this).find("input[type='checkbox']:eq(0)")[0].checked ? "1" : "0");
                item += ',"group":' + ($(this).find("input[type='checkbox']:eq(1)")[0].checked ? "1" : "0");
                item += ',"calctype":' + $(this).find("select[name='calctype']").val();
                item += ',"index":' + (i + 1);
                item += '}';
                if (i > 0) json += ",";
                json += item;
            });
            json += "]";
            //alert(json);
            //return;
            var url = document.location.href;
            $.ajax({
                type: "POST",
                data: { data: json },
                url: url,
                dataType: "html",
                success: function (data) {
                    //alert(data);
                    //return;
                    //parent.arrLayerIndex.pop();
                    //parent.layer.close(index);
                    var url = parent.document.location.href;
                    url = url.addquerystring("mode", "pivot");
                    url = url.removequerystring("orderby");
                    var info = parent.getSearchInfo(parent.frmsearch);
                    var hash = info.hash;
                    var keys = hash.keys();
                    for (var i = 0; i < keys.length; i++) {
                        var key = keys[i];
                        var value = hash.get(key);
                        if (value.length > 0) {
                            url = url.addquerystring(key, value);
                        } else {
                            url = url.removequerystring(key);
                        }
                    }
                    parent.document.location.assign(url);
                }
            });
        };
        function addItem(obj)
        {
            var table = $(obj).parents("table:first");
            var tfoot = table.find("tfoot");
            var tr = tfoot.find("tr")[0].cloneNode(true);
            var tbody = table.find("tbody");
            tbody[0].appendChild(tr);
            etable_init();
        };
        function deleteItem(obj)
        {
            var tr = $(obj).parents("tr:first");
            layer.confirm('确认要删除吗?', {
                title: "删除",
                shadeClose: true,
                area: ['200', '300'],
                btn: ['删除', '取消'],
                btn1: function (idx, layero) {
                    tr.remove();
                    layer.close(idx);
                }
            });
            
        };
        function chg(obj)
        {
            var td = obj.parentNode;
            var tr = td.parentNode;
            //$(tr).attr("edata", obj.value);
            var ipt = $(td).find("input[type='text']:eq(0)");
            //if (ipt.val().length == 0)
            ipt.val(obj.options[obj.selectedIndex].text);               
            
        };
    </script>
<body>
<div style="margin:10px;">
<asp:Repeater id="Rep" runat="server" >
<headertemplate>
<table id="eDataTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" >
<thead>
    <tr> 
        <td width="30"><a href="javascript:;" title="添加" debugtag="添加" onclick="addItem(this);" class="btnadd">&nbsp;</a></td>
        <td width="235">显示</td>
        <td width="60">分组</td>
        <td width="100">计算</td>
        <td width="60">顺序</td>
    </tr>
</thead>
<tbody eSize="false" eMove="true">
</headertemplate>
<itemtemplate>
  
      <tr edata="<%# Eval("ModelItemID").ToString()%>">
        <td><%# Eval("parentid").ToString().Length == 0 ? "&nbsp;" : "<a href=\"javascript:;\" title=\"删除\" onclick=\"deleteItem(this);\" class=\"btndel\">&nbsp;</a>" %></td>
        <td height="32"><label><input id="showlist_" type="checkbox"<%# eBase.parseBool(Eval("show").ToString()) ? " checked=\"checked\"" : "" %> />
             <%#
   Eval("parentid").ToString().Length > 0 ? 
   "<select name=\"parentid\" style=\"width:75px;\" onchange=\"chg(this);\">\n"+
   "<option value=\"\">选择列</option>\n"+
   ConTable.Select("ModelItemID,oldMC","len(Code)>0 or len(CustomCode)>0","ListOrder").toOptions("ModelItemID","oldMC", Eval("parentid").ToString()) +
   "</select>\n"
   :""%>
            <input type="text" name="mc" value="<%# Eval("MC").ToString()%>" style="width:105px;" /></label></td>
           <td><input <%#  Eval("Code").ToString().Length == 0 &&  Eval("CustomCode").ToString().Length == 0 ? " disabled=\"disabled\"" : "" %>  name="groups" type="checkbox"<%# eBase.parseBool(Eval("group").ToString()) ? " checked=\"checked\"" : "" %> /></td>
           <td><select name="calctype">
      <option value="0"<%# Eval("calctype").ToString()=="0" ? " selected=\"selected\"" : "" %>>默认</option>
      <option value="1"<%# Eval("calctype").ToString()=="1" ? " selected=\"selected\"" : "" %>>合计</option>
      <option value="2"<%# Eval("calctype").ToString()=="2" ? " selected=\"selected\"" : "" %>>计数</option>
      <option value="3"<%# Eval("calctype").ToString()=="3" ? " selected=\"selected\"" : "" %>>最大值</option>
      <option value="4"<%# Eval("calctype").ToString()=="4" ? " selected=\"selected\"" : "" %>>最小值</option>
      <option value="5"<%# Eval("calctype").ToString()=="5" ? " selected=\"selected\"" : "" %>>平均</option>      
    </select></td>
 <td>&nbsp;</td>
    </tr>


</itemtemplate>
<footertemplate></tbody>
    <tfoot style="display:none;">
        <tr edata="">
            <td height="32"><a href="javascript:;" title="删除" onclick="deleteItem(this);" class="btndel">&nbsp;</a></td>
            <td><input id="showlist_" type="checkbox" checked="checked" />
                <select name="parentid" style="width:75px;" onchange="chg(this);">
                <option value="">选择列</option>
                <%=ConTable.Select("ModelItemID,oldMC","len(Code)>0 or len(CustomCode)>0","ListOrder").toOptions("ModelItemID","oldMC","") %>
            </select>
                <input type="text" name="mc" style="width:105px;" />
            </td>
            <td><input disabled="disabled" id="Checkbox1" name="groups" type="checkbox"></td>
            <td><select name="calctype">
       <option value="0">默认</option>
      <option value="1">合计</option>
      <option value="2">计数</option>
      <option value="3">最大值</option>
      <option value="4">最小值</option>
      <option value="5">平均</option> 
                </select></td>
             <td>&nbsp;</td>
            </tr>
    </tfoot>
    </table></footertemplate>
</asp:Repeater>
</div>
    <%if(1==11){ %>
<input type="button" class="button" name="Submit" value="按钮" onclick="saveConfigs();" /><%} %>
</body>
</html>