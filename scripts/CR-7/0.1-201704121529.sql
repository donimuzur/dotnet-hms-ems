/*
   Wednesday, April 12, 20173:45:05 PM
   User: 
   Server: localhost
   Database: EMS_QAS
   Application: 
*/


BEGIN TRANSACTION
GO
ALTER TABLE dbo.PAGE ADD
	MAIN_TABLE varchar(50) NULL
GO
ALTER TABLE dbo.PAGE SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
