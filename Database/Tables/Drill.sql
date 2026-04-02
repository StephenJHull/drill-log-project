CREATE TABLE [dbo].[Drill]
(
	[MachineID] INT NOT NULL PRIMARY KEY, 
    [ModelName] NCHAR(100) NOT NULL, 
    [Age] INT NOT NULL, 
    [TotalHours] INT NOT NULL, 
    [Rebuilts] INT NULL DEFAULT 0
)
