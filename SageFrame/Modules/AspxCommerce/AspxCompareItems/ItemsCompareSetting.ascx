<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ItemsCompareSetting.ascx.cs" Inherits="Modules_AspxCommerce_AspxItemsCompare_ItemsCompareSetting" %>

<script type="text/javascript">
    (function ($) {
        $.CompareItemsSettings = function (param) {
            param = $.extend({
                compareItemsSettings: {}
            }, param);

            var p = $.parseJSON(param.compareItemsSettings);
            var AspxCommonObj = function () {
                var aspxCommonObj = {
                    StoreID: AspxCommerce.utils.GetStoreID(),
                    PortalID: AspxCommerce.utils.GetPortalID(),
                    CultureName: AspxCommerce.utils.GetCultureName()
                };
                return aspxCommonObj;
            };
            compareItemsSettings = {
                config: {
                    isPostBack: false,
                    async: false,
                    cache: false,
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    data: '{}',
                    dataType: 'json',
                    baseURL: p.CompareItemsModulePath + "Handler.ashx/",
                    method: "",
                    url: "",
                    ajaxCallMode: ""
                },
                ajaxCall: function (config) {
                    $.ajax({
                        type: compareItemsSettings.config.type,
                        contentType: compareItemsSettings.config.contentType,
                        cache: compareItemsSettings.config.cache,
                        async: compareItemsSettings.config.async,
                        url: compareItemsSettings.config.url,
                        data: compareItemsSettings.config.data,
                        dataType: compareItemsSettings.config.dataType,
                        success: compareItemsSettings.config.ajaxCallMode,
                        error: compareItemsSettings.config.ajaxFailure
                    });
                },
                GetCompareItemsSetting: function () {
                    var param = JSON2.stringify({ aspxCommonObj: AspxCommonObj() });
                    this.config.method = "GetCompareItemsSetting";
                    this.config.url = this.config.baseURL + this.config.method;
                    this.config.data = param;
                    this.config.ajaxCallMode = compareItemsSettings.BindCompareItemsSetting;
                    this.ajaxCall(this.config);
                },
                BindCompareItemsSetting: function () {
                    $("#chkEnableCompareItem").prop("checked", p.IsEnableCompareItem)
                    $("#txtCompareItemCount").val(p.CompareItemCount);
                    $("#txtCompareItemDetailPage").val(p.CompareDetailsPage);
                },

                CompareItemsSettingUpdate: function () {
                    var isEnableCompareItem = $("#chkEnableCompareItem").prop("checked");
                    var compareItemCount = $("#txtCompareItemCount").val();
                    var compareItemDetailsPage = $("#txtCompareItemDetailPage").val();
                    var settingKeys = "IsEnableCompareItem*CompareItemCount*CompareItemDetailsPage";
                    var settingValues = isEnableCompareItem + "*" + compareItemCount + "*" + compareItemDetailsPage;
                    var compareItemsSettingListObj = {
                        SettingKey: settingKeys,
                        SettingValue: settingValues
                    };
                    var param = JSON2.stringify({ aspxCommonObj: AspxCommonObj(), compareItems: compareItemsSettingListObj });
                    this.config.method = "SaveAndUpdateCompareItemsSetting";
                    this.config.url = this.config.baseURL + this.config.method;
                    this.config.data = param;
                    this.config.ajaxCallMode = compareItemsSettings.CompareItemSettingSuccess;
                    this.ajaxCall(this.config);
                },
                CompareItemSettingSuccess: function (data) {
                    SageFrame.messaging.show(getLocale(AspxItemsCompare, "Setting Saved Successfully"), "Success");
                },
                init: function () {
                    compareItemsSettings.BindCompareItemsSetting();
                    $("#btnCompareItemSettingSave").click(function () {
                        compareItemsSettings.CompareItemsSettingUpdate();
                    });
                }
            };
            compareItemsSettings.init();
        };
        $.fn.compareSetting = function (p) {
            $.CompareItemsSettings(p);
        };
    })(jQuery);
    $(function () {
        $(".sfLocale").localize({
            moduleKey: AspxItemsCompare
        });
        $(this).compareSetting({
            compareItemsSettings: '<%=compareItemsSettings %>'
        });
    });
</script>
<div class="cssCompareItemSetting sfFormwrapper">
    <h3 class="sfLocale">Compare Items Settings</h3>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblEnableCompareItem" runat="server" Text="Enable CompareItem"></asp:Label>
            </td>
            <td>
                <input type="checkbox" id="chkEnableCompareItem" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCompareItemCount" runat="server" Text="Enter the Number of Products Displayed"></asp:Label>
            </td>
            <td>
                <input type="text" id="txtCompareItemCount" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCompareItemDeatailPage" runat="server" Text="Compare Item Detail Page:"></asp:Label>
            </td>
            <td>
                <input type="text" id="txtCompareItemDetailPage" disabled="disabled" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" id="btnCompareItemSettingSave" class="sfLocale sfbtn" value="Save" />
            </td>
        </tr>
    </table>
</div>
