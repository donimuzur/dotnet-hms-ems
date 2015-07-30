USE [EMS_DEV2]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] DROP CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] DROP CONSTRAINT [FK_WORKFLOW_STATE_USERS_USER]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE_USERS]    Script Date: 7/30/2015 8:51:39 PM ******/
DROP TABLE [dbo].[WORKFLOW_STATE_USERS]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE_USERS]    Script Date: 7/30/2015 8:51:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WORKFLOW_STATE_USERS](
	[ACTION_ID] [int] NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
	[WF_STATE_USER_ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_WORKFLOW_STATE_USERS_1] PRIMARY KEY CLUSTERED 
(
	[WF_STATE_USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_STATE_USERS_USER] FOREIGN KEY([USER_ID])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] CHECK CONSTRAINT [FK_WORKFLOW_STATE_USERS_USER]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE] FOREIGN KEY([ACTION_ID])
REFERENCES [dbo].[WORKFLOW_STATE] ([ACTION_ID])
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] CHECK CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE]
GO


