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

using Equin.ApplicationFramework;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem
{
    [DataObject(true)]
    public class UserManager : Utilities.DatabaseManager
    {
        /// <summary>
        ///     Retrieve User of the given ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        ///
        public User GetUserByID(int UserID)
        {
            try
            {
                return Session.QueryOver<User>().Where(x => x.ID == UserID).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_User_ByID : " + ex.Message, ex);
            }
        }

        /// <summary>
        ///     Retrieve a list of all Users of the given roleName & company. default is 'User' and empty, which should return all
        ///     users.
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="company"></param>
        /// <returns>ObservableCollection<User></returns>
        public ObservableCollection<User> GetUsersByRole(Role role, string company = "")
        {
            var list = new ObservableCollection<User>();

            try
            {
                list = new ObservableCollection<User>(Session
                    .CreateSQLQuery("exec Users.GetUsersByRole :roles, :companyName")
                    .AddEntity(typeof(User))
                    .SetParameter("roles", Enum.GetName(typeof(Role),role))
                    .SetParameter("companyName", string.IsNullOrWhiteSpace(company) ? "%" : company, NHibernateUtil.String)
                    .List<User>());
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_User_ByRole : " + ex.Message, ex);
            }

            return list;
        }

        /// <summary>
        ///     Retrieves a list of all users saved in the database from given company. by default this is all users
        /// </summary>
        /// <param name="company"></param>
        /// <returns>ObservableCollection<UserInfo></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<User> GetUsers(string contains, Role? role, string SortBy = null,bool? activeOnly = true)
        {
            ObservableCollection<User> list = GetUsers(Settings.GetCompanyName(), contains,
                role ?? Role.AllRoles,
                activeOnly);
            var ret = new BindingListView<User>(list.ToList());

            if (!String.IsNullOrWhiteSpace(SortBy))
                ret.Sort = SortBy;

            return ret;
        }

        public ObservableCollection<User> GetUsers(string company, string searchText = null, Role role = Role.AllRoles, bool? activeOnly = true)
        {
            try
            {
                return new ObservableCollection<User>(Session.CreateSQLQuery("exec Users.GetUsers :company, :active, :search, :role")
                    .AddEntity(typeof(User))
                    .SetParameter("company", string.IsNullOrWhiteSpace(company) ? "%" : company)
                    .SetParameter("active", activeOnly.HasValue ? (activeOnly.Value ? 1 : 0) : (object)DBNull.Value, NHibernateUtil.Int32)
                    .SetParameter("search", string.IsNullOrWhiteSpace(searchText) ? "%" : searchText)
                    .SetParameter("role", role != Role.AllRoles ? Enum.GetName(typeof(Role), role) : null)
                    .List<User>());
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_Users : " + ex.Message, ex);
            }
            
        }

        /// <summary>
        ///     Checks user login information. if the login is correct, the user info is altered with a unique user hash and will
        ///     return true
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>bool correct y/n</returns>
        public bool LoginUser(ref User user, string password)
        {
            if (user == null || string.IsNullOrWhiteSpace(password) || user.IsActive == false)
                return false;

            var ret = false;
            var company = "";

            try
            {
                var connection = Session.Connection;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "General.GetDepartmentCompany";
                    if (Session.Transaction.IsActive)
                        Session.Transaction.Enlist(command);

                    var depID = command.CreateParameter();
                    depID.Value = user.Department.ID;
                    depID.ParameterName = "departmentID";
                    command.Parameters.Add(depID);

                    if(connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var rdr = command.ExecuteReader())
                    {
                        var CompanyNamePos = rdr.GetOrdinal("CompanyName");

                        if (rdr.Read()) company = rdr.GetString(CompanyNamePos);

                        if (string.IsNullOrWhiteSpace(company)) throw new Exception("Company not found!");

                        var SettingCompany = Settings.GetCompanyName();

                        if (SettingCompany != company) throw new Exception("User is not from the setup company!");
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Users.CheckLogin";
                    if (Session.Transaction.IsActive)
                        Session.Transaction.Enlist(command);

                    var userID = command.CreateParameter();
                    userID.Value = user.ID;
                    userID.ParameterName = "userID";
                    command.Parameters.Add(userID);

                    var passwordParam = command.CreateParameter();
                    passwordParam.Value = password;
                    passwordParam.ParameterName = "password";
                    command.Parameters.Add(passwordParam);

                    var companyName = command.CreateParameter();
                    companyName.Value = company;
                    companyName.ParameterName = "companyName";
                    command.Parameters.Add(companyName);

                    var userHash = command.CreateParameter();
                    userHash.Direction = ParameterDirection.Output;
                    userHash.ParameterName = "UserHash";
                    userHash.Size = 265;
                    command.Parameters.Add(userHash);

                    command.ExecuteNonQuery();
                    user.Password = string.Empty;

                    if (string.IsNullOrWhiteSpace(userHash.Value.ToString()))
                    {
                        ret = false;
                    }
                    else
                    {
                        user.UserHash = userHash.Value.ToString();
                        if (user.UserHash != null && user.UserHash.Length > 0)
                        {
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                            user.UserHash = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_LoginUser : " + ex.Message, ex);
            }

            return ret;
        }

        /// <summary>
        ///     Check whether the user's hash is correct or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns>boolean</returns>
        public bool CheckUserHash(User user)
        {
            if (user == null || user.ID <= 0 || string.IsNullOrWhiteSpace(user.UserHash))
                return false;

            var ret = false;
            try
            {
                var connection = Session.Connection;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Users.CheckLoginHash";
                    if (Session.Transaction.IsActive)
                        Session.Transaction.Enlist(command);

                    var IDparam = command.CreateParameter();
                    IDparam.Value = user.ID;
                    IDparam.ParameterName = "UserID";
                    command.Parameters.Add(IDparam);

                    var hashParam = command.CreateParameter();
                    hashParam.Value = user.UserHash;
                    hashParam.ParameterName = "UserHash";
                    command.Parameters.Add(hashParam);

                    var retValue = command.CreateParameter();
                    retValue.Direction = ParameterDirection.ReturnValue;
                    retValue.Value = 0;
                    command.Parameters.Add(retValue);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    command.ExecuteNonQuery();
                    var value = retValue.Value as int?;
                    if (!value.HasValue || value.Value <= 0)
                        ret = false;
                    else
                        ret = true;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        //user opened state stuff. its +/- the same as in task manager. for information go check there :P
        public bool UserEditable(int UserID, string userHash)
        {
            var ret = false;

            try
            {
                var connection = Session.Connection;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Users.IsUserEditable";
                    if (Session.Transaction.IsActive)
                        Session.Transaction.Enlist(command);

                    var param = command.CreateParameter();
                    param.ParameterName = "UserID";
                    param.Value = UserID;
                    command.Parameters.Add(param);

                    var hashParam = command.CreateParameter();
                    hashParam.Value = userHash;
                    hashParam.ParameterName = "userHash";
                    command.Parameters.Add(hashParam);

                    var retValue = command.CreateParameter();
                    retValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(retValue);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    bool? temp = null;
                    if (command.ExecuteNonQuery() >= -1)
                        if (retValue.Value != null)
                            temp = retValue.Value.ToString() == "1";

                    if (temp.HasValue)
                        ret = temp.Value;
                    else
                        throw new Exception("SQL error in retrieving User opened state");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Check_Open : ", ex);
            }


            return ret;
        }

        public bool SetUserOpened(int UserID, string userHash)
        {
            return SetUserOpenedState(UserID, userHash, true);
        }

        public bool SetUserClosed(int UserID, string userHash)
        {
            return SetUserOpenedState(UserID, userHash, false);
        }

        protected bool SetUserOpenedState(int UserID, string userHash, bool openedState)
        {
            var ret = false;
            try
            {
                var connection = Session.Connection;
                using (var command = connection.CreateCommand())
                {
                    if (Session.Transaction.IsActive)
                        Session.Transaction.Enlist(command);
                    command.CommandType = CommandType.StoredProcedure;

                    if (openedState)
                        command.CommandText = "Users.SetUserOpened";
                    else
                        command.CommandText = "Users.SetUserClosed";

                    var param = command.CreateParameter();
                    param.ParameterName = "UserID";
                    param.Value = UserID;
                    command.Parameters.Add(param);

                    var hashParam = command.CreateParameter();
                    hashParam.Value = userHash;
                    hashParam.ParameterName = "userHash";
                    command.Parameters.Add(hashParam);

                    var retValue = command.CreateParameter();
                    retValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(retValue);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    bool? temp = null;
                    if (command.ExecuteNonQuery() >= -1)
                        if (retValue.Value != null)
                            temp = retValue.Value.ToString() == "1";

                    if (temp.HasValue)
                        ret = temp.Value;
                    else
                        throw new Exception("SQL error in setting User opened state");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Check_Set_Open : ", ex);
            }

            return ret;
        }

        public bool AddOrChangeUser(User user)
        {
            if (user == null)
                return false;
            
            try
            {
                Session.BeginTransaction();
                try
                {
                    //first handle the image
                    if (user.Photo != null && user.Photo.ID <= 0)
                    {
                        //save photo
                        var gnrlManager = new GeneralManager();
                        Photo photo = user.Photo;
                        gnrlManager.SavePhotoToServer(ref photo);
                        user.Photo = photo;
                    }

                    //user
                    Session.SaveOrUpdate(user);

                    //if the password has a value it means it was set somewhere, so lets push the change to the database through the stored procedure
                    if (!String.IsNullOrWhiteSpace(user.Password))
                    {
                        Session.CreateSQLQuery("exec Users.AssignPassword :UserID, :Password")
                            .SetParameter("UserID", user.ID)
                            .SetParameter("Password", user.Password)
                            .ExecuteUpdate();
                    }

                    Session.Transaction.Commit();
                }
                catch
                {
                    Session.Transaction.Rollback();
                    throw;
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"User_Manager_Failed_Save_User : {ex.Message} {ex?.InnerException?.Message??""}", ex);
            }

            return true;
        }
    }
}