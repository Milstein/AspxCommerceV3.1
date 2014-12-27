<%@ WebService Language="C#" Class="DashboardWebService" %>
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Collections.Generic;
using SageFrame.Web;
using SageFrame.Dashboard;
using System.IO;
using SageFrame.Pages;
using SageFrame.ModuleManager;
using SageFrame.Security.Entities;
using SageFrame.Security;
using SageFrame.Services;

/// <summary>
/// Summary description for DashboardWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class DashboardWebService : AuthenticateService
{

    public DashboardWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<Link> GetPages(int PortalID, int userModuleId, string UserName, string secureToken)
    {
        if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
        {
            return DashboardController.GetAdminPages(PortalID);
        }
        else
        {
            return null;
        }
    }

    [WebMethod]
    public void AddLink(QuickLink linkObj, int PortalID, int userModuleId, string UserName, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                linkObj.ImagePath = linkObj.ImagePath == null || linkObj.ImagePath == "" ? "" : linkObj.ImagePath;
                DashboardController.AddQuickLink(linkObj);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    [WebMethod]
    public List<QuickLink> GetQuickLinks(string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                return (DashboardController.GetQuickLinksAll(UserName, PortalID));
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    [WebMethod]
    public void DeleteQuickLink(int QuickLinkID, string UserName, int PortalID, int userModuleId, string secureToken)
    {
        if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
        {
            DashboardController.DeleteQuickLink(QuickLinkID);
        }

    }

    [WebMethod]
    public List<Sidebar> GetSidebar(string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                return (DashboardController.GetSidebarAll(UserName, PortalID));
            }
            else
            {
                return null;
            }

        }
        catch (Exception)
        {

            throw;
        }
    }
    [WebMethod]
    public List<Sidebar> GetParentLinks(int SidebarItemID, string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                return (DashboardController.GetParentLinks(SidebarItemID));
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    [WebMethod]
    public void AddSidebar(Sidebar sidebarObj, string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                sidebarObj.ImagePath = sidebarObj.ImagePath == null || sidebarObj.ImagePath == "" ? "" : sidebarObj.ImagePath;
                DashboardController.AddSidebar(sidebarObj);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    [WebMethod]
    public void DeleteSidebarItem(int SidebarItemID, string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                DashboardController.DeleteSidebarItem(SidebarItemID);
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    [WebMethod]
    public Sidebar GetSidebarItem(int SidebarItemID, int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        Sidebar objSidebar = new Sidebar();
        try
        {
            if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
            {
                objSidebar = (DashboardController.GetSidebarDetails(SidebarItemID));
            }
            return objSidebar;
        }
        catch (Exception)
        {

            throw;
        }
    }


    //[WebMethod]
    //public QuickLink GetQuickLinkItem(int QuickLinkItemID)
    //{
    //    try
    //    {
    //        return (DashboardController.GetQuickLinkDetails(QuickLinkItemID));
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}

    [WebMethod]
    public void ReorderSidebar(List<DashboardKeyValue> OrderList, string UserName, int PortalID, int userModuleId, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                DashboardController.ReorderSidebarLink(OrderList);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    //[WebMethod]
    //public void ReorderQuickLinks(List<DashboardKeyValue> OrderList, string UserName, int PortalID, int userModuleId, string secureToken)
    //{
    //    try
    //    {
    //        if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
    //        {
    //            DashboardController.ReorderQuickLinks(OrderList);
    //        }
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}

    [WebMethod]
    public void UpdateSidebarLinks(Sidebar sidebarObj, int PortalID, int userModuleId, string UserName, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                sidebarObj.ImagePath = sidebarObj.ImagePath != null ? sidebarObj.ImagePath : "";
                DashboardController.UpdateSidebar(sidebarObj);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    [WebMethod]
    public void UpdateLink(QuickLink linkObj, int PortalID, int userModuleId, string UserName, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
            {
                linkObj.ImagePath = linkObj.ImagePath != null ? linkObj.ImagePath : "";
                DashboardController.UpdateLink(linkObj);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }


    //[WebMethod]
    //public DashboardQuickInfo GetDashboardLinks(string UserName, int PortalID)
    //{
    //    DashboardQuickInfo dashObj = new DashboardQuickInfo();
    //    dashObj.LSTQuickLinks = DashboardController.GetQuickLinks(UserName, PortalID);
    //    return dashObj;
    //}

    [WebMethod]
    public string GetLocalizedMessage(string CultureCode, string ModuleName, string MessageType, int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        string LocalizedMessage = string.Empty;

        if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
        {
            LocalizedMessage = (SageMessage.ProcessSageMessage(CultureCode, ModuleName, MessageType));
        }
        return LocalizedMessage;
    }

    [WebMethod]
    public void UpdateSidebar(string status, int PortalID, string UserName)
    {
        DashbordSettingInfo objSetting = new DashbordSettingInfo();
        objSetting.SettingKey = DashboardSettingKeys.SIDEBAR_MODE.ToString();
        objSetting.SettingValue = status;
        objSetting.PortalID = PortalID;
        objSetting.UserName = UserName;
        try
        {
            DashboardController.AddUpdateDashboardSettings(objSetting);
        }
        catch (Exception)
        {

            throw;
        }
    }
    [WebMethod]
    public void UpdateAppearance(string theme, int PortalID, string UserName, int userModuleId, string secureToken)
    {
        if (IsPostAuthenticated(PortalID, userModuleId, UserName, secureToken))
        {
            DashbordSettingInfo objSetting = new DashbordSettingInfo();
            objSetting.SettingKey = DashboardSettingKeys.DASHBOARD_THEME.ToString();
            objSetting.SettingValue = theme;
            objSetting.PortalID = PortalID;
            objSetting.UserName = UserName;
            try
            {
                DashboardController.AddUpdateDashboardSettings(objSetting);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    [WebMethod]
    public List<ModuleWebInfo> GetModuleWebInfo()
    {
        try
        {

            return DashboardController.GetModuleWebInfo();
        }
        catch (Exception)
        {

            throw;
        }
    }

    [WebMethod]
    public string GetDefaultBlog()
    {
        try
        {
            return DashboardController.GetBlogContent();
        }
        catch (Exception)
        {

            throw;
        }

    }

    [WebMethod]
    public string GetDefaultNews()
    {
        try
        {
            return DashboardController.GetNewsContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    [WebMethod]
    public string GetDefaultTutorial()
    {
        try
        {
            return DashboardController.GetTutorialContent();
        }
        catch (Exception)
        {
            throw;
        }
    }

    [WebMethod]
    public void UpdageDefaultBlog(string content)
    {
        try
        {
            DashboardController.UpdateBlogContent(content);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    [WebMethod]
    public void UpdateDefaultTutorial(string content)
    {
        try
        {
            DashboardController.UpdateTutorialContent(content);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    [WebMethod]
    public void UpdateDefaultNews(string content)
    {
        try
        {
            DashboardController.UpdateNewsContent(content);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    [WebMethod]
    public void DeleteIcon(string IconPath, int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        try
        {
            if (IsPostAuthenticated(PortalID, UserModuleID, UserName, secureToken))
            {

                string filepath = SageFrame.Templating.Utils.GetAbsolutePath(string.Format("Modules/DashBoard/Icons/{0}", IconPath));
                if (File.Exists(filepath))
                {
                    File.SetAttributes(filepath, System.IO.FileAttributes.Normal);
                    File.Delete(filepath);
                }
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    //---------PageModuleStatistics--------------------
    [WebMethod]
    public List<PageEntity> GetPageName(bool IsAdmin, int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        List<PageEntity> objPageEntity = new List<PageEntity>();
        try
        {
            if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
            {
                PageController objPageController = new PageController();
                objPageEntity = (objPageController.GetPages(PortalID, IsAdmin));
            }
            return objPageEntity;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
    [WebMethod]
    public List<LayoutMgrInfo> Getmodules(int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        List<LayoutMgrInfo> objLayoutMgr = new List<LayoutMgrInfo>();
        try
        {
            if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
            {
                objLayoutMgr = (LayoutMgrDataProvider.GetModules(PortalID));
            }
            return objLayoutMgr;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }


    [WebMethod]
    public CountUserInfo GeneralSnapShot(bool IsAdmin, int PortalID, int UserModuleID, string UserName, string secureToken)
    {
        CountUserInfo objCountUser = new CountUserInfo();
        try
        {
            if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
            {
                objCountUser = (DashboardDataProvider.GetGeneralSnapShot(PortalID, IsAdmin));
            }
            return objCountUser;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }


    [WebMethod]
    public List<UserInfo> GetUsers(int PortalID, int UserModuleID, string secureToken, string UserName)
    {
        List<UserInfo> objUserInfo = new List<UserInfo>();
        try
        {
            if (IsPostAuthenticatedView(PortalID, UserModuleID, UserName, secureToken))
            {
                MembershipController m = new MembershipController();
                objUserInfo = (m.SearchUsers("", "", PortalID, UserName).UserList);
            }
            return objUserInfo;
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
}