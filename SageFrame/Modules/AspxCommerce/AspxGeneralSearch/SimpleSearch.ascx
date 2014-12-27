<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimpleSearch.ascx.cs"
    Inherits="Modules_AspxGeneralSearch_SimpleSearch" %>

<script type="text/javascript">
    //<![CDATA[
    $(function () {

        $(".sfLocale").localize({
            moduleKey: AspxGeneralSearch
        });

        $(this).SimpleSearchInit({           
            ShowCategoryForSearch: '<%=ShowCategoryForSearch %>',
            EnableAdvanceSearch: '<%=EnableAdvanceSearch %>',
            ShowSearchKeyWords: '<%=ShowSearchKeyWords %>',
            ResultPage: '<%=ResultPage%>'
        });
             
        if ('<%=ShowCategoryForSearch %>'.toLowerCase() == 'true') {
            $("#sfFrontCategory").show();

        }
        if ('<%=EnableAdvanceSearch %>'.toLowerCase() == 'true') {
            $("#lnkAdvanceSearch").show();
        }
        if ('<%=ShowSearchKeyWords %>'.toLowerCase() == 'true') {
            $("#topSearch").show();
        }
    });

    //]]>
</script>

<div class="cssClassSageSearchWrapper">
<div class="cssSearchButton" id="divGeneralSearch">
    <label class="i-search cssClassSearch sfLocale" id="lblGeneralSearch"></label>
    </div>
    <div class="cssSearchContainer" style="display:none">   
    <ul>
        <li>
            <div id="sfFrontCategory" style="display: none">
                <div class="sfCategoryFrontMenuDropdown">
                   <%-- <dl class="sfCategoryDropdown" >
                        <dt><a href="#"><span id="spanResult" class="sfLocale">Select Category</span></a></dt>
                        <dd>--%>
                            <asp:Literal ID="litSSCat" runat="server" EnableViewState="False" 
                                meta:resourcekey="litSSCatResource1"></asp:Literal>
                       <%-- </dd>
                    </dl>--%>
                </div>
                <input type="hidden" value="0" id="txtSelectedCategory" name="selectedCategory" />
            </div>
        </li>
        <li>
            <input type="text" id="txtSimpleSearchText" class="cssClassSageSearchBox" />
            <input type="button" id="btnSimpleSearch" class="cssClassSageSearchButton sfLocale"
                value="Go" />
       
            </li>
    </ul>
    <%--<a href="#" id="lnkAdvanceSearch" class="cssClassAdvanceSearch">Go For Advanced Search</a>--%>
    <asp:Literal ID="litTopSearch" runat="server" EnableViewState="False" 
        meta:resourcekey="litTopSearchResource1"></asp:Literal>
    <div>  
    <a href="#" id="lnkAdvanceSearch" class="cssClassAdvanceSearchLink sfLocale" style="display: none">
            </a>
     </div>
 </div>
</div>
