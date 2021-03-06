﻿/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
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

namespace TechnicalServiceSystem.Entities.Tasks
{
    public class Note : BaseEntity
    {
        public Note()
        {
        }

        public Note(string text, DateTime noteDate)
        {
            Text = text;
            NoteDate = noteDate;
        }

        public virtual string Text { get; set; }
        public virtual DateTime NoteDate { get; set; }
        public virtual Task NoteTask { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}