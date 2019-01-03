use TechnicalServiceSystem
go

-- first backup the database!
exec General.BackupDatabase
go

-- then check for repeating tasks!
exec Tasks.CheckAndCreateRepeatingTasks
go

-- check and delete Users that are set as deletion
exec Users.CheckAndDeleteUsers
go