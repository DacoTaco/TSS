use TechnicalServiceSystem
go

-- ----------------------------
-- Procedures
-- ----------------------------

--BACKUP!

CREATE PROCEDURE General.BackupDatabase
(
	@path nvarchar(max) = 'C:\SQL_BACKUPS'
)
as
BEGIN
	declare @date as date = GetDate()
	declare @filename as nvarchar(max)
	set @filename = CONCAT(@path,'\TechnicalServiceSystem_',@date,'.bak')
	BACKUP DATABASE TechnicalServiceSystem
	TO DISK = @filename 
	   WITH FORMAT,  
		  MEDIANAME = 'C_TechnicalServiceSystemBackups',  
		  NAME = 'Full Backup of TechnicalServiceSystem';  
END
go

--retrieval
CREATE PROCEDURE Suppliers.GetSupplierID
(
	@supplierName nvarchar(35)
)
AS
BEGIN
	declare @supplierID int = 0
	select @supplierID = sup.SupplierID
	from Suppliers.Supplier sup
	where sup.SupplierName = @supplierName
	
	RETURN @supplierID
END
go

CREATE PROCEDURE Suppliers.GetContacts
(
	@supplierName nvarchar(50) = '%'
)
AS
BEGIN
	Select sup.SupplierName as 'Supplier Name', con.ContactName as 'Contact', con.ContactStatus as'Status',
	con.PhoneNr as 'Phone Number', con.Fax as 'Fax', con.Email as 'E-mail',
	addr.AddressLine as 'Address Line', addr.AddressNr as 'Number', addr.AddressLine2 as 'Address Line 2',
	addr.Postcode, addr.City , addr.Region , addr.Country
	From Suppliers.Contact con
	left join General.Addresses addr on con.AddressID = addr.AddressID
	inner join Suppliers.Supplier sup on con.SupplierID = sup.SupplierID
	where sup.SupplierName like @supplierName
	order by sup.SupplierName asc,con.ContactID asc
END
go 

CREATE PROCEDURE Suppliers.AddMachine
(
	@MachineName nvarchar(40) = 'Not Set',
	@SerialNumber nvarchar(35) = null,
	@ModelNumber nvarchar(35) = 'Unknown',
	@ModelName nvarchar(35) = 'Unknown',
	@SupplierID int = null,
	@TypeID int = null
)
AS
BEGIN
	insert into Suppliers.Machine(MachineName,SerialNumber,ModelNumber,ModelName,SupplierID,TypeID)
	values
	(
		@MachineName,
		@SerialNumber,
		@ModelNumber,
		@ModelName,
		@SupplierID,
		@TypeID
	)
	IF(@@ERROR > 0)
		RETURN 0
	ELSE
		RETURN SCOPE_IDENTITY()
END
GO

CREATE PROCEDURE Suppliers.AddMachineDocumentation
(
	@DocumentationName nvarchar(max),
	@MachineID int
)
AS
BEGIN
	IF(
	(@DocumentationName is not null) or (@MachineID is not null)
	)
	BEGIN
		insert into Suppliers.Documentation(Documentation,MachineID)
		values
		(
			@DocumentationName,
			@MachineID
		)
	END
	IF(@@ERROR > 0)
		RETURN 0
	ELSE
		RETURN SCOPE_IDENTITY()
END
GO

CREATE PROCEDURE Suppliers.GetMachines
AS
BEGIN
	select ma.MachineID,ma.MachineName, ma.SupplierID, sup.SupplierName as 'Supplier Name', 
		ma.TypeID,mt.TypeName as 'Type Name' , ma.ModelName, ma.ModelNumber, ma.SerialNumber,
		(Select count(PhotoID) from Suppliers.MachinePhotos where MachineID = ma.MachineID) as 'Photos'
	from Suppliers.Machine ma
		inner join Suppliers.Supplier sup on sup.SupplierID=ma.SupplierID
		inner join Suppliers.MachineType mt on mt.TypeID = ma.TypeID
END
GO

CREATE PROCEDURE Suppliers.GetMachinesByDepartment
(
	@companyName nvarchar(35)
)
AS
BEGIN
	select dep.DepartmentName as 'Department' ,ma.MachineName as 'Machine Name', sup.SupplierName as 'Supplier', mt.TypeName as 'Type Name' , ma.ModelName as 'Model Name' ,
	ma.ModelNumber as 'Model NR' , ma.SerialNumber as 'Serie Number' ,
	(Select count(PhotoID) from Suppliers.MachinePhotos where MachineID = ma.MachineID) as 'Photos'
	from Suppliers.Machine ma
	inner join Suppliers.MachineDepartment md on md.MachineID = ma.MachineID
	inner join General.Department dep on dep.DepartmentID = md.DepartmentID
	inner join General.CompanyDepartment cd on cd.DepartmentID = dep.DepartmentID
	inner join General.Company cp on cp.CompanyID = cd.CompanyID
	inner join Suppliers.Supplier sup on sup.SupplierID=ma.SupplierID
	inner join Suppliers.MachineType mt on mt.TypeID = ma.TypeID
	where cp.CompanyName like @companyName
	order by dep.DepartmentName

END
go
CREATE PROCEDURE Suppliers.ChangeMachine
(
	@machineID int,
	@machineName nvarchar(40) = null,
	@serialNumber nvarchar(35) = null,
	@modelNumber nvarchar(35) = null,
	@modelName nvarchar(35) = null,
	@supplierID int = null,
	@typeID int = null
)
AS
BEGIN
	IF(@machineID is null or @machineID = 0)
	BEGIN
		RaisError ('Machine ID is invalid!', 15, 10)
		RETURN
	END

	BEGIN TRANSACTION CHANGE_MACHINE

		IF(@machineName is not null)
		BEGIN
			update Suppliers.Machine
			set
				MachineName = @machineName
			where MachineID = @machineID
		END

		IF(@serialNumber is not null)
		BEGIN
			update Suppliers.Machine
			set
				SerialNumber = @serialNumber
			where MachineID = @machineID
		END

		IF(@modelNumber is not null)
		BEGIN
			update Suppliers.Machine
			set
				ModelNumber = @modelNumber
			where MachineID = @machineID
		END

		IF(@modelName is not null)
		BEGIN
			update Suppliers.Machine
			set
				ModelName = @modelName
			where MachineID = @machineID
		END

		If(@supplierID is not null)
		BEGIN
			update Suppliers.Machine
			set
				SupplierID = @supplierID
			where MachineID = @machineID
		END

		IF(@typeID is not null)
		BEGIN
			update Suppliers.Machine
			set
				TypeID = @typeID
			where MachineID = @machineID
		END

	COMMIT TRANSACTION CHANGE_MACHINE
END
GO

--Tasks!

CREATE PROCEDURE Tasks.ReseedTables
AS
BEGIN
	declare @max int = null
	select @max=max([TaskID])from Tasks.Task
	if @max IS NUll   --check when max is returned as null
		SET @max = 1
	DBCC CHECKIDENT ('Tasks.Task', RESEED,@max)

	set @max = null
	select @max=max([NoteID])from Tasks.Notes
	if @max IS NUll   --check when max is returned as null
		SET @max = 1
	DBCC CHECKIDENT ('Tasks.Notes', RESEED,@max)

	set @max = null
	select @max=max([InfoID])from Tasks.RepeatingInfo
	if @max IS NUll  
		SET @max = 1
	DBCC CHECKIDENT ('Tasks.RepeatingInfo', RESEED,@max)

	set @max = null
	select @max=max([ID])from Tasks.TaskPhotos
	if @max IS NUll  
		SET @max = 1
	DBCC CHECKIDENT ('Tasks.TaskPhotos', RESEED,@max)
END
go

CREATE PROCEDURE Tasks.GetTask
(
	@TaskID int,
	@companyName nvarchar(max) = '%',
	@departmentID int = null,
	@contains nvarchar(max) = null,
	@showRepeating bit = 0,
	@showAll bit = 0
)
as
Begin

	IF(@departmentID is not null)
	BEGIN
		if(@departmentID = 0)
		BEGIN
		--	set @contains = null;
			set @departmentID = null;
		END
		--IF(@departmentID > 0)
		--BEGIN
		--	set @contains = null;
		--END
		IF(@departmentID < 0)
		BEGIN
			IF(@contains is null)
			BEGIN
				set @departmentID = null;
			END
		END
	END

	declare @text nvarchar(max) = null
	if(@contains is not null)
		set @text = ('%' + LOWER(@contains) + '%');

	declare @minStatusID int;
	declare @minTypeID int;


	if(@showRepeating is not null and @showRepeating = 1)
	BEGIN
		set @minTypeID = (select top 1 TypeID from Tasks.TaskType order by TypeID desc) + 1
		set @showAll = 1;
	END
	ELSE
	BEGIN
		set @minTypeID = 1;
	END

	if(@showAll is not null and @showAll = 1)
	BEGin
		set @minStatusID = (select TOP 1 StatusID from Tasks.TaskStatus order by StatusID desc) +1
	END
	ELSE
	BEGIN
		set @minStatusID = (select StatusID from Tasks.TaskStatus where StatusDescription = 'Task Finished')
	END

	select distinct tk.TaskID, tk.TaskDescription, tk.Urguent , tt.TypeID,
		tt.TypeDescription,tk.ReporterID,ISNULL((select UserName from Users.Users where UserID = tk.ReporterID),tk.ReporterName) as 'ReporterName', 
		tk.TechnicianID as 'TechnicianID', ISNULL(us.UserName,'None') as Technician,dep.DepartmentID,
		dep.DepartmentName as 'DepartmentName', tk.LocationID, loc.LocationName,tk.StatusID , 
		ts.StatusDescription as 'Status' , ISNULL(tk.MachineID,0) as 'MachineID', ISNULL(ma.MachineName,'Not Set') as 'MachineName', 
		tk.CreationDate, tk.DateLastADjustment, rt.RepeatInterval as 'Interval in days', 
		TRY_CONVERT(DATE, CAST(rt.ActivationDate as DATETIME),0) as 'Next Planned Date',
		tk.UserOpened, tk.OpenTimeDue,
		(Select count(1) from Tasks.Notes where TaskID = tk.TaskID) as 'Notes',
		(Select count(1) from Tasks.TaskPhotos where TaskID = tk.TaskID) as 'Photos'
	from Tasks.Task tk 
		inner join Tasks.TaskType tt on tk.TypeID = tt.TypeID
		inner join Tasks.TaskStatus ts on tk.StatusID = ts.StatusID
		left join Users.Users us on us.UserID=tk.TechnicianID
		inner join General.Locations loc on loc.LocationID = tk.LocationID
		inner join General.Department dep on loc.DepartmentID = dep.DepartmentID
		left join General.CompanyDepartment cd on cd.DepartmentID = dep.DepartmentID
		left join General.Company cp on cp.CompanyID = cd.CompanyID
		left join Tasks.RepeatingInfo rt on tk.TaskID = rt.ParentTaskID
		left join Suppliers.Machine ma on ma.MachineID = tk.MachineID
		left join Tasks.Notes nt on nt.TaskID = tk.TaskID
		where ((@TaskID is null) or (tk.TaskID = @TaskID))
			and cp.CompanyName like @companyName
			and
			(
				(@departmentID is null) or 
				( 
					(@departmentID != dep.DepartmentID or @departmentID is null) and
					(
						LOWER(dep.DepartmentName) like @text
						or LOWER(tk.ReporterName) like @text
						or LOWER(loc.LocationName) like @text
						or LOWER(us.UserName) like @text
						or LOWER(ma.MachineName) like @text
						or LOWER(tk.TaskDescription) like @text
					)
				)
				or
				(@departmentID = dep.DepartmentID)
			)
		and 
		(
			( tk.TypeID <= @minTypeID) 
			and 
			(
				( tk.StatusID < @minStatusID) or
				( datediff(day,tk.DateLastAdjustment,GETDATE()) < 14  or @showAll > 0)
			)
		)
	order by tt.TypeID asc,dep.DepartmentName asc,loc.LocationName asc,tk.DateLastAdjustment asc, tk.TaskDescription asc
END
GO

CREATE PROCEDURE Tasks.GetTasks
(
	@companyName nvarchar(max) = '%',
	@departmentID int = null,
	@contains nvarchar(max) = '%'
)
as
Begin
	exec Tasks.GetTask NULL,@companyName,@departmentID,@contains
END
GO

CREATE PROCEDURE Tasks.IsTaskEditable
(
	@TaskID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	IF(@TaskID is null or @TaskID <= 0)
	BEGIN
		RAISERROR('Given Task INVALID!',15,1);
	END

	declare @ret as bit = 1;
	declare @TimeDue DateTime = null;
	declare @userOpened nvarchar(max) = null;

	select @userOpened = tk.UserOpened , @TimeDue = tk.OpenTimeDue
	from Tasks.Task tk
	where tk.TaskID = @TaskID

	if(@@ROWCOUNT != 1)
	BEGIN
		RAISERROR('Given Task not found!',15,1);
	END

	if(@TimeDue is null or @userOpened is null)
		BEGIN
			set @ret = 1;
		END
	else
		BEGIN
			if(
				(DATEDIFF(minute,@TimeDue,GETDATE()) > 30) or 
				(@userOpened = @userHash)
			  )
				BEGIN
					set @ret = 1;
				END
			else
				BEGIN
					set @ret = 0;
				END
		END	
	RETURN @ret;
END
go

CREATE PROCEDURE Tasks.SetTaskOpenState
(
	@TaskID int,
	@userHash nvarchar(max),
	@state bit
)
AS
BEGIN
	declare @ret bit = 1;
	declare @newUser nvarchar(max);
	declare @newTime DateTime;

	IF(@TaskID is null or @TaskID <= 0)
	BEGIN
		RAISERROR('Given Task INVALID!',15,1);
		return 0;
	END
	
	IF(@userHash is null or LEN(@userHash) <= 1)
	BEGIN
		RAISERROR('Given UserHash is INVALID!',15,1);
		return 0;
	END

	IF(@state is null)
	BEGIN
		RAISERROR('No Task state is given (null)',15,1);
		return 0;
	END

	IF(@state = 1)
	BEGIN
		set @newUser = @userHash;
		set @newTime = DATEADD(minute,40,GETDATE());
	END
	ELSE
	BEGIN
		set @newUser = null;
		set @newTime = null;
	END

	declare @permission bit = 0;

	select @permission = 1 
	from Tasks.Task tk
	where tk.TaskID = @TaskID and 
	(
		( (tk.UserOpened is null) or (tk.UserOpened = @userHash)) or
		( (DATEDIFF(minute,tk.OpenTimeDue,GETDATE()) > 30) or (tk.OpenTimeDue is null))
	)

	IF(@permission != 1)
	BEGIN
		RAISERROR('Access Denied when setting Task Open State',15,1);
		return 0;
	END

	

	BEGIN Transaction SET_TaskOpenState
		update Tasks.Task
		set UserOpened = @newUser,
		OpenTimeDue = @newTime
		where TaskID = @TaskID

		if(@@ROWCOUNT != 1)
		BEGIN
			ROLLBACK TRANSACTION SET_TaskOpenState
			set @ret = 0;
		END
		ELSE
		BEGIN
			COMMIT TRANSACTION SET_TaskOpenState
		END

	RETURN @ret;

END
go

CREATE PROCEDURE Tasks.SetTaskOpened
(
	@TaskID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	declare @ret bit = 1;
	exec @ret = Tasks.SetTaskOpenState @TaskID,@userHash,1
	RETURN @ret;
END
go

CREATE PROCEDURE Tasks.SetTaskClosed
(
	@TaskID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	declare @ret bit = 1;
	exec @ret = Tasks.SetTaskOpenState @TaskID,@userHash,0
	RETURN @ret;
END
go

CREATE PROCEDURE Tasks.GetPhotos
(
	@taskID int
)
AS
BEGIN
	select tp.ID, tp.TaskID , ph.PhotoID , ph.PhotoName
	from Tasks.TaskPhotos tp
	left join General.Photo ph on tp.PhotoID = ph.PhotoID
	where tp.TaskID = @taskID
END

go

CREATE PROCEDURE General.GetDepartmentID
(
@departmentName nvarchar(50),@companyName nvarchar(50)
)
AS
BEGIN
	declare @ret int = 0
	Select @ret = dp.DepartmentID 
	from General.Department dp
	inner join General.CompanyDepartment cd on cd.DepartmentID = dp.DepartmentID
	inner join General.Company cp on cp.CompanyID = cd.CompanyID
	where dp.DepartmentName like @departmentName and cp.CompanyName like @companyName

	RETURN @ret
END
go

CREATE PROCEDURE General.GetDepartments
(
	@companyName nvarchar(128) = '%'
)
AS
BEGIN
	select dep.DepartmentID,dep.DepartmentName, ParentDepartmentID
	from General.Department dep
	left join General.CompanyDepartment cd on cd.DepartmentID = dep.DepartmentID
	left join General.Company com on com.CompanyID = cd.CompanyID
	where com.CompanyName like @companyName
END
GO

CREATE PROCEDURE General.GetDepartmentCompany
(
	@departmentID int
)
AS
BEGIN
	Select com.CompanyID, com.CompanyName
	from General.CompanyDepartment gd
	left join General.Company com on gd.CompanyID = com.CompanyID
	where gd.DepartmentID = @departmentID
END
go

CREATE PROCEDURE General.GetCompanyID
(
	@companyName nvarchar(50)
)
AS
BEGIN
	declare @ret int = null
	Select @ret = cp.CompanyID
	from General.Company cp
	where cp.CompanyName like @companyName

	RETURN @ret
END
go

CREATE PROCEDURE General.GetLocations
(
	@departmentID int = null,
	@companyName nvarchar(50) = '%'
)
AS
BEGIN

	IF(@departmentID is not null and @departmentID <= 0)
		set @departmentID = null;

	Select loc.LocationID as 'LocationID', loc.LocationName as 'LocationName', dep.DepartmentID as 'DepartmentID', dep.DepartmentName 'DepartmentName'
	From General.Locations loc
	inner join General.Department dep on dep.DepartmentID = loc.DepartmentID
	inner join General.CompanyDepartment cd on cd.DepartmentID = dep.DepartmentID
	inner join General.Company cp on cp.CompanyID = cd.CompanyID
	where cp.CompanyName like @companyName and 
	(@departmentID is null or dep.DepartmentID = @departmentID)
	order by loc.LocationName,dep.DepartmentName
END
go

CREATE PROCEDURE General.GetLocationsByID
(
	@departmentID int = NULL,
	@companyName nvarchar(50) = '%'
)
AS
BEGIN
	declare @name as nvarchar(60) = null

	select @name=dep.DepartmentName 
	from General.Department dep
	left join General.CompanyDepartment cd on cd.DepartmentID = dep.DepartmentID
	left join General.Company cp on cp.CompanyID = cd.CompanyID
	where (@departmentID is null) or (dep.DepartmentID = @departmentID)

	if(@name is null)
		set @name = '%'

	exec General.GetLocations @departmentID,@companyName
END

go

CREATE PROCEDURE General.GetLocation
(
	@locationName nvarchar(50),@departmentName nvarchar(50),@companyName nvarchar(50)
)
as
BEGIN
	declare @departmentID int = 0, @ret int = 0
	exec @departmentID = General.GetDepartmentID @departmentName,@companyName
	if(@departmentID > 0)
	BEGIN
		Select @ret = loc.LocationID
		From General.Locations loc
		where loc.LocationName = @locationName and loc.DepartmentID = @departmentID
	END
	RETURN @ret
END
go

--Company and Department stuff

CREATE PROCEDURE General.AddCompany
(
	@companyName nvarchar(50)
)
AS
BEGIN
	IF(@companyName is not null)
	BEGIN
		Insert into General.Company(CompanyName)
		values
		(
			@companyName
		)
	END
	Return SCOPE_IDENTITY()
END
go

CREATE PROCEDURE General.AddDepartment
(
	@departmentName nvarchar(40),@companyName nvarchar(50),@parentDepartmentName nvarchar(40) = null
)
AS
BEGIN
	declare @newDepID int = null

	IF(@companyName is not null and @departmentName is not null)
	BEGIN
		declare @companyID int = null
		exec @companyID = General.GetCompanyID @companyName

		declare @parentDepartmentID int = null
		exec @parentDepartmentID = General.GetDepartmentID @parentDepartmentName, @companyName

		IF(@parentDepartmentID = 0)
			set @parentDepartmentID = null

		IF(@companyID > 0)
		BEGIN
			BEGIN TRANSACTION ADD_DEP
				Insert into General.Department(DepartmentName,ParentDepartmentID)
				values
				(
					@departmentName,
					@parentDepartmentID
				);
				IF(@@ERROR <= 0)
				BEGIN
					set @newDepID = SCOPE_IDENTITY()
					insert into General.CompanyDepartment(CompanyID,DepartmentID)
					values
					(
						@companyID,
						@newDepID
					)
				END

				IF(@@ERROR > 0)
				BEGIN
					ROLLBACK TRANSACTION ADD_DEP
				END
				ELSE
				BEGIN
					COMMIT TRANSACTION ADD_DEP
				END
		END			
	END
	Return @newDepID
END
go

CREATE PROCEDURE General.AddLocation
(
	@locationName nvarchar(40),@departmentName nvarchar(40),@companyName nvarchar(40)
)
AS
BEGIN
	declare @ret int = 0
	if(@locationName is not null and @departmentName is not null and @companyName is not null)
	BEGIN
		declare @departmentID int = 0
		exec @departmentID = General.GetDepartmentID @departmentName,@companyName

		IF(@departmentID > 0)
		BEGIN
			insert into General.Locations(LocationName,DepartmentID)
			values
			(
				@locationName,
				@departmentID
			)
			set @ret = SCOPE_IDENTITY()
		END
	END
	RETURN @ret
END
go

--Login & users
CREATE PROCEDURE Users.ReseedTables
AS
BEGIN
	declare @max int
	select @max=max([UserID])from Users.Users
	if @max IS NUll   --check when max is returned as null
		SET @max = 1
	DBCC CHECKIDENT ('Users.Users', RESEED,@max)

	set @max = null
	select @max=max([UserID])from Users.UserRoles
	if @max IS NUll   --check when max is returned as null
		SET @max = 1
	DBCC CHECKIDENT ('Users.UserRoles', RESEED,@max)
END
go

CREATE PROCEDURE Users.IsUserEditable
(
	@UserID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	IF(@UserID is null or @UserID <= 0)
	BEGIN
		RAISERROR('Given User INVALID!',15,1);
		RETURN 0;
	END

	declare @ret as bit = 1;
	declare @TimeDue DateTime = null;
	declare @userOpened nvarchar(max) = null;

	select @userOpened = us.UserOpened , @TimeDue = us.OpenTimeDue
	from Users.Users us
	where us.UserID = @UserID

	if(@@ROWCOUNT != 1)
	BEGIN
		RAISERROR('Given User not found!',15,1);
		RETURN 0;
	END

	if(@TimeDue is null or @userOpened is null)
		BEGIN
			set @ret = 1;
		END
	else
		BEGIN
			if(
				(DATEDIFF(minute,@TimeDue,GETDATE()) > 30) or 
				(@userOpened = @userHash)
			  )
				BEGIN
					set @ret = 1;
				END
			else
				BEGIN
					set @ret = 0;
				END
		END	
	RETURN @ret;
END
go

CREATE PROCEDURE Users.SetUserOpenState
(
	@UserID int,
	@userHash nvarchar(max),
	@state bit
)
AS
BEGIN
	declare @ret bit = 1;
	declare @newUser nvarchar(max);
	declare @newTime DateTime;

	IF(@UserID is null or @UserID <= 0)
	BEGIN
		RAISERROR('Given User INVALID!',15,1);
		return 0;
	END
	
	IF(@userHash is null or LEN(@userHash) <= 1)
	BEGIN
		RAISERROR('Given UserHash is INVALID!',15,1);
		return 0;
	END

	IF(@state is null)
	BEGIN
		RAISERROR('No User state is given (null)',15,1);
		return 0;
	END

	IF(@state = 1)
	BEGIN
		set @newUser = @userHash;
		set @newTime = DATEADD(minute,40,GETDATE());
	END
	ELSE
	BEGIN
		set @newUser = null;
		set @newTime = null;
	END

	declare @permission bit = 0;

	select @permission = 1 
	from Users.Users us
	where us.UserID = @UserID and 
	(
	( (us.UserOpened is null) or (us.UserOpened = @userHash)) or
	( (DATEDIFF(minute,us.OpenTimeDue,GETDATE()) > 30) or (us.OpenTimeDue is null))
	)

	IF(@permission != 1)
	BEGIN
		RAISERROR('Access Denied when setting User Open State',15,1);
		return 0;
	END

	

	BEGIN Transaction SET_UserOpenState
		update Users.Users
		set UserOpened = @newUser,
		OpenTimeDue = @newTime
		where UserID = @UserID

		if(@@ROWCOUNT != 1)
		BEGIN
			ROLLBACK TRANSACTION SET_UserOpenState
			set @ret = 0;
		END
		ELSE
		BEGIN
			COMMIT TRANSACTION SET_UserOpenState
		END

	RETURN @ret;

END
go

CREATE PROCEDURE Users.SetUserOpened
(
	@UserID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	declare @ret bit = 1;
	exec @ret = Users.SetUserOpenState @UserID,@userHash,1
	RETURN @ret;
END
go

CREATE PROCEDURE Users.SetUserClosed
(
	@UserID int,
	@userHash nvarchar(max)
)
AS
BEGIN
	declare @ret bit = 1;
	exec @ret = Users.SetUserOpenState @UserID,@userHash,0
	RETURN @ret;
END
go

CREATE PROCEDURE Users.AssignRoles
(
	@userID int, 
	@Roles Users.UserRolesTable READONLY
)
AS
BEGIN
	declare @tempTable Users.UserRolesTable;
	/*Select *
	Into   #Temp
	From   @Roles*/
	insert into @tempTable
		select * from @Roles

	Declare @roleID int = 0;
	declare @tempID int = 0;

	--select @userID = (Select UserID from Users.Users where UserName like @userName and PasswordSalt like @passSalt)
	--IF (@userID is NULL) or (@@ERROR >0)
		--return 0;

	While EXISTS(SELECT * FROM @tempTable)
	Begin

		Select Top 1 @tempID = roleID From @tempTable

		IF(@tempID is not null and @tempID > 0)
		BEGIN
			Select @roleID = RoleID from Users.Roles rl where rl.RoleID = @tempID

			IF(@roleID is not null and @roleID > 0)
			BEGIN
				IF not EXISTS (SELECT * FROM Users.UserRoles WHERE UserID = @userID and RoleID = @roleID)
				begin
					insert into Users.UserRoles(UserID,RoleID)
					values
					(
						@userID,
						@roleID
					);
				end
			END
			Delete @tempTable Where roleID = @tempID
		END
		ELSE
		BEGIN
			WITH deleteTable AS
				(
				SELECT TOP 1 *
				FROM    @tempTable
				)
			DELETE
			FROM deleteTable
		END
		
	End
END
go

CREATE PROCEDURE Users.CheckAndDeleteUsers
AS
BEGIN
	declare @users as TABLE(ID int,UserName nvarchar(max),DateToDelete DateTime);
	
	insert into @users(ID,UserName,DateToDelete)
		select us.UserID,us.UserName,us.DateToDelete from Users.Users us
		where (us.DateToDelete is not null) and (CONVERT(date, us.DateToDelete) <= CONVERT(date, getdate()))


	IF EXISTS(Select * from @users)
	BEGIN
		While EXISTS(SELECT * FROM @users)
		BEGIN
			declare @userID int = 0;
			select TOP 1 @userID = ID from @users

			--delete from Users.Users 
			--where UserID = @userID
			update Users.Users
			set 
				Deleted = 1,
				Active = 0
			where 
				userID = @userID
				AND (Deleted is null or Deleted = 0)

			delete from @users where ID = @userID
		END

		--reseed the tables after deletion
		exec Users.ReseedTables;
	END
END
go

CREATE PROCEDURE Users.CreateUser
(
	@Username NVARCHAR(35),  
	@Password nvarchar(max), 
	@PhotoID integer, 
	@CompanyName nvarchar(50) , 
	@DepartmentID Integer, 
	@Roles Users.UserRolesTable READONLY,
	@Active bit = 1
)
AS
BEGIN
	DECLARE @RolesTable AS Users.UserRolesTable;

	IF not EXISTS (SELECT * FROM @Roles)
	BEGIN
		declare @id integer = 0;
		select @id = RoleID from Users.Roles where RoleName = 'User'
		INSERT INTO @RolesTable(roleID)
		values
			(
				--@id
				(select RoleID from Users.Roles where RoleName = 'User')
			)
	END
	else
	BEGIN
		insert into @RolesTable(roleID)
		select roleID from @Roles
	END

	declare @newUser integer = 0
	declare @passSalt nvarchar(max) = null

	set @passSalt = CONVERT(nvarchar(max), NEWID());

	declare @company nvarchar(max)

	Select @company = com.CompanyName
	from General.CompanyDepartment gd
	left join General.Company com on gd.CompanyID = com.CompanyID
	where gd.DepartmentID = @DepartmentID

	IF(
		(@DepartmentID = 0) or
		(@company != @CompanyName)
		)
		BEGIN
			RAISERROR('DepartmentID is 0 or company name is not correct',15,1);
			RETURN 0
		END

	IF(@PhotoID = 0)
	BEGIN
		set @PhotoID = null
	END


	begin transaction AddUser

	declare @error as int
	insert into Users.Users(UserName,DepartmentID,PasswordSalt,PasswordHash,PhotoID,Active)
	values
	(
		@Username,
		@DepartmentID,
		@passSalt,
		HASHBytes('SHA2_512',CONCAT(@passSalt,HASHBytes('SHA2_512',@Password)) ),
		@PhotoID,
		@Active
	);
	set @error = @@ERROR;
	
	IF @error > 0
		begin
			ROLLBACK transaction AddUser
			RAISERROR('Error was raised Inserting row',15,1);
			RETURN 0;
		end
	ELSE
		begin
			set @newUser = SCOPE_IDENTITY();
			exec Users.AssignRoles @newUser, @roles
		end
		
	commit transaction AddUser
	return @newUser
END
go

CREATE PROCEDURE Users.AssignPassword
(
	@UserID int,
	@Password nvarchar(max)
)
AS
BEGIN
	IF(
	(@userID is null or @UserID <= 0) OR
	@Password is null
	)
	BEGIN
		RAISERROR('Given values are invalid!',15,1);
		RETURN 0;
	END

	declare @rows int = 0;

	update Users.Users 
	set 
		PasswordHash = HASHBytes('SHA2_512',CONCAT(PasswordSalt,HASHBytes('SHA2_512',@Password)) )
	where UserID = @UserID

	set @rows = @@ROWCOUNT
	IF(@rows <= 0)
	BEGIN
		RAISERROR('FAILURE CHANGING USER PASSWORD!',15,1);
		RETURN 0;
	END
END
GO

CREATE PROCEDURE Users.CheckLogin
(
	@userID Int,  
	@password nvarchar(max), 
	@companyName nvarchar(50) , 
	@UserHash nvarchar(128) = null OUTPUT
)
AS
BEGIN
	declare @ret nvarchar(128)

	--declare @passSalt nvarchar(max)
	--select @passSalt = us.PasswordSalt from Users.Users us where us.UserID = @userID
	--declare @hash varbinary(128)
	--select @hash = us.PasswordHash from Users.Users us where us.UserID = @userID
	--select @passSalt as 'PassSalt',CONCAT('(',@userID,')') as 'userID',@hash as 'pass hash',HASHBytes('SHA2_512',CONCAT(@passSalt,HASHBytes('SHA2_512',@password)) ) as 'generated password'
	--select Username,PasswordHash,'0x' + CONVERT(nvarchar(max),PasswordHash,2),CONVERT(nvarchar(max),(HASHBytes('SHA2_512',CONCAT(us.PasswordSalt,HASHBytes('SHA2_512',CONCAT(us.UserID,':',us.UserName))) )),2)
	--from Users.Users us

	SELECT @ret = CONVERT(nvarchar(128),(HASHBytes('SHA2_512',CONCAT(us.PasswordSalt,HASHBytes('SHA2_512',CONCAT(us.UserID,':',us.UserName))) )),2)
	FROM Users.Users us
	inner join General.CompanyDepartment cd on cd.DepartmentID = us.DepartmentID
	inner join General.Company cp on cp.CompanyID = cd.CompanyID
	WHERE us.UserID = @userID
	AND us.PasswordHash is not null
	AND us.PasswordHash = HASHBytes('SHA2_512',CONCAT(us.PasswordSalt,HASHBytes('SHA2_512',@password)) )
	AND cp.CompanyName = @companyName
	AND us.Active = 1
	AND (us.Deleted is null or us.Deleted = 0)

	IF(@ret is null)
		return 0
	ELSE
	BEGIN
		set @UserHash = @ret
		return 1
	END
END
go

CREATE PROCEDURE Users.CheckLoginHash
(
	@UserID int,
	@UserHash nvarchar(128)
)
AS
BEGIN
	IF(
		(@UserID is null or @UserID <= 0) or
		(@UserHash is null or LEN(@UserHash) < 0)
	)
	BEGIN
		RETURN 0;
	END

	declare @ret int

	IF EXISTS(
		Select * 
		from Users.Users us
		where us.UserID = @UserID and 
		@UserHash = CONVERT(nvarchar(max),(HASHBytes('SHA2_512',CONCAT(us.PasswordSalt,HASHBytes('SHA2_512',CONCAT(us.UserID,':',us.UserName))) )),2)
		AND (us.Deleted is null or us.Deleted = 0)
	)
	BEGIN
		RETURN 1;
	END
	ELSE
	BEGIN
		RETURN 0;
	END
END
go

CREATE PROCEDURE Users.GetUsers
(
	@companyName nvarchar(max) = '%',
	@Active int = null,
	@search nvarchar(max) = '%',
	@RoleID int = null
)
AS
BEGIN
	declare @SearchText nvarchar(max)

	if(@search is not null and LEN(@search) > 0)
	BEGIN
		set @SearchText = CONCAT('%',LOWER(@search),'%');
	END
	ELSE
		set @SearchText = '%';

	if(@RoleID is not null and @RoleID <= 0)
		set @RoleID = null

	Select DISTINCT us.UserID, us.UserName, us.DepartmentID, us.PhotoID, us.Active, 
	CAST(
		 CASE 
			  WHEN us.DateToDelete is null or us.DateToDelete = 0
				 THEN 0
			  ELSE 1
		 END AS bit) as 'Delete'
	from Users.Users us
	left join General.CompanyDepartment cd on cd.DepartmentID = us.DepartmentID
	left join General.Company cp on cp.CompanyID = cd.CompanyID
	left join General.Department dp on dp.DepartmentID = us.DepartmentID
	left join Users.UserRoles ur on ur.UserID = us.UserID
	left join Users.Roles rl on rl.RoleID = ur.RoleID
	where cp.CompanyName like @companyName 
	and
	(
		(@RoleID is null or @RoleID = rl.RoleID) and
		(LOWER(us.UserName) like @SearchText or LOWER(dp.DepartmentName) like @SearchText)
	)
	AND (us.Deleted is null or us.Deleted = 0)
	AND ( @Active is null or us.Active = @Active)
	AND (@Active is null or us.PasswordHash is not null)
	order by UserName
END
go

CREATE PROCEDURE Users.GetUsersByRole
(
	@roles nvarchar(max) = '%',
	@companyName nvarchar(max) = '%'
)
AS
BEGIN
	Select us.UserID,us.UserName,us.DepartmentID,us.Active,us.PhotoID
	from Users.Users us
	left join Users.UserRoles ur on us.UserID = ur.UserID
	left join Users.Roles rs on rs.RoleID = ur.RoleID
	left join General.CompanyDepartment cd on cd.DepartmentID = us.DepartmentID
	left join General.Company cp on cp.CompanyID = cd.CompanyID
	where rs.RoleName like @roles and us.Active > 0
	and cp.CompanyName like @companyName
	AND (us.Deleted is null or us.Deleted = 0)
END

go

CREATE PROCEDURE Users.GetUserByID
(
	@userID int = 0
)
AS
BEGIN
	Select us.UserID,us.Username,us.Active,ph.PhotoID,dep.DepartmentID,dep.DepartmentName,
	CAST(
		CASE 
			WHEN us.DateToDelete is null or us.DateToDelete = 0
				THEN 0
			ELSE 1
		END AS bit) as 'Delete'
	from Users.Users us
	left join General.Department dep on Us.DepartmentID = dep.DepartmentID
	left join General.Photo ph on ph.PhotoID = us.PhotoID
	where us.UserID = @userID 
	AND (us.Deleted is null or us.Deleted = 0)
	--and us.Active = 1
END

go

CREATE PROCEDURE Users.GetUserRoles
(
	@userID int = 0
)
AS
BEGIN
	select us.UserID,us.UserName,rl.RoleID,rl.RoleName from Users.Users us
	left join Users.UserRoles ur on ur.UserID = us.UserID
	left join Users.Roles rl on rl.RoleID = ur.RoleID
	where us.UserID = @userID
	AND (us.Deleted is null or us.Deleted = 0)
END
go

CREATE PROCEDURE Tasks.AddNote
(
	@Note nvarchar(max),
	@TaskID int

)
AS
BEGIN
	IF( (@Note is null) or (@TaskID is null) )
	BEGIN
		RAISERROR('Parameters should not be null!',15,1)
		return 0
	END

	IF NOT EXISTS ( SELECT 1 FROM Tasks.Task WHERE TaskID = @TaskID )
	BEGIN
		RAISERROR('Given Task not found!',15,1)
		return 0
	END

	Insert Into Tasks.Notes(Note,TaskID)
	values
	(
		@Note,
		@TaskID
	)
	
	return SCOPE_IDENTITY()
END
go

CREATE PROCEDURE Tasks.CreateNormalTask
(
	@Description nvarchar(max),
	@Urguent bit = 0,
	@reporterInfo as Users.UserInfo READONLY,
	@locationID int,
	@MachineID int = null,
	@StatusID int = 1,
	@Technician int = null
)
as
Begin
	IF(
		(@Description is null or LEN(@Description) <= 0) or
		(@locationID is null or @locationID <= 0)
	)
	BEGIN
		RAISERROR('Tasks.CreateNormalTask : Invalid parameters!',15,1);
		RETURN 0;
	END
	--declare @machineID int = NULL
	declare @reporterName nvarchar(35) = NULL
	declare @reporterID int = NULL

	--IF(@MachineName is not null)
	--BEGIN
	--	select @machineID = ma.MachineID
	--	from Suppliers.Machine ma
	--	where ma.MachineName like @MachineName
	--END

	IF(@StatusID = 0)
	BEGIN
		Select @StatusID = ts.StatusID from Tasks.TaskStatus ts where ts.StatusDescription like 'Being Processed'
	END

	IF EXISTS(SELECT * FROM @reporterInfo)
	BEGIN
		select TOP 1 @reporterID=UserID from @reporterInfo
		IF(@reporterID is not null and @reporterID > 0)
		BEGIN
			select @reporterName=UserName,@reporterID=UserID from Users.Users where UserID like (Select TOP 1 UserID from @reporterInfo)
		END
		ELSE
		BEGIN
			Select TOP 1 @reporterName=userName,@reporterID=NULL from @reporterInfo
		END

	END

	IF(@reporterName is null)
	BEGIN
		RAISERROR('Failure to find username!',15,1)
		return 0
	END

	Insert into Tasks.Task (TaskDescription,Urguent,
							CreationDate,LocationID,MachineID,
							ReporterName,ReporterID,TechnicianID,StatusID,TypeID)
	values
	(
		@Description,
		@Urguent,
		GETDATE(),
		@locationID,
		@MachineID,
		@reporterName,
		@reporterID,
		@Technician,
		@StatusID,
		(Select tt.TypeID from Tasks.TaskType tt where tt.TypeDescription like 'Normal Task')
	)
	declare @newTask integer
	set @newTask = SCOPE_IDENTITY();

	IF(@newTask is null or @newTask <= 0)
	BEGIN
		RAISERROR('Tasks.CreateNormalTask : failed to create norma task!',15,1);
		RETURN 0;
	END

	return @newTask;
end
go

CREATE PROCEDURE Tasks.CheckAndCreateRepeatingTasks
as
BEGIN
	declare @currentDate int
	--convert the days since 1/1/1900
	set @currentDate = FLOOR(TRY_CONVERT(float, CAST (GetDate() as DATETIME),0))

	declare @taskID int
	declare @taskDate date
	set @taskDate = NULL;

	Select tk.TaskID,tk.TaskDescription,tk.Urguent,tk.ReporterID,tk.LocationID,ma.MachineID,rt.ActivationDate,rt.RepeatInterval,rt.InfoID
	Into   #Temp
	from Tasks.Task tk 
	inner join Tasks.TaskType tt on tk.TypeID = tt.TypeID
	inner join Tasks.TaskStatus ts on tk.StatusID = ts.StatusID
	inner join Users.Users us on us.UserID=tk.ReporterID
	left join Users.Users us2 on us2.UserID=tk.TechnicianID
	inner join General.Locations loc on loc.LocationID = tk.LocationID
	left join Tasks.RepeatingInfo rt on tk.TaskID = rt.ParentTaskID
	left join Suppliers.Machine ma on ma.MachineID = tk.MachineID
	where tt.TypeDescription like 'Repeating Task'

	While EXISTS(SELECT * FROM #Temp)
	Begin
		Select Top 1 @taskID = TaskID From #Temp
		--Select TOP 1 @taskDate = RepeatingInfoID from #Temp

		declare @ActivationDate int

		select TOP 1 @ActivationDate = ActivationDate from #Temp
		--select @ActivationDate

		IF(@currentDate = @ActivationDate)
		BEGIN
			--y0, today is the day! make new task!
			declare @Description nvarchar(max),
			@Urguent bit,
			@reporterInfo as Users.UserInfo,
			@locationID int,
			@MachineID int,
			@InfoID int

			insert into @reporterInfo(userID)
			values
			(
				(SELECT ReporterID from #Temp)
			)

			Select @Description=TaskDescription,@Urguent=Urguent,@locationID=LocationID,@MachineID = MachineID,@InfoID = InfoID from #Temp

			declare @newTaskID int
			exec @newTaskID = Tasks.CreateNormalTask @Description,@Urguent,@reporterInfo,@locationID,@MachineID

			--SELECT @newTaskID as IndexTask

			--task created, update the new activationDate!
			update Tasks.RepeatingInfo
			set ActivationDate += RepeatInterval,ChildTaskID=@newTaskID
			where @InfoID = InfoID

		END

		--select * from #Temp
		
		Delete #Temp Where TaskID = @taskID

	End
END
go


CREATE PROCEDURE Tasks.CreateRepeatingTask
(
	@Description nvarchar(max),
	@Urguent bit,
	@reporterInfo as Users.UserInfo READONLY,
	@locationID int,
	@MachineID int = null,
	@ActivationDate date,
	@DaysToRepeat int
)
as
Begin
	declare @activateionDateInt int = FLOOR(TRY_CONVERT(float, CAST (@ActivationDate as DATETIME),0))
	declare @reporterName nvarchar(35) = NULL
	declare @reporterID int = NULL
	declare @errors int = 0


	--select @machineID = ma.MachineName
	--from Suppliers.Machine ma
	--where ma.MachineName like @MachineName

	IF EXISTS(SELECT * FROM @reporterInfo)
	BEGIN
		select TOP 1 @reporterID=UserID from @reporterInfo
		IF(@reporterID is not null and @reporterID > 0)
		BEGIN
			select @reporterName=UserName,@reporterID=UserID from Users.Users where UserID like (Select TOP 1 UserID from @reporterInfo)
		END
		ELSE
		BEGIN
			Select TOP 1 @reporterName=userName,@reporterID=NULL from @reporterInfo
		END

	END

	IF(@reporterName is null)
	BEGIN
		RAISERROR('Failure to find username!',15,1)
		return 0
	END
	

	begin transaction ADD_TASK
		declare @taskID int;

		Insert into Tasks.Task (TaskDescription,Urguent,CreationDate,LocationID,MachineID,ReporterName,ReporterID,StatusID,TypeID)
		values
		(
			@Description,
			@Urguent,
			GetDate(),
			@locationID,
			@MachineID,
			@reporterName,
			@reporterID,
			(Select ts.StatusID from Tasks.TaskStatus ts where ts.StatusDescription like 'Active'),
			(Select tt.TypeID from Tasks.TaskType tt where tt.TypeDescription like 'Repeating Task')
		)

		set @errors = @@ERROR;
	
		IF @errors <= 0
		BEGIN
			set @taskID = SCOPE_IDENTITY();

			Insert Into Tasks.RepeatingInfo(ActivationDate,RepeatInterval,ParentTaskID)
			values
			(
				@activateionDateInt,
				@DaysToRepeat,@taskID
			);

			set @errors = @@ERROR;
		END

		if(@errors <= 0)
			COMMIT TRANSACTION ADD_TASK
		ELSE
		BEGIN
			RAISERROR('Errors creating repeating task ''%s'' !',15,1,@Description)
			ROLLBACK TRANSACTION ADD_TASK
		END

	exec Tasks.CheckAndCreateRepeatingTasks
end

go

CREATE PROCEDURE Tasks.ChangeTask
(
	@TaskID int,
	@Description nvarchar(max) = null,
	@Urguent bit = null,
	@locationID int = null,
	@StatusID int = null,
	@MachineID int = null,
	@TechnicianID int = 0,
	@ActivationDate date = null,
	@DaysToRepeat int = null,
	@reporterInfo as Users.UserInfo READONLY
)
as
BEGIN
	BEGIN TRANSACTION Set_Task

		IF(@Description is not null)
		BEGIN
			update Tasks.Task
			set
				TaskDescription = @Description
			where TaskID = @TaskID
		END

		IF(@Urguent is not null)
		BEGIN
			update Tasks.Task
			set
				Urguent = @Urguent
			where TaskID = @TaskID
		END

		IF(@locationID is not null)
		BEGIN
			update Tasks.Task
			set
				LocationID = @locationID
			where TaskID = @TaskID
		END

		IF(@StatusID is not null)
		BEGIN
			update Tasks.Task
			set
				StatusID = @StatusID
			where TaskID = @TaskID
		END

		IF((@TechnicianID is null) or (@TechnicianID > 0))
		BEGIN
			update Tasks.Task
			set
				TechnicianID = @TechnicianID
			where TaskID = @TaskID
		END

		IF EXISTS ( Select 1 from @reporterInfo)
		Begin
			declare @reporterID int = null
			declare @reporterName nvarchar(max) = null

			select TOP 1 @reporterID=UserID from @reporterInfo
			IF( ISNULL(@reporterID,0) > 0)
			BEGIN
				select @reporterName=UserName,@reporterID=UserID from Users.Users where UserID like (Select TOP 1 UserID from @reporterInfo)
			END
			ELSE
			BEGIN
				Select TOP 1 @reporterName=userName,@reporterID=NULL from @reporterInfo
			END

			if(@reporterName is not null)
			BEGIN
				update Tasks.Task
				set
					ReporterName = @reporterName,
					ReporterID = @reporterID
				where TaskID = @TaskID
			END
		End

		update Tasks.Task
		set
			MachineID = @MachineID,
			DateLastAdjustment = GETDATE()
		where TaskID = @TaskID

	COMMIT TRANSACTION Set_Task
END
go

CREATE PROCEDURE Tasks.GetSQLJobs
as
BEGIN
	SELECT
		ja.job_id,
		j.name AS job_name,
		ja.start_execution_date,      
		ISNULL(last_executed_step_id,0)+1 AS current_executed_step_id,
		Js.step_name
	FROM msdb.dbo.sysjobactivity ja 
	LEFT JOIN msdb.dbo.sysjobhistory jh 
		ON ja.job_history_id = jh.instance_id
	JOIN msdb.dbo.sysjobs j 
	ON ja.job_id = j.job_id
	JOIN msdb.dbo.sysjobsteps js
		ON ja.job_id = js.job_id
		AND ISNULL(ja.last_executed_step_id,0)+1 = js.step_id
	WHERE ja.session_id = (SELECT TOP 1 session_id FROM msdb.dbo.syssessions ORDER BY agent_start_date DESC)
	AND start_execution_date is not null
	AND stop_execution_date is null;
END
GO

CREATE PROCEDURE General.AddAddress
(
	@addressLine1 nvarchar(max), @addressLine2 nvarchar(max), @addressNr int, @addressBus nvarchar(15),
	@postcode nvarchar(15), @city nvarchar(35) , @region nvarchar(max) , @country nvarchar(35)
)
AS
BEGIN
	
	IF(
		(@addressLine1 is null) or
		(@addressNr is null) or
		(@postcode is null) or
		(@country is null)
	)
	BEGIN
		RAISERROR('null values detected in address!',15,1);
	END
	
	declare @AddressIndex int = 0

	insert into General.Addresses(AddressLine, AddressLine2 , AddressNr, AddressBus, Postcode,City,Region,Country)
	values
	(
		@addressLine1,
		@addressLine2,
		@addressNr,
		@addressBus,
		@postcode,
		@city,
		@region,
		@country
	)

	IF (@@ERROR < 0)
	BEGIN
		set @AddressIndex = 0;
	END
	ELSE
	BEGIN
		set @AddressIndex = SCOPE_IDENTITY();
	END

	RETURN @AddressIndex
END
go

create procedure Suppliers.AddSupplier
(
	@supplierName nvarchar(35), @phoneNr nvarchar(20), @faxNr nvarchar(35), @email nvarchar(35) , 
	@addressID int
)
AS
BEGIN
	IF(@supplierName is null)
	BEGIN
		RAISERROR('Supplier name is null!',15,1)
		return 0
	END

	declare @error int = 0
	declare @newID int = 0

	BEGIN TRANSACTION ADD_SUPPLIER

		INSERT INTO Suppliers.Supplier( SupplierName)--, PhoneNr , Fax, Email , AddressID)
		VALUES
		(
			@supplierName--,
			--@phoneNr,
			--@faxNr,
			--@email,
			--@addressID
		)

		set @error = @@error

		IF(@error <= 0)
		BEGIN
			set @newID=SCOPE_IDENTITY()

			insert into Suppliers.Contact(ContactName,ContactStatus,PhoneNr,Fax,Email,SupplierID,AddressID)
			values
			(
				@supplierName,
				'General',
				@phoneNr,
				@faxNr,
				@email,
				@newID,
				@addressID
			)

			set @error=@@ERROR
		END

		IF(@error <= 0)
		BEGIN
			COMMIT TRANSACTION ADD_SUPPLIER
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION ADD_SUPPLIER
			set @newID = 0
		END

		RETURN @newID
END
go

CREATE PROCEDURE Suppliers.AddContact
(
	 @contactName nvarchar(35), @contactStatus nvarchar(35),@phoneNr nvarchar(20),@fax nvarchar(35),@email nvarchar(35),
	 @supplierName nvarchar(35),@addressID int = null
)
AS
BEGIN
	IF
	(
		(@contactName is null) or
		(@contactStatus is null) or
		(@supplierName is null)
	)
	BEGIN
		Raiserror('Errors with the parameters : null found!',15,1)
	END

	declare @supplierID int
	exec @supplierID = Suppliers.GetSupplierID @supplierName

	--IF(@addressID = 0)
	--BEGIN
	--	select @addressID = sup.AddressID
	--	from Suppliers.Supplier sup
	--	where sup.SupplierID = @supplierID
	--END
	
	insert into Suppliers.Contact( SupplierID, ContactName , ContactStatus, Email , Fax, PhoneNr, AddressID)
	values
	(
		@supplierID,
		@contactName,
		@contactStatus,
		@email,
		@fax,
		@phoneNr,
		@addressID
	)

	IF(@@ERROR > 0)
		RETURN 0
	ELSE
		RETURN SCOPE_IDENTITY()

END
go

CREATE PROCEDURE Suppliers.AssignMachineToDepartment
(
	@machineName nvarchar(40),
	@departmentName nvarchar(35),
	@companyName nvarchar(35)
)
AS
BEGIN
	declare @departmentID integer, @companyID integer, @machineID integer = 0
	exec @departmentID = General.GetDepartmentID @departmentName, @companyName
	SELECT @machineID=MachineID from Suppliers.Machine where MachineName like @machineName

	insert into Suppliers.MachineDepartment( MachineID , DepartmentID )
	values
	(
		@machineID,
		@departmentID
	)
END
go

CREATE PROCEDURE General.GetPhoto
(
	@photoID int = null
)
AS
BEGIN
	IF(@photoID is not null and @photoID = 0)
	BEGIN
		set @photoID = NULL
	END

	select * from General.Photo
	where (@photoID is null) or (PhotoID = @photoID)
	order by PhotoID asc
END
go

CREATE PROCEDURE General.GetPhotoID
(
	@photoName nvarchar(max)
)
AS
BEGIN
	declare @ret integer = 0;

	select ph.PhotoID,ph.PhotoName from General.Photo ph
	where ph.PhotoName = @photoName;

	select @ret = ph.PhotoID from General.Photo ph
	where ph.PhotoName = @photoName;

	RETURN @ret;
END
GO

CREATE PROCEDURE General.AddPhoto
(
	@PhotoName nvarchar(256)
)
AS
BEGIN
	IF(	@PhotoName is null or
		LEN(ltrim(rtrim(@PhotoName))) = 0)
	BEGIN
		RAISERROR('Invalid Photo Name!',15,1);
	END
		
	insert into General.Photo(PhotoName)
	values
	(
		@PhotoName
	);

	RETURN SCOPE_IDENTITY();

END
GO

CREATE PROCEDURE Tasks.AssignPhoto
(
	@taskID int,
	@photoName nvarchar(265)
)
AS
BEGIN
	IF(
		(@taskID is null or @taskID = 0) or
		(@photoName is null or @photoName like '')
	)
	BEGIN
		RAISERROR('parameters should not be null or 0!',15,1)
		RETURN 0;
	END
	Insert into Tasks.TaskPhotos(PhotoID,TaskID)
	values
	(
		(select PhotoID from General.Photo where PhotoName like @photoName),
		@taskID
	)

	IF(@@ERROR > 0)
		RETURN 0
	ELSE
		RETURN SCOPE_IDENTITY()

END
GO

CREATE PROCEDURE Suppliers.AssignPhotoToMachine
(
	@MachineID int,
	@photoName nvarchar(265)
)
AS
BEGIN
	IF(
		(@MachineID is null or @MachineID = 0) or
		(@photoName is null or @photoName like '')
	)
	BEGIN
		RAISERROR('parameters should not be null or 0!',15,1)
		RETURN 0;
	END

	Insert into Suppliers.MachinePhotos(PhotoID,MachineID)
	values
	(
		(select PhotoID from General.Photo where PhotoName like @photoName),
		@MachineID
	)

	IF(@@ERROR > 0)
		RETURN 0
	ELSE
		RETURN SCOPE_IDENTITY()

END


go