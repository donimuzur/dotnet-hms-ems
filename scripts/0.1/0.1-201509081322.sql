
/****** Object:  Table [dbo].[LACK1_TRACKING]    Script Date: 9/8/2015 11:56:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LACK1_TRACKING](
	[LACK1_TRACKING_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LACK1_ID] [int] NOT NULL,
	[INVENTORY_MOVEMENT_ID] [bigint] NOT NULL,
	[CREATED_DATE] [datetime] NOT NULL,
 CONSTRAINT [PK_LACK1_TRACKING] PRIMARY KEY CLUSTERED 
(
	[LACK1_TRACKING_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LACK1_TRACKING]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_TRACKING_INVENTORY_MOVEMENT] FOREIGN KEY([INVENTORY_MOVEMENT_ID])
REFERENCES [dbo].[INVENTORY_MOVEMENT] ([INVENTORY_MOVEMENT_ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LACK1_TRACKING] CHECK CONSTRAINT [FK_LACK1_TRACKING_INVENTORY_MOVEMENT]
GO

ALTER TABLE [dbo].[LACK1_TRACKING]  WITH CHECK ADD  CONSTRAINT [FK_LACK1_TRACKING_LACK1] FOREIGN KEY([LACK1_ID])
REFERENCES [dbo].[LACK1] ([LACK1_ID])
GO

ALTER TABLE [dbo].[LACK1_TRACKING] CHECK CONSTRAINT [FK_LACK1_TRACKING_LACK1]
GO


