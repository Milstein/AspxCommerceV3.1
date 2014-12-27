using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Web;
using SageFrame.Dashboard;
using System.Web.UI;

namespace SageFrame.Layout
{
    public class Feed
    {

        public string GetKey(string activationkey, string domainName)
        {
            string macaddress = String.Empty;
            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != string.Empty ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string licenseKey = activationkey;
            string[] args = new string[4];
            args[0] = licenseKey;
            args[1] = macaddress;
            args[2] = ipAddress;
            args[3] = domainName;
            string service = "Oau" + "th" + "sageframe";
            string method = "Acti" + "vate" + "Account";
            string url = "http://lic" + "ense." + "sageframe." + "com/" + "Man" + "age/Oauth" + "sageframe" + ".asmx";
            WebServiceInvoker invoker =
                new WebServiceInvoker(
                    new Uri(url));
            string result = invoker.InvokeMethod<string>(service, method, args);
            return result;
        }
    }
}
