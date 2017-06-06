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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem
{
    /// <summary>
    /// Suppliers Manager of TSS. Manages all data of the suppliers and their contact information
    /// </summary>
    public class SupplierManager :  DatabaseManager
    {
        /// <summary>
        /// Retrieve a list with all known machines from database
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MachineInfo> GetMachines()
        {
            ObservableCollection<MachineInfo> ret = new ObservableCollection<MachineInfo>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Suppliers.GetMachines";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 MachineIDPos = rdr.GetOrdinal("Machine ID");
                            Int32 MachineNamePos = rdr.GetOrdinal("Machine Name");
                            Int32 SerialNumberPos = rdr.GetOrdinal("Serial Number");
                            Int32 ModelNumberPos = rdr.GetOrdinal("Model Number");
                            Int32 ModelNamePos = rdr.GetOrdinal("Model Name");
                            Int32 SupplierPos = rdr.GetOrdinal("Supplier");
                            Int32 TypeNumberPos = rdr.GetOrdinal("Type ID");
                            
                            while (rdr.Read())
                            {
                                int id = rdr.GetInt32(MachineIDPos);
                                string name = rdr.GetString(MachineNamePos);
                                string serial;
                                if (rdr.IsDBNull(SerialNumberPos))
                                    serial = null;
                                else
                                    serial = rdr.GetString(SerialNumberPos);

                                string modelNumber;
                                if (rdr.IsDBNull(ModelNumberPos))
                                    modelNumber = null;
                                else
                                    modelNumber = rdr.GetString(ModelNumberPos);

                                ret.Add(new MachineInfo(
                                    id,
                                    name,
                                    serial,
                                    modelNumber,
                                    rdr.GetString(ModelNamePos),
                                    rdr.GetInt32(SupplierPos),
                                    rdr.GetInt32(TypeNumberPos)
                                    ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Supplier_Manager_Failed_GetMachines : " + ex.Message, ex);
            }


            return ret;
        }
            
        /// <summary>
        /// Retrieve a list of all known Suppliers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<SupplierInfo> GetSuppliers()
        {
            ObservableCollection<SupplierInfo> ret = new ObservableCollection<SupplierInfo>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from Suppliers.Supplier";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 IDPos = rdr.GetOrdinal("SupplierID");
                            Int32 NamePos = rdr.GetOrdinal("SupplierName");

                            while(rdr.Read())
                            {
                                ret.Add(new SupplierInfo(
                                    rdr.GetInt32(IDPos),
                                    rdr.GetString(NamePos)
                                    ));
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return ret;
        }

        /// <summary>
        /// Retrieve a list of all known machine types
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MachineType> GetMachineTypes()
        {
            ObservableCollection<MachineType> ret = new ObservableCollection<MachineType>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from Suppliers.MachineType";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 TypeIDPos = rdr.GetOrdinal("TypeID");
                            Int32 TypeNamePos = rdr.GetOrdinal("TypeName");
                            
                            while(rdr.Read())
                            {
                                ret.Add(new MachineType(rdr.GetInt32(TypeIDPos), rdr.GetString(TypeNamePos)));
                            }
                        }
                    }
                }
            }
            catch( Exception ex)
            {
                throw new Exception("SuppliesManager : GetMachinesType Failure : " + ex.Message);
            }
            return ret;
        }



        //---------------------------------------------------------------------------------
        //                                  SYNCING
        //                               -------------
        //      these syncing functions are used in WPF to push all changes in 1 instant
        //---------------------------------------------------------------------------------

        /// <summary>
        /// Add machine to the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void AddMachine(List<MachineInfo> MachineList)
        {
            if (MachineList == null || MachineList.Count <= 0)
            {
                throw new ArgumentException("Machine_Manager : AddMachines failure, arguments should not be null");
            }

            int newMachineID = 0;
            int errors = 0;
            using (var con = this.GetConnection())
            {
                con.Open();
                foreach (var machine in MachineList)
                {
                    using (var trans = con.BeginTransaction())
                    {
                        try
                        {
                            errors = 0;

                            //add machine
                            using (var command = con.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "Suppliers.AddMachine";

                                var Name = command.CreateParameter();
                                Name.Value = machine.Name;
                                Name.ParameterName = "MachineName";
                                command.Parameters.Add(Name);

                                var Serial = command.CreateParameter();
                                Serial.ParameterName = "SerialNumber";
                                if (String.IsNullOrWhiteSpace(machine.SerialNumber))
                                    Serial.Value = DBNull.Value;
                                else
                                    Serial.Value = machine.SerialNumber;
                                command.Parameters.Add(Serial);

                                var ModelNumber = command.CreateParameter();
                                ModelNumber.ParameterName = "ModelNumber";
                                if (String.IsNullOrWhiteSpace(machine.ModelNumber))
                                    ModelNumber.Value = DBNull.Value;
                                else
                                    ModelNumber.Value = machine.ModelNumber;
                                command.Parameters.Add(ModelNumber);

                                var ModelName = command.CreateParameter();
                                ModelName.ParameterName = "ModelName";
                                if (String.IsNullOrWhiteSpace(machine.ModelName))
                                    ModelName.Value = DBNull.Value;
                                else
                                    ModelName.Value = machine.ModelName;
                                command.Parameters.Add(ModelName);

                                var Supplier = command.CreateParameter();
                                Supplier.ParameterName = "SupplierID";
                                Supplier.Value = machine.SupplierID;
                                command.Parameters.Add(Supplier);

                                var Type = command.CreateParameter();
                                Type.ParameterName = "TypeID";
                                Type.Value = machine.TypeID;
                                command.Parameters.Add(Type);

                                var NewID = command.CreateParameter();
                                NewID.Direction = ParameterDirection.ReturnValue;
                                command.Parameters.Add(NewID);


                                if (command.ExecuteNonQuery() == 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }
                                else
                                {
                                    newMachineID = (int)NewID.Value;
                                }

                                if (newMachineID == 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }

                            }

                            //add photos
                            foreach (var photo in machine.Photos)
                            {
                                using (var command = con.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "Suppliers.AssignPhotoToMachine";

                                    var MachID = command.CreateParameter();
                                    MachID.ParameterName = "MachineID";
                                    MachID.Value = newMachineID;
                                    command.Parameters.Add(MachID);

                                    var photoParam = command.CreateParameter();
                                    photoParam.ParameterName = "photoName";
                                    photoParam.Value = photo.FileName;
                                    command.Parameters.Add(photoParam);

                                    if (command.ExecuteNonQuery() == 0)
                                    {
                                        errors = 1;
                                        break;
                                    }
                                }
                            }

                            if (errors != 0)
                            {
                                trans.Rollback();
                                continue;
                            }

                            //photos added, documentation time!
                            foreach (var document in machine.Documentations)
                            {
                                using (var command = con.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandText = "Suppliers.AddMachineDocumentation";
                                    command.CommandType = CommandType.StoredProcedure;

                                    var DocuName = command.CreateParameter();
                                    DocuName.ParameterName = "DocumentationName";
                                    DocuName.Value = document.Path;
                                    command.Parameters.Add(DocuName);

                                    var MachineID = command.CreateParameter();
                                    MachineID.ParameterName = "MachineID";
                                    MachineID.Value = newMachineID;
                                    command.Parameters.Add(MachineID);

                                    if(command.ExecuteNonQuery() == 0)
                                    {
                                        errors = 1;
                                        break;
                                    }
                                }
                            }


                            if (errors == 0)
                            {
                                trans.Commit();
                            }
                            else
                                trans.Rollback();
                        }
                        catch(Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }
        public void AddMachine(MachineInfo Machine)
        {
            if (Machine == null)
                return;

            List<MachineInfo> machine = new List<MachineInfo>();
            machine.Add(Machine);
            AddMachine(machine);
        }

        /// <summary>
        /// Delete machines from the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void DeleteMachine(List<MachineInfo> MachineList)
        {
            if (MachineList == null || MachineList.Count <= 0)
                return;

            using (var con = this.GetConnection())
            {
                con.Open();
                foreach (var machine in MachineList)
                {
                    using (var trans = con.BeginTransaction())
                    {
                        try
                        {
                            //first, remove all documentation & photos
                            //documentation
                            using (var command = con.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandText = "delete from Suppliers.Documentation where MachineID = @machineID";
                                command.CommandType = CommandType.Text;

                                var ID = command.CreateParameter();
                                ID.ParameterName = "machineID";
                                ID.Value = machine.ID;
                                command.Parameters.Add(ID);

                                if (command.ExecuteNonQuery() < 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }
                            }

                            //photo
                            using (var command = con.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandText = "delete from Suppliers.MachinePhotos where MachineID = @machineID";
                                command.CommandType = CommandType.Text;

                                var ID = command.CreateParameter();
                                ID.ParameterName = "machineID";
                                ID.Value = machine.ID;
                                command.Parameters.Add(ID);

                                if (command.ExecuteNonQuery() < 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }
                            }

                            using (var command = con.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandType = CommandType.Text;
                                command.CommandText = "delete from Suppliers.Machine where MachineID = @machineID";

                                var ID = command.CreateParameter();
                                ID.ParameterName = "machineID";
                                ID.Value = machine.ID;
                                command.Parameters.Add(ID);

                                if (command.ExecuteNonQuery() <= 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }
                            }
                            trans.Commit();
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }

        }
        public void DeleteMachine(MachineInfo machine)
        {
            if (machine == null)
                return;

            List<MachineInfo> machines = new List<MachineInfo>();
            DeleteMachine(machines);
        }

        /// <summary>
        /// Update machines in the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void ChangeMachine(List<ChangedMachineInfo> MachineList)
        {
            if (MachineList == null || MachineList.Count <= 0)
                return;

            using (var connection = GetConnection())
            {
                connection.Open();
                int error = 0;
                foreach (ChangedMachineInfo machine in MachineList)
                {
                    using (var trans = connection.BeginTransaction())
                    {
                        try
                        {
                            error = 0;
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "Suppliers.ChangeMachine";

                                var machineID = command.CreateParameter();
                                machineID.Value = machine.ID;
                                machineID.ParameterName = "machineID";
                                command.Parameters.Add(machineID);

                                foreach (KeyValuePair<string, Boolean> entry in machine.Changed_Properties)
                                {
                                    if (entry.Value == true)
                                    {
                                        var changed_parameter = command.CreateParameter();

                                        switch (entry.Key)
                                        {
                                            case "Name":
                                                changed_parameter.ParameterName = "machineName";
                                                changed_parameter.Value = machine.Name;
                                                break;
                                            case "SerialNumber":
                                                changed_parameter.ParameterName = "serialNumber";
                                                changed_parameter.Value = machine.SerialNumber;
                                                break;
                                            case "ModelNumber":
                                                changed_parameter.ParameterName = "modelNumber";
                                                changed_parameter.Value = machine.ModelNumber;
                                                break;
                                            case "ModelName":
                                                changed_parameter.ParameterName = "modelName";
                                                changed_parameter.Value = machine.ModelName;
                                                break;
                                            case "SupplierID":
                                                changed_parameter.ParameterName = "supplierID";
                                                changed_parameter.Value = machine.SupplierID;
                                                break;
                                            case "TypeID":
                                                changed_parameter.ParameterName = "typeID";
                                                changed_parameter.Value = machine.TypeID;
                                                break;
                                            default:
                                                break;
                                        }

                                        if (!String.IsNullOrWhiteSpace(changed_parameter.ParameterName))
                                            command.Parameters.Add(changed_parameter);
                                    }
                                }

                                if(command.Parameters.Count > 1)
                                {
                                    if(command.ExecuteNonQuery() == 0)
                                    {
                                        trans.Rollback();
                                        continue;
                                    }
                                }
                            }

                            //edditing machine done, add photos and documentations :)
                            foreach (var photo in machine.Photos)
                            {
                                if (photo.ID > 0)
                                    continue;

                                using (var command = connection.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "Suppliers.AssignPhotoToMachine";

                                    var MachID = command.CreateParameter();
                                    MachID.ParameterName = "MachineID";
                                    MachID.Value = machine.ID;
                                    command.Parameters.Add(MachID);

                                    var photoParam = command.CreateParameter();
                                    photoParam.ParameterName = "photoName";
                                    photoParam.Value = photo.FileName;
                                    command.Parameters.Add(photoParam);

                                    if (command.ExecuteNonQuery() == 0)
                                    {
                                        error = 1;
                                        break;
                                    }
                                }
                            }

                            if(error != 0)
                            {
                                trans.Rollback();
                                continue;
                            }

                            //photos added, documentation time!
                            foreach (var document in machine.Documentations)
                            {
                                using (var command = connection.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandText = "Suppliers.AddMachineDocumentation";
                                    command.CommandType = CommandType.StoredProcedure;

                                    var DocuName = command.CreateParameter();
                                    DocuName.ParameterName = "DocumentationName";
                                    DocuName.Value = document.Path;
                                    command.Parameters.Add(DocuName);

                                    var MachineID = command.CreateParameter();
                                    MachineID.ParameterName = "MachineID";
                                    MachineID.Value = machine.ID;
                                    command.Parameters.Add(MachineID);

                                    if (command.ExecuteNonQuery() == 0)
                                    {
                                        error = 1;
                                        break;
                                    }
                                }
                            }

                            if(error != 0)
                            {
                                trans.Rollback();
                                continue;
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            continue;
                        }
                    }
                }
            }
        }
        public void ChangeMachine(ChangedMachineInfo machine)
        {
            if (machine == null)
                return;

            List<ChangedMachineInfo> list = new List<ChangedMachineInfo>();
            list.Add(machine);
            ChangeMachine(list);
        }
    }
}
