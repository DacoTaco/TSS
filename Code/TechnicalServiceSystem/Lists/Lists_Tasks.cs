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
using System.Collections.Specialized;
using TechnicalServiceSystem.Entities.Tasks;

namespace TechnicalServiceSystem.Lists
{
    public partial class SystemLists
    {
        public List<Task> DeletedTasks = new List<Task>();
        public List<Task> EditedTasks = new List<Task>();


        //----------------
        // LISTS
        //----------------

        //lists for syncing tasks
        public List<Task> NewTasks = new List<Task>();

        //lists of tasks, tasktypes and taskstatuses. their function is simple
        private ObservableCollection<Task> tasks;

        private ObservableCollection<TaskStatus> taskStatuses;

        private ObservableCollection<TaskType> taskTypes;

        public ObservableCollection<Task> TasksList
        {
            get
            {
                if (tasks == null)
                    tasks = new ObservableCollection<Task>();
                return tasks;
            }
            set
            {
                if (tasks != null)
                    tasks.CollectionChanged -= Tasks_Changed;

                tasks = value;
                tasks.CollectionChanged += Tasks_Changed;

                OnPropertyChanged("Tasks");
            }
        }

        public ObservableCollection<TaskType> TaskTypes
        {
            get
            {
                if (taskTypes == null)
                    taskTypes = new ObservableCollection<TaskType>();

                return taskTypes;
            }
            set
            {
                taskTypes = value;
                OnPropertyChanged("TaskTypes");
            }
        }

        public ObservableCollection<TaskStatus> TaskStatuses
        {
            get
            {
                if (taskStatuses == null)
                    taskStatuses = new ObservableCollection<TaskStatus>();

                return taskStatuses;
            }
            set
            {
                taskStatuses = value;
                OnPropertyChanged("TaskStatuses");
            }
        }

        /// <summary>
        ///     Event handler for the Task List. when attached to the list, it will add tasks to the right lists for syncing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Tasks_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
                foreach (Task item in e.NewItems)
                    //if the same task is in the deleted lists at the same time, it means an assigning was done.
                    //therefor , we do nothing with the item..
                    //tom?
                    if (e.OldItems == null || !e.OldItems.Contains(item))
                        NewTasks.Add(item);

            if (e.OldItems != null && e.OldItems.Count > 0)
                foreach (Task task in e.OldItems)
                    if (!TasksList.Contains(task) &&
                        (e.NewItems == null || !e.NewItems.Contains(task))
                    )
                        DeletedTasks.Add(task);
            OnPropertyChanged("Tasks");
        }


        //----------------------------
        // List retrieval functions
        //----------------------------

        /// <summary>
        ///     Retrieve the list of Tasks that contain the given string and/or are from the given Department
        /// </summary>
        /// <param name="contains"></param>
        /// <param name="DepartmentID"></param>
        public void GetTasks(string contains = "", int? DepartmentID = null)
        {
            try
            {
                //proxy of the datasource manager
                var manager = new TaskManager();
                TasksList = manager.GetTasks(contains,Settings.GetCompanyName(),DepartmentID,null);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskList : " + ex.Message, ex);
            }
        }

        /// <summary>
        ///     Retrieve the list of task status. if a string array is given it will be attempted to be translated
        /// </summary>
        /// <param name="StatusDescriptions"></param>
        public void GetTaskStatuses(string[] StatusDescriptions)
        {
            try
            {
                ObservableCollection<TaskStatus> temp;
                var taskmgr = new TaskManager();
                temp = taskmgr.GetTaskStatuses();

                if (temp != null &&
                    temp.Count > 0 &&
                    StatusDescriptions != null &&
                    StatusDescriptions.Length > 0 &&
                    StatusDescriptions.Length >= temp.Count
                )
                    foreach (var item in temp)
                        item.Description = StatusDescriptions[item.ID];

                TaskStatuses = temp;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskStatuses : " + ex.Message, ex);
            }
        }

        /// <summary>
        ///     Retrieve the list of Task Types. if a string array is given, it will be attempted to be translated
        /// </summary>
        /// <param name="TypeDescriptions"></param>
        private void GetTaskTypes(string[] TypeDescriptions)
        {
            try
            {
                ObservableCollection<TaskType> temp;
                var taskmgr = new TaskManager();
                temp = taskmgr.GetTaskTypes();

                if (temp != null &&
                    temp.Count > 0 &&
                    TypeDescriptions != null &&
                    TypeDescriptions.Length > 0 &&
                    TypeDescriptions.Length >= temp.Count
                )
                    foreach (var item in temp)
                        item.Description = TypeDescriptions[item.ID];

                TaskTypes = temp;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskTypes: " + ex.Message, ex);
            }
        }

        /// <summary>
        ///     General function of the task lists retrieval. this will retrieve all tasks,types and status and attempt
        ///     translations
        /// </summary>
        /// <param name="StatusDescriptions"></param>
        /// <param name="TypeDescriptions"></param>
        /// <param name="DepartmentID"></param>
        public void GetTaskList(string[] StatusDescriptions, string[] TypeDescriptions, int? DepartmentID = null)
        {
            GetTaskList(null, StatusDescriptions, TypeDescriptions, DepartmentID);
        }

        /// <summary>
        ///     General function of the task lists retrieval. this will retrieve all tasks of the given department and/or conains a
        ///     certain string,types and status and attempt translations
        /// </summary>
        /// <param name="contains"></param>
        /// <param name="StatusDescriptions"></param>
        /// <param name="TypeDescriptions"></param>
        /// <param name="DepartmentID"></param>
        public void GetTaskList(string contains, string[] StatusDescriptions, string[] TypeDescriptions,
            int? DepartmentID = null)
        {
            GetTasks(contains, DepartmentID);
            GetTaskStatuses(StatusDescriptions);
            GetTaskTypes(TypeDescriptions);
        }
    }
}