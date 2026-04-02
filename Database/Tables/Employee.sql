CREATE TABLE [dbo].[Employee]
(
	[SSN] INT NOT NULL PRIMARY KEY, 
    [JobTitle] NCHAR(100) NULL, 
    [SupervisorSSN] INT NULL,
    [FirstName] NCHAR(50) NOT NULL, 
    [LastName] NCHAR(100) NOT NULL, 
    [PhoneNo] NCHAR(15) NOT NULL, 
    [Email] NCHAR(100) NULL, 
    [LicenseNo] INT NULL, 
)
