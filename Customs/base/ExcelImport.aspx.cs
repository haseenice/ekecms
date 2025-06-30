using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data.OleDb;
using EKETEAM.FrameWork;
using EKETEAM.Data;
using LitJson;

namespace eFrameWork.Customs.Base
{
    public partial class ExcelImport : System.Web.UI.Page
    {
        public string ModelID = eParameters.QueryString("ModelID");
        public string AppItem = eParameters.QueryString("AppItem");
        public string UserArea = eParameters.QueryString("area");
        eUser user;
        public eModel model;
        public string file = eParameters.QueryString("file");
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
                    if (model.ModelInfo["DataSourceID"].ToString().Length > 0)
                    {
                        _database = new eDataBase(model.ModelInfo);
                    }
                    else
                    {
                        _database = eConfig.DefaultDataBase;
                    }
                }
                return _database;
            }
        }


        private string tempPath = "";
        private string tempUrl = "";
        public eExtensionsList allowExts = new eExtensionsList(".xls.xlsx.csv");
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser(UserArea);
            user.Check();

            tempPath = eRunTime.tempPath;
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            tempUrl = tempPath.toVirtualUrl();


            
           // eModelInfo customModel = new eModelInfo(user);
            //model = customModel.Model;
            model = new eModel();

            //eBase.PrintDataRow(model.Power);
            if (!model.Power["import"])
            {
                eBase.Writeln("没有导入权限!");
                eBase.End();
            }

            if (file.Length == 0)
            {
                #region 保存文件
                if (Request.Files.Count==1)
                {
                    HttpPostedFile postfile = Request.Files[0];
                    if (postfile.ContentLength > 0)
                    {
                        string ext = postfile.FileName.fileExtension();
                        string filename = eBase.GetFileName() + ext;
                        string pathname = tempPath + filename;
                        postfile.safeSaveAs(pathname);
                        Response.Redirect(eBase.getAspxFileName() +  "?area=" + UserArea + (AppItem.Length > 0 ? "&AppItem=" + AppItem : "") + (ModelID.Length > 0 ? "&ModelID=" + ModelID : "") + "&file=" + filename, true);

                    }
                }
                #endregion

            }
            else
            {
                if(Request.Form.Count==0)
                {
                    ReadFile();
                }
                else
                {
                    Import();
                }
            }
        }
        private void Import()
        {
            string pathname = tempPath + file;
            if (!File.Exists(pathname)) return;                    
            #region 尝试打开上传文件
            eExcel excel = new eExcel(pathname);
            if (!excel.isOpen == null)
            {
                System.IO.File.Delete(pathname);
                Response.Write("文件格式有误!");
                Response.End();
            }
            #endregion
            DataTable col = DataBase.getSchemaColumns(model.ModelInfo["code"].ToString());
            DataRow[] rows = col.Select("COLUMN_NAME='siteid'");
            DataRow[] _rows = col.Select("COLUMN_NAME='code'");
            DataRow[] sinrs = model.Items.Select("single=1");
            #region 循环导入
            int midx = 1;
            //成功，失败，重复
            int allCount=0;
            int successCount = 0;
            int repeatCount = 0; //重复
            int errCount = 0; //数据不合格
            foreach (string table in excel.Tables)
            {
                if (Request.Form["model" + midx.ToString()] != null)
                {
                    DataTable tb = excel[table];
                    if (tb.Rows.Count > 0)
                    {
                        allCount += tb.Rows.Count;
                        #region 批量添加
                        if (1 == 2)
                        {
                            #region 列信息
                            string cols = "";
                            for (int i = 0; i < tb.Columns.Count; i++)
                            {
                                string code = eParameters.Form("fa" + midx.ToString() + "_" + i.ToString());
                                string code1 = eParameters.Form("fb" + midx.ToString() + "_" + i.ToString());
                                if (code.Length > 0 && code1.Length > 0)
                                {
                                    if (cols.Length > 0) cols += ",";
                                    cols += code1;
                                    //eBase.Writeln(code + "::" + code1);
                                }
                            }
                            DataRow[] _rs = col.Select("COLUMN_NAME='siteid'");
                            if (user["siteid"].Length > 0 && _rs.Length > 0) cols += ",siteid";
                            _rs = col.Select("COLUMN_NAME='orgcode'");
                            if (user["orgcode"].Length > 0 && _rs.Length > 0) cols += ",orgcode";
                            _rs = col.Select("COLUMN_NAME='postlevel'");
                            if (user["postlevel"].Length > 0 && _rs.Length > 0) cols += ",postlevel";
                            _rs = col.Select("COLUMN_NAME='dataflags'");
                            if (user["dataflags"].Length > 0 && _rs.Length > 0) cols += ",dataflags";

                            _rs = col.Select("COLUMN_NAME='adduser'");
                            if (_rs.Length > 0) cols += ",adduser";
                            #endregion

                            DataTable _dt = DataBase.getDataTable("select " + cols + " from " + model.ModelInfo["code"].ToString() + " where 1=2");
                            _dt.TableName = model.ModelInfo["code"].ToString();
                            foreach (DataRow dr in tb.Rows)
                            {
                                //if (dr[0].ToString().Length == 0 && dr[1].ToString().Length == 0) continue;//空数据跳出
                                //DataRow row = _dt.Rows.Add();
                                DataRow row = _dt.NewRow();
                                bool hasnull = false;
                                #region 系统字段处理
                                if (_dt.Columns.Contains("siteid") && user.ContainsKey("siteid")) row["siteid"] = user["siteid"];
                                if (_dt.Columns.Contains("orgcode") && user.ContainsKey("orgcode")) row["orgcode"] = user["orgcode"];
                                if (_dt.Columns.Contains("usercode") && user.ContainsKey("usercode")) row["usercode"] = user["usercode"];
                                if (_dt.Columns.Contains("postlevel") && user.ContainsKey("postlevel")) row["postlevel"] = user["postlevel"];
                                if (_dt.Columns.Contains("dataflags") && user.ContainsKey("dataflags")) row["dataflags"] = user["dataflags"];
                                if (_dt.Columns.Contains("adduser")) row["adduser"] = user.ID;
                                #endregion
                                for (int i = 0; i < tb.Columns.Count && !hasnull; i++)
                                {
                                    #region 循环列
                                    string code = eParameters.Form("fa" + midx.ToString() + "_" + i.ToString());
                                    string code1 = eParameters.Form("fb" + midx.ToString() + "_" + i.ToString());
                                    if (code.Length > 0 && code1.Length > 0)
                                    {
                                        DataRow[] rs = model.Items.Select("code='" + code1 + "'");
                                        if (rs.Length > 0)
                                        {
                                            string value = dr[code].ToString().Trim();
                                            if (rs[0]["notNull"].ToString().ToLower().Replace("1", "true") == "true" && value.Length == 0)
                                            {
                                                hasnull = true;
                                                continue;
                                            }
                                            //eBase.Writeln(rs[0]["notNull"].ToString() + "::" + value);
                                            string options = rs[0]["options"].ToString();
                                            if (options.Length > 2 && options.StartsWith("["))
                                            {
                                                //eBase.Writeln(options);                         
                                                JsonData json = options.ToJsonData();
                                                foreach (JsonData m in json)
                                                {
                                                    value = value.Replace(eBase.RemoveHTML(m.getValue("text")), m.getValue("value"));
                                                }
                                                if (rs[0]["type"].ToString().ToLower() == "bit")
                                                {
                                                    //value = value.ToLower().Replace("true", "1").Replace("false", "0");
                                                    value = value.ToLower().Replace("1", "true").Replace("0", "false");
                                                }
                                            }
                                            if (rs[0]["BindObject"].ToString().ToLower() == "dictionaries")
                                            {
                                                string tmp = eBase.DataBase.getValue("select DictionarieID from Dictionaries where mc='" + value + "'");
                                                if (tmp.Length > 0) value = tmp;
                                            }
                                            if (value.Length > 0)
                                            {
                                                try
                                                {
                                                    string _type = _dt.Columns[code1].DataType.ToString().ToLower().Replace("system.", "");
                                                    if (_type == "boolean")
                                                    {
                                                        row[code1] = eBase.parseBool(value);
                                                    }
                                                    else
                                                    {
                                                        row[code1] = value;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    //eBase.Writeln(ex.Message + "::" + value);
                                                }
                                            }
                                            //eBase.Writeln(code1 + "::" + value);
                                        }
                                    }
                                    #endregion
                                }
                                #region 唯一性验证
                                bool has = false;
                                if (sinrs.Length > 0)
                                {
                                    //model.ModelInfo["SingleType"].ToString();
                                    eList list = new eList(model.ModelInfo["code"].ToString());
                                    list.Fields.Add("count(1)");
                                    if (model.ModelInfo["SingleType"].ToString() == "0")//一起验证
                                    {
                                        foreach (DataRow drr in sinrs)
                                        {
                                            string code = drr["code"].ToString();
                                            if (row.Contains(code)) list.Where.Add(code, "=", row[code].ToString());
                                        }
                                        string ct = DataBase.getValue("select count(1) from " + model.ModelInfo["code"].ToString() + list.Where.getCondition());
                                        if (ct != "0") has = true;
                                    }
                                    else //逐个验证
                                    {
                                        foreach (DataRow drr in sinrs)
                                        {
                                            string code = drr["code"].ToString();
                                            if (row.Contains(code))
                                            {
                                                list.Where.Add(code, "=", row[code].ToString());
                                                string ct = DataBase.getValue("select count(1) from " + model.ModelInfo["code"].ToString() + list.Where.getCondition());
                                                if (ct != "0")
                                                {
                                                    has = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    //eBase.Writeln( "::" + list.Where.getCondition() + "::" + ct);
                                    //eBase.PrintDataTable(list.getDataTable());
                                }
                                #endregion
                                if (!hasnull && !has) _dt.Rows.Add(row);
                                if (has) repeatCount++;
                                //eBase.Writeln(has.ToString());
                            }
                            successCount += _dt.Rows.Count;

                            //eBase.Writeln(allCount.ToString() + "::" + successCount.ToString());
                            // eBase.AppendLog(_dt.toJSONData().ToJson());
                            //eBase.PrintDataTable(_dt);
                            if (_dt.Rows.Count > 0) _dt.Append(DataBase);

                            //eBase.Writeln(cols + "::" );
                            //eBase.PrintDataTable(model.Items);
                            //eBase.Print(_dt);
                            //Oleconn.Close();
                            //eBase.End();

                        }
                        #endregion

                        #region 单条添加
                        if (1 == 1)
                        {                           
                            foreach (DataRow dr in tb.Rows)
                            {
                                bool isnull = false;
                                //eBase.PrintDataRow(dr);
                                eTable etb = new eTable(model.ModelInfo["code"].ToString(), user);
                                etb.DataBase = DataBase;
                                //if (rows.Length > 0) etb.Fields.Add("SiteID", siteid);//eTable会添加
                                //if (_rows.Length > 0) etb.Fields.Add("code", user["code"]);
                                string rowvalue = "";
                                for (int i = 0; i < tb.Columns.Count; i++)
                                {
                                    string code = eParameters.Form("fa" + midx.ToString() + "_" + i.ToString());
                                    string code1 = eParameters.Form("fb" + midx.ToString() + "_" + i.ToString());

                                    //eBase.Writeln(code + "::" + code1);
                                    
                                    if (code.Length > 0 && code1.Length > 0)
                                    {
                                        DataRow[] rs = model.Items.Select("code='" + code1 + "'");
                                        if (rs.Length > 0)
                                        {
                                            string value = dr[code].ToString().Trim().Replace("，", ",");
                                            rowvalue += value;
                                            string options = rs[0]["options"].ToString();
                                            if (options.Length > 2 && options.StartsWith("["))
                                            {
                                                JsonData json = options.ToJsonData();
                                                foreach (JsonData m in json)
                                                {
                                                    value = value.Replace(m.getValue("text").removeHTML(), m.getValue("value"));
                                                }
                                                if (rs[0]["type"].ToString().ToLower() == "bit")
                                                {
                                                    value = value.ToLower().Replace("true", "1").Replace("false", "0");
                                                }
                                            }
                                            else
                                            {
                                                string BindObject = rs[0]["BindObject"].ToString();
                                                string BindText = rs[0]["BindText"].ToString();
                                                string BindValue = rs[0]["BindValue"].ToString();
                                                string BindCondition = rs[0]["BindCondition"].ToString();
                                                if (value.Length > 0 && BindObject.Length > 0 && BindText.Length > 0 && BindValue.Length > 0 && BindText.ToLower() != BindValue.ToLower())
                                                {
                                                    string[] arr = value.Split(",".ToCharArray());
                                                    for(int k=0;k<arr.Length;k++)
                                                    {
                                                        string temp = eBase.DataBase.getValue("Select " + BindValue + " from " + BindObject + " where " + BindText + "='" + arr[k] + "'" + (BindCondition.Length > 2 ? " and " + BindCondition : ""));
                                                        if (temp.Length > 0) arr[k] = temp;
                                                    }
                                                    value = string.Join(",", arr);
                                                }
                                            }
                                            if (value.Length == 0 && rs[0]["notnull"].ToString().ToLower() == "true")
                                            {
                                                errCount++;
                                                isnull = true;
                                            }
                                            etb.Fields.Add(code1, value);
                                        }
                                    }
                                }
                                if (isnull) { continue; }
                                if (rowvalue.Length == 0) //空行判断
                                {
                                    break;
                                }
                                #region 唯一性验证
                                bool has = false;
                                if (sinrs.Length > 0)
                                {
                                    eList list = new eList(model.ModelInfo["code"].ToString());
                                    list.Fields.Add("count(1)");
                                    //eBase.Writeln(sinrs.Length.ToString());
                                    if (model.ModelInfo["SingleType"].ToString() == "0")//一起验证
                                    {
                                        foreach (DataRow drr in sinrs)
                                        {
                                            string code = drr["code"].ToString();
                                            if (etb.Fields.ContainsKey(code)) list.Where.Add(code, "=", etb.Fields[code].ToString());
                                            //eBase.Writeln(code + "::" + etb.Fields.ContainsKey(code).ToString() + "::" + etb.Fields[code].ToString() );
                                        }
                                        if (list.ColumnCollection.ContainsKey("siteid") && user["siteid"].ToString().Length > 0)
                                        {
                                            list.Where.Add("SiteID", "=", user["siteid"].ToString());
                                        }
                                        string ct = DataBase.getValue("select count(1) from " + model.ModelInfo["code"].ToString() + list.Where.getCondition());
                                        if (ct != "0") has = true;
                                    }
                                    else //逐个验证
                                    {
                                        foreach (DataRow drr in sinrs)
                                        {
                                            string code = drr["code"].ToString();
                                            if (etb.Fields.ContainsKey(code))
                                            {
                                                list.Where.Add(code, "=", etb.Fields[code].ToString());
                                                if (list.ColumnCollection.ContainsKey("siteid") && user["siteid"].ToString().Length > 0)
                                                {
                                                    list.Where.Add("SiteID", "=", user["siteid"].ToString());
                                                }
                                                string ct = DataBase.getValue("select count(1) from " + model.ModelInfo["code"].ToString() + list.Where.getCondition());
                                                if (ct != "0")
                                                {
                                                    has = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                if (has)
                                {
                                    repeatCount++;
                                }
                                else
                                {
                                    bool result= etb.Add();
                                    string sql = model.ModelInfo["addsql"].ToString();
                                    if (sql.Length > 0)
                                    {
                                         model.ExecuteActionSQL(sql, null, etb.ID);
                                    }
                                    model.ExecuteAction("insert", null, etb.ID);
                                    if (result)
                                    {
                                        successCount++;
                                    }
                                    else
                                    {
                                        errCount++;
                                    }
                                }
                                //eBase.Writeln( etb.addSQL + "<br>");                      
                                //eBase.Writeln(etb.ID + "<br>");
                            }
                        }
                        #endregion
                    }
                }
                midx++;
            }


           
            #endregion
            eBase.Writeln("成功:" + successCount.ToString());
            eBase.Writeln("失败:" + (allCount - successCount).ToString() + " (重复：" + repeatCount.ToString() + " , 数据不合格：" + errCount.ToString() + ")");
            excel.Close();
            excel.Remove();
        }
        private void ReadFile()
        {
            string pathname = tempPath + file;
            if (!File.Exists(pathname)) return;
            eFileInfo fi = new eFileInfo(pathname);
            #region 尝试打开上传文件
            eExcel excel = new eExcel(pathname);
            if (!excel.isOpen == null)
            {
                System.IO.File.Delete(pathname);
                Response.Write("文件格式有误!");
                Response.End();
            }
            tempUrl += file;
            Response.Write("<script>parent.uploadTempFile=\"" + tempUrl + "\";</script>");
            #endregion



            #region 循环展示
            StringBuilder sb = new StringBuilder();
            sb.Append("<form id=\"form1\" name=\"form1\" method=\"post\" action=\"" + eBase.getAspxFileName() + "?area=" + UserArea + "&act=import" + (AppItem.Length > 0 ? "&AppItem=" + AppItem : "") + (ModelID.Length > 0 ? "&ModelID=" + ModelID : "") + "&file=" + file + "\">\r\n");
            int midx = 1;
            DataRow[] addItems = model.Items.Select("showAdd=1 and hasUI=1 and (len(code)>0 or len(ProgrameFile)>0) and len(isnull(addControlType,''))>0 and controlType<>'hidden'", "AddOrder, PX, addTime");
            foreach (string table in excel.Tables)
            {
                DataTable tb = excel.Select(table, 1);
                if (tb.Rows.Count > 0)
                {
                    if (tb.Rows.Count == 1 &&  tb.Columns.Count == 1 && tb.Columns[0].ColumnName.ToLower() == "f1") continue;//空表格
                    sb.Append("<label><input type=\"checkbox\" name=\"model" + midx.ToString() + "\" checked=\"checked\" onclick=\"show(this,'" + midx.ToString() + "');\" value=\"1\">" + table + "</label>");
                    sb.Append(eBase.getTableHTML(tb));
                    sb.Append("<div class=\"box" + midx.ToString() + "\" style=\"di3splay:none;\">");
                    sb.Append("<div class=\"itemtitle\">对应关系</div>");
                    #region 对应关系
                    sb.Append("<table width=\"600\" border=\"0\" cellpadding=\"0\" cellspacing=\"1\" bgcolor=\"#DDDDDD\" class=\"eDataTable\" style=\"margin-bottom:10px;\">\r\n");
                    sb.Append("<thead>\r\n");
                    sb.Append("<tr bgcolor=\"#F9F9F9\">\r\n");
                    sb.Append("<td height2=\"30\">Excel</td>\r\n");
                    sb.Append("<td>" + model.ModelInfo["mc"].ToString() + "</td>\r\n");
                    sb.Append("</tr>\r\n");
                    sb.Append("</thead>\r\n");
                    sb.Append("<tbody>\r\n");
                    for (int i = 0; i < tb.Columns.Count; i++)
                    {
                        string style = " style=\"color:#ff0000;\"";
                        for (int j = 0; j < addItems.Length; j++)
                        {
                            if (tb.Columns[i].ColumnName == addItems[j]["code"].ToString() || tb.Columns[i].ColumnName == addItems[j]["mc"].ToString())
                            {
                                style = "";
                            }
                        }
                        sb.Append("<tr bgcolor=\"#FFFFFF\">\r\n");
                        sb.Append("<td height=\"30\">" );
                        //Excel列
                        sb.Append("<select name=\"fa" + midx.ToString() + "_" + i.ToString() + "\"" + style + ">\r\n");
                        sb.Append("<option value=\"\">忽略</option>\r\n");
                        for (int j = 0; j < tb.Columns.Count; j++)
                        {
                            sb.Append("<option value=\"" + tb.Columns[j].ColumnName + "\"" + (tb.Columns[i].ColumnName == tb.Columns[j].ColumnName ? " selected=\"true\"" : "") + ">" + tb.Columns[j].ColumnName + "</option>\r\n");
                        }
                        sb.Append("</select>");
                        sb.Append("</td>\r\n");
                        sb.Append("<td>");
                        //模块列
                        sb.Append("<select name=\"fb" + midx.ToString() + "_" + i.ToString() + "\"" + style + ">\r\n");
                        sb.Append("<option value=\"\">忽略</option>\r\n");
                        for (int j = 0; j < addItems.Length; j++)
                        {
                            sb.Append("<option value=\"" + addItems[j]["code"].ToString() + "\"" + (tb.Columns[i].ColumnName == addItems[j]["code"].ToString() || tb.Columns[i].ColumnName == addItems[j]["mc"].ToString() ? " selected=\"true\"" : "") + ">" + addItems[j]["mc"].ToString() + "</option>\r\n");
                        }
                        sb.Append("</select>");
                        sb.Append("</td>\r\n");
                        sb.Append("</tr>\r\n");
                    }
                    sb.Append("</tbody>\r\n");
                    sb.Append("</table>\r\n");
                    #endregion                  
                    sb.Append("</div>");
                }
                midx++;         
            }
            sb.Append("<p style=\"margin-top:10px;margin-bottom:15px;\"><a class=\"btn\" href=\"javascript:;\" onclick=\"submitform();\" _click=\"form1.submit();\">确定导入</a></p>");
            sb.Append("<div style=\"height:20px;\"></div>\r\n");   
            sb.Append("</form>\r\n");   
            litBody.Text = sb.ToString();
            #endregion
            excel.Close();
        }
    }
}