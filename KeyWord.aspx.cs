using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using System.Text.RegularExpressions;

namespace EKECMS
{
    public partial class KeyWord : System.Web.UI.Page
    {

        private eDataBase db;
        private eDataBase DataBase
        {
            get
            {
                if (db == null) db = new eDataBase();
                return db;
            }
        }
        public string Title = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            string id = eParameters.QueryString("id");
            string type = eParameters.QueryString("type");
            if (type.Length == 0) type = "1";
            string sql = "";
            if (type == "1")
            {
                DataBase.Execute("update eWeb_KeyWords set ViewCount=ViewCount+1 where KeyWordID=" + id);
                sql = "select * from eWeb_KeyWords where KeyWordID=" + id;
            }
            else
            {
                DataBase.Execute("update Help_Items set ViewCount=ViewCount+1 where HelpItemID=" + id);
                sql = "select * from Help_Items where HelpItemID=" + id;
            }
            DataTable tb = DataBase.getDataTable(sql);
            if (tb.Rows.Count > 0)
            {
                Title = tb.Rows[0]["Name"].ToString();
                string body = tb.Rows[0]["Body"].ToString();
                body = Regex.Replace(body, "upload/", eBase.getAbsolutePath() + "upload/", RegexOptions.IgnoreCase);
                body = Regex.Replace(body, @"('|"")([/]?)(images)/", "$1" + eRunTime.absRoot + "$3/", RegexOptions.IgnoreCase);
                body = eBase.handlePlayer(body);
                LitBody.Text = body;
            }
            else
            {
                LitBody.Text = "暂无相关信息!";
            }
        }
    }
}