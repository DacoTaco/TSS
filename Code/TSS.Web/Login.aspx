<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TSS.Web.Login" EnableEventValidation = "false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="CSS/bootstrap.css" rel="stylesheet" type="text/css"/>
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
<form id="form1" runat="server" defaultbutton="btnClose">
    <div class="centerPage container-fluid" id="MainContainer" style="background-color: lightblue; border: solid; border-width: 2px;">
        <div style="margin-bottom: 4em;">
            <asp:button runat="server" ID="btnClose" Text="X" CssClass="btn btn-close" style="text-align: center;" OnClientClick="returnToIndex();return false;"/>
        </div>
        <!-- next div is required for our javascript/ajax script! -->
        <div id="Login" class="container-fluid" style="width: 100%;">
            <h1 style="margin-bottom: 10px; width: 100%;">
                <asp:Label runat="server" Text="Login as" meta:resourcekey="LoginAs"></asp:Label>
            </h1>
            <asp:GridView ID="userlist" runat="server" AllowPaging="False" AutoGenerateColumns="False" CellPadding="10" CellSpacing="5" CssClass="gridview UserTable table table-responsive"
                          ShowHeader="false" OnRowDataBound="userlist_RowDataBound" RowStyle-HorizontalAlign="Center" BorderWidth="2px"
                          style="background-color: white; margin: 1vw; margin-bottom: auto; margin-left: auto; margin-right: auto; margin-top: auto; width: 50vmin;">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True" SortExpression="Login" meta:resourcekey="Login"></asp:BoundField>
                </Columns>
            </asp:GridView>
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