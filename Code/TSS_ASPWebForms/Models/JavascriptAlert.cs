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

using System.Web;
using System.Web.UI;

namespace TSS_ASPWebForms.Models
{
    /// <summary>
    ///     a static class that injects a javascript alert into the page to display a message upon page rendering/load!
    /// </summary>
    public static class JavascriptAlert
    {
        /// <summary>
        ///     display given message when page loads
        /// </summary>
        /// <param name="message"></param>
        public static void Show(string message)
        {
            // Cleans the message to allow single quotation marks
            var cleanMessage = message.Replace("'", "\\'");
            var script = "<script type=\"text/javascript\">alert('" + cleanMessage + "');</script>";

            // Gets the executing web page
            var page = HttpContext.Current.CurrentHandler as Page;

            // Checks if the handler is a Page and that the script isn't allready on the Page
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alertMsg"))
                page.ClientScript.RegisterClientScriptBlock(typeof(JavascriptAlert), "alertMsg", script);
        }
    }
}