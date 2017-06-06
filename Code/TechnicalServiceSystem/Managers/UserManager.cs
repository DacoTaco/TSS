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
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem
{
    public class UserManager : DatabaseManager
    {
        /// <summary>
        /// Retrieve UserInfo of the given ID
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public UserInfo GetUserByID(int UserID)
        {
            UserInfo user = null;
            try
            {
                using (var connection = this.GetConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.GetUserByID";
                        var param = command.CreateParameter();
                        param.ParameterName = "userID";
                        param.Value = UserID;
                        command.Parameters.Add(param);


                        using (var rdr = command.ExecuteReader())
                        {
                            //public UserInfo(int id,string name,string department,int photoid,bool activeUser)
                            Int32 UsernamePos = rdr.GetOrdinal("Username");
                            Int32 DepartmentPos = rdr.GetOrdinal("DepartmentID");
                            Int32 PhotoIDPos = rdr.GetOrdinal("PhotoID");
                            Int32 ActivePos = rdr.GetOrdinal("Active");

                            if (rdr.Read())
                            {
                                user = new UserInfo
                                    (
                                    UserID,
                                    rdr.GetString(UsernamePos),
                                    rdr.GetInt32(DepartmentPos),
                                    rdr.GetInt32(PhotoIDPos),
                                    rdr.GetBoolean(ActivePos)
                                    );

                                var gnrlManager = new GeneralManager();
                                int id = user.PhotoID;
                                string name = gnrlManager.GetPhoto(id);
                                user.Photo = new PhotoInfo(id, name);
                            }
                            else
                                return null;
                        }
                    }

                    if (user == null)
                        return null;

                    //get all dem user roles!
                    ObservableCollection<RoleInfo> roles = new ObservableCollection<RoleInfo>();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "Users.GetUserRoles";
                        command.CommandType = CommandType.StoredProcedure;
                        var param = command.CreateParameter();
                        param.ParameterName = "userID";
                        param.Value = UserID;
                        command.Parameters.Add(param);

                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 roleID = rdr.GetOrdinal("RoleID");
                            Int32 roleName = rdr.GetOrdinal("RoleName");

                            while (rdr.Read())
                            {
                                if (rdr.IsDBNull(roleID) || rdr.IsDBNull(roleName))
                                    continue;

                                roles.Add(new RoleInfo(
                                    rdr.GetInt32(roleID),
                                    rdr.GetString(roleName)
                                    ));
                            }
                        }
                    }
                    user.UserRoles = roles;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_User_ByID : " + ex.Message, ex);
            }
            return user;
        }

        /// <summary>
        /// Retrieve a list of all Users of the given roleName & company. default is 'User' and empty, which should return all users. 
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="company"></param>
        /// <returns>ObservableCollection<UserInfo></returns>
        public ObservableCollection<UserInfo> GetUsersByRole(string rolename = "User", string company = "")
        {
            ObservableCollection<UserInfo> list = new ObservableCollection<UserInfo>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.GetUsersByRole";

                        var role = command.CreateParameter();
                        role.ParameterName = "roles";
                        role.Value = rolename;
                        command.Parameters.Add(role);

                        if (!String.IsNullOrWhiteSpace(company))
                        {
                            var companyParam = command.CreateParameter();
                            companyParam.Value = company;
                            companyParam.ParameterName = "companyName";
                            command.Parameters.Add(companyParam);
                        }

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {

                            Int32 UserIDPos = rdr.GetOrdinal("UserID");
                            Int32 UsernamePos = rdr.GetOrdinal("Username");
                            while (rdr.Read())
                            {
                                int id = rdr.GetInt32(UserIDPos);
                                string name = rdr.GetString(UsernamePos);

                                UserInfo user = GetUserByID(id);
                                if (user != null)
                                    list.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_User_ByRole : " + ex.Message, ex);
            }

            return list;
        }

        /// <summary>
        /// Retrieves a list of all users saved in the database from given company. by default this is all users
        /// </summary>
        /// <param name="company"></param>
        /// <returns>ObservableCollection<UserInfo></returns>
        public ObservableCollection<UserInfo> GetUsers(string company = "",string searchText = "",int roleID = 0)
        {
            ObservableCollection<UserInfo> ret = new ObservableCollection<UserInfo>();
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.GetUsers";

                        if (!String.IsNullOrWhiteSpace(company))
                        {
                            var companyParam = command.CreateParameter();
                            companyParam.Value = company;
                            companyParam.ParameterName = "companyName";
                            command.Parameters.Add(companyParam);
                        }

                        if(!String.IsNullOrWhiteSpace(searchText))
                        {
                            var search = command.CreateParameter();
                            search.Value = searchText;
                            search.ParameterName = "search";
                            command.Parameters.Add(search);
                        }

                        if(roleID > 0)
                        {
                            var department = command.CreateParameter();
                            department.ParameterName = "RoleID";
                            department.Value = roleID;
                            command.Parameters.Add(department);
                        }

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {

                            Int32 UserIDPos = rdr.GetOrdinal("User ID");
                            while (rdr.Read())
                            {
                                int id = rdr.GetInt32(UserIDPos);

                                UserInfo user = GetUserByID(id);
                                if (user != null)
                                    ret.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_Users : " + ex.Message, ex);
            }


            return ret;
        }

        public ObservableCollection<RoleInfo> GetRoles()
        {
            ObservableCollection<RoleInfo> ret = new ObservableCollection<RoleInfo>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "Select * from Users.Roles";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {

                            Int32 RoleIDPos = rdr.GetOrdinal("RoleID");
                            Int32 RoleNamePos = rdr.GetOrdinal("RoleName");
                            while (rdr.Read())
                            {
                                ret.Add(new RoleInfo(rdr.GetInt32(RoleIDPos), rdr.GetString(RoleNamePos)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Get_Users : " + ex.Message, ex);
            }
            return ret;
        }
        /// <summary>
        /// Checks user login information. if the login is correct, the user info is altered with a unique user hash and will return true
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>bool correct y/n</returns>
        public bool LoginUser(ref UserInfo user, string password)
        {
            if (user == null || String.IsNullOrWhiteSpace(password) || user.Active == false)
                return false;

            bool ret = false;
            string company = "";

            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "General.GetDepartmentCompany";

                        var depID = command.CreateParameter();
                        depID.Value = user.DepartmentID;
                        depID.ParameterName = "departmentID";
                        command.Parameters.Add(depID);

                        connection.Open();

                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 CompanyNamePos = rdr.GetOrdinal("CompanyName");

                            if (rdr.Read())
                            {
                                company = rdr.GetString(CompanyNamePos);
                            }

                            if (String.IsNullOrWhiteSpace(company))
                            {
                                throw new Exception("Company not found!");
                            }

                            string SettingCompany = Settings.GetAppSetting("company");

                            if (SettingCompany != company)
                            {
                                throw new Exception("User is not from the setup company!");
                            }

                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.CheckLogin";

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

                        if (String.IsNullOrWhiteSpace(userHash.Value.ToString()))
                        {
                            ret = false;
                        }
                        else
                        {
                            user.UserHash = userHash.Value.ToString();
                            if (user.UserHash != null && user.UserHash.Length > 0)
                                ret = true;
                            else
                            {
                                ret = false;
                                user.UserHash = string.Empty;
                            }
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
        /// Check whether the user's hash is correct or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns>boolean</returns>
        public bool CheckUserHash(UserInfo user)
        {
            if (user == null || user.ID <= 0 || String.IsNullOrWhiteSpace(user.UserHash))
                return false;

            bool ret = false;
            //Users.CheckLoginHash @UserID,@UserHash
            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.CheckLoginHash";

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

                        connection.Open();

                        command.ExecuteNonQuery();
                        int? value = retValue.Value as int?;
                        if (!value.HasValue || value.Value <= 0)
                            ret = false;
                        else
                            ret = true;
                    }
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
            bool ret = false;

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Users.IsUserEditable";

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

                        connection.Open();
                        bool? temp = null;
                        if (command.ExecuteNonQuery() >= -1)
                        {
                            if (retValue.Value != null)
                                temp = (retValue.Value.ToString() == "1");
                        }

                        if (temp.HasValue)
                            ret = temp.Value;
                        else
                            throw new Exception("SQL error in retrieving User opened state");

                    }
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
        public bool SetUserOpenedState(int UserID, string userHash, bool openedState)
        {
            bool ret = false;
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
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

                        connection.Open();
                        bool? temp = null;
                        if (command.ExecuteNonQuery() >= -1)
                        {
                            if (retValue.Value != null)
                                temp = (retValue.Value.ToString() == "1");
                        }

                        if (temp.HasValue)
                            ret = temp.Value;
                        else
                            throw new Exception("SQL error in setting User opened state");

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("User_Manager_Failed_Check_Set_Open : ", ex);
            }

            return ret;
        }

        public bool AddUser(List<UserInfo> users, string company = "")
        {
            return AddOrChangeUser(users, company);
        }
        public bool AddUser(UserInfo user, string company = "")
        {
            List<UserInfo> users = new List<UserInfo>();
            users.Add(user);

            return AddOrChangeUser(users, company);
        }

        public bool ChangeUser(List<UserInfo> users, string company = "")
        {
            return AddOrChangeUser(users, company);
        }
        public bool ChangeUser(UserInfo user, string company = "")
        {
            List<UserInfo> users = new List<UserInfo>();
            users.Add(user);
    
            return AddOrChangeUser(users, company);
        }

        public bool AddOrChangeUser(List<UserInfo> users, string company = "")
        {
            if (users == null || users.Count <= 0)
                return false;

            try
            {
                using (var connection = GetConnection())
                {
                    if (!(connection is SqlConnection))
                        return false;

                    SqlConnection con = connection as SqlConnection;
                    con.Open();

                    foreach (UserInfo user in users)
                    {
                        if (user == null || (user.ID == 0 && String.IsNullOrWhiteSpace(user.Password)) || user.ID < 0)
                            continue;

                        int userID = 0;
                        int photoID = 0;

                        if (user.ID > 0)
                            userID = user.ID;

                        using (var trans = con.BeginTransaction())
                        {
                            try
                            {
                                //before doing the user stuff we need to make sure the photo is saved to server! :)
                                if (user.PhotoID == 0 && user.Photo != null && !String.IsNullOrWhiteSpace(user.Photo.PhotoSource))
                                {
                                    //the photo ID is 0, and has data, so time to add it!
                                    var gnrlManager = new GeneralManager();
                                    var photoInfo = user.Photo;
                                    photoID = gnrlManager.SavePhotoToServer(ref photoInfo);
                                }
                                else
                                {
                                    photoID = user.PhotoID;
                                }

                                if(photoID <= 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }

                                using (SqlCommand command = con.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandType = CommandType.StoredProcedure;
                                    if (user.ID > 0)
                                    {
                                        command.CommandText = "Users.ChangeUser";

                                        var paramID = command.CreateParameter();
                                        paramID.Value = user.ID;
                                        paramID.ParameterName = "UserID";
                                        command.Parameters.Add(paramID);
                                    }
                                    else
                                    {
                                        command.CommandText = "Users.CreateUser";
                                    }

                                    var paramPhoto = command.CreateParameter();
                                    paramPhoto.ParameterName = "PhotoID";
                                    paramPhoto.Value = photoID;
                                    command.Parameters.Add(paramPhoto);

                                    var paramUserName = command.CreateParameter();
                                    paramUserName.Value = user.Username;
                                    paramUserName.ParameterName = "Username";
                                    command.Parameters.Add(paramUserName);

                                    if(
                                        (user.ID > 0 && !String.IsNullOrWhiteSpace(user.Password)) ||
                                        user.ID == 0)
                                    {
                                        var paramPassword = command.CreateParameter();
                                        paramPassword.Value = user.Password;
                                        paramPassword.ParameterName = "Password";
                                        command.Parameters.Add(paramPassword);
                                    }

                                    var paramUserActive = command.CreateParameter();
                                    paramUserActive.ParameterName = "Active";
                                    paramUserActive.Value = user.Active;
                                    command.Parameters.Add(paramUserActive);

                                    var paramDepartmentID = command.CreateParameter();
                                    paramDepartmentID.ParameterName = "DepartmentID";
                                    paramDepartmentID.Value = user.DepartmentID;
                                    command.Parameters.Add(paramDepartmentID);

                                    var paramCompany = command.CreateParameter();
                                    paramCompany.ParameterName = "CompanyName";
                                    paramCompany.Value = company;
                                    command.Parameters.Add(paramCompany);

                                    SqlParameter paramRoles = command.CreateParameter();
                                    paramRoles.SqlDbType = SqlDbType.Structured;
                                    paramRoles.ParameterName = "Roles";
                                    paramRoles.Value = RoleInfo.GenerateRolesTable(user.UserRoles.ToList());
                                    command.Parameters.Add(paramRoles);

                                    var retValue = command.CreateParameter();
                                    retValue.Direction = ParameterDirection.ReturnValue;
                                    retValue.Value = null;
                                    command.Parameters.Add(retValue);

                                    if (command.ExecuteNonQuery() <= 0 || retValue.Value == null || (retValue.Value as int?) == null)
                                    {
                                        trans.Rollback();
                                        continue;
                                    }

                                    if(user.ID == 0)
                                    {
                                        user.ID = (int)retValue.Value;
                                        userID = (int)retValue.Value;
                                    }

                                    if(userID <= 0)
                                    {
                                        trans.Rollback();
                                        continue;
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                trans.Rollback();
                                continue;
                            }
                            trans.Commit();
                        }
                    }
                }
            }
            catch
            {

            }

            return true;
        }
        public bool AddOrChangeUser(UserInfo user, string company = "")
        {
            if(user == null)
            {
                return false;
            }

            List<UserInfo> users = new List<UserInfo>();
            users.Add(user);
            return AddOrChangeUser(users,company);
        }
    }
}
