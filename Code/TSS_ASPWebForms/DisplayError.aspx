<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayError.aspx.cs" Inherits="TSS_ASPWebForms.DisplayError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>

    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <meta name="google" content="notranslate" />
</head>
<body>
    <form id="form1" runat="server">
    <div class ="centerPage container" id="MainContainer" style="font-size:3em;" >
        <asp:label id="lblErrorMessage" runat="server"></asp:label>
    </div>
    </form>
    <div id="footer" class="navbar navbar-fixed-bottom" style="text-align:center" >
        <p>&copy; DacoTaco 2017</p>
    </div>
</body>
</html>
