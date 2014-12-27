using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SageFrame.Security;
using SageFrame.Security.Entities;
using SageFrame.Web;
using System.Collections;
using SageFrame.Framework;
using System.Web.Security;
using SageFrame;
using SageFrame.Web.Utilities;
using SageFrame.Web.Common.SEO;
using AspxCommerce.Core;


public partial class Modules_AspxCommerce_AspxItemDetails_ItemDetails :BaseUserControl
{
    public string itemSKU;
    public int itemID;
    public string itemName;

    public int storeID,
               portalID,
               UserModuleID,
               customerID,
               minimumItemQuantity,
               maximumItemQuantity;

    public bool allowMultipleReviewPerIP, allowMultipleReviewPerUser;
    public string userName, cultureName;
    public string userEmail = string.Empty;
    public string userIP;
    public string countryName = string.Empty;
    public string sessionCode = string.Empty;
    public string aspxfilePath;
    public bool isReviewExistByUser, isReviewExistByIP;
    public string noItemDetailImagePath,
                  enableEmailFriend,
                  AllowAddToCart,
                  allowAnonymousReviewRate,
                  allowOutStockPurchase,
                  AllowRealTimeNotifications;

    public bool IsUseFriendlyUrls = true;
    public string variantQuery = string.Empty;
    public int RatingCount = 0;
    public double AvarageRating = 0.0;
    public int itemTypeId = 0;
    public string ItemPagePath = string.Empty;

    protected void page_init(object sender, EventArgs e)
    {
        aspxfilePath = ResolveUrl("~") + "Modules/AspxCommerce/AspxItemsManagement/";

        try
        {
            SageFrameConfig pagebase = new SageFrameConfig();
            IsUseFriendlyUrls = pagebase.GetSettingBollByKey(SageFrameSettingKeys.UseFriendlyUrls);
            SageFrameRoute parentPage = (SageFrameRoute)this.Page;

            itemSKU = parentPage.Key;
            userIP = HttpContext.Current.Request.UserHostAddress;
            if (!IsPostBack)
            {
                storeID = GetStoreID;
                portalID = GetPortalID;
                customerID = GetCustomerID;
                userName = GetUsername;
                cultureName = GetCurrentCultureName;
                variantQuery = Request.QueryString["varId"];
                if (HttpContext.Current.Session.SessionID != null)
                {
                    sessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
                OverRideSEOInfo(itemSKU, storeID, portalID, userName, cultureName);                
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                
                ipToCountry.GetCountry(userIP, out countryName);
                if (countryName == null)
                    countryName = "";
                SecurityPolicy objSecurity = new SecurityPolicy();
                FormsAuthenticationTicket ticket = objSecurity.GetUserTicket(GetPortalID);
                if (ticket != null && ticket.Name != ApplicationKeys.anonymousUser)
                {
                    MembershipController member = new MembershipController();
                    UserInfo userDetail = member.GetUserDetails(GetPortalID, GetUsername);
                    userEmail = userDetail.Email;
                }

                StoreSettingConfig ssc = new StoreSettingConfig();
                AllowRealTimeNotifications = ssc.GetStoreSettingsByKey(StoreSetting.AllowRealTimeNotifications, storeID, portalID, cultureName);
                noItemDetailImagePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, storeID, portalID,
                                                                  cultureName);
                enableEmailFriend = ssc.GetStoreSettingsByKey(StoreSetting.EnableEmailAFriend, storeID, portalID,
                                                              cultureName);
                allowAnonymousReviewRate =
                    ssc.GetStoreSettingsByKey(StoreSetting.AllowAnonymousUserToWriteItemRatingAndReviews, storeID,
                                              portalID, cultureName);
                allowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, storeID, portalID,
      cultureName);
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, storeID, portalID,
                                                        cultureName);
                allowMultipleReviewPerUser =
                    bool.Parse(ssc.GetStoreSettingsByKey(StoreSetting.AllowMultipleReviewsPerUser, storeID, portalID,
                                                         cultureName));
                allowMultipleReviewPerIP =
                    bool.Parse(ssc.GetStoreSettingsByKey(StoreSetting.AllowMultipleReviewsPerIP, storeID, portalID,
                                                         cultureName));
                ItemPagePath = ResolveUrl("~/Item/");
            }

            if (SageUserModuleID != "")
            {
                UserModuleID = int.Parse(SageUserModuleID);
            }
            else
            {
                UserModuleID = 0;
            }
            IncludeJs("itemdetails", "/js/encoder.js", "/js/StarRating/jquery.rating.pack.js", "/js/StarRating/jquery.MetaData.js", "/js/Paging/jquery.pagination.js");
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void OverRideSEOInfo(string itemSKU, int storeID, int portalID, string userName, string cultureName)
    {
        ItemSEOInfo dtItemSEO = GetSEOSettingsBySKU(itemSKU, storeID, portalID, userName, cultureName);
        if (dtItemSEO != null)
        {
            itemID = int.Parse(dtItemSEO.ItemID.ToString());
            itemName = dtItemSEO.Name.ToString();
            string PageTitle = dtItemSEO.MetaTitle.ToString();
            string PageKeyWords = dtItemSEO.MetaKeywords.ToString();
            string PageDescription = dtItemSEO.MetaDescription.ToString();

            if (!string.IsNullOrEmpty(PageTitle))
                SEOHelper.RenderTitle(this.Page, PageTitle, false, true, this.GetPortalID);
            else
                SEOHelper.RenderTitle(this.Page, itemName, false, true, this.GetPortalID);
            if (!string.IsNullOrEmpty(PageKeyWords))
                SEOHelper.RenderMetaTag(this.Page, "KEYWORDS", PageKeyWords, true);

            if (!string.IsNullOrEmpty(PageDescription))
                SEOHelper.RenderMetaTag(this.Page, "DESCRIPTION", PageDescription, true);
        }
    }

    public ItemSEOInfo GetSEOSettingsBySKU(string itemSKU, int storeID, int portalID, string userName,
                                           string cultureName)
    {
        List<KeyValuePair<string, object>> ParaMeter = new List<KeyValuePair<string, object>>();
        ParaMeter.Add(new KeyValuePair<string, object>("@itemSKU", itemSKU));
        ParaMeter.Add(new KeyValuePair<string, object>("@StoreID", storeID));
        ParaMeter.Add(new KeyValuePair<string, object>("@PortalID", portalID));
        ParaMeter.Add(new KeyValuePair<string, object>("@UserName", userName));
        ParaMeter.Add(new KeyValuePair<string, object>("@CultureName", cultureName));
        SQLHandler sqlH = new SQLHandler();
        return sqlH.ExecuteAsObject<ItemSEOInfo>("usp_Aspx_ItemsSEODetailsBySKU", ParaMeter);
    }

    AspxCommonInfo aspxCommonObj = new AspxCommonInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            IncludeCss("ItemDetails",                      
                       "/Templates/" + TemplateName + "/css/PopUp/style.css",
                       "/Templates/" + TemplateName + "/css/StarRating/jquery.rating.css",
                       "/Templates/" + TemplateName + "/css/JQueryUIFront/jquery-ui.all.css",
                       "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                       "/Templates/" + TemplateName + "/css/FancyDropDown/fancy.css",
                       "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                       "/Templates/" + TemplateName + "/css/PopUp/popbox.css",
					   "/Modules/AspxCommerce/AspxItemDetails/css/module.css"
					   );

            IncludeJs("ItemDetails",
                      "/js/jDownload/jquery.jdownload.js", "/js/MessageBox/alertbox.js", "/js/DateTime/date.js",
                      "/js/PopUp/custom.js", "/js/FormValidation/jquery.validate.js",
                      "/js/StarRating/jquery.rating.js",
                      "/Modules/AspxCommerce/AspxItemDetails/js/ItemDetails.js",
                       "/Modules/AspxCommerce/AspxItemDetails/js/jquery.currencydropdown.js",                     
                      "/js/PopUp/popbox.js",
                      "/js/FancyDropDown/itemFancyDropdown.js", "/js/jquery.tipsy.js",
                      "/js/jquery.labelify.js");
        }
        aspxCommonObj.UserName = GetUsername;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.CustomerID = GetCustomerID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        aspxCommonObj.SessionCode = sessionCode;
        IncludeLanguageJS();
        ItemCommonInfo objItemInfo = AspxCommonController.GetItemInfoFromSKU(itemSKU, aspxCommonObj);
        if (objItemInfo != null)
        {
            itemTypeId = objItemInfo.ItemTypeID;
        }
        BindItemQuantityDiscountByUserName(itemSKU);       
        BindItemAverageRating();
        if (itemTypeId != 5)
        {
            GetPriceHistory();
        }
        BindRatingCriteria();
        AddUpdateRecentlyViewedItem();
        CheckReviewByUser();
        CheckReviewByIP();
    }

    private void BindRatingCriteria()
    {
        List<RatingCriteriaInfo> lstRating = AspxCommonProvider.GetItemRatingCriteria(aspxCommonObj, false);
        if (lstRating != null && lstRating.Count > 0)
        {
            StringBuilder ratingCriteria = new StringBuilder();
            foreach (RatingCriteriaInfo item in lstRating)
            {
                ratingCriteria.Append("<tr><td class='cssClassReviewCriteria'><label class='cssClassLabel'>");
                ratingCriteria.Append(item.ItemRatingCriteria);
                ratingCriteria.Append(":<span class='cssClassRequired'>*</span></label></td><td>");
                ratingCriteria.Append("<input name=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" type='radio' class='auto-submit-star' value='1' title=\"");
                ratingCriteria.Append(getLocale("Worst"));
                ratingCriteria.Append("\" validate='required:true' />");
                ratingCriteria.Append("<input name=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" type='radio' class='auto-submit-star' value='2' title=\"");
                ratingCriteria.Append(getLocale("Bad"));
                ratingCriteria.Append("\"/>");
                ratingCriteria.Append("<input name=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" type='radio' class='auto-submit-star' value='3' title=\"");
                ratingCriteria.Append(getLocale("OK"));
                ratingCriteria.Append("\"/>");
                ratingCriteria.Append("<input name=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" type='radio' class='auto-submit-star' value='4' title=\"");
                ratingCriteria.Append(getLocale("Good"));
                ratingCriteria.Append("\"/>");
                ratingCriteria.Append("<input name=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" type='radio' class='auto-submit-star' value='5' title=\"");
                ratingCriteria.Append(getLocale("Best"));
                ratingCriteria.Append("\"/>");
                ratingCriteria.Append("<span id=\"hover-test");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" class='cssClassRatingText'></span>");
                ratingCriteria.Append("<label for=\"star");
                ratingCriteria.Append(item.ItemRatingCriteriaID);
                ratingCriteria.Append("\" class='error'>");
                ratingCriteria.Append(getLocale("Please rate for") + ' ' + item.ItemRatingCriteria);
                ratingCriteria.Append("</label></td></tr>");
            }
            ltrRatingCriteria.Text = ratingCriteria.ToString();
        }
    }  
    public void BindItemAverageRating()
    {
        int index = 0;
        List<ItemRatingAverageInfo> avgRating = AspxRatingReviewController.GetItemAverageRating(itemSKU, aspxCommonObj);
        StringBuilder ratingBind = new StringBuilder();
        if (avgRating != null && avgRating.Count > 0)
        {
            string script = "$('.cssClassAddYourReview').html('" + getLocale("Write Your Own Review") +
                            "');$('.cssClassItemRatingBox').addClass('cssClassToolTip');";
            string rating = "<div class=\"cssClassToolTipInfo\">",
                starrating = "<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\" id=\"tblAverageRating\">";
            foreach (ItemRatingAverageInfo item in avgRating)
            {
                if (index == 0)
                {
                    string spt = "$('.cssClassTotalReviews').html('" + getLocale("Read Reviews") + "[" +
                                              item.TotalReviewsCount + "]" + "');";
                    RatingCount = item.TotalReviewsCount;
                    AvarageRating = (double)item.TotalRatingAverage;
                    starrating += BindStarRating((double)item.TotalRatingAverage, script, spt);
                }
                index++;
                rating += BindViewDetailsRatingInfo(item.ItemRatingCriteriaID, item.ItemRatingCriteria,
                                            (double)item.RatingCriteriaAverage);
            }
            starrating += "</table>";
            rating += "</div>";
            rating += GetScriptRun("$('input.star').rating();");
            starrating += GetScriptRun(ratingScript);
            ltrRatings.Text = starrating;
            ltrratingDetails.Text = rating;
        }
        else
        {
            ratingBind.Append("<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\" id=\"tblAverageRating\"><tr><td>" + getLocale("Currently there are no reviews") + "</td></tr></table>");
            string script = "$('.cssClassItemRatingBox').removeClass('cssClassToolTip');$('.cssClassSeparator').hide();$('.cssClassAddYourReview').html('" +
                                         getLocale("Be the first to review this item.") + "');";
            ratingBind.Append(GetScriptRun(script));
            ltrRatings.Text = ratingBind.ToString();

        }
    }

    private string ratingScript = "";

    public string BindStarRating(double totalTatingAvg, string spt, string sp)
    {
        StringBuilder ratingStars = new StringBuilder();
        string[] ratingTitle = {
                                   getLocale("Worst"), getLocale("Ugly"), getLocale("Bad"), getLocale("Not Bad"),
                                   getLocale("Average"), getLocale("OK"), getLocale("Nice"), getLocale("Good"),
                                   getLocale("Best"), getLocale("Excellent")
                               }; double[] ratingText = { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };
        int i = 0;
        ratingStars.Append("<tr><td>");
        for (i = 0; i < 10; i++)
        {
            if (totalTatingAvg == ratingText[i])
            {
                ratingStars.Append(
                    "<input name=\"avgItemRating\" type=\"radio\" class=\"star {split:2}\" disabled=\"disabled\" checked=\"checked\" value=" +
                    ratingTitle[i] + " />");
                ratingScript += "$('.cssClassRatingTitle').html('" + ratingTitle[i] + "');";
            }
            else
            {
                ratingStars.Append(
                    "<input name=\"avgItemRating\" type=\"radio\" class=\"star {split:2}\" disabled=\"disabled\" value=" +
                    ratingTitle[i] + " />");
            }
        }
        ratingStars.Append("</td></tr>");
        ratingStars.Append(GetScriptRun(spt + sp));
        return ratingStars.ToString();
    }

    public string BindViewDetailsRatingInfo(int itemRatingCriteriaId, string itemRatingCriteriaNm, double ratingAvg)
    {
        StringBuilder ratingStarsDetailsInfo = new StringBuilder();
        string[] ratingTitle1 = {
                                    getLocale("Worst"), getLocale("Ugly"), getLocale("Bad"), getLocale("Not Bad"),
                                    getLocale("Average"), getLocale("OK"), getLocale("Nice"), getLocale("Good"),
                                    getLocale("Best"), getLocale("Excellent")
                                }; double[] ratingText1 = { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };
        int i = 0;
        ratingStarsDetailsInfo.Append("<div class=\"cssClassToolTipDetailInfo\">");
        ratingStarsDetailsInfo.Append("<span class=\"cssClassCriteriaTitle\">" + itemRatingCriteriaNm + ": </span>");
        for (i = 0; i < 10; i++)
        {
            if (ratingAvg == ratingText1[i])
            {
                ratingStarsDetailsInfo.Append("<input name=\"avgItemDetailRating" + itemRatingCriteriaId +
                                              "\" type=\"radio\" class=\"star {split:2}\" disabled=\"disabled\" checked=\"checked\" value=" +
                                              ratingTitle1[i] + " />");
            }
            else
            {
                ratingStarsDetailsInfo.Append("<input name=\"avgItemDetailRating" + itemRatingCriteriaId +
                                              "\" type=\"radio\" class=\"star {split:2}\" disabled=\"disabled\" value=" +
                                              ratingTitle1[i] + " />");
            }
        }
        ratingStarsDetailsInfo.Append("</div>");
        return ratingStarsDetailsInfo.ToString();
    }

    private class test
    {
        public int key { get; set; }
        public string value { get; set; }
        public string html { get; set; }
    }

    private Hashtable hst = null;

    private void AddUpdateRecentlyViewedItem()
    {
        RecentlyAddedItemInfo addUpdateRecentObj = new RecentlyAddedItemInfo
            {
             SKU= itemSKU,
                IP= userIP,
                CountryName= countryName
                               };
        AspxItemMgntController.AddUpdateRecentlyViewedItems(addUpdateRecentObj, aspxCommonObj);
    }
    private void GetPriceHistory()
    {
        List<PriceHistoryInfo> lstPriceHistory = PriceHistoryController.GetPriceHistory(itemID, aspxCommonObj);
        if (lstPriceHistory != null && lstPriceHistory.Count > 0)
        {
            StringBuilder priceHistory = new StringBuilder();
            priceHistory.Append("<div class='popbox'><a class='open sfLocale' href='javascript:void(0);'>Price History</a><div class='collapse'>");
            priceHistory.Append("<div class='box'><div class='arrow'></div><div class='arrow-border'></div><div class='classPriceHistory'>");
            priceHistory.Append("<table class=classPriceHistoryList><thead><th>Date</th><th>Price</th></thead><tbody>");
            foreach (PriceHistoryInfo item in lstPriceHistory)
            {
                priceHistory.Append("<tr><td><span>");
                priceHistory.Append(item.Date);
                priceHistory.Append("</span></td><td><span class='cssClassFormatCurrency'>");
                priceHistory.Append((item.ConvertedPrice).ToString("N2"));
                priceHistory.Append("</span></td></tr>");
            }
            priceHistory.Append("</tbody></table>");
            priceHistory.Append("</div></div></div></div>");
            ltrPriceHistory.Text = priceHistory.ToString();
        }
        
    }
    public string GetItemTags(string sku)
    {
        string itemTags = string.Empty;
        string tagNames = string.Empty;
        string myTags = string.Empty;
        string userTags = string.Empty;
        StringBuilder bindTag = new StringBuilder();
        List<ItemTagsInfo> lstItemTags = AspxTagsController.GetItemTags(itemSKU, aspxCommonObj);
        foreach (ItemTagsInfo item in lstItemTags)
        {
            if (tagNames.IndexOf(item.Tag) == -1)
            {
                itemTags += item.Tag + "(" + item.TagCount + "), ";
                tagNames += item.Tag;
            }

            if (item.AddedBy == GetUsername)
            {
                if (userTags.IndexOf(item.Tag) == -1)
                {
                    myTags += item.Tag + "<button type=\"button\" class=\"cssClassCross\" value=" + item.ItemTagID +
                              " onclick =ItemDetail.DeleteMyTag(this)><span>" + getLocale("x") + "</span></button>, ";
                    userTags += item.Tag;
                }
            }

            bindTag.Append("$('#divItemTags').html('" + itemTags.Substring(0, itemTags.Length - 2) + "');");
            if (myTags.Length > 2)
            {
                bindTag.Append("$('#divMyTags').html('" + myTags.Substring(0, myTags.Length - 2) + "');");
            }
        }
        string tag = GetScriptRun(bindTag.ToString());
        return tag;
    }

    public string BindDataInTab(string sku, int attrId, int itemTypeId)
    {
        List<AttributeFormInfo> frmItemAttributes = AspxItemMgntController.GetItemDetailsInfoByItemSKU(itemSKU, attrId,
                                                                                                       itemTypeId,
                                                                                                       aspxCommonObj);
        StringBuilder scriptBuilder = new StringBuilder();

        foreach (AttributeFormInfo item in frmItemAttributes)
        {
            string id = item.AttributeID + "_" + item.InputTypeID + "_" + item.ValidationTypeID + "_" + item.IsRequired +
                        "_" + item.GroupID
                        + "_" + item.IsIncludeInPriceRule + "_" + item.IsIncludeInPromotions + "_" + item.DisplayOrder;
            switch (item.InputTypeID)
            {
                case 1:
                    if (item.ValidationTypeID == 3)
                    {
                        if (item.AttributeValues != "")
                        {
                            scriptBuilder.Append(" $('#" + id + "').html('" + Math.Round(decimal.Parse(item.AttributeValues), 2).ToString() + "');");
                        }
                        else
                            scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");

                        break;
                    }
                    else if (item.ValidationTypeID == 5)
                    {
                        scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                        break;
                    }
                    else
                    {

                        scriptBuilder.Append(" $(\"#" + id + "\").html(\"" + item.AttributeValues + "\");");
                        break;
                    }
                case 2:
                    scriptBuilder.Append(" $('#" + id + "').html(Encoder.htmlDecode('" + item.AttributeValues + "'));");
                    break;
                case 3:
                    scriptBuilder.Append(" $('#" + id + "').html('" + Format_The_Date(item.AttributeValues) + "');");
                    break;
                case 4:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 5:
                    scriptBuilder.Append(" $('#" + id + "').append('" + item.AttributeValues + ",');");
                    break;
                case 6:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 7:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 8:
                    scriptBuilder.Append("var div = $('#" + id + "');");
                    scriptBuilder.Append("var filePath = '" + item.AttributeValues + "';");
                    scriptBuilder.Append("var fileName = filePath.substring(filePath.lastIndexOf('/') + 1);");
                    scriptBuilder.Append("if (filePath != '') {");
                    scriptBuilder.Append("var fileExt = (-1 !== filePath.indexOf('.')) ? filePath.replace(/.*[.]/, '') : '';");
                    scriptBuilder.Append("myregexp = new RegExp('(jpg|jpeg|jpe|gif|bmp|png|ico)', 'i');");
                    scriptBuilder.Append("if (myregexp.test(fileExt)) {");
                    scriptBuilder.Append("$(div).append('<span class=\"response\"><img src=' + aspxRootPath + filePath + ' class=\"uploadImage\" /></span>')");
                    scriptBuilder.Append("} else {");

                    scriptBuilder.Append("$(div).append('<span class=\"response\"><span id=\"spanFileUpload\"  class=\"cssClassLink\"  href=' + 'uploads/' + fileName + '>' + fileName + '</span></span>');");
                    scriptBuilder.Append("}");
                    scriptBuilder.Append("}");
                    break;
                case 9:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 10:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 11:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 12:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
                case 13:
                    scriptBuilder.Append(" $('#" + id + "').html('" + item.AttributeValues + "');");
                    break;
            }
        }
        string spt = GetScriptRun(scriptBuilder.ToString());
        return spt;
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
    public string Format_The_Date(string input)
    {
        string dt;
        DateTime strDate = DateTime.Parse(input);
        dt = strDate.ToString("yyyy/MM/dd");//Specify Format you want the O/P as...
        return dt;
    }

    public void BindItemQuantityDiscountByUserName(string sku)
    {
        List<ItemQuantityDiscountInfo> lstIQtyDiscount =
            AspxQtyDiscountMgntController.GetItemQuantityDiscountByUserName(aspxCommonObj, itemSKU);
        StringBuilder QtyDiscount = new StringBuilder();
        if (lstIQtyDiscount != null && lstIQtyDiscount.Count > 0)
        {
            QtyDiscount.Append("<div class=\"cssClassCommonGrid\">");
            QtyDiscount.Append("<p class=\"sfLocale\">Item Quantity Discount:</p>");
            QtyDiscount.Append("<table id=\"itemQtyDiscount\">");
            QtyDiscount.Append("<thead>");
            QtyDiscount.Append(
                "<tr class=\"cssClassHeadeTitle\"><th class=\"sfLocale\">Quantity (more than)</th><th class=\"sfLocale\">Price Per Unit</th></tr>");
            QtyDiscount.Append("</thead><tbody>");
            foreach (ItemQuantityDiscountInfo item in lstIQtyDiscount)
            {
                QtyDiscount.Append("<tr><td>" + Convert.ToInt32(item.Quantity) + "</td><td><span class='cssClassFormatCurrency'>" +
                                   Convert.ToInt32(item.Price).ToString("N2") + "</span></td></tr>");
            }
            QtyDiscount.Append("</tbody></table>");
            QtyDiscount.Append("</div>");
            string script = GetScriptRun("$('.cssClassDwnWrapper').show();");
            litQtyDiscount.Text = QtyDiscount.ToString() + script;
        }
        else
        {
            string script = GetScriptRun("$('#bulkDiscount,#divQtyDiscount').hide();");
            litQtyDiscount.Text = script;
        }
    }

    private void CheckReviewByUser()
    {
        isReviewExistByUser = AspxRatingReviewController.CheckReviewByUser(itemID, aspxCommonObj);
    }

    private void CheckReviewByIP()
    {
        isReviewExistByIP = AspxRatingReviewController.CheckReviewByIP(itemID, aspxCommonObj, userIP);
    }
    private string GetScriptRun(string code)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<script type=\"text/javascript\">$(document).ready(function(){setTimeout(function(){ " + code +
                  "},500); });</script>");
        return sb.ToString();
    }
}