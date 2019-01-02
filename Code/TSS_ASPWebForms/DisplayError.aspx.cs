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
using System.Web.UI;

namespace TSS_ASPWebForms
{
    public partial class DisplayError : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //get and generate string , then display it :)
            var msg = "A general error has occured. please contact the administrator" + Environment.NewLine;
            if (Session["exceptionMessage"] != null &&
                !string.IsNullOrWhiteSpace(Session["exceptionMessage"].ToString()))
            {
                var error = Session["exceptionMessage"].ToString();
                msg += Environment.NewLine + "Error Message : " + Environment.NewLine + error;
                Session["exceptionMessage"] = null;
            }

            lblErrorMessage.Text = msg;
        }
    }
}