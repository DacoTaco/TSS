﻿/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Lists;
using TechnicalServiceSystem.UI.HTML;
using TSS.Web.Feature.Users;

namespace TSS.Web
{
    public partial class EditUser : Page
    {
        //userID , if user is read only and the error msg
        protected int UserID;
        public string ReadOnlyMsg => LanguageFiles.GetLocalTranslation("UserOpenMsg","");
        public static bool ReadOnly
        {
            get
            {
                var ret = Settings.GetSessionSetting<bool?>("UserReadOnly");
                if (ret.HasValue)
                    return ret.Value;
                return false;
            }
            set { Settings.SetSessionSetting("UserReadOnly", value); }
        }

        protected static User EdittedUser
        {
            get { return Settings.GetSessionSetting<User>("EditUser"); }
            set { Settings.SetSessionSetting("EditUser", value); }
        }

        protected static User OriginalUser
        {
            get { return Settings.GetSessionSetting<User>("OriginalUser"); }
            set { Settings.SetSessionSetting("OriginalUser", value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings.RequireLogin() && LoggedInUser.IsUserLoggedIn == false)
                Response.Redirect("Login.aspx");

            if (LoggedInUser.IsUserLoggedIn == false || RoleManager.UserHasPermission(LoggedInUser.GetUser(), RolesPermissions.ManageUsers) == false)
                Response.Redirect("Index.aspx");

            if (IsPostBack)
                return;

            Page_LoadUser();
            Page_Setup();
            Page_Fill();
        }
        protected void Page_LoadUser()
        {
            EdittedUser = null;
            OriginalUser = null;
            UserID = 0;

            //retrieve UserID and load user
            if (!int.TryParse(Request.Params["UserID"], out UserID) || UserID <= 0)
            {
                OriginalUser = new User(0) 
                { 
                    UserName = "", 
                    IsActive = true,
                    Roles = new List<Role>() 
                    { 
                        Role.User 
                    }
                };

                var gnrlManager = new GeneralManager();
                var photo = gnrlManager.GetPhoto("./system/DefaultUser.jpg");
                if ((photo?.ID ?? 0) > 0)
                    OriginalUser.Photo = photo;
                else
                    OriginalUser.Photo = new Photo(0, "./system/DefaultUser.jpg");

                UserID = 0;                
            }
            else
            {
                var userMngr = new UserManager();
                OriginalUser = userMngr.GetUserByID(UserID);
                UserID = OriginalUser.ID;
            }

            EdittedUser = (User)OriginalUser.Clone();
        }
        protected void Page_Setup()
        {
            ReadOnly = false;
            if (UserID > 0)
            {
                var userHash = LoggedInUser.GetUserHash();
                var usrManager = new UserManager();
                if (!usrManager.UserEditable(UserID, userHash) || !usrManager.SetUserOpened(UserID, userHash))
                {
                    ReadOnly = true;

                    /* Add a warning/alert to the page saying we can't open the task */
                    JavascriptAlert.Show(ReadOnlyMsg);
                }
            }
            else
            {
                ReadOnly = false;
            }
        }
        protected void Page_Fill()
        {
            RolesTable.DataSource = SystemLists.User.Roles.Where(x => x != Role.AllRoles && x != Role.Unknown).ToList();
            var departments = new GeneralManager().GetDepartments(Settings.GetCompanyName());
            selectDepartment.DataSource = departments;
            selectDepartment.DataBind();

            if (EdittedUser.Department == null)
                EdittedUser.Department = departments.First();

            selectDepartment.SelectItem(EdittedUser.Department.ID.ToString());

            if ((EdittedUser.Photo?.ID??0) != 0)
            {
                if (!string.IsNullOrWhiteSpace(EdittedUser.Photo.FileName))
                    UserPhoto.ImageUrl = EdittedUser.Photo.FileName;
            }
    
            DataBind();
        }
        //set the checkbox set or not depending on the user's roles.
        protected void RolesTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e?.Row?.DataItem == null || !(e.Row.DataItem is Role))
                return;

            if (e.Row.RowType != DataControlRowType.DataRow || (Role)e.Row.DataItem == Role.Unknown)
                return;

            var checkbox = e.Row?.Cells[1]?.FindControl("UserRole") as HtmlInputCheckBox;
            var label = e.Row?.Cells[0]?.FindControl("UserRoleName") as RoleLabel;
            if (checkbox == null || label == null)
                return;

            var hasRole = false;
            var role = (Role)e.Row.DataItem;
            label.TranslationKey = (int)role;

            //is the user an admin or not?
            if (RoleManager.UserHasRole(OriginalUser, Role.Admin))
            {
                //incase of admin, enable role and disable other checkboxes
                hasRole = true;
                if (e.Row.RowIndex > 0) checkbox.Disabled = true;
            }
            else
            {
                //does the user have the role?
                hasRole = RoleManager.UserHasRole(OriginalUser, role);
            }

            checkbox.Checked = hasRole;
            //assign javascript function that will deal with the checkbox being changed
            checkbox.Attributes.Add("onchange", $"updateRole(this,'{role}',{e.Row.RowIndex}); return false;");

            if (ReadOnly)
                checkbox.Disabled = true;
        }

        //-----------------------------------------------
        //                  WebMethods
        //-----------------------------------------------
        //  AKA functions we can call from javascript

        [WebMethod]
        public static string UserReadOnly()
            => ReadOnly ? LanguageFiles.GetLocalTranslation("UserOpenMsg", "User info is already opened by another user. the user info will be opened as read only.") : null;

        [WebMethod]
        public static bool CloseUser()
        {
            var ret = true;

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

        [WebMethod]
        public static string SaveUser()
        {
            var errorMsg = string.Empty;
            string Empty = LanguageFiles.GetLocalTranslation("Empty", "Empty");

            if (EdittedUser == null || OriginalUser == null || ReadOnly)
            {
                return LanguageFiles.GetLocalTranslation("ErrorSaveUser", "Function 'SaveTasks' can not be called at this time");
            }

            //Verify required fields!
            if (string.IsNullOrWhiteSpace(EdittedUser.UserName))
                errorMsg += $"{LanguageFiles.GetLocalTranslation("Username.Text", "Username").TrimEnd(':')} ({Empty}){Environment.NewLine}";

            if (EdittedUser.ID <= 0)
            {
                if (string.IsNullOrWhiteSpace(EdittedUser.Password))
                    errorMsg += $"{LanguageFiles.GetLocalTranslation("Password.Text", "Password").TrimEnd(':')} ({Empty}){Environment.NewLine}";

                if(EdittedUser.Department == null)
                    errorMsg += $"{LanguageFiles.GetLocalTranslation("Department.Text", "Department").TrimEnd(':')} ({Empty}){Environment.NewLine}";
            }

            if (!String.IsNullOrWhiteSpace(errorMsg))
            {
                return LanguageFiles.GetLocalTranslation("ErrorMsg",
                           $"One or more fields are not valid. please recheck the following fields : {Environment.NewLine}{Environment.NewLine}") +
                       errorMsg;
            }

            try
            {
                var usrManager = new UserManager();
                usrManager.AddOrChangeUser(EdittedUser);
                //Cleanup the User so this function doesnt get abused
                CloseUser();
            }
            catch (Exception ex)
            {
                return "an error has occured : " + ex.Message;
            }

            return null;
        }

        [WebMethod]
        public static bool NewPropertyValue(string PropertyName, object value)
        {
            Console.WriteLine($"Property : '{PropertyName}'");
            var type = typeof(User);
            var property = type.GetProperty(PropertyName);
            if (property == null || EdittedUser == null || ReadOnly)
                return false;

            var changed = false;
            var regex = "";
            try
            {
                switch (PropertyName.ToLower())
                {
                    case "roles":
                        //we receive our role data through a json object. we try and parse the object and set the user accordingly
                        var changedRole = RoleModel.TryParse(value as string);
                        if (changedRole == null)
                        {
                            changed = false;
                            break;
                        }

                        var roles = SystemLists.User.Roles;

                        if (changedRole.Role == Role.Admin && changedRole.IsChecked)
                        {
                            EdittedUser.Roles.Clear();
                            EdittedUser.Roles = new List<Role>(roles);
                            changed = true;
                            break;
                        }

                        if (!changedRole.IsChecked && EdittedUser.Roles.Any(r => r == changedRole.Role))
                        {
                            var role = EdittedUser.Roles.First(r => r == changedRole.Role);
                            EdittedUser.Roles.Remove(role);
                        }
                        else if(changedRole.IsChecked && !EdittedUser.Roles.Any(r => r == changedRole.Role))
                        {
                            EdittedUser.Roles.Add(roles.First(r => r == changedRole.Role));
                        }
                        changed = true;
                        break;
                    case "password":
                        //does any character in the password not pass our test?
                        regex = @"^(?=.*?[^A-Za-z0-9#?!@$%^&*_+=-]).{1,}$";
                        goto case "GenericString";
                    case "username":
                        regex = @"^(?=.*?[^A-Za-z0-9 ]).{1,}$";
                        goto case "GenericString";
                    case "GenericString":
                        try
                        {
                            var variableType = property.PropertyType;
                            var valueType = value.GetType();
                            var newValue = Convert.ChangeType(value, variableType);
                            var newString = (newValue as string)?.Trim();

                            if (String.IsNullOrWhiteSpace(newString))
                            {
                                changed = true;
                                break;
                            }

                            if (newString.Length < 8 || Regex.IsMatch(newString, regex))
                            {
                                changed = false;
                                break;
                            }                         

                            changed = true;
                            property.SetValue(EdittedUser, newValue);
                            Console.WriteLine($"new value : {property.GetValue(EdittedUser)}");
                        }
                        catch
                        {
                            changed = false;
                        }

                        break;
                    case "photo":
                        var data = value as string;

                        if (data == null)
                            break;

                        EdittedUser.Photo = new Photo(0,$"User_{EdittedUser.ID}_Photo_{DateTime.Now.ToString("yyyyMMdd_HH_mm_ss")}");
                        EdittedUser.Photo.PhotoSource = data;
                        changed = true;
                        break;
                    case "isactive":

                        var isChecked = false;

                        if (bool.TryParse(value.ToString(), out isChecked))
                            EdittedUser.IsActive = isChecked;

                        changed = true;
                        break;
                    case "department":
                        var depID = 0;
                        if (!int.TryParse(value as string, out depID))
                            break;

                        EdittedUser.Department = new GeneralManager().GetDepartment(depID);
                        changed = true;
                        break;
                    default:
                        changed = false;
                        break;
                }
            }
            catch
            {
                changed = false;
            }


            return changed;
        }
    }
}