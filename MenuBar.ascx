<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuBar.ascx.cs" Inherits="Control_MenuBar" %>

<div id="top">
    <div>
    <div id="topBanner">
        
    <div  class="userInfo" style="margin-top: 85px;margin-right: 0px;"></div>
    </div>
    </div>
    <div class="clearLeftFloat"></div>
     
    <ul id="topMenu">
        <li><a href="home.aspx"><div class="filter"></div><div class="content">HOME</div></a></li>
        <li><a href="http://172.16.2.108/page_com/Login.aspx"><div class="filter"></div><div class="content">E-LEAVE</div></a></li>
        <li runat="server" style="display:none;" id="liAdmin1"><a href="javascript:;"><div class="filter" ></div><div class="content">E-Payslip</div></a></li>
        <li runat="server" style="display:none;" id="liAdmin2"><a href="javascript:;"><div class="filter" ></div><div class="content">E-Tax</div></a></li>
        <li><a href="CareerList.aspx" ><div class="filter"></div><div class="content">CAREER OPPORTUNITIES</div></a></li>
        <li runat="server" id="liAdmin" style="position: relative; z-index:2;">
            <a href="javascript:;"><div class="filter"></div><div class="content">ADMIN</div></a> 
            <ul class="sub-menu">
                <li><a href="ArticleManager.aspx">ARTICLE MANAGER</a></li>
                <li><a href="FileUpload.aspx">FILE UPLOAD MANAGER</a></li>
                <li><a href="SystemLinkManager.aspx">SYSTEM LINK MANAGER</a></li> 
                <li  runat="server" id="liChangeUserRole" ><a href="ChangeUserRole.aspx">USER ROLE MANAGER</a></li>
                <li><a href="SystemParameters.aspx">SYSTEM PARAMETERS</a></li>
                <li runat="server" id="liApprove" ><a href="/Approve.aspx">User Admin</a></li>
            </ul>
        </li>
        <li  style="position: relative; z-index:2;">
            <a href="javascript:;"><div class="filter"></div><div class="content">REPORT</div></a> 
            <ul class="sub-menu"> 
                <li  runat="server" id="liTrainingReport" ><a href="TrainingReport.aspx">TRAINING REPORT</a></li>
                <li  runat="server" id="liEventReport" ><a href="EventReport.aspx">EVENT REPORT</a></li> 
                <li  runat="server" id="liSuggestionReport" ><a href="SuggestionReport.aspx">SUGGESTION REPORT</a></li> 
            </ul>
        </li>
        <li id="liSystemLink" style="position: relative;z-index:2; display:none;">
            <a href="javascript:;"><div class="filter"></div><div class="content">OTHER SYSTEM</div></a>

        </li> 
    </ul>
    <div style="position: relative; margin-bottom: 12px;">
                <div class="userInfo" style="position: absolute; top:3px; right: 10px;">
                    Welcome! <%= Session["LOGINNAME"] %> <span id="spanLogout" style="padding-right: 20px;" runat="server"> | <a id="linkLogout" href="javascript:;">Logout</a></span>
         &nbsp&nbsp&nbsp&nbsp Last Login Time: <%= Session["LASTLOGINDATETIME"].ToString() %> 
        </div>
        
    <div class="clearLeftFloat"></div>
    </div>
</div>   

<script>
    $(function () {
        var isAdmin = '<%= (Session["USERGROUP"].ToString().Equals(GlobalSetting.SystemRoles.Normal)).ToString().ToUpper() %>';
        
        if (isAdmin == "TRUE")
            $("#topMenu>li").css('width', '25%');
        else
            $("#topMenu>li").css('width', '20%');

    });

</script>
