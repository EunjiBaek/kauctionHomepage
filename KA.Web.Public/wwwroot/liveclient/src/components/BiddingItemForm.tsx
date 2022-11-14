import React from "react";
import { useSelector } from 'react-redux';
import styled from 'styled-components';
import { FormattedMessage } from "react-intl";
import palette from '../styles/palette';
import { currentBidHstResType } from "../type";
import { priceChange } from "../utils/priceChange";
import { RootState } from '../reducers';



const ListWrap = styled.div`
    padding: 20px 0;
    border-bottom: 1px solid ${palette.gray[2]};

    > p.date {
        font-size: 11px;
        color: ${palette.gray[6]};

        &.active {
            text-decoration-line: line-through;
        }
    }

    > .bid-info {
        display: flex;
        align-items: center;
        justify-content: space-between;

        > div:first-child {
            display: flex;
            align-items: center;

            > span {
                font-weight: 600;
                &.active {
                    text-decoration-line: line-through;
                }
            }
        }

        > div.price {
            color: ${palette.gray[7]};

            &.high {
                color: ${palette.orange[6]};
            }
            
            &.active {
                text-decoration-line: line-through;
            }

        }
    }
`;

const MyBid = styled.div`
    font-size: 11px;
    padding-left: 5px;
    color: ${palette.orange[5]}
`;
const BidSucIcon = styled.div`
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 10px;
    height: 22px;
    border-radius: 22px;
    font-size: 11px;
    border: 1px solid ${palette.orange[5]};
    color: ${palette.orange[5]};
    background-color: #fff;
    margin-left: 5px;
`;
interface props {
    list: currentBidHstResType;
    index: number;
}


const BiddingItemForm = ({list, index}: props) => {
    
    const { bid_price, bid_stat_cd, paddle_num } = list;
    const { my_paddle_num } = useSelector((state: RootState) => state.auction?.scheduleCnt);
    const { bidListClaimLoading } = useSelector((state: RootState) => state.auction);

    if (bid_stat_cd === 'DEL') {
        return null
    }


    return (
        <>
        
        {/* 응찰 리스트 */}
            <ListWrap>
                <p className={bid_stat_cd !== "BID" ? "date active" : "date"}>
                    {
                        bid_stat_cd !== "BID" && list.bid_reg_date.indexOf("T") !== -1 ? 
                        <span>{list.bid_reg_date.replace("T", " ").replace(/\..*/, '').split(" ")[1]}</span>
                        :
                        <span>{list.bid_reg_date}</span>
                    }
                </p>

                <div className='bid-info'>
                    <div>
                        {list.bid_type_cd === "FLD" ?
                            <>   
                                {!bidListClaimLoading && bid_stat_cd === "CNL"?
                                    <span className="active"><FormattedMessage id="FLD"/></span>
                                    :
                                    <span><FormattedMessage id="FLD"/></span>
                                }
                            </> 
                            :
                            <>   
                                {!bidListClaimLoading && bid_stat_cd === "CNL" ?
                                    <span className="active">{paddle_num}</span>
                                    :
                                    <span>{paddle_num}</span>
                                }
                            </> 
                        }

                        {/* mypaddle_num */}
                        {list.paddle_num === `"# "+ ${my_paddle_num.toString()}` &&
                            <MyBid>My Bid</MyBid> 
                        }
                        

                        {/* 낙찰일 경우 */}
                        {list.successful_bid === false &&
                            <BidSucIcon>낙찰</BidSucIcon> 
                        }


                    </div>
                    {index === 0 ?
                        <div className={bid_stat_cd === "CNL" ? "price high active" : "price high"}>KRW {priceChange(bid_price)}</div>
                        :
                        // price active                    
                        <div className={bid_stat_cd === "CNL" ? "price active" : "price"}>KRW {priceChange(bid_price)}</div>
                    }
                </div>
            </ListWrap>
        </>
    )
}

export default BiddingItemForm;