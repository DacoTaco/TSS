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
using System.Collections;
using System.ComponentModel;

namespace TechnicalServiceSystem.Entities
{
    internal interface IBaseEntity
    {
        int ID { get; }
    }

    public class BaseEntity : IBaseEntity, INotifyPropertyChanged
    {
        protected int _id;

        protected BaseEntity()
        {
        }

        public virtual int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        ///     Compare the current object to the input object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Compare(object obj)
        {
            if (obj == null)
                return false;
            var type = GetType();

            if (obj.GetType() != type)
                return false;

            //loop trough all properties and compare!
            var properties = type.GetProperties();
            foreach (var item in properties)
            {
                var selfValue = type.GetProperty(item.Name).GetValue(this, null);
                var toValue = type.GetProperty(item.Name).GetValue(obj, null);

                if (selfValue as IList != null)
                {
                    // the item is a list. lets go trough all items and check
                    var SelfList = selfValue as IList;
                    var ToList = toValue as IList;

                    if (SelfList == null || ToList == null || SelfList.Count != ToList.Count)
                        return false;

                    for (var i = 0; i < SelfList.Count; i++)
                        if (SelfList[i] != ToList[i])
                            return false;
                }
                else
                {
                    if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))) return false;
                }
            }

            return true;
        }

        public virtual void Assign(object Input)
        {
            if (Input == null)
                return;

            if (Input.GetType() != GetType() && Input.GetType().IsAssignableFrom(GetType()) &&
                GetType().IsAssignableFrom(Input.GetType())
            )
                return;

            try
            {
                var type = Input.GetType();
                var properties = type.GetProperties();
                foreach (var item in properties)
                    if (type.GetProperty(item.Name) != null && GetType().GetProperty(item.Name) != null)
                    {
                        var selfValue = type.GetProperty(item.Name).GetValue(this, null);
                        var toValue = type.GetProperty(item.Name).GetValue(Input, null);

                        if (selfValue as IList != null)
                        {
                            // the item is a list. lets go trough all items and check
                            var SelfList = selfValue as IList;
                            var ToList = toValue as IList;

                            SelfList.Clear();
                            foreach (var listItem in ToList) SelfList.Add(listItem);
                        }
                        else
                        {
                            type.GetProperty(item.Name).SetValue(this, toValue);
                        }

                        //just to be sure, raise a propertychanged event so all data bindings are refreshed from the assigning!
                        OnPropertyChanged(item.Name);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///     Creates a clone of the current instance. clones all properties
        /// </summary>
        /// <returns>a cloned object of type T</returns>
        protected virtual T _clone<T>()
        {
            //this is the base of cloning. because we use Generic Type here, it also works with inheritance!
            var GenericOutput = (T) MemberwiseClone();

            return GenericOutput;
        }

        /// <summary>
        ///     Basic Baseclass CLone function. creates a clone of current instance
        /// </summary>
        /// <returns>BaseClass</returns>
        public virtual BaseEntity Clone()
        {
            return _clone<BaseEntity>();
        }
    }
}