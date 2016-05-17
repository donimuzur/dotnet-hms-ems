USE [EMS]
GO

INSERT INTO [dbo].[PAGE]
           ([PAGE_ID]
           ,[PAGE_NAME]
           ,[PAGE_URL]
           ,[MENU_NAME]
           ,[PARENT_PAGE_ID])
     VALUES
           (36
           ,'SchedulerSettings'
           ,'~/SchedulerSetting'
           ,'Scheduler Settings'
           ,5)
GO


