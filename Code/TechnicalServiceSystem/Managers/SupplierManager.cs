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

using Equin.ApplicationFramework;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TechnicalServiceSystem.Entities.Suppliers;

namespace TechnicalServiceSystem
{
    /// <summary>
    ///     Suppliers Manager of TSS. Manages all data of the suppliers and their contact information
    /// </summary>
    [DataObject(true)]
    public class SupplierManager : Utilities.DatabaseManager
    {
        /// <summary>
        ///     Retrieve a list with all known machines from database
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<Machine> GetMachines(string SortBy = null)
        {
            var query = Session.QueryOver<Machine>();

            if(!string.IsNullOrWhiteSpace(SortBy))
            {
                var desc = SortBy.ToLower().Contains(" desc");
                var property = SortBy.Split().First();
                var queryBuilder = query.OrderBy(Projections.Property(property));

                if (desc)
                    query = queryBuilder.Desc;
                else
                    query = queryBuilder.Asc;
            }

            var list = query.List().ToList();
            return new BindingListView<Machine>(list);
        }

        public ObservableCollection<Machine> GetMachines()
        {
            try
            {
                return new ObservableCollection<Machine>(
                    Session.CreateSQLQuery("exec Suppliers.GetMachines")
                        .AddEntity(typeof(Machine))
                        .List<Machine>()
                );
            }
            catch (Exception ex)
            {
                throw new Exception("SuppliesManager : GetMachines Failure : " + ex.Message,ex);
            }
        }

        /// <summary>
        ///     Retrieve a list of all known machine types
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MachineType> GetMachineTypes()
        {
            try
            {
                return new ObservableCollection<MachineType>(
                    Session.QueryOver<MachineType>()
                    .OrderBy(x => x.ID).Asc
                    .List());
            }
            catch (Exception ex)
            {
                throw new Exception("SuppliesManager : GetMachinesType Failure : " + ex.Message);
            }
        }

        public Machine GetMachine(int machineID)
        {
            return Session.QueryOver<Machine>()
                .Where(m => m.ID == machineID)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Add machine to the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void AddMachine(Machine machine)
        {
            if (machine == null)
                throw new ArgumentException("Machine_Manager : AddMachines failure, arguments should not be null");

            var newMachineID = 0;
            var con = GetConnection();
            if(con.State == ConnectionState.Closed)
                con.Open();

            using (var trans = con.BeginTransaction())
            {
                try
                {
                    var errors = 0;
                    //add machine
                    using (var command = con.CreateCommand())
                    {
                        command.Transaction = trans;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Suppliers.AddMachine";

                        var Name = command.CreateParameter();
                        Name.Value = machine.Description;
                        Name.ParameterName = "MachineName";
                        command.Parameters.Add(Name);

                        var Serial = command.CreateParameter();
                        Serial.ParameterName = "SerialNumber";
                        Serial.Value = string.IsNullOrWhiteSpace(machine.SerialNumber) ? (object)DBNull.Value : machine.SerialNumber;
                        command.Parameters.Add(Serial);

                        var ModelNumber = command.CreateParameter();
                        ModelNumber.ParameterName = "ModelNumber";
                        ModelNumber.Value = string.IsNullOrWhiteSpace(machine.ModelNumber) ? (object)DBNull.Value : machine.ModelNumber;
                        command.Parameters.Add(ModelNumber);

                        var ModelName = command.CreateParameter();
                        ModelName.ParameterName = "ModelName";
                        ModelName.Value = string.IsNullOrWhiteSpace(machine.ModelName) ? (object)DBNull.Value : machine.ModelName;
                        command.Parameters.Add(ModelName);

                        var Supplier = command.CreateParameter();
                        Supplier.ParameterName = "SupplierID";
                        Supplier.Value = machine.Supplier.ID;
                        command.Parameters.Add(Supplier);

                        var Type = command.CreateParameter();
                        Type.ParameterName = "TypeID";
                        Type.Value = machine.Type.ID;
                        command.Parameters.Add(Type);

                        var NewID = command.CreateParameter();
                        NewID.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(NewID);


                        if (command.ExecuteNonQuery() == 0)
                        {
                            trans.Rollback();
                            return;
                        }

                        newMachineID = (int) NewID.Value;

                        if (newMachineID == 0)
                        {
                            trans.Rollback();
                            return;
                        }
                    }

                    //add photos
                    foreach (var photo in machine.Photos)
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

                    if (errors != 0)
                    {
                        trans.Rollback();
                        return;
                    }

                    //photos added, documentation time!
                    foreach (var document in machine.Documentations)
                        using (var command = con.CreateCommand())
                        {
                            command.Transaction = trans;
                            command.CommandText = "Suppliers.AddMachineDocumentation";
                            command.CommandType = CommandType.StoredProcedure;

                            var DocuName = command.CreateParameter();
                            DocuName.ParameterName = "DocumentationName";
                            DocuName.Value = document.DocumentationPath;
                            command.Parameters.Add(DocuName);

                            var MachineID = command.CreateParameter();
                            MachineID.ParameterName = "MachineID";
                            MachineID.Value = newMachineID;
                            command.Parameters.Add(MachineID);

                            if (command.ExecuteNonQuery() == 0)
                            {
                                errors = 1;
                                break;
                            }
                        }


                    if (errors == 0)
                        trans.Commit();
                    else
                        trans.Rollback();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        ///     Delete machines from the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void DeleteMachine(List<Machine> MachineList)
        {
            if (MachineList == null || MachineList.Count <= 0)
                return;

            var con = GetConnection();
            if(con.State == ConnectionState.Closed)
                con.Open();
            foreach (var machine in MachineList)
                using (var trans = con.BeginTransaction())
                {
                    try
                    {
                        //first, remove all documentation & photos
                        //documentation
                        using (var command = con.CreateCommand())
                        {
                            command.Transaction = trans;
                            command.CommandText =
                                "delete from Suppliers.Documentation where MachineID = @machineID";
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
                            command.CommandText =
                                "delete from Suppliers.MachinePhotos where MachineID = @machineID";
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
                    catch (Exception ex)
                    {
                        throw ex;
                    }
            }
        }

        public void DeleteMachine(Machine machine)
        {
            if (machine == null)
                return;

            var machines = new List<Machine>();
            DeleteMachine(machines);
        }

        /// <summary>
        ///     Update machines in the database
        /// </summary>
        /// <param name="MachineList"></param>
        public void ChangeMachine(List<Machine> MachineList)
        {
            if (MachineList == null || MachineList.Count <= 0)
                return;

            var connection = GetConnection();

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            var error = 0;
            foreach (var machine in MachineList)
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

                            var changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "machineName";
                            changed_parameter.Value = machine.Description;

                            changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "serialNumber";
                            changed_parameter.Value = machine.SerialNumber;

                            changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "modelNumber";
                            changed_parameter.Value = machine.ModelNumber;

                            changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "modelName";
                            changed_parameter.Value = machine.ModelName;

                            changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "supplierID";
                            changed_parameter.Value = machine.Supplier.ID;

                            changed_parameter = command.CreateParameter();
                            changed_parameter.ParameterName = "typeID";
                            changed_parameter.Value = machine.Type.ID;

                            if (command.Parameters.Count > 1 && command.ExecuteNonQuery() == 0)
                            {
                                trans.Rollback();
                                continue;
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

                        if (error != 0)
                        {
                            trans.Rollback();
                            continue;
                        }

                        //photos added, documentation time!
                        foreach (var document in machine.Documentations)
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandText = "Suppliers.AddMachineDocumentation";
                                command.CommandType = CommandType.StoredProcedure;

                                var DocuName = command.CreateParameter();
                                DocuName.ParameterName = "DocumentationName";
                                DocuName.Value = document.DocumentationPath;
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

                        if (error != 0)
                        {
                            trans.Rollback();
                            continue;
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
        }

        public void ChangeMachine(Machine machine)
        {
            if (machine == null)
                return;

            var list = new List<Machine>();
            list.Add(machine);
            ChangeMachine(list);
        }
    }
}