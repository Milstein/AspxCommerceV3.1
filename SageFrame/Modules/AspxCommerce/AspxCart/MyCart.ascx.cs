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
using AspxCommerce.RewardPoints;
using SageFrame.Web;
using AspxCommerce.Core;
using SageFrame.Core;
using System.Web.Script.Serialization;
using System.Linq;
using System.Text.RegularExpressions;
using AspxCommerce.ImageResizer;


public partial class MyCart : BaseAdministrationUserControl
{

    public int StoreID, PortalID, CustomerID, MinimumItemQuantity, MaximumItemQuantity,UserModuleIDCart;
    public int CartItemCount = 0;
    public string UserName, CultureName;
    public string SessionCode = string.Empty;
    public string AllowRealTimeNotifications, NoImageMyCartPath, AllowMultipleAddShipping, AllowShippingRateEstimate, AllowCouponDiscount, ShowItemImagesOnCart, MinCartSubTotalAmount, AllowOutStockPurchase, MultipleAddressChkOutURL;
    public bool IsUseFriendlyUrls = true;
    private AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
    public string Coupon = "";
    public string Items = "";
    public decimal qtyDiscount = 0;
    public string CartPRDiscount = "";
    public string CartModulePath = string.Empty;
    JavaScriptSerializer json_serializer = new JavaScriptSerializer();

    protected void Page_Load(object sender, EventArgs e)
    {
        HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.Now);
        Response.Cache.SetValidUntilExpires(true);
        StoreID = GetStoreID;
        PortalID = GetPortalID;
        UserName = GetUsername;
        CustomerID = GetCustomerID;
        CultureName = GetCurrentCultureName;
        UserModuleIDCart = int.Parse(SageUserModuleID);
        StoreSettingConfig ssc = new StoreSettingConfig();
        try
        {
            CartModulePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
            SageFrameConfig pagebase = new SageFrameConfig();
            IsUseFriendlyUrls = pagebase.GetSettingBollByKey(SageFrameSettingKeys.UseFriendlyUrls);

            if (HttpContext.Current.Session.SessionID != null)
            {
                SessionCode = HttpContext.Current.Session.SessionID.ToString();
            }
            if (!IsPostBack)
            {
                IncludeCss("MyCart", "/Templates/" + TemplateName + "/css/GridView/tablesort.css",
                           "/Templates/" + TemplateName + "/css/MessageBox/style.css",
                           "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                           "/Modules/AspxCommerce/AspxCart/css/module.css");
                IncludeJs("MyCart", "/js/encoder.js", "/js/MessageBox/alertbox.js", "/js/jquery.easing.1.3.js",
                          "/Modules/AspxCommerce/AspxCart/js/MyCart.js", "/js/jquery.tipsy.js");

                aspxCommonObj.StoreID = StoreID;
                aspxCommonObj.PortalID = PortalID;
                aspxCommonObj.UserName = UserName;
                aspxCommonObj.CultureName = CultureName;
                aspxCommonObj.SessionCode = SessionCode;
                aspxCommonObj.CustomerID = CustomerID;
                NoImageMyCartPath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID,
                                                              CultureName);
                AllowMultipleAddShipping = ssc.GetStoreSettingsByKey(StoreSetting.AllowMultipleShippingAddress, StoreID,
                                                                     PortalID, CultureName);
                ShowItemImagesOnCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowItemImagesInCart, StoreID, PortalID,
                                                                 CultureName);
                MinCartSubTotalAmount = ssc.GetStoreSettingsByKey(StoreSetting.MinimumCartSubTotalAmount, StoreID,
                                                                  PortalID, CultureName);
                                              AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID,
                                                                  CultureName);
                MultipleAddressChkOutURL = ssc.GetStoreSettingsByKey(StoreSetting.MultiCheckOutURL, StoreID, PortalID,
                                                                     CultureName);
                AllowShippingRateEstimate = ssc.GetStoreSettingsByKey(StoreSetting.AllowShippingRateEstimate, StoreID, PortalID,
                                                                     CultureName);
                AllowCouponDiscount = ssc.GetStoreSettingsByKey(StoreSetting.AllowCouponDiscount, StoreID, PortalID,
                                                                    CultureName);
                AllowRealTimeNotifications = ssc.GetStoreSettingsByKey(StoreSetting.AllowRealTimeNotifications, StoreID, PortalID, CultureName);

            }
            DisplayCartItems();
            IncludeLanguageJS();
            CouponInfo();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void CouponInfo()
    {
        List<CouponSession> cs = new List<CouponSession>();
        cs = CheckOutSessions.Get<List<CouponSession>>("CouponSession");// (List<CouponSession>)Session["CouponSession"];
        Coupon = json_serializer.Serialize(cs);
    }

    Hashtable hst = null;
    private string getLocale(string messageKey)
    {
        string retStr = messageKey;
        if (hst != null && hst[messageKey] != null)
        {
            retStr = hst[messageKey].ToString();
        }
        return retStr;
    }


    private void DisplayCartItems()
    {

        string modulePath = this.AppRelativeTemplateSourceDirectory;
        hst = AppLocalized.getLocale(modulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        string aspxRootPath = ResolveUrl("~/");

        double arrRewardtotalPrice = 0;
        string arrRewardDetails = "";
        string arrRewardSub = "";


        List<CartInfo> itemsList = LoadCartItems();
        if (itemsList.Count > 0)
        {
            CartPRDiscount = AspxCartController.GetDiscountPriceRule(itemsList[0].CartID, aspxCommonObj, 0);
            GetDiscount();
        }
        itemsList = itemsList.Select(e => { e.KitData = Regex.Replace(e.KitData, "[\"\"]+", "'"); return e; }).ToList();
        Items = json_serializer.Serialize(itemsList);

        StringBuilder scriptBuilder_root = new StringBuilder();
        StringBuilder cartItemList = new StringBuilder();
        if (itemsList.Count > 0)
        {
            CartItemCount = itemsList.Count;
            cartItemList.Append(
                GetStringScript(
                    " $('.cssClassSubTotalAmount,.cssClassLeftRightBtn,.cssClassapplycoupon,.cssClassBlueBtnWrapper').show();"));
            cartItemList.Append(
                "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" id=\"tblCartList\" class=\"sfGridTableWrapper\">");
            cartItemList.Append("<thead><tr class=\"cssClassHeadeTitle\" >");
            cartItemList.Append("<th class=\"cssClassSN\">Sn.");
            if (ShowItemImagesOnCart.ToLower() == "true")
            {
                cartItemList.Append("</th><th class=\"cssClassItemImageWidth\">");
                cartItemList.Append(getLocale("Item Description"));
            }
            cartItemList.Append("</th><th>");
                                             cartItemList.Append(getLocale("Variants"));
            cartItemList.Append("</th>");
            cartItemList.Append("<th class=\"cssClassQTY\">");
            cartItemList.Append(getLocale("Qty"));
            cartItemList.Append("</th>");
                                             cartItemList.Append("<th class=\"cssClassItemPrice\">");
            cartItemList.Append(getLocale("Unit Price"));
            cartItemList.Append("</th>");
                                             cartItemList.Append("<th class=\"cssClassSubTotal\">");
            cartItemList.Append(getLocale("Line Total"));
            cartItemList.Append("</th>");
                                                                              cartItemList.Append("<th class=\"cssClassAction\">");
            cartItemList.Append(getLocale("Action"));
            cartItemList.Append("</th>");
            cartItemList.Append(" </tr>");
            cartItemList.Append("</thead>");
            cartItemList.Append("<tbody>");
            List<BasketItem> basketItems = new List<BasketItem>();
            int index = 0;

            string bsketItems = "";
            bsketItems += "[";
            foreach (CartInfo item in itemsList)
            {
                                              if (item.ItemTypeID == 1)
                {
                    string bitems = "{" +
                                    string.Format(
                                        "\'Height\':'{0}',\'ItemName\':'{1}',\'Length\':'{2}',\'Quantity\':'{3}',\'WeightValue\':'{4}',\'Width\':'{5}'",
                                        item.Height ?? 0, item.ItemName,
                                        item.Length ?? 0, item.Quantity.ToString(),
                                        decimal.Parse(item.Weight.ToString()), item.Width ?? 0
                                        )

                                    + "},";
                    bsketItems += bitems;

                }

                index = index + 1;
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.ImagePath;
                if (item.ImagePath == "")
                {
                    imagePath = NoImageMyCartPath;
                }
                else if (item.AlternateText == "")
                {
                    item.AlternateText = item.ItemName;
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(item.ImagePath, ImageType.Small, ImageCategoryType.Item, aspxCommonObj);
                }
                else if (item.ImagePath != "")
                {
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(item.ImagePath, ImageType.Small, ImageCategoryType.Item, aspxCommonObj);
                }

                if ((itemsList.IndexOf(item)) % 2 == 0)
                {
                    cartItemList.Append("<tr class=\"sfEven\" >");
                }
                else
                {
                    cartItemList.Append("<tr class=\"sfOdd\" >");
                }
                cartItemList.Append("<td>");
                cartItemList.Append("<b>" + index + "." + "</b>");
                ;
                cartItemList.Append("</td>");
                if (item.ItemTypeID == 3)
                {



                    cartItemList.Append("<td>");

                    if (ShowItemImagesOnCart.ToLower() == "true")
                    {

                        cartItemList.Append("<p class=\"cssClassCartPicture\">");
                        cartItemList.Append("<img src='" + aspxRootPath +
                                            imagePath.Replace("uploads", "uploads/Small") + "'  alt=\"" +
                                            item.AlternateText + "\" title=\"" + item.AlternateText + "\" ></p>");
                                           }
                                       cartItemList.Append("<div class=\"cssClassCartPictureInformation\">");


                    cartItemList.Append("<a href=\"item/" + item.SKU + pageExtension + "\" costvariants=\"" +
                                        item.CostVariants +
                                        "\" onclick=\"AspxCart.SetCostVartSession(this);\" >" + item.ItemName +
                                        "</a>");
                    cartItemList.Append("<ul class='giftcardInfo'>");
                    cartItemList.Append("<li>");
                    cartItemList.Append(item.ShortDescription);
                    cartItemList.Append("</li>");
                    cartItemList.Append("</ul>");
                    cartItemList.Append("</div>");


                   
                    cartItemList.Append("</td>");
                }
                else if (item.ItemTypeID == 6)
                {

                    cartItemList.Append("<td>");

                    if (ShowItemImagesOnCart.ToLower() == "true")
                    {

                        cartItemList.Append("<p class=\"cssClassCartPicture\">");
                        cartItemList.Append("<img src='" + aspxRootPath +
                                            imagePath.Replace("uploads", "uploads/Small") + "'  alt=\"" +
                                            item.AlternateText + "\" title=\"" + item.AlternateText + "\" ></p>");
                                           }
                                       cartItemList.Append("<div class=\"cssClassCartPictureInformation\">");


                    cartItemList.Append("<a href=\"item/" + item.SKU + pageExtension + "\" costvariants=\"" +
                                        item.CostVariants +
                                        "\" onclick=\"AspxCart.SetCostVartSession(this);\" >" + item.ItemName +
                                        "</a>");
                    string[] lis = Regex.Split(item.ShortDescription, "</br>");
                    cartItemList.Append("<ul class='kitInfo'>");

                    foreach (var li in lis)
                    {
                        cartItemList.Append("<li>" + li + "</li>");
                    }
                    cartItemList.Append("</ul>");
                    cartItemList.Append("</div>");

                  
                    cartItemList.Append("</td>");
                }
                else
                {
                    cartItemList.Append("<td>");
                    if (ShowItemImagesOnCart.ToLower() == "true")
                    {
                       
                        cartItemList.Append("<p class=\"cssClassCartPicture\">");
                        cartItemList.Append("<img src='" + aspxRootPath +
                                            imagePath.Replace("uploads", "uploads/Small") + "'  alt=\"" +
                                            item.AlternateText + "\" title=\"" + item.AlternateText + "\" ></p>");
                                           }
                                       cartItemList.Append("<div class=\"cssClassCartPictureInformation\">");

                    if (item.CostVariantsValueIDs != "")
                    {
                        cartItemList.Append("<a href=\"item/" + item.SKU + pageExtension + "?varId=" +
                                          item.CostVariantsValueIDs + "\"  costvariants=\"" + item.CostVariants +
                                          "\" onclick=\"AspxCart.SetCostVartSession(this);\" >" + item.ItemName +
                                          "</a>");
                    }
                    else
                    {
                        cartItemList.Append("<a href=\"item/" + item.SKU + pageExtension + "\" costvariants=\"" +
                                            item.CostVariants +
                                            "\" onclick=\"AspxCart.SetCostVartSession(this);\" >" + item.ItemName +
                                            "</a>");
                                           }
                                                                                                cartItemList.Append("</div>");
                    cartItemList.Append("</td>");
                }
                cartItemList.Append("<td class=\"row-variants\">");
                cartItemList.Append(item.CostVariants);
                cartItemList.Append("</td>");
                cartItemList.Append("<td class=\"cssClassQTYInput\">");
                cartItemList.Append("<input class=\"num-pallets-input\" autocomplete=\"off\" price=\"" +
                                    Math.Round(double.Parse((item.Price).ToString()), 2).ToString() +
                                    "\" id=\"txtQuantity_" + item.CartItemID + "\" type=\"text\" cartID=\"" +
                                    item.CartID +
                                    "\" value=\"" + item.Quantity + "\" sku=\"" + item.SKU + "\"  quantityInCart=\"" + item.Quantity +
                                    "\" actualQty=\"" + item.ItemQuantity + "\" costVariantID=\"" +
                                    item.CostVariantsValueIDs + "\" itemID=\"" + item.ItemID + "\" addedValue=\"" +
                                    item.Quantity + "\" minCartQuantity=\"" + item.MinCartQuantity + "\" maxCartQuantity=\"" + item.MaxCartQuantity + "\">");
                cartItemList.Append("<label class=\"lblNotification\" style=\"color: #FF0000;\"></label></td>");
                                                             cartItemList.Append("<td class=\"price-per-pallet\">");
                cartItemList.Append(
                    "<span class=\"cssClassFormatCurrency\">" +
                    Math.Round(double.Parse((item.Price).ToString()), 2).ToString("N2") + "</span>");
                cartItemList.Append("</td>");
                                                             cartItemList.Append("<td class=\"row-total\">");
                cartItemList.Append("<input class=\"row-total-input cssClassFormatCurrency\" autocomplete=\"off\" id=\"txtRowTotal_" +
                                    item.CartID + "\" value=\"" +
                                    Math.Round(double.Parse((item.TotalItemCost).ToString()), 2).ToString("N2") +
                                    "\"  readonly=\"readonly\" type=\"text\" />");
                cartItemList.Append("</td>");
                                                                                                          cartItemList.Append("<td>");
                cartItemList.Append(" <a class=\"ClassDeleteCartItems\" title=\"Delete\" value=\"" +
                                    item.CartItemID + "\" cartID=\"" + item.CartID + "\"><i class=\"i-delete\"></i></a>");
                cartItemList.Append("</td>");
                cartItemList.Append("</tr>");
               
                arrRewardtotalPrice += Math.Round(double.Parse((item.Price * item.Quantity).ToString()), 2);

                arrRewardSub += "'<li>'+ " + item.Quantity + "+'X' +eval(" + (item.Price) +
                                "* rewardRate).toFixed(2)+ '</li>'+";
                arrRewardDetails += "'<li><b>'+eval( " + (item.TotalItemCost) +
                                    "* rewardRate).toFixed(2)+ '</b> " + getLocale("Points for Product:") + item.ItemName +
                                    "&nbsp &nbsp</li>'+";

                if (index == itemsList.Count)
                {

                    StringBuilder scriptBuilder = new StringBuilder();

                    scriptBuilder.Append("AspxCart.Vars.CartID =" + item.CartID + ";");
                    scriptBuilder.Append(" var rewardRate = parseFloat($('#hdnRewardRate').val());");
                    scriptBuilder.Append("var arrRewardDetails =" +
                                         arrRewardDetails.Substring(0, arrRewardDetails.Length - 1) + ";");
                    scriptBuilder.Append("var  arrRewardSub =" + arrRewardSub.Substring(0, arrRewardSub.Length - 1) +
                                         ";");
                    scriptBuilder.Append("if (isPurchaseActive == true){");
                    scriptBuilder.Append("$('#dvPointsTotal').empty(); $('#ulRewardDetails').html(arrRewardDetails);");
                    scriptBuilder.Append("$('#ulRewardSub').html(arrRewardSub);");
                    scriptBuilder.Append("$('#dvPointsTotal').append(eval(" + arrRewardtotalPrice + " * rewardRate).toFixed(2));");

                    scriptBuilder.Append("} ");
                    scriptBuilder.Append("AspxCart.GetDiscountCartPriceRule(AspxCart.Vars.CartID, 0);");
                    scriptBuilder.Append("$('#tblCartList tr:even ').addClass('sfEven');");
                    scriptBuilder.Append("$('#tblCartList tr:odd ').addClass('sfOdd');");
                    scriptBuilder.Append("$('.cssClassCartPicture img[title]').tipsy({ gravity: 'n' });");
                    scriptBuilder.Append("AspxCart.BindCartFunctions();");
                    bsketItems = bsketItems.Substring(0, bsketItems.Length - 1);
                    bsketItems += "]";
                    scriptBuilder.Append(" AspxCart.SetBasketItems(eval(\"" + bsketItems + "\"));");

                                      
                }

            }
            cartItemList.Append("</table>");

                                            
            string rewardScript = LoadRewardPoints();
            scriptBuilder_root.Append(rewardScript);
                       scriptBuilder_root.Append("if (isPurchaseActive == true){");
            scriptBuilder_root.Append(" var rewardRate = parseFloat($('#hdnRewardRate').val());");
            scriptBuilder_root.Append("var arrRewardDetails =" +
                                 arrRewardDetails.Substring(0, arrRewardDetails.Length - 1) + ";");
            scriptBuilder_root.Append("var  arrRewardSub =" + arrRewardSub.Substring(0, arrRewardSub.Length - 1) +
                                 ";");
            scriptBuilder_root.Append("$('#dvPointsTotal').empty(); $('#ulRewardDetails').html(arrRewardDetails);");
            scriptBuilder_root.Append("$('#ulRewardSub').html(arrRewardSub);");
            scriptBuilder_root.Append("$('#dvPointsTotal').append(eval(" + arrRewardtotalPrice + " * rewardRate).toFixed(2));");

            scriptBuilder_root.Append("} ");
                                  ltCartItems.Text = cartItemList.ToString();
        }
        else
        {
            StringBuilder scriptBuilder = new StringBuilder();
                      
            scriptBuilder.Append("$('.cssClassCartInformation').html('<span class=\"cssClassNotFound\">" +
                                 getLocale("Your cart is empty!") + "</span>');");
            string script = GetStringScript(scriptBuilder.ToString());
            ltCartItems.Text = script;
        }
    }

    private void GetDiscount()
    {

        qtyDiscount = AspxCartController.GetDiscountQuantityAmount(aspxCommonObj);

           }

    private string LoadRewardPoints()
    {

        StoreSettingConfig ssc = new StoreSettingConfig();
        bool isEnableRewardPoint = true;
       

        if (isEnableRewardPoint)
        {
            List<GeneralSettingInfo> lstGeneralSet = RewardPointsController.GetGeneralSetting(aspxCommonObj);

            StringBuilder scriptrewardPoint = new StringBuilder();

            if (lstGeneralSet.Count > 0)
            {

                foreach (GeneralSettingInfo generalSettingInfo in lstGeneralSet)
                {
                    if (generalSettingInfo.IsActive)
                    {

                        decimal maxRewardPoints = generalSettingInfo.TotalRewardPoints;
                                               scriptrewardPoint.Append("  isRewardPointEnbled = true; $('#hdnRate').val(eval(" +
                                                 generalSettingInfo.RewardExchangeRate / generalSettingInfo.RewardPoints +
                                                 "));");
                        if (generalSettingInfo.AmountSpent != 0 && generalSettingInfo.AmountSpent != null)
                        {
                            scriptrewardPoint.Append(" $('#hdnRewardRate').val(eval(" +
                                                     generalSettingInfo.RewardPointsEarned / generalSettingInfo.AmountSpent +
                                                     "));");
                        }
                        else
                        {
                            scriptrewardPoint.Append(" $('#hdnRewardRate').val(eval(" + 0 + "));");
                        }
                        scriptrewardPoint.Append("   $('#hdnTotalRewardAmount').val(eval(" +
                                                 generalSettingInfo.TotalRewardAmount + "));");
                        scriptrewardPoint.Append("  $('#hdnTotalRewardPoints').val(eval(" +
                                                 generalSettingInfo.TotalRewardPoints + "));");
                        scriptrewardPoint.Append("  $('#hdnMinRedeemBalance').val(eval(" +
                                                 generalSettingInfo.MinRedeemBalance + "));");
                        scriptrewardPoint.Append("  $('#hdnCapped').val(eval(" + generalSettingInfo.BalanceCapped +
                                                 "));");
                        scriptrewardPoint.Append("  $('#spanCapped').html(eval(" + generalSettingInfo.BalanceCapped +
                                                 "));");
                        scriptrewardPoint.Append("  $('#spanTotalRewardPoints').html('" +
                                                 generalSettingInfo.TotalRewardPoints + "');");
                        scriptrewardPoint.Append("  $('#spanTotalRewardAmount').html('" +
                                                 generalSettingInfo.TotalRewardAmount + "');");
                        scriptrewardPoint.Append("   $('#spanRPExchangePoints').html('" +
                                                 generalSettingInfo.RewardPoints + "');");
                        scriptrewardPoint.Append("  $('#spanRPExchangeAmount').html('" +
                                                 generalSettingInfo.RewardExchangeRate + "');");
                        scriptrewardPoint.Append("  $('#spanTotalRP').html('" + generalSettingInfo.TotalRewardPoints +
                                                 "');");
                        scriptrewardPoint.Append("  $('#spanTotalRA').html('" + generalSettingInfo.TotalRewardAmount +
                                                 "');");
                                               scriptrewardPoint.Append("   $('.divRange').show();");
                        scriptrewardPoint.Append("  $('#dvUsePoints').show();");
                        scriptrewardPoint.Append("   $('#dvRewardPointsMain').show();");
                        scriptrewardPoint.Append("   $('#trDiscountReward').show();");
                                               bool IsPurchaseActive = RewardPointsController.IsPurchaseActive(aspxCommonObj);
                        scriptrewardPoint.Append("isPurchaseActive=" + IsPurchaseActive.ToString().ToLower() + ";");
                        if (IsPurchaseActive)
                        {
                            scriptrewardPoint.Append(" $('#dvRPCurrent').show();");

                        }
                       
                        scriptrewardPoint.Append(
                            "$('#slider-range').slider({range: true,min: 0,max:" + maxRewardPoints + ",values: [0, " +
                            maxRewardPoints +
                            "],slide: function(event, ui) {$('#amount').html('<span>' + ui.values[0] + '</span>' + ' - ' + '<span>' + ui.values[1] + '</span>');},change: function(event, ui) { $('#amount').html('<span>' + ui.values[0] + '</span>' + '-' + '<span>' + ui.values[1] + '</span>');}});");
                        scriptrewardPoint.Append(
                            "$('#amount').html('<span>' + $('#slider-range').slider('values', 0) + '</span>' +' - ' + '<span>' + $('#slider-range').slider('values', 1) + '</span>');");

                       
                                               return scriptrewardPoint.ToString();
                    }
                }
            }
        }
        return "";
    }


    private string BindJsFunctions()
    {
        StringBuilder functionBuilder = new StringBuilder();
               functionBuilder.Append("AspxCart.GetDiscountCartPriceRule(AspxCart.Vars.CartID, 0);");
        functionBuilder.Append("$('#tblCartList tr:even ').addClass('sfEve');");
        functionBuilder.Append("$('#tblCartList tr:odd ').addClass('sfOdd');");
        functionBuilder.Append("$('.cssClassCartPicture img[title]').tipsy({ gravity: 'n' });");
        functionBuilder.Append("AspxCart.BindCartFunctions();");
        return functionBuilder.ToString();

    }

    private string GetStringScript(string codeToRun)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script type=\"text/javascript\">$(document).ready(function(){ setTimeout(function(){ " +
                      codeToRun + "},500); });</script>");
        return script.ToString();
    }

    private List<CartInfo> LoadCartItems()
    {

        List<CartInfo> cartInfos = AspxCoreController.GetCartDetails(aspxCommonObj);
        return cartInfos;
    }
}
