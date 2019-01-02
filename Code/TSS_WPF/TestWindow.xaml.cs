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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.Tasks;
using TechnicalServiceSystem.Lists;

namespace TSS_WPF
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }

        // ok SO. dynamic resource binding is not allowed for TargetNullValue in multibinding. le suck. SO, we use this variable, use it as a static resource in wpf.
        // done and done :P
        public string NotSet
        {
            get
            {
                string ret = null;

                try
                {
                    ret = (string)FindResource("NotSet");
                }
                catch
                {
                    ret = "Not Set";
                }

                if (String.IsNullOrEmpty(ret))
                    ret = "Not Set";

                return ret;
            }
        }


        public TestWindow()
        {
            //first off all, load in the resources
            LanguageFiles.LoadLanguageFile(this, "MainWindow");
            LanguageFiles.LoadLanguageFile(Application.Current, "TaskStrings");
            LanguageFiles.LoadLanguageFile(Application.Current, "ValidationErrors");

            //Collect all data before initing window...
            try
            {
                Lists.GetLists((string[])TryFindResource("StatusArray"), (string[])TryFindResource("TypesArray"), (string[])TryFindResource("RoleNames"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to retrieve data! Error : " + Environment.NewLine + ex.Message);
                Application.Current.Shutdown();
            }



            //Init Window
            InitializeComponent();
            TaskGrid.DataContext = this;
            lsbNotes.DataContext = Lists.TasksList[0];
            Lists.TasksList.CollectionChanged += Lists.Tasks_Changed;
        }

        private bool EdittingTask = false;

        private bool GetTaskDetails(ref Task task, bool NewTask = false)
        {
            bool ret = false;
            Task taskCopy;

            if (task == null)
                taskCopy = new Task()
            {
                ID = 0,
                CreationDate = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                IsUrguent = false,
                TypeID = 1,
                Reporter = "",
                StatusID = 1
            };
            else
                taskCopy = task.Clone();

            TaskDetails dlgDetails = new TaskDetails(taskCopy, false);
            dlgDetails.Owner = (this);
            if (dlgDetails.ShowDialog().Value == true)
            {
                //We assign edited tasks here cause we actually need a Task for the editted list.
                if (!NewTask)
                    Lists.EditedTasks.Add(dlgDetails.OutputTask);

                //properly assign the task so we dont reassign the lists AND dont upgrade the task to a derrived class which seems to kill binding :)
                if (task == null)
                    task = dlgDetails.OutputTask;
                else
                    task.Assign(dlgDetails.OutputTask);

                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = (DataGridRow)sender;
            Task targetTask = (Task)row.Item;
            int index = Lists.TasksList.IndexOf(targetTask);
            EdittingTask = true;

            if (GetTaskDetails(ref targetTask))
            {
                Lists.TasksList[index] = targetTask;
            }
            EdittingTask = false;
        }

        private void btnNewTask_Click(object sender, RoutedEventArgs e)
        {
            Task task = null;
            if(GetTaskDetails(ref task,true))
            {
                Lists.TasksList.Add(task);
            }
        }
        private void btnSyncTasks_Click(object sender, RoutedEventArgs e)
        {
            var Taskmgr = new TaskManager();

            if(Lists.NewTasks.Count > 0)
            {
                Taskmgr.AddTasks(Lists.NewTasks);
                Lists.NewTasks.Clear();
            }

            if(Lists.EditedTasks.Count > 0)
            {
                Taskmgr.ChangeTasks(Lists.EditedTasks);
                Lists.EditedTasks.Clear();
            }

            if(Lists.DeletedTasks.Count > 0)
            {
                Taskmgr.DeleteTasks(Lists.DeletedTasks);
                Lists.DeletedTasks.Clear();
            }

            //reget the tasks y0
            Lists.GetTasks();

            MessageBox.Show("Sync Complete!");
        }

        private void Technician_Changed(object sender, SelectionChangedEventArgs e)
        {
            //this also gets triggered when databinding changes the combobox. therefor we have the edittingTask variable to prevent it from adding it to the list
            if (TaskGrid.SelectedItem != null && EdittingTask == false)
            {
                Task task = (TaskGrid.SelectedItem as Task)?.Clone();//Task.UpgradeBase(TaskGrid.SelectedItem as Task);
                /*task.Changed_Properties["TechnicianID"] = true;
                if (task.StatusID < 3)
                    task.Changed_Properties["StatusID"] = true;*/

                Lists.EditedTasks.Add(task);
            }
        }

        private void TaskGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //get the object that triggered the change!
            object o = TaskGrid.ItemContainerGenerator.ItemFromContainer(e.Row);

            //if we aren't editting a task, add it to the list of changes to push to the Database!
            if(Lists.TasksList.Contains(o) && EdittingTask == false)
            {
                Task task = (o as Task)?.Clone(); 
                Lists.EditedTasks.Add(task);
            }
        }
    }
}
