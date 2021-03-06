/****** Object:  Table [dbo].[PBCK1_DECREE_DOC]    Script Date: 24/07/2015 15:01:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PBCK1_DECREE_DOC](
	[PBCK1_DECREE_DOC_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PBCK1_ID] [int] NOT NULL,
	[FILE_NAME] [nvarchar](50) NOT NULL,
	[FILE_PATH] [nvarchar](250) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[CREATED_BY] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_PBCK1_DECREE_DOC] PRIMARY KEY CLUSTERED 
(
	[PBCK1_DECREE_DOC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PBCK1_DECREE_DOC]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_DECREE_DOC_PBCK1] FOREIGN KEY([PBCK1_ID])
REFERENCES [dbo].[PBCK1] ([PBCK1_ID])
GO

ALTER TABLE [dbo].[PBCK1_DECREE_DOC] CHECK CONSTRAINT [FK_PBCK1_DECREE_DOC_PBCK1]
GO

ALTER TABLE [dbo].[PBCK1_DECREE_DOC]  WITH CHECK ADD  CONSTRAINT [FK_PBCK1_DECREE_DOC_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[PBCK1_DECREE_DOC] CHECK CONSTRAINT [FK_PBCK1_DECREE_DOC_USER]
GO


