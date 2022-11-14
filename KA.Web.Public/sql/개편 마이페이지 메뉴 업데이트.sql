SELECT * FROM db_kauction.dbo.tbl_menu WHERE type = 'UTIL';

UPDATE db_kauction.dbo.tbl_menu SET name = '응찰/낙찰 내역', name_en = 'Bid/Successful Bid Details', link = '/myPage/bids' WHERE type = 'UTIL' AND name = '응찰내역';
UPDATE db_kauction.dbo.tbl_menu SET name = '결제/출고 관리', name_en = 'Payment/Shipment Management', link = '/MyPage/Payment', use_yn='N' WHERE type = 'UTIL' AND name = '낙찰내역';
UPDATE db_kauction.dbo.tbl_menu SET name = '문의 관리', name_en = 'Inquiry Management', link = '/myPage/inquiries/consignments' WHERE type = 'UTIL' AND name = '위탁내역';
UPDATE db_kauction.dbo.tbl_menu SET name = '내 정보', name_en = 'My Info', link = '/Mypage/Member' WHERE type = 'UTIL' AND name = '위탁신청';
DELETE FROM db_kauction.dbo.tbl_menu WHERE type = 'UTIL' AND name = '도록신청'
DELETE FROM db_kauction.dbo.tbl_menu WHERE type = 'UTIL' AND name = '작품 문의내역'
DELETE FROM db_kauction.dbo.tbl_menu WHERE type = 'UTIL' AND name = '정보수정'
UPDATE db_kauction.dbo.tbl_menu SET sub_code = '008', [order] = 8 WHERE type = 'UTIL' AND name = '로그아웃';


-- 문의하기 테이블 수정
-- 1. manager_uid 컬럼 nullable 처리
-- 2. 카테고리 컬럼 추가
ALTER TABLE db_kauction.[dbo].[tbl_member_inquiry] ALTER COLUMN [manager_uid] INT NULL

ALTER TABLE db_kauction.dbo.tbl_member_inquiry ADD category VARCHAR(20) DEFAULT '001'