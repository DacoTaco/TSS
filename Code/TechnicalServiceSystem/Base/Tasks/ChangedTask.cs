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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    //in the end this class is kinda useless. this is a class i made to try and compensate as much as possible for when multiple users had something opened
    //this made it so no unnecesary changes were pushed to the database.
    //but now that we have the checks to see if its opened or not... :P
    //we are keeping this so in possible futures we can keep track of what changes are made
    /// <summary>
    /// Class containing all information to pass a changed task to the manager so we dont change to much
    /// </summary>
    public class ChangedTask : Task
    {
        /// <summary>
        /// a dictionary list containing all properties that changed (and a boolean to verify it is indeed changed)
        /// </summary>
        public Dictionary<String, Boolean> Changed_Properties { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// See Task.Clone(). 
        /// </summary>
        /// <returns></returns>
        public new ChangedTask Clone()
        {
            //get basic clone
            ChangedTask output = (ChangedTask)base.Clone();

            //create new dictionary so the base and clone dont share the same dictionary
            output.Changed_Properties = new Dictionary<string, bool>();
            foreach (var item in this.Changed_Properties)
            {
                output.Changed_Properties.Add(item.Key, item.Value);
            }

            return output;
        }

        /// <summary>
        /// sets the class' properties to the value's of the input. also checks if changes are made and sets the properties accordingly
        /// </summary>
        /// <param name="Input"></param>
        public override void Assign(object Input)
        {
            if (Input == null)
                return;

            try
            {   
                //compare and set changed flags!
                Type type = typeof(ChangedTask);
                Type baseType = typeof(Task);

                if (Input.GetType() != type && Input.GetType() != baseType)
                    return;

                PropertyInfo[] properties = baseType.GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    object selfValue = type.GetProperty(item.Name).GetValue(this, null);
                    object toValue = baseType.GetProperty(item.Name).GetValue(Input, null);

                    bool changed = false;
                    if (
                        (selfValue == null && toValue != null) ||
                        (selfValue != null && toValue == null)
                        )
                    {
                        changed = true;
                    }
                    else if (
                        (selfValue != null && toValue != null) &&
                        (!(selfValue.ToString() == toValue.ToString()))
                        )
                    {
                        changed = true;
                    }

                    Changed_Properties[item.Name] = changed;
                }

                //then assign all the base properties :)
                //its a bit ineffecient since we are looping twice
                //TODO : think of a more effecient way?
                base.Assign(Input);
            }
            catch (Exception ex)
            {
                throw new Exception("ChangedTask_Failed_Assign : " + ex.Message);
            }
            
        }

        /// <summary>
        /// Takes in a Task object and outputs a ChangedTask object with the same values and references. all changed properties are set to false.
        /// </summary>
        /// <param name="baseClass"></param>
        /// <returns>ChangedTask object</returns>
        static public ChangedTask UpgradeBase(Task baseClass)
        {
            return UpgradeBase(baseClass, false);
        }
        static public ChangedTask UpgradeBase(Task baseClass,bool ChangedValue)
        {
            if (baseClass == null)
                return null;

            ChangedTask ret = new ChangedTask();

            try
            {
                //set all items of the changedclass as the base, then set the changed flag if set
                Type type = typeof(ChangedTask);
                Type baseType = typeof(Task);
                PropertyInfo[] properties = baseType.GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    object toValue = baseType.GetProperty(item.Name).GetValue(baseClass, null);
                    type.GetProperty(item.Name).SetValue(ret, toValue);

                    if(ChangedValue == true)
                    {
                        ret.Changed_Properties.Add(item.Name, ChangedValue);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChangedTask_Failed_Upgrade : " + ex.Message);
            }


            return ret;
        }

        /// <summary>
        /// Constructor of the Changed Task
        /// </summary>
        public ChangedTask() { }
    }
}
