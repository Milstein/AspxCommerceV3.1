using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageFrame.Web;
using AspxCommerce.Core;
using AspxCommerce.WishItem;
using System.Web.Script.Serialization;

public partial class Modules_AspxCommerce_AspxWishList_WishItemsSetting : BaseAdministrationUserControl
{
    public string WishItemsModulePath;
    public int StoreID;
    public int CustomerID;
    public int PortalID;
    public string CultureName;
      
    public string wishItemsSettings = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            WishItemsModulePath = ResolveUrl((this.AppRelativeTemplateSourceDirectory));

            IncludeJs("RecentlyCompareJs", "/Modules/AspxCommerce/AspxWishList/js/WishItemsSetting.js");
            StoreID = GetStoreID;
            PortalID = GetPortalID;
            CustomerID = GetCustomerID;
            CultureName = GetCurrentCultureName;
            GetWishListItemsSettig();
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void GetWishListItemsSettig()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        WishItemController wic = new WishItemController();
        WishItemsSettingInfo objWishItemSetting = wic.GetWishItemsSetting(aspxCommonObj);
        if (objWishItemSetting != null)
        {
            object obj = new
            {
                IsEnableWishList = objWishItemSetting.IsEnableWishList,
                IsEnableImageInWishlist = objWishItemSetting.IsEnableImageInWishlist,
                NoOfRecentAddedWishItems = objWishItemSetting.NoOfRecentAddedWishItems,
                WishListPageName = objWishItemSetting.WishListPageName,
                WishItemsModulePath = WishItemsModulePath
            };
            wishItemsSettings = json_serializer.Serialize(obj);
        }
    }
}