CREATE TABLE [dbo].[DrillLog]
(
	[ShotID] INT NOT NULL PRIMARY KEY,
	[TimeBegan] DATETIME2 NOT NULL,
	[TimeLastSaved] DATETIME2 NULL,
	[TimeSubmitted] DATETIME2 NULL, 
    [DrillerSSN] INT NOT NULL, 
    [QuarryName] NCHAR(100) NOT NULL,
)
