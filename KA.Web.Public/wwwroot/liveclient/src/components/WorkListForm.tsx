/* eslint-disable jsx-a11y/alt-text */
import React, {useCallback, useState, useEffect} from "react";
import styled from 'styled-components';
import { useDispatch } from 'react-redux';
import {
    HeartOutlined,
    HeartFilled 
  } from '@ant-design/icons';
import { FormattedMessage } from "react-intl";
import palette from "../styles/palette";
import { WorkListResType, AucStateResType } from "../type";
import { likeWorkRequest } from "../actions/auction";
import { priceChange } from "../utils/priceChange";


const Wrapper = styled.div`
    position: relative;
    box-sizing: border-box;
    padding: 0 20px;
    &.active {
        border: 2px solid ${palette.orange[5]};
        > div {
            border-bottom: none;
        }
    }

    > div.lot_info {
        position: relative;
        min-height: 107px;
        display: flex;
        align-items: flex-start;
        box-sizing: border-box;
        padding: 14px 0 14px 95px;
        border-bottom: 1px solid ${palette.gray[2]};


        > .img_wrap {
            position: absolute;
            top: 50%; left: 0;
            transform: translateY(-50%);
            width: 85px;
            height: 85px;
            background-color: ${palette.gray[1]};
            border-radius: 5px;
            overflow: hidden;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 5px;
            box-sizing: border-box;
    
            > img {
                width: 100%;
            }
        }


        > .cnt_wrap {
            flex-grow: 1;
    
            > p.lot_num {
                font-size: 12px;
            }
    
            > p.w_name {
                font-size: 16px;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }
    
            > p.sub_tit {
                font-size: 12px;
                overflow: hidden;
                text-overflow: ellipsis;
                word-break: break-all;
                display: -webkit-box;
                -webkit-line-clamp: 2; 
                -webkit-box-orient: vertical;
                color: ${palette.gray[3]};
            }
    
            > p.w_price {
                font-size: 12px;
                color: ${palette.gray[5]};

                &.success {
                    font-weight: 700;
                    color: ${palette.orange[6]};
                }
            }
        }
    }


    &:hover {
        > div.hober-box {
            display: flex;
        }
    }


    > div.hober-box {
        position: absolute;
        top: 0; left: 0;
        width: 100%; height: 100%;
        background-color: rgba(0,0,0,0.4);
        display: none;
        align-items: center;
        justify-content: center;


        > div {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0 10px;

            > button, a {
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 12px;
                height: 30px;
                padding: 0 24px;
                background-color: ${palette.gray[0]};
                border-radius: 5px;
                color: ${palette.gray[8]};

                &:hover {
                    background-color: ${palette.gray[1]};
                }

                > span {
                    margin-right: 3px;
                }
            }
        }
     }


`;


interface Props {
    list: WorkListResType;
    className: string | '';
    id: string | '';
    currentLotStat: AucStateResType;
};


const WorkListForm = ({ list, className, id, currentLotStat }: Props) => {


    const dispatch = useDispatch();
    const { w_low_price, w_high_price, successful_bid_price, successful_bid } = list;
    const [name, setName] = useState<string>('');
    const [state, setState] = useState<string>(`KRW ${  priceChange(w_low_price)  } ~ ${  priceChange(w_high_price)}`);
    const { auc_num, auc_kind } = currentLotStat;



    useEffect(() => {
        if (list.lot_num === currentLotStat.lot_num) {
            setState(`KRW ${  priceChange(w_low_price)  } ~ ${  priceChange(w_high_price)}`)
            setName('');
        }
    }, [currentLotStat.lot_num]);

    useEffect(() => {
        if (successful_bid_price !== 0 && successful_bid === false) {
            setState(` KRW ${  priceChange(successful_bid_price)}`)
            setName('success')
        }
    }, [])

    useEffect(() => {
        if (list.lot_num === currentLotStat.lot_num && currentLotStat.lot_stat_cd === 'F') {
            setState(` KRW ${  priceChange(currentLotStat.successful_bid_price)}`);
            setName('success');
        }
    }, [currentLotStat.lot_stat_cd]);

    const onToggleLiked = useCallback(() => {
        
        dispatch(likeWorkRequest({    
            "auc_kind": auc_kind,
            "auc_num": auc_num,
            "lot_num": list.lot_num,
            "page_no": 1,
            "page_size": 100,
            "work_seq": list.work_seq
        }));

    }, [auc_kind, auc_num, list.lot_num, list.work_seq]);



    return (
        <Wrapper className={className} id={id}>
            <div className="lot_info">
                <div className='img_wrap'>
                    <img src={list.img_file_name} />
                </div>

                <div className='cnt_wrap'>
                    <p className='lot_num'>{list.lot_num}</p>
                    <p className='w_name'>{list.a_name}</p>
                    <p className='sub_tit'>{list.w_name}</p>
                    <p className={`'w_price' + ${name}`}>
                        {name === 'success' && <FormattedMessage id="Sold"/>}
                        {state}
                    </p> 
                </div>
            </div>

            <div className="hober-box">
                <div>
                    <button type="button" onClick={onToggleLiked}>
                        <span><FormattedMessage id='Add_Interest'/></span> 
                        {list.isWish === true ?
                            <HeartFilled style={{'color': '#f67137'}} />
                            :
                            <HeartOutlined/>                       
                        }
                    </button>
                    <a target="_blank" href={`/Auction/Major/${auc_num}/${list.work_seq}`} rel="noreferrer"><span><FormattedMessage id='LOT_INFO'/></span> <img src={require('../assets/img/LinkIcon.png')}/></a>
                </div>
            </div>
        </Wrapper>
    )
}

export default WorkListForm;