/*
   Thursday, January 28, 20163:01:10 PM
   User: 
   Server: localhost
   Database: EMS2
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

ALTER TABLE dbo.CK5 ADD
	MATDOC varchar(50) NULL
GO
ALTER TABLE dbo.CK5 SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
