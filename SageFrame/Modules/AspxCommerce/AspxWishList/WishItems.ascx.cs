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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SageFrame.Web;
using SageFrame.Framework;
using AspxCommerce.Core;
using SageFrame.Core;
using AspxCommerce.WishItem;
using AspxCommerce.ImageResizer;

public partial class Modules_AspxWishItems_WishItems : BaseAdministrationUserControl
{
    public string UserIp;
    public string CountryName = string.Empty;
    public int StoreID, PortalID, NoOfRecentAddedWishItems;
    public string UserName, CultureName;
    public string NoImageWishItemPath, ShowWishedItemImage, WishListURL;//AllowWishItemList
    public bool IsUseFriendlyUrls = true;
    public bool userFriendlyURL = true;
    public bool AllowWishItemList = true;
    public string ModuleCollapsible;
    protected void page_init(object sender, EventArgs e)
    {
        try
        {
            SageFrameConfig pagebase = new SageFrameConfig();
            IsUseFriendlyUrls = pagebase.GetSettingBollByKey(SageFrameSettingKeys.UseFriendlyUrls);
            UserIp = HttpContext.Current.Request.UserHostAddress;
            IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
            ipToCountry.GetCountry(UserIp, out CountryName);
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                IncludeJs("WishItems", "/js/DateTime/date.js", "/Modules/AspxCommerce/AspxWishList/js/WishItems.js");
                IncludeCss("WishItemsCss", "/Modules/AspxCommerce/AspxWishList/css/WishItems.css");
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                userFriendlyURL = IsUseFriendlyUrls;
                GetWishListItemsSettig();
                StoreSettingConfig ssc = new StoreSettingConfig();
                NoImageWishItemPath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                ShowWishedItemImage = ssc.GetStoreSettingsByKey(StoreSetting.ShowItemImagesInWishList, StoreID, PortalID, CultureName);
                ModuleCollapsible = ssc.GetStoreSettingsByKey(StoreSetting.ModuleCollapsible, StoreID, PortalID, CultureName);
                
            }
            if (AllowWishItemList == true && NoOfRecentAddedWishItems > 0)
            {
                BindMyWishList();
            }
            IncludeLanguageJS();

        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }
    Hashtable hst = null;
    public void BindMyWishList()
    {
        string flagShowAll = "0";       
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
        WishItemController ctrl = new WishItemController();
        List<WishItemsInfo> lstWishItem = ctrl.GetRecentWishItemList(aspxCommonObj, flagShowAll,
                                                                                       NoOfRecentAddedWishItems);
        StringBuilder recentWishList = new StringBuilder();
        recentWishList.Append("<div class=\"cssClassCommonSideBoxTable wishItem\">");
        if (lstWishItem != null && lstWishItem.Count > 0)
        {
            recentWishList.Append("<table class=\"cssClassMyWishItemTable\" id=\"tblWishItem\" width=\"100%\">");            
            string myWishListLink = "";
            if (userFriendlyURL)
            {
                myWishListLink = WishListURL + pageExtension;
            }
            else
            {
                myWishListLink = WishListURL;
            }
            recentWishList.Append("<tbody>");
            for (int i = 0; i < lstWishItem.Count; i++)
            {
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + lstWishItem[i].ImagePath;
                if (string.IsNullOrEmpty(lstWishItem[i].ImagePath))
                {
                    imagePath = NoImageWishItemPath;
                }
                else
                {
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(lstWishItem[i].ImagePath, ImageType.Small, ImageCategoryType.Item, aspxCommonObj);
                }
                if (lstWishItem[i].AlternateText == "")
                {
                    lstWishItem[i].AlternateText = lstWishItem[i].ItemName;
                }
                var href = "";
                if (lstWishItem[i].CostVariantValueIDs == "")
                {
                    href = aspxRedirectPath + "item/" + lstWishItem[i].SKU + pageExtension;
                }
                else
                {
                    href = aspxRedirectPath + "item/" + lstWishItem[i].SKU + pageExtension + "?varId=" +
                           lstWishItem[i].CostVariantValueIDs;
                }
				StringBuilder dataContent = new StringBuilder();
                dataContent.Append("data-class=\"addtoCart\" data-type=\"button\" data-addtocart=\"");
                dataContent.Append("addtocart");
                dataContent.Append(lstWishItem[i].ItemID);
                dataContent.Append("\" data-title=\"");
                dataContent.Append(lstWishItem[i].ItemName);
                dataContent.Append("\" data-onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                dataContent.Append(lstWishItem[i].ItemID);
                dataContent.Append(",");
                dataContent.Append(lstWishItem[i].Price);
                dataContent.Append(",'");
                dataContent.Append(lstWishItem[i].SKU);
                dataContent.Append("',");
                dataContent.Append(1);
                dataContent.Append(",'");
                dataContent.Append(lstWishItem[i].IsCostVariantItem);
                dataContent.Append("',this);\"");
                if (i % 2 == 0)
                {
                    recentWishList.Append("<tr class=\"sfEven\" id=\"trWishItem_" + lstWishItem[i].ItemID +
                                          "\"><td class=\"cssClassWishItemDetails\">");
                    if (ShowWishedItemImage.ToLower() == "true")
                    {
                        recentWishList.Append("<a href =\"" + href + "\">");
                        recentWishList.Append("<div class=\"cssClassImage\"><img src=\"" + aspxRootPath +
                                              imagePath.Replace("uploads", "uploads/Small") + "\" alt=\"" +
                                              lstWishItem[i].AlternateText + "\"  title=\"" +
                                              lstWishItem[i].AlternateText +
                                              "\"/></div></a>");
                    }
                    if (lstWishItem[i].ItemCostVariantValue != "")
                    {
                        recentWishList.Append("<a href=\"" + href + "\">" + lstWishItem[i].ItemName + "(" +
                                              lstWishItem[i].ItemCostVariantValue + ")" + "</a>");
                        recentWishList.Append("</br><span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                              decimal.Parse(lstWishItem[i].Price).ToString("N2") + "</span></td>");
                        recentWishList.Append("<td class=\"cssClassDelete\">");
                        recentWishList.Append(
                            "<img onclick=\"wishItemsFront.DeleteWishListItem(" + lstWishItem[i].WishItemID + ")\" src=\"" +
                            aspxTemplateFolderPath + "/images/admin/btndelete.png\"/>");
                        recentWishList.Append("</td></tr>");
                    }
                    else
                    {
                        recentWishList.Append("<a href=\"" + href + "\">" + lstWishItem[i].ItemName + "</a>");
                        recentWishList.Append("</br><span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                              decimal.Parse(lstWishItem[i].Price).ToString("N2") + "</span></td>");
                        recentWishList.Append("<td class=\"cssClassDelete\">");
                        recentWishList.Append(
                            "<img onclick=\"wishItemsFront.DeleteWishListItem(" + lstWishItem[i].WishItemID + ")\" src=\"" +
                            aspxTemplateFolderPath + "/images/admin/btndelete.png\"/>");
                        recentWishList.Append("</td></tr>");
                    }
                }
                else
                {
                    recentWishList.Append("<tr class=\"sfOdd\" id=\"trWishItem_" + lstWishItem[i].ItemID +
                                          "\"><td class=\"cssClassWishItemDetails\">");
                    if (ShowWishedItemImage.ToLower() == "true")
                    {
                        recentWishList.Append("<a href =\"" + href + "\">");
                        recentWishList.Append("<div class=\"cssClassImage\"><img src=\"" + aspxRootPath +
                                             imagePath.Replace("uploads", "uploads/Small") + "\" alt=\"" +
                                              lstWishItem[i].AlternateText + "\"  title=\"" +
                                              lstWishItem[i].AlternateText + "\"/></div></a>");
                    }
                    if (lstWishItem[i].ItemCostVariantValue != "")
                    {
                        recentWishList.Append("<a href=\"" + href + "\">" + lstWishItem[i].ItemName + "(" +
                                              lstWishItem[i].ItemCostVariantValue + ")" + "</a>");
                        recentWishList.Append("</br><span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                              decimal.Parse(lstWishItem[i].Price).ToString("N2") + "</span></td>");
                        recentWishList.Append("<td class=\"cssClassDelete\">");
                        recentWishList.Append(
                            "<img onclick=\"wishItemsFront.DeleteWishListItem(" + lstWishItem[i].WishItemID + ")\" src=\"" +
                            aspxTemplateFolderPath + "/images/admin/btndelete.png\"/>");
                        recentWishList.Append("</td></tr>");
                    }
                    else
                    {
                        recentWishList.Append("<a href=\"" + href + "\">" + lstWishItem[i].ItemName + "</a>");
                        recentWishList.Append("</br><span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                              decimal.Parse(lstWishItem[i].Price).ToString("N2") + "</span></td>");
                        recentWishList.Append("<td class=\"cssClassDelete\">");
                        recentWishList.Append(
                            "<img onclick=\"wishItemsFront.DeleteWishListItem(" + lstWishItem[i].WishItemID + ")\" src=\"" +
                            aspxTemplateFolderPath + "/images/admin/btndelete.png\"/>");
                        recentWishList.Append("</td></tr>");
                    }
                }
            }
            recentWishList.Append("</tbody>");
            recentWishList.Append("</table>");
            recentWishList.Append("<div class=\"cssClassWishLink\">");
            recentWishList.Append("<a href=\"" + aspxRedirectPath + myWishListLink + "\" id=\"lnkGoToWishlist\">");
            recentWishList.Append("<span class=\"gowishlist\">" + getLocale("Go to Wishlist") + "</span></a>");
            recentWishList.Append("</div></div>");
        }
        else
        {
            recentWishList.Append("<tbody>");
            recentWishList.Append("<tr><td><span class=\"cssClassNotFound\">" + getLocale("Your Wishlist is empty!") +
                                  "</span></td></tr>");
            recentWishList.Append("</tbody>");
            recentWishList.Append("</table></div>");
        }
        
        ltrWishItem.Text = recentWishList.ToString();
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

    private void GetWishListItemsSettig()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
        WishItemController wic = new WishItemController();
        WishItemsSettingInfo objWishItemSetting = wic.GetWishItemsSetting(aspxCommonObj);
        if (objWishItemSetting != null)
        {
            AllowWishItemList = objWishItemSetting.IsEnableWishList;
            NoOfRecentAddedWishItems = objWishItemSetting.NoOfRecentAddedWishItems;
            WishListURL = objWishItemSetting.WishListPageName;
        }
    }
}