
BEGIN TRANSACTION
GO
ALTER TABLE dbo.CK5 ADD
	FLAG_NPPBKC_IMPORT_DEST bit NULL
GO


COMMIT
