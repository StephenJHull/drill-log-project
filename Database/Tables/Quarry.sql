CREATE TABLE [dbo].[Quarry]
(
	[Name] NCHAR(100) NOT NULL PRIMARY KEY, 
    [Location] NCHAR(250) NOT NULL, 
    [Company] NCHAR(100) NOT NULL, 
    [RockType] NCHAR(200) NULL, 
    [AnnualProduction] INT NULL
)
