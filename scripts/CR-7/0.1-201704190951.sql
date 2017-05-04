
INSERT INTO [dbo].[PAGE]
           ([PAGE_ID]
           ,[PAGE_NAME]
           ,[PAGE_URL]
           ,[MENU_NAME]
           ,[PARENT_PAGE_ID])
     VALUES
           (38
           ,'MasterDataApproveSetting'
           ,'~/MasterDataApproveSetting'
           ,'Master Data Approval Settings'
           ,5)
           
GO

INSERT INTO [dbo].[PAGE]
           ([PAGE_ID]
           ,[PAGE_NAME]
           ,[PAGE_URL]
           ,[MENU_NAME]
           ,[PARENT_PAGE_ID])
     VALUES
           (39
           ,'MasterDataApproval'
           ,'~/MasterDataApproval'
           ,'Master Data Approval'
           ,5)
           
GO



UPDATE [dbo].[PAGE]
   SET [MAIN_TABLE] = 'POA'
 WHERE PAGE_ID= 18
GO


UPDATE [dbo].[PAGE]
   SET [MAIN_TABLE] = 'ZAIDM_EX_BRAND'
 WHERE PAGE_ID= 22
GO

UPDATE [dbo].[PAGE]
   SET [MAIN_TABLE] = 'ZAIDM_EX_MATERIAL'
 WHERE PAGE_ID= 24
GO

