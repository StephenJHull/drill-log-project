CREATE TABLE [dbo].[Drilling]
(
	[DrillingInstanceNo] INT NOT NULL PRIMARY KEY,
    [SSN] INT NOT NULL , 
    [MachineID] INT NOT NULL, 
    [Date] DATETIME2 NOT NULL, 
    [Duration] DECIMAL NOT NULL,
)
