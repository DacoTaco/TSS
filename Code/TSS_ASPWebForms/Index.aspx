<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="TSS_ASPWebForms.index" %>
<%@ Register assembly="TechnicalServiceSystem.UI" namespace="TechnicalServiceSystem.UI.HTML" tagprefix="web" %>
<%@ Import Namespace="TechnicalServiceSystem" %>
<%@ Import Namespace="TechnicalServiceSystem.Entities.Users" %>
<%@ Import Namespace="Equin.ApplicationFramework" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/EditTask.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-EditTask.js"></script>
    <script src="Scripts/TSS-EditUser.js"></script>
    <script src="Scripts/TSS-Index.js"></script>
    <link href="CSS/lightbox.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/lightbox.js"></script>
    <!-- script to set the bootstrap tab again -->
    <script type="text/javascript">
        function setTab() {
            var url = document.location.toString();
            if (url.match('#')) {
                $('#TabControl a[href="#' + url.split('#')[1] + '"]').tab('show');
            } //add a suffix
            else {
                var tab = document.getElementById('<%= hidTABControl.ClientID %>').value;
                $('#TabControl a[href="' + tab + '"]').tab('show');
            }
        }

        $(document).ready(setTab);
    </script>
    <meta name="viewport" content="width=device-width,user-scalable=no"/>
    <meta name="google" content="notranslate"/>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:HiddenField ID="hidTABControl" runat="server" Value="#Tasks"/>
        <!-- loading div -->
        <div id="loadingDiv" class="blocker" hidden="hidden" runat="server" style="z-index:999;">
            <div class="blocker-content">
                <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="/system/loading.gif" style="width:3em;height:auto;z-index:9999;"/>
            </div>
        </div>
        <div class="row" id="TopBar" style="margin-left: 5%; margin-right: 3%;">
            <div class="col-xs-9 col-sm-10 col-lg-11" style="margin-top: 1%; text-align: right;">
                <asp:Label ID="lblUserName" runat="server"/>
                <!--Browser Size:<br>
                <div id="size"></div>
                <br><br>
                Screen Size
                <div id="size2"></div>
                <script>
                        $(window).on('resize', showSize);

                        showSize();

                        function showSize() {
                            $('#size').html('HEIGHT : ' + $(window).height() + '<br>WIDTH : ' + $(window).width());
                            $('#size2').html('HEIGHT : ' + screen.height + '<br>WIDTH : ' + screen.width);
                        }
                    </script>
                <span class="ScreenClassSpan"></span>-->
                <asp:Label runat="server" id="conID"></asp:Label>
            </div>
            <div class="col-xs-3 col-sm-2 col-lg-1 dropdown" id="UserMenu">
                <div class=" UserButton" style="height: 100%; width: 100%;">
                    <button class="btn btn-default btn-lg btn-xs dropdown-toggle" id="profileBtn" type="button" data-toggle="dropdown" style="background-color: white; border-color: transparent; height: auto; width: 100%;">
                        <asp:Image runat="server" ID="userImage" CssClass="img" AlternateText="userImage" style="height: inherit; width: inherit;"/>
                        <span class="caret" style="color: darkgray; display: inline-block; float: right; margin-top: 5px"></span>
                    </button>
                    <ul role="menu" class="dropdown-menu text-responsive dropdown-menu-right" aria-labelledby="UserMenu" style="max-width: 10em; min-width: 10em;">
                        <li id="ProfileMenu" runat="server">
                            <a role="menuitem">
                                <asp:Button runat="server" BackColor="Transparent" BorderStyle="None" Text="Profile" Width="100%" style="min-height: 0.4em; text-align: left;" UseSubmitBehavior="false"></asp:Button>
                            </a>
                        </li>
                        <li role="separator" class="divider" runat="server" id="LoginSeperator"></li>
                        <li>
                            <a role="menuitem">
                                <asp:Button runat="server" ID="LoginMenu" BackColor="Transparent" BorderStyle="None" OnClick="LoginMenu_Click"
                                            style="min-height: 0.2em; text-align: left; width: 100%;" UseSubmitBehavior="false">

                                </asp:Button>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
             
        <!-- Editing Model , needs to be outside the main container or it will not close decently -->
        <div class="modal fade" id="EditWindow" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" style="margin-left: auto; margin-right: auto; max-width: 50em; width: 98%;">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="return false;">×</button>
                        <h4 class="modal-title"></h4>
                    </div>
                    <div class="modal-body" id="editModal" style="margin: 0%; margin-left: auto; margin-right: auto; padding: 3px;">
                        <!--data goes here -->
                    </div>
                    <div class="modal-footer">
                        <asp:button id="SaveModal" type="button" data-dismiss="modal" runat="server" Text="Save changes" OnClientClick="Save();CloseModal();return false;" meta:resourcekey="Save"/>
                        <asp:button id="CloseBtn" type="button" data-dismiss="modal" runat="server" Text="Close" OnClientClick="CloseModal();return false;" meta:resourcekey="Close"/>
                    </div>
                </div><!-- /.modal-content -->
            </div><!-- /.modal-dialog -->
        </div><!-- /.modal -->
        <!-- the maincontainer that has everything starting with the tabcontrol -->
        <div class="container-fluid col-xs-12" id="MainContainer" style="margin-left: auto; margin-right: auto; text-align: center;">
        
            <!-- declaration of the tab controls. tabs that need to be toggled hidden or not need to have an ID and runat so we can hide it or not -->
            <!-- fun fact, if visability is toggled in code behind, its content is not visable in HTML, so no exploitation :D -->
            <ul class="nav nav-tabs" id="TabControl">
                <li style="border-bottom: none" id="TasksTab" runat="server" onclick="SetActiveTab('#Tasks');">
                    <a data-toggle="tab" href="#Tasks">
                        <asp:Label Text="Tasks" meta:resourcekey="Tasks" runat="server"/>
                    </a>
                </li>
                <li id="MachinesTab" runat="server" style="border-bottom: none" onclick="SetActiveTab('#Machines');">
                    <a href="#Machines" data-toggle="tab">
                        <asp:Label Text="Machines" meta:resourcekey="Machines" runat="server"/>
                    </a>
                </li>
                <li id="SuppliersTab" runat="server" style="border-bottom: none" onclick="SetActiveTab('#Suppliers');">
                    <a href="#Suppliers" data-toggle="tab">
                        <asp:Label Text="Suppliers" meta:resourcekey="Suppliers" runat="server"/>
                    </a>
                </li>
                <li id="UsersTab" runat="server" style="border-bottom: none" onclick="SetActiveTab('#Users');">
                    <a href="#Users" data-toggle="tab">
                        <asp:Label Text="Users" meta:resourcekey="Users" runat="server"/>
                    </a>
                </li>
            </ul>
            
            <!-- container containing the tab's content -->
            <div class="tab-content" id="TabContent"
                 style="align-content: center; align-items: center; border-bottom: 1px solid #ddd; border-left: 1px solid #ddd; border-right: 1px solid #ddd; border-top: none; margin-left: auto; margin-right: auto; max-width: 100%; padding-top: 10px; text-align: center;">

                <!-- Tasks tab -->
                <div id="Tasks" class="tab-pane fade in active" style="border: none !important;" runat="server">
                    <h1>
                        <asp:Label Text="Tasks" meta:resourcekey="Tasks" runat="server"></asp:Label>
                    </h1>
                     <!-- the top bar of the tasks tab. this has the sorting and buttons and stuff -->
                    <div class="row option-row">
                        <div class="col-xs-3 col-lg-3" style="height: inherit; margin-left: 0.5em; margin-right: 0.4em; padding: 0px;">
                            <button runat="server" name="btnNewTask" class="lg-float-right md-button-max-width btn-block"
                                    style="height: 100%; min-height: 1em; text-align: center;" onclick="loadTaskPage(0);return false;">
                                <asp:Label runat="server" Text="New Task" meta:resourcekey="NewTask" style="height: inherit"></asp:Label>
                            </button>
                        </div>
                        <div class="col-xs-3 col-lg-3" style="display: table-row; height: inherit; margin-right: 0.4em; padding: 0px;">
                            <asp:Label runat="server" Text="Afdeling : " style="float: left; height: inherit; margin-left: 15%; min-height: inherit; position: relative; text-align: center; vertical-align: central;"
                                       CssClass="hidden-xs hidden-sm visible-lg visible-md">
                            </asp:Label>
                            <select runat="server" id="DropDownSorting" DataValueField="ID" DataTextField="Description" style="height: inherit; margin-left: 1em; margin-right: 1em; min-height: 1em; width: 90%;"
                                    class="xs-float-left md-float-left md-dropdown-max-width btn-block" onchange="onDepartmentChanged()">
                            </select>
                        </div>
                        <div class="col-xs-4 col-lg-3" style="height: inherit; margin-right: 0.4em; padding: 0px;">
                            <asp:TextBox runat="server" ID="searchbar" class="md-float-center" style="height: 100%; min-height: 1em; min-width: 1em; width: 100%;" AutoCompleteType="Disabled"
                                         onkeypress="onSearchKeyEnter(event,searchTaskPage);" AutoPostBack="false">
                            </asp:TextBox>
                        </div>
                        <div class="col-xs-1 col-lg-2" style="height: inherit; padding: 0px;">
                            <button runat="server" name="btnSearch" class="searchButton xs-float-right sm-float-left md-float-left lg-float-left" onclick="getTasksPage();return false;"
                                    style="height: 100%; max-height: 100%; max-width: 100%; text-align: center;">
                                <asp:Label runat="server" Text="Search" meta:resourcekey="Search" CssClass="hidden-xs hidden-sm"></asp:Label>
                                <asp:Image runat="server" CssClass="img hidden-lg hidden-md" AlternateText="Search" ImageUrl="./system/search.png" BackColor="Transparent"
                                           style="background-size: contain; border: none; max-height: inherit; max-width: inherit; text-align: center;"/>
                            </button>
                        </div>
                    </div>
                    <!-- the grid ! -->
                    <!--  DataSourceID="TaskSource"  -->
                    <asp:GridView runat="server" ID="TaskGrid" DataKeyNames="ID" AutoGenerateColumns="False" OnRowDataBound="TaskGrid_RowDataBound"
                                  CssClass="alternating-table-color gridview table table-responsive" RowStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White"
                                  HorizontalAlign="Center" AllowSorting="True" HeaderStyle-BorderColor="Black" HeaderStyle-CssClass="gridview-header"
                                  ShowHeaderWhenEmpty="True" style="font-size: 2vmin; margin-left: auto; margin-right: auto; margin-top: 1vw; max-width: 100%" BackColor="White" BorderColor="#868686"
                                  RowStyle-CssClass="text-responsive">
                        <Columns>
                            <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" SortExpression="Description" meta:resourcekey="Description" ItemStyle-CssClass="gridview-align-left">
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Urguency" meta:resourcekey="Urguent" SortExpression="Urguent" ControlStyle-Width="6.4em">
                                <ItemTemplate>
                                    <asp:CheckBox ID="urguentchkbx" runat="server" Checked='<%# Bind("IsUrguent") %>' style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Reporter" HeaderText="Reporter" ReadOnly="true" SortExpression="Reporter" meta:resourcekey="Reporter" ItemStyle-CssClass="hidden-xs" HeaderStyle-CssClass="hidden-xs">
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="Technician.UserName" HeaderText="Technician" meta:resourcekey="Technician" SortExpression="TechnicianName" ItemStyle-CssClass="hidden-xs hidden-sm" HeaderStyle-CssClass="hidden-xs hidden-sm">
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="Department" HeaderText="Department" meta:resourcekey="Department" SortExpression="DepartmentID">
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="Location" HeaderText="Location" meta:resourcekey="Location" SortExpression="LocationName">
                            </asp:BoundField>
                            
                            <asp:TemplateField HeaderText="Status" meta:resourcekey="Status" SortExpression="StatusID" HeaderStyle-CssClass="hidden-xs hidden-sm" ItemStyle-CssClass="hidden-xs hidden-sm">
                                <ItemTemplate><%# GetTranslation("TaskStatus",(int) Eval("StatusID")) %></ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="LastModifiedOn" HeaderText="Last Adjustments" ReadOnly="true" SortExpression="LastModifiedOn" meta:resourcekey="LastAdjustments"
                                            ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                            </asp:BoundField>

                        </Columns>
                    </asp:GridView>

                    <!-- Data source! -->
                    <asp:ObjectDataSource ID="TaskSource" runat="server"
                        TypeName="<%# typeof(TechnicalServiceSystem.TaskManager) %>" 
                        SelectMethod ="<%# nameof(TechnicalServiceSystem.TaskManager.GetTasks) %>"
                        SortParameterName="SortBy" DataObjectTypeName="<%# nameof(TechnicalServiceSystem.Entities.Tasks) %>">
                        <SelectParameters>        
                            <asp:Parameter Name="SearchText" DefaultValue="" Type="String"/>
                            <asp:Parameter Name="DepartmentID" DefaultValue="-1" Type="Int32"/>
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
                
                <!-- Users tab -->
                <div id="Users" class="tab-pane fade in" style="border: none !important;" runat="server">
                    <h1>
                        <asp:Label Text="Users" meta:resourcekey="Users" runat="server"></asp:Label>
                    </h1>
                    <div class="row option-row">
                        <div class="col-xs-3 col-lg-3" style="height: inherit; margin-left: 0.5em; margin-right: 0.4em; padding: 0px;">
                            <button runat="server" name="btnNewUser" class="lg-float-right md-button-max-width btn-block"
                                    style="height: 100%; min-height: 1em; text-align: center;" onclick="loadUserPage(0);return false;">
                                <asp:Label runat="server" Text="New User" meta:resourcekey="NewUser" style="height: inherit"></asp:Label>
                            </button>
                        </div>
                        <div class="col-xs-3 col-lg-3" style="height: inherit; margin-right: 0.4em; padding: 0px;">
                            <select runat="server" id="selectUserType" DataValueField="ID" DataTextField="RoleName" class="xs-float-left md-float-center md-dropdown-max-width btn-block"
                                    style="height: inherit; min-height: 1em;" onchange="onUserTypeChanged()">
                            </select>
                        </div>
                        <div class="col-xs-4 col-lg-3" style="height: inherit; margin-right: 0.4em; padding: 0px;">
                            <asp:TextBox runat="server" ID="txtSearchUser" class="md-float-center" style="height: 100%; min-height: 1em; min-width: 1em; width: 100%;" AutoCompleteType="Disabled"
                                         onkeypress="onSearchKeyEnter(event,SearchUserPage);" AutoPostBack="false">
                            </asp:TextBox>
                        </div>
                        <div class="col-xs-1 col-lg-2" style="height: inherit; padding: 0px;">
                            <button runat="server" name="btnSearchUser" class="searchButton xs-float-right sm-float-left md-float-left lg-float-left" onclick="getUserPage();return false;"
                                    style="height: 100%; max-height: 100%; max-width: 100%; text-align: center;">
                                <asp:Label runat="server" Text="Search" meta:resourcekey="Search" CssClass="hidden-xs hidden-sm"></asp:Label>
                                <asp:Image runat="server" CssClass="img hidden-lg hidden-md" AlternateText="Search" ImageUrl="./system/search.png" BackColor="Transparent"
                                           style="background-size: contain; border: none; max-height: inherit; max-width: inherit; text-align: center;"/>
                            </button>
                        </div>
                    </div>
                    
                    <asp:GridView runat="server" ID="UserGrid" AutoGenerateColumns="False" DataKeyNames="ID" OnRowDataBound="UserGrid_RowDataBound"
                                  CssClass="gridview table table-responsive alternating-table-color" RowStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White"
                                  HorizontalAlign="Center" AllowSorting="True" HeaderStyle-BorderColor="Black" HeaderStyle-CssClass="gridview-header"
                                  ShowHeaderWhenEmpty="True" style="font-size: 2vmin; margin-left: auto; margin-right: auto; margin-top: 1vw; max-width: 100%" BackColor="White" BorderColor="#868686"
                                  RowStyle-CssClass="text-responsive">
                        <Columns>
                            <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True" SortExpression="Username" meta:resourcekey="UserName"/>
                            <asp:CheckBoxField HeaderText="Active" meta:resourcekey="Active" SortExpression="IsActive" DataField="IsActive" ReadOnly="false" ControlStyle-Width="6.4em"/>
                            <asp:BoundField DataField="Department" HeaderText="Department" meta:resourcekey="Department" SortExpression="Department"/>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="AdminColumn" Text="Admin" TranslationKey="1"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleAdmin" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 1) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="UserColumn" Text="User" TranslationKey="2"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleUser" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 2) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="TechnicianColumn" Text="Technician" TranslationKey="3"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleTechnician" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 3) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="UserManagerColumn" Text="User Manager" TranslationKey="4"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleUserManager" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 4) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="TaskManagerColumn" Text="Supplier Manager" TranslationKey="5"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleTaskManager" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 5) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                                <HeaderTemplate>
                                    <web:RoleLabel runat="server" ID="SupManagerColumn" Text="Supplier Manager" TranslationKey="6"/>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="UserRoleSupManager" runat="server"
                                                  Checked='<%# RoleManager.UserHasRole(((ObjectView<User>)Container.DataItem).Object, 6) %>'
                                                  style="zoom: 0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    
                    <!-- User Data source! -->
                    <asp:ObjectDataSource ID="UserSource" runat="server" 
                        TypeName="<%# typeof(TechnicalServiceSystem.UserManager) %>" 
                        SelectMethod ="<%# nameof(TechnicalServiceSystem.UserManager.GetUsers) %>"
                        SortParameterName="SortBy" DataObjectTypeName="<%# nameof(TechnicalServiceSystem.Entities.Users) %>">
                        <SelectParameters>
                            <asp:Parameter Name="contains" DefaultValue="" Type="String"></asp:Parameter>
                            <asp:Parameter Name="RoleID" DefaultValue="-1" Type="Int32"></asp:Parameter>
                            <asp:Parameter Name="activeOnly" DefaultValue="" Type="Boolean" ConvertEmptyStringToNull="True"/>
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
            </div>
        </div>
        <div class="navbar navbar-fixed-bottom" style="text-align:center;width:auto;max-width:8em;margin-left:auto;margin-right:auto;">
            <asp:Label runat="server" Font-Size="0.8em">&copy; DacoTaco 2017</asp:Label>
        </div>
    </form>
</body>
</html>
