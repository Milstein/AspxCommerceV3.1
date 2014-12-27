using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
using SageFrame.Core;
using System.Web.UI.HtmlControls;

public partial class Modules_AspxDetails__AspxItemDetails_ItemDetails : BaseAdministrationUserControl
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
                userIP = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(userIP, out countryName);
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
            IncludeCss("ItemDetails", "/Templates/" + TemplateName + "/css/ImageGallery/Slider.css",
                       "/Templates/" + TemplateName + "/css/ImageGallery/multizoom.css",
                       "/Templates/" + TemplateName + "/css/PopUp/style.css",
                       "/Templates/" + TemplateName + "/css/StarRating/jquery.rating.css",
                       "/Templates/" + TemplateName + "/css/JQueryUIFront/jquery-ui.all.css",
                       "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                       "/Templates/" + TemplateName + "/css/FancyDropDown/fancy.css",
                       "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                       "/Templates/" + TemplateName + "/css/Scroll/scrollbars.css",
                       "/Templates/" + TemplateName + "/css/ResponsiveTab/responsive-tabs.css",
                       "/Templates/" + TemplateName + "/css/PopUp/popbox.css");

            IncludeJs("ItemDetails", "/js/ImageGallery/jquery.jcarousel.js", "/js/ImageGallery/jquery.mousewheel.js",

                      "/js/jDownload/jquery.jdownload.js", "/js/MessageBox/alertbox.js", "/js/DateTime/date.js",
                      "/js/PopUp/custom.js", "/js/FormValidation/jquery.validate.js",
                      "/js/StarRating/jquery.rating.js",
                      "/Modules/AspxCommerce/DetailsBrowse/js/ItemDetails.js",
                       "/Modules/AspxCommerce/DetailsBrowse/js/jquery.currencydropdown.js",                     
                      "/js/ResponsiveTab/responsiveTabs.js",
                      "/js/PopUp/popbox.js",
                      "/js/FancyDropDown/itemFancyDropdown.js", "/js/jquery.tipsy.js", "/js/Scroll/mwheelIntent.js",
                      "/js/Scroll/jScrollPane.js", "/js/ImageGallery/multizoom.js",
                      "/js/VideoGallery/jquery.youtubepopup.min.js", "/js/jquery.labelify.js");
        }
        aspxCommonObj.UserName = GetUsername;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.CustomerID = GetCustomerID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        aspxCommonObj.SessionCode = sessionCode;

        IncludeLanguageJS();
        BindItemQuantityDiscountByUserName(itemSKU);
        GetFormFieldList(itemSKU);
        BindItemAverageRating();

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
                               };        double[] ratingText = { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };
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
                                };        double[] ratingText1 = { 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5 };
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

    public void GetFormFieldList(string itemSKU)
    {
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        string aspxRootPath = ResolveUrl("~/");
        hst = AppLocalized.getLocale(modulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        List<test> arrList = new List<test>();
        int attributeSetId = 0;
        int index = 0;
        List<AttributeFormInfo> frmItemFieldList = AspxItemMgntController.GetItemFormAttributesByItemSKUOnly(itemSKU,
                                                                                                               aspxCommonObj);
        StringBuilder dynHtml = new StringBuilder();
        foreach (AttributeFormInfo item in frmItemFieldList)
        {
            bool isGroupExist = false;
            dynHtml = new StringBuilder();
           
            if (index == 0)
            {
                attributeSetId = (int)item.AttributeSetID;
                itemTypeId = (int)item.ItemTypeID;
            }
            index++;
            test t = new test();
            t.key = (int)item.GroupID;
            t.value = item.GroupName;
            t.html = "";
            foreach (test tt in arrList)
            {
                if (tt.key == item.GroupID)
                {
                    isGroupExist = true;
                    break;
                }
            }
            if (!isGroupExist)
            {
                if ((item.ItemTypeID == 2 || item.ItemTypeID == 3) && item.GroupID == 11)
                {
                }
                else
                {
                    arrList.Add(t);
                }
            }
            StringBuilder tr = new StringBuilder();
            if ((item.ItemTypeID == 2 || item.ItemTypeID == 3) && item.AttributeID == 32 && item.AttributeID == 33 && item.AttributeID == 34)
            {
            }
            else
            {
                tr.Append("<tr><td class=\"cssClassTableLeftCol\"><label class=\"cssClassLabel\">" + item.AttributeName +
                          ": </label></td>");
                tr.Append("<td><div id=\"" + item.AttributeID + "_" + item.InputTypeID + "_" + item.ValidationTypeID +
                          "_" + item.IsRequired + "_" + item.GroupID + "_" + item.IsIncludeInPriceRule + "_" +
                          item.IsIncludeInPromotions + "_" + item.DisplayOrder + "\" name=\"" + item.AttributeID + "_" +
                          item.InputTypeID + "_" + item.ValidationTypeID + "_" + item.IsRequired + "_" +
                          item.GroupID + "_" + item.IsIncludeInPriceRule + "_" + item.IsIncludeInPromotions +
                          "_" + item.DisplayOrder + "\" title=\"" + item.ToolTip + "\">");
                tr.Append("</div></td>");
                tr.Append("</tr>");
            }
            foreach (test ttt in arrList)
            {
                if (ttt.key == item.GroupID)
                {
                    ttt.html += tr;
                }

            }

            StringBuilder itemTabs = new StringBuilder();
                       dynHtml.Append("<div id=\"dynItemDetailsForm\" class=\"sfFormwrapper\" style=\"display:none\">");
            dynHtml.Append("<div class=\"cssClassTabPanelTable\">");
            dynHtml.Append(
                "<div id=\"ItemDetails_TabContainer\" class=\"responsive-tabs\">");
                       for (var i = 0; i < arrList.Count; i++)
            {
                itemTabs.Append("<h2><span>" + arrList[i].value +
                                "</span></a></h2>");
                
                itemTabs.Append("<div id=\"ItemTab-" + arrList[i].key +
                               "\"><div><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                               arrList[i].html + "</table></div></div>");
            }
            itemTabs.Append("<h2><span>" + getLocale("Tags") + "</span></h2>");
            StringBuilder itemTagsBody = new StringBuilder();
            itemTagsBody.Append("<div class=\"cssClassPopularItemTags\"><h2>" + getLocale("Popular Tags:") +
                                "</h2><div id=\"divItemTags\" class=\"cssClassPopular-Itemstags\"></div>");

            if (GetCustomerID > 0 && GetUsername.ToLower() != "anonymoususer")
            {
                itemTagsBody.Append("<h2>" + getLocale("My Tags:") +
                                    "</h2><div id=\"divMyTags\" class=\"cssClassMyTags\"></div>");
                itemTagsBody.Append("<table id=\"AddTagTable\"><tr><td>");
                itemTagsBody.Append("<input type=\"text\" class=\"classTag\" maxlength=\"20\"/>");
                itemTagsBody.Append("<button class=\"cssClassDecrease\" type=\"button\"><span>-</span></button>");
                itemTagsBody.Append("<button class=\"cssClassIncrease\" type=\"button\"><span>+</span></button>");
                itemTagsBody.Append("</td></tr></table>");
                itemTagsBody.Append(
                    "<div class=\"sfButtonwrapper\"><button type=\"button\" id=\"btnTagSubmit\"><span>" +
                    getLocale("+ Tag") + "</span></button></div>");
                           }
            else
            {
                SageFrameConfig sfConfig = new SageFrameConfig();
                itemTagsBody.Append("<a href=\"" + aspxRedirectPath + sfConfig.GetSettingsByKey(SageFrameSettingKeys.PortalLoginpage) + pageExtension + "?ReturnUrl=" +
                                    aspxRedirectPath + "item/" + itemSKU + pageExtension +
                                    "\" class=\"cssClassLogIn\"><span>" +
                                    getLocale("Sign in to enter tags") + "</span></a>");
            }
            itemTagsBody.Append("</div>");
            itemTabs.Append(
                "<div  id=\"ItemTab-Tags\"><table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td>" +
                itemTagsBody + "</td></tr></table></div>");

            itemTabs.Append("<h2><span>" + getLocale("Ratings & Reviews") +
                            " </span></h2>");
            itemTabs.Append(
                "<div id=\"ItemTab-Reviews\"><table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\" id=\"tblRatingPerUser\"><tr><td></td></tr></table>");
                       itemTabs.Append
                ("<div class=\"cssClassPageNumber\" id=\"divSearchPageNumber\"><div class=\"cssClassPageNumberMidBg\">");
            itemTabs.Append("<div id=\"Pagination\"></div><div class=\"cssClassViewPerPage\">" +
                           getLocale("View Per Page:") +
                           "<select id=\"ddlPageSize\" class=\"sfListmenu\">");
            itemTabs.Append(
                "<option value=\"5\">5</option><option value=\"10\">10</option><option value=\"15\">15</option><option value=\"20\">20</option><option value=\"25\">25</option><option value=\"40\">40</option></select></div>");
            itemTabs.Append("</div></div></div>");

            itemTabs.Append("<h2 style=\"display:none\"><span>" + getLocale("Videos") + " </span></h2>");
            itemTabs.Append("<div><div id=\"ItemVideos\" style=\"display:none\"></div></div>");
                                             dynHtml.Append(itemTabs);
            dynHtml.Append("</div></div></div>");
        }
        if (itemSKU.Length > 0)
        {
            string script = BindDataInTab(itemSKU, attributeSetId, itemTypeId);
            string tagBind = "";
                                     tagBind = GetItemTags(itemSKU);
                      dynHtml.Append(script);
                                      dynHtml.Append(tagBind);
                       ltrItemDetailsForm.Text = dynHtml.ToString();
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
            litQtyDiscount.Text = QtyDiscount.ToString();
        }
        else
        {
            string script = GetScriptRun("$('#bulkDiscount,#divQtyDiscount').hide();");
            litQtyDiscount.Text = script;
        }
    }

    private string GetScriptRun(string code)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<script type=\"text/javascript\">$(document).ready(function(){setTimeout(function(){ " + code +
                  "},500); });</script>");
        return sb.ToString();
    }

}
