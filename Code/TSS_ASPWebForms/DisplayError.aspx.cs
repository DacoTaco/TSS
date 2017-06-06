﻿/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TSS_ASPWebForms
{
    public partial class DisplayError : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //get and generate string , then display it :)
            string msg = "A general error has occured. please contact the administrator" + Environment.NewLine;
            if (this.Session["exceptionMessage"] != null && !String.IsNullOrWhiteSpace(this.Session["exceptionMessage"].ToString()))
            {
                string error = this.Session["exceptionMessage"].ToString();
                msg += Environment.NewLine + "Error Message : " + Environment.NewLine + error;
                this.Session["exceptionMessage"] = null;
            }
            lblErrorMessage.Text = msg;
        }
    }
}