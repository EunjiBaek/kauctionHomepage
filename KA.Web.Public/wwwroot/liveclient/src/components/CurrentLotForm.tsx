import React from "react";
import styled from 'styled-components';
import { FormattedMessage } from "react-intl";
import palette from '../styles/palette';
import { WorkListResType } from "../type";


const Wrapper = styled.div`
    position: relative;
    width: 100%; height: 175px;
    box-sizing: border-box;
    padding: 25px;
    background-color: #f6f6f6;
    border-radius: 5px;
    display: flex;
    align-items: flex-start;
    gap: 15px;
    margin-bottom: 12px;

    > .img-wrap {
        width: 116px; height: 116px;
        display: felx;
        align-items: center;
        justify-content: center;
        background-color: #fff;
        padding: 5px;
        box-sizing: border-box;
        border-radius: 5px;
        overflow: hidden;

        > img {
            height: 100%;
        }

    }

    .cnt-wrap {
        flex-grow: 1;

        > p.a_name {
            font-size: 18px;
            font-weight: 700;
        }

        > p.w_name {
            font-size: 18px;
            color: ${palette.gray[5]}
        }

        > p.w_unit,
        > p.w_size {
            color: ${palette.gray[5]}
        }
    }

    .link_wrap {
        position: absolute;
        top: 25px; right: 25px;
        color: ${palette.gray[5]};

        > a {
            display: flex;
            align-items: center;
            color: ${palette.gray[5]} !important;
        }

        img {
            opacity: 0.8;
        }
    }
`;


interface Props {
    currentLotInfo: WorkListResType;
    auc_num: number;
};


const CurrentLotForm = ({ currentLotInfo, auc_num }: Props) => {

    const { work_seq } = currentLotInfo;
    


    return (
        <Wrapper>
            <div className='img-wrap'>
                <img src={currentLotInfo.img_file_name} alt="이미지링크"/>
            </div>

            
            <div className='cnt-wrap'>
                <p className='lot_num'>LOT {currentLotInfo.lot_num}</p>
                <p className='a_name'>{currentLotInfo.a_name}</p>
                <p className='w_name'>{currentLotInfo.w_name}</p>
                <p className='w_unit'>{currentLotInfo.w_unit}</p>
                <p className='w_size'>{currentLotInfo.w_size}</p>
            </div>
            

            <div className='link_wrap'>
                <a target="_blank" href={`/Auction/Major/${auc_num}/${work_seq}`} rel="noreferrer" >
                    <FormattedMessage id='LOT_INFO'/>
                    <img src={require('../assets/img/LinkIcon.png')} alt="링크아이콘"/>
                </a>
            </div>
        </Wrapper>
    )
}

export default CurrentLotForm