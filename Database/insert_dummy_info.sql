use TechnicalServiceSystem
go

--first clean data so we can actually reset this shit :P
--script found on Stackoverflow. from what i understand it disables all relations, deletes it all and re-enables the relations
--and then reseeds the database
begin tran Clear_Data
SET QUOTED_IDENTIFIER ON;
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? NOCHECK CONSTRAINT ALL'  
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? DISABLE TRIGGER ALL'  
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?' 
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? CHECK CONSTRAINT ALL'  
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; ALTER TABLE ? ENABLE TRIGGER ALL' 
EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON;

IF NOT EXISTS (
    SELECT
        *
    FROM
        SYS.IDENTITY_COLUMNS
        JOIN SYS.TABLES ON SYS.IDENTITY_COLUMNS.Object_ID = SYS.TABLES.Object_ID
    WHERE
        SYS.TABLES.Object_ID = OBJECT_ID(''?'') AND SYS.IDENTITY_COLUMNS.Last_Value IS NULL
)
AND OBJECTPROPERTY( OBJECT_ID(''?''), ''TableHasIdentity'' ) = 1

    DBCC CHECKIDENT (''?'', RESEED, 0) WITH NO_INFOMSGS'

insert into Users.Roles (RoleName)
values
( 'Admin' ), ( 'User' ),
( 'Technician' ),( 'User Manager' ),
('Task Manager' ),( 'Suppliers Manager' );

exec General.AddPhoto './system/DefaultUser.jpg'
exec General.AddPhoto './images/User4.jpg'

insert into Tasks.TaskType(TypeDescription)
values
( 'Normal Task' ), ( 'Repeating Task' );

insert into Tasks.TaskStatus(StatusDescription)
values
( 'Being Processed' ),( 'Assigned' ),
( 'Part requires ordering' ), ( 'Part ordered' ),
( 'External Intervention required' ), ( 'External Intervention Requested' ),
( 'Task Finished' ), ('Active'), ('Disabled');

insert into Suppliers.MachineType(TypeName)
values
('Hoog Laag Bed'),('Nachtkastje'),('Active Tillift'),
('Passieve Tillift'),('WC'),('Hoog Laag bad'),
('Zit bad'),('Computer'),('Lift'),('Kamerpost'),('Andere');

commit tran Clear_Data








begin tran Insert_dummy_info

--dummy information to put into the database to play with. its +/- real data

--first the company
exec General.AddCompany 'Sint-Elisabeth'
exec General.AddCompany 'Ons Zomerheem'

--departments
exec General.AddDepartment  'Gelijkvloers' , 'Sint-Elisabeth'
exec General.AddDepartment  'De Koraal' , 'Sint-Elisabeth', 'Gelijkvloers'
exec General.AddDepartment  'De Haven' , 'Sint-Elisabeth'
exec General.AddDepartment  'De Branding' , 'Sint-Elisabeth'
exec General.AddDepartment  'De Parel' , 'Sint-Elisabeth'
exec General.AddDepartment  'De Deining' , 'Sint-Elisabeth'
exec General.AddDepartment  'Serviceflats' , 'Sint-Elisabeth', 'De Deining'
exec General.AddDepartment  'Kelder Deining' , 'Sint-Elisabeth', 'De Deining'
exec General.AddDepartment  'Ander' , 'Sint-Elisabeth'

exec General.AddDepartment 'Gelijkvloers','Ons Zomerheem'


DECLARE @i int = 0
declare @department nvarchar(35)
declare @in int = 1
declare @location nvarchar(50)
WHILE @i <= 7
BEGIN
	IF(@i = 0)
		set @department = 'De Haven'
	else if (@i = 1)
		set @department = 'De Branding'
	else if (@i = 2)
		set @department = 'De Parel'
	else if (@i = 3)
		set @department = 'De Deining'
	else if (@i = 4)
		set @department = 'Kelder Deining'
	else if (@i = 5)
		set @department = 'Serviceflats'
	else if (@i = 6)
		set @department = 'Gelijkvloers'
	else if (@i = 7)
		set @department = 'De Koraal'
	else if (@i = 8)
		set @department = 'Andere'

	IF(@i < 3)
	BEGIN
		--these locations are there on 3 departments
		exec General.AddLocation 'Inkom Afdeling', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau verpleegkundige', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Verdeelkeuken', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Verpleeglokaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Verbandkamer', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Utility', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer - Ligbad', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer - Zitbad', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Hoofdverpleegkundige', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Drank Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Onderhouds Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Incontinentie Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Centrale trap', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'WC lift', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'A Gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'B Gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'C Gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'D Gang', @department , 'Sint-Elisabeth'
	END

	IF(@i = 0)
	BEGIN
		-- These are unique to the first floor
		exec General.AddLocation 'Badkamer - Ligbad 2', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zitplaats A gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'De Schelp', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging B gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Rookzaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Lokaal verpleeger', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Lokaal dokter', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zeebries', @department , 'Sint-Elisabeth'

		set @in = 1
		WHILE @in <= 52
		BEGIN
			IF(@in = 5)
				set @in = 7
			ELSE IF ( @in = 27 )
				set @in = 28
			ELSE IF ( @in = 36)
				set @in = 38
			ELSE
				BEGIN
					set @location = CONCAT('Kamer 1',RIGHT('00' + TRY_CAST(@in as nvarchar(2)),2) )
					exec General.AddLocation @location, @department , 'Sint-Elisabeth'
					SET @in = @in + 1
				END
		END
	END
	ELSE IF(@i = 1)
	BEGIN
		--unique to the second floor
		exec General.AddLocation 'Badkamer - Ligbad 2', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'gèirnaart ', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zitplaats B gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging B gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kerrekolle', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging C Gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Fixatie Materiaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Animatie Materiaal', @department , 'Sint-Elisabeth'

		set @in = 1
		WHILE @in <= 49
		BEGIN
			IF(@in = 5)
				set @in = 7
			ELSE IF ( @in = 27 )
				set @in = 28
			ELSE IF ( @in = 36)
				set @in = 37
			ELSE
				BEGIN
					set @location = CONCAT('Kamer 2',RIGHT('00' + TRY_CAST(@in as nvarchar(2)),2) )
					exec General.AddLocation @location, @department , 'Sint-Elisabeth'
					SET @in = @in + 1
				END
		END
	END
	ELSE IF(@i = 2)
	BEGIN
		--unique shit the the 3th floor
		exec General.AddLocation 'Berging Rolstoelen', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zitplaats A gang', @department , 'Sint-Elisabeth'	
		exec General.AddLocation 'Zeester', @department , 'Sint-Elisabeth'	

		set @in = 1
		WHILE @in <= 36
		BEGIN
			IF(@in = 11)
				set @in = 12
			ELSE IF ( @in = 21 )
				set @in = 22
			ELSE
				BEGIN
					set @location = CONCAT('Kamer 3',RIGHT('00' + TRY_CAST(@in as nvarchar(2)),2) )
					exec General.AddLocation @location, @department , 'Sint-Elisabeth'
					SET @in = @in + 1
				END
		END
	END
	ELSE IF(@i = 3)
	BEGIN
		exec General.AddLocation 'Ingang Deining', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Fitniss ruimte', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau verpleegkundige', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Tussenplek', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zandkorrel', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Snoezelruimte', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer 2', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Utility', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging onderhoud', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Strandjutter', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Hoofdverpleegkundige', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer 1', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Incontinentie Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Drank Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Zeekraal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Tussen ruimte leefgroepen', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Apotheek', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer 3 - Snoezelbad', @department , 'Sint-Elisabeth'

		set @in = 1
		WHILE @in <= 38
		BEGIN
				set @location = CONCAT('Kamer 0',RIGHT('00' + TRY_CAST(@in as nvarchar(2)),2) )
				exec General.AddLocation @location, @department , 'Sint-Elisabeth'
				SET @in = @in + 1
		END
	END
	ELSE IF(@i = 4)
	BEGIN
		exec General.AddLocation 'Ruimte Lift', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Trap Kelder', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Gang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kleedkamers Dames', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kleedkamers Heren', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kleedkamers Keuken', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging Deining', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging Technische Dienst', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Data lokaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Verluchtings Lokaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Tussenruimte Waterlokalen', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Verwarmings ruimte', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Koud water ruimte', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Eletrische kast', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Nooduitgang', @department , 'Sint-Elisabeth'
	END
	ELSE IF(@i = 6)
	BEGIN
		--Gelijkvloers
		exec General.AddLocation 'Kapel', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Sacristie', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Joris', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Ronny', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Veerle', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'WC Personeel', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Personeelsdienst', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Administratie', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Inkom', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Copy Lokaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'WC Bezoekers', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Personeels Ingang', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kleedkamers stagairs', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Algemene Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Linda', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Keuken', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Centrale Trap', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Opnamedienst', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Grote Vergaderzaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Ergo', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging Onderhoud', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kinesitherapie', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Pastorale Dienst', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Bureau Technische Dienst', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kapsalon', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Drank Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Cafetaria', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Gang Deining', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kelder : Trap', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kelder : Liftkamer', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kelder : Berging', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kelder : Lift', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Kelder : Inkom Stookplaats', @department , 'Sint-Elisabeth'

	END
	ELSE IF(@i = 7)
	BEGIN
		--De Koraal
		exec General.AddLocation 'Keuken', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Living', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'WC''s', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Slaapzaal', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Badkamer', @department , 'Sint-Elisabeth'
		exec General.AddLocation 'Berging', @department , 'Sint-Elisabeth'
	END


		exec General.AddLocation 'Andere', @department , 'Sint-Elisabeth'
	
	SET @i = @i + 1
END

go

--exec General.GetLocations 'De Haven' , 'Sint-Elisabeth'




declare @AddressIndex int;
exec @AddressIndex = General.AddAddress 'Bedrijvenlaan' , null , 1 , null , 2800 , 'Mechelen','Antwerpen','Belgium'
declare @SupplierID int;
exec @SupplierID = Suppliers.AddSupplier 'Wissner-Bosserhoff' , '+32 (0) 15210841' , '+32 (0) 15291464' , 'info@wi-bo.be' , @AddressIndex
exec Suppliers.AddContact 'After Sales', 'Bestelling en hulp', null,null,'sales@wi-bo.be', 'Wissner-Bosserhoff'


exec @AddressIndex = General.AddAddress 'Leo Bekaertlaan',null,1, null, 8870, 'Izegem', 'West-Vlaanderen','Belgium'
exec @SupplierID = Suppliers.AddSupplier 'Televic' , '+32 51 30 30 45' , '+32 51 31 06 70', 'healthcare@televic.com', @AddressIndex
exec Suppliers.AddContact 'Televic After Sales','Bestelling en hulp','+32 51 33 18 87 ',null,'support@televic.com', 'Televic'



exec @AddressIndex = General.AddAddress 'Bretagnestraat' , null , 24 , null , 1200 , 'Brussel',null,'Belgium'
exec @SupplierID = Suppliers.AddSupplier 'Kone' , '+32 2 730 92 11' , '+32 2 340 03 29' , 'customer.service.be@kone.com' , @AddressIndex
exec Suppliers.AddContact 'Kone Loppem', 'Depanage en magezijn', '050 36 74 10','050 36 74 35','customer.service.be@kone.com', 'Kone'


--exec Suppliers.GetContacts
declare @supID as int
Select @supID=SupplierID from Suppliers.Supplier where Supplier.SupplierName like 'Wissner-Bosserhoff'
declare @typeID as int
Select @typeID=TypeID from Suppliers.MachineType where TypeName like 'Hoog Laag Bed'
exec Suppliers.AddMachine 'Houten Bed',	NULL, 'WB-010101', 'Estetica', @supID, @typeID

Select @typeID=TypeID from Suppliers.MachineType where TypeName like 'Nachtkastje'
exec Suppliers.AddMachine 'Houten Nachtkastje',null,null,'WB Nachtkastje', @supID , @typeID

Select @supID=SupplierID from Suppliers.Supplier where Supplier.SupplierName like 'Kone'
Select @typeID=TypeID from Suppliers.MachineType where TypeName like 'Lift'
exec Suppliers.AddMachine 'Lift Shindler Kelder (Links)', null,null, 'Onbekend',@supID,@typeID

exec Suppliers.AddMachine 'Lift Shindler (Rechts)', null,null, 'Onbekend',@supID,@typeID

exec General.AddPhoto './images/Estetica1.jpg'
exec Suppliers.AssignPhotoToMachine 1,'./images/Estetica1.jpg'

exec Suppliers.AssignMachineToDepartment 'Lift Shindler Kelder (Links)','Gelijkvloers','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Lift Shindler (Rechts)','Gelijkvloers','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Bed','De Haven','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Nachtkastje','De Haven','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Bed','De Branding','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Nachtkastje','De Branding','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Bed','De Parel','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Nachtkastje','De Parel','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Bed','De Deining','Sint-Elisabeth'
exec Suppliers.AssignMachineToDepartment 'Houten Nachtkastje','De Deining','Sint-Elisabeth'
--exec Suppliers.GetMachines

--exec Suppliers.GetMachinesByDepartment 'Sint-Elisabeth'






declare @photoID int
select @photoID=PhotoID from General.Photo where PhotoName like './system/DefaultUser.jpg'

exec Users.CreateUser 'Rombaut Joris', 'test' ,@photoID , 'Sint-Elisabeth' , 'Gelijkvloers'

DECLARE @UserRoles AS Users.UserRolesTable;    
INSERT INTO @UserRoles(roleID,roleName)
values
	('User', (select RoleID from Users.Roles where RoleName = 'User')),
	('Task Manager', (select RoleID from Users.Roles where RoleName = 'Task Manager')),
	('Suppliers Manager', (select RoleID from Users.Roles where RoleName = 'Suppliers Manager')),
	('Technician', (select RoleID from Users.Roles where RoleName = 'Technician'))
exec Users.CreateUser 'Dossche Paul', 'test',@photoID , 'Sint-Elisabeth' , 'Gelijkvloers', @RoleNames = @UserRoles

exec Users.CreateUser 'Alain','test',@photoID,'Ons Zomerheem','Gelijkvloers',@UserRoles

declare @tempID int
select @tempID=PhotoID from General.Photo where PhotoName like './images/User4.jpg'
delete from @UserRoles
INSERT INTO @UserRoles(roleName)
values
	('User'),('Task Manager'),('Suppliers Manager'),('Technician'),('Admin')
exec Users.CreateUser 'Vermeylen Joris','test',@tempID , 'Sint-Elisabeth' , 'Gelijkvloers', @RoleNames = @UserRoles

delete from @UserRoles
INSERT INTO @UserRoles(roleName)
values
	('User'),('Technician')
exec Users.CreateUser 'Pauwels Krist','test',@photoID , 'Sint-Elisabeth' , 'Gelijkvloers', @RoleNames = @UserRoles


delete from @UserRoles
INSERT INTO @UserRoles(roleName)
values
	('User'),('User Manager')
exec Users.CreateUser 'Pauwels Helene','test',@photoID , 'Sint-Elisabeth' , 'Gelijkvloers', @RoleNames = @UserRoles

exec Users.CreateUser 'Legrand Tamara','test',@photoID , 'Sint-Elisabeth' , 'De Haven'
exec Users.CreateUser 'de Smet Evy', 'test',@photoID , 'Sint-Elisabeth' , 'De Koraal'

--LOGIN! :D
declare @userHash nvarchar(128)
exec Users.CheckLogin 'Pauwels Helene', 'test', 'Sint-Elisabeth', @userHash OUTPUT
--select @userHash

declare @reporterInfo as Users.UserInfo;
insert into @reporterInfo (UserId,UserName) values(0, 'Legrand Tamara')
declare @TechnicianID int = 0
declare @locationID int = 0
declare @taskID int = 0
exec @locationID = General.GetLocation 'Kamer 135','De Haven','Sint-Elisabeth'
exec Tasks.CreateNormalTask 'WC Lekt',0,@reporterInfo,@locationID,null

update @reporterInfo set UserID = (select UserID from Users.Users where UserName like 'Legrand Tamara')
exec @locationID = General.GetLocation 'Kamer 114','De Haven','Sint-Elisabeth'
exec Tasks.CreateNormalTask 'Kamer moet geverft worden',1,@reporterInfo,@locationID,null

exec @locationID = General.GetLocation 'Kamer 140','De Haven','Sint-Elisabeth'
declare @MachineID int
select @MachineID = MachineID from Suppliers.Machine where MachineName = 'Houten Bed'
exec @taskID = Tasks.CreateNormalTask 'Bed gaat niet naar boven',1,@reporterInfo,@locationID, @MachineID
exec Tasks.AssignPhoto @taskID, './images/Estetica1.jpg'
exec Tasks.AssignPhoto @taskID, './images/Estetica1.jpg'
exec Tasks.AssignPhoto @taskID, './images/Estetica1.jpg'

set @TechnicianID = (select UserID from Users.Users where UserName like 'Pauwels Krist')
exec Tasks.AssignTechnician 2,@TechnicianID

update @reporterInfo set UserID = 0,userName = 'Leydens Bianca'
exec @locationID = General.GetLocation 'Kamer 230','De Branding','Sint-Elisabeth'
set @taskID = 0
exec @taskID = Tasks.CreateNormalTask 'kaders moeten omhoog gehangen worden',0,@reporterInfo,@locationID,null
exec Tasks.AddNote 'er ligt eeen schema met waar de kaders moeten in het nachtkastje', @taskID
exec Tasks.AddNote 'klein kadertje moet ook gemaakt worden', @taskID



update @reporterInfo set UserID = (select UserID from Users.Users where UserName like 'de Smet Evy')
exec @locationID = General.GetLocation 'Badkamer','De Koraal','Sint-Elisabeth'
exec Tasks.CreateNormalTask 'licht pinkt',0,@reporterInfo,@locationID,null

update @reporterInfo set UserID = (select UserID from Users.Users where UserName like 'Pauwels Helene')
exec @locationID = General.GetLocation 'Copy Lokaal','Gelijkvloers','Sint-Elisabeth'
exec Tasks.CreateNormalTask 'deur geblokeerd',1,@reporterInfo,@locationID,null

set @TechnicianID = (select UserID from Users.Users where UserName like 'Vermeylen Joris')
exec Tasks.AssignTechnician 5,@TechnicianID
exec Tasks.AssignTechnician 3,@TechnicianID

update @reporterInfo set UserID = 0,userName = 'Maegerman Sara'
exec @locationID = General.GetLocation 'Strandjutter','De Deining','Sint-Elisabeth'
set @taskID = 0
exec @taskID = Tasks.CreateNormalTask 'Vaatwas machine werkt niet',0,@reporterInfo,@locationID,null
exec Tasks.AssignTechnician @taskID,@TechnicianID

--repeating task!
update @reporterInfo set UserID = (select UserID from Users.Users where UserName like 'Dossche Paul');
update @reporterInfo set userName = (select UserName from Users.Users where UserName like 'Dossche Paul');
exec @locationID = General.GetLocation 'Andere','Gelijkvloers','Sint-Elisabeth'
declare @activationDate date
set @activationDate = GetDate()
exec Tasks.CreateRepeatingTask 'Controle Noodverlichting', 0,@reporterInfo,@locationID,null,@activationDate,365

set @activationDate = DATEADD(DAY,1,@activationDate)
exec Tasks.CreateRepeatingTask 'Controle Legionela', 0,@reporterInfo,@locationID,null,@activationDate,7

exec Tasks.GetTasks
exec Tasks.GetNotes

--exec Tasks.GetTasks
--exec Tasks.CheckAndCreateRepeatingTasks
--exec Tasks.GetTasks

commit tran Insert_dummy_info