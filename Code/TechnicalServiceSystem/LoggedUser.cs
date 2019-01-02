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
using System.Web;
using Remotion.Linq.Utilities;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem
{
    /// <summary>
    ///     class to handle the logged in user
    /// </summary>
    public static class LoggedInUser
    {
        private static User LoggedUser;

        /// <summary>
        ///     boolean that indicates if a user is logged in or not
        /// </summary>
        public static bool IsUserLoggedIn
        {
            get
            {
                var user = GetUser();

                //verify weither the user is legit and has gotten a hash
                //i could verify if the hash is correct with UserManager.CheckUserHash but i think this might slow down things alot!
                //so we only do it when setting the user
                if (
                    user == null ||
                    user.ID == 0 ||
                    string.IsNullOrWhiteSpace(user.UserHash) ||
                    user.IsActive == false
                )
                    return false;
                return true;
            }
        }

        /// <summary>
        ///     return the logged in UserInfo
        /// </summary>
        /// <returns></returns>
        public static User GetUser() => (Settings.IsWebEnvironment)
            ? Settings.GetSessionSetting<User>("LoggedInUser")
            : LoggedUser;


        /// <summary>
        ///     Return the current user's hash. if no user is logged in, the SessionID or MachineName is returned
        /// </summary>
        /// <returns></returns>
        public static string GetUserHash()
        {
            var hash = string.Empty;
            if (!IsUserLoggedIn)
            {
                return (Settings.IsWebEnvironment)
                    ? HttpContext.Current.Session.SessionID
                    : Environment.MachineName;
            }

            var user = GetUser();
            if (string.IsNullOrWhiteSpace(user.UserHash))
                throw new ArgumentEmptyException("User Hash can not be null or empty!");
                //hash = string.Format("{0}:{1}", user.ID, user.Username);

            return user.UserHash;
        }

        /// <summary>
        ///     set the userinfo of the logged user. this also checks whether the user is actually allowed to be set as logged in
        /// </summary>
        /// <param name="user"></param>
        public static void SetUser(User user)
        {
            var usrManager = new UserManager();
            //here we do check whether if the user hash a valid hash or not!
            if (
                user == null ||
                user.ID == 0 ||
                user.IsActive == false ||
                string.IsNullOrWhiteSpace(user.UserHash) ||
                usrManager.CheckUserHash(user) == false
            )
                user = null;

            if (Settings.IsWebEnvironment)
                Settings.SetSessionSetting("LoggedInUser", user);
            else
                LoggedUser = user;


        }
    }
}