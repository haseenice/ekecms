using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using EKETEAM.UserControl;
using LitJson;

namespace eFrameWork.Manage
{
    public partial class ModelItems : System.Web.UI.Page
    {
        public string act = eParameters.QueryString("act");
        private string value = eParameters.QueryString("value").Replace("'", "''");
        private string item = eParameters.QueryString("item");
        #region 属性
        private string sql = "";
        private string _modelid = eParameters.QueryString("modelid");
        public string ModelID
        {
            get
            {
                return _modelid;
            }
        }
        private string _parentid = "";
        public string ParentID
        {
            get
            {
                if (_parentid.Length == 0)
                {
                    string temp = ModelInfo["ParentID"].ToString();

                    if (temp.Length == 0 || temp == "0") //1级
                    {
                        _parentid = ModelID;
                    }
                    else
                    {
                        string type = temp.Length == 0 ? "" : DataBase.getValue("select Type FROM a_eke_sysModels where ModelID='" + temp + "'");
                        if (type == "2")
                        {
                            _parentid = ModelID;
                            return _parentid;
                        }
                        _parentid = temp;
                        while (temp != "0" && temp.Length > 0 && type!="2")
                        {
                            temp = DataBase.getValue("select ParentID FROM a_eke_sysModels where ModelID='" + temp + "'");
                            type = temp.Length == 0 ? "" : DataBase.getValue("select Type FROM a_eke_sysModels where ModelID='" + temp + "'");
                            if (temp != "0" && temp.Length > 0 && type != "2")
                            {
                                _parentid = temp;
                            }


                            if (type == "2")
                            {
                                //temp = "";
                            }
                            else
                            {
                               
                            }
                        }
                    }
                }
                return _parentid;
            }
        }
        private string _tablename = "";
        public string TableName
        {
            get
            {
                if (_tablename.Length == 0)
                {
                    _tablename = ModelInfo["code"].ToString();
                }
                return _tablename;
            }
        }
        public bool AutoModel = false;
        public bool SubModel = false;     
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
                    if (ModelInfo["DataSourceID"].ToString().Length > 4)
                    {
                        _exdatabase = new eDataBase(ModelInfo);
                    }
                    else
                    {
                        _exdatabase= DataBase;
                    }
                }
                return _exdatabase;
            }
        }


        private DataTable _columns;//所有列
        public DataTable Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = ExDataBase.getSchemaColumns(TableName,false);
                }
                return _columns;
            }
        }
        #endregion
        public string modelType = "0";
        public string linkArrys = "";
        public string titles = "";
        public string bodys = "";
        public bool ReadOnly
        {
            get
            {
                if (eConfig.getString("keepReadonly").ToLower() == "true") return true;
                if (Session["model_readonly"] == null) return true;
                return eBase.parseBool(Session["model_readonly"].ToString());
            }
        }
        private string getModelTree(string pid, int space=0)
        {
           // eBase.Writeln(pid);

            DataTable dt = DataBase.getDataTable("select * from a_eke_sysModels where ModelID='" + pid + "' order by addtime");
            //eBase.PrintDataTable(dt);
            if (dt.Rows.Count == 0) return "";
            StringBuilder sb = new StringBuilder();

            sb.Append("<div style=\"text-indent:" + space.ToString() + "px;line-height:22px;vertical-align:middle;\">");
            if (space > 0) sb.Append("<img src=\"images/left_ico.jpg\" width=\"11\" height=\"11\" align=\"absmiddle\">&nbsp;");
            sb.Append("<a target=\"" + (dt.Rows[0]["MappingModelID"].ToString().Length == 36 ? "_blank" : "_self") + "\" href=\"ModelItems.aspx?ModelID=" + (dt.Rows[0]["MappingModelID"].ToString().Length == 36 ? dt.Rows[0]["MappingModelID"].ToString() : pid) + "\" style=\"font-size:12px;" + (ModelID == pid ? "color:#ff0000;" : "color:#333333;") + "\">");
            //temp += DataBase.getValue("select mc from a_eke_sysModels where ModelID='" + pid + "'") + "</a>&nbsp;&nbsp;";
            sb.Append (dt.Rows[0]["mc"].ToString() + "</a>&nbsp;&nbsp;&nbsp;<font color=\"#666666\">");
            switch (dt.Rows[0]["Type"].ToString())
            {
                case "1"://模块
                    if (!eBase.parseBool( dt.Rows[0]["Auto"]))
                    {
                        sb.Append( "[自定义模块]");
                    }
                    else
                    {
                        if (eBase.parseBool( dt.Rows[0]["subModel"]))
                        {
                            sb.Append("[子模块 " + (eBase.parseBool( dt.Rows[0]["JoinMore"]) ? "1VN" : "1v1") + "]");
                        }
                        else
                        {
                            sb.Append("[主模块]");
                        }
                    }
                    break;
                case "2":
                   sb.Append( "[菜单]");
                    break;
                case "3":
                    sb.Append("[数据模块]");
                    break;
                case "6":
                    sb.Append("[互动模块]");
                    break;
                case "7": 
                    sb.Append("[只读列表]");
                    break;
                case "8":
                    sb.Append("[编辑步骤]");
                    break;
                case "12":
                    sb.Append("[映射模块]");
                    break;
            }
            sb.Append("</font>&nbsp;&nbsp;&nbsp;<a href=\"javascript:;\" onclick=\"Model_Copy('" + pid + "');\" _href=\"Models.aspx?act=copy&single=true&ID=" + pid + "\" _onclick=\"javascript:return confirm('确认要复制吗？');\">单个复制</a>&nbsp;&nbsp;&nbsp;");
            sb.Append("<a href=\"Models.aspx?act=edit&ID=" + pid + "\" target=\"_blank\">编辑</a>&nbsp;&nbsp;&nbsp;");


            if (dt.Rows[0]["Type"].ToString() == "1" && !eBase.parseBool(dt.Rows[0]["subModel"])) sb.Append("<a href=\"javascript:;\" onclick=\"createStep('" + pid + "','编辑2');\" _href=\"Models.aspx?act=edit&ID=" + pid + "\">添加编辑步骤</a>&nbsp;&nbsp;&nbsp;");

            sb.Append("<a href=\"Models.aspx?act=del&ID=" + pid + "\" style=\"" + (ModelID == pid ? "display:none;" : "") + "\" onclick=\"javascript:return confirm('确认要删除吗？');\">删除</a>");


            #region 运行测试
            if (dt.Rows[0]["ModelID"].ToString() == ParentID && (dt.Rows[0]["Type"].ToString() == "1" || dt.Rows[0]["Type"].ToString() == "4" || dt.Rows[0]["Type"].ToString() == "5") || dt.Rows[0]["Type"].ToString() == "10" || dt.Rows[0]["Type"].ToString() == "11")
            {
                sb.Append("&nbsp;&nbsp;");
                if (!eBase.parseBool( dt.Rows[0]["Auto"]))
                {
                    sb.Append("<a href=\"javascript:;\" onclick=\"layer.msg('暂未完成!');\" _href=\"RunCustom.aspx?ModelID=" + ParentID + "\" _target=\"_blank\"><img src=\"../images/sw_true.gif\">运行测试</a>");
                }
                else
                {
                    sb.Append("<a _href=\"javascript:;\" _onclick=\"layer.msg('暂未完成!');\" href=\"RunModel.aspx?ModelID=" + ParentID + "\" target=\"_blank\"><img src=\"../images/sw_true.gif\">运行测试</a>");
                }
            }
            #endregion
            if (dt.Rows[0]["Type"].ToString() == "12")//映射模块
            {
                sb.Append("&nbsp;&nbsp;映射外键：<input title=\"上级模块联合本模块的外键\" type=\"text\" value=\"" + dt.Rows[0]["MappingForeignkey"].ToString() + "\" oldvalue=\"" + dt.Rows[0]["MappingForeignkey"].ToString() + "\"  class=\"edit\" style=\"width:80px;\" onBlur=\"setModel(this,'MappingForeignkey','" + dt.Rows[0]["modelid"].ToString() + "');\" />");

                if (dt.Rows[0]["MappingForeignkey"].ToString().Length > 0)
                {
                    sb.Append("&nbsp;&nbsp;对应：&nbsp;<select onchange=\"setModel(this,'MappingType','" + dt.Rows[0]["modelid"].ToString() + "');\">");
                    sb.Append("<option value=\"0\"" + (dt.Rows[0]["MappingType"].ToString() == "0" ? "  selected=\"selected\"" : "") + ">主键</option>");
                    sb.Append("<option value=\"1\"" + (dt.Rows[0]["MappingType"].ToString() == "1" ? "  selected=\"selected\"" : "") + ">上级外键</option>");
                    sb.Append("</select>");

                    sql = "select frmid from a_eke_sysModelItems where ModelID='" + dt.Rows[0]["ParentID"].ToString() + "' and Code='" + dt.Rows[0]["MappingForeignkey"].ToString() + "'";
                    string frmid = DataBase.getValue(sql);// dt.Rows[0]["ParentID"].ToString() + "::" + dt.Rows[0]["MappingForeignkey"].ToString();
                    //eBase.Writeln(frmid);
                    sb.Append("&nbsp;&nbsp;<a class=\"copy\" title=\"复制查看连接\" style=\"margin-right:0px;vertical-align: middle;\" href=\"javascript:;\" data-clipboard-action=\"copy\" data-clipboard-text=\"<a class=&quot;viewmodel&quot; href=&quot;javascript:;&quot; onclick=&quot;viewMappingModel('查看标题','" + dt.Rows[0]["ModelID"].ToString() + "','" + frmid + "','" + (dt.Rows[0]["MappingType"].ToString() == "0" ? "id" : "pid") + "','90%','90%');&quot;>查看</a>\"></a>");
                    sb.Append("&nbsp;&nbsp;<a class=\"copy\" title=\"复制加载函数\" style=\"margin-right:0px;vertical-align: middle;\" href=\"javascript:;\" data-clipboard-action=\"copy\" data-clipboard-text=\"readMappingModel('" + dt.Rows[0]["ModelID"].ToString() + "','" + frmid + "','" + (dt.Rows[0]["MappingType"].ToString() == "0" ? "id" : "pid") + "');\"></a>");
                }
            }

            if (dt.Rows[0]["Type"].ToString() == "1" && !eBase.parseBool(dt.Rows[0]["JoinMore"]))
            {
                //sb.Append("&nbsp;&nbsp;<label><input type=\"checkbox\" onclick=\"setModel(this,'JoinType','" + dt.Rows[0]["modelid"].ToString() + "');\"" + (eBase.parseBool(dt.Rows[0]["JoinType"]) ? " checked" : "") + " />联合</label>");

                sb.Append("&nbsp;&nbsp;<select onchange=\"setModel(this,'JoinType','" + dt.Rows[0]["modelid"].ToString() + "');\">");
                sb.Append("<option value=\"0\"" + (dt.Rows[0]["JoinType"].ToString() == "0" ? "  selected=\"selected\"" : "") + ">不联合</option>");
                sb.Append("<option value=\"1\"" + (dt.Rows[0]["JoinType"].ToString() == "1" ? "  selected=\"selected\"" : "") + ">inner联合</option>");
                sb.Append("<option value=\"2\"" + (dt.Rows[0]["JoinType"].ToString() == "2" ? "  selected=\"selected\"" : "") + ">left联合</option>");
                sb.Append("<option value=\"3\"" + (dt.Rows[0]["JoinType"].ToString() == "3" ? "  selected=\"selected\"" : "") + ">right联合</option>");
                sb.Append("</select>");
            }

            if (dt.Rows[0]["Type"].ToString() != "3" && eBase.parseBool(dt.Rows[0]["subModel"]))
            {
                sb.Append("&nbsp;&nbsp;<label><input type=\"checkbox\" onclick=\"setModel(this,'show','" + dt.Rows[0]["modelid"].ToString() + "');\"" + (eBase.parseBool(dt.Rows[0]["show"]) ? " checked" : "") + " />启用</label>");
                
            }
            if ((dt.Rows[0]["Type"].ToString() == "1" || dt.Rows[0]["Type"].ToString() == "12") && eBase.parseBool(dt.Rows[0]["subModel"])) sb.Append("<label><input type=\"checkbox\" onclick=\"setModel(this,'ReadOnly','" + dt.Rows[0]["modelid"].ToString() + "');\"" + (eBase.parseBool(dt.Rows[0]["ReadOnly"]) ? " checked" : "") + " />只读</label>");
           
            
            sb.Append("</div>");

            DataTable tb = DataBase.getDataTable("select ModelID,MC from a_eke_sysModels where ParentID='" + pid + "' and Type in (1,3,6,7,8,12) and delTag=0 order by addtime,JoinMore");
            if (tb.Rows.Count == 0)
            {
                return sb.ToString();
            }
            else
            {
                for (int i = 0; i < tb.Rows.Count; i++)
                {
                    sb.Append(getModelTree(tb.Rows[i]["ModelID"].ToString(), space + 20));
                }
                return sb.ToString();
            }
        }
        private JsonData controljson;
        public JsonData ControlJson
        {
            get
            {
                if (controljson == null)
                {
                    controljson = new JsonData();
                    controljson.SetJsonType(JsonType.Array);
                    appendModelJson(controljson, ModelID);
                }
                return controljson;
            }
        }
        private void appendModelJson(JsonData jd,string modelid)
        {
            DataTable tb = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ModelID='" + modelid + "'");
            if (tb.Rows.Count == 0) return;
            DataRow row = tb.Rows[0];
            JsonData model = new JsonData();
            model["name"] = row["mc"].ToString();
            JsonData items = new JsonData();
            items.SetJsonType(JsonType.Array);
            appendModelItems(items, modelid);
            appendSubModel(items, modelid);
            model["items"] = items;
            jd.Add(model);

            //eBase.Writeln(modelid);
            DataTable dt = eBase.DataBase.getDataTable("select modelid,mc from a_eke_sysModels where ParentID='" + modelid + "' and JoinMore=0 and JoinType>0 and show=1 and deltag=0");
            foreach (DataRow dr in dt.Rows)
            {
                appendModelJson(jd, dr["modelid"].ToString());
            }            
        }
        private void appendModelItems(JsonData items, string modelid)
        {
            string sql="select ModelItemID,MC,Code,CustomCode,Num from a_eke_sysModelItems";
            sql += " where delTag=0 and ModelID='" + modelid + "' ";
            //sql += " and Code not in ('delTime','delUser','delTag') and MC not in ('序号','操作')";
            sql += " and isnull(Code,'') not in ('delTime','delUser','delTag') and MC not in ('操作')";
            sql += " order by num";


            sql = "select * from (";
            sql += "select ModelItemID,MC,Code,CustomCode,Num from a_eke_sysModelItems ";
            sql += " where delTag=0 and ModelID='" + modelid + "' ";
            //sql += " and isnull(Code,'') not in ('delTime','delUser','delTag') and MC not in ('序号','操作') ";
            sql += " and isnull(Code,'') not in ('delTime','delUser','delTag') and isnull(MC,'') not in ('操作') ";
            sql += " union ";
            sql += " select b.ModelItemID,b.MC,b.Code,b.CustomCode,b.Num from a_eke_sysModels a ";
            sql += " inner join a_eke_sysModelItems b on a.ModelID=b.ModelID ";
            sql += " where a.JoinMore=0 and b.delTag=0 and a.ParentID ='" + modelid + "' ";
            //sql += "and isnull( b.Code,'') not in ('" + tb.primaryKey + "','addTime','addUser','editTime','editUser','delTime','delUser','delTag') ";
            sql += "and isnull( b.Code,'') not in ('addTime','addUser','editTime','editUser','delTime','delUser','delTag') ";
            //sql += " and b.MC not in ('序号','操作') and b.primaryKey=0";
            sql += " and isnull(b.MC,'') not in ('序号','操作')";
            sql += ") as c order by c.Num";


            DataTable tb = eBase.DataBase.getDataTable(sql);
            //eBase.PrintDataTable(tb);
            foreach (DataRow dr in tb.Rows)
            {
                if (dr["Code"].ToString().Length > 0)
                {
                    JsonData item = new JsonData();
                    item["type"] = "data";
                    item["name"] = dr["MC"].ToString();
                    item["code"] = dr["Code"].ToString();
                    items.Add(item);
                }
            }

           

        }
        private void appendSubModel(JsonData items, string modelid)
        {
            DataTable tb = eBase.DataBase.getDataTable("SELECT ModelID,MC FROM a_eke_sysModels where ParentID='" + modelid + "' and show=1 and delTag=0 and JoinMore=1 and Auto=1");
            foreach (DataRow dr in tb.Rows)
            {
                JsonData item = new JsonData();
                item["type"] = "model";
                item["name"] = dr["MC"].ToString();
                item["modelid"] = dr["ModelID"].ToString();
                items.Add(item);
            }   
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //eResult.WriteJson(ControlJson);
            //eBase.WriteDiv(Regex.Unescape( ControlJson.ToJson()));
            //eBase.End();

            eUser user = new eUser("Manage");
            if (ModelInfo == null)
            {
                Response.Write("模块不存在!");
                Response.End();
            }

            /*
             print OBJECT_ID('eWeb_Columns') print OBJECT_ID('sys.extended_properties') select * from INFORMATION_SCHEMA.TABLES
             */
            string sql = "";
            if (act.Length > 0)
            {
                if (Request.UrlReferrer == null)
                {
                    eResult.Error("来源错误!");
                }
                else
                {
                    if (Request.Url.Host.ToLower() != Request.UrlReferrer.Host.ToLower())
                    {
                        eResult.Error("来源错误!");
                    }
                }
                eBase.clearDataCache(); //清除所有缓存
                if (value.Length == 0)
                {
                    value = eParameters.Form("value");
                    //eBase.AppendLog("11:" + value);
                    value = eBase.decode(value);
                    //eBase.AppendLog("22:" + value);
                    //value = value.Replace("'", "''");
                    if (value.ToLower() == "true") value = "1";
                    if (value.ToLower() == "false") value = "0";
                }
                if (act == "readonly")
                {
                    Session["model_readonly"] = eParameters.QueryString("value");
                    eResult.Success("设置成功!");
                }
                #region 备份模块
                #region 添加
                if (act == "addbakmodel")
                {
                    DataBase.Execute("insert into a_eke_sysModels (ModelID,Type,Auto,subModel,ParentID) values ('" + Guid.NewGuid().ToString() + "',9,1,1,'" + ModelID + "')");
                    eResult.Success("添加成功!");       
                }
                #endregion
                #region 修改
                if (act == "setbakmodel")
                {
                    if (value == "NULL")
                    {
                        DataBase.Execute("update a_eke_sysModels set " + item + "=" + value + " where ModelID='" + ModelID + "'");
                    }
                    else
                    {
                        DataBase.Execute("update a_eke_sysModels set " + item + "='" + value + "' where ModelID='" + ModelID + "'");
                    }
                    if (item.ToLower() == "code" && value == "NULL")
                    {
                        DataBase.Execute("update a_eke_sysModels set Foreignkey=null where ModelID='" + ModelID + "'");
                    }
                    eResult.Success("修改成功!");
                }
                #endregion
                #region 删除
                if (act == "delbakmodel")
                {
                    DataBase.Execute("delete from a_eke_sysModels where ModelID='" + ModelID + "'");
                    eResult.Success("删除成功!");
                }
                #endregion
                #endregion
                #region 打开、关闭数据表编辑功能
                if (act == "setdbstate")
                {
                    Session["dbeditstate_" + ModelID.Replace("-","")] = value;
                    eResult.Success("设置成功!");
                }
                #endregion
                #region 删除用户自定义信息
                if (act == "clearcustoms")
                {
                    sql = "delete from a_eke_sysUserColumns where ModelID='" + ModelID + "'";
                    eBase.UserInfoDB.Execute(sql);
                    sql = "delete from a_eke_sysUserCustoms where ModelID='" + ModelID + "'";
                    eBase.UserInfoDB.Execute(sql);
                    eBase.clearDataCache("a_eke_sysUserColumns");
                    eBase.clearDataCache("a_eke_sysUserCustoms");
                    eResult.Success("清除成功!");
                }
                #endregion
                #region 打印
                if (act == "addprint")
                {
                    eBase.DataBase.eTable("a_eke_sysModelPrints")
                       .Fields.Add("ModelID",  ModelID)           
                       .Add();
                    eResult.Success("添加成功,手动刷新后加载!");
                }
                if (act == "setprint")
                {
                    string modelprintid = eParameters.QueryString("modelprintid");
                    DataBase.eTable("a_eke_sysModelPrints").Fields.Add(item, value == "NULL" ? null : value).Update("ModelPrintID='" + modelprintid + "'");
                    eResult.Success("设置成功!");
                }
                #endregion
                #region 同步基础模块
                if (act == "syncmodel")
                {
                    #region 1.数据项
                    DataTable columns = DataBase.getSchemaColumns(ModelInfo["Code"].ToString(),false);
                    DataTable _items = DataBase.getDataTable("select * from a_eke_sysModelItems where ModelID='" + ModelID + "' and delTag=0 and Custom=0");
                    foreach (DataRow dr in columns.Rows)
                    {
                        DataRow[] rs = _items.Select("code='" + dr["COLUMN_NAME"].ToString() + "'");
                        if (rs.Length == 0)
                        {
                            string url = Request.Url.PathAndFile() + "?act=selcolumn&modelid=" + ModelID + "&code=" + dr["COLUMN_NAME"].ToString() + "&value=1&t=" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            System.Net.WebClient wc = new System.Net.WebClient();
                            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                            byte[] b = wc.DownloadData(url);
                            string str = System.Text.Encoding.GetEncoding("utf-8").GetString(b);   
                        }
                    }
                    #endregion

                    #region 2.数据绑定情况
                    foreach (DataRow dr in _items.Rows)
                    {
                        //eBase.Writeln(dr["code"].ToString());
                        sql = "update a set a.Options=b.Options,a.FormatString=b.FormatString,a.ReplaceString=b.ReplaceString,a.BindObject=b.BindObject";
                        sql += ",a.BindCode=b.BindCode";
                        sql += ",a.BindValue=b.BindValue";
                        sql += ",a.BindText=b.BindText";
                        sql += ",a.BindRows=b.BindRows";
                        sql += ",a.BindCondition=b.BindCondition";
                        sql += ",a.BindOrderBy=b.BindOrderBy";
                        sql += ",a.BindGroupBy=b.BindGroupBy";
                        sql += ",a.BindAuto=b.BindAuto";
                        sql += ",a.BindSQL=b.BindSQL";
                        sql += ",a.BindForeignkey=b.BindForeignkey";
                        sql += ",a.ControlType=b.ControlType";
                        sql += ",a.addControlType=b.addControlType";
                        sql += ",a.editControlType=b.editControlType";
                        sql += ",a.Width=b.Width";

                        sql+=" from a_eke_sysModelItems a inner join a_eke_sysModelItems b on a.code=b.code and b.modelid='" + ModelInfo["basemodelid"].ToString() + "'";
                        sql+=" where a.modelid='" + ModelInfo["modelid"].ToString() + "' and a.code='" + dr["code"].ToString() + "'";

                        DataBase.Execute(sql);
                    }
                    #endregion
                    #region 3.搜索条件
                    DataTable cond = DataBase.getDataTable("select * from a_eke_sysModelConditions where ModelID='" + ModelID + "' and delTag=0");
                   // eBase.PrintDataTable(cond);

                    DataTable bcond = DataBase.getDataTable("select * from a_eke_sysModelConditions where ModelID='" + ModelInfo["basemodelid"].ToString() + "' and delTag=0");
                    //eBase.PrintDataTable(bcond);

                    foreach (DataRow dr in bcond.Rows)
                    {
                        DataTable bconditem = DataBase.getDataTable("select * from a_eke_sysModelConditionItems where ModelConditionID='" + dr["ModelConditionID"].ToString() + "'");
                        //if (bconditem.Rows.Count > 0) eBase.PrintDataTable(bconditem);

                        DataRow[] rs = cond.Select("code='" + dr["code"].ToString() + "'");
                        if (rs.Length == 0) //添加
                        {
                            DataBase.Execute("update a_eke_sysModels set MaxConds=MaxConds+1 where ModelID='" + ModelID + "'");
                            string MaxConds = DataBase.getValue("select MaxConds from a_eke_sysModels where ModelID='" + ModelID + "'");


                            //eBase.Writeln(dr["code"].ToString() + "::添加" + rs.Length.ToString());
                            #region 添加
                            eTable etb = new eTable("a_eke_sysModelConditions", user);
                            etb.DataBase = DataBase;
                            etb.Fields.Add("modelid",ModelID);
                            etb.Fields.Add("Num", MaxConds);
                            etb.Fields.Add("code", dr["code"].ToString());
                            etb.Fields.Add("mc", dr["mc"].ToString());
                            etb.Fields.Add("ControlType", dr["ControlType"].ToString());
                            etb.Fields.Add("Width", dr["Width"].ToString());
                            etb.Fields.Add("DateFormat", dr["DateFormat"].ToString());
                            etb.Fields.Add("Custom", dr["Custom"].ToString());
                            etb.Fields.Add("Operator", dr["Operator"].ToString());

                            etb.Fields.Add("BindObject", dr["BindObject"].ToString());
                            etb.Fields.Add("BindRows", dr["BindRows"].ToString());
                            etb.Fields.Add("BindCode", dr["BindCode"].ToString());
                            etb.Fields.Add("BindValue", dr["BindValue"].ToString());
                            etb.Fields.Add("BindText", dr["BindText"].ToString());
                            etb.Fields.Add("BindCondition", dr["BindCondition"].ToString());
                            etb.Fields.Add("BindOrderBy", dr["BindOrderBy"].ToString());
                            etb.Fields.Add("BindGroupBy", dr["BindGroupBy"].ToString());
                            etb.Fields.Add("BindSQL", dr["BindSQL"].ToString());
                            etb.Fields.Add("BindForeignkey", dr["BindForeignkey"].ToString());
                            etb.Fields.Add("BindAuto", dr["BindAuto"].ToString());
                            etb.Fields.Add("defaultValue", dr["defaultValue"].ToString());
                            etb.Fields.Add("Options", dr["Options"].ToString());


                            etb.Add();
                            string ModelConditionID = etb.ID;
                            #endregion
                            #region 自定义条件
                            if (bconditem.Rows.Count > 0)
                            {
                                foreach (DataRow _dr in bconditem.Rows)
                                {
                                    etb = new eTable("a_eke_sysModelConditionItems", user);
                                    etb.Fields.Add("modelid", ModelID);
                                    etb.Fields.Add("ModelConditionID", ModelConditionID);
                                    etb.Fields.Add("mc", _dr["mc"].ToString());
                                    etb.Fields.Add("ConditionValue", _dr["ConditionValue"].ToString());
                                    etb.Add();
                                }
                            }
                            #endregion
                        }
                        else //更新
                        {


                            //eBase.Writeln(dr["code"].ToString() + "::更新" + rs.Length.ToString());
                            eTable etb = new eTable("a_eke_sysModelConditions", user);
                            etb.DataBase = DataBase;
                            //etb.Where.Add("modelid='" + ModelID + "' and code='" + dr["code"].ToString() + "' and deltag=0");
                            etb.Where.Add("ModelConditionID='" + rs[0]["ModelConditionID"].ToString() + "'");
                            etb.Fields.Add("Operator", dr["Operator"].ToString());
                            etb.Fields.Add("BindObject", dr["BindObject"].ToString());
                            etb.Fields.Add("BindRows", dr["BindRows"].ToString());
                            etb.Fields.Add("BindCode", dr["BindCode"].ToString());
                            etb.Fields.Add("BindValue", dr["BindValue"].ToString());
                            etb.Fields.Add("BindText", dr["BindText"].ToString());
                            etb.Fields.Add("BindCondition", dr["BindCondition"].ToString());
                            etb.Fields.Add("BindOrderBy", dr["BindOrderBy"].ToString());
                            etb.Fields.Add("BindGroupBy", dr["BindGroupBy"].ToString());
                            etb.Fields.Add("BindSQL", dr["BindSQL"].ToString());
                            etb.Fields.Add("BindForeignkey", dr["BindForeignkey"].ToString());
                            etb.Fields.Add("BindAuto", dr["BindAuto"].ToString());
                            etb.Fields.Add("Options", dr["Options"].ToString());
                            etb.Update();
                            #region 自定义条件
                            DataTable conditem = DataBase.getDataTable("select * from a_eke_sysModelConditionItems where ModelConditionID='" + rs[0]["ModelConditionID"].ToString() + "'");
                           // if (conditem.Rows.Count > 0) eBase.PrintDataTable(conditem);
                            if (bconditem.Rows.Count > 0)
                            {
                                foreach (DataRow _dr in bconditem.Rows)
                                {
                                    DataRow[] _rs = conditem.Select("mc='" + _dr["mc"].ToString() + "'");
                                    if (_rs.Length == 0)
                                    {
                                        //eBase.Writeln(dr["mc"].ToString() + ":: 添加");
                                        etb = new eTable("a_eke_sysModelConditionItems", user);
                                        etb.Fields.Add("modelid", ModelID);
                                        etb.Fields.Add("ModelConditionID", rs[0]["ModelConditionID"].ToString());
                                        etb.Fields.Add("mc", _dr["mc"].ToString());
                                        etb.Fields.Add("ConditionValue", _dr["ConditionValue"].ToString());

                                        etb.Add();
                                    }
                                    else
                                    {
                                        //eBase.Writeln(dr["mc"].ToString() + ":: 更新");
                                        etb = new eTable("a_eke_sysModelConditionItems", user);
                                        etb.Where.Add("ModelConditionItemID='" + _rs[0]["ModelConditionItemID"].ToString() + "'");
                                        etb.Fields.Add("ConditionValue",_dr["ConditionValue"].ToString());
                                        etb.Update();
                                    }
                                    //eBase.PrintDataRow(_dr);
                                }
                            }
                            #endregion
                        }
                       
                    }
                    #endregion


                    eResult.Success("操作成功!");
                }
                #endregion
                #region 复制还原表单编码
                if (act == "synccode")
                {
                    sql = "update a_eke_sysModelItems set frmName=Code,frmID=Code where ModelID='" + ModelID + "' and LEN(code)>0";
                    DataBase.Execute(sql);
                    eResult.Success("操作成功!");
                }
                if (act == "restorecode")
                {
                    switch (DataBase.DataBaseType)
                    {
                        case eDataBaseType.Oracle:
                        case eDataBaseType.SQLite:
                            sql = "update a_eke_sysModelItems set frmName='M" + ModelID.ToLower().Substring(0, 2) + "_F' || Num,frmID='M" + ModelID.ToLower().Substring(0, 2) + "_F' || Num where ModelID='" + ModelID + "' and LEN(code)>0";
                            break;
                        case eDataBaseType.MySQL:
                            sql = "update a_eke_sysModelItems set frmName=CONCAT('M" + ModelID.ToLower().Substring(0, 2) + "_F',Num),frmID=CONCAT('M" + ModelID.ToLower().Substring(0, 2) + "_F',Num) where ModelID='" + ModelID + "' and LEN(code)>0";
                            break;
                        default:
                            sql = "update a_eke_sysModelItems set frmName='M" + ModelID.ToLower().Substring(0, 2) + "_F' + cast(Num as varchar(5)),frmID='M" + ModelID.ToLower().Substring(0, 2) + "_F' + cast(Num as varchar(5)) where ModelID='" + ModelID + "' and LEN(code)>0";
                            break;
                    }                   
                    DataBase.Execute(sql);
                    eResult.Success("操作成功!");
                }                
                #endregion
                #region 数据结构
                #region 移动列顺序

                if (act == "movecolumn")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }

                    //string tableName = DataBase.getValue("select Code from a_eke_sysModels where ModelID='" + ModelID + "'");
                    string tableName = DataBase.eList("a_eke_sysModels").Fields.Add("code").Where.Add("ModelID='" + ModelID + "'").getValue();
                    if (tableName.Length == 0) eResult.Error("移动失败!");
           
                    DataTable Columns = ExDataBase.getColumns(tableName);
                    int index = Convert.ToInt32(eParameters.QueryString("index")) -1 ;
                    int nindex = Convert.ToInt32(eParameters.QueryString("nindex")) - 1;
                    Columns = eBase.moveColumn(Columns, index, nindex);


                    sql = ExDataBase.getTableSql(Columns, tableName);
                    //eBase.AppendLog(sql);
                    ExDataBase.Execute(sql);
                    eResult.Success("移动成功!");

                   

                }
                #endregion
                #region 选择列
                if (act == "selcolumn")
                {
                   
                    if (value == "1") //添加
                    {
                        DataRow[] dr = Columns.Select("COLUMN_NAME='" + eParameters.QueryString("code") + "'");
                      
                        if (dr.Length > 0)
                        {
                            //sql = "select count(*) from a_eke_sysModelItems Where ModelID='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                            //string ct = DataBase.getValue(sql);
                            string ct = new eList("a_eke_sysModelItems").Fields.Add("count(1)").Where.Add("ModelID='" + ModelID + "'").Where.Add("code", "=", eParameters.QueryString("code")).getValue();
                            //eBase.AppendLog(sql + "::" + ct);
                            string primaryKey = "0";
                            string zj = ExDataBase.getPrimaryKey(TableName);
                            if (zj.ToLower() == eParameters.QueryString("code").ToLower()) primaryKey = "1";
                            string syscolumns = eConfig.getAllSysColumns() + "," + zj.ToLower() + ",";
                            string sys = (syscolumns.IndexOf("," + dr[0]["COLUMN_NAME"].ToString().ToLower() + ",") > -1 ? "1" : "0");
                            if (ct == "0")//添加
                            {

                                DataBase.Execute("update a_eke_sysModels set MaxItems=MaxItems+1 where ModelID='" + ModelID + "'");
                                string MaxItems = DataBase.getValue("select MaxItems from a_eke_sysModels where ModelID='" + ModelID + "'");

                                string _itemid = Guid.NewGuid().ToString();
                                //sql = "insert into a_eke_sysModelItems (ModelItemID,Num,listOrder,ModelID,MC,Code,primaryKey,Type,Length,sys,PX) ";
                                //sql += " values ('" + _itemid + "','" + MaxItems + "','" + MaxItems + "','" + ModelID + "','" + dr[0]["DESCRIPTION"].ToString() + "','" + dr[0]["COLUMN_NAME"].ToString() + "'," + primaryKey + ",'" + dr[0]["DATA_TYPE"].ToString() + "','" + dr[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() + "'," + sys + ",'" + dr[0]["ORDINAL_POSITION"].ToString() + "')";
                                //DataBase.Execute(sql);

                                DataBase.eTable("a_eke_sysModelItems")
                                    .Fields.Add("ModelItemID", _itemid)
                                    .Fields.Add("Num", MaxItems)
                                    .Fields.Add("listOrder", MaxItems)
                                    .Fields.Add("ModelID", ModelID)
                                    .Fields.Add("MC", dr[0]["DESCRIPTION"].ToString())
                                    .Fields.Add("Code", dr[0]["COLUMN_NAME"].ToString())
                                    .Fields.Add("primaryKey", primaryKey)
                                    .Fields.Add("Type", dr[0]["DATA_TYPE"].ToString())
                                    .Fields.Add("Length", dr[0]["CHARACTER_MAXIMUM_LENGTH"].ToString())
                                    .Fields.Add("sys", sys)
                                    .Fields.Add("PX", dr[0]["ORDINAL_POSITION"].ToString())
                                    .Add();

        
      
                                
    
                                #region 设置默认值
                                string frmName = "M" + ModelID.Substring(0, 2) + "_" + "F" + MaxItems;
                                eTable _etb = DataBase.eTable("a_eke_sysModelItems");
                                _etb.Fields.Add("sys", sys);
                                _etb.Fields.Add("frmName", frmName);
                                _etb.Fields.Add("frmID", frmName);        
                                if (sys == "0") //非系统列
                                {
                                    _etb.Fields.Add("ControlType","text");
                                    _etb.Fields.Add("addControlType", "text");
                                    _etb.Fields.Add("editControlType", "text");
                                    _etb.Fields.Add("showList", 1);
                                    _etb.Fields.Add("showView", 1);
                                    _etb.Fields.Add("showAdd", 1);
                                    _etb.Fields.Add("showEdit", 1);
                                    bool submodel = eBase.parseBool(DataBase.getValue("select subModel from a_eke_sysModels where ModelID='" + ModelID + "'"));
                                    if (!submodel)
                                    {
                                        _etb.Fields.Add("OrderBy", 1);
                                        _etb.Fields.Add("Move", 1);
                                        _etb.Fields.Add("Size", 1);
                                    }
                                }
                                else
                                {
                                    _etb.Fields.Add("editControlType", "text");
                                }
                                _etb.Where.Add("ModelItemID='" + _itemid + "'");
                                _etb.Update();
                               



                                #region 客户端验证
                                //dr[0]["COLUMN_NAME"].ToString().ToLower()
                                eTable etb = new eTable("a_eke_sysModelItems");
                                if (dr[0]["DATA_TYPE"].ToString().ToLower().IndexOf("uniqueidentifier") > -1)
                                {
                                    etb.Fields.Add("minlength", "36");
                                    etb.Fields.Add("maxlength", "36");
                                }
                                if (dr[0]["DATA_TYPE"].ToString().ToLower().IndexOf("int") > -1)
                                {
                                    etb.Fields.Add("datatype", "int");
                                }
                                if (dr[0]["DATA_TYPE"].ToString().ToLower().IndexOf("date") > -1)
                                {
                                    etb.Fields.Add("datatype", "date");
                                    etb.Fields.Add("ControlType", "date");
                                    etb.Fields.Add("ControlType", "date");
                                    etb.Fields.Add("ControlType", "date");
                                    etb.Fields.Add("formatstring", (dr[0]["DATA_TYPE"].ToString().ToLower().IndexOf("datetime") > -1 ? "{0:yyyy-MM-dd HH:mm:ss}" : "{0:yyyy-MM-dd}"));
                                    etb.Fields.Add("dateformat", "yyyy-MM-dd HH:mm:ss");
                                }
                                if (",float,numeric,decimal,".IndexOf("," + dr[0]["DATA_TYPE"].ToString().ToLower() + ",") > -1)
                                {
                                    etb.Fields.Add("datatype", "float");
                                }

                                etb.Where.Add("ModelItemID ='" + _itemid + "'");
                                etb.Update();
                                #endregion

                                #endregion
                            }
                            else //修改
                            {
                                //sql = "update a_eke_sysModelItems set delTag=0,MC='" + dr[0]["DESCRIPTION"].ToString() + "',Type='" + dr[0]["DATA_TYPE"].ToString() + "',Length='" + dr[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() + "',PX='" + dr[0]["ORDINAL_POSITION"].ToString() + "',SYS='" + sys + "' where ModelID='" + ModelID + "' and Code='" + dr[0]["COLUMN_NAME"].ToString() + "'";
                                //DataBase.Execute(sql);
                                DataBase.eTable("a_eke_sysModelItems").Fields.Add("deltag", 0)
                                    .Fields.Add("mc", dr[0]["DESCRIPTION"].ToString())
                                    .Fields.Add("Type", dr[0]["DATA_TYPE"].ToString())
                                    .Fields.Add("Length", dr[0]["CHARACTER_MAXIMUM_LENGTH"].ToString())
                                    .Fields.Add("PX", dr[0]["ORDINAL_POSITION"].ToString())
                                    .Fields.Add("SYS", sys)
                                    .Where.Add("ModelID='" + ModelID + "'")
                                    .Where.Add("code", "=", dr[0]["COLUMN_NAME"].ToString())
                                    .Update();
                            }
                        }
                    }
                    else//删除
                    {
                        //sql = "update a_eke_sysModelItems set delTag=1 Where ModelID='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";//Code为mysql关键词
                        //DataBase.Execute(sql);
                        DataBase.eTable("a_eke_sysModelItems").Where.Add("ModelID='" + ModelID + "'").Where.Add("code", "=", eParameters.QueryString("code")).Delete();
                    }
                    eResult.Success("选择成功!");
                }
                #endregion
                #region 添加列-新
                if (act == "addcolumnnew")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.addSchemaColumn(TableName, eParameters.Form("code"), eParameters.Form("type"), eParameters.Form("name"));
                    eResult.Success("添加成功!");
                }
                #endregion
                if (act == "addcolumn2")
                {
                    string code = eParameters.QueryString("code");
                    DataRow[] rs = eBase.CommonColumns.Select("COLUMN_NAME='" + code + "'");
                    if (rs.Length > 0)
                    {
                        DataRow dr = rs[0];
                        ExDataBase.addSchemaColumn(TableName, dr);
                        //eBase.AppendLog("addcolumn2:" + code + "::" + rs.Length.ToString() + "::" + TableName);
                    }
                    eResult.Success("添加成功!");
                }
                #region 添加列
                if (act == "addcolumn")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    string code = "F" + (Columns.Rows.Count + 1).ToString();
                    ExDataBase.addSchemaColumn(TableName, code);
                    eResult.Success("添加成功!");
                }
                #endregion
                #region 删除列
                if (act == "delcolumn")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.removeSchemaColumn(TableName, eParameters.QueryString("code"));
                    sql = "delete from a_eke_sysModelItems where ModelID='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                    DataBase.Execute(sql);
                    sql = "delete from a_eke_sysModelConditions where ModelID='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                    DataBase.Execute(sql);
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #region 重命名列
                if (act == "renamecolumn")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.renameSchemaColumn(TableName, eParameters.QueryString("code"), eParameters.QueryString("newcode"));
                    sql = "update a_eke_sysModelItems set code='" + eParameters.QueryString("newcode") + "' where ModelId='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                    DataBase.Execute(sql);
                    sql = "update a_eke_sysModelConditions set code='" + eParameters.QueryString("newcode") + "' where ModelId='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                    DataBase.Execute(sql);
                    ExDataBase.removePrimaryKeys(); //清除主键缓存
                    eResult.Success("重命名成功!");
                }
                #endregion
                #region 列说明
                if (act == "columnname")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.setSchemaColumnDescription(TableName, eParameters.QueryString("code"), value);
                    sql = "update a_eke_sysModelItems set MC='" + value + "' where ModelId='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "' and len(isnull(mc,''))=0";
                    DataBase.Execute(sql);
                    eResult.Success("设置成功!"); 
                }
                #endregion
                #region 小数位
                if (act == "columnscale")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.setSchemaColumnScale(TableName, eParameters.QueryString("code"), value);
                    eResult.Success("设置成功!");                   
                }
                #endregion
                #region 默认值
                if (act == "columndefault")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.setSchemaColumnDefault(TableName, eParameters.QueryString("code"), value);
                    eResult.Success("设置成功!"); 
                }
                #endregion
                #region 数据类型
                if (act == "columntype")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.setSchemaColumnDateType(TableName, eParameters.QueryString("code"), value);
                    DataTable dt = ExDataBase.getSchemaColumns(TableName,false);
                    DataRow[] rows = dt.Select("COLUMN_NAME='" + eParameters.QueryString("code") + "'");
                    if (rows.Length > 0)
                    {
                        DataBase.Execute("update a_eke_sysModelItems set Type='" + rows[0]["DATA_TYPE"].ToString() + "',Length='" + rows[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() + "' where ModelId='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'");
                    }
                    eResult.Success("设置成功!"); 
                    
                }
                #endregion
                #region 列长度
                if (act == "columnlength")
                {
                    if (!ExDataBase.Structure)
                    {
                        eResult.Error("禁止修改!");
                    }
                    ExDataBase.setSchemaColumnLength(TableName, eParameters.QueryString("code"), value);                    
                    DataTable dt = ExDataBase.getSchemaColumns(TableName,false);
                    DataRow[] rows = dt.Select("COLUMN_NAME='" + eParameters.QueryString("code") + "'");
                    if (rows.Length > 0)
                    {
                        sql = "update a_eke_sysModelItems set Length='" + rows[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() + "' where ModelId='" + ModelID + "' and Code='" + eParameters.QueryString("code") + "'";
                    }
                    DataBase.Execute(sql);
                    eResult.Success("设置成功!"); 
                }

                #endregion
                #endregion
                #region 模块
                string modelitemid = eParameters.QueryString("modelitemid");
                #region 模块扩展属性
                if (act == "setmodelpropertys")
                {
                    string oldValue = DataBase.getValue("select Propertys from a_eke_sysModels where ModelID='" + ModelID + "'");
                    JsonData jd = JsonMapper.ToObject("{}");
                    if (oldValue.Length > 2 && oldValue.StartsWith("{"))
                    {
                        jd = JsonMapper.ToObject(oldValue);
                    }
                    jd.Add(item, value);

                    DataBase.Execute("update a_eke_sysModels set Propertys='" + jd.ToJson() + "' where ModelID='" + ModelID + "'");
                    eResult.Success("设置成功!"); 
                }
                #endregion
                if (act == "setmodel")
                {
                   // eBase.AppendLog(item + "=" + value);
                    if (item.ToLower() == "code")
                    {
                        string oldName = DataBase.getValue("select code from a_eke_sysModels where ModelID='" + ModelID + "'");
                        ExDataBase.renameSchemaTable(oldName, value);
                    }
                    if (item.ToLower() == "modelcondition")
                    {
                        /*
                        sql = "if exists (select * from a_eke_sysConditions Where ModelID='" + ModelID + "' and RoleID is null and UserID is null)";
                        sql += "update a_eke_sysConditions set CondValue='" + value + "' where ModelID='" + ModelID + "' and RoleID is null and UserID is null";
                        sql += " else ";
                        sql += "insert into a_eke_sysConditions (ModelConditionID,ModelID,CondValue) ";
                        sql += " values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','" + value + "')";
                        */
                        string ct = DataBase.getValue("select count(1) from a_eke_sysConditions Where ModelID='" + ModelID + "' and RoleID is null and UserID is null");
                        if (ct == "0")
                        {
                            sql = "insert into a_eke_sysConditions (ModelConditionID,ModelID,CondValue) ";
                            sql += " values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','" + value + "')";
                        } 
                        else
                        {
                            sql += "update a_eke_sysConditions set CondValue='" + value + "' where ModelID='" + ModelID + "' and RoleID is null and UserID is null";                        
                        }
                        DataBase.Execute(sql);
                    }
                    else 
                    {
                        //eBase.AppendLog("XX:" + value);
                        //DataBase.Execute("update a_eke_sysModels set " + item + "='" + value + "' where ModelID='" + ModelID + "'");
                        //new eTable("a_eke_sysModels").Fields.Add(item,value).Update("ModelID='" + ModelID + "'");
                        new eTable("a_eke_sysModels").Fields.Add(item, value == "NULL" ? null : value).Where.Add("ModelID='" + ModelID + "'").Update();
                        if (item.ToLower() == "modeltabid")
                        {
                            DataBase.Execute("update a_eke_sysModels set ModelPanelID=NULL where ModelID='" + ModelID + "'");
                        }
                    }
                    if (item.ToLower() == "addcolumncount")
                    {
                        DataBase.Execute("update a_eke_sysModels set editcolumncount='" + value + "',viewcolumncount='" + value + "' where ModelID='" + ModelID + "'");
                    }

                    eResult.Success("设置成功!"); 
                }
                #endregion
                #region 列
                #region 模块扩展属性
                if (act == "setmodelitempropertys")
                {
                    string oldValue = DataBase.getValue("select Propertys from a_eke_sysModelItems where ModelItemID='" + modelitemid + "'");
                    JsonData jd = JsonMapper.ToObject("{}");
                    if (oldValue.Length > 2 && oldValue.StartsWith("{"))
                    {
                        jd = JsonMapper.ToObject(oldValue);
                    }
                    jd.Add(item, value);

                    DataBase.Execute("update a_eke_sysModelItems set Propertys='" + jd.ToJson() + "' where ModelItemID='" + modelitemid + "'");
                    eResult.Success("设置成功!"); 
                }
                #endregion

               
                if (act == "quickbind")
                {
                    if (value.ToLower() == "a_eke_sysUsers".ToLower())
                    {
                        DataBase.Execute("update a_eke_sysModelItems set BindObject='a_eke_sysUsers',BindValue='UserID',BindText='XM',BindCondition='',BindOrderBy='',BindAuto=1 where ModelItemID='" + modelitemid + "'");
                    }
                    else
                    {
                        DataBase.Execute("update a_eke_sysModelItems set BindObject='Dictionaries',BindValue='DictionarieID',BindText='MC',BindCondition='ParentID=''" + value + "'' and deltag=0 and show=1',BindOrderBy='px,addtime',BindAuto=1 where ModelItemID='" + modelitemid + "'");
                    }
                    eResult.Success("设置成功!"); 
                }
                if (act == "addmodelitem")
                {
                    DataBase.Execute("update a_eke_sysModels set MaxItems=MaxItems+1 where ModelID='" + ModelID + "'");
                    string MaxItems = DataBase.getValue("select MaxItems from a_eke_sysModels where ModelID='" + ModelID + "'");
                    // string clientprefix = DataBase.getValue("select ClientPrefix from a_eke_sysModels where ModelID='" + ModelID + "'");
                    string frmName = "M" + ModelID.Substring(0, 2) + "_" + "F" + MaxItems.ToString(); ;

                    //DataBase.Execute("insert into a_eke_sysModelItems (ModelItemID,NUM,listOrder,FrmName,FrmID,ModelID,Custom) values ('" + Guid.NewGuid().ToString() + "','" + MaxItems + "','" + MaxItems + "','" + frmName + "','" + frmName + "','" + ModelID + "','1')");
                    DataBase.eTable("a_eke_sysModelItems")
                        .Fields.Add("ModelItemID", Guid.NewGuid().ToString())
                        .Fields.Add("NUM", MaxItems)
                        .Fields.Add("listOrder", MaxItems)
                        .Fields.Add("FrmName", frmName)
                        .Fields.Add("FrmID", frmName)
                        .Fields.Add("ModelID", ModelID)
                        .Fields.Add("Custom", 1)
                        .Add();
                    
                    eResult.Success("添加成功!"); 

                }
                if (act == "delmodelitem")
                {
                    DataBase.Execute("delete from a_eke_sysModelItems where ModelItemID='" + modelitemid + "'");
                    eResult.Success("删除成功!"); 
                }
                if (act == "setmodelitem")
                {
                    if (item.ToLower() == "fillmodelitemid")
                    {
                        string oldvalue = DataBase.getValue("select fillmodelitemid from a_eke_sysModelItems where ModelItemID='" + modelitemid + "'");
                        
                        //if(oldvalue.
                        //eBase.Writeln("OK::" + oldvalue + "::" + value);

                        DataBase.Execute("update a_eke_sysModelItems set fillmodelitemid='" + value + "' where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    //导出文件名
                    if (item.ToLower() == "exportname")
                    {
                        //DataBase.Execute("update a_eke_sysModelItems set mliststate=0 where ModelID='" + ModelID + "'");

                        DataBase.Execute("update a_eke_sysModelItems set exportname=0 where ModelID='" + ModelID + "' and ModelItemID<>'" + modelitemid + "'");
                        DataBase.Execute("update a_eke_sysModelItems set exportname=" + value + " where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!");
                    }
                   //移动端状态列
                    if (item.ToLower() == "mliststate")
                    {
                        //DataBase.Execute("update a_eke_sysModelItems set mliststate=0 where ModelID='" + ModelID + "'");

                        DataBase.Execute("update a_eke_sysModelItems set mliststate=0 where ModelID='" + ModelID + "' and ModelItemID<>'" + modelitemid + "'");
                        DataBase.Execute("update a_eke_sysModelItems set mliststate=" + value + " where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    //拖动排序
                    if (item.ToLower() == "setorders")
                    {
                        string ids=eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelItems set AddOrder='" + value + "',EditOrder='" + value + "',ViewOrder='" + value + "' where ModelItemID='" + arr[i] + "'"); //1V1为不同ModelID  ModelID='" + ModelID + "' and 
                        }
                        DataBase.Execute("update a_eke_sysModelItems set AddOrder='999999',EditOrder='999999',ViewOrder='999999' where ModelID='" + ModelID + "' and ModelItemID not in ('" + ids.Replace(",", "','") + "')");
                        eResult.Success("设置成功!"); 
                    }
                    //拖动排序-列表
                    if (item.ToLower() == "setlistorders")
                    {
                        string ids = eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelItems set ListOrder='" + value + "' where ModelItemID='" + arr[i] + "'");//ModelID='" + ModelID + "' and
                        }
                        DataBase.Execute("update a_eke_sysModelItems set ListOrder='999999' where ModelID='" + ModelID + "' and ModelItemID not in ('" + ids.Replace(",", "','") + "')");
                        eResult.Success("设置成功!"); 
                    }
                    //拖动排序-导出
                    if (item.ToLower() == "setexportorders")
                    {
                        string ids = eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelItems set ExportOrder='" + value + "' where ModelItemID='" + arr[i] + "'");// ModelID='" + ModelID + "' and
                        }
                        DataBase.Execute("update a_eke_sysModelItems set ExportOrder='999999' where ModelID='" + ModelID + "' and ModelItemID not in ('" + ids.Replace(",", "','") + "')");
                        eResult.Success("设置成功!"); 
                    }

                    if (item.ToLower() == "listhtml")
                    {
                        //DataBase.Execute("update a_eke_sysModelItems set viewhtml='" + value + "' where ModelID='" + ModelID + "' and ModelItemID='" + modelitemid + "'");
                    }
                    if (item.ToLower() == "listorder" && (value.Length == 0 || value == "0")) value = "999999";
                    if (item.ToLower() == "exportorder" && (value.Length == 0 || value == "0")) value = "999999";
                    #region 日期格式
                    if (item.ToLower() == "dateformat")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set FormatString='" + (value.Length > 0 ? "{0:" + value + "}" : "") + "' where ModelItemID='" + modelitemid + "'");
                    }
                    #endregion
                    if (item.ToLower() == "addorder")
                    {
                        if (value.Length == 0 || value == "0") value = "999999";
                        DataBase.Execute("update a_eke_sysModelItems set addorder='" + value + "',editorder='" + value + "',vieworder='" + value + "' where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "showadd_bak")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set showadd='" + value + "',showedit='" + value + "',showview='" + value + "' where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "controltype")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set controltype='" + value + "',addcontroltype='" + value + "',editcontroltype='" + value + "' where ModelItemID='" + modelitemid + "'");
                        if (value != "text")
                        {
                            DataBase.Execute("update a_eke_sysModelItems set width='' where ModelItemID='" + modelitemid + "' and width='300'");
                        }
                        if (value == "file")
                        {
                            DataBase.Execute("update a_eke_sysModelItems set maxlength='0' where ModelItemID='" + modelitemid + "'");
                        }

                        //星级评分
                        if (value == "raty")
                        {
                            string options = "[{\"text\":\"data-number\",\"value\":\"5\"}";
                            options += ",{\"text\":\"data-staroff\",\"value\":\"raty_star-off.png\"}";
                            options += ",{\"text\":\"data-starhalf\",\"value\":\"raty_star-half.png\"}";
                            options += ",{\"text\":\"data-staron\",\"value\":\"raty_star-on.png\"}";
                            options += ",{\"text\":\"data-half\",\"value\":\"false\"}";
                            options += "]";

                            DataBase.Execute("update a_eke_sysModelItems set Options='" + options + "' where ModelItemID='" + modelitemid + "'");
                        }
                        //非数据选择框，取消绑定关系
                        if (value != "datatext")
                        {
                            DataBase.Execute("update a_eke_sysModelItems set fillmodelid=NULL where ModelItemID='" + modelitemid + "'");
                        }

                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "fillmodelid")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set " + item + "=" + (value.Length==0 ? "NULL" : "'" + value + "'") + " where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "addcolspan")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set addcolspan='" + value + "',editcolspan='" + value + "',viewcolspan='" + value + "' where  ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "addrowspan")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set addrowspan='" + value + "',editrowspan='" + value + "',viewrowspan='" + value + "' where ModelItemID='" + modelitemid + "'");
                        eResult.Success("设置成功!"); 
                    }
                    if (item.ToLower() == "listwidth" && value!="0")
                    {
                        eBase.UserInfoDB.Execute("update a_eke_sysUserColumns set width='" + value + "' where ModelItemID='" + modelitemid + "' and width=0");
                    }
                    if (value == "NULL")
                    {
                        //DataBase.Execute("update a_eke_sysModelItems set " + item + "=" + value + " where ModelItemID='" + modelitemid + "'");
                    }
                    else
                    {
                        /*
                        DataTable dt = DataBase.getSchemaColumns("a_eke_sysModelItems");
                        //DataRow[] rs = dt.Select("PrimaryKey='True'");
                        DataRow[] rs = dt.Select("COLUMN_NAME='" + item + "'");
                        if (rs.Length > 0 && rs[0]["Data_Type"].ToString() == "bit")
                        {
                            sql = "update a_eke_sysModelItems set " + item + "=" + value + " where ModelItemID='" + modelitemid + "'";
                        }
                        else
                        {
                            sql = "update a_eke_sysModelItems set " +  item + "=" + (DataBase.DataBaseType == eDataBaseType.SQLServer ? "N" : "") + "'" + value + "' where ModelItemID='" + modelitemid + "'";
                        }
                        DataBase.Execute(sql);
                        eBase.AppendLog(rs.Length.ToString() + "::" + sql + "::" + rs[0]["Data_Type"].ToString());
                        */
                        

                    }
                    DataBase.eTable("a_eke_sysModelItems").Fields.Add(item, value == "NULL" ? null : value).Update("ModelItemID='" + modelitemid + "'");

                    if (item.ToLower() == "modeltabid")
                    {
                        DataBase.Execute("update a_eke_sysModelItems set ModelPanelID=NULL where ModelItemID='" + modelitemid + "'");
                    }
                    eResult.Success("设置成功!"); 
                }
                #endregion
                #region 条件

                string modelconditionid = eParameters.QueryString("modelconditionid");
                string modelconditionitemid = eParameters.QueryString("modelconditionitemid");
                if (act == "loadcolumnoptions")
                {

                    sql = "update a set a.DataSourceID=b.DataSourceID,a.BindObject=b.BindObject,a.BindRows=b.BindRows,a.BindValue=b.BindValue,a.BindText=b.BindText,a.BindCondition=b.BindCondition,a.BindOrderBy=b.BindOrderBy,a.BindGroupBy=b.BindGroupBy,a.BindForeignkey=b.BindForeignkey,a.Options=b.Options,a.BindSQL=b.BindSQL,a.BindAuto=b.BindAuto from a_eke_sysModelConditions a inner join a_eke_sysModelItems b on a.ModelID=b.ModelID and a.Code=b.code ";
                    sql += " where a.ModelConditionID='" + modelconditionid + "'";
                    DataBase.Execute(sql);
                    eResult.Success("设置成功!"); 
                }
                if (act == "addmodelcondition")
                {
                    //string maxnum = DataBase.getValue("select isnull(MAX(Num),0)+1 from a_eke_sysModelConditions where ModelID='" + ModelID + "'");
                    //string frmname = "s" + maxnum;
                    //DataBase.Execute("insert into a_eke_sysModelConditions (ModelConditionID,ModelID,Num,frmName) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','" + maxnum + "','" + frmname + "')");



                    DataBase.Execute("update a_eke_sysModels set MaxConds=MaxConds+1 where ModelID='" + ModelID + "'");
                    string MaxConds = DataBase.getValue("select MaxConds from a_eke_sysModels where ModelID='" + ModelID + "'");



                    //DataBase.Execute("insert into a_eke_sysModelConditions (ModelConditionID,ModelID,Num) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','" + MaxConds + "')");
                    DataBase.eTable("a_eke_sysModelConditions")
                        .Fields.Add("ModelConditionID", Guid.NewGuid().ToString())
                        .Fields.Add("ModelID", ModelID)
                        .Fields.Add("Num", MaxConds)
                        .Add();
                    eResult.Success("添加成功!"); 
                }
                if (act == "setmodelcondition")
                {
                    if (item.ToLower() == "code")
                    {
                        string smodelid = eParameters.QueryString("smodelid");
                        if (smodelid.Length == 0) smodelid = ModelID;
                        if (value == "NULL")
                        {
                            //DataBase.Execute("update a_eke_sysModelConditions set " + item + "=" + value + ",ModelID='" + smodelid + "' where ModelConditionID='" + modelconditionid + "'");//ModelID='" + ModelID + "' and 
                        }
                        else
                        {
                            //DataBase.Execute("update a_eke_sysModelConditions set " + item + "='" + value + "',ModelID='" + smodelid + "' where ModelConditionID='" + modelconditionid + "'");//ModelID='" + ModelID + "' and 
                        }
                        DataBase.eTable("a_eke_sysModelConditions")
                            .Fields.Add(item, value == "NULL" ? null : value)
                            .Fields.Add("ModelID", smodelid)
                            .Where.Add("ModelConditionID='" + modelconditionid + "'")
                            .Update();
                        eResult.Success("设置成功!"); 
                    }
                     //拖动排序
                    if (item.ToLower() == "setorders")
                    {
                        string ids=eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelConditions set px='" + value + "' where ModelConditionID='" + arr[i] + "'");//ModelID='" + ModelID + "' and 
                        }
                        DataBase.Execute("update a_eke_sysModelConditions set px='999999' where ModelID='" + ModelID + "' and ModelConditionID not in ('" + ids.Replace(",", "','") + "')");
                        eResult.Success("设置成功!"); 
                    }


                    if (item.ToLower() == "px" && (value.Length == 0 || value == "0")) value = "999999";
                    //DataBase.Execute("update a_eke_sysModelConditions set " + item + "='" + value + "' where ModelConditionID='" + modelconditionid + "'");//ModelID='" + ModelID + "' and 
                    DataBase.eTable("a_eke_sysModelConditions").Fields.Add(item, value == "NULL" ? null : value).Where.Add("ModelConditionID='" + modelconditionid + "'").Update();
                    eResult.Success("设置成功!"); 
                }
                if (act == "delmodelcondition")
                {
                    DataBase.Execute("delete from a_eke_sysModelConditionItems where ModelConditionID='" + modelconditionid + "'");//ModelID='" + ModelID + "' and
                    DataBase.Execute("delete from a_eke_sysModelConditions where ModelConditionID='" + modelconditionid + "'");//ModelID='" + ModelID + "' and 
                    //DataBase.Execute("update a_eke_sysModelConditionItems set delTag=1 where ModelID='" + ModelID + "' and ModelConditionID='" + modelconditionid + "'");
                    //DataBase.Execute("update a_eke_sysModelConditions set delTag=1 where ModelID='" + ModelID + "' and ModelConditionID='" + modelconditionid + "'");
                    eResult.Success("删除成功!"); 
                }

                if (act == "addmodelconditionitem")
                {
                    //DataBase.Execute("insert into a_eke_sysModelConditionItems (ModelConditionItemID,ModelID,ModelConditionID) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','" + modelconditionid + "')");
                    DataBase.eTable("a_eke_sysModelConditionItems")
                        .Fields.Add("ModelConditionItemID",Guid.NewGuid().ToString())
                        .Fields.Add("ModelID", ModelID)
                        .Fields.Add("ModelConditionID", modelconditionid)
                        .Add();
                    eResult.Success("添加成功!"); 
                }
                if (act == "setmodelconditionitem")
                {
                    if (item.ToLower() == "px" && (value.Length == 0 || value == "0")) value = "999999";
                    //DataBase.Execute("update a_eke_sysModelConditionItems set " + item + "='" + value + "' where ModelConditionItemID='" + modelconditionitemid + "'");//ModelID='" + ModelID + "' and 
                    DataBase.eTable("a_eke_sysModelConditionItems").Fields.Add(item, value == "NULL" ? null : value).Where.Add("ModelConditionItemID='" + modelconditionitemid + "'").Update();
                    eResult.Success("设置成功!"); 
                }
                if (act == "delmodelconditionitem")
                {
                    DataBase.Execute("delete from a_eke_sysModelConditionItems where ModelConditionItemID='" + modelconditionitemid + "'");// ModelID='" + ModelID + "' and 
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #region 动作
                #region 添加动作
                if (act == "addaction")
                {
                    //DataBase.Execute("insert into a_eke_sysActions (ActionID,ModelID) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "')");
                    DataBase.eTable("a_eke_sysActions").Fields.Add("ActionID", Guid.NewGuid().ToString()).Fields.Add("ModelID", ModelID).Add();
                    eResult.Success("添加成功!"); 
                }
                #endregion
                #region 修改动作
                if (act == "setaction")
                {
                    //DataBase.Execute("update a_eke_sysActions set " + item + "='" + value + "' where ModelID='" + ModelID + "' and ActionID='" + eParameters.QueryString("ActionID") + "'");
                    DataBase.eTable("a_eke_sysActions").Fields.Add(item, value == "NULL" ? null : value).Update("ActionID='" + eParameters.QueryString("ActionID") + "'");
                    eResult.Success("修改成功!"); 
                }
                #endregion
                #region  删除动作
                if (act == "delaction")
                {
                    DataBase.Execute("delete from a_eke_sysActions where ActionID='" + eParameters.QueryString("ActionID") + "'");
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #endregion
                #region 报表
                if (act == "addreport")
                {
                    //DataBase.Execute("insert into a_eke_sysReports (ReportID,ModelID,ControlType) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "','table')");
                    DataBase.eTable("a_eke_sysReports").Fields.Add("ReportID", Guid.NewGuid().ToString()).Fields.Add("ModelID", ModelID).Fields.Add("ControlType", "table").Add();
                    eResult.Success("添加成功!"); 
                }
                if (act == "setreport")
                {
                    if (value == "NULL")
                    {
                        //DataBase.Execute("update a_eke_sysReports set " + item + "=" + value + " where ReportID='" + eParameters.QueryString("ReportID") + "'");

                    }
                    else
                    {
                        //DataBase.Execute("update a_eke_sysReports set " + item + "=" + (DataBase.DataBaseType == eDataBaseType.SQLServer ? "N" : "") + "'" + value + "' where ReportID='" + eParameters.QueryString("ReportID") + "'");
                    }
                    DataBase.eTable("a_eke_sysReports").Fields.Add(item, value == "NULL" ? null : value).Update("ReportID='" + eParameters.QueryString("ReportID") + "'");
                    eResult.Success("修改成功!"); 
                }
                if (act == "delreport")
                {
                    DataBase.Execute("delete from a_eke_sysReportItems where ReportID='" + eParameters.QueryString("ReportID") + "'");
                    DataBase.Execute("delete from a_eke_sysReports where ReportID='" + eParameters.QueryString("ReportID") + "'");

                    eResult.Success("删除成功!"); 
                }
                if (act == "setreportorders")
                {

                    string ids = eParameters.Form("ids");
                    string[] arr = ids.Split(",".ToCharArray());
                    for (int i = 0; i < arr.Length; i++)
                    {
                        value = (i + 1).ToString();
                        DataBase.Execute("update a_eke_sysReports set PX='" + value + "' where ModelID='" + ModelID + "' and ReportID='" + arr[i] + "'");
                    }
                    eBase.clearDataCache("a_eke_sysReports");
                    eResult.Success("修改成功!"); 
                }
                if (act == "setsearchoptionsorders")
                {

                    string ids = eParameters.Form("ids");
                    string[] arr = ids.Split(",".ToCharArray());
                    for (int i = 0; i < arr.Length; i++)
                    {
                        value = (i + 1).ToString();
                        DataBase.Execute("update a_eke_sysModelConditionItems set PX='" + value + "' where ModelID='" + ModelID + "' and ModelConditionItemID='" + arr[i] + "'");
                    }
                    eBase.clearDataCache("a_eke_sysModelConditionItems");
                    eResult.Success("修改成功!");
                }
                
                #endregion
                #region 流程
                #region 添加
                if (act == "addcheckup")
                {
                    //DataBase.Execute("insert into a_eke_sysCheckUps (CheckupID,ModelID) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "')");
                    DataBase.eTable("a_eke_sysCheckUps").Fields.Add("CheckupID", Guid.NewGuid().ToString()).Fields.Add("ModelID", ModelID).Add();
                    eResult.Success("添加成功!"); 
                }
                #endregion
                #region 修改动作
                if (act == "setcheckup")
                {
                    //拖动排序
                    if (item.ToLower() == "setorders")
                    {
                        string ids = eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysCheckUps set px='" + value + "' where ModelID='" + ModelID + "' and CheckupID='" + arr[i] + "'");
                        }
                        Response.End();
                    }
                    if (item.ToLower() == "px" && (value.Length == 0 || value == "0")) value = "999999";
                    //DataBase.Execute("update a_eke_sysCheckUps set " + item + "='" + value + "' where ModelID='" + ModelID + "' and CheckupID='" + eParameters.QueryString("CheckupID") + "'");
                    DataBase.eTable("a_eke_sysCheckUps").Fields.Add(item, value == "NULL" ? null : value).Update("CheckupID='" + eParameters.QueryString("CheckupID") + "'");
                    eResult.Success("修改成功!"); 
                }
                #endregion
                #region  删除
                if (act == "delcheckup")
                {
                    DataBase.Execute("delete from a_eke_sysCheckUps where CheckupID='" + eParameters.QueryString("CheckupID") + "'");
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #endregion
                string modeltabid = eParameters.QueryString("modeltabid");
                #region 选项卡
                #region 添加
                if (act == "addmodeltab")
                {
                    //DataBase.Execute("insert into a_eke_sysModelTabs (ModelTabID,ModelID) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "')");
                    DataBase.eTable("a_eke_sysModelTabs").Fields.Add("ModelTabID", Guid.NewGuid().ToString()).Fields.Add("ModelID", ModelID).Add();
                    eResult.Success("添加成功!"); 
                }
                #endregion
                #region 修改
                if (act == "setmodeltab")
                {
                    //拖动排序
                    if (item.ToLower() == "setorders")
                    {
                        string ids = eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelTabs set px='" + value + "' where ModelID='" + ModelID + "' and ModelTabID='" + arr[i] + "'");
                        }
                        eResult.Success("修改成功!"); 
                    }

                    if (item.ToLower() == "px" && (value.Length == 0 || value == "0")) value = "999999";
                    //DataBase.Execute("update a_eke_sysModelTabs set " + item + "='" + value + "' where ModelID='" + ModelID + "' and ModelTabID='" + modeltabid + "'");
                    new eTable("a_eke_sysModelTabs").Fields.Add(item, value == "NULL" ? null : value).Where.Add("ModelTabID='" + modeltabid + "'").Update();
                    eResult.Success("修改成功!"); 
                }
                #endregion
                #region  删除
                if (act == "delmodeltab")
                {

                    DataBase.Execute("delete from a_eke_sysModelTabs where ModelTabID='" + modeltabid + "'");
                    DataBase.Execute("update a_eke_sysModelPanels set ModelTabID=NULL where ModelTabID='" + modeltabid + "'");
                    DataBase.Execute("update a_eke_sysModelItems set ModelTabID=NULL,ModelPanelID=NULL where ModelTabID='" + modeltabid + "'");
                    DataBase.Execute("update a_eke_sysModels set ModelTabID=NULL,ModelPanelID=NULL where ModelTabID='" + modeltabid + "'");
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #endregion
                string modelpaneid = eParameters.QueryString("modelpaneid");
                #region 面板
                #region 添加
                if (act == "addmodelgroup")
                {
                    //DataBase.Execute("insert into a_eke_sysModelPanels (ModelPanelID,ModelID) values ('" + Guid.NewGuid().ToString() + "','" + ModelID + "')");
                    DataBase.eTable("a_eke_sysModelPanels").Fields.Add("ModelPanelID", Guid.NewGuid().ToString()).Fields.Add("ModelID", ModelID).Add();
                    eResult.Success("添加成功!"); 
                }
                #endregion
                #region 修改
                if (act == "setmodelgroup")
                {
                    //拖动排序
                    if (item.ToLower() == "setorders")
                    {
                        string ids = eParameters.Form("ids");
                        string[] arr = ids.Split(",".ToCharArray());
                        for (int i = 0; i < arr.Length; i++)
                        {
                            value = (i + 1).ToString();
                            DataBase.Execute("update a_eke_sysModelPanels set px='" + value + "' where ModelID='" + ModelID + "' and ModelPanelID='" + arr[i] + "'");
                        }
                        eResult.Success("修改成功!"); 
                    }
                    if (item.ToLower() == "px" && (value.Length == 0 || value == "0")) value = "999999";
                    if (value == "NULL")
                    {
                        //DataBase.Execute("update a_eke_sysModelPanels set " + item + "=" + value + " where ModelPanelID='" + modelpaneid + "'");
                    }
                    else
                    {
                        //DataBase.Execute("update a_eke_sysModelPanels set " + item + "='" + value + "' where ModelPanelID='" + modelpaneid + "'");
                    }
                    new eTable("a_eke_sysModelPanels").Fields.Add(item, value == "NULL" ? null : value).Where.Add("ModelPanelID='" + modelpaneid + "'").Update();
                    eResult.Success("修改成功!"); 
                }
                #endregion
                #region  删除
                if (act == "delmodelgroup")
                {
                    DataBase.Execute("delete from a_eke_sysModelPanels where ModelPanelID='" + modelpaneid + "'");
                    DataBase.Execute("update a_eke_sysModelItems set ModelPanelID=NULL where ModelPanelID='" + modelpaneid + "'");
                    DataBase.Execute("update a_eke_sysModels set ModelPanelID=NULL where ModelPanelID='" + modelpaneid + "'");
                    eResult.Success("删除成功!"); 
                }
                #endregion
                #endregion
            }


            LitMenu.Text = getModelTree(ParentID,0);
            modelType = ModelInfo["Type"].ToString();
            AutoModel = eBase.parseBool(ModelInfo["Auto"]);
            SubModel = eBase.parseBool(ModelInfo["SubModel"]);

            //DataTable items = DataBase.getDataTable("select ModelItemID,MC,Code,CustomCode from a_eke_sysModelItems where delTag=0 and ModelID='" + ModelID + "' order by px");
            eTable tb = new eTable(TableName);
            sql = "select * from (";
            sql += "select ModelItemID,MC,Code,CustomCode,Num from a_eke_sysModelItems ";
            sql += " where delTag=0 and ModelID='" + ModelID + "' ";
            //sql += " and isnull(Code,'') not in ('delTime','delUser','delTag') and MC not in ('序号','操作') ";
            sql += " and isnull(Code,'') not in ('delTime','delUser','delTag') and isnull(MC,'') not in ('操作') ";
            sql += " union ";
            sql += " select b.ModelItemID,b.MC,b.Code,b.CustomCode,b.Num from a_eke_sysModels a ";
            sql += " inner join a_eke_sysModelItems b on a.ModelID=b.ModelID ";
            sql += " where a.JoinMore=0 and b.delTag=0 and a.ParentID ='" + ModelID + "' ";
            //sql += "and isnull( b.Code,'') not in ('" + tb.primaryKey + "','addTime','addUser','editTime','editUser','delTime','delUser','delTag') ";
            sql += "and isnull( b.Code,'') not in ('addTime','addUser','editTime','editUser','delTime','delUser','delTag') ";
            //sql += " and b.MC not in ('序号','操作') and b.primaryKey=0";
            sql += " and isnull(b.MC,'') not in ('序号','操作')";
            sql += ") as c order by c.Num";
            //eBase.WriteDiv(sql);
            DataTable items = eBase.DataBase.getDataTable(sql);
            for (int i = 0; i < items.Rows.Count; i++)
            {
                string code = items.Rows[i]["Code"].ToString();
                if (code.Length == 0) code = items.Rows[i]["CustomCode"].ToString();
                if (code.Length > 0)
                {
                    if (linkArrys.Length > 0) linkArrys += ",";
                    linkArrys += "'data," + code + "," + items.Rows[i]["MC"].ToString() + " (" + code + ")" + "'";
                }
            }
            //eBase.WriteDiv(linkArrys);

            items = eBase.DataBase.getDataTable("SELECT ModelID,MC FROM a_eke_sysModels where ParentID='" + ModelID + "' and show=1 and delTag=0 and JoinMore=1 and Auto=1");
            foreach (DataRow dr in items.Rows)
            {
                if (linkArrys.Length > 0) linkArrys += ",";
                linkArrys += "'model," + dr["ModelID"].ToString().ToLower() + ",模块：" + dr["MC"].ToString() + "'";
            }

            #region 功能选项卡
            StringBuilder title = new StringBuilder();
            StringBuilder body = new StringBuilder();
            #region 数据结构
            if (modelType != "4" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel)
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">数据结构</a>\n");// class=\"cur\"
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Columns.aspx?modelid=" + ModelID + (Request.QueryString["debug"]!= null ? "&debug=1" : "") + "\"><!--数据结构--></div>\n");
            }
            #endregion
            #region 基本设置
            title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">基本设置</a>\n");
            body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Basic.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--基本设置--></div>\n");
            #endregion
            #region 报表
            if (modelType == "4")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">报表</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Report.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--报表--></div>\n");
            }
            #endregion
            #region 客户端验证
            if (modelType != "3" && modelType != "4" && modelType != "5" && modelType != "7" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel)
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">客户端验证</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Client.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--客户端验证--></div>\n");
            }
            #endregion
            #region 列表
            if (AutoModel && modelType != "4" && modelType != "8" && modelType != "111" && modelType != "12")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">列表</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_List.aspx?modelid=" + ModelID + (Request.QueryString["debug"]!= null ? "&debug=1" : "") + "\"><!--列表--></div>\n");
            }
            #endregion
            #region 搜索
            //if (AutoModel && modelType != "4" && ((modelType == "1" && !SubModel) || (modelType == "3" && SubModel) || modelType == "5"))
            if (eBase.parseBool(ModelInfo["JoinMore"]) && AutoModel && modelType != "4a" && modelType != "7" && modelType != "8" && modelType != "11" && modelType != "12")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">搜索</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Search.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--搜索--></div>\n");
            }
            #endregion
            #region 数据
            if (AutoModel && modelType != "4" && modelType != "11" && modelType != "12")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">数据</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Data.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--数据--></div>\n");
            }
            #endregion
            #region 动作
            if (modelType == "8" || (modelType != "3" && modelType != "4" && modelType != "10000" && modelType != "11" && AutoModel && !SubModel)) 
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">动作</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Action.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--动作--></div>\n");
            }
            #endregion
            #region JS
            if (modelType == "3" || modelType == "8" || (modelType != "3333" && modelType != "4" && modelType != "555" && modelType != "111" && modelType != "12" && AutoModel && !SubModel))
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">JS</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_JS.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--JS--></div>\n");
            }
            #endregion
            #region 布局
            if (modelType == "8" || (eBase.parseBool(ModelInfo["JoinMore"]) && modelType != "3" && modelType != "4" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel))// && !SubModel
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">布局</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Layout.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--布局--></div>\n");
            }
            #endregion
            #region 生成代码
            if (modelType == "333")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">生成代码</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Bulid.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--生成代码--></div>\n");
            }
            #endregion
            #region 对应关系
            if (1 == 1 && modelType == "3" && AutoModel)//
            {
                string paddmode = DataBase.getValue("select AddMode from  a_eke_sysModels where ModelID='" + ModelInfo["ParentID"].ToString() + "'");
                if (paddmode == "1")//选择添加
                {
                    title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">对应关系</a>\n");
                    body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_FillData.aspx?act=sub&modelid=" + ModelID + "&parentid=" + ModelInfo["ParentID"].ToString() + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--对应关系--></div>\n");
                }
            }
            #endregion
            #region 导出
            if (modelType != "3" && modelType != "4" && modelType != "11" && modelType != "12" && AutoModel && !SubModel)
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">导出</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Export.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--导出--></div>\n");
            }
            #endregion
            #region 审批
            if (modelType != "3" && modelType != "4" && modelType != "5" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel && !SubModel)
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">审批</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_CheckUp.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--审批--></div>\n");
            }
            #endregion
            #region 打印
            if (eBase.parseBool(ModelInfo["JoinMore"]) && modelType != "3" && modelType != "4" && modelType != "51" && modelType != "8" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel)
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">打印</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Prints.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--打印--></div>\n");
            }
            if (1==3 && eBase.parseBool(ModelInfo["JoinMore"]) && modelType != "3" && modelType != "4" && modelType != "51" && modelType != "8" && modelType != "10" && modelType != "11" && modelType != "12" && AutoModel)
            {
                //title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">打印</a>\n");
                //body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_Prints.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--打印--></div>\n");
              
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">打印</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"\" loaded=\"true\">\n<!--打印-->\n");
                body.Append("<a href=\"http://frame.eketeam.com\" style=\"float:right;\" target=\"_blank\" title=\"eFrameWork开发框架\"><img src=\"images/help.gif\"></a>");
            
                if (eConfig.showHelp()) body.Append("<h1 class=\"tips\" style=\"margin-bottom:6px;\">打印</h1>\n");

                body.Append("<dl class=\"ePanel\">\n");
                body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"\" onfocus=\"this.blur();\"></a>默认打印</h1></dt>\n");
                body.Append("<dd style=\"padding:8px 8px 0px 8px;\">");

                body.Append("<dl class=\"ePanel\">\n");
                body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"cur\" onfocus=\"this.blur();\"></a>列表</h1></dt>\n");
                body.Append("<dd style=\"display:none;\">");
                #region 头部HTML
                body.Append("头部HTML：<br>");
                string ListPrintHTMLStart = ModelInfo["ListPrintHTMLStart"].ToString();
                if (ListPrintHTMLStart.Length == 0 && AutoModel && !SubModel)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<!DOCTYPE html>\n");
                    sb.Append("<html>\n");
                    sb.Append("<head>\n");
                    sb.Append("<title>打印</title>\n");
                    sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n");
                    sb.Append("<link href=\"../Plugins/eControls/default/style.css\" rel=\"stylesheet\" type=\"text/css\" />\n");
                    sb.Append("<style type=\"text/css\" media=\"print\">@media print {.btnprint{display:none;}}</style>\n");
                    sb.Append("</head>\n");
                    sb.Append("<body>\n");
                    ListPrintHTMLStart = sb.ToString();
                }
                body.Append("<textarea htmltag=\"true\" id=\"ListPrintHTMLStart\" name=\"ListPrintHTMLStart\" style=\"width:95%;height:100px;\" on_Blur=\"setModel(this,'ListPrintHTMLStart');\" oldvalue=\"" + HttpUtility.HtmlEncode(ListPrintHTMLStart) + "\">" + HttpUtility.HtmlEncode(ListPrintHTMLStart) + "</textarea><br>\n");
                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setModel(ListPrintHTMLStart,'ListPrintHTMLStart');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                #endregion
                #region 尾部HTML
                body.Append("尾部HTML：<br>");
                string ListPrintHTMLEnd = ModelInfo["ListPrintHTMLEnd"].ToString();
                if (ListPrintHTMLEnd.Length == 0 && AutoModel && !SubModel)
                {
                    ListPrintHTMLEnd = "</body>\n</html>";
                }
                body.Append("<textarea htmltag=\"true\" id=\"ListPrintHTMLEnd\" name=\"ListPrintHTMLEnd\" style=\"width:95%;height:100px;\"  on_Blur=\"setModel(this,'ListPrintHTMLEnd');\" oldvalue=\"" + HttpUtility.HtmlEncode(ListPrintHTMLEnd) + "\">" + HttpUtility.HtmlEncode(ListPrintHTMLEnd) + "</textarea><br>\n");
                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setModel(ListPrintHTMLEnd,'ListPrintHTMLEnd');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                #endregion
                body.Append("</dd>\n");
                body.Append("</dl>\n");


                body.Append("<dl class=\"ePanel\">\n");
                body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"cur\" onfocus=\"this.blur();\"></a>详细</h1></dt>\n");
                body.Append("<dd style=\"display:none;\">");
                #region 头部HTML
                body.Append("头部HTML：<br>");
                string printHTMLStart = ModelInfo["printHTMLStart"].ToString();
                if (printHTMLStart.Length == 0 && AutoModel && !SubModel)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<!DOCTYPE html>\n");
                    sb.Append("<html>\n");
                    sb.Append("<head>\n");
                    sb.Append("<title>打印</title>\n");
                    sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n");
                    sb.Append("<link href=\"../Plugins/eControls/default/style.css\" rel=\"stylesheet\" type=\"text/css\" />\n");
                    sb.Append("</head>\n");
                    sb.Append("<body>\n");
                    printHTMLStart = sb.ToString();
                }                
                body.Append("<textarea htmltag=\"true\" id=\"printHTMLStart\" name=\"printHTMLStart\" style=\"width:95%;height:100px;\"  on_Blur=\"setModel(this,'printHTMLStart');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTMLStart) + "\">" + HttpUtility.HtmlEncode(printHTMLStart) + "</textarea><br>\n");
                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setModel(printHTMLStart,'printHTMLStart');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                #endregion
                #region 循环HTML
                body.Append("循环HTML：<a href=\"javascript:;\" onclick=\"insertData('printHTML');\" style=\"color:#0066CC;\">引用数据</a><br>");
                //body.Append("引用数据：");
                //body.Append("<br>");
                string printHTML = ModelInfo["PrintHTML"].ToString();
                if (printHTML.Length == 0 && AutoModel && !SubModel)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table class=\"eDataView\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
                    sb.Append("<tbody>\n");
                    sb.Append("<tr>\n");
                    sb.Append("<td class=\"title\" width=\"120\">标题：</td>\n");
                    sb.Append("<td class=\"content\">内容</td>\n");
                    sb.Append("</tr>\n");
                    sb.Append("<tr>\n");
                    sb.Append("<td class=\"title\">标题：</td>\n");
                    sb.Append("<td class=\"content\">内容</td>\n");
                    sb.Append("</tr>\n");
                    sb.Append("</tbody>\n");
                    sb.Append("</table>\n");


                    sb.Append("<table class=\"eDataTable\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
                    sb.Append("<thead>\n");
                    sb.Append("<tr>\n");
                    sb.Append("<td width=\"80\">序号</td>\n");
                    sb.Append("<td>审核流程</td>\n");
                    sb.Append("</tr>\n");
                    sb.Append("</thead>\n");
                    sb.Append("<tbody>\n");
                    sb.Append("<tr>\n");
                    sb.Append("<td height=\"40\">1</td>\n");
                    sb.Append("<td title=\"部门确认\">部门确认</td>\n");
                    sb.Append("</tr>\n");
                    sb.Append("\n");
                    sb.Append("<tr class=\"alternating\" eclass=\"alternating\">\n");
                    sb.Append("<td height=\"40\">2</td>\n");
                    sb.Append("<td title=\"总经办确认\">总经办确认</td>\n");
                    sb.Append("</tr>\n");
                    sb.Append("</tbody>\n");
                    sb.Append("</table>\n");
                    printHTML = sb.ToString();
                }
                body.Append("<textarea htmltag=\"true\" id=\"printHTML\" name=\"printHTML\" style=\"width:95%;height:100px;\"  on_Blur=\"setModel(this,'printHTML');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTML) + "\">" + HttpUtility.HtmlEncode(printHTML) + "</textarea><br>\n");
                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setModel(printHTML,'printHTML');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                #endregion
                #region 生成循环HTML
                if (1 == 3)
                {
                    body.Append("生成循环HTML(复制到上一项保存)：<br>");
                    body.Append("<textarea htmltag=\"true\" name=\"printHTMLcreate\" class=\"input\" id=\"printHTMLcreate\" style=\"display:none;\">" + HttpUtility.HtmlEncode(printHTML) + "</textarea>\n");
                    body.Append("<script>\n");
                    //body.Append("var ControlJson = " + ControlJson.ToJson() + ";\n");
                    //body.Append("var linkArrys = [" + linkArrys + "];\n");
                    body.Append("KE.show({\n");
                    body.Append("id: 'printHTMLcreate',\n");
                    body.Append("width: '100%',\n");
                    body.Append("height: '400px',\n");
                    body.Append("newlineTag: 'br',\n");
                    //body.Append("cssPath: 'index.css',\n");
                    body.Append("items: ['source', '|', 'justifyleft', 'justifycenter', 'justifyright', 'justifyfull', '|', 'title', 'fontname', 'fontsize', '|', 'bold', 'italic', 'underline', '|', 'advtable', 'newekecontrol'],\n");
                    //body.Append("filterMode:false,\n");
                    body.Append("afterCreate: function (id) {\n");
                    body.Append("KE.event.ctrl(document, 13, function () {\n");
                    body.Append("KE.util.setData(id);\n");
                    //body.Append("document.forms['example'].submit();\n");
                    body.Append("});\n");
                    body.Append("KE.event.ctrl(KE.g[id].iframeDoc, 13, function () {\n");
                    body.Append("KE.util.setData(id);\n");
                    //body.Append("document.forms['example'].submit();\n");
                    body.Append("});\n");
                    body.Append("}\n");
                    body.Append("});\n");
                    body.Append("</script>\n");
                }
                #endregion
                #region 尾部HTML
                body.Append("尾部HTML：<br>");
                string printHTMLEnd = ModelInfo["printHTMLEnd"].ToString();
                if (printHTMLEnd.Length == 0 && AutoModel && !SubModel)
                {
                    printHTMLEnd = "</body>\n</html>";
                }
                body.Append("<textarea htmltag=\"true\" id=\"printHTMLEnd\" name=\"printHTMLEnd\" style=\"width:95%;height:100px;\"  on_Blur=\"setModel(this,'printHTMLEnd');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTMLEnd) + "\">" + HttpUtility.HtmlEncode(printHTMLEnd) + "</textarea><br>\n");
                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setModel(printHTMLEnd,'printHTMLEnd');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                #endregion
                body.Append("</dd>\n");
                body.Append("</dl>\n");


                body.Append("</dd>\n");
                body.Append("</dl>\n");

                body.Append("<input type=\"button\" name=\"Submit\" onclick=\"addprint(this);\" value=\" 增加打印格式 \" style=\"padding:3px 10px 3px 10px;margin-bottom:8px;\" /><br>\n");
               #region 其他打印
               DataTable dt = DataBase.getDataTable("select * from a_eke_sysModelPrints where ModelID='" + ModelID + "' and deltag=0 order by addTime");
               //body.Append(dt.Rows.Count.ToString());
               int idx = 1;
               foreach (DataRow dr in dt.Rows)
               {
                   body.Append("<dl class=\"ePanel\">\n");
                   body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"cur\" onfocus=\"this.blur();\"></a>" + dr["MC"].ToString() + "</h1></dt>\n");
                   body.Append("<dd style=\"display:none;padding:8px 8px 0px 8px;\">");
                   body.Append("名称：<input type=\"text\" value=\"" + dr["MC"].ToString() + "\" oldvalue=\"" + dr["MC"].ToString() + "\" class=\"edit\" style=\"width:90px;\" onBlur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','mc');\" />&nbsp;&nbsp;");
                   body.Append("编码：<input type=\"text\" value=\"" + dr["name"].ToString() + "\" oldvalue=\"" + dr["name"].ToString() + "\" class=\"edit\" style=\"width:90px;\" onBlur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','name');\" />&nbsp;&nbsp;");

                   body.Append("<dl class=\"ePanel\" style=\"margin-top:8px;\">\n");
                   body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"cur\" onfocus=\"this.blur();\"></a>列表</h1></dt>\n");
                   body.Append("<dd style=\"display:none;\">");
                   #region 头部HTML
                   body.Append("头部HTML：<br>");
                   ListPrintHTMLStart = dr["ListPrintHTMLStart"].ToString();
                   if (ListPrintHTMLStart.Length == 0 && AutoModel && !SubModel)
                   {
                       StringBuilder sb = new StringBuilder();
                       sb.Append("<!DOCTYPE html>\n");
                       sb.Append("<html>\n");
                       sb.Append("<head>\n");
                       sb.Append("<title>打印</title>\n");
                       sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n");
                       sb.Append("<link href=\"../Plugins/eControls/default/style.css\" rel=\"stylesheet\" type=\"text/css\" />\n");
                       sb.Append("<style type=\"text/css\" media=\"print\">@media print {.btnprint{display:none;}}</style>\n");
                       sb.Append("</head>\n");
                       sb.Append("<body>\n");
                       ListPrintHTMLStart = sb.ToString();
                   }
                   body.Append("<textarea htmltag=\"true\" style=\"width:95%;height:100px;\" on_Blur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','ListPrintHTMLStart');\" oldvalue=\"" + HttpUtility.HtmlEncode(ListPrintHTMLStart) + "\">" + HttpUtility.HtmlEncode(ListPrintHTMLStart) + "</textarea><br>\n");
                   body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setPrint2(this,'" + dr["ModelPrintID"].ToString() + "','ListPrintHTMLStart');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                   #endregion
                   #region 尾部HTML
                   body.Append("尾部HTML：<br>");
                   ListPrintHTMLEnd = dr["ListPrintHTMLEnd"].ToString();
                   if (ListPrintHTMLEnd.Length == 0 && AutoModel && !SubModel)
                   {
                       ListPrintHTMLEnd = "</body>\n</html>";
                   }
                   body.Append("<textarea htmltag=\"true\" style=\"width:95%;height:100px;\"  on_Blur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','ListPrintHTMLEnd');\" oldvalue=\"" + HttpUtility.HtmlEncode(ListPrintHTMLEnd) + "\">" + HttpUtility.HtmlEncode(ListPrintHTMLEnd) + "</textarea><br>\n");
                   body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setPrint2(this,'" + dr["ModelPrintID"].ToString() + "','ListPrintHTMLEnd');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                   #endregion
                   body.Append("</dd>\n");
                   body.Append("</dl>\n");


                   body.Append("<dl class=\"ePanel\">\n");
                   body.Append("<dt><h1 onclick=\"showPanel(this);\"><a href=\"javascript:;\" class=\"cur\" onfocus=\"this.blur();\"></a>详细</h1></dt>\n");
                   body.Append("<dd style=\"display:none;\">");
                   #region 头部HTML
                   body.Append("头部HTML：<br>");
                   printHTMLStart = dr["printHTMLStart"].ToString();
                   if (printHTMLStart.Length == 0 && AutoModel && !SubModel)
                   {
                       StringBuilder sb = new StringBuilder();
                       sb.Append("<!DOCTYPE html>\n");
                       sb.Append("<html>\n");
                       sb.Append("<head>\n");
                       sb.Append("<title>打印</title>\n");
                       sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n");
                       sb.Append("<link href=\"../Plugins/eControls/default/style.css\" rel=\"stylesheet\" type=\"text/css\" />\n");
                       sb.Append("</head>\n");
                       sb.Append("<body>\n");
                       printHTMLStart = sb.ToString();
                   }
                   body.Append("<textarea htmltag=\"true\" style=\"width:95%;height:100px;\"  on_Blur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','printHTMLStart');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTMLStart) + "\">" + HttpUtility.HtmlEncode(printHTMLStart) + "</textarea><br>\n");
                   body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setPrint2(this,'" + dr["ModelPrintID"].ToString() + "','printHTMLStart');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                   #endregion
                   #region 循环HTML
                   body.Append("循环HTML：<a href=\"javascript:;\" onclick=\"insertData('printHTML"+ idx.ToString() +"');\" style=\"color:#0066CC;\">引用数据</a><br>");
                   printHTML = dr["PrintHTML"].ToString();
                   if (printHTML.Length == 0 && AutoModel && !SubModel)
                   {
                       StringBuilder sb = new StringBuilder();
                       sb.Append("<table class=\"eDataView\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
                       sb.Append("<tbody>\n");
                       sb.Append("<tr>\n");
                       sb.Append("<td class=\"title\" width=\"120\">标题：</td>\n");
                       sb.Append("<td class=\"content\">内容</td>\n");
                       sb.Append("</tr>\n");
                       sb.Append("<tr>\n");
                       sb.Append("<td class=\"title\">标题：</td>\n");
                       sb.Append("<td class=\"content\">内容</td>\n");
                       sb.Append("</tr>\n");
                       sb.Append("</tbody>\n");
                       sb.Append("</table>\n");


                       sb.Append("<table class=\"eDataTable\" width=\"600\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
                       sb.Append("<thead>\n");
                       sb.Append("<tr>\n");
                       sb.Append("<td width=\"80\">序号</td>\n");
                       sb.Append("<td>审核流程</td>\n");
                       sb.Append("</tr>\n");
                       sb.Append("</thead>\n");
                       sb.Append("<tbody>\n");
                       sb.Append("<tr>\n");
                       sb.Append("<td height=\"40\">1</td>\n");
                       sb.Append("<td title=\"部门确认\">部门确认</td>\n");
                       sb.Append("</tr>\n");
                       sb.Append("\n");
                       sb.Append("<tr class=\"alternating\" eclass=\"alternating\">\n");
                       sb.Append("<td height=\"40\">2</td>\n");
                       sb.Append("<td title=\"总经办确认\">总经办确认</td>\n");
                       sb.Append("</tr>\n");
                       sb.Append("</tbody>\n");
                       sb.Append("</table>\n");
                       printHTML = sb.ToString();
                   }
                   body.Append("<textarea id=\"printHTML" + idx.ToString() + "\" htmltag=\"true\" style=\"width:95%;height:100px;\"  on_Blur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','printHTML');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTML) + "\">" + HttpUtility.HtmlEncode(printHTML) + "</textarea><br>\n");
                   body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setPrint2(this,'" + dr["ModelPrintID"].ToString() + "','printHTML');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                   #endregion
                   #region 尾部HTML
                   body.Append("尾部HTML：<br>");
                   printHTMLEnd = dr["printHTMLEnd"].ToString();
                   if (printHTMLEnd.Length == 0 && AutoModel && !SubModel)
                   {
                       printHTMLEnd = "</body>\n</html>";
                   }
                   body.Append("<textarea htmltag=\"true\"  style=\"width:95%;height:100px;\"  on_Blur=\"setPrint(this,'" + dr["ModelPrintID"].ToString() + "','printHTMLEnd');\" oldvalue=\"" + HttpUtility.HtmlEncode(printHTMLEnd) + "\">" + HttpUtility.HtmlEncode(printHTMLEnd) + "</textarea><br>\n");
                   body.Append("<input type=\"button\" name=\"Submit\" onclick=\"setPrint2(this,'" + dr["ModelPrintID"].ToString() + "','printHTMLEnd');\" value=\" 保  存 \" style=\"padding:3px 10px 3px 10px;\" /><br>\n");
                   #endregion
                   body.Append("</dd>\n");
                   body.Append("</dl>\n");



                   body.Append("</dd>\n");
                   body.Append("</dl>\n");
                   idx++;
               }
               #endregion

               body.Append("</div>\n");

            }
            #endregion
            #region WebAPI
            if (AutoModel && eBase.parseBool(ModelInfo["JoinMore"]) && modelType != "3" && modelType != "4" && modelType != "8" && modelType != "10" && modelType != "11" && modelType != "12")
            {
                title.Append("<a href=\"javascript:;\" onclick=\"selecttab(this);\" onfocus=\"this.blur();\">WebAPI</a>\n");
                body.Append("<div style=\"height:100%;display:none;\" dataurl=\"ModelItems_WebAPI.aspx?modelid=" + ModelID + (Request.QueryString["debug"] != null ? "&debug=1" : "") + "\"><!--WebAPI--></div>\n");
            }
            #endregion
            titles = title.ToString();
            bodys = body.ToString();
            #endregion
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Master == null) return;
            Literal lit = (Literal)Master.FindControl("LitTitle");
            if (lit != null)
            {
                lit.Text =ModelInfo["MC"].ToString() + " - 模块管理 - " + eConfig.manageName(); 
            }
        }
    }
}