using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using EKETEAM.Data;
using EKETEAM.FrameWork;
using EKETEAM.UserControl;
using LitJson;

namespace eFrameWork.Manage
{
    public partial class AppUsers : System.Web.UI.Page
    {
        public string UserArea = "Manage";
        public string id = eParameters.Request("id");
        public string act = eParameters.Request("act").ToLower();
        public string sql = "";
        public eForm eform;
        public eUser user;
        public eList elist;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            user = new eUser(UserArea);
            eform = new eForm("a_eke_sysUsers", user);
            eform.DataBase = eBase.UserInfoDB;
            eform.AddControl(eSubForm1);
            eform.AddControl(eSubForm2);
            //edt.AutoRedirect = false;
            if (act.Length == 0)
            {
                List(user);
                return;
            }
            if (act == "getrole")
            {
                string roleid = eParameters.QueryString("roleid");
                DataTable rolePower = eBase.getUserPower(roleid, id);
                eBase.WriteJson(rolePower.toJSON());
                Response.End();
            }
            #region 信息添加、编辑
            if (act == "active") //是否显示
            {
                string sql = eParameters.Replace("update a_eke_sysUsers set Active={querystring:value} where UserID='{querystring:id}'", null, null);
                eBase.UserInfoDB.Execute(sql);
                Response.Redirect(Request.ServerVariables["HTTP_REFERER"] == null ? "Default.aspx" : Request.ServerVariables["HTTP_REFERER"].ToString(), true);
                eBase.End();
            }
            if (act == "getuser")
            {
                sql = "select count(*) from a_eke_sysUsers where yhm='" + eParameters.QueryString("value") + "'";
                string temp = eBase.UserInfoDB.getValue(sql);
                if (temp == "0")
                {
                    Response.Write("true");
                }
                else
                {
                    Response.Write("false");
                }
                Response.End();
            }
            if (act == "edit")
            {
                //f1.Attributes = " readOnly";
            }
           

            eform.AddControl(eFormControlGroup);
            eFormControl _roles = new eFormControl("Roles");
            _roles.Field = "RoleID";
            eform.AddControl(_roles);
            eform.onChange += new eFormTableEventHandler(eform_onChange);
            eform.Handle();

            F9.User = user;
            #endregion
            if (act == "add" || act == "edit")
            {
                eBase.clearDataCache("a_eke_sysPowers");
            }
        }

        public void eform_onChange(object sender, eFormTableEventArgs e)
        {
            if (e.eventType == eFormTableEventType.Inserted || e.eventType == eFormTableEventType.Updated)
            {
                string sql = "update a set a.Department=b.mc,a.Code=b.Code from a_eke_sysUsers a inner join Organizationals b on a.OrganizationalID=b.OrganizationalID where a.UserID='" + e.ID + "'";
                eBase.UserInfoDB.Execute(sql);
                //sql = "update a set a.postLevel=isnull(b.px,99) from a_eke_sysUsers a left join Dictionaries b on a.JobType=b.DictionarieID where a.UserID='" + e.ID + "'";
                sql = "update a set a.postLevel=isnull(b.orgLevel,99) from a_eke_sysUsers a left join Organizationals b on a.OrganizationalID=b.OrganizationalID where a.UserID='" + e.ID + "'";
                eBase.UserInfoDB.Execute(sql);

            }
            if (e.eventType == eFormTableEventType.Inserting || e.eventType == eFormTableEventType.Updating || e.eventType == eFormTableEventType.Deleting)
            {
               
            }

            if (e.eventType == eFormTableEventType.Inserting)
            {
                if (user["ServiceID"].Length > 0) eform.Fields.Add("ServiceID", user["ServiceID"]);
            }
            string parentID = e.ID;
        }



        private void List(eUser user)
        {
            eDataTable.CanEdit = true;
            eDataTable.CanDelete = true;
             elist = new eList("a_eke_sysUsers");
            elist.DataBase = eBase.UserInfoDB;
            elist.Fields.Add("CASE WHEN Active=1 THEN 'images/sw_true.gif' ELSE 'images/sw_false.gif' END as ShowPIC,CASE WHEN Active=1 THEN '0' ELSE '1' END as ShowValue");
            elist.Where.Add("delTag=0");
            //elist.Where.Add("ServiceID" + (user["ServiceID"].Length == 0 ? " is null" : "='" + user["ServiceID"] + "'"));
            elist.Where.Add("UserType<3 and UserType>0");
            elist.Where.Add(eSearchControlGroup);
            elist.OrderBy.Add("addTime");
            //elist.Bind(Rep, ePageControl1);
            elist.Bind(eDataTable, ePageControl1);
        }
        protected void Rep_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Control ctrl = e.Item.Controls[0];
                Literal lit = (Literal)ctrl.FindControl("LitBM");
                if (lit != null)
                {
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Master == null) return;
            Literal lit = (Literal)Master.FindControl("LitTitle");
            if (lit != null)
            {
                lit.Text = "用户管理 - " + eConfig.manageName(); 
            }
        }
    }
}