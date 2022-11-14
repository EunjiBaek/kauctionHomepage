/* eslint-disable react/jsx-no-useless-fragment */
import React from "react";
import { useSelector } from 'react-redux';
import BiddingInsertForm from '../components/BiddingInsertForm';
import { AucStateResType } from '../type';
import { RootState } from '../reducers';



interface Props {
    currentLotStat: AucStateResType;
};



const BiddingInsertContainer = ({ currentLotStat }: Props) => {
    const { biddingInsertError, biddingInsertDone } = useSelector((state: RootState) => state.auction);
    const scheduleCnt = useSelector((state: RootState) => state.auction?.scheduleCnt);
    const my_paddle_num = useSelector((state: RootState) => state.auction.scheduleCnt?.my_paddle_num);

    
    return (
        <>
            { scheduleCnt &&
                <BiddingInsertForm currentLotStat={currentLotStat} my_paddle_num={my_paddle_num} error={biddingInsertError}
                done={biddingInsertDone} 
            />
            }
        </>
    )
}

export default BiddingInsertContainer;