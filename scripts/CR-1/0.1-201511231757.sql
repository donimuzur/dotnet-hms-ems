IF OBJECT_ID('dbo.GOOD_PROD_TYPE', 'U') IS NOT NULL
  DROP TABLE dbo.GOOD_PROD_TYPE; 

CREATE TABLE [dbo].[GOOD_PROD_TYPE](
	[PROD_CODE] [nvarchar](2) NOT NULL,
	[EXC_GOOD_TYP] [nvarchar](2) NOT NULL,
 CONSTRAINT [PK_PROD_CODE] PRIMARY KEY CLUSTERED 
(
	[PROD_CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO