CREATE TABLE [dbo].[DrillPattern]
(
	[PatternId] INT NOT NULL PRIMARY KEY,
	[ShotNo] INT NOT NULL,
	[HoleDiameter] DECIMAL NOT NULL,
	[Burden] DECIMAL NOT NULL,
	[Spacing] DECIMAL NOT NULL,
	[FaceHeight] DECIMAL NOT NULL,
	[SubDrill] DECIMAL NOT NULL,
	[ShotType] NCHAR(50) NOT NULL,
	[NoHoles] INT NOT NULL,
	[DesignDate] DATETIME2 NULL,
	[ShotDate] DATETIME2 NULL,
	[BlasterSSN] INT NOT NULL,
	[ShotID] INT NOT NULL,
)
