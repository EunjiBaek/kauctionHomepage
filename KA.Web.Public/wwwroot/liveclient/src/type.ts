// 옥션 스케줄 조회
export interface AucStateResType {
  auc_title: string;
  auc_kind: number;
  auc_num: number;
  auc_date: string;
  lot_stat_cd: string;
  auc_place: string;
  auc_start_date: string;
  bid_increase_price: number;
  auc_end_date: string;
  my_paddle_num: number;
  my_bid_type: number;
  auc_stat_cd: string;
  img_file_name: string;
  lot_num: number;
  pre_bid_price: number;
  bid_price: number;
  next_bid_price: number;
  successful_bid: boolean;
  successful_bid_price: number;
  successful_paddle_num: number;
  pre_bid_proc_yn: boolean;
  is_fair_warning: boolean;
  usd_next_bid_price: number;
  jpy_next_bid_price: number;
  cny_next_bid_price: number;
  hkd_next_bid_price: number;
  eur_next_bid_price: number;
  usd_bid_price: number;
  jpy_bid_price: number;
  cny_bid_price: number;
  hkd_bid_price: number;
  eur_bid_price: number;
  claim_bid_hst_seq: number;
}

// 작품리스트 조회
export interface WorkListResType {
  img_file_name: string | undefined;
  t_seq: number;
  lot_num: number;
  a_name: string;
  w_name: string;
  w_price: number;
  w_low_price: number;
  w_high_price: number;
  usd_w_low_price: number;
  usd_w_high_price: number;
  jpy_w_low_price: number;
  jpy_w_high_price: number;
  cny_w_low_price: number;
  cny_w_high_price: number;
  hkd_w_low_price: number;
  hkd_w_high_price: number;
  eur_w_low_price: number;
  eur_w_high_price: number;
  w_unit: string;
  w_size: string;
  successful_bid_price: number;
  successful_bid: boolean;
  is_fair_warning: boolean;
  work_seq: number;
  isWish: boolean;
}

// 관심작품목록 조회 (payload)
export interface WishInfoSelectReqType {
  auc_kind: string;
  auc_num: string;
  page_no: number;
  page_size: number;
}

// 관심작품조회 조회
export interface WishInfoSelectResType {
  lot_num: number;
  w_name: string;
  a_name: string;
  w_unit: string;
  w_make_date: string;
  w_size: string;
  w_price: number;
  w_low_price: number;
  w_high_price: number;
  successful_bid: boolean;
  successful_bid_price: number;
  work_seq: number;
  work_code: string;
  t_seq: number;
  img_file_name: string;
  isWish: number;
}

// 작품 종류 조회 (근현대/고미술)
export interface tSeqResType {
  t_seq: number;
}

// 공지사항조회
export interface noticeCntResType {
  noti_seq: number;
  noti_sort_num: number;
  noti_memo: string;
  noti_reg_date: string;
  noti_mod_date: string;
}

// 현재진행중인 lot의 최고 응찰가
export interface currentBidHstResType {
  bid_hst_seq: number;
  bid_reg_date: string;
  bid_price: number;
  usd_bid_price: number;
  jpy_bid_price: number;
  cny_bid_price: number;
  hdk_bid_price: number;
  eur_bid_price: number;
  lot_num: number;
  bid_type_cd: string;
  paddle_num: string;
  successful_bid: boolean;
  bid_stat_cd: string;
  isCancel: boolean;
  total_cnt: number;
}

// 응찰등록
export interface biddingInsertReqType {
  auc_kind: number;
  auc_num: number;
  lot_num: number;
  paddle_num: number;
  bid_price: number;
}

// 응찰등록결과
export interface biddingInsertResType {
  code: string;
  msg: string;
  bid_price: number;
  lot_num: number;
  reg_date: string;
}

// 내 응찰내역 조회
export interface mybidInfoResType {
  lot_num: number;
  bid_reg_date: string;
  bid_price: number;
  bid_stat_cd: string;
  successful_bid: boolean;
  w_name: string;
  a_name: string;
}

// 전체 응찰내역 조회
export interface bidsInfoResType {
  bid_hst_seq: number;
  bid_reg_date: string;
  bid_price: number;
  usd_bid_price: number;
  jpy_bid_price: number;
  cny_bid_price: number;
  hdk_bid_price: number;
  eur_bid_price: number;
  lot_num: number;
  bid_type_cd: string;
  paddle_num: string;
  successful_bid: boolean;
  bid_stat_cd: string;
  isCancel: boolean;
  total_cnt: number;
}

// 현재진행중인랏의 클레임
export interface currentBidClaimType {
  bid_hst_seq: number;
  auc_kind: number;
  auc_num: number;
  lot_num: number;
  paddle_num: number;
  bid_price: number;
  bid_type_cd: string;
  bid_stat_cd: string;
  bid_reg_dat: string;
  bid_chg_reasn_memo: string;
  bid_chg_date: string;
  bid_noti_memo: string;
}
