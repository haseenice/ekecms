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
    public partial class Articles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Pub.checkPC();//不是PC访问则跳转到移动版
            List();
        }
        private void List()
        {
            string id = eParameters.QueryString("id");
            string pid = eParameters.QueryString("pid");
            eList list = new eList("eWeb_Articles");
            list.Fields.Add("ArticleID,bt,addTime,url,CASE WHEN DATEDIFF(d,addTime,GETDATE()) < 7 THEN '<img src=\"images/new.gif\">' ELSE '' END as newimg");
            list.Where.Add("ColumnID=" + id);
            if (pid.Length > 0) list.Where.Add("ClassID in (" + pid + ",(select ClassID from eWeb_Class where ParentClassID=" + pid + " and delTag=0))");
            list.Where.Add("WebCode='" + eBase.getWebCode() + "'");
            list.Where.Add("delTag=0 and show=1");
            list.OrderBy.Add("ZD desc,px,ArticleID desc");
            list.Bind(RepList, ePageControl1);
           
        }
    }
}