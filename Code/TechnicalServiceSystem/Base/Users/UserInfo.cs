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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Class containing all user information
    /// </summary>
    public class UserInfo : BaseClass
    {

        //---------------------
        //      Properties
        //---------------------

        //User's Username
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        //User's Password. used for login and creating user
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        //User's Hash. this is used to identify itself once logged in
        private string userHash;
        public string UserHash
        {
            get { return userHash; }
            set { userHash = value; }
        }

        //User's home department where he/she/it works
        private int departmentID;
        public int DepartmentID
        {
            get { return departmentID; }
            set { departmentID = value; }
        }

        //picture ID of the user. if the PhotoInfo is not null it will use it's ID. else it will return its known ID
        private int photoID;
        public int PhotoID
        {
            get
            {
                int id = photoID;

                if (Photo != null)
                    id = Photo.ID;

                return id;
            }
            set
            {
                int id = value;
                if (Photo != null)
                    Photo.ID = id;
                photoID = id;
            }
        }

        //actual Photoinfo of the the user.
        private PhotoInfo photo;
        public PhotoInfo Photo
        {
            get {
                if (photo == null)
                    photo = new PhotoInfo(0, "");
                return photo;
            }
            set { photo = value; }
        }

        //is the user active and usable?
        private bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        //all roles assigned to the user
        private ObservableCollection<RoleInfo> roles;
        public ObservableCollection<RoleInfo> UserRoles
        {
            get
            {
                if (roles == null)
                    roles = new ObservableCollection<RoleInfo>();

                return roles;
            }
            set { roles = value; }
        }



        /// <summary>
        /// Initialise User, set all given properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="department"></param>
        /// <param name="photoid"></param>
        /// <param name="activeUser"></param>
        private void Init(int id,string name,int department,int photoid,bool activeUser)
        {
            //we dont check on ID because ID 0 means unregistered in our logic.
            //we also dont check on Department and photo incase of new users when its not set yet
            if(
                String.IsNullOrEmpty(name)
              )
            {
                throw new ArgumentException("UserInfo_Failed_Init : parameters have invalid values");
            }
            
            ID = id;
            Username = name;
            DepartmentID = department;
            PhotoID = photoid;
            Active = activeUser;
        }

        /// <summary>
        /// default constructor of the User's Info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="department"></param>
        /// <param name="photoid"></param>
        /// <param name="activeUser"></param>
        public UserInfo(int id,string name,int department,int photoid,bool activeUser)
        {
            Init(id, name, department, photoid,activeUser);
        }

        /// <summary>
        /// ToString Override to return username instead of Type
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Username;
        }

        //this is commented as it was written in attempt to kill a bug.
        //however, the bug was elsewhere and is now fixed. so untill we need this, im leaving this commented
        /*public override bool Equals(object obj)
        {
           // return base.Equals(obj);
            if (obj == null)
            {
                return false;
            }

            try
            {
                Type objType = obj.GetType();
                Type thisType = this.GetType();

                if ((obj as UserInfo == null) || (this as UserInfo == null))
                {
                    return false;
                }

                //ok, at this point we can be sure these objects are the same type
                UserInfo input = obj as UserInfo;

                if (
                    (this.ID == input.ID) &&
                    (this.Username == input.Username)
                    )
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }
        public override int GetHashCode()
        {
            //return base.GetHashCode();
            //found online. it gets the values and calculates a Hash that is semi unique thanks to the properties given.
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)17;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 23) ^ this.ID.GetHashCode();
                hash = (hash * 23) ^ this.Username.GetHashCode();
                return hash;
            }
        }*/

        /// <summary>
        /// Generate a DataTable used by TSS as Userinformation. used for tasks in the database
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        static public DataTable GenerateUserTable(int UserID, string UserName)
        {
            DataTable table = new DataTable();
            table.Columns.Add("userID",typeof(int));
            table.Columns.Add("userName",typeof(string));

            table.Rows.Add(new object[] { UserID, UserName });

            return table;
        }
    }
}
