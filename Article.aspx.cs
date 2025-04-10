using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace EKECMS
{
    public partial class Article : System.Web.UI.Page
    {
        public string nr = "";
        public string syp = "";
        public string xyp = "";

        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            ((EKECMS.Master.Site)Master).SiteInfo = siteinfo;
            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            string id = Request.QueryString["id"].ToString();
            #endregion

            siteinfo.BT = siteinfo.Data["BT"].ToString() + "-";
         

            string ColumnID = siteinfo.Data["ColumnID"].ToString();
            string ClassID = siteinfo.Data["ClassID"].ToString();
            nr = siteinfo.Data["NR"].ToString().replaceKeyWord(siteinfo.KeyWord).replaceAbsPath().handlePlayer();


            eBase.DataBase.Execute("update " + Pub.getTablePrefix() + "Articles set ckcs=ckcs+1 where  ArticleID=" + id);

            #region 上一篇
            DataTable tb = eBase.DataBase.getDataTable("select ArticleID,bt from " + Pub.getTablePrefix() + "Articles where ArticleID in (select top 1 ArticleID from " + Pub.getTablePrefix() + "Articles where " + (ClassID.Length > 0 ? "ClassID=" + ClassID + " and " : "") + "ColumnID=" + ColumnID + " and delTag=0 and ArticleID>" + id + " order by px,ArticleID)");//上一篇
            if (tb.Rows.Count == 1)
            {
                syp = "<a href=\"article.aspx?id=" + tb.Rows[0]["ArticleID"].ToString() + "\">" + tb.Rows[0]["bt"].ToString() + "</a>";
            }
            else
            {
                syp = "<font color=\"#666666\">没有了</font>";
            }
            #endregion
            #region 下一篇
            tb = eBase.DataBase.getDataTable("select ArticleID,bt from " + Pub.getTablePrefix() + "Articles where ArticleID in (select top 1 ArticleID from " + Pub.getTablePrefix() + "Articles where " + (ClassID.Length > 0 ? "ClassID=" + ClassID + " and " : "") + "ColumnID=" + ColumnID + " and delTag=0 and ArticleID<" + id + " order by px, ArticleID desc)");//上一篇
            if (tb.Rows.Count == 1)
            {
                xyp = "<a href=\"article.aspx?id=" + tb.Rows[0]["ArticleID"].ToString() + "\">" + tb.Rows[0]["bt"].ToString() + "</a>";
            }
            else
            {
                xyp = "<font color=\"#666666\">没有了</font>";
            }
            #endregion
        }
        
    }
}