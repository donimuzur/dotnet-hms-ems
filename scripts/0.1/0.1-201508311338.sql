ALTER TABLE dbo.PBCK1
	DROP CONSTRAINT FK_PBCK1_APPROVED_BY_USER
GO
ALTER TABLE dbo.[USER] SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.PBCK1
	DROP COLUMN APPROVED_BY, APPROVED_DATE
GO
ALTER TABLE dbo.PBCK1 SET (LOCK_ESCALATION = TABLE)
GO