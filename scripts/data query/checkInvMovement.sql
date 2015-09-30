DECLARE @periodMonth int = 9
DECLARE @periodYear int = 2015
DECLARE @destPlantId nvarchar(5) = 'ID01'

SELECT * FROM INVENTORY_MOVEMENT
WHERE MVT IN('261', '262') AND YEAR(POSTING_DATE) = @periodYear AND MONTH(POSTING_DATE) = @periodMonth
		AND PLANT_ID = @destPlantId

SELECT * FROM LACK1