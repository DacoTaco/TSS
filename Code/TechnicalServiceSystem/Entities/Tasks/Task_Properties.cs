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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Suppliers;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem.Entities.Tasks
{
    public partial class Task : BaseEntity
    {
        protected ObservableCollection<Note> _notes;

        protected ObservableCollection<Photo> _photos;
        public virtual string Description { get; set; }
        public virtual bool IsUrguent { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime LastModifiedOn { get; set; }
        public virtual string OpenedBy { get; set; }
        public virtual DateTime? OpenedTimeDue { get; set; }

        //foreign data
        protected internal virtual User _technician { get; set; }
        public virtual User Technician
        {
            get { return _technician;}
            set { SetTechnicianAndStatus(value); }
        }
        public virtual Department Department => Location?.Department;
        public virtual Location Location { get; set;}
        public virtual Machine Device { get; set; }
        public virtual int? TechnicianID => Technician?.ID??0;
        public virtual int DepartmentID => Department?.ID??0;

        protected internal virtual int _statusID { get; set; }
        public virtual int StatusID
        {
            get { return _statusID; }
            set { SetStatusAndTechnician(value); }
        }
        public virtual int TypeID { get; set; }
        public virtual RepeatingInfo RepeatingInfo { get; set; }

        //sorting (/retrieval) keys
        public virtual string LocationName => Location?.Description;
        public virtual string TechnicianName => Technician?.UserName;


        protected virtual string _reporterName { get; set; }
        public virtual User ReporterUser { get; set; }
        public virtual string Reporter
        {
            get
            {
                return (ReporterUser?.ID ?? 0) <= 0 ? _reporterName : ReporterUser.UserName;
            }
            set
            {
                if (ReporterUser != null)
                    return;
                _reporterName = value;
            }
        }
        public virtual ICollection<Note> Notes
        {
            get { return _notes; }
            protected set
            {
                if (value != null)
                    _notes = new ObservableCollection<Note>(value);
                else
                    _notes = null;

                OnPropertyChanged("Notes");
                OnPropertyChanged("strNotes");
            }
        }
        public virtual string strNotes
        {
            get
            {
                var ret = "";

                //replace with NoteListToString converter = new NoteListToString();
                foreach (var note in Notes)
                {
                    if (note == null)
                        continue;

                    if (string.IsNullOrEmpty(ret))
                        ret = string.Format("{0} - {1}", note.NoteDate, note.Text);
                    else
                        ret += string.Format("{0}{1} - {2}", Environment.NewLine, note.NoteDate, note.Text);
                }

                return ret;
            }
        }
        public virtual ICollection<Photo> Photos
        {
            get { return _photos; }
            protected set
            {
                if (value != null)
                    _photos = new ObservableCollection<Photo>(value);
                else
                    _photos = null;
                OnPropertyChanged("Photos");
            }
        }
    }
}