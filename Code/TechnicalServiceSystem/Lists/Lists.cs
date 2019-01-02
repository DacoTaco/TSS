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

using System.ComponentModel;

namespace TechnicalServiceSystem.Lists
{
    //a singleton so we can keep the lists and use them all over the application. not caring about instances, reloading the list etc etc.
    //i do love myself a singleton :P
    //this is a partial class so we can split it up depending on what part of the system we are working on   
    public partial class SystemLists : INotifyPropertyChanged
    {
        //----------------------
        //new implementation
        //----------------------
        public static UserList User = new UserList();
        public static GeneralLists General = new GeneralLists();
        public static TaskList Tasks = new TaskList();
        public static SupplierLists Supplier = new SupplierLists();

        //----------------------
        //singleton business
        //----------------------

        private static SystemLists _instance;
        private static readonly object syncRoot = new object();

        private SystemLists()
        {
            //we could retrieve all lists while init, but for startup time sake... lets not :P
            //GetLists();
        }

        //retrieval of the Lists
        private static SystemLists Instance
        {
            get
            {
                SystemLists ret = null;
                lock (syncRoot)
                {
                    //in ASP we can't use the static stuff since static is the same for all requests/sessions. so savinig to sessions it is xD
                    if (Settings.IsWebEnvironment)
                    {
                        ret = Settings.GetSessionSetting<SystemLists>("SystemLists");
                        if (ret == null)
                        {
                            ret = new SystemLists();
                            Settings.SetSessionSetting("SystemLists", ret);
                        }
                    }
                    else
                    {
                        if (_instance == null) _instance = new SystemLists();
                        ret = _instance;
                    }
                }

                return ret;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     Retrieves the current SystemList Object
        /// </summary>
        /// <returns></returns>
        public static SystemLists GetInstance()
        {
            return Instance;
        }

        //----------------------------
        // List retrieval functions
        //----------------------------

        /// <summary>
        ///     Retrieves all lists from the database. Also sets the text of status descriptions,type descriptions and the not set
        ///     string for machines. this is meant for translations and localisations
        /// </summary>
        /// <param name="StatusDescriptions">string array that contains translations of the Task statuses</param>
        /// <param name="TypeDescriptions">string array that contains translations for the Task Types</param>
        /// <param name="MachineNotSetString">string that contains translation for the words "Not Set"</param>
        public void GetLists(string[] StatusDescriptions, string[] TypeDescriptions, string[] RoleNames,
            string MachineNotSetString = null, int? departmentID = null)
        {
            GetLists(null, StatusDescriptions, TypeDescriptions, RoleNames, MachineNotSetString, departmentID);
        }

        /// <summary>
        ///     Retrieves all lists from the database. Also lets the system give a string that tasks may contain
        ///     and sets the text of status descriptions,type descriptions and the not set string for machines.
        ///     this is meant for translations and localisations
        /// </summary>
        /// <param name="tasksContain">string that may contain within a task</param>
        /// <param name="StatusDescriptions">string array that contains translations of the Task statuses</param>
        /// <param name="TypeDescriptions">string array that contains translations for the Task Types</param>
        /// <param name="RoleNames">string array that contains the translations for the roles</param>
        /// <param name="NotSetString">string that contains translation for the words "Not Set"</param>
        /// <param name="departmentID">ID of the department in which to look for tasks</param>
        public void GetLists(string tasksContain, string[] StatusDescriptions, string[] TypeDescriptions,
            string[] RoleNames, string NotSetString = null, int? departmentID = null)
        {
            //All partial classes of the systemlists class should have a main function that retrieves all lists from that part of the class.
            //then its main function should be placed here so that by calling GetLists it gets the lists of all parts of the system

            //the string arrays containing translations. if they are null we make them empty arrays so it doesn't crash somewhere down the line
            if (StatusDescriptions == null)
                StatusDescriptions = new string[0];
            if (TypeDescriptions == null)
                TypeDescriptions = new string[0];
            if (RoleNames == null)
                RoleNames = new string[0];

            //retrival of the lists
            GetSuppliersLists(NotSetString);
            GetTaskList(tasksContain, StatusDescriptions, TypeDescriptions, departmentID);
        }
    }
}