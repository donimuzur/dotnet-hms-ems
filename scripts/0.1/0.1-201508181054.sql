
GO
ALTER TABLE dbo.WORKFLOW_STATE
	DROP COLUMN MENU_ID
GO
ALTER TABLE dbo.WORKFLOW_STATE SET (LOCK_ESCALATION = TABLE)
GO

