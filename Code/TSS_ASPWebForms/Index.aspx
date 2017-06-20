<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="TSS_ASPWebForms.Index" EnableEventValidation = "false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TSS : Technical Service System</title>
    <link href="Content/bootstrap.css" rel="stylesheet" type="text/css"/>
    <link href="CSS/Style.css" rel="stylesheet" type="text/css" />
    <link href="CSS/EditTask.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-3.1.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/TSS.js"></script>
    <script src="Scripts/TSS-EditTask.js"></script>
    <script src="Scripts/TSS-EditUser.js"></script>
    <script src="Scripts/TSS-Index.js"></script>
    <link href="CSS/lightbox.css" rel="stylesheet" type="text/css"/>
    <script src="Scripts/lightbox.js"></script>
    <!-- script to set the bootstrap tab again -->
    <!-- TODO : talk this over with tom -->
    <script type="text/javascript">
        function setTab() 
        {
            var url = document.location.toString();
            if (url.match('#')) {
                $('#TabControl a[href="#' + url.split('#')[1] + '"]').tab('show');
            } //add a suffix
            else
            {
                var tab = document.getElementById('<%= hidTABControl.ClientID%>').value;
                $('#TabControl a[href="' + tab + '"]').tab('show');
            } 
        }
        $(document).ready(setTab);
    </script>
    <meta name="viewport" content="width=device-width,user-scalable=no" />
    <meta name="google" content="notranslate" />
</head>
<body class="" style="">
    <!-- the form -->
    <form id="form1" runat="server" autocomplete="off">
    <asp:HiddenField ID="hidTABControl" runat="server" Value="#Tasks" />
    <!-- The top bar. Profile button etc -->
    <div class="row" id="TopBar" style="margin-right:3%;margin-left:5%">
        <div class="col-xs-9 col-sm-10 col-lg-11" style="text-align:right;margin-top:1%">
            <asp:Label ID="lblUserName" runat="server" />
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
            <div class=" UserButton" style="width:100%;height:100%;">
                <button class="btn btn-default btn-lg btn-xs dropdown-toggle" id="profileBtn" type="button" data-toggle="dropdown" style="background-color:white;border-color:transparent;height:auto;width:100%;">
                    <asp:Image runat="server" ID="userImage"  CssClass="img" AlternateText="userImage" style="height:inherit;width:inherit;"/>
                    <span class="caret" style="display:inline-block;float:right;color:darkgray;margin-top:5px"></span>
                </button>
                <ul role="menu" class="dropdown-menu text-responsive dropdown-menu-right" aria-labelledby="UserMenu" style="min-width:10em;max-width:10em;">
                    <li id="ProfileMenu" runat="server"><a role="menuitem"><asp:Button runat="server" BackColor="Transparent" BorderStyle="None" Text="Profile" Width="100%" style="text-align:left;min-height:0.4em" UseSubmitBehavior="false" ></asp:Button></a></li>
                    <li role="separator" class="divider" runat="server" id="LoginSeperator"></li>
                    <li><a role="menuitem">
                        <asp:Button runat="server" ID="LoginMenu" BackColor="Transparent" BorderStyle="None" OnClick="LoginMenu_Click"
                            style="text-align:left;min-height:0.2em;width:100%;" UseSubmitBehavior="false">

                        </asp:Button>
                    </a></li>
                </ul>
            </div>
        </div>  
    </div>

    <!-- the maincontainer that has everything starting with the tabcontrol -->
    <div class="container-fluid col-xs-12" id="MainContainer" style="text-align:center;margin-left:auto;margin-right:auto;">

        <!-- declaration of the tab controls. tabs that need to be toggled hidden or not need to have an ID and runat so we can hide it or not -->
        <!-- fun fact, if visability is toggled in code behind, its content is not visable in HTML, so no exploitation :D -->
        <ul class="nav nav-tabs" id="TabControl">
          <li style="border-bottom:none" id="TasksTab" runat="server" onclick="SetActiveTab('#Tasks');"><a data-toggle="tab" href="#Tasks"><asp:Label Text="Tasks" meta:resourcekey="Tasks" runat="server"/></a></li>
          <li id="MachinesTab" runat="server" style="border-bottom:none" onclick="SetActiveTab('#Machines');"><a href="#Machines" data-toggle="tab"><asp:Label Text="Machines" meta:resourcekey="Machines" runat="server"/></a></li>
          <li id="SuppliersTab" runat="server" style="border-bottom:none" onclick="SetActiveTab('#Suppliers');"><a href="#Suppliers" data-toggle="tab"><asp:Label Text="Suppliers" meta:resourcekey="Suppliers" runat="server"/></a></li>
          <li id="UsersTab" runat="server" style="border-bottom:none" onclick="SetActiveTab('#Users');"><a href="#Users" data-toggle="tab"><asp:Label Text="Users" meta:resourcekey="Users" runat="server"/></a></li>
        </ul>

        <!-- Editing Model -->
        <div class="modal fade" id="EditWindow" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" style="width:98%;margin-left:auto;margin-right:auto;max-width:50em">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="return false;">×</button>
                        <h4 class="modal-title"></h4>
                    </div>
                    <div class="modal-body" id="editModal" style="padding:3px;margin:0%;margin-left:auto;margin-right:auto;">
                        <!--data goes here -->
                    </div>
                    <div class="modal-footer">
                        <asp:button id="CloseBtn" type="button" data-dismiss="modal" runat="server" Text="Close" OnClientClick="CloseModal(); return false;" meta:resourcekey="Close"></asp:button>
                        <asp:button id="SaveModal" type="button" Text="Save changes" runat="server" OnClientClick="Save(); CloseModal(); return false;" meta:resourcekey="Save"/>
                    </div>
                </div><!-- /.modal-content -->
            </div><!-- /.modal-dialog -->
        </div><!-- /.modal -->

        <!-- container containing the tab's content -->
        <div class="tab-content"  id="TabContent"
            style="text-align:center;align-content:center;align-items:center;
                border-left:1px solid #ddd;border-right:1px solid #ddd;border-bottom:1px solid #ddd;border-top:none;
                padding-top:10px;margin-left:auto;margin-right:auto;
                max-width:100%;">

            <!-- Tasks tab -->
            <div id="Tasks" class="tab-pane fade in active" style="border:none!important;" runat="server">
                <h1><asp:Label Text="Tasks" meta:resourcekey="Tasks" runat="server"></asp:Label></h1>

                <!-- the top bar of the tasks tab. this has the sorting and buttons and stuff -->
                <div class="row option-row">
                    <div class="col-xs-3 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;margin-left:0.5em">
                        <button runat="server" name="btnNewTask" class="lg-float-right md-button-max-width btn-block"
                        style="text-align: center; height:100%;min-height: 1em;" onclick="loadTaskPage(0);return false;">
                            <asp:Label runat="server" Text="New Task" meta:resourcekey="NewTask" style="height:inherit"></asp:Label>
                        </button>
                    </div>
                    <div class="col-xs-3 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;display:table-row">
                        <asp:Label runat="server" Text="Afdeling : " style="float:left;text-align:center;vertical-align:central;min-height:inherit;height:inherit;position:relative;margin-left:15%;" 
                            CssClass="hidden-xs hidden-sm visible-lg visible-md"></asp:Label>
                        <select runat="server" id="DropDownSorting" DataValueField="ID" DataTextField="Name" style="min-height:1em;height:inherit;width:90%;margin-left:1em;margin-right:1em" 
                                class="xs-float-left md-float-left md-dropdown-max-width btn-block" onchange="onDepartmentChanged()"></select>
                    </div>
                    <div class="col-xs-4 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;">
                        <asp:TextBox runat="server" ID="searchbar" class="md-float-center" style="width:100%;height:100%;min-width:1em;min-height:1em;" AutoCompleteType="Disabled"
                        onkeypress='onSearchKeyEnter(event,searchTaskPage);' AutoPostBack="false"> </asp:TextBox>
                    </div>
                    <div class="col-xs-1 col-lg-2" style="height:inherit;padding:0px;">
                        <button runat="server" name="btnSearch" class="searchButton xs-float-right sm-float-left md-float-left lg-float-left" onclick="getTasksPage();return false;" 
                        style="height:100%;text-align:center;max-height:100%; max-width:100%;">
                            <asp:Label runat="server" Text="Search" meta:resourcekey="Search" CssClass="hidden-xs hidden-sm"></asp:Label>
                            <asp:Image runat="server" CssClass="img hidden-lg hidden-md" AlternateText="Search" ImageUrl="./system/search.png" BackColor="Transparent" 
                            style="border:none;background-size: contain;text-align:center;max-height:inherit;max-width:inherit;"/>
                        </button>
                    </div>
                </div>
                <!-- the grid ! -->
                <asp:GridView runat="server" ID="TaskView" DataSourceID="TaskSource" AutoGenerateColumns="False" DataKeyNames="ID"
                CssClass="alternating-table-color gridview table table-responsive" RowStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White"
                OnRowDataBound="TaskView_RowDataBound" HorizontalAlign="Center" AllowSorting="True" HeaderStyle-BorderColor="Black" HeaderStyle-CssClass="gridview-header"
                ShowHeaderWhenEmpty="True" style="margin-top:1vw;font-size:2vmin;margin-left:auto;margin-right:auto;max-width:100%" BackColor="White" BorderColor="#868686" 
                RowStyle-CssClass="text-responsive" >
                    <Columns>
                        <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" SortExpression="Description" meta:resourcekey="Description" ItemStyle-CssClass="gridview-align-left">
                        </asp:BoundField>

                        <asp:TemplateField HeaderText="Urguency" meta:resourcekey="Urguent" SortExpression="Urguent" ControlStyle-Width="6.4em">
                            <ItemTemplate>
                                <asp:CheckBox ID="urguentchkbx" runat="server" Checked='<%# Bind("Urguent") %>' style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Task Type" meta:resourcekey="TaskType" SortExpression="TaskTypeID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.TaskTypes, (int)Eval("TaskTypeID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Reporter" HeaderText="Reporter" ReadOnly="true" SortExpression="Reporter" meta:resourcekey="Reporter"  ItemStyle-CssClass="hidden-xs" HeaderStyle-CssClass="hidden-xs" >
                        </asp:BoundField>

                        <asp:TemplateField HeaderText="Technician" meta:resourcekey="Technician" SortExpression="TechnicianID" ItemStyle-CssClass="hidden-xs hidden-sm" HeaderStyle-CssClass="hidden-xs hidden-sm">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Technicians, (int)Eval("TechnicianID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Department" meta:resourcekey="Department" SortExpression="DepartmentID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Departments, (int)Eval("DepartmentID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Location" meta:resourcekey="Location" SortExpression="LocationID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Locations,(int)Eval("LocationID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" meta:resourcekey="Status" SortExpression="StatusID" HeaderStyle-CssClass="hidden-xs hidden-sm" ItemStyle-CssClass="hidden-xs hidden-sm">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.TaskStatuses,(int)Eval("StatusID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Machine" meta:resourcekey="Machine" SortExpression="MachineID" HeaderStyle-CssClass="hidden-xs hidden-sm" ItemStyle-CssClass="hidden-xs hidden-sm">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Machines,(int)Eval("MachineID")) %></ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="LastAdjustment" HeaderText="Last Adjustments" ReadOnly="true" SortExpression="LastAdjustment" meta:resourcekey="LastAdjustments"
                        ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
            <!-- Data source! -->
                <asp:ObjectDataSource ID="TaskSource" runat="server" SelectMethod="GetTasks" TypeName="TechnicalServiceSystem.DataSourceManagers.TasksManager" SortParameterName="SortBy" DataObjectTypeName="TechnicalServiceSystem.Base.Task" InsertMethod="InsertTask" UpdateMethod="UpdateTask" OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="showAll" QueryStringField="showAll" DefaultValue="False" Type="Boolean"/>
                        <asp:QueryStringParameter QueryStringField="showRepeating" Name="showRepeating" DefaultValue="False" Type="Boolean"/>
                        <asp:QueryStringParameter DefaultValue="" Name="contains" QueryStringField="Search" Type="String" />
                        <asp:QueryStringParameter Name="DepartmentID" QueryStringField="depID" Type="Int32" />
                        <asp:Parameter Name="SortBy" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </div>

            <!-- Users tab -->
            <div id="Users" class="tab-pane fade in" style="border:none!important;" runat="server">
                <h1><asp:Label Text="Users" meta:resourcekey="Users" runat="server"></asp:Label></h1>
                <div class="row option-row">
                    <div class="col-xs-3 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;margin-left:0.5em">
                        <button runat="server" name="btnNewUser" class="lg-float-right md-button-max-width btn-block"
                        style="text-align: center; height:100%;min-height: 1em;" onclick="loadUserPage(0);return false;">
                            <asp:Label runat="server" Text="New User" meta:resourcekey="NewUser" style="height:inherit"></asp:Label>
                        </button>
                    </div>
                    <div class="col-xs-3 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;">
                        <select runat="server" id="selectUserType" DataValueField="ID" DataTextField="Name" class="xs-float-left md-float-center md-dropdown-max-width btn-block"
                        style="min-height:1em;height:inherit;" onchange="onUserTypeChanged()"></select>
                    </div>
                    <div class="col-xs-4 col-lg-3" style="height:inherit;padding:0px;margin-right:0.4em;">
                        <asp:TextBox runat="server" ID="txtSearchUser" class="md-float-center" style="width:100%;height:100%;min-width:1em;min-height:1em;" AutoCompleteType="Disabled"
                        onkeypress='onSearchKeyEnter(event,SearchUserPage);' AutoPostBack="false"> </asp:TextBox>
                    </div>
                    <div class="col-xs-1 col-lg-2" style="height:inherit;padding:0px;">
                        <button runat="server" name="btnSearchUser" class="searchButton xs-float-right sm-float-left md-float-left lg-float-left" onclick="getUserPage();return false;" 
                        style="height:100%;text-align:center;max-height:100%; max-width:100%;">
                            <asp:Label runat="server" Text="Search" meta:resourcekey="Search" CssClass="hidden-xs hidden-sm"></asp:Label>
                            <asp:Image runat="server" CssClass="img hidden-lg hidden-md" AlternateText="Search" ImageUrl="./system/search.png" BackColor="Transparent" 
                            style="border:none;background-size: contain;text-align:center;max-height:inherit;max-width:inherit;"/>
                        </button>
                    </div>
                </div>
                <asp:GridView runat="server" ID="UserGrid" DataSourceID="UserSource" AutoGenerateColumns="False" DataKeyNames="ID"
                    CssClass="gridview table table-responsive alternating-table-color" RowStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White"
                    OnRowDataBound="UserView_RowDataBound" HorizontalAlign="Center" AllowSorting="True" HeaderStyle-BorderColor="Black" HeaderStyle-CssClass="gridview-header"
                    ShowHeaderWhenEmpty="True" style="margin-top:1vw;font-size:2vmin;margin-left:auto;margin-right:auto;max-width:100%" BackColor="White" BorderColor="#868686" 
                    RowStyle-CssClass="text-responsive" >
                    <Columns>
                        <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True" SortExpression="Username" meta:resourcekey="UserName" />
                        <asp:CheckBoxField HeaderText="Active" meta:resourcekey="Active" SortExpression="Active" DataField="Active" ReadOnly="false" ControlStyle-Width="6.4em"/>
                        <asp:TemplateField HeaderText="Department" meta:resourcekey="Department" SortExpression="DepartmentID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Departments, (int)Eval("DepartmentID")) %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="AdminColumn" Text="<%# GetRoleName(1) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleAdmin" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"Admin") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User" ControlStyle-Width="6.4em">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="UserColumn" Text="<%# GetRoleName(2) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleUser" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"User") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Technician" ControlStyle-Width="6.4em">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="TechColumn" Text="<%# GetRoleName(3) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleTechnician" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"Technician") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Manager" ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="UserMngrColumn" Text="<%# GetRoleName(4) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleUserMngr" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"User Manager") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ControlStyle-Width="6.4em" ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="SupMngrColumn" Text="<%# GetRoleName(6) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleSupManager" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"Suppliers Manager") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Task Manager" ControlStyle-Width="6.4em"  ItemStyle-CssClass="hidden-xs hidden-sm hidden-md" HeaderStyle-CssClass="hidden-xs hidden-sm hidden-md">
                            <HeaderTemplate>
                                <asp:Label runat="server" ID="TaskMngrColumn" Text="<%# GetRoleName(5) %>" ></asp:Label>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="UserRoleTaskManager" runat="server" 
                                    Checked='<%# TechnicalServiceSystem.RoleManager.UserHasRole((System.Collections.ObjectModel.ObservableCollection<TechnicalServiceSystem.Base.RoleInfo>)Eval("UserRoles"),"Task Manager") %>' 
                                    style="zoom:0.7;" onclick="return false;" onkeydown="e = e || window.event; if(e.keyCode !== 9) return false;"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <!-- User Data source! -->
                <asp:ObjectDataSource ID="UserSource" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetUsers" TypeName="TechnicalServiceSystem.DataSourceManagers.UsersManager" SortParameterName="SortBy">
                        <SelectParameters>
                            <asp:QueryStringParameter QueryStringField="SearchUser" Name="contains" Type="String"></asp:QueryStringParameter>
                            <asp:QueryStringParameter QueryStringField="UserRoleID" DefaultValue="" Name="RoleID" Type="Int32"></asp:QueryStringParameter>
                            <asp:Parameter Name="SortBy" Type="String"></asp:Parameter>
                        </SelectParameters>
                </asp:ObjectDataSource>
            </div>

            <!-- Machines Tab! -->
            <div class="tab-pane fade in" id="Machines" runat="server">
                <h1><asp:Label Text="Machines" meta:resourcekey="Machines" runat="server"/></h1>
                <asp:GridView ID="MachineView" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="ID" CellPadding="10" CellSpacing="5" CssClass="gridview" 
                    RowStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="Gray" HeaderStyle-ForeColor="White" AllowSorting="True" DataSourceID="MachineSource"
                    HorizontalAlign="Center" >
                    <Columns>
                            <asp:BoundField DataField="Name" HeaderText="Machine" ReadOnly="True" SortExpression="Name" meta:resourcekey="MachineName"></asp:BoundField>
                        <asp:TemplateField HeaderText="Type" meta:resourcekey="MachineType" SortExpression="TypeID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.MachineTypes, (int)Eval("TypeID")) %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ModelName" HeaderText="Model" meta:resourcekey="ModelName" SortExpression="ModelName" ReadOnly="true" />
                        <asp:TemplateField HeaderText="Supplier" meta:resourcekey="Supplier" SortExpression="SupplierID">
                            <ItemTemplate><%# TechnicalServiceSystem.BaseClassListIndexConverter.Convert(Lists.Suppliers, (int)Eval("SupplierID")) %></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                <HeaderStyle HorizontalAlign="Center" BackColor="Gray" ForeColor="White"></HeaderStyle>
                <RowStyle HorizontalAlign="Center"></RowStyle>
                </asp:GridView>
                <asp:ObjectDataSource ID="MachineSource" runat="server" SelectMethod="GetMachines" TypeName="TechnicalServiceSystem.DataSourceManagers.MachinesManager" SortParameterName="SortBy" DataObjectTypeName="TechnicalServiceSystem.Base.MachineInfo" InsertMethod="InsertMachines" UpdateMethod="UpdateMachine">
                    <SelectParameters>
                        <asp:Parameter Name="SortBy" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource> 
            </div>

            <!-- Suppliers Tab! -->
            <div class="tab-pane fade in" id="Suppliers" runat="server" >
            <h1><asp:Label Text="Suppliers" meta:resourcekey="Suppliers" runat="server"/></h1>
                <br />
                <asp:Label Text="Currently not implemented" runat="server"/>
        </div>
        <!-- closing tab div -->
        </div>
        <!-- loading div -->
        <div id="loadingDiv" class="blocker" hidden="hidden" runat="server" style="z-index:999;">
           <div class="blocker-content">
               <asp:Image ID="loadingImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="/system/loading.gif" style="width:3em;height:auto;z-index:9999;"/>
           </div>
       </div>
    <!-- closing main div -->
    </div>
    <div class="navbar navbar-fixed-bottom" style="text-align:center;width:auto;max-width:8em;margin-left:auto;margin-right:auto;">
        <asp:Label runat="server" Font-Size="0.8em">&copy; DacoTaco 2017</asp:Label>
    </div>
    </form>
</body>
</html>
