ALTER TABLE [dbo].[DrillLog]
	ADD CONSTRAINT [FK_DrillLog_Employee]
	FOREIGN KEY (DrillerSSN)
	REFERENCES [Employee] (SSN)
