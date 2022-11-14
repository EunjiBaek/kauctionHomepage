import React from "react";
import { useSelector } from 'react-redux';
import styled from 'styled-components';
import { WorkListResType } from "../type";
import { RootState } from '../reducers';
import palette from '../styles/palette';
import { priceChange } from "../utils/priceChange";

const CurrentLotWrap = styled.div`
    width: 100%;
    min-height: 86px;
    position: relative;
    display: flex;
    align-items: center;
    border-bottom: 1px solid ${palette.gray[2]};
    background-color: #F5F5F5;
    padding: 10px 15px 10px 69px;
    box-sizing: border-box;

    > .lot_num {
        width: 44px; height: 44px;
        border-radius: 44px;
        background-color: ${palette.blue[9]};
        font-size: 12px;
        font-weight: 700;
        color: #fff;
        position: absolute;
        top: 50%;
        left: 15px;
        transform: translateY(-50%);
        display: flex;
        align-items: center;
        justify-content: center;
    }

    > .lot_content {
    
        > .w_info {
            width: 100%;            
            > strong {
                position: relative;
                display: block;
                line-height: 1.2em;
                font-size: 15px;
                font-weight: 600;
            }

            > span {
                display: block;
                font-weight: 600;
                line-height: 1.2em;
                margin-bottom: 3px;
                color: ${palette.gray[5]}
            }
        }

        .w_price_currency,
        .w_price {
            font-size: 12px;
            line-height: 1.2em;
            color: ${palette.gray[4]}
        }
    }

`;

interface props {
    currentLotInfo: WorkListResType;
}

const BiddingSelectInfo = ({currentLotInfo}: props) => {

    const { w_low_price, w_high_price, usd_w_low_price, usd_w_high_price, jpy_w_low_price, jpy_w_high_price, cny_w_low_price, cny_w_high_price, hkd_w_low_price, hkd_w_high_price, eur_w_low_price, eur_w_high_price } = currentLotInfo;
    // currency 통화
    const { currency } = useSelector((state: RootState) => state.auction);
    return (
        <CurrentLotWrap>
            <div className='lot_num'>
                {currentLotInfo.lot_num}
            </div>

            <div className='lot_content'>
                <div className='w_info'>
                    <strong>{currentLotInfo.a_name}</strong>
                    <span>{currentLotInfo.w_name}</span>
                </div>

                <div className='w_price'>KRW {priceChange(w_low_price)} ~ {priceChange(w_high_price)}</div>
                {currency === "USD" ?
                    <div className='w_price_currency'>( USD {priceChange(usd_w_low_price)} ~ {priceChange(usd_w_high_price)} )</div> :
                currency === "JPY" ?
                    <div className='w_price_currency'>( JPY {priceChange(jpy_w_low_price)} ~ {priceChange(jpy_w_high_price)} )</div> :
                currency === "CNY" ?
                    <div className='w_price_currency'>( CNY {priceChange(cny_w_low_price)} ~ {priceChange(cny_w_high_price)} )</div> :
                currency === "HKD" ?
                    <div className='w_price_currency'>( HKD {priceChange(hkd_w_low_price)} ~ {priceChange(hkd_w_high_price)} )</div> :
                currency === "EUR" ?
                    <div className='w_price_currency'>( EUR {priceChange(eur_w_low_price)} ~ {priceChange(eur_w_high_price)} )</div> :
                null
                }
            </div>

        </CurrentLotWrap>
    )
}

export default BiddingSelectInfo;