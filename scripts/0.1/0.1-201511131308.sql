ALTER TABLE WASTE_ROLE
ADD CONSTRAINT uc_WasteRole1 UNIQUE (USER_ID,WERKS,GROUP_ROLE)

ALTER TABLE WASTE_STOCK
ADD CONSTRAINT uc_PlantAndMaterial UNIQUE (WERKS,MATERIAL_NUMBER)