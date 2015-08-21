/*new*/
select number as pbck_decree, QTY_APPROVED  as approved 
from PBCK1 a 
where PERIOD_FROM <= GETDATE() and PERIOD_TO >= GETDATE();

/*additional*/
select number as pbck_decree,QTY_APPROVED as approved 
from PBCK1 a 
where PERIOD_FROM <= GETDATE() and PERIOD_TO >= GETDATE();


/*ck5*/
select b.NUMBER as pbck_degree , a.GRAND_TOTAL_EX as moved
from CK5 a 
join pbck1 b on b.pbck1_id = a.PBCK1_DECREE_ID 
where a.STATUS_ID = 1 and a.SUBMISSION_DATE >= b.PERIOD_FROM 
and a.SUBMISSION_DATE <= b.PERIOD_TO;

