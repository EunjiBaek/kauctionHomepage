import React from 'react';
import { useSelector } from 'react-redux';
import NoticeForm from '../components/NoticeForm';
import { RootState } from '../reducers';



const NoticeContainer = () => {

    const { noticeCnt } = useSelector((state: RootState) => state.auction);

    return (
        <NoticeForm noticeCnt={noticeCnt}/>
    )
}

export default NoticeContainer