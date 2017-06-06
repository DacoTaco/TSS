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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechnicalServiceSystem.Base
{
    /// <summary>
    /// Base class of nearly all database classes. contains the ID and propertyChanged. also used in the converter to retrieve items out of a list by identifying item using its ID
    /// </summary>
    public class BaseClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        
        protected BaseClass()
        {

        }

        /// <summary>
        /// Creates a clone of the current instance. clones all properties
        /// </summary>
        /// <returns>a cloned object of type T</returns>
        protected virtual T _clone<T>()
        {
            //this is the base of cloning. because we use Generic Type here, it also works with inheritance!
            T GenericOutput = (T)this.MemberwiseClone();

            return GenericOutput;
        }

        /// <summary>
        /// Basic Baseclass CLone function. creates a clone of current instance
        /// </summary>
        /// <returns>BaseClass</returns>
        public virtual BaseClass Clone()
        {
            return _clone<BaseClass>();
        }

        /// <summary>
        /// Function to change all values of the current object. it keeps list references, keeping bindings alive.
        /// </summary>
        /// <param name="Input"></param>
        public virtual void Assign(object Input)
        {
            if (Input == null)
                return;

            if (Input.GetType() != this.GetType() &&
                (Input.GetType().IsAssignableFrom(this.GetType()) && this.GetType().IsAssignableFrom(Input.GetType()))
               )
                return;

            try
            {
                Type type = Input.GetType();
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    if (type.GetProperty(item.Name) != null && this.GetType().GetProperty(item.Name) != null)
                    {
                        object selfValue = type.GetProperty(item.Name).GetValue(this, null);
                        object toValue = type.GetProperty(item.Name).GetValue(Input, null);

                        if ((selfValue as IList) != null)
                        {
                            // the item is a list. lets go trough all items and check
                            IList SelfList = selfValue as IList;
                            IList ToList = toValue as IList;

                            SelfList.Clear();
                            foreach (var listItem in ToList)
                            {
                                SelfList.Add(listItem);
                            }
                        }
                        else
                        {
                            type.GetProperty(item.Name).SetValue(this, toValue);
                        }
                        //just to be sure, raise a propertychanged event so all data bindings are refreshed from the assigning!
                        OnPropertyChanged(item.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Compare the current object to the input object. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Compare(object obj)
        {
            if (obj == null)
                return false;
            Type type = this.GetType();

            if (obj.GetType() != type)
                return false;

            //loop trough all properties and compare!
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo item in properties)
            {
                object selfValue = type.GetProperty(item.Name).GetValue(this, null);
                object toValue = type.GetProperty(item.Name).GetValue(obj, null);

                if ((selfValue as IList) != null)
                {
                    // the item is a list. lets go trough all items and check
                    IList SelfList = selfValue as IList;
                    IList ToList = toValue as IList;

                    if ((SelfList == null || ToList == null) || (SelfList.Count != ToList.Count))
                        return false;

                    for (int i = 0; i < SelfList.Count; i++)
                    {
                        if (SelfList[i] != ToList[i])
                            return false;
                    }
                }
                else
                {
                    if ((selfValue != toValue) && (selfValue == null || !selfValue.Equals(toValue)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
