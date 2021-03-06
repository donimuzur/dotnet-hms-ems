
/****** Object:  Table [dbo].[BACK1]    Script Date: 8/18/2015 9:22:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BACK1](
	[BACK1_ID] [int] IDENTITY(1,1) NOT NULL,
	[BACK1_NUMBER] [nvarchar](30) NOT NULL,
	[BACK1_DATE] [datetime] NOT NULL,
	[PBCK3_PBCK7_ID] [int] NOT NULL,
 CONSTRAINT [PK_BACK1] PRIMARY KEY CLUSTERED 
(
	[BACK1_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BACK1_DOCUMENT]    Script Date: 8/18/2015 9:22:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BACK1_DOCUMENT](
	[BACK1_DOCUMENT_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[BACK1] [int] NOT NULL,
	[FILE_NAME] [nvarchar](50) NOT NULL,
	[FILE_PATH] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_BACK1_DOCUMENT] PRIMARY KEY CLUSTERED 
(
	[BACK1_DOCUMENT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BACK3_CK2_PBCK3_PBCK7]    Script Date: 8/18/2015 9:22:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BACK3_CK2_PBCK3_PBCK7](
	[BACK3_CK2_PBCK3_PBCK7_ID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PBCK3_PBCK7]    Script Date: 8/18/2015 9:22:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PBCK3_PBCK7](
	[PBCK3_PBCK7_ID] [int] IDENTITY(1,1) NOT NULL,
	[PBCK7_NUMBER] [nvarchar](30) NOT NULL,
	[PBCK3_NUMBER] [nvarchar](30) NULL,
	[PBCK7_STATUS] [int] NOT NULL,
	[PBCK3_STATUS] [int] NULL,
	[PBCK7_DATE] [datetime] NOT NULL,
	[PBCK3_DATE] [datetime] NULL,
	[DOC_TYPE] [int] NOT NULL,
	[NPPBCK_ID] [nvarchar](15) NULL,
	[PLANT_ID] [nvarchar](2) NULL,
	[PLANT_NAME] [nvarchar](30) NULL,
	[PLANT_CITY] [nvarchar](25) NULL,
	[EXEC_DATE_FROM] [datetime] NULL,
	[EXEC_DATE_TO] [datetime] NULL,
	[GOV_STATUS] [int] NOT NULL,
	[STATUS] [int] NOT NULL,
	[APPROVED_BY] [nvarchar](50) NULL,
	[APPROVED_DATE] [datetime] NOT NULL,
	[CREATED_BY] [nvarchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[MODIFIED_BY] [nvarchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
 CONSTRAINT [PK_PBCK3_PBCK7] PRIMARY KEY CLUSTERED 
(
	[PBCK3_PBCK7_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[BACK1]  WITH CHECK ADD  CONSTRAINT [FK_BACK1_PBCK3_PBCK7] FOREIGN KEY([PBCK3_PBCK7_ID])
REFERENCES [dbo].[PBCK3_PBCK7] ([PBCK3_PBCK7_ID])
GO
ALTER TABLE [dbo].[BACK1] CHECK CONSTRAINT [FK_BACK1_PBCK3_PBCK7]
GO
ALTER TABLE [dbo].[BACK1_DOCUMENT]  WITH CHECK ADD  CONSTRAINT [FK_BACK1_DOCUMENT_BACK1] FOREIGN KEY([BACK1])
REFERENCES [dbo].[BACK1] ([BACK1_ID])
GO
ALTER TABLE [dbo].[BACK1_DOCUMENT] CHECK CONSTRAINT [FK_BACK1_DOCUMENT_BACK1]
GO
ALTER TABLE [dbo].[PBCK3_PBCK7]  WITH CHECK ADD  CONSTRAINT [FK_PBCK3_PBCK7_APPROVED_USER] FOREIGN KEY([APPROVED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[PBCK3_PBCK7] CHECK CONSTRAINT [FK_PBCK3_PBCK7_APPROVED_USER]
GO
ALTER TABLE [dbo].[PBCK3_PBCK7]  WITH CHECK ADD  CONSTRAINT [FK_PBCK3_PBCK7_CREATED_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[PBCK3_PBCK7] CHECK CONSTRAINT [FK_PBCK3_PBCK7_CREATED_USER]
GO
ALTER TABLE [dbo].[PBCK3_PBCK7]  WITH CHECK ADD  CONSTRAINT [FK_PBCK3_PBCK7_MODIFIED_USER] FOREIGN KEY([MODIFIED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO
ALTER TABLE [dbo].[PBCK3_PBCK7] CHECK CONSTRAINT [FK_PBCK3_PBCK7_MODIFIED_USER]
GO
