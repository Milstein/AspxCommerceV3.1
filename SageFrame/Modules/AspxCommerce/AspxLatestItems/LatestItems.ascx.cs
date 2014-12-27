/*
AspxCommerce® - http://www.aspxcommerce.com
Copyright (c) 2011-2014 by AspxCommerce

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OF OTHER DEALINGS IN THE SOFTWARE. 
*/



using System;
using System.Web;
using System.Web.UI;
using SageFrame.Web;
using SageFrame.Framework;
using AspxCommerce.Core;
using SageFrame.Core;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using AspxCommerce.LatestItems;
using AspxCommerce.ImageResizer;

public partial class Modules_AspxLatestItems_LatestItems : BaseAdministrationUserControl
{
    public string UserIp;
    public string CountryName = string.Empty;
    public string SessionCode = string.Empty;
    public int StoreID, PortalID, CustomerID, UserModuleID;
    public string UserName, CultureName;
    public string aspxfilePath;
    public bool EnableLatestItems,EnableLatestItemRss;
    public string DefaultImagePath,  AllowAddToCart, AllowOutStockPurchase, AllowWishListLatestItem, AllowAddToCompareLatest;
    public int NoOfLatestItems, NoOfLatestItemsInARow, MaxCompareItemCount;
    public string  RssFeedUrl, LatestItemModPath;

    protected void Page_Init(object sender, EventArgs e)
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {

                IncludeCss("LatestItems", "/Templates/" + TemplateName + "/css/MessageBox/style.css", "/Templates/" + TemplateName + "/css/ImageGallery/styles.css",
                       "/Templates/" + TemplateName + "/css/PopUp/style.css",
                       "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                       "/Templates/" + TemplateName + "/css/FancyDropDown/fancy.css",
                       "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css", "/Templates/" + TemplateName + "/css/PopUp/popbox.css",
                        "/Modules/AspxCommerce/AspxLatestItems/css/module.css");
                IncludeJs("LatestItems", "/js/DateTime/date.js", "/js/jquery.easing.1.3.js", "/Modules/AspxCommerce/AspxLatestItems/js/LatestItems.js"
                    , "/js/jquery.tipsy.js", "/js/ImageGallery/jquery.pikachoose.js", "/js/ImageGallery/jquery.jcarousel.js", "/js/ImageGallery/jquery.mousewheel.js",
                                "/js/jDownload/jquery.jdownload.js", "/js/MessageBox/alertbox.js", "/js/PopUp/custom.js", "/js/PopUp/popbox.js" );
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                CustomerID = GetCustomerID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                aspxfilePath = ResolveUrl("~") + "Modules/AspxCommerce/AspxItemsManagement/";
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();

                }
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
                if (SageUserModuleID != "")
                {
                    UserModuleID = int.Parse(SageUserModuleID);
                }
                else
                {
                    UserModuleID = 0;
                }
                AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
                aspxCommonObj.StoreID = StoreID;
                aspxCommonObj.PortalID = PortalID;
                aspxCommonObj.UserName = UserName;
                aspxCommonObj.CultureName = CultureName;
                UserIp = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(UserIp, out CountryName);
                StoreSettingConfig ssc = new StoreSettingConfig();
                DefaultImagePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
                AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);                            
                LatestItemModPath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
                GetLatestItemSetting();
            }
            IncludeLanguageJS();
            if (EnableLatestItems && NoOfLatestItems > 0)
            {
                GetLatestItemsByCount();
            }
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void GetLatestItemSetting()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        LatestItemSettingInfo objLatestSetting = new LatestItemSettingInfo();
        AspxLatestItemsController objLatestItem = new AspxLatestItemsController();
        objLatestSetting = objLatestItem.GetLatestItemSetting(aspxCommonObj);
        if (objLatestSetting != null)
        {
            EnableLatestItems = objLatestSetting.IsEnableLatestItem;
            NoOfLatestItems = objLatestSetting.LatestItemCount;
            EnableLatestItemRss = objLatestSetting.IsEnableLatestRss;
            NoOfLatestItemsInARow = objLatestSetting.LatestItemInARow;
            RssFeedUrl = objLatestSetting.LatestItemRssPage;

        }
    }

    Hashtable hst = null;
    private void GetLatestItemsByCount()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        AspxLatestItemsController objLatestItems = new AspxLatestItemsController();
        List<LatestItemsInfo> latestItemsInfo = objLatestItems.GetLatestItemsByCount(aspxCommonObj, NoOfLatestItems);
        StringBuilder RecentItemContents = new StringBuilder();
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        hst = AppLocalized.getLocale(modulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        string aspxRootPath = ResolveUrl("~/");
        if (latestItemsInfo != null && latestItemsInfo.Count > 0)
        {
            foreach (LatestItemsInfo item in latestItemsInfo)
            {
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.ImagePath;
                string altImagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.AlternateImagePath;
                if (item.ImagePath == "")
                {
                    imagePath = DefaultImagePath;
                    
                }
                else
                {   //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(item.ImagePath, ImageType.Medium,ImageCategoryType.Item, aspxCommonObj);
                }
                if (item.AlternateText == "")
                {
                    item.AlternateText = item.Name;
                }
                if (item.AlternateImagePath == "")
                {
                    altImagePath = imagePath;
                }
                string itemPrice = Math.Round(double.Parse((item.Price).ToString()), 2).ToString("N2");
                string itemPriceValue = Math.Round(double.Parse((item.Price).ToString()), 2).ToString();
                string itemPriceRate = Math.Round(double.Parse((item.Price).ToString()), 2).ToString("N2");

                if ((latestItemsInfo.IndexOf(item) + 1) % NoOfLatestItemsInARow == 0)
                {
                    RecentItemContents.Append("<div class=\"cssClassProductsBox cssClassNoMargin\">");
                }
                else
                {
                    RecentItemContents.Append("<div class=\"cssClassProductsBox\">");
                }
                var hrefItem = aspxRedirectPath + "item/" + fixedEncodeURIComponent(item.SKU) + pageExtension;
                RecentItemContents.Append("<div id=\"productImageWrapID_");
                RecentItemContents.Append(item.ItemID);
                RecentItemContents.Append("\" class=\"cssClassProductsBoxInfo\" costvariantItem=");
                RecentItemContents.Append(item.IsCostVariantItem);
                RecentItemContents.Append("  itemid=\"");
                RecentItemContents.Append(item.ItemID);
                RecentItemContents.Append("\">");
                RecentItemContents.Append("<h3>");
                RecentItemContents.Append(item.SKU);
                RecentItemContents.Append("</h3><div><div class=\"divitemImage cssClassProductPicture\"><a href=\"");
                RecentItemContents.Append(hrefItem);
                RecentItemContents.Append("\" ><img id=\"img_");
                RecentItemContents.Append(item.ItemID);
                RecentItemContents.Append("\"  alt=\"");
                RecentItemContents.Append(item.AlternateText);
                RecentItemContents.Append("\"  title=\"");
                RecentItemContents.Append(item.AlternateText);
                RecentItemContents.Append("\"");
                RecentItemContents.Append("src=\"");
                RecentItemContents.Append(aspxRootPath);
                RecentItemContents.Append(imagePath.Replace("uploads", "uploads/Medium"));
                RecentItemContents.Append("\" orignalPath=\"");
                RecentItemContents.Append(imagePath.Replace("uploads", "uploads/Medium"));
                RecentItemContents.Append("\" altImagePath=\"");
                RecentItemContents.Append(altImagePath.Replace("uploads", "uploads/Medium"));
               
                RecentItemContents.Append("\"/></a></div>");
                RecentItemContents.Append("<div class='cssLatestItemInfo clearfix'>");
                RecentItemContents.Append("<h2><a href=\"");
                RecentItemContents.Append(hrefItem);
                RecentItemContents.Append("\" title=\"" + item.Name + "\">");
                string name = string.Empty;
                if (item.Name.Length > 50)
                {
                    name = item.Name.Substring(0, 50);
                    int index = 0;
                    index = name.LastIndexOf(' ');
                    name = name.Substring(0, index);
                    name = name + "...";
                }
                else
                {
                    name = item.Name;
                }
                RecentItemContents.Append(name);
                RecentItemContents.Append("</a></h2>");
                if (item.HidePrice != true)
                {
                    if (item.ListPrice != null)
                    {                          if (item.ItemTypeID == 5)
                        {
                            RecentItemContents.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            RecentItemContents.Append("<p class=\"cssClassProductRealPrice \">");
                            RecentItemContents.Append(getLocale("Starting At "));
                            RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            RecentItemContents.Append(itemPriceRate);
                            RecentItemContents.Append("</span></p></div></div>");

                        }
                        else
                        {
                            string strAmount = Math.Round((double)(item.ListPrice), 2).ToString("N2");
                            RecentItemContents.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            RecentItemContents.Append("<p class=\"cssClassProductOffPrice\">");
                            RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            RecentItemContents.Append(strAmount);
                            RecentItemContents.Append("</span></p><p class=\"cssClassProductRealPrice \">");
                            RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            RecentItemContents.Append(itemPriceRate);
                            RecentItemContents.Append("</span></p></div></div>");
                        }
                    }
                    else
                    {                          if (item.ItemTypeID == 5)
                        {
                            RecentItemContents.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            RecentItemContents.Append("<p class=\"cssClassProductRealPrice \">");
                            RecentItemContents.Append(getLocale("Starting At "));
                            RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            RecentItemContents.Append(itemPriceRate);
                            RecentItemContents.Append("</span></p></div></div>");
                        }
                        else
                        {
                            RecentItemContents.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            RecentItemContents.Append("<p class=\"cssClassProductRealPrice \">");
                            RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            RecentItemContents.Append(itemPriceRate);
                            RecentItemContents.Append("</span></p></div></div>");
                        }
                    }
                }
                else
                {
                    RecentItemContents.Append("<div class=\"cssClassProductPriceBox\"></div>");
                }
                RecentItemContents.Append("<div class=\"cssClassProductDetail\"><p><a href=\"");
                RecentItemContents.Append(aspxRedirectPath);
                RecentItemContents.Append("item/");
                RecentItemContents.Append(item.SKU);
                RecentItemContents.Append(pageExtension);
                RecentItemContents.Append("\">");
                RecentItemContents.Append(getLocale("Details"));
                RecentItemContents.Append("</a></p></div>");
                RecentItemContents.Append("<div class=\"sfQuickLook\" style=\"display:none\">");
                RecentItemContents.Append("<img itemId=\"");
                RecentItemContents.Append(item.ItemID);
                RecentItemContents.Append("\" sku=\"");
                RecentItemContents.Append(item.SKU);
                RecentItemContents.Append("\" src=\"");
                RecentItemContents.Append(aspxTemplateFolderPath);
                RecentItemContents.Append("/images/QV_Button.png\" alt=\"\" rel=\"popuprel\" />");
                RecentItemContents.Append("</div>");
                if (item.AttributeValues != null)
                {
                    if (item.AttributeValues != "")
                    {
                        RecentItemContents.Append("<div class=\"cssGridDyanamicAttr\">");
                        string[] attributeValues = item.AttributeValues.Split(',');
                        foreach (string element in attributeValues)
                        {
                            string[] attributes = element.Split('#');
                            string attributeName = attributes[0];
                            string attributeValue = attributes[1];
                            int inputType = Int32.Parse(attributes[2]);
                            string validationType = attributes[3];
                            RecentItemContents.Append("<div class=\"cssDynamicAttributes\">");
                            RecentItemContents.Append("<span>");
                            RecentItemContents.Append(attributeName);
                            RecentItemContents.Append("</span> :");
                            if (inputType == 7)
                            {
                                RecentItemContents.Append("<span class=\"cssClassFormatCurrency\">");
                            }
                            else
                            {
                                RecentItemContents.Append("<span>");
                            }
                            RecentItemContents.Append(attributeValue);
                            RecentItemContents.Append("</span></div>");
                        }
                        RecentItemContents.Append("</div>");
                    }
                }
                string itemSKU = item.SKU;
                string itemName = item.Name;
                StringBuilder dataContent = new StringBuilder();
                dataContent.Append("data-class=\"addtoCart\" data-type=\"button\" data-addtocart=\"");
                dataContent.Append("addtocart");
                dataContent.Append(item.ItemID);
                dataContent.Append("\" data-title=\"");
                dataContent.Append(itemName);
                dataContent.Append("\" data-onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                dataContent.Append(item.ItemID);
                dataContent.Append(",");
                dataContent.Append(itemPriceValue);
                dataContent.Append(",'");
                dataContent.Append(itemSKU);
                dataContent.Append("',");
                dataContent.Append(1);
                dataContent.Append(",'");
                dataContent.Append(item.IsCostVariantItem);
                dataContent.Append("',this);\"");
				RecentItemContents.Append("<div class=\"cssClassTMar20 clearfix\">");
                if (AllowAddToCart.ToLower() == "true")
                {
                    if (AllowOutStockPurchase.ToLower() == "false")
                    {
                        if (item.IsOutOfStock == true)
                        {

                            RecentItemContents.Append("<div class=\"cssClassAddtoCard\"><div class=\"sfButtonwrapper cssClassOutOfStock\"  data-ItemTypeID=\"" + item.ItemTypeID + "\" data-ItemID=\"" + item.ItemID + "\" " + dataContent + ">");
                            RecentItemContents.Append("<button type=\"button\"><span>");
                            RecentItemContents.Append(getLocale("Out Of Stock"));
                            RecentItemContents.Append("</span></button></div></div>");
                        }
                        else
                        {
                            RecentItemContents.Append("<div class=\"cssClassAddtoCard\"><div data-ItemTypeID=\"" + item.ItemTypeID + "\" data-ItemID=\"" + item.ItemID + "\" " + dataContent + " class=\"sfButtonwrapper\">");
                            RecentItemContents.Append("<label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\"");
                            RecentItemContents.Append("addtocart=\"");
                            RecentItemContents.Append("addtocart");
                            RecentItemContents.Append(item.ItemID);
                            RecentItemContents.Append("\" title=\"");
                            RecentItemContents.Append(itemName);
                            RecentItemContents.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                            RecentItemContents.Append(item.ItemID);
                            RecentItemContents.Append(",");
                            RecentItemContents.Append(itemPriceValue);
                            RecentItemContents.Append(",'");
                            RecentItemContents.Append(itemSKU);
                            RecentItemContents.Append("',");
                            RecentItemContents.Append(1);
                            RecentItemContents.Append(",'");
                            RecentItemContents.Append(item.IsCostVariantItem);
                            RecentItemContents.Append("',this);\">");
                            RecentItemContents.Append(getLocale("Cart +"));
                            RecentItemContents.Append("</button></label></div></div>");
                        }
                    }
                    else
                    {
                        RecentItemContents.Append("<div class=\"cssClassAddtoCard\"><div data-ItemTypeID=\"" + item.ItemTypeID + "\" data-ItemID=\"" + item.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper\">");
                        RecentItemContents.Append("<label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\"");
                        RecentItemContents.Append("addtocart=\"");
                        RecentItemContents.Append("addtocart");
                        RecentItemContents.Append(item.ItemID);
                        RecentItemContents.Append("\" title=\"");
                        RecentItemContents.Append(itemName);
                        RecentItemContents.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                        RecentItemContents.Append(item.ItemID);
                        RecentItemContents.Append(",");
                        RecentItemContents.Append(itemPriceValue);
                        RecentItemContents.Append(",'");
                        RecentItemContents.Append(itemSKU);
                        RecentItemContents.Append("',");
                        RecentItemContents.Append(1);
                        RecentItemContents.Append(",'");
                        RecentItemContents.Append(item.IsCostVariantItem);
                        RecentItemContents.Append("',this);\">");
                        RecentItemContents.Append(getLocale("Cart +"));
                        RecentItemContents.Append("</button></label></div></div>");

                    }
                }
                RecentItemContents.Append("<div class=\"cssClassWishListButton\">");
                RecentItemContents.Append("<input type=\"hidden\" name='itemwish' value=");
                RecentItemContents.Append(item.ItemID);
                RecentItemContents.Append(",'");
                RecentItemContents.Append(item.SKU);
                RecentItemContents.Append("',this  />");
                RecentItemContents.Append("</div>");                
                RecentItemContents.Append("</div></div>"); 
                RecentItemContents.Append("</div></div>");
                RecentItemContents.Append("</div>");
            }
        }

        else
        {
            RecentItemContents.Append("<span class=\"cssClassNotFound\">");
            RecentItemContents.Append(getLocale("This store has no items listed yet!"));
            RecentItemContents.Append("</span>");
        }
        tblRecentItems.InnerHtml = RecentItemContents.ToString();
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
