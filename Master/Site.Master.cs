using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using EKETEAM.FrameWork;
using EKETEAM.Data;

namespace EKECMS.Master
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected eSiteInfo siteinfo;
        public eSiteInfo SiteInfo
        {
            get { return siteinfo; }
            set { siteinfo = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Render(
            if (SiteInfo == null) siteinfo = new eSiteInfo();
            Head1.SiteInfo = SiteInfo;
            Top1.SiteInfo = SiteInfo;
            Left1.SiteInfo = SiteInfo;
            CopyRight1.SiteInfo = SiteInfo;
            LitSubTopAds.Text = SiteInfo.SubTopAds;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            //zh-chs 简体中文
            //zh_cht 簡體中文
            string lang = eParameters.QueryString("lang");
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            try
            {
                base.Render(htmlWriter);
                htmlWriter.Close();
                string content = stringWriter.ToString();
                if (lang.ToLower() == "zh_cht")
                {
                    content = content.TraditionalChinese();
                    content = content.Replace("簡體", "简体");
                    content = Regex.Replace(content, ".aspx\\?", ".aspx?lang=zh_cht&", RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, "default.aspx", "Default.aspx?lang=zh_cht", RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, "lang=zh_cht&\\?lang", "lang", RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, "lang=zh_cht&lang", "lang", RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, "lang=zh_cht\\?lang", "lang", RegexOptions.IgnoreCase);
                    
                }
                content = Pub.StaticURL(content);


                writer.Write(content);
            }
            catch { }
            finally
            {
                stringWriter.Dispose();
                htmlWriter.Close();
                htmlWriter.Dispose();
            }
        }
    }
}