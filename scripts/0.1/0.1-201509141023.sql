ALTER TABLE dbo.PBCK4_ITEM ADD
	FA_CODE nvarchar(18) NULL,
	PLANT_ID nvarchar(4) NULL,
	STICKER_CODE nvarchar(18) NULL,
	SERIES_CODE nvarchar(4) NULL,
	BRAND_NAME nvarchar(65) NULL,
	PRODUCT_ALIAS nvarchar(100) NULL,
	BRAND_CONTENT nvarchar(4) NULL,
	HJE decimal(15, 2) NULL,
	TARIFF decimal(15, 2) NULL,
	COLOUR nvarchar(70) NULL,
	REQUESTED_QTY decimal(15, 2) NULL,
	NO_PENGAWAS nvarchar(10) NULL
GO
