--  password set up --  
SELECT SUBSTRING(MASTER.dbo.fn_varbintohexstr(HASHBYTES('SHA2_256', '1111')), 3, 64)

SELECT * FROM db_kauction.dbo.tbl_member WHERE bid_allow_yn = 'Y' ORDER BY reg_date DESC

UPDATE db_kauction.dbo.tbl_member SET pwd = '0ffe1abd1a08215353c233d6e009613e95eec4253832a761af28ff37ac5a150c' WHERE id = 'uicsbk';
