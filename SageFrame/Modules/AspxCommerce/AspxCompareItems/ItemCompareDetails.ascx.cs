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
using SageFrame.Web;
using SageFrame.Framework;
using AspxCommerce.Core;
using SageFrame.Core;

public partial class Modules_AspxCommerce_AspxItemsCompare_ItemCompareDetails : BaseAdministrationUserControl
{
    public string UserIP;
    public int StoreID, PortalID, CustomerID;
    public string CountryName = string.Empty;
    public string SessionCode = string.Empty;
    public string UserName, CultureName,ServicePath;
    public string NoImageItemComparePath,AllowAddToCart, AllowOutStockPurchase;

    protected void page_init(object sender, EventArgs e)
    {
        try
        {
            ServicePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory + "/Service/Service.asmx/");

            UserIP = HttpContext.Current.Request.UserHostAddress;
            IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
            ipToCountry.GetCountry(UserIP, out CountryName);         
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
                IncludeJs("ItemCompareDetails", "/js/encoder.js", "/js/Templating/tmpl.js", "/js/jquery.cookie.js");
                IncludeCss("ItemCompareDetails", "/Modules/AspxCommerce/AspxCompareItems/css/module.css");
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                CustomerID = GetCustomerID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
                UserIP = HttpContext.Current.Request.UserHostAddress;
                IPAddressToCountryResolver ipToCountry = new IPAddressToCountryResolver();
                ipToCountry.GetCountry(UserIP, out CountryName);

                StoreSettingConfig ssc = new StoreSettingConfig();
                AllowAddToCart = ssc.GetStoreSettingsByKey(StoreSetting.ShowAddToCartButton, StoreID, PortalID, CultureName);
                NoImageItemComparePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
                AllowOutStockPurchase = ssc.GetStoreSettingsByKey(StoreSetting.AllowOutStockPurchase, StoreID, PortalID, CultureName);                   
            }
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }  
}
