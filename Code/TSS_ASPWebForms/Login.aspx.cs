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
    public partial class Login : System.Web.UI.Page
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

        //Intialise Page. if we are already logged in, direct to index. no need to be here :)
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (LoggedInUser.GetUser() != null)
                {
                    Response.Redirect("Index");
                }

                //load user list
                Lists.GetUserLists();

                userlist.DataSource = Lists.ActiveUsers;
                userlist.DataBind();
            }
            catch (Exception ex)
            {
                this.Session["exceptionMessage"] = ex.Message;
                Response.Redirect("DisplayError");
            }
        }

        //attach the onclick event to every row so when we click on it, we go over to the login page
        protected void userlist_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex > -1)
            {
                e.Row.Attributes["onclick"] = "getLoginPage(" + e.Row.RowIndex + ")";
            }
        }

        [WebMethod]
        public static bool RequireLogin()
        {
            bool ret = false;

            try
            {
                string requireLogin = Settings.GetAppSetting("RequireLogin");
                if (String.IsNullOrWhiteSpace(requireLogin) || requireLogin != "0")
                    ret = true;
            }
            catch
            {
                ret = false;
            }

            return ret;
        }
    }
}