var ItemImageGalleryApi = "";
Variable = function (height, width, thumbWidth, thumbHeight) {
    this.height = height;
    this.width = width;
    this.thumbHeight = thumbHeight;
    this.thumbWidth = thumbWidth;
};
var newObject = new Variable(255, 320, 87, 75);
(function ($) {
    $.ItemImageGalleryView = function (p) {
        p = $.extend
    ({
        ItemImageGalleryModulePath: '',
        referImagePath: ''
    }, p);

        var aspxCommonObj = function () {
            var aspxCommonInfo = {
                StoreID: AspxCommerce.utils.GetStoreID(),
                PortalID: AspxCommerce.utils.GetPortalID(),
                UserName: AspxCommerce.utils.GetUserName(),
                CultureName: AspxCommerce.utils.GetCultureName(),
                CustomerID: AspxCommerce.utils.GetCustomerID(),
                SessionCode: AspxCommerce.utils.GetSessionCode()
            }
        };
        var Imagelist = '';
        ItemImageGallery = {
            config: {
                isPostBack: false,
                async: true,
                cache: true,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: "json",
                baseURL: AspxCommerce.utils.GetAspxServicePath() + "AspxCoreHandler.ashx/",
                url: "",
                method: "",
                ajaxCallMode: ""
            },
            vars:
               {
                   aspxCommonInfo: {
                       StoreID: AspxCommerce.utils.GetStoreID(),
                       PortalID: AspxCommerce.utils.GetPortalID(),
                       UserName: AspxCommerce.utils.GetUserName(),
                       CultureName: AspxCommerce.utils.GetCultureName(),
                       CustomerID: AspxCommerce.utils.GetCustomerID(),
                       SessionCode: AspxCommerce.utils.GetSessionCode()
                   }

               },

            ajaxCall: function (config) {
                $.ajax({
                    type: ItemImageGallery.config.type,
                    contentType: ItemImageGallery.config.contentType,
                    cache: ItemImageGallery.config.cache,
                    async: ItemImageGallery.config.async,
                    data: ItemImageGallery.config.data,
                    dataType: ItemImageGallery.config.dataType,
                    url: ItemImageGallery.config.url,
                    success: ItemImageGallery.config.ajaxCallMode,
                    error: ItemImageGallery.ajaxFailure
                });
            },

            init: function () {
                $('.cssClassProductBigPicture').attr('imagepath', p.referImagePath);
                var windowsWidth = $(window).width();               
                if (windowsWidth > 800) {
                    $('#multizoom1').addimagezoom({
                        zoomrange: [3, 10],
                        magnifiersize: [600, 300],
                        disablewheel: true
                    });
                }               
                $(".cssClassProductBigPicture").show();
                $('.multizoom1').jcarousel({
                    vertical: false,
                    scroll: 1,
                    itemFallbackDimension: 300
                });
            },

            GetImageLists: function (itemId, itemTypeId, sku, combinationId, aspxCommonObj) {               
                if (itemTypeId == 3) {
                    ItemImageGallery.GetGiftCardThemes(itemId, aspxCommonObj);
                } else {
                    this.config.method = "GetItemsImageGalleryInfoBySKU";
                    this.config.url = this.config.baseURL + this.config.method;
                    this.config.data = JSON2.stringify({ itemSKU: sku, aspxCommonObj: aspxCommonObj, combinationId: combinationId });
                    this.config.ajaxCallMode = ItemImageGallery.BindItemsImageGallery;
                    this.ajaxCall(this.config);
                }
            },

            BindItemsImageGallery: function (msg) {                
                $(".targetarea").html('');
                $(".multizoom1").html('');
                var windowsWidth = $(window).width();
                if (msg.d.length > 0) {
                    var bindImage = '';
                    var bindImageThumb = '';                   
                    $.each(msg.d, function (index, item) {
                        var imagePath = itemImagePath + item.ImagePath;
                        if (item.ImagePath == "") {
                            imagePath = noItemDetailImagePath;
                        }
                        else {
                            Imagelist += item.ImagePath + ';';
                        }

                        if (index == 0) {
                            $('.cssClassProductBigPicture').attr('imagepath', imagePath);
                            bindImage = "<img  id='multizoom1' title='" + item.AlternateText + "' src='" + aspxRootPath + imagePath.replace('uploads', 'uploads/Large') + "'>";
                            bindImageThumb += '<li><a  href="' + aspxRootPath + imagePath.replace('uploads', 'uploads/Large') + '" data-large="' + aspxRootPath + imagePath + '"><img title="' + item.AlternateText + '" src="' + aspxRootPath + imagePath.replace('uploads', 'uploads/Small') + '" ></a></li>';
                            var href = aspxRootPath + imagePath.replace('uploads', 'uploads/Medium')
                            $(".st_facebook_hcount").attr("st_image", href)
                        } else {
                            bindImageThumb += '<li><a  href="' + aspxRootPath + imagePath.replace('uploads', 'uploads/Large') + '" data-large="' + aspxRootPath + imagePath + '" ><img title="' + item.AlternateText + '" src="' + aspxRootPath + imagePath.replace('uploads', 'uploads/Small') + '" ></a></li>';

                        }
                    });
                    ImageType = {
                        "Large": "Large",
                        "Medium": "Medium",
                        "Small": "Small"
                    };
                    ImageTypes = ImageType.Large + ';' + ImageType.Small;
                    ItemImageGallery.ResizeImageDynamically(Imagelist, ImageTypes);
                    $(".targetarea").append(bindImage);
                    if (windowsWidth > 800) {
                        $(".multizoom1").append("<ul>" + bindImageThumb + "</ul>");
                    }                   
                } else {
                    var bindImage = '';
                    var bindImageThumb = '';
                    bindImage = "<img  id='multizoom1' src='" + aspxRootPath + noItemDetailImagePath.replace('uploads', 'uploads/Large') + "'>";
                    bindImageThumb += '<li><a  href="' + aspxRootPath + noItemDetailImagePath.replace('uploads', 'uploads/Large') + '" data-large="' + aspxRootPath + noItemDetailImagePath + '"><img  src="' + aspxRootPath + noItemDetailImagePath.replace('uploads', 'uploads/Small') + '" ></a></li>';
                    $(".targetarea").append(bindImage);
                    if (windowsWidth > 800) {
                        $(".multizoom1").append("<ul>" + bindImageThumb + "</ul>");
                    }                  
                }
                $('.multizoom1').jcarousel({
                    vertical: false,
                    scroll: 1,
                    itemFallbackDimension: 300
                });
                if (windowsWidth > 800) {                   
                    $('#multizoom1').addimagezoom({
                        zoomrange: [3, 10],
                        magnifiersize: [600, 300],
                        disablewheel: true
                    });
                }
            },
            GetGiftCardThemes: function () {
                var param = JSON2.stringify({ itemId: itemId, aspxCommonObj: aspxCommonObj() });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    url: aspxservicePath + 'AspxCoreHandler.ashx/GetGiftCardThemeImagesByItem',
                    data: param,
                    dataType: "json",
                    success: function (data) {
                        $(".targetarea").html('');
                        $(".multizoom1").html('');
                        if (data.d.length > 0) {
                            var bindImage = '';
                            var bindImageThumb = '';
                            $.each(data.d, function (index, item) {
                                if (item.GraphicImage != "" && item.GraphicImage != null) {
                                    var ImageArr = item.GraphicImage.split('/');
                                    Imagelist += ImageArr[ImageArr.length - 1] + ';';
                                }
                                if (index == 0) {
                                    $('.cssClassProductBigPicture').attr('imagepath', aspxRootPath + item.GraphicImage);
                                    bindImage = "<img  id='multizoom1' title='" + item.GraphicName + "' src='" + aspxRootPath + item.GraphicImage + "'>";
                                    bindImageThumb += '<li><a class="selected" data-id="' + item.GiftCardGraphicId + '"  href="' + aspxRootPath + item.GraphicImage + '" data-large="' + aspxRootPath + item.GraphicImage + '"><img title="' + item.GraphicName + '" src="' + aspxRootPath + item.GraphicImage + '" ></a></li>';
                                } else {
                                    bindImageThumb += '<li><a data-id="' + item.GiftCardGraphicId + '"  href="' + aspxRootPath + item.GraphicImage + '" data-large="' + aspxRootPath + item.GraphicImage + '" ><img title="' + item.GraphicName + '" src="' + aspxRootPath + item.GraphicImage + '" ></a></li>';
                                }
                            });
                            $(".targetarea").append(bindImage);
                            $(".multizoom1").append("<ul>" + bindImageThumb + "</ul>");
                            var windowsWidth = $(window).width();
                            if (windowsWidth >= 760) {
                                $('#multizoom1').addimagezoom({
                                    zoomrange: [3, 10],
                                    magnifiersize: [600, 300],
                                    disablewheel: true
                                });
                            }
                            $('.multizoom1').jcarousel({
                                vertical: false,
                                scroll: 1,
                                itemFallbackDimension: 300
                            });
                        }
                    }
                });
            },
            ResizeImageDynamically: function (Imagelist, ImageTypes) {
                ItemDetail.config.method = "MultipleImageResizer";
                ItemDetail.config.url = aspxservicePath + "AspxImageResizerHandler.ashx/" + ItemDetail.config.method;
                ItemDetail.config.data = JSON2.stringify({ imgCollection: Imagelist, types: ImageTypes, imageCatType: "Item", aspxCommonObj: ItemImageGallery.vars.aspxCommonInfo });
                ItemDetail.config.ajaxCallMode = ItemDetail.ResizeImageSuccess;
                ItemDetail.ajaxCall(ItemDetail.config);
            },
            ResizeImageSuccess: function () {
            }

        };
        ItemImageGallery.init();       
        ItemImageGalleryApi = function () {
            return {
                ReloadItemImageGallery: function (itemId,itemTypeId, sku, combinationId, aspxCommonObj) {
                    ItemImageGallery.GetImageLists(itemId,itemTypeId, sku, combinationId, aspxCommonObj)
                }
            };
        }();       
    };
    $.fn.ItemImageGallery = function (p) {
        $.ItemImageGalleryView(p);
    };
})(jQuery);