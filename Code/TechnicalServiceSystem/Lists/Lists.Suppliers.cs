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

using System.Collections.ObjectModel;
using TechnicalServiceSystem.Entities.Suppliers;

namespace TechnicalServiceSystem.Lists
{
    public class SupplierLists
    {
        private readonly string MachineList = "MachineList";

        private ObservableCollection<Machine> _machines = null;

        public ObservableCollection<Machine> Machines
        {
            get
            {
                ObservableCollection<Machine> ret;

                if (Settings.IsWebEnvironment)
                    ret = Settings.GetSessionSetting<ObservableCollection<Machine>>(MachineList);
                else
                    ret = _machines;

                if (ret == null)
                {
                    var SupplierMngr = new SupplierManager();
                    ret = SupplierMngr.GetMachines();

                    if (Settings.IsWebEnvironment)
                        Settings.SetSessionSetting(MachineList, ret);
                    else
                        _machines = ret;
                }

                return ret;
            }
        }
    }
}
