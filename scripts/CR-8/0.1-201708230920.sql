DECLARE @parent_id int;

SELECT @parent_id = PAGE_ID FROM [PAGE] WHERE PAGE_NAME = 'ExcisableGoodsClaimable'

INSERT INTO [PAGE](PAGE_ID,PAGE_NAME,PAGE_URL,MENU_NAME,PARENT_PAGE_ID)
VALUES(54,'LACK10','~/LACK10','Damaged Goods Report (LACK-10)',@parent_id);