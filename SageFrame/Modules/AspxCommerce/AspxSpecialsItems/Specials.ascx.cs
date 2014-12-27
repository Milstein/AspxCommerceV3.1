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
using SageFrame.Web;
using AspxCommerce.Core;
using SageFrame.Core;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using AspxCommerce.Core;
using AspxCommerce.SpecialItems;
using AspxCommerce.ImageResizer;

public partial class Modules_AspxSpecialsItems_Specials : BaseAdministrationUserControl
{
    public int StoreID, PortalID, NoOfSpecialItems;
    public string UserName, CultureName, DefaultImagePath;
    public bool EnableSpecialItems = true;
    public string baseUrl = string.Empty;
    public string SpecialItemRss, RssFeedUrl;
    public int NoOfItemInRow = 2;
    public string AllowAddToCart, AllowOutStockPurchase;
    public string SpecialDetailPage;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                IncludeJs("Specials", "/Modules/AspxCommerce/AspxSpecialsItems/js/Specials.js");
                IncludeCss("SpecialCss", "/Modules/AspxCommerce/AspxSpecialsItems/css/SpecialItems.css");
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                StoreSettingConfig ssc = new StoreSettingConfig();              
                DefaultImagePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);             
                AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
            }
            baseUrl = ResolveUrl("~/");
            IncludeLanguageJS();
            GetSpecialItemSetting();
            if (EnableSpecialItems == true && NoOfSpecialItems > 0)
            {
                GetSpecialItems();
            }
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void GetSpecialItemSetting()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
       
        SpecialItemsController sic = new SpecialItemsController();
        SpecialItemsSettingInfo lstSpecialSetting = sic.GetSpecialItemsSetting(aspxCommonObj);

        if (lstSpecialSetting != null)
        {
            EnableSpecialItems = lstSpecialSetting.IsEnableSpecialItems;
            SpecialItemRss = lstSpecialSetting.IsEnableSpecialItemsRss.ToString();
            SpecialDetailPage = lstSpecialSetting.SpecialItemsDetailPageName;
            RssFeedUrl = lstSpecialSetting.SpecialItemsRssPageName;
            NoOfItemInRow = lstSpecialSetting.NoOfItemInRow;
            NoOfSpecialItems = lstSpecialSetting.NoOfItemShown;
        }
    }

    Hashtable hst = null;
    public void GetSpecialItems()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        string aspxRootPath = ResolveUrl("~/");
        hst = AppLocalized.getLocale(modulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        int rowTotal = 0;
        SpecialItemsController sic = new SpecialItemsController();      
        List<SpecialItemsInfo> lstSpecialItems = sic.GetSpecialItems(aspxCommonObj, NoOfSpecialItems);
        rowTotal = lstSpecialItems.Count;
        if (rowTotal > NoOfSpecialItems)
            lstSpecialItems.RemoveAt(lstSpecialItems.Count - 1);
        StringBuilder specialContent = new StringBuilder();

        if (lstSpecialItems != null && lstSpecialItems.Count > 0)
        {
            specialContent.Append("<div class=\"cssClassSpecialBoxInfo\" id=\"divSpItem\">");
            foreach (SpecialItemsInfo item in lstSpecialItems)
            {
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.ImagePath;
                string altImagePath = "";// "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.AlternateImagePath;
                if (item.ImagePath == "")
                {
                    imagePath = DefaultImagePath;
                }
                else
                {
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(item.ImagePath, ImageType.Medium, ImageCategoryType.Item, aspxCommonObj);
                }
                if (item.AlternateText == "")
                {
                    item.AlternateText = item.Name;
                }
                if (item.ImagePath == "")
                {
                    altImagePath = imagePath;
                }
                string itemPrice = Math.Round(double.Parse((item.Price).ToString()), 2).ToString("N2");
                string itemPriceValue = Math.Round(double.Parse((item.Price).ToString()), 2).ToString();
                string itemPriceRate = Math.Round(double.Parse((item.Price).ToString()), 2).ToString("N2");

                if ((lstSpecialItems.IndexOf(item) + 1) % NoOfItemInRow == 0)
                {
                    specialContent.Append("<div class=\"cssClassProductsBox cssClassNoMargin\">");
                }
                else
                {
                    specialContent.Append("<div class=\"cssClassProductsBox\">");
                }
                var hrefItem = aspxRedirectPath + "item/" + fixedEncodeURIComponent(item.SKU) + pageExtension;
                specialContent.Append("<div id=\"productImageWrapID_");
                specialContent.Append(item.ItemID);
                specialContent.Append("\" class=\"cssClassProductsBoxInfo\" costvariantItem=");
                specialContent.Append(item.CostVariants);
                specialContent.Append("  itemid=\"");
                specialContent.Append(item.ItemID);
                specialContent.Append("\">");
                specialContent.Append("<h3>");
                specialContent.Append(item.SKU);
                specialContent.Append("</h3><div class=\"divQuickLookonHover\"><div class=\"divitemImage cssClassProductPicture\"><a href=\"");
                specialContent.Append(hrefItem);
                specialContent.Append("\" ><img id=\"img_");
                specialContent.Append(item.ItemID);
                specialContent.Append("\"  alt=\"");
                specialContent.Append(item.AlternateText);
                specialContent.Append("\"  title=\"");
                specialContent.Append(item.AlternateText);
                specialContent.Append("\"");
                specialContent.Append("src=\"");
                specialContent.Append(aspxRootPath);
                specialContent.Append(imagePath.Replace("uploads", "uploads/Medium"));
                specialContent.Append("\" orignalPath=\"");
                specialContent.Append(imagePath.Replace("uploads", "uploads/Medium"));
                specialContent.Append("\" altImagePath=\"");
                specialContent.Append(altImagePath.Replace("uploads", "uploads/Medium"));
                specialContent.Append("\"/></a></div>");
                specialContent.Append("<div class='cssLatestItemInfo clearfix'>");
                specialContent.Append("<h2><a href=\"");
                specialContent.Append(hrefItem);
                specialContent.Append("\" title=\"" + item.Name + "\">");

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
                specialContent.Append(name);
                specialContent.Append("</a></h2>");
                StringBuilder dataContent = new StringBuilder();
                dataContent.Append("data-class=\"addtoCart\" data-ItemTypeID=\"" + item.ItemTypeID + "\" data-type=\"button\" data-ItemID=\"" + item.ItemID + "\" data-addtocart=\"");
                dataContent.Append("addtocart");
                dataContent.Append(item.ItemID);
                dataContent.Append("\" data-title=\"");
                dataContent.Append(name);
                dataContent.Append("\" data-onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                dataContent.Append(item.ItemID);
                dataContent.Append(",");
                dataContent.Append(itemPriceValue);
                dataContent.Append(",'");
                dataContent.Append(item.SKU);
                dataContent.Append("',");
                dataContent.Append(1);
                dataContent.Append(",'");
                dataContent.Append(item.CostVariants);
                dataContent.Append("',this);\"");
                if (item.HidePrice != true)
                {
                    if (item.ListPrice != null)
                    {//Added for group type products
                        if (item.ItemTypeID == 5)
                        {
                            specialContent.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            specialContent.Append("<p class=\"cssClassProductRealPrice \">");
                            specialContent.Append(getLocale("Starting At"));
                            specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            specialContent.Append(itemPriceRate);
                            specialContent.Append("</span></p></div></div>");

                        }
                        else
                        {
                            string strAmount = Math.Round((double)(item.ListPrice), 2).ToString("N2");
                            specialContent.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            specialContent.Append("<p class=\"cssClassProductOffPrice\">");
                            specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            specialContent.Append(strAmount);
                            specialContent.Append("</span></p><p class=\"cssClassProductRealPrice \">");
                            specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            specialContent.Append(itemPriceRate);
                            specialContent.Append("</span></p></div></div>");
                        }
                    }
                    else
                    {
                        if (item.ItemTypeID == 5)
                        {
                            specialContent.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            specialContent.Append("<p class=\"cssClassProductRealPrice \" >");
                            specialContent.Append(getLocale("Starting At"));
                            specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            specialContent.Append(itemPriceRate);
                            specialContent.Append("</span></p></div></div>");
                        }
                        else
                        {
                            specialContent.Append("<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\">");
                            specialContent.Append("<p class=\"cssClassProductRealPrice \" >");
                            specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            specialContent.Append(itemPriceRate);
                            specialContent.Append("</span></p></div></div>");
                        }
                    }
                }
                else
                {
                    specialContent.Append("<div class=\"cssClassProductPriceBox\"></div>");
                }

                specialContent.Append("<div class=\"cssClassProductDetail\"><p><a href=\"");
                specialContent.Append(aspxRedirectPath);
                specialContent.Append("item/");
                specialContent.Append(item.SKU);
                specialContent.Append(pageExtension);
                specialContent.Append("\">");
                specialContent.Append(getLocale("Details"));
                specialContent.Append("</a></p></div>");

                specialContent.Append("<div class=\"sfQuickLook\" style=\"display:none\">");
                specialContent.Append("<img itemId=\"");
                specialContent.Append(item.ItemID);
                specialContent.Append("\" sku=\"");
                specialContent.Append(item.SKU);
                specialContent.Append("\" src=\"");
                specialContent.Append(aspxTemplateFolderPath);
                specialContent.Append("/images/QV_Button.png\" alt=\"\" rel=\"popuprel\" />");
                specialContent.Append("</div>");
                if (item.AttributeValues != null)
                {
                    if (item.AttributeValues != "")
                    {
                        specialContent.Append("<div class=\"cssGridDyanamicAttr\">");
                        string[] attributeValues = item.AttributeValues.Split(',');
                        foreach (string element in attributeValues)
                        {
                            string[] attributes = element.Split('#');
                            string attributeName = attributes[0];
                            string attributeValue = attributes[1];
                            int inputType = Int32.Parse(attributes[2]);
                            string validationType = attributes[3];
                            specialContent.Append("<div class=\"cssDynamicAttributes\">");
                            specialContent.Append("<div class=\"cssDynamicAttributes\">");
                            specialContent.Append("<span>");
                            specialContent.Append(attributeName);
                            specialContent.Append("</span> :");
                            if (inputType == 7)
                            {
                                specialContent.Append("<span class=\"cssClassFormatCurrency\">");
                            }
                            else
                            {
                                specialContent.Append("<span>");
                            }
                            specialContent.Append(attributeValue);
                            specialContent.Append("</span></div>");
                        }
                        specialContent.Append("</div>");
                    }
                }
                string itemSKU = item.SKU;
                string itemName = item.Name;

                specialContent.Append("<div class=\"cssClassTMar20\">");
                if (AllowAddToCart.ToLower() == "true")
                {
                    if (AllowOutStockPurchase.ToLower() == "false")
                    {
                        if (item.IsOutOfStock == true)
                        {

                            specialContent.Append("<div class=\"cssClassAddtoCard\"><div " + dataContent + " class=\"sfButtonwrapper cssClassOutOfStock\">");
                            specialContent.Append("<button type=\"button\"><span>");
                            specialContent.Append(getLocale("Out Of Stock"));
                            specialContent.Append("</span></button></div></div>");
                        }
                        else
                        {
                            specialContent.Append("<div class=\"cssClassAddtoCard\"><div " + dataContent + " class=\"sfButtonwrapper\">");
                            specialContent.Append("<label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\"");
                            specialContent.Append("data-addtocart=\"");
                            specialContent.Append("addtocart");
                            specialContent.Append(item.ItemID);
                            specialContent.Append("\" title=\"");
                            specialContent.Append(itemName);
                            specialContent.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                            specialContent.Append(item.ItemID);
                            specialContent.Append(",");
                            specialContent.Append(itemPriceValue);
                            specialContent.Append(",'");
                            specialContent.Append(itemSKU);
                            specialContent.Append("',");
                            specialContent.Append(1);
                            specialContent.Append(",'");
                            specialContent.Append(item.CostVariants);
                            specialContent.Append("',this);\">");
                            specialContent.Append(getLocale("Cart +"));
                            specialContent.Append("</button></label></div></div>");
                        }
                    }
                    else
                    {
                        specialContent.Append("<div class=\"cssClassAddtoCard\"><div " + dataContent + " class=\"sfButtonwrapper\">");
                        specialContent.Append("<label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\"");
                        specialContent.Append("data-addtocart=\"");
                        specialContent.Append("addtocart");
                        specialContent.Append(item.ItemID);
                        specialContent.Append("\" title=\"");
                        specialContent.Append(itemName);
                        specialContent.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                        specialContent.Append(item.ItemID);
                        specialContent.Append(",");
                        specialContent.Append(itemPriceValue);
                        specialContent.Append(",'");
                        specialContent.Append(itemSKU);
                        specialContent.Append("',");
                        specialContent.Append(1);
                        specialContent.Append(",'");
                        specialContent.Append(item.CostVariants);
                        specialContent.Append("',this);\">");
                        specialContent.Append(getLocale("Cart +"));
                        specialContent.Append("</button></label></div></div>");

                    }
                }
                specialContent.Append("<div class=\"cssClassWishListButton\">");
                specialContent.Append("<input type=\"hidden\" name='itemwish' value=");
                specialContent.Append(item.ItemID);
                specialContent.Append(",'");
                specialContent.Append(item.SKU);
                specialContent.Append("',this  />");
                specialContent.Append("</div>");
                specialContent.Append("</div></div>");
                specialContent.Append("</div></div>");
                specialContent.Append("</div>");
            }
           
            specialContent.Append("</div>");
            if (rowTotal > NoOfItemInRow)
            {
                string strHtml = "<a href=\"" + aspxRedirectPath + SpecialDetailPage + pageExtension + "?id=special\">" + getLocale("View More") + "</a>";
                divViewMoreSpecial.InnerHtml = strHtml;
            }
            ltrSpecialItems.Text = specialContent.ToString();
        }
        else
        {
            StringBuilder noSpl = new StringBuilder();
            noSpl.Append("<span class=\"cssClassNotFound\">");
            noSpl.Append(getLocale("No special item found in this store!"));
            noSpl.Append("</span>");
            divSpclBox.InnerHtml = noSpl.ToString();
            divSpclBox.Attributes.Add("class", "");
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
