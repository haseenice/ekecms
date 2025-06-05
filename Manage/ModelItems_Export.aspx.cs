using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace eFrameWork.Manage
{
    public partial class ModelItems_Export : System.Web.UI.Page
    {
        public string modelid = eParameters.QueryString("modelid");
        private eUser user;
        private int modelpx = 0;
        private DataRow _modelinfo;
        public DataRow ModelInfo
        {
            get
            {
                if (_modelinfo == null)
                {
                    DataTable dt = eBase.DataBase.getDataTable("select * from a_eke_sysModels where ModelID='" + modelid + "'");
                    if (dt.Rows.Count > 0) _modelinfo = dt.Rows[0];
                }
                return _modelinfo;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser("Manage");
            user.Check();
            Response.Write("<a href=\"http://help.eketeam.com/1/104.html\" style=\"float:right;\" target=\"_blank\" title=\"eFrameWork开发框架\"><img src=\"images/help.gif\"></a>");
            
            if (eConfig.showHelp())
            {
                Response.Write("<div class=\"tips\" style=\"margin-bottom:8px;\">");
                Response.Write("<b>导出</b><br>");
                Response.Write("设置模块导出到Excel的列及宽度。");
                Response.Write("</div> ");
            }

            
            Response.Write("<span class=\"eform\">导出Word模板：");
            Response.Write("<input etype=\"fileselect\" type=\"text\" name=\"wordTemplate\" id=\"wordTemplate\" readonly=\"true\" value=\"" + ModelInfo["wordTemplate"].ToString() + "\" style=\"width:300px;\" class=\"text\" onclick=\"fileLibrary_select('wordTemplate','','upload');\" autocomplete=\"off\">");
            Response.Write("<input type=\"button\" name=\"btnfileselect\" class=\"btnfileselect\" onclick=\"fileLibrary_select('wordTemplate','','upload');\" value=\"选择\">");
            Response.Write("<input type=\"button\" name=\"btnfileclear\" class=\"btnfileselect\" onclick=\"fileorpath_clear('wordTemplate');\" value=\"清除\">");

            Response.Write("&nbsp;&nbsp;导出Excel模板：");
            Response.Write("<input etype=\"fileselect\" type=\"text\" name=\"excelTemplate\" id=\"excelTemplate\" readonly=\"true\" value=\"" + ModelInfo["excelTemplate"].ToString() + "\" style=\"width:300px;\" class=\"text\" onclick=\"fileLibrary_select('excelTemplate','','upload');\" autocomplete=\"off\">");
            Response.Write("<input type=\"button\" name=\"btnfileselect\" class=\"btnfileselect\" onclick=\"fileLibrary_select('excelTemplate','','upload');\" value=\"选择\">");
            Response.Write("<input type=\"button\" name=\"btnfileclear\" class=\"btnfileselect\" onclick=\"fileorpath_clear('excelTemplate');\" value=\"清除\">");

            Response.Write("</span>");


            /*
            eList datalist = new eList("a_eke_sysModelItems");
            datalist.Where.Add("ModelID='" + modelid + "' and delTag=0");
            datalist.OrderBy.Add("showExport desc,ExportOrder,addTime");
            datalist.Bind(Rep);
            */
            list();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Rep.RenderControl(htw);
            Rep.Visible = false;
            Response.Write(sw.ToString());
            sw.Close();
        }
        private void list()
        {
            DataTable tb = getItems(modelid);
            appendItems(tb, modelid);
            string pid = eParameters.QueryString("modelid");
            for (int i = tb.Rows.Count - 1; i > -1; i--)
            {
                if (!eBase.parseBool(tb.Rows[i]["showExport"].ToString()))
                {
                    tb.Rows[i]["ExportOrder"] = 999999;
                }
                if (tb.Rows[i]["modelid"].ToString() != pid)
                {
                    if (",addTime,addUser,editTime,editUser,delTime,delUser,delTag,CheckupCode,CheckupText,".ToLower().Contains("," + tb.Rows[i]["Code"].ToString().ToLower() + ","))
                    {
                        tb.Rows.Remove(tb.Rows[i]);
                    }
                }
               
            }
            tb = tb.Select("", "showExport desc,ExportOrder,modelpx,addTime").toDataTable();
            Rep.DataSource = tb;
            Rep.DataBind();
        }
        private DataTable getItems(string modelid)
        {
            modelpx++;
            return eBase.DataBase.getDataTable("select " + modelpx.ToString() + " as modelpx,b.mc as ModelName,a.* from a_eke_sysModelItems a inner join a_eke_sysModels b on a.modelid=b.modelid where a.ModelID='" + modelid + "' and a.delTag=0");
        }
        private void appendItems(DataTable tb, string modelid)
        {
            DataTable dt = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ParentID='" + modelid + "' and JoinMore=0 and JoinType>0 and show=1 and deltag=0");
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
    }
}