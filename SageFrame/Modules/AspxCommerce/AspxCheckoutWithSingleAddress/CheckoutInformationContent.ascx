<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckoutInformationContent.ascx.cs"
    Inherits="Modules_AspxCheckoutInformationContent_CheckoutInformationContent" %>



<div id="SingleCheckOut" class="cssClassCheckOutMethodLeft" style="display: none">
    <div class="cssClassAccordionWrapper">
        <div id="tabs">
            <ul>
                <li><a href="#checkout-1" class="sfLocale">1</a>
                </li>
                <li><a href="#checkout-2" class="sfLocale">2</a></li>
                <li><a href="#checkout-3" class="sfLocale">3</a></li>
                <li><a href="#checkout-4" class="sfLocale">4</a></li>
                <li><a href="#checkout-5" class="sfLocale">5</a></li>
                <li><a href="#checkout-6" class="sfLocale">6</a></li>
            </ul>
            <div id="checkout-1">
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Checkout Method</span></h2>
                </div>
                <div class="sfFormwrapper">
                    <div class="clearfix">
                        <div class="sfSignlecheckoutleft">
                            <p>
                                <span class="sfLocale">Checkout as a</span>&nbsp;<b class="sfLocale">Guest</b><span class="sfLocale">or</span>
                                <b><span class="sfLocale">Register</span></b>&nbsp;<span class="sfLocale">with us for future convenience:</span></p>
                            <div class="cssClassPadding">
                                <div><label>
                                    <input id="rdbGuest" type="radio" class="cssClassRadioBtn" name="guestOrRegister" />
                                    <span id="lblguest" class="sfLocale">
                                        Checkout as Guest</span>
                                </label></div>
                            
                                <div><label>
                                    <input id="rdbRegister" type="radio" class="cssClassRadioBtn" name="guestOrRegister" />
                                    <span class="sfLocale">
                                        Registered User</span>
                                </label></div>
                            </div>
                          <div class="cssClassCheck-Info">
                            <p>
                                <span class="cssClassRegisterlnk"><strong class="sfLocale">Register</strong></span>&nbsp;
                                    <span class="sfLocale">with us for future convenience. Benefits of using your registered account</span><br />
                            </p>
                            <ul class="cssClassSmallFont">
                                <li class="sfLocale">Fast and easy checkout</li>
                                <li class="sfLocale">Easy access and track to your order history and status</li>
                                <li class="sfLocale">To Track your Digital Purchase</li>
                            </ul>
                            </div>
                            <div class="sfButtonwrapper cssClassTMar15">
                                <label class="cssClassGreenBtn i-arrow-right"><button id="btnCheckOutMethodContinue" type="button">
                                    <span class="sfLocale">Continue</span></button></label>
                            </div> 
                        </div>
                        <asp:UpdatePanel ID="udpLogin" runat="server">
                            <ContentTemplate>
                                <div id="dvLogin" class="cssClassCheckOutMethodRight" style="display: none;">
                                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                                        <asp:View ID="View1" runat="server">
                                            <div class="cssClassloginbox">
                                                <div class="cssClassloginboxInside">
                                                    <div class="cssClassloginboxInsideDetails">
                                                        <div class="cssClassLoginLeftBox clearfix">
                                                            <div class="cssClassadminloginHeading">
                                                                <h2 class="cssClassBMar20">
                                                                    <asp:Label ID="lblAdminLogin" runat="server" Text="Login" meta:resourcekey="lblAdminLoginResource1"></asp:Label>
                                                                </h2>
                                                            </div>
                                                            <div class="cssClassadminloginInfo">
                                                              
                                                                            <p class="cssClassTextBox">
                                                                                <asp:TextBox ID="UserName" runat="server" meta:resourcekey="UserNameResource1"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                                                                    ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1"
                                                                                    CssClass="cssClassusernotfound" meta:resourcekey="UserNameRequiredResource1">*</asp:RequiredFieldValidator>
                                                                            </p>
                                                                       
                                                                            <p class="cssClassTextBox">
                                                                                <asp:TextBox ID="PasswordAspx" runat="server" TextMode="Password" meta:resourcekey="PasswordResource1"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordAspx"
                                                                                    ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1"
                                                                                    CssClass="cssClassusernotfound" meta:resourcekey="PasswordRequiredResource1">*</asp:RequiredFieldValidator>
                                                                            </p>
                                                                        
                                                                                        <asp:CheckBox ID="RememberMe" runat="server" CssClass="cssClassCheckBox" meta:resourcekey="RememberMeResource1" />
                                                                                   
                                                                                        <asp:Label ID="lblrmnt" runat="server" Text="Remember me." CssClass="cssClassRemember"
                                                                                            meta:resourcekey="lblrmntResource1"></asp:Label>
                                                                                    
                                                                                        <span class="cssClassForgetPass">
                                                                                            <asp:HyperLink ID="hypForgotPassword" CssClass="sfLocale" Text="Forgot Password?"
                                                                                                meta:resourcekey="hypForgotPasswordResource1" runat="server"></asp:HyperLink>
                                                                                        </span>
                                                                                   
                                                                                        <div class="sfButtonwrapper">
                                                                                         <label class="cssClassOrangeBtn">
                                                                                                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Sign In" ValidationGroup="Login1"
                                                                                                    OnClick="LoginButton_Click" meta:resourcekey="LoginButtonResource1" />
                                                                                            </label>
                                                                                        </div>
                                                                       
                                                                        <div class="cssClassusernotfound">
                                                                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False" meta:resourcekey="FailureTextResource1"></asp:Literal>
                                                                        </div>
                                                                    
                                                            </div>
                                                        </div>
                                                        <div class="cssClassLoginRighttBox" runat="server" id="divSignUp">
                                                       
                                                            <p>
                                                                <a href="/User-Registration${pageExtension}" runat="server" id="signup" class="sfLocale">
                                                                    Sign up</a> <span class="sfLocale">for a new account</span></p>
                                                            <div class="cssClassNewSIgnUp" style="display: none">
                                                                <span>»</span><a href="/User-Registration${pageExtension}" runat="server" id="signup1"
                                                                    class="sfLocale">Sign up</a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:View>
                                        <asp:View ID="View2" runat="server">
                                        </asp:View>
                                    </asp:MultiView>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div id="checkout-2">
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Billing Information</span></h2>
                </div>
                <div id="dvBilling" class="cssClassCheckoutInformationContent">
                    <div id="dvBillingInfo" class="cssClassCheckoutLeftBox">
                      
                                <ul class="clearfix">
                                    <li class="cssTextLi">
		<div><asp:Label ID="lblFirstName" runat="server" Text="First Name:" CssClass="cssClassLabel"
                                            meta:resourcekey="lblFirstNameResource1"></asp:Label><span class="cssClassRequired">*</span></div>                                   
                                        <input type="text" id="txtFirstName" name="FirstName" class="required" maxlength="40" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                       <div> <asp:Label ID="lblLastName" runat="server" Text="Last Name:" CssClass="cssClassLabel"
                                            meta:resourcekey="lblLastNameResource1"></asp:Label><span class="cssClassRequired">*</span></div>
                                   
                                        <input type="text" id="txtLastName" name="LastName" class="required" maxlength="40" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                        <div><asp:Label ID="lblEmail" runat="server" Text="Email:" CssClass="cssClassLabel" meta:resourcekey="lblEmailResource1"></asp:Label><span class="cssClassRequired">*</span>   </div>                                
                                        <input type="text" id="txtEmailAddress" name="Email" class="required email" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                       <div> <asp:Label ID="lblCompany" Text="Company:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblCompanyResource1"></asp:Label></div>                                   
                                        <input type="text" id="txtCompanyName" maxlength="40" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                        <div><asp:Label ID="lblAddress1" Text="Address 1:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblAddress1Resource1"></asp:Label><span class="cssClassRequired">*</span></div>
                                        <input type="text" id="txtAddress1" name="Address1" class="required" maxlength="250" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                       <div> <asp:Label ID="lblAddress2" Text="Address 2:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblAddress2Resource1"></asp:Label></div>                                   
                                        <input type="text" id="txtAddress2" maxlength="250" name="Address2" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                        <div><asp:Label ID="lblCountry" Text="Country:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblCountryResource1"></asp:Label><span class="cssClassRequired">*</span></div>
                                        <asp:Literal runat="server" ID="ltBLCountry"></asp:Literal>
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                        <div><asp:Label ID="lblState" Text="State/Province:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblStateResource1"></asp:Label><span class="cssClassRequired">*</span></div>
                                       <input type="text" id="txtState" name="stateprovince" class="required" maxlength="250" />
                                        <select id="ddlBLState" class="sfListmenu">
                                            <option></option>
                                        </select>
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                       <div> <asp:Label ID="lblZip" Text="Zip/Postal Code:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblZipResource1"></asp:Label><span class="cssClassRequired">*</span></div>
                                        <input type="text" id="txtZip" name="biZip" class="required alpha_dash" maxlength="10" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                       <div> <asp:Label ID="lblCity" Text="City:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblCityResource1"></asp:Label><span class="cssClassRequired">*</span></div>                                    
                                        <input type="text" id="txtCity" name="City" class="required" maxlength="250" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                       <div> <asp:Label ID="lblPhone" Text="Phone:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblPhoneResource1"></asp:Label><span class="cssClassRequired">*</span></div>                                   
                                        <input type="text" id="txtPhone" name="Phone" class="required number" maxlength="20" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                   <div><asp:Label ID="lblMobile" Text="Mobile:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblMobileResource1"></asp:Label></div><input type="text" id="txtMobile" class="number" name="mobile" maxlength="20" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li class="cssTextLi">
                                        <div><asp:Label ID="lblFax" Text="Fax:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblFaxResource1"></asp:Label></div>
                                        <input type="text" id="txtFax" name="Fax" class="number" maxlength="20" />
                                    </li>
                                    <li class="cssTextLi cssClassRMar0">
                                        <div><asp:Label ID="lblWebsite" Text="Website:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblWebsiteResource1"></asp:Label></div><input type="text" id="txtWebsite" class="url" maxlength="50" />
                                    </li>
                                </ul>
                                <ul id="trShippingAddress" class="clearfix">
                                    <li class="cssBillingChk" style="display:none">
                                        <input type="checkbox" id="chkShippingAddress" />                                   
                                        <asp:Label ID="lblDefaultShipping" Text="Use as Default Shipping Address" runat="server"
                                            CssClass="cssClassLabel" meta:resourcekey="lblDefaultShippingResource1"></asp:Label>
                                    </li>
                                </ul>
                                <ul id="trBillingAddress" class="clearfix">
                                    <li class="cssBillingChk" style="display:none">
                                        <input type="checkbox" id="chkBillingAddress" />                                   
                                        <asp:Label ID="lblDefaultBilling" Text="Use as Default Billing Address" runat="server"
                                            CssClass="cssClassLabel" meta:resourcekey="lblDefaultBillingResource1"></asp:Label>
                                    </li>
                                </ul>
                           <%-- </tbody>
                        </table>--%>
                        <input type="hidden" id="hdnAddressID" />
                    </div>
                    <div id="dvBillingSelect">
                        <h3>
                            <strong class = "sfLocale">Billing Address</strong>:<span class="cssClassRequired">*</span></h3>
                            <div class="sfButtonwrapper cssClassRightBtn cssClassTMar15">
                            <label class="cssClassGreyBtn i-plus2"><button id="addBillingAddress" type="button" value="Add Billing Address">
                                <span class="sfLocale">Billing Address</span></button></label>
                        </div>
                        <asp:Literal runat="server" ID="ltddlBilling"></asp:Literal>
                        
                    </div>
                    <p class="cssClassCheckBox">
                        <label>
                            <input id="chkBillingAsShipping" type="checkbox" /><span class="sfLocale">Use Billing Address As Shipping Address</span>
                                </label>
                    </p>
                    <p class="cssClassCheckBox">
                        <label style="display: none;">
                            <input type="checkbox" id="chkNewLetter" /><span class="sfLocale">Join the Sales Promotions and more Mailing List</span>
                                </label>
                    </p>
                    <div class="sfButtonwrapper cssClassRightBtn cssClassTMar15">
                        <label class="cssClassDarkBtn i-arrow-left"><button id="btnBillingBack" type="button" value="" class="back">
                        <span class="sfLocale">Back</span></button></label>
                        <label class="cssClassGreenBtn i-arrow-right"><button id="btnBillingContinue" type="button" value="" class="next">
                    <span class="sfLocale">Continue</span></button></label>
                    </div>
                </div>
            </div>
            <div id="checkout-3">
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Shipping Information</span></h2>
                </div>
                <div id="dvShipping" class="cssClassCheckoutInformationContent">
                    <div id="dvShippingInfo" class="cssClassCheckoutLeftBox">                                               
                                <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPFirstName" runat="server" Text="First Name" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPFirstNameResource1"></asp:Label>
                                        <span class="cssClassRequired">*</span>                                    
                                        <input id="txtSPFirstName" name="spFName" type="text" class="required" maxlength="40" />
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPLastName" runat="server" Text="Last Name:" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPLastNameResource1"></asp:Label><span class="cssClassRequired">*</span>
                                       <input id="txtSPLastName" name="spLName" type="text" class="required" maxlength="40" />
                                    </li>
                                </ul>
                               <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPEmail" runat="server" Text="Email:" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPEmailResource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <input type="text" id="txtSPEmailAddress" name="Email" class="required email" />
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPCompany" Text="Company:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPCompanyResource1"></asp:Label>                                    
                                        <input id="txtSPCompany" type="text" maxlength="50" name="SPCompany" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPAddress1" Text="Address 1:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPAddress1Resource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <input id="txtSPAddress" name="spAddress1" type="text" class="required" maxlength="250" />
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPAddress2" Text="Address2:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPAddress2Resource1"></asp:Label>
                                        <input type="text" id="txtSPAddress2" maxlength="250" name="SPAddress2" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPCountry" Text="Country:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPCountryResource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <asp:Literal runat="server" ID="ltSPCountry"></asp:Literal>
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPState" Text="State/Province:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPStateResource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <input type="text" id="txtSPState" name="spstateprovince" class="required" maxlength="250" />
                                        <select id="ddlSPState" class="sfListmenu">
                                            <option></option>
                                        </select>
                                    </li>
                                </ul>
                               <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPZip" Text="Zip/Postal Code:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPZipResource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <input id="txtSPZip" name="spZip" type="text" class="required alpha_dash" maxlength="10" />
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPCity" Text="City:" runat="server" CssClass="cssClassLabel" meta:resourcekey="lblSPCityResource1"></asp:Label><span
                                            class="cssClassRequired">*</span>                                   
                                        <input type="text" id="txtSPCity" name="City" class="required" maxlength="250" />
                                    </li>
                                </ul>
                                <ul class="clearfix">
                                    <li>
                                        <asp:Label ID="lblSPPhone" Text="Phone:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPPhoneResource1"></asp:Label><span class="cssClassRequired">*</span>
                                        <input id="txtSPPhone" name="spPhone" type="text" class="required number" maxlength="20" />
                                    </li>
                                    <li class="cssClassRMar0">
                                        <asp:Label ID="lblSPMobile" Text="Mobile:" runat="server" CssClass="cssClassLabel"
                                            meta:resourcekey="lblSPMobileResource1"></asp:Label>                                   
                                        <input type="text" id="txtSPMobile" name="spmobile" class="number" maxlength="20" />
                                    </li>
                                </ul>                           
                    </div>
                    <div id="dvShippingSelect">
                        <h3 class="sfLocale">
                            <strong>Shipping Address :</strong>
                             <span class="cssClassRequired">*</span>
                        </h3>
                       
                        <div class="sfButtonwrapper cssClassRightBtn cssClassTMar15">
                           <label class="cssClassGreyBtn i-plus2"><button id="addShippingAddress" type="button">
                                <span class="sfLocale">Shipping Address</span></button></label>
                        </div>
                        <asp:Literal runat="server" ID="ltddlShipping"></asp:Literal>
                    </div>
                 
                    <div class="sfButtonwrapper cssClassRightBtn">
                       <label class="cssClassDarkBtn i-arrow-left"> <button id="btnShippingBack" type="button" value="" class="back">
                            <span class="sfLocale">Back</span></button></label>
                        <label class="cssClassGreenBtn i-arrow-right"><button id="btnShippingContinue" type="button" value="" class="continue">
                            <span class="sfLocale">Continue</span></button></label>
                    </div>
                    <div class="cssClassClear">
                    </div>
                </div>
            </div>
            <div id="checkout-4">
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Shipping Method</span>
                    </h2>
                </div>
                <div id="dvPaymentsMethod" class="cssClassShippingMethodTab">
                    <div id="divShippingMethod" class="cssClassShippingMethodInfo cssClassCartInformation">
                    </div>
                    <div class="sfButtonwrapper cssClassRightBtn cssClassTMar15">
                       <label class="cssClassDarkBtn i-arrow-left"> <button id="btnShippingMethodBack" type="button" value="" class="back">
                           <span class="sfLocale">Back</span></button></label>
                        <label class="cssClassGreenBtn i-arrow-right"><button id="btnShippingMethodContinue" type="button" value="" class="continue">
                            <span class="sfLocale">Continue</span></button></label>
                    </div>
                </div>
            </div>
            <div id="checkout-5">
           
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Payment Information</b></span>
                </div>
                <div id="dvPaymentInfo" class="cssClassPaymentMethods">
                    <div id="dvPGList">
                        <asp:Literal runat="server" ID="ltPgList"></asp:Literal>
                    </div>
                    <div id="dvPGListLogo">
                    </div>
                    <div class="sfButtonwrapper cssClassRightBtn cssClassTMar20">
                        <label class="cssClassDarkBtn i-arrow-left"><button id="btnPaymentInfoBack" type="button" value="" class="back">
                            <span class="sfLocale">Back</span></button></label>
                        <label class="cssClassGreenBtn i-arrow-right"><button id="btnPaymentInfoContinue" type="button" value="" class="continue">
                            <span class="sfLocale">Continue</span></button></label>
                    </div>
                </div>
            </div>
            <div id="checkout-6">
                <div class="accordionHeading">
                    <h2 class="cssClassBMar20">
                        <span class="sfLocale">Order Review</span>
                    </h2>
                </div>
                <div id="dvPlaceOrder" class="cssClassOrderReview">
                    <div class="cssClassCartInformationDetails" id="divCartDetails">
                        <asp:Literal runat="server" ID="ltTblCart"></asp:Literal>
                    </div>
                 
                <table class="cssClassSubTotalAmount noborder"  width="100%" >
      
    </table>
<div class="cssClassCartInformation">
<div class="cssGrandTotal">  <strong class="sfLocale">Grand Total:</strong>
<label id="lblTotalCost" class="cssClassFormatCurrency sfLocale"></label></div>
<div> <strong class="sfLocale">Additional Note:</strong>
<textarea id="txtAdditionalNote" class="cssClassTextarea" rows="3" cols="90"></textarea></div></div>
                    <div class="sfButtonwrapper cssClassTMar20">
                        <label class="cssClassDarkBtn i-arrow-left"><button id="btnPlaceBack" type="button" value="back" class="back">
                            <span class="sfLocale">Back</span></button></label>
                        <%--<button id="btnPlaceOrder" type="submit" class ="submit" ><span><span>Place Order</span></span></button>--%>
                    </div>
                    <div>
                        <asp:Literal runat="server" ID="ltRewardPoint"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="cssClassRightAccordainMenu" style="display:none;">
    <div class="cssClassRightAccordainTab">
        <div class="cssClassRightAccordainMenuInfo">
            <h3>
               <b class="sfLocale">Billing Address</b><span id="divBillingBtn" class="sfBtnChange"></span></h3>
            <div id="dvCPBilling">
            </div>
        </div>
        <div class="cssClassRightAccordainMenuInfo">
            <h3>
               <b class="sfLocale">Shipping Address</b><span id="divShippingAddressBtn" class="sfBtnChange"></span></h3>
            <div id="dvCPShipping">
            </div>
        </div>
        <div class="cssClassRightAccordainMenuInfo">
            <h3>
                <b class="sfLocale">Shipping Method</b><span id="divShippingMethodBtn" class="sfBtnChange"></span></h3>
            <div id="dvCPShippingMethod">
            </div>
        </div>
        <div class="cssClassRightAccordainMenuInfoSelected">
            <h3>
               <b class="sfLocale">Payment Method</b><span id="divPaymentBtn" class="sfBtnChange"></span></h3>
            <div id="dvCPPaymentMethod">
            </div>
        </div>
        <%--<div class="cssClassRightAccordainMenuInfoSelected">
            <h2>
                Payment Gateway Type</h2>
        </div>
        <div id="dvPaymentGatewayTypeMethod">
        </div>--%>
    </div>
</div>
<div class="popupbox" id="popuprel">
<div class="cssPopUpBody">
    <div class="cssClassCloseIcon">
        <button type="button" class="cssClassClose">
            <span class="sfLocale"><i class="i-close"></i>Close</span></button>
    </div>
    <h2>
        <asp:Label ID="lblAddressTitle" runat="server" Text="Address Details" meta:resourcekey="lblAddressTitleResource1"></asp:Label>
    </h2>
    <div class="sfFormwrapper">
        <div class="sfButtonwrapper">
           <label class="cssClassGreenBtn i-save"><button type="button" id="btnSubmitAddress" class="cssClassButtonSubmit">
                <span class="sfLocale">Save</span></button></label>
        </div>
    </div>
</div>
</div>
<select style="display: none;" id="hdnCountryList">
    <option value="AF">Afghanistan</option>
    <option value="AL">Albania</option>
    <option value="DZ">Algeria</option>
    <option value="AS">American Samoa</option>
    <option value="AD">Andorra</option>
    <option value="AO">Angola</option>
    <option value="AI">Anguilla</option>
    <option value="AQ">Antarctica</option>
    <option value="AG">Antigua and Barbuda</option>
    <option value="AR">Argentina</option>
    <option value="AM">Armenia</option>
    <option value="AW">Aruba</option>
    <option value="AU">Australia</option>
    <option value="AT">Austria</option>
    <option value="AZ">Azerbaijan</option>
    <option value="BS">Bahamas</option>
    <option value="BH">Bahrain</option>
    <option value="BD">Bangladesh</option>
    <option value="BB">Barbados</option>
    <option value="BY">Belarus</option>
    <option value="BE">Belgium</option>
    <option value="BZ">Belize</option>
    <option value="BJ">Benin</option>
    <option value="BM">Bermuda</option>
    <option value="BT">Bhutan</option>
    <option value="BO">Bolivia</option>
    <option value="BA">Bosnia and Herzegovina</option>
    <option value="BW">Botswana</option>
    <option value="BV">Bouvet Island</option>
    <option value="BR">Brazil</option>
    <option value="IO">British Indian Ocean Territory</option>
    <option value="VG">British Virgin Islands</option>
    <option value="BN">Brunei Darussalam</option>
    <option value="BG">Bulgaria</option>
    <option value="BF">Burkina Faso</option>
    <option value="BI">Burundi</option>
    <option value="KH">Cambodia</option>
    <option value="CM">Cameroon</option>
    <option value="CA">Canada</option>
    <option value="CV">Cape Verde</option>
    <option value="KY">Cayman Islands</option>
    <option value="CF">Central African Republic</option>
    <option value="TD">Chad</option>
    <option value="CL">Chile</option>
    <option value="CN">China</option>
    <option value="CX">Christmas Island</option>
    <option value="CC">Cocos</option>
    <option value="CO">Colombia</option>
    <option value="KM">Comoros</option>
    <option value="CG">Congo</option>
    <option value="CK">Cook Islands</option>
    <option value="CR">Costa Rica</option>
    <option value="Country">Country</option>
    <option value="HR">Croatia</option>
    <option value="CU">Cuba</option>
    <option value="CY">Cyprus</option>
    <option value="CZ">Czech Republic</option>
    <option value="DK">Denmark</option>
    <option value="DJ">Djibouti</option>
    <option value="DM">Dominica</option>
    <option value="DO">Dominican Republic</option>
    <option value="TP">East Timor</option>
    <option value="EC">Ecuador</option>
    <option value="EG">Egypt</option>
    <option value="SV">El Salvador</option>
    <option value="GQ">Equatorial Guinea</option>
    <option value="ER">Eritrea</option>
    <option value="EE">Estonia</option>
    <option value="ET">Ethiopia</option>
    <option value="FK">Falkland Islands</option>
    <option value="FO">Faroe Islands</option>
    <option value="FJ">Fiji</option>
    <option value="FI">Finland</option>
    <option value="FR">France</option>
    <option value="GF">French Guiana</option>
    <option value="PF">French Polynesia</option>
    <option value="TF">French Southern Territories</option>
    <option value="GA">Gabon</option>
    <option value="GM">Gambia</option>
    <option value="GE">Georgia</option>
    <option value="DE">Germany</option>
    <option value="GH">Ghana</option>
    <option value="GI">Gibraltar</option>
    <option value="GR">Greece</option>
    <option value="GL">Greenland</option>
    <option value="GD">Grenada</option>
    <option value="GP">Guadeloupe</option>
    <option value="GU">Guam</option>
    <option value="GT">Guatemala</option>
    <option value="GN">Guinea</option>
    <option value="GW">Guinea-Bissau</option>
    <option value="GY">Guyana</option>
    <option value="HT">Haiti</option>
    <option value="HM">Heard and McDonald Islands</option>
    <option value="HN">Honduras</option>
    <option value="HK">Hong Kong</option>
    <option value="HU">Hungary</option>
    <option value="IS">Iceland</option>
    <option value="IN">India</option>
    <option value="ID">Indonesia</option>
    <option value="IR">Iran</option>
    <option value="IQ">Iraq</option>
    <option value="IE">Ireland</option>
    <option value="IL">Israel</option>
    <option value="IT">Italy</option>
    <option value="CI">Ivory Coast</option>
    <option value="JM">Jamaica</option>
    <option value="JP">Japan</option>
    <option value="JO">Jordan</option>
    <option value="KZ">Kazakhstan</option>
    <option value="KE">Kenya</option>
    <option value="KI">Kiribati</option>
    <option value="KW">Kuwait</option>
    <option value="KG">Kyrgyzstan</option>
    <option value="LA">Laos</option>
    <option value="LV">Latvia</option>
    <option value="LB">Lebanon</option>
    <option value="LS">Lesotho</option>
    <option value="LR">Liberia</option>
    <option value="LY">Libya</option>
    <option value="LI">Liechtenstein</option>
    <option value="LT">Lithuania</option>
    <option value="LU">Luxembourg</option>
    <option value="MO">Macau</option>
    <option value="MK">Macedonia</option>
    <option value="MG">Madagascar</option>
    <option value="MW">Malawi</option>
    <option value="MY">Malaysia</option>
    <option value="MV">Maldives</option>
    <option value="ML">Mali</option>
    <option value="MT">Malta</option>
    <option value="MH">Marshall Islands</option>
    <option value="MQ">Martinique</option>
    <option value="MR">Mauritania</option>
    <option value="MU">Mauritius</option>
    <option value="YT">Mayotte</option>
    <option value="MX">Mexico</option>
    <option value="FM">Micronesia</option>
    <option value="MD">Moldova</option>
    <option value="MC">Monaco</option>
    <option value="MN">Mongolia</option>
    <option value="MS">Montserrat</option>
    <option value="MA">Morocco</option>
    <option value="MZ">Mozambique</option>
    <option value="MM">Myanmar</option>
    <option value="NA">Namibia</option>
    <option value="NR">Nauru</option>
    <option value="NP">Nepal</option>
    <option value="NL">Netherlands</option>
    <option value="AN">Netherlands Antilles</option>
    <option value="NC">New Caledonia</option>
    <option value="NZ">New Zealand</option>
    <option value="NI">Nicaragua</option>
    <option value="NE">Niger</option>
    <option value="NG">Nigeria</option>
    <option value="NU">Niue</option>
    <option value="NF">Norfolk Island</option>
    <option value="KP">North Korea</option>
    <option value="MP">Northern Mariana Islands</option>
    <option value="NO">Norway</option>
    <option value="OM">Oman</option>
    <option value="PK">Pakistan</option>
    <option value="PW">Palau</option>
    <option value="PA">Panama</option>
    <option value="PG">Papua New Guinea</option>
    <option value="PY">Paraguay</option>
    <option value="PE">Peru</option>
    <option value="PH">Philippines</option>
    <option value="PN">Pitcairn</option>
    <option value="PL">Poland</option>
    <option value="PT">Portugal</option>
    <option value="PR">Puerto Rico</option>
    <option value="QA">Qatar</option>
    <option value="RE">Reunion</option>
    <option value="RO">Romania</option>
    <option value="RU">Russian Federation</option>
    <option value="RW">Rwanda</option>
    <option value="GS">S. Georgia and S. Sandwich Islands</option>
    <option value="KN">Saint Kitts and Nevis</option>
    <option value="LC">Saint Lucia</option>
    <option value="VC">Saint Vincent and The Grenadines</option>
    <option value="WS">Samoa</option>
    <option value="SM">San Marino</option>
    <option value="ST">Sao Tome and Principe</option>
    <option value="SA">Saudi Arabia</option>
    <option value="SN">Senegal</option>
    <option value="SC">Seychelles</option>
    <option value="SL">Sierra Leone</option>
    <option value="SG">Singapore</option>
    <option value="SK">Slovakia</option>
    <option value="SI">Slovenia</option>
    <option value="SB">Solomon Islands</option>
    <option value="SO">Somalia</option>
    <option value="ZA">South Africa</option>
    <option value="KR">South Korea</option>
    <option value="SU">Soviet Union</option>
    <option value="ES">Spain</option>
    <option value="LK">Sri Lanka</option>
    <option value="SH">St. Helena</option>
    <option value="PM">St. Pierre and Miquelon</option>
    <option value="SD">Sudan</option>
    <option value="SR">Suriname</option>
    <option value="SJ">Svalbard and Jan Mayen Islands</option>
    <option value="SZ">Swaziland</option>
    <option value="SE">Sweden</option>
    <option value="CH">Switzerland</option>
    <option value="SY">Syria</option>
    <option value="TW">Taiwan</option>
    <option value="TJ">Tajikistan</option>
    <option value="TZ">Tanzania</option>
    <option value="TH">Thailand</option>
    <option value="TG">Togo</option>
    <option value="TK">Tokelau</option>
    <option value="TO">Tonga</option>
    <option value="TT">Trinidad and Tobago</option>
    <option value="TN">Tunisia</option>
    <option value="TR">Turkey</option>
    <option value="TM">Turkmenistan</option>
    <option value="TC">Turks and Caicos Islands</option>
    <option value="TV">Tuvalu</option>
    <option value="UG">Uganda</option>
    <option value="UA">Ukraine</option>
    <option value="AE">United Arab Emirates</option>
    <option value="UK">United Kingdom</option>
    <option value="US">United States</option>
    <option value="UY">Uruguay</option>
    <option value="UM">US Minor Outlying Islands</option>
    <option value="VI">US Virgin Islands</option>
    <option value="UZ">Uzbekistan</option>
    <option value="VU">Vanuatu</option>
    <option value="VE">Venezuela</option>
    <option value="VN">Viet Nam</option>
    <option value="WF">Wallis and Futuna Islands</option>
    <option value="EH">Western Sahara</option>
    <option value="YE">Yemen</option>
    <option value="YU">Yugoslavia</option>
    <option value="ZR">Zaire</option>
    <option value="ZM">Zambia</option>
</select>


<script type="text/javascript">
    //<![CDATA[   
    var isKPIInstalled = false;   
    $(document).ready(function () {
        $(this).SingleCheckout({
            ServerVars: '<%=ServerVars %>',
            Coupon: '<%=Coupon %>',
            Items: '<%=Items %>',
            GiftCard: '<%=GiftCard%>',
            InitScript: '<%=ScriptsToRun%>',
            cartCount:'<%=cartCount %>'
        });

        $(function () {
            $(".sfLocale").localize({
                moduleKey: AspxCheckoutWithSingleAddress
            });

        });
        //ABTest Saves Visit for Checkout Page
        $(window).load(function () {
            if (AspxCommerce.IsModuleInstalled('AspxABTesting')) {
                ABTest.ABTestSaveVisitCount();
            }
            if (AspxCommerce.IsModuleInstalled('AspxKPI')) {
                isKPIInstalled = true;
            }
            if (isKPIInstalled) {
                //KPI Saves Visit Counts for billing     
                KPICommon.KPISaveVisit('Billing and Shipping');
            }
        });

        function afterAsyncPostBack() {
            if ($('#rdbRegister').is(":checked")) {

                $('#dvLogin').show();
            }
        }

        Sys.Application.add_init(appl_init);

        function appl_init() {
            var pgRegMgr = Sys.WebForms.PageRequestManager.getInstance();
            pgRegMgr.add_endRequest(EndHandler);
        }
        function EndHandler() {
            afterAsyncPostBack();
        }
    });
    //]]>
</script>