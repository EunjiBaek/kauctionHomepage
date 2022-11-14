import React from "react";
import styled from 'styled-components';
import { BellOutlined } from '@ant-design/icons';
import { FormattedMessage } from "react-intl";
import palette from '../styles/palette';
import { noticeCntResType } from '../type';

const Wrapper = styled.div`
    border: 1px solid ${palette.gray[2]};
    overflow: hidden;

    > .noticeform-top {
        display: flex;
        align-items: center;
        padding: 0 20px;
        box-sizing: border-box;
        height: 46px;
        background-color: #f5f5f5;
        border-bottom: 1px solid ${palette.gray[2]};

        > .icon {
            width: 22px; height: 22px;
            border-radius: 22px;
            background-color: ${palette.gray[3]};
            display: flex;
            align-items: center;
            justify-content: center;
        }

        > span {
            font-size: 15px;
            font-weight: 700;
            padding-left: 8px;
        }
    }

    > .noticeform-bottom {
        height: 122px;
        overflow-y: auto;
        background-color: ${palette.gray[0]};
        box-sizing: border-box;
        padding: 20px;
    }
`;

const BellIcon = styled(BellOutlined)`
    svg {
        fill: white;
    }
`;

const NoticeListWrap = styled.ul`
    > li {
        display: flex;
        aligin-items: flex-start;
        margin-bottom: 5px;

        > div {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 20px; height: 18px;

            > span {
                font-size: 12px;
                color: ${palette.gray[5]}
            }
        }

        > p {
            flex-grow: 1;
            font-size: 14px;
            color: ${palette.gray[5]}
        }
    }
`;


interface Props {
    noticeCnt: noticeCntResType[] | null;
};


const NoticeForm = ({ noticeCnt }: Props) => {
    return (
        <>

        <Wrapper>
            <div className='noticeform-top'>
                <div className='icon'>
                    <BellIcon />
                </div>
                <span><FormattedMessage id="Notice"/></span>
            </div>

            <div className='noticeform-bottom'>
                <NoticeListWrap>
                    {noticeCnt?.map((cnt) => 
                        <li key={cnt.noti_sort_num}>
                            <div>
                                <span>•</span>
                            </div>
                            <p>{cnt.noti_memo}</p>
                        </li>
                    )}
                </NoticeListWrap>
            </div>
        </Wrapper>
        </>
    )
}

export default NoticeForm