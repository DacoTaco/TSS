use master
drop database TechnicalServiceSystem
go
--create database and its schema's
create database TechnicalServiceSystem
go

use TechnicalServiceSystem
go

--give users of [public] access to the database
GRANT ALTER TO [public]
GRANT CONNECT TO [public]
GRANT CONNECT REPLICATION TO [public]
GRANT EXECUTE TO [public]
GRANT INSERT TO [public]
GRANT REFERENCES TO [public]
GRANT SELECT TO [public]
GRANT UPDATE TO [public]
--GRANT VIEW ANY COLUMN ENCRYPTION KEY DEFINITION TO [public]
GRANT VIEW DATABASE STATE TO [public]
go

--create all schema's
create SCHEMA Users AUTHORIZATION [public]
go
create SCHEMA General AUTHORIZATION [public]
go
create SCHEMA Suppliers AUTHORIZATION [public] 
go 
create SCHEMA Tasks AUTHORIZATION [public]
go

--begin transaction of all database stuff
Begin Tran Create_Database

--user & general stuff
create table Users.Roles
(
	RoleID integer not null Identity(1,1) Primary key,
	RoleName nvarchar(35) NOT NULL
);

Create type Users.UserRolesTable as table (roleID integer,roleName nvarchar(50))
Create type Users.UserInfo as table (userID integer,userName nvarchar(35))

insert into Users.Roles (RoleName)
values
( 'Admin' ), ( 'User' ),
( 'Technician' ),( 'User Manager' ),
('Task Manager' ),( 'Suppliers Manager' );

go

Create Table General.Photo
(
	PhotoID int not null identity(1,1) primary key,
	PhotoName nvarchar(265) unique
);

insert into General.Photo(PhotoName)
values(
	'./system/DefaultUser.jpg'
);

create table General.Addresses
(
	AddressID int not null identity(1,1) primary key,
	AddressLine nvarchar(max) not null,
	AddressLine2 nvarchar(max),
	AddressNr integer not null,
	AddressBus nvarchar(15),
	Postcode nvarchar(15) not null,
	City nvarchar(35) not null,
	Region nvarchar(max),
	Country nvarchar(35) not null,
);

create table General.Company
(
	CompanyID int not null identity(1,1) primary key,
	CompanyName nvarchar(256) not null unique
);

create table General.Department
(
	DepartmentID int not null identity(1,1) primary key,
	DepartmentName nvarchar(40) not null,
	ParentDepartmentID int foreign key references General.Department(DepartmentID)
);

--TODO : ask tom about classes like these, what do we do with these when writing the classes
create table General.CompanyDepartment
(
	ID int not null identity(1,1) primary key,
	CompanyID int not null foreign key references General.Company(CompanyID) on delete cascade,
	DepartmentID int not null foreign key references General.Department(DepartmentID) on delete cascade,

);

create table General.Locations
(
	LocationID int not null identity(1,1) primary key,
	LocationName nvarchar(40) not null,
	DepartmentID int not null foreign key references General.Department(DepartmentID) on delete cascade
)
go

Create Table Users.Users
(
	UserID int not null identity(1,1) primary key,
	UserName nvarchar(35) not null,
	PasswordHash varbinary(128) null,
	PasswordSalt nvarchar(max) not null default CONVERT(nvarchar(max), NEWID()),
	DepartmentID int not null default 0 foreign key references General.Department(DepartmentID) on delete set default,
	PhotoID int foreign key references General.Photo(PhotoID),
	Active bit not null default 1,
	UserOpened nvarchar(max) default null,
	OpenTimeDue DateTime default null,
	DateToDelete DateTime default null,
	Deleted bit default null
)

create Table Users.UserRoles
(
	ID int not null identity(1,1) primary key,
	UserID int foreign key references Users.Users(UserID) on delete cascade not null,
	RoleID Integer foreign key references Users.Roles(RoleID) on delete cascade not null
);
go

ALTER TABLE Users.UserRoles
  ADD CONSTRAINT UQ_UserRoles UNIQUE(UserID, RoleID);
GO

--Suppliers & machine stuff

create table Suppliers.MachineType
(
	TypeID integer identity(1,1) primary key,
	TypeName nvarchar(35) not null unique
);

create table Suppliers.Supplier 
(
	SupplierID int not null identity(1,1) primary key,
	SupplierName nvarchar(35) not null unique,
	UserOpened nvarchar(max) default null,
	OpenTimeDue DateTime default null
);

create table Suppliers.Contact
(
	ContactID int not null identity(1,1) primary key,
	ContactName nvarchar(35) not null,
	ContactStatus nvarchar(35) not null,
	PhoneNr nvarchar(20),
	Email nvarchar(35),
	Fax nvarchar(35),
	AddressID int foreign key references General.Addresses(AddressID),
	SupplierID int foreign key references Suppliers.Supplier(SupplierID)
);

create table Suppliers.Machine
(
	MachineID integer not null identity(1,1) primary key,
	MachineName nvarchar(40) not null unique,
	SerialNumber nvarchar(35),
	ModelNumber nvarchar(35),
	ModelName nvarchar(35) not null,
	UserOpened nvarchar(max) default null,
	OpenTimeDue DateTime default null,
	SupplierID int not null default 0 foreign key references Suppliers.Supplier(SupplierID) on delete set default,
	TypeID integer not null foreign key references Suppliers.MachineType(TypeID),
);

create table Suppliers.MachinePhotos
(
	ID int not null identity(1,1) primary key,
	PhotoID integer not null foreign key references General.Photo(PhotoID),
	MachineID int not null foreign key references Suppliers.Machine(MachineID)
);

create table Suppliers.Documentation
(
	DocumentationID int not null identity(1,1) primary key,
	Documentation nvarchar(max),
	MachineID int not null foreign key references Suppliers.Machine(MachineID)
);

--create table Suppliers.ModelDocumentation
--(
--	ID int not null identity(1,1) primary key,
--	MachineID int not null foreign key references Suppliers.Machine(MachineID),
--	DocumentationID int not null foreign key references Suppliers.Documentation(DocumentationID)
--);

create table Suppliers.MachineDepartment
(
	ID int not null identity(1,1) primary key,
	MachineID integer not null foreign key references Suppliers.Machine(MachineID) on delete cascade,
	DepartmentID int not null foreign key references General.Department(DepartmentID) on delete cascade
);

go

--and finally, TASKS!
create table Tasks.TaskType
(
	TypeID int not null identity(1,1) primary key,
	TypeDescription nvarchar(35) not null
);

insert into Tasks.TaskType(TypeDescription)
values
( 'Normal Task' ), ( 'Repeating Task' );

create table Tasks.TaskStatus
(
	StatusID int not null identity(1,1) primary key,
	StatusDescription nvarchar(35) not null
);

insert into Tasks.TaskStatus(StatusDescription)
values
( 'Being Processed' ),( 'Assigned' ),
( 'Part required ordering' ), ( 'Part ordered' ),
( 'External Intervention required' ), ( 'External Intervention Requested' ),
( 'Task Finished' ),('Active');

create table Tasks.Task
(
	TaskID int not null identity(1,1) primary key,
	TaskDescription nvarchar(max) not null,
	Urguent bit not null default 0,
	CreationDate DateTime not null default GetDate(),
	DateLastAdjustment DateTime not null default GetDate(),
	ReporterName nvarchar(35) not null,
	UserOpened nvarchar(max) default null,
	OpenTimeDue DateTime default null,
	TypeID int not null foreign key references Tasks.TaskType(TypeID),
	StatusID int not null foreign key references Tasks.TaskStatus(StatusID),
	ReporterID int foreign key references Users.Users(UserID),
	TechnicianID int foreign key references Users.Users(UserID),
	LocationID int not null default 0 foreign key references General.Locations(LocationID) on delete set DEFAULT,
	MachineID int foreign key references Suppliers.Machine(MachineID) on delete set null
);

create table Tasks.TaskPhotos
(
	ID int not null identity(1,1) primary key,
	PhotoID int not null foreign key references General.Photo(PhotoID),
	TaskID int not null foreign key references Tasks.Task(TaskID) on delete cascade
)

create table Tasks.RepeatingInfo
(
	InfoID int not null identity(1,1) primary key,
	ActivationDate int not null,
	RepeatInterval int not null,
	ParentTaskID int default null foreign key references Tasks.Task(TaskID) on delete cascade,
	ChildTaskID int default null foreign key references Tasks.Task(TaskID)
);

create table Tasks.Notes
(
	NoteID int not null identity(1,1) primary key,
	Note nvarchar(max) not null,
	NoteDate DateTime default GetDate(),
	TaskID int not null foreign key references Tasks.Task(TaskID) on delete cascade
);

go

--truncate table General.Addresses;
--truncate table General.Company;
--truncate table General.Department;
--truncate table General.Location;
--truncate table General.Photo;
--truncate table Suppliers.Contact;
--truncate table Suppliers.Documentation;
--truncate table Suppliers.Machine;
--truncate table Suppliers.MachineDepartment;
--truncate table Suppliers.MachineType;
--truncate table Suppliers.Model;
--truncate table Suppliers.ModelDocumentation;
--truncate table Suppliers.Supplier;
--truncate table Tasks.Notes;
--truncate table Tasks.Task;
--truncate table Tasks.TaskStatus;
--truncate table Tasks.TaskType;
--truncate table Users.Roles;
--truncate table Users.Users
--truncate table UsersRole;
go
commit TRAN Create_Database;