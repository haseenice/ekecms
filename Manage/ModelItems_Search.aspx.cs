using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using LitJson;
using EKETEAM.ePinyin;

namespace eFrameWork.Manage
{
    public partial class ModelItems_Search : System.Web.UI.Page
    {
        public string modelid = eParameters.QueryString("modelid");
        public string ModelID
        {
            get
            {
                return modelid;
            }
        }
        private DataRow _modelinfo;
        public DataRow ModelInfo
        {
            get
            {
                if (_modelinfo == null)
                {
                    DataTable dt = eBase.DataBase.getDataTable("select * from a_eke_sysModels where ModelID='" + ModelID + "'");
                    if (dt.Rows.Count > 0) _modelinfo = dt.Rows[0];
                }
                return _modelinfo;
            }
        }

        /// <summary>
        /// 主库或扩展库
        /// </summary>
        private eDataBase _database;
        private eDataBase DataBase
        {
            get
            {
                if (_database == null)
                {
                    if (ModelInfo["DataSourceID"].ToString().Length > 0)
                    {
                        _database = new eDataBase(ModelInfo);
                    }
                    else
                    {
                        _database = eConfig.DefaultDataBase;
                    }
                }
                return _database;
            }
        }
        private DataTable _alltables;//所有表
        private DataTable AllTables2
        {
            get
            {
                if (_alltables == null)
                {
                    string sql = "SELECT id,name FROM sysobjects where (xtype='U' or xtype='V') "; //name!='dtproperties' and 
                    //sql += " and charindex('a_eke_sys',lower(name))=0 and name not in (" + eBase.getSystemTables() + ")";
                    sql += " and (charindex('a_eke_sys',lower(name))=0 or lower(name)='a_eke_sysusers' or lower(name)='a_eke_sysroles' or lower(name)='a_eke_sysmodels')";
                    sql += " and (name not in (" + eBase.getSystemTables() + ") or  lower(name)='a_eke_sysmodels' or lower(name)='a_eke_sysroles')";
                    sql += " order by name";//crdate";
                    //_alltables = DataBase.getDataTable(sql);
                    _alltables = DataBase.getSchemaTableViews();
                }
                return _alltables;
            }
        }       
        public string getJsonText(string jsonstr, string name)
        {
            StringBuilder sb = new StringBuilder();
            if (jsonstr.Length > 0)
            {
                /*
                eJson json = new eJson(jsonstr);
                foreach (eJson m in json.GetCollection())
                {
                    sb.Append("<span style=\"display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;\">" + HttpUtility.HtmlDecode(m.GetValue(name)) + "</span>");
                }
                */
                JsonData json = jsonstr.ToJsonData();
                foreach (JsonData m in json)
                {
                    sb.Append("<span style=\"display:inline-block;margin-right:6px;border:1px solid #ccc;padding:3px 12px 3px 12px;\">" + HttpUtility.HtmlDecode(m.getValue(name)) + "</span>");
                }
            }
            return sb.ToString();
        }

        private DataTable _columns;
        private DataTable getColumns2()
        {
            if (_columns == null)
            {
                _columns = DataBase.getSchemaColumns(ModelInfo["code"].ToString());
                //eBase.Writeln(tableName + "::" + ModelInfo["DataSourceID"].ToString() + "::" + DataBase.Connection.Database);
                //eBase.PrintDataTable(_columns);
              //  eBase.End();
                DataTable dt = eBase.DataBase.getDataTable("select Code,MC from a_eke_sysModelItems where ModelID='" + modelid + "' and delTag=0 and Custom=0");
               // eBase.PrintDataTable(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] rows = _columns.Select("COLUMN_NAME='" + dr["Code"].ToString() + "'");
                    if (rows.Length > 0) rows[0]["DESCRIPTION"] = dr["mc"].ToString();
                }
               
            }
            return _columns;
        }
        private DataTable _modelconditions;
        private DataTable ModelConditions
        {
            get
            {
                if (_modelconditions == null)
                {
                    _modelconditions = eBase.DataBase.getDataTable("select * from a_eke_sysModelConditions where delTag=0 and ModelID='" + modelid + "' order by px");
                }
                return _modelconditions;
            }
        }
        private eUser user;
        private DataTable _allmodels;
        public DataTable allModels
        {
            get
            {
                if (_allmodels == null)
                {
                    _allmodels = eBase.DataBase.getDataTable("select * from a_eke_sysModels where delTag=0");
                }
                return _allmodels;
            }
        }

        private DataTable _allcolumns;
        private DataTable allColumns
        {
            get
            {
                if (_allcolumns == null)
                {
                    _allcolumns = getColumns(modelid);
                    appendColumns(_allcolumns, modelid);
                }
                return _allcolumns;
            }
        }
        private DataTable getColumns(string modelid)
        {
            string pid = eParameters.QueryString("modelid");
            //eBase.Writeln(modelid);
            string tablename = eBase.DataBase.getValue("select code from a_eke_sysModels where ModelID='" + modelid + "'");
            string modelname = eBase.DataBase.getValue("select mc from a_eke_sysModels where ModelID='" + modelid + "'");
            DataTable columns = DataBase.getSchemaColumns(tablename).Select("COLUMN_NAME,DESCRIPTION", "", "ORDINAL_POSITION");
            if (pid != modelid)
            {
                for (int i = columns.Rows.Count - 1; i > -1; i--)
                {
                    if (",addTime,addUser,editTime,editUser,delTime,delUser,delTag,CheckupCode,CheckupText,".ToLower().Contains("," + columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() + ","))
                    {
                        columns.Rows.Remove(columns.Rows[i]);
                    }
                }
                //eBase.PrintDataTable(columns);
            }
            //eBase.PrintDataTable(DataBase.getSchemaColumns(tablename));
            columns.Columns.Add("ModelID", typeof(string));
            columns.Columns.Add("ModelName", typeof(string));
            //columns.Columns["TABLE_NAME"].MaxLength = 200;
            foreach (DataRow dr in columns.Rows)
            {
                dr["ModelID"] = modelid;
                dr["ModelName"] = modelname;
            }

            DataTable dt = eBase.DataBase.getDataTable("select Code,MC from a_eke_sysModelItems where ModelID='" + modelid + "' and delTag=0 and Custom=0");
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    DataRow[] rows = columns.Select("COLUMN_NAME='" + dr["Code"].ToString() + "'");
                    if (rows.Length > 0) rows[0]["DESCRIPTION"] = dr["mc"].ToString();
                }
                catch (Exception ex)
                {
                    //eBase.Writeln( dr["Code"].ToString() + "::" + ex.Message.ToString() );
                }
            }
            return columns;
        }
        private void appendColumns(DataTable tb, string modelid)
        {
            DataTable dt = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ParentID='" + modelid + "' and JoinMore=0 and JoinType>0 and show=1 and deltag=0");
            foreach (DataRow dr in dt.Rows)
            {
                DataTable tb2 = getColumns(dr["modelid"].ToString());
                foreach (DataRow _dr in tb2.Rows)
                {
                    tb.Rows.Add(_dr.ItemArray);
                }
                appendColumns(tb, dr["modelid"].ToString());
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           //DataTable  _allcolumns2 = getColumns(modelid);
           //eBase.PrintDataTable(_allcolumns2);
           //eBase.Writeln(DataBase.ConnectionString);
            user = new eUser("Manage");
            user.Check();
            Response.Write("<a href=\"http://help.eketeam.com/1/104.html\" style=\"float:right;\" target=\"_blank\" title=\"eFrameWork开发框架\"><img src=\"images/help.gif\"></a>");


            //eBase.PrintDataTable(getColumns());
            //eBase.PrintDataTable(allColumns);
            //eBase.PrintDataTable(ModelConditions);
            if (eConfig.showHelp())
            {
                Response.Write("<div class=\"tips\" style=\"margin-bottom:6px;\">");
                Response.Write("<b>搜索</b><br>");
                Response.Write("设置列表搜索条件。");
                Response.Write("</div> ");
            }

            Response.Write("<div style=\"margin:6px 0px 8px 0px;\">");
            Response.Write("&nbsp;搜索列数：<input type=\"text\" value=\"" + ModelInfo["searchcolumncount"].ToString() + "\" oldvalue=\"" + ModelInfo["searchcolumncount"].ToString() + "\"  class=\"edit\" style=\"width:40px;\" onBlur=\"setModel(this,'searchcolumncount');\" />");
            if (eConfig.showHelp()) Response.Write(" <img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(107);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;margin-bottom:4px;\">");

            eBase.Write("&nbsp;&nbsp;<label><input type=\"checkbox\" name=\"opensearchbox\" id=\"opensearchbox\" onclick=\"setModel(this,'opensearchbox');\"" + (eBase.parseBool( ModelInfo["opensearchbox"]) ? " checked" : "") + " />默认打开搜索框</label>");
            eBase.Write("&nbsp;&nbsp;<label><input type=\"checkbox\" name=\"globalSearch\" id=\"globalSearch\" onclick=\"setModel(this,'globalSearch');\"" + (eBase.parseBool( ModelInfo["globalSearch"]) ? " checked" : "") + " />全局显示</label>");
            eBase.Write("&nbsp;&nbsp;<label><input type=\"checkbox\" name=\"searchResetBtn\" id=\"searchResetBtn\" onclick=\"setModel(this,'searchResetBtn');\"" + (eBase.parseBool(ModelInfo["searchResetBtn"]) ? " checked" : "") + " />重置按钮</label>");
            eBase.Writeln("&nbsp;&nbsp;<label><input type=\"checkbox\" name=\"searchSaveBtn\" id=\"searchSaveBtn\" onclick=\"setModel(this,'searchSaveBtn');\"" + (eBase.parseBool(ModelInfo["searchSaveBtn"]) ? " checked" : "") + " />保存按钮</label>");
            Response.Write("</div> ");
            /*
            eList datalist = new eList("a_eke_sysModelConditions");
            datalist.Where.Add("delTag=0 and ModelID='" + modelid + "'");
            datalist.OrderBy.Add("show desc,px,addTime");
            //eBase.WriteDebug(datalist.SQL);
            Rep.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
            datalist.Bind(Rep);
            */
            list();

            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Rep.RenderControl(htw);
            Rep.Visible = false;//不输出，要在获取后设，不然取不到内容。
            Response.Write(sw.ToString());
            sw.Close();
            Response.End();
        }
        private void list()
        {
            DataTable tb = getItems(modelid);
            appendItems(tb, modelid);
            tb = tb.Select("", "show desc,PX,addTime").toDataTable();
            Rep.ItemDataBound += new RepeaterItemEventHandler(Rep_ItemDataBound);
            Rep.DataSource = tb;
            Rep.DataBind();
        }
        private DataTable getItems(string modelid)
        {
            return eBase.DataBase.getDataTable("select b.mc as ModelName,a.* from a_eke_sysModelConditions a inner join a_eke_sysModels b on a.modelid=b.modelid where a.ModelID='" + modelid + "' and a.delTag=0");
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

        public string getDisplay(string ModelConditionID)
        {
            return "";
            string count = DataBase.getValue("select count(*) from a_eke_sysModelConditionItems where ModelConditionID='" + ModelConditionID + "' and deltag=0");
            if (count == "0")
            {
                return "";
            }
            else
            {
                return "display:none;";
            }            
        }
        protected void Rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            string sql = "";
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                StringBuilder sb = new StringBuilder();
                Control ctrl = e.Item.Controls[0];
                Literal lit = (Literal)ctrl.FindControl("LitColnums");
                if (lit != null)
                {
                    DataTable dt = allColumns;
                    StringBuilder _sb = new StringBuilder();
                    foreach (DataRow _dr in dt.Rows)
                    {
                        _sb.Append("<option modelid=\"" + _dr["ModelID"].ToString() + "\" value=\"" + _dr["COLUMN_NAME"].ToString() + "\"" + (_dr["ModelID"].ToString().ToLower() == DataBinder.Eval(e.Item.DataItem, "ModelID").ToString().ToLower() && _dr["COLUMN_NAME"].ToString().ToLower() == DataBinder.Eval(e.Item.DataItem, "Code").ToString().ToLower() ? " selected=\"true\"" : "") + ">" + _dr["DESCRIPTION"].ToString() + "(" + _dr["COLUMN_NAME"].ToString() + ") - " + _dr["ModelName"].ToString() + "</option>");
                    }
                    lit.Text = _sb.ToString();
                }
                lit = (Literal)ctrl.FindControl("LitObjects");
                #region 数据绑定
                if (lit != null)
                {
                    eDataBase db;
                    string dsid = DataBinder.Eval(e.Item.DataItem, "DataSourceID").ToString();

                    if (dsid.Length == 0)
                    {
                        db = DataBase;
                    }
                    else if (dsid == "maindb")
                    {
                        db = eConfig.DefaultDataBase;
                    }
                    else
                    {
                        db = new eDataBase(new Guid(dsid));
                    }
                    DataTable AllTables = db.getSchemaTableViews();

                    for (int i = 0; i < AllTables.Rows.Count; i++)
                    {
                        //sb.Append("<option value=\"" + AllTables.Rows[i]["value"].ToString() + "\"" + (DataBinder.Eval(e.Item.DataItem, "BindObject").ToString() == AllTables.Rows[i]["value"].ToString() ? " selected=\"true\"" : "") + " title=\"" + AllTables.Rows[i]["text"].ToString() + "\">" + AllTables.Rows[i]["text"].ToString() + "</option>\r\n");
                        sb.Append("<li title=\"" + AllTables.Rows[i]["text"].ToString() + "\" onclick=\"set_filterText(this);call_filterEvent(this);\" py=\"" + ePinyin.getFirstLetter(AllTables.Rows[i]["text"].ToString()).ToLower() + "\" value=\"" + AllTables.Rows[i]["value"].ToString() + "\">" + AllTables.Rows[i]["text"].ToString() + "</li>");
                    }
                    lit.Text = sb.ToString();
                    if (DataBinder.Eval(e.Item.DataItem, "BindObject").ToString().Length > 0)
                    {
                        lit = (Literal)ctrl.FindControl("LitValue");
                        if (lit != null)
                        {
                            //绑定表-列
                            DataTable cols = db.getSchemaColumns(DataBinder.Eval(e.Item.DataItem, "BindObject").ToString());

                           // eBase.PrintDataTable(AllTables);
                            //sql = "select b.name from sysobjects a inner join  syscolumns b on a.id=b.id where a.name='" + DataBinder.Eval(e.Item.DataItem, "BindObject").ToString() + "' order by b.colid";//b.colid";
                            //lit.Text = ExDataBase.getOptions(sql, "name", "name", DataBinder.Eval(e.Item.DataItem, "BindValue").ToString());
                            /*
                            lit.Text = cols.toOptions("COLUMN_NAME", "COLUMN_NAME", DataBinder.Eval(e.Item.DataItem, "BindValue").ToString());
                            lit = (Literal)ctrl.FindControl("LitText");
                            if (lit != null)
                            {
                                //lit.Text = ExDataBase.getOptions(sql, "name", "name", DataBinder.Eval(e.Item.DataItem, "BindText").ToString());
                                lit.Text = cols.toOptions("COLUMN_NAME", "COLUMN_NAME", DataBinder.Eval(e.Item.DataItem, "BindText").ToString());
                            }

                            lit = (Literal)ctrl.FindControl("LitCode");
                            if (lit != null)
                            {
                                lit.Text = cols.toOptions("COLUMN_NAME", "COLUMN_NAME", DataBinder.Eval(e.Item.DataItem, "BindCode").ToString());
                            }
                            */

                            sb = new StringBuilder();
                            sb.Append("<li title=\"无\" onclick=\"set_filterText(this);call_filterEvent(this);\" value=\"NULL\">无</li>");
                            foreach (DataRow dr in cols.Rows)
                            {
                                sb.Append("<li title=\"" + dr["COLUMN_NAME"].ToString() + "\" onclick=\"set_filterText(this);call_filterEvent(this);\" py=\"" + ePinyin.getFirstLetter(dr["COLUMN_NAME"].ToString()).ToLower() + "\" value=\"" + dr["COLUMN_NAME"].ToString() + "\">" + dr["COLUMN_NAME"].ToString() + "</li>");
                            }
                            lit.Text = sb.ToString();
                            lit = (Literal)ctrl.FindControl("LitText");
                            if (lit != null) lit.Text = sb.ToString();
                            lit = (Literal)ctrl.FindControl("LitCode");
                            if (lit != null) lit.Text = sb.ToString();


                            lit = (Literal)ctrl.FindControl("LitBindForeignKey");
                            if (lit != null)
                            {
                                string bindfk = DataBinder.Eval(e.Item.DataItem, "BindForeignKey").ToString();
                                if (bindfk.Length == 0) bindfk = "ParentID";
                                /*
                                sql = "select b.name from sysobjects a inner join  syscolumns b on a.id=b.id ";
                                sql += " inner join systypes c on b.xtype=c.xusertype ";
                                sql +=" where a.name='" + DataBinder.Eval(e.Item.DataItem, "BindObject").ToString() + "'";
                                sql += " and b.name not in ('addtime','adduser','edittime','edituser','deltime','deluser','deltag','" + DataBinder.Eval(e.Item.DataItem, "BindValue").ToString() + "')";
                                sql += " and (charindex('int',c.name)>0 or charindex('varchar',c.name)>0 or charindex('uniqueidentifier',c.name)>0) ";
                                sql += " order by b.colid";
                                */
                                //lit.Text = ExDataBase.getOptions(sql, "name", "name", bindfk);

                                //lit.Text += cols.Select("", "DATA_TYPE in ('int','uniqueidentifier','char','nchar','varchar','nvarchar') and COLUMN_NAME not in ('addtime','adduser','edittime','edituser','deltime','deluser','deltag','" + DataBinder.Eval(e.Item.DataItem, "BindValue").ToString() + "')", "ORDINAL_POSITION").toOptions("COLUMN_NAME", "COLUMN_NAME", bindfk);
                                cols = cols.Select("", "DATA_TYPE in ('int','uniqueidentifier','char','nchar','varchar','nvarchar') and COLUMN_NAME not in ('addtime','adduser','edittime','edituser','deltime','deluser','deltag','" + DataBinder.Eval(e.Item.DataItem, "BindValue").ToString() + "')", "ORDINAL_POSITION");
                                sb = new StringBuilder();
                                sb.Append("<li title=\"无\" onclick=\"set_filterText(this);call_filterEvent(this);\" value=\"NULL\">无</li>");
                                foreach (DataRow dr in cols.Rows)
                                {
                                    sb.Append("<li title=\"" + dr["COLUMN_NAME"].ToString() + "\" onclick=\"set_filterText(this);call_filterEvent(this);\" py=\"" + ePinyin.getFirstLetter(dr["COLUMN_NAME"].ToString()).ToLower() + "\" value=\"" + dr["COLUMN_NAME"].ToString() + "\">" + dr["COLUMN_NAME"].ToString() + "</li>");
                                }
                                lit.Text = sb.ToString();
                            }

                        }
                    }
                }
                #endregion                
                #region 联动加载
                lit = (Literal)ctrl.FindControl("Litcolumns");
                if (lit != null)
                {

                    sql = "SELECT ModelConditionID,MC FROM a_eke_sysModelConditions ";
                    sql += " where ModelID='" + DataBinder.Eval(e.Item.DataItem, "ModelID").ToString() + "' and BindAuto=0 and len(BindObject)>0 ";
                    sql += " and ModelConditionID<>'" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "' ";
                    sql += " and ControlType in ('select','radio','checkbox','autoselect','filterselect') ";
                    //lit.Text = ModelItems.Select("", "len(Convert(BindObject, 'System.String'))>0 and Convert(ModelConditionID, 'System.String')<>'" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "' and ControlType in ('select','radio','checkbox')", "").toOptions("ModelConditionID", "MC", DataBinder.Eval(e.Item.DataItem, "FillItem").ToString());
                    //lit.Text = eBase.DataBase.getOptions(sql, "mc", "ModelConditionID", DataBinder.Eval(e.Item.DataItem, "FillItem").ToString());

                    //DataRow[] rows=
                    //eBase.Writeln(DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString());

                    DataTable tb = ModelConditions.Select("", "len(Convert(BindObject, 'System.String'))>0 and BindAuto=0 and Convert(ModelConditionID, 'System.String')<>'" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "' and ControlType in ('select','radio','checkbox','autoselect','filterselect')", "");
                    
                    sb = new StringBuilder();
                    string curitemid = DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString();
                    foreach (DataRow dr in tb.Rows)
                    {
                        DataRow[] myrows = ModelConditions.Select("ModelConditionID='" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "' and FillItem like '%" + dr["ModelConditionID"].ToString() + "%'");
                        DataRow[] allrows = ModelConditions.Select("FillItem like '%" + dr["ModelConditionID"].ToString() + "%'");
                        if (myrows.Length > 0 || allrows.Length == 0)
                        {
                            sb.Append("<label><input reload=\"true\" type=\"checkbox\" name=\"fillitem_" + curitemid.Replace("-", "") + "\"");
                            sb.Append(" onclick=\"setModelCondition_FillItem(this,'fillitem_" + curitemid.Replace("-", "") + "','" + curitemid + "','fillitem');\" value=\"" + dr["ModelConditionID"].ToString() + "\"");
                            if (DataBinder.Eval(e.Item.DataItem, "FillItem").ToString().IndexOf(dr["ModelConditionID"].ToString()) > -1) sb.Append(" checked");
                            sb.Append(" />");
                            sb.Append(dr["MC"].ToString() + "</label>");
                        }
                    }
                    lit.Text = sb.ToString().Length > 10 ? sb.ToString() : "无";
                }
                #endregion
                #region 选项
                lit = (Literal)ctrl.FindControl("LitOptions");
                if (lit != null)
                {
                    //HtmlControl hc = (HtmlControl)ctrl.FindControl("spanbind");
                    HtmlGenericControl hc = (HtmlGenericControl)ctrl.FindControl("spanbind");
                    if (hc != null)
                    {
                        //hc.Attributes.Add("style","border:1px solid #ff0000");
                        //Response.Write("has<br>");
                    }
                    sb = new StringBuilder();
                    sql = "select ModelConditionItemID,mc,conditionvalue,px from a_eke_sysModelConditionItems where ModelConditionID='" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "' and delTag=0 order by px,addTime ";
                    DataTable tb = eBase.DataBase.getDataTable(sql);
                    // sb.Append("<a href=\"?act=addconditem&modelid=" + modelid + "&cid=" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "\">添加选项</a><br>");


                    sb.Append("<table id=\"eDataTableOpt\" class=\"eDataTable eDataTableOpt\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" widt5h=\"100%\">");
                    sb.Append("<thead>");
                    sb.Append("<tr>");
                    sb.Append("<td height=\"25\" width=\"30\" bgc3olor=\"#ffffff\" align=\"center\"><a title=\"添加选项\" href=\"javascript:;\" onclick=\"addModelConditionItem(this,'" + DataBinder.Eval(e.Item.DataItem, "ModelConditionID").ToString() + "');\"><img width=\"16\" height=\"16\" src=\"images/add.png\" border=\"0\"></a></td>");
                    sb.Append("<td width=\"110\">&nbsp;选项名称</td>");
                    sb.Append("<td width=\"200\">&nbsp;条件</td>");
                    sb.Append("<td width=\"60\">&nbsp;顺序</td>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    sb.Append("<tbody eSize=\"false\" eMove=\"true\">");
                    if (tb.Rows.Count > 0)
                    {
                        if (hc != null)
                        {
                            hc.Attributes.Add("style", "disp3lay:none;");
                        }
                        for (int i = 0; i < tb.Rows.Count; i++)
                        {
                            sb.Append("<tr" + ((i + 1) % 2 == 0 ? " class=\"alternating\" eclass=\"alternating\"" : " eclass=\"\"") + " onmouseover=\"this.className='cur';\" onmouseout=\"this.className=this.getAttribute('eclass');\" erowid=\"" + tb.Rows[i]["ModelConditionItemID"].ToString() + "\">");
                            sb.Append("<td height=\"32\" align=\"center\"><a title=\"删除选项\" href=\"javascript:;\" onclick=\"delModelConditionItem(this,'" + tb.Rows[i]["ModelConditionItemID"].ToString() + "');\"><img width=\"16\" height=\"16\" src=\"images/del.png\" border=\"0\"></a></td>");
                            sb.Append("<td><input type=\"text\" value=\"" + tb.Rows[i]["mc"].ToString() + "\" oldvalue=\"" + tb.Rows[i]["mc"].ToString() + "\" class=\"edit\"  onBlur=\"setModelConditionItem(this,'" + tb.Rows[i]["ModelConditionItemID"].ToString() + "','mc');\" /></td>");
                            sb.Append("<td><input style=\"width:180px;\" type=\"text\" value=\"" + tb.Rows[i]["conditionvalue"].ToString() + "\" oldvalue=\"" + tb.Rows[i]["conditionvalue"].ToString() + "\" class=\"edit\" ondblclick=\"dblClick(this,'" + tb.Rows[i]["mc"].ToString() + "-条件');\" onBlur=\"setModelConditionItem(this,'" + tb.Rows[i]["ModelConditionItemID"].ToString() + "','conditionvalue');\" /></td>");
                            //sb.Append("<td><input reload=\"true\" style=\"width:60px;\" type=\"text\" value=\"" + (tb.Rows[i]["px"].ToString() == "999999" || tb.Rows[i]["px"].ToString() == "0" ? "" : tb.Rows[i]["px"].ToString()) + "\" oldvalue=\"" + (tb.Rows[i]["px"].ToString() == "999999" || tb.Rows[i]["px"].ToString() == "0" ? "" : tb.Rows[i]["px"].ToString()) + "\" class=\"edit\"  onBlur=\"setModelConditionItem(this,'" + tb.Rows[i]["ModelConditionItemID"].ToString() + "','px');\" /></td>");
                            sb.Append("<td>" + (i + 1).ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                    }
                    sb.Append("</tbody>");
                    sb.Append("</table>");

                    lit.Text = sb.ToString();
                }
                #endregion
            }
        }
    }
}