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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Class that contains all information about a Task
    /// </summary>
    public partial class Task : BaseClass
    {
        //Base Class : Task. The properties
        //We split this up to make it easier to manage since this has alot of properties & functions and it has alot going on.

        //list of photos in the task
        private ObservableCollection<PhotoInfo> photos;
        public ObservableCollection<PhotoInfo> Photos
        {
            get { return photos; }
            set
            {
                photos = value;
                OnPropertyChanged("Photos");
            }
        }

        //list of notes in the task
        private ObservableCollection<Note> notes;
        public ObservableCollection<Note> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
                OnPropertyChanged("strNotes");
            }
        }

        /// <summary>
        /// string version of notes. used for binding and what have you
        /// </summary>
        public String strNotes
        {
            get
            {
                string ret = "";
                object notes;
                Type type = typeof(string);
                NoteListToString converter = new NoteListToString();
                if (Notes != null)
                {
                    notes = converter.Convert(Notes, type,null,null);
                    ret = notes as string;
                }
                
                return ret;
            }
            set
            {

            }
        }

        /// <summary>
        /// Task's Description. also used to identify the task in string format(ToString)
        /// </summary>
        private string taskDesc;
        public string Description
        {
            get { return taskDesc; }
            set { taskDesc = value; }
        }

        /// <summary>
        /// Whether the task is an urguent one or not
        /// </summary>
        private bool urguency;
        public bool Urguent
        {
            get { return urguency; }
            set { urguency = value; }
        }

        /// <summary>
        /// The Task's Type
        /// </summary>
        private int taskType;
        public int TaskTypeID
        {
            get { return taskType; }
            set { taskType = value; }
        }

        /// <summary>
        /// The Reporter's ID, if given
        /// </summary>
        private int reporterID;
        public int ReporterID
        {
            get { return reporterID; }
            set { reporterID = value; }
        }

        /// <summary>
        /// Reporter's name. often linked to the ID
        /// </summary>
        private string reporterName;
        public string Reporter
        {
            get { return reporterName; }
            set { reporterName = value; }
        }

        /// <summary>
        /// Assigned Technician's ID
        /// </summary>
        private int technicianID;
        public int TechnicianID
        {
            get { return technicianID; }
            set
            {
                int val = value;
                technicianID = val;

                if (val > 0 && StatusID == 1)
                {
                    stats = 2;
                }
                else if(StatusID == 2 && val < 1)
                {
                    stats = 1;
                }

                OnPropertyChanged("TechnicianID");
                OnPropertyChanged("StatusID");
            }
        }

        /// <summary>
        /// The ID of the department where the task needs to be excuted
        /// </summary>
        private int dep;
        public int DepartmentID
        {
            get { return dep; }
            set { dep = value; }
        }

        /// <summary>
        /// locationID of where the task should be executed
        /// </summary>
        private int loc;
        public int LocationID
        {
            get { return loc; }
            set { loc = value; }
        }

        /// <summary>
        /// the StatusID of the task
        /// </summary>
        private int stats;
        public int StatusID
        {
            get { return stats; }
            set
            {
                int val = value;

                if (val == 2 && TechnicianID == 0)
                    val = 1;
                if (TechnicianID > 0 && val <= 1)
                    technicianID = 0;

                stats = val;
                OnPropertyChanged("TechnicianID");
                OnPropertyChanged("StatusID");
            }
        }

        /// <summary>
        /// Assigned Machine , if any
        /// </summary>
        private int machine;
        public int MachineID
        {
            get { return machine; }
            set { machine = value; }
        }

        /// <summary>
        /// The Date in which the Task was created
        /// </summary>
        private DateTime creationDate;
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        /// <summary>
        /// DateTime of when the task was last adjusted
        /// </summary>
        private DateTime lastAdjustment;
        public DateTime LastAdjustment
        {
            get { return lastAdjustment; }
            set { lastAdjustment = value; }
        }

        /// <summary>
        /// Repeating Tasks : the next time a task should be activated and child task should be executed
        /// </summary>
        private DateTime? activationDate;
        public DateTime? ActivationDate
        {
            get { return activationDate; }
            set { activationDate = value; }
        }

        /// <summary>
        /// Repeating Task : Interval on how many days should be between each activation
        /// </summary>
        private int? taskInterval;
        public int? TaskInterval
        {
            get { return taskInterval; }
            set { taskInterval = value; }
        }

    }
}
