UPDATE [USER_PLANT_MAP]
SET NPPBKC_ID = (SELECT TOP 1 NPPBKC_ID FROM T001W WHERE T001W.WERKS = [USER_PLANT_MAP].PLANT_ID)