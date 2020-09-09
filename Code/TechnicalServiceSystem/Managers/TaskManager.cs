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

using Equin.ApplicationFramework;
using NHibernate;
using NHibernate.Util;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using TechnicalServiceSystem.Entities.Tasks;
using TechnicalServiceSystem.Entities.Users;
using TechnicalServiceSystem.Utilities;

namespace TechnicalServiceSystem
{
    [DataObject(true)]
    public class TaskManager : DatabaseManager
    {
        /// <summary>
        ///     Retrieves a list of tasks from the database
        /// </summary>
        /// <param name="desc"></param>
        /// <returns>ObservableCollection</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public BindingListView<Task> GetTasks(int? DepartmentID = null, string SortBy = null, string SearchText = null)
        {
            var company = Settings.GetCompanyName();
            if (DepartmentID.HasValue && DepartmentID.Value <= 0)
                DepartmentID = -1;

            var list = new BindingListView<Task>(GetTasks(SearchText, company, DepartmentID, null));

            //basically, this string has the property + direction. for example : StatusID DESC
            //TODO : pull this out of here and process the sorting in database to speed it up...
            if(!String.IsNullOrWhiteSpace(SortBy))
                list.Sort = SortBy;

            return list;
        }

        public Task GetTasks(string company, int TaskID) => (Task)GetTasks(null, company, null, TaskID).FirstOrNull();

        public ObservableCollection<Task> GetTasks(string contains, string company, int? DepartmentID, int? taskID)
        {
            ObservableCollection<Task> taskList = null;
            try
            {
                taskList = new ObservableCollection<Task>(
                    Session.CreateSQLQuery(
                            "exec Tasks.GetTask :TaskID, :companyName, :departmentID, :contains")
                        .AddEntity(typeof(Task))
                        .SetParameter("TaskID", (taskID ?? 0) > 0 ? taskID : null, NHibernateUtil.Int32)
                        .SetParameter("companyName", company, NHibernateUtil.String)
                        .SetParameter("departmentID", (DepartmentID ?? -5) > -5 ? DepartmentID : null,
                            NHibernateUtil.Int32)
                        .SetParameter("contains", !string.IsNullOrWhiteSpace(contains) ? contains : null,
                            NHibernateUtil.String)
                        .List<Task>()
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Tasks : " + ex.Message, ex);
            }

            return taskList;
        }


        /// <summary>
        ///     Checks whether a Task is allowed to be editted
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool TaskEditable(int taskID, string userHash)
        {
            var ret = false;

            try
            {
                var connection = GetSession()?.Connection;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Tasks.IsTaskEditable";

                    var param = command.CreateParameter();
                    param.ParameterName = "TaskID";
                    param.Value = taskID;
                    command.Parameters.Add(param);

                    var hashParam = command.CreateParameter();
                    hashParam.Value = userHash;
                    hashParam.ParameterName = "userHash";
                    command.Parameters.Add(hashParam);

                    var retValue = command.CreateParameter();
                    retValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(retValue);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    bool? temp = null;
                    if (command.ExecuteNonQuery() >= -1)
                        if (retValue.Value != null)
                            temp = retValue.Value.ToString() == "1";

                    if (temp.HasValue)
                        ret = temp.Value;
                    else
                        throw new Exception("SQL error in retrieving Task opened state");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Check_Open : ", ex);
            }


            return ret;
        }

        /// <summary>
        ///     Set the given task as opened. this will keep the task set as opened for 30min or untill closed
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool SetTaskOpened(int TaskID, string userHash) => SetTaskOpenedState(TaskID, userHash, true);


        /// <summary>
        ///     Set a task closed so it can be editted again by somebody else
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <returns></returns>
        public bool SetTaskClosed(int TaskID, string userHash) => SetTaskOpenedState(TaskID, userHash, false);

        /// <summary>
        ///     Set the opened state of a given task.
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="userHash"></param>
        /// <param name="openedState"></param>
        /// <returns></returns>
        private bool SetTaskOpenedState(int taskID, string userHash, bool openedState)
        {
            var ret = false;
            try
            {
                //no return value's in Nhibernate :(
                //good old ADO.NET it is!
                var connection = GetSession()?.Connection;
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (openedState)
                        command.CommandText = "Tasks.SetTaskOpened";
                    else
                        command.CommandText = "Tasks.SetTaskClosed";

                    var param = command.CreateParameter();
                    param.ParameterName = "TaskID";
                    param.Value = taskID;
                    command.Parameters.Add(param);

                    var hashParam = command.CreateParameter();
                    hashParam.Value = userHash;
                    hashParam.ParameterName = "userHash";
                    command.Parameters.Add(hashParam);

                    var retValue = command.CreateParameter();
                    retValue.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(retValue);

                    bool? temp = null;
                    if (command.ExecuteNonQuery() >= -1)
                        if (retValue.Value != null)
                            temp = retValue.Value.ToString() == "1";

                    if (temp.HasValue)
                        ret = temp.Value;
                    else
                        throw new Exception("SQL error in setting Task opened state");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SetTaskOpenedState : " + ex.Message, ex);
            }

            return ret;
        }


        /// <summary>
        ///     retrieves list of all known tasktypes saved in database
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TaskType> GetTaskTypes()
        {
            try
            {
                return new ObservableCollection<TaskType>(
                    Session.QueryOver<TaskType>()
                        .OrderBy(x => x.ID).Asc
                        .List()
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Task_Types : " + ex.Message, ex);
            }
        }

        /// <summary>
        ///     Retrieves list from database with all known Task statusses
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TaskStatus> GetTaskStatuses()
        {
            try
            {
                return new ObservableCollection<TaskStatus>(
                    Session.QueryOver<TaskStatus>()
                        .OrderBy(x => x.ID).Asc
                        .List()
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Task_Manager_Failed_Get_Task_Statuses : " + ex.Message, ex);
            }
        }


        //saving tasks
        [DataObjectMethod(DataObjectMethodType.Insert)]
        public void AddTasks(Task task)
        {
            if (task == null)
                return;

            var con = Session.Connection;
            int errors;
            int newTaskID;

            if (!(con is SqlConnection))
                throw new Exception("TaskManager.AddTasks : function requires SQLconnection, which was not retrieved");
            var connection = con as SqlConnection;

            var transaction = Session.BeginTransaction();
            errors = 0;
            newTaskID = 0;
            try
            {
                //add task
                using (var commandAddTask = connection.CreateCommand())
                {
                    transaction.Enlist(commandAddTask);
                    commandAddTask.CommandType = CommandType.StoredProcedure;
                    var paraDescription = commandAddTask.CreateParameter();
                    paraDescription.ParameterName = "Description";
                    paraDescription.Value = task.Description;
                    paraDescription.DbType = DbType.String;
                    paraDescription.Size = task.Description.Length;
                    commandAddTask.Parameters.Add(paraDescription);

                    var parUrguent = commandAddTask.CreateParameter();
                    parUrguent.ParameterName = "Urguent";
                    parUrguent.Value = task.IsUrguent;
                    parUrguent.DbType = DbType.Boolean;
                    commandAddTask.Parameters.Add(parUrguent);

                    SqlParameter parReporterInfo;
                    parReporterInfo = commandAddTask.Parameters.AddWithValue("reporterInfo",
                        User.GenerateUserTable(
                            task.ReporterUser != null ? task.ReporterUser.ID : 0,
                            task.Reporter)
                    );
                    parReporterInfo.SqlDbType = SqlDbType.Structured;
                    parReporterInfo.TypeName = "Users.UserInfo";

                    var parLocation = commandAddTask.CreateParameter();
                    parLocation.Value = task.Location.ID;
                    parLocation.ParameterName = "locationID";
                    commandAddTask.Parameters.Add(parLocation);

                    var parMachine = commandAddTask.CreateParameter();
                    parMachine.ParameterName = "MachineID";
                    if ((task.Device?.ID ?? 0) == 0)
                        parMachine.Value = DBNull.Value;
                    else
                        parMachine.Value = task.Device.ID;
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


                    var TypeID = task.TypeID;
                    if ((TypeID == Convert.ToInt32(TaskTypes.RepeatingTask)) & (task.RepeatingInfo != null))
                    {
                        commandAddTask.CommandText = "Tasks.CreateRepeatingTask";

                        var parActivationDate = commandAddTask.CreateParameter();
                        parActivationDate.ParameterName = "ActivationDate";
                        parActivationDate.Value = task.RepeatingInfo.ActivationDate;
                        parActivationDate.DbType = DbType.DateTime;
                        commandAddTask.Parameters.Add(parActivationDate);

                        var parInterval = commandAddTask.CreateParameter();
                        parInterval.ParameterName = "DaysToRepeat";
                        parInterval.Value = task.RepeatingInfo.Interval;
                        commandAddTask.Parameters.Add(parInterval);
                    }
                    else
                    {
                        commandAddTask.CommandText = "Tasks.CreateNormalTask";
                    }

                    if (commandAddTask.ExecuteNonQuery() == 0)
                    {
                        //Drop this shit and go to the next task
                        Session.Transaction.Rollback();
                        return;
                    }

                    newTaskID = (int)TaskID.Value;

                    if (newTaskID == 0)
                    {
                        transaction.Rollback();
                        return;
                    }
                }

                //task created, add notes && photo
                foreach (var note in task.Notes)
                    using (var command = connection.CreateCommand())
                    {
                        transaction.Enlist(command);
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

                        if (command.ExecuteNonQuery() == 0)
                        {
                            errors = 1;
                            break;
                        }
                    }

                if (errors != 0)
                {
                    transaction.Rollback();
                    return;
                }

                var gnrlManager = new GeneralManager();
                //no errors there, so lets add the photos to the DB...
                foreach (var photo in task.Photos)
                {
                    photo.FileName = string.Format("Task_{0}_Photo_{1}", newTaskID,
                        DateTime.Now.ToString("yyyyMMdd_HH_mm_ss"));

                    var item = photo;
                    if (!gnrlManager.SavePhotoToServer(ref item))
                        continue;

                    var photoID = item.ID;
                    if (photoID <= 0) continue;

                    using (var command = connection.CreateCommand())
                    {
                        transaction.Enlist(command);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.AssignPhoto";

                        var parTaskID = command.CreateParameter();
                        parTaskID.ParameterName = "taskID";
                        parTaskID.Value = newTaskID;
                        command.Parameters.Add(parTaskID);

                        var parPhoto = command.CreateParameter();
                        parPhoto.ParameterName = "photoName";
                        parPhoto.Value = item.FileName;
                        command.Parameters.Add(parPhoto);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            errors = 1;
                            break;
                        }
                    }
                }

                if (errors != 0)
                {
                    transaction.Rollback();
                    return;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }

            transaction.Commit();
        }

        [DataObjectMethod(DataObjectMethodType.Update)]
        public void ChangeTasks(Task task)
        {
            if (task == null)
                return;

            var session = GetSession();
            var connection = session.Connection;
            using (var trans = session.BeginTransaction())
            {
                try
                {
                    using (var command = session.Connection.CreateCommand())
                    {
                        trans.Enlist(command);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Tasks.ChangeTask";

                        var taskID = command.CreateParameter();
                        taskID.ParameterName = "TaskID";
                        taskID.Value = task.ID;
                        command.Parameters.Add(taskID);

                        var changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "Description";
                        changed_parameter.Value = task.Description;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "Urguent";
                        changed_parameter.Value = task.IsUrguent;
                        command.Parameters.Add(changed_parameter);

                        //we need SqlParameter sadly... 
                        //but the casting seems to work fine since our connection is probably SQL, so meh :P
                        changed_parameter = command.CreateParameter();
                        var parReporterInfo = (SqlParameter)command.CreateParameter();
                        parReporterInfo.ParameterName = "reporterInfo";
                        parReporterInfo.Value = User.GenerateUserTable(
                            task.ReporterUser?.ID ?? 0,
                            task.Reporter);
                        parReporterInfo.SqlDbType = SqlDbType.Structured;
                        parReporterInfo.TypeName = "Users.UserInfo";
                        changed_parameter = parReporterInfo;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "TechnicianID";
                        changed_parameter.Value = (task.TechnicianID == 0) ? (object)DBNull.Value : (object)task.TechnicianID;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "MachineID";
                        changed_parameter.Value = ((task.Device?.ID ?? 0) == 0) ? (object)DBNull.Value : (object)task.Device?.ID;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "StatusID";
                        changed_parameter.Value = task.StatusID;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "locationID";
                        changed_parameter.Value = task.Location.ID;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "ActivationDate";
                        changed_parameter.Value = (object)task.RepeatingInfo?.ActivationDate ?? (object)DBNull.Value;
                        command.Parameters.Add(changed_parameter);

                        changed_parameter = command.CreateParameter();
                        changed_parameter.ParameterName = "DaysToRepeat";
                        changed_parameter.Value = (object)task.RepeatingInfo?.Interval ?? (object)DBNull.Value;
                        command.Parameters.Add(changed_parameter);

                        if (command.Parameters.Count < 1 || command.ExecuteNonQuery() == 0)
                        {
                            trans.Rollback();
                            return;
                        }

                        foreach (var note in task.Notes)
                        {
                            if (note.ID > 0)
                                continue;

                            using (var noteCommand = connection.CreateCommand())
                            {
                                trans.Enlist(noteCommand);
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

                                if (noteCommand.ExecuteNonQuery() == 0) continue;
                            }
                        }

                        //assign new photos
                        foreach (var photo in task.Photos)
                        {
                            if (photo.ID > 0) continue;

                            var gnrlManager = new GeneralManager();
                            var item = photo;

                            if (!gnrlManager.SavePhotoToServer(ref item) && item.ID <= 0) continue;

                            using (var photoCommand = connection.CreateCommand())
                            {
                                trans.Enlist(photoCommand);
                                photoCommand.CommandType = CommandType.StoredProcedure;
                                photoCommand.CommandText = "Tasks.AssignPhoto";

                                var parTaskID = photoCommand.CreateParameter();
                                parTaskID.ParameterName = "taskID";
                                parTaskID.Value = task.ID;
                                photoCommand.Parameters.Add(parTaskID);

                                var parPhoto = photoCommand.CreateParameter();
                                parPhoto.ParameterName = "photoName";
                                parPhoto.Value = item.FileName;
                                photoCommand.Parameters.Add(parPhoto);

                                if (photoCommand.ExecuteNonQuery() == 0) continue;
                            }
                        }
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }
    }
}