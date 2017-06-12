/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2017 - Joris 'DacoTaco' Vermeylen

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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TSS_ASPWebForms
{
    public partial class Index : System.Web.UI.Page
    {

        public string SearchText
        {
            get
            {
                return (string)HttpContext.Current.Session["SearchText"];
            }

            set
            {
                HttpContext.Current.Session["SearchText"] = value;
            }
        }

        public int? SearchDepartmentID
        {
            get
            {
                int? ret = default(int?);
                if (HttpContext.Current.Session["SearchDepartmentID"] != null)
                    ret = (int?)HttpContext.Current.Session["SearchDepartmentID"];
                return ret;
            }
            set
            {
                HttpContext.Current.Session["SearchDepartmentID"] = value;
            }
        }


        //system lists, contains all tasks,users, etc etc
        private SystemLists lists = null;
        public SystemLists Lists
        {
            get
            {
                if (lists == null)
                    lists = SystemLists.GetInstance();

                return lists;
            }

        }

        //retrieve role name from the lists. these are already translated anyway :)
        public string GetRoleName(int roleID)
        {
            string ret = "[unknown]";

            foreach (RoleInfo role in Lists.Roles)
            {
                if(role.ID == roleID)
                {
                    return role.Name;
                }
            }

            return ret;
        }

        /// <summary>
        /// function to set all tabs visable or invisable depending on the user's roles
        /// </summary>
        /// <param name="user"></param>
        protected void SetTabs(UserInfo user)
        {
            //OK, HOW THIS WORKS.
            //We loop trough all roles. per function of the system the user has access to we add a number that is a bitwise new number ( 0,1,2,4,8,16,32,64,128,...)
            //after we've looped trough the roles, we do a bitwise or so we can easily tell if the user has access to a part of the system
            int roles = 0;
            roles = RoleManager.GetUserPermissions(user, Lists.Roles.ToList());


            //handle the tasks tab
            if ((roles & (int)RoleInfo.RolesPermissions.Tasks) <= 0)
            {
                TasksTab.Visible = false;
                Tasks.Visible = false;
            }
            else
            {
                //so incase a selectedindexchanged is fired, the index doesn't reset by the databind...
                if (!IsPostBack)
                {
                    //TODO : change the datasource to more then just the departments. need to add current user if it logged in, add show all
                    ObservableCollection<DepartmentInfo> DepartmentsList = new ObservableCollection<DepartmentInfo>();

                    string alldep = GetLocalResourceObject("AllDepartments") as string;
                    if (String.IsNullOrWhiteSpace(alldep))
                        alldep = "All";

                    DepartmentsList.Add(new DepartmentInfo(0, alldep));
                    foreach (DepartmentInfo item in Lists.Departments)
                    {
                        DepartmentsList.Add(item);
                    }

                    DropDownSorting.DataSource = DepartmentsList;
                    DropDownSorting.DataBind();
                }
                
                /*if ((roles & (int)RoleInfo.RolesPermissions.Technician) <= 0)
                {*/
                    //user isn't technician or task manager. hide a few columns in the task grid
                    try
                    {
                        string TaskTypeHeader = GetLocalResourceObject("TaskType.HeaderText") as string;
                        string MachineHeader = GetLocalResourceObject("Machine.HeaderText") as string;

                        if (String.IsNullOrEmpty(TaskTypeHeader))
                            TaskTypeHeader = "Task Type";

                        if (string.IsNullOrEmpty(MachineHeader))
                            MachineHeader = "Machine";

                        ((DataControlField)TaskView.Columns
                           .Cast<DataControlField>()
                           .Where(fld => (fld.HeaderText == TaskTypeHeader))
                           .SingleOrDefault()).Visible = false;

                        ((DataControlField)TaskView.Columns
                          .Cast<DataControlField>()
                          .Where(fld => (fld.HeaderText == MachineHeader))
                          .SingleOrDefault()).Visible = false;

                        if(!IsPostBack)
                            DropDownSorting.SelectedIndex = (LoggedUser.UserLoggedIn == true) ? user.DepartmentID : 0;


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                /*}
                else*/
                if ((roles & (int)RoleInfo.RolesPermissions.Technician) > 0)
                {
                    //we have a technician or task mananger! set department to all!
                    if (!IsPostBack)
                        DropDownSorting.SelectedIndex = 0;
                }
            }

            //permissions to the machines tab!
            if(true == true)//(roles & (int)RoleInfo.RolesPermissions.ManageMachines) <= 0)
            {
                //no permissions!
                MachinesTab.Visible = false;
                Machines.Visible = false;
            }
            else
            {
                MachinesTab.Visible = true;
                Machines.Visible = true;
            }

            //permissions to the Suppliers tab!
            if(/*
                ((roles & (int)RoleInfo.RolesPermissions.ViewSuppliers) <= 0 ) &&
                ((roles & (int)RoleInfo.RolesPermissions.ManageSuppliers) <= 0)*/
                true == true
              )
            {
                SuppliersTab.Visible = false;
                Suppliers.Visible = false;
            }
            else
            {
                SuppliersTab.Visible = true;
                Suppliers.Visible = true;
            }


            //permissions for the Users tab!
            if ((roles & (int)RoleInfo.RolesPermissions.ManageUsers) <= 0)
            {
                UsersTab.Visible = false;
                Users.Visible = false;
            }
            else
            {
                UsersTab.Visible = true;
                Users.Visible = true;

                //fill the roles we can pick to filter from
                if (!IsPostBack)
                {
                    ObservableCollection<RoleInfo> RolesList = new ObservableCollection<RoleInfo>();

                    string allRoles = GetLocalResourceObject("AllRoles") as string;
                    if (String.IsNullOrWhiteSpace(allRoles))
                        allRoles = "All Roles";

                    RolesList.Add(new RoleInfo(0, allRoles));
                    foreach (RoleInfo item in Lists.Roles)
                    {
                        RolesList.Add(item);
                    }
                    //bind User dropdown of roles
                    selectUserType.DataSource = RolesList;
                    selectUserType.DataBind();
                }
            }

        }

        /// <summary>
        /// Main function of the page. checks the configuration , redirects to login page if needed, loads the lists and databinds all contents
        /// Pass Parameter depID with a departmentID to list tasks for a certain department if no user is logged in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //clear the shit from the Task editting!
            if(HttpContext.Current.Session["OriginalTask"] != null)
            {
                HttpContext.Current.Session["OriginalTask"] = null;
                HttpContext.Current.Session["EditTask"] = null;
            }

            try
            {
                //check if user login is needed
                string requireLogin = Settings.GetAppSetting("RequireLogin");
                if (requireLogin == null)
                    requireLogin = "";
                if(
                    requireLogin != "0" &&
                    LoggedUser.GetUser() == null
                  )
                {
                    Response.Redirect("Login");
                }

                UserInfo user = LoggedUser.GetUser();

                //just some leftover tool to see the user hash :P
                //conID.Text = LoggedUser.GetUserHash();

                //set department string from the url parameters
                SearchDepartmentID = null;
                //if we dont have a role that needs to see all tasks, look for the tasks of said user

                //if the user isn't logged in, we might as well listen for specific departments
                string depID = Request.Params["depID"];
                if (!String.IsNullOrWhiteSpace(depID))
                {
                    int department;
                    Int32.TryParse(Request.Params["depID"], out department);
                    if (department >= -5)
                    {
                        SearchDepartmentID = department;
                    }
                }
                else
                {
                    //if all that isn't given, use the value from the dropdown
                    int selecteddep = -1;
                    if (DropDownSorting.SelectedIndex >= 0 && DropDownSorting.SelectedIndex < DropDownSorting.Items.Count)
                    {
                        bool result = Int32.TryParse(DropDownSorting.Items[DropDownSorting.SelectedIndex].Value, out selecteddep);
                        if (result == true)
                        {
                            if (selecteddep > 0)
                                SearchDepartmentID = selecteddep;
                            else
                                SearchDepartmentID = -2;
                        }
                    }

                    bool hasvalue = SearchDepartmentID.HasValue;

                    if (
                        !SearchDepartmentID.HasValue && 
                        user != null && 
                        !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.Technician)
                        )
                        SearchDepartmentID = user.DepartmentID;
                    else if (!SearchDepartmentID.HasValue && LoggedUser.UserLoggedIn)
                        SearchDepartmentID = -1;
                }


                //retrieve what to search for
                if (String.IsNullOrWhiteSpace(searchbar.Text))
                    searchbar.Text = String.Empty;

                //set search string in the textbox
                searchbar.Attributes["placeholder"] = String.Format("{0}...", (string)GetLocalResourceObject("Search.Text"));

                //set the session value's for searching and departmentID. this is fallback value's incase the DataSource fails and doesn't pass any values... ive seen it happen a few times...
                SearchText = null;
                string contains = Request.Params["Search"];
                if (!String.IsNullOrWhiteSpace(contains))
                {
                    SearchText = contains;
                }
                else if (!String.IsNullOrWhiteSpace(searchbar.Text))
                    SearchText = searchbar.Text;
                else if (LoggedUser.UserLoggedIn && contains != String.Empty && SearchDepartmentID >= 0)
                    SearchText = LoggedUser.GetUser().Username;
                else
                    SearchText = null;


                if (!IsPostBack || Lists.TaskStatuses.Count == 0)
                    GetLists();

                //setup the page for the current user
                SetupPage();
            }
            catch (Exception ex)
            {
                this.Session["exceptionMessage"] = ex.Message;
                Response.Redirect("DisplayError");
            }
        }

        /// <summary>
        /// set the page up depending on the given user
        /// </summary>
        /// <param name="user"></param>
        protected void SetupPage()
        {
            UserInfo user = null;

            if(LoggedUser.UserLoggedIn)
                user = LoggedUser.GetUser();

            //setup user related stuff!
            if (user != null)
            {
                lblUserName.Text = "&lt; " + user.Username + " &gt;";
                var genMngr = new GeneralManager();
                string path = genMngr.GetPhoto(user.PhotoID);
                if (String.IsNullOrWhiteSpace(path))
                    path = "./system/DefaultUser.jpg";
                userImage.ImageUrl = genMngr.GetPhoto(user.PhotoID);
                LoginMenu.Text = "Logout";
                ProfileMenu.Visible = true;
                LoginSeperator.Visible = true;
            }
            else
            {
                //set up the default stuff
                lblUserName.Text = String.Empty;
                userImage.ImageUrl = "./system/DefaultUser.jpg";
                LoginMenu.Text = "Login";
                ProfileMenu.Visible = false;
                LoginSeperator.Visible = false;
            }

            //load the Tasks before setting up the Tabs to the current user info
            if(IsPostBack)
                GetLists(true);

            //go through the roles and set the tabs for the user
            SetTabs(user);
        }

        /// <summary>
        /// Gets the Lists needed for this page
        /// </summary>
        /// <param name="TasksOnly"></param>
        protected void GetLists(bool TasksOnly = false)
        {
            UserInfo user = LoggedUser.GetUser();

            //if we aren't looking for something specific and we are logged in, add tasks of the user
            if (
                String.IsNullOrWhiteSpace(SearchText) && user != null && !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.Technician) &&
                (
                    DropDownSorting.SelectedIndex < 0 ||
                    ((DropDownSorting.SelectedIndex >= 0 && DropDownSorting.SelectedIndex < DropDownSorting.Items.Count) && DropDownSorting.Items[DropDownSorting.SelectedIndex].Value == user.DepartmentID.ToString())
                )
               )
            {
                SearchText = user.Username;
            }

            //get tasks!
            if (TasksOnly)
                Lists.GetTasks(SearchText, null);
            else
            {
                string notset = (string)GetGlobalResourceObject("NotSet", "0");
                string[] TaskStatus = LanguageFiles.LoadLanguageFile("TaskStatus");
                string[] TaskTypes = LanguageFiles.LoadLanguageFile("TaskTypes");
                string[] RoleNames = LanguageFiles.LoadLanguageFile("Roles");
                Lists.GetLists(SearchText, TaskStatus, TaskTypes,RoleNames, notset, null);
            }
        }

        //setup all the tooltips for task notes and register the onclick event for a row
        protected void TaskView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int index = e.Row.RowIndex;
            if (
                index < 0 ||
                index >= Lists.Tasks.Count              
              )
            {
                return;
            }
            else
            {
                if(!(String.IsNullOrWhiteSpace(Lists.Tasks[index].strNotes)))
                    e.Row.ToolTip = Lists.Tasks[index].strNotes;
                else
                    e.Row.ToolTip = String.Empty;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = "SelectTaskRow(" + Lists.Tasks[index].ID + "," + index + ",event);";
            }

        }

        //setup all shit for the user grid rows!
        protected void UserView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int index = e.Row.RowIndex;
            if (
                index < 0 ||
                index >= Lists.Users.Count
              )
            {
                return;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = "SelectUserRow(" + Lists.Users[index].ID + "," + index + ",event);";
            }
            return;
        }

        //Click event handler for the Login/Logout button in the user menu
        protected void LoginMenu_Click(object sender, EventArgs e)
        { 
            if (!LoggedUser.UserLoggedIn)
            {
                Response.Redirect("Login");
            }
            else
            {
                LoggedUser.SetUser(null);
                DropDownSorting.SelectedIndex = 0;
                hidTABControl.Value = "#Tasks";
                SearchText = null;
                SearchDepartmentID = null;
                TaskSource.Select();
            }

            SetupPage();
        }
    }

}