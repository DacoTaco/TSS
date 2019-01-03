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
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace TechnicalServiceSystem
{
    /// <summary>
    ///     Class to handle loading and setting of the application Settings & session settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        ///     boolean to check weither we are running in a ASP/MVC web environment or in a Windows(WPF) environment
        /// </summary>
        public static bool IsWebEnvironment => (HttpRuntime.AppDomainAppId != null);


        /// <summary>
        ///     function to load an application setting. In ASP/MVC it will load from web.config, in WPF it will load from
        ///     app.config
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static string GetAppSetting(string settingName)
        {
            var ret = string.Empty;

            if (string.IsNullOrWhiteSpace(settingName))
                return ret;

            if (IsWebEnvironment)
                ret = WebConfigurationManager.AppSettings[settingName];
            else
                ret = ConfigurationManager.AppSettings[settingName];


            return ret;
        }

        public static string GetServerPath()
        {
            string path = @".\";
            if (IsWebEnvironment && String.IsNullOrWhiteSpace(GetAppSetting("ServerPath")))
            {
                path = HttpContext.Current.Server.MapPath(@".\");
            }
            else
            {
                path = GetAppSetting("ServerPath");
                if (String.IsNullOrWhiteSpace(path))
                    path = @".\";
                path = Path.GetFullPath(Path.Combine(path, @".\"));
            }

            return Path.GetFullPath(path);
        }

        public static string GetCompanyName()
        {
            return GetAppSetting("company");
        }

        public static T GetSessionSetting<T>(string setting)
        {
            return (T) GetSessionSetting(setting);
        }

        public static object GetSessionSetting(string setting)
        {
            if (!IsWebEnvironment)
                return null;

            return HttpContext.Current?.Session?[setting];
        }

        public static void SetSessionSetting(string setting, object value)
        {
            if (!IsWebEnvironment || HttpContext.Current?.Session == null)
                return;
            HttpContext.Current.Session[setting] = value;
        }
    }
}