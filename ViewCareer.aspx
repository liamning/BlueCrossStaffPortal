<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewCareer.aspx.cs" Inherits="ViewCareer" %>
<%@ Register Src="~/Control/MenuBar.ascx" TagPrefix="uc1" TagName="MenuBar" %>
<%@ Register Src="~/Control/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>
<%@ Register Src="~/Control/PublicHeader.ascx" TagPrefix="uc1" TagName="PublicHeader" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title></title> 
    <uc1:PublicHeader runat="server" ID="PublicHeader" />
    <script type="text/javascript" src="Resource/ckeditor/ckeditor.js"></script>
    <script type="text/javascript">
        $(function () {

        });
    </script>
</head>
<body>
    <form id="form1" runat="server"> 
        <uc1:MenuBar runat="server" ID="MenuBar" />
    <div id="content">
        <div class="clearLeftFloat"></div>
        <div id="navigationBar">
            <p>Staff Portal > Article Manager> Career</p> 
        </div> 
             
        <div id="center">
    <div class="font12pt"> 
    <table id="tableContent" class="viewTable">
       
        <tr>
            <td  style="width: 110px;">Position :</td>
            <td  class="articleTitle" colspan=3> 
                <label runat="server" id="txtJobFunction" />
            </td>
        </tr> 
        <tr> 
            <td>Employment Type :</td>
            <td  style="width: 200px;">
                <label runat="server" id="txtType" ></label>
            </td>
            <td  style="width: 100px;">Education Level :</td>
            <td > 
                <label runat="server" id="txtCareerLevel" style="display:none;" >
                </label>
                <asp:HiddenField ID="trainingID" runat="server" />
                <label runat="server" id="txtQualification" /> 
            </td>
        </tr>
        <tr>
            <td>Yr(s) of Exp :</td>
            <td>
                <label runat="server" id="txtExp"  ></label>
            </td>
            <td >Division :</td>
            <td >
                <label runat="server" id="txtDivision"  />
            </td>
        </tr> 
        <tr>
            <td >Department :</td>
            <td>
                <label runat="server" id="txtDepartment" ></label>
            </td>
            <td >Email Address :</td>
            <td colspan=3> 
                <a runat="server" id="txtEmail" class="fileUploadWidth" />
            </td>
        </tr> 
        <tr>
            <td >Location :</td>
            <td colspan=3> 
                <label runat="server" id="txtLocation" class="editorWidth" />
            </td>
        </tr>   
        
        <tr>
            <td >Details :</td>
            <td colspan="3"> 
                <div runat="server" id="txtDetails" class="editorWidth normal" ></div>
            </td>
        </tr>
        <tr>
            <td  style="font-weight:bold;">Disclaimer :</td>
            <td colspan="3"> 
                <div runat="server" id="txtDisclaimer" class="editorWidth" ></div>
            </td>
        </tr>
    </table> 
    
    </div>
        
        </div>
        <div style="clear:both;"></div>
        <uc1:Footer runat="server" ID="Footer" />
    </div>

    </form>
</body>
</html>
