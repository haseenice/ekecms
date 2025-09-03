using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using EKETEAM.UserControl;

namespace eFrameWork.Manage
{
    public partial class ModelItems_Columns : System.Web.UI.Page
    {
        public string act = eParameters.QueryString("act");
        #region 属性
        private string sql = "";
        private string modelid = eParameters.QueryString("modelid");
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
                    DataTable dt = DataBase.getDataTable("select * from a_eke_sysModels where ModelID='" + ModelID + "'");
                    if (dt.Rows.Count > 0) _modelinfo = dt.Rows[0];
                }
                return _modelinfo;
            }
        }
        /// <summary>
        /// 主库
        /// </summary>
        private eDataBase _database;
        private eDataBase DataBase
        {
            get
            {
                if (_database == null)
                {
                    _database = eConfig.DefaultDataBase;
                }
                return _database;
            }
        }
        /// <summary>
        /// 主库或扩展库
        /// </summary>
        private eDataBase _exdatabase;
        private eDataBase ExDataBase
        {
            get
            {
                if (_exdatabase == null)
                {
                    if (ModelInfo["DataSourceID"].ToString().Length > 0)
                    {
                        _exdatabase = new eDataBase(ModelInfo);
                    }
                    else
                    {
                        return DataBase;
                    }
                }
                return _exdatabase;
            }
        }

        private string _tablename = "";
        public string TableName
        {
            get
            {
                if (_tablename.Length == 0)
                {
                    _tablename = eBase.DataBase.getValue("select code from a_eke_sysModels where ModelID='" + ModelID + "'");
                }
                return _tablename;
            }
        }


        private DataTable _columns;//所有列
        public DataTable Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = ExDataBase.getSchemaColumns(TableName);
                   // eBase.PrintDataTable(_columns);

                }
                return _columns;
            }
        }
        #endregion
        private DataTable _alltables;//所有表
        public DataTable AllTables
        {
            get
            {
                if (_alltables == null)
                {
                    string sql = "SELECT id,name FROM sysobjects where (xtype='U' or xtype='V') "; //name!='dtproperties' and 
                    sql += " and (charindex('a_eke_sys',lower(name))=0 or lower(name)='a_eke_sysusers' or lower(name)='a_eke_sysroles' or lower(name)='a_eke_sysmodels')";
                    sql += " and (name not in (" + eBase.getSystemTables() + ") or  lower(name)='a_eke_sysmodels' or lower(name)='a_eke_sysroles')";

                    //sql += " and (name not in (" + eBase.getSystemTables() + ") or  lower(name)='a_eke_sysmodels' or lower(name)='a_eke_sysusers')";
                    sql += " order by name";//crdate";
                    // _alltables = DataBase.getDataTable(sql);
                    _alltables = DataBase.getSchemaTableViews();
                    //eBase.PrintDataTable(_alltables);
                    //eBase.End();

                }
                return _alltables;
            }
        }

        private DataTable _items;//所有列
        public DataTable Items
        {
            get
            {
                if (_items == null)
                {
                    _items = DataBase.getDataTable("select * from a_eke_sysModelItems where ModelID='" + ModelID + "' and deltag=0");
                }
                return _items;
            }
        }
       
        private eUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser("Manage");
            user.Check();
           // string pk = eBase.DataBase.getPrimaryKey("mesData_OEEDaily");
            //eBase.Writeln(pk);

            Response.Write("<a href=\"http://help.eketeam.com/1/104.html\" style=\"float:right;\" target=\"_blank\" title=\"eFrameWork开发框架\"><img src=\"images/help.gif\"></a>");
            if (eConfig.showHelp())
            {
                Response.Write("<div class=\"tips\">");
                Response.Write("<b>数据结构</b><br>项目开发期间管理数据库物理结构。<br>");
                Response.Write("<b>注</b>：项目正式运行后请谨慎使用。<br>");
                Response.Write("</div> ");
            }
           // eBase.Writeln(ModelInfo["Type"].ToString());
           // eBase.End();
            if (ModelInfo["Type"].ToString() != "10") List();
            if (ModelInfo["Code"].ToString().Length > 0 && ModelInfo["Type"].ToString() != "10")
            {
                Response.Write("<div style=\"margin:6px 0px 8px 0px;\">");


                Response.Write("<label><input type=\"checkbox\" id=\"editdatatable\" onclick=\"editdatatable(this);\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? " checked" : "") + " />我了解风险,启用编辑。</label><br>");
                Response.Write("&nbsp;表名：");
                if (ExDataBase.Structure && !ModelInfo["Code"].ToString().ToLower().StartsWith("a_eke_sys"))
                {
                    Response.Write("<input" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " id=\"tablename\" type=\"text\" value=\"" + ModelInfo["Code"].ToString() + "\" oldvalue=\"" + ModelInfo["Code"].ToString() + "\"  class=\"edit\" style=\"width:180px;\" onBlur=\"setModel(this,'code');\" />");
                }
                else
                {
                    Response.Write(ModelInfo["Code"].ToString());
                }
                if (eConfig.showHelp()) Response.Write(" <img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(56);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;margin-bottom:4px;\">");
               
                string pk = ExDataBase.getPrimaryKey(TableName);
                //eBase.WriteDebug(ExDataBase.ConnectionString + "::" + pk);                


                //eBase.Writeln(cols.ContainsKey(deltag).ToString());
                if (!ModelInfo["Code"].ToString().ToLower().StartsWith("a_eke_sys") && (pk.Length == 0 || 1>0))
                {
                    Response.Write("&nbsp;主键：<input id=\"inputpk\" type=\"text\" value=\"" + ModelInfo["PrimaryKey"].ToString() + "\" oldvalue=\"" + ModelInfo["PrimaryKey"].ToString() + "\"  class=\"edit\" style=\"width:120px;\" onBlur=\"setModel(this,'primarykey');\" />");
                }

                if (ModelInfo["subModel"].ToString().ToLower().Replace("true","1") == "1" || 1>0)
                {
                    Response.Write("&nbsp;上级外键：<input id=\"inputpk\" type=\"text\" value=\"" + ModelInfo["ParentForeignkey"].ToString() + "\" oldvalue=\"" + ModelInfo["ParentForeignkey"].ToString() + "\"  class=\"edit\" style=\"width:120px;\" onBlur=\"setModel(this,'ParentForeignkey');\" />");
                }

                //Dictionary<string, string> cols = ExDataBase.getColumnCollection(TableName);
                ConcurrentDictionary<string, string> cols = ExDataBase.getColumnCollection(TableName);//Dictionary to ConcurrentDictionary
                string deltag = eConfig.deleteTag();
                if (deltag.Length == 0) deltag = "delTag";
                if (!cols.ContainsKey(deltag))
                {
                    Response.Write("&nbsp;删除标签：<input id=\"inputpk\" type=\"text\" value=\"" + ModelInfo["deleteTag"].ToString() + "\" oldvalue=\"" + ModelInfo["deleteTag"].ToString() + "\"  class=\"edit\" style=\"width:120px;\" onBlur=\"setModel(this,'deleteTag');\" />");
                }
                Response.Write("</div> ");
            }
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            LitBody.RenderControl(htw);
            LitBody.Visible = false;//不输出，要在获取后设，不然取不到内容。
            Response.Write(sw.ToString());
            sw.Close();
            Response.End();

        }
        private void List()
        {
            //eBase.PrintDataTable(Columns);
           
            bool submodel = eBase.parseBool(ModelInfo["submodel"]);
            string modeltype = ModelInfo["type"].ToString();
            //string zj1 = ExDataBase.getPrimaryKey(TableName);
            // eBase.Writeln(zj + "BB");
            //eBase.Writeln("1234:" );

            StringBuilder sb = new StringBuilder();
            string ct = "";
            if (Columns.Rows.Count > 0)
            {
                ct = "0";// eBase.DataBase.getValue("select count(*) from a_eke_sysModelItems where Custom=0 and ModelID='" + ModelID + "'");
                sb.Append("<table id=\"eDataTable_Columns\" class=\"eDataTable\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" wi3dth=\"99%\" style=\"margin-top:0px;\">\r\n");
                
                sb.Append("<colgroup>\r\n");
                sb.Append("<col width=\"50\" />\r\n");  //添加删除
                sb.Append("<col width=\"75\" />\r\n");  //选择
                sb.Append("<col width=\"170\" />\r\n"); //编码
                sb.Append("<col width=\"170\" />\r\n"); //说明
                sb.Append("<col width=\"160\" />\r\n"); //数据类型
                sb.Append("<col width=\"100\" />\r\n");  //长度
                sb.Append("<col width=\"100\" />\r\n");  //小数位
                sb.Append("<col width=\"170\" />\r\n"); //默认值
                sb.Append("<col width=\"75\" />\r\n");  //主键


                //if (submodel && modeltype != "3") sb.Append("<col width=\"75\" />\r\n");  //外键
                //if (modeltype != "3") 
                if (submodel || 1>0) sb.Append("<col width=\"75\" />\r\n");  //上级外键
                //if (modeltype != "3") 
                sb.Append("<col width=\"75\" />\r\n");  //下级外键
                sb.Append("<col width=\"75\" />\r\n"); //顺序

                sb.Append("</colgroup>\r\n");
                sb.Append("<thead>\r\n");
                sb.Append("<tr>\r\n");
                sb.Append("<td align=\"center\">");
                if (ExDataBase.Structure)
                {
                    sb.Append("<a title=\"添加列\" href=\"javascript:;\" style=\"margin:0px;" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : "display:none;") + "\" onclick=\"addColumnN();\"><img width=\"16\" height=\"16\" src=\"images/add.png\" border=\"0\"></a>");
                }
                else
                {
                    sb.Append("&nbsp;");
                }
                sb.Append("</td>\r\n");                
                sb.Append("<td align=\"center\"><input type=\"checkbox\" onclick=\"selColumnAll(this);\" name=\"selall\" id=\"selall\" style=\"margin-right:8px;\" value=\"0\"" + (ct == "0" ? "" : " checked") + " />");
                if (eConfig.showHelp()) sb.Append("<img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(57);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;\">");
                sb.Append("</td>\r\n");
                sb.Append("<td>编码");
                if (eConfig.showHelp()) sb.Append(" <img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(58);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;margin-bottom:4px;\">");
                sb.Append("</td>\r\n");
                sb.Append("<td>说明");
                if (eConfig.showHelp()) sb.Append(" <img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(59);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;margin-bottom:4px;\">");
                sb.Append("</td>\r\n");
                sb.Append("<td>数据类型</td>\r\n");
                sb.Append("<td>长度</td>\r\n");
                sb.Append("<td>小数位</td>\r\n");
                sb.Append("<td>默认值");
                if (eConfig.showHelp()) sb.Append(" <img src=\"images/help.gif\" align=\"absmiddle\" border=\"0\" onclick=\"showHelp(60);\" title=\"查看帮助\" alt=\"查看帮助\" style=\"cursor:pointer;margin-bottom:4px;\">");
                sb.Append("</td>\r\n");
                sb.Append("<td>主键</td>\r\n");
                //if (submodel && modeltype!="3") sb.Append("<td width=\"60\">外键</td>\r\n");
                if (submodel || 1 > 0) sb.Append("<td>向上外键</td>\r\n");
                sb.Append("<td>向下外键</td>\r\n");
                //sb.Append("<td  align=\"center\">顺序</td>\r\n");
                sb.Append("<td>顺序</td>\r\n");
                sb.Append("</tr>\r\n");
                sb.Append("</thead>\r\n");
                if (ExDataBase.Structure)
                {
                    sb.Append("<tbody eSize=\"false\" eMove=\"true\">\r\n");
                }
                else
                {
                    sb.Append("<tbody eSize=\"false\" eMove=\"false\">\r\n");
                }
              
                string zj = ExDataBase.getPrimaryKey(TableName);
               // eBase.Writeln(zj + "BB");

                string syscolumns = eConfig.getAllSysColumns() + zj.ToLower() + ",";

                string foreignkey = ModelInfo["foreignkey"].ToString();
                if (foreignkey.Length == 0 && ModelInfo["ParentID"].ToString().Length > 0)
                {
                    foreignkey = DataBase.getValue("SELECT Code FROM a_eke_sysModelItems where ModelID='" + ModelInfo["ParentID"].ToString() + "' and primaryKey='1'");
                   // eBase.Writeln(foreignkey);
                }
                string subforeignkey = ModelInfo["subforeignkey"].ToString();
                string mappingforeignkey = ModelInfo["mappingforeignkey"].ToString();
                string codes = "";
                //eBase.Print(Columns);
                #region 数据库列
                for (int i = 0; i < Columns.Rows.Count; i++)
                {
                    if (i > 0) codes += ",";
                    codes += Columns.Rows[i]["COLUMN_NAME"].ToString();
                    sb.Append("<tr" + ((i + 1) % 2 == 0 ? " class=\"alternating\" eclass=\"alternating\"" : " eclass=\"\"") + ">\r\n");
                    sb.Append("<td height=\"32\" align=\"center\">");
                    if (syscolumns.IndexOf("," + Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() + ",") > -1)
                    {
                        sb.Append("&nbsp;");
                    }
                    else
                    {
                        //sb.Append("<a title=\"删除列\" style=\"margin:0px;" + ("sys" == "True" ? "display:none;" : "") + "\" href=\"javascript:;\" onclick=\"delColumn(this,'" + Columns.Rows[i]["code"].ToString() + "');\"><img width=\"16\" height=\"16\" src=\"images/del.png\" border=\"0\"></a>");
                        if (ExDataBase.Structure)
                        {
                            sb.Append("<a title=\"删除列\" style=\"margin:0px;" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : "display:none;") + "\" href=\"javascript:;\" onclick=\"delColumn(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "');\"><img width=\"16\" height=\"16\" src=\"images/del.png\" border=\"0\"></a>");
                        }
                        else
                        {
                            sb.Append("&nbsp;"); 
                        }
                    }
                    sb.Append("</td>\r\n");
                    sb.Append("<td align=\"center\">\r\n");
                    //ct = DataBase.getValue("select count(*) from a_eke_sysModelItems where ModelID='" + ModelID + "' and UPPER(Code)='" + Columns.Rows[i]["COLUMN_NAME"].ToString().ToUpper() + "' and delTag=0");


                    ct = Items.Select("Code='" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "'").Length.ToString();

                    sb.Append("<input type=\"checkbox\" onclick=\"selColumn(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "');\" name=\"selItem\" value=\"" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "\"" + (ct == "0" ? "" : " checked") + " />\r\n");
                    sb.Append("</td>\r\n");

                    #region 编码
                    sb.Append("<td>");
                    if (ExDataBase.Structure)
                    {
                        sb.Append("<input style=\"width:150px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " reload=\"true\" type=\"text\" oldvalue=\"" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "\" value=\"" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "\" class=\"edit\" onBlur=\"ReNameColumn(this);\" /></td>\r\n");
                    }
                    else
                    {
                        sb.Append(Columns.Rows[i]["COLUMN_NAME"].ToString()); 
                    }
                    sb.Append("</td>\r\n");
                    #endregion

                    #region 描述
                    sb.Append("<td>");
                    if (ExDataBase.Structure)
                    {
                        sb.Append("<input style=\"width:150px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " type=\"text\" oldvalue=\"" + Columns.Rows[i]["DESCRIPTION"].ToString() + "\" value=\"" + Columns.Rows[i]["DESCRIPTION"].ToString() + "\"  class=\"edit\" onBlur=\"ColumnName(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\" /></td>\r\n");
                    }
                    else
                    {
                        sb.Append(Columns.Rows[i]["DESCRIPTION"].ToString());
                    }
                    sb.Append("</td>\r\n");
                    #endregion

                    #region 数据类型
                    string colname = Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower();
                    string coltype = Columns.Rows[i]["DATA_TYPE"].ToString().ToLower();
                    int colprecision = Columns.Rows[i]["NUMERIC_PRECISION"] == DBNull.Value ? 0 : Convert.ToInt32(Columns.Rows[i]["NUMERIC_PRECISION"]);
                    int colscale = Columns.Rows[i]["NUMERIC_SCALE"] == DBNull.Value ? 0 : Convert.ToInt32(Columns.Rows[i]["NUMERIC_SCALE"]);
                    sb.Append("<td>");
                    if (ExDataBase.Structure)
                    {
                        sb.Append("<select style=\"width:150px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " reload=\"true\" oldvalue=\"" + Columns.Rows[i]["DATA_TYPE"].ToString() + "\" onChange=\"ColumnType(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\">\r\n");
                        if (ExDataBase.DataBaseType == eDataBaseType.SQLServer) sb.Append("<option value=\"uniqueidentifier\"" + (coltype == "uniqueidentifier" ? " selected=\"true\"" : "") + ">GUID(uniqueidentifier)</option>\r\n");
                        if (zj.ToLower() != colname) sb.Append("<option value=\"nchar\"" + (coltype == "char" || coltype == "nchar" ? " selected=\"true\"" : "") + ">字符(nchar)</option>\r\n");
                        //if (zj.ToLower() != colname)
                        sb.Append("<option value=\"nvarchar\"" + (coltype.IndexOf("varchar") > -1 ? " selected=\"true\"" : "") + ">文本(nvarchar)</option>\r\n");
                        sb.Append("<option value=\"int\"" + (coltype == "int" || (coltype == "number" && colprecision == 0 && colscale == 0) ? " selected=\"true\"" : "") + ">整数(int)</option>\r\n");
                        //if (zj.ToLower() != colname) sb.Append("<option value=\"float\"" + (coltype == "float" ? " selected=\"true\"" : "") + ">小数(float)</option>\r\n");
                        if (zj.ToLower() != colname) sb.Append("<option value=\"decimal\"" + (coltype == "decimal" || coltype == "float" || colscale > 0 ? " selected=\"true\"" : "") + ">小数(decimal)</option>\r\n");
                        if (zj.ToLower() != colname) sb.Append("<option value=\"datetime\"" + (coltype.IndexOf("date") > -1 || coltype == "timestamp" ? " selected=\"true\"" : "") + ">时间(datetime)</option>\r\n");
                        if (zj.ToLower() != colname) sb.Append("<option value=\"bit\"" + (coltype == "bit" || (coltype == "number" && colprecision == 1) ? " selected=\"true\"" : "") + ">是/否(bit)</option>\r\n");
                        if (zj.ToLower() != colname) sb.Append("<option value=\"ntext\"" + (coltype.IndexOf("text") > -1 || coltype == "clob" ? " selected=\"true\"" : "") + ">备注(text)</option>\r\n");
                        sb.Append("<select>\r\n");
                    }
                    else
                    {
                        sb.Append(Columns.Rows[i]["DATA_TYPE"].ToString());
                    }
                    sb.Append("</td>\r\n");
                    #endregion

                    #region 长度
                    sb.Append("<td>");
                    if (ExDataBase.Structure)
                    {
                        if (coltype.IndexOf("char") > -1 || colscale > 0)
                        {
                            if (coltype == "decimal" || coltype == "number")
                            {
                                sb.Append("<input style=\"width:90px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " type=\"text\" oldvalue=\"" + Columns.Rows[i]["NUMERIC_PRECISION"].ToString() + "\" value=\"" + Columns.Rows[i]["NUMERIC_PRECISION"].ToString() + "\" class=\"edit\" onBlur=\"ColumnLength(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\" />");

                            }
                            else
                            {
                                sb.Append("<input style=\"width:90px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " type=\"text\" oldvalue=\"" + Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString() + "\" value=\"" + (coltype == "nvarchar" || coltype == "nchar" ? (Convert.ToInt32(Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]) / 1).ToString() : Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString()) + "\" class=\"edit\" onBlur=\"ColumnLength(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\" />");
                            }
                        }
                        else
                        {
                            sb.Append(coltype == "nvarchar" || coltype == "nchar" ? (Convert.ToInt32(Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"]) / 1).ToString() : Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                        }
                    }
                    else
                    {
                        sb.Append(Columns.Rows[i]["CHARACTER_MAXIMUM_LENGTH"].ToString());
                    }
                    sb.Append("</td>\r\n");
                    #endregion

                    #region 小数位
                    sb.Append("<td>");               
                    if ((coltype == "decimal" || coltype == "number") && colscale > 0)
                    {
                        if (ExDataBase.Structure)
                        {
                            sb.Append("<input style=\"width:90px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " type=\"text\" oldvalue=\"" + Columns.Rows[i]["NUMERIC_SCALE"].ToString() + "\" value=\"" + Columns.Rows[i]["NUMERIC_SCALE"].ToString() + "\" class=\"edit\" onBlur=\"ColumnScale(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\" />");
                        }
                        else
                        {
                            sb.Append(Columns.Rows[i]["NUMERIC_SCALE"].ToString());
                        }
                    }
                    else
                    {
                        sb.Append("&nbsp;");
                    }
                    sb.Append("</td>\r\n");
                    #endregion

                    #region 默认值
                    string _default = Columns.Rows[i]["COLUMN_DEFAULT"].ToString();
                    if (_default.Length > 0)
                    {
                        if (_default.IndexOf("((") > -1)
                        {
                            //_default = _default.Substring(2, _default.Length - 4);
                        }
                        else
                        {
                            //_default = _default.Substring(1, _default.Length - 2);
                        }
                    }
                    if (zj.ToLower() == Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() && Columns.Rows[i]["Data_Type"].ToString() != "uniqueidentifier")
                    {
                        sb.Append("<td>&nbsp;</td>\r\n");
                    }
                    else
                    {
                        if (ExDataBase.Structure)
                        {
                            sb.Append("<td><input style=\"width:150px;\"" + (Session["dbeditstate_" + modelid.Replace("-", "")] != null && Session["dbeditstate_" + modelid.Replace("-", "")].ToString() == "1" ? "" : " disabled=\"true\"") + " type=\"text\" oldvalue=\"" + _default + "\" value=\"" + _default + "\" class=\"edit\" onBlur=\"ColumnDefault(this,'" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "',this.value);\" /></td>\r\n");
                        }
                        else
                        {
                            sb.Append("<td>" + _default + "</td>");
                        }
                    }
                    #endregion

                    //主键
                    sb.Append("<td align=\"center\">" + (zj.ToLower() == Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() ? "是" : "&nbsp;") + "</td>\r\n");

                    #region 上级外键
                    //if (submodel && modeltype != "3")
                    //if (modeltype != "3")
                    //{
                    if (submodel || 1 > 0)
                    {
                        sb.Append("<td align=\"center\">");
                        if (
                            // syscolumns.IndexOf("," + Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() + ",") > -1 || 
                            ct == "0"
                            || coltype.IndexOf("bit") > -1
                            || coltype.IndexOf("text") > -1
                            || coltype.IndexOf("numeric") > -1
                            || coltype.IndexOf("date") > -1
                            )
                        {
                            sb.Append("&nbsp;");
                        }
                        else
                        {
                            sb.Append("<input type=\"radio\" onclick=\"setModel(this,'foreignkey');\" name=\"foreignkey\" value=\"" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "\"" + (foreignkey.ToLower() == Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() ? " checked" : "") + " />\r\n");
                        }
                        sb.Append("</td>\r\n");
                    }
                    //}
                    #endregion
                    #region 下级外键
                    //if (modeltype != "3")
                    //{
                        sb.Append("<td align=\"center\">");
                        if (
                            // syscolumns.IndexOf("," + Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() + ",") > -1 || 
                            //ct == "0"
                            0>1
                            || coltype.IndexOf("bit") > -1
                            || coltype.IndexOf("text") > -1
                            || coltype.IndexOf("numeric") > -1
                            || coltype.IndexOf("date") > -1
                            )
                        {
                            sb.Append("&nbsp;");// uniqueidentifier

                        }
                        else
                        {
                            sb.Append("<input type=\"radio\" onclick=\"setModel(this,'subforeignkey');\" name=\"subforeignkey\" value=\"" + Columns.Rows[i]["COLUMN_NAME"].ToString() + "\"" + (subforeignkey.ToLower() == Columns.Rows[i]["COLUMN_NAME"].ToString().ToLower() ? " checked" : "") + " />\r\n");
                        }
                        sb.Append("</td>\r\n");
                        //}                   
                    #endregion
                    sb.Append("<td align=\"center\" style=\"cursor:move;\" title=\"鼠标按下拖动改变列位置!\">" + (i + 1).ToString() + "</td>\r\n");
                    sb.Append("</tr>\r\n");
                }
                #endregion
                #region 删除的

                sql = "select * from a_eke_sysModelItems where ModelID='" + ModelID + "' and delTag=0 and Custom=0 and '" + codes + "' not like  '%,' + Code + ',%'";
                sql = "select * from a_eke_sysModelItems where ModelID='" + ModelID + "' and delTag=0 and Custom=0 and UPPER(Code) not in ('" + codes.ToUpper().Replace(",", "','") + "')";
                
                //eBase.Writeln(codes);
                //eBase.Writeln(sql);
                DataTable tb = DataBase.getDataTable(sql);
                //eBase.PrintDataTable(tb);
                foreach (DataRow dr in tb.Rows)
                {
                    sb.Append("<tr>\r\n");
                    sb.Append("<td height=\"32\" align=\"center\">&nbsp;</td>\r\n");

                    sb.Append("<td align=\"center\"><input type=\"checkbox\" onclick=\"selColumn(this,'" + dr["code"].ToString() + "');\" value=\"" + dr["code"].ToString() + "\" checked=\"true\" /></td>\r\n");
                    sb.Append("<td style=\"color:#ccc;\">" + dr["code"].ToString() + "</td>\r\n");
                    sb.Append("<td style=\"color:#ccc;\">" + dr["mc"].ToString() + "</td>\r\n");
                    sb.Append("<td style=\"color:#ccc;\">" + dr["type"].ToString() + "</td>\r\n");
                    sb.Append("<td>" + (dr["type"].ToString().ToLower() == "nvarchar" ? (Convert.ToInt32(dr["length"]) / 2).ToString() : dr["length"].ToString()) + "</td>\r\n");
                    sb.Append("<td>&nbsp;</td>\r\n"); //小数位
                    sb.Append("<td>&nbsp;</td>\r\n"); //默认值
                    sb.Append("<td>&nbsp;</td>\r\n");//主键
                    //if (submodel && modeltype != "3")
                    //if (modeltype != "3")
                    //{
                    if (submodel) sb.Append("<td>&nbsp;</td>\r\n"); //上级外键
                    sb.Append("<td>&nbsp;</td>\r\n"); //下级外键
                    //}
                    sb.Append("<td>&nbsp;</td>\r\n"); //顺序
                    sb.Append("</tr>\r\n");
                }
                #endregion
                sb.Append("</tbody>\r\n");
                sb.Append("</table>");

            }
            if (!Columns.Columns.Contains("COLUMN_NAME"))
            {
                eBase.Writeln("表不存在!");
                eBase.End();

            }
            DataTable dt;
            #region 常用字段

            if (ExDataBase.Structure)
            {
                dt = eBase.CommonColumns.Copy();
                for (int i = dt.Rows.Count - 1; i > -1; i--)
                {
                    DataRow[] rs = Columns.Select("COLUMN_NAME='" + dt.Rows[i]["COLUMN_NAME"].ToString() + "'");
                    if (rs.Length > 0) dt.Rows.Remove(dt.Rows[i]);
                    //eBase.PrintDataRow(dr);
                }
                //eBase.PrintDataTable(dt);
                sb.Append("常用字段：<br>");
                sb.Append("<table class=\"eDataTable\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" width=\"600\" style=\"margin-top:0px;\">\r\n");
                sb.Append("<colgroup>\r\n");
                sb.Append("<col width=\"40\" />\r\n");  //操作
                sb.Append("<col width=\"150\" />\r\n");  //编码
                sb.Append("<col width=\"150\" />\r\n");  //名称
                sb.Append("<col width=\"150\" />\r\n"); //外键
                sb.Append("</colgroup>\r\n");
                sb.Append("<thead>\r\n");
                sb.Append("<tr>\r\n");
                sb.Append("<td align=\"center\">&nbsp;</td>\r\n");
                sb.Append("<td>编码</td>\r\n");
                sb.Append("<td>名称</td>\r\n");
                sb.Append("<td>数据类型</td>\r\n");
                sb.Append("</tr>\r\n");
                sb.Append("<tbody>\r\n");
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>\r\n");
                    sb.Append("<td height=\"32\" align=\"center\"><input type=\"checkbox\" onclick=\"addColumn2(this,'" + dr["COLUMN_NAME"].ToString() + "');\" value=\"" + dr["COLUMN_NAME"].ToString() + "\"  /></td>\r\n");
                    sb.Append("<td>" + dr["COLUMN_NAME"].ToString() + "</td>\r\n");
                    sb.Append("<td>");
                    sb.Append(dr["DESCRIPTION"].ToString());
                    sb.Append("</td>\r\n");
                    sb.Append("<td>");
                    sb.Append(dr["DATA_TYPE"].ToString());
                    sb.Append("</td>\r\n");
                    sb.Append("</tr>\r\n");
                }
                sb.Append("</tbody>\r\n");
                sb.Append("</table>");
            }
            #endregion

            #region 自定义数据备份
            if (modeltype != "3" && ExDataBase.Structure)
            {
                sb.Append("自定义数据备份：<br>");
                sb.Append("<table class=\"eDataTable\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" width=\"600\" style=\"margin-top:0px;\">\r\n");
                sb.Append("<colgroup>\r\n");
                sb.Append("<col width=\"40\" />\r\n");  //操作
                sb.Append("<col width=\"260\" />\r\n");  //名称
                sb.Append("<col width=\"150\" />\r\n");  //表名
                sb.Append("<col width=\"150\" />\r\n"); //外键
                sb.Append("</colgroup>\r\n");
                sb.Append("<thead>\r\n");
                sb.Append("<tr>\r\n");
                sb.Append("<td align=\"center\"><a title=\"添加\" href=\"javascript:;\" style=\"margin:0px;\" onclick=\"addBakModel(this);\"><img width=\"16\" height=\"16\" src=\"images/add.png\" border=\"0\"></a></td>\r\n");
                sb.Append("<td>名称</td>\r\n");
                sb.Append("<td>表名</td>\r\n");
                sb.Append("<td>外键</td>\r\n");
                sb.Append("</tr>\r\n");
                sb.Append("<tbody>\r\n");
                dt = DataBase.getDataTable("select ModelID,MC,Code,Foreignkey from a_eke_sysModels where ParentID='" + ModelID + "' and Type=9 and deltag=0 order by addTime");
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append("<tr>\r\n");
                    sb.Append("<td height=\"32\" align=\"center\"><a title=\"删除\" style=\"margin:0px;\" href=\"javascript:;\" onclick=\"delBakModel(this,'" + dr["ModelID"].ToString() + "');\"><img width=\"16\" height=\"16\" src=\"images/del.png\" border=\"0\"></a></td>\r\n");
                    sb.Append("<td><input type=\"text\" value=\"" + dr["mc"].ToString() + "\" oldvalue=\"" + dr["mc"].ToString() + "\"  class=\"edit\" style=\"width:240px;\" onBlur=\"setBakModel(this,'" + dr["ModelID"].ToString() + "','mc');\" /></td>\r\n");
                    sb.Append("<td>");
                    sb.Append("<select reload=\"true\" onchange=\"setBakModel(this,'" + dr["ModelID"].ToString() + "','code');\" style=\"width:100px;\">\r\n");
                    sb.Append("<option value=\"NULL\">无</option>\r\n");
                    for (int i = 0; i < AllTables.Rows.Count; i++)
                    {
                        sb.Append("<option value=\"" + AllTables.Rows[i]["value"].ToString() + "\"" + (dr["code"].ToString().ToLower() == AllTables.Rows[i]["value"].ToString().ToLower() ? " selected=\"true\"" : "") + " title=\"" + AllTables.Rows[i]["text"].ToString() + "\">" + AllTables.Rows[i]["text"].ToString() + "</option>\r\n");
                    }
                    sb.Append("</select>\r\n");
                    sb.Append("</td>\r\n");
                    sb.Append("<td>");
                    sb.Append("<select reload=\"false\" onchange=\"setBakModel(this,'" + dr["ModelID"].ToString() + "','foreignkey');\" style=\"width:100px;\">\r\n");
                    sb.Append("<option value=\"NULL\">无</option>\r\n");
                    if (dr["code"].ToString().Length > 0)
                    {
                        DataTable cols = DataBase.getSchemaColumns(dr["code"].ToString(),false);
                        for (int i = 0; i < cols.Rows.Count; i++)
                        {
                            sb.Append("<option value=\"" + cols.Rows[i]["COLUMN_NAME"].ToString() + "\"" + (dr["foreignkey"].ToString().ToLower() == cols.Rows[i]["COLUMN_NAME"].ToString().ToLower() ? " selected=\"true\"" : "") + " title=\"" + cols.Rows[i]["COLUMN_NAME"].ToString() + "\">" + cols.Rows[i]["COLUMN_NAME"].ToString() + "</option>\r\n");
                        }
                    }
                    sb.Append("</select>\r\n");
                    sb.Append("</td>\r\n");
                    sb.Append("</tr>\r\n");
                }
                sb.Append("</tbody>\r\n");
                sb.Append("</table>");
            }
            #endregion
            LitBody.Text = sb.ToString();
        }
    }
}