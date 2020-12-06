/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2019 - Joris 'DacoTaco' Vermeylen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see http://www.gnu.org/licenses */

using Equin.ApplicationFramework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Tasks;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Lists;
using TSS.Web.Feature.Users;

namespace TSS.Web
{
    public partial class index : Page
    {
        protected string GetTranslation(string table,int ID)
        {
            if (String.IsNullOrWhiteSpace(table) || ID < 0)
                return null;

            var list = LanguageFiles.LoadLanguageFile(table);
            if (list == null || ID > list.Length)
                return null;

            return list[ID];
        }
        protected static string ActiveTab
        {
            get { return Settings.GetSessionSetting<string>(nameof(ActiveTab)); }
            set { Settings.SetSessionSetting(nameof(ActiveTab), value); }
        }

        protected void Setup_UserContent()
        {
            User user = null;

            if (LoggedInUser.IsUserLoggedIn)
                user = LoggedInUser.GetUser();

            //setup user related stuff!
            if (user != null)
            {
                lblUserName.Text = "&lt; " + user.UserName + " &gt;";
                var path = user.Photo?.FileName;
                if (string.IsNullOrWhiteSpace(path))
                    path = "./system/DefaultUser.jpg";
                userImage.ImageUrl = path;
                LoginMenu.Text = "Logout";
                ProfileMenu.Visible = true;
                LoginSeperator.Visible = true;
            }
            else
            {
                //set up the default stuff
                lblUserName.Text = string.Empty;
                userImage.ImageUrl = "./system/DefaultUser.jpg";
                LoginMenu.Text = "Login";
                ProfileMenu.Visible = false;
                LoginSeperator.Visible = false;
            }
        }

        protected void Setup_Tabs()
        {
            //OK, HOW THIS WORKS.
            //We loop trough all roles. per function of the system the user has access to we add a number that is a bitwise new number ( 0,1,2,4,8,16,32,64,128,...)
            //after we've looped trough the roles, we do a bitwise or so we can easily tell if the user has access to a part of the system

            User user = null;

            if (LoggedInUser.IsUserLoggedIn)
                user = LoggedInUser.GetUser();

            var roles = RoleManager.GetUserPermissions(user);
            //handle the tasks tab
            if (!roles.Any(x => x == RolesPermissions.Tasks))
            {
                TasksTab.Visible = false;
                Tasks.Visible = false;
            }
            else
            {
                //link the departments drop down
                var list = SystemLists.General.Departments;

                var DepartmentsList = new ObservableCollection<Department>();

                DepartmentsList.Add(new Department(0) {Description = LanguageFiles.GetLocalTranslation("AllDepartments", "All") });
                foreach (var item in SystemLists.General.Departments) DepartmentsList.Add(item);

                DropDownSorting.DataSource = DepartmentsList;
                DropDownSorting.DataBind();

                if (LoggedInUser.IsUserLoggedIn && (!roles.Any(x => x == RolesPermissions.Technician)))
                {
                    DropDownSorting.SelectedIndex =
                        DepartmentsList.ToList().FindIndex(x => x.ID == LoggedInUser.GetUser().Department.ID);
                }
                else
                    DropDownSorting.SelectedIndex = 0;


                //set the parameters for the datasource
                string departmentID = Request.QueryString["depID"];
                var searchText = Request.QueryString["Search"];

                var departmentField = TaskSource.SelectParameters["DepartmentID"];
                var searchField = TaskSource.SelectParameters["SearchText"];
                if (LoggedInUser.IsUserLoggedIn)
                {
                    if (
                        !RoleManager.UserHasPermission(LoggedInUser.GetUser(),RolesPermissions.ManageTasks) &&
                        (
                            (String.IsNullOrWhiteSpace(departmentID) && String.IsNullOrWhiteSpace(searchText)) ||
                            (LoggedInUser.GetUser().Department.ID.ToString() == departmentID)
                        ))
                    {
                        // logged in + either nothing was given or the user's department was chosen -> show department + user's tasks
                        departmentField.DefaultValue = LoggedInUser.GetUser().Department.ID.ToString();
                        searchText = LoggedInUser.GetUser().UserName;
                    }
                    else if (!String.IsNullOrWhiteSpace(departmentID))
                    {
                        //department was set.
                        departmentField.DefaultValue = departmentID;
                    }

                    if (!String.IsNullOrWhiteSpace(searchText))
                    {
                        //search text was given.
                        searchField.DefaultValue = searchText;
                    }
                    
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(departmentID))
                        departmentField.DefaultValue = departmentID;

                    if (!String.IsNullOrWhiteSpace(searchText))
                        searchField.DefaultValue = searchText;
                }

                //set the department parameter
                TaskSource.SelectParameters["DepartmentID"] = departmentField;
                TaskSource.SelectParameters["SearchText"] = searchField;

                //set datasource and bind/retrieve data (databind also executes all inline code to bind to them)
                TaskSource.DataBind();
                TaskGrid.DataSourceID = nameof(TaskSource);
                TaskGrid.DataBind();
            }

            //handle machines tab
            //if (!roles.Any(x => x == RolesPermissions.ManageMachines))
            {
                //no permissions!
                MachinesTab.Visible = false;
                //Machines.Visible = false;
            }
            /*else
            {
                MachinesTab.Visible = true;
                //Machines.Visible = true;
            }*/

            //permissions to the Suppliers tab!
            /*if (!roles.Any(x => x == RolesPermissions.ManageSuppliers) &&
                !roles.Any(x => x == RolesPermissions.ViewSuppliers))*/
            {
                SuppliersTab.Visible = false;
                //Suppliers.Visible = false;
            }
            /*else
            {
                SuppliersTab.Visible = true;
                Suppliers.Visible = true;
            }*/


            //permissions for the Users tab!
            if (!roles.Any(x => x == RolesPermissions.ManageUsers))
            {
                UsersTab.Visible = false;
                Users.Visible = false;
            }
            else
            {
                UsersTab.Visible = true;
                Users.Visible = true;

                var RolesList = new ObservableCollection<RoleModel>();
                var roleList = LanguageFiles.LoadLanguageFile("Roles");

                for (int i = 0; i < roleList.Length; i++)
                {
                    RolesList.Add(RoleModel.CreateModel((Role)i, roleList[i]));
                }

                //bind User dropdown of roles
                selectUserType.DataSource = RolesList;
                selectUserType.DataBind();

                //set the parameters for the datasource
                string role = Request.QueryString["UserRole"];
                var searchText = Request.QueryString["SearchUser"];

                if (!String.IsNullOrWhiteSpace(role) && role != nameof(Role.AllRoles))
                {
                    var RoleField = UserSource.SelectParameters["role"];
                    RoleField.DefaultValue = role;
                    UserSource.SelectParameters["role"] = RoleField;
                }

                if (!String.IsNullOrWhiteSpace(searchText))
                {
                    var searchField = UserSource.SelectParameters["contains"];
                    searchField.DefaultValue = searchText;
                    UserSource.SelectParameters["contains"] = searchField;
                }

                UserSource.DataBind();
                UserGrid.DataSourceID = nameof(UserSource);
                UserGrid.DataBind();
            }

            //set the last known open tab
            if (!String.IsNullOrWhiteSpace(ActiveTab))
                hidTABControl.Value = ActiveTab;
        }

        protected void Setup_Page()
        {
            Setup_UserContent();
            Setup_Tabs();           
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings.RequireLogin() && LoggedInUser.IsUserLoggedIn == false)
                Response.Redirect("Login");

            if (IsPostBack)
                return;

            Setup_Page();
        }
        //Click event handler for the Login/Logout button in the user menu
        protected void LoginMenu_Click(object sender, EventArgs e)
        {
            //Login
            if (!LoggedInUser.IsUserLoggedIn)
            {
                Response.Redirect("Login");
                return;
            }

            //Logout
            LoggedInUser.SetUser(null);

            if (Settings.RequireLogin() && LoggedInUser.IsUserLoggedIn == false)
                Response.Redirect("Login");

            DropDownSorting.SelectedIndex = 0;
            hidTABControl.Value = "#Tasks";

            //page_load didn't setup the page(because this is a postback). so setup page
            Setup_Page();
        }
        protected void UserGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            if (e?.Row?.DataItem == null || e.Row.DataItem as ObjectView<User> == null)
                return;

            var User = (e.Row.DataItem as ObjectView<User>).Object;

            e.Row.Attributes["onclick"] = $"SelectUserRow( {User.ID},event);";
        }
        //event when a row is added
        protected void TaskGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            if (e?.Row?.DataItem == null || e.Row.DataItem as ObjectView<Task> == null)
                return;

            var Task = (e.Row.DataItem as ObjectView<Task>).Object;

            if (!string.IsNullOrWhiteSpace(Task?.strNotes))
                e.Row.ToolTip = Task.strNotes;
            else
                e.Row.ToolTip = string.Empty;

            e.Row.Attributes["onclick"] = $"SelectTaskRow({Task.ID},event);";
        }

        //-----------------------------------------------
        //                  WebMethods
        //-----------------------------------------------
        //  AKA functions / API Calls we can call from javascript

        [WebMethod]
        public static bool NewPropertyValue(string PropertyName, object value)
        {
            if (String.IsNullOrWhiteSpace(PropertyName))
                return true;

            switch (PropertyName.ToLower())
            {
                case "tab":
                    string newTab = value as string;
                    if (string.IsNullOrWhiteSpace(newTab) || !newTab.StartsWith("#"))
                        return false;

                    ActiveTab = newTab;
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}