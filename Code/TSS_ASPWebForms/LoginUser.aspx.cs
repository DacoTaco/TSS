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

using System;
using System.Web.Services;
using System.Web.UI;
using TechnicalServiceSystem;

namespace TSS_ASPWebForms
{
    public partial class LoginUser : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedInUser.GetUser() != null)
                Response.Redirect("Index");

            if (IsPostBack)
                return;

            var stringID = Request.Params["ID"];

            if (string.IsNullOrEmpty(stringID))
                Response.Redirect("Login");

            var lists = new UserManager().GetUsers(null, null, $"{nameof(TechnicalServiceSystem.Entities.Users.User.UserName)} ASC");

            userList.DataSource = lists;
            userList.DataBind();

            userList.SelectItem(stringID);
        }

        [WebMethod]
        public static bool Login(int userID, string password)
        {
            try
            {
                if (userID == 0)
                    return false;

                var ret = false;
                var usrMngr = new UserManager();
                var user = usrMngr.GetUserByID(userID);

                if (user != null)
                    ret = usrMngr.LoginUser(ref user, password);

                if (ret)
                    LoggedInUser.SetUser(user);
                else
                    LoggedInUser.SetUser(null);

                return ret;
            }
            catch
            {
                LoggedInUser.SetUser(null);
                return false;
            }
        }
    }
}