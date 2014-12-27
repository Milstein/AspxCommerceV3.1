
var LatestItemAPI = "";
(function($) {
    $.fn.LatestItemView = function(p) {
        p = $.extend
        ({
            UsrModuleID: 0,
            countryName: '',
            defaultImagePath: '',
            noOfLatestItems: 0,
            noOfLatestItemsInARow: 0,
            allowAddToCart: '',
            allowWishListLatestItem: '',
            allowAddToCompareLatest: '',
            allowOutStockPurchase: '',
            maxCompareItemCount: '',
            enableLatestItems: '',
            aspxFilePath: '',
            lblListPrice: '',
            lblSaving: '',
            lblItemQnty: '',
            tblRecentItems: '',
            latestItemRss: '',
            rssFeedUrl: '',
            latestItemModPath: ''
        }, p);
        function SwapImageOnMouseOver(id, altImage) {
            $('#' + id).attr('src', altImage);
        }
        function SwapImageOnMouseOut(id, altImage) {
            $('#' + id).attr('src', altImage);
        }
        Variable = function(height, width, thumbWidth, thumbHeight) {
            this.height = height;
            this.width = width;
            this.thumbHeight = thumbHeight;
            this.thumbWidth = thumbWidth;
        };
        var newObject = new Variable(255, 320, 87, 75);
        var LatestItems = "";
        var costVariantValueIDs = "";

        var userModuleID = p.UsrModuleID;
        var storeId = AspxCommerce.utils.GetStoreID();
        var portalId = AspxCommerce.utils.GetPortalID();
        var userName = AspxCommerce.utils.GetUserName();
        var cultureName = AspxCommerce.utils.GetCultureName();
        var customerId = AspxCommerce.utils.GetCustomerID();
        var ip = AspxCommerce.utils.GetClientIP();
        var countryName = AspxCommerce.utils.GetAspxClientCoutry();
        var sessionCode = AspxCommerce.utils.GetSessionCode();
        var userFriendlyURL = AspxCommerce.utils.IsUserFriendlyUrl();
        var itemId = '';
        var itemName = '';
        var itemSKU = '';
        var RelatedItems = '';
        var arrCostVariants;
        var FormCount = new Array();
        var arrCombination = [];
        var variantValuesID = [];
        var itemTypeId = 0;
        var aspxCommonObj = function() {
            var aspxCommonInfo = {
                StoreID: AspxCommerce.utils.GetStoreID(),
                PortalID: AspxCommerce.utils.GetPortalID(),
                UserName: AspxCommerce.utils.GetUserName(),
                CultureName: AspxCommerce.utils.GetCultureName(),
                SessionCode: AspxCommerce.utils.GetSessionCode(),
                CustomerID: AspxCommerce.utils.GetCustomerID()
            };
            return aspxCommonInfo;
        };
        GetCount = function(id) {
            var count = 0;
            for (var i = 0; i < variantValuesID.length; i++) {
                if (variantValuesID[i] == id) {
                    count++;
                }
            }
            return count;
        };
        CheckContains = function(checkon, toCheck) {
            var x = checkon.split('@');
            for (var i = 0; i < x.length; i++) {
                if (x[i] == toCheck) {
                    return true;
                }
            }
            return false;
        };
        IsExists = function(arr, val) {
            var isExist = false;
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] == val) {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        };

        function getObjects(obj, key) {
            var objects = [];
            for (var i in obj) {
                if (!obj.hasOwnProperty(i)) continue;
                if (typeof obj[i] == 'object') {
                    objects.push(obj[i][key]);
                }

            }
            return objects;
        }

        function getValByObjects(obj, key, key2, x, y, keyOfValue) {
            var value = '';
            for (var i in obj) {
                if (!obj.hasOwnProperty(i)) continue;
                if (typeof obj[i] == 'object') {
                    if (obj[i][key] == x && obj[i][key2] == y) {
                        value = obj[i][keyOfValue];
                    }
                }

            }
            return value;
        }

        function getModifiersByObjects(obj, combinationId) {
            var modifiers = { Price: '', IsPricePercentage: false, Weight: '', IsWeightPercentage: false, Quantity: 0 };

            for (var i in obj) {
                if (!obj.hasOwnProperty(i)) continue;
                if (typeof obj[i] == 'object') {
                    if (obj[i]["CombinationID"] == combinationId) {
                        modifiers.Price = obj[i]["CombinationPriceModifier"];
                        modifiers.IsPricePercentage = obj[i]["CombinationPriceModifierType"];
                        modifiers.Weight = obj[i]["CombinationWeightModifier"];
                        modifiers.IsWeightPercentage = obj[i]["CombinationWeightModifierType"];
                        modifiers.Quantity = obj[i]["CombinationQuantity"];
                    }
                }

            }
            return modifiers;
        }

        var info = {
            IsCombinationMatched: false,
            AvailableCombination: [],
            CombinationID: 0,
            PriceModifier: '',
            IsPricePercentage: false,
            WeightModifier: '',
            IsWeightPercentage: false,
            Quantity: 0
        };

        function checkAvailibility(elem) {
            var cvids = [];
            var values = [];
            var currentValue = elem == null ? 1 : $(elem).val();
            var currentCostVariant = elem == null ? 1 : $(elem).parents('span:eq(0)').attr('id').replace('subDiv', '');
            $("#divCostVariant select option:selected").each(function() {               
                if (this.value != 0) {
                    values.push(this.value);
                    cvids.push($(this).parents("span:eq(0)").attr('id').replace('subDiv', ''));
                }
            });

            $("#divCostVariant input[type=radio]:checked").each(function() {
                $(this).is(":checked") ? $(this).addClass("cssRadioChecked") : $(this).removeClass("cssRadioChecked");
                values.push(this.value);
                cvids.push($(this).parents("span:eq(0)").attr('id').replace('subDiv', ''));
            });

            $("#divCostVariant input[type=radio]").each(function() {
                $(this).is(":checked") ? $(this).addClass("cssRadioChecked") : $(this).removeClass("cssRadioChecked");
            });

            $("#divCostVariant input[type=checkbox]:checked").each(function() {
                values.push(this.value);
                cvids.push($(this).parents("span:eq(0)").attr('id').replace('subDiv', ''));
            });
            var infos = CheckVariantCombination(cvids.join('@'), values.join('@'), currentCostVariant, currentValue);
            $("#spanAvailability").html(info.IsCombinationMatched == true ? '<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>' : '<b>' + getLocale(AspxLatestItem, 'Not Available') + '</b>');
            info.IsCombinationMatched == true ? $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart")) : $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));
            if (info.IsCombinationMatched) {
                $("#hdnQuantity").val('').val(info.Quantity);
                $("#txtQty").removeAttr('disabled').attr("enabled", "enabled");
                if (info.Quantity == 0 || info.Quantity < 0) {
                    if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                        $("#spanAvailability").html("<b>" + getLocale(AspxLatestItem, "Out Of Stock") + "</b>");
                    }

                } else {
                    $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>' + '</b>');
                }
                var quantityinCart = LatestItems.CheckItemQuantityInCart(itemId, values.join('@') + '@');
                if (info.Quantity == quantityinCart) {
                    if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                        $("#txtQty").removeAttr('enabled').attr("disabled", "disabled");
                        $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));
                        $("#spanAvailability").html("<b>" + getLocale(AspxLatestItem, "Out Of Stock") + "</b>");
                    }
                }
                var price = 0;
                if (info.IsPricePercentage) {
                    price = eval($("#hdnPrice").val()) * eval(info.PriceModifier) / 100;
                } else {
                    price = eval(info.PriceModifier);
                }

                $("#spanPrice").html(parseFloat((eval($("#hdnPrice").val())) + (eval(price))).toFixed(2));
                var taxPriceVariant = parseFloat(($("#hdnPrice").val()) + eval(price)).toFixed(2);
                var taxrate = (eval($("#hdnTaxRateValue").val()) * 100) / (eval($("#hdnPrice").val()));
                $("#spanTax").html(parseFloat((taxPriceVariant * taxrate) / 100).toFixed(2));              
                if ($("#hdnListPrice").val() != null) {
                    $(".cssClassYouSave").show();
                    var variantAddedPrice = eval($("#hdnPrice").val()) + eval(price);
                    var variantAddedSavingPercent = (($("#hdnListPrice").val() - variantAddedPrice) / $("#hdnListPrice").val()) * 100;
                    savingPercent2 = variantAddedSavingPercent.toFixed(2);
                    $("#spanSaving").html('<b>' + variantAddedSavingPercent.toFixed(2) + '%</b>');

                }
                LatestItems.ResetGallery(info.CombinationID);
            } else {
                $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart"));
            }
        }

        function selectFirstcombination() {
            if (arrCombination.length > 0) {
                var cvcombinationList = getObjects(arrCombination, 'CombinationType');
                var cvValuecombinationList = getObjects(arrCombination, 'CombinationValues');
                var x = cvcombinationList[0].split('@');
                var y = cvValuecombinationList[0].split('@');
                $("#divCostVariant select").each(function(i) {
                    if (parseInt($(this).parent("span:eq(0)").attr('id').replace('subDiv', '')) == x[i]) {
                        if ($(this).find("option[value=" + y[i] + "]").length > 0) {
                            $(this).find("option[value=" + y[i] + "]").attr('selected', 'selected');

                        } else {
                            var options = $(this).html();
                            var noOption = "<option value='0'>" + "Not required" + "</option>";
                            $(this).html(noOption + options);
                            $(this).find('option[value=0]').attr('selected', 'selected');
                        }
                    } else {
                        var val = parseInt($(this).parent("span:eq(0)").attr('id').replace('subDiv', ''));
                        var xIndex = 0;
                        for (var indx = 0; indx < x.length; indx++) {
                            if (x[indx] == val) {
                                xIndex = indx;
                                break;
                            }
                        }
                        if ($(this).find("option[value=" + y[xIndex] + "]").length > 0) {
                            $(this).find("option[value=" + y[xIndex] + "]").attr('selected', 'selected');

                        } else {
                            var options = $(this).html();
                            var noOption = "<option value='0'>" + "Not required" + "</option>";
                            $(this).html(noOption + options);
                            $(this).find('option[value=0]').attr('selected', 'selected');
                        }
                    }

                });

                $("#divCostVariant input[type=radio]").each(function(i) {
                    if (parseInt($(this).parents("span:eq(0)").attr('id').replace('subDiv', '')) == x[i]) {
                        if ($(this).val() == y[i]) {
                            $(this).attr('checked', 'checked').addClass("cssRadioChecked");

                        } else {
                            $(this).removeAttr('checked');
                        }
                    } else {
                        var val = parseInt($(this).parents("span:eq(0)").attr('id').replace('subDiv', ''));
                        var xIndex = 0;
                        for (var indx = 0; indx < x.length; indx++) {
                            if (x[indx] == val) {
                                xIndex = indx;
                                break;
                            }
                        }
                        if ($(this).val() == y[xIndex]) {
                            $(this).attr('checked', 'checked').addClass("cssRadioChecked");

                        } else {
                            $(this).removeAttr('checked');
                        }
                    }
                });

                $("#divCostVariant input[type=checkbox]:checked").each(function(i) {
                    if (parseInt($(this).parent("span:eq(0)").attr('id').replace('subDiv', '')) == x[i]) {
                        if ($(this).val() == y[i]) {
                            $(this).attr('checked', 'checked');

                        } else {
                            $(this).removeAttr('checked');
                        }
                    } else {
                        var val = parseInt($(this).parent("span:eq(0)").attr('id').replace('subDiv', ''));
                        var xIndex = 0;
                        for (var indx = 0; indx < x.length; indx++) {
                            if (x[indx] == val) {
                                xIndex = indx;
                                break;
                            }
                        }
                        if ($(this).val() == y[xIndex]) {
                            $(this).attr('checked', 'checked');

                        } else {
                            $(this).removeAttr('checked');
                        }
                    }
                });

                CheckVariantCombination(cvcombinationList[0], cvValuecombinationList[0], x[0], y[0]);
                $("#spanAvailability").html(info.IsCombinationMatched == true ? '<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>' : '<b>' + getLocale(AspxLatestItem, 'Not Available') + '</b>');
                info.IsCombinationMatched == true ? $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart")) : $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));

                if (info.IsCombinationMatched) {
                    $("#hdnQuantity").val('').val(info.Quantity);
                    $("#txtQty").removeAttr('disabled').attr("enabled", "enabled");
                    if (info.Quantity == 0 || info.Quantity < 0) {
                        if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                            $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));
                            $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'Out Of Stock') + '</b>');
                        }

                    } else {
                        $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart"));
                        $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>');
                    }

                    var quantityinCart = LatestItems.CheckItemQuantityInCart(itemId, cvValuecombinationList[0] + '@');
                    if (info.Quantity == quantityinCart) {
                        if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                            $("#txtQty").removeAttr('enabled').attr("disabled", "disabled");
                            $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));
                            $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'Out Of Stock') + '</b>');
                        }
                    }
                    var price = 0;
                    if (info.IsPricePercentage) {
                        price = eval($("#hdnPrice").val()) * eval(info.PriceModifier) / 100;
                    } else {
                        price = eval(info.PriceModifier);
                    }

                    $("#spanPrice").html(parseFloat((eval($("#hdnPrice").val())) + (eval(price))).toFixed(2));
                    var taxPriceVariant = parseFloat(eval($("#hdnPrice").val()) + eval(price)).toFixed(2);
                    var taxrate = parseFloat((eval($("#hdnTaxRateValue").val()) * 100) / (eval($("#hdnPrice").val()))).toFixed(2);
                    if ($("#hdnListPrice").val() != '') {
                        $(".cssClassYouSave").show();
                        var variantAddedPrice = eval($("#hdnPrice").val()) + eval(price);
                        var variantAddedSavingPercent = (($("#hdnListPrice").val() - variantAddedPrice) / $("#hdnListPrice").val()) * 100;
                        savingPercent2 = variantAddedSavingPercent.toFixed(2);
                        $("#spanSaving").html('<b>' + variantAddedSavingPercent.toFixed(2) + '%</b>');

                    }
                    LatestItems.ResetGallery(info.CombinationID);
                }
            } else {
                LatestItems.ResetGallery(0);
            }

        }


        function CheckVariantCombination(costVIds, costValIds, currentCostVar, currentCostVal) {
            info.IsCombinationMatched = false;
            var cvcombinationList = getObjects(arrCombination, 'CombinationType');
            var cvValuecombinationList = getObjects(arrCombination, 'CombinationValues');
            for (var j = 0; j < cvcombinationList.length; j++) {
                if (info.IsCombinationMatched == true)
                    break;
                var matchedIndex = 0;
                var matchedValues = 0;
                if (cvcombinationList[j].length == costVIds.length) {
                    var cb = costVIds.split('@');
                    for (var id = 0; id < cb.length; id++) {
                        if (info.IsCombinationMatched == true)
                            break;
                        var element = cb[id];
                        if (CheckContains(cvcombinationList[j], element)) {
                            matchedIndex++;
                            if (matchedIndex == cb.length) {
                                var cvb = costValIds.split('@');
                                for (var d = 0; d < cvb.length; d++) {
                                    var element1 = cvb[d];
                                    if (CheckContains(cvValuecombinationList[j], element1)) {
                                        matchedValues++;
                                    }
                                    if (matchedValues == cvb.length) {
                                        var combinationId = getValByObjects(arrCombination, 'CombinationType', 'CombinationValues', cvcombinationList[j], cvValuecombinationList[j], 'CombinationID');
                                        var modifiers = getModifiersByObjects(arrCombination, combinationId);
                                        info.IsCombinationMatched = true;
                                        info.CombinationID = combinationId;
                                        info.IsPricePercentage = modifiers.IsPricePercentage;
                                        info.PriceModifier = modifiers.Price;
                                        info.Quantity = modifiers.Quantity;
                                        info.IsWeightPercentage = modifiers.IsWeightPercentage;
                                        info.WeightModifier = modifiers.Weight;
                                        break;
                                    } else {
                                        info.IsCombinationMatched = false;
                                        info.CombinationID = 0;
                                        info.IsPricePercentage = false;
                                        info.PriceModifier = 0;
                                        info.Quantity = 0;
                                        info.IsWeightPercentage = false;
                                        info.WeightModifier = 0;
                                    }
                                }
                            } else {
                                info.IsCombinationMatched = false;
                                info.CombinationID = 0;
                                info.IsPricePercentage = false;
                                info.PriceModifier = 0;
                                info.Quantity = 0;
                                info.IsWeightPercentage = false;
                                info.WeightModifier = 0;

                                var combinationIndex0 = cvcombinationList[j].split('@');
                                var combinationIndex01 = cvValuecombinationList[j].split('@');
                                for (var w = 0; w < combinationIndex0.length; w++) {
                                    if (combinationIndex0[w] == currentCostVar) {
                                        if (combinationIndex01[w] == currentCostVal) {
                                            if (!IsExists(info.AvailableCombination, cvValuecombinationList[j]))
                                                info.AvailableCombination.push(cvValuecombinationList[j]);
                                        }
                                    }
                                }
                            }
                        }

                    }
                } else {
                    var combinationIndex = cvcombinationList[j].split('@');
                    var combinationIndex1 = cvValuecombinationList[j].split('@');
                    for (var z = 0; z < combinationIndex.length; z++) {
                        if (combinationIndex[z] == currentCostVar) {
                            if (combinationIndex1[z] == currentCostVal) {
                                if (!IsExists(info.AvailableCombination, cvValuecombinationList[j]))
                                    info.AvailableCombination.push(cvValuecombinationList[j]);
                            }
                        }
                    }
                }
            }
            return info;
        }

        LatestItems = {
            config: {
                isPostBack: false,
                async: false,
                cache: false,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: 'json',
                baseURL: aspxservicePath,
                method: "",
                url: "",
                ajaxCallMode: "",                itemid: 0
            },

            vars: {
                countCompareItems: 0,
                itemSKU: itemSKU,
                itemQuantityInCart: "",
                itemId: itemId
            },

            ajaxCall: function(config) {
                $.ajax({
                    type: LatestItems.config.type,
                    contentType: LatestItems.config.contentType,
                    cache: LatestItems.config.cache,
                    async: LatestItems.config.async,
                    url: LatestItems.config.url,
                    data: LatestItems.config.data,
                    dataType: LatestItems.config.dataType,
                    success: LatestItems.config.ajaxCallMode,
                    error: LatestItems.ajaxFailure
                });
            },

            GetLatestItems: function() {
                LatestItems.config.method = "Services/AspxLatestItemService.asmx/GetLatestItemsList";
                LatestItems.config.url = p.latestItemModPath + LatestItems.config.method;
                LatestItems.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj(), count: p.noOfLatestItems });
                LatestItems.config.ajaxCallMode = LatestItems.BindLatestItems;
                LatestItems.config.async = true;
                LatestItems.ajaxCall(LatestItems.config);
            },

            BindRecentItems: function(msg) {
                var RecentItemContents = '';
                if (msg.d.length > 0) {
                    $.each(msg.d, function (index, item) {
                        var imagePath = itemImagePath + item.ImagePath;
                        var altImagePath = itemImagePath + item.AlternateImagePath;
                        if (item.ImagePath == "") {
                            imagePath = p.defaultImagePath;
                        }
                        if (item.AlternateText == "") {
                            item.AlternateText = item.Name;
                        }
                        if (item.AlternateImagePath == "") {
                            altImagePath = imagePath;
                        }
                        RecentItemContents += "<div class=\"cssClassProductsBox\">";
                        var hrefItem = aspxRedirectPath + "item/" + LatestItems.fixedEncodeURIComponent(item.SKU) + pageExtension;

                        var name = '';
                        if (item.Name.length > 50) {
                            name = item.Name.substring(0, 50);
                            var i = 0;
                            i = name.lastIndexOf(' ');
                            name = name.substring(0, i);
                            name = name + "...";
                        }
                        else {
                            name = item.Name;
                        }
                        RecentItemContents += '<div id="productImageWrapID_' + item.ItemID + '" class="cssClassProductsBoxInfo" costvariantItem=' + item.IsCostVariantItem + '  itemid="' + item.ItemID + '"><h3>' + item.SKU + '</h3><div class="divQuickLookonHover"><div class="divitemImage cssClassProductPicture"><a href="' + hrefItem + '" ><img id="img_' + item.ItemID + '"  alt="' + item.AlternateText + '"  title="' + item.AlternateText + '" src="' + aspxRootPath + imagePath.replace('uploads', 'uploads/Medium') + '" orignalPath="' + imagePath.replace('uploads', 'uploads/Medium') + '" altImagePath="' + altImagePath.replace('uploads', 'uploads/Medium') + '"/></a></div><a href="' + hrefItem + '" title="' + item.Name + '" ><h2>' + name + '</h2></a>';

                        if (!item.HidePrice) {
                            if (item.ListPrice != null) {
                                if (item.ItemTypeID == 5)
                        { RecentItemContents += "<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\"><p class=\"cssClassProductRealPrice \" >'"+getLocale(AspxLatestItem,"Starting At ")+"'<span class=\"cssClassFormatCurrency\" value=" + (item.Price).toFixed(2) + ">" + parseFloat(item.Price).toFixed(2) + "</span></p></div></div>";
                        }
                        else{
                                RecentItemContents += "<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\"><p class=\"cssClassProductOffPrice\"><span class=\"cssClassFormatCurrency\" value=" + (item.ListPrice).toFixed(2) + ">" + parseFloat(item.ListPrice).toFixed(2) + "</span></p><p class=\"cssClassProductRealPrice \" ><span class=\"cssClassFormatCurrency\" value=" + (item.Price).toFixed(2) + ">" + parseFloat(item.Price).toFixed(2) + "</span></p></div></div>";
                            }
                            } else {
                            if (item.ItemTypeID == 5)
                            {
                                RecentItemContents += "<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\"><p class=\"cssClassProductRealPrice \" >'"+getLocale(AspxLatestItem,"Starting At ")+"'<span class=\"cssClassFormatCurrency\" value=" + (item.Price).toFixed(2) + ">" + parseFloat(item.Price).toFixed(2) + "</span></p></div></div>";
                            }
                            else
                            { RecentItemContents += "<div class=\"cssClassProductPriceBox\"><div class=\"cssClassProductPrice\"><p class=\"cssClassProductRealPrice \" ><span class=\"cssClassFormatCurrency\" value=" + (item.Price).toFixed(2) + ">" + parseFloat(item.Price).toFixed(2) + "</span></p></div></div>";
                            }
                            }
                        } else {
                            RecentItemContents += "<div class=\"cssClassProductPriceBox\"></div>";
                        }
                        RecentItemContents += '<div class="cssClassProductDetail"><p><a href="' + aspxRedirectPath + 'item/' + item.SKU + pageExtension + '">' + getLocale(AspxLatestItem, "Details") + '</a></p></div>';
                        RecentItemContents += '<div class="sfQuickLook" style="display:none"><img itemId="' + item.ItemID + '" sku="' + item.SKU + '" src="' + AspxCommerce.utils.GetAspxTemplateFolderPath() + '/images/QV_Button.png" rel="popuprel"/></div></div>';
                        var dataContent='';
                          dataContent+='data-class="addtoCart" data-type="button" data-ItemTypeID="'+item.ItemTypeID+'" data-ItemID="'+item.ItemID+'" data-addtocart="';
                        dataContent+='addtocart'+item.ItemID+'"data-title="'+item.Name+'"';
                        dataContent+="data-onclick='AspxCommerce.RootFunction.AddToCartFromJS(" + item.ItemID + "," + parseFloat(item.Price).toFixed(2) + "," + JSON2.stringify(item.SKU) + "," + 1 + "," + "\"" + item.CostVariants + "\"" + "," + "this);'";
                        var itemSKU = JSON2.stringify(item.SKU);
                        var itemName = JSON2.stringify(item.Name);
                        if (p.allowAddToCart.toLowerCase() == 'true') {
                            if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                                if (item.IsOutOfStock) {
                                    RecentItemContents += "</div><div class=\"cssClassAddtoCard\"><div "+dataContent+" class=\"sfButtonwrapper cssClassOutOfStock\"><button type=\"button\"><span>" + getLocale(AspxLatestItem, "Out Of Stock") + "</span></button></div></div>";
                                } else {
                                    RecentItemContents += "</div><div class=\"cssClassAddtoCard\"><div " + dataContent + " class=\"sfButtonwrapper\"><label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\" addtocart=\"addtocart" + item.ItemID + "\" title=" + itemName + "   onclick='AspxCommerce.RootFunction.AddToCartFromJS(" + item.ItemID + "," + parseFloat(item.Price).toFixed(2) + "," + JSON2.stringify(item.SKU) + "," + 1 + "," + "\"" + item.CostVariants + "\"" + "," + "this);'>" + getLocale(AspxLatestItem, "Cart +") + "</label></button></div></div>";
                                }
                            } else {
                                RecentItemContents += "</div><div class=\"cssClassAddtoCard\"><div " + dataContent + " class=\"sfButtonwrapper\"><label class='i-cart cssClassCartLabel cssClassGreenBtn'><button type=\"button\" class=\"addtoCart\" addtocart=\"addtocart" + item.ItemID + "\" title=" + itemName + "  onclick='AspxCommerce.RootFunction.AddToCartFromJS(" + item.ItemID + "," + parseFloat(item.Price).toFixed(2) + "," + JSON2.stringify(item.SKU) + "," + 1 + "," + "\"" + item.CostVariants + "\"" + "," + "this);'>" + getLocale(AspxLatestItem, "Cart +") + "</label></button></div></div>";
                            }
                        }
                        RecentItemContents += "<div class=\"cssClassWishListButton\"><input type=\"hidden\" name=\"itemwish\"  value='" + item.ItemID + ',' + JSON2.stringify(item.SKU) + ",this' /></div>";
                        RecentItemContents += "<div class=\"cssClassclear\"></div>";
                        RecentItemContents += "<div class=\"cssClassCompareButton\"><input type=\"hidden\" name=\"itemcompare\"  value='" + item.ItemID + ',' + JSON2.stringify(item.SKU) + ",this' /></div>";
                        RecentItemContents += "</div>";
                    });
                    $(".divitemImage img").on('mouseout', function() {
                        SwapImageOnMouseOut($(this).attr("id"), aspxRootPath + $(this).attr("orignalPath"));
                    }).on('mouseover', function() {
                        SwapImageOnMouseOver($(this).attr("id"), aspxRootPath + $(this).attr("altImagePath"));
                    });
                } else {
                    RecentItemContents += "<span class=\"cssClassNotFound\">" + getLocale(AspxLatestItem, "This store has no items listed yet!") + "</span>";
                }
                $("#" + p.tblRecentItems).html(RecentItemContents);             
                $('.divitemImage a img[title]').tipsy({ gravity: 'n' });
                var $container = $("#" + p.tblRecentItems);
                $container.masonry('destroy');
                $container.imagesLoaded(function() {
                    $container.masonry({
                        itemSelector: '.cssClassProductsBox',
                        EnableSorting: false
                    });
                });
            },           
           

            fixedEncodeURIComponent: function(str) {
                return encodeURIComponent(str).replace(/!/g, '%21').replace(/'/g, '%27').replace(/\(/g, '%28').replace(/\)/g, '%29').replace(/\*/g, '%2A');
            },
          

            AddToCartToJS: function(itemId, itemPrice, itemSKU, itemQuantity, elem) {
                if (Boolean.parse($.trim($(elem).offsetParent().find(".cssClassProductsBoxInfo").attr("costvariantItem")))) {
                    AspxCommerce.RootFunction.RedirectToLatestItems(itemSKU);
                } else {
                    AspxCommerce.RootFunction.AddToCartFromJS(itemId, itemPrice, itemSKU, itemQuantity);
                }
            },        
            
            BindCostVariantsByitemSKU: function(data) {
                if (data.d.length > 0) {
                    var CostVariant = '';
                    var variantValue = [];
                    $.each(data.d, function(index, item) {
                        if (CostVariant.indexOf(item.CostVariantID) == -1) {
                            CostVariant += item.CostVariantID;
                            var addSpan = '';
                            addSpan += '<div id="div_' + item.CostVariantID + '" class="cssClassHalfColumn">';
                            addSpan += '<span id="spn_' + item.CostVariantID + '" ><b>' + item.CostVariantName + '</b>: ' + '</span>';
                            addSpan += '<span class="spn_Close"><a href="#"><img class="imgDelete" src="' + aspxTemplateFolderPath + '/images/admin/uncheck.png" title=' + "Don\'t use this option" + '" alt=' + "Don\'t use this option" + '/></a></span>';
                            addSpan += '</div>';
                            $('#divCostVariant').append(addSpan);
                        }
                        var valueID = '';
                        var itemCostValueName = '';
                        if (item.CostVariantsValueID != -1) {
                            if (item.InputTypeID == 5 || item.InputTypeID == 6) {
                                if ($('#controlCostVariant_' + item.CostVariantID + '').length == 0) {
                                    itemCostValueName += '<span class="sfListmenu" id="subDiv' + item.CostVariantID + '">';
                                    valueID = 'controlCostVariant_' + item.CostVariantID;
                                    itemCostValueName += LatestItems.CreateControl(item, valueID, false);

                                    itemCostValueName += "</span>";
                                    $('#div_' + item.CostVariantID + '').append(itemCostValueName);
                                }
                                if (!IsExists(variantValue, item.CostVariantsValueID)) {
                                    variantValue.push(item.CostVariantsValueID);
                                    optionValues = LatestItems.BindInsideControl(item, valueID);
                                    $('#controlCostVariant_' + item.CostVariantID + '').append(optionValues);
                                }
                                $('#controlCostVariant_' + item.CostVariantID + ' option:first-child').attr("selected", "selected");
                               } else {
                                if ($('#subDiv' + item.CostVariantID + '').length == 0) {
                                    itemCostValueName += '<span class="cssClassRadio" id="subDiv' + item.CostVariantID + '">';
                                    valueID = 'controlCostVariant_' + item.CostVariantID;
                                    itemCostValueName += LatestItems.CreateControl(item, valueID, true);
                                    itemCostValueName += "</span>";
                                    $('#div_' + item.CostVariantID + '').append(itemCostValueName);
                                } else {
                                    valueID = 'controlCostVariant_' + item.CostVariantID;
                                    itemCostValueName += LatestItems.CreateControl(item, valueID, false);
                                    $('#subDiv' + item.CostVariantID + '').append(itemCostValueName);
                                }
                            }
                        }
                    });
                    $('#divCostVariant').append('<div class="cssClassClear"></div>');
                    if ($.session("ItemCostVariantData") != undefined) {
                        $.each(arrCostVariants, function(i, variant) {
                            var itemColl = $("#divCostVariant").find("[Variantname=" + variant + "]");
                            if ($(itemColl).is("input[type='checkbox'] ,input[type='radio']")) {
                                $("#divCostVariant").find("input:checkbox").removeAttr("checked");
                                $(itemColl).attr("checked", "checked");
                            } else if ($(itemColl).is('select>option')) {
                                $("#divCostVariant").find("select>option").removeAttr("selected");
                                $(itemColl).attr("selected", "selected");
                            }

                        });
                        $.session("ItemCostVariantData", 'empty');
                    }
                                       $('#divCostVariant select,#divCostVariant input[type=radio],#divCostVariant input[type=checkbox]').unbind().bind("change", function() {
                        checkAvailibility(this);

                    });
                    $("#divCostVariant .spn_Close").on("click", function() {
                        $(this).next('span:first').find(" input[type=radio]").removeAttr('checked');
                        if ($(this).next('span:first').find("select").find("option[value=0]").length == 0) {
                            var options = $(this).next('span:first').find("select").html();
                            var noOption = "<option value=0 >" + getLocale(AspxLatestItem, "Not required") + "</option>";
                            $(this).next('span:first').find("select").html(noOption + options);
                        } else {
                            $(this).next('span:first').find("select").find("option[value=0]").attr('selected', 'selected');
                        }
                        checkAvailibility(null);
                    });
                }
            },

            BindItemBasicByitemSKUSucess: function(data) {
                if (data.d != null) {
                    LatestItems.BindItemsBasicInfo(data.d);
                    LatestItems.GetCostVariantsByitemSKU(itemSKU);
                    LatestItems.LoadCostVariantCombination(itemSKU);
                    if (itemTypeId == 3) {
                        LatestItems.GetPriceHistory(itemId);
                        $('.popbox').width(120);
                    }
                    else {
                        LatestItems.GetPriceHistory(itemId);
                    }
                }
            },

            CheckItemQuantityInCartSucess: function(data) {
                LatestItems.vars.itemQuantityInCart = data.d;
            },

            BindImageLists: function(data) {
                LatestItems.GetFilePath(data);
                LatestItems.Gallery();
                LatestItems.ImageZoom();
            },

            BindStyle: function(data) {
                LatestItems.SetValueForStyle(data);
            },

            AddToMyCartSuccess: function(data) {
                if (data.d == 1) {
                    var myCartUrl;
                    if (userFriendlyURL) {
                        myCartUrl = myCartURL + pageExtension;
                    } else {
                        myCartUrl = myCartURL;
                    }
                    window.location.href = AspxCommerce.utils.GetAspxRedirectPath() + myCartURL + pageExtension;
                } else if (data.d == 2) {
                    if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                        csscody.alert("<h2>" + getLocale(AspxLatestItem, 'Information Alert') + '</h2><p>' + getLocale(AspxLatestItem, 'This product is currently out of stock!') + "</p>");
                    }
                    else {
                        var myCartUrl;
                        if (userFriendlyURL) {
                            myCartUrl = myCartURL + pageExtension;
                        } else {
                            myCartUrl = myCartURL;
                        }
                        var addToCartProperties = {
                            onComplete: function(e) {
                                if (e) {
                                    window.location.href = AspxCommerce.utils.GetAspxRedirectPath() + myCartURL + pageExtension;
                                }
                            }
                        };
                    }
                }
            },

            GetAddToCartErrorMsg: function() {
                csscody.error('<h2>' + getLocale(AspxLatestItem, 'Information Alert') + '</h2><p>' + getLocale(AspxLatestItem, 'Failed to add item to cart!') + '</p>');
            },

            BindCostVariantCombination: function(data) {
                $.each(data.d, function(index, item) {
                    var CostVariantCombination = {
                        CombinationID: 0,
                        CombinationType: "",
                        CombinationValues: "",
                        CombinationPriceModifier: "",
                        CombinationPriceModifierType: "",
                        CombinationWeightModifier: "",
                        CombinationWeightModifierType: "",
                        CombinationQuantity: 0,
                        ImageFile: ""
                    };
                    CostVariantCombination = item;
                    arrCombination.push(CostVariantCombination);
                });
                selectFirstcombination();
            },

            BindCostVariantsByitemSKUFailure: function() {
                csscody.error('<h2>' + getLocale(AspxLatestItem, "Error Message") + '</h2><p>' + getLocale(AspxLatestItem, 'Failed to load cost variants!') + '</p>');
            },

            BindLatestItems: function(data) {
                LatestItems.BindRecentItems(data);
            },

            CheckCompareItems: function(data) {
                LatestItems.config.ajaxCallMode = "";
                if (data.d) {
                    csscody.alert('<h2>' + getLocale(AspxLatestItem, 'Information Alert') + '</h2><p>' + getLocale(AspxLatestItem, 'The selected item already exist in compare list.') + '</p>');
                    return false;
                } else {
                    LatestItems.AddToMyCompare();
                }
            },
            
            BindDownloadEvent: function() {
                $(".cssClassLink").jDownload({
                    root: p.aspxFilePath,
                    dialogTitle: 'AspxCommerce download sample item:'
                });
            },

            LoadCostVariantCombination: function() {
                var param = JSON2.stringify({ itemSku: itemSKU, aspxCommonObj: aspxCommonObj() });
                this.config.method = "AspxCoreHandler.ashx/GetCostVariantCombinationbyItemSku";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = param;
                this.config.ajaxCallMode = LatestItems.BindCostVariantCombination;
                this.config.error = 23;
                this.ajaxCall(this.config);
            },

            GetCostVariantsByitemSKU: function(itemSKU) {
                $('#divCostVariant').html('');
                var param = JSON2.stringify({ itemSku: itemSKU, aspxCommonObj: aspxCommonObj() });
                this.config.method = "AspxCoreHandler.ashx/GetCostVariantsByItemSKU";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = param;
                this.config.async = false;
                this.config.ajaxCallMode = LatestItems.BindCostVariantsByitemSKU;
                this.config.error = LatestItems.BindCostVariantsByitemSKUFailure;
                this.ajaxCall(this.config);
            },

            CreateControl: function(item, controlID, isChecked) {
                var controlElement = '';
                var costPriceValue = item.CostVariantsPriceValue;
                var weightValue = item.CostVariantsWeightValue;
                if (item.InputTypeID == 5) {                    controlElement = "<select id='" + controlID + "' multiple></select>";
                } else if (item.InputTypeID == 6) {                    controlElement = "<select id='" + controlID + "'></select>";
                } else if (item.InputTypeID == 9 || item.InputTypeID == 10) {                    controlElement = "<label><input  name='" + controlID + "' type='radio' checked='checked' value='" + item.CostVariantsValueID + "'><span>" + item.CostVariantsValueName + "</span></label>";

                } else if (item.InputTypeID == 11 || item.InputTypeID == 12) {                    controlElement = "<input  name='" + controlID + "' type='radio' checked='checked' value='" + item.CostVariantsValueID + "'><label>" + item.CostVariantsValueName + "</label></br>";

                }
                return controlElement;
            },

            BindInsideControl: function(item, controlID) {
                var optionValues = '';
                var costPriceValue = item.CostVariantsPriceValue;
                var weightValue = item.CostVariantsWeightValue;
                if (item.InputTypeID == 5) {                    optionValues = "<option value=" + item.CostVariantsValueID + ">" + item.CostVariantsValueName + "</option>";

                } else if (item.InputTypeID == 6) {
                    optionValues = "<option value=" + item.CostVariantsValueID + ">" + item.CostVariantsValueName + "</option>";

                }
                return optionValues;
            },

            BindItemBasicByitemSKU: function(itemSKU) {
                var checkparam = { itemSKU: itemSKU, aspxCommonObj: aspxCommonObj() };
                var checkdata = JSON2.stringify(checkparam);
                this.config.method = "AspxCoreHandler.ashx/GetItemBasicInfoByitemSKU";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = checkdata;
                this.config.async = false;
                this.config.ajaxCallMode = LatestItems.BindItemBasicByitemSKUSucess;
                this.ajaxCall(this.config);
            },

            AddToCartToJS: function(itemId, itemPrice, itemSKU, itemQuantity) {
                AspxCommerce.RootFunction.AddToCartFromJS(itemId, itemPrice, itemSKU, itemQuantity);
            },

            CheckItemQuantityInCart: function(itemId, itemCostVariantIDs) {
                this.config.method = "AspxCoreHandler.ashx/CheckItemQuantityInCart";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ itemID: itemId, aspxCommonObj: aspxCommonObj(), itemCostVariantIDs: itemCostVariantIDs });
                this.config.ajaxCallMode = LatestItems.CheckItemQuantityInCartSucess;
                this.ajaxCall(this.config);
                return LatestItems.vars.itemQuantityInCart;
            },

            GetImageLists: function(cids, sku, combinationId) {
                if (itemTypeId == 3) {
                    LatestItems.GetGiftCardThemes();
                } else {
                    this.config.method = "AspxCoreHandler.ashx/GetItemsImageGalleryInfoBySKU";
                    this.config.url = this.config.baseURL + this.config.method;
                    this.config.data = JSON2.stringify({ itemSKU: sku, aspxCommonObj: aspxCommonObj(), combinationId: combinationId });
                    this.config.ajaxCallMode = LatestItems.BindImageLists;
                    this.config.async = true;
                    this.ajaxCall(this.config);
                }

            },

            AddStyle: function() {
                this.config.method = "AspxCoreHandler.ashx/ReturnDimension";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ userModuleID: userModuleID, aspxCommonObj: aspxCommonObj() });
                this.config.ajaxCallMode = LatestItems.BindStyle;
                this.config.error = 18;
                this.ajaxCall(this.config);
            },

            GetFilePath: function(msg) {
                var imgList = '';
                var html = '';
                html = '<ul id="pikame" class="jcarousel-skin-pika">';
                if (msg.d.length > 0) {
                    $.each(msg.d, function(index, item) {
                        item.ImagePath = itemImagePath + item.ImagePath;
                        html += '<li><a href="#"><img id="image1" src="' + aspxRootPath + item.ImagePath.replace('uploads', 'uploads/Large') + ' " alt="' + item.AlternateText + '" title="' + item.AlternateText + '" /></a></li>';
                    });
                    html += '</ul>';
                } else {
                    html += '<li><a href="#"><img id="image1" src="' + aspxRootPath + p.defaultImagePath + '" /></a></li>';
                    html += '</ul>';
                }
                $(".pikachoose").append(html);
            },

            ImageZoom: function() {

            },
            Gallery: function() {
                $("#pikame").PikaChoose({ autoPlay: true, showCaption: false });
                $("#pikame").jcarousel({
                    scroll: 3,
                    transition: [6],
                    initCallback: function(carousel) {
                        $(carousel.list).find('img').click(function() {
                            carousel.scroll(parseInt($(this).parents('.jcarousel-item').attr('jcarouselindex')));
                        });
                    }

                });
            },

            SetValueForStyle: function(data) {
                $('div.pika-image').css("width", data.d[0] + 2);
                $('div.pika-image').css("height", data.d[1] + 2);
                $('#image1').css('width', data.d[2]);
                $('#image1').css('height', data.d[2]);
                newObject = new Variable(data.d[1], data.d[0], data.d[2], data.d[3]);
            },
            GetPriceHistory: function(id) {
                var param = JSON2.stringify({ itemId: id, aspxCommerceObj: aspxCommonObj() });
                this.config.method = "AspxCoreHandler.ashx/GetPriceHistoryList";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = param;
                this.config.ajaxCallMode = LatestItems.BindPriceHistory;
                this.ajaxCall(this.config);
            },
            BindPriceHistory: function(msg) {
                if (msg.d.length > 0) {
                    $('.popbox').show();
                    var element = "<a class='open' href='#'>" + getLocale(AspxLatestItem, "Price History") + "</a>";
                    element += "<div class='collapse'><div class='box'><div class='arrow'></div>";
                    element += "<div class='arrow-border'></div><div class='classPriceHistory'>";
                    element += '<table class=classPriceHistoryList><thead><th>' + getLocale(AspxLatestItem, "Date") + '</th><th>' + getLocale(AspxLatestItem, "Price") + '</th></thead><tbody>';
                    $.each(msg.d, function(index, item) {
                        element += '<tr><td><span>' + item.Date + '</span></td><td><span>' + item.Price + '</span></td></tr>';
                    });
                    element += '</tbody></table></div>';
                    $('.popbox').append(element);
                    $('.popbox').popbox();
                } else {
                    $("div.popbox").html('');
                    $('.popbox').hide();
                }
            },

            BindItemsBasicInfo: function(item) {
                info.IsCombinationMatched = true;
                info.IsPricePercentage = false;
                info.IsWeightPercentage = false;
                info.PriceModifier = 0;
                info.WeightModifier = 0;
                info.Quantity = item.Quantity;
                $(".pikachoose").html('');
                $("#divGift").remove();
                $("#tblGift").remove();
                if (item.ItemViewCount == null) {
                    item.ItemViewCount = 0;
                }
                $("#viewCount").text(item.ItemViewCount);
                LatestItems.config.async = true;
                if (itemTypeId == 3) {
                    LatestItems.BindGiftCardInfo(item);

                } else {
                    $(".cssClassDwnWrapper").show();
                    $("#divCostVariant").show();
                    if (p.allowAddToCart.toLowerCase() == 'true') {
                        $(".cssClssQTY").show();
                    }
                    $("#" + p.lblItemQnty).show();
                    $("#txtQty").show();
                    $("#lblNotification").show();
                    $("#tblGift").html('');
                    if (item.ListPrice != "") {
                        $("#" + p.lblListPrice).show();
                        $("#spanListPrice").show();
                        $(".cssClassBulkDiscount").show();
                        $("#" + p.lblSaving).show();
                        $("#spanSaving").show();
                        $("#spanListPrice").html(parseFloat(item.ListPrice).toFixed(2));
                        $("#hdnListPrice").val(item.ListPrice);
                    } else {
                        $("#" + p.lblListPrice).hide();
                        $("#spanListPrice").hide();
                        $("#" + p.lblSaving).hide();
                        $("#spanSaving").hide();
                        $(".cssClassYouSave").hide();
                    }
                    $("#spanItemName").html(item.Name);
                    $("#spanPrice").html(parseFloat(item.Price).toFixed(2));
                    $("#hdnPrice").val(item.Price);
                    $("#hdnListPrice").val(item.ListPrice);
                    $("#hdnWeight").val(item.Weight);
                    $("#hdnQuantity").val(item.Quantity);
                    if (item.BrandName != "") {
                        var html = '';
                        var brandUrl = aspxRootPath + "brand/" + item.BrandName + pageExtension;
                        html = "<h2><span class=\"sflocale\">" + getLocale(AspxLatestItem, "Brand") + "</span><span id=\"spanItemBrandName\"><a title=" + getLocale(AspxLatestItem, 'View all items under this brand') + " href='" + brandUrl + "'>" + item.BrandName + "</a></span></h2>";
                        $(".cssClassItemBrands").append(html);
                    }
                    else {
                        $(".cssClassItemBrands").html('');
                    }
                    if (p.allowAddToCart.toLowerCase() == 'true') {
                        $("#btnAddToMyCart").parent('div').show();
                    }
                    if (p.allowAddToCart.toLowerCase() == 'true') {
                        if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                            if (item.IsOutOfStock) {
                                $("#txtQty").removeAttr('enabled').attr("disabled", "disabled");
                                $("#btnAddToMyCart").removeClass("cssClassAddToCard").addClass('cssClassOutOfStock').attr("disabled", "disabled").find("span>span").html(getLocale(AspxLatestItem, "Out Of Stock"));
                                $("#btnAddToMyCart").show();
                                $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'Out Of Stock') + '</b>');
                            } else {
                                if (itemTypeId == 2) {
                                    $("#txtQty").removeAttr('enabled').attr("disabled", "disabled");
                                }
                                else {
                                    $("#txtQty").removeAttr('disabled').attr("enabled", "enabled");
                                }
                                $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart"));
                                $("#btnAddToMyCart").show();
                                $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>');
                            }
                        } else {
                            $("#btnAddToMyCart").show();
                        }
                    }
                    $("#txtQty").val('1');
                    $("#txtQty").attr('addedValue', 1);
                    $("#hdnTaxRateValue").val(item.TaxRateValue);
                    if (item.SampleLink != '' && item.SampleFile != '') {
                        $("#dwnlDiv").show();
                        $("#spanDownloadLink").html(item.SampleLink);
                        $("#spanDownloadLink").attr("href", item.SampleFile);

                    } else {
                        $("#dwnlDiv").hide();
                    }
                    if (item.ListPrice != "") {
                        $(".cssClassYouSave").show();
                        var savingPercent = ((item.ListPrice - item.Price) / item.ListPrice) * 100;
                        savingPercent = savingPercent.toFixed(2);
                        $("#spanSaving").html('<b>' + savingPercent + '%</b>');
                    }
                    else {
                        $(".cssClassYouSave").hide();
                    }
                    var shortDesc = '';
                    if (item.ShortDescription.length > 870) {
                        shortDesc = item.ShortDescription.substring(0, 870);
                        shortDesc += " >>>";
                    } else {
                        shortDesc = item.ShortDescription;
                    }
                    $('.cssClassProductInformation').show();
                    $('.cssClassItemQuickOverview').show();                 
                }
            },

            GetGiftCardThemes: function() {
                function mycarousel_initCallback(carousel) {
                    jQuery('#next').bind('click', function () {
                        carousel.next();
                        return false;
                    });

                    jQuery('#prev').bind('click', function () {
                        carousel.prev();
                        return false;
                    });
                };

                var param = JSON2.stringify({ itemId: itemId, aspxCommonObj: aspxCommonObj() });
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    async: true,
                    url: aspxservicePath + 'AspxCoreHandler.ashx/GetGiftCardThemeImagesByItem',
                    data: param,
                    dataType: "json",
                    success: function(data) {
                        if (data.d.length > 0) {
                            var $ul = $("<ul>").attr('id', 'pikame').addClass("jcarousel-skin-pika");
                            $.each(data.d, function(index, item) {
                                var $li = $("<li>");
                                var $a = $("<a>");
                                var $img = $("<img>");
                                $a.attr("href", "#");
                                $img.attr("src", aspxRootPath + item.GraphicImage).attr('alt', item.GraphicName).attr('data-id', item.GiftCardGraphicId);
                                $li.append($a);
                                $a.append($img);
                                $ul.append($li);
                            });

                            var $preview = $("<div>").addClass("cssClassPreview");
                            $ul.find('li').click(function() {
                            });
                            $(".pikachoose").html('').append($ul).append("<div id='divGift'><span>" + getLocale(AspxLatestItem, "Note:- select your card theme") + "</span></div>");
                            LatestItems.Gallery();
                            LatestItems.ImageZoom();
                        }
                    }
                });
            },

            BindGiftCardInfo: function(item) {
                $(".cssClassYouSave").hide();
                $("#" + p.lblListPrice).hide();
                $("#spanListPrice").hide();
                $("#" + p.lblSaving).hide();
                $("#spanSaving").hide();
                $("#" + p.lblItemQnty).hide();
                $("#txtQty").hide();
                $(".cssClssQTY").hide();
                $("#lblNotification").hide();
                $("#spanItemName").html(item.Name);
                $("#spanPrice").html(parseFloat(item.Price).toFixed(2));
                var price = parseFloat(item.Price).toFixed(2);
                $("#hdnPrice").val(item.Price);
                $("#hdnWeight").val(item.Weight);
                $(".cssClassDwnWrapper").html('');
                $("#btnAddToMyCart").removeClass("cssClassOutOfStock").addClass('cssClassAddToCard').removeAttr("disabled").attr('enabled', "enabled").find("span>span").html(getLocale(AspxLatestItem, "Add to Cart"));
                $("#btnAddToMyCart").show();
                $("#spanAvailability").html('<b>' + getLocale(AspxLatestItem, 'In stock') + '</b>');

                var $li = '<div id="divCostVariant"></div><table id="tblGift">';                $li += '<tr ><td>  <span>Sender Name:<em>*</em></span></td>';
                $li += '<td> <input id ="txtgc_senerName" type="text" minlength="2" messages="*" class="sfTextBoxSmall" name="gcsender_name"> </td></tr>';
                $li += ' <tr ><td> <span>Sender Email:<em>*</em></span></td>';
                $li += ' <td><input type="text" value="" class="sfTextBoxSmall" name="gcsender_email" id="txtgc_senerEmail"> </td></tr> ';
                $li += '<tr><td><span >Recipient Name:<em>*</em></span></td>';
                $li += ' <td><input type="text" class="sfTextBoxSmall" minlength="2" name="gcrecipient_name" id="txtgc_recieverName"> </td></tr>';
                $li += '<tr> <td><span >Recipient Email:<em>*</em></span></td> ';
                $li += '<td><input type="text" class="sfTextBoxSmall" name="gcrecipient_email" id="txtgc_recieverEmail"> </td></tr>';
                $li += '<tr> <td><span >Message:</span> </td>';
                $li += ' <td><textarea class="sfTextBoxSmall" rows="3" cols="5" style="width: 135px; height: 50px; id="txtgc_messege" name="gcmessage"></textarea> </td> </tr>';
                $li += '<tr><td><span>Send gift by:</span></td><td id="GiftCardTypes"></td></tr></table> ';
                $(".cssClassDwnWrapper").html($li);
                $('.cssClassProductInformation').show();
              
                LatestItems.GetGiftCardTypes();
            },
            GetGiftCardTypes: function() {
                this.config.method = "AspxCoreHandler.ashx/GetGiftCardTypes";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj() });
                this.config.ajaxCallMode = function(data) {

                    $("#GiftCardTypes").html('');
                    $.each(data.d.reverse(), function(i, item) {
                        var x = $('<label>').append($("<input type=radio />").attr('name', 'giftcard-type').attr('value', item.TypeId)).append(item.Type.toLowerCase());

                        $("#GiftCardTypes").append(x);

                    });
                    $("input[name=giftcard-type]:first").attr('checked', 'checked');
                };
                this.ajaxCall(this.config);
            },
            GetFormFieldList: function(itemSKU) {
                LatestItems.vars.itemSKU = itemSKU;
                this.config.method = "AspxCoreHandler.ashx/GetItemFormAttributesByitemSKUOnly";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.async = false;
                this.config.data = JSON2.stringify({ itemSKU: itemSKU, aspxCommonObj: aspxCommonObj() });
                this.config.ajaxCallMode = LatestItems.BindItemFormAttributes;
                this.ajaxCall(this.config);
            },

            BindItemFormAttributes: function(msg) {
                $.each(msg.d, function(index, item) {
                    if (index == 0) {
                        itemTypeId = item.ItemTypeID;
                    }
                });
            },
          

            ResetGallery: function(costcombinationId) {
                $('.pika-image,.jcarousel-skin-pika,.pikachoose').html('');
                var ids = '';
                $("#divCostVariant input[type=radio]:checked").each(function() {
                    ids += $(this).val() + "@";
                });

                $("#divCostVariant input[type=checkbox]:checked").each(function() {
                    ids += $(this).val() + "@";
                });
                $("#divCostVariant select option:selected").each(function() {
                    ids += $(this).val() + "@";
                });
                ids = ids.substr(0, ids.length - 1);
                LatestItems.GetImageLists(ids, itemSKU, costcombinationId);
            },
            LoadLatestItemRssImage: function() {
                var pageurl = aspxRedirectPath + p.rssFeedUrl + pageExtension;
                $('#latestItemRssImage').parent('a').show();
                $('#latestItemRssImage').parent('a').removeAttr('href').attr('href', pageurl + '');
                $('#latestItemRssImage').removeAttr('src').attr('src', aspxTemplateFolderPath + '/images/rss-icon.png');
                $('#latestItemRssImage').removeAttr('title').attr('title', getLocale(AspxRssFeedLocale, "Latest Items Rss Feed Title"));
                $('#latestItemRssImage').removeAttr('alt').attr('alt', getLocale(AspxRssFeedLocale, "Latest Items Rss Feed Alt"));
            },

            init: function() {
                $("#divLatestItems").hide();
                if (p.enableLatestItems.toLowerCase() == 'true' && p.noOfLatestItems > 0) {                    
                    if ($("#divLatestItems").find("span.cssClassNotFound").length ==0) {
                        var $container = $("#" + p.tblRecentItems);
                        $container.imagesLoaded(function () {
                            $container.masonry({
                                itemSelector: '.cssClassProductsBox',
                                EnableSorting: false
                            });
                        });
                    }
                    $("#divLatestItems").show();

                                      $('.divitemImage a img[title]').tipsy({ gravity: 'n' });

                }
                $("#dwnlDiv").click(function() {
                    LatestItems.BindDownloadEvent();
                });

                $("#btnAddToMyCart").hide();
                var itemVariantIds = '';
                $(".sfQuickLook").find('img').on('click', function() {
                    arrCombination = [];
                    variantValuesID = [];
                    var costVariantsData = '';
                    arrCostVariants = [];
                    $(".popbox").html('');
                    $(".cssClassItemBrands").html('');
                    if ($.session("ItemCostVariantData")) {
                        costVariantsData = $.session("ItemCostVariantData");
                        arrCostVariants = costVariantsData.split(',');
                    }
                    itemId = $(this).attr('itemid');
                    itemSKU = $(this).attr('sku');
                    LatestItems.vars.ItemSKU = itemSKU;
                    LatestItems.GetFormFieldList(itemSKU);
                    LatestItems.BindItemBasicByitemSKU(itemSKU);
                    $(".cssClassQuickView").css('margin-top', '-187px');
                    $("#viewMore").find('a').attr('href', aspxRedirectPath + 'item/' + itemSKU + pageExtension);
                    ShowPopup(this);
                });
                $(".cssClassClose").bind("click", function() {
                    RemovePopUp();
                });
                
                $("#divCostVariant input[type=radio]:checked").each(function() {
                    itemVariantIds += $(this).val() + ",";
                });
                $("#divCostVariant input[type=checkbox]:checked").each(function() {
                    itemVariantIds += $(this).val() + ",";
                });
                $("#divCostVariant select option:selected").each(function() {
                    itemVariantIds += $(this).val() + ",";
                });
                itemVariantIds = itemVariantIds.substr(0, itemVariantIds.length - 1);
                $("#txtQty").bind("contextmenu", function(e) {
                    return false;
                });
                $('#txtQty').bind('paste', function(e) {
                    e.preventDefault();
                });
                $("#txtQty").bind('focus', function(e) {
                    $(this).val('');
                    $('#lblNotification').html('');
                });
                $("#txtQty").bind('select', function(e) {
                    $(this).val('');
                    $('#lblNotification').html('');
                });
                $("#txtQty").bind('blur', function(e) {
                    $('#lblNotification').html('');
                    $("#txtQty").val($(this).attr('addedValue'));
                });

                $("#txtQty").bind("keypress", function(e) {
                    var itemCostVariantIDs = '';
                    if ($('#divCostVariant').is(':empty')) {
                        itemCostVariantIDs = '0@';
                    } else {
                        $("#divCostVariant select option:selected").each(function() {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs += $(this).val() + "@";
                            } else {
                            }
                        });
                        $("#divCostVariant input[type=radio]:checked").each(function() {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs += $(this).val() + "@";
                            } else {
                            }
                        });

                        $("#divCostVariant input[type=checkbox]:checked").each(function() {
                            if ($(this).val() != 0) {
                                itemCostVariantIDs += $(this).val() + "@";
                            } else {
                            }
                        });
                    }
                    if (p.allowAddToCart.toLowerCase() == 'true') {
                        if (p.allowOutStockPurchase.toLowerCase() == 'false') {
                            if (info.Quantity <= 0) {
                                return false;
                            } else {
                                if ((e.which >= 48 && e.which <= 57)) {
                                    var num;
                                    if (e.which == 48)
                                        num = 0;
                                    if (e.which == 49)
                                        num = 1;
                                    if (e.which == 50)
                                        num = 2;
                                    if (e.which == 51)
                                        num = 3;
                                    if (e.which == 52)
                                        num = 4;
                                    if (e.which == 53)
                                        num = 5;
                                    if (e.which == 54)
                                        num = 6;
                                    if (e.which == 55)
                                        num = 7;
                                    if (e.which == 56)
                                        num = 8;
                                    if (e.which == 57)
                                        num = 9;

                                    var itemQuantityInCart = LatestItems.CheckItemQuantityInCart(itemId, itemCostVariantIDs);
                                    if (itemQuantityInCart != 0.1) {
                                        if ((eval($("#txtQty").val() + '' + num) + eval(itemQuantityInCart)) > eval(info.Quantity)) {
                                            $('#lblNotification').html(getLocale(AspxLatestItem, 'The quantity is greater than the available quantity.'));
                                            return false;
                                        } else {
                                            $('#lblNotification').html('');
                                        }
                                    } else {
                                        $("#txtQty").val(1).attr("disabled", "disabled");
                                    }

                                }
                            }
                        }
                    }

                    if ($(this).val() == "") {
                        if (e.which != 8 && e.which != 0 && (e.which < 49 || e.which > 57)) {
                            return false;
                        }
                    } else {
                        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                            return false;
                        }
                    }
                    if (num != undefined) {
                        $("#txtQty").attr('addedValue', eval($("#txtQty").val() + '' + num));
                    }
                });
                if (p.latestItemRss.toLowerCase() == 'true') {
                    LatestItems.LoadLatestItemRssImage();
                }
            }
        };
        LatestItems.init();

        LatestItemAPI = function() {
            return { Add: LatestItems.AddToCartToJS,
                Reload: LatestItems.GetLatestItems
            }
        } ();
    };
    $.fn.LatestItems = function(p) {
        $.fn.LatestItemView(p);
    };

})(jQuery);    