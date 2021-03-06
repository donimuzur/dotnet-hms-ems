ALTER TABLE ZAIDM_EX_MATERIAL
ALTER COLUMN ISSUE_STORANGE_LOC NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_MATERIAL
ALTER COLUMN BASE_UOM_ID NVARCHAR(5) NULL
ALTER TABLE ZAIDM_EX_MATERIAL
ALTER COLUMN EXC_GOOD_TYP NVARCHAR(2) NULL
ALTER TABLE ZAIDM_EX_MATERIAL
ALTER COLUMN MATERIAL_DESC NVARCHAR(50) NULL

ALTER TABLE ZAIDM_EX_KPPBC
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_KPPBC
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_KPPBC
ADD IS_DELETED BIT NULL DEFAULT 0


ALTER TABLE ZAIDM_EX_SERIES
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_SERIES
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_SERIES
ADD IS_DELETED BIT NULL DEFAULT 0


ALTER TABLE ZAIDM_EX_PRODTYP
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_PRODTYP
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_PRODTYP
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE ZAIDM_EX_GOODTYP
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_GOODTYP
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_GOODTYP
ADD IS_DELETED BIT NULL DEFAULT 0


ALTER TABLE ZAIDM_EX_MARKET
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_MARKET
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_MARKET
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE ZAIDM_EX_KPPBC
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_KPPBC
ADD IS_DELETED BIT NULL DEFAULT 0


ALTER TABLE T001K
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE T001K
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE T001K
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE T001K
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE T001K
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE T001K
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE T001W
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE T001W
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE T001
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE T001
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE T001
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE T001
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE ZAIDM_EX_PCODE
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_PCODE
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE ZAIDM_EX_PCODE
ADD IS_DELETED BIT NULL DEFAULT 0


ALTER TABLE UOM
ADD IS_DELETED BIT NULL DEFAULT 0

ALTER TABLE LFA1
ADD CREATED_BY NVARCHAR(50) NULL
ALTER TABLE LFA1
ADD MODIFIED_BY NVARCHAR(50) NULL
ALTER TABLE LFA1
ADD IS_DELETED BIT NULL DEFAULT 0