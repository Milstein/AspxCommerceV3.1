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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspxCommerce.RewardPoints;
using SageFrame.Framework;
using SageFrame.Web;
using System.Collections;
using System.Web.Script.Serialization;
using SageFrame.Security;
using SageFrame.Security.Entities;
using SageFrame.Security.Helpers;
using System.Web.Security;
using System.IO;
using SageFrame.RolesManagement;
using SageFrame.Web.Utilities;
using SageFrame.Security.Crypto;
using SageFrame.Shared;
using AspxCommerce.Core;
using SageFrame.Core;
using SageFrame.Common;
using System.Text.RegularExpressions;
using System.Linq;



public partial class Modules_AspxCheckoutInformationContent_CheckoutInformationContent : BaseAdministrationUserControl
{
    private int storeID, portalID, customerID;
    private string userName;
    private string cultureName;
    private string sessionCode = string.Empty;
    private string strRoles = string.Empty;
    private bool RegisterURL = true;

    private decimal Discount = 0;

    private string userIP = string.Empty;
    private string ShippingDetailPage, ShowSubscription, noImageCheckOutInfoPath, myAccountURL, SingleAddressCheckOutURL, DimentionalUnit, WeightUnit, ShoppingCartURL = string.Empty;
    private bool IsUseFriendlyUrls = true;
    private string AllowededShippingCountry = string.Empty;
    private string AllowededBillingCountry = string.Empty;
    public string MainCurrency;
    public int cartCount;
                      private Random random = new Random();
    private string allowMultipleAddress = string.Empty;
    SageFrameConfig pagebase = new SageFrameConfig();
    StoreSettingConfig ssc = new StoreSettingConfig();
    AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
    private int CountDownloadableItem = 0, CountAllItem = 0;

    public string Coupon = "", Items = "", ServerVars = "", GiftCard = "", ScriptsToRun = "";
    JavaScriptSerializer json_serializer = new JavaScriptSerializer();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            IncludeLanguageJS();

            storeID = GetStoreID;
            portalID = GetPortalID;
            userName = GetUsername;
            customerID = GetCustomerID;
            cultureName = GetCurrentCultureName;
            IsUseFriendlyUrls = pagebase.GetSettingBollByKey(SageFrameSettingKeys.UseFriendlyUrls);

            if (HttpContext.Current.Session.SessionID != null)
            {
                sessionCode = HttpContext.Current.Session.SessionID.ToString();
            }

            List<CouponSession> cs = new List<CouponSession>();
            cs = CheckOutSessions.Get<List<CouponSession>>("CouponSession");
            Coupon = json_serializer.Serialize(cs);

            List<GiftCardUsage> gc = CheckOutSessions.Get<List<GiftCardUsage>>("UsedGiftCard");
            GiftCard = json_serializer.Serialize(gc);

            Discount = CheckOutSessions.Get<Decimal>("DiscountAmount", 0);

            if (!IsPostBack)
            { 
                IncludeCss("CheckOutInformationContent", "/Templates/" + TemplateName + "/css/MessageBox/style.css", "/Templates/" + TemplateName + "/css/JQueryUIFront/jquery.ui.all.css",
                    "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css", "/Modules/AspxCommerce/AspxCheckoutWithSingleAddress/css/module.css");
                IncludeJs("CheckOutInformationContent", "/js/encoder.js", "/js/FormValidation/jquery.validate.js", "/js/jquery.cookie.js", "/js/MessageBox/jquery.easing.1.3.js", "/js/MessageBox/alertbox.js", "/Modules/AspxCommerce/AspxCheckoutWithSingleAddress/js/SingleCheckOut.js", "/js/jquery.tipsy.js");
           
                if (HttpContext.Current.Session.SessionID != null)
                {
                    sessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
                aspxCommonObj.SessionCode = sessionCode;
                aspxCommonObj.StoreID = storeID;
                aspxCommonObj.PortalID = portalID;
                aspxCommonObj.UserName = userName;
                aspxCommonObj.CultureName = cultureName;
                aspxCommonObj.CustomerID = customerID;


                noImageCheckOutInfoPath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, storeID, portalID, cultureName);
                ShoppingCartURL = ssc.GetStoreSettingsByKey(StoreSetting.ShoppingCartURL, storeID, portalID, cultureName);
                myAccountURL = ssc.GetStoreSettingsByKey(StoreSetting.MyAccountURL, storeID, portalID, cultureName);
                AllowededShippingCountry = ssc.GetStoreSettingsByKey(StoreSetting.AllowedShippingCountry, storeID, portalID, cultureName);
                AllowededBillingCountry = ssc.GetStoreSettingsByKey(StoreSetting.AllowedBillingCountry, storeID, portalID, cultureName);
                SingleAddressCheckOutURL = ssc.GetStoreSettingsByKey(StoreSetting.SingleCheckOutURL, storeID, portalID, cultureName);
                DimentionalUnit = ssc.GetStoreSettingsByKey(StoreSetting.DimensionUnit, GetStoreID, GetPortalID,
                                                           GetCurrentCultureName);
                WeightUnit = ssc.GetStoreSettingsByKey(StoreSetting.WeightUnit, GetStoreID, GetPortalID,
                                                       GetCurrentCultureName);
                ShowSubscription = ssc.GetStoreSettingsByKey(StoreSetting.AskCustomerToSubscribe, GetStoreID, GetPortalID,
                                                   GetCurrentCultureName);

                ShippingDetailPage = ssc.GetStoreSettingsByKey(StoreSetting.ShipDetailPageURL, GetStoreID, GetPortalID,
                                                   GetCurrentCultureName);
                allowMultipleAddress = ssc.GetStoreSettingsByKey(StoreSetting.AllowUsersToCreateMultipleAddress, storeID, portalID, cultureName);
                userIP = HttpContext.Current.Request.UserHostAddress;




                HideSignUp();
                PasswordAspx.Attributes.Add("onkeypress", "return clickButton(event,'" + LoginButton.ClientID + "')");
               
                if (!IsParent)
                {
                    hypForgotPassword.NavigateUrl =
                        ResolveUrl(GetParentURL + "/portal/" + GetPortalSEOName + "/sf/" +
                                   pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalForgotPassword) + SageFrameSettingKeys.PageExtension);
                }
                else
                {
                    hypForgotPassword.NavigateUrl =
                        ResolveUrl("~/sf/" + pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalForgotPassword) +
                                  SageFrameSettingKeys.PageExtension);
                }
                string registerUrl =
                    ResolveUrl("~/sf/" + pagebase.GetSettingsByKey(SageFrameSettingKeys.PortalUserRegistration) +
                               SageFrameSettingKeys.PageExtension);
                signup.Attributes.Add("href", ResolveUrl("~/sf/sfUser-Registration" + SageFrameSettingKeys.PageExtension));
                signup1.Attributes.Add("href", ResolveUrl("~/sf/sfUser-Registration" + SageFrameSettingKeys.PageExtension));

                if (pagebase.GetSettingBollByKey(SageFrameSettingKeys.RememberCheckbox))
                {
                    RememberMe.Visible = true;
                    lblrmnt.Visible = true;
                }
                else
                {
                    RememberMe.Visible = false;
                    lblrmnt.Visible = false;
                }

                object serverVars = new
                {
                    noImageCheckOutInfoPath = noImageCheckOutInfoPath,
                    ShoppingCartURL = ShoppingCartURL,
                    myAccountURL = myAccountURL,
                    singleAddressCheckOutURL = SingleAddressCheckOutURL,
                    CartUrl = ShoppingCartURL,
                    AllowedShippingCountry = AllowededShippingCountry,
                    AllowedBillingCountry = AllowededBillingCountry,
                    dimentionalUnit = DimentionalUnit,
                    weightunit = WeightUnit,
                    showSubscription = ShowSubscription,
                    allowMultipleAddress = allowMultipleAddress, 
                    shippingDetailPage = ShippingDetailPage,
                    userIP = userIP,
                    Discount = Discount

                };

                ServerVars = json_serializer.Serialize(serverVars);
                LoadCartDetails();
                LoadCountry();
                LoadAddress();
                LoadPaymentGateway();
                LoadRewardPoints();
            }

            if (HttpContext.Current.User != null)
            {
                SecurityPolicy objSecurity = new SecurityPolicy();
                FormsAuthenticationTicket ticket = objSecurity.GetUserTicket(GetPortalID);
                if (ticket != null && ticket.Name != ApplicationKeys.anonymousUser)
                {
                    int LoggedInPortalID = int.Parse(ticket.UserData.ToString());
                    string[] sysRoles = SystemSetting.SUPER_ROLE;
                    MembershipController member = new MembershipController();
                    UserInfo userDetail = member.GetUserDetails(GetPortalID, GetUsername);
                    if (GetPortalID == LoggedInPortalID || Roles.IsUserInRole(userDetail.UserName, sysRoles[0]))
                    {
                        RoleController _role = new RoleController();
                        string userinroles = _role.GetRoleNames(GetUsername, LoggedInPortalID);
                        if (userinroles != "" || userinroles != null)
                        {
                            MultiView1.ActiveViewIndex = 1;
                        }
                        else
                        {
                            MultiView1.ActiveViewIndex = 0;
                        }
                    }
                    else
                    {
                        MultiView1.ActiveViewIndex = 0;
                    }
                }
                else
                {
                    MultiView1.ActiveViewIndex = 0;
                }
            }

        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }

    private void LoadAddress()
    {
        StringBuilder addressBuilder = new StringBuilder();
        addressBuilder.Append("<div id=\"ddlBilling\" class=\"clearfix\">");
        StringBuilder addressBuilderShip = new StringBuilder();
        addressBuilderShip.Append("<div id=\"ddlShipping\" class=\"clearfix\">");
        StringBuilder addressScript = new StringBuilder();
        List<AddressInfo> lstAddress = AspxUserDashController.GetUserAddressDetails(aspxCommonObj);
        addressBuilder.Append("");
        string tempAddress = "";
        tempAddress += "[";
        if (lstAddress.Count > 0)
        {
                       foreach (AddressInfo item in lstAddress)
            {

               
                string add = "{" +
                             string.Format(
                                 "Address1:\\'{0}\\',Address2:\\'{1}\\',AddressID:\\'{2}\\',City:\\'{3}\\',Company:\\'{4}\\',Country:\\'{5}\\' ,{6} DefaultBilling:\\'{7}\\',DefaultShipping:\\'{8}\\',Email:\\'{9}\\',Fax:\\'{10}\\',FirstName:\\'{11}\\',LastName:\\'{12}\\',Mobile:\\'{13}\\',Phone:\\'{14}\\',State:\\'{15}\\',Website:\\'{16}\\',Zip:\\'{17}\\'",
                                 item.Address1, item.Address2,
                                 item.AddressID, item.City, item.Company, item.Country, "",
                                 item.DefaultBilling.ToString().ToLower(),
                                 item.DefaultShipping.ToString().ToLower(), item.Email, item.Fax, item.FirstName,
                                 item.LastName,
                                 item.Mobile, item.Phone, item.State, item.Website, item.Zip)
                             + "},";
                tempAddress += add;
                               if (item.DefaultBilling != null && bool.Parse(item.DefaultBilling.ToString()))
                {
                    addressBuilder.Append("<div><label><input type=\"radio\" name=\"billing\" value=\"" + item.AddressID +
                                          "\" checked=\"checked\" class=\"cssBillingShipping\" />");
                }
                else
                {
                    addressBuilder.Append("<div><label><input type=\"radio\" name=\"billing\" value=\"" + item.AddressID +
                                          "\" class=\"cssBillingShipping\" />");
                }
                addressBuilder.Append(item.FirstName.Replace(",", "-") + " " + item.LastName.Replace(",", "-"));

                if (item.Address1 != "")
                    addressBuilder.Append(", " + item.Address1.Replace(",", "-"));

                if (item.City != "")
                    addressBuilder.Append(", " + item.City.Replace(",", "-"));

                if (item.State != "")
                    addressBuilder.Append(", " + item.State.Replace(",", "-"));

                if (item.Country != "")
                    addressBuilder.Append(", " + item.Country.Replace(",", "-"));

                if (item.Zip != "")
                    addressBuilder.Append(", " + item.Zip.Replace(",", "-"));

                if (item.Email != "")
                    addressBuilder.Append(", " + item.Email.Replace(",", "-"));

                if (item.Phone != "")
                    addressBuilder.Append(", " + item.Phone.Replace(",", "-"));

                if (item.Mobile != "")
                    addressBuilder.Append(", " + item.Mobile.Replace(",", "-"));

                if (item.Fax != "")
                    addressBuilder.Append(", " + item.Fax.Replace(",", "-"));

                if (item.Website != "")
                    addressBuilder.Append(", " + item.Website.Replace(",", "-"));

                if (item.Address2 != "")
                    addressBuilder.Append(", " + item.Address2.Replace(",", "-"));

                if (item.Company != "")
                    addressBuilder.Append(", " + item.Company.Replace(",", "-"));
                addressBuilder.Append("</label></div>");

                if (item.DefaultShipping != null && bool.Parse(item.DefaultShipping.ToString()))
                {
                    addressBuilderShip.Append("<div><label><input type=\"radio\" name=\"shipping\" value=\"" + item.AddressID +
                                              "\" checked=\"checked\" class=\"cssBillingShipping\" />");
                }
                else
                {
                    addressBuilderShip.Append("<div><label><input type=\"radio\" name=\"shipping\" value=\"" + item.AddressID +
                                              "\" class=\"cssBillingShipping\" />");

                }
                addressBuilderShip.Append(item.FirstName.Replace(",", "-") + " " + item.LastName.Replace(",", "-"));

                if (item.Address1 != "")
                    addressBuilderShip.Append(", " + item.Address1.Replace(",", "-"));

                if (item.City != "")
                    addressBuilderShip.Append(", " + item.City.Replace(",", "-"));

                if (item.State != "")
                    addressBuilderShip.Append(", " + item.State.Replace(",", "-"));

                if (item.Country != "")
                    addressBuilderShip.Append(", " + item.Country.Replace(",", "-"));

                if (item.Zip != "")
                    addressBuilderShip.Append(", " + item.Zip.Replace(",", "-"));

                if (item.Email != "")
                    addressBuilderShip.Append(", " + item.Email.Replace(",", "-"));

                if (item.Phone != "")
                    addressBuilderShip.Append(", " + item.Phone.Replace(",", "-"));

                if (item.Mobile != "")
                    addressBuilderShip.Append(", " + item.Mobile.Replace(",", "-"));

                if (item.Fax != "")
                    addressBuilderShip.Append(", " + item.Fax.Replace(",", "-"));

                if (item.Website != "")
                    addressBuilderShip.Append(", " + item.Website.Replace(",", "-"));

                if (item.Address2 != "")
                    addressBuilderShip.Append(", " + item.Address2.Replace(",", "-"));

                if (item.Company != "")
                    addressBuilderShip.Append(", " + item.Company.Replace(",", "-"));
                addressBuilderShip.Append("</label></div>");

            }
                       addressBuilderShip.Append("</div>");
            addressBuilder.Append("</div>");
            tempAddress = tempAddress.Substring(0, tempAddress.Length - 1);
            tempAddress += "]";
            string script = string.Empty;
            if (CountDownloadableItem == CountAllItem)
            {

               

                ScriptsToRun += addressScript.Append("CheckOut.SetTempAddr(eval(\"" + tempAddress +
                                      "\"));$(\"#dvBilling .cssClassCheckBox\").hide();$(\"#dvCPShipping\").parent(\"div\").hide();$(\"#dvCPShippingMethod\").parent(\"div\").hide();$(\"#addBillingAddress\").show(); $(\"#addShippingAddress\").show();")
                     .ToString();
            }
            else
            {

               

                ScriptsToRun += addressScript.Append("CheckOut.SetTempAddr(eval(\"" + tempAddress +
                                        "\"));$(\"#dvBilling .cssClassCheckBox\").show(); $(\"#dvCPShipping\").parent(\"div\").show();$(\"#dvCPShippingMethod\").parent(\"div\").show(); $(\"#addShippingAddress\").show();")
                       .ToString();
            }

                       ltddlBilling.Text = addressBuilder.ToString();
            ltddlShipping.Text = addressBuilderShip.ToString();


        }
        else
        {
            addressBuilderShip.Append("</div>");
            addressBuilder.Append("</div>");
            ltddlBilling.Text = addressBuilder.ToString();
            ltddlShipping.Text = addressBuilderShip.ToString();
        }
    }

    private void LoadCountry()
    {

        StringBuilder blCountry = new StringBuilder();
        StringBuilder spCountry = new StringBuilder();
        StringBuilder optionCountry = new StringBuilder();
        List<CountryInfo> lstCountry = AspxCommonController.BindCountryList();
        blCountry.Append("<select id=\"ddlBLCountry\">");
        spCountry.Append("<select id=\"ddlSPCountry\">");
        foreach (var countryInfo in lstCountry)
        {
            optionCountry.Append("<option class=\"cssBillingShipping\" value=\"" + countryInfo.Value + "\"> " +
                                 countryInfo.Text + "</option>");

        }
        blCountry.Append(optionCountry);
        spCountry.Append(optionCountry);
        blCountry.Append("</select>");
        spCountry.Append("</select>");

        ltSPCountry.Text = spCountry.ToString();
        ltBLCountry.Text = blCountry.ToString();
    }

    private void LoadCartDetails()
    {
        string modulePath = this.AppRelativeTemplateSourceDirectory;
        hst = AppLocalized.getLocale(modulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        string aspxTemplateFolderPath = ResolveUrl("~/") + "Templates/" + TemplateName;
        string aspxRootPath = ResolveUrl("~/");


        StringBuilder cartDetails = new StringBuilder();
        StringBuilder scriptBuilder = new StringBuilder();

        List<CartInfo> lstCart = AspxCartController.GetCartCheckOutDetails(aspxCommonObj);
        cartCount = lstCart.Count;
        lstCart = lstCart.Select(e => { e.KitData = Regex.Replace(e.KitData, "[\"\"]+", "'"); return e; }).ToList();
        Items = json_serializer.Serialize(lstCart);
        cartDetails.Append(
            "<table class=\"sfGridTableWrapper\" width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" id=\"tblCartList\">");
        cartDetails.Append(
            "<thead><tr class=\"cssClassHeadeTitle\">");
        cartDetails.Append(
            "<th class=\"cssClassSN\"> Sn.");
        cartDetails.Append(" </th><th class=\"cssClassProductImageWidth\">");
        cartDetails.Append(
            getLocale("Item Image"));
        cartDetails.Append(
            "</th>");
        cartDetails.Append(
            "<th>");
        cartDetails.Append(
            getLocale("Variants"));
        cartDetails.Append("</th>");
        cartDetails.Append("<th class=\"cssClassQTY\">");
        cartDetails.Append(
            getLocale("Qty"));
        cartDetails.Append("</th>");
        cartDetails.Append("<th class=\"cssClassProductPrice\">");
        cartDetails.Append(
            getLocale("Unit Price"));
        cartDetails.Append(
            "</th>");
        cartDetails.Append("<th class=\"cssClassSubTotal\">");
        cartDetails.Append(getLocale("Line Total"));
        cartDetails.Append("</th>");
        cartDetails.Append("<th class=\"cssClassTaxRate\">");

        cartDetails.Append(getLocale("Unit Tax"));
        cartDetails.Append("</th>");
        cartDetails.Append("</tr>");
        cartDetails.Append("</thead");
        cartDetails.Append("<tbody>");

                      int giftcardCount = 0;
        int index = 0;
        string itemids = "";
        bool IsDownloadItemInCart = false, ShowShippingAdd = false;
        int CartID = 0;//int CountDownloadableItem = 0, CountAllItem = 0, 
        string bsketItems = "";
        bsketItems += "[";
        foreach (CartInfo item in lstCart)
        {
                       if (item.ItemTypeID == 1 || item.ItemTypeID == 6)
            {
                string bitems = "{" +
                             string.Format(
                                 "Height:{0},ItemName:\\'{1}\\',Length:{2},Quantity:{3},WeightValue:{4},Width:{5}",
                                 item.Height ?? 0,HttpUtility.HtmlEncode(item.ItemName),
                                 item.Length ?? 0, item.Quantity.ToString(), decimal.Parse(item.Weight.ToString()), item.Width ?? 0
                                 )

                             + "},";
                bsketItems += bitems;

            }


            itemids += item.ItemID + "#" + item.CostVariantsValueIDs + ",";

            index++;
            string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.ImagePath;
            if (item.ImagePath == "")
            {
                imagePath = noImageCheckOutInfoPath;
            }
            else if (item.AlternateText == "")
            {
                item.AlternateText = item.ItemName;
            }
            if (item.ItemTypeID == 2)
            {
                IsDownloadItemInCart = true;
                CountDownloadableItem++;
            }
                       var isVirtual = false;
            if (item.ItemTypeID == 3)
            {

                int typ = AspxGiftCardController.GetGiftCardType(aspxCommonObj, item.CartItemID);

                if (typ > 0)
                {
                    if (typ == 2)
                    {
                        ShowShippingAdd = false;
                        isVirtual = false;
                    }
                    else
                    {
                        ShowShippingAdd = true;
                        isVirtual = true;
                    }

                }

                giftcardCount++;
                if (lstCart.Count != giftcardCount)
                {
                    ShowShippingAdd = false;
                }
            }
                      

                                                                                        
                                                                                                                                                                                                      
           

           


            CountAllItem++;
                       cartDetails.Append("<tr >");
            cartDetails.Append("<td><input type=\"hidden\" name=\"cartItemId\" value=\"" + item.CartItemID + "\" />");
            cartDetails.Append("<b>" + index + ".</b>");
            cartDetails.Append("</td>");
            cartDetails.Append("<td>");
            cartDetails.Append("<p class=\"cssClassCartPicture\">");
            cartDetails.Append("<img title=\"" + item.AlternateText + "\" src=\"" + aspxRedirectPath +
                               imagePath.Replace("uploads", "uploads/Small") + "\" ></p>");
                                  cartDetails.Append("<div class=\"cssClassCartPictureInformation\">");
            cartDetails.Append("<h3>");
            if (item.CostVariantsValueIDs != "")
            {
                cartDetails.Append("<a class=\"cssClassLink\" id=\"item_" + item.ItemID + " itemType=\"" +
                                   item.ItemTypeID +
                                   "\"  href=\"" + aspxRedirectPath + "item/" + item.SKU +
                                   pageExtension + "?varId=" + item.CostVariantsValueIDs + "\">" + item.ItemName +
                                   "\" </a></h3>");

            }
            else
            {

                if (item.ItemTypeID == 3)
                {
                    cartDetails.Append("<a class=\"cssClassLink\" id=\"item_" + item.ItemID + "_" + index + "\" isvirtual=\"" +
                                       isVirtual +
                                       "\" itemType=\"" + item.ItemTypeID + "\"  href=\"" +
                                       aspxRedirectPath + "item/" + item.SKU + pageExtension +
                                       "\">" + item.ItemName + "</a></h3>");
                }
                else
                {
                    cartDetails.Append("<a class=\"cssClassLink\" id=\"item_" + item.ItemID + "_" + index + "\"  itemType=\"" +
                                       item.ItemTypeID + "\"  href=\"" + aspxRedirectPath + "item/" +
                                       item.SKU + pageExtension + "\">" + item.ItemName + "</a></h3>");

                }
            }
                                                        cartDetails.Append("</div>");
            cartDetails.Append("</td>");
            cartDetails.Append("<td class=\"row-variants\" varIDs=\"" + item.CostVariantsValueIDs + "\">");
            cartDetails.Append(item.CostVariants);
            cartDetails.Append("</td>");
            cartDetails.Append("<td class=\"cssClassPreviewQTY\">");
            cartDetails.Append("<input class=\"num-pallets-input\" taxrate=\"0\" price=\"" +
                               item.Price + "\" id=\"txtQuantity_" + item.CartID +
                               "\" type=\"text\" readonly=\"readonly\" disabled=\"disabled\" value=\"" + item.Quantity +
                               "\">");
            cartDetails.Append("</td>");
                                             cartDetails.Append("<td class=\"price-per-pallet\">");
            cartDetails.Append("<span id=\"" + item.Weight + "\" class=\"cssClassFormatCurrency\">" +
                               Math.Round(double.Parse((item.Price).ToString()), 3).ToString("N3") + "</span>");
            cartDetails.Append("</td>");
                                             cartDetails.Append("<td class=\"row-total\">");
            cartDetails.Append("<input class=\"row-total-input cssClassFormatCurrency\" id=\"txtRowTotal_" + item.CartID +
                               "\"  value=\"" +
                               Math.Round(double.Parse((item.TotalItemCost).ToString()), 3).ToString("N3") +
                               "\" baseValue=\"" +
                               Math.Round(double.Parse((item.TotalItemCost).ToString()), 3).ToString("N3") +
                               "\"  readonly=\"readonly\" type=\"text\" />")
                ;
            cartDetails.Append("</td>");
            cartDetails.Append("<td class=\"row-taxRate\">");
                       cartDetails.Append("<span class=\"cssClassFormatCurrency\">0.00</span>");
            cartDetails.Append("</td>");
            cartDetails.Append("</tr>");
            CartID = item.CartID;
                      

        }
        cartDetails.Append("</table>");
        if (bsketItems.Length > 1)
            bsketItems = bsketItems.Substring(0, bsketItems.Length - 1);
        bsketItems += "]";
        scriptBuilder.Append("  CheckOut.SetBasketItems(eval(\"" + bsketItems + "\")); CheckOut.Vars.ItemIDs=\"" + itemids + "\";");
        scriptBuilder.Append("CheckOut.UserCart.CartID=" + CartID + ";");
        scriptBuilder.Append(" CheckOut.UserCart.ShowShippingAdd=" + ShowShippingAdd.ToString().ToLower() + ";");
        scriptBuilder.Append(" CheckOut.UserCart.IsDownloadItemInCart=" + IsDownloadItemInCart.ToString().ToLower() + ";");
        scriptBuilder.Append(" CheckOut.UserCart.CountDownloadableItem=" + CountDownloadableItem + ";");
        scriptBuilder.Append(" CheckOut.UserCart.CountAllItem=" + CountAllItem + "; CheckOut.BindFunction();");
        scriptBuilder.Append(
            "$(\"#tblCartList tr:even\").addClass(\"sfEven\");$(\"#tblCartList tr:odd\").addClass(\"sfOdd\");");


               ScriptsToRun += scriptBuilder.ToString();
                      ltTblCart.Text = cartDetails.ToString();
       
    }

    private void LoadRewardPoints()
    {
        StoreSettingConfig ssc = new StoreSettingConfig();
        bool isEnableRewardPoint = true;//ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, storeID, portalID, cultureName);
        if (isEnableRewardPoint)
        {
            List<GeneralSettingInfo> lstGeneralSet = RewardPointsController.GetGeneralSetting(aspxCommonObj);
            StringBuilder rewardPointBuilder = new StringBuilder();
            StringBuilder scriptrewardPoint = new StringBuilder();

            if (lstGeneralSet.Count > 0)
            {

                foreach (GeneralSettingInfo generalSettingInfo in lstGeneralSet)
                {
                    if (generalSettingInfo.IsActive)
                    {
                                                                                            
                        if (generalSettingInfo.AmountSpent != 0 && generalSettingInfo.AmountSpent != null)
                        {
                            scriptrewardPoint.Append(" $(\"#hdnRewardRate\").val(eval(" +
                                                     generalSettingInfo.RewardPointsEarned / generalSettingInfo.AmountSpent +
                                                     "));");
                        }
                        else
                        {
                            scriptrewardPoint.Append(" $(\"#hdnRewardRate\").val(eval(" + 0 + "));");
                        }

                        scriptrewardPoint.Append("$(\"#hdnRate\").val(eval(" +
                                                 generalSettingInfo.RewardExchangeRate / generalSettingInfo.RewardPoints +
                                                 "));");

                        bool IsPurchaseActive = RewardPointsController.IsPurchaseActive(aspxCommonObj);
                        scriptrewardPoint.Append("CheckOut.UserCart.IsPurchaseActive=" +
                                                 IsPurchaseActive.ToString().ToLower() + ";");

                        RewardPointsController.RewardPointsSelectedValue("btnRewardPointsValue");



                        decimal rewardPointsSliderSelectedValue =
                            RewardPointsController.RewardPointsSelectedValue("btnRewardPointsValue");

                        if (rewardPointsSliderSelectedValue > 0)
                        {
                            scriptrewardPoint.Append("CheckOut.UserCart.RewardPointsDiscount = eval(" +
                                                     rewardPointsSliderSelectedValue +
                                                     "* parseFloat($(\"#hdnRate\").val()).toFixed(2));");
                            scriptrewardPoint.Append("CheckOut.UserCart.UsedRewardPoints =" +
                                                     rewardPointsSliderSelectedValue + ";");
                            scriptrewardPoint.Append(
                                " $(\"#txtRewardAmount\").val(CheckOut.UserCart.RewardPointsDiscount);");



                        }
                        else
                        {
                            scriptrewardPoint.Append(
                              "CheckOut.UserCart.RewardPointsDiscount = 0;CheckOut.UserCart.UsedRewardPoints = 0;");


                        }
                        scriptrewardPoint.Append(" $(\"#trDiscountReward\").show();");
                        ScriptsToRun += scriptrewardPoint.ToString();
                                                                  }
                }
            }
        }

    }

    private void LoadPaymentGateway()
    {
        string aspxRootPath = ResolveUrl("~/");
        List<PaymentGatewayListInfo> pginfo = AspxCartController.GetPGList(aspxCommonObj);

        StringBuilder paymentGateWay = new StringBuilder();
        foreach (var item in pginfo)
        {
            if (item.LogoUrl != "")
            {
                paymentGateWay.Append(
                    "<label><input id=\"rdb" +
                    item.PaymentGatewayTypeName + "\" name=\"PGLIST\" type=\"radio\" realname=\"" +
                    item.PaymentGatewayTypeName + "\" friendlyname=\"" + item.FriendlyName + "\"  source=\"" +
                    item.ControlSource + "\" value=\"" + item.PaymentGatewayTypeID +
                    "\" class=\"cssClassRadioBtn\" /><img class=\"cssClassImgPGList\" src=\"" + aspxRootPath +
                    item.LogoUrl + "\" alt=\"" + item.PaymentGatewayTypeName + "\" title=\"" +
                    item.PaymentGatewayTypeName + "\" visible=\"true\" /></label>");

            }
            else
            {
                paymentGateWay.Append("<label><input id=\"rdb" + item.PaymentGatewayTypeName +
                                      "\" name=\"PGLIST\" type=\"radio\" realname=\"" + item.PaymentGatewayTypeName +
                                      "\" friendlyname=\"" + item.FriendlyName + "\"  source=\"" + item.ControlSource +
                                      "\" value=\"" + item.PaymentGatewayTypeID + "\" class=\"cssClassRadioBtn\" /><b>" +
                                      item.PaymentGatewayTypeName + "</b></label>");

            }

        }
        ScriptsToRun += "CheckOut.BindPGFunction();";
               ltPgList.Text = paymentGateWay.ToString();
    }


    private string GetStringScript(string codeToRun)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script type=\"text/javascript\">$(document).ready(function(){ setTimeout(function(){ " + codeToRun + "},500); });</script>");
        return script.ToString();
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




    #region ServerSide Mthods
    private void HideSignUp()
    {
        int UserRegistrationType = pagebase.GetSettingIntByKey(SageFrameSettingKeys.PortalUserRegistration);
        RegisterURL = UserRegistrationType > 0 ? true : false;
        if (!RegisterURL)
        {
            this.divSignUp.Visible = false;
        }
    }


    public void SetUserRoles(string strRoles)
    {
        Session[SessionKeys.SageUserRoles] = strRoles;
        HttpCookie cookie = HttpContext.Current.Request.Cookies[CookiesKeys.SageUserRolesCookie];
        if (cookie == null)
        {
            cookie = new HttpCookie(CookiesKeys.SageUserRolesCookie);
        }
        cookie[CookiesKeys.SageUserRolesCookie] = strRoles;
        HttpContext.Current.Response.Cookies.Add(cookie);
    }

    protected void LoginButton_Click(object sender, EventArgs e)
    {
        MembershipController member = new MembershipController();
        RoleController role = new RoleController();
        UserInfo user = member.GetUserDetails(GetPortalID, UserName.Text);
        if (user.UserExists && user.IsApproved)
        {
            if (!(string.IsNullOrEmpty(UserName.Text) && string.IsNullOrEmpty(PasswordAspx.Text)))
            {
                if (PasswordHelper.ValidateUser(user.PasswordFormat, PasswordAspx.Text, user.Password, user.PasswordSalt))
                {
                    string userRoles = role.GetRoleNames(user.UserName, GetPortalID);
                    strRoles += userRoles;
                    if (strRoles.Length > 0)
                    {
                        SetUserRoles(strRoles);
                        SessionTracker sessionTracker = (SessionTracker)Session[SessionKeys.Tracker];
                        sessionTracker.PortalID = GetPortalID.ToString();
                        sessionTracker.Username = UserName.Text;
                        Session[SessionKeys.Tracker] = sessionTracker;
                        SageFrame.Web.SessionLog SLog = new SageFrame.Web.SessionLog();
                        SLog.SessionTrackerUpdateUsername(sessionTracker, sessionTracker.Username, GetPortalID.ToString());

                                                                      StringBuilder redirectURL = new StringBuilder();
                        SecurityPolicy objSecurity = new SecurityPolicy();
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                          user.UserName,
                          DateTime.Now,
                          DateTime.Now.AddMinutes(30),
                          true,
                          GetPortalID.ToString(),
                          FormsAuthentication.FormsCookiePath);

                                               string encTicket = FormsAuthentication.Encrypt(ticket);

                                               string randomCookieValue = GenerateRandomCookieValue();
                        Session[SessionKeys.RandomCookieValue] = randomCookieValue;
                                               HttpCookie cookie = new HttpCookie(objSecurity.FormsCookieName(GetPortalID), encTicket);
                                               SageFrameConfig objConfig = new SageFrameConfig();
                        string ServerCookieExpiration = objConfig.GetSettingValueByIndividualKey(SageFrameSettingKeys.ServerCookieExpiration);
                        int expiryTime = Math.Abs(int.Parse(ServerCookieExpiration));
                        expiryTime = expiryTime < 5 ? 5 : expiryTime;
                                               cookie.Expires = DateTime.Now.AddMinutes(expiryTime);
                                               Response.Cookies.Add(cookie);
                                               ServiceSecurity.IssueToken(GetPortalID);
                        if (Request.QueryString["ReturnUrl"] != null)
                        {
                            string PageNotFoundPage = PortalAPI.PageNotFoundURLWithRoot;
                            string UserRegistrationPage = PortalAPI.RegistrationURLWithRoot;
                            string PasswordRecoveryPage = PortalAPI.PasswordRecoveryURLWithRoot;
                            string ForgotPasswordPage = PortalAPI.ForgotPasswordURL;
                            string PageNotAccessiblePage = PortalAPI.PageNotAccessibleURLWithRoot;

                            string ReturnUrlPage = Request.QueryString["ReturnUrl"].Replace("%2f", "-").ToString();

                            if (ReturnUrlPage == PageNotFoundPage || ReturnUrlPage == UserRegistrationPage || ReturnUrlPage == PasswordRecoveryPage || ReturnUrlPage == ForgotPasswordPage || ReturnUrlPage == PageNotAccessiblePage)
                            {
                                redirectURL.Append(GetParentURL);
                                redirectURL.Append(PortalAPI.DefaultPageWithExtension);
                            }
                            else
                            {
                                redirectURL.Append(ResolveUrl(Request.QueryString["ReturnUrl"].ToString()));
                            }
                        }
                        else
                        {
                                                                                                                                                                                             
                                                      
                                                                                 

                            if (!IsParent)
                            {
                                redirectURL.Append(GetParentURL);
                                redirectURL.Append("/portal/");
                                redirectURL.Append(GetPortalSEOName);
                                redirectURL.Append("/");
                                redirectURL.Append(ssc.GetStoreSettingsByKey(StoreSetting.SingleCheckOutURL, GetStoreID, GetPortalID, GetCurrentCultureName));
                                redirectURL.Append(SageFrameSettingKeys.PageExtension);
                                                           }
                            else
                            {
                                redirectURL.Append(GetParentURL);
                                redirectURL.Append("/");
                                redirectURL.Append(ssc.GetStoreSettingsByKey(StoreSetting.SingleCheckOutURL, GetStoreID, GetPortalID, GetCurrentCultureName));
                                redirectURL.Append(SageFrameSettingKeys.PageExtension);
                                                           }

                        }

                                                                      int customerID = GetCustomerID;
                        if (customerID == 0)
                        {
                            CustomerGeneralInfo sageUserCust = CustomerGeneralInfoController.CustomerIDGetByUsername(user.UserName, GetPortalID, GetStoreID);
                            if (sageUserCust != null)
                            {
                                customerID = sageUserCust.CustomerID;
                            }
                        }
                        UpdateCartAnonymoususertoRegistered(GetStoreID, GetPortalID, customerID, sessionCode);
                        Response.Redirect(redirectURL.ToString(), false);
                    }
                    else
                    {
                        FailureText.Text = string.Format("<p class='sfError'>{0}</p>", GetSageMessage("UserLogin", "Youarenotauthenticatedtothisportal"));
                                           }
                }
                else
                {
                    FailureText.Text = string.Format("<p class='sfError'>{0}</p>", GetSageMessage("UserLogin", "UsernameandPasswordcombinationdoesntmatched"));//"Username and Password combination doesn't matched!";
                                   }
            }
        }
        else
        {
            FailureText.Text = string.Format("<p class='sfError'>{0}</p>", GetSageMessage("UserLogin", "UserDoesnotExist"));
                   }
    }

    public int GetCustomerIDByUserName(int storeID, int portalID, string userName)
    {
        try
        {
            List<KeyValuePair<string, object>> ParaMeter = new List<KeyValuePair<string, object>>();
            ParaMeter.Add(new KeyValuePair<string, object>("@StoreID", storeID));
            ParaMeter.Add(new KeyValuePair<string, object>("@PortalID", portalID));
            ParaMeter.Add(new KeyValuePair<string, object>("@UserName", userName));
            SQLHandler sqlH = new SQLHandler();
            return sqlH.ExecuteAsScalar<int>("usp_Aspx_GetCustomerIDByUserName", ParaMeter);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool UpdateCartAnonymoususertoRegistered(int storeID, int portalID, int customerID, string sessionCode)
    {
        try
        {
            List<KeyValuePair<string, object>> ParaMeter = new List<KeyValuePair<string, object>>();
            ParaMeter.Add(new KeyValuePair<string, object>("@StoreID", storeID));
            ParaMeter.Add(new KeyValuePair<string, object>("@PortalID", portalID));
            ParaMeter.Add(new KeyValuePair<string, object>("@CustomerID", customerID));
            ParaMeter.Add(new KeyValuePair<string, object>("@SessionCode", sessionCode));
            SQLHandler sqlH = new SQLHandler();
            return sqlH.ExecuteNonQueryAsBool("usp_Aspx_UpdateCartAnonymousUserToRegistered", ParaMeter, "@IsUpdate");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private string GenerateRandomCookieValue()
    {
        string s = "";
        string[] CapchaValue = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        for (int i = 0; i < 10; i++)
            s = String.Concat(s, CapchaValue[this.random.Next(36)]);
        return s;
    }

    #endregion
}