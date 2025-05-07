//报表
function addReportItem(obj, type) {
    var _reload = parseBool(Attribute(obj, "reload"));
    var url = "?act=additem&ReportID=" + ReportID;
    url += "&type=" + type;
    url += "&t=" + now();
    showloading();
    $.ajax({
        type: "GET",
        async: true,
        url: url,
        dataType: "json",
        success: function (data) {
            hideloading();
            skipLogin(data);
            //if(!_reload){return;}
            loadData();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            hideloading();
        }
    });
};
function setReportItem(obj, ReportItemID, Item) {
    var _reload = parseBool(Attribute(obj, "reload"));
    //if (obj.getAttribute("oldvalue") == obj.value) { return; }
    var value = getValue(obj);
    if (value == "error") { return; }
    showloading();
    var url = "?act=setitem&ReportID=" + ReportID + "&ReportItemID=" + ReportItemID + "&item=" + Item + "&t=" + now();
    $.ajax({
        type: "post",
        async: true,
        data: { value: value },
        url: url,
        dataType: "json",
        success: function (data) {            
            hideloading();
            skipLogin(data);
            if (Item == "finsh") {
                obj.style.color = obj.value == 1 ? "#33cc00" : "#FF0000";
            }
            if (!_reload) { return; }
            loadData();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            hideloading();
        }
    });
};
function delReportItem(obj, ReportItemID) {
    var _reload = parseBool(Attribute(obj, "reload"));
    if (!confirm('确认要删除吗？')) { return; }
    showloading();
    var url = "?act=delitem&ReportID=" + ReportID + "&ReportItemID=" + ReportItemID + "&t=" + now();
    $.ajax({
        type: "GET", async: true,
        url: url,
        dataType: "json",
        success: function (data) {
            hideloading();
            skipLogin(data);
            if (obj.parentNode.tagName == "TD") {
                $(getParent(obj, "tr")).remove();
            }
            else {
                $(getParent(obj, "dl")).remove();
            }

            //if(!_reload){return;}
            //loadData();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            hideloading();
        }
    });
};
function setReport3(obj, Item) {
    alert(ReportID);
    var _reload = parseBool(Attribute(obj, "reload"));
    //if (obj.getAttribute("oldvalue") == obj.value) { return; }
    var value = getValue(obj);
    if (value == "error") { return; }
    showloading();
    var url = "?act=setreport&ReportID=" + ReportID + "&item=" + Item + "&t=" + now();
    $.ajax({
        type: "post",
        data: { value: value },
        async: true,
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
function getLoginUrl()
{
	var url=top.document.location.href;
	var arr=url.split("/");
	url="Login.aspx?fromURL="+ encodeURIComponent(arr[arr.length-1]);
	return url;
};
function skipLogin(data)
{
	if(typeof(data.message)!=="undefined")
	{
		layer.msg(data.message);
	}
	if(typeof(data.errcode)!=="undefined")
	{
	    if (data.errcode == "101") {
	        setTimeout(function () {
	            top.document.location.assign(getLoginUrl());
	        }, 1500);
	    }
	}
};
function call_filterEvent(obj)
{
	var span=obj.parentNode.parentNode.parentNode;
	var ipt=$(span).find("input[type='hidden']:eq(0)")[0];
	ipt.blur();
	ipt.onblur();	
};
function reportdlg(obj,modelid,reportid)
{
	var url ="ModelItems_ReportItems.aspx?modelid=" + modelid + "&reportid=" + reportid;
    layer.open({
      type: 2,
      title: "报表数据选项",
      maxmin: true,
      shadeClose: true, //点击遮罩关闭层
      area : ["80%" , "80%"],
	  // content: [url,'no'], 
	  content: url,
	  success: function(layero, index){},
	  cancel: function(index, layero){},
	  end : function(index){}
    });	
};
function setreadonly()
{
	
	$("a:contains('复制')").hide();
	$("a:contains('编辑')").hide();
	$("a:contains('删除')").hide();
	$("a:contains('运行')").hide();
		
	$("a[title*='添']:not([title*='列'])").hide();
	$("a[title*='删']:not([title*='列'])").hide();
	
	$("input[type='checkbox']:not([name='chkdisabled']):not([id='editdatatable'])").attr("disabled",true); 
	$("input[type='radio']").attr("disabled",true); 
	$("select").attr("disabled",true); 
	$("input[type='text']").attr("disabled",true); 
	$("textarea").attr("disabled",true); 
	$("input[type='button']").attr("disabled",true); 
	$("img[src*='jsonedit']").hide();
	
	if($("#printHTML").length>0)
	{
		//KE.toolbar.disable("printHTML", []);
		//KE.readonly("printHTML");
		//KE.g["printHTML"].newTextarea.disabled = true;
	}
};
function cancelreadonly()
{
	$("a:contains('复制')").show();
	$("a:contains('编辑')").show();
	$("a:contains('删除')").show();
	$("a:contains('运行')").show();
	
	$("a[title*='添']:not([title*='列'])").show();
	$("a[title*='删']:not([title*='列'])").show();
	$("input[type='checkbox']:not([name='chkdisabled']):not([id='editdatatable'])").attr("disabled",false); 
	$("input[type='radio']").attr("disabled",false); 
	$("select").attr("disabled",false); 
	$("input[type='text']").attr("disabled",false); 	
	$("textarea").attr("disabled",false); 
	$("input[type='button']").attr("disabled",false); 	
	$("img[src*='jsonedit']").show();
	if($("#printHTML").length>0)
	{
		//KE.toolbar.disable("printHTML", []);
		//KE.readonly("printHTML",false);
		//KE.g["printHTML"].newTextarea.disabled = false;
	}
};
function chkdisabled_click(obj)
{
	var value="";
	if(obj.checked)
	{
		setreadonly();
		value="true";
	}
	else
	{
		cancelreadonly();
		value="false";
	}
	var url="?modelid=" + document.location.href.getquerystring("modelid") + "&act=readonly&value=" + value;
	$.ajax({
		   type: "get",
			url: url,
			dataType: "json",
			success: function(data)
			{
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
			}
		});
};
//模块权限
function switch_checkupstate(obj)
{
	if($('#xmlcode').is(':visible'))
	{
		$('#xmlcode').hide();
		$(obj).find("i").text("导出XML");
	}
	else
	{
		$('#xmlcode').show();
		$(obj).find("i").text("关闭XML");
	}	
};
function import_checkupdata()
{
	var xml=$("#xmlcode").val();
	if(xml.length<10) return;
	var url="ModelItems_CheckUp.aspx?modelid=" + ModelID+"&ajax=true";	
	showloading();
	$.ajax({
		   type: "POST",
			data:{xml:xml},
			url: url,
			dataType: "json",
			success: function(data)
			{
			    hideloading();
			    skipLogin(data);
				loadData($("#xmlcode").parents("div[dataurl]"));
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
				//hideloading();
			}
		});
};
var baseurl = document.location.protocol + "//" + document.location.host + document.location.pathname;
function getJsonString(str)
{
	var valuestr=str;
	if(valuestr.length>0)
	{

		if(valuestr.indexOf("[{text:")>-1 && valuestr.indexOf(",value:")>-1)
		{
			valuestr=valuestr.replace(/{/g,"{\"");
			valuestr=valuestr.replace(/:/g,"\":\"");
			valuestr=valuestr.replace(/,/g,"\",\"");
			valuestr=valuestr.replace(/}/g,"\"}");
			valuestr=valuestr.replace(/}\",\"{/g,"},{");
			valuestr=valuestr.replace(/\"\"/g,"\"");

		}

	}
	else
	{
		valuestr="[]";
	}
	return valuestr;
};
function select_power(chk)
{
	if(chk.checked)
	{
		var html='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';		
		html+='<td><input type="text" name="text" value="' + $(chk).attr("text") + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		html+='<td><input type="text" name="value" value="' + chk.value + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		
		var len= $("#JsonTable tbody > tr").length + 1;
		html+='<td style="cursor:move;">' + len + '</td>';
		html+='</tr>';
		$("#JsonTable tbody").append(html);	
		var	tb=new eDataTable("JsonTable",1);
		tb.moveRow=function(index,nindex)
		{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
		};	
	}
	else
	{
		$('#JsonTable input[value="' + chk.value + '"]').parents("tr:first").remove();
		
		$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
		}); 
	}
};
function Json_modelPowerEdit(objid,title)
{

	var obj=$("#" + objid);	
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();	
	var valuestr=getJsonString(obj.val());
	var values=valuestr.toJson();	
	var width= 240;
	var	html='<div style="margin:6px 0px 0px 10px;">常用权限：';	
	var powerCodes=",";
	var customactionstr=obj.attr("customaction");
	var cusjson = customactionstr.toJson();	
	//alert(cusjson.length);
	
	
	values.foreach(function (e)
    {
		powerCodes+=values[e].value.toLowerCase() + ",";		
    });	
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_list" name="power_list" type="checkbox" text="列表" value="list" ' + (powerCodes.indexOf(",list,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_list" style="display:inline-block;margin-left:4px;">列表</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_view" name="power_view" type="checkbox" text="详细" value="view" ' + (powerCodes.indexOf(",view,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_view" style="display:inline-block;margin-left:4px;">详细</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_add" name="power_add" type="checkbox" text="添加" value="add" ' + (powerCodes.indexOf(",add,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_add" style="display:inline-block;margin-left:4px;">添加</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_edit" name="power_edit" type="checkbox" text="编辑" value="edit" ' + (powerCodes.indexOf(",edit,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_edit" style="display:inline-block;margin-left:4px;">编辑</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_del" name="power_del" type="checkbox" text="删除" value="del" ' + (powerCodes.indexOf(",del,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_del" style="display:inline-block;margin-left:4px;">删除</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_copy" name="power_copy" text="复制" type="checkbox" value="copy" ' + (powerCodes.indexOf(",copy,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_copy" style="display:inline-block;margin-left:4px;">复制</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_change" name="power_change" text="变更" type="checkbox" value="change" ' + (powerCodes.indexOf(",change,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_change" style="display:inline-block;margin-left:4px;">变更</label></span>';	
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_print" name="power_print" type="checkbox" text="打印" value="print" ' + (powerCodes.indexOf(",print,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_print" style="display:inline-block;margin-left:4px;">打印</label></span><br>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;margin-left:60px;"><input id="power_import" name="power_import" type="checkbox" text="导入" value="import" ' + (powerCodes.indexOf(",import,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_import" style="display:inline-block;margin-left:4px;">导入</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_export" name="power_export" type="checkbox" text="导出" value="export" ' + (powerCodes.indexOf(",export,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_export" style="display:inline-block;margin-left:4px;">导出</label></span>';
	html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_pivot" name="power_pivot" type="checkbox" text="透视" value="pivot" ' + (powerCodes.indexOf(",pivot,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_pivot" style="display:inline-block;margin-left:4px;">透视</label></span>';
	html+='<span style="display:inline-block;width:80px;line-height:25px;"><input id="power_version" name="power_version" type="checkbox" text="数据版本" value="version" ' + (powerCodes.indexOf(",version,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_version" style="display:inline-block;margin-left:4px;">数据版本</label></span>';
	html+='<span style="display:inline-block;width:80px;line-height:25px;margin-left:10px;"><input id="power_recycle" name="power_recycle" type="checkbox" text="回收站" value="recycle" ' + (powerCodes.indexOf(",recycle,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_recycle" style="display:inline-block;margin-left:4px;">回收站</label></span>';
	html+='<span style="display:inline-block;width:80px;line-height:25px;margin-left:0px;"><input id="power_recycle" name="power_viewmore" type="checkbox" text="数据对比" value="viewmore" ' + (powerCodes.indexOf(",viewmore,")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_viewmore" style="display:inline-block;margin-left:4px;">数据对比</label></span>';
	
	cusjson.foreach(function (e)
    {
		//alert(cusjson[e].text  + "::" + cusjson[e].value );
		html+='<span style="display:inline-block;width:60px;line-height:25px;"><input id="power_' + cusjson[e].value + '" name="power_' + cusjson[e].value + '" type="checkbox" text="' + cusjson[e].text + '" value="' + cusjson[e].value + '" ' + (powerCodes.indexOf("," + cusjson[e].value + ",")>-1 ? 'checked="checked"' : '') + ' onclick="select_power(this);" /><label for="power_' + cusjson[e].value + '" style="display:inline-block;margin-left:4px;">' + cusjson[e].text + '</label></span>';
	});	
	html+='</div>';

	html+='<table id="JsonTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" width="' + ((json.length*(width-10)) + 35 + 35) +  '" style="margin:8px;">';	
	html+='<colgroup>';
	html+='<col width="35" />';
	for(var i=0;i<json.length;i++)
	{
		html+='<col width="' + (width-10) + '" />';
	}
	html+='<col width="35" />';
	html+='</colgroup>';	
	html+='<thead>';
	html+='<tr>';
	html+='<td height="30" align="center"><img src="images/add.png" style="cursor:pointer;" onclick="Json_Add(\'' + objid + '\');" /></td>';
	json.foreach(function (e)
    {
		html+='<td height="30">' + json[e].text + '</td>';
	});
	html+='<td>顺序</td>';
	html+='</tr>';
	html+='</thead>';
	html+='<tbody eMove="true">';
	for(var j=0;j<values.length;j++)
	{
		html+='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';
		json.foreach(function (e)
		{
			//html+='<td height="30">' + values[j][json[e].value] + '</td>';
			html+='<td><input type="text" name="' + json[e].value + '" value="' + (values[j][json[e].value] ? values[j][json[e].value] : "") + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		});
		html+='<td style="cursor:move;">' + (j+1) + '</td>';
		html+='</tr>';
	}
	html+='</tbody>';
	html+='</table>';
	

	
	layer.open({
      type: 1,
	  title: title ? title : "模块权限",
	  scrollbar: false,
      area: [(json.length*width + 20 + 35 + 35 + 20) + 'px', '60%'],
      shadeClose: true, //点击遮罩关闭
	  content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		  var hasnull=false;
		  var _json='[';
		  var _html='';
		 $("#JsonTable tbody tr").each(function(index1,objtr){
			if(index1>0){_json+=',';}
			_json+='{';
			$(objtr).find("input").each(function(index2,input){

				if(input.value.length==0){hasnull=true;}
				if(index2>0){_json+=',';} 
				if(input.name=="text" || input.name=="action" || input.name=="Field"){_html += '<span style="display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;">' + input.value + '</span>';}
				
				_json += '"' + input.name +  '":"' + input.value.replace(/\"/g,"&quot;") + '"';
			});	
			_json+='}';
		});
		if(hasnull){alert("数据不能为空!");return;}
		_json+=']';
		if(_json.length==0){_json='';}
		obj.parent().find("span").remove();
		obj.next().after(_html);


		obj.val(_json);	
		obj.get(0).onblur();
		layer.close(index);
		//alert();
	  },
      cancel: function (index, layero) 
	  {
	  	//arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	//arrLayerIndex.push(index);
		var tb=getobj("JsonTable");
		if(tb)
		{
			tb=new eDataTable("JsonTable",1);
			tb.moveRow=function(index,nindex)
			{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
			};		
		}
      }
    });
};

//操作权限-添加
function Json_OperationPowerAdd(objid)
{
	var obj=$("#" + objid);
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();
	var html='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';
		html+='<td><input type="text" name="action" value="" style="border:0px;background-color:transparent;width:100%;" /></td>';
		html+='<td>';
		html+='引用：<input type="text" name="variable" value="" style="border:0px;background-color:#f1f1f1;width:162px;" /><br>';
		html+='字段名：<input type="text" name="field" value="" style="border:0px;background-color:#f1f1f1;width:150px;" />';
		html+='</td>';
		html+='<td>';
		var operator= "=";
		html+='<select name="operator">';
  		html+='<option value="="' + (operator=='=' ? ' selected="selected"' : '') + '>等于</option>';
		html+='<option value="<>"' + (operator=='<>' ? ' selected="selected"' : '') + '>不等于</option>';
  		html+='<option value=">"' + (operator=='>' ? ' selected="selected"' : '') + '>大于</option>';
		html+='<option value="<"' + (operator=='<' ? ' selected="selected"' : '') + '>小于</option>';
		html+='</select>';
		html+='</td>';		
		html+='<td><input type="text" name="value" value="" style="border:0px;background-color:transparent;width:100%;" /></td>';

		var len= $("#JsonTable tbody > tr").length + 1;
		html+='<td style="cursor:move;">' + len + '</td>';
		html+='</tr>';
		//alert(obj.find("tbody").length);

	//alert(json.length);
	$("#JsonTable tbody").append(html);	
	var	tb=new eDataTable("JsonTable",1);
};

//步骤条件-添加
function Json_StepConditionAdd(objid)
{
	var obj=$("#" + objid);
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();
	var html='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';
		html+='<td>';
		html+='<input type="text" name="variable" value="" style="height:25px;border:0px;background-color:transparent;back2ground-color:#f1f1f1;width:100%;" />';
		html+='</td>';
		html+='<td>';
		var operator= "=";
		html+='<select name="operator">';
  		html+='<option value="="' + (operator=='=' ? ' selected="selected"' : '') + '>等于</option>';
		html+='<option value="<>"' + (operator=='<>' ? ' selected="selected"' : '') + '>不等于</option>';
  		html+='<option value=">"' + (operator=='>' ? ' selected="selected"' : '') + '>大于</option>';
		html+='<option value="<"' + (operator=='<' ? ' selected="selected"' : '') + '>小于</option>';
		html+='</select>';
		html+='</td>';		
		html+='<td><input type="text" name="value" value="" style="height:25px;border:0px;background-color:transparent;backgro2und-color:#f1f1f1;width:100%;" /></td>';

		var len= $("#JsonTable tbody > tr").length + 1;
		html+='<td style="cursor:move;">' + len + '</td>';
		html+='</tr>';
		//alert(obj.find("tbody").length);

	//alert(json.length);
	$("#JsonTable tbody").append(html);	
	var	tb=new eDataTable("JsonTable",1);
};
//步骤条件-编辑
function Json_StepConditionEdit(objid,title)
{
	var obj=$("#" + objid);	
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();
	var valuestr=getJsonString(obj.val());
	var values=valuestr.toJson();
	
	
	var html='<table id="JsonTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" width="600" style="margin:10px;">';
	
	html+='<colgroup>';
	html+='<col width="35" />';
	html+='<col width="180" />';
	html+='<col width="60" />';
	html+='<col width="100" />';
	html+='<col width="35" />';
	html+='</colgroup>';	
	html+='<thead>';
	html+='<tr>';
	html+='<td height="30" align="center"><img src="images/add.png" style="cursor:pointer;" onclick="Json_StepConditionAdd(\'' + objid + '\');" /></td>';
	html+='<td>变量</td>';
	html+='<td>运算符</td>';
	html+='<td>固定值</td>';
	html+='<td>顺序</td>';
	html+='</tr>';
	html+='</thead>';
	html+='<tbody eMove="true">';
	for(var j=0;j<values.length;j++)
	{
		html+='<tr>';
		html+='<td height="40" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';		
		html+='<td>';
		html+='<input type="text" name="variable" value="' + values[j]["variable"] + '" style="height:25px;border:0px;background-color:transparent;back2ground-color:#f1f1f1;width:100%;" />';
		html+='</td>';
		html+='<td>';
		//html+='<input type="text" name="operator" value="' + (values[j]["operator"] ? values[j]["operator"] : "") + '" style="border:0px;background-color:transparent;width:100%;" />';
		var operator= (values[j]["operator"] ? values[j]["operator"] : "=");
		html+='<select name="operator">';
  		html+='<option value="="' + (operator=='=' ? ' selected="selected"' : '') + '>等于</option>';
		html+='<option value="<>"' + (operator=='<>' ? ' selected="selected"' : '') + '>不等于</option>';
  		html+='<option value=">"' + (operator=='>' ? ' selected="selected"' : '') + '>大于</option>';//&gt;
		html+='<option value="<"' + (operator=='<' ? ' selected="selected"' : '') + '>小于</option>';//&lt;
		html+='</select>';
		html+='</td>';		
		html+='<td><input type="text" name="value" value="' + values[j]["value"] + '" style="height:25px;border:0px;background-color:transparent;bac2kground-color:#f1f1f1;width:100%;" /></td>';
		html+='<td style="cursor:move;">' + (j+1) + '</td>';
		html+='</tr>';
	}
	html+='</tbody>';
	html+='</table>';

	layer.open({
      type: 1,
	  title: title ? title : "选项",
	  scrollbar: false,
      area: ['630px', '60%'],
      shadeClose: true, //点击遮罩关闭
	  content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		  var hasnull=false;
		  var _json='[';
		  var _html='';
		 $("#JsonTable tbody tr").each(function(index1,objtr){
			if(index1>0){_json+=',';}
			_json+='{';
			$(objtr).find("input,select").each(function(index2,input){
				if(input.value.length==0)
				{
					if(input.name=="variable" || input.name=="field")
					{
						if($(objtr).find("[name='variable']").val().length==0 && $(objtr).find("[name='field']").val().length==0)						
						{
							hasnull=true;
						}
					}
					else
					{
						hasnull=true;
					}
				}
				if(index2>0){_json+=',';} 
				if(input.name=="action"){_html += '<span style="display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;">' + input.value + '</span>';}
				
				_json += '"' + input.name +  '":"' + input.value.replace(/\"/g,"&quot;") + '"';
			});	
			_json+='}';
		});
		if(hasnull){alert("数据不能为空!");return;}
		_json+=']';
		if(_json.length==0){_json='';}
		obj.parent().find("span").remove();
		obj.next().after(_html);


		obj.val(_json);	
		obj.get(0).onblur();
		layer.close(index);
		//alert();
	  },
      cancel: function (index, layero) 
	  {
	  	//arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	//arrLayerIndex.push(index);
		var tb=getobj("JsonTable");
		if(tb)
		{
			tb=new eDataTable("JsonTable",1);
			tb.moveRow=function(index,nindex)
			{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
			};		
		}
      }
    });

};

//操作权限-编辑
function Json_OperationPowerEdit(objid,title)
{
	var obj=$("#" + objid);	
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();		
	var valuestr=getJsonString(obj.val());
	var values=valuestr.toJson();

	
	var width= 180;

	var html='<div class="tips" style="margin:6px;">为操作菜单项增加显示的条件!(条件成立则显示,多个同一动作为and关系)</div>';
	html+='<table id="JsonTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" width="600" style="margin:10px;">';
	
	html+='<colgroup>';
	html+='<col width="35" />';
	html+='<col width="80" />';
	html+='<col width="180" />';
	html+='<col width="60" />';
	html+='<col width="100" />';
	html+='<col width="35" />';
	html+='</colgroup>';	
	html+='<thead>';
	html+='<tr>';
	html+='<td height="30" align="center"><img src="images/add.png" style="cursor:pointer;" onclick="Json_OperationPowerAdd(\'' + objid + '\');" /></td>';
	html+='<td>动作编码</td>';
	html+='<td>变量</td>';
	html+='<td>运算符</td>';
	html+='<td>固定值</td>';
	html+='<td>顺序</td>';
	html+='</tr>';
	html+='</thead>';
	html+='<tbody eMove="true">';
	for(var j=0;j<values.length;j++)
	{
		html+='<tr>';
		html+='<td height="40" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';		
		html+='<td><input type="text" name="action" value="' + values[j]["action"] + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		html+='<td>';
		html+='引用：<input type="text" name="variable" value="' + (values[j]["variable"] ? values[j]["variable"] : "")+ '" style="border:0px;background-color:#f1f1f1;width:162px;" /><br>';
		html+='字段名：<input type="text" name="field" value="' + values[j]["field"] + '" style="border:0px;background-color:#f1f1f1;width:150px;" />';
		html+='</td>';
		html+='<td>';
		//html+='<input type="text" name="operator" value="' + (values[j]["operator"] ? values[j]["operator"] : "") + '" style="border:0px;background-color:transparent;width:100%;" />';
		var operator= (values[j]["operator"] ? values[j]["operator"] : "=");
		html+='<select name="operator">';
  		html+='<option value="="' + (operator=='=' ? ' selected="selected"' : '') + '>等于</option>';
		html+='<option value="<>"' + (operator=='<>' ? ' selected="selected"' : '') + '>不等于</option>';
  		html+='<option value=">"' + (operator=='>' ? ' selected="selected"' : '') + '>大于</option>';//&gt;
		html+='<option value="<"' + (operator=='<' ? ' selected="selected"' : '') + '>小于</option>';//&lt;
		html+='<option value="in"' + (operator=='in' ? ' selected="selected"' : '') + '>in</option>';
		html+='</select>';
		html+='</td>';		
		html+='<td><input type="text" name="value" value="' + values[j]["value"] + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		html+='<td style="cursor:move;">' + (j+1) + '</td>';
		html+='</tr>';
	}
	html+='</tbody>';
	html+='</table>';
	layer.open({
      type: 1,
	  title: title ? title : "选项",
	  scrollbar: false,
      area: ['630px', '60%'],
      shadeClose: true, //点击遮罩关闭
	  content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		  var hasnull=false;
		  var _json='[';
		  var _html='';
		 $("#JsonTable tbody tr").each(function(index1,objtr){
			if(index1>0){_json+=',';}
			_json+='{';
			$(objtr).find("input,select").each(function(index2,input){
				if(input.value.length==0)
				{
					if(input.name=="variable" || input.name=="field")
					{
						if($(objtr).find("[name='variable']").val().length==0 && $(objtr).find("[name='field']").val().length==0)						
						{
							hasnull=true;
						}
					}
					else
					{
						hasnull=true;
					}
				}
				if(index2>0){_json+=',';} 
				if(input.name=="action"){_html += '<span style="display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;">' + input.value + '</span>';}
				
				_json += '"' + input.name +  '":"' + input.value.replace(/\"/g,"&quot;") + '"';
			});	
			_json+='}';
		});
		if(hasnull){alert("数据不能为空!");return;}
		_json+=']';
		if(_json.length==0){_json='';}
		obj.parent().find("span").remove();
		obj.next().after(_html);


		obj.val(_json);	
		obj.get(0).onblur();
		layer.close(index);
		//alert();
	  },
      cancel: function (index, layero) 
	  {
	  	//arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	//arrLayerIndex.push(index);
		var tb=getobj("JsonTable");
		if(tb)
		{
			tb=new eDataTable("JsonTable",1);
			tb.moveRow=function(index,nindex)
			{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
			};		
		}
      }
    });
};



//编辑-数据表
function editdatatable(obj)
{
	var value = getValue(obj);	
	var url = "?act=setdbstate&ajax=true&modelid=" + ModelID + "&value=" + value + "&t=" + now();
	showloading();
	$.ajax({
		   type: "GET", async: true,
		   url:url,
		   dataType: "json",
		   success: function (data)
			{
		       hideloading();
		       skipLogin(data);
			}
		});	
	

	var tb=getobj("eDataTable_Columns");
	if (tb && obj.checked)
	{

		

		tb.moveRow=function(index,nindex)
		{			
			var url = "?act=movecolumn&modelid=" + ModelID + "&index=" + index + "&nindex=" + nindex + "&t=" + now();
			showloading();
			$.ajax({
				type: "GET", async: true,
				url:url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
					//loadData(div);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
			});	
		};

	}
	if(obj.checked)
	{
		$("#tablename").attr("disabled",false);
		$("#eDataTable_Columns tr").each(function(index,row){
			$(this).find("td:eq(0) a").show();
			$(this).find("td:eq(2) input").attr("disabled",false);
			$(this).find("td:eq(3) input").attr("disabled",false);
			$(this).find("td:eq(4) select").attr("disabled",false);
			$(this).find("td:eq(5) input").attr("disabled",false);
			$(this).find("td:eq(6) input").attr("disabled",false);
			$(this).find("td:eq(7) input").attr("disabled",false);
		});
		if(tb)
		{
			$("#eDataTable_Columns tbody").attr("emove","true");
			tb=new eDataTable("eDataTable_Columns",1);
			tb.moveRow=function(index,nindex)
			{			
				var url = "?act=movecolumn&modelid=" + ModelID + "&index=" + index + "&nindex=" + nindex + "&t=" + now();
				showloading();
				$.ajax({
					type: "GET", async: true,
					url:url,
					dataType: "json",
					success: function (data) 
					{
					    hideloading();
					    skipLogin(data);
						//loadData(div);
					},
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						hideloading();
					}
				});	
			};
		}
	}
	else
	{
		$("#tablename").attr("disabled",true);
		$("#eDataTable_Columns tr").each(function(index,row){
			$(this).find("td:eq(0) a").hide();	
			$(this).find("td:eq(2) input").attr("disabled",true);
			$(this).find("td:eq(3) input").attr("disabled",true);
			$(this).find("td:eq(4) select").attr("disabled",true);
			$(this).find("td:eq(5) input").attr("disabled",true);
			$(this).find("td:eq(6) input").attr("disabled",true);
			$(this).find("td:eq(7) input").attr("disabled",true);
		});
		if(tb)
		{
			$("#eDataTable_Columns tbody").attr("emove","false");
			tb=new eDataTable("eDataTable_Columns",1);
			tb.moveRow=null;
		}
	}
};
//Json编辑-添加
function Json_Add(objid)
{
	var obj=$("#" + objid);
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();
	var html='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';
		json.foreach(function (e)
		{
			html+='<td><input type="text" name="' + json[e].value + '" value="" style="border:0px;background-color:transparent;width:100%;" /></td>';
		});
		var len= $("#JsonTable tbody > tr").length + 1;
		html+='<td style="cursor:move;">' + len + '</td>';
		html+='</tr>';
		//alert(obj.find("tbody").length);

	//alert(json.length);
	$("#JsonTable tbody").append(html);	
	var	tb=new eDataTable("JsonTable",1);
};



//Json编辑-删除
function Json_Delete(obj)
{
	if(!confirm('确认要删除吗？')){return;}
	$(obj).parent().parent().remove();
	$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
		$(obj).html(1+index1);
	}); 	
};
function Json2_Add(objid)
{
	var obj=$("#" + objid);
	var jsonstr=obj.attr("jsonformat");
	var items=jsonstr.split(";");
	var html='<tr>';
	html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json2_Delete(this);" /></td>';
	for(var i=0;i<items.length;i++)
	{
		var item=items[i].split(",");
		html+='<td><input type="text" name="' + item[0] + '" value="" style="border:0px;background-color:transparent;width:100%;" /></td>';
	}
	var len= $("#JsonTable tbody > tr").length + 1;
	html+='<td style="cursor:move;">' + len + '</td>';
	html+='</tr>';
	$("#JsonTable tbody").append(html);	
	var	tb=new eDataTable("JsonTable",1);
};
function Json2_Delete(obj)
{
	if(!confirm('确认要删除吗？')){return;}
	$(obj).parent().parent().remove();
	$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
		$(obj).html(1+index1);
	}); 	
};
function Json2_Edit(objid,title)
{
	var obj=$("#" + objid);	
	var jsonstr=obj.attr("jsonformat");
	var items=jsonstr.split(";");
	var valuestr=getJsonString(obj.val());
	var values=valuestr.toJson();
	var width= 180;	
	var html='<table id="JsonTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" width="' + ((items.length*(width-10)) + 35 + 35) +  '" style="margin:10px;">';	
	html+='<colgroup>';
	html+='<col width="35" />';
	for(var i=0;i<items.length;i++)
	{
		var item=items[i].split(",");
		html+='<col width="' + (width-10) + '" />';
	}
	html+='<col width="35" />';
	html+='</colgroup>';	
	html+='<thead>';
	html+='<tr>';
	html+='<td height="30" align="center"><img src="images/add.png" style="cursor:pointer;" onclick="Json2_Add(\'' + objid + '\');" /></td>';
	for(var i=0;i<items.length;i++)
	{
		var item=items[i].split(",");
		html+='<td' + (i==110?' height="30"':'')+ '>' + item[0] + '</td>';
	}
	html+='<td>顺序</td>';
	html+='</tr>';
	html+='</thead>';
	html+='<tbody eMove="true">';
	for(var j=0;j<values.length;j++)
	{
		html+='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json2_Delete(this);" /></td>';
		for(var i=0;i<items.length;i++)
		{
			var item=items[i].split(",");
			html+='<td><input type="text" name="' + item[1] + '" value="' + (values[j][item[1]] ? values[j][item[1]] : "") + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		}
		html+='<td style="cursor:move;">' + (j+1) + '</td>';
		html+='</tr>';
	}
	html+='</tbody>';
	html+='</table>';
	
	layer.open({
      type: 1,
	  title: title ? title : "选项",
	  scrollbar: false,
      area: [(items.length*width + 20 + 35 + 35 + 20) + 'px', '60%'],
      shadeClose: true, //点击遮罩关闭
	  content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		  var hasnull=false;
		  var _json='[';

		 $("#JsonTable tbody tr").each(function(index1,objtr){
			if(index1>0){_json+=',';}
			_json+='{';
			$(objtr).find("input").each(function(index2,input){

				if(input.value.length==0){hasnull=true;}
				if(index2>0){_json+=',';} 				
				_json += '"' + input.name +  '":"' + input.value.replace(/\"/g,"&quot;") + '"';
			});	
			_json+='}';
		});
		_json+=']';
		//alert(_json);
		obj.val(_json);	
		layer.close(index);
		//alert();
	  },
      cancel: function (index, layero) 
	  {
	  	//arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	//arrLayerIndex.push(index);
		var tb=getobj("JsonTable");
		if(tb)
		{
			tb=new eDataTable("JsonTable",1);
			tb.moveRow=function(index,nindex)
			{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
			};		
		}
      }
    });
	
};


//Json编辑
function Json_Edit(objid,title)
{	
//alert(5);
	var obj=$("#" + objid);	
	var jsonstr=obj.attr("jsonformat");
	var json = jsonstr.toJson();		
	var valuestr=getJsonString(obj.val());
	var values=valuestr.toJson();
	//alert(valuestr);
	//alert(json.length);
	json.foreach(function (e)
    {
		//alert(json[e].text + "::" + json[e].value);
    });
	
	var width= 180;	
	var html='<table id="JsonTable" class="eDataTable" border="0" cellpadding="0" cellspacing="1" width="' + ((json.length*(width-10)) + 35 + 35) +  '" style="margin:10px;">';	
	html+='<colgroup>';
	html+='<col width="35" />';
	for(var i=0;i<json.length;i++)
	{
		html+='<col width="' + (width-10) + '" />';
	}
	html+='<col width="35" />';
	html+='</colgroup>';	
	html+='<thead>';
	html+='<tr>';
	html+='<td height="30" align="center"><img src="images/add.png" style="cursor:pointer;" onclick="Json_Add(\'' + objid + '\');" /></td>';
	json.foreach(function (e)
    {
		html+='<td height="30">' + json[e].text + '</td>';
	});
	html+='<td>顺序</td>';
	html+='</tr>';
	html+='</thead>';
	html+='<tbody eMove="true">';
	for(var j=0;j<values.length;j++)
	{
		html+='<tr>';
		html+='<td height="30" align="center"><img src="images/del.png" style="cursor:pointer;" onclick="Json_Delete(this);" /></td>';
		json.foreach(function (e)
		{
			//html+='<td height="30">' + values[j][json[e].value] + '</td>';
			html+='<td><input type="text" name="' + json[e].value + '" value="' + (values[j][json[e].value] ? values[j][json[e].value] : "") + '" style="border:0px;background-color:transparent;width:100%;" /></td>';
		});
		html+='<td style="cursor:move;">' + (j+1) + '</td>';
		html+='</tr>';
	}
	html+='</tbody>';
	html+='</table>';
	layer.open({
      type: 1,
	  title: title ? title : "选项",
	  scrollbar: false,
      area: [(json.length*width + 20 + 35 + 35 + 20) + 'px', '60%'],
      shadeClose: true, //点击遮罩关闭
	  content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		  var hasnull=false;
		  var _json='[';
		  var _html='';
		 $("#JsonTable tbody tr").each(function(index1,objtr){
			if(index1>0){_json+=',';}
			_json+='{';
			$(objtr).find("input").each(function(index2,input){

				if(input.value.length==0){hasnull=true;}
				if(index2>0){_json+=',';} 
				if(input.name=="text" || input.name=="action" || input.name=="Field"){_html += '<span style="display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;">' + input.value + '</span>';}
				
				_json += '"' + input.name +  '":"' + input.value.replace(/\"/g,"&quot;") + '"';
			});	
			_json+='}';
		});
		//if(hasnull){alert("数据不能为空!");return;}
		_json+=']';
		if(_json.length==0){_json='';}
		//alert(obj.next().get(0).tagName + "::" + _html + "::" + _json);
		obj.parent().find("span").remove();
		obj.next().after(_html);
		/*
		var next=obj.next();
		while(next.next()[0].tagName == "SPAN")
		{
			//alert(next.next()[0].tagName);
			next.next().remove();
		}		
		next.after(_html);
		*/


		obj.val(_json);	
		obj.get(0).onblur();
		layer.close(index);
		//alert();
	  },
      cancel: function (index, layero) 
	  {
	  	//arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	//arrLayerIndex.push(index);
		var tb=getobj("JsonTable");
		if(tb)
		{
			tb=new eDataTable("JsonTable",1);
			tb.moveRow=function(index,nindex)
			{
				$("#JsonTable tbody > tr > td:last-child").each(function(index1,obj){
					$(obj).html(1+index1);
				}); 
			};		
		}
      }
    });
	
};
//双击修改
function dblClick1(objid,title,w,h)
{
	var obj=$("#"+objid);
	if(obj.length>0)
	{
		dblClick(obj[0],title,w,h);
	}
};
function dblClick(obj,title,w,h)
{
	var width=w||'600px';
	var height=h||'360px';
	var type=obj.type;
	var value=obj.value;
	layer.open({
      type: 1,
	  title: title,
	  scrollbar: false,
      area: [width, height],
      shadeClose: true, //点击遮罩关闭
      //content: '<div style="width:100%;height:100%;overflow:hidden;padding:10px;"><textarea name="textareabody" id="textareabody" style="padding:5px;font-size:12px;line-height:20px;width:95%;height:85%;border:1px solid #ccc;">' + value + '</textarea></div>',
	  content: '<textarea name="textareabody" id="textareabody" style="margin:10px;padding:5px;font-size:12px;line-height:20px;min-width:90%;min-height:75%;border:1px solid #ccc;">' + value + '</textarea>',
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
	  	var nvalue=$("#textareabody").val();
		if(obj.tagName=="TEXTAREA")
		{
			//nvalue=nvalue.replace(/\n/g, "");
		}
		else{nvalue=nvalue.replace(/\n/g, " ");}
		obj.value=nvalue;
		obj.onblur();
		arrLayerIndex.pop();
		layer.close(index);
	  },
      cancel: function (index, layero) 
	  {
	  	arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	arrLayerIndex.push(index);
      }
    });
};
function showloading()
{
	$("#divloading").show();
};
function hideloading()
{
	$("#divloading").hide();
};
//同步列选项
function loadColumnOptions(obj,ModelConditionID)
{

	var td=$(obj).parent().prev().prev();
	var sel=td.find("select:eq(0)");
	var code=sel.val();
	if(code.length==0)
	{
		alert("请先选择条件列!");
		return;
	}


	showloading();
	var url = "?act=loadcolumnoptions&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&code=" + code + "&t=" + now();
    $.ajax({
            type: "post", 
			async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
                loadData($(obj).parents("div[dataurl]"));
            },
			error: function (XMLHttpRequest, textStatus, errorThrown) 
			{
				//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
				hideloading();
			}
    });
	
	
	
	//$("#divloading").hide();
};
//全选所有列
function selColumnAll(obj)
{
	//alert(obj.checked)
	var inputs=$("#eDataTable_Columns [name='selItem']");
	inputs.each(function (i) 
	{
		if(!$(this).attr("checked"))
		{
			//alert($(this).get(0).value);
			$(this).attr("checked", true );
			selColumn($(this).get(0),$(this).get(0).value);
		}
		//$(this).attr("checked",(obj.checked ? true : false));
		//selColumn($(this).get(0),$(this).get(0).value);
		//alert($(this).get(0).checked);
	});
	//alert(inputs.length);
};
function addColumn2(obj,code)
{
	var _reload=true;//parseBool(Attribute(obj,"reload"));		
	showloading();	
	var url="?act=addcolumn2&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",
		   async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
			//alert(data.message);
		    hideloading();
		    skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
			hideloading();
		}
	});
};
//选择取消列OK
function selColumn(obj,code)
{
	var _reload=parseBool(Attribute(obj,"reload"));		
	showloading();	
	var url="?act=selcolumn&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:(obj.checked ? "1" : "0")},
		url: url,
		dataType: "json",
		success: function(data)
		{
			//alert(data.message);
		    hideloading();
		    skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
			hideloading();
		}
	});
};
//添加备份模块
function addBakModel(obj)
{
	showloading();
	var url="?act=addbakmodel&modelid=" + ModelID  + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
		    hideloading();
		    skipLogin(data);
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
			hideloading();
		}
	});
};
function setBakModel(obj,ModelID,Item)
{
	if (obj.getAttribute("oldvalue") == obj.value) { return; }
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setbakmodel&modelid=" + ModelID + "&item=" + Item + "&t=" + now();
	 $.ajax({
            type: "POST",
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) 
			{
                //obj.setAttribute("oldvalue", obj.value);
				$(obj).attr("oldvalue",obj.value);
				$(obj).prop("oldvalue",obj.value);
				hideloading();
				skipLogin(data);
				if(!_reload){return;}
				loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
   });
};
//删除备份模块
function delBakModel(obj,ModelID)
{
	if(!confirm('确认要删除吗？')){return;}
	showloading();
	var url="?act=delbakmodel&modelid=" + ModelID + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
		    hideloading();
		    skipLogin(data);
			$(getParent(obj,"tr")).remove();
			//loadData($(".eTab dd div:eq(0)"));
			//alert(data.message);
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};


//添加报表OK
function addReport(obj)
{
	showloading();
	var url="?act=addreport&modelid=" + ModelID  + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
		    hideloading();
		    skipLogin(data);
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			//alert("error:" + XMLHttpRequest.responseText + "::" + XMLHttpRequest.status + "::" + XMLHttpRequest.readyState + "::" + XMLHttpRequest.statusText);
			hideloading();
		}
	});
};
function setReport(obj,ReportID,Item)
{
    //alert(obj);
	//if (obj.getAttribute("oldvalue") == obj.value) { return; }
	var value = getValue(obj);
	//alert(value + ":" + obj.type);
	//return;
	if (value == "error") { return; }
	
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setreport";	
	if (typeof (ModelID) == "string" && ModelID.length > 0) { url += "&modelid=" + ModelID; }
	url+= "&ReportID=" + ReportID + "&item=" + Item + "&t=" + now();
    $.ajax({
		   type: "post", 
		   async: true,
		   data:{value:value},
           url: url,
           dataType: "json",
           success: function (data) {
			    //obj.setAttribute("oldvalue", obj.value);
				if(obj.type!="checkbox")
				{
					$(obj).attr("oldvalue",obj.value);
					$(obj).prop("oldvalue",obj.value);
				}
				hideloading();
				skipLogin(data);
			   	if(!_reload){return;}
               	loadData($(obj).parents("div[dataurl]"));
           },
           error: function (XMLHttpRequest, textStatus, errorThrown) {
			   hideloading();
           }
    });
	
};
//删除报表
function delReport(obj,ReportID)
{
	if(!confirm('确认要删除吗？')){return;}
	showloading();
	var url="?act=delreport&modelid=" + ModelID  + "&ReportID=" + ReportID + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
		    hideloading();
		    skipLogin(data);
			$(getParent(obj,"dl")).remove();
			//loadData($(".eTab dd div:eq(0)"));
			//alert(data.message);
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
function setReportMC(obj) {
    $(obj).parent().parent().find("h1 span").html(obj.value);
};
//添加列OK
function addColumnN()
{
	var html='';
	html+='<div style="margin:10px;"><span style="display:inline-block;width:100px;">名称：</span><input name="columnname" type="text" id="columnname" /></div>';
	html+='<div style="margin:10px;"><span style="display:inline-block;width:100px;">编码：</span><input name="columncode" type="text" id="columncode" /></div>';
	html+='<div style="margin:10px;"><span style="display:inline-block;width:100px;">数据类型：</span>';
	html+='<select id="columntype" style="width:150px;" reload="true">';
	html+='<option value="nchar">字符(char)</option>';
	html+='<option value="nvarchar" selected="true">文本(varchar)</option>';
	html+='<option value="uniqueidentifier">GUID(uniqueidentifier)</option>';
	html+='<option value="int">数字(int)</option>';
	html+='<option value="decimal">小数(decimal)</option>';
	html+='<option value="datetime">时间(datetime)</option>';
	html+='<option value="bit">是/否(bit)</option>';
	html+='<option value="ntext">备注(text)</option>';
	html+='</select>';
	html+='</div>';
	
	layer.open({
      type: 1,
	  title: "添加列",
	  scrollbar: false,
      area: ['400px', '200px'],
      shadeClose: true, //点击遮罩关闭
      content: html,
	  btn: ['确定', '取消'],
	  yes: function(index,layero)
	  {
		var name=$("#columnname").val();
		if(name.length==0)
		{
			alert("名称不能为空!");
			$("#columnname").focus();
			return;
		}
	  	var code=$("#columncode").val();
		if(code.length==0)
		{
			alert("编码不能为空!");
			$("#columncode").focus();
			return;
		}
		var type=$("#columntype").val();
		
		var url="?act=addcolumnnew&modelid=" + ModelID  + "&t=" + now();		
		$.ajax({type: "POST",async: false,
		url: url,
		data:{name:name,code:code,type:type},
		dataType: "json",
		success: function(data)
		{
			//alert(data.message);
		    hideloading();
		    skipLogin(data);
			loadData($(".eTab dd div:eq(0)"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
		
		arrLayerIndex.pop();
		layer.close(index);
	  },
      cancel: function (index, layero) 
	  {
	  	arrLayerIndex.pop();
      },
	  success: function (layero, index)
	  {
	  	arrLayerIndex.push(index);
      }
    });
};
function addColumn()
{	
	showloading();
	var url="?act=addcolumn&modelid=" + ModelID  + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
			//alert(data.message);
		    hideloading();
		    skipLogin(data);
			loadData($(".eTab dd div:eq(0)"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//删除列OK
function delColumn(obj,code)
{
	if(!confirm('确认要删除吗？')){return;}
	showloading();
	var url="?act=delcolumn&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
		    hideloading();
		    skipLogin(data);
			$(getParent(obj,"tr")).remove();
			//loadData($(".eTab dd div:eq(0)"));
			//alert(data.message);
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//重命名列OK
function ReNameColumn(obj)
{
	if(obj.getAttribute("oldvalue")==obj.value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=renamecolumn&modelid=" + ModelID  + "&code=" + obj.getAttribute("oldvalue") + "&newcode=" + obj.value.toCode()  + "&t=" + now();
	$.ajax({type: "POST",async: false,
		url: url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",obj.value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//列备注OK
function ColumnName(obj,code,value)
{
	
	if(obj.getAttribute("oldvalue")==value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=columnname&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:value.toCode()},
		url:url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//小数位
function ColumnScale(obj,code,value)
{
	if(obj.getAttribute("oldvalue")==value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=columnscale&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:value.toCode()},
		url: url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};

//列默认值OK
function ColumnDefault(obj,code,value)
{
	if(obj.getAttribute("oldvalue")==value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=columndefault&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:value.toCode()},
		url: url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//数据类型OK
function ColumnType(obj,code,value)
{
	if(obj.getAttribute("oldvalue")==value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=columntype&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:value.toCode()},
		url: url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//数据长度OK
function ColumnLength(obj,code,value)
{
	if(obj.getAttribute("oldvalue")==value){return;}
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url="?act=columnlength&modelid=" + ModelID  + "&code=" + code + "&t=" + now();
	$.ajax({type: "POST",async: false,
		data:{value:value.toCode()},
		url: url,
		dataType: "json",
		success: function(data)
		{
			obj.setAttribute("oldvalue",value);
			//alert(data.message);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}		
	});
};
function eDataTable_event(div)
{
	//数据结构顺序//暂时取消移动-
	var tb=getobj("eDataTable_Columns");
	if(tb && $("#editdatatable").prop("checked"))
	{
		tb=new eDataTable("eDataTable_Columns",1);
		tb.moveRow=function(index,nindex)
		{
			
			//if(!confirm('移动位置可能会造成数据丢失风险,正式运行的系统建议在数据库下进行调整，现在还要继续吗？')){return;}
			
			var url = "?act=movecolumn&modelid=" + ModelID + "&index=" + index + "&nindex=" + nindex + "&t=" + now();
			showloading();
			$.ajax({
				type: "GET", async: true,
				url:url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
					//loadData(div);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
			});	
		};
	}
	//基本设置
	var tb1=getobj("eDataTable_Basic");
	if(tb1)
	{
		tb1=new eDataTable("eDataTable_Basic",1);
		tb1.moveRow=function(index,nindex)
		{
			$("#eDataTable_Basic tbody tr td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			//$("#eDataTable_Basic tbody tr td:first-child input:checked").each(function(index1,obj){	
			$("#eDataTable_Basic tbody tr td.tdshowedit").find("input:checked:eq(0)").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				//ids+=$(obj).parent().parent().attr("erowid");
				ids+=$(obj).parents("tr:first").attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelitem&modelid=" + ModelID + "&item=setorders&t=" + now();			
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//列表
	var tb2=getobj("eDataTable_List");
	if(tb2)
	{
		tb2=new eDataTable("eDataTable_List",1);
		tb2.moveRow=function(index,nindex)
		{
			$("#eDataTable_List tbody tr td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			//$("#eDataTable_List tbody tr td:first-child input:checked").each(function(index1,obj){	
			$("#eDataTable_List tbody tr td.tdshowlist input:checked").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				//ids+=$(obj).parent().parent().attr("erowid");
				ids+=$(obj).parents("tr:first").attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelitem&modelid=" + ModelID + "&item=setlistorders&t=" + now();			
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//导出
	var tb3=getobj("eDataTable_Export");
	if(tb3)
	{
		tb3=new eDataTable("eDataTable_Export",1);
		//alert(tb3);
		tb3.moveRow=function(index,nindex)
		{
			$("#eDataTable_Export tbody tr td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			//$("#eDataTable_Export tbody tr td:first-child input:checked").each(function(index1,obj){		
			$("#eDataTable_Export tbody tr td.tdshowexport input:checked").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				//ids+=$(obj).parent().parent().attr("erowid");
				ids+=$(obj).parents("tr:first").attr("erowid");
			}); 
			//alert(ids);
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelitem&modelid=" + ModelID + "&item=setexportorders&t=" + now();			
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//搜索
	var tb4=getobj("eDataTable_Search");
	if(tb4)
	{
		tb4=new eDataTable("eDataTable_Search",1);
		tb4.moveRow=function(index,nindex)
		{
			$("#eDataTable_Search tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$("#eDataTable_Search tbody > tr > td:nth-child(2) input:checked").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				//ids+=$(obj).parent().parent().attr("erowid");
				ids+=$(obj).parents("tr:first").attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelcondition&modelid=" + ModelID + "&item=setorders&t=" + now();			
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};		
	}
	//审核
	var tb5=getobj("eDataTable_CheckUp");
	if(tb5)
	{
		tb5=new eDataTable("eDataTable_CheckUp",1);
		tb5.moveRow=function(index,nindex)
		{
			$("#eDataTable_CheckUp tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$("#eDataTable_CheckUp tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			var url = "?act=setcheckup&modelid=" + ModelID + "&item=setorders&t=" + now();
			showloading();
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//选项卡
	var tb6=getobj("eDataTable_Tab");
	if(tb6)
	{
		tb6=new eDataTable("eDataTable_Tab",1);
		tb6.moveRow=function(index,nindex)
		{
			$("#eDataTable_Tab tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$("#eDataTable_Tab tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodeltab&modelid=" + ModelID + "&item=setorders&t=" + now();
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//面板
	var tb7=getobj("eDataTable_Panel");
	if(tb7)
	{
		tb7=new eDataTable("eDataTable_Panel",1);
		tb7.moveRow=function(index,nindex)
		{
			$("#eDataTable_Panel tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$("#eDataTable_Panel tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelgroup&modelid=" + ModelID + "&item=setorders&t=" + now();
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//布局-列
	var tb8=getobj("eDataTable_Items");
	if(tb8)
	{
		tb8=new eDataTable("eDataTable_Items",1);
		tb8.moveRow=function(index,nindex)
		{
			$("#eDataTable_Items tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$("#eDataTable_Items tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setmodelitem&modelid=" + ModelID + "&item=setorders&t=" + now();
			$.ajax({
				type: "POST", async: true,
				data:{ids:ids},
				url: url,
				dataType: "json",
				success: function (data) 
				{
				    hideloading();
				    skipLogin(data);
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					hideloading();
				}
   			});
		};
	}
	//报表
	var tb9=getobj("eDataTable_Report");
	if(tb9)
	{
		tb9=new eDataTable("eDataTable_Report",1);
		tb9.moveRow=function(index,nindex)
		{
			var ids="";
			$("#eDataTable_Report tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setreportorders&ModelID=" + ModelID + "&t=" + now();
			$.ajax({
				type: "POST", async: true,
				data: { ids: ids },
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
	}
	//搜索-选项
	$(".eDataTableOpt").each(function(i,elem)
	{
		var _tb=new eDataTable(elem,1);
		_tb.moveRow=function(index,nindex)
		{
			$(elem).find("tbody > tr > td:last-child").each(function(index1,obj){
				$(obj).html(1+index1);
			}); 
			var ids="";
			$(elem).find("tbody > tr").each(function(index1,obj){	
				if(index1>0){ids+=",";}
				ids+=$(obj).attr("erowid");
			}); 
			if(ids.length==0){return;}
			showloading();
			var url = "?act=setsearchoptionsorders&ModelID=" + ModelID + "&t=" + now();
			$.ajax({
				type: "POST", async: true,
				data: { ids: ids },
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
	});
};
//选项卡切换
function selecttab(a) 
{
       // alert(a.Index());
        $(a).parent().find("a").attr("class", "");
        $(a).attr("class", "cur");
        var index = $(a).index();
        var dd = $(a).parent().next();
		//alert(dd.html());
        dd.find("div").hide();
        //var div=dd.find("div:eq(" + index + ")"); //DIV下DIV也算在内
        var div = dd.children("div:eq(" + index + ")");
        var loaded = div.attr("loaded");
        if (loaded == undefined) {
            loadData(div);
        }
        div.show();
		
};
//加载层
function loadData(div)
{
	
	if(!div.attr("dataurl"))
	{
		return;
	}
	showloading();
    $.ajax({
		   type: "GET", async: true,
           url: div.attr("dataurl") + "&t=" + now(),
           dataType: "html",
           success: function (html) {
			   div.html(html);		
			   //executeJS(html);
			   //div[0].innerHTML=html;
               //div.attr("loaded", "true");
			   try
			   {
					eDataTable_event(div);
					var chk=$("input[name='chkdisabled']");
					if(chk[0].checked)
					{
						setreadonly();
					}
					else
					{
						cancelreadonly();
					}
			   }
			   catch(e)
			   {
			   }
			   hideloading();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
                div.show();
            }
        });
};
$(document).ready(function () {       
		$(".eTab dt a:eq(0)").addClass("cur");
		
		 //loadData($(".eTab dd div:eq(0)"));
        loadData($(".eTab dd").find("div").eq(0));
		$(".eTab dd").find("div").eq(0).show();
});
//清除用户自定义
function clearCustoms()
{
	var _back=confirm('确认要删除用户自定义的信息吗？');
	if(!_back){return;}
	
	showloading();
	var url = "?act=clearcustoms&modelid=" + ModelID + "&t=" + now();
	 $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
			hideloading();
			layer.msg(data.message);
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
    });
};
//同步模块
function syncModel()
{
	showloading();
	var url = "?act=syncmodel&modelid=" + ModelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
		    hideloading();
		    skipLogin(data);
			loadData($(".eTab dd").children("div").eq(1));
			//loadData($(".eTab dd").find("div").eq(1));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
    });
	
};
//同步编码OK
function syncCode()
{
	showloading();
	var url = "?act=synccode&modelid=" + ModelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
		    hideloading();
		    skipLogin(data);
			loadData($(".eTab dd").children("div").eq(1));
			//loadData($(".eTab dd").find("div").eq(1));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
    });
};
//还原编码OK
function restoreCode()
{
	showloading();
	var url = "?act=restorecode&modelid=" + ModelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
		    hideloading();
		    skipLogin(data);
			loadData($(".eTab dd").children("div").eq(1));
			//loadData($(".eTab dd").find("div").eq(1));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
    });
};


//取控件值
function getValue(obj)
{
    if (obj.type.toLowerCase() == "checkbox")
	{
		return (obj.checked ? "1" : "0");
	}
	else if(obj.type.toLowerCase() == "radio")
	{
		return obj.value;
	}
	else 
	{
		return obj.value.encode();
	}
};

function getJsonValue(obj)
{
    if (obj.type.toLowerCase() == "checkbox")
	{
		return (obj.checked ? "True" : "False");
	}
	else if(obj.type.toLowerCase() == "radio")
	{
		return obj.value;
	}
	else 
	{
		return obj.value.encode();
	}
};


function setModelPropertys(obj,Item)
{
	if (obj.type.toLowerCase() != "checkbox" && obj.type.toLowerCase() != "radio" && obj.getAttribute("oldvalue") == obj.value) {hideloading(); return; }
	var value = getJsonValue(obj);	
	//alert(value);
	//return;
	if (value == "error") {hideloading();  return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setmodelpropertys&modelid=" + ModelID + "&item=" + Item + "&t=" + now();
	 $.ajax({
            type: "POST",
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) 
			{
                //obj.setAttribute("oldvalue", obj.value);
				$(obj).attr("oldvalue",obj.value);
				$(obj).prop("oldvalue",obj.value);
				hideloading();
				skipLogin(data);
				if(!_reload){return;}
				loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
   });
	
};

//模块OK
function setModel(obj,Item,mid)
{
	if (obj.type.toLowerCase() != "checkbox" && obj.type.toLowerCase() != "radio" && obj.getAttribute("oldvalue") == obj.value) {hideloading(); return; }
	var value = getValue(obj);
	//alert(obj.value.replace(/\n/gi,"aa"));
	//alert(value.replace(/\n/g,"cc"));
	//return;
	if (value == "error") {hideloading();  return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var _modelid=ModelID;
	if(mid){_modelid=mid;}
	var url = "?act=setmodel&modelid=" + _modelid + "&item=" + Item + "&t=" + now();	
    //ajax:false同步,true异步
    $.ajax({
            type: "POST",
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) 
			{
				//document.title=obj.value;
                //obj.setAttribute("oldvalue", obj.value);
				$(obj).attr("oldvalue",obj.value);
				$(obj).prop("oldvalue",obj.value);
				hideloading();
				skipLogin(data);
				if(!_reload){return;}
				loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
   });
};
//添加审批流程OK
function addCheckUp(obj)
{
	showloading();
    var url = "?act=addcheckup&modelid=" + ModelID + "&t=" + now();
	$.ajax({
		   type: "GET", async: true,
           url: url,
           dataType: "json",
           success: function (data) {
               hideloading();
               skipLogin(data);
               loadData($(obj).parents("div[dataurl]"));
           },
           error: function (XMLHttpRequest, textStatus, errorThrown) {
		   		hideloading();
           }
    });
};	
//修改审批流程OK
function setCheckUp(obj, CheckupID, Item)
{
	if (obj.getAttribute("oldvalue") == obj.value) { return; }
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setcheckup&modelid=" + ModelID + "&CheckupID=" + CheckupID + "&item=" + Item + "&t=" + now();
    $.ajax({
		   type: "post", 
		   async: true,
		   data:{value:value},
           url: url,
           dataType: "json",
           success: function (data) {
			    //obj.setAttribute("oldvalue", obj.value);
				$(obj).attr("oldvalue",obj.value);
				$(obj).prop("oldvalue",obj.value);
				hideloading();
				skipLogin(data);
			   	if(!_reload){return;}
               	loadData($(obj).parents("div[dataurl]"));
           },
           error: function (XMLHttpRequest, textStatus, errorThrown) {
			   hideloading();
           }
    });
};
//删除审批流程OK
function delCheckUp(obj, CheckupID)
{
	if (!confirm('确认要删除吗？')) { return; }
	showloading();
	var url = "?act=delcheckup&modelid=" + ModelID + "&CheckupID=" + CheckupID + "&t=" + now();
	$.ajax({
		   type: "GET", async: true,
           url: url,
           dataType: "json",
           success: function (data) {
               hideloading();
               skipLogin(data);
			   $(getParent(obj,"tr")).remove();
               //loadData($(obj).parents("div[dataurl]"));
           },
           error: function (XMLHttpRequest, textStatus, errorThrown) {
			   hideloading();
           }
    });
};
//添加动作OK
function addAction(obj)
{
	showloading();
	var url = "?act=addaction&modelid=" + ModelID + "&t=" + now();
	$.ajax({
		   type: "GET", async: true,
           url: url,
           dataType: "json",
           success: function (data) {
               hideloading();
               skipLogin(data);
               loadData($(obj).parents("div[dataurl]"));
           },
           error: function (XMLHttpRequest, textStatus, errorThrown) {
			   hideloading();
           }
    });
};
//修改动作OK
function setAction(obj, ActionID, Item)
{
	if (obj.getAttribute("oldvalue") == obj.value) { return; }	
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setaction&modelid=" + ModelID + "&actionid=" + ActionID + "&item=" + Item + "&t=" + now();
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
				//obj.setAttribute("oldvalue", obj.value);
				$(obj).attr("oldvalue",obj.value);
				$(obj).prop("oldvalue",obj.value);
				hideloading();
				skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//删除动作OK
function delAction(obj, ActionID)
{
	if (!confirm('确认要删除吗？')) { return; }
	showloading();
	var url = "?act=delaction&modelid=" + ModelID + "&actionid=" + ActionID + "&t=" + now();
    $.ajax({
            type: "GET", async: true,
            url:url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
				$(getParent(obj,"tr")).remove();
                //loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//添加模块-列OK
function addModelItem(obj)
{
	var url = "?act=addmodelitem&modelid=" + ModelID + "&t=" + now();
	showloading();
	$.ajax({
            type: "GET", async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
function setModelItemPropertys(obj, ModelItemID, Item)
{

	if (obj.type=="text" && obj.getAttribute("oldvalue") == obj.value) { return; }
	var value = getValue(obj);
	
	if(Item.toLowerCase()=="mliststate"){value=obj.checked ? "True" : "False";}
	
	
	
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	var url = "?act=setmodelitempropertys&modelid=" + ModelID + "&modelitemid=" + ModelItemID + "&item=" + Item + "&t=" + now();

	showloading();
	
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
				//obj.setAttribute("oldvalue", obj.value);
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
function quickBind(obj,ModelItemID)
{
	var value = obj.value;
	var url = "?act=quickbind&modelid=" + ModelID + "&modelitemid=" + ModelItemID + "&t=" + now();
	showloading();
	 $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};

function setModelItem_FillItem(obj,name, ModelItemID)
{
	var value="";
	$("input[name='" + name + "']:checked").each(function(i,el){
		if(i>0){value+=",";}
		value+=$(this).val();
	});
	var _reload=parseBool(Attribute(obj,"reload"));
	var url = "?act=setmodelitem&modelid=" + ModelID + "&modelitemid=" + ModelItemID + "&item=fillitem&t=" + now();
	showloading();
	$.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
	
};

//修改模块-列OK
function setModelItem(obj, ModelItemID, Item)
{

	if (obj.type=="text" && obj.getAttribute("oldvalue") == obj.value) { return; }
	var value = getValue(obj);
	if(Item.toLowerCase()==="mliststate"){value=obj.checked ? "1" : "0";}
	if(Item.toLowerCase()=="exportname"){value=obj.checked ? "1" : "0";}
	if (value == "error") { return; }
	if(Item.toLowerCase()=="fillmodelitemidbak")
	{
		var ovalue=obj.getAttribute("oldvalue");
		for(var i=0;i< obj.options.length;i++)
		{
			ovalue=ovalue.replace(obj.options[i].value,"");
		}
		ovalue=ovalue.replace(",,",",");
		if(ovalue.substr(0,1)==","){ovalue=ovalue.substr(1);}
		if(ovalue.substr(ovalue.length-1,1)==","){ovalue=ovalue.substr(0,ovalue.length-1);}
		
		if(value.length>5)
		{
			if(ovalue.length>5){ovalue+=",";}
			ovalue=ovalue+value;			
		}
		else
		{
			ovalue=value;
		}
		value=ovalue;
	}	
	
	var _reload=parseBool(Attribute(obj,"reload"));
	var url = "?act=setmodelitem&modelid=" + ModelID + "&modelitemid=" + ModelItemID + "&item=" + Item + "&t=" + now();
	//alert(value);
	//return;
	showloading();
	
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
				if(Item.toLowerCase()=="fillmodelitemid")
				{
					obj.setAttribute("oldvalue", ovalue);
				}
				else
				{
					obj.setAttribute("oldvalue", value);
				}
				hideloading();
				skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//删除模块-列OK
function delModelItem(obj, ModelItemID)
{
	if (!confirm('确认要删除吗？')) { return; }
	showloading();
	var url = "?act=delmodelitem&modelid=" + ModelID + "&modelitemid=" + ModelItemID + "&t=" + now();
    $.ajax({
            type: "GET", async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
				$(getParent(obj,"tr")).remove();
                //loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//添加模块条件OK
function addModelCondition(obj)
{
	var url = "?act=addmodelcondition&modelid=" + ModelID + "&t=" + now();
	showloading();
	$.ajax({
            type: "GET", async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//修改模块条件OK
function setModelCondition_FillItem(obj,name, ModelConditionID)
{
	var value="";
	$("input[name='" + name + "']:checked").each(function(i,el){
		if(i>0){value+=",";}
		value+=$(this).val();
	});
	var _reload=parseBool(Attribute(obj,"reload"));
	var url = "?act=setmodelcondition&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&item=fillitem&t=" + now();
	showloading();	
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {	
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
function setModelConditionColumn(obj, ModelConditionID, Item)
{
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	var smodelid=obj.options[obj.selectedIndex].getAttribute("modelid");
	//alert(smodelid);
	showloading();
	var url = "?act=setmodelcondition&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&item=" + Item + "&smodelid=" + smodelid + "&t=" + now();
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {	
				//obj.setAttribute("oldvalue", obj.value);
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
function setModelCondition(obj, ModelConditionID, Item)
{
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setmodelcondition&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&item=" + Item + "&t=" + now();
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {	
				//obj.setAttribute("oldvalue", obj.value);
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//删除模块条件OK
function delModelCondition(obj, ModelConditionID)
{
	if (!confirm('确认要删除吗？')) { return; }
	showloading();
	var url = "?act=delmodelcondition&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&t=" + now();
    $.ajax({
            type: "GET", async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
				$(getParent(obj,"tr")).remove();
                //loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//添加模块条件-选项 OK
function addModelConditionItem(obj, ModelConditionID)
{
	showloading();
	var url = "?act=addmodelconditionitem&modelid=" + ModelID + "&modelconditionid=" + ModelConditionID + "&t=" + now();
	$.ajax({
            type: "GET", async: true,
            url: url,
            dataType: "json",
            success: function (data) {
                hideloading();
                skipLogin(data);
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//修改模块条件-选项OK;
function setModelConditionItem(obj, ModelConditionItemID, Item)
{
	var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setmodelconditionitem&modelid=" + ModelID + "&modelconditionitemid=" + ModelConditionItemID + "&item=" + Item + "&t=" + now();
    $.ajax({
            type: "post", 
			async: true,
			data:{value:value},
            url: url,
            dataType: "json",
            success: function (data) {
				//obj.setAttribute("oldvalue", obj.value);
                hideloading();
                skipLogin(data);
				if(!_reload){return;}
                loadData($(obj).parents("div[dataurl]"));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};
//删除模块条件-选项OK
function delModelConditionItem(obj, ModelConditionItemID)
{
	if (!confirm('确认要删除吗？')) { return; }
	showloading();
	var url = "?act=delmodelconditionitem&modelid=" + ModelID + "&modelconditionitemid=" + ModelConditionItemID + "&t=" + now();
	$.ajax({
		   type: "GET", async: true,
           url: url,
           dataType: "json",
           success: function (data) {
               hideloading();
               skipLogin(data);
			   $(getParent(obj,"tr")).remove();
               //loadData($(obj).parents("div[dataurl]"));
           },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
				hideloading();
            }
    });
};

//添加模块-选项卡
function addModelTab(obj)
{
	showloading();
	var url = "?act=addmodeltab&modelid=" + ModelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data) 
		{
		    hideloading();
		    skipLogin(data);
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			hideloading();
		}
	});
};
//修改模块-选项卡OK
function setModelTab(obj, ModelTabID, Item)
{
	if (obj.type!="checkbox" && obj.type!="radio" && obj.getAttribute("oldvalue") == obj.value) {return;}
    var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setmodeltab&modelid=" + ModelID + "&modeltabid=" + ModelTabID + "&item=" + Item + "&t=" + now();
	$.ajax({
		type: "post", 
		async: true,
		data:{value:value},
		url: url,		
		dataType: "json",
		success: function (data)
		{
			//obj.setAttribute("oldvalue", obj.value);
			$(obj).attr("oldvalue",obj.value);
			$(obj).prop("oldvalue",obj.value);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown)
		{
			hideloading();
		}
	});
};
//删除模块-选项卡
function delModelTab(obj, ModelTabID)
{
	if (!confirm('确认要删除吗？')) {return;}
	showloading();
	var url = "?act=delmodeltab&modelid=" + ModelID + "&modeltabid=" + ModelTabID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
		    hideloading();
		    skipLogin(data);
			$(getParent(obj,"tr")).remove();
			//loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown)
		{
			hideloading();
		}
	});
};
//添加模块-面板 OK
function addModelGroup(obj)
{
	showloading();
	var url = "?act=addmodelgroup&modelid=" + ModelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data) 
		{
		    hideloading();
		    skipLogin(data);
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown)
		{
			hideloading();
		}
	});
};
//修改模块-面板OK
function setModelGroup(obj, ModelPanelID, Item)
{
	if (obj.type!="checkbox" && obj.type!="radio" && obj.getAttribute("oldvalue") == obj.value) {return;}
    var value = getValue(obj);
	if (value == "error") { return; }
	var _reload=parseBool(Attribute(obj,"reload"));
	showloading();
	var url = "?act=setmodelgroup&modelid=" + ModelID + "&modelpaneid=" + ModelPanelID + "&item=" + Item + "&t=" + now();
	$.ajax({
		type: "post",
		async: true,
		data:{value:value},
		url: url,
		dataType: "json",
		success: function (data)
		{
			//obj.setAttribute("oldvalue", obj.value);
			$(obj).attr("oldvalue",obj.value);
			$(obj).prop("oldvalue",obj.value);
			hideloading();
			skipLogin(data);
			if(!_reload){return;}
			loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown)
		{
			hideloading();
		}
	});
};
//删除模块-面板
function delModelGroup(obj, ModelPanelID)
{
	if (!confirm('确认要删除吗？')) {return;}
	showloading();
	var url = "?act=delmodelgroup&modelid=" + ModelID + "&modelpaneid=" + ModelPanelID + "&t=" + now();
    $.ajax({
		type: "GET", async: true,
		url: url,
		dataType: "json",
		success: function (data)
		{
		    hideloading();
		    skipLogin(data);
			$(getParent(obj,"tr")).remove();
			//loadData($(obj).parents("div[dataurl]"));
		},
		error: function (XMLHttpRequest, textStatus, errorThrown)
		{
			hideloading();
		}
	});
};
//打印设置
function savePrintHTML()
{	
	setModel($("#listprintHTMLStart").get(0), 'listprinthtmlstart');	
	setModel($("#listprintHTMLEnd").get(0), 'listprinthtmlend');
	setModel($("#printHTMLStart").get(0), 'printhtmlstart');	
    setModel($("#printHTML").get(0), 'printhtml');
	setModel($("#printHTMLEnd").get(0), 'printhtmlend');
};