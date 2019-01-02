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

namespace TechnicalServiceSystem.Entities.Tasks
{
    public class RepeatingInfo : BaseEntity
    {
        public virtual Task ParentTask { get; set; }
        public virtual Task ChildTask { get; set; }

        protected virtual int _activationDate { get; set; }

        public virtual DateTime ActivationDate
        {
            get { return new DateTime(1900, 1, 1).AddDays(_activationDate); }
            set { _activationDate = (int) (value - new DateTime(1900, 1, 1)).TotalDays; }
        }

        protected virtual int _interval { get; set; }

        public virtual TimeSpan Interval
        {
            get { return new TimeSpan(_interval, 0, 0, 0); }
            set { _interval = value.Days; }
        }

        public override string ToString()
        {
            return ParentTask.Description;
        }
    }
}