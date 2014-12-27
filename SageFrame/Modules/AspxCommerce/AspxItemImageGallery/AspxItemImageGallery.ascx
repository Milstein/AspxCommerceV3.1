<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AspxItemImageGallery.ascx.cs" Inherits="Modules_AspxCommerce_AspxItemImageGallery_AspxItemImageGallery" %>

<script type="text/javascript">
    //<![CDATA[
$(function () {
    //$(".sfLocale").localize({
    //    moduleKey: AspxItemImageGallery
    //});
    $(this).ItemImageGallery({
        ItemImageGalleryModulePath: '<%=ItemImageGalleryModulePath %>',
        referImagePath:'<%=referImagePath %>'
    });
});
    //]]>
</script>

<div class="cssClassProductBigPicture cssClassPad25 clearfix" style="display:none;">     
    <asp:Literal ID="ltrItemGallery" runat="server"></asp:Literal>   
    <asp:Literal ID="ltrItemThumb" runat="server"></asp:Literal>    
</div>
