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
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Base class containing Information for notes
    /// </summary>
    public class Note : BaseClass
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private DateTime _noteDate;

        public DateTime NoteDate
        {
            get { return _noteDate; }
            set { _noteDate = value; }
        }


        /// <summary>
        /// Create a new Note
        /// </summary>
        /// <param name="noteText"></param>
        /// <param name="noteDate"></param>
        public Note(string noteText,DateTime noteDate)
        {
            //like everywhere in TSS, ID 0 means new or invalid
            //so ye, this is the constructor for a new note
            Init(0, noteText, noteDate);
        }

        /// <summary>
        /// Constructor to a note , who's ID is known
        /// </summary>
        /// <param name="id"></param>
        /// <param name="noteText"></param>
        /// <param name="noteDate"></param>
        public Note(int id, string noteText, DateTime noteDate)
        {
            Init(id, noteText, noteDate);
        }

        protected void Init(int id, string noteText, DateTime noteDate)
        {
            ID = id;
            Text = noteText;
            NoteDate = noteDate;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", NoteDate, Text);
        }
    }
}
