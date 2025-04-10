using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using System.Text.RegularExpressions;

namespace EKECMS
{
    public partial class Default : System.Web.UI.Page
    {
        public eSiteInfo siteinfo = new eSiteInfo();
        public DataRow articles, content, products, cases;
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版
            ((EKECMS.Master.Home)Master).SiteInfo = siteinfo;
            siteinfo.BT = "首页-";
            eList list;
            #region 简介
            string sql = "select top 1 ColumnID,MC from eWeb_Columns where Type=1 and WebCode='" + eBase.getWebCode() + "' and delTag=0 and show=1 and ShowIndex=1";
            
            DataTable dt = eBase.DataBase.getDataTable(sql);
            if (dt.Rows.Count > 0)
            { 
                content = dt.Rows[0];
                LitIntroduce.Text = eBase.DataBase.getValue("select JJ from eWeb_Columns where ColumnID='" + dt.Rows[0]["ColumnID"].ToString() + "'");
            }
            #endregion
            #region 文章
            sql = "select top 1 ColumnID,MC from eWeb_Columns where Type=2 and WebCode='" + eBase.getWebCode() + "' and delTag=0 and show=1 and ShowIndex=1";
            dt = eBase.DataBase.getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                //DataTable Articles = eOleDB.getDataTable("select top 8 ArticleID,BT from eWeb_Articles where SiteID=" + Pub.getSiteID() + " and Show=1 and delTag=0 order by px,ArticleID desc");
                //RepArticles.DataSource = Articles;
                //RepArticles.DataBind();

                articles = dt.Rows[0];
                list = new eList("eWeb_Articles");
                list.Fields.Add("ArticleID,BT");
                list.Rows = 8;
                list.Where.Add("WebCode='" + eBase.getWebCode() + "' and Show=1 and delTag=0 and SYZS=1");
                list.Where.Add("ColumnID='" + dt.Rows[0]["ColumnID"].ToString() + "'");
                list.OrderBy.Add("ZD desc,px,ArticleID desc");

                list.Bind(RepArticles);
            }
            #endregion
            #region 产品
            sql = "select top 1 ColumnID,MC from eWeb_Columns where Type=3 and WebCode='" + eBase.getWebCode() + "' and delTag=0 and show=1 and ShowIndex=1";
            dt = eBase.DataBase.getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                products = dt.Rows[0];
                list = new eList("eWeb_Products");
                list.Fields.Add("ProductID,CPMC,GGXH,XTP");
                list.Rows = 8;
                list.Where.Add("WebCode='" + eBase.getWebCode() + "' and Show=1 and delTag=0 and SYZS=1");
                list.Where.Add("ColumnID='" + dt.Rows[0]["ColumnID"].ToString() + "'");
                list.OrderBy.Add("ZD desc,px,ProductID desc");
                list.Bind(RepProducts);
            }
            #endregion
            #region 案例
            sql = "select top 1 ColumnID,MC from eWeb_Columns where Type=5 and WebCode='" + eBase.getWebCode() + "' and delTag=0 and show=1 and ShowIndex=1";
            dt = eBase.DataBase.getDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                cases = dt.Rows[0];
                list = new eList("eWeb_Cases");
                list.Fields.Add("CaseID,BT,XTP");
                list.Rows = 12;
                list.Where.Add("WebCode='" + eBase.getWebCode() + "' and Show=1 and delTag=0 and SYZS=1");
                list.Where.Add("ColumnID='" + dt.Rows[0]["ColumnID"].ToString() + "'");
                list.OrderBy.Add("ZD desc,px,CaseID desc");
                list.Bind(RepCases);
            }
            #endregion
            #region 友情连接
            eList textLink = new eList("eWeb_Links");
            textLink.Fields.Add("LinkID,MC,URL");
            textLink.Where.Add("WebCode='" + eBase.getWebCode() + "' and Type=0 and show=1");
            textLink.Where.Add("delTag=0");
            textLink.OrderBy.Add("px,LinkID");
            textLink.Bind(RepTextLinks);

            DataTable picLink = eBase.DataBase.getDataTable("select LinkID,MC,Url,Pic from eWeb_Links where WebCode=" + eBase.getWebCode() + " and Type=1 and Show=1 and delTag=0 order by px,LinkId");
            RepPicLinks.DataSource = picLink;
            RepPicLinks.DataBind();
            #endregion
        }
    }
}