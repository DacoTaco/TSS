<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="TSS_ASPWebForms.EditUser" %>
<%@ Register assembly="TSS_ASPWebForms" namespace="TSS_ASPWebForms" tagprefix="web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>TSS : Technical Service System</title>
        <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
        <link href="CSS/Style.css" rel="stylesheet" type="text/css"/>
        <link href="CSS/EditUser.css" rel="stylesheet" type="text/css"/>
        <script src="Scripts/jquery-3.1.1.js"></script>
        <script src="Scripts/bootstrap.js"></script>
        <script src="Scripts/exif.js"></script>
        <script src="Scripts/TSS.js"></script>
        <script src="Scripts/TSS-EditUser.js"></script>

        <meta name="viewport" content="initial-scale=1.0, user-scalable=no"/>
        <meta name="google" content="notranslate"/>
    </head>
    <body>
        <div class="centerPage container-fluid" id="MainContainer" style="background-color: lightblue; border: solid; border-width: 2px;">
            <form id="EditUserForm" runat="server" autocomplete="off">
                <div id="EditUser">
                    <div id="loadingUserDiv" class="blocker" hidden="hidden" runat="server" style="z-index: 999;">
                        <div class="blocker-content">
                            <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/system/loading.gif" style="height: auto; width: 3em; z-index: 9999;"/>
                        </div>
                    </div>
                    <script type="text/javascript" id="ReadOnly">
                        function myFunction() {
                            if ('True' == '<%# ReadOnly %>')
                                alert('<%# ReadOnlyMsg %>');
                        }
                    </script>
                    <table style="margin-bottom: 10px; margin-left: 10%; margin-right: 10%; max-height: 100%; table-layout: fixed; width: 80%;">
                        <tr>
                            <!-- amount of columns -->
                            <th style="width: 25%;"/>
                            <th style="width: 25%;"/>
                            <th style="width: 25%;"/>
                            <th style="width: 25%;"/>
                        </tr>
                        <tr>
                            <td style="height: 8em; margin-left: auto; margin-right: auto; max-height: 400px; padding-bottom: 1em; padding-right: 1em; text-align: right; text-align: center; width: auto;" colspan="4">
                                <asp:Image id="UserPhoto" runat="server" ImageUrl="./system/DefaultUser.jpg" AlternateText="User Image" style="height: inherit; max-height: inherit; max-width: inherit; width: inherit;"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="margin-left: auto; margin-right: auto; padding-bottom: 1em; padding-right: 1em; text-align: right; text-align: center; width: auto;" colspan="4">
                                <button runat="server" id="addPicture" type="button" onclick="$('#ProfilePicInput').click(); return false;" style="height: 100%; min-height: 2.5em;" disabled="<%# ReadOnly %>">
                                    <asp:Label runat="server" Text="Add Picture" meta:resourcekey="AddPicture"/>
                                </button>
                                <input id="ProfilePicInput" runat="server" type="file" accept="image/*" capture="camera" onchange="AddUserPhoto(this); return false;" style="visibility: hidden" disabled="<%# ReadOnly %>"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <asp:Label runat="server" style="float: right;" Text="Username :" meta:resourcekey="UserName"/>
                            </td>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;" colspan="3">
                                <input id="txtUsername" runat="server" type="text" autocomplete="off" style="width: 100%;" value="<%# EdittedUser.UserName %>" onblur="updateUsername(); return false;" disabled="<%# ReadOnly %>"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <asp:Label runat="server" style="float: right;" Text="Password :" meta:resourcekey="Password"/>
                            </td>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;" colspan="3">
                                <input id="txtPassword" runat="server" type="password" autocomplete="off" style="width: 100%;" onblur="updatePassword(); return false;" disabled="<%# ReadOnly %>"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <asp:Label runat="server" style="float: right;" Text="Department :" meta:resourcekey="Department"/>
                            </td>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <web:DropDownObject runat="server" id="selectDepartment" style="width: 100%;" DataValueField="ID" DataTextField="Description" onchange="updateDepartment(); return false;" Enabled="<%# ReadOnly == false %>"/>
                            </td>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <asp:Label runat="server" style="float: right;" Text="User Enabled :" meta:resourcekey="UserEnabled"/>
                            </td>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                                <input type="checkbox" runat="server" id="UserActive" style="float: left" checked="<%# EdittedUser.IsActive %>" onchange="updateActive(); return false;" disabled="<%# ReadOnly %>"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;" colspan="1">
                                <asp:Label runat="server" style="float: right;" Text="Assigned roles:" meta:resourcekey="AssignedRoles"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="max-width: 100%; min-height: 10%; padding-bottom: 1em; padding-right: 1em; text-align: right;" colspan="4">
                                <asp:GridView runat="server" id="RolesTable" style="border: solid; border-width: 1px; margin-left: auto; margin-right: auto; width: 100%;"
                                              CssClass="alternating-table-color gridview table-responsive" AutoGenerateColumns="false" OnRowDataBound="RolesTable_RowDataBound" Enabled="<%# ReadOnly == false %>">
                                    <Columns>
                                        <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                            <HeaderTemplate>
                                                <asp:Label runat="server" ID="RoleNameColumn" Text="Role" meta:resourcekey="Role" Font-Bold="True"/>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <web:RoleLabel runat="server" Width="75%" ID="UserRoleName" Text="Role"/>
                                            </ItemTemplate>
                                        </asp:TemplateField>              
                                        <asp:TemplateField ItemStyle-Width="25%" HeaderStyle-Font-Bold="true" HeaderText="Allow" meta:resourcekey="Allow">
                                            <ItemTemplate>
                                                <input type="checkbox" id="UserRole" runat="server" style="zoom: 0.7;"/>
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
