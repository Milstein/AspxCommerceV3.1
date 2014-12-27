<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserDashBoard.ascx.cs"
    Inherits="Modules_AspxUserDashBoard_UserDashBoard" %>

<script type="text/javascript" id="tst">
    //<![CDATA[
    $(document).ready(function () {
        $(".sfLocale").localize({
            moduleKey: AspxUserDashBoard
        });
    });
    var currencyCode = '<%=CurrencyCodeSlected %>';
    var isRewardPointEnbled = false;
    var aspxCommonObj = {};
    var userEmail, userFirstName, userLastName,
     allowMultipleAddress, downloadIP, aspxfilePath, userIP, ReviewID, status, ratingValues, countryName, count, isAll;
    aspxCommonObj = {
        StoreID: "<%=storeID %>",
              PortalID: "<%= portalID%>",
              UserName: "<%=userName %>",
              CultureName: "<%= cultureName%>",
              CustomerID: "<%=customerID %>",
              SessionCode: "<%=sessionCode %>"
          };
          var moduleRootPath = "<%=ModulePath %>"; userEmail = '<%=userEmail %>';
    userFirstName = '<%=userFirstName%>';
    userLastName = '<%=userLastName%>';
    userPicture = '<%=userPicture %>';
    allowMultipleAddress = '<%=allowMultipleAddress %>';
    downloadIP = "<%=userIP %>"; aspxfilePath = '<%=aspxfilePath %>';
    userIP = "<%=userIP %>"; ReviewID = '';
    status = '';
    ratingValues = '';
    countryName = '<%=countryName %>';
    userModuleIDUD = '<%=userModuleIDUD %>';
        count = 1;
        isAll = 1;

        $(function () {
            var AspxCommonObj = function () {
                var aspxCommonObj = {
                    StoreID: AspxCommerce.utils.GetStoreID(),
                    PortalID: AspxCommerce.utils.GetPortalID(),
                    CultureName: AspxCommerce.utils.GetCultureName(),
                    UserName: AspxCommerce.utils.GetUserName(),
                    CustomerID: AspxCommerce.utils.GetCustomerID(),
                    SessionCode: AspxCommerce.utils.GetSessionCode()
                };
                return aspxCommonObj;
            };

            var UserDashBoard = {
                LoadControl: function (controlName) {
                    $.ajax({
                        type: "POST",beforeSend: function (request) {
                            request.setRequestHeader('ASPX-TOKEN', _aspx_token);
                            request.setRequestHeader("UMID", userModuleIDUD);
                            request.setRequestHeader("UName", AspxCommerce.utils.GetUserName());
                            request.setRequestHeader("PID", AspxCommerce.utils.GetPortalID());
                            request.setRequestHeader("PType", "v");
                            request.setRequestHeader('Escape', '0');
                        },
                        url: AspxCommerce.utils.GetAspxServicePath() + "LoadControlHandler.aspx/Result",
                        data: "{ controlName:'" + AspxCommerce.utils.GetAspxRootPath() + controlName + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            $('#divLoadUserControl').html(response.d);
                        },
                        error: function () {
                            csscody.error('<h2>' + getLocale(AspxUserDashBoard, 'Error Message') + '</h2><p>' + getLocale(AspxUserDashBoard, 'Failed to load control!.') + '</p>');
                        }
                    });
                },
                GetGeneralSettings: function () {
                    var ModuleServicePath = aspxRootPath + "Modules/AspxCommerce/AspxRewardPoints/RewardPointsHandler.ashx/";
                    $.ajax({
                        type: "POST", beforeSend: function (request) {
                            request.setRequestHeader('ASPX-TOKEN', _aspx_token);
                            request.setRequestHeader("UMID", userModuleIDUD);
                            request.setRequestHeader("UName", AspxCommerce.utils.GetUserName());
                            request.setRequestHeader("PID", AspxCommerce.utils.GetPortalID());
                            request.setRequestHeader("PType", "v");
                            request.setRequestHeader('Escape', '0');
                        },
                        url: ModuleServicePath + "RewardPointsGeneralSettingsIsActive",
                        data: JSON2.stringify({ aspxCommonObj: AspxCommonObj() }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,
                        success: function (response) {
                            if (response.d) {
                                $('#MyRewardPoints').show();
                            }
                            else {
                                $('#MyRewardPoints').hide();
                            }
                        },
                        error: function () {
                            csscody.error("<h2>" + getLocale(AspxUserDashBoard, "Error Message") + "</h2><p>" + getLocale(AspxUserDashBoard, "Failed to load reward points setting values!") + "</p>");
                        }
                    });
                },
                Init: function () {
                    if (AspxCommonObj().CustomerID > 0 && userName.toLowerCase() != 'anonymoususer') {
                        if (!isRewardPointEnbled) {
                            if (AspxCommerce.IsModuleInstalled('AspxRewardPoints')) {
                                isRewardPointEnbled = true;
                                UserDashBoard.GetGeneralSettings();
                            }
                            else {
                                $('#MyRewardPoints').hide();
                            }
                        }
                        if (userPicture != '' && userPicture != null) {
                            var userImagePath = '/Modules/Admin/UserManagement/UserPic/' + userPicture + '';
                        }
                        else {
                            userPicture = "default-profile-pic.png";                            
                            var userImagePath = '/Modules/Admin/UserManagement/UserPic/' + userPicture + '';
                        }
                        UserDashBoard.LoadControl("Modules/AspxCommerce/AspxUserDashBoard/AccountDashboard.ascx");
                        $("ul.cssClassMyDashBoard li a").bind("click", function () {
                            $("ul.cssClassMyDashBoard li a").removeClass("cssClassmyAccountActive");
                            $(this).addClass("cssClassmyAccountActive");
                            var linkId = $(this).attr("name");
                            var ControlName = '';
                            switch (linkId) {

                                case 'AccountDashBoard':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/AccountDashboard.ascx";
                                    break;
                                case 'AddressBook':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/AddressBook.ascx";
                                    break;
                                case 'MyOrders':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/MyOrders.ascx";
                                    break;
                                case 'MyReturns':
                                    ControlName = "Modules/AspxCommerce/AspxReturnAndPolicy/ReturnsHistory.ascx";
                                    break;
                                case 'MyRewardPoints':
                                    ControlName = "Modules/AspxCommerce/AspxRewardPoints/MyRewardPoints.ascx";
                                    break;
                                case 'MyItemReviews':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/UserItemReviews.ascx";
                                    break;
                                case 'MyTags':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/MyTags.ascx";
                                    break;
                                case 'MyWishList':
                                    ControlName = "Modules/AspxCommerce/AspxWishList/WishItemList.ascx";
                                    break;
                                case 'SharedWishList':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashboard/ShareWishListItems.ascx";
                                    break;
                                case 'MyDownloadableItems':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/UserDownloadableProducts.ascx";
                                    break;
                                case 'ReferredFriends':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashBoard/ReferredFriends.ascx";
                                    break;
                                case 'RecentHistory':
                                    ControlName = "Modules/AspxCommerce/AspxUserDashboard/UserRecentHistory.ascx";
                                    break;
                                case 'GiftCard':
                                    ControlName = "Modules/AspxCommerce/AspxGiftCardManagement/BalanceInquiry.ascx";
                                    break;
                            }
                            UserDashBoard.LoadControl(ControlName);
                        });
                    }
                    else {
                        var loginPage = '';
                        if (AspxCommerce.utils.IsUserFriendlyUrl()) {
                            loginPage = LogInURL + pageExtension;
                        }
                        else {
                            loginPage = LogInURL;
                        }
                        window.location = AspxCommerce.utils.GetAspxRedirectPath() + loginPage;
                    }
                }
            };
            UserDashBoard.Init();
        });
        //]]>
</script>

<div class="cssClassMyDashBoard">
    <div class="cssClassCommonSideBox">
        <h2>
            <span class="sfLocale">My Account</span>
        </h2>
        <div class="cssClassMyAccountInformation clearfix">
            <asp:Literal ID="ltrUserDetails" runat="server"></asp:Literal>
        </div>
        <div class="cssClasMyAccount">
            <ul class="cssClassMyDashBoard">
                <li><a href="#" name="AccountDashBoard" class="cssClassmyAccountActive sfLocale">Account Dashboard</a></li>
                <li><a href="#" name="AddressBook" class="sfLocale">Address Book</a></li>
                <li><a href="#" name="MyOrders" class="sfLocale">My Orders</a></li>
                <li><a href="#" name="MyReturns" class="sfLocale">My Returns</a></li>
                <li><a href="#" name="MyRewardPoints" id="MyRewardPoints" class="sfLocale">My Reward Points</a></li>
                <%--<li><a href="#" name="BillingAgreements">Billing Agreements</a></li>
                <li><a href="#" name="RecurringProfiles">Recurring Profiles</a></li>--%>
                <li><a href="#" name="MyItemReviews" class="sfLocale">My Item Reviews</a></li>
                <li><a href="#" name="MyTags" class="sfLocale">My Tags</a></li>
                <li><a href="#" name="MyWishList" class="sfLocale">My Wishlist</a></li>
                <li><a href="#" name="SharedWishList" class="sfLocale">Shared Wishlist</a></li>
                <li><a href="#" name="MyDownloadableItems" class="sfLocale">My Digital Items</a></li>
                <%--<li><a href="#" name="NewsLetterSubscriptions">Newsletter Subscriptions</a></li>--%>
                <li><a href="#" name="ReferredFriends" class="sfLocale">Referred Friends</a></li>
                <%--<li><a href="#" name="StoreCredit">Store Credit</a></li>
                <li><a href="#" name="GiftCard">Gift Card</a></li>
                <li><a href="#" name="RewardPoints">Reward Points</a></li>--%>
                <li><a href="#" name="RecentHistory" class="sfLocale">Recent History</a></li>
                <li><a href="#" name="GiftCard" class="sfLocale">Gift Card</a></li>
            </ul>
        </div>
    </div>
    <div id="divLoadUserControl" class="cssClasMyAccountInformation">

        <div class="cssClassMyDashBoardInformation">
            <%-- <p>
            Hello,<span id="spanName"></span> From your My Account Dashboard you
            have the ability to view a snapshot of your recent account activity and update your
            account information. Select a link below to view or edit information.
        </p>--%>
        </div>
    </div>
    <div class="cssClassclear">
    </div>

</div>
