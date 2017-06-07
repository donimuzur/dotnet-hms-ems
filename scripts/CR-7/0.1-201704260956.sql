


USE [EMS_QAS]
GO

/****** Object:  Table [dbo].[QUOTA_MONITORING]    Script Date: 4/27/2017 1:40:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[QUOTA_MONITORING](
	[MONITORING_ID] [int] IDENTITY(1,1) NOT NULL,
	[NPPBKC_ID] [varchar](20) NULL,
	[SUPPLIER_NPPBKC_ID] [varchar](20) NULL,
	[SUPPLIER_WERKS] [varchar](50) NULL,
	[PERIOD_FROM] [datetime] NULL,
	[PERIOD_TO] [datetime] NULL,
	[EX_GROUP_TYPE] [int] NULL,
	[WARNING_LEVEL] [int] NOT NULL,
 CONSTRAINT [PK_QUOTA_MONITORING] PRIMARY KEY CLUSTERED 
(
	[MONITORING_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[QUOTA_MONITORING]  WITH CHECK ADD  CONSTRAINT [FK_QUOTA_MONITORING_EX_GROUP_TYPE] FOREIGN KEY([EX_GROUP_TYPE])
REFERENCES [dbo].[EX_GROUP_TYPE] ([EX_GROUP_TYPE_ID])
GO

ALTER TABLE [dbo].[QUOTA_MONITORING] CHECK CONSTRAINT [FK_QUOTA_MONITORING_EX_GROUP_TYPE]
GO








/****** Object:  Table [dbo].[QUOTA_MONITORING_DETAIL]    Script Date: 4/26/2017 11:21:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QUOTA_MONITORING_DETAIL](
	[MONITORING_DETAIL_ID] [int] IDENTITY(1,1) NOT NULL,
	[MONITORING_ID] [int] NULL,
	[USER_ID] [nvarchar](50) NULL,
	[ROLE_ID] [int] NULL,
	[EMAIL_STATUS] [int] NULL,
 CONSTRAINT [PK_QUOTA_MONITORING_DETAIL] PRIMARY KEY CLUSTERED 
(
	[MONITORING_DETAIL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QUOTA_MONITORING_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_QUOTA_MONITORING_DETAIL_QUOTA_MONITORING] FOREIGN KEY([MONITORING_ID])
REFERENCES [dbo].[QUOTA_MONITORING] ([MONITORING_ID])
GO

ALTER TABLE [dbo].[QUOTA_MONITORING_DETAIL] CHECK CONSTRAINT [FK_QUOTA_MONITORING_DETAIL_QUOTA_MONITORING]
GO

ALTER TABLE [dbo].[QUOTA_MONITORING_DETAIL]  WITH CHECK ADD  CONSTRAINT [FK_QUOTA_MONITORING_DETAIL_QUOTA_MONITORING_DETAIL] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[QUOTA_MONITORING_DETAIL] CHECK CONSTRAINT [FK_QUOTA_MONITORING_DETAIL_QUOTA_MONITORING_DETAIL]
GO
