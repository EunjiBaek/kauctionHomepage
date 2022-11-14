import React from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../reducers';
import AucStateForm from '../components/AucStateForm';



// eslint-disable-next-line react/function-component-definition
const AucStateContainer = () => {

    // 임시 작업
    const scheduleCnt = useSelector((state: RootState) => state.auction?.scheduleCnt);

    return (
        <AucStateForm scheduleCnt={scheduleCnt} />
    )
}

export default AucStateContainer;