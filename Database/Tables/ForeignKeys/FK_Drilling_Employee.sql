ALTER TABLE [dbo].[Drilling]
	ADD CONSTRAINT [FK_Drilling_Employee]
	FOREIGN KEY (SSN)
	REFERENCES [Employee] (SSN)
