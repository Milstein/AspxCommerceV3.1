﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ItemDetailTab.ascx.cs" Inherits="Modules_AspxCommerce_AspxItemDetailTab_ItemDetailTab" %>

<script type="text/javascript">
    //<![CDATA[   
    $(function() {
        $(".sfLocale").localize({
            moduleKey: AspxItemDetailTab
        });
    });
    //]]>
</script>

<div class="cssClassProductDetailInformation">
    <asp:Literal ID="ltrItemDetailsForm" runat="server" EnableViewState="false"></asp:Literal>
</div>