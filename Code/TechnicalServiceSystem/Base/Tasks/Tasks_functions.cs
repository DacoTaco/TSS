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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using TechnicalServiceSystem.Base;
using TechnicalServiceSystem;

namespace TechnicalServiceSystem.Base
{
    public partial class Task
    {
        //Base Class : Task. The methods and functions
        //this is a partial class because it has so much stuff and going on its easier to manage like this

        //constructors
        protected Task() { }
        public Task(
            int TaskID,
            string desc,
            bool urguent,
            int tasktype,
            string reporter,
            int technician,
            int departmentID,
            int locationID,
            int statusID,
            int machineID,
            DateTime taskCreationDate,
            DateTime lastAdjustments,
            ObservableCollection<Note> notes = null,
            ObservableCollection<PhotoInfo> PhotoCollection = null
            )
        {
            Init(TaskID, desc, urguent, tasktype, 0, reporter, technician, departmentID, locationID, statusID, machineID, taskCreationDate, lastAdjustments, null,null, notes, PhotoCollection);
        }
        public Task(
            int TaskID,
            string desc,
            bool urguent,
            int tasktype,
            int reporterid,
            string reporter,
            int technician,
            int departmentID,
            int locationID,
            int statusID,
            int machineID,
            DateTime taskCreationDate,
            DateTime lastAdjustments,
            ObservableCollection<Note> notes = null,
            ObservableCollection<PhotoInfo> PhotoCollection = null
            )
        {
            Init(TaskID, desc, urguent, tasktype, reporterid, reporter, technician, departmentID, locationID, statusID, machineID, taskCreationDate,lastAdjustments, null,null, notes, PhotoCollection);
        }

        //constructor of repeating Task
        public Task(
            int TaskID,
            string desc,
            bool urguent,
            int tasktype,
            int reporterid,
            string reporter,
            int technician,
            int departmentID,
            int locationID,
            int statusID,
            int machineID,
            DateTime taskCreationDate,
            DateTime lastAdjustments,
            DateTime? activation,
            int? interval,
            ObservableCollection<Note> notes = null,
            ObservableCollection<PhotoInfo> PhotoCollection = null
            )
        {
            Init(TaskID, desc, urguent, tasktype, reporterid, reporter, technician, departmentID, locationID, statusID, machineID, taskCreationDate,lastAdjustments,activation,interval, notes, PhotoCollection);
        }

        /// <summary>
        /// Initialise the new Task. set all properties to the given properties and setup lists for OnPropertyChanged
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="desc"></param>
        /// <param name="urguent"></param>
        /// <param name="tasktype"></param>
        /// <param name="reporterid"></param>
        /// <param name="reporter"></param>
        /// <param name="technician"></param>
        /// <param name="departmentID"></param>
        /// <param name="locationID"></param>
        /// <param name="statusID"></param>
        /// <param name="machineID"></param>
        /// <param name="lastAdjustments"></param>
        /// <param name="activationdate"></param>
        /// <param name="interval"></param>
        /// <param name="notes"></param>
        /// <param name="PhotoCollection"></param>
        protected void Init(
            int TaskID,
            string desc,
            bool urguent,
            int tasktype,
            int reporterid,
            string reporter,
            int technician,
            int departmentID,
            int locationID,
            int statusID,
            int machineID,
            DateTime taskCreationDate,
            DateTime lastAdjustments,
            DateTime? activationdate,
            int? interval,
            ObservableCollection<Note> notes = null,
            ObservableCollection<PhotoInfo> PhotoCollection = null
            )
        {
            ID = TaskID;
            Description = desc;
            Urguent = urguent;
            TaskTypeID = tasktype;
            ReporterID = reporterid;
            Reporter = reporter;
            TechnicianID = technician;
            DepartmentID = departmentID;
            LocationID = locationID;
            StatusID = statusID;
            MachineID = machineID;
            CreationDate = taskCreationDate;
            LastAdjustment = lastAdjustments;
            ActivationDate = activationdate;
            TaskInterval = interval;


            if (notes != null)
                Notes = notes;
            else
                Notes = new ObservableCollection<Note>();


            if (PhotoCollection != null)
                Photos = PhotoCollection;
            else
                Photos = new ObservableCollection<PhotoInfo>();

            if (taskCreationDate == null || taskCreationDate.Date == default(DateTime))
                taskCreationDate = DateTime.Now;


            Notes.CollectionChanged += Notes_CollectionChanged;
            Photos.CollectionChanged += Photos_CollectionChanged;
        }

        //handlers for the CollectionChanged
        protected void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Photos");
        }
        protected void Notes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //send msg to the interface that the string needs to be updated
            OnPropertyChanged("strNotes");
            OnPropertyChanged("Notes");
        }

        //always handy for converters or identification!
        public override string ToString()
        {
            return Description;
        }

        /// <summary>
        /// override of equals to check if the tasks are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Type type = this.GetType();
            Type objType = obj.GetType();

            //we check and change the current objects type because inheriting Tasks are still Tasks. however, the object comparing will fail with
            //derrived classes otherwise! since we are here we know the current object is a Task type anyway...
            if ((obj as Task == null) || (this as Task == null))
                return false;

            type = typeof(Task);
            objType = typeof(Task);

            if (objType != type)
                return false;

            //if the TaskID & CreationDate are the same we can be pretty possitive the Tasks are equal
            //but to be sure, lets throw in the tasktype
            Task input = obj as Task;
            if(
                (this.ID == input.ID) &&
                (this.CreationDate == input.CreationDate) &&
                (this.TaskTypeID == input.TaskTypeID)
               )
                return true;
            else
                return false;
        }
        // and cause we override Equals, also override GetHashCode!
        public override int GetHashCode()
        {
            //found online. it gets the values and calculates a Hash that is semi unique thanks to the properties given.
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)17;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 23) ^ this.ID.GetHashCode();
                hash = (hash * 23) ^ this.TaskTypeID.GetHashCode();
                hash = (hash * 23) ^ this.CreationDate.GetHashCode();
                //hash = (hash * 23) ^ this.Notes.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Creates a Clone of the current instance, this also creates new lists so bindings dont interfere
        /// </summary>
        /// <returns>a cloned Task</returns>
        public virtual new Task Clone()
        {
            Task output = _clone<Task>();

            //fix the observablecollections. the output lists are sharing references, so new list and readd everything!
            output.Notes = new ObservableCollection<Note>();
            foreach (var item in this.Notes)
            {
                output.Notes.Add(item);
            }
            output.Notes.CollectionChanged += output.Notes_CollectionChanged;

            output.Photos = new ObservableCollection<PhotoInfo>();
            foreach (var item in this.Photos)
            {
                output.Photos.Add(item);
            }
            output.Photos.CollectionChanged += output.Photos_CollectionChanged;

            return output;
        }
    }
}
