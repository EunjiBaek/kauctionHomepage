import axios from 'axios';

// 공지사항조회
export const loadNotice = (data: any) => {
    return axios.post('/api/Live/usp_Live_Auc_Notice_Info_Select', data);
}

// 경매정보조회
export const loadSchedule = (data: any) => {
    return axios.post('/api/Live/usp_Live_auction_schedule_Select', data);
}

// 응찰등록
export const biddingInsert = (data: any) => {
    return axios.post('/api/Live/usp_Live_Auc_Bidding_Hst_InsertProc', data);
}

// 전체응찰내역조회
export const getBids = (data: any) => {
    return axios.post('/api/Live/usp_Live_Auc_Bidding_Hst_SelectForPage', data);
}


// 관심작품추가하기
export const likeWork = (data: any) => {
    return axios.post('/api/Live/usp_Live_Mem_Wish_Info_Insert', data);
}


// 관심작품조회
export const wishInfoSelect = (data: any) => {
    return axios.post('/api/Live/usp_Live_Mem_Wish_Info_Select', data);
}

