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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TechnicalServiceSystem
{
    /// <summary>
    /// Language class to handle loading Translation text. Loads from APP_LocalResources in ASP/MVC and from dictionaries in the LANG folder in WPF
    /// </summary>
    static public class LanguageFiles
    {
        //WPF functions
        static public void LoadLanguageFile(Window window)
        {
            if( window == null ||
                String.IsNullOrEmpty(window.Name))
            {
                throw new ArgumentNullException("LanguageFiles_Failed_Load(window) : arguments can not be null or empty");
            }

            LoadLanguageFile(window, window.Name);
        }
        static public void LoadLanguageFile(object objectToLoadInto , string fileName)
        {
            //retrieve the collection in which all dictionaries are loaded
            Collection<ResourceDictionary> collection = null;
            if ((objectToLoadInto as Window) != null)
            {
                Window source = objectToLoadInto as Window;
                collection = source.Resources.MergedDictionaries;
            }
            else if((objectToLoadInto as Application) != null)
            {
                Application source = objectToLoadInto as Application;
                collection = source.Resources.MergedDictionaries;
            }



            if (String.IsNullOrEmpty(fileName) || collection == null)
                throw new ArgumentNullException("LanguageFiles_Failed_Load(window,windowName) : arguments can not be null or empty");


            //first off all, load in the resources into the merged dictionary
            ResourceDictionary dict = new ResourceDictionary();
            try
            {
                string filename = String.Format(@".\Lang\{0}_{1}.xaml",fileName, ConfigurationManager.AppSettings["Lang"]);
                if (!System.IO.File.Exists(filename))
                    filename = String.Format(@".\Lang\{0}_EN.xaml", fileName, ConfigurationManager.AppSettings["Lang"]);

                dict.Source = new Uri(filename, UriKind.Relative);
                collection.Add(dict);
            }
            catch
            {
                //fall back on the internal English text
                try
                {
                    dict.Source = new Uri(@"pack://application:,,,/TSS_WPF;component/Lang/MainWindow_EN.xaml");
                    collection.Add(dict);
                }
                catch (Exception ex)
                {
                    throw new Exception("LanguageFiles_Failed_Load(window,windowName) : " + ex.Message, ex);
                }
            }
        }


        //ASP.NET Resource Functions
        /// <summary>
        /// Loads the strings from a APP_GlobalResources given Resource name
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        static public string[] LoadLanguageFile(string ResourceName)
        {
            if (String.IsNullOrWhiteSpace(ResourceName))
                return null;

            string[] ret = null;

            string filename = "Resources." + ResourceName;
            try
            {
                //get the manager, get the resource set and return it.
                ResourceManager manager = new ResourceManager(filename, Assembly.Load("App_GlobalResources"));
                ResourceSet set = manager.GetResourceSet(System.Threading.Thread.CurrentThread.CurrentCulture, true, true);
                if (set != null)
                {
                    System.Collections.IDictionaryEnumerator enumerator = set.GetEnumerator();

                    SortedDictionary<string, string> sortedResources = new SortedDictionary<string, string>();

                    while (enumerator.MoveNext())
                    {
                        sortedResources.Add((string)enumerator.Key, (string)enumerator.Value);
                    }

                    ret = sortedResources.Values.ToArray();
                }
            }
            catch(Exception ex)
            {
                ret = null;
            }

            return ret;
        }
    }
}
