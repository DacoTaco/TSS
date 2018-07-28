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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;
using TSS_ASPWebForms.Models;

namespace TSS_ASPWebForms
{
    public partial class EditUser : System.Web.UI.Page
    {
        //just like in the task : the systemlist, original loaded user and the one we edit
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }
        static public UserInfo EdittedUser
        {
            get
            {
                return Settings.GetSessionSetting<UserInfo>("EditUser");
            }
            set
            {
                Settings.SetSessionSetting("EditUser", value);
            }
        }
        static public UserInfo OriginalUser
        {
            get
            {
                return Settings.GetSessionSetting<UserInfo>("OriginalUser");
            }
            set
            {
                Settings.SetSessionSetting("OriginalUser", value);
            }
        }

        //userID , if user is read only and the error msg
        protected int UserID = 0;
        public static bool ReadOnly
        {
            get
            {
                bool? ret = Settings.GetSessionSetting<bool?>("UserReadOnly");
                if (ret.HasValue)
                    return ret.Value;
                else
                    return false;
            }
            set
            {
                Settings.SetSessionSetting("UserReadOnly", value);
            }
        }
        public string ReadOnlyMsg
        {
            get
            {
                string ret = "";
                object temp = GetLocalResourceObject("UserOpenMsg");
                if ( temp != null)
                    ret = temp.ToString();
                return ret;
            }
        }

        //setup the page
        private void SetupPage()
        {
            ReadOnly = false;
            if(UserID > 0)
            {
                string userHash = LoggedInUser.GetUserHash();
                var usrManager = new UserManager();
                if (!usrManager.UserEditable(UserID, userHash) || !usrManager.SetUserOpened(UserID, userHash))
                {
                    ReadOnly = true;             

                    /* Add a warning/alert to the page saying we can't open the task */
                    JavascriptAlert.Show(ReadOnlyMsg);
                    return;
                }
            }
            else
            {
                ReadOnly = false;
            }
        }

        //load the page
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedInUser.IsUserLoggedIn == false || RoleManager.UserHasPermission(LoggedInUser.GetUser(), RoleInfo.RolesPermissions.ManageUsers, Lists.Roles.ToList()) == false)
            {
                Response.Redirect("Index.aspx");
            }

            try
            {
                if (!IsPostBack)
                {
                    //retrieve UserID and load user
                    bool result = Int32.TryParse(Request.Params["UserID"], out UserID);
                    if (result == true && UserID > 0)
                    {
                        try
                        {
                            var userMngr = new UserManager();
                            OriginalUser = userMngr.GetUserByID(UserID);
                            UserID = OriginalUser.ID;
                        }
                        catch
                        {
                            UserID = 0;
                            OriginalUser = null;
                            EdittedUser = null;
                        }
                        
                    }
                    else
                    {
                        UserID = 0;
                        OriginalUser = null;
                        EdittedUser = null;
                    }

                    if (OriginalUser == null || UserID <= 0)
                    {
                        OriginalUser = new UserInfo(0, "test", 0, 0, true);
                        OriginalUser.Username = "";
                        var gnrlManager = new GeneralManager();
                        int photoID = gnrlManager.GetPhoto("./system/DefaultUser.jpg");
                        if (photoID > 0)
                        {
                            OriginalUser.Photo = new PhotoInfo(photoID, "./system/DefaultUser.jpg");
                        }
                        else
                            OriginalUser.Photo = new PhotoInfo(0, "./system/DefaultUser.jpg");
                        
                        UserID = 0;
                    }
                        

                    EdittedUser = (UserInfo)OriginalUser.Clone();
                    SetupPage();

                    //setup lists
                    if (Lists.Roles == null || Lists.Roles.Count <= 0)
                    {
                        string notset = (string)GetGlobalResourceObject("NotSet", "0");
                        string[] TaskStatus = LanguageFiles.LoadLanguageFile("TaskStatus");
                        string[] TaskTypes = LanguageFiles.LoadLanguageFile("TaskTypes");
                        string[] RoleNames = LanguageFiles.LoadLanguageFile("Roles");

                        Lists.GetLists(null, TaskStatus, TaskTypes, RoleNames, notset, null);
                    }

                    selectDepartment.DataSource = Lists.Departments;
                    selectDepartment.DataBind();                        
                    var item = selectDepartment.Items.FindByValue(EdittedUser.DepartmentID.ToString());
                    if (item != null)
                    {
                        //select the right department!
                        int index = selectDepartment.Items.IndexOf(item);
                        selectDepartment.SelectedIndex = index;
                    }

                    RolesTable.DataSource = Lists.Roles;
                    RolesTable.DataBind();

                    if (EdittedUser.PhotoID != 0)
                    {
                        var GnrlManager = new GeneralManager();
                        string source = GnrlManager.GetPhoto(EdittedUser.PhotoID);
                        if (!String.IsNullOrWhiteSpace(source))
                        {
                            UserPhoto.ImageUrl = source;
                        }
                    }

                    this.DataBind();
                }
            }
            catch (Exception ex)
            {
                this.Session["exceptionMessage"] = ex.Message;
                Response.Redirect("DisplayError");
            }
        }

        //set the checkbox set or not depending on the user's roles.
        protected void RolesTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int index = e.Row.RowIndex;
            if (
                index < 0 ||
                index >= Lists.Roles.Count
              )
            {
                return;
            }

            var checkbox = e.Row.Cells[1].FindControl("UserRole") as System.Web.UI.HtmlControls.HtmlInputCheckBox;
            if(checkbox != null)
            {
                bool hasRole = false;

                //is the user an admin or not?
                if (RoleManager.UserHasRole(OriginalUser, 1))
                {
                    //incase of admin, enable role and disable other checkboxes
                    hasRole = true;
                    if(index > 0)
                    {
                        checkbox.Disabled = true;
                    }
                }  
                else
                {
                    if(EdittedUser.ID == 0 && Lists.Roles[index].ID == 2)
                    {
                        //new user & UserID found. check it & add it
                        hasRole = true;
                        EdittedUser.UserRoles.Add(Lists.Roles[index]);
                    }
                    else
                    {
                        //does the user have the role?
                        hasRole = RoleManager.UserHasRole(OriginalUser, Lists.Roles[index].ID);
                    }
                }

                checkbox.Checked = hasRole;
                //assign javascript function that will deal with the checkbox being changed
                checkbox.Attributes.Add("onchange", String.Format("updateRole(this,{0},{1}); return false;",Lists.Roles[index].ID,index));
                //assign the alt value that we will use to check if the checkbox is admin or not
                checkbox.Attributes.Add("alt", Lists.Roles[index].Name);

                if (ReadOnly)
                    checkbox.Disabled = true;
            }
                
        }

        //-----------------------------------------------
        //                  WebMethods
        //-----------------------------------------------
        //  AKA functions we can call from javascript

        [WebMethod]
        public static string UserReadOnly()
        {
            string ret = null;

            if (ReadOnly == true)
            {
                ret = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "UserOpenMsg").ToString();
            }

            return ret;
        }

        [WebMethod]
        public static bool NewPropertyValue(string PropertyName, object value)
        {
            bool ret = true;

            Type type = typeof(UserInfo);
            PropertyInfo property = type.GetProperty(PropertyName);

            if (property == null || EdittedUser == null || ReadOnly == true)
                return false;

            bool changed = false;
            try
            {
                switch (PropertyName.ToLower())
                {
                    case "userroles":
                        //ok, this is a whole process, so lets explain.
                        //we receive an Json array from ajax, but we need to convert it into an string array so we can use it decently.
                        //so we deserialise the object, try and cast it as a Json Array, and then convert it into a string array.
                        //once we have that, we try and parse the ID & bool out of the array.
                        //now we know what role we are changing, so get the roleinfo from the lists
                        //once we have the role too, we can see if the user has the role and if we are removing the role or adding them.
                        //and so, it is DONE AND DONE :P
                        var arrData = Newtonsoft.Json.JsonConvert.DeserializeObject(value.ToString());

                        if (arrData == null)
                            break;

                        JArray javascriptArray = arrData as JArray;

                        var temp = javascriptArray.ToObject<string[]>();

                        if (temp == null || temp.Length != 2)
                            break;

                        int ID = 0;
                        bool isRoleChecked = false;
                        bool convertResult = Int32.TryParse(temp[0], out ID);

                        if (convertResult)
                            convertResult = Boolean.TryParse(temp[1], out isRoleChecked);

                        if (convertResult == false || ID <= 0)
                            break;

                        SystemLists Lists = SystemLists.GetInstance();
                        RoleInfo info = null;

                        foreach (RoleInfo item in Lists.Roles)
                        {
                            if (item.ID == ID)
                            {
                                info = item;
                                break;
                            }
                        }

                        if (info == null)
                            break;

                        bool contains = EdittedUser.UserRoles.Contains(info);
                        if (isRoleChecked == true && !contains)
                        {
                            EdittedUser.UserRoles.Add(info);
                        }

                        if(!isRoleChecked && contains == true)
                        {
                            EdittedUser.UserRoles.Remove(info);
                        }

                        if (IsAdmin(info.Name) && isRoleChecked)
                        {
                            foreach (RoleInfo role in Lists.Roles)
                            {
                                if(!EdittedUser.UserRoles.Contains(role))
                                {
                                    EdittedUser.UserRoles.Add(role);
                                }
                            }
                        }

                        changed = true;
                        break;
                    case "active":
                        bool result = false;

                        bool isChecked = false;
                        result = Boolean.TryParse(value.ToString(),out isChecked);

                        if(result == true)
                            EdittedUser.Active = isChecked;

                        changed = true;
                        break;
                    case "departmentid":
                        int depID = 0;
                        if (Int32.TryParse(value as string, out depID))
                            EdittedUser.DepartmentID = depID;

                        changed = true;
                        break;
                    case "username":
                        var username = value as string;
                        if (String.IsNullOrWhiteSpace(username))
                        {
                            changed = true;
                            break;
                        }
                        if(Regex.IsMatch(username, @"^[A-Za-z0-9 ]+$") == false)
                        {
                            changed = false;
                            break;
                        }

                        EdittedUser.Username = username;

                        changed = true;
                        break;
                    case "password":
                        string password = value as string;

                        if (String.IsNullOrWhiteSpace(password) )
                        {
                            changed = true;
                            break;
                        }
                        if (Regex.IsMatch(password, @"^[A-Za-z0-9]+$") == false)
                        {
                            changed = false;
                            break;
                        }

                        EdittedUser.Password = password;

                        changed = true;
                        break;
                    case "photoid":
                        var data = value as string;

                        if (data == null)
                            break;

                        EdittedUser.Photo = new PhotoInfo(0, String.Format("User_{0}_Photo_{1}", EdittedUser.ID, DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")));
                        EdittedUser.Photo.PhotoSource = data;
                        changed = true;
                        break;
                    default:
                        changed = false;
                        break;
                }
            }
            catch(Exception ex)
            {
                changed = false;
            }
            ret = changed;

            return ret;
        }

        /// <summary>
        /// WebMethod to check if the given role name is admin or not. used in javascript to control the checkboxes when assining roles.
        /// </summary>
        /// <param name="rolename"></param>
        /// <returns></returns>
        [WebMethod]
        public static bool IsAdmin(string rolename)
        {
            SystemLists Lists = SystemLists.GetInstance();

            if(Lists == null || Lists.Roles == null || Lists.Roles.Count < 1 )
            {
                return false;
            }

            foreach (RoleInfo role in Lists.Roles)
            {
                if (role.ID == 1 && role.Name == rolename)
                    return true;
            }

            return false;
        }

        [WebMethod]
        public static string SaveUser()
        {
            string errorMsg = String.Empty;
            List<String> IncorrectFields = new List<string>();
            string Empty;

            try
            {
                Empty = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "Empty").ToString();
            }
            catch
            {
                Empty = "Empty";
            }

            if (EdittedUser == null || OriginalUser == null || ReadOnly == true)
            {
                try
                {
                    errorMsg = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "ErrorSaveUser").ToString();
                }
                catch
                {
                    errorMsg = "Function 'SaveTasks' can not be called at this time";
                }
                return errorMsg;
            }

            //Verify required fields!
            if(String.IsNullOrWhiteSpace(EdittedUser.Username))
            {
                try
                {
                    string propName = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "Username.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch (Exception ex)
                {
                    IncorrectFields.Add(String.Format("Username({0})", Empty));
                }
            }

            if (String.IsNullOrWhiteSpace(EdittedUser.Password) && EdittedUser.ID <= 0)
            {
                try
                {
                    string propName = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "Password.Text").ToString();
                    propName = propName.TrimEnd(new char[] { ':' });
                    IncorrectFields.Add(String.Format("{0}({1})", propName, Empty));
                }
                catch (Exception ex)
                {
                    IncorrectFields.Add(String.Format("Password({0})", Empty));
                }
            }

            //set a correct departmentID
            if (EdittedUser.ID == 0)
            {
                SystemLists Lists = SystemLists.GetInstance();
                if (Lists.Departments != null && Lists.Departments.Count > 0)
                    EdittedUser.DepartmentID = Lists.Departments[0].ID;
            }

            if (IncorrectFields.Count > 0)
            {
                try
                {
                    errorMsg = HttpContext.GetLocalResourceObject("~/EditUser.aspx", "ErrorMsg").ToString() + " : " + Environment.NewLine + Environment.NewLine;
                }
                catch
                {
                    errorMsg = "One or more fields are not valid. please recheck the following fields : " + Environment.NewLine + Environment.NewLine;
                }


                foreach (string item in IncorrectFields)
                {
                    errorMsg += (item + Environment.NewLine);
                }
            }

            if (string.IsNullOrWhiteSpace(errorMsg))
            {
                try
                {
                    var usrManager = new UserManager();
                    usrManager.AddOrChangeUser(EdittedUser,Settings.GetAppSetting("company"));
                    errorMsg = string.Empty;
                    //Cleanup the User so this function doesnt get abused
                    CloseUser();
                }
                catch (Exception ex)
                {
                    errorMsg = "an error has occured : " + ex.Message;
                }
            }

            return errorMsg;
        }

        [WebMethod]
        public static bool CloseUser()
        {
            bool ret = true;

            if (ReadOnly || EdittedUser == null || OriginalUser == null)
                return ret;

            try
            {
                var usrManager = new UserManager();
                ret = usrManager.SetUserClosed(EdittedUser.ID, LoggedInUser.GetUserHash());

                if (ret)
                {
                    EdittedUser = null;
                    OriginalUser = null;
                    ReadOnly = false;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
    }
}