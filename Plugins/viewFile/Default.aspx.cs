using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using System.IO;
using EKETEAM.Extension.ComObject;
using System.Reflection;
using System.Text;

namespace eFrameWork.Plugins.ViewFile
{
    public partial class Default : System.Web.UI.Page
    {
        string path = eParameters.QueryString("path");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (path.ToLower().IndexOf("upload") == -1)
            {
                Response.Write("权限受限!");
                Response.End();
            }
            path = path.Substring(path.ToLower().IndexOf("upload/"));;
            eFileInfo fi = new eFileInfo(path);
            string tagfolder = fi.Path;
            //if (tagfolder.ToLower().IndexOf("/temp/") == -1) tagfolder = tagfolder.RegexReplace("upload/", "upload/temp/");
            tagfolder += "Cache/";
            tagfolder += fi.Name + fi.fileExtension.Replace(".", "");
          

            string tagPath = Server.MapPath("~/" + tagfolder);
            if (!Directory.Exists(tagPath))
            {
                if (fi.fileExtension.IndexOf("doc") > -1 || fi.fileExtension.IndexOf("xls") > -1 || fi.fileExtension.IndexOf("ppt") > -1 || fi.fileExtension.IndexOf("pdf") > -1)
                {
                    Directory.CreateDirectory(tagPath);
                }
            }
            string abspath = eBase.getAbsolutePath();
            if (abspath.Length > 1) tagfolder = abspath.Substring(1) +  tagfolder;
            if (File.Exists(tagPath + "\\default.htm")) Response.Redirect("/" + tagfolder + "/default.htm", true);
            string readPath = Server.MapPath("~/" + path);
            string savePath = Server.MapPath("~/" + fi.Path + "Cache/" + fi.Name + fi.fileExtension.Replace(".",""));
            if (!File.Exists(readPath))
            {
                Response.Write("文件不存在或已被删除!");
                Response.End();
            }
            bool result = false;
            #region Word
            if (fi.fileExtension.IndexOf(".doc") > -1)
            {
                Type wbType = Type.GetType("Aspose.Words.Document,Aspose.Words");
                if (wbType == null)
                {
                    LitBody.Text = "文件类型暂不支持在线预览!";
                    Response.End();
                }
                string toPath = tagPath + "\\default.htm";
                result = eFileConvertHelper.docTo(readPath, toPath, "Html");
                if (result) Response.Redirect("/" + tagfolder + "/default.htm", true);
            }
            #endregion
            #region Excel
            if (fi.fileExtension.IndexOf("xls") > -1)
            {             
                Type wbType = Type.GetType("Aspose.Cells.Workbook,Aspose.Cells");
                if (wbType == null)
                {
                    LitBody.Text = "文件类型暂不支持在线预览!";
                    Response.End();
                }
                string toPath= tagPath + "\\default.htm";
                result = eFileConvertHelper.xlsTo(readPath, toPath, "Html");

                if (!result)
                {
                    eFileInfo _fi = new eFileInfo(readPath);
                    toPath = _fi.Path + _fi.Name + ".pdf";
                    result = eFileConvertHelper.xlsTo(readPath, toPath, "Pdf");
                    if (result)
                    {
                        
                        readPath = toPath;
                        toPath = tagPath + "\\default.htm";
                        result = eFileConvertHelper.pdfTo(readPath, toPath, "Html");
                        try
                        {
                            File.Delete(readPath);
                        }
                        catch { }
                    }
                }              
                if(result) Response.Redirect("/" + tagfolder + "/default.htm", true);
            }
            #endregion
            #region PPT
            if (fi.fileExtension.IndexOf("ppt") > -1)
            {
                Type wbType = Type.GetType("Aspose.Slides.Presentation,Aspose.Slides");
                if (wbType == null)
                {
                    LitBody.Text = "文件类型暂不支持在线预览!";
                    Response.End();
                }
                string toPath = tagPath + "\\default.htm";
                result = eFileConvertHelper.pptTo(readPath, toPath, "Html");
                if (result) Response.Redirect("/" + tagfolder + "/default.htm", true);
            }
            #endregion
            #region PDF
            if (fi.fileExtension.IndexOf("pdf") > -1)
            {               
                Type wbType = Type.GetType("Aspose.Pdf.Document,Aspose.Pdf");
                if (wbType == null)
                {
                    LitBody.Text = "文件类型暂不支持在线预览!";
                    Response.End();
                }
                string toPath = tagPath + "\\default.htm";
                result = eFileConvertHelper.pdfTo(readPath, toPath, "Html");
                if ((File.Exists(tagPath + "\\default_files\\img_01.png") && File.Exists(tagPath + "\\default_files\\img_02.svg")) || (File.Exists(tagPath + "\\default_files\\img_01.svg") && File.Exists(tagPath + "\\default_files\\img_02.png")))
                {
                    try
                    {
                        File.Delete(toPath);
                        eFolderHelper.remove(tagPath + "\\default_files");
                    }
                    catch
                    { 
                    }
                    result = eFileConvertHelper.pdfTohtml(readPath, toPath);
                }                
                if (result) Response.Redirect("/" + tagfolder + "/default.htm", true);
            }
            #endregion
            string width = "";
            string height = "";
            if (".css.js.".IndexOf( fi.fileExtension + ".") > -1) //文本文件
            {
                Response.Redirect(eBase.getAbsolutePath() + path, true);
            }
            if (".txt.html.htm.".IndexOf(fi.fileExtension + ".") > -1) //文本文件
            {
                string body = eBase.ReadFile(Server.MapPath("~/" + path));
                body = HttpUtility.HtmlEncode(body);
                body = body.Replace("\n", "<br>");
                LitBody.Text = "<div style=\"text-align:left;vertical-align:top;position:fixed;top:0px;left:0px;margin:10px;line-height:22px;font-size:13px;\">" + body + "</div>";
                return;
            }
            if (".bmp.gif.jpg.jpeg.png.tif.".IndexOf( fi.fileExtension + ".") > -1) //图片文件
            {
                LitBody.Text = "<img src=\"" + eBase.getAbsolutePath() + path + "\" />";
                return;
            }
            if (".avi.flv.mkv.mpg.mp4.ts.mov.mxf.".IndexOf( fi.fileExtension + ".") > -1)
            {
                width = eBase.IsMobile() ? "98%" : "600";
                height = eBase.IsMobile() ? "240" : "480";
                LitBody.Text = "<video src=\"" + eBase.getAbsolutePath() + path + "\" width=\"" + width + "\" height=\"" + height + "\" controls=\"controls\" style=\"display:block;object-fit: cover;background-color:#f5f5f5;\"></video>";
                return;
            }
            if (".mp3.wav.wma.".IndexOf(fi.fileExtension + ".") > -1)
            {
                LitBody.Text = "<audio src=\"" + eBase.getAbsolutePath() + path + "\" controls=\"controls\" width=\"400\"  style=\"display:block;\" ></audio>";
                return;
            }

            LitBody.Text = "文件类型暂不支持在线预览!";
        }
        
    }
}