using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.FrameWork;

namespace EKECMS.Customs
{
    public partial class SiteInfo : System.Web.UI.Page
    {
        public string UserArea = "Application";
        public eForm eform;
        public eUser user;
        protected void Page_Load(object sender, EventArgs e)
        {
         
            user = new eUser(eBase.getUserArea(UserArea));
            user.Check();
       
            eModelInfo customModel = new eModelInfo(user);
            eModel model = customModel.Model;
            eform = new eForm("a_eke_sysWebSites", user);
            DataRow[] rows = eBase.a_eke_sysWebSites.Select("WebCode='" + user["WebCode"].ToString() + "'");
            if (rows.Length > 0)
            {
                string itemid = rows[0]["WebID"].ToString();
             
                eform.AddControl(eFormControlGroup);
                string act = eParameters.Form("act");
                if (act == "save")
                {
                    eform.LoadAction("save", itemid);
                }
                else
                {
                    eform.LoadAction("edit", itemid);
                }
            }

        }
    }
}