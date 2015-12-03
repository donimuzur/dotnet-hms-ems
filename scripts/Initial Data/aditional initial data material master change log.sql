-- additional change logs for missing itemcode

--item code AA05ZC - ID01

use EMS;
declare @formtypematerialmaster int, @sticker_code varchar(10),
@werks varchar(4),@uom_id varchar(5),@umren decimal(18,2),
@modifieduser varchar(10);

set @formtypematerialmaster = 24;
set @sticker_code = 'AA05ZC';
set @werks = 'ID01';
set @uom_id = 'G';
set @umren = 0.0010;
set @modifieduser = 'SCRIPTED';

insert into CHANGES_HISTORY(FORM_TYPE_ID,FORM_ID,FIELD_NAME,OLD_VALUE,NEW_VALUE,MODIFIED_DATE,MODIFIED_BY)
VALUES(@formtypematerialmaster,@sticker_code+@werks,'CONVERTION ADDED','',@uom_id+' - '+CONVERT(varchar(10),@umren,0),CURRENT_TIMESTAMP,@modifieduser);