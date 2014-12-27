<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ItemCompareDetails.ascx.cs"
    Inherits="Modules_AspxCommerce_AspxItemsCompare_ItemCompareDetails" %>

<script type="text/javascript">
    //<![CDATA[

    $(document).ready(function () {
        $(".sfLocale").localize({
            moduleKey: AspxItemsCompare
        });
        var customerId, ip, countryName, sessionCode, userFriendlyURL;

        var IDs = "";
        var costVar = "";
        var groupItems = [];

        customerId = AspxCommerce.utils.GetCustomerID();
        ip = AspxCommerce.utils.GetClientIP();
        countryName = AspxCommerce.utils.GetAspxClientCoutry();
        sessionCode = AspxCommerce.utils.GetSessionCode();
        userFriendlyURL = AspxCommerce.utils.IsUserFriendlyUrl();
        var aspxCommonObj = function () {
            var aspxCommonInfo = {
                StoreID: AspxCommerce.utils.GetStoreID(),
                PortalID: AspxCommerce.utils.GetPortalID(),
                CultureName: AspxCommerce.utils.GetCultureName(),
                UserName: AspxCommerce.utils.GetUserName()
            };
            return aspxCommonInfo;
        };

        var servicepath = '<%=ServicePath%>';
        function GetCompareList(IDs, costVar) {
            $.ajax({
                type: "POST",
                url: servicepath + "GetCompareList",
                data: JSON2.stringify({ itemIDs: IDs, CostVariantValueIDs: costVar, aspxCommonObj: aspxCommonObj() }),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var myIds = new Array();
                    var myAttributes = new Array();
                    $("#tblItemCompareList >tbody").html('');
                    $("#divItemCompareElements").html("");
                    $("#scriptStaticField").tmpl().appendTo("#divItemCompareElements");
                    Array.prototype.RemoveNA = function (arr, obj) {
                        var i = this.length;
                        while (i--) {
                            if (this[i].AttributeID === obj) {
                                arr.splice(i, 1);
                            }
                        }
                    };
                    Array.prototype.Count = function (obj) {
                        var i = this.length;
                        var cc = 0;
                        while (i--) {
                            if (this[i] === obj) {
                                cc++;
                            }
                        }
                        return cc;
                    };
                    var oldc;
                    var itemCount;
                    var emArr = [];
                    $.each(msg.d, function (index, value) {

                        if (index == 0) {
                            oldc = value.AttributeID; itemCount = 1;
                        }
                        if (index != 0 && value.AttributeID == oldc) {
                            itemCount++;
                        }
                        if (value.AttributeValue == "") {
                            emArr.push(value.AttributeID);
                        }
                    });
                    $.each(emArr, function (index, value) {

                        if (itemCount == emArr.Count(value)) {
                            msg.d.RemoveNA(msg.d, value);
                        }
                    });

                    $.each(msg.d, function (index, value) {
                        var cssClass = '';
                        var noAttValue = [];
                        cssClass = 'cssClassCompareAttributeClass';
                        var pattern = '"', re = new RegExp(pattern, "g");
                        if (value.InputTypeID == 7) {
                            if (value.AttributeValue != "") {
                                cssClass = 'cssClassFormatCurrency cssClassCompareAttributeClass';
                            }
                        }

                        if (jQuery.inArray(value.AttributeID, myAttributes) < 0) {
                            $("#tblItemCompareList >tbody").append('<tr id="trCompare_' + index + '"></tr>');
                            if (value.AttributeName == 'Variants') {
                                value.AttributeName = getLocale(AspxItemsCompare, "Variants");
                            }
                            $("#tblItemCompareList >tbody> tr:last").append('<td class="cssClassCompareAttributeClass"><span class="cssClassLabel">' + value.AttributeName + ': </span></td>');
                            var valz;
                            if (value.AttributeValue == "") {
                                valz = "n/a"; noAttValue.push(value.AttributeID);
                            } else {
                                if (value.InputTypeID == 7) {
                                    valz = parseFloat(value.AttributeValue).toFixed(2);
                                }
                                else {
                                    valz = value.AttributeValue;
                                }
                            }
                            var y = Encoder.htmlDecode(valz);
                            y = y.replace(re, '\\');
                            var attributValue;
                            if (groupItems.length > 0) {
                                $.each(groupItems, function (index, item) {
                                    if (value.ItemID = item) {
                                        if (value.InputTypeID == 7) {
                                            if (value.AttributeValue != "") {
                                                attributValue = [{ CssClass: cssClass, AttributeValue: getLocale(AspxItemsCompare, "Starting At") + y }];
                                                return;
                                            }
                                            else {
                                                attributValue = [{ CssClass: cssClass, AttributeValue: y }];
                                                return;
                                            }
                                        }
                                        else {
                                            attributValue = [{ CssClass: cssClass, AttributeValue: y }];
                                            return;
                                        }
                                    }
                                    else {
                                        attributValue = [{ CssClass: cssClass, AttributeValue: y }];
                                        return;
                                    }
                                });
                            }
                            else {
                                attributValue = [{ CssClass: cssClass, AttributeValue: y }];
                            }
                            $("#scriptAttributeValue").tmpl(attributValue).appendTo("#tblItemCompareList tbody#itemDetailBody>tr:last");
                            myAttributes.push(value.AttributeID);

                        }
                        else {
                            var valz1;
                            if (value.AttributeValue == "") {
                                valz1 = "n/a";
                            } else {
                                if (value.InputTypeID == 7) {
                                    valz1 = parseFloat(value.AttributeValue).toFixed(2);
                                }
                                else {
                                    valz1 = value.AttributeValue;
                                }
                            }
                            var z = Encoder.htmlDecode(valz1);
                            z = z.replace(re, '\\');
                            var i = index % (myAttributes.length);
                            attributValue = [{ CssClass: cssClass, AttributeValue: z }];
                            $("#scriptAttributeValue").tmpl(attributValue).appendTo("#trCompare_" + i + "");
                        }
                    });

                    $("#tblItemCompareList tr:even").addClass("sfEven");
                    $("#tblItemCompareList tr:odd").addClass("sfOdd");
                    var cookieCurrency = $("#ddlCurrency").val();
                    Currency.currentCurrency = BaseCurrency;
                    Currency.convertAll(Currency.currentCurrency, cookieCurrency);
                },
                error: function () {
                    csscody.error('<h2>' + getLocale(AspxItemsCompare, 'Error Message') + '</h2><p>' + getLocale(AspxItemsCompare, 'Sorry, Compare list error occured!') + '</p>');
                }
            });
            if (groupItems.length > 0) {
                $.each(groupItems, function (index, item) {
                    $(".cssClassCompareAttributeClass >span").each(function () {
                        var colposition = $(this).parents("td").prop('cellIndex');
                        if ($(this).html() == "Price: ") {
                            $(this).parents("tr").find("td").eq(index).prepend(getLocale(AspxItemsCompare, "Starting At"));
                        }
                    });
                });
            }
        }

        function RecentAdd(Id, costVar) {
            var param = JSON2.stringify({ IDs: Id, CostVarinatIds: costVar, aspxCommonObj: aspxCommonObj() });
            $.ajax({
                type: "Post",
                url: servicepath + "AddComparedItems",
                data: param,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                },
                error: function () {
                    csscody.error('<h2>' + getLocale(AspxItemsCompare, 'Error Message') + '</h2><p>' + getLocale(AspxItemsCompare, 'Sorry, error occured!') + '</p>');
                }
            });
        }

        function GetCompareListImage(IDs, costVar) {
            $.ajax({
                type: "POST",
                async: false,
                url: servicepath + "GetCompareListImage",
                data: JSON2.stringify({ itemIDs: IDs, CostVariantValueIDs: costVar, aspxCommonObj: aspxCommonObj() }),
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var htMl = '';
                    $("#divCompareElements").html("");
                    if (msg.d.length > 0) {
                        $.each(msg.d, function (index, value) {
                            var imagePath = itemImagePath + value.BaseImage;
                            if (value.BaseImage == "") {
                                imagePath = '<%=NoImageItemComparePath %>';
                            }
                            else if (value.AlternateText == "") {
                                value.AlternateText = value.Name;
                            }

                            var items = [{
                                aspxRedirectPath: aspxRedirectPath, itemID: value.ItemID, CostVariant: (value.CostVariantItemID == 0 ? false : true), index: index, name: value.Name, sku: value.SKU,
                                imagePath: aspxRootPath + imagePath.replace('uploads', 'uploads/Small'), alternateText: value.AlternateText, listPrice: value.ListPrice,
                                price: value.Price, shortDescription: Encoder.htmlDecode(value.ShortDescription), itemTypeID: value.ItemTypeID
                            }];
                            $("#scriptResultProductGrid2").tmpl(items).appendTo("#tblItemCompareList thead > tr");
                            if (value.ListPrice == "") {
                                $(".cssRegularPrice_" + value.ItemID + "").parent('p').remove();
                            }
                            if ('<%=AllowAddToCart %>'.toLowerCase() == 'true') {
                                                            $("#cssClassAddtoCard_" + value.ItemID + "_" + index).show();
                                                            if ('<%=AllowOutStockPurchase %>'.toLowerCase() == 'false') {
                                    if (value.IsOutOfStock) {
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index + "").find('label').removeClass();
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index + " span").html(getLocale(AspxItemsCompare, 'Out Of Stock'));
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index).removeClass('cssClassAddtoCard');
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index).addClass('cssClassOutOfStock');
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index + " a").removeAttr('onclick');
                                        $("#cssClassAddtoCard_" + value.ItemID + "_" + index).find(".sfButtonwrapper").addClass('cssClassOutOfStock');
                                    }
                                }
                            }
                            if (value.ItemTypeID == 5) {
                                groupItems.push(value.ItemID);
                                $("#cssClassProductsGridRealPrice_" + value.ItemID).prepend(getLocale(AspxItemsCompare, "Starting At"));
                            }
                            if (value.CostVariantItemID != '0') {
                                var href = AspxCommerce.utils.GetAspxRedirectPath() + 'item/' + value.SKU + pageExtension + '?varId=' + value.CostVariantItemID + '';
                                $("#cssClassAddtoCard_" + value.ItemID + "_" + index).find('a').attr('href', href);
                            }

                        });
                    }
                    else {
                        $(".cssClassHeaderRight").hide();
                    }
                    var cookieCurrency = $("#ddlCurrency").val();
                    Currency.currentCurrency = BaseCurrency;
                    Currency.convertAll(Currency.currentCurrency, cookieCurrency);
                },
                error: function () {
                    csscody.error('<h2>' + getLocale(AspxItemsCompare, 'Error Message') + '</h2><p>' + getLocale(AspxItemsCompare, 'Sorry, compare list error occured!') + '</p>');
                }
            });
        }

        function CheckWishListUniqueness(itemID, sku, CostVariant) {
            if (customerId > 0 && userName.toLowerCase() != "anonymoususer") {
                if (CostVariant == '0') {
                    CostVariant = "";
                }
                var checkparam = { ID: itemID, costVariantValueIDs: CostVariant, aspxCommonObj: aspxCommonObj() };
                var checkdata = JSON2.stringify(checkparam);
                $.ajax({
                    type: "POST",
                    url: servicepath + "CheckWishItems",
                    data: checkdata,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d) {
                            csscody.alert('<h2>' + getLocale(AspxItemsCompare, 'Information Alert') + '</h2><p>' + getLocale(AspxItemsCompare, 'The selected item already in your wishlist.') + '</p>');
                        } else {
                            AspxCommerce.RootFunction.AddToWishListFromJS(itemID, ip, countryName, CostVariant);
                        }
                    }
                });

            } else {
                window.location.href = aspxRootPath + LogInURL + pageExtension;
                return false;
            }
        }


        IDs = $.cookies.get("ItemCompareDetail");
        costVar = $.cookies.get("costVariant");


        if (IDs != null && IDs != '') {
            GetCompareListImage(IDs, costVar);
            GetCompareList(IDs, costVar);
            RecentAdd(IDs, costVar);
        } else {
            $("#divCompareElementsPopUP").html('<span class="cssClassNotFound">' + getLocale(AspxItemsCompare, 'No Items found in you Compare Item List.') + '</span>');
        }
        if ($("#tblRecentlyComparedItemList").length > 0) {
            RecentlyComparedItems.RecentlyCompareItemsList();
        }
        $('#btnPrintItemCompare').click(function () {
            printPage();
        });
    });
    printPage = function () {
        window.print();
        if (window.stop) {
            location.reload(); window.stop();
        }
        return false;

    };

    //]]>      
</script>

<script id="scriptStaticField" type="text/x-jquery-tmpl">
</script>

<script id="scriptAttributeValue" type="txt/x-jquery-tmpl">
<td class="${CssClass}">{{html AttributeValue}}</td>
</script>

<script id="scriptResultProductGrid2" type="text/x-jquery-tmpl">
    <td>
        <div id="comparePride" class="cssClassProductsGridBox">
            <div class="cssClassProductsGridInfo">

                <div class="cssClassProductsGridPicture cssClassBMar20">
                    <img src='${imagePath}' alt='${alternateText}' title='${name}' /></div>
                <h2><a href="${aspxRedirectPath}item/${sku}${pageExtension}">${name}</a></h2>
                <div class="cssClassProductsGridPriceBox">
                    <div class="cssClassProductsGridPrice">
                        <p class="cssClassProductsGridOffPrice"><span class="cssRegularPrice_${itemID} sfLocale">Price :</span><span class="cssRegularPrice_${itemID} cssClassFormatCurrency">${parseFloat(listPrice).toFixed(2)}</span> </p>
                        <p id="cssClassProductsGridRealPrice_${itemID}" class="cssClassProductsGridRealPrice"><span class="cssClassFormatCurrency">${parseFloat(price).toFixed(2)}</span></p>
                    </div>
                </div>

                <div id="compareAddToWishlist" class="sfButtonwrapper clearfix cssClassTMar10">
                    <div class="cssClassWishListButton">
                        <label class="cssClassDarkBtn i-wishlist">
                            <button onclick="WishItemAPI.CheckWishListUniqueness(${itemID},${JSON2.stringify(sku)},${JSON2.stringify(CostVariant)});" id="addWishList" type="button"><span class="sfLocale">Wishlist +</span></button></label>
                    </div>

                    <div class="compareAddToCart cssClassAddtoCard" id="cssClassAddtoCard_${itemID}_${index}" style="display: none">
                        <div class="sfButtonwrapper" data-itemtypeid="${itemTypeID}" data-itemid="${itemID}" data-type="button" data-title="${name}" data-addtocart="addtocart${itemID}" data-onclick="AspxCommerce.RootFunction.AddToCartFromJS(${itemID},${price},${JSON2.stringify(sku)},${1},'${CostVariant}',this);">
                            <label class="cssClassGreenBtn i-cart">
                                <button type="button" addtocart="addtocart${itemID}" title="${name}" onclick="AspxCommerce.RootFunction.AddToCartFromJS(${itemID},${price},${JSON2.stringify(sku)},${1},'${CostVariant}',this);"><span class="sfLocale">Cart +</span></button></label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </td>
</script>

<div id="divItemCompareElements" class="sfFormwrapper">
</div>
<div id="dvCompareList" class="cssClassCommonBox cssClassCompareBox">
    <div class="cssClassHeader">
        <h2>
            <asp:Label ID="lblCompareTitle" runat="server" class="cssClassCompareItem" Text="Compare following Items"
                meta:resourcekey="lblCompareTitleResource1"></asp:Label>
        </h2>
    </div>
    <div id="divCompareElementsPopUP" class="sfFormwrapper cssBox">
        <table id="tblItemCompareList" width="100%" border="0" cellspacing="0" cellpadding="0">
            <thead>
                <tr class="cssGreenBorderBtm">
                    <td></td>
                </tr>
            </thead>
            <tbody id="itemDetailBody">
                <tr>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="cssClassHeaderRight clearfix">
        <div class="sfButtonwrapper">
            <label class="cssClassDarkBtn i-print">
                <button type="button" id="btnPrintItemCompare">
                    <span class="sfLocale">Print</span></button></label>

        </div>
    </div>
</div>
