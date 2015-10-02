BEGIN TRANSACTION
GO
ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_CK5
GO
ALTER TABLE dbo.CK5 SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_POA
GO
ALTER TABLE dbo.POA SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_PBCK7
GO
ALTER TABLE dbo.PBCK7 SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_MANAGER
GO
ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_MODIFIED_USER
GO
ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_REJECT
GO
ALTER TABLE dbo.PBCK3
	DROP CONSTRAINT FK_PBCK3_USER
GO
ALTER TABLE dbo.[USER] SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE dbo.Tmp_PBCK3
	(
	PBCK3_ID int NOT NULL IDENTITY (1, 1),
	PBCK3_NUMBER nvarchar(35) NOT NULL,
	PBCK3_DATE date NOT NULL,
	PBCK7_ID int NULL,
	GOV_STATUS int NULL,
	STATUS int NULL,
	APPROVED_BY nvarchar(50) NULL,
	APPROVED_DATE datetime NULL,
	CREATED_BY nvarchar(50) NOT NULL,
	CREATED_DATE datetime NOT NULL,
	MODIFIED_BY nvarchar(50) NULL,
	MODIFIED_DATE datetime NULL,
	APPROVED_BY_MANAGER nvarchar(50) NULL,
	APPROVED_BY_MANAGER_DATE datetime NULL,
	REJECTED_BY nvarchar(50) NULL,
	REJECTED_DATE datetime NULL,
	CK5_ID bigint NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_PBCK3 SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_PBCK3 ON
GO
IF EXISTS(SELECT * FROM dbo.PBCK3)
	 EXEC('INSERT INTO dbo.Tmp_PBCK3 (PBCK3_ID, PBCK3_NUMBER, PBCK3_DATE, PBCK7_ID, GOV_STATUS, STATUS, APPROVED_BY, APPROVED_DATE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, APPROVED_BY_MANAGER, APPROVED_BY_MANAGER_DATE, REJECTED_BY, REJECTED_DATE, CK5_ID)
		SELECT PBCK3_ID, PBCK3_NUMBER, PBCK3_DATE, PBCK7_ID, GOV_STATUS, STATUS, APPROVED_BY, APPROVED_DATE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, APPROVED_BY_MANAGER, APPROVED_BY_MANAGER_DATE, REJECTED_BY, REJECTED_DATE, CK5_ID FROM dbo.PBCK3 WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_PBCK3 OFF
GO
ALTER TABLE dbo.CK2
	DROP CONSTRAINT FK_CK2_PBCK3
GO
ALTER TABLE dbo.BACK3
	DROP CONSTRAINT FK_BACK3_PBCK3
GO
DROP TABLE dbo.PBCK3
GO
EXECUTE sp_rename N'dbo.Tmp_PBCK3', N'PBCK3', 'OBJECT' 
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	PK_PBCK3 PRIMARY KEY CLUSTERED 
	(
	PBCK3_ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_MANAGER FOREIGN KEY
	(
	APPROVED_BY_MANAGER
	) REFERENCES dbo.[USER]
	(
	USER_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_MODIFIED_USER FOREIGN KEY
	(
	MODIFIED_BY
	) REFERENCES dbo.[USER]
	(
	USER_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_PBCK7 FOREIGN KEY
	(
	PBCK7_ID
	) REFERENCES dbo.PBCK7
	(
	PBCK7_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_POA FOREIGN KEY
	(
	APPROVED_BY
	) REFERENCES dbo.POA
	(
	POA_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_CK5 FOREIGN KEY
	(
	CK5_ID
	) REFERENCES dbo.CK5
	(
	CK5_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_REJECT FOREIGN KEY
	(
	REJECTED_BY
	) REFERENCES dbo.[USER]
	(
	USER_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PBCK3 ADD CONSTRAINT
	FK_PBCK3_USER FOREIGN KEY
	(
	CREATED_BY
	) REFERENCES dbo.[USER]
	(
	USER_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.BACK3 ADD CONSTRAINT
	FK_BACK3_PBCK3 FOREIGN KEY
	(
	PBCK3_ID
	) REFERENCES dbo.PBCK3
	(
	PBCK3_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.BACK3 SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.CK2 ADD CONSTRAINT
	FK_CK2_PBCK3 FOREIGN KEY
	(
	PBCK3_ID
	) REFERENCES dbo.PBCK3
	(
	PBCK3_ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.CK2 SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
