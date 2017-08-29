/****** Object:  Table [dbo].[LACK10_DECREE_DOC]    Script Date: 22/08/2017 10:53:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LACK10_DECREE_DOC](
	[LACK10_DECREE_DOC_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK10_ID] [int] NOT NULL,
	[FILE_NAME] [nvarchar](50) NOT NULL,
	[FILE_PATH] [nvarchar](250) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[CREATED_BY] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LACK10_DECREE_DOC] PRIMARY KEY CLUSTERED 
(
	[LACK10_DECREE_DOC_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LACK10_DECREE_DOC]  WITH CHECK ADD  CONSTRAINT [FK_LACK10_DECREE_DOC_LACK10] FOREIGN KEY([LACK10_ID])
REFERENCES [dbo].[LACK10] ([LACK10_ID])
GO

ALTER TABLE [dbo].[LACK10_DECREE_DOC] CHECK CONSTRAINT [FK_LACK10_DECREE_DOC_LACK10]
GO

ALTER TABLE [dbo].[LACK10_DECREE_DOC]  WITH CHECK ADD  CONSTRAINT [FK_LACK10_DECREE_DOC_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[LACK10_DECREE_DOC] CHECK CONSTRAINT [FK_LACK10_DECREE_DOC_USER]
GO


