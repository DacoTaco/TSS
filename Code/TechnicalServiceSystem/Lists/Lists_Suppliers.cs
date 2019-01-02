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
using System.Linq;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem.Lists
{
    public partial class SystemLists
    {
        //list which is used for the machines tab. it gets the machines list, skips the first and returns result
        private ObservableCollection<MachineInfo> actualMachines;
        public List<MachineInfo> DeletedMachines = new List<MachineInfo>();
        public List<ChangedMachineInfo> EditedMachines = new List<ChangedMachineInfo>();


        //list of all machines + "Not Set"
        private ObservableCollection<MachineInfo> machines;

        //----------------
        // LISTS
        //----------------

        //lists for syncing tasks
        public List<MachineInfo> NewMachines = new List<MachineInfo>();

        //list of all machine types
        public ObservableCollection<MachineInfo> Machines
        {
            get
            {
                if (machines == null)
                    machines = new ObservableCollection<MachineInfo>();
                return machines;
            }
            set
            {
                var temp = new ObservableCollection<MachineInfo>();
                temp.Add(new MachineInfo(0, "Not Set", "", "", "", 0, 0));
                foreach (var item in value) temp.Add(item);
                machines = temp;
                OnPropertyChanged("Machines");
                OnPropertyChanged("ActualMachines");
            }
        }

        public ObservableCollection<MachineInfo> ActualMachines
        {
            get
            {
                if (actualMachines != null)
                    actualMachines.CollectionChanged -= ActualMachines_Changed;

                if (Machines.Count > 1)
                    actualMachines = new ObservableCollection<MachineInfo>(Machines.Skip(1));
                else
                    actualMachines = new ObservableCollection<MachineInfo>();

                actualMachines.CollectionChanged += ActualMachines_Changed;

                return actualMachines;
            }
        }

        public ObservableCollection<MachineType> MachineTypes { get; set; }


        //----------------------------
        // List retrieval functions
        //----------------------------

        private void Machines_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
                foreach (MachineInfo item in e.NewItems)
                    if (e.OldItems == null || !e.OldItems.Contains(item))
                        NewMachines.Add(item);

            if (e.OldItems != null && e.OldItems.Count > 0)
                foreach (MachineInfo machine in e.OldItems)
                    if (!Machines.Contains(machine) &&
                        (e.NewItems == null || !e.NewItems.Contains(machine))
                    )
                        DeletedMachines.Add(machine);
            OnPropertyChanged("Machines");
            OnPropertyChanged("ActualMachines");
        }

        private void ActualMachines_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
                foreach (MachineInfo item in e.NewItems)
                    if (e.OldItems == null || !e.OldItems.Contains(item))
                        Machines.Add(item);

            if (e.OldItems != null && e.OldItems.Count > 0)
                foreach (MachineInfo machine in e.OldItems)
                    if (!actualMachines.Contains(machine) &&
                        (e.NewItems == null || !e.NewItems.Contains(machine))
                    )
                        Machines.Remove(machine);
        }

        public void RegisterMachineHandles()
        {
            Machines.CollectionChanged += Machines_Changed;
        }


        //----------------------------
        // List retrieval functions
        //----------------------------

        /// <summary>
        ///     Retrieve a list of all machines in the database. when given a valid string, will attempt to translate "Not Set"
        ///     into the given string
        /// </summary>
        /// <param name="NotSetString"></param>
        public void GetMachinesList(string NotSetString)
        {
            var mngr = new SupplierManager();
            Machines = mngr.GetBaseMachines();
            if (Machines != null && Machines.Count > 0 && !string.IsNullOrWhiteSpace(NotSetString))
                Machines[0].Name = NotSetString;
        }

        public void GetMachineTypes()
        {
            try
            {
                var supMngr = new SupplierManager();
                MachineTypes = supMngr.GetMachineTypes();
            }
            catch (Exception ex)
            {
                throw new Exception("Lists_Failed_Get_SUPPLIERS : " + ex.Message, ex);
            }
        }

        public void GetSuppliersLists(string NotSet = "Not Set")
        {
            GetMachinesList(NotSet);
            RegisterMachineHandles();
            GetMachineTypes();
        }
    }
}