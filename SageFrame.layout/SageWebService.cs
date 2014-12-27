using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Xml;
using SageFrame.Layout;


namespace SageFrame
{
    /// <summary>
    /// Summary description for SageWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SageWebService : WebService
    {

        public SageWebService()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }
        [WebMethod]
        public string ActivateAccount(string activationkey, string domainName)
        {
            Feed objFeed = new Feed();
            string result = objFeed.GetKey(activationkey, domainName);
            string message = ParseResult(result);
            return result;
        }

        private void WriteName(string key)
        {
            XmlDocument doc = new XmlDocument();
            string app = string.Format("{0}{1}{2}{3}{4}{5}{6}", "~/App", "_", "Data", "/Lic", "ense/Lice", "nse.", "xml");
            string xmlPath = HttpContext.Current.Server.MapPath(app);
            doc.Load(xmlPath);
            XmlNode root = doc.DocumentElement;
            string name = string.Format("{0}{1}{2}", "SageFrame", "Lic", "ense");
            XmlNode pageNode = root.SelectSingleNode(name);
            pageNode.InnerText = key;
            doc.Save(xmlPath);
        }

        private string ParseResult(string result)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(result); // suppose that myXmlString contains "<Names>...</Names>"
            XmlNodeList xnList = xml.SelectNodes("/Names");
            string message = string.Empty;
            string firstName = string.Empty;
            foreach (XmlNode xn in xnList)
            {
                firstName = xn["Name"].InnerText;
                message = xn["Message"].InnerText;
            }
            if (firstName != string.Empty)
            {
                WriteName(firstName);
            }
            return message;
        }
    }
}