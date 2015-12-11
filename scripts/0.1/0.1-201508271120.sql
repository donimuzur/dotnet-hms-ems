alter table BOM
DROP COLUMN LEVEL10
go
alter table BOM
add LEVEL10 NVARCHAR(18) NULL
GO
