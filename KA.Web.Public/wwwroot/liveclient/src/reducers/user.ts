/* eslint-disable no-param-reassign */
import produce from 'immer';
import { mybidInfoResType } from '../type';
import { ActionRequest } from '../actions/user';

// 내응찰내역조회
export const GET_MYBID_REQUEST = 'user/GET_MYBID_REQUEST' as const;
export const GET_MYBID_SUCCESS = 'user/GET_MYBID_SUCCESS' as const;
export const GET_MYBID_FAILURE = 'user/GET_MYBID_FAILURE' as const;



export interface AuctionState {
    getMyBidLoding: boolean;
    myBid: mybidInfoResType[] | null; // 경매 작품 리스트
    getMyBidError: String | null; // 경매 스케쥴 조회
}


export const initialState: AuctionState = {
    getMyBidLoding: false,
    myBid: null,
    getMyBidError: null,
}


const reducer = (state = initialState, action: ActionRequest) => produce(state, (draft) => {
    switch(action.type) {
        case GET_MYBID_REQUEST:
            draft.getMyBidLoding = true;
            draft.getMyBidError = null;
            break
        case GET_MYBID_SUCCESS:
            draft.getMyBidLoding = false;
            draft.myBid = action.data;
            draft.getMyBidError = null;
            break
        case GET_MYBID_FAILURE:
            draft.getMyBidLoding = false;
            draft.getMyBidError = action.error;
            break
        default:
            break;
    }
})


export default reducer




