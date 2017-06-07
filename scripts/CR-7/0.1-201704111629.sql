
/****** Object:  Table [dbo].[MASTER_DATA_APPROVE_SETTING]    Script Date: 4/11/2017 4:28:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MASTER_DATA_APPROVE_SETTING](
	[PAGE_ID] [int] NOT NULL,
	[COLUMN_NAME] [varchar](50) NOT NULL,
	[IS_APPROVAL] [bit] NULL,
 CONSTRAINT [PK_MASTER_DATA_APPROVE_SETTING] PRIMARY KEY CLUSTERED 
(
	[PAGE_ID] ASC,
	[COLUMN_NAME] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MASTER_DATA_APPROVE_SETTING]  WITH CHECK ADD  CONSTRAINT [FK_MASTER_DATA_APPROVE_SETTING_PAGE] FOREIGN KEY([PAGE_ID])
REFERENCES [dbo].[PAGE] ([PAGE_ID])
GO

ALTER TABLE [dbo].[MASTER_DATA_APPROVE_SETTING] CHECK CONSTRAINT [FK_MASTER_DATA_APPROVE_SETTING_PAGE]
GO

