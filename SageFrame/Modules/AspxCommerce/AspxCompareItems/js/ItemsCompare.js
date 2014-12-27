var ItemsCompareAPI = "";
 (function ($) {
    $.ItemCompareView = function (p) {
        p = $.extend
        ({
            EnableCompareItem: '',
            CompareItemListURL: '',
            MaxCompareItemCount: 3,
            CompareLen: 0,
            DefaultImagePath: '',
            ButtonTemplate: "<label><input type='checkbox' onclick='ItemsCompareAPI.Add(${Params});' id='${ID}'><span>"+getLocale(AspxItemsCompare,"Compare")+"</span></label>",
            ItemDetailButtonTemplate:'<label class="i-compare cssCompareLabel"><button type="button" id={ID} onclick="ItemsCompareAPI.AddToCompareFromDetails(${Params})" >'+getLocale(AspxItemsCompare,"Compare +")+'</button></label>',
            ServicePath: aspxservicePath        }, p);

        var aspxCommonObj = function () {
            var aspxCommonInfo = {
                StoreID: AspxCommerce.utils.GetStoreID(),
                PortalID: AspxCommerce.utils.GetPortalID(),
                UserName: AspxCommerce.utils.GetUserName(),
                CultureName: AspxCommerce.utils.GetCultureName(),
                sessionCode: AspxCommerce.utils.GetSessionCode()
            };
            return aspxCommonInfo;
        };
        var ip = AspxCommerce.utils.GetClientIP();
        var countryName = AspxCommerce.utils.GetAspxClientCoutry();
        var costVariantIds = "";
        var costVariantID = "";
        var itemId = 0;
        var Imagelist = '';

        var ItemsCompare = {
            config: {
                isPostBack: false,
                async: true,
                cache: false,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: "json",
                baseURL: p.ServicePath + "Handler.ashx/",//aspxRootPath + "Modules/AspxCommerce/AspxCompareItems/CompareItemsHandler.ashx/",
                url: "",
                method: "",
                ajaxCallMode: ""
            },

            ajaxCall: function (config) {
                $.ajax({
                    type: ItemsCompare.config.type,
                    contentType: ItemsCompare.config.contentType,
                    cache: ItemsCompare.config.cache,
                    async: ItemsCompare.config.async,
                    data: ItemsCompare.config.data,
                    dataType: ItemsCompare.config.dataType,
                    url: ItemsCompare.config.url,
                    success: ItemsCompare.config.ajaxCallMode,
                    error: ItemsCompare.ajaxFailure
                });
            },
            GetCompareItemList: function () {
                this.config.url = this.config.baseURL + "GetItemCompareList";
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj() });
                this.config.ajaxCallMode = ItemsCompare.BindItemCompareList;
                this.ajaxCall(this.config);
            },            

            DeleteCompareItem: function (compareId) {
                this.config.url = this.config.baseURL + "DeleteCompareItem";
                this.config.data = JSON2.stringify({ compareItemID: compareId, aspxCommonObj: aspxCommonObj() });
                this.config.ajaxCallMode = null;
                this.ajaxCall(this.config);

                var ds = ItemsCompare.CompareProductLists();
                var variant = costVariantIds.substring(0, costVariantIds.length - 1);

                $.cookies.set("ItemCompareDetail", ds);
                $.cookies.set("showCompareList", 'true');
                $.cookies.set("costVariant", variant);
            },

            CheckCompareItem: function () {              
                this.config.method = "CheckCompareItems";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ ID: itemId, aspxCommonObj: aspxCommonObj(), costVariantValueIDs: costVariantID });
                this.config.ajaxCallMode = ItemsCompare.BindCheckCompareItems;
                this.config.async = true;
                this.ajaxCall(this.config);
            },

            BindCheckCompareItems: function (data) {
                if (data.d) {
                    csscody.alert('<h2>' + getLocale(AspxItemsCompare, 'Information Alert') + '</h2><p>' + getLocale(AspxItemsCompare, 'The selected item already exist in compare list.') + '</p>');
                    $('#compare-' + itemId + '').attr('checked', true);
                    return false;
                } else {
                    ItemsCompare.AddToCompareBox();
                }
            },

            GetItemDetails: function () {
                this.config.method = "GetItemDetailsForCompare";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ ItemID: itemId, aspxCommonObj: aspxCommonObj(), costVariantValueIDs: costVariantID });
                this.config.ajaxCallMode = ItemsCompare.BindAddToCompareProducts;
                this.config.async = false;
                this.ajaxCall(this.config);
            },
            //Send the list of images to the ImageResizer
            ResizeImageDynamically: function (Imagelist) {
                var ImageType = {
                    "Large": "Large",
                    "Medium": "Medium",
                    "Small": "Small"
                };
                ItemsCompare.config.method = "DynamicImageResizer";
                ItemsCompare.config.url = aspxservicePath + "AspxImageResizerHandler.ashx/" + ItemsCompare.config.method;
                ItemsCompare.config.data = JSON2.stringify({ imgCollection: Imagelist, type: ImageType.Small,imageCatType: "Item", aspxCommonObj: aspxCommonObj() });
                ItemsCompare.config.ajaxCallMode = ItemsCompare.ResizeImageSuccess;
                ItemsCompare.ajaxCall(ItemsCompare.config);

            },
            ResizeImageSuccess: function () {
            },

              BindAddToCompareProducts: function (data) {
                var html = '';
                if (data.d != '') {
                    Imagelist = '';
                    var prodImage = itemImagePath + data.d.ImagePath;
                    if (data.d.ImagePath == "") {
                        prodImage = p.DefaultImagePath;
                    }
                    else {
                        Imagelist = '';
                        Imagelist = data.d.ImagePath + ';';

                    }
                    if (Imagelist.length > 0) {
                        ItemsCompare.ResizeImageDynamically(Imagelist);
                    }
                   
                    var prodName = data.d.ItemName;
                    var costVariantValue = data.d.ItemCostVariantValue;
                    html = '<div id="compareCompareClose-' + itemId + '" onclick="ItemsCompareAPI.RemoveFromAddToCompareBox(' + itemId + ',' + compareID + ');" class="compareProductClose"><i class="i-close">cancel</i></div>';
                    if (costVariantValue != '') {
                        html += '<div class="productImage"><img src="' + aspxRootPath + prodImage.replace('uploads', 'uploads/Small') + '"></div><div class="productName">' + prodName + '<br/>' + costVariantValue + '</div>';
                    } else {
                        html += '<div class="productImage"><img src="' + aspxRootPath + prodImage.replace('uploads', 'uploads/Small') + '"></div><div class="productName">' + prodName + '</div>';
                    }
                    $('.productBox').eq(p.CompareLen - 1).removeClass('empty').addClass('compareProduct').attr('id', 'compareProductBox-' + compareID).attr('data', itemId).attr('costVariant', costVariantID).html(html);
                    $('.fixed').show();
                    $('#compareProductsContainer').show();
                    $(".cssCompareBtnWrapper").hide();
                }
            },

            BindItemCompareList: function (data) {
                if (data.d.length > 0) 
                {
                    $("#compareProductsBox").html('');
                    p.CompareLen = 0;
                    var bindData = '';
                    costVariantIds = '';
                    
                    $.each(data.d, function (index, item) {
                        if (index < p.MaxCompareItemCount) {
                            item.ImagePath = itemImagePath + item.ImagePath;
                            bindData += '<div class="productBox compareProduct" id="compareProductBox-' + item.CompareItemID + '" data=' + item.ItemID + ' costVariant=' + item.CostVariantValueID + '>';
                            bindData += '<div id="compareCompareClose-' + item.ItemID + '" onclick="ItemsCompareAPI.RemoveFromAddToCompareBox(' + item.ItemID + ',' + item.CompareItemID + ');" class="compareProductClose">cancel</div>';
                            bindData += '<div class="productImage"><img src="' + aspxRootPath + item.ImagePath.replace('uploads', 'uploads/Small') + '"></div>';
                            if (item.ItemCostVariantValue != '') {
                                bindData += '<div class="productName">' + item.ItemName + '<br/>' + item.ItemCostVariantValue + '</div></div>';
                            } else {
                                bindData += '<div class="productName">' + item.ItemName + '</div></div>';
                            }
                            costVariantIds += item.CostVariantValueID + "#";
                            if (item.CostVariantValueID == '0') {
                                $('#compare-' + item.ItemID).attr('checked', 'checked');
                            }
                            p.CompareLen++;
                        }
                    });
                }
                if ((p.MaxCompareItemCount - data.d.length) > 0) {
                    for (var i = 0; i < (p.MaxCompareItemCount - data.d.length) ; i++) {
                        bindData += "<div class=\"empty productBox\"></div>";
                    }
                }
                $("#compareProductsBox").append(bindData);
                $('#compareProductsContainer').hide();              
                $("#compareError").append('<div id="compareErrorText">' + getLocale(AspxItemsCompare, 'Sorry, You can not add more than ') + '' + p.MaxCompareItemCount + '&nbsp' + getLocale(AspxItemsCompare, "items") + '.</div>');
            },

            AddToMyCompare: function () {          
                var SaveCompareItemObj = {
                    ItemID: itemId,
                    CostVariantIDs: costVariantID,
                    IP: ip,
                    CountryName: countryName
                };
                this.config.method = "SaveCompareItems";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj(), saveCompareItemObj: SaveCompareItemObj });
                this.config.ajaxCallMode = ItemsCompare.BindCompareID;
                this.ajaxCall(this.config);
            },

            BindCompareID: function (data) {
                compareID = data.d;
                ItemsCompare.GetItemDetails();
            },  
          
            AddItemsToCompare: function (itemId, itemSKU, elem) {               
                if (Boolean.parse($.trim($(elem).offsetParent().find(".cssClassProductsBoxInfo").attr("costvariantItem")))) {
                    AspxCommerce.RootFunction.RedirectToItemDetails(itemSKU);
                } else {               
                    var costVariantIds = '0';
                    ItemsCompareAPI.AddToCompare(itemId, costVariantIds,elem);
                }
            },
            AddToCompare: function (Id, costVariantId, elem) {               
                itemId = Id;
                costVariantID = costVariantId;
                if ($(elem).is(':checked')) {
                    ItemsCompare.CheckCompareItem();                   
                }
                else {               
                    var compareIdValue = $('#compareCompareClose-' + itemId).parent('div').attr('id');
                    var compareIdRemove = compareIdValue.split('-')[1];
                    ItemsCompare.RemoveFromAddToCompareBox(itemId, compareIdRemove);
                    $(".cssCompareBtnWrapper").hide();
                }
                if ($("#compareImgClose").length == 0) {
                    $(".cssClassClose").html("<img id='compareImgClose' src='" + aspxRootPath + "Modules/AspxCommerce/AspxCompareItems/images/compare-close.png' alt='close' />");
                }
            },

            AddToCompareFromDetails: function (itemID, itemSKU, elem) {
               itemId = itemID;                
                 var itemCostVariantIDs = [];
                    if ($('#divCostVariant').is(':empty')) {           
                        itemCostVariantIDs.push(0);
                                              costVariantID='0';
                    } else {
                        $("#divCostVariant select option:selected").each(function () {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs.push($(this).val());
                            } else {
                            }
                        });
                        $("#divCostVariant input[type=radio]:checked").each(function () {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs.push($(this).val());
                            } else {
                            }
                        });

                        $("#divCostVariant input[type=checkbox]:checked").each(function () {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs.push($(this).val());
                            } else {
                            }
                        });
                        costVariantID=itemCostVariantIDs.join('@');            
                    }
                ItemsCompare.CheckCompareItem();
           },

            AddToCompareBox: function () {
                if (p.CompareLen >= parseInt(p.MaxCompareItemCount)) {
                    $('#compareError').show();
                    $('#compare-' + itemId + '').attr('checked', false);
                    $(".cssCompareBtnWrapper").hide();
                    $('#compareProductsContainer').show();
                    return false;
                }
                ItemsCompare.AddToMyCompare();  
                p.CompareLen++;         
                return true;
            },

            RemoveFromAddToCompareBox: function (itemID, compareId) {               
                $('#compareProductsContainer').show();
                $('#compareError').hide();
                $('#compareProductBox-' + compareId).remove();
                $('#compareProductsBox').append("<div class='empty productBox'></div>");
                $('#compare-' + itemID).attr('checked', false);
                ItemsCompare.DeleteCompareItem(compareId);
                p.CompareLen--;                
                if (p.CompareLen == 0) {
                    $('#compareProductsContainer').hide();
                    $(".cssCompareBtnWrapper").hide();
                    $(".fixed").hide();
                }
            },
            BindCompareEmptyBox: function () {
                $("#compareProductsBox").html('');
                var emptyHtml = '';
                for (var i = 1; i <= parseInt(p.MaxCompareItemCount) ; i++) {
                    emptyHtml += "<div class='empty productBox'></div>";
                }
                $("#compareProductsBox").html(emptyHtml);
                $("#compareError").append('<div id="compareErrorText">' + getLocale(AspxItemsCompare, 'Sorry, You cant add more than ') + ' ' + p.MaxCompareItemCount + '&nbsp' + 'items.</div>');
            },
            CompareProductLists: function () {
                var comparedProdIds = '';
                costVariantIds = '';
                $('.compareProduct').each(function () {
                    if ($(this).is(":visible")) {
                        comparedProdIds += $(this).attr('data') + '#';
                        var costvariant = $(this).attr('costVariant') + '#';
                        costVariantIds += costvariant;
                    }
                });
                comparedProdIds = comparedProdIds.substring(0, comparedProdIds.lastIndexOf("#"));
                return comparedProdIds;
            },
            ClearAll: function () {               
                if (p.CompareLen > 0) {               
                    var ids = [];
                    var itemIDs = ItemsCompare.CompareProductLists();
                    ids = itemIDs.split('#');
                    $.each(ids, function (index, value) {
                        if (value != '') {
                            var compareIdValue = $('#compareCompareClose-' + value).parent('div').attr('id');
                            var compareId = compareIdValue.split('-')[1];
                            ItemsCompare.RemoveFromAddToCompareBox(value, compareId);
                        }
                    });
                    p.CompareLen = 0;
                }
            },

            ExtendCore: function () {

            },
            EnableCompare: function () {             
                var template = p.ButtonTemplate;
                $.template("ItemCompareButton", template); 
                       
                $("input:hidden[name='itemcompare']").each(function (index, item) {                
                    var value = $(this).val();
                    if(value !=""){
                    if (!$(this).attr('done')) {
                        $(this).attr('done', 1);
                        var itemid = value.split(',')[0];
                        var param = { Params: value, ID: 'compare-' + itemid };
                        $.tmpl("ItemCompareButton", param).insertAfter($(this));
                    }
                  }
                });

                               var template = p.ItemDetailButtonTemplate;
                 $.template("ItemCompareButton", template); 
                   $("input:hidden[name='itemDetailCompare']").each(function (index, item) {                
                    var value = $(this).val();
                    if(value !=""){
                    if (!$(this).attr('done')) {
                        $(this).attr('done', 1);
                        var itemid = value.split(',')[0];
                        var param = { Params: value, ID: 'compare-' + itemid };
                        $.tmpl("ItemCompareButton", param).insertAfter($(this));
                    }
                  }
                });
            },
            Init: function () {              
                if (p.EnableCompareItem.toLowerCase() == 'true') {                  
                    $('#compareCloseBtn').click(function () {
                        $('#compareProductsContainer').hide();
                        $(".cssCompareBtnWrapper").show();
                    });
                    if (p.CompareLen > 0) {
                        $(".fixed").show();
                        $(".cssCompareBtnWrapper").show();
                        $(".cssClassClose").html("<img id='compareImgClose' src='" + aspxRootPath + "Modules/AspxCommerce/AspxCompareItems/images/compare-close.png' alt='close' />")
                    }
                    $('#btnCompare').click(function () {
                        if (p.CompareLen > 1) {
                            var ds = ItemsCompare.CompareProductLists();
                            var variant = costVariantIds.substring(0, costVariantIds.length - 1);
                            $.cookies.set("ItemCompareDetail", ds);
                            $.cookies.set("showCompareList", 'true');
                            $.cookies.set("costVariant", variant);
                            window.location.href = aspxRedirectPath + p.CompareItemListURL + pageExtension;
                        } else {
                            csscody.alert("<h2>" + getLocale(AspxItemsCompare, "Information Alert") + "</h2><p>" + getLocale(AspxItemsCompare, "You must have more than one item to compare!") + "</p>");

                        }
                    });
                    $(".cssCompareBtnShow").on('click', function () {
                        $("#compareProductsContainer").show();
                        $(".cssCompareBtnWrapper").hide();
                    });
                    $('#btnCompareClearAll').click(function () {
                        ItemsCompare.ClearAll();
                    });                  
                    ItemsCompare.EnableCompare();

                    $(document).bind('DOMNodeInserted', function (event) {
                        ItemsCompare.EnableCompare();
                    });
                }
            }
        };

        ItemsCompare.Init();
        ItemsCompareAPI = function () {
            return {
                Add: ItemsCompare.AddItemsToCompare,
                AddToCompare:ItemsCompare.AddToCompare,
                AddToCompareFromDetails:ItemsCompare.AddToCompareFromDetails,
                RemoveFromAddToCompareBox:ItemsCompare.RemoveFromAddToCompareBox
            };
        }();
    };
    $.fn.ItemCompare = function (p) {
        $.ItemCompareView(p);
    };
})(jQuery);