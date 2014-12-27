(function ($) {
    $.BrandSlideView = function (p) {
        p = $.extend
        ({
            brandModulePath: '',
            enableBrand: '',
            brandCount: 0,
            brandAllPage: '',
            enableBrandRss: '',
            brandRssPage: ''
        }, p);
        var aspxCommonObj = {
            StoreID: AspxCommerce.utils.GetStoreID(),
            PortalID: AspxCommerce.utils.GetPortalID(),
            CultureName: AspxCommerce.utils.GetCultureName()
        };
        var BrandSlide = {
            config: {
                isPostBack: false,
                async: true,
                cache: true,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: 'json',
                baseURL: p.brandModulePath + "/Services/AspxBrandViewServices.asmx/",
                method: "",
                url: "",
                ajaxCallMode: "",
                error: ""
            },
            ajaxCall: function (config) {
                $.ajax({
                    type: BrandSlide.config.type,
                    contentType: BrandSlide.config.contentType,
                    cache: BrandSlide.config.cache,
                    async: BrandSlide.config.async,
                    url: BrandSlide.config.url,
                    data: BrandSlide.config.data,
                    dataType: BrandSlide.config.dataType,
                    success: BrandSlide.config.ajaxCallMode,
                    error: BrandSlide.config.error
                });
            },
            GetAllBrandForSlider: function () {
                this.config.url = this.config.baseURL + "GetAllBrandForSlider";
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj });
                this.config.ajaxCallMode = BrandSlide.GetAllBrandForSliderSucess;
                this.ajaxCall(this.config);
            },

            GetAllBrandForSliderSucess: function (msg) {
                var element = '';
                $("#brandSlider").html('');
                if (msg.d.length > 0) {
                    $.each(msg.d, function (index, value) {
                        rowTotal = value.RowTotal;
                        var imagepath = aspxRootPath + value.BrandImageUrl;
                        element += "<li><a href='" + aspxRedirectPath + "brand/" + value.BrandName + pageExtension + "'><img brandId='" + value.BrandID + "' src='" + imagepath.replace('uploads', 'uploads/Small') + "'   alt='" + value.BrandName + "' title='" + value.BrandName + "'  /></a></li>";
                    });
                    $("#brandSlider").append(element);
                    $("#brandSlider").BxSlider({
                        auto: true,
                        slideWidth: 410,
                        minSlides: 8,
                        maxSlides: 8,
                        moveSlides: 1,
                        slideMargin: 10,
                        controls: false
                    });
                    $("#slide").append('<a class="cssClassReadMore" href=' + aspxRedirectPath + 'Brands' + pageExtension + '>' + getLocale(AspxRssFeedLocale, "View All Brands") + '</a>');
                }
                else {
                    $("#slide").append("<span class='cssClassNotFound'>" + getLocale(AspxBrandView, "The store has no brand!") + "</span>");
                }
            },
            LoadFrontBrandRssImage: function () {
                var pageurl = aspxRedirectPath + p.brandRssPage + pageExtension;
                $('#frontBrandRssImage').parent('a').show();
                $('#frontBrandRssImage').parent('a').removeAttr('href').attr('href', pageurl + '?type=brands');
                $('#frontBrandRssImage').removeAttr('src').attr('src', aspxTemplateFolderPath + '/images/rss-icon.png');
                $('#frontBrandRssImage').removeAttr('title').attr('title', getLocale(AspxBrandView, "Popular Brands Rss Feed Title"));
                $('#frontBrandRssImage').removeAttr('alt').attr('alt', getLocale(AspxBrandView, "Popular Brands Rss Feed Alt"));
            },
            Init: function () {
                if (p.enableBrand.toLowerCase() == "true") {
                    $(".cssClassBrandWrapper").show();
                }
                if (p.enableBrandRss.toLowerCase() == "true") {
                    BrandSlide.LoadFrontBrandRssImage();
                }
               setTimeout(function(){
                    $("#brandSlider").bxSlider({
                        auto: true,
                        slideWidth: 120,
                        minSlides: 2,
                        maxSlides: 10,
                        moveSlides: 1,
                        slideMargin: 10,
                        controls: false
                    });
                    },500);
                $('#brandSlider a img[title]').tipsy({ gravity: 'n' });

            }
        };
        BrandSlide.Init();
    };
    $.fn.BrandSlide = function (p) {
        $.BrandSlideView(p);
    };
})(jQuery);