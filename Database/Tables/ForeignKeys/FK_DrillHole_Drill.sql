ALTER TABLE [dbo].[DrillHole]
	ADD CONSTRAINT [FK_DrillHole_Drill]
	FOREIGN KEY (MachineID)
	REFERENCES [Drill] ([MachineID])
