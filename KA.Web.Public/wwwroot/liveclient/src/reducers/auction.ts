import produce from 'immer';
import { ActionRequest } from '../actions/auction';
import {
  AucStateResType,
  WorkListResType,
  tSeqResType,
  noticeCntResType,
  biddingInsertResType,
  bidsInfoResType,
  currentBidClaimType,
} from '../type';

// 공지사항 조회
export const LOAD_NOTICE_REQUEST = 'auction/LOAD_NOTICE_REQUEST' as const;
export const LOAD_NOTICE_SUCCESS = 'auction/LOAD_NOTICE_SUCCESS' as const;
export const LOAD_NOTICE_FAILURE = 'auction/LOAD_NOTICE_FAILURE' as const;

// 경매 정보 조회
export const GET_SCHEDULE_REQUEST = 'auction/GET_SCHEDULE_REQUEST' as const;
export const GET_SCHEDULE_SUCCESS = 'auction/GET_SCHEDULE_SUCCESS' as const;
export const GET_SCHEDULE_FAILURE = 'auction/GET_SCHEDULE_FAILURE' as const;

// 현재작품조회
export const GET_CURRENTLOT_REQUEST = 'auction/GET_CURRENTLOT_REQUEST' as const;
export const GET_CURRENTLOT_SUCCESS = 'auction/GET_CURRENTLOT_SUCCESS' as const;
export const GET_CURRENTLOT_FAILURE = 'auction/GET_CURRENTLOT_FAILURE' as const;

// 응찰등록
export const BIDDING_INSERT_REQUEST = 'auction/BIDDING_INSERT_REQUEST' as const;
export const BIDDING_INSERT_SUCCESS = 'auction/BIDDING_INSERT_SUCCESS' as const;
export const BIDDING_INSERT_FAILURE = 'auction/BIDDING_INSERT_FAILURE' as const;

// 응찰내역조회
export const GET_BIDS_REQUEST = 'auction/GET_BIDS_REQUEST' as const;
export const GET_BIDS_SUCCESS = 'auction/GET_BIDS_SUCCESS' as const;
export const GET_BIDS_FAILURE = 'auction/GET_BIDS_FAILURE' as const;

// 관심작품추가하기
export const LIKE_WORK_REQUEST = 'auction/LIKE_WORK_REQUEST' as const;
export const LIKE_WORK_SUCCESS = 'auction/LIKE_WORK_SUCCESS' as const;
export const LIKE_WORK_FAILURE = 'auction/LIKE_WORK_FAILURE' as const;

// 관심작품목록가져오기
export const WISH_INFO_SELECT_REQUEST = 'auction/WISH_INFO_SELECT_REQUEST' as const;
export const WISH_INFO_SELECT_SUCCESS = 'auction/WISH_INFO_SELECT_SUCCESS' as const;
export const WISH_INFO_SELECT_FAILURE = 'auction/WISH_INFO_SELECT_FAILURE' as const;

// 언어변경
export const CURRENCY_REQUEST = 'auction/CURRENCY_REQUEST' as const;
export const CURRENCY_SUCCESS = 'auction/CURRENCY_SUCCESS' as const;

// 응찰내역 업데이트
export const BIDS_LIST_UPDATE_REQUEST = 'auction/BIDS_LIST_UPDATE_REQUEST' as const;
export const BIDS_LIST_UPDATE_SUCCESS = 'auction/BIDS_LIST_UPDATE_SUCCESS' as const;

// 클레임 응찰내역
export const BIDS_LIST_CLAIM_REQUEST = 'auction/BIDS_LIST_CLAIM_REQUEST' as const;
export const BIDS_LIST_CLAIM_SUCCESS = 'auction/BIDS_LIST_CLAIM_SUCCESS' as const;



export interface AuctionState {
  auc_kind: number; // 경매번호
  lotNoticeLoding: boolean;
  noticeCnt: noticeCntResType[] | null; // 공지사항
  lotNoticeError: String | null;
  getScheduleLoding: boolean;
  scheduleCnt: AucStateResType | null; // 경매 정보
  getScheduleError: String | null;
  currentLot: AucStateResType[] | null;
  workList: WorkListResType[] | null; // 경매 작품 리스트
  tSeq: tSeqResType[] | null;
  biddingInsertLoading: boolean;
  biddingInsertDone: boolean;
  biddingInsert: biddingInsertResType | null;
  biddingInsertError: String | null;
  likeWorkLoading: boolean;
  likeWorkError: String | null;
  likeWorkDone: boolean;
  getBidsLoding: boolean;
  getBidsError: null;
  bidsList: bidsInfoResType[] | null; // 응찰내역
  currency: String | null; // 통화
  bidListClaimLoading: boolean;
  bidListClaimDone: boolean;
  bidNoti: currentBidClaimType[] | null; // 클레임정보
}

export const initialState: AuctionState = {
  auc_kind: 1,
  lotNoticeLoding: false,
  noticeCnt: null,
  lotNoticeError: null,
  getScheduleLoding: false,
  scheduleCnt: null,
  getScheduleError: null,
  currentLot: null,
  workList: null,
  tSeq: null,
  biddingInsertLoading: false,
  biddingInsertDone: false,
  biddingInsert: null,
  biddingInsertError: null,
  likeWorkLoading: false,
  likeWorkError: null,
  likeWorkDone: false,
  getBidsLoding: false,
  getBidsError: null,
  bidsList: null,
  currency: null,
  bidListClaimLoading: false,
  bidListClaimDone: false,
  bidNoti: [],
};

const reducer = (state = initialState, action: ActionRequest) =>
  produce(state, (draft) => {
    switch (action.type) {
      // 경매 정보 조회
      case GET_SCHEDULE_REQUEST:
        draft.getScheduleLoding = true;
        draft.getScheduleError = null;
        break;
      case GET_SCHEDULE_SUCCESS:
        draft.getScheduleLoding = false;
        draft.scheduleCnt = action.data.data.Table[0];
        draft.workList = action.data.data.Table2;
        draft.tSeq = action.data.data.Table1;
        draft.currentLot = action.data.data.Table3;
        draft.getScheduleError = null;
        break;
      case GET_SCHEDULE_FAILURE:
        draft.getScheduleLoding = false;
        draft.getScheduleError = action.error;
        break;
      // 현재 진행중인 랏조회
      case GET_CURRENTLOT_SUCCESS:
        draft.currentLot.pop();
        draft.currentLot.push(action.data);
        break;
      // 공지사항 조회
      case LOAD_NOTICE_REQUEST:
        draft.lotNoticeLoding = true;
        draft.lotNoticeError = null;
        break;
      case LOAD_NOTICE_SUCCESS:
        draft.lotNoticeLoding = false;
        draft.noticeCnt = action.data.data.Table;
        draft.lotNoticeError = null;
        break;
      case LOAD_NOTICE_FAILURE:
        draft.lotNoticeLoding = false;
        draft.lotNoticeError = action.error;
        break;
      // 응찰등록
      case BIDDING_INSERT_REQUEST:
        draft.biddingInsertLoading = true;
        draft.biddingInsertDone = false;
        draft.biddingInsertError = null;
        break;
      case BIDDING_INSERT_SUCCESS:
        draft.biddingInsertLoading = false;
        draft.biddingInsertDone = true;
        draft.biddingInsert = action.data;
        draft.biddingInsertError = null;
        break;
      case BIDDING_INSERT_FAILURE:
        draft.biddingInsertLoading = false;
        draft.biddingInsertError = action.error;
        break;
      // 관심작품 목록 추가하기
      case LIKE_WORK_REQUEST:
        draft.likeWorkLoading = true;
        draft.likeWorkError = null;
        draft.likeWorkDone = false;
        break;
      case LIKE_WORK_SUCCESS:
        draft.likeWorkLoading = false;
        draft.likeWorkError = null;
        const list = draft.workList.find((v) => v.lot_num === action.data);
        const wishState = list.isWish;
        list.isWish = !wishState;
        draft.likeWorkDone = true;
        break;
      case LIKE_WORK_FAILURE:
        draft.likeWorkLoading = false;
        draft.likeWorkError = action.error;
        break;
      // 응착내역 조회
      case GET_BIDS_REQUEST:
        draft.getBidsLoding = true;
        draft.getBidsError = null;
        break;
      case GET_BIDS_SUCCESS:
        draft.getBidsLoding = false;
        draft.bidsList = action.data;
        draft.getBidsError = null;
        break;
      case GET_BIDS_FAILURE:
        draft.getBidsLoding = false;
        draft.getBidsError = action.error;
        break;
      // 통화 바꾸기
      case CURRENCY_SUCCESS:
        draft.currency = action.data;
        break;
      // 응찰내역 업데이트 (5개씩 가져오는 응찰내역과 비교하여 응찰내역 업데이트 처리)
      case BIDS_LIST_UPDATE_SUCCESS:
        if (!draft.bidsList || draft.bidsList == null) {
          draft.bidsList = action.data;
        } else {
          let idx = 0;
          action.data.forEach((e: any) => {
            idx = draft.bidsList.findIndex((b) => b.bid_hst_seq === e.bid_hst_seq);

            if (idx > -1) {
              draft.bidsList[idx] = e;
            } else {
              draft.bidsList.unshift(e);
            }
          });
          draft.bidsList
            ?.sort((a, b) => {
              return a.bid_hst_seq - b.bid_hst_seq;
            })
            .reverse();
        }
        break;
      // 클레임정보 업데이트 (응찰이 취소되거나 삭제되었을 경우 업데이트 처리)
      case BIDS_LIST_CLAIM_REQUEST:
        draft.bidListClaimLoading = true;
        draft.bidListClaimDone = false;
        break;
      case BIDS_LIST_CLAIM_SUCCESS:
        action.data?.forEach((e: any) => {
          const idx = draft.bidsList.findIndex((b) => b.bid_hst_seq === e.bid_hst_seq);
          if (e.bid_stat_cd === 'DEL') {
            if (idx > -1) draft.bidsList.splice(idx, 1);
          }

          if (e.bid_stat_cd === 'CNL') {
            if (idx > -1) draft.bidsList[idx] = e;
          }
          // 현재진행중인 랏의 클레임정보
          draft.currentLot[0].claim_bid_hst_seq = e.bid_hst_seq;
        });
        draft.bidsList
          ?.sort((a, b) => {
            return a.bid_hst_seq - b.bid_hst_seq;
          })
          .reverse();

        // 클레임정보
        draft.bidNoti = action.data;
        draft.bidListClaimLoading = false;
        draft.bidListClaimDone = true;
        break;
      default:
        break;
    }
  });

export default reducer;
