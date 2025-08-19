using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Customs.Base
{
    public partial class CheckupRecords : System.Web.UI.Page
    {
        string AppItem = eParameters.QueryString("AppItem");
        string parentModelID = eParameters.QueryString("modelid");
        string parentID = eParameters.QueryString("id");
        string modelid = eParameters.QueryString("modelid");
        public string act = eParameters.QueryString("act");
        private eDataBase _database;
        private eDataBase DataBase
        {
            get
            {
                if (_database == null)
                {
                    DataRow[] rows;
                    if (AppItem.Length > 0)
                    {
                        rows = eBase.a_eke_sysApplicationItems.Select("ApplicationItemID='" + AppItem + "'");
                        if (rows.Length > 0) modelid = rows[0]["ModelID"].ToString();
                    }
                    rows = eBase.a_eke_sysModels.Select("ModelID='" + modelid + "'");
                    if (rows.Length > 0)
                    {
                        if (rows[0]["DataSourceID"].ToString().Length > 0)
                        {
                            _database = new eDataBase(rows[0]);
                            string pk = _database.getPrimaryKey("a_eke_sysCheckupRecords");
                            if (pk.Length == 0) _database = eConfig.DefaultDataBase;
                            //eBase.Writeln("1234:" + pk);
                        }
                        else
                        {
                            _database = eConfig.DefaultDataBase;
                        }
                    }
                    else
                    {
                        _database = eConfig.DefaultDataBase;
                    }
                }
                return _database;
            }
        }
        private string _columntitlewidth;
        public string ColumnTitleWidth
        {
            get
            {
                if (_columntitlewidth == null)
                {
                    DataRow[] rs = eBase.a_eke_sysModels.Select("ModelID='" + modelid + "'");
                    if (rs.Length > 0)
                    {
                        _columntitlewidth = rs[0]["mcolumntitlewidth"].ToString();
                    }
                    else
                    {
                        _columntitlewidth = "80";
                    }
                }
                return _columntitlewidth;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            AppItem = eParameters.QueryString("AppItem");
            parentModelID = eParameters.QueryString("modelid");
            parentID = eParameters.QueryString("id");
            modelid = eParameters.QueryString("modelid");

            if ((parentModelID.Length > 0 || AppItem.Length > 0) && parentID.Length > 0)
            {
                eList elist = new eList("a_eke_sysCheckupRecords");
                elist.DataBase = DataBase;
                elist.Where.Add("CheckupContentId='" + parentID + "'");
                if (elist.ColumnCollection.ContainsKey("SignField")) elist.Where.Add("SignField is null");
                //if (modelid.Length > 0) elist.Where.Add("modelid='" + modelid + "'");
                elist.OrderBy.Add("addTime");
                if (Context.Items["LogData"] != null)
                {
                    DataSet ds = Context.Items["LogData"] as DataSet;
                    DataTable dt=new DataTable();
                    if (ds.Tables.Contains("a_eke_sysCheckupRecords"))
                    {
                        dt = ds.Tables["a_eke_sysCheckupRecords"];
                    }
                    elist.DataSource = dt;
                }
                if (eBase.IsMobile())
                {
                    eListControl1.Visible = false;
                    RepMobile.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
                    elist.Bind(RepMobile);
                    if (elist.RecordsCount == 0)
                    {
                        Rep.Visible = false;
                        Response.Write("<div class=\"noRecordTip\">暂无审核记录!</div>");
                    }
                }
                else
                {

                    eListControl1.Visible = false; 
                    //elist.Bind(eListControl1);
                    Rep.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
                    elist.Bind(Rep);
                    if (elist.RecordsCount == 0)
                    {
                        Rep.Visible = false;
                        Response.Write("<div class=\"noRecordTip\">暂无审核记录!</div>");
                    }
                }
            }
            string act = eParameters.QueryString("act");
            if (act == "add")
            {
               // Response.Write("暂无审核记录!");
            }
        }
        protected void Rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                string userid = DataBinder.Eval(e.Item.DataItem, "UserID").ToString();
                Control ctrl = e.Item.Controls[0];
                Literal lit = (Literal)ctrl.FindControl("LitUser");
                if (lit != null && userid.Length>0)
                {
                    lit.Text = DataBase.getValue("select xm from a_eke_sysUsers where UserId='" + userid + "'");
                }
            }
        }
    }
}