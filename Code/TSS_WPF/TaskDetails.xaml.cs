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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Lists;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Tasks;

namespace TSS_WPF
{
    /// <summary>
    /// Interaction logic for TaskDetails.xaml
    /// </summary>
    public partial class TaskDetails : Window, INotifyPropertyChanged
    {
        public ObservableCollection<TaskStatus> TaskStatuses { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Task OutputTask = null;
        private Task EditedTask = null;
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }

        private ObservableCollection<Location> locations;

        public ObservableCollection<Location> Locations
        {
            get { return locations; }
            set
            {
                locations = value;
                OnPropertyChanged("Locations");
            }
        }

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


        private ObservableCollection<BitmapImage> ImageList = new ObservableCollection<BitmapImage>();

        private TaskDetails() { }

        public TaskDetails(Task task, bool UserMode)
        {
            //first off all, load in the resources
            if(String.IsNullOrEmpty(this.Name))
                this.Name = "TaskDetails";
            LanguageFiles.LoadLanguageFile(this, "Details");
            LanguageFiles.LoadLanguageFile(this, "Generic");
            TaskStatuses = SystemLists.Tasks.GetTranslatedTaskStatuses((string[])TryFindResource("StatusArray"));


            //make copy of task to use in the window
            OutputTask = task.Clone();
            EditedTask = task.Clone();

            if (EditedTask.StatusID <= 0)
                EditedTask.StatusID = 1;

            //get the locations of the department
            GetLocations();


            //Init Window
            InitializeComponent();

            //set datagrid for the bindings
            gridMain.DataContext = EditedTask;

            //add the key handler of the window (aka for esc)
            this.PreviewKeyDown += new KeyEventHandler(KeyHandler);

            //change the window for the userMode
            if (UserMode)
            {
                //TODO : change window for users
                //making fields that a user can't edit as readonly/not editable
                cbTechnician.IsEditable = false;
                cbTechnician.IsHitTestVisible = false;

                cbMachine.IsEditable = false;
                cbMachine.IsHitTestVisible = false;

                cbState.AllowDrop = false;
                cbState.IsHitTestVisible = false;
                cbState.IsEditable = false;
            }
        }
        private void KeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                CloseWindow();
            }
        }
        private void btn_AddNote(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtAddNote.Text))
                return;

            EditedTask.Notes.Add(new Note(txtAddNote.Text, DateTime.Now));
            txtAddNote.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            //force initial validation
            foreach (FrameworkElement item in gridMain.Children)
            {
                if (item is TextBox)
                {
                    TextBox txt = item as TextBox;
                    if (txt.GetBindingExpression(TextBox.TextProperty) == null)
                        continue;

                    txt.GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
                }
            }

            if (
                String.IsNullOrEmpty(txtReporter.Text) ||
                String.IsNullOrEmpty(txtDescription.Text) ||
                cbLocation.SelectedIndex == -1 ||
                cbMachine.SelectedIndex == -1 ||
                cbState.SelectedIndex == -1
                )
            {
                MessageBox.Show("Some fields are not correct!");
            }
            else
            {
                if (!OutputTask.Compare(EditedTask))
                {
                    //no changes detected
                    EditedTask.LastModifiedOn = DateTime.Now;
                    OutputTask.Assign(EditedTask);
                    DialogResult = true;
                }
                else
                {
                    DialogResult = false;
                } 
                CloseWindow();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            CloseWindow();
        }

        private void Departments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetLocations();
            if (
                (EditedTask.DepartmentID != OutputTask.DepartmentID) ||
                (OutputTask.Location?.ID == null || OutputTask.Location.ID == 0)
               )
            {
                cbLocation.SelectedIndex = 0;
            }
            else
            {
                cbLocation.SelectedValue = OutputTask.Location.ID;
            }
        }
        private void CloseWindow()
        {
            this.Close();
        }
        private void GetLocations()
        {
            try
            {
                var generalMgr = new GeneralManager();
                Locations = generalMgr.GetLocations(EditedTask.DepartmentID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to retrieve Department Locations : " + ex.Message);
            }
        }

        private void btnAddPicture_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                MessageBox.Show("This function is not implemented yet!");
            }
        }
    }
}
