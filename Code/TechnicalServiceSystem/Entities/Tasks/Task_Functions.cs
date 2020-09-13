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
using TechnicalServiceSystem.Entities.General;
using TechnicalServiceSystem.Entities.Users;

namespace TechnicalServiceSystem.Entities.Tasks
{
    public partial class Task : BaseEntity
    {
        public Task()
        {
            _notes = new ObservableCollection<Note>();
            Photos = new ObservableCollection<Photo>();
        }

        public Task(int id) : base()
        {
            ID = id;
        }

        public override string ToString()
        {
            return Description;
        }

        private void SetTechnicianAndStatus(User technician)
        {
            _technician = technician;

            if ((_technician?.ID ?? 0) > 0 && _statusID == 1)
            {
                _statusID = 2;
            }
            else if (StatusID == 2 && (_technician?.ID ?? 0) < 1)
            {
                _statusID = 1;
            }
        }

        private void SetStatusAndTechnician(int statusID)
        {
            if (statusID < 1)
                return;

            int val = statusID;

            if (val == 2 && (_technician?.ID ?? 0) == 0)
                val = 1;

            if (TechnicianID > 0 && val <= 1)
                _technician = null;

            _statusID = val;
        }

        /// <summary>
        ///     override of equals to check if the tasks are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var type = GetType();
            var objType = obj.GetType();

            //we check and change the current objects type because inheriting Tasks are still Tasks. however, the object comparing will fail with
            //derrived classes otherwise! since we are here we know the current object is a Task type anyway...
            if (obj as Task == null || this == null)
                return false;

            type = typeof(Task);
            objType = typeof(Task);

            if (objType != type)
                return false;

            //if the TaskID & CreationDate are the same we can be pretty possitive the Tasks are equal
            //but to be sure, lets throw in the tasktype
            var input = obj as Task;
            if (
                ID == input.ID &&
                CreationDate == input.CreationDate &&
                TypeID == input.TypeID
            )
                return true;
            return false;
        }

        // and cause we override Equals, also override GetHashCode!
        public override int GetHashCode()
        {
            //found online. it gets the values and calculates a Hash that is semi unique thanks to the properties given.
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 23) ^ ID.GetHashCode();
                hash = (hash * 23) ^ TypeID.GetHashCode();
                hash = (hash * 23) ^ CreationDate.GetHashCode();
                //hash = (hash * 23) ^ this.Notes.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Creates a Clone of the current instance, this also creates new lists so bindings dont interfere
        /// </summary>
        /// <returns>a cloned Task</returns>
        public new virtual Task Clone()
        {
            var output = _clone<Task>();

            //fix the observablecollections. the output lists are sharing references, so new list and re-add everything!
            output.ClearNotes();
            foreach (var item in Notes) output.AddNote(item);

            output.Photos = new ObservableCollection<Photo>();
            foreach (var item in Photos) output.Photos.Add(item);

            return output;
        }

        public virtual void AddNote(Note note)
        {
            note.NoteTask = this;
            _notes.Add(note);

            OnPropertyChanged(nameof(strNotes));
            OnPropertyChanged(nameof(Notes));
        }

        public virtual void ClearNotes()
        {
            _notes = new ObservableCollection<Note>();
        }
    }
}