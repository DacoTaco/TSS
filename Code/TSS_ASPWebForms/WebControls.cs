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
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace TSS_ASPWebForms
{
    //if used add <%@ Register assembly="TSS_ASPWebForms" namespace="TSS_ASPWebForms" tagprefix="web" %> at start of page to use classes
    public class selectObject : HtmlSelect
    {
        public int SelectItem(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return SelectedIndex;

            var item = Items.FindByValue(value);
            if (item != null)
            {
                SelectedIndex = Items.IndexOf(item);
            }

            return SelectedIndex;
        }
    }

    public class DropDownObject : DropDownList
    {
        public int SelectItem(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return SelectedIndex;

            var item = Items.FindByValue(value);
            if (item != null)
            {
                SelectedIndex = Items.IndexOf(item);
            }

            return SelectedIndex;
        }
    }

    public class RoleLabel : Label
    {
        public int TranslationKey
        {
            set
            {
                if(value < 0)
                    return;
                var translation = (string) HttpContext.GetGlobalResourceObject("Roles", value.ToString());
                Text = translation;
            }
        }
    }
}