using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using LitJson;

namespace eFrameWork.Customs.Base
{
    public partial class AppRolePower : System.Web.UI.Page
    {
        public string AppItem = "";
        public string roleid = "";
        public string act = "";
        private string parentModelID = "";
        string sql = "";
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
        private DataTable _applications;
        public DataTable Applications
        {
            get
            {
                if (_applications == null)
                {
                    string sql = "select ApplicationID,MC from a_eke_sysApplications where delTag=0";
                    if (eItems.Get("eUser") != null)
                    {                      
                        eUser user = eItems.Get("eUser");
                        string siteid = user["siteid"].ToString();
                        if (siteid.Length > 0 && siteid != "0")
                        {
                            sql += " and ApplicationID in (select ApplicationID from a_eke_sysSiteItems where deltag=0 and SiteID=" + siteid + ")";
                        }
                    }
                    sql += " order by isnull(px,999999),addTime";
                    _applications = DataBase.getDataTable(sql);
                }
                return _applications;
            }
        }
        private DataTable _applicationitems;
        public DataTable ApplitionItems
        {
            get
            {
                if (_applicationitems == null)
                {
                    sql = "select a.ApplicationItemID,a.ApplicationID,a.ModelID,a.ParentID,a.MC,b.Power,c.Power as userPower,c.canList,c.Condition,c.RoleID,a.PX,a.addTime";
                    sql += " from a_eke_sysApplicationItems a";
                    sql += " left join a_eke_sysModels b on a.ModelID=b.ModelID";
                    sql += " left join a_eke_sysPowers2 c on a.ApplicationItemID=c.ApplicationItemID and a.ModelID=c.ModelID and c.delTag=0 and c.UserID is null and c.RoleID " + (roleid.Length == 0 ? "is null" : "='" + roleid + "'");
                    sql += " where a.delTag=0";// order by a.addTime
                    //_applicationitems = DataBase.getDataTable(sql);
                    //eBase.Writeln(_applicationitems.Rows.Count.ToString());
                   // eBase.PrintDataTable(_applicationitems);


                    sql = "select a.ApplicationItemID,a.ApplicationID,a.ModelID,a.ParentID,a.MC,b.Power,a.PX,a.addTime";//,c.Power as userPower,c.canList,c.Condition,c.RoleID
                    sql += " from a_eke_sysApplicationItems a";
                    sql += " left join a_eke_sysModels b on a.ModelID=b.ModelID";
                    sql += " where a.delTag=0"; // order by a.addTime
                    //sql += " and (a.PackID is null or (a.PackID is not null and a.show=1 and b.Auto=1))";



                    _applicationitems = DataBase.getDataTable(sql);
                    _applicationitems.Columns.Add("userPower", typeof(string));
                    _applicationitems.Columns.Add("canList", typeof(bool));
                    _applicationitems.Columns.Add("Condition", typeof(string));
                    _applicationitems.Columns.Add("RoleID", typeof(Guid));
                    _applicationitems.Columns.Add("Propertys", typeof(string));

                    _applicationitems = _applicationitems.Select("ApplicationItemID,ApplicationID,ModelID,ParentID,MC,Power,userPower,canList,Condition,RoleID,Propertys,PX,addTime", "", "");
                    
                   

                    //DataRow[] rs = eBase.a_eke_sysPowers.Select("(UserID='" + user["ID"].ToString() + "'" + (RoldID.Length > 0 ? " or Convert(RoleID, 'System.String') in ('" + RoldID.Replace(",", "','") + "')" : " and RoleID is null") + ")  and canList=1 and ApplicationID is not null and Convert(ApplicationID, 'System.String') in (" + appids + ")");

                    if (roleid.Length > 0)
                    {
                        foreach (DataRow dr in _applicationitems.Rows)
                        {
                            DataRow[] _rs = eBase.a_eke_sysPowers.Select("ApplicationItemID='" + dr["ApplicationItemID"].ToString() + "' and ApplicationID='" + dr["ApplicationID"].ToString() + "' and ModelID='" + dr["ModelID"].ToString() + "' and UserID is null and RoleID='" + roleid + "' and delTag=0");
                            if (_rs.Length > 0)
                            {
                                dr["userPower"] = _rs[0]["Power"];
                                dr["canList"] = _rs[0]["canList"];
                                dr["Condition"] = _rs[0]["Condition"];
                                dr["RoleID"] = roleid;
                                dr["Propertys"] = _rs[0]["Propertys"];
                            }
                        }
                    }
                   
                }
                return _applicationitems;
            }
        }
        private DataTable _checkups;
        public DataTable CheckUps
        {
            get
            {
                if (_checkups == null)
                {
                    sql = "SELECT ModelID,CheckMC as text,LOWER(CheckCode) as value,px,addTime FROM a_eke_sysCheckUps where delTag=0 and CheckCode not in ('Submit','HandoverConfirm') and LEN(CheckMC)>0 and LEN(CheckCode)>0";
                    _checkups = DataBase.getDataTable(sql);
                }
                return _checkups;
            }
        }
        public string filename = "";
        public eUser user;
        public eModel model;
        public string UserArea = "Application";
        private DataTable _userpower;
        /// <summary>
        /// 当前登录用户的权限
        /// </summary>
        private DataTable UserPower
        {
            get
            {
                if (_userpower == null)
                {
                    string user_Roles = eBase.UserInfoDB.getValue("SELECT RoleID FROM a_eke_sysUsers where UserID='" + user.ID + "'");//当前登录用户的角色
                    _userpower = eBase.getUserPower(user_Roles, user.ID);
                }
                return _userpower;
            }
        }
        private List<string> _userpowerids;
        private List<string> UserPowerIDS
        {
            get
            {
                if (_userpowerids == null)
                {
                    //仅仅模块，多层级有BUG
                    //_userpowerids = (from row in UserPower.AsEnumerable() where row["URL"].ToString().Length == 0 && row["list"].ToString() == "true" select row["ApplicationItemID"].ToString()).ToList();
                    _userpowerids = (from row in UserPower.AsEnumerable() select row["ApplicationItemID"].ToString()).ToList();//模块级目录
                }
                return _userpowerids;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser(UserArea);
            model = new eModel(user);
            filename = eBase.getAspxFileName().ToLower();
            roleid = eParameters.Request("id");
            act = eParameters.Request("act").ToLower();
            AppItem = eParameters.Request("AppItem");

            if (AppItem.Length > 0)
            {
                DataRow[] appRows = eBase.a_eke_sysApplicationItems.Select("ApplicationItemID='" + AppItem + "'");
                if (appRows.Length == 0) return;
                parentModelID = appRows[0]["ModelID"].ToString();
            }
            switch (act)
            {
                case "del":
                    sql = "update a_eke_sysPowers set delTag=1 where RoleID='" + roleid + "' and UserId is null and ApplicationID is not null";
                    eBase.UserInfoDB.Execute(sql);

                    break;
                case "save":
                    save();                    
                    runtimeCache.Remove();
                    HttpRuntime.Cache.Remove("DataCache_AppTopModelURL");
                    break;
                default:
                    if (act.Length > 0) LitBody.Text = getAppRolePower();  
                    break;
            }

        }
        private void save()
        {
            if(roleid.Length==0)
            {
                if (HttpContext.Current.Items["ID"] != null) roleid = HttpContext.Current.Items["ID"].ToString();
            }
            JsonData json = null;
            string jsonstr = eParameters.Form("eformdata_" + parentModelID);
            if (jsonstr.Length > 0)
            {
                json = JsonMapper.ToObject(jsonstr);
                json = json.GetCollection("eformdata_" + parentModelID).GetCollection()[0];
            }   
            foreach (DataRow dr in Applications.Rows)
            {
                DataRow[] rows = ApplitionItems.Select("ApplicationID='" + dr["ApplicationID"].ToString() + "'", "px,addTime");
              
                string appitems = "";
                string modelids = "";
                foreach (DataRow _dr in rows)
                {
                    string name = "model_list_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "");
                    string value = json == null ? eParameters.Form(name) : json.getValue(name);
                    
                    if (value.Length > 0) //有权限
                    {
                        string canList = "0";
                        string cond = "";
                        string prop = "";
                        string power = "";
                        JsonData uPower = JsonMapper.ToObject("[]");
                        #region 基本权限
                        DataTable Power = _dr["Power"].ToString().ToJsonData().toRows();
                        foreach (DataRow dr1 in Power.Rows)
                        {
                            name = "model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "");
                            value = json == null ? eParameters.Form(name) : json.getValue(name); 

                            JsonData _power = new JsonData();
                            if (value.Length == 0)
                            {
                                _power.Add(dr1["value"].ToString(), "false");
                                if (dr1["value"].ToString().ToLower() == "list") canList = "0";
                            }
                            else
                            {
                                _power.Add(dr1["value"].ToString(), "true");
                                if (dr1["value"].ToString().ToLower() == "list") canList = "1";
                            }
                            uPower.Add(_power);
                        }
                        #endregion
                        #region 审批权限
                        DataRow[] _rs = CheckUps.Select("ModelID='" + _dr["ModelID"].ToString() + "'", "px,addTime");
                        foreach (DataRow dr1 in _rs)
                        {
                            name = "model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "");
                          
                            value = json == null ? eParameters.Form(name) : json.getValue(name);
                            JsonData _power = new JsonData();
                            if (value.Length == 0)
                            {
                                _power.Add(dr1["value"].ToString(), "false");
                                if (dr1["value"].ToString().ToLower() == "list") canList = "0";
                            }
                            else
                            {
                                _power.Add(dr1["value"].ToString(), "true");
                                if (dr1["value"].ToString().ToLower() == "list") canList = "1";
                            }
                            uPower.Add(_power);
                        }
                        power = uPower.ToJson();
                  

                        //eBase.Writeln(canList + "::" + power);
                        #endregion
                        name = "model_cond_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "");
                        cond = json == null ? eParameters.Form(name) : json.getValue(name);
                        if (cond.Length > 0)  cond = eBase.decode(cond);
                        if (cond.Length == 0 && roleid.Length > 0 && _dr["ModelID"].ToString().Length > 0)
                        {
                            if ((json != null && json.Contains(name) == false) || (json == null && Request.Form[name] == null))
                            {
                                //cond = eBase.UserInfoDB.getValue("select Condition from a_eke_sysPowers where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "' and delTag=0");
                                cond = eBase.UserInfoDB.eList("a_eke_sysPowers")
                                .Fields.Add("Condition")
                                .Where.Add("UserID is Null")
                                .Where.Add("ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "'")
                                .Where.Add("ModelID='" + _dr["ModelID"].ToString() + "'")
                                .Where.Add("RoleID='" + roleid + "'")
                                .Where.Add("delTag=0")
                                .getValue(); 
                            }
                        }
                        //cond = cond.Replace("'", "''");

                        name = "model_prop_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "");
                        prop = json == null ? eParameters.Form(name) : json.getValue(name);
                        if (prop.Length > 0) prop = eBase.decode(prop);

                        if (prop.Length == 0 && roleid.Length > 0 && _dr["ModelID"].ToString().Length>0)
                        {
                            if ((json != null && json.Contains(name) == false) || (json == null && Request.Form[name] == null))
                            {
                                prop = eBase.UserInfoDB.getValue("select Propertys from a_eke_sysPowers where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "' and delTag=0");
                            }
                        }

                        //prop = prop.Replace("'", "''");
                        if (prop.Length == 0) prop = "{}";
                        /*
                        sql = "if exists (select * from a_eke_sysPowers where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "')";
                        sql += " update a_eke_sysPowers set delTag=0,canList='" + canList + "',Condition='" + cond + "',Propertys='" + prop + "',power='" + power + "' where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "'";
                        sql += " else ";
                        sql += "insert into a_eke_sysPowers (PowerID,ApplicationItemID,ApplicationID,ModelID,UserID,RoleID,canList,Condition,Propertys,Power) ";
                        sql += " values ('" + Guid.NewGuid().ToString() + "','" + _dr["ApplicationItemID"].ToString() + "','" + _dr["ApplicationID"].ToString() + "','" + _dr["ModelID"].ToString() + "',NULL,'" + roleid + "','" + canList + "','" + cond + "','" + prop + "','" + power + "')";
                        */
                        string ct = eBase.UserInfoDB.getValue("select count(1) from a_eke_sysPowers where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "'");
                        if (ct == "0")
                        {
                            //sql = "insert into a_eke_sysPowers (PowerID,ApplicationItemID,ApplicationID,ModelID,UserID,RoleID,canList,Condition,Propertys,Power) ";
                            //sql += " values ('" + Guid.NewGuid().ToString() + "','" + _dr["ApplicationItemID"].ToString() + "','" + _dr["ApplicationID"].ToString() + "','" + _dr["ModelID"].ToString() + "',NULL,'" + roleid + "','" + canList + "','" + cond + "','" + prop + "','" + power + "')";
                            eBase.UserInfoDB.eTable("a_eke_sysPowers")
                                .Fields.Add("PowerID", Guid.NewGuid().ToString())
                                .Fields.Add("ApplicationItemID", _dr["ApplicationItemID"].ToString())
                                .Fields.Add("ApplicationID", _dr["ApplicationID"].ToString())
                                .Fields.Add("ModelID", _dr["ModelID"].ToString())
                                .Fields.Add("UserID", null)
                                .Fields.Add("RoleID",roleid)
                                .Fields.Add("canList", canList)
                                .Fields.Add("Condition", cond)
                                .Fields.Add("Propertys", prop)
                                .Fields.Add("Power", power)
                                .Add();
                        }
                        else
                        {
                            sql = " update a_eke_sysPowers set delTag=0,canList='" + canList + "',Condition='" + cond + "',Propertys='" + prop + "',power='" + power + "' where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "'  and RoleID='" + roleid + "'";
                            eBase.UserInfoDB.eTable("a_eke_sysPowers")
                                .Fields.Add("delTag", 0)
                                .Fields.Add("canList", canList)
                                .Fields.Add("Condition", cond)
                                .Fields.Add("Propertys", prop)
                                .Fields.Add("Power", power)
                                .Where.Add("ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "'")
                                .Where.Add("ModelID='" + _dr["ModelID"].ToString() + "'")
                                .Where.Add("RoleID='" + roleid + "'")
                                .Update();
                        }
                       
                    }
                    else //无权限
                    {
                        //continue;
                        //sql = "update a_eke_sysPowers set canList=0,Power='',Condition='',delTag=1 where userID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID='" + _dr["ModelID"].ToString() + "' and RoleID='" + roleid + "'";
                        sql = "delete from a_eke_sysPowers where UserID is Null and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID" + (_dr["ModelID"].ToString().Length > 0 ? "='" + _dr["ModelID"].ToString() + "'" : " is null") + " and RoleID='" + roleid + "'";
                        eBase.UserInfoDB.Execute(sql);
                    }
                    //continue;
                    if (_dr["ApplicationItemID"].ToString().Length > 0) appitems += (appitems.Length == 0 ? "" : ",") + "'" + _dr["ApplicationItemID"].ToString() + "'";
                    if( _dr["ModelID"].ToString().Length>0 ) modelids += (modelids.Length == 0 ? "" : ",") + "'" + _dr["ModelID"].ToString() + "'";

                    #region 清理应用里不存在的权限
                    if (_dr["ApplicationItemID"].ToString().Length > 0 && _dr["ModelID"].ToString().Length > 0)
                    {
                        //重新绑定时去删除
                        //sql = "delete from a_eke_sysPowers where ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "' and ModelID!='" + _dr["ModelID"].ToString() + "'";
                        //eBase.UserInfoDB.Execute(sql);
                    }
                    #endregion
                }
                #region 清理应用里不存在的权限
                if (appitems.Length > 0)
                {
                    sql = "delete from a_eke_sysPowers where UserID is Null and RoleID='" + roleid + "' and ApplicationID='" + dr["ApplicationID"].ToString() + "' and ApplicationItemID not in (" + appitems + ")";
                    eBase.UserInfoDB.Execute(sql);
                }
                if (modelids.Length > 0)
                {
                    sql = "delete from a_eke_sysPowers where UserID is Null and RoleID='" + roleid + "' and ApplicationID='" + dr["ApplicationID"].ToString() + "' and ModelID not in (" + modelids + ")";
                    eBase.UserInfoDB.Execute(sql);
                }
                #endregion
            }
            //eBase.End();
        }
        private string getmodels( string appid, string parentid)
        {
            StringBuilder sb = new StringBuilder();
            DataRow[] rows = ApplitionItems.Select("ApplicationID='" + appid + "' and " + (parentid.Length > 0 ? "ParentID='" + parentid + "'" : "ParentID is null"), "px,addTime");
            foreach (DataRow _dr in rows)
            {
                if (_dr["ModelID"].ToString().Length > 0)
                {
                    DataRow[] rss = UserPower.Select("ApplicationID='" + appid + "' and ApplicationItemID='" + _dr["ApplicationItemID"].ToString() + "'");//新增加

                    

                    if (!model.Power["allpower"])
                    {
                        if (rss.Length == 0) continue;//新增加
                        
                    }
                    #region 模块
                    sb.Append("<div class=\"powerModel\">");
                    sb.Append("<span class=\"modelname\">");
                    sb.Append("<input type=\"checkbox\" name=\"model_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" id=\"model_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" value=\"true\" onclick=\"userSelectAll(this);\"" + (_dr["canList"].ToString() == "True" ? " checked" : "") + (act == "view" ? " disabled" : "") + " />");
                    sb.Append("<label for=\"model_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\">" + _dr["mc"].ToString() + "</label>");
                    sb.Append("</span>");
                    #region 开发平台
                    if (filename.StartsWith("app"))
                    {
                        sb.Append("<span class=\"cond\">");
                        sb.Append("条件：<input type=\"text\" class=\"text\" name=\"model_cond_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" value=\"" + _dr["Condition"].ToString() + "\" " + (act == "view" ? " disabled" : "") + " />");
                        sb.Append("</span>");


                        sb.Append("<span class=\"cond\">");
                        string prop = _dr["Propertys"].ToString();
                        if (prop.Length == 0) prop = "{}";
                        sb.Append("扩展：<textarea style=\"height:20px;vertical-align:middle;\" name=\"model_prop_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\"" + (act == "view" ? " disabled" : "") + ">" + prop + "</textarea>");
                        sb.Append("</span>");
                    }
                    #endregion




                    string temp = eParameters.ReplaceJsonChar(_dr["Power"].ToString());
                    DataTable Power = temp.StartsWith("[") ?  JsonMapper.ToObject(temp).toRows() : new DataTable();
                    temp = eParameters.ReplaceJsonChar(_dr["UserPower"].ToString());
                    DataTable UPower = temp.StartsWith("[") ? JsonMapper.ToObject(temp).toColumn() : new DataTable();

                    #region 基本权限
                    foreach (DataRow dr1 in Power.Rows)
                    {
                        if (rss.Length > 0 && !rss[0].Table.Columns.Contains(dr1["value"].ToString())) continue;//新增加
                        //if (rss.Length > 0 && rss[0][dr1["value"].ToString()].ToString().ToLower() != "true") continue;//新增加(本人无权限，则退出)
                        bool has = false;
                        if (UPower.Rows.Count > 0 && UPower.Columns.Contains(dr1["value"].ToString()))
                        {
                            has = Convert.ToBoolean(UPower.Rows[0][dr1["value"].ToString()].ToString());
                        }
                        sb.Append("<span class=\"poweritem\">");
                        sb.Append("<input type=\"checkbox\" name=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" id=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" value=\"true\"" + (has == true ? " checked" : "") + (act == "view" ? " disabled" : ""));
                        if (dr1["value"].ToString().ToLower() == "list") sb.Append(" onclick=\"userCanelAll(this);\"");
                        sb.Append(" />");
                        sb.Append("<label for=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\">" + dr1["text"].ToString() + "</label>");
                        sb.Append("</span>");
                    }

                    #endregion
                    #region 审批权限
                    DataRow[] _rs = CheckUps.Select("ModelID='" + _dr["ModelID"].ToString() + "'", "px,addTime"); 
                    foreach (DataRow dr1 in _rs)
                    {
                        if (rss.Length > 0 && !rss[0].Table.Columns.Contains(dr1["value"].ToString())) continue;//新增加
                        //if (rss.Length > 0 && rss[0][dr1["value"].ToString()].ToString().ToLower() != "true") continue;//新增加(本人无权限，则退出)

                        bool has = false;
                        if (UPower.Rows.Count > 0 && UPower.Columns.Contains(dr1["value"].ToString()))
                        {
                            has = Convert.ToBoolean(UPower.Rows[0][dr1["value"].ToString()].ToString());
                        }
                        sb.Append("<span class=\"powercheckupitem\">");
                        sb.Append("<input type=\"checkbox\" name=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" id=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\" value=\"true\"" + (has == true ? " checked" : "") + (act == "view" ? " disabled" : "") + " />");
                        sb.Append("<label for=\"model_" + dr1["value"].ToString() + "_" + _dr["ApplicationItemID"].ToString().Replace("-", "") + "_" + _dr["ModelID"].ToString().Replace("-", "") + "\">" + dr1["text"].ToString() + "</label>");
                        sb.Append("</span>");
                    }
                    #endregion
                    sb.Append("</div>");
                    #endregion
                    if (_dr["ApplicationItemID"].ToString() == "6d396879-e05f-4063-97f0-40474c3c0fed")
                    {
                    }
                }
                else//目录
                {
                    DataRow[] subrows = ApplitionItems.Select("ApplicationID='" + _dr["ApplicationID"].ToString() + "' and ParentID='" + _dr["ApplicationItemID"].ToString() + "'");
                    if (subrows.Length == 0) continue;

                    if (!model.Power["allpower"])
                    {
                        DataRow[] _rows = (from row in subrows.AsEnumerable()
                                          where
                                          UserPowerIDS.Any(p => row["ApplicationItemID"].ToString().Contains(p))
                                          select row).ToArray();
                        if (_rows.Length == 0) continue;
                    }

                    sb.Append("<div class=\"powerico\">\r\n");
                    sb.Append("<a href=\"javascript:;\" class=\"close\" onclick=\"showPower(this);\">" + _dr["MC"].ToString() + "</a>");
                    sb.Append("</div>\r\n");
                    sb.Append("<div class=\"powerContent\" style=\"display:none;\">\r\n");
                    sb.Append("<label style=\"margin-left:27px;\"><input type=\"checkbox\" onclick=\"SelectContent(this);\"" + (act == "view" ? " disabled" : "") + ">全选</label>");
                    sb.Append(getmodels(_dr["ApplicationID"].ToString(), _dr["ApplicationItemID"].ToString()));
                    sb.Append("</div>\r\n");
                }
            }
            return sb.ToString();
        }
        private string getAppRolePower()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"allmodelbody\" class=\"powerModel\">\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall\" onclick=\"SelectAllModel(this);\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall\">全选</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_list\" onclick=\"SelectAllItem(this,'model_list_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_list\">列表</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_view\" onclick=\"SelectAllItem(this,'model_view_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_view\">详细</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_add\" onclick=\"SelectAllItem(this,'model_add_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_add\">添加</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_edit\" onclick=\"SelectAllItem(this,'model_edit_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_edit\">编辑</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_del\" onclick=\"SelectAllItem(this,'model_del_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_del\">删除</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_copy\" onclick=\"SelectAllItem(this,'model_copy_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_copy\">复制</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_print\" onclick=\"SelectAllItem(this,'model_print_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_print\">打印</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_import\" onclick=\"SelectAllItem(this,'model_import_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_import\">导入</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_export\" onclick=\"SelectAllItem(this,'model_export_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_export\">导出</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_recycle\" onclick=\"SelectAllItem(this,'model_recycle_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_recycle\">回收站</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_version\" onclick=\"SelectAllItem(this,'model_version_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_version\">数据版本</label></span>\r\n");
            sb.Append("<span class=\"poweritem\"><input type=\"checkbox\" id=\"selall_change\" onclick=\"SelectAllItem(this,'model_change_');\"" + (act == "view" ? " disabled" : "") + "><label for=\"selall_change\">变更</label></span>\r\n");
            sb.Append("</div>\r\n");
            foreach (DataRow dr in Applications.Rows)
            {
                DataRow[] rows = ApplitionItems.Select("ApplicationID='" + dr["ApplicationID"].ToString() + "' and ParentID is null", "px,addTime");
                if (rows.Length == 0) continue;
                DataRow[] rss = UserPower.Select("ApplicationID='" + dr["ApplicationID"].ToString() + "' and list='true'");
                if (rss.Length == 0) continue;//新增加

                sb.Append("<div class=\"powerico\">\r\n");
                sb.Append("<a href=\"javascript:;\" class=\"close\" onclick=\"showPower(this);\">" + dr["MC"].ToString() + "</a>");
                sb.Append("</div>\r\n");
                sb.Append("<div class=\"powerContent\" style=\"display:none;\">\r\n");
                sb.Append("<label style=\"margin-left:27px;\"><input type=\"checkbox\" onclick=\"SelectContent(this);\"" + (act == "view" ? " disabled" : "") + ">全选</label>");
                sb.Append(getmodels(dr["ApplicationID"].ToString(), ""));
                sb.Append("</div>");
            }
            
            return sb.ToString();
        }
    }
}