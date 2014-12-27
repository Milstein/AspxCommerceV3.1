using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageFrame.Web;
using AspxCommerce.Core;
using System.Web.Script.Serialization;
using AspxCommerce.CompareItem;

public partial class Modules_AspxCommerce_AspxItemsCompare_ItemsCompareSetting :BaseAdministrationUserControl
{
    public string CompareItemsModulePath = string.Empty;
    public int StoreID;
    public int CustomerID;
    public int PortalID;
    public string CultureName;
    public string compareItemsSettings = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            CompareItemsModulePath = ResolveUrl((this.AppRelativeTemplateSourceDirectory));
            StoreID = GetStoreID;
            PortalID = GetPortalID;
            CustomerID = GetCustomerID;
            CultureName = GetCurrentCultureName;
            GetCompareItemsSettig();
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void GetCompareItemsSettig()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        CompareItemController cic = new CompareItemController();
        CompareItemsSettingInfo objCompareItemsSetting = cic.GetCompareItemsSetting(aspxCommonObj);
        if (objCompareItemsSetting != null)
        {
            object obj = new
            {
                IsEnableCompareItem = objCompareItemsSetting.IsEnableCompareItem,
                CompareItemCount = objCompareItemsSetting.CompareItemCount,
                CompareDetailsPage = objCompareItemsSetting.CompareDetailsPage,
                CompareItemsModulePath = CompareItemsModulePath
            };
            compareItemsSettings = json_serializer.Serialize(obj);
        }
    }
}