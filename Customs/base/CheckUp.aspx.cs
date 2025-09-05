using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using LitJson;

namespace eFrameWork.Customs.Base
{
    public partial class CheckUp : System.Web.UI.Page
    {
        public string UserArea = "Application";
        public string AppItem = eParameters.QueryString("AppItem");
        public string modelid = eParameters.QueryString("modelid");
        public string id = eParameters.QueryString("id");
        eUser user;
        eModel model;
        public DataRow cur = null;
        DataRow next = null;
        DataRow last = null;
        DataRow priv = null;
        public string options = "";
        public int backCount = 0;
        int height = 150;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["UserArea"] != null) UserArea = Request.QueryString["UserArea"].ToString();
          
            user = new eUser(UserArea);
            user.Check();
            model = new eModel();
            model.Action = "checkup";

            eDataBase db = model.DataBase;
            string _pk = db.getPrimaryKey("a_eke_sysCheckupRecords");
            if (_pk.Length == 0) db = eConfig.DefaultDataBase;

            DataTable dt = model.eForm.readData;
            if (dt.Rows.Count == 0) eResult.Message(new { success = 1, errcode = "-1", message = "数据不存在!" });
            DataRow dr = dt.Rows[0];
            if (!dr.Table.Columns.Contains("CheckupCode")) eResult.Message(new { success = 1, errcode = "-1", message = "数据没设置审核流程!" });
            string CheckupCode = dr["CheckupCode"].ToString();
            if (CheckupCode.Length == 0) eResult.Message(new { success = 1, errcode = "-1", message = "当前编码为空!" });
            bool self = false;
            //相关：1.eListControl,2.eModel,3,Checkup
            #region 填报人提交
            if (dr["CheckupCode"].ToString().ToLower() == "edit")
            {
                if (dr.Contains("adduser") && user.ID == dr["adduser"].ToString())
                {
                    self = true;
                }
                else
                {
                    if (dr.Contains("UserID") && user.ID == dr["UserID"].ToString())
                    {
                        self = true;
                    }
                }
            }
            #endregion
            #region 交接人确认
            {
                if (dr["CheckupCode"].ToString().ToLower().Contains("handover"))
                {
                    if (dr.Contains("Handover") && user.ID == dr["Handover"].ToString())
                    {
                        self = true;
                    }
                }
            }
            #endregion
           // eResult.Message(new { success = 1, errcode = "-1", message = "当前编码(" + CheckupCode + ")在审批流程中不存在!" });

            #region 当前审批编码
            /*
            bool self = false;
            string CheckupCode = "", addUserID = "";
            //string CheckupCode = model.DataBase.getValue("select CheckupCode from " + model.eForm.TableName + " Where " + model.eForm.primaryKey + "='" + model.eForm.ID + "'");
            string temp = model.DataBase.getValue("select CheckupCode + '|' + addUser from " + model.eForm.TableName + " Where " + model.eForm.primaryKey + "='" + model.eForm.ID + "'");
            if (temp.Length > 0)
            {
                string[] arr = temp.Split("|".ToCharArray());
                if (arr.Length == 2)
                {
                    CheckupCode = arr[0];
                    addUserID = arr[1];
                }
            }
            if (user.ID == addUserID && CheckupCode.ToLower() == "edit") self = true;
            if (CheckupCode.Length == 0)  eResult.Message(new { success = 1, errcode = "-1", message = "当前编码为空!" });
            */
            #endregion



            #region 当前审批编码在流程中是否存在
            DataRow[] Rows = model.CheckUpItems.Select("Code='" + CheckupCode + "'");
            if (Rows.Length == 0)  eResult.Message(new { success = 1, errcode = "-1", message = "当前编码(" + CheckupCode + ")在审批流程中不存在!" });
            cur = Rows[0];
            #endregion            
            #region 判断流程是否结束
            if (Convert.ToInt32(cur["PXNumber"]) >= model.CheckUpItems.Rows.Count) eResult.Message(new { success = 1, errcode = "-1", message = "该审批流程已经结束!" });
 
            #endregion


            #region 权限
            if (!self && !model.Power[cur["CheckCode"].ToString()])  eResult.Message(new { success = 1, errcode = "-1", message = "没有权限!" });

            #endregion


            #region 作废
            if (Request.QueryString["act"] != null)
            {
                string state = eParameters.QueryString("f1");
                if (state == "3")
                {
                    string idea = "无";
                    if (eParameters.QueryString("f2").Length > 0) idea = eParameters.QueryString("f2");
                    #region 审批记录
                    eTable etb = new eTable("a_eke_sysCheckupRecords", user);
                    etb.DataBase = db;// model.DataBase;
                    if (user["siteid"].Length > 0) etb.Fields.Add("SiteID", user["siteid"]);
                    etb.Fields.Add("UserID", user.ID);
                    if (modelid.Length > 0) etb.Fields.Add("modelid", modelid);
                    etb.Fields.Add("Checkuptype", model.ModelInfo["code"].ToString());
                    etb.Fields.Add("CheckupContentId", id);
                    etb.Fields.Add("CheckupCode", cur["CheckCode"].ToString());
                    etb.Fields.Add("CheckupText", cur["CheckMC"].ToString());
                    etb.Fields.Add("CheckupIdea", idea);
                    etb.Fields.Add("CheckupState", state);
                    etb.Add();
                    #endregion
                    #region 被审批信息
                    etb = new eTable(model.ModelInfo["code"].ToString(),user);
                    etb.DataBase = model.DataBase;
                    etb.Fields.Add("CheckupCode", "Stop");
                    etb.Fields.Add("CheckupText", "作废");
                    etb.Where.Add(model.eForm.primaryKey + "='" + id + "'");
                    etb.Update();
                    #endregion
                    eResult.Message(new { success = 1, errcode = "0", message = "作废成功!" });
                }
            }
            #endregion
            #region 取得下级及顶级审批编码
            next = model.CheckUpItems.Rows[Convert.ToInt32(cur["PXNumber"]) ];
            last = model.CheckUpItems.Rows[model.CheckUpItems.Rows.Count - 1];
            if (Convert.ToInt32(cur["PXNumber"]) > 1)
            {
                priv = model.CheckUpItems.Rows[Convert.ToInt32(cur["PXNumber"]) - 2];
            }            
            #endregion
            #region 返回流程
            if (cur["BackProcess"].ToString().Length > 0)
            {
                options = "<option value=\"\" selected>请选择</option>\r\n";;
                JsonData json = cur["BackProcess"].ToString().ToJsonData();
                backCount = json.Count;
                foreach (JsonData _json in json)
                {
                    options += "<option value=\"" + _json.GetValue("value") + "\">" + _json.GetValue("text") + "</option>\r\n";
                }
            }
            #endregion
            #region 审批
            if (Request.QueryString["act"] != null && Request.QueryString["act"].ToString().ToLower() == "checkup")
            {
                //HttpContext.Current.Items["ID"] = id;
                //model.CheckUp_callBack();
                //return;
                #region 审批记录
                string idea = "无";
                string state = "1";
                if (eParameters.QueryString("f1").Length > 0) state = eParameters.QueryString("f1");
                if (eParameters.QueryString("f2").Length > 0) idea = eParameters.QueryString("f2");
                string SignFile = eParameters.QueryString("SignFile");
                eTable etb = new eTable("a_eke_sysCheckupRecords", user);
                etb.DataBase = db;// model.DataBase;
                if (user["siteid"].Length > 0) etb.Fields.Add("SiteID", user["siteid"]);
                etb.Fields.Add("UserID", user.ID);
                if (modelid.Length > 0) etb.Fields.Add("modelid", modelid);
                etb.Fields.Add("Checkuptype",model.ModelInfo["code"].ToString());
                etb.Fields.Add("CheckupContentId", id);
                etb.Fields.Add("CheckupCode", cur["CheckCode"].ToString());
                etb.Fields.Add("CheckupText", cur["CheckMC"].ToString());
                etb.Fields.Add("CheckupIdea", idea);
                etb.Fields.Add("CheckupState", state);
                if (SignFile.Length > 0) etb.Fields.Add("SignFile", SignFile);
                etb.Add();
                #endregion
                #region 被审批信息
                etb = new eTable(model.ModelInfo["code"].ToString(),user);
                etb.DataBase = model.DataBase;
                if (state == "1") //通过
                {
                    etb.Fields.Add("CheckupCode", next["Code"].ToString());
                    etb.Fields.Add("CheckupText", next["MC"].ToString());
                }
                else //退回
                {
                    string backPress = eParameters.QueryString("f3");
                    if (backPress.Length > 0) //选择退回
                    {
                        etb.Fields.Add("CheckupCode", backPress);
                        Rows = model.CheckUpItems.Select("Code='" + backPress + "'");
                        if (Rows.Length > 0)
                        {
                            //etb.Fields.Add("CheckupText", Rows[0]["MC"].ToString());
                            etb.Fields.Add("CheckupText", "返回");
                        }
                    }
                    else //退回上一层级
                    {
                        if (priv != null)
                        {
                            etb.Fields.Add("CheckupCode", priv["Code"].ToString());
                            //etb.Fields.Add("CheckupText", priv["MC"].ToString());
                            etb.Fields.Add("CheckupText", "返回");
                        }
                    }
                }               
                etb.Where.Add(model.eForm.primaryKey + "='" + id + "'");
                etb.Update();
                #endregion
                HttpContext.Current.Items["ID"] = id;
                model.CheckUp_callBack();//审核回调-传递给自定义程序
                model.ExecuteAction("checkup", null, id);
                /*
                DataRow[] rows = model.Actions.Contains("Action") ? model.Actions.Select("Action='checkup'") : new DataTable().Select();
                foreach (DataRow dr in rows)
                {
                    string sql = dr["sql"].ToString();
                    sql = eParameters.Replace(sql, null, user, true);
                    sql = Regex.Replace(sql, "{table}", model.ModelInfo["code"].ToString(), RegexOptions.IgnoreCase);
                    if (sql.ToLower().IndexOf("{pk}") > -1)
                    {
                        string pk = model.DataBase.getPrimaryKey(model.ModelInfo["code"].ToString());
                        sql = Regex.Replace(sql, "{pk}", pk, RegexOptions.IgnoreCase);
                    }
                    sql = Regex.Replace(sql, "{data:id}", id, RegexOptions.IgnoreCase);
                   // eBase.AppendLog(sql);
                    model.DataBase.Execute(sql);
                }
                */

                eResult.Message(new { success = 1, errcode = "0", message = "审核成功!" });

                
            }
            #endregion
        }
        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            htmlWriter.Close();
            htmlWriter.Dispose();
            string content = stringWriter.ToString();
            stringWriter.Close();
            stringWriter.Dispose();
            #region 窗口高度计算
            if (cur["showState"].ToString() == "1") height += 30 + 30;
            if (cur["showIdea"].ToString() == "1") height += 100;
            if (cur["showIdea"].ToString() == "2") height += 180;
            #endregion

            JsonData json = new JsonData();
            json.Add("success", "1");
            json.Add("errcode", "0");
            json.Add("message", "加载成功");
            //json.Add("title", cur["Text"].ToString());
            json.Add("title", cur["CheckMC"].ToString());
            json.Add("text", cur["Text"].ToString());


            json.Add("height", height.ToString());
            json.Add("html", eBase.encode(content));
            Response.Clear();
            Response.Write(json.ToJson());
            Response.End();
        }
    }
}