<%@ Page Language="C#" AutoEventWireup="true" CodeFile="signMobile.aspx.cs" Inherits="Plugins_signMobile" %><!DOCTYPE html>
<html>
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0,user-scalable=0,uc-fitscreen=yes" />
   <title>手写签名</title>
  <script src="../Scripts/jquery.js"></script>
   <script src="../Plugins/layui226/layui.all.js?ver=<%=Common.Version %>"></script>
   <link href="../Plugins/eControls/default/mobile/style.css?ver=<%=Common.Version %>" rel="stylesheet" type="text/css" /><!-- eFrameWork控件样式-->
</head>
<%if(act!="finish"){ %>
<style>
    .txt { position:absolute;top:-1120px; z-index:33233322;width:50px;
    }
html,body{margin:0;padding:0;height:100%;width:100%;
overflow:hidden; position:fixed;
 justify-content:center;align-items:center;display:flex;
}
*{box-sizing:border-box;}
.area{ border:0px solid #ff0000;width:100vh; height:100vw;position:fixed;
transform: rotate(90deg);  transition:transform 0.5s;color:#666; z-index:1;
}
.tip{ position:absolute; top:5px;left:6px; z-index:11;}
.chk{ position:absolute; bottom:10px;left:30px; z-index:11; }
.bottom{ position:absolute; bottom:10px;right:30px; z-index:11; }
.esignin { background-color:#f8f8f8;width:100%;height:100%; position:absolute; top:0px;left:0px; z-index:2;}

.btn{display:inline-block;padding:6px 25px 6px 25px; background-color:#f3f3f3;color:#333; text-decoration:none;border-radius:30px;margin-right:20px;}
.default{ background-color:#427EF0;color:#fff;}
</style><%} %>
<body>
<input name="txt" type="text" id="txt" class="txt" />
<asp:Literal id="litMsg" runat="server" />
<%if(act!="finish"){ %>
<div class="area">
<div class="tip">请在下方签名：</div>
<canvas id="esignin" class="esignin">浏览器不支持签名!</canvas><%if(mode!="mobile"){ %>
<div class="chk"><label><input type="checkbox" id="savesign" name="savesign" value="1" />保存为默认签名</label></div><%} %>
<div class="bottom"><a class="btn" id="clear" href="javascript:;">清除</a><a class="btn default" href="javascript:;" onclick="save();">确定</a></div>
</div>      
<script type="text/javascript">
    var eSignIn = {};
    eSignIn.listen = function () {
        if (!this.canvas) return;

        var painting = false;
        var lastPoint = { x: undefined, y: undefined };
        this.canvas.width = this.canvas.clientWidth;
        this.canvas.height = this.canvas.clientHeight;

        if (document.body.ontouchstart !== undefined) {
            this.canvas.ontouchstart = function (e) {
                e.preventDefault();
                var canvasLeft = e.target.offsetLeft;
                var canvasTop = e.target.offsetTop;
                painting = true;
                var x = e.touches[0].pageX - canvasLeft;// e.changedTouches[0].clientX - canvasLeft;
                var y = e.touches[0].pageY - canvasTop;//  e.changedTouches[0].clientY - canvasTop;
                lastPoint = { "x": x, "y": y };

                eSignIn.ctx.save();
                eSignIn.ctx.beginPath();
                eSignIn.ctx.moveTo(e.clientX, e.clientY);
            };
            this.canvas.ontouchmove = function (e) {
                e.preventDefault();
                if (painting) {
                    var canvasLeft = e.target.offsetLeft;
                    var canvasTop = e.target.offsetTop;
                    var x = e.changedTouches[0].pageX - canvasLeft;// e.changedTouches[0].clientX - canvasLeft;
                    var y = e.changedTouches[0].pageY - canvasTop; //e.changedTouches[0].clientY - canvasTop;
                    var newPoint = { "x": x, "y": y };
                    eSignIn.drawLine(lastPoint.x, lastPoint.y, newPoint.x, newPoint.y);
                    lastPoint = newPoint;
                }
            };
            this.canvas.ontouchend = function () { painting = false; };
        }
        else {

            this.canvas.onmousedown = function (e) {
                var canvasLeft = e.target.getBoundingClientRect().left;// e.target.offsetLeft;
                var canvasTop = e.target.getBoundingClientRect().top;//e.target.offsetTop;
                painting = true;
                var x = e.clientX - canvasLeft;
                var y = e.clientY - canvasTop;
                lastPoint = { "x": x, "y": y };
                eSignIn.ctx.save();
            };
            this.canvas.onmousemove = function (e) {
                if (painting) {
                    var canvasLeft = e.target.getBoundingClientRect().left;//;//e.target.offsetLeft;
                    var canvasTop = e.target.getBoundingClientRect().top;////e.target.offsetTop;
                    var x = e.clientX - canvasLeft;
                    var y = e.clientY - canvasTop;
                    var newPoint = { "x": x, "y": y };
                    eSignIn.drawLine(lastPoint.x, lastPoint.y, newPoint.x, newPoint.y);
                    lastPoint = newPoint;
                }
            };
            this.canvas.onmouseup = function () { painting = false; };
            this.canvas.mouseleave = function () { painting = false; };
        }
    };
    eSignIn.drawLine = function (x1, y1, x2, y2) {
        if (!this.canvas) return;
        var wCount = parseInt($(this.canvas).attr("wCount"))+1;
        $(this.canvas).attr("wCount", wCount);
        this.ctx.lineWidth = this.lineWidth;
        this.ctx.lineCap = "round";
        this.ctx.lineJoin = "round";
        //this.ctx.moveTo(x1, y1);
        //this.ctx.lineTo(x2, y2);
        x1 = this.canvas.clientHeight - x1;
        x2 = this.canvas.clientHeight - x2;
        this.ctx.moveTo(y1, x1);
        this.ctx.lineTo(y2, x2);


        this.ctx.stroke();
        this.ctx.closePath();
    };
    eSignIn.init = function () {
        var canvas = document.getElementById("esignin");
        if (!canvas) return;
        $(canvas).attr("wCount", 0);
        this.canvas = canvas;
        this.lineWidth = 4;
        this.ctx = canvas.getContext("2d");
        this.listen();
    };
    eSignIn.clear = function () {
        if (!this.canvas) return;
        $(this.canvas).attr("wCount", 0);
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.ctx.beginPath();
    };
    eSignIn.geturldata = function () {
        if (!this.canvas) return "";
        return this.canvas.toDataURL("image/png");
    };
    eSignIn.getdata = function () {
        var data = eSignIn.geturldata();
        data = data.replace(/^data:image\/(png|jpg);base64,/, "");
        return data;
    };
    eSignIn.init();


    document.getElementById("clear").addEventListener('click', function () {
        eSignIn.clear();
    });
    function save()
    {
        var canvas = document.getElementById("esignin");
        if (!canvas) return;
        var wCount = parseInt($(canvas).attr("wCount"));
        if (wCount < 35)
        {
            layer.msg("签名不正确!");
            return;
        }
        var mode = "<%=mode%>";
        if (mode == "mobile")//移动端Frame签名
        {
            saveDefault();
        }
        else
        {
            saveToServer();
        }
    }
    function saveDefault()
    {
        var wCount = parseInt($("#esignin").attr("wCount"));

        var edata = eSignIn.getdata();
        var box = $(parent.signlink).parent().parent();
        var canvas = box.find("canvas").get(0);
        $(canvas).attr("wCount", wCount);
        canvas.width = canvas.clientWidth;
        canvas.height = canvas.clientHeight;
        var img = new Image;
        img.onload = function () {
            var tow = 0, toh = 0;
            if (img.width / canvas.clientWidth < img.height / canvas.clientHeight) {
                toh = canvas.clientHeight;
                tow = parseInt(img.width * toh / img.height);
            }
            else {
                tow = canvas.clientWidth;
                toh = parseInt(img.height * tow / img.width);
            }
            canvas.getContext("2d").clearRect(0, 0, canvas.clientWidth, canvas.clientHeight);
            canvas.getContext("2d").drawImage(img, 0, 0, tow, toh);
        };
        img.src = "data:image/png;base64," + edata;
        parent.layer.close(parent.arrLayerIndex.pop());
        return;
        var area = "<%=area%>";        
        var savesign = $("#savesign").get(0).checked ? '1' : '0';
        if (savesign == "1")
        {
        }
    }
    function saveToServer() {
        var data = eSignIn.getdata();
        $.ajax({
            type: 'post',
            url: '?act=save&signid=<%=signid%>&signtokenid=<%=signtokenid%>',
            //data: '{ "imageData" : "' + data + '" }',
            data: { "imageData": data ,savesign:($("#savesign").get(0).checked ? '1' : '0')},
            //contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                //alert("Done, Picture Uploaded.");
                var url = '?act=finish&signid=<%=signid%>&signtokenid=<%=signtokenid%>';
                document.location.assign(url);
            }
        });

    };
    setTimeout(function () { document.getElementById("txt").focus(); }, 500);
   
</script>
<%} %>
</body>
</html>