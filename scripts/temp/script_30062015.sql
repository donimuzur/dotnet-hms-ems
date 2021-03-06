USE [EMS]
GO
/****** Object:  Table [dbo].[CHANGES_HISTORY]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CHANGES_HISTORY](
	[CHANGES_HISTORY_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FORM_TYPE_ID] [int] NOT NULL,
	[FORM_ID] [bigint] NULL,
	[FIELD_NAME] [nvarchar](25) NULL,
	[OLD_VALUE] [nvarchar](50) NULL,
	[NEW_VALUE] [nvarchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[MODIFIED_BY] [int] NULL,
 CONSTRAINT [PK_TABLE_HISTORY] PRIMARY KEY CLUSTERED 
(
	[CHANGES_HISTORY_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HEADER_FOOTER]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HEADER_FOOTER](
	[HEADER_FOOTER_ID] [int] IDENTITY(1,1) NOT NULL,
	[FORM_NAME] [nvarchar](50) NULL,
	[COMPANY_ID] [bigint] NULL,
	[HEADER_IMAGE_PATH] [nvarchar](150) NULL,
	[FOOTER_CONTENT] [nvarchar](max) NULL,
	[IS_ACTIVE] [bit] NULL,
	[CREATED_DATE] [datetime] NULL,
	[IS_DELETED] [bit] NULL,
 CONSTRAINT [PK_HEADER_FOOTER] PRIMARY KEY CLUSTERED 
(
	[HEADER_FOOTER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[T1001W]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T1001W](
	[PLANT_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[WERKS] [nvarchar](4) NULL,
	[NAME1] [nvarchar](50) NULL,
	[ORT01] [nvarchar](50) NULL,
	[CITY] [nvarchar](50) NULL,
	[PHONE] [nvarchar](15) NULL,
	[ADDRESS] [nvarchar](max) NULL,
	[SKEPTIS] [nvarchar](50) NULL,
	[NPPBCK_ID] [bigint] NULL,
	[IS_MAIN_PLANT] [bit] NULL,
	[RECEIVED_MATERIAL_TYPE_ID] [int] NULL,
	[CREATED_DATE] [datetime] NULL,
 CONSTRAINT [PK_T100W] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZAIDM_EX_BRAND]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZAIDM_EX_BRAND](
	[BRAND_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[STICKER_CODE] [nvarchar](18) NULL,
	[BRAND_CONTENT] [nvarchar](50) NULL,
	[CONVERSION] [decimal](18, 2) NULL,
	[PLANT_ID] [bigint] NOT NULL,
	[FA_CODE] [nvarchar](18) NOT NULL,
	[PER_ID] [bigint] NOT NULL,
	[BRAND_CE] [nvarchar](60) NOT NULL,
	[SKEP_NP] [nvarchar](30) NOT NULL,
	[SKEP_DATE] [datetime] NOT NULL,
	[PRODUCT_ID] [int] NULL,
	[SERIES_ID] [bigint] NOT NULL,
	[MARKET_ID] [bigint] NOT NULL,
	[COLOUR] [nvarchar](70) NULL,
	[COUNTRY_ID] [int] NULL,
	[CUT_FILLER_CODE] [nvarchar](25) NULL,
	[PRINTING_PRICE] [numeric](18, 2) NULL,
	[HJE_CURR] [int] NULL,
	[HJE_IDR] [numeric](18, 2) NULL,
	[TARIFF] [numeric](18, 2) NULL,
	[TARIFF_CURR] [int] NULL,
	[GOODTYP_ID] [int] NULL,
	[START_DATE] [datetime] NULL,
	[END_DATE] [datetime] NULL,
	[CREATED_DATE] [datetime] NULL,
	[IS_ACTIVE] [bit] NULL,
	[IS_FROM_SAP] [bit] NULL,
	[IS_DELETED] [bit] NULL,
 CONSTRAINT [PK_ZAIDM_EX_BRAND] PRIMARY KEY CLUSTERED 
(
	[BRAND_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZAIDM_EX_MATERIAL]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZAIDM_EX_MATERIAL](
	[MATERIAL_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MATERIAL_NUMBER] [nvarchar](35) NULL,
	[MATERIAL_DESC] [nvarchar](50) NULL,
	[MATERIAL_GROUP] [nvarchar](30) NULL,
	[PURCHASING_GROUP] [nvarchar](30) NULL,
	[PLANT_ID] [bigint] NULL,
	[EX_GOODTYP] [int] NULL,
	[ISSUE_STORANGE_LOC] [nvarchar](30) NULL,
	[BASE_UOM] [int] NULL,
	[CREATED_BY] [int] NULL,
	[IS_FROM_SAP] [bit] NULL,
	[CREATED_DATE] [datetime] NULL,
	[IS_DELETED] [bit] NULL,
 CONSTRAINT [PK_ZAIDM_EX_MATERIAL] PRIMARY KEY CLUSTERED 
(
	[MATERIAL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZAIDM_EX_NPPBKC]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZAIDM_EX_NPPBKC](
	[NPPBKC_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[NPPBKC_NO] [nvarchar](15) NOT NULL,
	[ADDR1] [nvarchar](50) NULL,
	[ADDR2] [nvarchar](50) NULL,
	[CITY] [nvarchar](50) NULL,
	[CITY_ALIAS] [nvarchar](15) NULL,
	[KPPBC_ID] [bigint] NULL,
	[REGION_OFFICE] [nvarchar](20) NULL,
	[REGION_OFFICE_DGCE] [nvarchar](20) NULL,
	[COMPANY_ID] [bigint] NULL,
	[VENDOR_ID] [bigint] NULL,
	[TEXT_TO] [nvarchar](50) NULL,
	[START_DATE] [datetime] NULL,
	[END_DATE] [datetime] NULL,
	[CREATED_DATE] [datetime] NULL,
	[IS_DELETED] [bit] NULL,
 CONSTRAINT [PK_ZAIDM_EX_NPPBKC] PRIMARY KEY CLUSTERED 
(
	[NPPBKC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ZAIDM_EX_POA]    Script Date: 6/30/2015 9:59:48 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZAIDM_EX_POA](
	[POA_ID] [int] IDENTITY(1,1) NOT NULL,
	[POA_CODE] [nvarchar](20) NULL,
	[POA_ID_CARD] [nvarchar](22) NOT NULL,
	[POA_ADDRESS] [nvarchar](max) NULL,
	[POA_PHONE] [nvarchar](15) NULL,
	[POA_PRINTED_NAME] [nvarchar](50) NULL,
	[TITLE] [nvarchar](50) NULL,
	[USER_ID] [int] NULL,
	[MANAGER_ID] [int] NULL,
	[EMAIL] [nvarchar](35) NULL,
	[PHONE] [nvarchar](15) NULL,
	[CREATED_DATE] [datetime] NULL,
	[MODIFIED_DATE] [datetime] NULL,
	[IS_DELETED] [bit] NULL,
	[IS_FROM_SAP] [bit] NULL,
 CONSTRAINT [PK_ZAIDM_EX_POA] PRIMARY KEY CLUSTERED 
(
	[POA_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[CHANGES_HISTORY]  WITH CHECK ADD  CONSTRAINT [FK_CHANGES_HISTORY_CHANGES_HISTORY] FOREIGN KEY([CHANGES_HISTORY_ID])
REFERENCES [dbo].[CHANGES_HISTORY] ([CHANGES_HISTORY_ID])
GO
ALTER TABLE [dbo].[CHANGES_HISTORY] CHECK CONSTRAINT [FK_CHANGES_HISTORY_CHANGES_HISTORY]
GO
ALTER TABLE [dbo].[CHANGES_HISTORY]  WITH CHECK ADD  CONSTRAINT [FK_CHANGES_HISTORY_USER] FOREIGN KEY([MODIFIED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[CHANGES_HISTORY] CHECK CONSTRAINT [FK_CHANGES_HISTORY_USER]
GO
ALTER TABLE [dbo].[HEADER_FOOTER]  WITH CHECK ADD  CONSTRAINT [FK_HEADER_FOOTER_T1001] FOREIGN KEY([COMPANY_ID])
REFERENCES [dbo].[T1001] ([COMPANY_ID])
GO
ALTER TABLE [dbo].[HEADER_FOOTER] CHECK CONSTRAINT [FK_HEADER_FOOTER_T1001]
GO
ALTER TABLE [dbo].[T1001W]  WITH CHECK ADD  CONSTRAINT [FK_T1001W_ZAIDM_EX_GOODTYP] FOREIGN KEY([RECEIVED_MATERIAL_TYPE_ID])
REFERENCES [dbo].[ZAIDM_EX_GOODTYP] ([GOODTYPE_ID])
GO
ALTER TABLE [dbo].[T1001W] CHECK CONSTRAINT [FK_T1001W_ZAIDM_EX_GOODTYP]
GO
ALTER TABLE [dbo].[T1001W]  WITH CHECK ADD  CONSTRAINT [FK_T1001W_ZAIDM_EX_NPPBKC] FOREIGN KEY([NPPBCK_ID])
REFERENCES [dbo].[ZAIDM_EX_NPPBKC] ([NPPBKC_ID])
GO
ALTER TABLE [dbo].[T1001W] CHECK CONSTRAINT [FK_T1001W_ZAIDM_EX_NPPBKC]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_COUNTRY] FOREIGN KEY([COUNTRY_ID])
REFERENCES [dbo].[COUNTRY] ([COUNTRY_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_COUNTRY]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_CURRENCY] FOREIGN KEY([HJE_CURR])
REFERENCES [dbo].[CURRENCY] ([CURRENCY_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_CURRENCY]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_CURRENCY1] FOREIGN KEY([TARIFF_CURR])
REFERENCES [dbo].[CURRENCY] ([CURRENCY_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_CURRENCY1]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_T1001W] FOREIGN KEY([PLANT_ID])
REFERENCES [dbo].[T1001W] ([PLANT_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_T1001W]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_GOODTYP] FOREIGN KEY([GOODTYP_ID])
REFERENCES [dbo].[ZAIDM_EX_GOODTYP] ([GOODTYPE_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_GOODTYP]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_MARKET] FOREIGN KEY([MARKET_ID])
REFERENCES [dbo].[ZAIDM_EX_MARKET] ([MARKET_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_MARKET]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_PCODE] FOREIGN KEY([PER_ID])
REFERENCES [dbo].[ZAIDM_EX_PCODE] ([PER_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_PCODE]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_PRODTYP] FOREIGN KEY([PRODUCT_ID])
REFERENCES [dbo].[ZAIDM_EX_PRODTYP] ([PRODUCT_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_PRODTYP]
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_SERIES] FOREIGN KEY([SERIES_ID])
REFERENCES [dbo].[ZAIDM_EX_SERIES] ([SERIES_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_BRAND] CHECK CONSTRAINT [FK_ZAIDM_EX_BRAND_ZAIDM_EX_SERIES]
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_MATERIAL_T1001W] FOREIGN KEY([PLANT_ID])
REFERENCES [dbo].[T1001W] ([PLANT_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL] CHECK CONSTRAINT [FK_ZAIDM_EX_MATERIAL_T1001W]
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_MATERIAL_UOM] FOREIGN KEY([BASE_UOM])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL] CHECK CONSTRAINT [FK_ZAIDM_EX_MATERIAL_UOM]
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_MATERIAL_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL] CHECK CONSTRAINT [FK_ZAIDM_EX_MATERIAL_USER]
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_NPPBKC_1LFA11] FOREIGN KEY([VENDOR_ID])
REFERENCES [dbo].[1LFA1] ([VENDOR_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC] CHECK CONSTRAINT [FK_ZAIDM_EX_NPPBKC_1LFA11]
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_NPPBKC_T1001] FOREIGN KEY([COMPANY_ID])
REFERENCES [dbo].[T1001] ([COMPANY_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC] CHECK CONSTRAINT [FK_ZAIDM_EX_NPPBKC_T1001]
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_NPPBKC_ZAIDM_EX_KPPBC] FOREIGN KEY([KPPBC_ID])
REFERENCES [dbo].[ZAIDM_EX_KPPBC] ([KPPBC_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_NPPBKC] CHECK CONSTRAINT [FK_ZAIDM_EX_NPPBKC_ZAIDM_EX_KPPBC]
GO
ALTER TABLE [dbo].[ZAIDM_EX_POA]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_POA_USER] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_POA] CHECK CONSTRAINT [FK_ZAIDM_EX_POA_USER]
GO
ALTER TABLE [dbo].[ZAIDM_EX_POA]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_POA_USER1] FOREIGN KEY([MANAGER_ID])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_POA] CHECK CONSTRAINT [FK_ZAIDM_EX_POA_USER1]
GO
