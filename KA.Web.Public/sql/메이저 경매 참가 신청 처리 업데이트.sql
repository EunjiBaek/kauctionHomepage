DECLARE @auc_kind char(1) -- 경매 종류 1 메이저
DECLARE @auc_num smallint -- 옥션 숫자
DECLARE @mem_seq int -- 홈페이지 회원 uid
DECLARE @bid_type char(1) -- 응찰 타입 5가 메이저
DECLARE @location_flag char(1) -- 장소 FLAG. K 한국, E 외국
DECLARE @reg_ip varchar(15) -- 요청 IP

-- TODO: Set parameter values here.

EXECUTE db_koffice.dbo.usp_auction_live_request_insert_www
   @auc_kind
  ,@auc_num
  ,@mem_seq
  ,@bid_type
  ,@location_flag
  ,@reg_ip
GO
