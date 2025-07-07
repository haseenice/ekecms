using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Plugins
{
    public partial class PathSelect : System.Web.UI.Page
    {
        private string basePath = "";//当前目录
        public string path = eParameters.QueryString("path");
        string aspxFile = eBase.getAspxFileName();
        public string obj = eParameters.QueryString("obj");
        public string siteid = "";
        string sitefolder = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            basePath = eRunTime.fileManagePath;
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            if (path.Contains("..")) path = "";//禁止越权上级目录
            if (path.Length > 0) path = path.toUrlFormat();
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            if (!Directory.Exists(basePath + path.toLocalFormat())) path = "";
            if (path.Length > 0) basePath += path.toLocalFormat() + "\\";
          

            //eBase.Writeln(basePath );
            //eBase.End();

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"filemanage_local\">");
            if (path.Length > 0)
            {
                sb.Append("<a href=\"" + aspxFile + "?obj=" + obj + "\">根目录</a>");
                string[] arr = path.Split("/".ToCharArray());
                string tmp = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i > 0) tmp += "/";
                    tmp += arr[i];
                    if (i == arr.Length - 1)
                    {
                        sb.Append(" / " + arr[i]);
                    }
                    else
                    {
                        sb.Append(" / <a href=\"" + aspxFile + "?obj=" + obj + "&path=" + tmp + "\">" + arr[i] + "</a>");
                    }

                }
            }
            else
            {
                sb.Append("根目录");
            }
            sb.Append("</div>");

            sb.Append("<div class=\"filemanage_files\">");
            System.IO.DirectoryInfo dinfo = new DirectoryInfo(basePath);
            foreach (DirectoryInfo folder in dinfo.GetDirectories())
            {
                sb.Append("<a href=\"" + aspxFile + "?obj=" + obj + "&path=" + (path.Length > 0 ? path + "/" : path) + folder.Name.ToString() + "\" title=\"" + folder.Name + "\">\r\n");
                sb.Append("<dl>\r\n");
                sb.Append("<dt class=\"folder\"></dt>\r\n");
                sb.Append("<dd>" + folder.Name + "</dd>\r\n");
                sb.Append("</dl>\r\n");
                sb.Append("</a>\r\n");
            }
            sb.Append("</div>");
            litBody.Text = sb.ToString();
        }
    }
}