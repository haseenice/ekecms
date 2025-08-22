<%@ Page Language="C#" MasterPageFile="Main.Master" AutoEventWireup="true" CodeFile="Models.aspx.cs" Inherits="eFrameWork.Manage.Models" %>
<%@ Register Src="GroupMenu.ascx" TagPrefix="uc1" TagName="GroupMenu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script>
var act = "<%=Action.Value%>";
window.onload=function()
{
    if (act == "add" || act == "edit") { selType(); getTables(); }
    if (act == "edit")
    {
        //$("#formtable").val("<%=f2.Value%>");
        var value = "<%=f2.Value%>".toLowerCase();
        $("#formtable option").each(function(i,opt){
            if($(opt).val().toLowerCase()==value)
            {
                $(opt).attr("selected",true);
            }  
        });
    }
};
//Excel生成模块
function ExcelToModel()
{
    var url = "ExcelToModel.aspx";
    layer.open({
        type: 2,
        skin: 'layui-layer-rim', //加上边框
        title: "Excel生成模块",
        maxmin: false,
        shadeClose: true, //点击遮罩关闭层
        area: ['75%', '80%'],
        content: url,
        success: function (layero, index) { arrLayerIndex.push(index); },
        cancel: function (index, layero) { arrLayerIndex.pop(); },
        end: function (index) { arrLayerIndex.pop(); document.location.assign(document.location.href); }
    });
};
//模块导入
function Model_import()
{
	var url= "ModelImport.aspx";	
	layer.open({
      type: 2,
	  skin: 'layui-layer-rim', //加上边框
      title: "模块导入",
      maxmin: false,
      shadeClose: true, //点击遮罩关闭层
      area : ['600px' , '300px'],
      content: url,
	  success: function(layero, index){arrLayerIndex.push(index);},
	  cancel: function(index, layero){arrLayerIndex.pop();},
	  end: function (index) { arrLayerIndex.pop(); document.location.assign(document.location.href); }
    });
};
function Model_Copy(modelid)
{
    var url = "ModelCopy.aspx?modelid=" + modelid;
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
function Model_Copy1(modelid,title,code,auto)
{
	
	var html='<div style="margin:15px;line-height:30px;">';
	html+='<input name="copyname" type="text" id="copyname" style="padding-left:5px;border:1px solid #ccc;height:20px;line-height:20px;font-size:13px;color:#333;width:230px;" value="' + title +' - 复件" /><br>';
	if(auto=="true")
	{
	    html += '<input type="radio" name="copyopt" id="copyopt1" value="1" onclick="$(\'#copycode\').hide();" checked="checked" /><label for="copyopt1" style="display:inline-block;margin-left:6px;">沿用原结构</label><br>';
	    html += '<input type="radio" name="copyopt" id="copyopt2" value="2" onclick="$(\'#copycode\').show();" /><label for="copyopt2" style="display:inline-block;margin-left:6px;">复制新结构</label>';
		html += '&nbsp;<input name="copycode" type="text" id="copycode" style="padding-left:5px;border:1px solid #ccc;height:20px;line-height:20px;font-size:13px;color:#333;display:none;" value="' + (code.length > 0 ? code + '_New' : '') + '" /><br>';
	}
	html+='</div>';
	layer.open({
		type: 1, //此处以iframe举例
		title: '复制模块', // + "窗口"
		//skin: 'layui-layer-molv' //加上边框 layui-layer-rim  layui-layer-lan layui-layer-molv layer-ext-moon
		shadeClose: true, //点击遮罩关闭层
		area: ["300px",(auto=="true" ? "230" : "180") + "px"],
		btnAlign: 'l', //lcr
		moveType: 0, //拖拽模式，0或者1
		content: html,
		btn: ['复制', '取消'], //只是为了演示
		yes: function(index,layero)
		{
			var name=$("#copyname").val();
			var url="?ID=" + modelid + "&act=copy";
			if(name.length>0){url+="&name=" + name.encode();}
			if(auto=="true")
			{
				var value= $("input[name='copyopt']:checked").val();				
				if(value=="2")
				{
					var code= $("#copycode").val();
					if(code.length>0)
					{
						url+= "&code=" + code;
					}
				}
			}			
			layer.close(index);
			//alert(url);
			document.location.href=url;
		}
	});
};
function Model_Delete(modelid,title,auto)
{

	var html='<div style="margin:15px;line-height:30px;">' + title + '<br>';
	if(auto=="1")
	{
	    html += '<input type="radio" name="delopt" id="delopt1" value="1" checked="checked" /><label for="delopt1">&nbsp;保留相关表</label><br>';
	    html += '<input type="radio" name="delopt" id="delopt2" value="2" onclick="return confirm(\'确认要删除吗？\');" /><label for="delopt2" style="display:inline-block;margin-left:6px;color:#ff0000;">删除相关表(谨慎操作)</label>';
	}
	else
	{
	    html += '<input type="radio" name="delopt" id="delopt3" value="3" checked="checked" /><label for="delopt3">&nbsp;保留程序文件</label><br>';
	    html += '<input type="radio" name="delopt" id="delopt4" value="4" onclick="return confirm(\'确认要删除吗？\');" /><label for="delopt4" style="display:inline-block;margin-left:6px;color:#ff0000;">删除程序文件(谨慎操作)</label><br>';
	    html += '<input type="checkbox" name="cusopt" id="cusopt" value="5" onclick="return confirm(\'确认要删除吗？\');" /><label for="cusopt" style="display:inline-block;margin-left:6px;color:#ff0000;">删除相关表(谨慎操作)</label>';
	}
	html+='</div>';
	layer.open({
		type: 1, //此处以iframe举例
		title: '删除模块', // + "窗口"
		//skin: 'layui-layer-molv' //加上边框 layui-layer-rim  layui-layer-lan layui-layer-molv layer-ext-moon
		shadeClose: true, //点击遮罩关闭层
		area: ["320px", "250px"],
		btnAlign: 'l', //lcr
		moveType: 0, //拖拽模式，0或者1
		content: html,
		btn: ['删除', '取消'], //只是为了演示
		yes: function(index,layero)
		{

			var url="?ID=" + modelid + "&act=del";
			var deltype= $("input[name='delopt']:checked").val();
			if(deltype=="2" || deltype=="4")
			{
				url+="&deltype=" + deltype;	
			}
			var cusopt = $("input[name='cusopt']:checked").val();
			if (cusopt == "5")
			{
			    url += "&cusopt=" + cusopt;
			}
			layer.close(index);
			document.location.href=url;
		}
	});
};
function Model_Export(modelid,title)
{
	var html='<div style="margin:15px;line-height:30px;">' + title + '<br>';
	html += '<input type="radio" name="exportopt" id="exportopt1" value="1" checked="checked" /><label for="exportopt1" style="display:inline-block;margin-left:6px;">仅模块表结构</label><br>';
	html += '<input type="radio" name="exportopt" id="exportopt2" value="2" /><label for="exportopt2" style="display:inline-block;margin-left:6px;">模块表结构和数据</label><br>';
	html+='</div>';
	layer.open({
		type: 1, //此处以iframe举例
		title: '导出模块', // + "窗口"
		//skin: 'layui-layer-molv' //加上边框 layui-layer-rim  layui-layer-lan layui-layer-molv layer-ext-moon
		shadeClose: true, //点击遮罩关闭层
		area: ["300px","230px"],
		btnAlign: 'l', //lcr
		moveType: 0, //拖拽模式，0或者1
		content: html,
		btn: ['导出', '取消'], //只是为了演示
		yes: function(index,layero)
		{
			var value= $("input[name='exportopt']:checked").val();
			var url="ModelExport.aspx?ModelID=" + modelid;
			if(value=="2"){url+= "&data=true";}
			window.open(url);
			layer.close(index);
		}
	});
};
function Export1(modelid,title)
{

	  
	   layer.confirm("导出"+ title + "?",{	
	   //layer.msg(title,{	   
	   icon:1,
	   time: 0,
	   btn: ['结构和数据','仅结构','取消'],
	   title:'导出模块',
	   shadeClose: true,
	   success: function(layero, index){},
	   yes: function(index,layero){
   		//alert('yes');
		var url="ModelExport.aspx?ModelID=" + modelid + "&data=true";
		window.open(url);
		layer.close(index);
  	   },
	   btn2: function(index,layero){
		//alert('yes2');
		var url="ModelExport.aspx?ModelID=" + modelid;
		 window.open(url);
		//return false;//取消关闭
  	   },	
	   btn3:function(index,layero)
	   { 
	   		//alert('yes4'); 
			//return false;//取消关闭
	   }, 
	   cancel: function(index, layero){},//标题栏-关闭
	   end : function(index){}
	   });
};
function selectTable(value)
{
   // alert(value);
    // return;
    
        var f2 = getobj("f2");
        if (value.length == 0) {
            f2.value = "";            
            f2.removeAttribute("readOnly");
            //f2.style.display = "";
        }
        else {
            f2.value = value;
            f2.setAttribute("readOnly", 'true');
            //f2.style.display = "none";
        }
};
function getTables()
{
    var dsid = $("#f10").val();//数据源
    var modelType = $("input[name='f9']:checked").val();//模块类型
    var url = "?act=gettables&modeltype=" + modelType + (dsid.length > 5 ? "&dsid=" + dsid : "");
    $.ajax({
        type: "GET",
        async: false,
        url: url,
        dataType: "json",
        success: function (data) {
            //alert(JSON.stringify(data));
            /*
            $("#formtable").get(0).length = 1;
            $.each(data, function () {
                $("#formtable").append('<option value="' + this["value"] + '">' + this["text"] + '</option>');
            });
            */
            var html = '<li title="新建" onclick="set_filterText(this);selectTable(\'\');" value="">新建</li>\n';
            $.each(data, function () {
                html += '<li title="' + this["text"] + '" onclick="set_filterText(this);selectTable(\'' + this["value"] + '\');" value="' + this["value"] + '">' + this["text"] + '</li>\n';
            });
            $("#ultables").html(html);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
        }
    });
	 $("#formtable").get(0).value=$("#f2").val();
};
function showJoin()
{
    var auto = $("input[name='f5']:checked").val().replace("0","False").replace("1","True");
    var parvalue = $("#f7").val();
    var modelType = $("input[name='f9']:checked").val();
    if (parvalue.length > 10 && modelType == "1" && auto == "True")
    {
        $("#tr6").show();
    }
    else
    {
        $("#tr6").hide();
    }
};
function selType() {
    var modelType = $("input[name='f9']:checked").val();
    switch (modelType)
    {
       
        case "1": //模块
            $("#tr0").show();//数据源
            $("#tr1").show();//属性
            var auto = $("input[name='f5']:checked").val().replace("0", "False").replace("1", "True");
            if (auto == "True")
            {
                $("#tr2").hide();//自定义文件名
                $("#f2").attr("notnull", "true");
                $("#tr4").show();//数据表
                $("#tr5").show();//上级模块
            }
            else
            {
                $("#tr2").show();//自定义文件名
                $("#f2").attr("notnull", "false");
                $("#tr4").show();//数据表
                $("#tr5").show();//上级模块
                $("#f7").val("");
            }
            showJoin();

            $("#tr7").show();//是否基础模块
            $("#tr8").show();//基础模块
            $("#tr9").show();//网站模型
            $("#tr10").show();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "2": //菜单
            $("#tr0").hide();//数据源
            $("#tr1").hide();//属性
            $("#tr2").hide();//自定义文件名
            //$("#tr4").hide();//数据表
            $("#tr5").show();//上级模块
            $("#tr6").hide();//与上级关系
            $("#f2").attr("notnull", "false");

            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "3": //数据模块
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").show();//数据表
            $("#tr5").show();//上级模块
            $("#tr6").hide();//与上级关系

            //$("#f8_1").attr("checked", "checked");
            $("#f8_1").get(0).checked = true;//与上级关系：1 对多            
            $("#f2").attr("notnull", "true");
            $("#f7").attr("notnull", "false");//上级模块可以为空

            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "4": //报表模块
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").hide();//数据表
            $("#tr5").hide();//上级模块
            $("#tr6").hide();//与上级关系

            $("#f8_1").get(0).checked = true;//与上级关系：1 对多            
            $("#f2").attr("notnull", "false");
            $("#f7").attr("notnull", "false");//上级模块可以为空
            $("#f7").val("");

            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "5": //流水报表模块
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").show();//数据表
            $("#tr5").hide();//上级模块
            $("#tr6").hide();//与上级关系
            $("#f8_1").get(0).checked = true;//与上级关系：1 对多            
            $("#f2").attr("notnull", "true");
            $("#f7").attr("notnull", "false");//上级模块可以为空
            $("#f7").val("");

            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "6": //互动模块
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").show();//数据表
            $("#tr5").show();//上级模块
            $("#tr6").hide();//与上级关系
            //$("#f8_1").attr("checked", "checked");
            $("#f8_1").get(0).checked = true;//与上级关系：1 对多           
            $("#f2").attr("notnull", "true");
            $("#f7").attr("notnull", "true");//上级模块不能为空
            
            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "7": //只读列表
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").show();//数据表
            $("#tr5").show();//上级模块
            $("#tr6").hide();//与上级关系
            $("#f8_1").get(0).checked = true;           
            $("#f2").attr("notnull", "true");//数据表
            $("#f7").attr("notnull", "true");//上级模块不能为空

            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "10": //联合查询模块
            $("#tr0").show();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").hide();//数据表
            $("#tr5").hide();//上级模块
            $("#f2").attr("notnull", "false");//数据表
            $("#tr4").hide();//数据表
            $("#tr5").hide();//上级模块
            $("#tr6").hide();//与上级关系
            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "11": //容器模块
            $("#tr0").hide();//数据源
            $("#tr1").hide();//属性
            $("#f5_1").prop("checked", true);//自动模块-配置
            $("#tr2").hide();//自定义文件名
            $("#tr4").hide();//数据表
            $("#tr5").hide();//上级模块
            $("#f2").attr("notnull", "false"); //数据表

            $("#tr4").hide();//数据表
            $("#tr5").hide();//上级模块
            $("#tr6").hide();//与上级关系
            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").hide();//映射模块ID
            break;
        case "12"://映射模块
            $("#tr0").hide();//数据源
            $("#tr1").hide();//属性
            $("#tr2").hide();//自定义文件名
            $("#f2").attr("notnull", "false"); //数据表
            $("#tr4").hide();//数据表
            $("#tr5").show();//上级模块
            $("#tr6").hide();//与上级关系
            $("#tr7").hide();//是否基础模块
            $("#tr8").hide();//基础模块
            $("#tr9").hide();//网站模型
            $("#tr10").hide();//网站栏目
            $("#tr11").show();//映射模块
            break;
    }
    getTables();

};
function selType_v1()
{
    //var md = document.createElement("input");// getobj("f9_1");//模块
    //md.type = "radio";
    //md.checked = true;
	var md = true;// getobj("f9_3");//菜单
    var at = getobj("f5_2");//自定义
    getobj("f2").setAttribute("notnull", (md.checked || at.checked || getobj("f9_4").checked ? "false" : "true"));
    if (md) //模块 !md.checked
    {
        getobj("tr1").style.display = "";//属性
        if (!at.checked) //自定义
        {
            getobj("tr2").style.display = "none";//文件
            getobj("tr3").style.display = "none";//类名
            getobj("tr4").style.display = "";//编码
			getobj("tr5").style.display = "";//上级
			var f7=getobj("f7");
			if(f7.value.length==0)			
			{
				getobj("tr6").style.display = "none";//关系
			}
			else
			{
            	getobj("tr6").style.display = "";//关系
			}
        }
        else //自定义
        {
            getobj("tr2").style.display = "";//文件
            getobj("tr3").style.display = "none";//类名
            getobj("tr4").style.display = "none";//编码
			getobj("tr5").style.display = "none";//上级
            getobj("tr6").style.display = "none";//关系
        }
    }
    else //菜单
    {
        getobj("tr1").style.display = "none";//属性
        getobj("tr2").style.display = "none";//文件
        getobj("tr3").style.display = "none";//类名
        getobj("tr4").style.display = "none";//编码
		getobj("tr5").style.display = "none";//上级
        getobj("tr6").style.display = "none";//关系
    }
};
function select_callback()
{
    if ($("input[name='selitem']:checked").length == 0) {
        $("#btn_export").hide();
    }
    else {
        $("#btn_export").show();
    }
};
function Model_ExportConfig()
{
    var sels = $("input[name='selitem']:checked");
    if (sels.length == 0) {
        return;
    }
    var ids = "";
    sels.each(function(i,elem)
    {
        if (ids.length > 0) { ids += ","; }
        ids += elem.value;
    });
    //layer.msg(ids);
    var html = '<div style="margin:15px;line-height:30px;">';
    html += '<input type="radio" name="exportopt" id="exportopt1" value="1" checked="checked" /><label for="exportopt1" style=\"display:inline-block;margin-left:6px;\">不导出模块表结构</label><br>';
    html += '<input type="radio" name="exportopt" id="exportopt2" value="2" /><label for="exportopt2" style=\"display:inline-block;margin-left:6px;\">导出模块表结构</label><br>';
    html += '</div>';
    layer.open({
        type: 1,
        title: '导出模块',
        shadeClose: true,
        area: ["300px", "200px"],
        btnAlign: 'l', //lcr
        moveType: 0, //拖拽模式，0或者1
        content: html,
        btn: ['导出', '取消'], //只是为了演示
        yes: function (index, layero) {
            var value = $("input[name='exportopt']:checked").val();
            var url = "ExportConfig.aspx?modelids=" + ids;
            if (value == "2") { url += "&columns=true"; }
            window.open(url);
            layer.close(index);
        }
    });


};

</script>
<uc1:GroupMenu runat="server" ID="GroupMenu" />

<div class="nav">您当前位置：<a href="Default.aspx">首页</a> -> 模块管理

<%if(Action.Value==""){%>

<a id="btn_add" class="button" href="<%=edt.getAddURL()%>"><span><i class="add">添加</i></span></a>
<a id="btn_excreate" class="button" href="javascript:;" onclick="ExcelToModel();"><span style="letter-spacing:0;"><i class="import">Excel生成模块</i></span></a>
<a id="btn_import" class="button" href="javascript:;" onclick="Model_import();" _href="ModelImport.aspx"><span><i class="set">导入</i></span></a>
<a id="btn_export" class="button" href="javascript:;" onclick="Model_ExportConfig();" style="display:none;"><span><i class="save">导出</i></span></a>

<%}%>
</div>

<%if (eRegisterInfo.Base == 0 && eRegisterInfo.Loaded)
  { %>
    <div style="margin:6px;line-height:25px;font-size:13px;">
<div class="tips" style="margin-bottom:6px;"><b>未授权提示</b><br><a href="http://frame.eketeam.com/getSerialNumber.html" style="color:#ff0000;" target="_blank">申请临时授权</a>,享更多功能。</div>
   </div>
 <%} %>

<%
if(Action.Value=="edit" || Action.Value=="add")
{
%>

<div style="margin:10px;">
	<form name="frmaddoredit" id="frmaddoredit" method="post" action="<%=edt.getSaveURL()%>">
	<input name="id" type="hidden" id="id" value="<%=edt.ID%>">
    <input name="act" type="hidden" id="act" value="save">
    <input type="hidden" name="f9aa" value="1" />
	<input type="hidden" id="fromurl" name="fromurl" value="<%=edt.FromURL%>">
<table width="100%" border="0" cellspacing="0" cellpadding="0" class="eDataView">
<colgroup>
<col width="126" />
<col />
</colgroup>
      <tr>
        <td class="title"><ins>*</ins>模块名称：</td>
        <td class="content"><span class="eform"><ev:eFormControl ID="f1" FieldName="模块名称" field="MC" notnull="true" Attributes="class=&quot;text&quot; style=&quot;width:180px;&quot;" runat="server" /></span></td>
      </tr>
    <tr>
        <td class="title">标签：</td>
        <td class="content"><span class="eform"><ev:eFormControl ID="f11" ControlType="checkbox" FieldName="标签" field="LabelIDS" BindObject="a_eke_sysLabels" BindValue="LableID" BindText="MC" BindCondition="delTag=0 and Type='1'" BindOrderBy="px,addtime" notnull="false" runat="server" /></span></td>
      </tr>

<%if(1==1){ %>
     <tr>
	    <td class="title">模块类型：</td>
	    <td class="content"><span class="eform"><ev:eFormControl ID="f9" FieldName="模块类型" controltype="radio" field="Type" Attributes="onclick=&quot;selType();&quot;" defaultvalue="1" runat="server" /></span></td>
	    </tr>
<%} %>
         <tr id="tr0">
          <td class="title">数据源：</td>
          <td class="content"><span class="eform">
		  <ev:eFormControl ID="f10" FieldName="数据源" ControlType="select" Field="DataSourceID" FieldType="uniqueidentifier" BindObject="a_eke_sysDataSources" BindValue="DataSourceID" BindText="MC" BindCondition="delTag=0" Attributes="onchange=&quot;getTables();&quot;" Options="[{text:主库,value:NULL}]" DefaultValue="NULL" BindOrderBy="addTime desc" runat="server" />
		   </span></td>
        </tr>
	  <tr id="tr1" style="<%=(edt.Fields["Type"].ToString()=="1" ? "" : "display:none;") %>">
	    <td class="title">属性：</td>
	    <td class="content"><span class="eform"><ev:eFormControl ID="f5" controltype="radio" field="Auto" Attributes="onclick=&quot;selType();&quot;" Options="[{text:配置,value:1},{text:自定义,value:0}]" defaultvalue="1" runat="server" /></span></td>
	    </tr>
	  <tr id="tr2" style="<%=(edt.Fields["Type"].ToString()=="1" && edt.Fields["Auto"].ToString().Replace("0","False")=="False" ? "" : "display:none;") %>">
	    <td class="title">程序文件：</td>
	    <td class="content"><span class="eform">PC端：<ev:eFormControl ID="f4" field="AspxFile" defaultvalue="" Attributes="class=&quot;text&quot; style=&quot;width:180px;&quot;"  runat="server" />
		移动端：<ev:eFormControl ID="f41" field="mAspxFile" defaultvalue="" Attributes="class=&quot;text&quot; style=&quot;width:180px;&quot;" Unit="如果没有移动端，则执行PC端" runat="server" />
		</span></td>
	  </tr>
	   <tr id="tr3" style="<%=(edt.Fields["Type"].ToString()=="1" && edt.Fields["Auto"].ToString().Replace("1","True")=="True" ? "display:none;" : "display:none;") %>">
	    <td class="title">生成类名：</td>
	    <td class="content"><span class="eform"><ev:eFormControl ID="f6" field="ClassName" defaultvalue="" Attributes="class=&quot;text&quot; style=&quot;width:180px;&quot;" runat="server" /></span> 如：/System/CProducts</td>
	  </tr>
	  <tr id="tr4" style="<%=(edt.Fields["Type"].ToString()=="1" && (edt.Fields["Auto"].ToString().Replace("1","True")=="True"||edt.Fields["Auto"].ToString().Replace("0","False")=="False") ? "" : "display:none;") %>">
	     <td class="title">数据表：</td>
	     <td class="content"><span class="eform">
            
             <select id="formtable" name="formtable" onchange="selectTable(this.value);" style="display:none;">
	       <option value="">新建</option>
		     <optgroup label="数据表">
		  
		     </optgroup>
	       </select>
             <span class="filterSelect" onclick="show_filterText(this);" style="width:200px;min-width:200px; z-index: 1;" title="<%= f2.Value.ToString().Length == 0 ? "新建" : f2.Value%>">
        <span title="<%= f2.Value.ToString().Length == 0 ? "新建" : f2.Value%>"><%= f2.Value.ToString().Length == 0 ? "新建" : f2.Value%></span>
        <input name="fromtablenew" type="hidden" value="<%= f2.Value%>">
        <div style="width: 200px; display: none;">
<input type="text" onkeyup="filterKeyUp(this);" placeholder="输入筛选..." style="width: 100%;">
<ul id="ultables">
	 <asp:Literal id="LitTable" runat="server" />
</ul>
</div>
    </span>

 

	     <input type="text" class="text" name="f2" id="f2" value="<%=f2.Value%>"  fieldname="编码" notnull="<%=((edt.Fields["Type"].ToString()=="1" || edt.Fields["Type"].ToString()=="3" || edt.Fields["Type"].ToString()=="5") && edt.Fields["Type"].ToString().Length>0  ? "true" : "false") %>" autocomplete="off"></span></td>
	     </tr>
		 <tr id="tr5" style="<%=(edt.Fields["Type"].ToString()=="1" && edt.Fields["Auto"].ToString().Replace("1","True")=="True" ? "" : "display:none;") %>">
	     <td class="title">上级模块：</td>
	     <td class="content">
		 <span class="eform">
		 <select id="f7a" name="f7a" fieldname="上级模块" onchange="showJoin();" style="display:none;">
		  <asp:Literal id="LitParenta" runat="server" />
		 </select>
              </span>

             <span class="filterSelect" onclick="show_filterText(this);" style="width:200px;min-width:200px;z-index: 1;" title="<%=f7.Text%>">
        <span title="<%=f7.Text%>"><%= f7.Text.Length == 0 ? "请选择" : f7.Text%></span>
        <input id="f7" name="f7" type="hidden" value="<%=f7.Value%>">
        <input name="f7_text" type="hidden" value="<%=f7.Text%>">
        <div style="width: 260px; display: none;">
<input type="text" onkeyup="filterKeyUp(this);" placeholder="输入筛选..." style="width: 100%;">
<ul>
	<asp:Literal id="LitParent" runat="server" />
</ul>
</div>
    </span>




		

		 <!-- <ev:eFormControl ID="f7a" controltype="select" field="ParentID" FieldType="uniqueidentifier" Attributes="onchange=&quot;selType();&quot;" BindObject="a_eke_sysModels" BindValue="ModelID" BindText="MC" BindCondition="delTag=0 and Type=1" BindOrderBy="ModelID" DefaultValue="0" runa3t="server" /> -->
		 </td>
	     </tr>
 		<tr id="tr6" style="<%=(edt.Fields["Type"].ToString()=="1" && edt.Fields["Auto"].ToString().Replace("1","True")=="True" && edt.Fields["ParentID"].ToString().Length>0 ? "" : "display:none;") %>">
              <td class="title">与上级关系：</td>             
              <td class="content"><span class="eform"><ev:eFormControl ID="f8" Name="f8" FieldName="与上级关系" controltype="radio" field="JoinMore" Options="[{text:一对多,value:1},{text:一对一,value:0}]" defaultvalue="1" runat="server" /></span></td>
       </tr>

        <tr id="tr7">
	    <td class="title">是否基础模块：</td>
	    <td class="content"><span class="eform"><ev:eFormControl ID="f12" Name="f12" FieldName="是否基础模块" controltype="radio" field="BaseModel" Options="[{text:是,value:1},{text:否,value:0}]" defaultvalue="0" runat="server" /></span></td>
	    </tr>

     <tr id="tr8">
	    <td class="title">基础模块：</td>
	    <td class="content"><span class="eform">
            		  <ev:eFormControl ID="f13" Name="f13" FieldName="基础模块" ControlType="select" Field="BaseModelID" FieldType="uniqueidentifier" BindObject="a_eke_sysModels" BindValue="ModelID" BindText="MC" BindCondition="baseModel=1 and delTag=0" DefaultValue="NULL" BindOrderBy="addTime desc" runat="server" />
            </span></td>
	    </tr>

    <tr  id="tr9">
          <td class="title">网站模型：</td>
         <td class="content"><span class="eform">
            		  <ev:eFormControl ID="f14" Name="f14" FieldName="网站模型" ControlType="radio" Field="WebModel" FieldType="int" Options="[{text:是,value:1},{text:否,value:0}]" NotNull="true" defaultvalue="0" runat="server" />
            </span></td>
    </tr>
     <tr id="tr10">
          <td class="title">网站栏目：</td>
         <td class="content"><span class="eform">
            		  <ev:eFormControl ID="f15" Name="f15" FieldName="网站栏目" ControlType="radio" Field="ColumnType" FieldType="bit" Options="[{text:是,value:1},{text:否,value:0}]" NotNull="true" defaultvalue="0" runat="server" />
            </span></td>
    </tr>
     <tr id="tr11">
          <td class="title">映射模块：</td>
         <td class="content"><span class="eform">
            		  <ev:eFormControl ID="f16" Name="f16" FieldName="映射模块ID" ControlType="text" Field="MappingModelID" Width="260" FieldType="uniqueidentifier" NotNull="false" Unit="复制要映射模块的ID（可以是子模块）" runat="server" />
            </span></td>
    </tr>
	   <tr>
	     <td class="title">说明：</td>
	     <td class="content"><span class="eform"><ev:eFormControl ID="f3" controltype="textarea" FieldName="说明" field="SM" notnull="false" htmlTag="true" runat="server" /></span></td>
	     </tr>
 		<tr>		
		<td colspan="2" class="title"  style="text-align:left;padding-left:100px;padding-top:10px;padding-bottom:10px;">		
		<a class="button" href="javascript:;" onclick="if(frmaddoredit.onsubmit()!=false){frmaddoredit.submit();}"><span><i class="save">保存</i></span></a>
		<a class="button" href="javascript:;" style="margin-left:30px;" onclick="history.back();"><span><i class="back">返回</i></span></a>
		</td>
	   </tr>	 
    </table>
    </form>
</div>
<%}else{%>

<div style="margin:6px;">
<dl id="eSearchBox" class="ePanel">
<dt><h1 onclick="showPanel(this);" class="search"><a href="javascript:;" class="cur" onfocus="this.blur();"></a>搜索</h1></dt>
<dd style="display:none;">
<asp:PlaceHolder ID="eSearchControlGroup" runat="server">
<form id="frmsearch" name="frmsearch" method="post" onsubmit="return goSearch(this);" action="<%=elist.getSearchURL()%>">
<table width="100%" border="0" cellpadding="0" cellspacing="0" class="eDataView">
<colgroup>
<col width="120" />
<col />
</colgroup>
<tr>
<td class="title">模块名称：</td>
<td class="content"><span class="eform"><ev:eSearchControl ID="s1" Name="s1" ControlType="text" Field="MC" Operator="like" FieldName="模块名称" DataType="string" Width="300px" runat="server" /></span></td>
<td class="title">模块编号：</td>
<td class="content"><span class="eform"><ev:eSearchControl ID="s4" Name="s4" ControlType="text" Field="ModelID" Operator="like" FieldName="模块编号" DataType="string" Width="300px" runat="server" /></span></td>
</tr>
<tr>
<td class="title">模块类型：</td>
 <td class="content"><span class="eform"><ev:eSearchControl ID="s5" Name="s5" ControlType="select" Field="Type" Operator="=" FieldName="模块类型" Options="[{text:模块,value:1},{text:数据选择模块,value:3},{text:图表模块,value:4},{text:流水报表模块,value:5},{text:联合查询模块,value:10},{text:容器模块,value:11},{text:未用,value:12}]" DataType="string" Width="300px" runat="server" /></span></td>
<td class="title">应用名称：</td>
<td class="content"><span class="eform"><ev:eSearchControl ID="s2" Name="s2" ControlType="radio" Field="ModelID" Operator="custom" FieldName="应用名称" BindObject="a_eke_sysApplications" BindValue="ApplicationID" BindText="MC" BindCondition="deltag=0" BindOrderBy="addTime" Custom="ModelID in (select ModelID from a_eke_sysApplicationItems where ApplicationID='{querystring:s2}' and delTag=0)" DataType="string" runat="server" /></span></td>
</tr>
<tr>
<td class="title">标签：</td>
<td class="content" colspan="3"><span class="eform"><ev:eSearchControl ID="s3" Name="s3" ControlType="checkbox" Field="LabelIDS" Operator="like" FieldName="标签" BindObject="a_eke_sysLabels" BindValue="LableID" BindText="MC" BindCondition="deltag=0 and Type='1'" BindOrderBy="px,addTime" DataType="string" runat="server" /></span></td>
</tr>
<tr>
<td colspan="4" class="title" style="text-align:left;padding-left:125px;"><a class="button" href="javascript:;" onclick="if(frmsearch.onsubmit()!=false){frmsearch.submit();}"><span><i class="search">搜索</i></span></a></td>
</tr>
</table>
</form>
</asp:PlaceHolder>
</dd>
</dl>
<%}%>


<asp:Repeater id="Rep" runat="server">
<headertemplate>
<%#
"<table id=\"eDataTable\" class=\"eDataTable\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" width=\"100%\" style=\"min-width:1400px;\">\r\n"+
"<thead>\r\n"+
  "<tr bgcolor=\"#f2f2f2\">\r\n"+
  "<td width=\"35\"><input type=\"checkbox\" name=\"selallitem\" value=\"0\" onclick=\"selectAllItems(this);\"></td>\r\n"+
  	"<td width=\"260\">编号</td>\r\n"+
	"<td width=\"150\">模块名称</td>\r\n"+
    "<td width=\"80\">模块类型</td>\r\n"+
	"<td width=\"80\">属性</td>\r\n"+    
    "<td>数据表</td>\r\n"+
    "<td>程序文件</td>\r\n"+
	"<td>程序文件(M)</td>\r\n"+
    "<td>标签</td>\r\n"+
	"<td>说明</td>\r\n"+
	"<td width=\"100\">添加时间</td>\r\n"+

	"<td width=\"150\">操作</td>\r\n"+
  "</tr>\r\n"+
"</thead>\r\n" %></headertemplate><itemtemplate><%# "<tr erowid=\"" + Eval("ModelID").ToString() + "\" " + ((Container.ItemIndex+1) % 2 == 0 ? " class=\"alternating\" eclass=\"alternating\"" : " eclass=\"\"") + ">\r\n"+
    "<td height=\"32\"><input type=\"checkbox\" name=\"selitem\" value=\"" + Eval("ModelID").ToString() + "\"></td>\r\n"+
    "<td><a class=\"copy\" href=\"javascript:;\" data-clipboard-action=\"copy\" data-clipboard-text=\"" + Eval("ModelID") + "\"></a>"+ Eval("ModelID") + "</td>\r\n"+
	"<td>" + (Eval("Type").ToString() == "12" || Eval("Type").ToString() == "11" || Eval("Type").ToString() == "10" || Eval("Type").ToString() == "1" || Eval("Type").ToString() == "3" || Eval("Type").ToString() == "4" || Eval("Type").ToString() == "5" || Eval("Type").ToString() == "6" ? "<a href=\"ModelItems.aspx?ModelID=" + Eval("ModelID") + "\">"+ Eval("MC") + "</a>" : Eval("MC"))  + "</td>\r\n"+
    "<td>"+ eBase.getModelTypeText( Eval("Type").ToString()) + "</td>\r\n" +
	"<td>"+ Eval("Auto").ToString().Replace("0","False").Replace("1","True").Replace("True","配置").Replace("False","自定义") + "</td>\r\n"+   
    "<td>"+ Eval("Code") + "</td>\r\n"+
	"<td>"+ Eval("AspxFile") + "</td>\r\n"+
	"<td>"+ Eval("mAspxFile") + "</td>\r\n"+
    "<td>"+ getLabels(Eval("LabelIDS").ToString()) + "</td>\r\n"+
    "<td>"+ Eval("SM") + "</td>\r\n"+
	"<td>"+ Eval("addTime","{0:yyyy-MM-dd}") + "</td>\r\n"+
	"<td>"+
	"<a _href=\""+ edt.getActionURL("copy",Eval("ModelID").ToString())  +"\" style=\"\" _onclick=\"javascript:return confirm('确认要复制吗？');\"\" href=\"javascript:;\" onclick2=\"Model_Copy('" + Eval("ModelID").ToString() + "','" +  Eval("MC").ToString() + "','" +  Eval("Code").ToString() + "','" + Eval("Auto").ToString().ToLower() + "');\" onclick=\"Model_Copy('" + Eval("ModelID").ToString() + "');\">复制</a>"+
	"<a href=\"" + edt.getActionURL("edit",Eval("ModelID").ToString())  + "\">修改</a>"+
	"<a _href=\""+ edt.getActionURL("del",Eval("ModelID").ToString()) +"\" _onclick=\"javascript:return confirm('确认要删除吗？');\" href=\"javascript:;\" onclick=\"Model_Delete('" + Eval("ModelID").ToString() + "','" +  Eval("MC").ToString() + "','" + Eval("Auto").ToString().ToLower().Replace("true","1") + "');\">删除</a>"+
        (Eval("Auto").ToString() == "True" || 1==1 ? "<a href=\"javascript:;\" onclick=\"Model_Export('" + Eval("ModelID").ToString() + "','" +  Eval("MC").ToString() + "');\" _href=\"ModelExport.aspx?ModelID=" + Eval("ModelID").ToString()  + "\" _target=\"_blank\">导出</a>" : "") +
	"</td>\r\n"+
"</tr>\r\n"%></itemtemplate>
<footertemplate><%#"</table>\r\n"%></footertemplate>
</asp:Repeater>
</div>
<div style="margin:6px;"><ev:ePageControl ID="ePageControl1" PageSize="20" PageNum="9" runat="server" /></div>

</asp:Content>