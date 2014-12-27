using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
using SageFrame.Web;
using System.Text;
using AspxCommerce.Core;

public partial class Modules_AspxCommerce_AspxStartUpEvents_AspxAPIEvent : SageFrameStartUpControl
{
    public string ModuleRedirectPath = "";
    public int StoreID, PortalID;
    public string CultureName = string.Empty;
    public string UserName = string.Empty;
    public string SessionCode = string.Empty;
    public int CustomerID;
    public string StoreDefaultCurrency, AllowRealTimeNotifications;
    protected void Page_Load(object sender, EventArgs e)
    {
        ModuleRedirectPath = ResolveUrl(this.aspxRedirectPath);
        StoreID = GetStoreID;
        PortalID = GetPortalID;
        CultureName = GetCurrentCultureName;
        UserName = GetUsername;
        CustomerID = GetCustomerID;

        if (HttpContext.Current.Session.SessionID != null)
        {
            SessionCode = HttpContext.Current.Session.SessionID.ToString();
        }
        StoreSettingConfig ssc = new StoreSettingConfig();
        AllowRealTimeNotifications = ssc.GetStoreSettingsByKey(StoreSetting.AllowRealTimeNotifications, GetStoreID, GetPortalID, GetCurrentCultureName);
        
        if (AllowRealTimeNotifications.ToLower() == "true")
        {
            IncludeJs("SignalR", false, "/js/SignalR/jquery.signalR-1.0.0-rc2.min.js", "/signalr/hubs", "/Modules/AspxCommerce/AspxStartUpEvents/js/RealTimeAspxMgmt.js");
        }
        StoreDefaultCurrency = ssc.GetStoreSettingsByKey(StoreSetting.MainCurrency, GetStoreID, GetPortalID, GetCurrentCultureName);
        IncludeAPIjs();
       IncludeLanguageAPIJS();
        
       
    }

    private void IncludeAPIjs()
    {
        try
        {
            Literal LitAPIjs = this.Page.FindControl("litAPIjs") as Literal;
            string strScript = string.Empty;
            string APIFolder = "~/Modules/AspxCommerce/AspxAPIJs/";
            if (Directory.Exists(Server.MapPath(APIFolder)))
            {
                bool isTrue = false;
                string[] fileList = Directory.GetFiles(Server.MapPath(APIFolder));

                foreach (var item in fileList)
                {
                    string regexPattern = ".*\\\\(?<file>[^\\.]+)(\\.[a-z]{2}-[A-Z]{2})?\\.js";

                    Regex regex = new Regex(regexPattern, RegexOptions.IgnorePatternWhitespace);

                    Match match = regex.Match(item);
                    string APIJsFile = match.Groups[2].Value;
                    string FileUrl = string.Empty;
                    isTrue = GetCurrentCulture() == "en-US" ? true : false;
                    if (isTrue)
                    {
                        FileUrl = APIFolder + APIJsFile + ".js";
                    }
                    else
                    {
                        FileUrl = APIFolder + APIJsFile + "." + GetCurrentCulture() + ".js";
                        if (!File.Exists(Server.MapPath(FileUrl)))
                        {
                            FileUrl = APIFolder + APIJsFile + ".js";
                        }
                    }
                    string inputString = string.Empty;

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<script type=\"text/javascript\">\n");
                    using (StreamReader streamReader = File.OpenText(Server.MapPath(FileUrl)))
                    {
                        inputString = streamReader.ReadLine();
                        while (inputString != null)
                        {
                            sb.Append(inputString + "\n");
                            inputString = streamReader.ReadLine();
                        }

                    }
                    sb.Append("</script>\n");
                    if (litAPIjs != null)
                    {
                        if (!litAPIjs.Text.Contains(sb.ToString()))
                        {
                            litAPIjs.Text += sb.ToString();
                        }
                    }
                    else
                    {
                        HttpContext.Current.Response.Write(sb.ToString());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }

    }
    public void IncludeLanguageAPIJS()
    {
        try
        {
            Literal LitLangResc = this.Page.FindControl("LitLangResc") as Literal;
            string strScript = string.Empty;
            string langFolder = "~/Modules/AspxCommerce/AspxAPIJs/Language/";
            if (Directory.Exists(Server.MapPath(langFolder)))
            {
                bool isTrue = false;
                string[] fileList = Directory.GetFiles(Server.MapPath(langFolder));

                string regexPattern = ".*\\\\(?<file>[^\\.]+)(\\.[a-z]{2}-[A-Z]{2})?\\.js";

                Regex regex = new Regex(regexPattern, RegexOptions.IgnorePatternWhitespace);

                Match match = regex.Match(fileList[0]);
                string languageFile = match.Groups[2].Value;
                string FileUrl = string.Empty;
                isTrue = GetCurrentCulture() == "en-US" ? true : false;
                if (isTrue)
                {
                    FileUrl = langFolder + languageFile + ".js";
                    // strScript = "<script src=\"" + ResolveUrl(FileUrl) + "\" type=\"text/javascript\"></script>";
                }
                else
                {
                    FileUrl = langFolder + languageFile + "." + GetCurrentCulture() + ".js";
                    // strScript = "<script src=\"" + ResolveUrl(FileUrl) + "\" type=\"text/javascript\"></script>";
                }
                string inputString = string.Empty;

                if (!File.Exists(Server.MapPath(FileUrl)))
                {
                    FileUrl = langFolder + languageFile + ".js";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<script type=\"text/javascript\">\n");
                using (StreamReader streamReader = File.OpenText(Server.MapPath(FileUrl)))
                {
                    inputString = streamReader.ReadLine();
                    while (inputString != null)
                    {
                        sb.Append(inputString + "\n");
                        inputString = streamReader.ReadLine();
                    }
                }
                sb.Append("</script>\n");
                if (LitLangResc != null)
                {
                    if (!LitLangResc.Text.Contains(sb.ToString()))
                    {
                        LitLangResc.Text += sb.ToString();
                    }
                }
                else
                {
                    HttpContext.Current.Response.Write(sb.ToString());

                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}