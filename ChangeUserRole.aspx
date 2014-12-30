<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeUserRole.aspx.cs" Inherits="ChangeUserRole" %>


<%@ Register Src="~/Control/MenuBar.ascx" TagPrefix="uc1" TagName="MenuBar" %>
<%@ Register Src="~/Control/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>
<%@ Register Src="~/Control/PublicHeader.ascx" TagPrefix="uc1" TagName="PublicHeader" %>




<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title></title>
    <uc1:PublicHeader runat="server" ID="PublicHeader" />


    <script type="text/javascript">
        $(function () {
            //init the user grid
            $userGridContainer = $("#userGridContainer");
            var userGrid = new function () {
                return new Grid({
                    parent: $userGridContainer[0],
                    checkbox: { selectAll: true, style: "width:20px;background:#E5F0D4;", titleStyle: "width:20px; text-align:left;" },
                    width: 800,
                    fields: [
                    { name: "fullname", title: "Name", style: "width:150px;text-align: left;", titleStyle: "width:150px;text-align: left;" },
                    { name: "email", title: "Email", style: "width:280px;text-align: left;", titleStyle: "width:280px;text-align: left;" },
                    { name: "logindatetime", title: "Last Login Time", style: "width:120px;text-align: left;", titleStyle: "width:120px;text-align: left;" },
                    { name: "usergroup", title: "User Role", style: "width:150px;text-align: left;", titleStyle: "width:150px;text-align: left;" }
                    ]
                });
            }

            var fillUserGrid = function () {

                $.ajax({
                    type: 'POST',
                    url: "Service/AjaxService.aspx",
                    data: {
                        action: "getAllUsers"
                    },
                    success: function (data) {
                        data = eval("(" + data + ')');
                        userGrid.setData(data);
                    }
                });
            }

            $btnApprove = $("#btnApprove");
            $ddlUserRole = $("#ddlUserRole");
             
            fillUserGrid(); 


            var updateUserRole = function (IDs, role) {

                //start to update the status
                $.ajax({
                    type: 'POST',
                    url: "Service/AjaxService.aspx",
                    data: {
                        action: "updateUserRole",
                        IDs: IDs,
                        Role: role
                    },
                    success: function (data) {
                        data = eval("(" + data + ')');
                        alert(data.message);
                        if (data.result) {
                            fillUserGrid(); 
                        }
                    }
                });
            }

            $btnApprove.click(function () {
                var dataSelected = userGrid.getSelectdData();
                var IDs = [];
                for (var i = 0, user; user = dataSelected[i]; i++) {
                    IDs.push(user.id);
                }

                if (IDs.length == 0) return;
                var role = $ddlUserRole.val();
                updateUserRole(IDs, role);

            });
             
             
        });
    </script>
</head>

<body>
    <form id="form1" runat="server">

        <uc1:MenuBar runat="server" ID="MenuBar" />


    <div id="content">
        <div class="clearLeftFloat"></div>
        <div id="navigationBar">
            <p>Staff Portal > User Role Manager</p> 
        </div>        
        <div id="center" class="font12pt" >

    <table style="width: 800px;"> 
        <tr>
            <td colspan=2 >
            <h4 class="bar">User Role Manager</h4>       
            </td>      
                  
        </tr>
        <tr>
            <td style="padding-bottom: 5px;" class="titleTd"> 
            </td>            
            
            <td style="padding-bottom: 5px; text-align: right;"  >
                <select id="ddlUserRole">
                    <option value="Normal">Normal User</option>
                    <option value="Admin">Administrator</option>
                </select>
            <input type="button" value="Update" id="btnApprove" /> </td>  
        </tr>
        <tr>
            <td colspan=2 class="titleTd">
                <div id="userGridContainer"></div>
            </td>
        </tr> 
    </table>

    </div>
        <div style="clear:both;"></div>
        <uc1:Footer runat="server" ID="Footer" />
    </div>
    </form>
</body>
</html>

