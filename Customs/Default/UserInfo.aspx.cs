using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;



    public partial class Customs_Default_UserInfo : System.Web.UI.Page
    {
        public eUser user;
        public string act = eParameters.Request("act");
        public string AppItem = eParameters.Request("AppItem");
        public eModel model;
        protected void Page_Load(object sender, EventArgs e)
        {
            user = new eUser("Application");
            user.Check();
            string ModelID = "5fcc3772-39be-4c7a-8c6e-9f64781407ae";
            model = new eModel(ModelID, user);

            model.Power["view"] = true; //给查看权限
            model.Power["edit"] = true; //给添加权限
            model.ID = user.ID;
            model.eForm.onChange += eForm_onChange;
            switch (act)
            {
                case "":
                    model.Action = "view";
                    //litBody.Text = model.getViewHTML();
                    litBody.Text = model.getActionHTML();
                    break;
                case "edit":
                     model.Action = "edit";
                    //litBody.Text = model.getEditHTML();
                     litBody.Text = model.getActionHTML();
                    break;
                case "save":
                    //model.Save();
                    model.autoHandle();
                    break;
            }
        }

        private void eForm_onChange(object sender, eFormTableEventArgs e)
        {
            if (e.eventType == eFormTableEventType.Updated)
            {
                DataTable tb = eBase.UserInfoDB.getDataTable("select * from a_eke_sysUsers where UserID='" + e.ID + "'");
                if (tb.Rows.Count > 0)
                {
                    user["xm"] = tb.Rows[0]["xm"].ToString();
                    if (tb.Rows[0]["face"].ToString().Length > 10)
                    {
                        user["face"] = eBase.getVirtualPath() + tb.Rows[0]["face"].ToString();
                    }
                    user.Save();
                }
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Master == null) return;
            Literal lit = (Literal)Master.FindControl("LitTitle");
            if (lit != null) lit.Text = eConfig.ApplicationTitle(user["SiteID"].ToString()); // model.ModelInfo["mc"].ToString() + " - " +

        }
    }
