using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageFrame.Web;
using AspxCommerce.LatestItems;
using AspxCommerce.Core;
using System.Web.Script.Serialization;

public partial class Modules_AspxCommerce_AspxLatestItems_LatestItemSettings : BaseAdministrationUserControl
{
    public int LatestItemCount, LatestItemInARow, LatestItemRssCount;
    public bool IsEnableLatestItem, IsEnableLatestRss;
    public string ModuleServicePath;
    public string Settings = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            UserModuleID = SageUserModuleID;
            ModuleServicePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
            IncludeJs("LatestItemSetting", "/js/FormValidation/jquery.validate.js");
            GetLatestItemSetting();
        }
        IncludeLanguageJS();
    }

    private void GetLatestItemSetting()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        AspxLatestItemsController objLatestItem = new AspxLatestItemsController();
        LatestItemSettingInfo objLatestSettingInfo = objLatestItem.GetLatestItemSetting(aspxCommonObj);
        if (objLatestSettingInfo != null)
        {
            object obj = new
            {

                IsEnableLatestItem = objLatestSettingInfo.IsEnableLatestItem,
                LatestItemCount=objLatestSettingInfo.LatestItemCount,
                IsEnableLatestRss = objLatestSettingInfo.IsEnableLatestRss,
                LatestItemRssCount = objLatestSettingInfo.LatestItemRssCount,
                LatestItemInARow = objLatestSettingInfo.LatestItemInARow,
                LatestItemRssPage=objLatestSettingInfo.LatestItemRssPage,
                ModuleServicePath = ModuleServicePath
            };
            Settings = json_serializer.Serialize(obj);
        }
    }

    public string UserModuleID { get; set; }
}