BEGIN TRANSACTION
GO
ALTER TABLE dbo.ZAIDM_EX_MATERIAL
	DROP CONSTRAINT DF_ZAIDM_EX_MATERIAL_IS_DELETED
GO
ALTER TABLE dbo.ZAIDM_EX_MATERIAL
	DROP COLUMN IS_DELETED
GO
ALTER TABLE dbo.ZAIDM_EX_MATERIAL SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

BEGIN TRANSACTION
GO
ALTER TABLE ZAIDM_EX_KPPBC
ADD MENGETAHUI_DETAIL TEXT NULL
GO
COMMIT