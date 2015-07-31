USE [EMS_DEV2]
GO

/****** Object:  Table [dbo].[WORKFLOW_STATE]    Script Date: 7/23/2015 2:32:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WORKFLOW_STATE](
	[ACTION_ID] [int] IDENTITY(1,1) NOT NULL,
	[ACTION_NAME] [varchar](50) NULL,
	[FORM_ID] [bigint] NULL,
	[EMAIL_TEMPLATE_ID] [int] NULL,
 CONSTRAINT [PK_WORKFLOW_STATE] PRIMARY KEY CLUSTERED 
(
	[ACTION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[WORKFLOW_STATE]  WITH CHECK ADD  CONSTRAINT [FK_WORKFLOW_STATE_EMAIL_TEMPLATE] FOREIGN KEY([EMAIL_TEMPLATE_ID])
REFERENCES [dbo].[EMAIL_TEMPLATE] ([EMAIL_TEMPLATE_ID])
GO

ALTER TABLE [dbo].[WORKFLOW_STATE] CHECK CONSTRAINT [FK_WORKFLOW_STATE_EMAIL_TEMPLATE]
GO





/****** Object:  Table [dbo].[WORKFLOW_STATE_USERS]    Script Date: 7/23/2015 2:38:53 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WORKFLOW_STATE_USERS](
	[ACTION_ID] [int] NOT NULL,
	[USER_ID] [nvarchar](10) NOT NULL
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


