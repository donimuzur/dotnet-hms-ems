BEGIN TRANSACTION
GO
ALTER TABLE dbo.CK5 ADD
	REGISTRATION_DATE datetime NULL
GO

COMMIT