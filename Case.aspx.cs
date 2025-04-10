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
    public partial class Case : System.Web.UI.Page
    {
        public string nr = "";
        public string bigimg = "";
        public string smimg = "";
        public string ct = "0";
        public eSiteInfo siteinfo = new eSiteInfo();
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版

            ((EKECMS.Master.Site)Master).SiteInfo = siteinfo;
            #region 安全性检查
            if (Request.QueryString["id"] == null) Response.Redirect("Default.aspx", true);
            string id = Request.QueryString["id"].ToString();
            #endregion

            siteinfo.BT = siteinfo.Data["bt"].ToString() + "-";

            nr = siteinfo.Data["xxnr"].ToString().replaceKeyWord(siteinfo.KeyWord).replaceAbsPath().handlePlayer();




            eBase.DataBase.Execute("update " + Pub.getTablePrefix() + "Cases set ckcs=ckcs+1 where CaseID=" + id);

            DataTable tb = eBase.DataBase.getDataTable("select bt,jj,xtp,dtp from " + Pub.getTablePrefix() + "CaseItems where CaseID=" + siteinfo.ID + " and delTag=0 order by case when px = 0 then 50000 else px end,CaseItemID");
            ct = tb.Rows.Count.ToString();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                sb.Append("<dl>\r\n");
                sb.Append("<dt><img src=\"" + tb.Rows[i]["dtp"].ToString() + "\" alt=\"" + tb.Rows[i]["bt"].ToString() + "\" /></dt>\r\n");
                sb.Append("<dd>" + tb.Rows[i]["jj"].ToString() + "</dd>\r\n");
                sb.Append("</dl>\r\n");
            }
            bigimg = sb.ToString();
            sb = new StringBuilder();
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                sb.Append("<li rel='" + (i + 1) + "'><img src=\"" + tb.Rows[i]["xtp"].ToString() + "\" alt=\"" + tb.Rows[i]["bt"].ToString() + "\" /></li>\r\n");
            }
            smimg = sb.ToString();

        }
        
    }
}