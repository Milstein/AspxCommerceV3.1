using System;
using SageFrame.Web;
using AspxCommerce.Core;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using AspxCommerce.BrandView;
using AspxCommerce.ImageResizer;

public partial class Modules_AspxCommerce_AspxBrandView_BrandSlider : BaseAdministrationUserControl
{
    public int StoreID;
    public int PortalID;
    public string UserName;
    public string CultureName;
    public string BrandModulePath;
    public int BrandCount, BrandRssCount;
    public bool EnableBrand, EnableBrandRss;
    public string  BrandAllPage, BrandRssPage;


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                StoreSettingConfig ssc = new StoreSettingConfig();
                IncludeJs("BrandSlider", "/Modules/AspxCommerce/AspxBrandView/js/BrandSlide.js", "/js/Sliderjs/jquery.bxSlider.js","/js/jquery.tipsy.js");
                IncludeCss("BrandSlider", "/Templates/" + TemplateName + "/css/Slider/style.css", "/Templates/" + TemplateName + "/css/ToolTip/tooltip.css",
                    "/Modules/AspxCommerce/AspxBrandView/css/module.css");                
                PortalID = GetPortalID;
                StoreID = GetStoreID;
                UserName = GetUsername;
                CultureName = GetCurrentCultureName;
                BrandModulePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);               
            }
            GetBrandSetting();
            GetAllBrandForSlider();
            IncludeLanguageJS();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }
    public void GetBrandSetting()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.CultureName = CultureName;
        AspxBrandViewController objBrand = new AspxBrandViewController();
        BrandSettingInfo lstBrandSetting = objBrand.GetBrandSetting(aspxCommonObj);
        if (lstBrandSetting != null)
        {
            EnableBrand = lstBrandSetting.IsEnableBrand;
            BrandCount = lstBrandSetting.BrandCount;
            BrandAllPage = lstBrandSetting.BrandAllPage;
            EnableBrandRss = lstBrandSetting.IsEnableBrandRss;
            BrandRssCount = lstBrandSetting.BrandRssCount;
            BrandRssPage = lstBrandSetting.BrandRssPage;
        }
    }

    Hashtable hst = null;
    public void GetAllBrandForSlider()
    {
        AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
        aspxCommonObj.StoreID = StoreID;
        aspxCommonObj.PortalID = PortalID;
        aspxCommonObj.UserName = UserName;
        aspxCommonObj.CultureName = CultureName;
        string aspxRootPath = ResolveUrl("~/");       
        hst = AppLocalized.getLocale(BrandModulePath);
        string pageExtension = SageFrameSettingKeys.PageExtension;
        AspxBrandViewController objBrand = new AspxBrandViewController();
        List<BrandViewInfo> lstBrand = objBrand.GetAllBrandForSlider(aspxCommonObj, BrandCount);
        StringBuilder element = new StringBuilder();
        if (lstBrand != null && lstBrand.Count > 0)
        {
            element.Append("<ul id=\"brandSlider\">");
            foreach (BrandViewInfo value in lstBrand)
            {
                
                
                var imagepath = aspxRootPath + value.BrandImageUrl;
                string []imageFile=value.BrandImageUrl.ToString().Split('/');
                if (value.BrandImageUrl != "")
                {
                    //Resize Image Dynamically
                    InterceptImageController.ImageBuilder(imageFile[imageFile.Length-1], ImageType.Small, ImageCategoryType.Brand, aspxCommonObj);
                }
                element.Append("<li><a href=\"");
                element.Append(aspxRedirectPath);
                element.Append("brand/");
                element.Append(AspxUtility.fixedEncodeURIComponent(value.BrandName));
                element.Append(pageExtension);
                element.Append("\"><img brandId=\"");
                element.Append(value.BrandID);
                element.Append("\" src=\"");
                element.Append(imagepath.Replace("uploads", "uploads/Small"));
                element.Append("\" alt=\"");
                element.Append(value.BrandName);
                element.Append("\" title=\"");

                element.Append(value.BrandName);
                element.Append("\"  /></a></li>");
            }
            element.Append("</ul>");
            element.Append("<span class=\"cssClassViewMore\"><a href=\"");
            element.Append(aspxRedirectPath);
            element.Append(BrandAllPage);
            element.Append(pageExtension);
            element.Append("\">"+ getLocale("View All Brands")+ "</a></span>");
        }

        else
        {
            element.Append("<span class='cssClassNotFound'>");
            element.Append(getLocale("The store has no brand!"));
            element.Append("</span>");
        }
        litSlide.Text = element.ToString();
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
