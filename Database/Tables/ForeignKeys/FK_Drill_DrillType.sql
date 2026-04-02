ALTER TABLE [dbo].[Drill]
	ADD CONSTRAINT [FK_Drill_DrillType]
	FOREIGN KEY (ModelName)
	REFERENCES [DrillType] (ModelName)
