<%@ webhandler Language="C#" class="getData" %>
using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Globalization;
using System.Configuration;
using System.Text.RegularExpressions;
using EKETEAM.Data;
using EKETEAM.UserControl;
using EKETEAM.FrameWork;

public class getData : IHttpHandler
{
     public bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
     {
            //直接确认，否则打不开
            return true;
     }
    private string getHTML(string url)
    {
         if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            }
         System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
         webRequest.UserAgent = HttpContext.Current.Request.UserAgent;
         webRequest.Method = "get";
         //webRequest.AllowAutoRedirect = false;//设置不自动跳转
         //webRequest.KeepAlive = false;
         //webRequest.Timeout = 10000;
        

         System.Net.HttpWebResponse response;
         try
         {
             response = (System.Net.HttpWebResponse)webRequest.GetResponse();
             System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
             if (response.CharacterSet.Trim().Length > 0)
             {
                 switch (response.CharacterSet.ToLower())
                 {
                     case "gbk":
                         sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);// Encoding.GetEncoding("GBK");
                         break;
                     case "gb2312":
                         sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//Encoding.GetEncoding("GB2312");
                         break;
                     case "iso-8859-1":
                         sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                         break;                    
                 }
             }
             string result = sr.ReadToEnd();
             sr.Close();
             sr.Dispose();
             return result;
         }
         catch (System.Net.WebException ex)
         {
             //response = (System.Net.HttpWebResponse)ex.Response;
         }
         return "";
    }
	public void ProcessRequest(HttpContext context)
	{
        string url = eParameters.Form("url");
        if (url.ToLower().StartsWith("http"))
        {
            string html = getHTML(url);
            string title = "";
            //Match ma = Regex.Match(html, @"<title>([\s\S]*?)(?=</title>)", RegexOptions.IgnoreCase);
            //<title>[sS]*?</title>
            //<title>.*?</title>
            Match ma = Regex.Match(html, @"<title>([\s\S]*?)</title>", RegexOptions.IgnoreCase);
            if (ma.Success)
            {
                title = ma.Groups[1].Value.Trim();
                eResult.Message(new { success = 1, errcode = "0", title = title });
            }

            eResult.Message(new { success = 1, errcode = "1", title = title });
           
        }
        eResult.Error("请求失败!");
	}
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
