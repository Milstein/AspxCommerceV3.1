using System;
using System.Collections.Generic;
using System.Text;
using SageFrame.Web;
using System.Collections;
using SageFrame;
using AspxCommerce.Core;

public partial class Modules_AspxCommerce_AspxItemDetailTab_ItemDetailTab :BaseUserControl
{
    public string itemSKU;
    public int storeID,
               portalID,
               UserModuleID,
               customerID;
    public string userName, cultureName;
    public string sessionCode = string.Empty;   
    public int itemTypeId = 0;
    protected void page_init(object sender, EventArgs e)
    {      

        try
        {
            SageFrameConfig pagebase = new SageFrameConfig();           
            SageFrameRoute parentPage = (SageFrameRoute)this.Page;

            itemSKU = parentPage.Key;
            if (!IsPostBack)
            {
                storeID = GetStoreID;
                portalID = GetPortalID;
                customerID = GetCustomerID;
                userName = GetUsername;
                cultureName = GetCurrentCultureName;
            }
            IncludeJs("ItemDetailTab", "/js/encoder.js", "/js/StarRating/jquery.rating.pack.js", "/js/StarRating/jquery.MetaData.js", "/js/Paging/jquery.pagination.js");
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }
  
    AspxCommonInfo aspxCommonObj = new AspxCommonInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            IncludeCss("ItemDetails", "/Templates/" + TemplateName + "/css/StarRating/jquery.rating.css",
                       "/Templates/" + TemplateName + "/css/JQueryUIFront/jquery-ui.all.css",
                       "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                       "/Templates/" + TemplateName + "/css/FancyDropDown/fancy.css",
                       "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                       "/Templates/" + TemplateName + "/css/Scroll/scrollbars.css",
                       "/Templates/" + TemplateName + "/css/ResponsiveTab/responsive-tabs.css",
					   "/Modules/AspxCommerce/AspxItemDetailTab/css/module.css"
					   );

            IncludeJs("ItemDetails", "/js/DateTime/date.js",
                      "/js/StarRating/jquery.rating.js",
                       "/Modules/AspxCommerce/AspxItemDetailTab/js/ItemDetailTab.js",
                      "/js/ResponsiveTab/responsiveTabs.js",
                      "/js/PopUp/popbox.js", "/js/Scroll/mwheelIntent.js",
                      "/js/Scroll/jScrollPane.js", 
                      "/js/VideoGallery/jquery.youtubepopup.min.js", "/js/jquery.labelify.js");
        }
        aspxCommonObj.UserName = GetUsername;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.CustomerID = GetCustomerID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        aspxCommonObj.SessionCode = sessionCode;
        IncludeLanguageJS();        
        GetFormFieldList(itemSKU);      
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
            itemTagsBody.Append("<div class=\"cssClassPopularItemTags\"><div id=\"popularTag\"></div>");            
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
            itemTabs.Append(BindItemVideo());
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

    private StringBuilder BindItemVideo()
    {
        StringBuilder videoContainer = new StringBuilder();
        string itemVideo = AspxItemMgntController.GetItemVideos(itemSKU,aspxCommonObj);
        if (itemVideo != null && itemVideo !="")
        {
            videoContainer.Append("<h2 ><span>" + getLocale("Videos") + " </span></h2>");
            videoContainer.Append("<div><div id=\"ItemVideos\"><ul>");  
            string[] arr = itemVideo.Split(',');    
            string href="http://img.youtube.com/vi/";
            foreach(string item in arr) {
                string source = href + item + "/default.jpg";
                videoContainer.Append("<li><img class='youtube' id=\"");
                videoContainer.Append(item);
                videoContainer.Append("\" src=\"");
                videoContainer.Append(source);              
                videoContainer.Append("title=\"Click me to play!\" /></li>");
            }           
            videoContainer.Append("</ul></div></div>");
        }
        return videoContainer;
    }    

    private string GetItemTags(string sku)
    {
        string itemTags = string.Empty;
        string tagNames = string.Empty;
        string myTags = string.Empty;
        string userTags = string.Empty;
        StringBuilder bindTag = new StringBuilder();
        StringBuilder popularTag = new StringBuilder();
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
         if(itemTags !="" && itemTags !=null)
            {
                popularTag.Append("<h2>" + getLocale("Popular Tags:")+"");
                popularTag.Append("</h2><div id=\"divItemTags\" class=\"cssClassPopular-Itemstags\">");
                popularTag.Append(itemTags.Substring(0, itemTags.Length - 2));
                popularTag.Append("</div>");
                bindTag.Append("$('#popularTag').html('"+popularTag+"')");
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

    private string GetScriptRun(string code)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<script type=\"text/javascript\">$(document).ready(function(){setTimeout(function(){ " + code +
                  "},500); });</script>");
        return sb.ToString();
    }

}