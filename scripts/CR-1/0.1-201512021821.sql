CREATE TABLE [dbo].[NlogLogs](
	[Nlog_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NULL,
	[Level] [varchar](50) NULL,	
	[Type] [nvarchar](255) NULL,
	[Logger] [nvarchar](100) NULL,
	[Message] [varchar](max) NULL,
	[Stacktrace] [varchar](max) NULL,
	[Source] [nvarchar](500) NULL,
	[Data] [nvarchar](max) NULL,
	[FileName] [nvarchar](100) NULL,	
 CONSTRAINT [PK_NlogLogs] PRIMARY KEY CLUSTERED 
(
	[Nlog_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO