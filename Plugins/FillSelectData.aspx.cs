using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EKETEAM.Data;
using EKETEAM.FrameWork;

namespace eFrameWork.Plugins
{
    public partial class FillSelectData : System.Web.UI.Page
    {
        public string ModelID = eParameters.Request("modelid");
        public string clientMode = eParameters.Request("clientMode");
        public string UserArea = "Application";
        public bool IsMobile = false;
        public eModel model;
        protected void Page_Load(object sender, EventArgs e)
        {
            IsMobile = clientMode == "mobile" ? true : false; //eBase.IsMobile();
            if (Request.QueryString["area"] != null) UserArea = Request.QueryString["area"].ToString();
            eUser user = new eUser(UserArea);
            model = new eModel(ModelID, user);
            //if (IsMobile) model.clientMode = "mobile";
            if (clientMode.Length > 0) model.clientMode = eParameters.QueryString("clientMode");
            LitBody.Text = model.getListHTML();

            if (model.Javasctipt.Length > 0) LitJavascript.Text = model.Javasctipt;
            if (model.cssText.Length > 0) LitStyle.Text = model.cssText;
        }
    }
}