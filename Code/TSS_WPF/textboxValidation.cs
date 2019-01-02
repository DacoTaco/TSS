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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace TSS_WPF
{
    public class textboxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string Error;
            Error = (string)Application.Current.TryFindResource("ErrorValidating");

            if (String.IsNullOrWhiteSpace(Error))
                Error = "Error validating text";

            string Failure;
            Failure = (string)Application.Current.TryFindResource("ErrorEmpty");

            if (String.IsNullOrWhiteSpace(Failure))
            {
                Failure = "Text should not be empty!";
            }

            try
            {
                if (String.IsNullOrWhiteSpace(value as string))
                    return new ValidationResult(false, Failure);

            }
            catch
            {
                return new ValidationResult(false, Error);
            }
            return new ValidationResult(true, null);
        }
    }
}
