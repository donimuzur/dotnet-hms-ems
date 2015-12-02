USE [EMS-DEV2]
GO
/****** Object:  Table [dbo].[BACK3]    Script Date: 9/14/2015 10:47:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BACK3](
	[BACK3_ID] [int] IDENTITY(1,1) NOT NULL,
	[BACK3_NUMBER] [nvarchar](35) NULL,
	[BACK3_DATE] [date] NULL,
	[PBCK3_PBCK7_ID] [int] NOT NULL,
 CONSTRAINT [PK_BACK3] PRIMARY KEY CLUSTERED 
(
	[BACK3_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BACK3_DOCUMENT]    Script Date: 9/14/2015 10:47:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BACK3_DOCUMENT](
	[BACK3_DOC_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FILE_PATH] [nvarchar](250) NOT NULL,
	[FILE_NAME] [nvarchar](150) NULL,
	[BACK3_ID] [int] NOT NULL,
 CONSTRAINT [PK_BACK3_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[BACK3_DOC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CK2]    Script Date: 9/14/2015 10:47:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CK2](
	[CK2_ID] [int] IDENTITY(1,1) NOT NULL,
	[CK2_NUMBER] [nvarchar](35) NULL,
	[CK2_DATE] [date] NULL,
	[CK2_VALUE] [decimal](15, 2) NULL,
	[PBCK3_PBCK7_ID] [int] NOT NULL,
 CONSTRAINT [PK_CK2] PRIMARY KEY CLUSTERED 
(
	[CK2_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CK2_DOCUMENT]    Script Date: 9/14/2015 10:47:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CK2_DOCUMENT](
	[CK2_DOC_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FILE_PATH] [nvarchar](250) NULL,
	[FILE_NAME] [nvarchar](150) NULL,
	[CK2_ID] [int] NOT NULL,
 CONSTRAINT [PK_CK2_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[CK2_DOC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PBCK3_PBCK7_ITEM]    Script Date: 9/14/2015 10:47:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PBCK3_PBCK7_ITEM](
	[PBCK3_PBCK7_ITEM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PBCK3_PBCK7_ID] [int] NOT NULL,
	[FA_CODE] [nvarchar](18) NULL,
	[PRODUCT_ALIAS] [nvarchar](50) NULL,
	[BRAND_CE] [nvarchar](65) NULL,
	[BRAND_CONTENT] [decimal](15, 2) NULL,
	[PBCK7_QTY] [decimal](15, 2) NULL,
	[BACK1_QTY] [decimal](15, 2) NULL,
	[SERIES_VALUE] [decimal](15, 2) NULL,
	[HJE] [decimal](15, 2) NULL,
	[TARIFF] [decimal](15, 2) NULL,
	[FISCAL_YEAR] [int] NULL,
	[EXCISE_VALUE] [decimal](15, 2) NULL,
 CONSTRAINT [PK_PBCK3_PBCK7_ITEM] PRIMARY KEY CLUSTERED 
(
	[PBCK3_PBCK7_ITEM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[BACK3]  WITH CHECK ADD  CONSTRAINT [FK_BACK3_PBCK3_PBCK7] FOREIGN KEY([PBCK3_PBCK7_ID])
REFERENCES [dbo].[PBCK3_PBCK7] ([PBCK3_PBCK7_ID])
GO
ALTER TABLE [dbo].[BACK3] CHECK CONSTRAINT [FK_BACK3_PBCK3_PBCK7]
GO
ALTER TABLE [dbo].[BACK3_DOCUMENT]  WITH CHECK ADD  CONSTRAINT [FK_BACK3_DOCUMENT_BACK3] FOREIGN KEY([BACK3_ID])
REFERENCES [dbo].[BACK3] ([BACK3_ID])
GO
ALTER TABLE [dbo].[BACK3_DOCUMENT] CHECK CONSTRAINT [FK_BACK3_DOCUMENT_BACK3]
GO
ALTER TABLE [dbo].[CK2]  WITH CHECK ADD  CONSTRAINT [FK_CK2_PBCK3_PBCK7] FOREIGN KEY([PBCK3_PBCK7_ID])
REFERENCES [dbo].[PBCK3_PBCK7] ([PBCK3_PBCK7_ID])
GO
ALTER TABLE [dbo].[CK2] CHECK CONSTRAINT [FK_CK2_PBCK3_PBCK7]
GO
ALTER TABLE [dbo].[CK2_DOCUMENT]  WITH CHECK ADD  CONSTRAINT [FK_CK2_DOCUMENT_CK2] FOREIGN KEY([CK2_ID])
REFERENCES [dbo].[CK2] ([CK2_ID])
GO
ALTER TABLE [dbo].[CK2_DOCUMENT] CHECK CONSTRAINT [FK_CK2_DOCUMENT_CK2]
GO
ALTER TABLE [dbo].[PBCK3_PBCK7_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_PBCK3_PBCK7_ITEM_PBCK3_PBCK7] FOREIGN KEY([PBCK3_PBCK7_ID])
REFERENCES [dbo].[PBCK3_PBCK7] ([PBCK3_PBCK7_ID])
GO
ALTER TABLE [dbo].[PBCK3_PBCK7_ITEM] CHECK CONSTRAINT [FK_PBCK3_PBCK7_ITEM_PBCK3_PBCK7]
GO
