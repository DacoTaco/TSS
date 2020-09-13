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


using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Suppliers;
using TechnicalServiceSystem.Entities.Tasks;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Lists;
using TechnicalServiceSystem.UI.HTML;

namespace TSS_ASPWebForms
{
    public partial class EditTask : Page
    {
        //read only related variables. set upon first load of the page
        public string ReadOnlyMsg => LanguageFiles.GetLocalTranslation("TaskOpenMsg");

        public static bool ReadOnly
        {
            get
            {
                var ret = Settings.GetSessionSetting<bool?>("TaskReadOnly");
                if (ret.HasValue)
                    return ret.Value;
                return false;
            }
            set { Settings.SetSessionSetting("TaskReadOnly", value); }
        }

        //the tasks. the original from database and the task that we are editting
        public static Task Task
        {
            get { return Settings.GetSessionSetting<Task>("EditTask"); }
            set { Settings.SetSessionSetting("EditTask", value); }
        }

        public static Task OriginalTask
        {
            get { return Settings.GetSessionSetting<Task>("OriginalTask"); }
            set { Settings.SetSessionSetting("OriginalTask", value); }
        }



        //Functions
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings.RequireLogin() && LoggedInUser.IsUserLoggedIn == false)
                    Response.Redirect("Login.aspx");

            if (IsPostBack)
                return;

            try
            {
                //setup page
                LoadTask();
                SetupPage();
                SetupDataBindings();

            }
            catch (Exception ex)
            {
                Session["exceptionMessage"] = ex;
                Response.Redirect("DisplayError");
            }
        }
        protected void LoadTask()
        {
            var TaskID = 0;
            if (int.TryParse(Request.Params["TaskID"], out TaskID) && TaskID > 0)
            {
                var taskMngr = new TaskManager();
                OriginalTask = taskMngr.GetTasks(Settings.GetCompanyName(), TaskID);
                Task = OriginalTask.Clone();
            }
            else
            {
                TaskID = 0;
                Task = null;
            }

            //if the task is null or 0, we are making a new task
            if (Task == null || TaskID == 0)
            {
                User user = null;
                if (LoggedInUser.IsUserLoggedIn)
                    user = LoggedInUser.GetUser();

                OriginalTask = new Task(0)
                {
                    Description = "",
                    IsUrguent = false,
                    TypeID = 0,
                    ReporterUser = null,
                    Reporter = "",
                    Location = null,
                    CreationDate = DateTime.Now,
                    LastModifiedOn = DateTime.Now
                };
                Task = OriginalTask.Clone();
                var department = user != null
                    ? SystemLists.General.Departments.Where(d => d.ID == user.Department.ID).First()
                    : SystemLists.General.Departments.First();

                Task.Location = department.Locations.OrderBy(l => l.Description).First();
                Task.Reporter = user != null ? user.UserName : "";
                TaskID = 0;
            }
        }
        protected void SetupDataBindings()
        {
            //setup all bindings in the page
            var user = LoggedInUser.GetUser();
            var department = Task == null ? (LoggedInUser.IsUserLoggedIn ? user.Department.ID : 0) : Task.Department.ID;
            var generalMngr = new GeneralManager();

            //data binding
            selectDepartments.DataSource = SystemLists.General.Departments;
            selectLocations.DataSource = generalMngr.GetLocations(department, Settings.GetCompanyName());
            selectTechnicians.DataSource =
                SystemLists.User.TranslatedTechnicians((string) GetGlobalResourceObject("NotSet", "0"));
            selectTaskState.DataSource =
                SystemLists.Tasks.GetTranslatedTaskStatuses(LanguageFiles.LoadLanguageFile("TaskStatus"));
            if (selectMachines.Visible)
            {
                var list = new ObservableCollection<Machine>();
                list.Add(new Machine(0) {Description = (string)GetGlobalResourceObject("NotSet", "0")});
                foreach (var machine in SystemLists.Supplier.Machines)
                {
                    list.Add(machine);
                }
                selectMachines.DataSource = list;
            }

            //call databind on page, and all its controls
            DataBind();



            //selections
            //machine. basically, if a device is set we pass a tostring from the id, if a null was somewhere -> empty string which ends up doing nothing.
            selectMachines.SelectItem(Task.Device?.ID.ToString() ?? "");
            selectDepartments.SelectItem(department.ToString());

            selectLocations.SelectItem((Task.Location?.ID.ToString() ?? ""));
            selectTaskState.SelectItem(Task.StatusID > 0 ? Task.StatusID.ToString() : "");
            selectTechnicians.SelectItem(Task.Technician?.ID.ToString() ?? "0");
        }
        protected void SetupPage()
        {
            //setup the page in general
            if (Task.ID > 0)
            {
                //editting a task!
                txtReporter.Disabled = true;

                var userHash = LoggedInUser.GetUserHash();
                var taskMngr = new TaskManager();

                //check if its editable at all. if not, disable and grey out a few shits
                if (!taskMngr.TaskEditable(Task.ID, userHash) || !taskMngr.SetTaskOpened(Task.ID, userHash))
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
                    AddPhotoBtn.Disabled = true;
                    AddPhotoInput.Disabled = true;
                    AddNotesButton.Disabled = true;

                    /* Add a warning/alert to the page saying we can't open the task */
                    JavascriptAlert.Show(ReadOnlyMsg);
                    return;
                }
            }

            ReadOnly = false;

            //check user permissions
            if (LoggedInUser.IsUserLoggedIn)
            {
                txtReporter.Disabled = true;

                var roles = RoleManager.GetUserPermissions(LoggedInUser.GetUser());

                if ((roles & (int) RolesPermissions.ManageMachines) == 0 &&
                    (roles & (int) RolesPermissions.Technician) == 0)
                    machineRow.Visible = false;

                if ((roles & (int)RolesPermissions.ManageTasks) == 0 &&
                    (roles & (int)RolesPermissions.Technician) == 0)
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

            //fill the images in the ul
            foreach (var item in Task.Photos)
            {
                var li = new HtmlGenericControl("li");
                li.Attributes.Add("style", "display:inline;max-height:inherit;");

                var ancor = new HtmlGenericControl("a");
                ancor.Attributes.Add("href", item.FileName);
                ancor.Attributes.Add("data-lightbox", "TaskImages");
                ancor.Attributes.Add("style",
                    "height:inherit;max-height:inherit;min-height:inherit;width:auto;margin-bottom:0;padding-right:5px;");

                var image = new HtmlGenericControl("img");
                image.Attributes.Add("src", item.FileName);
                image.Attributes.Add("style",
                    "height:inherit;max-height:inherit;min-height:inherit;width:auto;margin-bottom:0;");

                ancor.Controls.Add(image);
                li.Controls.Add(ancor);
                imagesList.Controls.Add(li);
            }

        }


        //-----------------------------------------------
        //                  WebMethods
        //-----------------------------------------------
        //  AKA functions / API Calls we can call from javascript

        /// <summary>
        ///     Checks whether the Task is opened in read only mode
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string TaskReadOnly() 
            => ReadOnly ? 
                LanguageFiles.GetLocalTranslation("TaskOpenMsg", "Task is already opened by another user. the task will be opened as read only.") : 
                null;

        /*
         ok so, when we save a Task we need to verify all properties. we need a string that will contain the error msg. 
         we begin by adding all properties that arent correct and
         at the end of the method we check if the string is empty. if it isnt, add the error msg at the beginning. else, save and proceed
         */
        [WebMethod]
        public static string SaveTask()
        {
            if (Task == null || OriginalTask == null || ReadOnly)
            {
                return LanguageFiles.GetLocalTranslation("ErrorSaveTasks", "You can not save task at this time");
            }

            var errorMsg = string.Empty;
            string Empty = LanguageFiles.GetLocalTranslation("Empty", "Empty");

            //Verify required fields!
            if (string.IsNullOrWhiteSpace(Task.Reporter))
                errorMsg += $"{LanguageFiles.GetLocalTranslation("Reporter.Text", "Reporter").TrimEnd(':')}({Empty}){Environment.NewLine}";

            if (string.IsNullOrWhiteSpace(Task.Description))
                errorMsg += $"{LanguageFiles.GetLocalTranslation("Description.Text", "Description").TrimEnd(':')}({Empty}){Environment.NewLine}";

            if (Task.DepartmentID <= 0)
                errorMsg += $"{LanguageFiles.GetLocalTranslation("Department.Text", "Department").TrimEnd(':')}({Empty}){Environment.NewLine}";

            if (Task.Location.ID <= 0)
                errorMsg += $"{LanguageFiles.GetLocalTranslation("Location.Text", "Location").TrimEnd(':')}({Empty}){Environment.NewLine}";

            if(!String.IsNullOrWhiteSpace(errorMsg))
                errorMsg = $"{LanguageFiles.GetLocalTranslation("ErrorMsg", "One or more fields are not valid. please recheck the following fields : ")} : " +
                           $"{Environment.NewLine}{Environment.NewLine}" + errorMsg;
            else
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
                catch (Exception ex)
                {
                    errorMsg = "an error has occured : " + ex.Message;
                }
            }

            return errorMsg;
        }

        /// <summary>
        ///     Set Task as Closed in database
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static bool CloseTask()
        {
            var ret = true;

            if (ReadOnly || Task == null || OriginalTask == null)
                return ret;

            try
            {
                var tskMngr = new TaskManager();
                ret = tskMngr.SetTaskClosed(Task.ID, LoggedInUser.GetUserHash());

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
        ///     verify and push new value to the Task
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [WebMethod]
        public static bool NewPropertyValue(string PropertyName, object value)
        {
            var type = typeof(Task);
            var property = type.GetProperty(PropertyName);

            //check if the property exists, if we are actuall working on a task and the task is not readonly
            if (property == null || Task == null || ReadOnly)
                return false;

            var changed = false;
            //see which property we are changing, verify value and set value as changed and found if its all ok
            switch (PropertyName)
            {
                case nameof(Task.Photos):
                    var data = value as string;

                    if (data == null)
                        break;

                    Photo photo;
                    if (Task.ID > 0)
                        photo = new Photo(0,
                            string.Format("Task_{0}_Photo_{1}", Task.ID, DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")));
                    else
                        photo = new Photo(0, string.Empty);
                    photo.PhotoSource = data;
                    Task.Photos.Add(photo);
                    changed = true;
                    break;
                case nameof(Task.Notes):
                    var descr = value as string;

                    if (string.IsNullOrWhiteSpace(descr) || Regex.IsMatch(descr, @"^[A-Za-z0-9?!,.:; ]+$") == false)
                        break;

                    var note = new Note(descr, DateTime.Now);
                    Task.Notes.Add(note);
                    changed = true;
                    break;
                case nameof(Task.Location):
                    try
                    {
                        var locID = int.Parse(value as string);
                        Task.Location = new GeneralManager().GetLocation(locID);
                        changed = (Task.Location.ID != OriginalTask.Location.ID);
                    }
                    catch
                    {
                        changed = false;
                    }
                    
                    break;
                case nameof(Task.StatusID):
                    try
                    {
                        var statusID = int.Parse(value as string);
                        Task.StatusID = statusID;
                        changed = Task.StatusID != OriginalTask.StatusID;

                        var statusChanged = Task.StatusID != OriginalTask.StatusID;
                    }
                    catch
                    {
                        changed = false;
                    }
                    break;
                case nameof(Task.Technician):
                    try
                    {
                        var userID = int.Parse(value as string);
                        Task.Technician = (User)SystemLists.User.Technicians.Where(t => t.ID == userID).FirstOrNull();
                        changed = Task.TechnicianID != OriginalTask.TechnicianID;

                        var statusChanged = Task.StatusID != OriginalTask.StatusID;
                    }
                    catch
                    {
                        changed = false;
                    }
                    break;
                case nameof(Task.Device):
                    try
                    {
                        var machineID = int.Parse(value as string);
                        Task.Device = new SupplierManager().GetMachine(machineID);
                        changed = Task.Device?.ID != OriginalTask.Device?.ID;
                    }
                    catch
                    {
                        changed = false;
                    }
                    break;
                case "DepartmentID":
                    var newDep = 0;
                    if (int.TryParse(value as string, out newDep))
                    {
                        //we have to set the department! so we get the new department, get the list of locations of said department, check the locationID's on which to set,
                        //set the changed flag in the task and then parse the DepartmentID like the rest
                        Location newloc;
                        var generalMngr = new GeneralManager();
                        var list = generalMngr.GetLocations(newDep, Settings.GetCompanyName());

                        if (newDep == OriginalTask.DepartmentID)
                        {
                            newloc = OriginalTask.Location;
                        }
                        else
                            newloc = list.First();

                        Task.Location = newloc;

                        var locationChanged = (Task.Location?.ID??0) != (OriginalTask.Location?.ID??0);
                    }
                    goto case "Generic";
                case "Reporter":
                    var text = value as string;

                    if (string.IsNullOrWhiteSpace(text) || Regex.IsMatch(text, @"^[A-Za-z0-9 ]+$") == false) break;
                    goto case "Generic";
                case "Description":
                    var desc = value as string;

                    if (string.IsNullOrWhiteSpace(desc) ||
                        Regex.IsMatch(desc, @"^[A-Za-z0-9?!.,:; ]+$") == false) break;
                    goto case "Generic";
                case "IsUrguent":
                case "Generic"://Generic is kinda our "default push change" case :P 
                    //Generic awesomeness!!
                    //basically, we take the value, try and convert it to the needed type, and assign the value. if the conversion fails it means we didn't get the right type anyway, so no change
                    try
                    {
                        var variableType = property.PropertyType;
                        var valueType = value.GetType();
                        var newValue = Convert.ChangeType(value, variableType);
                        if (newValue != property.GetValue(Task, null))
                            changed = true;
                        property.SetValue(Task, newValue);
                    }
                    catch
                    { 
                        changed = false;
                    }

                    break;
                default:
                    break;
            }

            return changed;
        }

        /// <summary>
        ///     Retrieve a property from the task
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        [WebMethod]
        public static object GetTaskProperty(string PropertyName)
        {
            var type = typeof(Task);
            var property = type.GetProperty(PropertyName);
            var test = type.GetProperties();

            if (property == null)
                return null;

            var ret = property.GetValue(Task, null);
            return ret;
        }

        /// <summary>
        ///     Get all locations of a given DepartmentID
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static List<Location> GetLocations()
        {
            var list = new List<Location>();
            try
            {
                var generalMngr = new GeneralManager();
                list = generalMngr.GetLocations(Task.DepartmentID, Settings.GetCompanyName()).ToList();
                if (list == null)
                    list = new List<Location>();
                return list;
            }
            catch
            {
                return new List<Location>();
            }
        }
    }
}