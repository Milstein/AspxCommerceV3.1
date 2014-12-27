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
#endregion

namespace SageFrame.PortalManagement
{
    /// <summary>
    /// Business logic for PortalMgrController.
    /// </summary>
    public class PortalMgrController
    {
        /// <summary>
        /// Add new portal.
        /// </summary>
        /// <param name="PortalName">Portal name.</param>
        /// <param name="IsParent">True for parent portal.</param>
        /// <param name="UserName">User name.</param>
        /// <param name="TemplateName">template name.</param>
        /// <param name="ParentPortal">Parent portal ID.</param>
        /// <param name="PSEOName">Page SEO name.</param>
        public static void AddPortal(string PortalName, bool IsParent, string UserName, string TemplateName,int ParentPortal,string  PSEOName)
        {
            try
            {
                PortalMgrDataProvider.AddPortal(PortalName, IsParent, UserName, TemplateName, ParentPortal, PSEOName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Update existing portal.
        /// </summary>
        /// <param name="PortalID">PortalID</param>
        /// <param name="PortalName">Portal name.</param>
        /// <param name="IsParent">True for parent.</param>
        /// <param name="UserName">User name.</param>
        /// <param name="TemplateName">Template name.</param>
        public static void UpdatePortal(int PortalID, string PortalName, bool IsParent, string UserName, string TemplateName)
        {
            try
            {
                PortalMgrDataProvider.UpdatePortal(PortalID, PortalName, IsParent, UserName, TemplateName);
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        /// <summary>
        /// Create portal for AspxCommerce.
        /// </summary>
        /// <param name="StoreName">Store name.</param>
        /// <param name="IsParent">Is Parent?</param>
        /// <param name="UserName">User Name.</param>
        /// <param name="TemplateName">Template Name.</param>
        /// <param name="ParentPortal">Parent Portal.</param>
        /// <param name="PSEOName">Portal SEO Name.</param>
        public static void AddStoreSubscriber(string StoreName, bool IsParent, string UserName, string TemplateName, int ParentPortal, string PSEOName)
        {
            try
            {
                PortalMgrDataProvider pmdp = new PortalMgrDataProvider();
                pmdp.AddStoreSubscriber(StoreName, IsParent, UserName, TemplateName, ParentPortal, PSEOName);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
