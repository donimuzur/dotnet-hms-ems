USE [EMS_TRN]
GO

--select * from MATERIAL_UOM where MATERIAL_UOM.MEINH = 'G';
--select CURRENT_TIMESTAMP;
SET NOCOUNT ON;

DECLARE @uom_id varchar(10), @umren decimal(18,4)
,@werks varchar(4), @sticker_code nvarchar(18)
, @uomexist int,@formtypematerialmaster int,@modifieduser varchar(50);

set @uom_id = 'G';
set @umren = 0.0010;
set @formtypematerialmaster = 24;
set @modifieduser = 'PI';

DECLARE material_cursor CURSOR FOR
select WERKS,STICKER_CODE from ZAIDM_EX_MATERIAL group by WERKS,STICKER_CODE;

OPEN material_cursor

FETCH NEXT FROM material_cursor 
INTO @werks, @sticker_code;

WHILE @@FETCH_STATUS = 0
BEGIN
	select @uomexist = count(*) from MATERIAL_UOM where WERKS = @werks and STICKER_CODE = @sticker_code and MEINH = @uom_id ;

	if @uomexist = 0
	begin
		insert into MATERIAL_UOM(STICKER_CODE,WERKS,MEINH,UMREN) values(@sticker_code,@werks,@uom_id,@umren);
		insert into CHANGES_HISTORY(FORM_TYPE_ID,FORM_ID,FIELD_NAME,OLD_VALUE,NEW_VALUE,MODIFIED_DATE,MODIFIED_BY)
		VALUES(@formtypematerialmaster,CONCAT(@sticker_code,@werks),'CONVERTION ADDED','',CONCAT(@uom_id,' - ',@umren),CURRENT_TIMESTAMP,@modifieduser);
	end
	FETCH NEXT FROM material_cursor 
	INTO @werks, @sticker_code;
END

CLOSE material_cursor;
DEALLOCATE material_cursor;