using System;
using System.Web;
using SageFrame.Web;
using AspxCommerce.Core;
using System.Collections.Generic;
using SageFrame;
using System.Text;
using AspxCommerce.ImageResizer;

public partial class Modules_AspxCommerce_AspxItemImageGallery_AspxItemImageGallery : BaseUserControl
{
    public int StoreID, PortalID, CustomerID, ItemID;
    public string UserName, CultureName, ItemImageGalleryModulePath, referImagePath;
    public string SessionCode = string.Empty;
    public int ItemTypeID = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                StoreID = GetStoreID;
                PortalID = GetPortalID;
                UserName = GetUsername;
                CustomerID = GetCustomerID;
                CultureName = GetCurrentCultureName;
                if (HttpContext.Current.Session.SessionID != null)
                {
                    SessionCode = HttpContext.Current.Session.SessionID.ToString();
                }
                ItemImageGalleryModulePath = ResolveUrl(this.AppRelativeTemplateSourceDirectory);
                IncludeJs("ItemImageGallery", "/Modules/AspxCommerce/AspxItemImageGallery/js/ItemImageGallery.js", "/Modules/AspxCommerce/AspxItemImageGallery/js/jquery.jcarousel.js",
                   "/Modules/AspxCommerce/AspxItemImageGallery/js/jquery.mousewheel.js", "/Modules/AspxCommerce/AspxItemImageGallery/js/multizoom.js");
                IncludeCss("ItemImageGallery", "/Modules/AspxCommerce/AspxItemImageGallery/css/Slider.css",
                       "/Modules/AspxCommerce/AspxItemImageGallery/css/multizoom.css");
            }
            //IncludeLanguageJS();
            BindItemImageGallery();
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }
    }
    AspxCommonInfo aspxCommonObj = new AspxCommonInfo();
    private void BindItemImageGallery()
    {
        SageFrameRoute parentPage = (SageFrameRoute)this.Page;
        string itemSKU = parentPage.Key;
        if (HttpContext.Current.Session.SessionID != null)
        {
            SessionCode = HttpContext.Current.Session.SessionID.ToString();
        }
        string costCombination = "";
        aspxCommonObj.UserName = GetUsername;
        aspxCommonObj.PortalID = GetPortalID;
        aspxCommonObj.StoreID = GetStoreID;
        aspxCommonObj.CustomerID = GetCustomerID;
        aspxCommonObj.CultureName = GetCurrentCultureName;
        aspxCommonObj.SessionCode = SessionCode;
        AspxCoreController obj = new AspxCoreController();
        ItemCommonInfo objItemInfo = obj.GetItemInfoFromSKU(itemSKU, aspxCommonObj);
        if (objItemInfo != null)
        {
            ItemID = objItemInfo.ItemID;
            ItemTypeID = objItemInfo.ItemTypeID;
        }
        if (ItemTypeID == 3)
        {
            List<GiftCardInfo> lstItemGallery = obj.GetGiftCardThemeImagesByItem(ItemID, aspxCommonObj);
            BindGiftGallery(lstItemGallery);
        }
        else
        {
            List<VariantCombination> lstItemCombination = obj.GetCostVariantCombinationbyItemSku(itemSKU, aspxCommonObj);
            if (lstItemCombination != null && lstItemCombination.Count > 0)
            {
                costCombination = lstItemCombination[0].CombinationID.ToString();
            }
            else
            {
                costCombination = "0";
            }
            List<ItemsInfoSettings> lstItemGallery = obj.GetItemsImageGalleryInfoBySKU(itemSKU, aspxCommonObj, costCombination);
            BindGallery(lstItemGallery);
        }
    }

    private void BindGallery(List<ItemsInfoSettings> lstItemGallery)
    {
        string aspxRootPath = ResolveUrl("~/");
        StoreSettingConfig ssc = new StoreSettingConfig();
        string NoImagePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
        StringBuilder galleryContainer = new StringBuilder();
        StringBuilder galleryThumbContainer = new StringBuilder();
        galleryThumbContainer.Append("<div class='multizoom1 thumbs jcarousel-skin'><ul>");
        galleryContainer.Append("<div class='targetarea'>");
        if (lstItemGallery != null && lstItemGallery.Count > 0)
        {
            foreach (ItemsInfoSettings item in lstItemGallery)
            {
                string imagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.ImagePath;
                string altImagePath = "Modules/AspxCommerce/AspxItemsManagement/uploads/" + item.AlternateText;
                if (item.ImagePath == "")
                {
                    imagePath = NoImagePath;
                }
                else
                {   //Resize Image Dynamically
                    InterceptImageController objImage = new InterceptImageController();
                    objImage.MultipleImageResizer(item.ImagePath, (ImageType.Medium + ";" + ImageType.Large + ";" + ImageType.Small), ImageCategoryType.Item.ToString(), aspxCommonObj);
                }
                if (lstItemGallery.IndexOf(item) == 0)
                {
                    referImagePath = imagePath;
                    galleryContainer.Append("<img  id='multizoom1' title=\"");
                    galleryContainer.Append(item.AlternateText);
                    galleryContainer.Append("\" src=\"");
                    galleryContainer.Append(aspxRootPath);
                    galleryContainer.Append(imagePath.Replace("uploads", "uploads/Large"));
                    galleryContainer.Append("\" >");
                }
                galleryThumbContainer.Append("<li><a  href=\"");
                galleryThumbContainer.Append(aspxRootPath);
                galleryThumbContainer.Append(imagePath.Replace("uploads", "uploads/Large"));
                galleryThumbContainer.Append("\" data-large=\"");
                galleryThumbContainer.Append(aspxRootPath);
                galleryThumbContainer.Append(imagePath);
                galleryThumbContainer.Append("\" ><img title=\"");
                galleryThumbContainer.Append(item.AlternateText);
                galleryThumbContainer.Append("\" src=\"");
                galleryThumbContainer.Append(aspxRootPath);
                galleryThumbContainer.Append(imagePath.Replace("uploads", "uploads/Small"));
                galleryThumbContainer.Append("\" ></a></li>");

            }
        }
        else
        {
            galleryContainer.Append("<img  id='multizoom1' src=\"");
            galleryContainer.Append(aspxRootPath);
            galleryContainer.Append(NoImagePath.Replace("uploads", "uploads/Large"));
            galleryContainer.Append("\">");
            galleryThumbContainer.Append("<li><a  href=\"");
            galleryThumbContainer.Append(aspxRootPath);
            galleryThumbContainer.Append(NoImagePath.Replace("uploads", "uploads/Large"));
            galleryThumbContainer.Append("\" data-large=\"");
            galleryThumbContainer.Append(aspxRootPath);
            galleryThumbContainer.Append(NoImagePath);
            galleryThumbContainer.Append("\" ><img ");
            galleryThumbContainer.Append(" src=\"");
            galleryThumbContainer.Append(aspxRootPath);
            galleryThumbContainer.Append(NoImagePath.Replace("uploads", "uploads/Small"));
            galleryThumbContainer.Append("\"></a></li>");
        }
        galleryContainer.Append("</ul></div>");
        galleryThumbContainer.Append("</div>");
        ltrItemGallery.Text = galleryContainer.ToString();
        ltrItemThumb.Text = galleryThumbContainer.ToString();
    }

    private void BindGiftGallery(List<GiftCardInfo> lstItemGallery)
    {
        string aspxRootPath = ResolveUrl("~/");
        StoreSettingConfig ssc = new StoreSettingConfig();
        string NoImagePath = ssc.GetStoreSettingsByKey(StoreSetting.DefaultProductImageURL, StoreID, PortalID, CultureName);
        StringBuilder galleryContainer = new StringBuilder();
        StringBuilder galleryThumbContainer = new StringBuilder();
        galleryThumbContainer.Append("<div class='multizoom1 thumbs jcarousel-skin'><ul>");
        galleryContainer.Append("<div class='targetarea'>");
        if (lstItemGallery != null && lstItemGallery.Count > 0)
        {
            foreach (GiftCardInfo item in lstItemGallery)
            {
                string imagePath = item.GraphicImage;
                if (item.GraphicImage == "")
                {
                    imagePath = NoImagePath;
                }
                else
                {  //Resize Image Dynamically
                    InterceptImageController objImage = new InterceptImageController();
                    objImage.MultipleImageResizer(item.GraphicImage, (ImageType.Medium + ";" + ImageType.Large + ";" + ImageType.Small), ImageCategoryType.Item.ToString(), aspxCommonObj);
                }
                if (lstItemGallery.IndexOf(item) == 0)
                {
                    referImagePath = imagePath;
                    galleryContainer.Append("<img  id='multizoom1' title=\"");
                    galleryContainer.Append(item.GraphicName);
                    galleryContainer.Append("\" src=\"");
                    galleryContainer.Append(aspxRootPath);
                    galleryContainer.Append(imagePath);
                    galleryContainer.Append("\" >");
                    galleryThumbContainer.Append("<li><a class='selected'  href=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" data-id=\"");
                    galleryThumbContainer.Append(item.GiftCardGraphicId);
                    galleryThumbContainer.Append("\" data-large=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" ><img title=\"");
                    galleryThumbContainer.Append(item.GraphicName);
                    galleryThumbContainer.Append("\" src=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" ></a></li>");
                }
                else
                {
                    galleryThumbContainer.Append("<li><a  href=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" data-id=\"");
                    galleryThumbContainer.Append(item.GiftCardGraphicId);
                    galleryThumbContainer.Append("\" data-large=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" ><img title=\"");
                    galleryThumbContainer.Append(item.GraphicName);
                    galleryThumbContainer.Append("\" src=\"");
                    galleryThumbContainer.Append(aspxRootPath);
                    galleryThumbContainer.Append(imagePath);
                    galleryThumbContainer.Append("\" ></a></li>");
                }

            }
        }
        galleryContainer.Append("</ul></div>");
        galleryThumbContainer.Append("</div>");
        ltrItemGallery.Text = galleryContainer.ToString();
        ltrItemThumb.Text = galleryThumbContainer.ToString();
    }
}