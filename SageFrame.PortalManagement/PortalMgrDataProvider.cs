#region "Copyright"
/*
FOR FURTHER DETAILS ABOUT LICENSING, PLEASE VISIT "LICENSE.txt" INSIDE THE SAGEFRAME FOLDER
*/
#endregion

#region "References"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SageFrame.Web.Utilities;
#endregion

namespace SageFrame.PortalManagement
{
    /// <summary>
    /// Manupulates data for PortalMgrDataProvider.
    /// </summary>
    public class PortalMgrDataProvider
    {
        /// <summary>
        /// Connect to database and add new portal.
        /// </summary>
        /// <param name="PortalName">Portal name.</param>
        /// <param name="IsParent">True for parent portal.</param>
        /// <param name="UserName">User name.</param>
        /// <param name="TemplateName">template name.</param>
        /// <param name="ParentPortal">Parent portal ID.</param>
        /// <param name="PSEOName">Page SEO name.</param>
        public static void AddPortal(string PortalName, bool IsParent, string UserName, string TemplateName,int ParentPortal,string PSEOName)
        {

            List<KeyValuePair<string, object>> ParaMeterCollection = new List<KeyValuePair<string, object>>();
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PortalName", PortalName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@IsParent", IsParent));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@TemplateName", TemplateName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@UserName", UserName));

            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PortalParentID", ParentPortal));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PSEOName", PSEOName));

            SQLHandler sagesql = new SQLHandler();
            sagesql.ExecuteNonQuery("sp_PortalAdd", ParaMeterCollection);
        }
		
        /// <summary>
        /// Connect to database and update existing portal.
        /// </summary>
        /// <param name="PortalID">PortalID</param>
        /// <param name="PortalName">Portal name.</param>
        /// <param name="IsParent">True for parent.</param>
        /// <param name="UserName">User name.</param>
        /// <param name="TemplateName">Template name.</param>
        public static void UpdatePortal(int PortalID, string PortalName, bool IsParent, string UserName, string TemplateName)
        {
            List<KeyValuePair<string, object>> ParaMeterCollection = new List<KeyValuePair<string, object>>();
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PortalID", PortalID));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PortalName", PortalName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@IsParent", IsParent));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@UserName", UserName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@TemplateName", TemplateName));

            SQLHandler sagesql = new SQLHandler();
            sagesql.ExecuteNonQuery("[sp_PortalUpdate]", ParaMeterCollection);


        }

        /// <summary>
        /// Connect to database and create portal for AspxCommerce.
        /// </summary>
        /// <param name="StoreName">Store name.</param>
        /// <param name="IsParent">Is Parent?</param>
        /// <param name="UserName">User Name.</param>
        /// <param name="TemplateName">Template Name.</param>
        /// <param name="ParentPortal">Parent Portal.</param>
        /// <param name="PSEOName">Portal SEO Name.</param>
        public void AddStoreSubscriber(string StoreName, bool IsParent, string UserName,
          string TemplateName, int ParentPortal, string PSEOName)
        {
            List<KeyValuePair<string, object>> ParaMeterCollection = new List<KeyValuePair<string, object>>();
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@StoreName", StoreName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@IsParent", IsParent));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@UserName", UserName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@TemplateName", TemplateName));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PortalParentID", ParentPortal));
            ParaMeterCollection.Add(new KeyValuePair<string, object>("@PSEOName", PSEOName));
            SQLHandler sagesql = new SQLHandler();
            sagesql.ExecuteNonQuery("[usp_Aspx_AddStoreSubscriber]", ParaMeterCollection);
        }
    }
}
