using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using LitJson;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;


namespace eFrameWork.Manage
{
    public partial class Update : System.Web.UI.Page
    {
        public string UserArea = "Manage";
        public string act = eParameters.Request("act");
        public string ModelID = eParameters.Request("ModelID");
        public eUser user;
        private string basePath = "";//当前目录
        public string path = eParameters.QueryString("path");
        string uploadURL = "http://demo.eketeam.com/Update/";
        string ExcludeFolders = "|upload|error|updatelog|";
        string ExcludeFiles = "|web.config|";       
        JsonData dir= JsonMapper.ToObject("{}");
        string aspxFile = eBase.getAspxFileName();      
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser(eBase.getUserArea(UserArea));
            if (eRegisterInfo.Base == 0 && eRegisterInfo.Loaded) return;
            basePath = Server.MapPath("~/");
            if (path.Length > 0) basePath += path.Replace("/", "\\") + "\\";

            switch (act)
            {
                case "":
                    List();
                    break;
                case "updatefile":
                    updateFile();
                    break;
                case "downfile":
                    downNewFile();
                    break;
                case "downfolder":
                    downNewFolder();
                    break;
            }
        }
        private void List()
        {
            LitNav1.Text = "<a href=\"" + aspxFile + "\">根目录</a>";
            if (path.Length > 0)
            {
                string[] arr = path.Split("/".ToCharArray());
                string tmp = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0) tmp += "/";
                    tmp += arr[i];
                    LitNav1.Text += "&nbsp;->&nbsp;<a href=\"" + aspxFile + "?path=" + tmp + "\">" + arr[i] + "</a>";
                }
            }
            dir["path"] = path;
            dir["directory"] = JsonMapper.ToObject("[]");
            appendFiles();

            //eBase.Writeln(dir.ToJson());
            //eBase.Write("<hr>");

            string result = postRequest(uploadURL, dir.ToJson());
            //eBase.Writeln(result);
            if (result.Length > 0 && result.StartsWith("["))
            {

                JsonData json = JsonMapper.ToObject(result);
                // sb.Append(result);
                StringBuilder sb = new StringBuilder();
                if (json.Count == 0)
                {
                    sb.Append("暂无更新!");
                }
                else
                {
                    sb.Append("<ul class=\"upload\">\r\n");
                    foreach (JsonData item in json)
                    {
                        sb.Append("<li class=\"" + (item.getValue("type").IndexOf("folder") > -1 ? "folder" : "file") + "\">");
                        sb.Append("<span style=\"width:220px;\">" + item.getValue("name") + "</span>");
                        switch (item.getValue("type"))
                        {
                            case "folder":
                                sb.Append("<span style=\"width:120px;\">有更新</span>");
                                sb.Append("<a href=\"?path=" + (path.Length == 0 ? "" : path + "/") + item.getValue("name") + "\">查看详细</a>");
                                break;
                            case "newfolder":
                                sb.Append("<span style=\"width:120px;\">新增文件夹</span>");
                                sb.Append("<a href=\"?act=downfolder&folder=" + item.getValue("name") + (path.Length > 0 ? "&path=" + path : "") + "\">下载</a>");
                                break;
                            case "delfolder":
                                sb.Append("<span style=\"width:120px;\">已删除</span>");
                                sb.Append("<a href=\"javascript:;\">删除</a>");
                                break;
                            case "file":
                                sb.Append("<span style=\"width:120px;\">有更新</span>");
                                sb.Append("<a href=\"?act=updatefile&file=" + item.getValue("name") + (path.Length > 0 ? "&path=" + path : "") + "\">下载</a>");
                                break;
                            case "newfile":
                                sb.Append("<span style=\"width:120px;\">新增文件</span>");
                                sb.Append("<a href=\"?act=downfile&file=" + item.getValue("name") + (path.Length > 0 ? "&path=" + path : "") + "\">下载</a>");
                                break;
                            case "delfile":
                                sb.Append("<span style=\"width:120px;\">已删除</span>");
                                sb.Append("<a href=\"javascript:;\">删除</a>");
                                break;
                        }
                        sb.Append("</li>\r\n");

                    }
                    sb.Append("</ul>\r\n");
                }
                litBody.Text = sb.ToString();
            }

        }
        private void addFolder(string folderPath, ZipFile zip)
        {
            zip.AddDirectory(folderPath);
            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach (FileInfo item in di.GetFiles())
            {
                zip.Add(item.FullName);
            }
            foreach (DirectoryInfo item in di.GetDirectories())//遍历出所有目录
            {
                addFolder(item.FullName, zip);
            }
        }
        private void updateFile()
        {
            #region 备份原来文件
            string file = eParameters.QueryString("file");
            string dir = Server.MapPath("~/UpdateLog/");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string zipPath = dir + "upload_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "_log.zip";
            ZipFile zip = ZipFile.Create(zipPath);
            string FromFile = basePath + file;
            zip.NameTransform = new ZipNameTransform(Server.MapPath("~/"));
            zip.BeginUpdate();
            zip.Add(FromFile);
            zip.CommitUpdate();
            zip.Close();
            #endregion
            downNewFile();          
        }
        private void downNewFile()
        {
            string file=eParameters.QueryString("file");
            string url = uploadURL + "?act=downfile" + (path.Length > 0 ? "&path="+path : "") + "&file=" + file;
            string filepath = basePath + file;
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(url, filepath);
                wc.Dispose();
            }
            catch
            { 
            }
            url = aspxFile + (path.Length > 0 ? "?path=" + path : "");
            Response.Redirect(url, true);
        }
        private void downNewFolder()
        {
            string folder = eParameters.QueryString("folder");
            string url = uploadURL + "?act=downfolder" + (path.Length > 0 ? "&path=" + path : "") + "&folder=" + folder;

            string dirPath = Server.MapPath("~/upload/temp/");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string filepath = dirPath + "temp.zip";
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(url, filepath);
                wc.Dispose();
            }
            catch
            {
            }
            if (File.Exists(filepath))
            {
                 new FastZip().ExtractZip(filepath, Server.MapPath("~/"), "");//覆盖
                 try{ File.Delete(filepath); }catch{}
            }
            url = aspxFile + (path.Length > 0 ? "?path=" + path : "");
            Response.Redirect(url, true);
        }
        private void getMaxTime(string folder,ref DateTime dt)
        {
            System.IO.DirectoryInfo dinfo = new DirectoryInfo(basePath + folder + "\\");
            foreach (FileInfo a in dinfo.GetFiles())
            {
                if (a.LastWriteTime > dt) dt = a.LastWriteTime;
            }
            foreach (DirectoryInfo a in dinfo.GetDirectories())
            {
                getMaxTime(folder + "\\" + a.Name, ref dt);
            }
        }
        private void appendFiles()
        {
            System.IO.DirectoryInfo dinfo = new DirectoryInfo(basePath);
            foreach (DirectoryInfo a in dinfo.GetDirectories())
            {

                if (ExcludeFolders.ToLower().IndexOf("|" + a.Name.ToLower() + "|") > -1) continue;
                JsonData jd = new JsonData();
                jd["name"] = a.Name;
                jd["type"] = "folder";
                //jd["time"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", a.LastWriteTime);
                DateTime dt = DateTime.Now.AddYears(-10);
                getMaxTime(a.Name, ref dt);
                jd["time"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
                dir["directory"].Add(jd);
                //eBase.Writeln(a.Name + "::" + string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt));
              
            }
            foreach (FileInfo a in dinfo.GetFiles())
            {
                if (ExcludeFiles.ToLower().IndexOf("|" + a.Name.ToLower() + "|") > -1) continue;
                JsonData jd = new JsonData();
                jd["name"] = a.Name;
                jd["type"] = "file";
                jd["time"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", a.LastWriteTime);
                dir["directory"].Add(jd);
            }
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开
            return true;
        }
        public static string postRequest(string url, string json)
        {
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UserAgent = HttpContext.Current.Request.UserAgent;
            webRequest.Method = "post";
            webRequest.Referer = HttpContext.Current.Request.Url.AbsoluteUri;
            webRequest.ContentLength = bytes.Length;
            webRequest.ContentType = "application/json";
            Stream reqstream = webRequest.GetRequestStream();
            reqstream.Write(bytes, 0, bytes.Length);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }


            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return result;
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Master == null) return;
            Literal lit = (Literal)Master.FindControl("LitTitle");
            if (lit != null)
            {
                lit.Text = "在线更新 - " + eConfig.manageName();
            }
        }
    }
}