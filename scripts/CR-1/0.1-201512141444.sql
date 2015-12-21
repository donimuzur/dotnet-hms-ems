IF OBJECT_ID('dbo.LACK1_TRACKING_ALCOHOL', 'U') IS NOT NULL
  DROP TABLE dbo.LACK1_TRACKING_ALCOHOL; 


CREATE TABLE [dbo].[LACK1_TRACKING_ALCOHOL](
	[LACK1_TRACKING_ALCOHOL_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NOT NULL,
	[MVT] [nvarchar](3) NOT NULL,
	[MATERIAL_ID] [nvarchar](18) NOT NULL,
	[PLANT_ID] [nvarchar](4) NOT NULL,
	[QTY] [decimal](18, 5) NULL,
	[BUN] [nvarchar](5) NULL,
	[PURCH_DOC] [nvarchar](10) NULL,
	[MAT_DOC] [nvarchar](10) NOT NULL,
	[BATCH] [nvarchar](35) NULL,
	[ORDR] [nvarchar](10) NULL,
	[IS_FINAL_GOODS] [bit] NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[TrackLevel] [int] NOT NULL,
 CONSTRAINT [PK_LACK1_TRACKING_ALCOHOL] PRIMARY KEY CLUSTERED 
(
	[LACK1_TRACKING_ALCOHOL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LACK1_TRACKING_ALCOHOL]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_TRACKING_ALCOHOL_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO

ALTER TABLE [dbo].[LACK1_TRACKING_ALCOHOL] CHECK CONSTRAINT [FK_LACK1_TRACKING_ALCOHOL_LACK1]
GO


