using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace eFrameWork.Manage
{
    public partial class ModelItems_FillData : System.Web.UI.Page
    {
        private eUser user;
        public string modelid = eParameters.QueryString("modelid");
        public string parentid = eParameters.QueryString("parentid");
        private DataTable _datas;
        private DataTable Datas
        {
            get
            {
                if (_datas == null)
                {
                    //string sql = "select ModelItemID,Code + ' (' + MC + ')' as MC from a_eke_sysModelItems where delTag=0 and ModelID='" + modelid + "' and Custom=0 and (SYS=0 or primaryKey=1) order by px,addTime";
                    //_datas = eBase.DataBase.getDataTable(sql);

                    _datas = getItems2(modelid);
                    appendItems2(_datas, modelid);
                }
                return _datas;
            }
        }
        //回填模块
        private DataTable getItems2(string modelid)
        {
            //eBase.Writeln(modelid);
            return eBase.DataBase.getDataTable("select b.mc as ModelName,b.addtime as TT,a.* from a_eke_sysModelItems a inner join a_eke_sysModels b on a.modelid=b.modelid where a.ModelID='" + modelid + "' and a.delTag=0 and (a.Custom=0 or len(a.customcode)>0)");
        }
        private void appendItems2(DataTable tb, string modelid)
        {
            DataTable dt = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ParentID='" + modelid + "' and JoinMore=0 and show=1 and deltag=0");
            foreach (DataRow dr in dt.Rows)
            {
                DataTable tb2 = getItems2(dr["modelid"].ToString());
                foreach (DataRow _dr in tb2.Rows)
                {
                    tb.Rows.Add(_dr.ItemArray);
                }
                appendItems2(tb, dr["modelid"].ToString());
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser("Manage");
            user.Check();


            //eBase.PrintDataTable(Datas);
            List();
            if(eParameters.QueryString("act")=="sub")
            {

                System.IO.StringWriter sw = new System.IO.StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                Rep.RenderControl(htw);
                Rep.Visible = false;
                Response.Write(sw.ToString());
                sw.Close();
                Response.End();
           }
        }

        private void List()
        {
            DataTable tb = getItems(parentid);
            appendItems(tb, parentid);
            tb = tb.Select("", "TT,PX, addTime").toDataTable();
            Rep.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
            Rep.DataSource = tb;
            Rep.DataBind();
        }
        //当前模块
        private DataTable getItems(string modelid)
        {
            return eBase.DataBase.getDataTable("select b.mc as ModelName,b.addtime as TT,a.* from a_eke_sysModelItems a inner join a_eke_sysModels b on a.modelid=b.modelid where a.ModelID='" + modelid + "' and a.delTag=0 and a.showAdd=1 and (a.Custom=0 or len(a.customcode)>0)");
        }
        private void appendItems(DataTable tb, string modelid)
        {
            DataTable dt = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ParentID='" + modelid + "' and JoinMore=0 and show=1 and deltag=0");
            foreach (DataRow dr in dt.Rows)
            {
                DataTable tb2 = getItems(dr["modelid"].ToString());
                foreach (DataRow _dr in tb2.Rows)
                {
                    tb.Rows.Add(_dr.ItemArray);
                }
                appendItems(tb, dr["modelid"].ToString());
            }
        }

        protected void Rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Control ctrl = e.Item.Controls[0];
                Literal lit = (Literal)ctrl.FindControl("LitColumns");
                if (lit != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow dr in Datas.Select("","TT,PX,addtime"))
                    {
                        sb.Append("<option value=\"" + dr["ModelItemID"].ToString() + "\"" + (DataBinder.Eval(e.Item.DataItem, "FillModelItemID").ToString().IndexOf(dr["ModelItemID"].ToString()) > -1 ? " selected=\"true\"" : "") + ">" + dr["Code"].ToString() + "(" + dr["MC"].ToString() + ") - " + dr["ModelName"].ToString() + "</option>\r\n");
                    }
                    lit.Text = sb.ToString();

                }
            }
        }
    }
}