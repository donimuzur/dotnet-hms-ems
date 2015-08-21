USE [EMS-DEV2]
GO

/****** Object:  Table [dbo].[LACK2_DOCUMENT]    Script Date: 8/21/2015 1:03:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LACK2_DOCUMENT](
	[LACK2_DOCUMENT_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK2_ID] [int] NOT NULL,
	[FILE_NAME] [nvarchar](50) NOT NULL,
	[FILE_PATH] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_LACK2_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[LACK2_DOCUMENT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LACK2_DOCUMENT]  WITH CHECK ADD  CONSTRAINT [FK_LACK2_DOCUMENT_LACK2] FOREIGN KEY([LACK2_ID])
REFERENCES [dbo].[LACK2] ([LACK2_ID])
GO

ALTER TABLE [dbo].[LACK2_DOCUMENT] CHECK CONSTRAINT [FK_LACK2_DOCUMENT_LACK2]
GO


