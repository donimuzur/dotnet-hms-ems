GO
EXECUTE sp_rename N'dbo.WASTE.WASTE_QTY_STICK', N'Tmp_MARKER_REJECT_STICK_QTY', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.WASTE.WASTE_QTY_GRAM', N'Tmp_PACKER_REJECT_STICK_QTY_1', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.WASTE.Tmp_MARKER_REJECT_STICK_QTY', N'MARKER_REJECT_STICK_QTY', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.WASTE.Tmp_PACKER_REJECT_STICK_QTY_1', N'PACKER_REJECT_STICK_QTY', 'COLUMN' 
GO
ALTER TABLE dbo.WASTE ADD
	DUST_WASTE_GRAM_QTY decimal(18, 2) NULL,
	FLOOR_WASTE_GRAM_QTY decimal(18, 2) NULL,
	DUST_WASTE_STICK_QTY decimal(18, 2) NULL,
	FLOOR_WASTE_STICK_QTY decimal(18, 2) NULL
GO
ALTER TABLE dbo.WASTE
	DROP COLUMN REJECT_QTY_STICK
GO
ALTER TABLE dbo.WASTE SET (LOCK_ESCALATION = TABLE)
GO