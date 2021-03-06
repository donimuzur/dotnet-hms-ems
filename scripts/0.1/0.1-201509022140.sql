DROP TABLE [dbo].[CK4C_ITEM]

GO

/****** Object:  Table [dbo].[CK4C_ITEM]    Script Date: 9/2/2015 9:35:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CK4C_ITEM](
	[CK4C_ITEM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CK4C_ID] [int] NOT NULL,
	[FA_CODE] [nvarchar](18) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[PROD_QTY] [decimal](15, 2) NOT NULL,
	[UOM_PROD_QTY] [nvarchar](5) NOT NULL,
	[PROD_DATE] [datetime] NOT NULL,
	[HJE_IDR] [decimal](18, 2) NULL,
	[TARIFF] [decimal](18, 2) NULL,
	[PROD_CODE] [nvarchar](5) NULL,
	[PACKED_QTY] [decimal](18, 2) NULL,
	[UNPACKED_QTY] [decimal](18, 2) NULL,
 CONSTRAINT [PK_CK4C_ITEM] PRIMARY KEY CLUSTERED 
(
	[CK4C_ITEM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CK4C_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_CK4C_ITEM_CK4C] FOREIGN KEY([CK4C_ID])
REFERENCES [dbo].[CK4C] ([CK4C_ID])
GO

ALTER TABLE [dbo].[CK4C_ITEM] CHECK CONSTRAINT [FK_CK4C_ITEM_CK4C]
GO

ALTER TABLE [dbo].[CK4C_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_CK4C_ITEM_UOM] FOREIGN KEY([UOM_PROD_QTY])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO

ALTER TABLE [dbo].[CK4C_ITEM] CHECK CONSTRAINT [FK_CK4C_ITEM_UOM]
GO

ALTER TABLE [dbo].[CK4C_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_CK4C_ITEM_ZAIDM_EX_BRAND] FOREIGN KEY([WERKS], [FA_CODE])
REFERENCES [dbo].[ZAIDM_EX_BRAND] ([WERKS], [FA_CODE])
GO

ALTER TABLE [dbo].[CK4C_ITEM] CHECK CONSTRAINT [FK_CK4C_ITEM_ZAIDM_EX_BRAND]
GO


CREATE TABLE [dbo].[PRODUCTION](
	[COMPANY_CODE] [nvarchar](4) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[FA_CODE] [nvarchar](10) NOT NULL,
	[BRAND_DESC] [nvarchar](200) NULL,
	[QTY_UNPACKED] [decimal](18, 2) NULL,
	[QTY_PACKED] [decimal](18, 2) NULL,
	[UOM] [nvarchar](5) NULL,
	[PRODUCTION_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_PRODUCTION] PRIMARY KEY CLUSTERED 
(
	[COMPANY_CODE] ASC,
	[WERKS] ASC,
	[FA_CODE] ASC,
	[PRODUCTION_DATE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[WASTE](
	[COMPANY_CODE] [nvarchar](4) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[FA_CODE] [nvarchar](10) NOT NULL,
	[BRAND_DESC] [nvarchar](200) NULL,
	[WASTE_QTY_STICK] [decimal](18, 2) NULL,
	[WASTE_QTY_GRAM] [decimal](18, 2) NULL,
	[REJECT_QTY_STICK] [decimal](18, 2) NULL,
	[WASTE_PROD_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_WASTE] PRIMARY KEY CLUSTERED 
(
	[COMPANY_CODE] ASC,
	[WERKS] ASC,
	[FA_CODE] ASC,
	[WASTE_PROD_DATE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


