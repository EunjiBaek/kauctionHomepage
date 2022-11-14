import axios from 'axios';

// 내응찰내역조회
export const getMyBid = (data: any) => {
    return axios.post('/api/Live/usp_Live_Auc_Bidding_Hst_SelectForMybid', data);
}

