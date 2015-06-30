ALTER TABLE ZAIDM_EX_MATERIAL
ADD [BRAND_ID] [bigint] NULL
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_MATERIAL_ZAIDM_EX_GOODTYP] FOREIGN KEY([EX_GOODTYP])
REFERENCES [dbo].[ZAIDM_EX_GOODTYP] ([GOODTYPE_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL] CHECK CONSTRAINT [FK_ZAIDM_EX_MATERIAL_ZAIDM_EX_GOODTYP]
GO
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL]  WITH CHECK ADD  CONSTRAINT [FK_ZAIDM_EX_MATERIAL_ZAIDM_EX_BRAND] FOREIGN KEY([BRAND_ID])
REFERENCES [dbo].[ZAIDM_EX_BRAND] ([BRAND_ID])
GO
ALTER TABLE [dbo].[ZAIDM_EX_MATERIAL] CHECK CONSTRAINT [FK_ZAIDM_EX_MATERIAL_ZAIDM_EX_BRAND]
GO