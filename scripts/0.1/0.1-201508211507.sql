BEGIN TRANSACTION

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_ITEM]') AND type in (N'U'))
BEGIN
 DROP TABLE LACK1_ITEM
END
GO

COMMIT TRANSACTION