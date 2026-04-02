ALTER TABLE [dbo].[Employee]
	ADD CONSTRAINT [FK_Employee_Employee]
	FOREIGN KEY (SupervisorSSN)
	REFERENCES [Employee] (SSN)
