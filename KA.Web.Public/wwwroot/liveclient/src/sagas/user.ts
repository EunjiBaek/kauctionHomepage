import { all, fork, put, takeLatest, call } from 'redux-saga/effects';
import { AxiosResponse } from 'axios';
import {
    GET_MYBID_REQUEST
} from '../reducers/user';
import {
    getMyBidSuccess, 
    getMyBidFailure
}
from '../actions/user';

import * as api from '../api/user';


/**
 * 
 * @param action 내응찰내역조회
 */
function* getMyBidSaga(action: any) {

    try {
        const result:AxiosResponse = yield call(api.getMyBid, action.data);
        if (result.data.resultCd === '00') {
            yield put(getMyBidSuccess(result.data.data.Table));
        } else {
            const err = new Error("유효하지 않은 통신");
            throw err;
        }

    } catch (err) {
        console.error(err);
        yield put(getMyBidFailure((err as any).message))
    }
}



function* watchGetMybid() {
    yield takeLatest(GET_MYBID_REQUEST, getMyBidSaga);
}



export default function* userSaga() {
    yield all([
        fork(watchGetMybid),
    ]);
}
