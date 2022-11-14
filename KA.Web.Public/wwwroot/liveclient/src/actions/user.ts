import {
    GET_MYBID_REQUEST,
    GET_MYBID_SUCCESS,
    GET_MYBID_FAILURE,
} from '../reducers/user';


/**
 * 
 * @param param0 내응찰내역조회
 * @returns 
 */
export const getMyBidRequest = ({auc_kind, auc_num, page_no, page_size}: {auc_kind: number, auc_num: number, page_no: number, page_size: number}) => ({
    type: GET_MYBID_REQUEST,
    data: {
        auc_kind,
        auc_num,
        page_no,
        page_size
    }
});

export const getMyBidSuccess = (data: any) => ({
    type: GET_MYBID_SUCCESS,
    data
});

export const getMyBidFailure = (err: unknown | any) => ({
    type: GET_MYBID_FAILURE,
    error: err
});



export type ActionRequest =
  | ReturnType<typeof getMyBidRequest>
  | ReturnType<typeof getMyBidSuccess>
  | ReturnType<typeof getMyBidFailure>
;
