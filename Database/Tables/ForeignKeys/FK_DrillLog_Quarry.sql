ALTER TABLE [dbo].[DrillLog]
	ADD CONSTRAINT [FK_DrillLog_Quarry]
	FOREIGN KEY (QuarryName)
	REFERENCES [Quarry] (Name)
