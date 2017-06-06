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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalServiceSystem.Base
{
    //TBH i dont think this class is that usefull anymore. just like the changedTask it was made to reduce the data being pushed to the SQL server and 
    //hopefully prevent data from overwriting somebody else's edits. but now that in ASP/MVC we set machines and tasks open AND push the changes instantly its not needed anymore
    /// <summary>
    /// Class containing the Changed MachineInfo Properties used for syncing
    /// </summary>
    public class ChangedMachineInfo : MachineInfo
    {
        protected ChangedMachineInfo() { }
        public ChangedMachineInfo(int MachineID, string name, string serialNumber, string model_number, string model_name, int supplierId, int typeID) : base(MachineID,name,serialNumber,model_number,model_name,supplierId,typeID) { }


        //a Dictionary list containing all properties that changed (and a boolean to verify it is indeed changed)
        public Dictionary<String, Boolean> Changed_Properties { get; set; } = new Dictionary<string, bool>();

        public new ChangedMachineInfo Clone()
        {
            ChangedMachineInfo output = (ChangedMachineInfo)base.Clone();

            output.Changed_Properties = new Dictionary<string, bool>();

            foreach (var item in this.Changed_Properties)
            {
                output.Changed_Properties.Add(item.Key, item.Value);
            }

            return output;
        }

        public override void Assign(object Input)
        {
            if (Input == null)
                return;

            try
            {
                //compare and set changed flags!
                Type type = typeof(ChangedMachineInfo);
                Type baseType = typeof(MachineInfo);

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
                throw new Exception("ChangedMachineInfo_Failed_Assign : " + ex.Message);
            }

        }
        static public ChangedMachineInfo UpgradeBase(MachineInfo baseClass,bool forcedChanged)
        {
            if (baseClass == null)
                return null;

            ChangedMachineInfo ret = new ChangedMachineInfo();

            //use reflection and generic methods to copy all data from the base class into the upgraded class!
            //also setting the changed flag if needed
            try
            {
                Type type = typeof(ChangedMachineInfo);
                Type baseType = typeof(MachineInfo);
                PropertyInfo[] properties = baseType.GetProperties();
                foreach (PropertyInfo item in properties)
                {
                    object toValue = baseType.GetProperty(item.Name).GetValue(baseClass, null);
                    type.GetProperty(item.Name).SetValue(ret, toValue);

                    if (forcedChanged == true)
                    {
                        ret.Changed_Properties.Add(item.Name, forcedChanged);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ChangedMachineInfo_Failed_Upgrade : " + ex.Message);
            }


            return ret;
        }
        static public ChangedMachineInfo UpgradeBase(MachineInfo baseClass)
        {
            return UpgradeBase(baseClass, false);
        }

    }
}
