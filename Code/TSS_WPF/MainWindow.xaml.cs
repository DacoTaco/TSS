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
using System.Collections.ObjectModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }

        public ObservableCollection<Task> TasksList { get; set; }
        public ObservableCollection<TaskStatus> TaskStatuses { get; set; }
        public ObservableCollection<TaskType> TaskTypes { get; set; }

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

        public MainWindow()
        {
            //first off all, load in the resources
            LanguageFiles.LoadLanguageFile(this, "MainWindow");
            LanguageFiles.LoadLanguageFile(this, "SuppliesWindow");
            LanguageFiles.LoadLanguageFile(Application.Current, "TaskStrings");
            LanguageFiles.LoadLanguageFile(Application.Current, "ValidationErrors");

            //Collect all data before initing window...
            try
            {
                TasksList = new TaskManager().GetTasks(null, Settings.GetCompanyName(), null, null);
                TaskStatuses = SystemLists.Tasks.GetTranslatedTaskStatuses((string[])TryFindResource("StatusArray"));
                TaskTypes = SystemLists.Tasks.GetTranslatedTaskTypes((string[])TryFindResource("TypesArray"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to retrieve data! Error : " + Environment.NewLine + ex.Message);
                Application.Current.Shutdown();
            }

            //Init Window
            InitializeComponent();
            TaskGrid.DataContext = this;

            lsbNotes.DataContext = TasksList?.FirstOrDefault();

            //SystemTabs.SelectedIndex = 1;
        }

        private bool Editting = false;

        private bool GetTaskDetails(ref Task task, bool NewTask = false)
        {
            bool ret = false;
            Task taskCopy;

            if (task == null)
                taskCopy = new Task(0)
                {
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
                //save task
                task = dlgDetails.OutputTask;
                var taskmngr = new TaskManager();
                if(taskmngr.TaskEditable(task.ID,LoggedInUser.GetUserHash()))
                    taskmngr.ChangeTasks(task);
                
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
            int index = TasksList.IndexOf(targetTask);
            Editting = true;

            if (GetTaskDetails(ref targetTask))
            {
                TasksList[index] = targetTask;
                TaskGrid.Items.Refresh();
            }           
            Editting = false;
        }
        private void btnNewTask_Click(object sender, RoutedEventArgs e)
        {
            Task task = null;
            if (GetTaskDetails(ref task, true))
            {
                TasksList.Add(task);
                TaskGrid.Items.Refresh();
            }
        }
        private void Technician_Changed(object sender, SelectionChangedEventArgs e)
        {
            //this also gets triggered when databinding changes the combobox. therefor we have the edittingTask variable to prevent it from adding it to the list
            if (TaskGrid.SelectedItem != null && Editting == false)
            {
                var taskmngr = new TaskManager();
                Task task = TaskGrid.SelectedItem as Task;
                if (task == null)
                    return;

                if (taskmngr.TaskEditable(task.ID, LoggedInUser.GetUserHash()))
                    taskmngr.ChangeTasks(task);
            }
        }
        private void TaskGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //get the object that triggered the change!
            object o = TaskGrid.ItemContainerGenerator.ItemFromContainer(e.Row);

            //if we aren't editting a task, add it to the list of changes to push to the Database!
            if (TasksList.Contains(o) && Editting == false)
            {
                Task task = (o as Task).Clone();
                var taskmngr = new TaskManager();
                if (taskmngr.TaskEditable(task.ID, LoggedInUser.GetUserHash()))
                    taskmngr.ChangeTasks(task);
            }
        }
    }
}
