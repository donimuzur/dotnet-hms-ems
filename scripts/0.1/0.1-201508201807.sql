BEGIN TRANSACTION
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_DOCUMENT]') AND type in (N'U'))
BEGIN
	DROP TABLE LACK1_DOCUMENT
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_INCOME_DETAIL]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1_INCOME_DETAIL
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_ITEM]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1_ITEM
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_PBCK1_MAPPING]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1_PBCK1_MAPPING
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_PLANT]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1_PLANT
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1_PRODUCTION_DETAIL]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1_PRODUCTION_DETAIL
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LACK1]') AND type in (N'U'))
BEGIN
DROP TABLE LACK1
END
GO

/****** Object:  Table [dbo].[LACK1]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1](
	[LACK1_ID] [int] IDENTITY(1,1) NOT NULL,
	[LACK1_NUMBER] [nvarchar](35) NULL,
	[BUKRS] [nvarchar](4) NULL,
	[BUTXT] [nvarchar](35) NULL,
	[PERIOD_MONTH] [int] NULL,
	[PERIOD_YEAR] [int] NULL,
	[SUBMISSION_DATE] [datetime] NULL,
	[SUPPLIER_PLANT] [nvarchar](70) NULL,
	[SUPPLIER_PLANT_WERKS] [nvarchar](4) NULL,
	[SUPPLIER_PLANT_ADDRESS] [nvarchar](50) NULL,
	[EX_GOODTYP] [nvarchar](2) NULL,
	[EX_TYP_DESC] [nvarchar](65) NULL,
	[WASTE_QTY] [decimal](15, 2) NULL,
	[WASTE_UOM] [nvarchar](5) NULL,
	[RETURN_QTY] [decimal](15, 2) NULL,
	[RETURN_UOM] [nvarchar](5) NULL,
	[STATUS] [int] NOT NULL,
	[GOV_STATUS] [int] NULL,
	[DECREE_DATE] [datetime] NULL,
	[NPPBKC_ID] [nvarchar](15) NULL,
	[BEGINING_BALANCE] [decimal](18, 2) NOT NULL,
	[TOTAL_INCOME] [decimal](18, 2) NOT NULL,
	[USAGE] [decimal](18, 2) NOT NULL,
	[TOTAL_PRODUCTION] [decimal](18, 2) NOT NULL,
	[LACK1_LEVEL] [int] NOT NULL,
	[CREATED_BY] [nvarchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[APPROVED_BY_POA] [nvarchar](50) NULL,
	[APPROVED_DATE_POA] [datetime] NULL,
	[APPROVED_BY_MANAGER] [nvarchar](50) NULL,
	[APPROVED_DATE_MANAGER] [datetime] NULL,
 CONSTRAINT [PK_LACK1] PRIMARY KEY CLUSTERED 
(
	[LACK1_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_DOCUMENT]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_DOCUMENT](
	[LACK1_DOCUMENT_ID] [int] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NULL,
	[FILE_NAME] [nvarchar](50) NULL,
	[FILE_PATH] [nvarchar](250) NULL,
 CONSTRAINT [PK_LACK1_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[LACK1_DOCUMENT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_INCOME_DETAIL]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_INCOME_DETAIL](
	[LACK1_INCOME_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NOT NULL,
	[CK5_ID] [bigint] NOT NULL,
	[AMOUNT] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_LACK1_INCOME_DETAIL] PRIMARY KEY CLUSTERED 
(
	[LACK1_INCOME_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_ITEM]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_ITEM](
	[LACK1_ITEM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NULL,
	[BEGINNING_BALANCE] [decimal](15, 2) NULL,
	[INCOME] [decimal](15, 2) NULL,
	[USAGE] [decimal](18, 2) NULL,
	[PRODUCTION] [decimal](18, 2) NULL,
 CONSTRAINT [PK_LACK1_ITEM] PRIMARY KEY CLUSTERED 
(
	[LACK1_ITEM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_PBCK1_MAPPING]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_PBCK1_MAPPING](
	[LACK1_ID] [int] NOT NULL,
	[PBCK1_ID] [int] NOT NULL,
 CONSTRAINT [PK_LACK1_PBCK1_MAPPING] PRIMARY KEY CLUSTERED 
(
	[LACK1_ID] ASC,
	[PBCK1_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_PLANT]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_PLANT](
	[LACK1_PLANT_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NOT NULL,
	[PLANT_ID] [nvarchar](4) NULL,
	[PLANT_NAME] [nvarchar](30) NULL,
	[PLANT_ADDRESS] [nvarchar](200) NULL,
 CONSTRAINT [PK_LACK1_PLANT] PRIMARY KEY CLUSTERED 
(
	[LACK1_PLANT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LACK1_PRODUCTION_DETAIL]    Script Date: 8/20/2015 5:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LACK1_PRODUCTION_DETAIL](
	[LACK1_PRODUCTION_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NOT NULL,
	[AMOUNT] [decimal](18, 2) NOT NULL,
	[PROD_CODE] [nvarchar](2) NOT NULL,
	[PRODUCT_TYPE] [nvarchar](35) NOT NULL,
	[PRODUCT_ALIAS] [nvarchar](35) NOT NULL,
 CONSTRAINT [PK_LACK1_PRODUCTION_DETAIL] PRIMARY KEY CLUSTERED 
(
	[LACK1_PRODUCTION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--GO
--SET IDENTITY_INSERT [dbo].[LACK1] ON 

--INSERT [dbo].[LACK1] ([LACK1_ID], [LACK1_NUMBER], [BUKRS], [BUTXT], [PERIOD_MONTH], [PERIOD_YEAR], [SUBMISSION_DATE], [SUPPLIER_PLANT], [SUPPLIER_PLANT_WERKS], [SUPPLIER_PLANT_ADDRESS], [EX_GOODTYP], [EX_TYP_DESC], [WASTE_QTY], [WASTE_UOM], [RETURN_QTY], [RETURN_UOM], [STATUS], [GOV_STATUS], [DECREE_DATE], [NPPBKC_ID], [BEGINING_BALANCE], [TOTAL_INCOME], [USAGE], [TOTAL_PRODUCTION], [LACK1_LEVEL], [CREATED_BY], [CREATED_DATE], [APPROVED_BY_POA], [APPROVED_DATE_POA], [APPROVED_BY_MANAGER], [APPROVED_DATE_MANAGER]) VALUES (1, N'00002/HMS-E//I/2016', N'3066', N'HMS-E', 1, 2015, CAST(0x0000A3FD00000000 AS DateTime), N'Sampoerna Bonded Warehouse', N'ID07', N' Pasuruan', N'01', N'Hasil Tembakau (HT)', CAST(1000.00 AS Decimal(15, 2)), N'KG', CAST(1000.00 AS Decimal(15, 2)), N'KG', 105, 2, CAST(0x0000A3FD00000000 AS DateTime), N'0508.1.3.5001', CAST(1000000.00 AS Decimal(18, 2)), CAST(1500000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), 1, N'FRATRIAN', CAST(0x0000A3FD00000000 AS DateTime), N'MYAMIN1', CAST(0x0000A3FD00000000 AS DateTime), NULL, NULL)
--INSERT [dbo].[LACK1] ([LACK1_ID], [LACK1_NUMBER], [BUKRS], [BUTXT], [PERIOD_MONTH], [PERIOD_YEAR], [SUBMISSION_DATE], [SUPPLIER_PLANT], [SUPPLIER_PLANT_WERKS], [SUPPLIER_PLANT_ADDRESS], [EX_GOODTYP], [EX_TYP_DESC], [WASTE_QTY], [WASTE_UOM], [RETURN_QTY], [RETURN_UOM], [STATUS], [GOV_STATUS], [DECREE_DATE], [NPPBKC_ID], [BEGINING_BALANCE], [TOTAL_INCOME], [USAGE], [TOTAL_PRODUCTION], [LACK1_LEVEL], [CREATED_BY], [CREATED_DATE], [APPROVED_BY_POA], [APPROVED_DATE_POA], [APPROVED_BY_MANAGER], [APPROVED_DATE_MANAGER]) VALUES (2, N'00007/HMS-E//I/2016', N'3066', N'HMS-E', 2, 2015, CAST(0x0000A3FD00000000 AS DateTime), N'Sampoerna Bonded Warehouse', N'ID07', N' Pasuruan', N'01', N'Hasil Tembakau (HT)', CAST(1000.00 AS Decimal(15, 2)), N'KG', CAST(1000.00 AS Decimal(15, 2)), N'KG', 105, 2, CAST(0x0000A3FD00000000 AS DateTime), N'0508.1.3.5001', CAST(1500000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), 1, N'FRATRIAN', CAST(0x0000A3FD00000000 AS DateTime), N'MYAMIN1', CAST(0x0000A3FD00000000 AS DateTime), NULL, NULL)
--INSERT [dbo].[LACK1] ([LACK1_ID], [LACK1_NUMBER], [BUKRS], [BUTXT], [PERIOD_MONTH], [PERIOD_YEAR], [SUBMISSION_DATE], [SUPPLIER_PLANT], [SUPPLIER_PLANT_WERKS], [SUPPLIER_PLANT_ADDRESS], [EX_GOODTYP], [EX_TYP_DESC], [WASTE_QTY], [WASTE_UOM], [RETURN_QTY], [RETURN_UOM], [STATUS], [GOV_STATUS], [DECREE_DATE], [NPPBKC_ID], [BEGINING_BALANCE], [TOTAL_INCOME], [USAGE], [TOTAL_PRODUCTION], [LACK1_LEVEL], [CREATED_BY], [CREATED_DATE], [APPROVED_BY_POA], [APPROVED_DATE_POA], [APPROVED_BY_MANAGER], [APPROVED_DATE_MANAGER]) VALUES (3, N'00008/HMS-E//I/2016', N'3066', N'HMS-E', 2, 2015, CAST(0x0000A3FD00000000 AS DateTime), N'Sampoerna Bonded Warehouse', N'ID07', N' Pasuruan', N'01', N'Hasil Tembakau (HT)', CAST(1000.00 AS Decimal(15, 2)), N'KG', CAST(1000.00 AS Decimal(15, 2)), N'KG', 105, 2, CAST(0x0000A3FD00000000 AS DateTime), N'0508.1.3.5001', CAST(1500000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), 1, N'FRATRIAN', CAST(0x0000A3FD00000000 AS DateTime), N'MYAMIN1', CAST(0x0000A3FD00000000 AS DateTime), NULL, NULL)
--INSERT [dbo].[LACK1] ([LACK1_ID], [LACK1_NUMBER], [BUKRS], [BUTXT], [PERIOD_MONTH], [PERIOD_YEAR], [SUBMISSION_DATE], [SUPPLIER_PLANT], [SUPPLIER_PLANT_WERKS], [SUPPLIER_PLANT_ADDRESS], [EX_GOODTYP], [EX_TYP_DESC], [WASTE_QTY], [WASTE_UOM], [RETURN_QTY], [RETURN_UOM], [STATUS], [GOV_STATUS], [DECREE_DATE], [NPPBKC_ID], [BEGINING_BALANCE], [TOTAL_INCOME], [USAGE], [TOTAL_PRODUCTION], [LACK1_LEVEL], [CREATED_BY], [CREATED_DATE], [APPROVED_BY_POA], [APPROVED_DATE_POA], [APPROVED_BY_MANAGER], [APPROVED_DATE_MANAGER]) VALUES (4, N'00009/HMS-E//I/2016', N'3066', N'HMS-E', 1, 2015, CAST(0x0000A3FD00000000 AS DateTime), N'Sampoerna Bonded Warehouse', N'ID07', N' Pasuruan', N'01', N'Hasil Tembakau (HT)', CAST(1000.00 AS Decimal(15, 2)), N'KG', CAST(1000.00 AS Decimal(15, 2)), N'KG', 105, 2, CAST(0x0000A3FD00000000 AS DateTime), N'0508.1.3.5001', CAST(1500000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), 2, N'FRATRIAN', CAST(0x0000A3FD00000000 AS DateTime), N'MYAMIN1', CAST(0x0000A3FD00000000 AS DateTime), NULL, NULL)
--SET IDENTITY_INSERT [dbo].[LACK1] OFF
--INSERT [dbo].[LACK1_PBCK1_MAPPING] ([LACK1_ID], [PBCK1_ID]) VALUES (1, 1)
--INSERT [dbo].[LACK1_PBCK1_MAPPING] ([LACK1_ID], [PBCK1_ID]) VALUES (2, 1)
--INSERT [dbo].[LACK1_PBCK1_MAPPING] ([LACK1_ID], [PBCK1_ID]) VALUES (3, 1)
--SET IDENTITY_INSERT [dbo].[LACK1_PLANT] ON 

--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (1, 4, N'ID01', N'Bekasi Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (2, 1, N'ID01', N'Bekasi Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (3, 1, N'ID02', N'Karawang Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (4, 1, N'ID04', N'Sukorejo Manufacturing', N' Pasuruan')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (5, 2, N'ID01', N'Bekasi Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (6, 2, N'ID02', N'Karawang Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (7, 2, N'ID04', N'Sukorejo Manufacturing', N' Pasuruan')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (8, 3, N'ID01', N'Bekasi Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (9, 3, N'ID02', N'Karawang Manufacturing', N'Karawang International Industr Karawang')
--INSERT [dbo].[LACK1_PLANT] ([LACK1_PLANT_ID], [LACK1_ID], [PLANT_ID], [PLANT_NAME], [PLANT_ADDRESS]) VALUES (10, 3, N'ID04', N'Sukorejo Manufacturing', N' Pasuruan')
--SET IDENTITY_INSERT [dbo].[LACK1_PLANT] OFF
--SET IDENTITY_INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ON 

--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (1, 1, CAST(500000.00 AS Decimal(18, 2)), N'01', N'SIGARET KRETEK TANGAN', N'SKT')
--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (2, 1, CAST(500000.00 AS Decimal(18, 2)), N'02', N'SIGARET KRETEK MESIN', N'SKM')
--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (3, 2, CAST(500000.00 AS Decimal(18, 2)), N'01', N'SIGARET KRETEK TANGAN', N'SKT')
--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (4, 2, CAST(500000.00 AS Decimal(18, 2)), N'02', N'SIGARET KRETEK MESIN', N'SKM')
--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (5, 3, CAST(500000.00 AS Decimal(18, 2)), N'01', N'SIGARET KRETEK TANGAN', N'SKT')
--INSERT [dbo].[LACK1_PRODUCTION_DETAIL] ([LACK1_PRODUCTION_ID], [LACK1_ID], [AMOUNT], [PROD_CODE], [PRODUCT_TYPE], [PRODUCT_ALIAS]) VALUES (6, 3, CAST(500000.00 AS Decimal(18, 2)), N'02', N'SIGARET KRETEK MESIN', N'SKM')
--SET IDENTITY_INSERT [dbo].[LACK1_PRODUCTION_DETAIL] OFF
ALTER TABLE [dbo].[LACK1]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_MONTH] FOREIGN KEY([PERIOD_MONTH])
REFERENCES [dbo].[MONTH] ([MONTH_ID])
GO
ALTER TABLE [dbo].[LACK1] CHECK CONSTRAINT [FK_LACK1_MONTH]
GO
ALTER TABLE [dbo].[LACK1]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_RETURN_UOM] FOREIGN KEY([RETURN_UOM])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO
ALTER TABLE [dbo].[LACK1] CHECK CONSTRAINT [FK_LACK1_RETURN_UOM]
GO
ALTER TABLE [dbo].[LACK1]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_USER_APPROVED] FOREIGN KEY([APPROVED_BY_POA])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[LACK1] CHECK CONSTRAINT [FK_LACK1_USER_APPROVED]
GO
ALTER TABLE [dbo].[LACK1]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_USER_CREATED] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[LACK1] CHECK CONSTRAINT [FK_LACK1_USER_CREATED]
GO
ALTER TABLE [dbo].[LACK1]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_WASTE_UOM] FOREIGN KEY([WASTE_UOM])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO
ALTER TABLE [dbo].[LACK1] CHECK CONSTRAINT [FK_LACK1_WASTE_UOM]
GO
ALTER TABLE [dbo].[LACK1_DOCUMENT]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_DOCUMENT_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_DOCUMENT] CHECK CONSTRAINT [FK_LACK1_DOCUMENT_LACK1]
GO
ALTER TABLE [dbo].[LACK1_INCOME_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_INCOME_DETAIL_CK5] FOREIGN KEY([CK5_ID])
REFERENCES [dbo].[CK5] ([CK5_ID])
GO
ALTER TABLE [dbo].[LACK1_INCOME_DETAIL] CHECK CONSTRAINT [FK_LACK1_INCOME_DETAIL_CK5]
GO
ALTER TABLE [dbo].[LACK1_INCOME_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_INCOME_DETAIL_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_INCOME_DETAIL] CHECK CONSTRAINT [FK_LACK1_INCOME_DETAIL_LACK1]
GO
ALTER TABLE [dbo].[LACK1_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_ITEM_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_ITEM] CHECK CONSTRAINT [FK_LACK1_ITEM_LACK1]
GO
ALTER TABLE [dbo].[LACK1_PBCK1_MAPPING]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_PBCK1_MAPPING_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_PBCK1_MAPPING] CHECK CONSTRAINT [FK_LACK1_PBCK1_MAPPING_LACK1]
GO
ALTER TABLE [dbo].[LACK1_PBCK1_MAPPING]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_PBCK1_MAPPING_PBCK1] FOREIGN KEY([PBCK1_ID])
REFERENCES [dbo].[PBCK1] ([PBCK1_ID])
GO
ALTER TABLE [dbo].[LACK1_PBCK1_MAPPING] CHECK CONSTRAINT [FK_LACK1_PBCK1_MAPPING_PBCK1]
GO
ALTER TABLE [dbo].[LACK1_PLANT]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_PLANT_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_PLANT] CHECK CONSTRAINT [FK_LACK1_PLANT_LACK1]
GO
ALTER TABLE [dbo].[LACK1_PRODUCTION_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_PRODUCTION_DETAIL_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO
ALTER TABLE [dbo].[LACK1_PRODUCTION_DETAIL] CHECK CONSTRAINT [FK_LACK1_PRODUCTION_DETAIL_LACK1]
GO

COMMIT TRANSACTION