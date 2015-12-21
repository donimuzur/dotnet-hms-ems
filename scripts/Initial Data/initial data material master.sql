USE [EMS_QAS] -- change this with database name which this script to be executed on
GO


SET NOCOUNT OFF;

DECLARE @uom_id varchar(10), @umren decimal(18,4)
,@werks varchar(4), @sticker_code nvarchar(18)
, @uomexist int,@formtypematerialmaster int,@modifieduser varchar(50)
,@goodtyp varchar(5),@tempgoodtyp varchar(5);

--set @goodtyp = '02'; -- good type code for TIS

set @uom_id = 'G';
set @umren = 0.0010;
set @formtypematerialmaster = 24;
set @modifieduser = 'SCRIPTED';

DECLARE material_cursor CURSOR FOR
select WERKS,STICKER_CODE from ZAIDM_EX_MATERIAL group by WERKS,STICKER_CODE;

OPEN material_cursor

FETCH NEXT FROM material_cursor 
INTO @werks, @sticker_code;

WHILE @@FETCH_STATUS = 0
BEGIN
	select @uomexist = count(*) from MATERIAL_UOM where WERKS = @werks and STICKER_CODE = @sticker_code and MEINH = @uom_id ;
	select @tempgoodtyp = EXC_GOOD_TYP from ZAIDM_EX_MATERIAL where WERKS = @werks and STICKER_CODE = @sticker_code;

	if @uomexist = 0 and @tempgoodtyp in ('02','03') -- '02' for TIS and '03' for TIS reject
	begin
		insert into MATERIAL_UOM(STICKER_CODE,WERKS,MEINH,UMREN) values(@sticker_code,@werks,@uom_id,@umren);
		insert into CHANGES_HISTORY(FORM_TYPE_ID,FORM_ID,FIELD_NAME,OLD_VALUE,NEW_VALUE,MODIFIED_DATE,MODIFIED_BY)
		VALUES(@formtypematerialmaster,@sticker_code+@werks,'CONVERTION ADDED','',@uom_id+' - '+CONVERT(varchar(10),@umren,0),CURRENT_TIMESTAMP,@modifieduser);
	end
	FETCH NEXT FROM material_cursor 
	INTO @werks, @sticker_code;
END

CLOSE material_cursor;
DEALLOCATE material_cursor;