using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using LitJson;

namespace eFrameWork.Plugins
{
    public partial class PivotConfig : System.Web.UI.Page
    {
        private string AppItem = eParameters.Request("appitem");
        public string ModelID = eParameters.Request("modelid");
        private string UserArea = eParameters.Request("area");

        private eModel model;
        private eUser user;
        string sessionid = eParameters.Request("sessionid");

        private DataTable _contable;
        public DataTable ConTable
        {
            get
            {
                if (_contable == null)
                {
                    _contable = model.Items.Select("ModelItemID,MC,ListOrder,Type,Custom,Code,CustomCode", "showlist=1 and MC not in ('操作')", "ListOrder");
                    #region 初始
                    //_contable.Columns.Add("extend", typeof(int));//扩展字段
                    _contable.Columns.Add("parentid", typeof(string));
                    _contable.Columns.Add("oldMC", typeof(string));
                    _contable.Columns.Add("show", typeof(bool));
                    _contable.Columns.Add("group", typeof(bool));
                    _contable.Columns.Add("calctype", typeof(int));

                    foreach (DataRow dr in _contable.Rows)
                    {
                        //dr["extend"] = 0;
                        dr["oldMC"] = dr["MC"].ToString();
                        dr["show"] = true;
                        dr["group"] = false;
                        string _type = dr["Type"].ToString().ToLower();
                        if (_type.IndexOf("int") > -1 || _type.IndexOf("float") > -1 || _type.IndexOf("money") > -1 || _type.IndexOf("numeric") > -1 || _type.IndexOf("decimal") > -1) //
                        {
                            dr["calctype"] = 1; //汇总
                        }
                        else
                        {
                            dr["calctype"] = 0;
                        }
                    }
                    #endregion
                    string temp = eBase.UserInfoDB.getValue("select parValue from a_eke_sysUserCustoms Where " + (AppItem.Length == 0 ? "ApplicationID is null" : "ApplicationItemID='" + AppItem + "'") + " and ModelID='" + model.ModelID + "' and UserID='" + user.ID + "' and parName='pivotconfig'");
                    if (temp.Length > 3 && temp.StartsWith("["))
                    {
                        JsonData json = temp.ToJsonData();
                        DataTable tb = json.toDataTable();
                        if (!tb.Columns.Contains("parentid")) tb.Columns.Add("parentid", typeof(string));
                        //eBase.PrintDataTable(tb);
                        DataRow[] rows;
                        foreach (DataRow dr in tb.Rows)
                        {
                            if (dr["parentid"].ToString().Length == 0)
                            {                               
                                rows = _contable.Select("ModelItemID='" + dr["ModelItemID"].ToString() + "'");
                                if (rows.Length > 0)
                                {

                                    if (tb.Columns.Contains("parentid")) rows[0]["parentid"] = dr["parentid"].ToString();
                                    rows[0]["mc"] = dr.Contains("mc") ? dr["mc"].ToString() : "";
                                    rows[0]["show"] = eBase.parseBool(dr["show"].ToString());
                                    rows[0]["group"] = eBase.parseBool(dr["group"].ToString());
                                    rows[0]["calctype"] = Convert.ToInt32(dr["calctype"].ToString());
                                    rows[0]["ListOrder"] = Convert.ToInt32(dr["index"].ToString());
                                    //if (tb.Columns.Contains("extend")) rows[0]["extend"] = Convert.ToInt32(dr["extend"].ToString());

                                }
                            }
                            else //自定义的列
                            {
                                rows = _contable.Select("ModelItemID='" + dr["parentid"].ToString() + "'");
                                if (rows.Length > 0)
                                {
                                    //eBase.Writeln(dr["mc"].ToString() + "X");
                                    DataRow row = _contable.NewRow();
                                    row["ModelItemID"] = dr["ModelItemID"].ToString();
                                    row["MC"] = dr["MC"].ToString();
                                    row["parentid"] = dr["parentid"].ToString();
                                    row["show"] = eBase.parseBool(dr["show"].ToString());
                                    row["group"] = eBase.parseBool(dr["group"].ToString());
                                    row["calctype"] = Convert.ToInt32(dr["calctype"].ToString());
                                    row["ListOrder"] = Convert.ToInt32(dr["index"].ToString());
                                    _contable.Rows.Add(row);
                                    //eBase.Print(row);
                                }
                            }
                        }
                        _contable = _contable.Select("", "ListOrder").toDataTable();
                    }

                }
                return _contable;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
          
            if (sessionid.Length > 0)
            {
                user = new eUser(UserArea, sessionid);
                user["siteid"] = "1";
                user["Name"] = "匿名用户";
            }
            else
            {
                user = new eUser(UserArea);
            }
            model = new eModel(user);
            if (Request.Form["data"] != null)
            {
              
                JsonData data = JsonMapper.ToObject(eParameters.Form("data"));
                foreach (JsonData jd in data)
                {
                    if (jd["ModelItemID"].ToString().Length == 0)
                    {
                        jd["ModelItemID"] = Guid.NewGuid().ToString();
                        //eBase.Writeln(jd["ModelItemID"].ToString());
                    }
                }
                //eBase.AppendLog(data.ToJson().Length.ToString() + ":" +  data.ToJson());
                //eBase.Writeln(data.ToJson());
                //eBase.End();
                string sql = "";
                string ct = eBase.UserInfoDB.getValue("select count(1) from a_eke_sysUserCustoms Where " + (AppItem.Length == 0 ? "ApplicationID is null" : "ApplicationItemID='" + AppItem + "'") + " and ModelID='" + model.ModelID + "' and UserID='" + user.ID + "' and parName='pivotconfig'");
                if (ct == "0")
                {
                    sql = "insert into a_eke_sysUserCustoms (UserCustomID,ApplicationItemID,ModelID,UserID,parName,MC,parValue) ";
                    sql += " values ('" + Guid.NewGuid().ToString() + "'," + (AppItem.Length == 0 ? "NULL" : "'" + AppItem + "'") + ",'" + model.ModelID + "','" + user.ID + "','pivotconfig','透视设置','" + data.ToJson() + "')";
                }
                else
                {
                    sql = "update a_eke_sysUserCustoms set parValue='" + data.ToJson() + "' where " + (AppItem.Length == 0 ? "ApplicationID is null" : "ApplicationItemID='" + AppItem + "'") + " and ModelID='" + model.ModelID + "' and UserID='" + user.ID + "' and parName='pivotconfig'";
                }
                eBase.UserInfoDB.Execute(sql);
                eBase.clearDataCache("a_eke_sysUserCustoms");
                Response.End();
            }

            //eBase.PrintDataTable(ConTable);

            Rep.DataSource = ConTable;
            Rep.DataBind();
        }
    }
}