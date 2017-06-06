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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TSS_WPF
{
    /// <summary>
    /// Interaction logic for MachineDetails.xaml
    /// </summary>
    public partial class MachineDetails : Window
    {
        public SystemLists Lists
        {
            get
            {
                return SystemLists.GetInstance();
            }

        }

        protected MachineInfo EdittingMachine;
        public ChangedMachineInfo OutputMachine;

        public MachineDetails(MachineInfo machine)
        {
            if (String.IsNullOrEmpty(this.Name))
                this.Name = "MachineDetails";
            LanguageFiles.LoadLanguageFile(this,"Details");
            LanguageFiles.LoadLanguageFile(this, "Generic");

            //add the key handler of the window (aka for esc)
            this.PreviewKeyDown += new KeyEventHandler(KeyHandler);

            //Set the variables

            OutputMachine = ChangedMachineInfo.UpgradeBase(machine);
            EdittingMachine = machine.Clone();

            InitializeComponent();

            gridMain.DataContext = EdittingMachine;

        }

        private void CloseWindow()
        {
            this.Close();
        }

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                CloseWindow();
            }
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

            //TODO : verify all fields
            if (!OutputMachine.Compare(EdittingMachine))
            {
                OutputMachine.Assign(EdittingMachine);
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
            
            CloseWindow();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            CloseWindow();
        }
    }
}
