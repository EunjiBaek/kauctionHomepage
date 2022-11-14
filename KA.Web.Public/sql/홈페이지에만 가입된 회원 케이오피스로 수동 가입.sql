DECLARE @auth_yn CHAR(1) = 'Y';
DECLARE @uid INT = 5331;
DECLARE @koffice_uid	INT				= 0;
DECLARE @activity_uid	INT				= 0;
DECLARE @code			VARCHAR(2)		= '00';
DECLARE @error			VARCHAR(1000)	= '';

UPDATE	db_kauction.dbo.tbl_member
SET		company_reg_no = '', company_president = '', company_rep_tel = '', company_tel = ''
WHERE	[uid]			= @uid
;

EXEC db_koffice.dbo.usp_mem_insert_From_kauction @auth_yn=@auth_yn, @www_seq = @uid, @activity_uid = @activity_uid, @mem_uid = @koffice_uid OUTPUT, @code = @code OUTPUT, @error = @error OUTPUT
;

IF @code <> '00'
BEGIN
    print @error
    ;
    ROLLBACK TRAN
    ;
    THROW 51000, 'KOFFICE Insert Error', 33
    ;
END

UPDATE	db_kauction.dbo.tbl_member
SET		koffice_uid		= @koffice_uid
WHERE	[uid]			= @uid
;

UPDATE	db_koffice.dbo.tbl_member_dlv_info
SET		[uid]	= @koffice_uid
WHERE	[dlv_seq]			= 106873

UPDATE	db_koffice.dbo.tbl_member_dlv_info
SET		[uid]	= @koffice_uid
WHERE	[uid]			= 114503

UPDATE	db_kauction.dbo.tbl_member_address
SET		koffice_uid		= (SELECT TOP 1 dlv_seq FROM db_koffice.dbo.tbl_member_dlv_info WITH (NOLOCK) WHERE [uid] = @koffice_uid AND dlv_typ_cd = '001')
WHERE	[uid]			= @uid;

-- 원본 케이오피스 주소 업데이트 쿼리

DECLARE @address_uid	INT	= 0;

UPDATE	db_koffice.dbo.tbl_member_dlv_info
SET		site_dlv_key	= @address_uid
WHERE	[uid]			= @koffice_uid
;


-- 조회용 보조 쿼리 
SELECT * FROM db_kauction.dbo.tbl_member WHERE uid = 5331;

SELECT * FROM db_kauction.dbo.tbl_member_address WHERE mem_uid = 5331;

SELECT * FROM db_koffice.dbo.tbl_member_dlv_info WHERE reg_www_uid = 5331;
SELECT * FROM db_koffice.dbo.off_member_www WHERE www_seq = 5331;
SELECT * FROM db_koffice.dbo.off_member WHERE uid = 102496;


SELECT * FROM db_kauction.dbo.tbl_member WHERE company_reg_no IS NOT NULL;
