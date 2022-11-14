/* eslint-disable no-nested-ternary */
import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import styled from 'styled-components';
import { Modal, Table } from 'antd';
import { FormattedMessage } from "react-intl";
import moment from "moment";
import palette from '../styles/palette';
import { AucStateResType } from '../type';
import { RootState } from '../reducers';
// getMyBidRequest
import { getMyBidRequest } from '../actions/user';
import { priceChange } from '../utils/priceChange';

const Wrapper = styled.div`
    position: relative;
    height: 55px;
    box-sizing: border-box;
    padding: 0 20px;
    display: felx;
    align-items: center;
    justify-content: space-between;
    border: 1px solid ${palette.gray[2]};
    background-color: #F5F5F5;
    border-radius: 5px;

    > div {
        display: flex;
        align-items: center;

        > span {
            font-size: 16px;
            margin-left: 5px
        }
    }


`;

const ButtonWrap = styled.button`
    height: 30px;
    color: ${palette.blue[4]};
    background-color: #ffffff;
    border: 1px solid ${palette.blue[4]};
    font-size: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0 10px;
    border-radius: 5px;
`;

const ModalWrapper = styled(Modal)`
    .ant-modal-wrap {
        display: flex;
        align-items: center;
        justify-content: center;

        > .ant-modal {
            padding-bottom: 0 !important;
        }
    }
    .ant-modal {
        padding-bottom: 0;
    } 
    .ant-modal-header {
        border-radius: 5px 5px 0 0;
    }
    .ant-modal-content {
        border-radius: 5px;
    }
    .ant-modal-footer {
        display: none;
    }
`;


const TableWrapper = styled(Table)`
    .ant-table-tbody > tr > td {
        text-align: center;
    }
`;



interface Props {
    scheduleCnt: AucStateResType;
};


const AucStateForm = ({ scheduleCnt }: Props) => {

    const dispatch = useDispatch();
    const { auc_kind } = useSelector((state: RootState) => state.auction);
    const { auc_num } = useSelector((state: RootState) => state.auction?.scheduleCnt);
    const { myBid } = useSelector((state: RootState) => state.user);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [mybidtable, setMybidtable] = useState([]);


    const columns = [
        {
          title: 'Lot',
          dataIndex: 'lot_num',
          key: 'lot_num'
        },
        {
          title: '작가명',
          dataIndex: 'a_name',
          key: 'a_name'
        },
        {
          title: '작품명',
          dataIndex: 'w_name',
          key: 'w_name'
        },
        {
            title: '접수일시',
            dataIndex: 'bid_reg_date',
            key: 'bid_reg_date'
        },
        {
            title: '응찰가(KRW)',
            dataIndex: 'bid_price',
            key: 'bid_price'
        },
        {
            title: '진행상태',
            dataIndex: 'bid_res',
            key: 'bid_res'
        },
    ];
    

    const showModal = useCallback(() => {
        setIsModalVisible(true);

        dispatch(getMyBidRequest({    
            "auc_kind": auc_kind,
            "auc_num": Number(auc_num),
            "page_no": 1,
            "page_size": 100
        }));

    }, [auc_num]);

    useEffect(() => {
        if(myBid) {
            const array: any[] = [];
            myBid?.map((bid: { lot_num: any; bid_reg_date: moment.MomentInput; bid_price: number; a_name: any; w_name: any; successful_bid: boolean; bid_stat_cd: string; }, idx:  number) => {

                const div = React.createElement("div", {className: 'bidState'}, `낙찰`);
               

                array[idx] = new Object();
                array[idx].lot_num = bid.lot_num;
                // array[idx].bid_reg_date = new Date(bid.bid_reg_date).toLocaleString();
                array[idx].bid_reg_date = moment(bid.bid_reg_date).format("YYYY.MM.DD HH:mm:ss");
                array[idx].bid_price = priceChange(bid.bid_price);
                array[idx].a_name = bid.a_name;
                array[idx].w_name = bid.w_name;
                array[idx].key = bid.bid_reg_date;
                {bid.successful_bid === false ? 
                    array[idx].bid_res = div               
                :
                    (
                        bid.successful_bid === true &&  bid.bid_stat_cd === 'BID'
                        ?
                            array[idx].bid_res = '응찰'
                        :
                            array[idx].bid_res = '응찰취소'
                    )
                }
            
            })
            setMybidtable(array);
        }

    }, [myBid]);


    const handleOk = () => {
        setIsModalVisible(false);
    };

    const handleCancel = () => {
        setIsModalVisible(false);
    };


    return (

        <>
        <Wrapper>
            <div>
                {/* W:대기,S:진행,P:일시정지,N:낙찰진행 */}

                {
                    scheduleCnt.auc_stat_cd === "S"
                    ? <img src={require('../assets/img/Live.png')} alt="경매시작아이콘"/>
                    :
                    scheduleCnt.auc_stat_cd === "P"
                    ? <img src={require('../assets/img/Waiting.png')} alt="경매일시정지아이콘"/>
                    : 
                    <img src={require('../assets/img/Waiting.png')} alt="경매일시정지아이콘"/>
                }

                <span>{scheduleCnt.auc_title}</span>
            </div>

            <ButtonWrap onClick={showModal}>
                <FormattedMessage id='mybid'/>
            </ButtonWrap>
        </Wrapper>
        <ModalWrapper title="내 응찰내역" open={isModalVisible} onOk={handleOk} onCancel={handleCancel}>
            <div className='content'>

                <TableWrapper 
                columns={columns} 
                size="middle" 
                dataSource={mybidtable}
                pagination={false}
                scroll={{
                    y: 270,
                }}
                />           
            </div>
        </ModalWrapper>
        </>
    )
}

export default AucStateForm;