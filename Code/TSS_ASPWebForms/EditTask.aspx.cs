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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;
using TSS_ASPWebForms.Models;

namespace TSS_ASPWebForms
{
    public partial class EditTask : System.Web.UI.Page
    {
        /// <summary>
        /// System Lists
        /// </summary>
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }

        //the tasts. the original from database and the task that we are editting
        static public ChangedTask Task
        {
            get
            {
                return HttpContext.Current.Session["EditTask"] as ChangedTask;
            }
            set
            {
                HttpContext.Current.Session["EditTask"] = value;
            }
        }
        static public Task OriginalTask
        {
            get
            {
                    return HttpContext.Current.Session["OriginalTask"] as Task;
            }
            set
            {
                    HttpContext.Current.Session["OriginalTask"] = value;
            }
        }

        //variables used in the html part
        protected int TaskID = 0;
        public bool Urguent
        {
            get
            {
                return Task.Urguent;
            }
            set
            {

            }
        }

        //Read only related. the message and the bool stating if it is read only or not
        public static bool ReadOnly
        {
            get
            {
                bool? ret = HttpContext.Current.Session["TaskReadOnly"] as bool?;
                if (ret.HasValue)
                    return ret.Value;
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["TaskReadOnly"] = value;
            }
        }
        public string ReadOnlyMsg
        {
            get
            {
                return GetLocalResourceObject("TaskOpenMsg").ToString();
            }
        }

        /* IDEA ON HOW TO TACKLE THIS (and edits in general) : 


        LOAD : load task(ID?) into session, load page into modal using AJAX, page gets task(ID?) out of session and verifies if its possible to edit.
        IF task's ID is invalid or 0, new task. else -> edit.



        DATA : Static ChangedTask in the page, saved in session. on load we push data from session in it, on save we verify and push data back into session


        SAVE : get the changedTask in the page class, verify data and push to the Taskmanager (which pushes it to SQL) followed by reloading the page using AJAX.*/
        protected void SetupDataBindings()
        {
            //setup all bindings in the page
            int department = 0;
            UserInfo user = LoggedUser.GetUser();

            if (Task != null)
                department = Task.DepartmentID;
            else
                department = (LoggedUser.UserLoggedIn == true) ? user.DepartmentID : 0;

            selectDepartments.DataSource = Lists.Departments;
            selectDepartments.DataBind();

            var generalMngr = new GeneralManager();
            selectLocations.DataSource = generalMngr.GetLocations(department, Settings.GetAppSetting("company"));
            selectLocations.DataBind();
            if ((TaskID <=0 || Task.ID <=0) && selectLocations.Items.Count > 0)
            {
                int id = 0;
                bool result = Int32.TryParse(selectLocations.Items[0].Value,out id);
                if(result)
                {
                    Task.LocationID = id;
                }
            }
            

            selectTechnicians.DataSource = Lists.Technicians;
            selectTechnicians.DataBind();

            selectMachines.DataSource = Lists.Machines;
            selectMachines.DataBind();

            selectTaskState.DataSource = Lists.TaskStatuses;
            selectTaskState.DataBind();


            if (!IsPostBack)
            {
                //select proper department
                var selected = selectDepartments.Items.FindByValue(department.ToString());
                if (selected != null)
                {
                    //select the right department!
                    int index = selectDepartments.Items.IndexOf(selected);
                    selectDepartments.SelectedIndex = index;
                }

                //select proper Location
                if (selectLocations.Items.Count > 0)
                {
                    selectLocations.SelectedIndex = 0;
                    if (Task.LocationID > 0)
                    {
                        ListItem item = selectLocations.Items.FindByValue(Task.LocationID.ToString());
                        if (item != null)
                        {
                            int index = selectLocations.Items.IndexOf(item);
                            selectLocations.SelectedIndex = index;
                        }
                    }
                }

                //select proper Technician
                if (selectTechnicians.Items.Count > 0)
                {
                    selectTechnicians.SelectedIndex = 0;
                    if (Task.TechnicianID > 0)
                    {   
                        ListItem item = selectTechnicians.Items.FindByValue(Task.TechnicianID.ToString());
                        if (item != null)
                        {
                            int index = selectTechnicians.Items.IndexOf(item);
                            selectTechnicians.SelectedIndex = index;
                        }
                    }
                }

                //select proper Task State  
                if (selectTaskState.Items.Count > 0)
                {
                    selectTaskState.SelectedIndex = 0;
                    if (Task.StatusID > 0)
                    {
                        ListItem item = selectTaskState.Items.FindByValue(Task.StatusID.ToString());
                        if (item != null)
                        {
                            int index = selectTaskState.Items.IndexOf(item);
                            selectTaskState.SelectedIndex = index;
                        }
                    }
                }
            }
        }

        protected void SetupPage()
        {
            //setup the page in general
            if (Task.ID > 0)
            {
                //editting a task!
                txtReporter.Disabled = true;

                string userHash = LoggedUser.GetUserHash();
                var taskMngr = new TaskManager();

                //check if its editable at all. if not, disable and grey out a few shits
                if (!taskMngr.TaskEditable(TaskID, userHash) || !taskMngr.SetTaskOpened(TaskID, userHash))
                {
                    ReadOnly = true;
                    machineRow.Visible = false;
                    selectTechnicians.Disabled = true;
                    selectTechnicians.Style.Add("background-color", "#EBEBE4");
                    selectDepartments.Disabled = true;
                    selectDepartments.Style.Add("background-color", "#EBEBE4");
                    selectLocations.Disabled = true;
                    selectLocations.Style.Add("background-color", "#EBEBE4");
                    txtDescription.Disabled = true;
                    NoteBox.Disabled = true;
                    selectMachines.Disabled = true;
                    selectMachines.Style.Add("background-color", "#EBEBE4");
                    selectTaskState.Disabled = true;
                    selectTaskState.Style.Add("background-color", "#EBEBE4");
                    chkUrguent.Enabled = false;
                    imagesList.Disabled = true;
                    AddPhotoBtn.Enabled = false;
                    AddPhotoInput.Disabled = true;
                    AddNotesButton.Enabled = false;

                    /* Add a warning/alert to the page saying we can't open the task */
                    JavascriptAlert.Show(ReadOnlyMsg);
                    return;
                }
                else
                {
                    ReadOnly = false;
                }

            }
            else
            {
                ReadOnly = false;
                if(LoggedUser.UserLoggedIn)
                {
                    txtReporter.Disabled = true;

                }
            }


            if (LoggedUser.UserLoggedIn)
            {
                UserInfo user = LoggedUser.GetUser();

                //check if user has permissions to change the technician
                if (!RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.ManageTasks))
                {
                    selectTechnicians.Disabled = true;
                    selectTechnicians.Style.Add("background-color", "#EBEBE4");
                }


                //check if user has permissions to change the machine
                if (
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.Technician) &&
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.ManageTasks) &&
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.ManageMachines) &&
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.ManageSuppliers)
                    )
                {
                    machineRow.Visible = false;
                }

                //check if the user has permissions to change the State
                if (
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.Technician) &&
                    !RoleManager.UserHasPermission(user, RoleInfo.RolesPermissions.ManageTasks)
                    )
                {
                    selectTaskState.Disabled = true;
                    selectTaskState.Style.Add("background-color", "#EBEBE4");
                }
            }
            else
            {
                selectTechnicians.Disabled = true;
                selectTechnicians.Style.Add("background-color", "#EBEBE4");
                machineRow.Visible = false;
                selectTaskState.Disabled = true;
                selectTaskState.Style.Add("background-color", "#EBEBE4");
            }

            //fill the images ul!
            foreach (PhotoInfo item in Task.Photos)
            {
                HtmlGenericControl li = new HtmlGenericControl("li");
                li.Attributes.Add("style", "display:inline;max-height:inherit;");

                HtmlGenericControl ancor = new HtmlGenericControl("a");
                ancor.Attributes.Add("href", item.FileName);
                ancor.Attributes.Add("data-lightbox", "TaskImages");
                ancor.Attributes.Add("style", "height:inherit;max-height:inherit;min-height:inherit;width:auto;margin-bottom:0;padding-right:5px;");

                HtmlGenericControl image = new HtmlGenericControl("img");
                image.Attributes.Add("src", item.FileName);
                image.Attributes.Add("style", "height:inherit;max-height:inherit;min-height:inherit;width:auto;margin-bottom:0;");

                //li.Controls.Add(image);
                ancor.Controls.Add(image);
                li.Controls.Add(ancor);
                imagesList.Controls.Add(li);
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //retrieve Task ID
                    bool result = Int32.TryParse(Request.Params["TaskID"], out TaskID);
                    if (result == true && TaskID > 0)
                    {
                        var taskMngr = new TaskManager();
                        OriginalTask = taskMngr.GetTasks(Settings.GetAppSetting("company"), TaskID);
                        Task = ChangedTask.UpgradeBase(OriginalTask, false);
                    }
                    else
                    {
                        TaskID = 0;
                        Task = null;
                    }

                    //check if we need to load the lists
                    if (Lists.Departments == null || Lists.Departments.Count <= 0)
                    {
                        string notset = (string)GetGlobalResourceObject("NotSet", "0");
                        string[] TaskStatus = LanguageFiles.LoadLanguageFile("TaskStatus");
                        string[] TaskTypes = LanguageFiles.LoadLanguageFile("TaskTypes");
                        string[] RoleNames = LanguageFiles.LoadLanguageFile("Roles");

                        Lists.GetLists(null, TaskStatus, TaskTypes,RoleNames, notset, null);
                    }
                }

                //if the task is null or 0, we are making a new task
                if (Task == null || TaskID == 0)
                {
                    UserInfo user = null;
                    if (LoggedUser.UserLoggedIn)
                        user = LoggedUser.GetUser();

                    OriginalTask = new Task(0, "", false, 0, "", 0, 0, 0, 0, 0, DateTime.Now, DateTime.Now);
                    Task = ChangedTask.UpgradeBase(OriginalTask);
                    Task.DepartmentID = (user != null) ? user.DepartmentID : Lists.Departments[0].ID;
                    Task.Reporter = (user != null) ? user.Username : "";
                    Task.ReporterID = (user != null) ? user.ID : 0;

                    TaskID = 0;
                }

                if (!IsPostBack)
                {
                    this.DataBind();
                }

                //setup page
                SetupDataBindings();
                SetupPage();
            }
            catch (Exception ex)
            {
                this.Session["exceptionMessage"] = ex.Message;
                Response.Redirect("DisplayError");
            }
        }


        //-----------------------------------------------
        //                  WebMethods
        //-----------------------------------------------
        //  AKA functions we can call from javascript


        /*
         ok so, when we save a Task we need to verify all properties. we need a string that will contain the error msg. 
         we begin by adding all properties that arent correct and
         at the end of the method we check if the string is empty. if it isnt, add the error msg at the beginning. else, save and proceed
         */
        [WebMethod]
        public static string SaveTask()
        {
            string errorMsg = String.Empty;
            List<String> IncorrectFields = new List<string>();
            string Empty;

            try
            {
                Empty = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "Empty").ToString();
            }
            catch
            {
                Empty = "Empty";
            }


            if (Task == null || OriginalTask == null || ReadOnly == true)
            {
                try
                {
                    errorMsg = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "ErrorSaveTasks").ToString();
                }
                catch
                {
                    errorMsg = "Function 'SaveTasks' can not be called at this time";
                }
                return errorMsg;
            }
                

            //Verify required fields!
            if (String.IsNullOrWhiteSpace(Task.Reporter))
            {
                try
                {
                    //(HttpContext.Current.CurrentHandler as Page).AppRelativeVirtualPath
                    string propName = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "Reporter.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch(Exception ex)
                {
                    IncorrectFields.Add(String.Format("Reporter({0})", Empty));
                }
            }

            if (String.IsNullOrWhiteSpace(Task.Description))
            {
                try
                {
                    //(HttpContext.Current.CurrentHandler as Page).AppRelativeVirtualPath
                    string propName = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "Description.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch (Exception ex)
                {
                    IncorrectFields.Add(String.Format("Description({0})", Empty));
                }
            }

            if(Task.DepartmentID <= 0)
            {
                try
                {
                    //(HttpContext.Current.CurrentHandler as Page).AppRelativeVirtualPath
                    string propName = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "Department.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch (Exception ex)
                {
                    IncorrectFields.Add(String.Format("Department({0})", Empty));
                }
            }

            if (Task.LocationID <= 0)
            {
                try
                {
                    //(HttpContext.Current.CurrentHandler as Page).AppRelativeVirtualPath
                    string propName = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "Location.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch (Exception ex)
                {
                    IncorrectFields.Add(String.Format("Location({0})", Empty));
                }
            }


            if (IncorrectFields.Count > 0)
            {
                try
                {
                    errorMsg = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "ErrorMsg").ToString() + " : " + Environment.NewLine + Environment.NewLine;
                }
                catch
                {
                    errorMsg = "One or more fields are not valid. please recheck the following fields : " + Environment.NewLine + Environment.NewLine;
                }
                

                foreach ( string item in IncorrectFields)
                {
                    errorMsg += (item + Environment.NewLine);
                }
            }

            if (string.IsNullOrWhiteSpace(errorMsg))
            {
                try
                {
                    var taskMngr = new TaskManager();
                    if (Task.ID > 0)
                        taskMngr.ChangeTasks(Task);
                    else
                        taskMngr.AddTasks(Task);

                    errorMsg = string.Empty;
                    //Cleanup the Task so this function doesnt get abused
                    CloseTask();
                }
                catch(Exception ex)
                {
                    errorMsg = "an error has occured : " + ex.Message;
                }
            }

            return errorMsg;
        }

        /// <summary>
        /// Set Task as Closed in database
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static bool CloseTask()
        {
            bool ret = true;

            if (ReadOnly || Task == null || OriginalTask == null)
                return ret;

            try
            {
                var tskMngr = new TaskManager();
                ret = tskMngr.SetTaskClosed(Task.ID, LoggedUser.GetUserHash());

                if (ret)
                {
                    Task = null;
                    OriginalTask = null;
                    ReadOnly = false;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// verify and push new value to the ChangedTask
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [WebMethod]
        public static bool NewPropertyValue(string PropertyName,object value)
        {
            bool ret = false;

            Type type = typeof(Task);
            PropertyInfo property = type.GetProperty(PropertyName);

            //check if the property exists, if we are actuall working on a task and the task is not readonly
            if (property == null || Task == null || ReadOnly == true)
                return false;

            bool changed = false;
            bool found = true;
            //see which property we are changing, verify value and set value as changed and found if its all ok
            switch(PropertyName)
            {
                case "DepartmentID":
                    int newDep = 0;
                    if(int.TryParse(value as string, out newDep))
                    {
                        //we have to set the department! so we get the new department, get the list of locations of said department, check the locationID's on which to set,
                        //set the changed flag in the task and then parse the DepartmentID like the rest
                        int newlocID = 0;
                        var generalMngr = new GeneralManager();
                        ObservableCollection<LocationInfo> list = generalMngr.GetLocations(newDep, Settings.GetAppSetting("company"));

                        foreach (LocationInfo item in list)
                        {
                            if (newDep == OriginalTask.DepartmentID && item.ID == OriginalTask.LocationID)
                            {
                                newlocID = OriginalTask.LocationID;
                                break;
                            }
                            else if(newlocID == 0)
                            {
                                newlocID = item.ID;
                            }
                        }
                        Task.LocationID = newlocID;
                        bool locationChanged = (Task.LocationID != OriginalTask.LocationID);
                        if (Task.Changed_Properties.ContainsKey("LocationID"))
                            Task.Changed_Properties["LocationID"] = locationChanged;
                        else
                        {
                            Task.Changed_Properties.Add("LocationID", locationChanged);
                        }
                    }
                    //locationID is a bit like the 'do the default changed' case at this point
                    goto case "LocationID";
                case "Photos":
                    var data = value as string;

                    if (data == null)
                        break;

                    PhotoInfo photo;
                    if (Task.ID > 0)
                        photo = new PhotoInfo(0, String.Format("Task_{0}_Photo_{1}", Task.ID, DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")));
                    else
                        photo = new PhotoInfo(0, String.Empty);
                    photo.PhotoSource = data;

                    Task.Photos.Add(photo);

                    changed = true;
                    found = true;
                    break;
                case "Notes":
                    var descr = value as string;

                    if (String.IsNullOrWhiteSpace(descr) || Regex.IsMatch(descr, @"^[A-Za-z0-9?!,.:; ]+$") == false)
                        break;

                    Note note = new Note(descr, DateTime.Now);
                    Task.Notes.Add(note);

                    changed = true;
                    found = true;
                    break;
                case "Reporter":
                    string text = value as string;

                    if (String.IsNullOrWhiteSpace(text) || Regex.IsMatch(text, @"^[A-Za-z0-9 ]+$") == false)
                    {
                        break;
                    }
                    goto case "LocationID";
                case "Description":
                    string desc = value as string;
                    
                    if(String.IsNullOrWhiteSpace(desc) || Regex.IsMatch(desc, @"^[A-Za-z0-9?!.,:; ]+$") == false)
                    {
                        break;
                    }
                    goto case "LocationID";
                case "Urguent":
                case "TechnicianID":
                case "StatusID":
                case "MachineID":
                case "LocationID": //locationID is kinda our "default push change" case :P 
                    //Generic awesomeness!!
                    //basically, we take the value, try and convert it to the needed type, and assign the value. if the conversion fails it means we didn't get the right type anyway, so no change
                    try
                    {
                        Type variableType = property.PropertyType;
                        Type valueType = value.GetType();
                        var newValue = Convert.ChangeType(value, variableType);
                        if (newValue != property.GetValue(Task, null))
                        {
                            changed = true;
                        }
                        property.SetValue(Task, newValue);
                    }
                    catch
                    {
                        changed = false;
                        found = false;
                    }
                    break;
                default:
                    found = false;
                    break;
            }

            //if it is set as found, we will push the Changed_Properties entry
            if (found)
            {
                if (Task.Changed_Properties.ContainsKey(PropertyName))
                    Task.Changed_Properties[PropertyName] = changed;
                else
                {
                    Task.Changed_Properties.Add(PropertyName, changed);
                }
            }

            ret = changed;

            return ret;
        }

        /// <summary>
        /// Retrieve a property from the task
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        [WebMethod]
        public static object GetTaskProperty(string PropertyName)
        {
            Type type = typeof(Task);
            PropertyInfo property = type.GetProperty(PropertyName);

            if (property == null)
                return null;

            var ret = property.GetValue(Task, null);
            return ret;

        }

        /// <summary>
        /// Get all locations of a given DepartmentID
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<LocationInfo> GetLocations(int departmentID)
        {
            List<LocationInfo> list = new List<LocationInfo>();
            try
            {
                var generalMngr = new GeneralManager();
                list = (generalMngr.GetLocations(departmentID, Settings.GetAppSetting("company"))).ToList();
                if (list != null)
                    return list;
            }
            catch
            {
                list = new List<LocationInfo>();
            }
            return list;      
        }

        /// <summary>
        /// Checks whether the Task is opened in read only mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string TaskReadOnly()
        {
            string ret = null;

            if(ReadOnly == true)
            {
                ret = HttpContext.GetLocalResourceObject("~/EditTask.aspx", "TaskOpenMsg").ToString();
            }

            return ret;
        }


    }
}