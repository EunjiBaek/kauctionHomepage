import { all, fork } from 'redux-saga/effects';
import auctionSaga from './auction';
import userSaga from './user';

// userSaga
export default function* rootSaga() {
    yield all([fork(auctionSaga), fork(userSaga)])
}