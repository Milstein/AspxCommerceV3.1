<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleCheckOut.ascx.cs" Inherits="Modules_AspxGoogleCheckOut_GoogleCheckOut" %>

<script type="text/javascript">

    //<![CDATA[

    $(function () {

        $(".sfLocale").localize({
            moduleKey: GoogleCheckOut
        });
        var aspxCommonObj = function () {
            var aspxCommonInfo = {
                StoreID: AspxCommerce.utils.GetStoreID(),
                PortalID: AspxCommerce.utils.GetPortalID(),
                CustomerID: AspxCommerce.utils.GetCustomerID(),
                SessionCode: AspxCommerce.utils.GetSessionCode()
            };
            return aspxCommonInfo;
        };
        var $securepost = function (method, param, successFx, error) {
            $.ajax({
                type: "POST",
                //contentType: "application/json; charset=utf-8",
                async: false,
                url: aspxservicePath + 'securepost.ashx?call=' + method,
                data: param,
                // dataType: "json",
                success: successFx,
                error: error
            });
        };

        var _checkoutVars;

        var tempCheckout = CheckOutApi.Get();
        var api = tempCheckout;
        var AspxOrder = {
            config: {
                isPostBack: false,
                async: false,
                cache: false,
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                data: '{}',
                dataType: 'json',
                baseURL: AspxCommerce.utils.GetAspxServicePath(),
                method: "",
                url: "",
                ajaxCallMode: 0, ///0 for get categories and bind, 1 for notification,2 for versions bind
                checkCartExist: false,
                sessionValue: "",
                error: 0
            },
            ajaxFailure: function () {
                switch (AspxOrder.config.error) {
                    case 3:
                        break;
                    case 4:
                        break;
                }
            },

            ajaxCall: function (config) {
                $.ajax({
                    type: AspxOrder.config.type,
                    contentType: AspxOrder.config.contentType,
                    cache: AspxOrder.config.cache,
                    async: AspxOrder.config.async,
                    url: AspxOrder.config.url,
                    data: AspxOrder.config.data,
                    dataType: AspxOrder.config.dataType,
                    success: AspxOrder.ajaxSuccess,
                    error: AspxOrder.ajaxFailure
                });
            },
            CheckCustomerCartExist: function () {
                var isExist = false;
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    async: false,
                    // url: aspxservicePath + 'AspxCommerceWebService.asmx/' + method,
                    url: AspxCommerce.utils.GetAspxServicePath() + 'AspxCoreHandler.ashx/CheckCustomerCartExist',
                    data: JSON2.stringify({ aspxCommonObj: aspxCommonObj() }),
                    dataType: "json",
                    success: function (data) {

                        AspxOrder.config.checkCartExist = data.d;
                        isExist = data.d.CartStatus;
                    },
                    error: null
                });
                return isExist;
            },
            SetSessionValue: function (sessionKey, sessionValue) {
                this.config.method = "AspxCoreHandler.ashx/SetSessionVariable";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ key: sessionKey, value: sessionValue }),
                this.config.ajaxCallMode = 5;
                this.ajaxCall(this.config);

            },
            init: function () {
                $('#btnGoogleCheckOut').on("click", function () {
                    if (AspxCommerce.IsModuleInstalled('AspxKPI')) {
                        //KPI Saves conversion for payment
                        KPICommon.KPISaveConversion('Order Review');
                        //KPI Saves visit for SalesConfirmation  
                        KPICommon.KPISaveVisit('Proceed to Payment');
                    }

                    $securepost("GCVs", "", function (data) {
                        _checkoutVars = $.parseJSON(data.d);
                    }, null);
                    if (api.UserCart.IsGiftCardUsed) {
                        if (!CheckOutApi.CalcAPI.GiftCard.CheckGiftCardIsUsed()) {
                            CheckOutApi.CalcAPI.GiftCard.ResetGiftCard();
                            SageFrame.messaging.show("Applied Gift Card has insufficient balance.Please veriry once again!", "Alert");
                            return false;
                        }
                    }
                    // if (AspxCommerce.utils.GetCustomerID() != 0 && AspxCommerce.utils.GetUserName() != 'anonymoususer') {
                    var checkIfCartExist = AspxOrder.CheckCustomerCartExist();
                    if (!checkIfCartExist) {
                        csscody.alert("<h2>" + getLocale(GoogleCheckOut, 'Information Alert') + '</h2><p>' + getLocale(GoogleCheckOut, 'Your cart has been emptied already!!') + "</p>");
                        return false;
                    }
                    //  }
                    // api.UserCart.TotalDiscount = eval(parseFloat(api.UserCart.TotalDiscount) + parseFloat(api.UserCart.CartDiscount));
                    // AspxOrder.SetSessionValue("DiscountAll", api.UserCart.TotalDiscount);
                    if ($('#SingleCheckOut').length > 0) {
                        AspxOrder.AddOrderDetails();
                    }
                    else {
                        AspxOrder.SendDataForPaymentGoogleCheckOut();
                    }
                });
            },
            ajaxSuccess: function (data) {
                switch (AspxOrder.config.ajaxCallMode) {
                    case 0:
                        break;
                    case 1:
                        AspxOrder.config.checkCartExist = data.d;
                        break;
                    case 2: //get session variable                       
                        AspxOrder.config.sessionValue = parseFloat(data.d);
                        break;
                    case 3:
                        var x = AspxCommerce.utils.GetStoreID() + "#" + AspxCommerce.utils.GetPortalID() + "#" + AspxCommerce.utils.GetUserName() + "#" + AspxCommerce.utils.GetCustomerID() + "#" + AspxCommerce.utils.GetSessionCode() + "#" + AspxCommerce.utils.GetCultureName();
                        var itemIds = "";
                        for (var i = 0; i < api.UserCart.lstItems.length; i++) {
                            itemIds += api.UserCart.lstItems[i].ItemID + "&";
                        }
                        x = x + "#" + itemIds + "#" + api.Coupon.Get();
                        $securepost("SS", { k: 'GoogleCheckOutData', v: x }, function () {
                            document.location = '<%=PathGoogleCheckOut%>' + "PayThroughGoogleCheckOut.aspx";
                        }, null);
                        break;
                    case 4:
                        var x = AspxCommerce.utils.GetStoreID() + "#" + AspxCommerce.utils.GetPortalID() + "#" + AspxCommerce.utils.GetUserName() + "#" + AspxCommerce.utils.GetCustomerID() + "#" + AspxCommerce.utils.GetSessionCode() + "#" + AspxCommerce.utils.GetCultureName();
                        var itemIds = "";
                        for (var i = 0; i < api.UserCart.lstItems.length; i++) {
                            itemIds += api.UserCart.lstItems[i].ItemID + "&" + api.UserCart.lstItems[i].Quantity + ',';
                        }
                        x = x + "#" + itemIds + "#" + api.Coupon.Get();
                        $securepost("SS", { k: 'GoogleCheckOutData', v: x }, function () {
                            document.location = '<%=PathGoogleCheckOut%>' + "PayThroughGoogleCheckOut.aspx";
                        }, null);
                        break;
                }
            },

            getSession: function (Key) {
                this.config.method = "AspxCoreHandler.ashx/GetSessionVariableCart",
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ key: Key });
                this.config.ajaxCallMode = 2;
                this.ajaxCall(this.config);
                return AspxOrder.config.sessionValue;
            },
            SendDataForPaymentGoogleCheckOut: function () {
                //credit card info
                var creditCardTransactionType = $('#ddlTransactionType option:selected').text();
                var cardNo = $('#txtCardNo').val();
                var cardCode = $('#txtCardCode').val();
                var CardType = $('#cardType option:selected').text();
                var expireDate;
                expireDate = $('#lstMonth option:selected').text();
                expireDate += $('#lstYear option:selected').text();

                //Cheque Number
                var accountNumber = $('#txtAccountNumber').val();
                var routingNumber = $('#txtRoutingNumber').val();
                var accountType = $('#ddlAccountType option:selected').text();
                var bankName = $('#txtBankName').val();
                var accountHoldername = $('#txtAccountHolderName').val();
                var checkType = $('#ddlChequeType option:selected').text();
                var checkNumber = $('#txtChequeNumber').val();
                var recurringBillingStatus = false;

                if ($('#chkRecurringBillingStatus').is(':checked'))
                    recurringBillingStatus = true;
                else
                    recurringBillingStatus = false;

                var paymentMethodName = "GoogleCheckOut";
                var paymentMethodCode = "GoogleCheckOut";
                var isBillingAsShipping = false;

                if ($('#chkBillingAsShipping').attr('checked'))
                    api.BillingAddress.IsBillingAsShipping = true;
                else
                    api.BillingAddress.IsBillingAsShipping = false;

                var orderRemarks = Encoder.htmlEncode($("#txtAdditionalNote").val());
                var currencyCode = '<%=MainCurrency %>';
                var isTestRequest = "TRUE";
                var isEmailCustomer = "TRUE";
                var taxTotal = _checkoutVars.TaxAll;// AspxOrder.getSession("TaxAll"); 
                var paymentGatewayID = _checkoutVars.Gateway;// AspxOrder.getSession("Gateway"); 
                // shippingRate = getSession("ShippingCostAll");
                var amount = _checkoutVars.GrandTotal;
                var paymentGatewaySubTypeID = 1;

                var OrderDetails = {
                    BillingAddressInfo: api.BillingAddress,
                    PaymentInfo: {
                        PaymentMethodName: paymentMethodName,
                        PaymentMethodCode: paymentMethodCode,
                        CardNumber: cardNo,
                        TransactionType: creditCardTransactionType,
                        CardType: CardType,
                        CardCode: cardCode,
                        ExpireDate: expireDate,
                        AccountNumber: accountNumber,
                        RoutingNumber: routingNumber,
                        AccountType: accountType,
                        BankName: bankName,
                        AccountHolderName: accountHoldername,
                        ChequeType: checkType,
                        ChequeNumber: checkNumber,
                        RecurringBillingStatus: recurringBillingStatus
                    },

                    OrderDetailsInfo: {
                        SessionCode: '',
                        IsGuestUser: false,
                        InvoiceNumber: "",
                        OrderStatus: "",
                        TransactionID: 0,
                        GrandTotal: amount,
                        DiscountAmount: api.UserCart.TotalDiscount,
                        CouponDiscountAmount: api.UserCart.CouponDiscountAmount,
                        Coupons: [],
                        UsedRewardPoints: api.UserCart.UsedRewardPoints,
                        RewardDiscountAmount: api.UserCart.RewardPointsDiscount,
                        PurchaseOrderNumber: 0,
                        PaymentGatewayTypeID: paymentGatewayID,
                        PaymentGateSubTypeID: paymentGatewaySubTypeID,
                        ClientIPAddress: clientIPAddress,
                        UserBillingAddressID: $('.cssClassBillingAddressInfo span').attr('id'),
                        ShippingMethodID: api.UserCart.spMethodID,
                        PaymentMethodID: 0,
                        TaxTotal: taxTotal,
                        CurrencyCode: currencyCode,
                        CustomerID: AspxCommerce.utils.GetCustomerID(),
                        ResponseCode: 0,
                        ResponseReasonCode: 0,
                        ResponseReasonText: "",
                        Remarks: orderRemarks,
                        IsMultipleCheckOut: true,
                        IsTest: isTestRequest,
                        IsEmailCustomer: isEmailCustomer,
                        IsDownloadable: api.UserCart.IsDownloadItemInCart,
                        IsShippingAddressRequired: api.UserCart.NoShippingAddress
                    },
                    CommonInfo: {
                        PortalID: AspxCommerce.utils.GetPortalID(),
                        StoreID: AspxCommerce.utils.GetStoreID(),
                        CultureName: AspxCommerce.utils.GetCultureName(),
                        AddedBy: AspxCommerce.utils.GetUserName(),
                        IsActive: api.UserCart.isActive
                    }
                };

                var paramData = {
                    OrderDetailsCollection: {
                        ObjOrderDetails: OrderDetails.OrderDetailsInfo,
                        LstOrderItemsInfo: api.UserCart.lstItems,
                        ObjPaymentInfo: OrderDetails.PaymentInfo,
                        GiftCardDetail: api.UserCart.GiftCardDetail,
                        ObjBillingAddressInfo: OrderDetails.BillingAddressInfo,
                        ObjCommonInfo: OrderDetails.CommonInfo,
                        ObjOrderTaxInfo: api.UserCart.objTaxList
                    }
                };
                this.config.method = "AspxCoreHandler.ashx/SaveOrderDetails";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ "orderDetail": paramData.OrderDetailsCollection });
                this.config.ajaxCallMode = 3;
                this.config.error = 3;
                this.ajaxCall(this.config);
            },

            AddOrderDetails: function () {
                if ($('#txtFirstName').val() == '') {
                    var billingAddress = $('#dvBillingSelect input:radio:checked').parent('label').text();
                    var addr = billingAddress.split(',');
                    var Name = addr[0].split(' ');
                    Array.prototype.clean = function (deleteValue) {
                        for (var i = 0; i < this.length; i++) {
                            if (this[i] == deleteValue) {
                                this.splice(i, 1);
                                i--;
                            }
                        }
                        return this;
                    };
                    Name.clean("");

                    if ($('#dvBillingSelect input:radio:checked').val() > 0)
                        api.BillingAddress.AddressID = $('#dvBillingSelect input:radio:checked').val();

                    api.BillingAddress.FirstName = Name[0];
                    api.BillingAddress.LastName = Name[1];
                    api.BillingAddress.CompanyName = addr[8];
                    api.BillingAddress.EmailAddress = addr[6];
                    api.BillingAddress.Address = addr[1];
                    api.BillingAddress.Address2 = addr[11];
                    api.BillingAddress.City = addr[2];
                    api.BillingAddress.State = addr[3];
                    api.BillingAddress.Zip = addr[5];
                    api.BillingAddress.Country = addr[4];
                    api.BillingAddress.Phone = addr[7];
                    api.BillingAddress.Mobile = addr[8];
                    api.BillingAddress.Fax = addr[9];
                    api.BillingAddress.WebSite = addr[10];
                }
                else {
                    api.BillingAddress.FirstName = $('#txtFirstName').val();
                    api.BillingAddress.LastName = $('#txtLastName').val();
                    api.BillingAddress.CompanyName = $('#txtCompanyName').val();
                    api.BillingAddress.EmailAddress = $('#txtEmailAddress').val();
                    api.BillingAddress.Address = $('#txtAddress1').val();
                    api.BillingAddress.Address2 = $('#txtAddress2').val();
                    api.BillingAddress.City = $('#txtCity').val();
                    api.BillingAddress.Country = $('#ddlBLCountry option:selected').text();
                    if ($.trim(api.BillingAddress.Country) == 'United States')
                        api.BillingAddress.State = $('#ddlBLState option:selected').text();
                    else
                        api.BillingAddress.State = $('#txtState').val();
                    api.BillingAddress.Zip = $('#txtZip').val();
                    api.BillingAddress.Phone = $('#txtPhone').val();
                    api.BillingAddress.Mobile = $('#txtMobile').val();
                    api.BillingAddress.Fax = $('#txtFax').val();
                    api.BillingAddress.Website = $('#txtWebsite').val();
                    api.BillingAddress.IsDefaultBilling = false;

                }

                if ($('#txtSPFirstName').val() == '') {
                    var address = $('#dvShippingSelect input:radio:checked').parent('label').text();
                    //" test test, Imadol, asdfasf, HH, Andorra, 235234, budiestpunk@gmail.com, 123434343434"
                    var addr = address.split(',');
                    var Name = addr[0].split(' ');
                    Array.prototype.clean = function (deleteValue) {
                        for (var i = 0; i < this.length; i++) {
                            if (this[i] == deleteValue) {
                                this.splice(i, 1);
                                i--;
                            }
                        }
                        return this;
                    };
                    Name.clean("");
                    api.ShippingAddress.AddressID = api.UserCart.spAddressID;
                    api.ShippingAddress.FirstName = Name[0];
                    api.ShippingAddress.LastName = Name[1];
                    api.ShippingAddress.CompanyName = addr[12];
                    api.ShippingAddress.EmailAddress = addr[6];
                    api.ShippingAddress.Address = addr[1];
                    api.ShippingAddress.Address2 = addr[11];
                    api.ShippingAddress.City = addr[2];
                    api.ShippingAddress.State = addr[3];
                    api.ShippingAddress.Zip = addr[5];
                    api.ShippingAddress.Country = addr[4];
                    api.ShippingAddress.Phone = addr[7];
                    api.ShippingAddress.Mobile = addr[8];
                    api.ShippingAddress.Fax = addr[9];
                    api.ShippingAddress.Website = addr[10];
                }
                else {
                    api.ShippingAddress.FirstName = $('#txtSPFirstName').val();
                    api.ShippingAddress.LastName = $('#txtSPLastName').val();
                    api.ShippingAddress.CompanyName = $('#txtSPCompany').val();
                    api.ShippingAddress.Address = $('#txtSPAddress').val();
                    api.ShippingAddress.Address2 = $('#txtSPAddress2').val();
                    api.ShippingAddress.City = $('#txtSPCity').val();
                    api.ShippingAddress.Zip = $('#txtSPZip').val();
                    api.ShippingAddress.Country = $('#ddlSPCountry option:selected').text();
                    if ($.trim(api.ShippingAddress.Country) == 'United States') {
                        api.ShippingAddress.State = $('#ddlSPState').val();
                    } else {
                        api.ShippingAddress.State = $('#txtSPState').val();
                    }
                    api.ShippingAddress.Phone = $('#txtSPPhone').val();
                    api.ShippingAddress.Mobile = $('#txtSPMobile').val();
                    api.ShippingAddress.Fax = '';
                    api.ShippingAddress.Email = $('#txtSPEmailAddress').val();
                    api.ShippingAddress.Website = '';
                    api.ShippingAddress.IsDefaultShipping = false;
                }

                //credit card info
                var creditCardTransactionType = $('#ddlTransactionType option:selected').text();
                var cardNo = $('#txtCardNo').val();
                var cardCode = $('#txtCardCode').val();
                var CardType = $('#cardType option:selected').text();

                var expireDate;
                expireDate = $('#lstMonth option:selected').text();
                expireDate += $('#lstYear option:selected').text();

                //Cheque Number
                var accountNumber = $('#txtAccountNumber').val();
                var routingNumber = $('#txtRoutingNumber').val();
                var accountType = $('#ddlAccountType option:selected').text();
                var bankName = $('#txtBankName').val();
                var accountHoldername = $('#txtAccountHolderName').val();
                var checkType = $('#ddlChequeType option:selected').text();
                var checkNumber = $('#txtChequeNumber').val();
                var recurringBillingStatus = false;
                var paymentMethodName = "GoogleCheckOut";
                var paymentMethodCode = "GoogleCheckOut";

                if ($('#chkRecurringBillingStatus').is(':checked'))
                    recurringBillingStatus = true;
                else
                    recurringBillingStatus = false;

                if ($('#chkBillingAsShipping').is(":checked"))
                    api.BillingAddress.IsBillingAsShipping = true;
                else
                    api.BillingAddress.IsBillingAsShipping = false;


                var orderRemarks = Encoder.htmlEncode($("#txtAdditionalNote").val());
                var orderItemRemarks = "Order Item Remarks";
                var currencyCode = '<%=MainCurrency %>';
                var isTestRequest = "TRUE";
                var isEmailCustomer = "TRUE";
                var paymentGatewaySubTypeID = 1;
                var shippingMethodID = api.UserCart.spMethodID;
                var taxTotal = _checkoutVars.TaxAll;
                var paymentGatewayID = _checkoutVars.Gateway;
                shippingRate = _checkoutVars.ShippingCost;//
                var amount = _checkoutVars.GrandTotal;//
                var OrderDetails = {
                    BillingAddressInfo: api.BillingAddress,
                    objSPAddressInfo: api.ShippingAddress,
                    PaymentInfo: {
                        PaymentMethodName: paymentMethodName,
                        PaymentMethodCode: paymentMethodCode,
                        CardNumber: "", TransactionType: creditCardTransactionType,
                        CardType: "",
                        CardCode: "",
                        ExpireDate: "",
                        AccountNumber: accountNumber,
                        RoutingNumber: routingNumber,
                        AccountType: accountType,
                        BankName: bankName,
                        AccountHolderName: accountHoldername,
                        ChequeType: checkType,
                        ChequeNumber: checkNumber,
                        RecurringBillingStatus: recurringBillingStatus
                    },
                    OrderDetailsInfo: {
                        SessionCode: AspxCommerce.utils.GetSessionCode(),
                        InvoiceNumber: "",
                        OrderStatus: "",
                        TransactionID: 0,
                        GrandTotal: amount,
                        DiscountAmount: api.UserCart.TotalDiscount,
                        CouponDiscountAmount: api.UserCart.CouponDiscountAmount,
                        Coupons: [],
                        UsedRewardPoints: api.UserCart.UsedRewardPoints,
                        RewardDiscountAmount: api.UserCart.RewardPointsDiscount,
                        PurchaseOrderNumber: 0,
                        PaymentGatewayTypeID: paymentGatewayID,
                        PaymentGateSubTypeID: paymentGatewaySubTypeID,
                        ClientIPAddress: AspxCommerce.utils.GetClientIP(),
                        UserBillingAddressID: api.BillingAddress.AddressID,
                        ShippingMethodID: shippingMethodID,
                        IsGuestUser: api.UserCart.isUserGuest,
                        PaymentMethodID: 0,
                        TaxTotal: taxTotal,
                        CurrencyCode: currencyCode,
                        CustomerID: AspxCommerce.utils.GetCustomerID(),
                        ResponseCode: 0,
                        ResponseReasonCode: 0,
                        ResponseReasonText: "",
                        Remarks: orderRemarks,
                        IsMultipleCheckOut: false,
                        IsTest: isTestRequest,
                        IsEmailCustomer: isEmailCustomer,
                        IsDownloadable: api.UserCart.IsDownloadItemInCart,
                        IsShippingAddressRequired: api.UserCart.NoShippingAddress
                    },
                    CommonInfo: {
                        PortalID: AspxCommerce.utils.GetPortalID(),
                        StoreID: AspxCommerce.utils.GetStoreID(),
                        CultureName: AspxCommerce.utils.GetCultureName(),
                        AddedBy: AspxCommerce.utils.GetUserName(),
                        IsActive: api.UserCart.isActive
                    }
                };

                var paramData = {
                    OrderDetailsCollection: {
                        ObjOrderDetails: OrderDetails.OrderDetailsInfo,
                        LstOrderItemsInfo: api.UserCart.lstItems,
                        ObjPaymentInfo: OrderDetails.PaymentInfo,
                        GiftCardDetail: api.UserCart.GiftCardDetail,
                        ObjBillingAddressInfo: OrderDetails.BillingAddressInfo,
                        ObjShippingAddressInfo: OrderDetails.objSPAddressInfo,
                        ObjCommonInfo: OrderDetails.CommonInfo,
                        ObjOrderTaxInfo: api.UserCart.objTaxList
                    }
                };

                this.config.method = "AspxCoreHandler.ashx/SaveOrderDetails";
                this.config.url = this.config.baseURL + this.config.method;
                this.config.data = JSON2.stringify({ "orderDetail": paramData.OrderDetailsCollection });
                this.config.ajaxCallMode = 4;
                this.config.error = 4;
                this.ajaxCall(this.config);
            }


        }
        AspxOrder.init();

    });

    //]]>
</script>

<label class="cssClassGreenBtn">
    <input type="button" id="btnGoogleCheckOut" class="sfLocale" value="Place Order" /></label>

