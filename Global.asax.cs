using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using System.Threading;

namespace eFrameWork
{
    public partial class Global : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (eConfig.Debug()) eBase.Writeln("BeginRequest");
            Application["StartTime"] = System.DateTime.Now;
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Application["StartTime"] != null)
            {
                System.DateTime startTime = (System.DateTime)Application["StartTime"];
                System.DateTime endTime = System.DateTime.Now;
                System.TimeSpan ts = endTime - startTime;
                //Response.Write("<br>页面执行时间:" + ts.TotalMilliseconds + " 毫秒"); //测试完指定页面后请注释，否则影响有些输出结果(如：json)
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            //eBase.AppendLog("Start");
        }
        protected void Application_End(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;
            if (HttpContext.Current.Request.Url.Port != 80 && HttpContext.Current.Request.Url.Port != 443) url += ":" + HttpContext.Current.Request.Url.Port.ToString();
            url += "/Plugins/FontIco.aspx";
            eBase.getRequest(url);
        }
    }
}