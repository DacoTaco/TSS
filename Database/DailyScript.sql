use TechnicalServiceSystem
go

-- first backup the database!
exec General.BackupDatabase
go

-- then check for repeating tasks!
exec Tasks.CheckAndCreateRepeatingTasks
go