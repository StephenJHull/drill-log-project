ALTER TABLE [dbo].[DrillPattern]
	ADD CONSTRAINT [FK_DrillPattern_Employee]
	FOREIGN KEY (BlasterSSN)
	REFERENCES [Employee] (SSN)
