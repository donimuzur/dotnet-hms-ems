--check CK4C Item --
DECLARE @src_nppbkc nvarchar(50) = '0508.1.3.0234'
DECLARE @companyCode nvarchar(5) = '3066'
DECLARE @exGroupTypeId int = 3
DECLARE @src_plant_id nvarchar(50) = 'ID02'
DECLARE @destPlantId nvarchar(5) = 'ID01'
DECLARE @periodMonth int = 9
DECLARE @periodYear int = 2015
DECLARE @statusId int = 105

DECLARE @ck4cId int = 2

SELECT * FROM CK4C
WHERE COMPANY_ID = @companyCode AND NPPBKC_ID = @src_nppbkc
		AND REPORTED_MONTH = @periodMonth AND REPORTED_YEAR = @periodYear
		AND STATUS >= @statusId AND PLANT_ID = @destPlantId 

SELECT * FROM CK4C_ITEM
WHERE CK4C_ID = @ck4cId

SELECT * FROM ZAIDM_EX_PRODTYP
WHERE PROD_CODE IN ('03', '02', '01')