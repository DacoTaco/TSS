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
using System.Collections.ObjectModel;
using System.Linq;
using TechnicalServiceSystem.Entities.Tasks;

namespace TechnicalServiceSystem.Lists
{
    public class TaskList
    {
        private readonly string TaskStatusList = "TaskStatusList";
        private readonly string TaskTypesList = "TaskTypesList";

        private ObservableCollection<TaskStatus> _taskStatuses;
        public ObservableCollection<TaskStatus> TaskStatuses
        {
            get
            {
                ObservableCollection<TaskStatus> ret;
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<TaskStatus>>(TaskStatusList);
                else
                    ret = _taskStatuses;

                if (ret == null)
                {
                    var TaskMngr = new TaskManager();
                    ret = TaskMngr.GetTaskStatuses();

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting(TaskStatusList, ret);
                    else
                        _taskStatuses = ret;
                }

                return ret;
            }
            protected set
            {
                if (Settings.IsWebEnvironment)
                    Settings.SetSessionSetting(TaskStatusList, value);
                else
                    _taskStatuses = value;
            }
        }

        private ObservableCollection<TaskType> _taskTypes;
        public ObservableCollection<TaskType> TaskTypes
        {
            get
            {
                ObservableCollection<TaskType> ret;
                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<TaskType>>(TaskTypesList);
                else
                    ret = _taskTypes;

                if (ret == null)
                {
                    var TaskMngr = new TaskManager();
                    ret = TaskMngr.GetTaskTypes();

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting(TaskTypesList, ret);
                    else
                        _taskTypes = ret;
                }
                return _taskTypes;
            }
            protected set
            {
                if (Settings.IsWebEnvironment)
                    Settings.SetSessionSetting(TaskTypesList, value);
                else
                    _taskTypes = value;
            }
        }


        public ObservableCollection<TaskStatus> GetTranslatedTaskStatuses(string[] translations)
        {
            try
            {
                if (
                    (TaskStatuses.First().ToString() != translations[TaskStatuses.First().ID].ToString()) &&
                    (translations != null || translations.Length > TaskStatuses.Count)
                   )
                {
                    foreach (var item in TaskStatuses)
                        item.Description = translations[item.ID];
                }
                return TaskStatuses;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskStatuses : " + ex.Message, ex);
            }
        }
        public ObservableCollection<TaskType> GetTranslatedTaskTypes(string[] translations)
        {
            try
            {
                if (
                    (TaskTypes.First().ToString() != translations[TaskTypes.First().ID].ToString()) &&
                    (translations != null || translations.Length > TaskTypes.Count)
                   )
                {
                    TaskTypes.ToList().ForEach(t => t.Description = translations[t.ID]);
                }
                return TaskTypes;
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_TaskTypes : " + ex.Message, ex);
            }
        }
    }
}
