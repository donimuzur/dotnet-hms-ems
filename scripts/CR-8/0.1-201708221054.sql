/****** Object:  Table [dbo].[LACK10_ITEM]    Script Date: 22/08/2017 10:54:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LACK10_ITEM](
	[LACK10_ITEM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK10_ID] [int] NOT NULL,
	[FA_CODE] [nvarchar](18) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[TYPE] [nvarchar](50) NULL,
	[WASTE_VALUE] [decimal](18, 3) NULL,
	[UOM] [nvarchar](5) NULL,
 CONSTRAINT [PK_LACK10_ITEM] PRIMARY KEY CLUSTERED 
(
	[LACK10_ITEM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LACK10_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_LACK10_ITEM_LACK10] FOREIGN KEY([LACK10_ID])
REFERENCES [dbo].[LACK10] ([LACK10_ID])
GO

ALTER TABLE [dbo].[LACK10_ITEM] CHECK CONSTRAINT [FK_LACK10_ITEM_LACK10]
GO

ALTER TABLE [dbo].[LACK10_ITEM]  WITH CHECK ADD  CONSTRAINT [FK_LACK10_ITEM_UOM] FOREIGN KEY([UOM])
REFERENCES [dbo].[UOM] ([UOM_ID])
GO

ALTER TABLE [dbo].[LACK10_ITEM] CHECK CONSTRAINT [FK_LACK10_ITEM_UOM]
GO


