USE [EMS_DEV2]
GO

DROP TABLE [CK5]
GO

/****** Object:  Table [dbo].[CK5]    Script Date: 22/07/2015 11:59:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CK5](
	[CK5_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CK5_TYPE] [int] NOT NULL,
	[KPPBC_CITY] [bigint] NULL,
	[SUBMISSION_NUMBER] [nvarchar](30) NULL,
	[SUBMISSION_DATE] [datetime] NULL,
	[REGISTRATION_NUMBER] [nvarchar](30) NULL,
	[EX_GOODS_TYPE_ID] [int] NULL,
	[EX_SETTLEMENT_ID] [int] NOT NULL,
	[EX_STATUS_ID] [int] NOT NULL,
	[REQUEST_TYPE_ID] [int] NOT NULL,
	[STO_SENDER_NUMBER] [nvarchar](50) NULL,
	[STO_RECEIVER_NUMBER] [nvarchar](50) NULL,
	[STOB_NUMBER] [nvarchar](50) NULL,
	[SOURCE_PLANT_ID] [bigint] NULL,
	[DEST_PLANT_ID] [bigint] NULL,
	[INVOICE_NUMBER] [nvarchar](50) NULL,
	[INVOICE_DATE] [datetime] NULL,
	[PBCK1_DECREE_ID] [int] NULL,
	[CARRIAGE_METHOD_ID] [int] NULL,
	[GRAND_TOTAL_EX] [numeric](18, 2) NULL,
	[PACKAGE_UOM_ID] [nvarchar](5) NULL,
	[DEST_COUNTRY_ID] [int] NULL,
	[HARBOUR] [nvarchar](50) NULL,
	[OFFICE_HARBOUR] [nvarchar](50) NULL,
	[LAST_SHELTER_HARBOUR] [nvarchar](50) NULL,
	[OFFICE_SHELTER] [nvarchar](50) NULL,
	[DN_NUMBER] [nvarchar](30) NULL,
	[GR_DATE] [datetime] NULL,
	[GI_DATE] [datetime] NULL,
	[SEALING_NOTIF_NUMBER] [nvarchar](50) NULL,
	[UNSEALING_NOTIF_NUMBER] [nvarchar](50) NULL,
	[SEALING_NOTIF_DATE] [datetime] NULL,
	[UNSEALING_NOTIF_DATE] [datetime] NULL,
	[LOADING_PORT] [nvarchar](50) NULL,
	[LOADING_PORT_NAME] [nvarchar](50) NULL,
	[LOADING_PORT_ID] [nvarchar](10) NULL,
	[FINAL_PORT] [nvarchar](50) NULL,
	[FINAL_PORT_ID] [nvarchar](10) NULL,
	[STATUS_ID] [int] NOT NULL,
	[CREATED_BY] [nvarchar](10) NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[APPROVED_BY] [nvarchar](10) NULL,
	[APPROVED_DATE] [datetime] NULL,
	[MODIFIED_DATE] [datetime] NULL,
 CONSTRAINT [PK_CK5_DOMESTIC] PRIMARY KEY CLUSTERED 
(
	[CK5_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CK5]  WITH CHECK ADD  CONSTRAINT [FK_CK5_APPROVED_BY_USER] FOREIGN KEY([APPROVED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[CK5] CHECK CONSTRAINT [FK_CK5_APPROVED_BY_USER]
GO

ALTER TABLE [dbo].[CK5]  WITH CHECK ADD  CONSTRAINT [FK_CK5_CARRIAGE_METHOD] FOREIGN KEY([CARRIAGE_METHOD_ID])
REFERENCES [dbo].[CARRIAGE_METHOD] ([CARRIAGE_METHOD_ID])
GO

ALTER TABLE [dbo].[CK5] CHECK CONSTRAINT [FK_CK5_CARRIAGE_METHOD]
GO

ALTER TABLE [dbo].[CK5]  WITH CHECK ADD  CONSTRAINT [FK_CK5_CREATED_BY_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[CK5] CHECK CONSTRAINT [FK_CK5_CREATED_BY_USER]
GO

ALTER TABLE [dbo].[CK5]  WITH CHECK ADD  CONSTRAINT [FK_CK5_PBCK1] FOREIGN KEY([PBCK1_DECREE_ID])
REFERENCES [dbo].[PBCK1] ([PBCK1_ID])
GO

ALTER TABLE [dbo].[CK5] CHECK CONSTRAINT [FK_CK5_PBCK1]
GO

ALTER TABLE [dbo].[CK5]  WITH CHECK ADD  CONSTRAINT [FK_CK5_UOM] FOREIGN KEY([PACKAGE_UOM_ID])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO

ALTER TABLE [dbo].[CK5] CHECK CONSTRAINT [FK_CK5_UOM]
GO

