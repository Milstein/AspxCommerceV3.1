using System;
using System.Collections.Generic;
using System.Web;
using SageFrame.Web;
using SageFrame;
using SageFrame.Framework;
using SageFrame.Web.Common.SEO;
using SageFrame.Web.Utilities;
using AspxCommerce.Core;
using SageFrame.Core;
using System.Collections;
using System.Text;

public partial class Modules_AspxDetails_AspxCategoryDetails_CategoryDetails : BaseAdministrationUserControl
{
    public int StoreID, PortalID, CustomerID;
    public string UserName, CultureName;
    public string UserIp;
    public string CountryName = string.Empty;
    public string SessionCode = string.Empty;
    public string Categorykey = "";
    public string NoImageCategoryDetailPath, AllowAddToCart, AllowOutStockPurchase;
    public int NoOfItemsInARow;
    public string ItemDisplayMode;
    public decimal minPrice = 0;
    public decimal maxPrice = 0;
    public int IsCategoryHasItems = 0;
    public List<Filter> arrPrice = new List<Filter>();
    public List<AspxTemplateKeyValue> AspxTemplateValue = new List<AspxTemplateKeyValue>();

    protected void page_init(object sender, EventArgs e)
    {
        try
        {
            SageFrameRoute parentPage = (SageFrameRoute)this.Page;
            Categorykey = parentPage.Key;
            Categorykey = AspxUtility.fixedDecodeURIComponent(Categorykey);
            StoreID = GetStoreID;
            PortalID = GetPortalID;
            CustomerID = GetCustomerID;
            UserName = GetUsername;
            CultureName = GetCurrentCultureName;
            if (HttpContext.Current.Session.SessionID != null)
            {
                SessionCode = HttpContext.Current.Session.SessionID.ToString();
            }
            if (!IsPostBack)
            {
                UserIp = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(UserIp, out CountryName);
                OverRideSEOInfo(Categorykey, StoreID, PortalID, UserName, CultureName);
                StoreSettingConfig ssc = new StoreSettingConfig();
                NoImageCategoryDetailPath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
                AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);
                NoOfItemsInARow = 3;//int.Parse(ssc.GetStoreSettingsByKey(StoreSetting.NoOfDisplayItems, StoreID, PortalID, CultureName));
                ItemDisplayMode = ssc.GetStoreSettingsByKey(StoreSetting.ItemDisplayMode, StoreID, PortalID, CultureName);
            }

        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            IncludeCss("CategoryDetailcss", "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                "/Templates/" + TemplateName + "/css/JQueryUIFront/jquery-ui.css",
                "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css", 
                "/Templates/" + TemplateName + "/css/JQueryCheckBox/uniform.default.css",
                "/Templates/" + TemplateName + "/css/MessageBox/style.css", "/Templates/" + TemplateName + "/css/CategoryBanner/cycle.css");
            IncludeJs("CategoryDetailjs", "/js/Templating/tmpl.js", "/js/encoder.js", "/js/Paging/jquery.pagination.js",
                "/js/Templating/AspxTemplate.js", "/js/jquery.cycle.min.js", "/js/DateTime/date.js", "/js/MessageBox/jquery.easing.1.3.js",
                      "/js/MessageBox/alertbox.js", "/js/jquery.cookie.js", "/js/jquery.tipsy.js", "/js/SageFrameCorejs/itemTemplateView.js",
                       "/js/Scroll/jquery.tinyscrollbar.min.js", "/js/JQueryCheckBox/jquery.uniform.js");
        }
        IncludeLanguageJS();
        GetAspxTemplates();
        GetAllSubCategoryForFilter();
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

    private void OverRideSEOInfo(string categorykey, int storeID, int portalID, string userName, string cultureName)
    {
        CategorySEOInfo dtCatSEO = GetSEOSettingsByCategoryName(categorykey, storeID, portalID, userName, cultureName);
        if (dtCatSEO != null)
        {
            string Name = dtCatSEO.Name.ToString();
            string PageTitle = dtCatSEO.MetaTitle.ToString();
            string PageKeyWords = dtCatSEO.MetaKeywords.ToString();
            string PageDescription = dtCatSEO.MetaDescription.ToString();

            if (!string.IsNullOrEmpty(PageTitle))
                SEOHelper.RenderTitle(this.Page, PageTitle, false, true, this.GetPortalID);
            else
                SEOHelper.RenderTitle(this.Page, Name, false, true, this.GetPortalID);

            if (!string.IsNullOrEmpty(PageKeyWords))
                SEOHelper.RenderMetaTag(this.Page, "KEYWORDS", PageKeyWords, true);

            if (!string.IsNullOrEmpty(PageDescription))
                SEOHelper.RenderMetaTag(this.Page, "DESCRIPTION", PageDescription, true);
        }
    }

    private CategorySEOInfo GetSEOSettingsByCategoryName(string categorykey, int storeID, int portalID, string userName, string cultureName)
    {
        List<KeyValuePair<string, object>> ParaMeter = new List<KeyValuePair<string, object>>();
        ParaMeter.Add(new KeyValuePair<string, object>("@CatName", categorykey));
        ParaMeter.Add(new KeyValuePair<string, object>("@StoreID", storeID));
        ParaMeter.Add(new KeyValuePair<string, object>("@PortalID", portalID));
        ParaMeter.Add(new KeyValuePair<string, object>("@UserName", userName));
        ParaMeter.Add(new KeyValuePair<string, object>("@CultureName", cultureName));
        SQLHandler sqlH = new SQLHandler();
        return sqlH.ExecuteAsObject<CategorySEOInfo>("usp_Aspx_CategorySEODetailsByCatName", ParaMeter);
    }

    Hashtable hst = null;
    public void GetAllSubCategoryForFilter()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        hst = AppLocalized.getLocale(modulePath);
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        List<CategoryDetailFilter> lstCatDet = AspxFilterController.GetAllSubCategoryForFilter(Categorykey, aspxCommonObj);
        StringBuilder elem = new StringBuilder();
        elem.Append("<div class=\"filter\">");
        if (lstCatDet != null && lstCatDet.Count > 0)
        {
            elem.Append("<div id=\"divCat\" value=\"b01\" class=\"cssClasscategorgy\">");
            elem.Append("<div class=\"divTitle\"><b><label style=\"color:#006699\">" + getLocale("Categories") + "</label></b><img align=\"right\" src=\"" + aspxTemplateFolderPath + "/images/arrow_up.png\"/></div> <div id=\"scrollbar1\" class=\"cssClassScroll\"><div class=\"viewport\"><div class=\"overview\" id=\"catOverview\"><div class=\"divContentb01\"><ul id=\"cat\">");
            foreach (CategoryDetailFilter value in lstCatDet)
            {
                elem.Append("<li><label><input class=\"chkCategory\" type=\"checkbox\" name=\"" + value.CategoryName + "\" ids=\"" + value.CategoryID + "\" value=\"" + value.CategoryName + "\"/> " + value.CategoryName + "<span> (" + value.ItemCount + ")</span></label></li>");
            }
            elem.Append("</ul></div></div></div></div></div>");
        }
        string brandFilter = GetAllBrandForCategory();
        elem.Append(brandFilter);
        elem.Append("</div>");
        ltrFilter.Text = elem.ToString();
    }

    public string GetAllBrandForCategory()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        bool isByCategory = false;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        List<BrandItemsInfo> lstBrandItem = AspxFilterController.GetAllBrandForCategory(Categorykey, isByCategory, aspxCommonObj);
        StringBuilder elem = new StringBuilder();
        StringBuilder scriptToExecute = new StringBuilder();
        List<int> arrBrand = new List<int>();

        if (lstBrandItem.Count > 0)
        {
            elem.Append("<div value=\"0\" class=\"cssClasscategorgy\">");
            elem.Append("<div class=\"divTitle\"><b><label style=\"color:#006699\">" + getLocale("Brands") + "</label></b><img align=\"right\" src=\"" + aspxTemplateFolderPath + "/images/arrow_up.png\" /></div><div id=\"scrollbar2\" class=\"cssClassScroll\"><div class=\"viewport\"><div class=\"overview\"><div class=\"divContent0\"><ul>");
            foreach (BrandItemsInfo value in lstBrandItem)
            {
                if (arrBrand.IndexOf(value.BrandID) == -1)
                {
                    elem.Append("<li><label><input class=\"chkFilter chkBrand\" type=\"checkbox\" name=\"" + value.BrandName + "\" ids=\"" + value.BrandID + "\" value=\"0\"/> " + value.BrandName + "<span id=\"count\"> (" + value.ItemCount + ")</span></label></li>");
                    arrBrand.Add(value.BrandID);
                }
            }
            elem.Append("</ul></div></div></div></div></div>");
            string script = GetStringScript(scriptToExecute.ToString());
            elem.Append(script);
        }
        string shopFilter = GetShoppingFilter();
        elem.Append(shopFilter);
        return elem.ToString();
    }
    public string GetShoppingFilter()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        bool isByCategory = false;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        List<Filter> lstFilter = AspxFilterController.GetShoppingFilter(aspxCommonObj, Categorykey, isByCategory);
        List<string> attrID = new List<string>();
        List<string> attrValue = new List<string>();
        StringBuilder elem = new StringBuilder();
        StringBuilder scriptExecute = new StringBuilder();

        if (lstFilter != null && lstFilter.Count > 0)
        {
            int currentAttributeID = 0;
            scriptExecute.Append("$('#divShopFilter').show();$('.divRange').show();");
            foreach (Filter value in lstFilter)
            {
                IsCategoryHasItems = 1;
                if (Int32.Parse(value.AttributeID) != 8 && Int32.Parse(value.AttributeID) > 48)
                {
                    if (attrID.IndexOf(value.AttributeID) == -1)
                    {
                        attrID.Add(value.AttributeID);
                        if (attrID.IndexOf(value.AttributeID) != -1)
                        {
                            if (Int32.Parse(value.AttributeID) != currentAttributeID && currentAttributeID > 0)
                            {
                                elem.Append("</ul></div></div></div></div></div>");
                            }
                            currentAttributeID = Int32.Parse(value.AttributeID);
                        }
                        elem.Append("<div value=" + value.AttributeID + " class=\"cssClasscategorgy\"><div class=\"divTitle\"><b><label style=\"color:#006699\">" + value.AttributeName + "</label></b><img align=\"right\" src=\"" + aspxTemplateFolderPath + "/images/arrow_up.png\"/></div> <div id=\"scrollbar3\" class=\"cssClassScroll\"><div class=\"viewport\"><div class=\"overview\"><div class=" + "divContent" + value.AttributeID + "><ul>");
                        attrValue = new List<string>();
                        elem.Append("<li><label><input class= \"chkFilter\" type=\"checkbox\" name=\"" + value.AttributeValue + "\" inputTypeID=\"" + value.InputTypeID + "\"  value=\"" + value.AttributeID + "\"/> " + value.AttributeValue + "<span id=\"count\"> (" + value.ItemCount + ")</span></label></li>");
                        attrValue.Add(value.AttributeValue);

                    }
                    else
                    {
                        if (attrID.IndexOf(value.AttributeID) != -1)
                        {
                            if (Int32.Parse(value.AttributeID) != currentAttributeID && currentAttributeID > 0)
                            {
                                elem.Append("</ul></div></div></div></div></div>");
                            }
                            currentAttributeID = Int32.Parse(value.AttributeID);
                        }

                        if (attrValue.IndexOf(value.AttributeValue) == -1)
                        {
                            elem.Append("<li><label><input class=\"chkFilter\" type=\"checkbox\" name=\"" + value.AttributeValue + "\" inputTypeID=\"" + value.InputTypeID + "\"  value=\"" + value.AttributeID + "\"/> " + value.AttributeValue + "<span id=\"count\"> (" + value.ItemCount + ")</span></label></li>");
                            attrValue.Add(value.AttributeValue);
                        }
                    }
                }

                else if (Int32.Parse(value.AttributeID) == 8)
                {
                    arrPrice.Add(value);
                    if (decimal.Parse(value.AttributeValue) > maxPrice)
                    {
                        maxPrice = decimal.Parse(value.AttributeValue);
                    }
                                                                                            }
            }
            if (attrID.Count > 0)
            {
                elem.Append("</ul></div></div></div></div></div>");
            }
            decimal interval = (maxPrice - minPrice) / 4;
            elem.Append("<div value=\"8\" class=\"cssClassbrowseprice\">");
            elem.Append("<div class=\"divTitle\"><b><label style=\"color:#006699\">" + getLocale("Price") + "</label></b><img align=\"right\" src=\"" + aspxTemplateFolderPath + "/images/arrow_up.png\"/></div><div class=\"divContent8\"><ul>");            
            if (arrPrice.Count > 1)
            {
               
                elem.Append(GetPriceData(minPrice, 1, interval) != minPrice.ToString("N2") ? "<li><a id=\"f1\" href=\"#\"  minprice=" + GetPriceData(minPrice, 0, interval) + " maxprice=" + GetPriceData(minPrice, 1, interval) + ">" + "<span class=\"cssClassFormatCurrency\">" + minPrice.ToString("N2") + "</span>" + " - " + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice, 1, interval) + "</span>" + "</a></li>" : "");
                elem.Append(GetPriceData(minPrice, 0, interval) != GetPriceData(minPrice + decimal.Parse("0.01"), 1, interval) && GetPriceData(minPrice, 2, interval) != GetPriceData(minPrice, 1, interval) ? "<li><a id=\"f2\" href=\"#\"  minprice=" + GetPriceData(minPrice + decimal.Parse("0.01"), 1, interval) + " maxprice=" + GetPriceData(minPrice, 2, interval) + ">" + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice + decimal.Parse("0.01"), 1, interval) + "</span>" + " - " + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice, 2, interval) + "</span>" + "</a></li>" : "");
                elem.Append(GetPriceData(minPrice + decimal.Parse("0.01"), 1, interval) != GetPriceData(minPrice + decimal.Parse("0.01"), 2, interval) && GetPriceData(minPrice, 2, interval) != GetPriceData(minPrice, 3, interval) ? "<li><a id=\"f3\" href=\"#\"  minprice=" + GetPriceData(minPrice + decimal.Parse("0.01"), 2, interval) + " maxprice=" + GetPriceData(minPrice, 3, interval) + ">" + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice + decimal.Parse("0.01"), 2, interval) + "</span>" + " - " + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice, 3, interval) + "</span>" + "</a></li>" : "");
                elem.Append(GetPriceData(minPrice, 3, interval) != maxPrice.ToString("N2") ? "<li><a id=\"f4\" href=\"#\"  minprice=" + GetPriceData(minPrice + decimal.Parse("0.01"), 3, interval) + " maxprice=" + maxPrice + ">" + "<span class=\"cssClassFormatCurrency\">" + GetPriceDataFloat(minPrice + decimal.Parse("0.01"), 3, interval) + "</span>" + " - " + "<span class=\"cssClassFormatCurrency\">" + maxPrice.ToString("N2") + "</span>" + "</a></li>" : "");
            }
            if (arrPrice.Count == 1)
            {
                elem.Append("<li><a id=\"f1\" href=\"#\"   minprice=" + GetPriceData(minPrice, 0, interval) + " maxprice=" + GetPriceData(minPrice, 1, interval) + ">" + "<span class=\"cssClassFormatCurrency\">" + minPrice.ToString("N2") + "</span>" + "</a></li>");
                minPrice = 0;
            }

            elem.Append("</ul></div>");
            elem.Append("<div class=\"divRange\"><div id=\"slider-range\"></div></div></div>");
            scriptExecute.Append(
                "$('#amount').html('<span class=\"cssClassFormatCurrency\">" + minPrice.ToString("N2") + "</span>' +' - ' + '<span class=\"cssClassFormatCurrency\">" + maxPrice.ToString("N2") + "</span>');");
                       string script = GetStringScript(scriptExecute.ToString());
            elem.Append(script);
            return elem.ToString();
        }
        return "";
    }

    private string GetStringScript(string codeToRun)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script type=\"text/javascript\">$(document).ready(function(){ setTimeout(function(){ " + codeToRun + "},500); });</script>");
        return script.ToString();
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

    private string GetPriceData(decimal price, int count, decimal interval)
    {
        return ((price + (count * interval)).ToString());
    }
    private string GetPriceDataFloat(decimal price, int count, decimal interval)
    {
        return ((price + (count * interval)).ToString("N2"));
    }
}