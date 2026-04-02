ALTER TABLE [dbo].[Employee]
	ADD CONSTRAINT [FK_Employee_EmployeeRole]
	FOREIGN KEY (JobTitle)
	REFERENCES [EmployeeRole] (JobTitle)
