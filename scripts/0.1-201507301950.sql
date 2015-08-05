USE [EMS_DEV2]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE] DROP CONSTRAINT [FK_WORKFLOW_STATE_EMAIL_TEMPLATE]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE]    Script Date: 7/31/2015 11:58:49 AM ******/
DROP TABLE [dbo].[WORKFLOW_STATE]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE]    Script Date: 7/31/2015 11:58:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WORKFLOW_STATE](
	[WORKFLOW_STATE_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ACTION] [int] NOT NULL,
	[MENU_ID] [int] NULL,
	[EMAIL_TEMPLATE_ID] [int] NULL,
	[FORM_TYPE_ID] [int] NULL,
 CONSTRAINT [PK_WORKFLOW_STATE_1] PRIMARY KEY CLUSTERED 
(
	[WORKFLOW_STATE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WORKFLOW_STATE]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_STATE_EMAIL_TEMPLATE] FOREIGN KEY([EMAIL_TEMPLATE_ID])
REFERENCES [dbo].[EMAIL_TEMPLATE] ([EMAIL_TEMPLATE_ID])
GO

ALTER TABLE [dbo].[WORKFLOW_STATE] CHECK CONSTRAINT [FK_WORKFLOW_STATE_EMAIL_TEMPLATE]
GO

USE [EMS_DEV2]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] DROP CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE]
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] DROP CONSTRAINT [FK_WORKFLOW_STATE_USERS_USER]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE_USERS]    Script Date: 7/31/2015 11:59:19 AM ******/
DROP TABLE [dbo].[WORKFLOW_STATE_USERS]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE_USERS]    Script Date: 7/31/2015 11:59:19 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WORKFLOW_STATE_USERS](
	[WF_STATE_USER_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[WORKFLOW_STATE_ID] [bigint] NOT NULL,
	[USER_ID] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_WORKFLOW_STATE_USERS] PRIMARY KEY CLUSTERED 
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

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE] FOREIGN KEY([WORKFLOW_STATE_ID])
REFERENCES [dbo].[WORKFLOW_STATE] ([WORKFLOW_STATE_ID])
GO

ALTER TABLE [dbo].[WORKFLOW_STATE_USERS] CHECK CONSTRAINT [FK_WORKFLOW_STATE_USERS_WORKFLOW_STATE]
GO

