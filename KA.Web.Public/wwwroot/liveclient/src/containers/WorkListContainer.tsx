
import React, { useCallback, useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import styled from 'styled-components';
import { Checkbox } from 'antd';
import { AimOutlined, DownOutlined } from '@ant-design/icons';
import { CheckboxChangeEvent } from 'antd/lib/checkbox';
import { Link } from 'react-scroll';
import { FormattedMessage } from "react-intl";
import { AucStateResType } from '../type';
import WorkListForm from '../components/WorkListForm';
import palette from '../styles/palette';
import SearchForm from '../components/SearchForm';
import { RootState } from '../reducers';
import useDisplay from '../hooks/useDisplay';


interface Props {
    currentLotStat: AucStateResType;
};


// 진행중인 lot아이콘
const AimIcon = styled(AimOutlined)`
    font-size: 14px !important;
    padding-right: 5px;
`;

// 체크박스아이콘
const CheckboxIcon = styled(Checkbox)`

    align-items: center;
    justify-content: center;
    font-size: 0;

    .ant-checkbox {
        top: auto;
    }
    .ant-checkbox + span {
        font-size: 12px;
        padding-left: 5px;
        padding-right: 0;
        top: auto;
    }
    
    .ant-checkbox-inner {
        border-radius: 20px;
    }
    .ant-checkbox-checked .ant-checkbox-inner {
        background-color: #000;
        border-color: #000;
    }

`;

// BarWrap
const BarWrap = styled.div`
    width: 100%; height: 24px;
    background-color: ${palette.gray[1]};
    box-sizing: border-box;
    padding: 0 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    > span {
        font-size: 12px;
    }

`;

const WorkListWrapper = styled.div`
    position: relative;
    box-sizing: border-box;
    height: 100%;
    background-color: #fff;

    @media only screen and (max-width: 1250px) {
        padding-top: 40px
    }
    
    > .list_check {
        width: 100%;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: space-between;

        box-sizing: border-box;
        padding: 0 20px;
        border-bottom: 1px solid ${palette.gray[2]};

        @media only screen and (max-width: 1250px) {
            position: absolute;
            top: 0; left: 0;
            width: 100%;
        }
        
        > .list-current-lot {
            font-size :0;
            > a {
                display: flex;
                align-items: center;
            }

            span {
                font-size: 12px;
            }
            cursor: pointer;
        }

        > .list-wish-lot {
            font-size: 0;
        }
    }

    > .list_wrap {
        height: 648px;
        padding-bottom: 20px;
        overflow-y: auto;

        @media only screen and (max-width: 1250px) {
            height: 100%;
        }
    }
`;

const WorkEmpty = styled.div`
    width: 100%; height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;

    > p {
        margin-top: 5px;
        color: ${palette.gray[3]}
    }
`;


const WorkListContainer = ({ currentLotStat }: Props) => {

    const { workList, tSeq, getScheduleLoding } = useSelector((state: RootState) => state.auction);
    const [state, setState] = useState<string>('');
    const [searchTerm, setSearchTerm] = useState<string>("");
    const [checked, setChecked] = useState<boolean>(false);
    // 현재 진행중인 lot 번호
    const { lot_num } = currentLotStat;
    const [allList, setAllList] = useState([]);




    const onChange = useCallback((e: CheckboxChangeEvent) => {
        const target = e.target.checked;
        setChecked(target);
    }, []);



    const onSearchChange = useCallback((e: any) => {
        // eslint-disable-next-line prefer-destructuring, no-undef
        const value = e.target.value
        setSearchTerm(value.toString().toLowerCase());
    }, [searchTerm]);



    useEffect(() => {
        if (checked === true) {
            setAllList(workList.filter(list => list.isWish === true));
        } else {
            setAllList(workList);
        }
    }, [checked, workList]);



    const onClick = useCallback(() => {
        setState('active');
    }, []);



    
    return (
        <>
            <SearchForm onSearchChange={onSearchChange}/>

            <WorkListWrapper>
                <div className='list_check'>
                    <div className='list-current-lot'>
                        <Link containerId='containerElement' smooth to={lot_num.toString()} spy onClick={onClick} >
                            <AimIcon />
                            <span><FormattedMessage id='Current_lot'/></span>
                        </Link>
                    </div>
                    <div className='list-wish-lot'><CheckboxIcon onChange={onChange}><FormattedMessage id='MY_FAVORITE_LOTS'/></CheckboxIcon></div>
                </div>

                <div className='list_wrap' id='containerElement'>

                    {!getScheduleLoding && 
                        allList.length === 0 ?
                            <WorkEmpty>
                                <img src={require('../assets/img/no_bids_icon.png')} alt="아이콘"/>
                                <p><FormattedMessage id='No_history'/></p>
                            </WorkEmpty>
                        :
                        <>

                        {/* 근현대 */}
                        <BarWrap><span><FormattedMessage id='ModernArt' /></span><DownOutlined/></BarWrap>
                        <div>
                        {allList.filter(list => {
                            if (searchTerm == null) {
                                return list
                            } else if (list.lot_num.toString().includes(searchTerm) || list.a_name.toLowerCase().includes(searchTerm)) {
                                return list
                            }

                        }).map(list => { 
                            return (
                                list.t_seq === tSeq[0].t_seq &&
                                <>
                                <WorkListForm key={list} list={list} className={currentLotStat.lot_num === list.lot_num ? state : ''} id={(list.lot_num).toString()} currentLotStat={currentLotStat} />
                                </>
                            )
                        })}
                        </div>
                        {/* 고미술 */}
                        <BarWrap><span><FormattedMessage id='KoreanArt'/></span><DownOutlined/></BarWrap> 
                        <div>
                        {allList.filter(list => {
                            if (searchTerm == null) {
                                return list
                            } else if (list.lot_num.toString().includes(searchTerm) || list.a_name.toLowerCase().includes(searchTerm)) {
                                return list
                            }
                        }).map(list => { 
                            return (
                                list.t_seq === tSeq[1].t_seq &&
                                <>
                                <WorkListForm key={list} list={list} className={currentLotStat.lot_num === list.lot_num ? state : ''} id={(list.lot_num).toString()} currentLotStat={currentLotStat} />
                                </>
                            )
                        })}
                        </div>
                        </> 
                    }

                </div>
            </WorkListWrapper>
        </>
    )
}

export default WorkListContainer;