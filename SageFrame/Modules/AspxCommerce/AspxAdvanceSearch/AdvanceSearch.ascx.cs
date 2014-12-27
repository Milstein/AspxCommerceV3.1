using System;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Collections;
using SageFrame.Web;
using SageFrame.Framework;
using AspxCommerce.Core;
using AspxCommerce.AdvanceSearch;

public partial class Modules_AspxCommerce_AspxAdvanceSearch_AdvanceSearch : BaseAdministrationUserControl
{
    public int StoreID;
    public int CustomerID;
    public int PortalID;
    public string UserName;
    public string CultureName;
    public string UserIP, CountryName;
    public string SessionCode = string.Empty;
    public string NoImageAdSearchPath, AllowAddToCart, AllowOutStockPurchase;
    public int NoOfItemsInARow;
    public string ItemDisplayMode;
    public string AdvanceSearchModulePath;
    public AdvanceSearchSettingInfo adSettingInfo = new AdvanceSearchSettingInfo();
    public List<AspxTemplateKeyValue> AspxTemplateValue = new List<AspxTemplateKeyValue>();

    protected void Page_Load(object sender, EventArgs e)    
    {
        try
        {
            if (!IsPostBack)
            {
                IncludeCss("AdvanceSearch", "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                           "/Templates/" + TemplateName + "/css/PopUp/style.css",
                           "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                           "/Templates/" + TemplateName + "/css/FancyDropDown/fancy.css",
                           "/Modules/AspxCommerce/AspxAdvanceSearch/css/AdvanceSearch.css");
                IncludeJs("AdvanceSearch", "/js/Templating/tmpl.js", "/js/encoder.js", "/js/Paging/jquery.pagination.js",
                    "/js/Templating/AspxTemplate.js", "/js/PopUp/custom.js",
                          "/js/jquery.tipsy.js", "/js/FancyDropDown/itemFancyDropdown.js",
                          "/js/SageFrameCorejs/itemTemplateView.js",
                          "/Modules/AspxCommerce/AspxAdvanceSearch/js/AdvanceSearch.js",
                          "/Modules/AspxCommerce/AspxAdvanceSearch/js/AdvanceSearchAPI.js");
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                CustomerID = GetCustomerID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
               
                AdvanceSearchModulePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
                UserIP = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(UserIP, out CountryName);

                GetAdvanceSearchSetting();
                StoreSettingConfig ssc = new StoreSettingConfig();
                NoImageAdSearchPath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
                AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);                
                NoOfItemsInARow = adSettingInfo.NoOfItemsInARow;
                ItemDisplayMode = ssc.GetStoreSettingsByKey(StoreSetting.ItemDisplayMode, StoreID, PortalID, CultureName);
                IncludeLanguageJS();
            }
            LoadAllCategoryForSearch();
            if (adSettingInfo.IsEnableBrandSearch)
            {
                trBrand.Visible = true;
                GetAllBrandForItem(0, false);
            }
            GetAspxTemplates();
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void GetAdvanceSearchSetting()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
        AdvanceSearchController asc = new AdvanceSearchController();
        List<AdvanceSearchSettingInfo> adSettingList = asc.GetAdvanceSearchSetting(aspxCommonObj);
        if (adSettingList != null && adSettingList.Count > 0)
        {
            foreach (var item in adSettingList)
            {
                adSettingInfo = adSettingList[0];
            }
        }
    }

    Hashtable hst = null;
    public void LoadAllCategoryForSearch()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        int rowCount = 0;
        hst = AppLocalized.getLocale(modulePath);
        AdvanceSearchController asc = new AdvanceSearchController();
        List<CategoryInfo> catList = asc.GetAllCategoryForSearch("---", true, aspxCommonObj);
        StringBuilder categoryContent = new StringBuilder();
        categoryContent.Append("<select id=\"ddlCategory\" class=\"\">");
        if (catList != null && catList.Count > 0)
        {            
            categoryContent.Append("<option value='0'>" + getLocale("--All Category--") + "</option>");
            categoryContent.Append("<optgroup label=\"");
            categoryContent.Append(getLocale("General Categories"));
            categoryContent.Append("\">");

            foreach (CategoryInfo item in catList)
            {
                if (item.IsChecked == false)
                {
                    categoryContent.Append("<option value=" + item.CategoryID + " isGiftCard=" + item.IsChecked + ">" + item.LevelCategoryName + "</option>");
                }
                else
                {

                    rowCount += 1;
                    if (rowCount == 1)
                    {
                        categoryContent.Append("</optgroup>");
                        categoryContent.Append("<optgroup label=\"");
                        categoryContent.Append(getLocale("Gift Card Categories"));
                        categoryContent.Append("\">");
                    }
                    categoryContent.Append("<option value=" + item.CategoryID + " isGiftCard=" + item.IsChecked + ">" + item.LevelCategoryName + "</option>");
                }

            }
            if (rowCount > 0)
            {
                categoryContent.Append("</optgroup>");
            }           
        }
        else {
            categoryContent.Append("<option value=\"-1\">No Category Listed!</option>");
        }
        categoryContent.Append("</select>");
        ltrCategories.Text = categoryContent.ToString();
    }

    public void GetAllBrandForItem(int categoryID, bool isGiftCard)
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        hst = AppLocalized.getLocale(modulePath);

        AdvanceSearchController asc = new AdvanceSearchController();
        List<BrandItemsInfo> lstBrandItem = asc.GetAllBrandForSearchByCategoryID(aspxCommonObj, categoryID, isGiftCard);
        StringBuilder brandContent = new StringBuilder();
        brandContent.Append("<select id=\"lstBrands\">");

        if (lstBrandItem != null && lstBrandItem.Count > 0)
        {
            brandContent.Append("<option value='0'>" + getLocale("All Brands") + "</option>");
            if (lstBrandItem != null && lstBrandItem.Count > 0)
            {
                foreach (BrandItemsInfo item in lstBrandItem)
                {
                    brandContent.Append("<option value=" + item.BrandID + ">" + item.BrandName + "</option>");
                }
                brandContent.Append("</select>");
            }
        }
        else
        {
            brandContent.Append("<option value='0'>" + getLocale("No Brands") + "</option>");
        }
        ltrBrands.Text = brandContent.ToString();
    }

    private void GetAspxTemplates()
    {
        AspxTemplateValue = AspxGetTemplates.GetAspxTemplateKeyValue();

        foreach (AspxTemplateKeyValue value in AspxTemplateValue)
        {
            string xx = value.TemplateKey + "@" + value.TemplateValue;
            Page.ClientScript.RegisterArrayDeclaration("jsTemplateArray", "\'" + xx + "\'");
        }
    }

    private string getLocale(string messageKey)
    {
        string retStr = messageKey;
        if (hst != null && hst[messageKey] != null)
        {
            retStr = hst[messageKey].ToString();
        }
        return retStr;
    }
}