using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using System.Xml;
using System.Xml.Serialization;

public partial class Manage_ModelCopy : System.Web.UI.Page
{
    private string ModelID = eParameters.QueryString("modelid");
    private string Single = eParameters.QueryString("single");    
    public eUser user;
    private eDataBase DataBase
    {
        get
        {
            return eConfig.DefaultDataBase;
        }
    }
    private string _parentid;
    private string getpid(string modelid)
    {
        string parentid = DataBase.getValue("select ParentID FROM a_eke_sysModels where ModelID='" + modelid + "'");
        if (parentid.Length == 0 || parentid == "0") //1级
        {
            return modelid;
        }
        else
        {
            return getpid(parentid);
        }
    }
    public string ParentID
    {
        get
        {
            if (_parentid==null)
            {
                _parentid = getpid(ModelID);
            }
            return _parentid;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        user = new eUser("Manage");
        user.Check();
        string act = eParameters.Request("act");
        if (act.Length == 0)
        {
            if (Single.Length > 0)
            {
                litBody.Text = getModelTree(ModelID, 0);
            }
            else
            {
                litBody.Text = getModelTree(ParentID, 0);
            }
        }
        else
        {
            Save();
        }
    }
    private void Save()
    {
       // saveModel(ParentID);
        CopyModel(ModelID);
        litBody.Text = "复制完成!";
        eBase.clearDataCache(); //清除所有缓存
    }
    private void saveModel(string pid)
    {
        DataTable dt = DataBase.getDataTable("select ModelID,MC,Code from a_eke_sysModels where ModelID='" + pid + "' and auto=1");
        if (dt.Rows.Count == 0) return;
        
        //eBase.Writeln(dt.Rows[0]["modelid"].ToString());
        DataTable tb = DataBase.getDataTable("select ModelID,MC from a_eke_sysModels where ParentID='" + pid + "' and delTag=0 order by JoinMore");// and Type in (1,3,6,7,8,12)
        for (int i = 0; i < tb.Rows.Count; i++)
        {
            saveModel(tb.Rows[i]["ModelID"].ToString());
        }
    }
    public void getids(ArrayList IDS, string pid)
    {
        if(!IDS.Contains(pid.ToLower())) IDS.Add(pid.ToLower());
        DataTable tb = DataBase.getDataTable("select ModelID,MC from a_eke_sysModels where ParentID='" + pid + "' and delTag=0 order by JoinMore");// and Type in (1,3,6,7,8,12)
        foreach (DataRow dr in tb.Rows)
        {
            getids(IDS,dr["ModelID"].ToString());
        }
    }
    private void CopyModel(string modelid)
    {
        string sql = "";
        XmlDocument doc = new XmlDocument();
       
        //eBase.Writeln("'" + string.Join("','", IDS.ToArray()) + "'");
        //eBase.End();
        #region  生成关系
        eList a_eke_sysModels = new eList("a_eke_sysModels");
        a_eke_sysModels.Where.Add("deltag=0");
        if (Single.Length > 0)
        {
            a_eke_sysModels.Where.Add("modelid='" + modelid + "'");
        }
        else
        {
            ArrayList IDS = new ArrayList();
            getids(IDS, this.ModelID);
            a_eke_sysModels.Where.Add("modelid in ('" + string.Join("','", IDS.ToArray()) + "')");
        }
        //a_eke_sysModels.Where.Add("modelid in ('7254820c-df55-4655-be0a-f14527556493','bf9da797-4b14-488f-9297-b7599edfe5cc')"); //'5ac4ba59-a545-467e-997d-7721802a403b','C6399A56-C165-4610-AE02-F593DBE8226A'

        

        eList a_eke_sysActions = new eList("a_eke_sysActions");
        a_eke_sysActions.Where.Add("deltag=0");
        a_eke_sysModels.Add(a_eke_sysActions);

        eList a_eke_sysCheckUps = new eList("a_eke_sysCheckUps");
        a_eke_sysCheckUps.Where.Add("deltag=0");
        a_eke_sysModels.Add(a_eke_sysCheckUps);

        eList a_eke_sysConditions = new eList("a_eke_sysConditions");
        a_eke_sysConditions.Where.Add("deltag=0");
        a_eke_sysModels.Add(a_eke_sysConditions);

        eList a_eke_sysModelConditions = new eList("a_eke_sysModelConditions");
        a_eke_sysModelConditions.Where.Add("deltag=0");
        eList a_eke_sysModelConditionItems = new eList("a_eke_sysModelConditionItems");
        a_eke_sysModelConditionItems.Where.Add("deltag=0");
        a_eke_sysModelConditions.Add(a_eke_sysModelConditionItems);
        a_eke_sysModels.Add(a_eke_sysModelConditions);


        eList a_eke_sysModelItems = new eList("a_eke_sysModelItems");
        a_eke_sysModelItems.Where.Add("deltag=0");
        //a_eke_sysModelItems.Rows = 1;
        a_eke_sysModels.Add(a_eke_sysModelItems);

        eList a_eke_sysModelTabs = new eList("a_eke_sysModelTabs");
        a_eke_sysModelTabs.Where.Add("deltag=0");
        a_eke_sysModels.Add(a_eke_sysModelTabs);

        eList a_eke_sysModelPanels = new eList("a_eke_sysModelPanels");
        a_eke_sysModelPanels.Where.Add("deltag=0");
        a_eke_sysModels.Add(a_eke_sysModelPanels);




        eList a_eke_sysReports = new eList("a_eke_sysReports");
        a_eke_sysReports.Where.Add("deltag=0");
        eList a_eke_sysReportItems = new eList("a_eke_sysReportItems");
        a_eke_sysReportItems.Where.Add("deltag=0");
        //a_eke_sysReportItems.Rows = 5;
        a_eke_sysReports.Add(a_eke_sysReportItems);
        a_eke_sysModels.Add(a_eke_sysReports);
        #endregion
        #region 获取数据
        //eBase.Writeln("a_eke_sysModels");
        DataTable Models = a_eke_sysModels.getDataTable();
        //Models.Columns.Remove("addTime");
        //eBase.PrintDataTable(Models);     


        //eBase.Writeln("a_eke_sysActions");
        DataTable Actions = a_eke_sysActions.getDataTable();

        //eBase.PrintDataTable(Actions);

        // eBase.Writeln("a_eke_sysCheckUps");
        DataTable CheckUps = a_eke_sysCheckUps.getDataTable();

        //eBase.PrintDataTable(CheckUps);



        //eBase.Writeln("a_eke_sysModelConditions");
        DataTable ModelConditions = a_eke_sysModelConditions.getDataTable();

        //eBase.PrintDataTable(ModelConditions);

        //eBase.Writeln("a_eke_sysModelConditionItems");
        DataTable ModelConditionItems = a_eke_sysModelConditionItems.getDataTable();

        //eBase.PrintDataTable(ModelConditionItems);

        //eBase.Writeln("a_eke_sysModelItems");
        DataTable ModelItems = a_eke_sysModelItems.getDataTable();
        //eBase.PrintDataTable(ModelItems);


        //eBase.Writeln("a_eke_sysModelTabs");
        DataTable ModelTabs = a_eke_sysModelTabs.getDataTable();
        //eBase.PrintDataTable(ModelTabs);

        //eBase.Writeln("a_eke_sysModelPanels");
        DataTable ModelPanels = a_eke_sysModelPanels.getDataTable();
        //eBase.PrintDataTable(ModelPanels);


        //eBase.Writeln("a_eke_sysReports");
        DataTable Reports = a_eke_sysReports.getDataTable();
        //eBase.PrintDataTable(Reports);

        //eBase.Writeln("a_eke_sysReportItems");
        DataTable ReportItems = a_eke_sysReportItems.getDataTable();
        //eBase.PrintDataTable(ReportItems);
        #endregion
        #region 数据处理
        DataRow[] rows;
        foreach (DataRow dr in Models.Rows)
        {
            string ModelID = dr["ModelID"].ToString();
            if (Models.Columns.Contains("addtime"))
            {
                dr["addtime"] = DateTime.Now;
            }
            string _name = ModelID.Split("-".ToCharArray())[0];
            dr["mc"] = eParameters.Form("mc_" + _name );
            string code = dr["Code"].ToString();
            string newcode = eParameters.Form("code_" + _name);
            if (code.Length > 1 && code.ToLower() != newcode.ToLower())
            {

                sql = eBase.DataBase.getTableSql(code);
                sql = System.Text.RegularExpressions.Regex.Replace(sql, "\\[" + code + "\\]", "[" + newcode + "]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                sql = System.Text.RegularExpressions.Regex.Replace(sql, "'" + code + "'", "'" + newcode + "'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //eBase.DataBase.Execute(sql);
                //eBase.Writeln(sql);
                DataTable dt = DataBase.getSchemaColumns(code);
                dt.TableName = newcode;
                if (!dt.ExtendedProperties.Contains("name"))
                {
                    dt.ExtendedProperties.Add("name", newcode);
                }
                else
                {
                    dt.ExtendedProperties["name"] = newcode;
                }
                DataBase.SchemaCreate(dt); 
                //eBase.PrintDataTable(dt);
                //eBase.End();
                dr["code"] = newcode;
            }
            /*
            if (modelid == ModelID) //主模块
            {
                string name = eParameters.QueryString("name");
                if (name.Length > 0)
                {
                    dr["mc"] = name;
                }
                else
                {
                    dr["mc"] = dr["mc"].ToString() + " - 复件";
                }
                string code = eParameters.QueryString("code");
                string oldcode = dr["code"].ToString();
                if (code.Length > 0 && oldcode.Length > 0)
                {
                    sql = eBase.DataBase.getTableSql(oldcode);
                    sql = System.Text.RegularExpressions.Regex.Replace(sql, "\\[" + oldcode + "\\]", "[" + code + "]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    sql = System.Text.RegularExpressions.Regex.Replace(sql, "'" + oldcode + "'", "'" + code + "'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    eBase.DataBase.Execute(sql);

                    dr["code"] = code;
                }
            }
            else
            {
                string code = eParameters.QueryString("code");
                if (code.Length > 0)
                {
                    string oldcode = dr["code"].ToString();
                    if (oldcode.Length > 0 && oldcode.ToLower().IndexOf("a_eke_sys") == -1)
                    {
                        code = oldcode + "_Copy";
                        sql = eBase.DataBase.getTableSql(oldcode);
                        sql = System.Text.RegularExpressions.Regex.Replace(sql, "\\[" + oldcode + "\\]", "[" + code + "]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        sql = System.Text.RegularExpressions.Regex.Replace(sql, "'" + oldcode + "'", "'" + code + "'", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        eBase.DataBase.Execute(sql);

                        dr["code"] = code;
                    }
                }
            }
            */
            string newModelID = Guid.NewGuid().ToString();
            dr["ModelID"] = newModelID;

            rows = Models.Select("ParentID='" + ModelID + "'");
            rows.UpdateForeignKey("ParentID", ModelID, newModelID, false);


            rows = ModelItems.Select("BindModelID='" + ModelID + "'");
            rows.UpdateForeignKey("BindModelID", ModelID, newModelID, true);

            rows = ModelItems.Select("FillModelID='" + ModelID + "'");
            rows.UpdateForeignKey("FillModelID", ModelID, newModelID, true);


            // eBase.Writeln(ModelID + " to " + newModelID);

            rows = Actions.Select("ModelID='" + ModelID + "'");
            rows.UpdateForeignKey("ModelID", ModelID, newModelID, true);

            rows = CheckUps.Select("ModelID='" + ModelID + "'");
            rows.UpdateForeignKey("ModelID", ModelID, newModelID, true);




            rows = ModelConditions.Select("ModelID='" + ModelID + "'");
            //rows.UpdateForeignKey("ModelID", newModelID, true);
            foreach (DataRow _dr in rows)
            {
                string _ModelConditionID = _dr["ModelConditionID"].ToString();
                string _newModelConditionID = Guid.NewGuid().ToString();
                _dr["ModelConditionID"] = _newModelConditionID;

                rows = ModelConditionItems.Select("ModelConditionID='" + _ModelConditionID + "'");
                rows.UpdateForeignKey("ModelConditionID", _ModelConditionID, _newModelConditionID, true);
                rows.UpdateForeignKey("ModelID", ModelID, newModelID, false);


                _dr["ModelID"] = newModelID;
            }


            rows = ModelItems.Select("ModelID='" + ModelID + "'");
            foreach (DataRow _dr in rows)
            {
                string _ModelItemID = _dr["ModelItemID"].ToString();
                string _newModelItemID = Guid.NewGuid().ToString();
                DataRow[] rs = ModelItems.Select("FillItem='" + _ModelItemID + "'");
                rs.UpdateForeignKey("FillItem", _ModelItemID, _newModelItemID, false);

                rs = ModelItems.Select("FillModelItemID='" + _ModelItemID + "'");
                rs.UpdateForeignKey("FillModelItemID", _ModelItemID, _newModelItemID, false);

                _dr["ModelItemID"] = _newModelItemID;
                _dr["ModelID"] = newModelID;
                //
            }
            //rows.UpdateForeignKey("ModelID",ModelID, newModelID,true);
            //eBase.PrintDataRow(rows);
            //eBase.PrintDataTable(ModelItems);




            rows = ModelTabs.Select("ModelID='" + ModelID + "'");
            foreach (DataRow _dr in rows)
            {
                string _ModelTabID = _dr["ModelTabID"].ToString();
                string _newModelTabID = Guid.NewGuid().ToString();
                _dr["ModelTabID"] = _newModelTabID;

                rows = Models.Select("ModelTabID='" + _ModelTabID + "'");
                rows.UpdateForeignKey("ModelTabID", _ModelTabID, _newModelTabID, false);

                rows = ModelPanels.Select("ModelTabID='" + _ModelTabID + "'");
                rows.UpdateForeignKey("ModelTabID", _ModelTabID, _newModelTabID, false);

                rows = ModelItems.Select("ModelTabID='" + _ModelTabID + "'");
                rows.UpdateForeignKey("ModelTabID", _ModelTabID, _newModelTabID, false);

                _dr["ModelID"] = newModelID;
            }


            rows = ModelPanels.Select("ModelID='" + ModelID + "'");
            foreach (DataRow _dr in rows)
            {
                string _ModelPanelID = _dr["ModelPanelID"].ToString();
                string _newModelPanelID = Guid.NewGuid().ToString();
                _dr["ModelPanelID"] = _newModelPanelID;

                rows = Models.Select("ModelPanelID='" + _ModelPanelID + "'");
                rows.UpdateForeignKey("ModelPanelID", _ModelPanelID, _newModelPanelID, false);

                rows = ModelItems.Select("ModelPanelID='" + _ModelPanelID + "'");
                rows.UpdateForeignKey("ModelPanelID", _ModelPanelID, _newModelPanelID, false);

                _dr["ModelID"] = newModelID;
            }
            //rows.UpdateForeignKey("ModelID", ModelID, newModelID, true);

            rows = Reports.Select("ModelID='" + ModelID + "'");
            foreach (DataRow _dr in rows)
            {
                string _ReportID = _dr["ReportID"].ToString();
                string _newReportID = Guid.NewGuid().ToString();
                _dr["ReportID"] = _newReportID;



                rows = ReportItems.Select("ReportID='" + _ReportID + "'");
                rows.UpdateForeignKey("ReportID", _ReportID, _newReportID, true);
                _dr["ModelID"] = newModelID;
            }
        }
        #endregion
        #region 生成XML
        //eBase.Write("<hr>");
        //eBase.Writeln("a_eke_sysModels");
        // eBase.PrintDataTable(Models);
        Models.ExtendedProperties.Add("name", "a_eke_sysModels");
        doc.appendData(Models);



        //eBase.Writeln("a_eke_sysActions");
        //eBase.PrintDataTable(Actions);
        Actions.ExtendedProperties.Add("name", "a_eke_sysActions");
        doc.appendData(Actions);


        //eBase.Writeln("a_eke_sysCheckUps");
        //eBase.PrintDataTable(CheckUps);
        CheckUps.ExtendedProperties.Add("name", "a_eke_sysCheckUps");
        doc.appendData(CheckUps);



        //eBase.Writeln("a_eke_sysModelConditions");
        //eBase.PrintDataTable(ModelConditions);
        ModelConditions.ExtendedProperties.Add("name", "a_eke_sysModelConditions");
        doc.appendData(ModelConditions);


        //eBase.Writeln("a_eke_sysModelConditionItems");
        //eBase.PrintDataTable(ModelConditionItems);
        ModelConditionItems.ExtendedProperties.Add("name", "a_eke_sysModelConditionItems");
        doc.appendData(ModelConditionItems);



        //eBase.Writeln("a_eke_sysModelItems");
        //eBase.PrintDataTable(ModelItems);
        ModelItems.ExtendedProperties.Add("name", "a_eke_sysModelItems");
        doc.appendData(ModelItems);




        //eBase.Writeln("a_eke_sysModelTabs");
        //eBase.PrintDataTable(ModelTabs);
        ModelTabs.ExtendedProperties.Add("name", "a_eke_sysModelTabs");
        doc.appendData(ModelTabs);



        //eBase.Writeln("a_eke_sysModelPanels");
        //eBase.PrintDataTable(ModelPanels);
        ModelPanels.ExtendedProperties.Add("name", "a_eke_sysModelPanels");
        doc.appendData(ModelPanels);

        //eBase.Writeln("a_eke_sysReports");
        //eBase.PrintDataTable(Reports);
        Reports.ExtendedProperties.Add("name", "a_eke_sysReports");
        doc.appendData(Reports);

        //eBase.Writeln("a_eke_sysReportItems");
        //eBase.PrintDataTable(ReportItems);
        ReportItems.ExtendedProperties.Add("name", "a_eke_sysReportItems");
        doc.appendData(ReportItems);

        #endregion
        // eBase.WriteHTML(doc.InnerXml);
        XmlNode node = doc.SelectSingleNode("/root/data");
        if (node != null)
        {
            foreach (XmlNode _node in node.ChildNodes)
            {
                DataTable dt = _node.ChildNodes.toDataTable();

                //eBase.Writeln(dt.Rows.Count.ToString());
                //eBase.PrintDataTable(dt);

                eBase.DataBase.SchemaImport(dt);
                //Access.SchemaImport(dt);
            }
        }
    }
    private string getModelTree(string pid, int space = 0)
    {
        DataTable dt = DataBase.getDataTable("select ModelID,MC,Code,Auto from a_eke_sysModels where ModelID='" + pid + "' and (auto=1 or ModelID='" + ParentID + "')");
        if (dt.Rows.Count == 0) return "";
        StringBuilder sb = new StringBuilder();

        sb.Append("<div style=\"line-height:30px;\">");

        sb.Append("<span style=\"display:inline-block;text-indent:" + space.ToString() + "px;width:160px;border:0px solid #ff0000;\">");
        if (space > 0) sb.Append("<img src=\"images/left_ico.jpg\" width=\"11\" height=\"11\" align=\"absmiddle\">&nbsp;");
        sb.Append(dt.Rows[0]["mc"].ToString() + "&nbsp;");
        sb.Append("</span>");
        string _name = dt.Rows[0]["modelid"].ToString().Split("-".ToCharArray())[0];
        //<input name="chk_" type="checkbox" id="chk_" value="checkbox" />
        sb.Append("名称：<input name=\"mc_" + _name + "\" type=\"text\" id=\"mc_" + _name + "\" style=\"padding-left:3px;height:21px;line-height:21px;width:150px;\" value=\"" + dt.Rows[0]["mc"].ToString() + (ParentID == dt.Rows[0]["modelid"].ToString() ? "-复件" : "") + "\" fieldname=\"名称\" notnull=\"true\" />&nbsp;&nbsp;");
        //sb.Append("表名：<input name=\"code_" + _name + "\" type=\"text\" id=\"code_" + _name + "\" style=\"padding-left:3px;height:21px;line-height:21px;width:150px;\" value=\"" + dt.Rows[0]["code"].ToString() + "\" fieldname=\"表名\" notnull=\"" + (dt.Rows[0]["auto"].ToString().ToLower().Replace("true","1")=="1" ?"true":"false") + "\" />");
        sb.Append("表名：<input name=\"code_" + _name + "\" type=\"text\" id=\"code_" + _name + "\" style=\"padding-left:3px;height:21px;line-height:21px;width:150px;\" value=\"" + dt.Rows[0]["code"].ToString() + "\" fieldname=\"表名\" notnull=\"" + (dt.Rows[0]["Code"].ToString().Length>0 ? "true" : "false") + "\" />");
        sb.Append("</div>");
        if (Single.Length > 0) return sb.ToString();

        DataTable tb = DataBase.getDataTable("select ModelID,MC from a_eke_sysModels where ParentID='" + pid + "' and delTag=0 order by JoinMore");// and Type in (1,3,6,7,8)
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
}