ALTER TABLE [dbo].[Drilling]
	ADD CONSTRAINT [FK_Drilling_Drill]
	FOREIGN KEY (MachineID)
	REFERENCES [Drill] (MachineID)
