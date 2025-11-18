using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using LitJson;

namespace eFrameWork.Manage
{
    public partial class Labels : System.Web.UI.Page
    {
        public string UserArea = "Manage";
        public eAction Action;
        public eList elist;
        public eForm eform;
        public eUser user;
        public string AppId = "";
        private string id = eParameters.QueryString("id");
        public string pid = eParameters.QueryString("pid");
        public string allids = "";
        string sql = "";
        public bool Ajax = false;
        public string aspxfile = eBase.getAspxFileName();
        string tablename = "a_eke_sysLabels";
        string primaryKey = "LableID";
        string foreignKey = "ParentID";
        protected void Page_Load(object sender, EventArgs e)
        {

            allids = getParentIDS(pid);
            user = new eUser(eBase.getUserArea(UserArea));


            Action = new eAction();
            Action.Actioning += new eActionHandler(Action_Actioning);
            Action.Listen();
        }
        private string getParentIDS(string ID)
        {
            if (ID.Length == 0) return "";
            string _back = "";
            string pid = eBase.DataBase.getValue("select " + foreignKey + " from " + tablename + " where " + primaryKey + "='" + ID + "'");
            if (pid.Length == 0)
            {
                _back = ID;
            }
            else
            {
                _back = getParentIDS(pid) + "," + ID;
            }
            return _back;
        }
        private string getTree(string ParentID)
        {
            StringBuilder sb = new StringBuilder();


            sql = "select isnull(max(px),0) as maxpx,count(*) as ct from " + tablename + " where DelTag=0 and Type=1";
            sql += (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'");


            DataTable tb = eBase.DataBase.getDataTable(sql);
            if (tb.Rows.Count == 0) return "";
            if (Convert.ToInt32(tb.Rows[0]["ct"]) != Convert.ToInt32(tb.Rows[0]["maxpx"]))
            {
                sql = "update " + tablename + " set PX=(";
                sql += "select b.rownum from ";
                sql += "(";
                sql += "select ROW_NUMBER() over(order by px,addtime) as rownum," + primaryKey + ",addTime from " + tablename + " where delTag=0 and Type=1";
                sql += (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'");
                sql += ") as b where b." + primaryKey + "=" + tablename + "." + primaryKey;
                sql += ")  where delTag=0 and Type=1";
                sql += (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'");
                eBase.DataBase.Execute(sql);
            }

            sql = "select " + primaryKey + "," + foreignKey + ",MC,PX from " + tablename + " where DelTag=0 and Type=1";
            sql += (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'");
            sql += " Order by PX,addTime";
            tb = eBase.DataBase.getDataTable(sql);

            if (ParentID.Length == 0)
            {
                sb.Append("<ul id=\"etree\" class=\"etree\" PID=\"NULL\">\r\n");
            }
            else
            {
                sb.Append("<ul PID=\"" + ParentID + "\"" + (allids.IndexOf(ParentID) == -1 ? " style=\"display:none;\"" : "") + ">\r\n");
            }
            foreach (DataRow dr in tb.Rows)
            {
                sql = "select count(*) from  " + tablename + " where DelTag=0 and Type=1 and " + foreignKey + "='" + dr[primaryKey].ToString() + "'";


                string ct = eBase.DataBase.getValue(sql);
                sb.Append("<li oncontextmenu=\"return false;\" dataid=\"" + dr[primaryKey].ToString() + "\"");
                if (allids.ToLower().IndexOf(dr[primaryKey].ToString().ToLower()) == -1 || ct == "0")
                {
                    sb.Append(" dataurl=\"" + (ct == "0" ? "" : aspxfile + "?act=gethtml&pid=" + dr[primaryKey].ToString()) + "\"");
                    sb.Append(" class=\"" + (ct == "0" ? "" : "close") + "\">");
                    sb.Append("<div oncontextmenu=\"return false;\" style=\"-webkit-touch-callout:none; \" onmousedown2=\"div_contextmenu(event,this);\">");
                    sb.Append("<a dataid=\"" + dr[primaryKey].ToString() + "\" href=\"" + aspxfile + "?pid=" + dr[primaryKey].ToString() + "\" ");
                    sb.Append("oncontextmenu=\"return false;\" onmousedown2=\"contextmenu(event,this);\">" + dr["MC"].ToString() + " (" + ct + ")</a>");
                    sb.Append("</div>");
                }
                else
                {
                    sb.Append(" dataurl=\"\"");
                    sb.Append(" class=\"\">");
                    sb.Append("<div oncontextmenu=\"return false;\" style=\"-webkit-touch-callout:none; \" onmousedown2=\"div_contextmenu(event,this);\">");
                    sb.Append("<a dataid=\"" + dr[primaryKey].ToString() + "\" href=\"" + aspxfile + "?pid=" + dr[primaryKey].ToString() + "\" ");
                    sb.Append("oncontextmenu=\"return false;\" onmousedown2=\"contextmenu(event,this);\">" + dr["MC"].ToString() + " (" + ct + ")</a>");
                    sb.Append("</div>");
                    sb.Append(getTree(dr[primaryKey].ToString()));
                }

                sb.Append("</li>\r\n");
            }
            sb.Append("</ul>\r\n");
            return sb.ToString();
        }
        protected void Action_Actioning(string Actioning)
        {

            eform = new eForm(tablename, user);
            eform.ModelID = "1";
            if (Actioning.ToLower() == "edit")
            {
                M1_F1.ControlType = "text";
            }
            if (Actioning.ToLower() == "gethtml")
            {
                //Response.AddHeader("Content-Type", "application/json; charset=UTF-8");
                Response.Write(getTree(eParameters.QueryString("pid")));
                Response.End();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            if (Actioning.ToLower() == "setsort")
            {
                #region 位置
                string ParentID = eParameters.QueryString("pid").Replace("NULL", "");
                int index = Convert.ToInt32(eParameters.QueryString("index"));
                DataRow dr = eBase.DataBase.getDataTable("SELECT * FROM " + tablename + " where " + primaryKey + "='" + id + "'").Select()[0];
                string oldpid = dr[foreignKey].ToString();
                int oldindex = Convert.ToInt32(dr["px"]);


                if (ParentID == oldpid)//父级不变
                {
                    if (oldindex < index) //小变大
                    {
                        sql = "update " + tablename + " set PX=PX-1 where delTag=0 and Type=1 " + (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'") + " and PX>" + oldindex.ToString() + " and PX<=" + index.ToString();
                        eBase.DataBase.Execute(sql);
                    }
                    else //大变小
                    {
                        sql = "update " + tablename + " set PX=PX+1 where delTag=0 and Type=1 " + (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'") + " and PX>=" + index.ToString() + " and PX<" + oldindex.ToString();
                        eBase.DataBase.Execute(sql);
                    }
                    sql = "update " + tablename + " set PX='" + index.ToString() + "' where " + primaryKey + "='" + id + "'";
                    eBase.DataBase.Execute(sql);
                }
                else
                {
                    sql = "update " + tablename + " set PX=PX-1 where delTag=0 and Type=1 " + (oldpid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + oldpid + "'") + " and PX>" + oldindex.ToString();
                    eBase.DataBase.Execute(sql);

                    sql = "update " + tablename + " set PX=PX+1 where delTag=0 and Type=1 " + (ParentID.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + ParentID + "'") + " and PX>=" + index.ToString();
                    eBase.DataBase.Execute(sql);

                    sql = "update " + tablename + " set PX='" + index.ToString() + "'," + foreignKey + "=" + (ParentID.Length == 0 ? "NULL" : "'" + ParentID + "'") + " where " + primaryKey + "='" + id + "'";
                    eBase.DataBase.Execute(sql);



                }
                eBase.End();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
                #endregion
            }

            if (Actioning.Length > 0)
            {
                eform.onChange += new eFormTableEventHandler(eform_onChange);
                eform.AddControl(eFormControlGroup);
                if (Actioning == "add" && pid.Length > 0) M1_F2.Value = pid;
                eform.Handle();
            }
            else
            {
                string eTree = getTree("");
                if (Request.QueryString["ajax"] != null)
                {
                    Response.Clear();

                    JsonData json = new JsonData();
                    //json.Add("body", eBase.encode(eTree));
                    json["body"] = eTree;
                    Response.Write(json.ToJson());
                    Response.End();
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    LitBody.Text = eTree;
                }
            }
        }
        private void eform_onChange(object sender, eFormTableEventArgs e)
        {

            DataRow dr;
            string pid = "";
            string oldpid = "";
            int oldindex = 0;
            string maxpx = "";

            switch (e.eventType)
            {


                case eFormTableEventType.Inserting:
                    #region 添加
                    string tmp = M1_F1.Value.ToString().Replace("，", ",");
                    if (tmp.IndexOf("	") > -1 || tmp.IndexOf("\n") > -1 || tmp.IndexOf(",") > -1)
                    {
                        #region 批量
                        DateTime time = DateTime.Now;
                        string value = M1_F1.Value.ToString().Replace("，", ",");
                        foreach (string str in value.Replace("\r", "").Split("\n".ToCharArray()))
                        {
                            if (str.Trim().Length > 0)
                            {
                                foreach (string _str in str.Split("	".ToCharArray()))
                                {
                                    if (_str.Trim().Length > 0)
                                    {
                                        foreach (string key in _str.Replace("，", ",").Split(",".ToCharArray()))
                                        {
                                            if (key.Trim().Length > 0)
                                            {
                                                eTable etb = new eTable(tablename, user);
                                                etb.Fields.Add("Type", M1_Type.Value);
                                                etb.Fields.Add("MC", key.Trim());
                                                string parid = M1_F2.Value.ToString();
                                                if (parid.Length > 0) etb.Fields.Add("ParentID", parid);
                                                etb.Fields.Add("addTime", time.ToString());
                                                etb.Add();
                                                time = time.AddSeconds(1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        eResult.Success("添加成功!");
                        #endregion
                    }
                    else
                    {
                        string px = eform.Fields["px"].ToString();
                        pid = eform.Fields[foreignKey].ToString();
                        maxpx = eBase.DataBase.getValue("select isnull(max(px),0) + 1 from " + tablename + " where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'"));
                        if (px == "" || px == "0" || px == "999999" || Convert.ToInt32(px) > Convert.ToInt32(maxpx))
                        {
                            eform.Fields["px"] = maxpx;
                        }
                        else
                        {
                            sql = "update " + tablename + " set PX=PX+1 where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'") + " and PX>=" + px;
                            eBase.DataBase.Execute(sql);
                        }
                    }
                    #endregion
                    break;
                case eFormTableEventType.Updating:
                    #region 修改
                    break;
                    dr = eBase.DataBase.getDataTable("SELECT * FROM " + tablename + " where " + primaryKey + "='" + e.ID + "'").Select()[0];
                    pid = eform.Fields[foreignKey].ToString();
                    oldpid = dr[foreignKey].ToString();
                    oldindex = Convert.ToInt32(dr["px"]);
                    int index = Convert.ToInt32(eform.Fields["px"]);
                    if (pid == oldpid)//父级不变
                    {
                        if (oldindex < index) //小变大
                        {
                            sql = "update " + tablename + " set PX=PX-1 where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'") + " and PX>" + oldindex.ToString() + " and PX<=" + index.ToString();
                            eBase.DataBase.Execute(sql);
                        }
                        else //大变小
                        {
                            sql = "update " + tablename + " set PX=PX+1 where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'") + " and PX>=" + index.ToString() + " and PX<" + oldindex.ToString();
                            eBase.DataBase.Execute(sql);
                        }
                        maxpx = eBase.DataBase.getValue("select isnull(max(px),0) + 1 from " + tablename + " where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'"));
                        if (index > Convert.ToInt32(maxpx))
                        {
                            eform.Fields["px"] = maxpx;
                        }

                    }
                    else
                    {
                        sql = "update " + tablename + " set PX=PX-1 where delTag=0 and Type=1 " + (oldpid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + oldpid + "'") + " and PX>" + oldindex.ToString();
                        eBase.DataBase.Execute(sql);

                        sql = "update " + tablename + " set PX=PX+1 where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'") + " and PX>=" + index.ToString();
                        eBase.DataBase.Execute(sql);

                        maxpx = eBase.DataBase.getValue("select isnull(max(px),0) + 1 from " + tablename + " where delTag=0 and Type=1 " + (pid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + pid + "'"));
                        if (index > Convert.ToInt32(maxpx))
                        {
                            eform.Fields["px"] = maxpx;
                        }
                    }
                    #endregion
                    break;
                case eFormTableEventType.Deleting:
                    #region 删除
                    dr = eBase.DataBase.getDataTable("SELECT * FROM " + tablename + " where " + primaryKey + "='" + e.ID + "'").Select()[0];
                    oldpid = dr[foreignKey].ToString();
                    oldindex = Convert.ToInt32(dr["px"]);

                    sql = "update " + tablename + " set PX=PX-1 where delTag=0 and Type=1 " + (oldpid.Length == 0 ? " and " + foreignKey + " IS NULL" : " and " + foreignKey + "='" + oldpid + "'") + " and PX>" + oldindex.ToString();
                    eBase.DataBase.Execute(sql);

                    sql = "update " + tablename + " set PX='0' where " + primaryKey + "='" + e.ID + "'";
                    eBase.DataBase.Execute(sql);


                    #endregion
                    break;
                case eFormTableEventType.Deleted:
                    oldpid = eBase.DataBase.getValue("SELECT " + foreignKey + " FROM " + tablename + " where " + primaryKey + "='" + e.ID + "'");

                    if (Request.QueryString["ajax"] != null)
                    {
                        eResult.Success("删除成功!");
                    }
                    else
                    {

                        string url = aspxfile + "?act=view&id=" + oldpid;
                        Response.Redirect(url, true);
                    }
                    break;
            }
        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString["ajax"] != null) Ajax = Convert.ToBoolean(Request.QueryString["ajax"]);
            if (Master == null) return;
            if (!Ajax)
            {
                MasterPageFile = "Main.Master";
            }
            else
            {
                MasterPageFile = "MainNone.Master";
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (Master == null) return;
            Literal lit = (Literal)Master.FindControl("LitTitle");
            if (lit != null)
            {
                lit.Text = "标签 - " + eConfig.manageName();
            }
        }
    }
}