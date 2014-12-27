<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountDashboard.ascx.cs"
    Inherits="Modules_AspxCommerce_AspxUserDashBoard_AccountDashboard" %>
<script type="text/javascript">
    //<![CDATA[
    var userFriendlyURL = AspxCommerce.utils.IsUserFriendlyUrl();
    var AccountDashboard = '';
    var defaultShippingExist = '<%=defaultShippingExist %>';
        var defaultBillingExist = '<%=defaultBillingExist %>';
    var addressId = '<%=addressId%>';
    aspxCommonObj.UserName = AspxCommerce.utils.GetUserName();
    $(function () {
        $(".sfLocale").localize({
            moduleKey: AspxUserDashBoard
        });       
        AccountDashboard = {
            config: {
                isPostBack: false,
                async: true,
                cache: true,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: 'json',
                baseURL: moduleRootPath,
                method: "",
                url: "",
                oncomplete: 0,
                ajaxCallMode: "",
                error: ""
            },

            ajaxCall: function (config) {
                $.ajax({
                    type: AccountDashboard.config.type,
                    beforeSend: function (request) {
                        request.setRequestHeader('ASPX-TOKEN', _aspx_token);
                        request.setRequestHeader("UMID", userModuleIDUD);
                        request.setRequestHeader("UName", AspxCommerce.utils.GetUserName());
                        request.setRequestHeader("PID", AspxCommerce.utils.GetPortalID());
                        request.setRequestHeader("PType", "v");
                        request.setRequestHeader('Escape', '0');
                    },
                    contentType: AccountDashboard.config.contentType,
                    cache: AccountDashboard.config.cache,
                    async: AccountDashboard.config.async,
                    url: AccountDashboard.config.url,
                    data: AccountDashboard.config.data,
                    dataType: AccountDashboard.config.dataType,
                    success: AccountDashboard.config.ajaxCallMode,
                    error: AccountDashboard.ajaxFailure
                });
            },
            vars: {
                itemQuantity: "",
                itemQuantityInCart: "",
                itemName: "",
                variantName: "",
                notExists: ''
            },
            GetAddressBookDetails: function () {
                this.config.method = "AspxCoreHandler.ashx/GetAddressBookDetails";
                this.config.url = aspxservicePath + this.config.method;
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj });
                this.config.ajaxCallMode = AccountDashboard.BindAddressBookDetails;
                this.ajaxCall(this.config);
            },


            GetSendEmailErrorMsg: function (msg) {
                $('#ddlUSState').show();
                $('#txtState').hide();
                $("#ddlUSState").html('');
                $.each(msg.d, function (index, item) {
                    $("#ddlUSState").append("<option value=" + item.Value + ">" + item.Text + "</option>");
                });
            },

            BindMyOrders: function (msg) {
                var elements = '';
                var tableElements = '';
                var grandTotal = '';
                var couponAmount = '';
                var rewardDiscountAmount = '';
                var taxTotal = '';
                var giftCard = '';
                var shippingCost = '';
                var discountAmount = '';
                $.each(msg.d, function (index, value) {
                    if (index < 1) {
                        var billAdd = '';
                        var arrBill;
                        arrBill = value.BillingAddress.split(',');
                         $.each(arrBill, function (index, value) {
                                billAdd += '<li>' + value + '</li>';
                         });
                        $(".cssBillingAddressUl").html(billAdd);

                        $("#orderedDate").html(' ' + value.OrderedDate);
                        $("#invoicedNo").html(' ' + value.InVoiceNumber);
                        $('#storeName').html(' ' + value.StoreName);
                        $("#paymentGatewayType").html(' ' + value.PaymentGatewayTypeName);
                        $("#paymentMethod").html(' ' + value.PaymentMethodName);
                    }
                    tableElements += '<tr>';
                    tableElements += '<td class="cssClassMyAccItemName">' + value.ItemName + '<br/>' + value.CostVariants + '</td>';
                    tableElements += '<td class="cssClassMyAccItemSKU">' + value.SKU + '</td>';
                    tableElements += '<td class="cssClassMyAccShippingAdd">' + value.ShippingAddress + '</td>';
                    tableElements += '<td class="cssClassMyAccShppingRate"><span class="cssClassFormatCurrency">' + parseFloat(value.ShippingRate).toFixed(2) + '</span></td>';
                    tableElements += '<td class="cssClassMyAccPrice"><span class="cssClassFormatCurrency">' + parseFloat(value.Price).toFixed(2) + '</span></td>';
                    tableElements += '<td class="cssClassMyAccQuantity">' + value.Quantity + '</td>';
                    tableElements += '<td class="cssClassMyAccSubTotal"><span class="cssClassFormatCurrency">' + parseFloat(value.Price * value.Quantity).toFixed(2) + '</span></td>';
                    tableElements += '</tr>';
                    if (index == 0) {
                        var orderID = value.OrderID;
                        $.ajax({
                            type: "POST",
                            url: aspxservicePath + "AspxCoreHandler.ashx/GetTaxDetailsByOrderID",
                            data: JSON2.stringify({ orderId: orderID, aspxCommonObj: aspxCommonObj }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: false,
                            success: function (msg) {
                                $.each(msg.d, function (index, val) {
                                    if (val.TaxSubTotal != 0) {
                                        taxTotal += '<tr><td></td><td></td><td></td><td></td><td></td><td class="cssClassLabel">' + val.TaxManageRuleName + '</td>';
                                        taxTotal += '<td><span class="cssClassFormatCurrency">' + (val.TaxSubTotal).toFixed(2) + '</span></td></tr>';

                                    }
                                });
                            }
                        });
                        shippingCost = '<tr>';
                        shippingCost += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Shipping Cost:') + '</td>';
                        shippingCost += '<td><span class="cssClassFormatCurrency">' + parseFloat(value.ShippingRate).toFixed(2) + '</span></td>';
                        shippingCost += '</tr>';
                        discountAmount = '<tr>';
                        discountAmount += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Discount Amount:') + '</td>';
                        discountAmount += '<td><span class="cssClassMinus"> - </span><span class="cssClassFormatCurrency">' + parseFloat(value.DiscountAmount).toFixed(2) + '</span></td>';
                        discountAmount += '</tr>';
                        couponAmount = '<tr>';
                        couponAmount += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Coupon Amount:') + '</td>';
                        couponAmount += '<td><span class="cssClassMinus"> - </span><span class="cssClassFormatCurrency">' + parseFloat(value.CouponAmount).toFixed(2) + '</span></td>';
                        couponAmount += '</tr>';
                        rewardDiscountAmount = '<tr>';
                        rewardDiscountAmount += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Discount(Reward Points):') + '</td>';
                        rewardDiscountAmount += '<td><span class="cssClassMinus"> - </span><span class="cssClassFormatCurrency cssClassSubTotal">' + parseFloat(value.RewardDiscountAmount).toFixed(2) + '</span></td>';
                        rewardDiscountAmount += '</tr>';
                        if (value.GiftCard != "" && value.GiftCard != null) {
                            var giftCardUsed = value.GiftCard.split('#');
                            for (var g = 0; g < giftCardUsed.length; g++) {
                                var keyVal = giftCardUsed[g].split('=');
                                giftCard += '<tr>';
                                giftCard += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Gift Card') + '(' + keyVal[0] +
                                    ') :</td>';
                                giftCard += '<td ><span class="cssClassMinus"> - </span><span class="cssClassFormatCurrency" >' + parseFloat(keyVal[1]).toFixed(2) + '</span></td>';
                                giftCard += '</tr>';
                            }
                        }
                        grandTotal = '<tr>';
                        grandTotal += '<td colspan="4"></td><td class="cssClassLabel cssClassRowbold" colspan="2">' + getLocale(AspxUserDashBoard, 'Grand Total:') + '</td>';
                        grandTotal += '<td class="cssClassGrandTotal"><span class="cssClassFormatCurrency">' + parseFloat(value.GrandTotal).toFixed(2) + '</span></td>';
                        grandTotal += '</tr>';

                        if (value.OrderType == 2) {
                            $.ajax({
                                type: "POST",
                                url: aspxservicePath + "AspxCommerceWebService.asmx/GetServiceDetailsByOrderID",
                                data: JSON2.stringify({ orderID: orderID, aspxCommonObj: aspxCommonObj }),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                async: false,
                                success: function (msg) {
                                    if (msg.d.length > 0) {
                                        $('.cssClassServiceDetails').show();
                                        $.each(msg.d, function (index, val) {
                                            $('#serviceName').html(' ' + val.ServiceCategoryName);
                                            $('#serviceProductName').html(' ' + val.ServiceProductName);
                                            $('#serviceDuration').html(' ' + val.ServiceDuration);
                                            $('#providerName').html(' ' + val.EmployeeName);
                                            $('#storeLocationName').html(' ' + val.StoreLocationName);
                                            var date = 'new ' + val.PreferredDate.replace(/[/]/gi, '');
                                            ;
                                            date = eval(date);
                                            $('#serviceDate').html(' ' + formatDate(date, 'yyyy/MM/dd'));
                                            $('#availableTime').html(' ' + val.PreferredTime);
                                            $('#bookAppointmentTime').html(' ' + val.PreferredTimeInterval);
                                        });
                                    }
                                }
                            });
                        } else {
                            $('.cssClassServiceDetails').hide();
                            $('#serviceName').html(' ');
                            $('#serviceProductName').html(' ');
                            $('#serviceDuration').html(' ');
                            $('#providerName').html(' ');
                            $('#storeLocationName').html(' ');
                            $('#serviceDate').html(' ');
                            $('#availableTime').html(' ');
                            $('#bookAppointmentTime').html(' ');
                        }
                    }
                });
                $("#divOrderDetails").find('table>tbody').html(tableElements);
                $("#divOrderDetails").find('table>tbody').append(taxTotal);
                $("#divOrderDetails").find('table>tbody').append(shippingCost);
                $("#divOrderDetails").find('table>tbody').append(discountAmount);
                $("#divOrderDetails").find('table>tbody').append(couponAmount);
                $("#divOrderDetails").find('table>tbody').append(rewardDiscountAmount);
                $("#divOrderDetails").find('table>tbody').append(giftCard);
                $("#divOrderDetails").find('table>tbody').append(grandTotal);
                $("#divOrderDetails").find('table>tbody>tr:even').addClass('sfEven');
                $("#divOrderDetails").find('table>tbody>tr:odd').addClass('sfOdd');
                AccountDashboard.OrderHideAll();
                $("#divOrderDetails").show();
                var cookieCurrency = $("#ddlCurrency").val();
                Currency.currentCurrency = BaseCurrency;
                Currency.convertAll(Currency.currentCurrency, cookieCurrency);
            },

            BindAddressBookDetails: function (msg) {
                var defaultBillingAddressElements = '';
                var defaultShippingAddressElements = '';
                var addressElements = '';
                var addressId = 0;
                var defaultShippingExist = false;
                var defaultBillingExist = false;
                $.each(msg.d, function (index, value) {
                    if (value.DefaultBilling && value.DefaultShipping) {
                        addressId = value.AddressID;
                    }

                    if (!defaultShippingExist) {
                        if ((value.DefaultShipping != null || value.DefaultShipping)) {
                            defaultShippingExist = true;
                        }
                        else {
                            defaultShippingExist = false;
                        }
                    }

                    if (!defaultBillingExist) {
                        if (value.DefaultBilling != null || value.DefaultBilling) {
                            defaultBillingExist = true;
                        }
                        else {
                            defaultBillingExist = false;
                        }
                    }

                    if (value.DefaultBilling || value.DefaultShipping) {
                        if (value.DefaultShipping) {
                            defaultShippingAddressElements += '<h3>' + getLocale(AspxUserDashBoard, 'Default Shipping Address') + '</h3>';
                            defaultShippingAddressElements += '<div><span name="FirstName">' + value.FirstName + '</span>' + " " + '<span name="LastName">' + value.LastName + '</span></div>';
                            defaultShippingAddressElements += '<div><span name="Email">' + value.Email + '</span></div>';
                            if (value.Company != "") {
                                defaultShippingAddressElements += '<div><span name="Company">' + value.Company + '</span></div>';
                            }
                            defaultShippingAddressElements += '<div><span name="Address1">' + value.Address1 + '</span></div>';
                            if (value.Address2 != "") {
                                defaultShippingAddressElements += '<div><span name="Address2">' + value.Address2 + '</span></div>';
                            }
                            defaultShippingAddressElements += '<div><span name="City">' + value.City + '</span>, ';
                            defaultShippingAddressElements += '<span name="State">' + value.State + '</span>, ';
                            defaultShippingAddressElements += '<span name="Country">' + value.Country + '</span></div>';
                            defaultShippingAddressElements += '<div>Zip: <span name="Zip">' + value.Zip + '</span></div>';
                            defaultShippingAddressElements += '<div><i class="i-phone"></i><span name="Phone">' + value.Phone + '</span></div>';
                            if (value.Mobile != "") {
                                defaultShippingAddressElements += '<div><i class="i-mobile"></i><span name="Mobile">' + value.Mobile + '</span></div>';
                            }
                            if (value.Fax != "") {
                                defaultShippingAddressElements += '<div><i class="i-fax"></i><span name="Fax">' + value.Fax + '</span></div>';
                            }
                            if (value.Website != "") {
                                defaultShippingAddressElements += '<div><span name="Website">' + value.Website + '</span></div>';
                            }
                            defaultShippingAddressElements += '</div>';

                            $("#liDefaultShippingAddress").html(defaultShippingAddressElements);
                        }
                        if (value.DefaultBilling) {
                            defaultBillingAddressElements += '<h3>' + getLocale(AspxUserDashBoard, 'Default Billing Address') + '</h3>';
                            defaultBillingAddressElements += '<div><span name="FirstName">' + value.FirstName + '</span>' + " " + '<span name="LastName">' + value.LastName + '</span></div>';
                            defaultBillingAddressElements += '<div><span name="Email">' + value.Email + '</span></div>';
                            if (value.Company != "") {
                                defaultBillingAddressElements += '<div><span name="Company">' + value.Company + '</span></div>';
                            }
                            defaultBillingAddressElements += '<div><span name="Address1">' + value.Address1 + '</span></div>';
                            if (value.Address2 != "") {
                                defaultBillingAddressElements += '<div><span name="Address2">' + value.Address2 + '</span></div>';
                            }
                            defaultBillingAddressElements += '<div><span name="City">' + value.City + '</span>, ';
                            defaultBillingAddressElements += '<span name="State">' + value.State + '</span>, ';
                            defaultBillingAddressElements += '<span name="Country">' + value.Country + '</span></div>';
                            defaultBillingAddressElements += '<div>Zip: <span name="Zip">' + value.Zip + '</span></div>';
                            defaultBillingAddressElements += '<div><i class="i-phone"></i><span name="Phone">' + value.Phone + '</span></div>';
                            if (value.Mobile != "") {
                                defaultBillingAddressElements += '<div><i class="i-mobile"></i><span name="Mobile">' + value.Mobile + '</span></div>';
                            }
                            if (value.Fax != "") {
                                defaultBillingAddressElements += '<div><i class="i-fax"></i><span name="Fax">' + value.Fax + '</span></div>';
                            }
                            if (value.Website != "") {
                                defaultBillingAddressElements += '<div><span name="Website">' + value.Website + '</span></div>';
                            }
                            defaultBillingAddressElements += '</div>';
                            $("#liDefaultBillingAddress").html(defaultBillingAddressElements);
                        }
                    }
                });

                if (defaultShippingExist) {
                    $("#hdnDefaultShippingExist").val('1');
                }
                else {
                    $("#hdnDefaultShippingExist").val('0');
                    $("#liDefaultShippingAddress").html("<span class=\"cssClassNotFound\">" + getLocale(AspxUserDashBoard, "You have not set Default Shipping Adresss Yet!") + "</span>");
                }
                if (defaultBillingExist) {
                    $("#hdnDefaultBillingExist").val('1');
                }
                else {
                    $("#hdnDefaultBillingExist").val('0');
                    $("#liDefaultBillingAddress").html("<span class=\"cssClassNotFound\">" + getLocale(AspxUserDashBoard, "You have not set Default Billing Adresss Yet!") + "</span>");
                }
            },

            BindUserAddressOnUpdate: function () {
                AccountDashboard.GetAddressBookDetails();
                RemovePopUp();
            },

            BindCountryList: function (msg) {
                var countryElements = '';
                $.each(msg.d, function (index, value) {
                    countryElements += '<option value=' + value.Value + '>' + value.Text + '</option>';
                });
                $("#ddlCountry").html(countryElements);
            },

            ajaxFailure: function () {
            },

            GetStateList: function (countryCode) {
                this.config.method = "AspxCoreHandler.ashx/BindStateList";
                this.config.url = aspxservicePath + this.config.method;
                this.config.data = JSON2.stringify({ countryCode: countryCode }),
                this.config.ajaxCallMode = AccountDashboard.BindStateList;
                this.ajaxCall(this.config);

            },
            BindStateList: function (msg) {
                $('#ddlUSState').show();
                $('#txtState').hide();
                $("#ddlUSState").html('');
                $.each(msg.d, function (index, item) {
                    if (item.Text != 'NotExists') {
                        $("#ddlUSState").append("<option value=" + item.Value + ">" + item.Text + "</option>");
                    } else {
                        $('#ddlUSState').hide();
                        $('#txtState').show();
                    }
                });
            },
            OrderHideAll: function () {
                $("#divMyOrders").hide();
                $("#divOrderDetails").hide();
                $("#popuprel").hide();
            },

            GetMyOrders: function () {

                var offset_ = 1;
                var current_ = 1;
                var perpage = ($("#gdvMyOrder_pagesize").length > 0) ? $("#gdvMyOrder_pagesize :selected").text() : 10;
                $("#gdvMyOrders").sagegrid({
                    url: this.config.baseURL + "UserDashBoardHandler.ashx/",
                    functionMethod: 'GetMyOrderList',
                    colModel: [
                    { display: getLocale(AspxUserDashBoard, 'Order ID'), name: 'order_id', cssclass: 'cssClassHeadNumber', coltype: 'label', align: 'left' },
                    { display: getLocale(AspxUserDashBoard, 'Invoice Number'), name: 'invoice_number', cssclass: 'cssClassHeadNumber', controlclass: '', coltype: 'label', align: 'left' },
                    { display: 'CustomerID', name: 'customerID', cssclass: 'cssClassHeadNumber', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: getLocale(AspxUserDashBoard, 'Customer Name'), name: 'customer_name', cssclass: '', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: 'Email', name: 'email', cssclass: '', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: getLocale(AspxUserDashBoard, 'Order Status'), name: 'order_status', cssclass: '', controlclass: '', coltype: 'label', align: 'left' },
                    { display: 'Grand Total', name: 'grand_total', cssclass: '', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: 'Payment Gateway Type Name', name: 'payment_gateway_typename', cssclass: '', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: 'Payment Method Name', name: 'payment_method_name', cssclass: '', controlclass: '', coltype: 'label', align: 'left', hide: true },
                    { display: getLocale(AspxUserDashBoard, 'Ordered Date'), name: 'ordered_date', cssclass: 'cssClassHeadDate', controlclass: '', coltype: 'label', align: 'left' },
                    { display: getLocale(AspxUserDashBoard, 'Actions'), name: 'action', cssclass: 'cssClassAction', coltype: 'label', align: 'center' }

                    ],

                    buttons: [
            { display: getLocale(AspxUserDashBoard, 'View'), name: 'viewOrder', enable: true, _event: 'click', trigger: '1', callMethod: 'AccountDashboard.GetOrderDetails', arguments: '' },
              { display: getLocale(AspxUserDashBoard, 'Reorder'), name: 'Reorder', enable: true, _event: 'click', trigger: '1', callMethod: 'AccountDashboard.ReOrder', arguments: '' },
               { display: getLocale(AspxUserDashBoard, 'Return'), name: 'Return', enable: true, _event: 'click', trigger: '1', callMethod: 'AccountDashboard.LoadControl', arguments: '' }
                    ],
                    rp: perpage,
                    nomsg: getLocale(AspxUserDashBoard, "No Records Found!"),
                    param: { aspxCommonObj: aspxCommonObj },
                    current: current_,
                    pnew: offset_,
                    sortcol: { 10: { sorter: false } }
                });
            },

            LoadControl: function (tblID, argus) {
                var controlName = "Modules/AspxCommerce/AspxReturnAndPolicy/ReturnsSubmit.ascx";
                $.ajax({
                    type: "POST",
                    url: AspxCommerce.utils.GetAspxServicePath() + "LoadControlHandler.aspx/Result",
                    data: "{ controlName:'" + AspxCommerce.utils.GetAspxRootPath() + controlName + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#divLoadUserControl').html(response.d);
                        var orderID = (argus[0]);
                        ReturnsSubmit.GetOrderDetails(orderID);
                    },
                    error: function () {
                        csscody.error('<h2>' + getLocale(AspxUserDashBoard, 'Error Message') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Failed to load control!.') + '</p>');
                    }
                });
            },



            ReOrder: function (tblID, argus) {
                switch (tblID) {
                    case "gdvMyOrders":
                        AccountDashboard.GetReOrderDetails(argus[0]);
                        break;
                }
            },
            GetReOrderDetails: function (argus) {
                var orderId = argus;
                this.config.method = "UserDashBoardHandler.ashx/GetMyOrdersforReOrder";
                this.config.url = moduleRootPath + this.config.method;
                this.config.data = JSON2.stringify({ orderID: orderId, aspxCommonObj: aspxCommonObj });
                this.config.ajaxCallMode = AccountDashboard.ReOrderItems;
                this.config.async = false;
                this.ajaxCall(this.config);
            },
            ReOrderItems: function (data) {
                $.each(data.d, function (index, value) {
                    var itemId = value.ItemID;
                    itemName = value.ItemName;
                    var itemPrice = value.Price;
                    var itemSKU = value.SKU;
                    var itemQuantity = 1;
                    var totalWeightVariant = value.Weight;
                    var itemCostVariantIDs = value.ItemCostVariantValueIDs;
                    variantName = value.CostVariants;
                    var sessionCode = AspxCommerce.utils.GetSessionCode();
                    if (itemCostVariantIDs == '' || itemCostVariantIDs == null) {
                        AccountDashboard.AddToCartFromJS(itemId, itemPrice, itemSKU, itemQuantity, aspxCommonObj);
                    }
                    else {
                        var itemQuantityTotal = AccountDashboard.CheckItemQuantity(itemId, itemCostVariantIDs);
                        var itemQuantityInCart = AccountDashboard.CheckItemQuantityInCart(itemId, itemCostVariantIDs + '@');
                        if (itemQuantityInCart != 0.1) {
                            if (itemQuantityTotal <= 0) {
                                csscody.alert("<h2>" + getLocale(AspxUserDashBoard, 'Information Alert') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Product') + " " + '(' + itemName + " " + ',' + variantName + ')' + " " + getLocale(AspxUserDashBoard, 'is currently Out Of Stock!') + "</p>");
                            } else {
                                if ((eval(itemQuantity) + eval(itemQuantityInCart)) > eval(itemQuantityTotal)) {
                                    csscody.alert("<h2>" + getLocale(AspxUserDashBoard, 'Information Alert') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Product') + " " + '(' + itemName + " " + ',' + variantName + ')' + " " + getLocale(AspxUserDashBoard, 'is currently Out Of Stock!') + "</p>");
                                }
                                else {
                                    AccountDashboard.AddItemstoCart(itemId, itemPrice, totalWeightVariant, itemQuantity, itemCostVariantIDs, aspxCommonObj);

                                }
                            }
                        }
                    }
                });
            },
            AddToCartFromJS: function (itemId, itemPrice, itemSKU, itemQuantity, aspxCommonObj) {

                var param = { itemID: itemId, itemPrice: itemPrice, itemQuantity: itemQuantity, aspxCommonObj: aspxCommonObj };
                var data = JSON2.stringify(param);
                var myCartUrl;
                var addToCartProperties = {
                    onComplete: function (e) {
                        if (e) {
                            if (AspxCommerce.utils.IsUserFriendlyUrl) {
                                myCartUrl = myCartURL + pageExtension;
                            } else {
                                myCartUrl = myCartURL;
                            }
                            window.location.href = AspxCommerce.utils.GetAspxRedirectPath() + myCartUrl;
                        }
                    }
                };
                $.ajax({
                    type: "POST",
                    url: aspxservicePath + "AspxCommonHandler.ashx/AddItemstoCart",
                    data: data,
                    async: false,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        if (msg.d == 1) {
                            AspxCommerce.RootFunction.RedirectToItemDetails(itemSKU);
                        }
                        else if (msg.d == 2) {
                            csscody.alert("<h2>" + getLocale(AspxUserDashBoard, 'Information Alert') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Product') + " " + '(' + itemName + ')' + " " + getLocale(AspxUserDashBoard, 'is currently Out Of Stock!') + "</p>");
                            HeaderControl.GetCartItemTotalCount(); ShopingBag.GetCartItemCount(); ShopingBag.GetCartItemListDetails();
                        }
                        else {
                            try {
                                var realTimeEvent = $.connection._aspxrthub;
                                realTimeEvent.server.checkIfItemOutOfStock(itemId, itemSKU, "", AspxCommerce.AspxCommonObj());
                            }
                            catch (Exception) {
                            }
                            csscody.info('<h2>' + getLocale(AspxUserDashBoard, "Successful Message") + '</h2><p>' + getLocale(AspxUserDashBoard, 'Item') + " " + '(' + itemName + ')' + " " + getLocale(AspxUserDashBoard, 'has been successfully added to cart.') + '</p>', addToCartProperties);
                            HeaderControl.GetCartItemTotalCount(); ShopingBag.GetCartItemCount(); ShopingBag.GetCartItemListDetails();
                        }
                    }
                });
            },

            AddItemstoCart: function (itemId, itemPrice, totalWeightVariant, itemQuantity, itemCostVariantIDs, aspxCommonObj) {

                var costVariantIDs = itemCostVariantIDs + '@';
                var isgiftCard = false;
                var giftCardDetail = null;

                var AddItemToCartObj = {
                    ItemID: itemId,
                    Price: itemPrice,
                    Weight: totalWeightVariant,
                    Quantity: itemQuantity,
                    CostVariantIDs: costVariantIDs,
                    IsGiftCard: isgiftCard
                };
                this.config.method = "AspxCommonHandler.ashx/AddItemstoCartFromDetail";
                this.config.url = aspxservicePath + this.config.method;
                this.config.data = JSON2.stringify({ aspxCommonObj: aspxCommonObj, AddItemToCartObj: AddItemToCartObj, giftCardDetail: giftCardDetail });
                this.config.ajaxCallMode = AccountDashboard.AddItemstoCartFromDetail;
                this.config.oncomplete = 20;
                this.config.error = AccountDashboard.GetAddToCartErrorMsg;
                this.config.async = false;
                this.ajaxCall(this.config);
            },
            AddItemstoCartFromDetail: function (msg) {
                if (msg.d == 1) {
                    var myCartUrl;
                    if (userFriendlyURL) {
                        myCartUrl = myCartURL + pageExtension;
                    } else {
                        myCartUrl = myCartURL;
                    }
                    var addToCartProperties = {
                        onComplete: function (e) {
                            if (e) {
                                window.location.href = AspxCommerce.utils.GetAspxRedirectPath() + myCartURL + pageExtension;
                            }
                        }
                    };
                    csscody.info('<h2>' + getLocale(AspxUserDashBoard, "Successful Message") + '</h2><p>' + getLocale(AspxUserDashBoard, 'Item') + " " + '(' + itemName + " " + ',' + variantName + ')' + " " + getLocale(AspxUserDashBoard, 'has been successfully added to cart.') + '</p>', addToCartProperties);
                    HeaderControl.GetCartItemTotalCount(); ShopingBag.GetCartItemCount(); ShopingBag.GetCartItemListDetails();
                }
                else if (msg.d == 2) {
                    csscody.alert("<h2>" + getLocale(AspxUserDashBoard, 'Information Alert') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Product') + " " + '(' + itemName + " " + ',' + variantName + ')' + " " + getLocale(AspxUserDashBoard, 'is currently Out Of Stock!') + "</p>");
                }
            },

            GetAddToCartErrorMsg: function () {
                csscody.error('<h2>' + getLocale(AspxUserDashBoard, 'Information Alert') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Failed') + " " + '(' + itemName + " " + ',' + variantName + ')' + " " + getLocale(AspxUserDashBoard, 'to add item to cart!') + '</p>');
            },
            oncomplete: function () {
                switch (ItemDetail.config.oncomplete) {
                    case 20:
                        ItemDetail.config.oncomplete = 0;
                        if ($("#divCartDetails").length > 0) {
                            AspxCart.GetUserCartDetails();
                        }
                        break;
                }
            },

            CheckItemQuantity: function (itemId, itemCostVariantIDs) {
                this.config.method = "UserDashBoardHandler.ashx/CheckItemQuantity";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ itemID: itemId, aspxCommonObj: aspxCommonObj, itemCostVariantIDs: itemCostVariantIDs });
                this.config.ajaxCallMode = AccountDashboard.SetItemQuantity;
                this.config.async = false;
                this.ajaxCall(this.config);
                return AccountDashboard.vars.itemQuantity;
            },
            SetItemQuantity: function (msg) {
                AccountDashboard.vars.itemQuantity = msg.d;
            },
            CheckItemQuantityInCart: function (itemId, itemCostVariantIDs) {
                this.config.method = "AspxCoreHandler.ashx/CheckItemQuantityInCart";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ itemID: itemId, aspxCommonObj: aspxCommonObj, itemCostVariantIDs: itemCostVariantIDs });
                this.config.ajaxCallMode = AccountDashboard.SetItemQuantityInCart;
                this.config.async = false;
                this.ajaxCall(this.config);
                return AccountDashboard.vars.itemQuantityInCart;
            },
            SetItemQuantityInCart: function (msg) {
                AccountDashboard.vars.itemQuantityInCart = msg.d;
            },

            GetOrderDetails: function (tblID, argus) {
                switch (tblID) {
                    case "gdvMyOrders":
                        AccountDashboard.GetAllOrderDetails(argus[0]);
                        break;
                }
            },

            GetAllOrderDetails: function (argus) {
                var orderId = argus;
                this.config.method = "UserDashBoardHandler.ashx/GetMyOrders";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ orderID: orderId, aspxCommonObj: aspxCommonObj });
                this.config.ajaxCallMode = AccountDashboard.BindMyOrders;
                this.ajaxCall(this.config);
            },

            GetCheckOutPage: function (tdlID, argus) {
                switch (tdlID) {
                    case "gdvMyOrders":
                        break;
                    default:
                        break;
                }
            },

            ClearAll: function () {
                $("#hdnAddressID").val(0);
                $("#txtFirstName").val('');
                $("#txtLastName").val('');
                $("#txtEmailAddress").val('');
                $("#txtCompanyName").val('');
                $("#txtAddress1").val('');
                $("#txtAddress2").val('');
                $("#txtCity").val('');
                $("#txtState").val('');
                $('#ddlCountry').val(1);
                $("#ddlUSState").val(1);
                $("#txtZip").val('');
                $("#txtPhone").val('');
                $("#txtMobile").val('');
                $("#txtFax").val('');
                $("#txtWebsite").val('');
                $("#chkShippingAddress").removeAttr("checked");
                $("#chkBillingAddress").removeAttr("checked");
                $("#chkShippingAddress").removeAttr("disabled");
                $("#chkBillingAddress").removeAttr("disabled");
            },

            AddUpdateUserAddress: function () {
                var addressId = $("#hdnAddressID").val();
                var firstName = $("#txtFirstName").val();
                var lastName = $("#txtLastName").val();
                var email = $("#txtEmailAddress").val();
                var company = $("#txtCompanyName").val();
                var address1 = $("#txtAddress1").val();
                var address2 = $("#txtAddress2").val();
                var city = $("#txtCity").val();
                var state = '';
                if (AccountDashboard.vars.notExists != "NotExists") {
                    state = $("#ddlUSState :selected").text();
                }
                else {
                    state = $("#txtState").val();
                }
                var zip = $("#txtZip").val();
                var phone = $("#txtPhone").val();
                var mobile = $("#txtMobile").val();
                var fax = $("#txtFax").val();
                var webSite = $("#txtWebsite").val();
                var countryName = $("#ddlCountry :selected").text();
                var isDefaultShipping = $("#chkShippingAddress").prop("checked");
                var isDefaultBilling = $("#chkBillingAddress").prop("checked");
                this.config.method = "UserDashBoardHandler.ashx/AddUpdateUserAddress";
                this.config.url = this.config.baseURL + this.config.method;
                var addressObj = {
                    AddressID: addressId,
                    FirstName: firstName,
                    LastName: lastName,
                    Email: email,
                    Company: company,
                    Address1: address1,
                    Address2: address2,
                    City: city,
                    State: state,
                    Zip: zip,
                    Phone: phone,
                    Mobile: mobile,
                    Fax: fax,
                    Country: countryName,
                    WebSite: webSite,
                    DefaultShipping: isDefaultShipping,
                    DefaultBilling: isDefaultBilling
                };
                this.config.data = JSON2.stringify({
                    addressObj: addressObj, aspxCommonObj: aspxCommonObj
                });
                this.config.ajaxCallMode = AccountDashboard.BindUserAddressOnUpdate;
                this.ajaxCall(this.config);

            },

            GetAllCountry: function () {
                this.config.method = "AspxCoreHandler.ashx/BindCountryList";
                this.config.url = aspxservicePath + this.config.method;
                this.config.data = "{}";
                this.config.ajaxCallMode = AccountDashboard.BindCountryList;
                this.ajaxCall(this.config);
            },

            Init: function () {
                WishItemAPI.Count();
                $.validator.addMethod("alpha_dash", function (value, element) {
                    return this.optional(element) || /^[a-z0-9_ \-]+$/i.test(value);
                });
                var v1 = $("#form1").validate({
                    rules: {
                        Phone: {
                            required: true,
                            digits: true
                        },
                        Mobile: {
                            digits: true
                        },
                        Fax: {
                            digits: true
                        },
                        Zip: { "alpha_dash": true, "required": true }
                    },
                    messages: {
                        FirstName: {
                            required: '*',
                            minlength: "*" + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*"
                        },
                        LastName: {
                            required: '*',
                            minlength: "*" + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*"
                        },
                        Email: {
                            required: '*',
                            email: getLocale(AspxUserDashBoard, "Please enter valid email id")
                        },
                        Wedsite: {
                            url: '*'
                        },
                        Address1: {
                            required: '*',
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*"
                        },
                        Address2: {
                            maxlength: "*"
                        },
                        Phone: {
                            required: '*',
                            minlength: "*" + getLocale(AspxUserDashBoard, "(at least 7 digits)") + "",
                            maxlength: "*",

                        },
                        Mobile: {
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 10 digits)") + "",
                            maxlength: "*"
                        },
                        Fax: {
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 7 digits)") + "",
                            maxlength: "*"
                        },
                        Zip: {
                            required: '*',
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*",
                            alpha_dash: "* " + getLocale(AspxUserDashBoard, "(no special character allowed)") + ""
                        },
                        State: {
                            required: '*',
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*"
                        },
                        Zip: {
                            required: '*',
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 4 chars)") + "",
                            maxlength: "*"
                        },
                        City: {
                            required: '*',
                            minlength: "* " + getLocale(AspxUserDashBoard, "(at least 2 chars)") + "",
                            maxlength: "*"
                        }
                    }, ignore: ":hidden"
                });

                $('#btnSubmitAddress').click(function () {

                    if (v1.form()) {
                        $.ajax({
                            type: "POST",
                            url: aspxservicePath + "AspxCoreHandler.ashx/",
                            data: JSON2.stringify({ countryCode: $("#ddlCountry :selected").val() }),
                            contentType: "application/json;charset=utf-8",
                            dataType: "json",
                            success: function (msg) {
                                var state = '';
                                if (msg.d.length > 2) {

                                    state = $("#ddlUSState :selected").text();
                                } else {
                                    state = $("#txtState").val();
                                }
                                AccountDashboard.AddUpdateUserAddress(state);
                            }
                        });
                        csscody.info("<h2>" + getLocale(AspxUserDashBoard, "Information Message") + "</h2><p>" + getLocale(AspxUserDashBoard, "Address saved Successfully.") + "</p>");
                        return false;
                    }
                    else {
                        return false;
                    }
                });

                $("#spanUserName").html(' ' + userName + '');
                AccountDashboard.GetMyOrders();
                AccountDashboard.OrderHideAll();
                $("#divMyOrders").show();

                $("#lnkBack").on("click", function () {
                    AccountDashboard.OrderHideAll();
                    $("#divMyOrders").show();
                });

                if (defaultShippingExist.toLowerCase() == 'true') {
                    $("#hdnDefaultShippingExist").val('1');
                }
                else {
                    $("#hdnDefaultShippingExist").val('0');
                    $("#liDefaultShippingAddress").html("<span class=\"cssClassNotFound\">" + getLocale(AspxUserDashBoard, 'You have not set Default Shipping Adresss Yet!') + "</span>");
                }
                if (defaultBillingExist.toLowerCase() == 'true') {
                    $("#hdnDefaultBillingExist").val('1');
                }
                else {
                    $("#hdnDefaultBillingExist").val('0');
                    $("#liDefaultBillingAddress").html("<span class=\"cssClassNotFound\">" + getLocale(AspxUserDashBoard, 'You have not set Default Billing Adresss Yet!') + "</span>");
                }

                $("a[name='EditAddress']").on("click", function () {
                    AccountDashboard.ClearAll();
                    $("#hdnAddressID").val($(this).attr("value"));
                    $("#txtFirstName").val($(this).parent('div').prev('div').find('span[name="FirstName"]').text());
                    $("#txtLastName").val($(this).parent('div').prev('div').find('span[name="LastName"]').text());
                    $("#txtEmailAddress").val($(this).parent('div').prev('div').find('span[name="Email"]').text());
                    $("#txtCompanyName").val($(this).parent('div').prev('div').find('span[name="Company"]').text());
                    $("#txtAddress1").val($(this).parent('div').prev('div').find('span[name="Address1"]').text());
                    $("#txtAddress2").val($(this).parent('div').prev('div').find('span[name="Address2"]').text());
                    $("#txtCity").val($(this).parent('div').prev('div').find('span[name="City"]').text());
                    $("#txtZip").val($(this).parent('div').prev('div').find('span[name="Zip"]').text());
                    var countryName = $.trim($(this).parent('div').prev('div').find('span[name="Country"]').text());
                    $('#ddlCountry').val($('#ddlCountry option:contains(' + countryName + ')').attr('value'));
                    var countryCode = $('#ddlCountry').val();
                    var txtState = $(this).parent('div').prev('div').find('span[name="State"]').text();
                    $("#ddlUSState").html('');
                    $.ajax({
                        type: "POST",
                        url: aspxservicePath + "AspxCoreHandler.ashx/BindStateList",
                        data: JSON2.stringify({ countryCode: countryCode }),
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                            $.each(msg.d, function (index, item) {
                                if (item.Text != 'NotExists') {
                                    $('#ddlUSState').show();
                                    $('#txtState').hide();
                                    if (txtState != '' || txtState != null) {
                                        $("#ddlUSState").append("<option value=" + item.Value + ">" + item.Text + "</option>");
                                    }
                                } else {
                                    AccountDashboard.vars.notExists = item.Text;
                                    $('#ddlUSState').hide();
                                    $('#txtState').show();
                                    $("#txtState").val(txtState);
                                }
                            });
                            $('#ddlUSState option').filter(function () { return ($(this).text() == $.trim(txtState)); }).prop('selected', 'selected');
                        }
                    });
                    $("#txtPhone").val($(this).parent('div').prev('div').find('span[name="Phone"]').text());
                    $("#txtMobile").val($(this).parent('div').prev('div').find('span[name="Mobile"]').text());
                    $("#txtFax").val($(this).parent('div').prev('div').find('span[name="Fax"]').text());
                    $("#txtWebsite").val($(this).parent('div').prev('div').find('span[name="Website"]').text());

                    $("#chkShippingAddress").removeAttr("checked");
                    $("#chkBillingAddress").removeAttr("checked");

                    if ($(this).attr("value") == addressId) {

                        $('#trBillingAddress ,#trShippingAddress').hide();

                        $("#chkBillingAddress").prop("disabled", "disabled");
                        $("#chkShippingAddress").prop("disabled", "disabled");
                    }
                    else if ($(this).attr("Flag") == 1) {

                        if ($(this).attr("Element") == "Billing") {

                            $("#chkBillingAddress").prop("disabled", "disabled");
                            $("#chkShippingAddress").removeAttr("disabled");
                        }
                        else {
                            $("#chkShippingAddress").prop("disabled", "disabled");
                            $("#chkBillingAddress").removeAttr("disabled");
                        }
                    }
                    else {
                        $("#chkShippingAddress").removeAttr("disabled");
                        $("#chkBillingAddress").removeAttr("disabled");
                    }
                    ShowPopup(this);

                });
                $("#lnkNewAddress").bind("click", function () {
                    AccountDashboard.ClearAll();
                    if ($("#hdnDefaultShippingExist").val() == "0") {
                        $("#chkShippingAddress").prop("checked", "checked");
                        $("#chkShippingAddress").prop("disabled", "disabled");
                    }
                    if ($("#hdnDefaultBillingExist").val() == "0") {
                        $("#chkBillingAddress").prop("checked", "checked");
                        $("#chkBillingAddress").prop("disabled", "disabled");
                    }
                    ShowPopup(this);
                });
                $(".cssClassClose").on("click", function () {
                    RemovePopUp();
                });
                $("#btnCancelAddNewAddress").on("click", function () {
                    RemovePopUp();
                });
                $("#btnAddNewAddress").on("click", function () {
                    RemovePopUp();
                });

                $('#ddlUSState').hide();
                $('#trBillingAddress ,#trShippingAddress').hide();
                $("#ddlCountry ").on("change", function () {
                    AccountDashboard.GetStateList($(this).val());
                    $('#txtState').val('');

                });
            }
        };
        AccountDashboard.Init();
    });
    //]]>
</script>
<div class="clearfix">
    <div class="welcome-msg">
        <h2 class="sub-title">
            <span class="sfLocale">Hello,</span><span id="spanUserName"></span>
        </h2>
        <p class="sfLocale">
            From your My Account Dashboard you have the ability to view a snapshot of your recent
            account activity and update your account information. Select a link below to view
            or edit information.
        </p>
    </div>
    <div class="cssCustomerRecentActivity">
        <asp:Literal ID="ltrRecentActivity" runat="server"></asp:Literal>
    </div>
</div>
<div id="divMyOrders">
    <div class="cssClassCommonBox Curve">
        <div class="cssClassHeader">
            <h2>
                <span id="lblTitle" class="sfLocale">My Orders</span>
            </h2>
        </div>
        <div class="sfGridwrapper">
            <div class="sfGridWrapperContent">
                <div class="loading">
                    <img id="ajaxAccountDashBoardImage" src="" alt="loading...." class="sfLocale" title="loading...." />
                </div>
                <div class="log">
                </div>
                <table id="gdvMyOrders" cellspacing="0" cellpadding="0" border="0" width="100%">
                </table>
            </div>
        </div>
    </div>
</div>
<div id="divOrderDetails" class="sfFormwrapper clearfix">
    <div class="cssClassStoreDetail cssClassBMar30 clearfix">
        <ul>
            <li><span class="cssClassLabel sfLocale">Ordered Date: </span><span id="orderedDate">
            </span></li>
            <li><span class="cssClassLabel sfLocale">Invoice Number: </span><span id="invoicedNo">
            </span></li>
            <li><span class="cssClassLabel sfLocale">Store Name: </span><span id="storeName"></span>
            </li>
            <li class="cssPaymentDetail"><span class="cssClassLabel sfLocale">Payment Method: </span>
                <span id="paymentMethod"></span></li>
        </ul>
    </div>
    <div class="cssClassBillingAddress cssClassStorePayment cssBox">
        <h2 class="sfLocale">
            Billing Address :</h2>
        <ul class="cssBillingAddressUl cssClassTMar10">
        </ul>
    </div>
    <div class="cssClassServiceDetails" style="display: none">
        <ul>
            <li>
                <h2>
                    <span class="sfLocale">Service Details:</span></h2>
            </li>
            <li>
                <label class="sfLocale">
                    Service Name:</label><span class="cssClassLabel" id="serviceName"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Product Name:</label><span class="cssClassLabel" id="serviceProductName"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Duration:</label><span class="cssClassLabel" id="serviceDuration"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Provider Name:</label><span class="cssClassLabel" id="providerName"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Store Location:</label><span class="cssClassLabel" id="storeLocationName"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Date:</label><span class="cssClassLabel" id="serviceDate"></span> </li>
            <li>
                <label class="sfLocale">
                    Available Time:</label><span class="cssClassLabel" id="availableTime"></span>
            </li>
            <li>
                <label class="sfLocale">
                    Appointment Time:</label><span class="cssClassLabel" id="bookAppointmentTime"></span>
            </li>
        </ul>
    </div>
    <div class="cssClassHeader">
        <h2>
            <span class="cssClassLabel sfLocale">Ordered Items: </span>
        </h2>
    </div>
    <div class="sfGridwrapper clearfix">
        <table class="sfGridWrapperTable" cellspacing="0" cellpadding="0" border="0" width="100%">
            <thead>
                <tr class="cssClassHeading">
                    <td class=" sfLocale">
                        <strong>Item Name </strong>
                    </td>
                    <td class=" sfLocale">
                        <strong>SKU </strong>
                    </td>
                    <td class=" sfLocale">
                        <strong>Shipping Address </strong>
                    </td>
                    <td class=" sfLocale">
                        <strong>Shipping Rate </strong>
                    </td>
                    <td class=" sfLocale">
                        <strong>Price </strong>
                    </td>
                    <td class=" cssClassQtyTbl sfLocale">
                        <strong>Quantity </strong>
                    </td>
                    <td class=" cssClassSubTotalTbl sfLocale">
                        <strong>Sub Total </strong>
                    </td>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
        <div class="cssClassTMar20 cssClassBMar20 cssClassLMar20">
            <a href="#" id="lnkBack" class="cssClassBack sfLocale cssClassDarkBtn">Go back</a></div>
    </div>
</div>
<%--<div>
    &nbsp;&nbsp</div>--%>
<%--<div class="cssClassMyAccountInformation">
    <div class="cssClassHeading">
        <h2>
            <span class="sfLocale">Account Information</span></h2>
        <div class="cssClassClear">
        </div>
    </div>
    <div>
        <h3 class="sfLocale">
            Contact Information
        </h3>
        <p>
            <span id="spanCustomerName"></span>
            <br />
            <span id="spanCustomerEmail"></span>
        </p>
        <div class="cssClassClear">
        </div>
    </div>
</div>--%>
<div class="cssClassMyAddressInformation">
    <div class="cssClassHeader">
        <h2>
            <span class="sfLocale">Address Book</span>
        </h2>
    </div>
    <div class="cssClassCommonWrapper">
        <div class="cssClassCol1">
            <div class="cssClassAddressBook sfCol_48">
                <div class="cssClassShippingAdd">
                    <asp:Literal ID="ltrShipAddress" runat="server" EnableViewState="false"></asp:Literal>
                    <%--<li id="liDefaultShippingAddress"></li>--%>
                </div>
            </div>
        </div>
        <div class="cssClassCol2">
            <div class="cssClassAddressBook sfCol_48">
                <div class="cssClassBillAdd">
                    <asp:Literal ID="ltrBillingAddress" runat="server" EnableViewState="false"></asp:Literal>
                    <%--<li id="liDefaultBillingAddress"></li>--%>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="popupbox" id="popuprel">
    <div class="cssPopUpBody">
        <div class="cssClassCloseIcon">
            <button type="button" class="cssClassClose">
                <span class="sfLocale"><i class="i-close"></i>Close</span></button>
        </div>
        <h2>
            <span id="lblAddressTitle" class="sfLocale">Address Details</span>
        </h2>
        <div class="sfFormwrapper cssClassTMar10">
            <div id="tblNewAddress">
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblFirstName" runat="server" Text="First Name:" CssClass="cssClassLabel"
                                meta:resourcekey="lblFirstNameResource1"></asp:Label><span class="cssClassRequired">*</span></div>
                        <input type="text" id="txtFirstName" name="FirstName" class="required" minlength="2"
                            maxlength="40" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblLastName" runat="server" Text="Last Name:" CssClass="cssClassLabel"
                                meta:resourcekey="lblLastNameResource1"></asp:Label><span class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtLastName" name="LastName" class="required" minlength="2"
                            maxlength="40" />
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblEmail" runat="server" Text="Email:" CssClass="cssClassLabel" meta:resourcekey="lblEmailResource1"></asp:Label><span
                                class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtEmailAddress" name="Email" class="required email" minlength="2" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblCompany" Text="Company:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblCompanyResource1"></asp:Label>
                        </div>
                        <input type="text" id="txtCompanyName" name="Company" maxlength="40" />
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblAddress1" Text="Address 1:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblAddress1Resource1"></asp:Label><span class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtAddress1" name="Address1" class="required" minlength="2"
                            maxlength="250" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblAddress2" Text="Address 2:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblAddress2Resource1"></asp:Label>
                        </div>
                        <input type="text" id="txtAddress2" name="Address2" maxlength="250" />
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblCountry" Text="Country:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblCountryResource1"></asp:Label><span class="cssClassRequired">*</span>
                        </div>
                        <asp:Literal ID="ltrCountry" runat="server" EnableViewState="false"></asp:Literal>
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblState" Text="State/Province:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblStateResource1"></asp:Label><span class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtState" name="State" class="required" minlength="2" maxlength="250" />
                        <select id="ddlUSState" class="sfListmenu">
                        </select>
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblZip" Text="Zip/Postal Code:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblZipResource1"></asp:Label><span class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtZip" name="Zip" class="required alpha_dash" minlength="4"
                            maxlength="10" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblCity" Text="City:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblCityResource1"></asp:Label><span
                                class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtCity" name="City" class="required" minlength="2" maxlength="250" />
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblPhone" Text="Phone:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblPhoneResource1"></asp:Label><span
                                class="cssClassRequired">*</span>
                        </div>
                        <input type="text" id="txtPhone" name="Phone" class="required number" minlength="7"
                            maxlength="20" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblMobile" Text="Mobile:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblMobileResource1"></asp:Label>
                        </div>
                        <input type="text" id="txtMobile" name="Mobile" class="number" minlength="10" maxlength="20" />
                    </li>
                </ul>
                <ul class="clearfix">
                    <li class="cssTextLi">
                        <div>
                            <asp:Label ID="lblFax" Text="Fax:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblFaxResource1"></asp:Label>
                        </div>
                        <input type="text" id="txtFax" name="Fax" class="number" maxlength="20" minlength="7" />
                    </li>
                    <li class="cssTextLi NoRmargin">
                        <div>
                            <asp:Label ID="lblWebsite" Text="Website:" runat="server" CssClass="cssClassLabel"
                                meta:resourcekey="lblWebsiteResource1"></asp:Label>
                        </div>
                        <input type="text" id="txtWebsite" name="Wedsite" class="url" maxlength="50" />
                    </li>
                </ul>
                <ul id="trShippingAddress" class="clearfix">
                    <li class="cssBillingChk">
                        <input type="checkbox" id="chkShippingAddress" class="sfLocale" />
                        <asp:Label ID="lblDefaultShipping" Text=" Use as Default Shipping Address" runat="server"
                            CssClass="cssClassLabel" meta:resourcekey="lblDefaultShippingResource1"></asp:Label>
                    </li>
                </ul>
                <ul id="trBillingAddress" class="clearfix">
                    <li class="cssBillingChk">
                        <input type="checkbox" id="chkBillingAddress" class="sfLocale" />
                        <asp:Label ID="lblDefaultBilling" Text="Use as Default Billing Address" runat="server"
                            CssClass="cssClassLabel" meta:resourcekey="lblDefaultBillingResource1"></asp:Label>
                    </li>
                </ul>
            </div>
            <div class="sfButtonwrapper">
                <label class="icon-save cssClassGreenBtn">
                    <button type="button" id="btnSubmitAddress" class="cssClassButtonSubmit">
                        <span class="sfLocale">Save</span></button></label>
            </div>
        </div>
    </div>
</div>
<div id="divLoadUserControl" class="cssClasMyAccountInformation">
    <div class="cssClassMyDashBoardInformation">
    </div>
</div>
<input type="hidden" id="hdnAddressID" />
<input type="hidden" id="hdnDefaultShippingExist" />
<input type="hidden" id="hdnDefaultBillingExist" />
