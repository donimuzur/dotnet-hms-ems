ALTER TABLE LACK1_TRACKING
ADD CONVERTED_QTY decimal(18, 5) null,
	CONVERTED_UOM_ID nvarchar(5) null,
	CONVERTED_UOM_DESC nvarchar(50) null;

ALTER TABLE LACK1_TRACKING_ALCOHOL
ADD CONVERTED_QTY decimal(18, 5) null,
	CONVERTED_UOM_ID nvarchar(5) null,
	CONVERTED_UOM_DESC nvarchar(50) null;