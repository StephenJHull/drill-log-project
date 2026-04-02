ALTER TABLE [dbo].[DrillPattern]
	ADD CONSTRAINT [FK_DrillPattern_DrillLog]
	FOREIGN KEY (ShotID)
	REFERENCES [DrillLog] (ShotID)
