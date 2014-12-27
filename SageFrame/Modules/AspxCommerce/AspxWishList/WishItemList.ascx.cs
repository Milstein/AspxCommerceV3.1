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
using System.Web.Script.Serialization;
using System.Web.UI;
using SageFrame.Security;
using SageFrame.Security.Entities;
using SageFrame.Web;
using System.Web.Security;
using SageFrame.Framework;
using AspxCommerce.Core;
using SageFrame.Core;
using AspxCommerce.WishItem;
using AspxCommerce.ImageResizer;

public partial class WishItemList : BaseAdministrationUserControl
{
    public int StoreID, PortalID, CustomerID, UserModuleIDWishList;
    public string UserName, CultureName;
    public string CountryName = string.Empty;
    public string SessionCode = string.Empty;
    public string UserEmailWishList = string.Empty, ServicePath = string.Empty;
    public string UserIp, NoImageWishList, AllowAddToCart, AllowOutStockPurchase;//ShowImageInWishlist,EnableWishList
    public bool IsUseFriendlyUrls = true;
    public int ArrayLength = 0;
    public int RowTotal = 0;
    public bool ShowImageInWishlist = true;
    public bool EnableWishList = true;

    protected void page_init(object sender, EventArgs e)
    {
        string modulePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory); 
        IncludeLanguageJS();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            ServicePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
            SageFrameConfig pagebase = new SageFrameConfig();
            IsUseFriendlyUrls = pagebase.GetSettingBollByKey(SageFrameSettingKeys.UseFriendlyUrls);
            SecurityPolicy objSecurity = new SecurityPolicy();
            FormsAuthenticationTicket ticket = objSecurity.GetUserTicket(GetPortalID);
            if (ticket != null && ticket.Name != ApplicationKeys.anonymousUser)
            {
                IncludeCss("WishItemList", "/Templates/" + TemplateName + "/css/MessageBox/style.css", "/Templates/" + TemplateName + "/css/PopUp/style.css", "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                    "/Modules/AspxCommerce/AspxWishList/css/WishItems.css");
                IncludeJs("WishItemList", "/js/Paging/jquery.pagination.js", "/js/DateTime/date.js", "/js/MessageBox/jquery.easing.1.3.js",
                          "/js/MessageBox/alertbox.js", "/js/PopUp/custom.js", "/js/jquery.tipsy.js", "/js/encoder.js");
               
                                              StoreID = GetStoreID;
                PortalID = GetPortalID;
                UserName = GetUsername;
                CustomerID = GetCustomerID;
                CultureName = GetCurrentCultureName;
                string userModuleID = SageUserModuleID;
                if (userModuleID != "")
                {
                    UserModuleIDWishList = int.Parse(SageUserModuleID);
                }
                GetWishListItemsSettig();
                if (!IsPostBack)
                {
                    MembershipController member = new MembershipController();
                    UserInfo userDetail = member.GetUserDetails(GetPortalID, GetUsername);
                    UserEmailWishList = userDetail.Email;
                    if (UserEmailWishList.Contains(","))
                    {
                        string[] commaSeparator = { "," };
                        string[] value= UserEmailWishList.Split(commaSeparator,StringSplitOptions.RemoveEmptyEntries);
                        UserEmailWishList = value[0];
                    }
                }
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();
                }

                UserIp = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(UserIp, out CountryName);

                StoreSettingConfig ssc = new StoreSettingConfig();
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
                NoImageWishList = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                                         AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);
                string sortByOptions = ssc.GetStoreSettingsByKey(StoreSetting.SortByOptions, StoreID, PortalID, CultureName);
            }
            else
            {
                if (IsUseFriendlyUrls)
                {
                    if (GetPortalID > 1)
                    {
                        Response.Redirect(ResolveUrl("~/portal/" + GetPortalSEOName + "/" + pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalLoginpage)) + ".aspx?ReturnUrl=" + Request.Url.ToString(), false);
                    }
                    else
                    {
                        Response.Redirect(ResolveUrl("~/" + pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalLoginpage)) + ".aspx?ReturnUrl=" + Request.Url.ToString(), false);
                    }
                }
                else
                {
                    Response.Redirect(ResolveUrl("~/Default.aspx?ptlid=" + GetPortalID + "&ptSEO=" + GetPortalSEOName + "&pgnm=" + pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalLoginpage)) + "?ReturnUrl=" + Request.Url.ToString(), false);
                }
            }
            if (EnableWishList)
            {
                BindWishListItem();
            }
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    Hashtable hst = null;
    private void SortByList()
    {
        StoreSettingConfig ssc = new StoreSettingConfig();
        string sortByOptions = ssc.GetStoreSettingsByKey(StoreSetting.SortByOptions, StoreID, PortalID, CultureName);
        string[] sortByOption = sortByOptions.Substring(0, sortByOptions.LastIndexOf(',')).Split(',');
        StringBuilder wishListSortByBdl = new StringBuilder();
        wishListSortByBdl.Append("<span class=\"sfLocale\">Sort by:</span><select id=\"ddlWishListSortBy\" class=\"sfListmenu\">");
        foreach (string sortByOption1 in sortByOption)
        {
            string[] sortByOptionsplit = sortByOption1.Split('#');
            wishListSortByBdl.Append("<option data-html-text=\"" + sortByOptionsplit[1] + "\" value=" + sortByOptionsplit[0] + ">" + sortByOptionsplit[1] + "</option>");

        }
        wishListSortByBdl.Append("</select>");
        ltrWishListSortBy.Text = wishListSortByBdl.ToString();
    }

    public void BindWishListItem()
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

        var count = 10;
        var isAll = "1";
        int limit = 5;        int offset = 1;
        int sortBy = 1;        WishItemController controller = new WishItemController();
        List<WishItemsInfo> lstWishList = controller.GetWishItemList(offset, limit, aspxCommonObj, isAll, count, sortBy);
        StringBuilder wishListStringBld = new StringBuilder();

        if (lstWishList != null && lstWishList.Count > 0)
        {
            SortByList();
            wishListStringBld.Append("<thead>");
            wishListStringBld.Append("<tr class=\"cssClassCommonCenterBoxTableHeading\">");
            wishListStringBld.Append(
                "<th class=\"cssClassWishItemChkbox\"> <input type=\"checkbox\" id=\"chkHeading\"/></th>");
				if (ShowImageInWishlist)
                    {
				wishListStringBld.Append(
                "<th class=\"cssClassWishItemImg\"> <label class=\"sfLocale\">Image</label></th>");
					}
            wishListStringBld.Append(
                "<th class=\"cssClassWishItemDetails\"><label id=\"lblItem\" class=\"sfLocale\">Item Details and Comment</label></th>");
            wishListStringBld.Append(
                "<th class=\"row-variants\"><label id=\"lblVariant\" class=\"sfLocale\">Variants</label></th>");          
            if (AllowAddToCart.ToLower() == "true")
            {
                wishListStringBld.Append(
                    "<th class=\"cssClassAddToCart\"><span id=\"lblAddToCart\" class=\"sfLocale\">Add To Cart</span></th>");
            }
            wishListStringBld.Append("<th class=\"cssClassDelete\"></th>");
            wishListStringBld.Append("</tr></thead>");
            wishListStringBld.Append("<tbody>");
            ArrayLength = lstWishList.Count;
            foreach (var response in lstWishList)
            {               

                RowTotal = response.RowTotal;
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + response.ImagePath;
                if (response.ImagePath == "")
                {
                    imagePath = NoImageWishList;
                   
                }
                else if (response.AlternateText == "")
                {
                    response.AlternateText = response.ItemName;
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(response.ImagePath, ImageType.Small,ImageCategoryType.Item, aspxCommonObj);
                }
                else
                {
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(response.ImagePath, ImageType.Small,ImageCategoryType.Item, aspxCommonObj);
                }
                JavaScriptSerializer ser = new JavaScriptSerializer();
                string WishDate = (Convert.ToDateTime(response.WishDate)).ToShortDateString();
                               var itemSKU = ser.Serialize(response.SKU);                var cosVaraint = ser.Serialize(response.CostVariantValueIDs);
                               var href = "";
                var cartUrl = "";
                if (response.CostVariantValueIDs == "")
                {
                    cartUrl = "#";
                    href = aspxRedirectPath + "item/" + response.SKU + pageExtension;
                }
                else
                {
                    cartUrl = aspxRedirectPath + "item/" + response.SKU + pageExtension + "?varId=" +
                              response.CostVariantValueIDs + "";
                    href = aspxRedirectPath + "item/" + response.SKU + pageExtension + "?varId=" +
                           response.CostVariantValueIDs + "";
                }
                StringBuilder dataContent = new StringBuilder();
                dataContent.Append("data-class=\"addtoCart\" data-type=\"button\" data-addtocart=\"");
                dataContent.Append("addtocart");
                dataContent.Append(response.ItemID);
                dataContent.Append("\" data-title=\"");
                dataContent.Append(response.ItemName);
                dataContent.Append("\" data-onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                dataContent.Append(response.ItemID);
                dataContent.Append(",");
                dataContent.Append(response.Price);
                dataContent.Append(",'");
                dataContent.Append(response.SKU);
                dataContent.Append("',");
                dataContent.Append(1);
                dataContent.Append(",'");
                dataContent.Append(response.IsCostVariantItem);
                dataContent.Append("',this);\"");
                if (lstWishList.IndexOf(response) % 2 == 0)
                {

                    wishListStringBld.Append("<tr class=\"sfEven\" id=\"tr_" + response.ItemID + "\">");
                    wishListStringBld.Append("<td class=\"cssClassWishItemChkbox\">");
                    wishListStringBld.Append("<input type=\"checkbox\" id=\"" + response.WishItemID +
                                             "\" class=\"cssClassWishItem\"/></td>");					
                    if (ShowImageInWishlist)
                    {
						wishListStringBld.Append("<td class=\"cssClassWishItemImg\">");
                        wishListStringBld.Append("<div class=\"cssClassImage\">");
                        wishListStringBld.Append("<img src=\"" + aspxRootPath +
                                                 imagePath.Replace("uploads", "uploads/Small") +
                                                 "\" alt=\"" + response.AlternateText + "\" title=\"" +
                                                 response.AlternateText + "\"/>");
                        wishListStringBld.Append("</div></td>");
                    }					 
                    wishListStringBld.Append("<td class=\"cssClassWishItemDetails\">");
                    
                    wishListStringBld.Append("<a href=\"" + href + "\">" + response.ItemName + "</a>");
					wishListStringBld.Append("<div class=\"cssClassWishDate\"><i class='i-calender'></i>" + WishDate + "</div>");
					wishListStringBld.Append("<div class=\"cssClassWishComment\">");
                    wishListStringBld.Append("<textarea maxlength=\"600\" onkeyup=\"" +
                                             "WishItem.ismaxlength(this)" + "\" id=\"comment_" +
                                             response.WishItemID + "\" class=\"comment\">" + response.Comment +
                                             "</textarea></div></td>");
                    wishListStringBld.Append(
                        "<td><input type=\"hidden\" name=\"hdnCostVariandValueIDS\" value=" + cosVaraint + "/>");
                    wishListStringBld.Append("<span>" + response.ItemCostVariantValue + "</span></td>");                   
                    if (AllowAddToCart.ToLower() == "true")
                    {
                        if (AllowOutStockPurchase.ToLower() == "false")
                        {
                            if (response.IsOutOfStock != null && (bool)response.IsOutOfStock)
                            {
                                wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                                if (response.ItemTypeID == 5)
                                {
                                    wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">" + getLocale("Starting At "));
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                   decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                                }
                                else
                                {
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                                }
                                 wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\" data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper cssClassOutOfStock\">");
                                wishListStringBld.Append("<span class=\"cssClassOutStock\">" + getLocale("Out Of Stock") +
                                                         "</span></div></td>");
                            }
                            else
                            {
                                wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                                if (response.ItemTypeID == 5)
                                {
                                    wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">"+ getLocale("Starting At "));
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">"  +
                                                   decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                                }
                                else
                                {
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                                }
                              
                                 wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\"data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper\">");
                                wishListStringBld.Append("<label class=\"i-cart cssClassCartLabel cssClassGreenBtn\"><button type=\"button\" class=\"addtoCart\"");
                                wishListStringBld.Append("addtocart=\"");
                                wishListStringBld.Append("addtocart");
                                wishListStringBld.Append(response.ItemID);
                                wishListStringBld.Append("\" title=\"");
                                wishListStringBld.Append(response.ItemName);

                                wishListStringBld.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                                wishListStringBld.Append(response.ItemID + ",");
                                wishListStringBld.Append(response.Price + ",");
                                wishListStringBld.Append("'" + response.SKU + "'" + "," + 1);
                                wishListStringBld.Append(",'");
                                wishListStringBld.Append(response.IsCostVariantItem);
                                wishListStringBld.Append("',this);\"><span>");
                                wishListStringBld.Append(getLocale("Cart +"));
                                wishListStringBld.Append("</span></button></label></div></td>");
                            }
                        }
                        else
                        {
                            wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                            if (response.ItemTypeID == 5)
                            {
                                wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">" + getLocale("Starting At "));
                                wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">"  +
                                               decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                            }
                            else
                            {
                                wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                            }
                            wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\" data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper\">");
                            wishListStringBld.Append("<label class=\"i-cart cssClassCartLabel cssClassGreenBtn\"><button type=\"button\" class=\"addtoCart\"");
                            wishListStringBld.Append("addtocart=\"");
                            wishListStringBld.Append("addtocart");
                            wishListStringBld.Append(response.ItemID);
                            wishListStringBld.Append("\" title=\"");
                            wishListStringBld.Append(response.ItemName);

                            wishListStringBld.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                            wishListStringBld.Append(response.ItemID + ",");
                            wishListStringBld.Append(response.Price + ",");
                            wishListStringBld.Append("'" + response.SKU + "'" + "," + 1);
                            wishListStringBld.Append(",'");
                            wishListStringBld.Append(response.IsCostVariantItem);
                            wishListStringBld.Append("',this);\"><span>");
                            wishListStringBld.Append(getLocale("Cart +"));
                            wishListStringBld.Append("</span></button></label></div></td>");
                        }
                    }
                    wishListStringBld.Append("<td class=\"cssClassDelete\">");
                    wishListStringBld.Append("<a onclick=\"WishItem.Delete(" + response.WishItemID +
                                             ")\"><i class='i-delete'></i></a>");
                    wishListStringBld.Append("</td></tr>");
                }

                else
                {

                    wishListStringBld.Append("<tr class=\"sfOdd\" id=\"tr_" + response.ItemID + "\">");
                    wishListStringBld.Append("<td class=\"cssClassWishItemChkbox\">");
                    wishListStringBld.Append("<input type=\"checkbox\" id=\"" + response.WishItemID +
                                             "\" class=\"cssClassWishItem\"/></td>");
											  if (ShowImageInWishlist)
                    {
						wishListStringBld.Append("<td class=\"cssClassWishItemImg\">");
                        wishListStringBld.Append("<div class=\"cssClassImage\">");
                        wishListStringBld.Append("<img src=\"" + aspxRootPath +
                                                 imagePath.Replace("uploads", "uploads/Small") +
                                                 "\" alt=\"" + response.AlternateText + "\" title=\"" +
                                                 response.AlternateText + "\"/>");
                        wishListStringBld.Append("</div></td>");
                    }	
                    wishListStringBld.Append("<td class=\"cssClassWishItemDetails\">");                   
                    wishListStringBld.Append("<a href=\"" + href + "\">" + response.ItemName + "</a>");
					wishListStringBld.Append("<div class=\"cssClassWishDate\"><i class='i-calender'></i>" + WishDate + "</div>");
					wishListStringBld.Append("<div class=\"cssClassWishComment\">");
                    wishListStringBld.Append("<textarea maxlength=\"600\" onkeyup=\"" +
                                             "WishItem.ismaxlength(this)" + "\" id=\"comment_" +
                                             response.WishItemID + "\" class=\"comment\">" + response.Comment +
                                             "</textarea></div></td>");
                    wishListStringBld.Append(
                        "<td><input type=\"hidden\" name=\"hdnCostVariandValueIDS\" value=" + cosVaraint + "/>");
                    wishListStringBld.Append("<span>" + response.ItemCostVariantValue + "</span></td>");                   
                    if (AllowAddToCart.ToLower() == "true")
                    {
                        if (AllowOutStockPurchase.ToLower() == "false")
                        {
                            if (response.IsOutOfStock != null && (bool)response.IsOutOfStock)
                            {
                                wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                                if (response.ItemTypeID == 5)
                                {
                                    wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">" + getLocale("Starting At "));
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                   decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                                }
                                else
                                {
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                                }
                                 wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\" data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper cssClassOutOfStock\">");
                                wishListStringBld.Append("<span class=\"cssClassOutStock\">" + getLocale("Out Of Stock") +
                                                         "</span></div></td>");
                            }
                            else
                            {
                                wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                                if (response.ItemTypeID == 5)
                                {
                                    wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">" + getLocale("Starting At "));
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                   decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                                }
                                else
                                {
                                    wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                                }
                                wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\" data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper\">");
                                wishListStringBld.Append("<label class=\"i-cart cssClassCartLabel cssClassGreenBtn\"><button type=\"button\" class=\"addtoCart\"");
                                wishListStringBld.Append("addtocart=\"");
                                wishListStringBld.Append("addtocart");
                                wishListStringBld.Append(response.ItemID);
                                wishListStringBld.Append("\" title=\"");
                                wishListStringBld.Append(response.ItemName);

                                wishListStringBld.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                                wishListStringBld.Append(response.ItemID + ",");
                                wishListStringBld.Append(response.Price + ",");
                                wishListStringBld.Append("'" + response.SKU + "'" + "," + 1);
                                wishListStringBld.Append(",'");
                                wishListStringBld.Append(response.IsCostVariantItem);
                                wishListStringBld.Append("',this);\"><span>");
                                wishListStringBld.Append(getLocale("Cart +"));
                                wishListStringBld.Append("</span></button></label></div></td>");
                            }
                        }
                        else
                        {
                            wishListStringBld.Append("<td class=\"cssClassWishToCart\">");
                            if (response.ItemTypeID == 5)
                            {
                                wishListStringBld.Append("<p class=\"cssClassGroupPriceWrapper\">" + getLocale("Starting At "));
                                wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">"+
                                               decimal.Parse(response.Price).ToString("N2") + "</span></p>");
                            }
                            else
                            {
                                wishListStringBld.Append("<span class=\"cssClassPrice cssClassFormatCurrency\">" +
                                                decimal.Parse(response.Price).ToString("N2") + "</span>");
                            }
                            wishListStringBld.Append("<div data-ItemTypeID=\"" + response.ItemTypeID + "\" data-ItemID=\"" + response.ItemID + "\"" + dataContent + " class=\"sfButtonwrapper\">");
                            wishListStringBld.Append("<label class=\"i-cart cssClassCartLabel cssClassGreenBtn\"><button type=\"button\" class=\"addtoCart\"");
                            wishListStringBld.Append("addtocart=\"");
                            wishListStringBld.Append("addtocart");
                            wishListStringBld.Append(response.ItemID);
                            wishListStringBld.Append("\" title=\"");
                            wishListStringBld.Append(response.ItemName);

                            wishListStringBld.Append("\" onclick=\"AspxCommerce.RootFunction.AddToCartFromJS(");
                            wishListStringBld.Append(response.ItemID + ",");
                            wishListStringBld.Append(response.Price + ",");
                            wishListStringBld.Append("'" + response.SKU + "'" + "," + 1);
                            wishListStringBld.Append(",'");
                            wishListStringBld.Append(response.IsCostVariantItem);
                            wishListStringBld.Append("',this);\"><span>");
                            wishListStringBld.Append(getLocale("Cart +"));
                            wishListStringBld.Append("</span></button></label></div></td>");
                        }
                    }
                    wishListStringBld.Append("<td class=\"cssClassDelete\">");
                    wishListStringBld.Append("<a onclick=\"WishItem.Delete(" + response.WishItemID +
                                             ")\"><i class='i-delete'></i></a>");
                    wishListStringBld.Append("</td></tr>");
                }
            }

            wishListStringBld.Append("</tbody>");
            wishListStringBld.Append(GetStringScript("$('.cssClassImage img[title]').tipsy({ gravity: 'n' });"));
            StringBuilder wishLstButtonBdl = new StringBuilder();
            wishLstButtonBdl.Append("<label class='i-wishlist cssClassGreenBtn'><button type=\"button\" id=\"shareWishList\">");
            wishLstButtonBdl.Append("<span class=\"sfLocale\">Share Wishlist</span></button></label>");
            wishLstButtonBdl.Append(
                "<label class='i-update cssClassDarkBtn'><button type=\"button\" id=\"updateWishList\" onclick=\"WishItem.Update();\">");
            wishLstButtonBdl.Append("<span class=\"sfLocale\">Update Selected</span></button></label>");
            wishLstButtonBdl.Append(
                "<label class='i-clear cssClassGreyBtn'><button type=\"button\" id=\"clearWishList\" onclick=\"WishItem.Clear();\">");
            wishLstButtonBdl.Append("<span class=\"sfLocale\">Clear WishList</span></button></label>");
            wishLstButtonBdl.Append("<label class='i-delete cssClassGreenBtn'><button type=\"button\" id=\"btnDeletedMultiple\">");
            wishLstButtonBdl.Append("");
            wishLstButtonBdl.Append("<span class=\"sfLocale\">Delete Selected</span></button></label>");
            wishLstButtonBdl.Append("<label class='i-arrow-right cssClassDarkBtn'><button type=\"button\" id=\"continueInStore\">");
            wishLstButtonBdl.Append("<span class=\"sfLocale\">Continue Shopping</span></button ></label>");

            StringBuilder wishListPaginationBdl = new StringBuilder();
            wishListPaginationBdl.Append("<span class=\"sfLocale\">View Per Page: </span><select id=\"ddlWishListPageSize\" class=\"sfListmenu\"><option value=\"\"></option></select>");
          
            ltrWishListButon.Text = wishLstButtonBdl.ToString();
            ltrWishListPagination.Text = wishListPaginationBdl.ToString();
        }
        else
        {
            wishListStringBld.Append("<tr><td class=\"cssClassNotFound\">" + getLocale("Your wishlist is empty!") + "</td></tr>");
        }
        ltrWishList.Text = wishListStringBld.ToString();
    }
                

    private string GetStringScript(string codeToRun)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script type=\"text/javascript\">$(document).ready(function(){ " +
                      codeToRun + " });</script>");
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
            EnableWishList = objWishItemSetting.IsEnableWishList;
            ShowImageInWishlist = objWishItemSetting.IsEnableImageInWishlist;
        }
    }
}
