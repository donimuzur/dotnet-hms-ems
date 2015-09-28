select pbck_comp.NUMBER, (QTY_APPROVED - ck5_temp.total) as quota
from

(select a.PBCK1_ID,a.NUMBER, a.QTY_APPROVED,a.NPPBKC_ID, a.SUPPLIER_PLANT_WERKS,a.SUPPLIER_NPPBKC_ID from PBCK1 a
where a.STATUS = 105) as pbck_comp

join
(select 
-- b.SOURCE_PLANT_ID,b.SOURCE_PLANT_NPPBKC_ID,b.DEST_PLANT_ID,b.DEST_PLANT_NPPBKC_ID,
b.PBCK1_DECREE_ID, sum(b.GRAND_TOTAL_EX) as total 

from ck5 b 
where ISNULL(b.PBCK1_DECREE_ID,0) <> 0 and b.STATUS_ID <> 100
group by 
--b.SOURCE_PLANT_ID,b.SOURCE_PLANT_NPPBKC_ID,b.DEST_PLANT_ID,b.DEST_PLANT_NPPBKC_ID,
b.PBCK1_DECREE_ID) as ck5_temp on pbck_comp.PBCK1_ID = ck5_temp.PBCK1_DECREE_ID;
