--this script must be runned before deploying the update

declare @lastnumber as int;

SELECT @lastnumber = [DOC_NUMBER_SEQ_LAST]
FROM [EMS_QAS].[dbo].[DOC_NUMBER_SEQ]
where FORM_TYPE_ID = 2;

DBCC checkident ('CK5', reseed, @lastnumber);
