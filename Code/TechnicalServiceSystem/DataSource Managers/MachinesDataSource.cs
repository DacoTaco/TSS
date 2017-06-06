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
using TechnicalServiceSystem.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Equin.ApplicationFramework;


namespace TechnicalServiceSystem.DataSourceManagers
{
    //state that its a dataobject so visual studio knows it can be used as a datasource
    /// <summary>
    /// Data Source Managers used for DataSources. this basically a wrapper of the object managers
    /// </summary>
    [DataObject(true)]
    public class MachinesManager
    {
        //select functions!
        [DataObjectMethod(DataObjectMethodType.Select)]
        public ObservableCollection<MachineInfo> GetMachines()
        {
            ObservableCollection<MachineInfo> ret = new ObservableCollection<MachineInfo>();
            try
            {
                var supMngr = new SupplierManager();
                ret = supMngr.GetMachines();
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_Machines : " + ex.Message, ex);
            }

            return ret;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<MachineInfo> GetMachines(string SortBy = null)
        {
            BindingListView<MachineInfo> ret = new BindingListView<MachineInfo>(GetMachines());
            ret.Sort = SortBy;
            return ret;
        }

        //update functions!
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateMachines(List<ChangedMachineInfo> Machines)
        {
            try
            {
                var machineMngr = new SupplierManager();
                machineMngr.ChangeMachine(Machines);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Set_Machines : " + ex.Message, ex);
            }
        }
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateMachine(ChangedMachineInfo Machine)
        {
            if (Machine == null)
                return;

            List<ChangedMachineInfo> machines = new List<ChangedMachineInfo>();
            machines.Add(Machine);

            UpdateMachines(machines);
        }
        [DataObjectMethod(DataObjectMethodType.Update)]
        public void UpdateMachine(MachineInfo Machine)
        {
            if (Machine == null)
                return;

            List<ChangedMachineInfo> machines = new List<ChangedMachineInfo>();
            machines.Add(ChangedMachineInfo.UpgradeBase(Machine,true));

            UpdateMachines(machines);
        }


        //insert functions!
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void InsertMachines(List<MachineInfo> Machines)
        {
            try
            {
                var machineMngr = new SupplierManager();
                machineMngr.AddMachine(Machines);
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Add_Machines : " + ex.Message, ex);
            }
        }
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void InsertMachines(MachineInfo Machine)
        {
            if (Machine == null)
                return;

            List<MachineInfo> machines = new List<MachineInfo>();
            machines.Add(Machine);

            InsertMachines(machines);
        }
    }
}
