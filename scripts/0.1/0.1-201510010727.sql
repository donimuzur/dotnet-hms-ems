ALTER TABLE CK4C
DROP COLUMN REMARKS
GO

ALTER TABLE CK4C_ITEM ADD
REMARKS nvarchar(100) null
GO

ALTER TABLE PRODUCTION ADD
REMARKS nvarchar(100) null
GO