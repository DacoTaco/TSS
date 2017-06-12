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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TechnicalServiceSystem.Base;

namespace TechnicalServiceSystem
{
    /// <summary>
    /// Task Manager of TSS. This Mananger contains everything to handle the Tasks Data of TSS
    /// </summary>
    public class TaskManager : DatabaseManager
    {
        /// <summary>
        /// Retrieves a list of tasks from the database
        /// </summary>
        /// <param name="desc"></param>
        /// <returns>ObservableCollection</returns>
        public ObservableCollection<Task> GetTasks(string company = "",int? DepartmentID = null)
        {
            return GetTasks(null, company, DepartmentID);
        }
        public ObservableCollection<Task> GetTasks(string contains,string company, int? DepartmentID, int? taskID)
        {
            ObservableCollection<Task> taskList = new ObservableCollection<Task>();
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.GetTask";
                        var param = command.CreateParameter();
                        param.ParameterName = "TaskID";
                        if (!taskID.HasValue || taskID == 0)
                            param.Value = DBNull.Value;
                        else
                            param.Value = taskID;
                        command.Parameters.Add(param);

                        if (!String.IsNullOrWhiteSpace(company))
                        {
                            var companyParam = command.CreateParameter();
                            companyParam.ParameterName = "companyName";
                            companyParam.Value = company;
                            command.Parameters.Add(companyParam);
                        }

                        if (DepartmentID.HasValue && DepartmentID.Value >= -5)
                        {
                            var DepartmentParam = command.CreateParameter();
                            DepartmentParam.ParameterName = "departmentID";
                            DepartmentParam.Value = DepartmentID;
                            command.Parameters.Add(DepartmentParam);
                        }

                        if(!String.IsNullOrWhiteSpace(contains))
                        {
                            var containsParam = command.CreateParameter();
                            containsParam.ParameterName = "contains";
                            containsParam.Value = contains;
                            command.Parameters.Add(containsParam);
                        }

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 TaskIDPos = rdr.GetOrdinal("Task ID");
                            Int32 TaskDescrPos = rdr.GetOrdinal("Task Description");
                            Int32 TaskUrgentPos = rdr.GetOrdinal("Urguent");
                            Int32 TaskTypePos = rdr.GetOrdinal("Task Type ID");
                            Int32 ReporterNamePos = rdr.GetOrdinal("Reporter");
                            Int32 TechnicianIDPos = rdr.GetOrdinal("Technician ID");
                            Int32 DepartmentIDPos = rdr.GetOrdinal("Department ID");
                            Int32 LocationIDPos = rdr.GetOrdinal("Location ID");
                            Int32 StatusPos = rdr.GetOrdinal("Status");
                            Int32 StatusIDPos = rdr.GetOrdinal("Status ID");
                            Int32 MachineIDPos = rdr.GetOrdinal("Machine ID");
                            Int32 CreatedPos = rdr.GetOrdinal("Created");
                            Int32 LastAdjustPos = rdr.GetOrdinal("Last Adjusted");
                            Int32 IntervalPos = rdr.GetOrdinal("Interval in days");
                            Int32 ActivationPos = rdr.GetOrdinal("Next Planned Date");

                            while (rdr.Read())
                            {
                                int MachineID;
                                int TaskID;
                                //depending on the Task type these can be null, so we need nullable variables
                                int? interval;
                                DateTime? activation;

                                ObservableCollection<Note> noteList;
                                ObservableCollection<PhotoInfo> photoList;


                                TaskID = rdr.GetInt32(TaskIDPos);

                                if (rdr.IsDBNull(MachineIDPos))
                                {
                                    MachineID = 0;
                                }
                                else
                                    MachineID = rdr.GetInt32(MachineIDPos);

                                //if one of the following is null, it means there is no repeating info. so null it is!
                                if (rdr.IsDBNull(ActivationPos) || rdr.IsDBNull(IntervalPos))
                                {
                                    activation = null;
                                    interval = null;
                                }
                                else
                                {
                                    activation = rdr.GetDateTime(ActivationPos);
                                    interval = rdr.GetInt32(IntervalPos);
                                }

                                //retrieve photo and note lists
                                noteList = GetNotes(TaskID);
                                photoList = GetPhotos(TaskID);



                                //add that shit!
                                taskList.Add(new Task(
                                    TaskID,
                                    rdr.GetString(TaskDescrPos),
                                    (rdr.GetString(TaskUrgentPos) == "Yes") ? true : false,
                                    rdr.GetInt32(TaskTypePos),
                                    0,
                                    rdr.GetString(ReporterNamePos),
                                    rdr.GetInt32(TechnicianIDPos),
                                    rdr.GetInt32(DepartmentIDPos),
                                    rdr.GetInt32(LocationIDPos),
                                    rdr.GetInt32(StatusIDPos),
                                    MachineID,
                                    rdr.GetDateTime(CreatedPos),
                                    rdr.GetDateTime(LastAdjustPos),
                                    activation,
                                    interval,
                                    noteList,
                                    photoList
                                    ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Tasks : " + ex.Message, ex);
            }

            return taskList;
        }
        public ObservableCollection<Task> GetTasks(string contains, string company = "", int? DepartmentID = null)
        {
            return GetTasks(contains, company, DepartmentID, 0);
        }
        public Task GetTasks(string company,int TaskID)
        {
            Task ret = null;

            ObservableCollection<Task> tasks = GetTasks(null, company, null, TaskID);

            if (tasks == null || tasks.Count > 0)
                ret = tasks[0];
            else
                ret = null;

            return ret;
        }

        /// <summary>
        /// Retrieves a list of photo filenames associated with the given TaskID
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns>ObservableCollection</returns>
        public ObservableCollection<PhotoInfo> GetPhotos(int TaskID = 0)
        {
            ObservableCollection<PhotoInfo> photos = new ObservableCollection<PhotoInfo>();
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.GetPhotos";
                        var param = command.CreateParameter();
                        param.ParameterName = "taskID";
                        param.Value = TaskID;
                        command.Parameters.Add(param);

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 PhotoIDPos = rdr.GetOrdinal("PhotoID");
                            Int32 PhotoNamePos = rdr.GetOrdinal("PhotoName");
                            while (rdr.Read())
                            {
                                photos.Add(new PhotoInfo(
                                    rdr.GetInt32(PhotoIDPos),
                                    rdr.GetString(PhotoNamePos)
                                    ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Notes : " + ex.Message, ex);
            }

            return photos;
        }
   
        /// <summary>
        /// Retrieval of the Notes of the given TaskID.
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns>ObservableCollection</returns>
        public ObservableCollection<Note> GetNotes(int TaskID = 0)
        {
            ObservableCollection<Note> notes = new ObservableCollection<Note>();
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.GetNotes";
                        var param = command.CreateParameter();
                        param.ParameterName = "taskID";
                        param.Value = TaskID;
                        command.Parameters.Add(param);

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 NoteIDPos = rdr.GetOrdinal("Note ID");
                            Int32 NotePos = rdr.GetOrdinal("Note");
                            Int32 NoteDatePos = rdr.GetOrdinal("Note Date");
                            while (rdr.Read())
                            {
                                notes.Add(new Note(rdr.GetInt32(NoteIDPos),rdr.GetString(NotePos), rdr.GetDateTime(NoteDatePos)));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Notes : " + ex.Message, ex);
            }

            return notes;
        }

        /// <summary>
        /// retrieves list of all known tasktypes saved in database
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TaskType> GetTaskTypes()
        {
            ObservableCollection<TaskType> ret = new ObservableCollection<TaskType>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from Tasks.TaskType";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 TypeIDPos = rdr.GetOrdinal("TypeID");
                            Int32 TypeNamePos = rdr.GetOrdinal("TypeDescription");
                            while (rdr.Read())
                            {
                                ret.Add(new TaskType(rdr.GetInt32(TypeIDPos), rdr.GetString(TypeNamePos)));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Task_Types : " + ex.Message, ex);
            }

            return ret;
        }

        /// <summary>
        /// Retrieves list from database with all known Task statusses
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TaskStatus> GetTaskStatuses()
        {
            ObservableCollection<TaskStatus> ret = new ObservableCollection<TaskStatus>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = "select * from Tasks.TaskStatus";

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 StatusIDPos = rdr.GetOrdinal("StatusID");
                            Int32 DescrPos = rdr.GetOrdinal("StatusDescription");
                            while (rdr.Read())
                            {
                                ret.Add(new TaskStatus(rdr.GetInt32(StatusIDPos), rdr.GetString(DescrPos)));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Task_Statuses : " + ex.Message, ex);
            }


            return ret;
        }


        /// <summary>
        /// Checks whether a Task is allowed to be editted
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool TaskEditable(int TaskID,string userHash)
        {
            bool ret = false;

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.IsTaskEditable";

                        var param = command.CreateParameter();
                        param.ParameterName = "TaskID";
                        param.Value = TaskID;
                        command.Parameters.Add(param);

                        var hashParam = command.CreateParameter();
                        hashParam.Value = userHash;
                        hashParam.ParameterName = "userHash";
                        command.Parameters.Add(hashParam);

                        var retValue = command.CreateParameter();
                        retValue.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(retValue);

                        connection.Open();
                        bool? temp = null;
                        if (command.ExecuteNonQuery() >= -1)
                        {
                            if (retValue.Value != null)
                                temp = (retValue.Value.ToString() == "1");
                        }

                        if (temp.HasValue)
                            ret = temp.Value;
                        else
                            throw new Exception("SQL error in retrieving Task opened state");

                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Check_Open : ",ex);
            }


            return ret;
        }

        /// <summary>
        /// Set the given task as opened. this will keep the task set as opened for 30min or untill closed
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool SetTaskOpened(int TaskID, string userHash)
        {
            return SetTaskOpenedState(TaskID, userHash, true);
        }
        
        /// <summary>
        /// Set a task closed so it can be editted again by somebody else
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool SetTaskClosed(int TaskID, string userHash)
        {
            return SetTaskOpenedState(TaskID, userHash, false);
        }

        /// <summary>
        /// Set the opened state of a given task. 
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <param name="openedState"></param>
        /// <returns></returns>
        private bool SetTaskOpenedState(int TaskID, string userHash,bool openedState)
        {
            bool ret = false;
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (openedState)
                            command.CommandText = "Tasks.SetTaskOpened";
                        else
                            command.CommandText = "Tasks.SetTaskClosed";

                        var param = command.CreateParameter();
                        param.ParameterName = "TaskID";
                        param.Value = TaskID;
                        command.Parameters.Add(param);

                        var hashParam = command.CreateParameter();
                        hashParam.Value = userHash;
                        hashParam.ParameterName = "userHash";
                        command.Parameters.Add(hashParam);

                        var retValue = command.CreateParameter();
                        retValue.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(retValue);

                        connection.Open();
                        bool? temp = null;
                        if (command.ExecuteNonQuery() >= -1)
                        {
                            if (retValue.Value != null)
                                temp = (retValue.Value.ToString() == "1");
                        }

                        if (temp.HasValue)
                            ret = temp.Value;
                        else
                            throw new Exception("SQL error in setting Task opened state");

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Check_Set_Open : ", ex);
            }

            return ret;
        }

        //---------------------------------------------------------------------------------
        //                                  SYNCING
        //                               -------------
        //      these syncing functions are used in WPF to push all changes in 1 instant
        //---------------------------------------------------------------------------------

        /// <summary>
        /// Syncing : add tasks to the database
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTasks(List<Task> tasks)
        {
            if(tasks == null || tasks.Count <= 0)
            {
                throw new ArgumentException("TaskManager.AddTasks : Arguments should not be null or empty");
            }



            int newTaskID = 0;
            int errors = 0;
            using (var con = this.GetConnection())
            {
                SqlConnection connection;
                if (!(con is SqlConnection))
                {
                    //we didn't get a sqlconnection D:
                    throw new Exception("TaskManager.AddTasks : function requires SQLconnection, which was not retrieved");
                }
                connection = con as SqlConnection;
                connection.Open();
                foreach (Task task in tasks)
                {
                    errors = 0;

                    using (var trans = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            //add task
                            using (SqlCommand commandAddTask = connection.CreateCommand())
                            {
                                commandAddTask.Transaction = trans;
                                commandAddTask.CommandType = CommandType.StoredProcedure;

                                var paraDescription = commandAddTask.CreateParameter();
                                paraDescription.ParameterName = "Description";
                                paraDescription.Value = task.Description;
                                paraDescription.DbType = DbType.String;
                                paraDescription.Size = task.Description.Length;
                                commandAddTask.Parameters.Add(paraDescription);

                                var parUrguent = commandAddTask.CreateParameter();
                                parUrguent.ParameterName = "Urguent";
                                parUrguent.Value = task.Urguent;
                                parUrguent.DbType = DbType.Boolean;
                                commandAddTask.Parameters.Add(parUrguent);

                                SqlParameter parReporterInfo;
                                parReporterInfo = commandAddTask.Parameters.AddWithValue("reporterInfo", UserInfo.GenerateUserTable(task.ReporterID, task.Reporter));
                                parReporterInfo.SqlDbType = SqlDbType.Structured;
                                parReporterInfo.TypeName = "Users.UserInfo";

                                var parLocation = commandAddTask.CreateParameter();
                                parLocation.Value = task.LocationID;
                                parLocation.ParameterName = "locationID";
                                commandAddTask.Parameters.Add(parLocation);

                                var parMachine = commandAddTask.CreateParameter();
                                parMachine.ParameterName = "MachineID";
                                if (task.MachineID == null || task.MachineID == 0)
                                {
                                    parMachine.Value = DBNull.Value;
                                }
                                else
                                    parMachine.Value = task.MachineID;
                                commandAddTask.Parameters.Add(parMachine);

                                var parStatus = commandAddTask.CreateParameter();
                                parStatus.ParameterName = "StatusID";
                                parStatus.Value = task.StatusID;
                                commandAddTask.Parameters.Add(parStatus);

                                var parTechnician = commandAddTask.CreateParameter();
                                parTechnician.ParameterName = "Technician";
                                if (task.TechnicianID == null || task.TechnicianID == 0)
                                    parTechnician.Value = DBNull.Value;
                                else
                                    parTechnician.Value = task.TechnicianID;
                                commandAddTask.Parameters.Add(parTechnician);

                                var TaskID = commandAddTask.CreateParameter();
                                TaskID.Direction = ParameterDirection.ReturnValue;
                                commandAddTask.Parameters.Add(TaskID);


                                int TypeID = task.TaskTypeID;
                                if (TypeID == 2)
                                {
                                    commandAddTask.CommandText = "Tasks.CreateRepeatingTask";

                                    var parActivationDate = commandAddTask.CreateParameter();
                                    parActivationDate.ParameterName = "ActivationDate";
                                    parActivationDate.Value = task.ActivationDate;
                                    parActivationDate.DbType = DbType.DateTime;
                                    commandAddTask.Parameters.Add(parActivationDate);

                                    var parInterval = commandAddTask.CreateParameter();
                                    parInterval.ParameterName = "DaysToRepeat";
                                    parInterval.Value = task.TaskInterval;
                                    commandAddTask.Parameters.Add(parInterval);
                                }
                                else
                                {
                                    commandAddTask.CommandText = "Tasks.CreateNormalTask";
                                }
                                if (commandAddTask.ExecuteNonQuery() == 0)
                                {
                                    //Drop this shit and go to the next task
                                    trans.Rollback();
                                    continue;
                                }
                                else
                                {
                                    newTaskID = (int)TaskID.Value;
                                }

                                if(newTaskID == 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }

                            } 


                            //task is added to DB, so lets add the task's notes...
                            foreach (var note in task.Notes)
                            {
                                using (var command = connection.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "Tasks.AddNote";

                                    var paraNote = command.CreateParameter();
                                    paraNote.ParameterName = "Note";
                                    paraNote.Value = note.Text;
                                    command.Parameters.Add(paraNote);

                                    var parTaskID = command.CreateParameter();
                                    parTaskID.ParameterName = "TaskID";
                                    parTaskID.Value = newTaskID;
                                    command.Parameters.Add(parTaskID);

                                    if(command.ExecuteNonQuery() == 0)
                                    {
                                        errors = 1;
                                        break;
                                    }
                                }
                            }
                            if(errors != 0)
                            {
                                trans.Rollback();
                                continue;
                            }
                            
                            //no errors there, so lets add the photos to the DB...
                            foreach (var photo in task.Photos)
                            {
                                photo.FileName = String.Format("Task_{0}_Photo_{1}", newTaskID, DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));

                                var gnrlManager = new GeneralManager();
                                PhotoInfo item = photo;
                                int photoID = gnrlManager.SavePhotoToServer(ref item);

                                if (photoID <= 0)
                                {
                                    //we failed to save the file, so fuck this NEXT!!
                                    continue;
                                }

                                using (var command = connection.CreateCommand())
                                {
                                    command.Transaction = trans;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "Tasks.AssignPhoto";

                                    var parTaskID = command.CreateParameter();
                                    parTaskID.ParameterName = "taskID";
                                    parTaskID.Value = newTaskID;
                                    command.Parameters.Add(parTaskID);

                                    var parPhoto = command.CreateParameter();
                                    parPhoto.ParameterName = "photoName";
                                    parPhoto.Value = photo.FileName;
                                    command.Parameters.Add(parPhoto);

                                    if(command.ExecuteNonQuery() == 0)
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

                            trans.Commit();
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
        public void AddTasks(Task task)
        {
            List<Task> TaskList = new List<Task>();
            TaskList.Add(task);
            AddTasks(TaskList);
        }

        /// <summary>
        /// Syncing : Delete tasks from the database
        /// </summary>
        /// <param name="tasks"></param>
        public void DeleteTasks(List<Task> tasks)
        {
            if (tasks == null || tasks.Count <= 0)
                return;

            using (var connection = GetConnection())
            {
                connection.Open();
                foreach (Task task in tasks)
                {
                    //hehehehe ( ͡° ͜ʖ ͡°)     
                    using (var transSexual = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transSexual;
                                command.CommandType = CommandType.Text;
                                command.CommandText = "delete from Tasks.Task where TaskID = @TaskID";

                                var taskID = command.CreateParameter();
                                taskID.ParameterName = "TaskID";
                                taskID.Value = task.ID;
                                command.Parameters.Add(taskID);

                                if(command.ExecuteNonQuery() == 0)
                                {
                                    transSexual.Rollback();
                                    continue;
                                }
                            }
                        }
                        catch
                        {
                            transSexual.Rollback();
                            continue;
                        }

                        transSexual.Commit();
                    }
                }
            }
        }
        public void DeleteTasks(Task Task)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task);
            DeleteTasks(tasks);
        }

        /// <summary>
        /// Syncing : Change tasks in the database, but only the properties set as changed
        /// </summary>
        /// <param name="tasks"></param>
        public void ChangeTasks(List<ChangedTask> tasks)
        {
            if (tasks == null || tasks.Count <= 0)
                return;

            using (var connection = GetConnection())
            {
                connection.Open();
                foreach (ChangedTask task in tasks)
                {
                    using (var trans = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = trans;
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "Tasks.ChangeTask";

                                var taskID = command.CreateParameter();
                                taskID.ParameterName = "TaskID";
                                taskID.Value = task.ID;
                                command.Parameters.Add(taskID);

                                foreach (KeyValuePair<string, Boolean> entry in task.Changed_Properties)
                                {
                                    if(entry.Value == true)
                                    {
                                        var changed_parameter = command.CreateParameter();
                                        switch (entry.Key)
                                        {
                                            case "Description":
                                                changed_parameter.ParameterName = "Description";
                                                changed_parameter.Value = task.Description;
                                                break;
                                            case "Urguent":
                                                changed_parameter.ParameterName = "Urguent";
                                                changed_parameter.Value = task.Urguent;
                                                break;
                                            case "Reporter":
                                                //we need SqlParameter sadly... 
                                                //but the casting seems to work fine since our connection is probably SQL, so meh :P
                                                SqlParameter parReporterInfo = (SqlParameter)command.CreateParameter();
                                                parReporterInfo.ParameterName = "reporterInfo";
                                                parReporterInfo.Value = UserInfo.GenerateUserTable(task.ReporterID, task.Reporter);
                                                parReporterInfo.SqlDbType = SqlDbType.Structured;
                                                parReporterInfo.TypeName = "Users.UserInfo";
                                                changed_parameter = (DbParameter)parReporterInfo;
                                                break;
                                            case "TechnicianID":
                                                changed_parameter.ParameterName = "TechnicianID";
                                                if (task.TechnicianID == 0)
                                                    changed_parameter.Value = DBNull.Value;
                                                else
                                                    changed_parameter.Value = task.TechnicianID;
                                                break;
                                            case "MachineID":
                                                changed_parameter.ParameterName = "MachineID";
                                                changed_parameter.Value = task.MachineID;
                                                break;
                                            case "StatusID":
                                                changed_parameter.ParameterName = "StatusID";
                                                changed_parameter.Value = task.StatusID;
                                                break;
                                            case "LocationID":
                                                changed_parameter.ParameterName = "locationID";
                                                changed_parameter.Value = task.LocationID;
                                                break;
                                            case "ActivationDate":
                                                changed_parameter.ParameterName = "ActivationDate";
                                                if (task.ActivationDate == null)
                                                    changed_parameter.Value = DBNull.Value;
                                                else
                                                    changed_parameter.Value = task.ActivationDate;
                                                break;
                                            case "TaskInterval":
                                                changed_parameter.ParameterName = "DaysToRepeat";
                                                if (task.TaskInterval == null)
                                                    changed_parameter.Value = DBNull.Value;
                                                else
                                                    changed_parameter.Value = task.TaskInterval;
                                                break;
                                            case "Notes":
                                                //add the new notes. dont pass the shit, we do this aside from the task itself
                                                foreach (Note note in task.Notes)
                                                {
                                                    if (note.ID > 0)
                                                        continue;

                                                    using (var noteCommand = connection.CreateCommand())
                                                    {
                                                        noteCommand.Transaction = trans;
                                                        noteCommand.CommandType = CommandType.StoredProcedure;
                                                        noteCommand.CommandText = "Tasks.AddNote";

                                                        var paraNote = noteCommand.CreateParameter();
                                                        paraNote.ParameterName = "Note";
                                                        paraNote.Value = note.Text;
                                                        noteCommand.Parameters.Add(paraNote);

                                                        var parTaskID = noteCommand.CreateParameter();
                                                        parTaskID.ParameterName = "TaskID";
                                                        parTaskID.Value = task.ID;
                                                        noteCommand.Parameters.Add(parTaskID);

                                                        if (noteCommand.ExecuteNonQuery() == 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                                break;
                                            case "Photos":
                                                //assign new photos
                                                foreach (PhotoInfo photo in task.Photos)
                                                {
                                                    if (photo.ID > 0)
                                                    {
                                                        continue;
                                                    }

                                                    var gnrlManager = new GeneralManager();
                                                    PhotoInfo item = photo;
                                                    int photoID = gnrlManager.SavePhotoToServer(ref item);

                                                    if (photoID <= 0)
                                                    {
                                                        //we failed to save the file, so fuck this NEXT!!
                                                        continue;
                                                    }

                                                    using (var photoCommand = connection.CreateCommand())
                                                    {
                                                        photoCommand.Transaction = trans;
                                                        photoCommand.CommandType = CommandType.StoredProcedure;
                                                        photoCommand.CommandText = "Tasks.AssignPhoto";

                                                        var parTaskID = photoCommand.CreateParameter();
                                                        parTaskID.ParameterName = "taskID";
                                                        parTaskID.Value = task.ID;
                                                        photoCommand.Parameters.Add(parTaskID);

                                                        var parPhoto = photoCommand.CreateParameter();
                                                        parPhoto.ParameterName = "photoName";
                                                        parPhoto.Value = photo.FileName;
                                                        photoCommand.Parameters.Add(parPhoto);

                                                        if (photoCommand.ExecuteNonQuery() == 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        if(!String.IsNullOrWhiteSpace(changed_parameter.ParameterName))
                                            command.Parameters.Add(changed_parameter);
                                    }
                                }
                                if(command.Parameters.Count < 1 || command.ExecuteNonQuery() == 0)
                                {
                                    trans.Rollback();
                                    continue;
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            continue;
                        }
                        trans.Commit();
                    }
                }

            }
        }
        public void ChangeTasks(ChangedTask task)
        {
            List<ChangedTask> Tasks = new List<ChangedTask>();
            Tasks.Add(task);
            ChangeTasks(Tasks);
        }
    }
}
