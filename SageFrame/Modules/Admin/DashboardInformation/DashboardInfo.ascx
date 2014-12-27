<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardInfo.ascx.cs"
    Inherits="Modules_Admin_DashboardInfo_DashboardInfo" %>
<div class="sfWelcomeWrap clearfix" runat="server" id="divDashboardHelp">
    <label class="icon-close sfFloatRight">
        <asp:Button runat="server" ID="btnDisableDashboardhelp" OnClick="btnDisableDashboardhelp_Click" />
    </label>
    <h1>
        Hi! Welcome to SageFrame Welcome Screen.</h1>
    <h2>
        You can take a quick tour to SageFrame, watch video tutorials, read user manuals
        and get an insight of available modules.</h2>
    <div class="sfTutorialWrap clearfix">
        <ul>
            <li class="sfTutorialA">
                <div class="sfTakeTour " id="sfTakeTour">
                </div>
                <h4>
                    Get acquainted with the SageFrame Dashboard.</h4>
                <span class="sfTour">Take the tour</span> </li>
            <li class="sfTutorialB"><a href="http://www.sageframe.com/Video-Gallery.aspx"></a>
                <h4>
                    Let’s start building the site.</h4>
                <span>Watch the video tutorials to get started with your first SageFrame site. </span>
            </li>
            <li class="sfTutorialC">
                <h3>
                    SageFrame Tutorials</h3>
                <a target="new" href="http://www.sageframe.com/Resources/Tutorials.aspx?RSS=Blog&amp;blogid=14">
                    Document Library</a> <a target="new" href="http://www.sageframe.com/Resources/Tutorials.aspx?RSS=Blog&amp;blogid=13">
                        How to create a module in SageFrame? </a><a href="#">How to create templates in SageFrame?</a>
                <li class="sfModuleList">
                    <h3>
                        SageFrame Module List</h3>
                    <a href="#">HTMLTab-Version: 3.0 </a> <a href="#">Event Manager-Version: 3.0</a>
                    <a href="#">CountOnlineUser-Version: 3.0 </a> 
                    <a href="#">Polling-Version: 3.0 </a> 
                    <a href="#">ReadMore-Version: 3.0 </a> 
                    </li>
        </ul>
    </div>
</div>
