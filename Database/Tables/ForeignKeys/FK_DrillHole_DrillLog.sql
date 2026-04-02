ALTER TABLE [dbo].[DrillHole]
	ADD CONSTRAINT [FK_DrillHole_DrillLog]
	FOREIGN KEY (ShotID)
	REFERENCES [DrillLog] (ShotID)
