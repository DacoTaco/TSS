<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginUser.aspx.cs" Inherits="TSS_ASPWebForms.LoginUser" EnableEventValidation = "false" ClientIDMode="AutoID" %>
<%@ Register assembly="TechnicalServiceSystem.UI" namespace="TechnicalServiceSystem.UI.HTML" tagprefix="web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-login.js"></script>
    <!-- disables zooming in, but makes everything huge?!-->
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no"/>
    <meta name="google" content="notranslate"/>
    <!-- for android > 4.4 ... -->
    <!--<meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=no" />-->
</head>
<body id="body">
<form id="form1" runat="server">
    <!-- next div is required for our javascript/ajax script! -->
    <div class="centerPage container-fluid" id="MainContainer" style="background-color: lightblue; border: solid; border-width: 2px;">
        <div id="Login" class="container-fluid" style="width: 100%">
            <!-- add margin top cause some old browsers dislike it when its not there xD -->
            <table class="UserTable" style="margin-bottom: 10px; margin-left: -10%; margin-right: auto; margin-top: 10%; table-layout: fixed; width: 100%;">
                <tr>
                    <th></th>
                    <th></th>
                </tr>
                <tr>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right;">
                        <asp:Label runat="server" Text="Login as : " ID="lblLogin" meta:resourcekey="LoginAs"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left;">
                        <web:DropDownObject runat="server" ID="userList" DataValueField="ID" DataTextField="UserName" style="height: 2em; max-height: inherit; max-width: 20em; min-height: 5px; text-align: center; width: 120%;"/>
                    </td>
                </tr>
                <tr>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: right; width: 50%;">
                        <asp:Label runat="server" Text="Password : " meta:resourcekey="Password"></asp:Label>
                    </td>
                    <td style="padding-bottom: 1em; padding-right: 1em; text-align: left; Width: 50%;">
                        <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" AutoCompleteType="Disabled" style="height: 2em; max-height: inherit; max-width: 20em; min-height: 5px; width: 120%;"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br/><br/>
            <asp:Button runat="server" ID="btnLogin" style="height: 4em; margin-left: 10px; min-height: 25px; width: 6em" Text="Login" OnClientClick="login();return false;" UseSubmitBehavior="false" meta:resourcekey="Login"/>
        </div>
    </div>
    <!-- loading div -->
    <div id="loadingDiv" class="blocker" hidden="hidden">
        <div class="blocker-content">
            <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="/system/loading.gif" style="height: auto; width: 3em;"/>
        </div>
    </div>
</form>
<div id="footer" class="navbar navbar-fixed-bottom" style="text-align: center">
    <p>&copy; DacoTaco 2017</p>
</div>
</body>
</html>