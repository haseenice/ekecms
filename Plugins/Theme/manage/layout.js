var layout_moving_time=180;
var layout_border_width=5;
function handlerInTop()
{
	 $(".eLayout_top").unbind("mouseenter",handlerInTop);
	 $(".eLayout_top").animate({top:'0'},layout_moving_time,function(){$(".eLayout_top").unbind("mouseleave",handlerOutTop).bind("mouseleave",handlerOutTop);});	 
	 $(".eLayout_top").removeClass("eLayout_top_float").addClass("eLayout_top_fixed eLayout_top_alpha");
};
function handlerOutTop()
{
	var h= $(".eLayout_top").height()-layout_border_width;
	$(".eLayout_top").unbind("mouseleave",handlerOutTop);
	$(".eLayout_top").animate({top:-h},layout_moving_time,function(){$(".eLayout_top").unbind("mouseenter",handlerInTop).bind("mouseenter",handlerInTop);});
	$(".eLayout_top").removeClass("eLayout_top_fixed").addClass("eLayout_top_float");
};
function handlerInLeft()
{
	 $(".eLayout_menu").unbind("mouseenter",handlerInLeft);
	 $(".eLayout_menu").animate({left:'0'},layout_moving_time,function(){$(".eLayout_menu").unbind("mouseleave",handlerOutLeft).bind("mouseleave",handlerOutLeft);});
	 $(".eLayout_menu").removeClass("eLayout_menu_float").addClass("eLayout_menu_fixed eLayout_left_alpha");
	 //$(".eLayout_menu").getNiceScroll().show();
};
function handlerOutLeft()
{
	var w=$(".eLayout_menu").width()-layout_border_width;
	$(".eLayout_menu").unbind("mouseleave",handlerOutLeft);
	$(".eLayout_menu").animate({left:-w},layout_moving_time,function(){$(".eLayout_menu").unbind("mouseenter",handlerInLeft).bind("mouseenter",handlerInLeft);});	
	$(".eLayout_menu").removeClass("eLayout_menu_fixed").addClass("eLayout_menu_float");
	//$(".eLayout_menu").getNiceScroll().hide();
};
function setFixed(name,value)
{
	var url="default.aspx?act=setfixed&name=" + name + "&value=" + value;
	$.ajax({
			type: 'get',
			url: url,			
			dataType: "html",
			success: function(data)
			{
				
			}
		});
};
function switchLeftFix()
{
	if($(".leftfixbtn").hasClass("leftfixbtn_float"))
	{
		//document.title="修改为固定";
		$(".leftfixbtn").removeClass("leftfixbtn_float").addClass("leftfixbtn_fixed");
		if($(".eLayout_content").hasClass("eLayout_content_float"))
		{
			$(".eLayout_content").removeClass("eLayout_content_float").addClass("eLayout_content_fixed");
		}
		$(".eLayout_menu").unbind("mouseenter",handlerInLeft);
		$(".eLayout_menu").unbind("mouseleave",handlerOutLeft);	
		$(".eLayout_menu").removeClass("eLayout_left_alpha");
		setFixed("manage_left_fixed","true");
	}
	else
	{
		//document.title="修改为浮动";
		$(".leftfixbtn").removeClass("leftfixbtn_fixed").addClass("leftfixbtn_float");
		if($(".eLayout_content").hasClass("eLayout_content_fixed"))
		{
			$(".eLayout_content").removeClass("eLayout_content_fixed").addClass("eLayout_content_float");
		}
		$(".eLayout_menu").unbind("mouseenter",handlerInLeft).bind("mouseenter",handlerInLeft);
		$(".eLayout_menu").unbind("mouseleave",handlerOutLeft).bind("mouseleave",handlerOutLeft);
		$(".eLayout_menu").addClass("eLayout_left_alpha");
		setFixed("manage_left_fixed","false");
	}
};
function switchTopFix()
{
	if($(".topfixbtn").hasClass("topfixbtn_float"))
	{
		//document.title="修改为固定";
		$(".topfixbtn").removeClass("topfixbtn_float").addClass("topfixbtn_fixed");
		if($(".eLayout_body").hasClass("eLayout_body_float"))
		{
			$(".eLayout_body").removeClass("eLayout_body_float").addClass("eLayout_body_fixed");
		}
		$(".eLayout_top").unbind("mouseenter",handlerInTop);
		$(".eLayout_top").unbind("mouseleave",handlerOutTop);	
		$(".eLayout_top").removeClass("eLayout_top_alpha");
		setFixed("manage_top_fixed","true");
	}
	else
	{
		//document.title="修改为浮动";
		$(".topfixbtn").removeClass("topfixbtn_fixed").addClass("topfixbtn_float");

		if($(".eLayout_body").hasClass("eLayout_body_fixed"))
		{
			$(".eLayout_body").removeClass("eLayout_body_fixed").addClass("eLayout_body_float");
		}
		$(".eLayout_top").unbind("mouseenter",handlerInTop).bind("mouseenter",handlerInTop);
		$(".eLayout_top").unbind("mouseleave",handlerOutTop).bind("mouseleave",handlerOutTop);
		$(".eLayout_top").addClass("eLayout_top_alpha");
		setFixed("manage_top_fixed","false");
	}	
	eLayout_autoSize();
};
function eLayout_autoSize()
{
	//$(".eLayout_body").css("height", (document.body.clientHeight - 62) + "px");
	//return;
	var minheight=document.body.clientHeight;
	minheight-= $(".eLayout_top").hasClass("eLayout_top_float") ? 10 : 67;
	
	
	var h1=$(".eLayout_menu").innerHeight();
	if($(".eLayout_menu").attr("oheight"))
	{
		h1=parseInt($(".eLayout_menu").attr("oheight"));
	}	
	var h2=$(".eLayout_content").innerHeight();
	if(h1<minheight && h2<minheight)
	{
		$(".eLayout_menu").css("height", minheight  + "px");
		return;
	}
	
	//document.title=minheight + "::" + h1 + "::" + h2;	
	if(h1<h2)
	{
		//$(".eLayout_body").css("height", h2  + "px");
		$(".eLayout_menu").css("height", h2  + "px");
		//document.title="h2:" + h2 + "::" + h1;
	}
	else
	{
		//$(".eLayout_body").css("height", h1  + "px");
		$(".eLayout_content").css("height", h1  + "px");
		//document.title="h1:" + h1;		
	}
	//document.title=h2;
	//$(".eLayout_body").css("height", $(".eLayout_content").innerHeight() + "px");
	//$(".eLayout_menu").css("height", "100%");
	//document.title=$(".eLayout_body").get(0).offsetHeight + "::" + $(".eLayout_menu").get(0).offsetHeight + "::" + $(".eLayout_content").get(0).offsetHeight;
};
$(window).resize(function(){
    var minheight=document.body.clientHeight;
	minheight-= $(".eLayout_top").hasClass("eLayout_top_float") ? 10 : 67;
	$(".eLayout_content").css("height", minheight  + "px");
});

$(document).ready(function(){
	$(".eLayout_top_float").unbind("mouseenter",handlerInTop).bind("mouseenter",handlerInTop);
	$(".eLayout_menu_float").unbind("mouseenter",handlerInLeft).bind("mouseenter",handlerInLeft);

	$(".eLayout_menu").attr("oheight",$(".eLayout_menu").innerHeight());
	$(".eLayout_body").css("min-height",document.body.clientHeight-80);
	eLayout_autoSize();
	$(".eLayout_content").bind("click",function(){
		eLayout_autoSize();
		//document.title= $(".eLayout_content").get(0).offsetHeight;
		//document.title=$(".eLayout_body").innerHeight() + "::" + $(".eLayout_menu").innerHeight() + "::" + $(".eLayout_content").innerHeight();
		//document.title=$(".eLayout_body").get(0).offsetHeight + "::" + $(".eLayout_menu").get(0).offsetHeight + "::" + $(".eLayout_content").get(0).offsetHeight;
												 
												   
	});

});