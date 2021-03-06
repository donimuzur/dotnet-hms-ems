USE [EMS_DEV2]
GO

DROP TABLE [PBCK1_PROD_PLAN]
GO

/****** Object:  Table [dbo].[PBCK1_PROD_PLAN]    Script Date: 14/07/2015 13:09:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PBCK1_PROD_PLAN](
	[PBCK1_PROD_PLAN_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PBCK1_ID] [int] NULL,
	[PROD_CODE] [nvarchar](2) NULL,
	[PRODUCT_TYPE] [nvarchar](35) NULL,
	[PRODUCT_ALIAS] [nvarchar](35) NULL,
	[AMOUNT] [decimal](18, 2) NULL,
	[BKC_REQUIRED] [decimal](18, 2) NULL,
	[MONTH] [int] NULL,
	[BKC_REQUIRED_UOM_ID] [nvarchar](5) NULL,
 CONSTRAINT [PK_PBCK1_PROD_PLAN] PRIMARY KEY CLUSTERED 
(
	[PBCK1_PROD_PLAN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_PROD_PLAN_MONTH] FOREIGN KEY([MONTH])
REFERENCES [dbo].[MONTH] ([MONTH_ID])
GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN] CHECK CONSTRAINT [FK_PBCK1_PROD_PLAN_MONTH]
GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_PROD_PLAN_PBCK1] FOREIGN KEY([PBCK1_ID])
REFERENCES [dbo].[PBCK1] ([PBCK1_ID])
GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN] CHECK CONSTRAINT [FK_PBCK1_PROD_PLAN_PBCK1]
GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_PROD_PLAN_UOM] FOREIGN KEY([BKC_REQUIRED_UOM_ID])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO

ALTER TABLE [dbo].[PBCK1_PROD_PLAN] CHECK CONSTRAINT [FK_PBCK1_PROD_PLAN_UOM]
GO


