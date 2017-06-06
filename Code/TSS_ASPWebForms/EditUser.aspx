<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="TSS_ASPWebForms.EditUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css" />
    <link href="CSS/EditUser.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/exif.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-EditUser.js"></script>

    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <meta name="google" content="notranslate" />
</head>
<body>
    <div class="centerPage container-fluid" id="MainContainer" style="border-width:2px;border:solid;background-color:lightblue;">
        <form id="EditUserForm" runat="server" autocomplete="off">
            <div id="EditUser">
                <div id="loadingUserDiv" class="blocker" hidden="hidden" runat="server" style="z-index:999;">
                    <div class="blocker-content">
                        <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/system/loading.gif" style="width:3em;height:auto;z-index:9999;"/>
                    </div>
                </div>
                <script type="text/javascript" id="ReadOnly">
                    function myFunction() {
                        if ('True' == '<%# (ReadOnly) %>')
                                alert('<%# ReadOnlyMsg %>');
                    }
                </script>
                <table style="width:80%;table-layout:fixed;margin-bottom:10px;margin-right:10%;margin-left:10%;max-height:100%;">
                    <tr>
                        <!-- amount of columns -->
                        <th style="width:25%;"></th>
                        <th style="width:25%;"></th>
                        <th style="width:25%;"></th>
                        <th style="width:25%;"></th>
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;margin-left:auto;margin-right:auto;height:8em;max-height:400px;width:auto;text-align:center" colspan="4">
                            <asp:Image id="UserPhoto" runat="server" ImageUrl="system/DefaultUser.jpg" AlternateText="User Image" style="height:inherit;max-height:inherit;width:inherit;max-width:inherit;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;margin-left:auto;margin-right:auto;width:auto;text-align:center" colspan="4">
                            <button runat="server" id="addPicture" type="button" onclick="$('#ProfilePicInput').click(); return false;" style="height:100%;min-height:2.5em;" disabled="<%# ReadOnly %>">
                                <asp:Label runat="server" Text="Add Picture" meta:resourcekey="AddPicture"></asp:Label>
                            </button>
                            <input id="ProfilePicInput" runat="server" type="file" accept="image/*" capture="camera" onchange="AddUserPhoto(this); return false;" style="visibility:hidden" disabled="<%# ReadOnly %>"/>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <asp:Label runat="server" style="float:right;" Text="Username :" meta:resourcekey="Username"></asp:Label>
                        </td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;" colspan="3">
                            <input id="txtUsername" runat="server" type="text" autocomplete="off" style="width:100%;" value="<%# EdittedUser.Username %>" onblur="updateUsername(); return false;" disabled="<%# ReadOnly %>" />
                        </td>  
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <asp:Label runat="server" style="float:right;" Text="Password :" meta:resourcekey="Password"></asp:Label>
                        </td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;" colspan="3">
                            <input id="txtPassword" runat="server" type="password" autocomplete="off" style="width:100%;" onblur="updatePassword(); return false;" disabled="<%# ReadOnly %>" />
                        </td>  
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <asp:Label runat="server" style="float:right;" Text="Department :" meta:resourcekey="Department"></asp:Label>
                        </td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <asp:DropDownList runat="server" id="selectDepartment" style="width:100%;" DataValueField="ID" DataTextField="Name" onchange="updateDepartment(); return false;" Enabled="<%# ReadOnly == false %>" ></asp:DropDownList>
                        </td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <asp:Label runat="server" style="float:right;" Text="User Enabled :" meta:resourcekey="UserEnabled"></asp:Label>
                        </td>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;">
                            <input type="checkbox" runat="server" id="UserActive" style="float:left" checked="<%# EdittedUser.Active %>" onchange="updateActive(); return false;" disabled="<%# ReadOnly %>" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;" colspan="1">
                            <asp:Label runat="server" style="float:right;" Text="Assigned roles:" meta:resourcekey="AssignedRoles"></asp:Label>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td style="text-align:right;padding-bottom:1em;padding-right:1em;max-width:100%;min-height:10%;" colspan="4">
                            <asp:GridView runat="server" id="RolesTable" style="width:100%;border:solid;border-width:1px;margin-left:auto;margin-right:auto;"
                                CssClass="alternating-table-color gridview table-responsive" AutoGenerateColumns="false" OnRowDataBound="RolesTable_RowDataBound" Enabled="<%# ReadOnly==false %>">
                                <Columns>
                                    <asp:BoundField HeaderText="Role" HeaderStyle-Font-Bold="true" ItemStyle-Width="75%" DataField="Name" meta:resourcekey="Role"></asp:BoundField>
                                    <asp:TemplateField ItemStyle-Width="25%" HeaderStyle-Font-Bold="true" HeaderText="Allow" meta:resourcekey="Allow">
                                        <ItemTemplate>
                                            <input type="checkbox" id="UserRole" runat="server" style="zoom:0.7;"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
