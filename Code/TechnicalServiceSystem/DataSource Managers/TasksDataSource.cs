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
using System.Linq;
using System.Text;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Equin.ApplicationFramework;
using System.Web;

namespace TechnicalServiceSystem.DataSourceManagers
{
    /// <summary>
    /// class to help with DataSources. basically takes everything and passes it on to the TaskManager
    /// </summary>
    [DataObject(true)]
    public class TasksManager
    {
        //select functions!
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ObservableCollection<Task> GetTasks()
        {
            return GetTasks(null, null);
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ObservableCollection<Task> GetTasks(string contains, int? DepartmentID)
        {
            ObservableCollection<Task> ret = null;
            var Tasks = GetTasks(contains, DepartmentID, null);

            if(Tasks != null)
            {
                ret = new ObservableCollection<Task>(Tasks.ToList());
            }

            return ret;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<Task> GetTasks(string contains, int? DepartmentID = null, string SortBy = null)
        {
            int? depID = null;

            if (DepartmentID.HasValue)
                depID = DepartmentID;

            return GetTasks(false,false,contains, depID, SortBy);

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<Task> GetTasks(bool showAll, bool showRepeating, string contains, int? DepartmentID = null, string SortBy = null)
        {
            BindingListView<Task> ret = null;
            int depID = -1;
            string search = null;

            try
            {
                string company = Settings.GetAppSetting("company");
                var taskmgr = new TaskManager();

                if(!String.IsNullOrWhiteSpace(contains))
                    search = contains;
                else
                {
                    string SearchParam = string.Empty;

                    if (Settings.IsWebEnvironment)
                        SearchParam = Settings.GetSessionSetting<string>("SearchText");

                    if (!String.IsNullOrWhiteSpace(SearchParam))
                        search = SearchParam;
                }

                //for some odd reason when we assign 0 it turns into null? weird shit...
                if (DepartmentID.HasValue && DepartmentID >= 0)
                    depID = DepartmentID.Value;

                if(depID <= 0)
                {
                    int? dep = null;

                    if (Settings.IsWebEnvironment)
                        dep = Settings.GetSessionSetting<int?>("SearchDepartmentID");

                    if (dep.HasValue && dep >= -5)
                    {
                        depID = dep.Value;
                    }
                }

                ret = new BindingListView<Task>(taskmgr.GetTasks(search, company, depID));
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskList : " + ex.Message, ex);
            }

            if (ret != null)
            {
                if (!string.IsNullOrWhiteSpace(SortBy))
                    ret.Sort = SortBy;

                //update the systemlist tasks so we are insync with the rest of the system
                SystemLists Lists = SystemLists.GetInstance();
                Lists.Tasks = new ObservableCollection<Task>(ret.ToList());
            }

            return ret;
        }

        //update functions!
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateTasks(List<ChangedTask> tasks)
        {
            try
            {
                var taskmgr = new TaskManager();

                taskmgr.ChangeTasks(tasks);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Set_Tasks : " + ex.Message, ex);
            }
        }
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateTask(ChangedTask task)
        {
            if (task == null)
                return;

            List<ChangedTask> tasks = new List<ChangedTask>();
            tasks.Add(task);
            UpdateTasks(tasks);
        }
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateTask(Task task)
        {
            if (task == null)
                return;

            List<ChangedTask> tasks = new List<ChangedTask>();
            ChangedTask changed = ChangedTask.UpgradeBase(task, true);

            tasks.Add(changed);
            UpdateTasks(tasks);
        }


        //Insert functions!
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void InsertTasks(List<Task> tasks)
        {
            try
            {
                var taskmngr = new TaskManager();
                taskmngr.AddTasks(tasks);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Insert_Tasks : " + ex.Message, ex);
            }
        }
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void InsertTask(Task task)
        {
            if (task == null)
                return;

            List<Task> tasks = new List<Task>();
            tasks.Add(task);
            InsertTasks(tasks);
        }

    }
}
