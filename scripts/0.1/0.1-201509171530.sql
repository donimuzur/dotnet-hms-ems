ALTER TABLE CHANGES_HISTORY
ALTER COLUMN OLD_VALUE nvarchar(MAX) NULL
GO

ALTER TABLE CHANGES_HISTORY
ALTER COLUMN NEW_VALUE nvarchar(MAX) NULL
GO