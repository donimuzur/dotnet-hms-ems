ALTER TABLE MONTH_CLOSING ADD
	IS_ACTIVE bit NULL
GO

UPDATE MONTH_CLOSING SET IS_ACTIVE = 1