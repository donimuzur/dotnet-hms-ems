--check CK5 Item --
DECLARE @src_nppbkc nvarchar(50) = '0508.1.3.0234'
DECLARE @companyCode nvarchar(5) = '3066'
DECLARE @exGroupTypeId int = 3
DECLARE @src_plant_id nvarchar(50) = 'ID02'
DECLARE @destPlantId nvarchar(5) = 'ID01'
DECLARE @periodMonth int = 9
DECLARE @periodYear int = 2015
DECLARE @statusId int = 105

SELECT * FROM CK5 
WHERE SOURCE_PLANT_NPPBKC_ID = @src_nppbkc AND SOURCE_PLANT_COMPANY_CODE = @companyCode
	AND EX_GOODS_TYPE = @exGroupTypeId AND SOURCE_PLANT_ID = @src_plant_id
	AND YEAR(GR_DATE) = @periodYear AND MONTH(GR_DATE) = @periodMonth
	AND STATUS_ID >= @statusId AND DEST_PLANT_ID = @destPlantId