CREATE TABLE [dbo].[DrillHole]
(
	[HoleId] INT NOT NULL PRIMARY KEY, 
    [HoleNo] INT NOT NULL, 
    [Easting] DECIMAL NOT NULL, 
    [Northing] DECIMAL NOT NULL, 
    [Elevation] DECIMAL NOT NULL, 
    [Degrees] DECIMAL NOT NULL DEFAULT 0, 
    [BrokenMaterial] DECIMAL NOT NULL, 
    [CompetentRock] DECIMAL NULL, 
    [TotalDepth] DECIMAL NOT NULL, 
    [WaterDepth] INT NULL, 
    [Notes] NCHAR(500) NULL, 
    [StartTime] DATETIME2 NOT NULL, 
    [EndTime] DATETIME2 NOT NULL, 
    [MachineID] INT NOT NULL, 
    [ShotID] INT NOT NULL
)
