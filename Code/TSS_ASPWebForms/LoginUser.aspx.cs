
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
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TSS_ASPWebForms
{
    public partial class LoginUser : System.Web.UI.Page
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (
                LoggedUser.GetUser() != null
              )
            {
                Response.Redirect("Index");
            }

            int userIndex = -1;
            string stringID = Request.Params["ID"];
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(stringID) ||
                    Int32.TryParse(stringID, out userIndex) == false
                    )
                {
                    Response.Redirect("Login");
                }
                else
                {
                    if(Lists.ActiveUsers == null || Lists.ActiveUsers.Count == 0)
                        Lists.GetUserLists();

                    userList.DataSource = Lists.ActiveUsers;
                    userList.DataBind();

                    userList.SelectedIndex = userIndex;
                }
            }
        }

        protected static bool Login(ref UserInfo user,string password)
        {
            var usrMngr = new UserManager();

            bool loggedin = usrMngr.LoginUser(ref user, password);
            //lblLogin.Text += " - " + (loggedin ? "true" : "false");
            return loggedin;
        }
        [WebMethod]
        public static bool Login(int userID,string password)
        {
            try
            {
                if (userID == 0)
                    return false;

                bool ret = false;
                SystemLists Lists = SystemLists.GetInstance();
                Lists.GetUserLists();

                if (Lists.Users == null || Lists.Users.Count == 0)
                {
                    Lists.GetUserLists();
                    if (Lists.Users.Count == 0)
                        return false;
                }

                UserInfo user = null;
                foreach (UserInfo item in Lists.Users)
                {
                    if (item.ID == userID)
                    {
                        user = item;
                        break;
                    }
                }

                if (user == null)
                    return false;

                ret = Login(ref user, password);

                if (ret)
                    LoggedUser.SetUser(user);
                else
                    LoggedUser.SetUser(null);

                return ret;
            }
            catch
            {
                return false;
            }
        }
    }
}