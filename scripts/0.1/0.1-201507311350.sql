USE [EMS_DEV2]
GO

/****** Object:  Table [dbo].[PBCK1_QUOTA]    Script Date: 31/07/2015 13:49:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PBCK1_QUOTA](
	[PBCK1_QUOTA_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PBCK1_ID] [int] NOT NULL,
	[CK5_ID] [bigint] NOT NULL,
	[CK5_TRANS_TYPE] [int] NOT NULL,
	[CK5_GRAND_TOTAL_EX] [decimal](18, 0) NOT NULL,
	[PBCK1_QTY_APPROVED] [decimal](18, 0) NOT NULL,
	[PREV_REMAINING_QUOTA] [decimal](18, 0) NOT NULL,
	[RECEIVED_AMOUNT] [decimal](18, 0) NOT NULL,
	[TOTAL_REMAINING_QUOTA] [decimal](18, 0) NOT NULL,
	[CREATED_BY] [nvarchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_PBCK1_QUOTA] PRIMARY KEY CLUSTERED 
(
	[PBCK1_QUOTA_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PBCK1_QUOTA]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_QUOTA_CK5] FOREIGN KEY([CK5_ID])
REFERENCES [dbo].[CK5] ([CK5_ID])
GO

ALTER TABLE [dbo].[PBCK1_QUOTA] CHECK CONSTRAINT [FK_PBCK1_QUOTA_CK5]
GO

ALTER TABLE [dbo].[PBCK1_QUOTA]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_QUOTA_PBCK1] FOREIGN KEY([PBCK1_ID])
REFERENCES [dbo].[PBCK1] ([PBCK1_ID])
GO

ALTER TABLE [dbo].[PBCK1_QUOTA] CHECK CONSTRAINT [FK_PBCK1_QUOTA_PBCK1]
GO

ALTER TABLE [dbo].[PBCK1_QUOTA]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_QUOTA_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[PBCK1_QUOTA] CHECK CONSTRAINT [FK_PBCK1_QUOTA_USER]
GO


