

GO
DROP table [MATERIAL_UOM]
GO
/****** Object:  Table [dbo].[MATERIAL_UOM]    Script Date: 7/22/2015 5:16:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MATERIAL_UOM](
	[MATERIAL_UOM_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[STICKER_CODE] [nvarchar](18) NOT NULL,
	[WERKS] [nvarchar](4) NOT NULL,
	[MEINH] [nvarchar](10) NULL,
	[UMREZ] [numeric](18, 2) NULL,
	[UMREN] [numeric](18, 2) NULL,
 CONSTRAINT [PK_MATERIAL_UOM] PRIMARY KEY CLUSTERED 
(
	[MATERIAL_UOM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[MATERIAL_UOM]  WITH CHECK ADD  CONSTRAINT [FK_MATERIAL_UOM_ZAIDM_EX_MATERIAL] FOREIGN KEY([STICKER_CODE], [WERKS])
REFERENCES [dbo].[ZAIDM_EX_MATERIAL] ([STICKER_CODE], [WERKS])
GO
ALTER TABLE [dbo].[MATERIAL_UOM] CHECK CONSTRAINT [FK_MATERIAL_UOM_ZAIDM_EX_MATERIAL]
GO
