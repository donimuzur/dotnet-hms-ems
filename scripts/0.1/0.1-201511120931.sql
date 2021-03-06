CREATE TABLE [dbo].[WASTE_STOCK](
	[WASTE_STOCK_ID] [int] IDENTITY(1,1) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[MATERIAL_NUMBER] [nvarchar](18) NOT NULL,
	[STOCK] [numeric](18, 2) NOT NULL,
	[CREATED_BY] [nvarchar](50) NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
	[MODIFIED_BY] [nvarchar](50) NULL,
	[MODIFIED_DATE] [datetime] NULL,
 CONSTRAINT [PK_WASTE_STOCK] PRIMARY KEY CLUSTERED 
(
	[WASTE_STOCK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WASTE_STOCK]  WITH CHECK ADD  CONSTRAINT [FK_WASTE_STOCK_USER] FOREIGN KEY([CREATED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[WASTE_STOCK] CHECK CONSTRAINT [FK_WASTE_STOCK_USER]
GO

ALTER TABLE [dbo].[WASTE_STOCK]  WITH CHECK ADD  CONSTRAINT [FK_WASTE_STOCK_USER1] FOREIGN KEY([MODIFIED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[WASTE_STOCK] CHECK CONSTRAINT [FK_WASTE_STOCK_USER1]
GO

ALTER TABLE [dbo].[WASTE_STOCK]  WITH CHECK ADD  CONSTRAINT [FK_WASTE_STOCK_USER2] FOREIGN KEY([MODIFIED_BY])
REFERENCES [dbo].[USER] ([USER_ID])
GO

ALTER TABLE [dbo].[WASTE_STOCK] CHECK CONSTRAINT [FK_WASTE_STOCK_USER2]
GO

ALTER TABLE [dbo].[WASTE_STOCK]  WITH CHECK ADD  CONSTRAINT [FK_WASTE_STOCK_ZAIDM_EX_MATERIAL] FOREIGN KEY([MATERIAL_NUMBER], [WERKS])
REFERENCES [dbo].[ZAIDM_EX_MATERIAL] ([STICKER_CODE], [WERKS])
GO

ALTER TABLE [dbo].[WASTE_STOCK] CHECK CONSTRAINT [FK_WASTE_STOCK_ZAIDM_EX_MATERIAL]
GO


