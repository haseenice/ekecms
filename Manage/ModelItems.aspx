<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true" CodeFile="ModelItems.aspx.cs" Inherits="eFrameWork.Manage.ModelItems" %>
<%@ Register Src="GroupMenu.ascx" TagPrefix="uc1" TagName="GroupMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<uc1:GroupMenu runat="server" ID="GroupMenu" />
  <div class="nav">您当前位置：<a href="Default.aspx">首页</a> -> 模块管理&nbsp;&nbsp;
      <label><input name="chkdisabled" type="checkbox" value="0"<%=(ReadOnly ? " checked=\"checked\"" : "") %> onclick="chkdisabled_click(this);">只读</label>
  </div>
  <script>var ModelID = "<%=ModelID%>";</script>
  <script src="javascript/columns.js?ver=<%=Common.Version %>"></script>
     <script src="../Plugins/kindeditor351/kindeditor.js"></script>
  <script>
      function insertFunction(id) {

          //$(document).ready(function(){   });
          var _style = 'display:inline-block;padding:2px 8px 2px 8px;min-width:60px;text-align:center;border: 1px dashed #ccc;margin:6px 6px 0px 0px;';
          var html = '<div style="margin:10px;line-height:20px;">';
          html += '<div style="font-size:16px;">控件</div>';
       


          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//人员选择回调函数\\nfunction UserSelect_callback(ctrlid,id,name)\\n{\\n\\n};\\n\');">人员选择回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//部门选择回调函数\\nfunction DepartmentSelect_callback(ctrlid,id,name)\\n{\\n\\n};\\n\');">部门选择回调函数</a>';

          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//列表数据选择回调函数\\nfunction select_callback()\\n{\\n\\n};\\n\');">列表数据选择回调函数</a>';

          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//选项卡回调函数\\nfunction eTab_callback(dd,div,index)\\n{\\n\\n};\\n\');">选项卡回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//面板回调回调函数\\nfunction ePanel_callback(dd, show)\\n{\\n\\n};\\n\');">面板回调回调函数</a>';

          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//过滤下拉框选择回调函数\\nfunction filterSelect_change(input,value)\\n{\\n\\n};\\n\');">过滤下拉框选择回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//自动下拉前回调函数\\nfunction autoselect_before(name,level)\\n{\\n\\n};\\n\');">自动下拉前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//自动下拉改变回调函数\\nfunction autoselect_change(name,value)\\n{\\n\\n};\\n\');">自动下拉改变回调函数</a>';

          html += '<div style="font-size:16px;margin-top:10px;">子模块</div>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//加载子模块表单前回调函数\\nfunction getAddHTML_before(modelid)\\n{\\n\\n};\\n\');">加载子模块表单前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//子模块添加完成回调函数\\nfunction form_add_finish(modelid)\\n{\\n\\n};\\n\');">子模块添加完成回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//子模块编辑完成回调函数\\nfunction form_edit_finish(modelid)\\n{\\n\\n};\\n\');">子模块编辑完成回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//子模块查看完成回调函数\\nfunction form_view_finish(modelid)\\n{\\n\\n};\\n\');">子模块查看完成回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//子模块数据改变回调函数\\nfunction submodel_onChange(modelid,trs,tbody)\\n{\\n\\n};\\n\');">子模块数据改变回调函数</a>';

          html += '<div style="font-size:16px;margin-top:10px;">数据回填</div>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//打开选择回填窗口前回调函数\\nfunction OpenFillSelectDlg_before(modelid)\\n{\\nreturn {result:true,query:&quot;&quot;};\\n};\\n\');">打开选择回填窗口前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//关闭选择回填窗口后回调函数\\nfunction OpenFillSelectDlg_finish(modelid)\\n{\\n\\n};\\n\');">关闭选择回填窗口后回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//打开回填窗口前回调函数\\nfunction OpenFillDlg_before(modelid)\\n{\\nreturn {result:true,query:&quot;&quot;};\\n};\\n\');">打开回填窗口前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//关闭回填窗口后回调函数\\nfunction OpenFillDlg_finish(modelid)\\n{\\n\\n};\\n\');">关闭回填窗口后回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//数据回填完成回调函数\\nfunction FillData_finish(fillparent,modelid)\\n{\\n\\n};\\n\');">数据回填完成回调函数</a>';


          html += '<div style="font-size:16px;margin-top:10px;">搜索</div>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//搜索前回调函数\\nfunction search_before(frm)\\n{\\n\\n};\\n\');">搜索前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//数据保存完成回调函数\\nfunction postback(data)\\n{\\n\\n};\\n\');">数据保存完成回调函数</a>';

          html += '<div style="font-size:16px;margin-top:10px;">表单验证</div>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//表单验证前回调函数\\nfunction eCheckform_before(frm)\\n{\\n\\n};\\n\');">表单验证前回调函数</a>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//表单验证完成回调函数\\nfunction eCheckform_finish(frm)\\n{\\n\\n};\\n\');">表单验证完成回调函数</a>';


          html += '<div style="font-size:16px;margin-top:10px;">其他</div>';
          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'//文档加载完毕函数\\n$(document).ready(function(){\\n\\n});\\n\');">文档加载完毕函数</a>';

          html += '<a href="javascript:;" style="' + _style + '" onclick="addHtmlTag(\'' + id + '\',\'getSelectIDS();\');">获取选中数据ID</a>';
          

          


          html += '</div>';
          layer.open({
              type: 1, //此处以iframe举例
              title: '插入函数', // + "窗口"
              shadeClose: true, //点击遮罩关闭层
              area: ["60%", "550px"],
              btnAlign: 'l', //lcr
              moveType: 0, //拖拽模式，0或者1
              content: html,
              btn: ['取消'],
              yes: function (index, layero) {
                  layer.close(index);
              }
          });
      };
      function fileLibrary_select_finish(objid)
      {
          var obj = $("#" + objid);
          var value = obj.val();
          if (value.toLowerCase().indexOf("upload/") > 0)
          {
              value = value.substring(value.toLowerCase().indexOf("upload/"));
              obj.val(value);
          }
          setModel(obj[0], objid);
      };
      //var ControlJson = < %=ControlJson.ToJson()% >;
      //var linkArrys = [< %=linkArrys% >];
      function insertData(id)
      {          
          //layer.msg(linkArrys.length);
          var html = '<div style="margin:10px;line-height:20px;">';
          var data = ControlJson;
          for (i = 0; i < data.length; i++) {
              html += '<div style="font-size:16px;">' + data[i].name + '</div>';
              var items = data[i].items;
              for (j = 0; j < items.length; j++) {
                  html += '<a href="javascript:;" style="display:inline-block;padding:2px 8px 2px 8px;min-width:60px;text-align:center;border: 1px dashed #ccc;margin:6px 6px 0px 0px;"';
                  if (items[j].type == "data") {
                      html += ' onclick="addHtmlTag(\'' + id + '\',\'{data:' + items[j].code + '}\');">' + items[j].name + '</a>';
                  }
                  else {
                      html += ' onclick="addHtmlTag(\'' + id + '\',\'{model:' + items[j].modelid + '}\');">' + items[j].name + '(模块)</a>';
                  }
              }
          }
          html += '</div>';
          layer.open({
              type: 1, //此处以iframe举例
              title: '引用数据', // + "窗口"
              shadeClose: true, //点击遮罩关闭层
              area: ["600px", "360px"],
              btnAlign: 'l', //lcr
              moveType: 0, //拖拽模式，0或者1
              content: html,
              btn: ['取消'], 
              yes: function (index, layero) {       
                  layer.close(index);
              }
          });
      };
 function createStep(modelid,title)
 {
 //alert(modelid);
 
 var html='<div style="margin:15px;line-height:30px;">';
	html+='<input name="copyname" type="text" id="copyname" style="border:1px solid #ccc;height:20px;line-height:20px;font-size:13px;color:#333;width:230px;" value="' + title +'" /><br>';

	html+='</div>';
	layer.open({
		type: 1, //此处以iframe举例
		title: '添加编辑步骤', // + "窗口"
		//skin: 'layui-layer-molv' //加上边框 layui-layer-rim  layui-layer-lan layui-layer-molv layer-ext-moon
		shadeClose: true, //点击遮罩关闭层
		area: ["300px","180px"],
		btnAlign: 'l', //lcr
		moveType: 0, //拖拽模式，0或者1
		content: html,
		btn: ['确认', '取消'], 
		yes: function(index,layero)
		{
			var name=$("#copyname").val();
			var url="Models.aspx?ID=" + modelid + "&act=createstep";
			if(name.length>0){url+="&name=" + name.encode();}			
			layer.close(index);
			//alert(url);
			document.location.href=url;
		}
	});
 
 
 };
//表单回填
 function openFillwin(parentid,modelid)
 {
     var url = "ModelItems_FillData.aspx?parentid=" + parentid + "&modelid=" + modelid;
     layer.open({
         type: 2,
         title: "对应关系",
         maxmin: false,
         shadeClose: true, //点击遮罩关闭层
         area: ["750px", "450px"],
         // content: [url,'no'], 
         content: url,
         success: function (layero, index) { },
         cancel: function (index, layero) { },
         end: function (index) { }
     });
 };
//搜索回填
 function openFillwins(parentid, modelid) {
     var url = "ModelItems_FillDatas.aspx?parentid=" + parentid + "&modelid=" + modelid;
     layer.open({
         type: 2,
         title: "对应关系",
         maxmin: false,
         shadeClose: true, //点击遮罩关闭层
         area: ["750px", "450px"],
         // content: [url,'no'], 
         content: url,
         success: function (layero, index) { },
         cancel: function (index, layero) { },
         end: function (index) { }
     });
 };
 function Model_Copy(modelid) {
     var url = "ModelCopy.aspx?single=true&modelid=" + modelid;
     layer.open({
         type: 2,
         title: "复制模块",
         maxmin: false,
         shadeClose: true, //点击遮罩关闭层
         area: ["650px", "300px"],
         // content: [url,'no'], 
         content: url,
         success: function (layero, index) { },
         cancel: function (index, layero) { },
         end: function (index) { document.location.assign(document.location.href); }
     });
 };
 function addprint(obj)
 {
     //var dl = $(obj).parents("dl:first");
     //alert(dl.find("dt a.cur").index());
     var url = "?act=addprint&modelid=" + ModelID;
     url += "&t=" + now();
     showloading();
     //alert(url);
     $.ajax({
         type: "GET", async: true,
         url: url,
         dataType: "json",
         success: function (data) {
             hideloading();
             skipLogin(data);
         },
         error: function (XMLHttpRequest, textStatus, errorThrown) {
             hideloading();
         }
     });
 };
 function setPrint2(obj, modelprintid, item)
 {
     obj = obj.previousElementSibling || obj.previousSibling;
     while (obj.tagName != "INPUT" && obj.tagName != "TEXTAREA") {
         obj = obj.previousElementSibling || obj.previousSibling;
     }
     setPrint(obj, modelprintid, item);
 };
 function setPrint(obj,modelprintid,item)
 {
     var value = getValue(obj);
     if (value == "error") { return; }
     showloading();
     var url = "?act=setprint&modelid=" + ModelID + "&modelprintid=" + modelprintid + "&item=" + item + "&t=" + now();
     $.ajax({
         type: "post",
         async: true,
         data: { value: value },
         url: url,
         dataType: "json",
         success: function (data) {
             hideloading();
             skipLogin(data);
         },
         error: function (XMLHttpRequest, textStatus, errorThrown) {
             hideloading();
             
         }
     });
 };
  </script>
  <style>
/*
td{-webkit-touch-callout: none;-webkit-user-select: none;-khtml-user-select: none;-moz-user-select: none;-ms-user-select: none;user-select: none;}
*/
input[type=checkbox],input[type=radio]{margin-right:6px;}
input.edit{ text-indent:5px;border:1px solid #ccc;font-size:12px;height:23px;line-height:23px;}
textarea {border:1px solid #ccc;font-size:12px;line-height:20px;padding-left:5px;}
.text{text-indent:5px;display:inline-block;width:100%;border:1px solid #ccc;font-size:12px;height:23px;line-height:23px;}
.eDataTable tbody td {text-overflow: inherit;-o-text-overflow: inherit;}
#eDataTable .edit{text-indent:5px;background-color:transparent;border:0px solid #ccc;height:26px;width:100%;font-size:12px;padding:0px;background-color:#f2f2f2;}
.divloading{filter:alpha(opacity=50);-moz-opacity:0.5;-khtml-opacity: 0.5; opacity: 0.5; position:fixed;width:100%;height:100%;top:0px;left:0px;background:#cccccc url(images/loading.gif) no-repeat center center;}
</style>
  <div id="divloading" class="divloading" style="display:none;">&nbsp;</div>
  <div style="margin:6px;margin-top:0px;border:0px solid #ff0000;">
                <input id="model_remark2" type="hidden" oldvalue="<%=ModelInfo["remark"].ToString().HtmlEncode() %>" value="<%=ModelInfo["remark"].ToString().HtmlEncode() %>" onBlur="setModel(this,'remark');">
      <textarea id="model_remark" name="model_remark" onBlur="setModel(this,'remark');" style=" position:absolute;top:-5000px;"><%=ModelInfo["remark"].ToString().HtmlEncode() %></textarea>
      <a href="javascript:;"  onclick="dblClick1('model_remark','模块备注','1000px','600px');" style="float:right;"><img src="images/jsonedit.png" style="cursor:pointer;margin-right:5px;" align="absmiddle">模块备注</a>
    <div style="margin-bottom:10px;">
      <asp:Literal ID="LitMenu" runat="server" />
    </div>         
    <dl class="eTab" style="min-width:1100px;">
      <dt><%=titles %></dt>
      <dd><%=bodys %></dd>
    </dl>
  </div>
</asp:Content>
