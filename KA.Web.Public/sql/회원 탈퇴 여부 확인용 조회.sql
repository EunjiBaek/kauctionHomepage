SELECT TOP (1000) * FROM [db_log].[dbo].[homepage_web_call_log] WHERE user_id = 104366 ORDER BY reg_date DESC
SELECT * FROM db_kauction.dbo.tbl_member_address WHERE mem_uid = 104366
SELECT * FROM [db_kauction].dbo.tbl_member WHERE uid = 104366

SELECT * FROM db_koffice.dbo.off_member_excp WHERE mem_name = '황호영'

SELECT * FROM db_kauction.dbo.tbl_member_retire where uid = 104366

SELECT * FROM db_koffice.dbo.off_member_www WHERE mem_uid = 103034

-- req_type_cd 를 KAUCTION으로 하면 자동삭제 처리
EXECUTE db_koffice.dbo.usp_Mem_ProgressDateChkForException @chk_mode=N'RETIRE', @koff_mem_uid=103034, @kauc_mem_uid=104366, @req_typ_cd=N'NO'
