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
using System.Web.UI.WebControls;

namespace TechnicalServiceSystem.UI.HTML
{
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
}
