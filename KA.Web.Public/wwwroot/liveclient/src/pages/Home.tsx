/* eslint-disable array-callback-return */
import React, { useCallback, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { UnorderedListOutlined, CloseOutlined } from '@ant-design/icons';
import { useLocation } from 'react-router-dom';
import queryString from 'query-string';
import { useDispatch, useSelector } from 'react-redux';
import styled from 'styled-components';
import useScrollLock from 'react-use-scroll-lock';
import { useMediaQuery } from 'react-responsive';
import { FormattedMessage, useIntl } from 'react-intl';
import * as signalR from '@microsoft/signalr';
import Layout from '../components/common/Layout';
import palette from '../styles/palette';
import ToastComponent from '../components/common/ToastComponent';
import VideoContainer from '../containers/VideoContainer';
import WorkListContainer from '../containers/WorkListContainer';
import AucStateContainer from '../containers/AucStateContainer';
import NoticeContainer from '../containers/NoticeContainer';
import CurrentLotContainer from '../containers/CurrentLotContainer';
import BiddingInsertContainer from '../containers/BiddingInsertContainer';
import { connection } from '../service/connection';
import { AucStateResType, bidsInfoResType, WorkListResType, currentBidClaimType } from '../type';
import {
  loadNoticeRequest,
  getCurrentLotRequest,
  getScheduleRequest,
  bidsListClaimRequest,
} from '../actions/auction';
import BiddingSelectContainer from '../containers/BiddingSelectContainer';
import { RootState } from '../reducers';
import 'react-toastify/dist/ReactToastify.min.css';


// 스타일 작업
const Wrapper = styled.div`
  height: 100%;
  display: grid;
  max-width: 1920px;
  grid-template-columns: 1fr 600px 1fr;
  column-gap: 20px;
  margin: 30px auto;

  @media only screen and (max-width: 1250px) {
    grid-template-columns: 600px 1fr;
  }
`;


// const BodyWrap = styled.div`
//   width: 100%;
//   height: 100vh;
//   background: url(../assets/img/bg_main.png) no-repeat center center;
//   background-size: cover;
// `;


const LeftWrapper = styled.div`
  position: relative;
  border: 1px solid ${palette.gray[2]};
  border-radius: 3px;
  box-sizing: border-box;
  overflow: hidden;
`;

const ListOpenBtn = styled.div`
  position: fixed;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  background-color: #ffffff;
  box-shadow: 3px 4px 20px rgba(0, 0, 0, 0.25);
  border-radius: 0px 11px 11px 0px;
  z-index: 50;
  width: 65px;
  height: 56px;
  display: none;
  align-items: center;
  justify-content: center;
  cursor: pointer;

  @media only screen and (max-width: 1250px) {
    display: flex;
  }
`;

const ListIcon = styled(UnorderedListOutlined)`
  svg {
    width: 1.4em;
    height: 1.4em;
  }
`;

const TabletList = styled.div`
  position: fixed;
  top: 0;
  left: -350px;
  width: 350px;
  height: 100%;
  padding-top: 90px;
  background-color: #ffffff;
  z-index: 500;
  transition: all 0.2s ease;

  &.active {
    left: 0;
    transition: all 0.2s ease;
  }
  .anticon.anticon-close {
    position: absolute;
    top: 10px;
    right: 10px;
    z-index: 505;
    cursor: pointer;

    svg {
      width: 1.4em;
      height: 1.1em;
      fill: ${palette.gray[4]};
    }
  }
`;

const DimWrap = styled.div`
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.45);
  position: fixed;
  top: 0;
  left: 0;
  z-index: 400;
`;

// HomeWrap
const HomeWrap = styled.div`
  @media only screen and (max-width: 1000px) {
    width: 1000px;
    margin: 0 auto;
    box-sizing: border-box;
  }
`;

interface HomeProps {
  currentLotStat: AucStateResType;
  isLoading: boolean;
  currentBidHst: bidsInfoResType[];
  currentLotInfo: WorkListResType;
  currentBidClaim: currentBidClaimType[];
}

const Home = () => {
  // 반응형
  const isPc = useMediaQuery({
    query: '(min-width:1251px)',
  });
  const isTablet = useMediaQuery({
    // query : "(min-width:769px) and (max-width:1250px)"
    query: '(max-width:1250px)',
  });


  const [shouldLockBody, setShouldLockBody] = useState(false);
  useScrollLock(shouldLockBody);
  const location = useLocation();
  const [currentLotStat, setCurrentLotStat] = useState<HomeProps['currentLotStat']>();
  const [currentBidHst, setCurrentBidHst] = useState<HomeProps['currentBidHst']>();
  const [currentBidClaim, setCurrentBidClaim] = useState<HomeProps['currentBidClaim']>();
  const dispatch = useDispatch();
  const { workList, auc_kind, biddingInsertError, biddingInsertDone, biddingInsert, currentLot, bidListClaimDone, bidNoti, bidListClaimLoading } = useSelector((state: RootState) => state.auction);
  const [active, setActive] = useState(false);
  const { formatMessage } = useIntl();

  // 현재 진행중인 lot 번호
  const currentLotInfo = workList?.find((v) => v.lot_num === currentLot[0]?.lot_num);
  const query = queryString.parse(location.search);
  const auc_num = query.auc_num.toString();
  const returnUrl = `%2Flive%2Fmajor%2Ftest%2F${auc_num}`;
  const scheduleCnt = useSelector((state: RootState) => state.auction?.scheduleCnt);
  const auc_stat_cd = currentLotStat?.auc_stat_cd;

  useEffect(() => {
    // 진행중인 랏에 대한 정보
    connection.on('CurrentLotStat', (message) => {
      // LiveFunc.fn_curr_lotinfo_complete(JSON.parse(message));
      const json = JSON.parse(message);
      if (!currentLotStat) {
        setCurrentLotStat(json.data.Table[0]);
      }
      if (json.data.Table1[0] !== undefined && !currentBidClaim) {
        setCurrentBidClaim(json.data.Table1);
      }
    });

    connection.on('CurrentBidHst', (message) => {
      // LiveFunc.fn_curr_lotinfo_bidhst_complete(JSON.parse(message));
      setCurrentBidHst(JSON.parse(message).data.Table);
    });

    connection.on('CurrentUserInfo', (message) => {
      console.info('CurrentUserInfo', message);
      // toastr.error(message);
    });

    connection.on('ErrorNotification', (message) => {
      if (message === 'ERR_LOGOUT') {
        // eslint-disable-next-line no-restricted-globals, no-undef
        window.location.href = `/Member/Login?returnUrl=${returnUrl}`;
      } else {
        console.error(message);
        // toastr.error(message);
      }
    });

    connection
      .start()
      .then(() => {
        try {
          connection.invoke('AddToGroup', query.auc_num);
        } catch (e: any) {
          console.error(e.message);
          // toastr.error(message);
        }
      })
      .catch((err) => {
        console.error(err.toString());
        // toastr.error(err.toString());
        // return;
      });

    connection.onreconnected((connectionId) => {
      console.assert(connection.state === signalR.HubConnectionState.Connected);
      try {
        connection.invoke('AddToGroup', auc_num);
      } catch (e: any) {
        console.error(e.message);
      }
    });
  }, []);

  const errorNotify = (error: any) => {
    toast.error(error);
  };

  const succNotify = (msg: any) => {
    toast.success(msg);
  };

  /**
   * 응찰에러시 토스트 메세지
   */
  useEffect(() => {
    if (biddingInsertError) {
      switch (biddingInsertError) {
        case "ERR_LOT_NOTALLOW":
          errorNotify(<FormattedMessage id="ERR_LOT_NOTALLOW" />);
          break
        case "ERR_LOT_SUCCESSBID":
          errorNotify(<FormattedMessage id="ERR_LOT_SUCCESSBID" />);
          break
        case "ERR_LOT_RESERVEDBID":
          errorNotify(<FormattedMessage id="ERR_LOT_RESERVEDBID" />);
          break
        case "ERR_LOT_LOWBID":
          errorNotify(<FormattedMessage id="ERR_LOT_LOWBID" values={{ bid_price: currentLotStat.bid_price }} />);
          break
        case "ERR_LOT_PREBID":
          errorNotify(<FormattedMessage id="ERR_LOT_PREBID" values={{ bid_price: currentLotStat.bid_price }} />);
          break
        case "ERR_NOTPADDLE":
          errorNotify(<FormattedMessage id="ERR_NOTPADDLE" />);
          break
        case "ERR_LOT_BESTBIDPRC_SAMEASME":
          errorNotify(<FormattedMessage id="ERR_LOT_BESTBIDPRC_SAMEASME" />);
          break
        default:
          errorNotify(biddingInsertError);
          break
      }
    }
  }, [biddingInsertError]);


 // 응찰 성공시 토스트 메세지
  useEffect(() => {
    if (biddingInsertDone) {
      succNotify(
        <div>
          <p>
            <FormattedMessage id="SUCC_LOT_NUM" values={{ lot_num: biddingInsert.lot_num }} />
          </p>
          <p>
            <FormattedMessage id="SUCC_BID_PRICE" values={{ bid_price: biddingInsert.bid_price }} />
          </p>
          <p>
            <FormattedMessage id="SUCC_REG_DATE" values={{ reg_date: biddingInsert.reg_date }} />
          </p>
        </div>,
      );
    }
  }, [biddingInsertDone]);



  useEffect(() => {
    dispatch(
      loadNoticeRequest({
        auc_kind,
        auc_num: Number(auc_num),
      }),
    );
  }, [auc_num]);

  useEffect(() => {
    dispatch(
      getScheduleRequest({
        auc_kind,
        auc_num: Number(auc_num),
      }),
    );
  }, [auc_stat_cd, auc_num]);


  useEffect(() => {
      if (currentLot !== null && currentLotStat !== undefined && currentLot[0]?.lot_num !== currentLotStat.lot_num) {
        dispatch(getCurrentLotRequest(currentLotStat));
      }  
    
  }, [currentLot, currentLotStat?.lot_num]);




  useEffect(() => {
    const idx = currentBidClaim?.findIndex((element, index, array) => element.bid_hst_seq === currentLot[0].claim_bid_hst_seq);

    if (currentBidClaim && idx === -1) {
      dispatch(
        bidsListClaimRequest({
          data: currentBidClaim,
        }),
      );
    }

  }, [currentBidClaim]);



  // 클레임토스트메세지
  useEffect(() => {
    if (scheduleCnt?.my_paddle_num > 0 && bidListClaimDone && !bidListClaimLoading) {
      bidNoti.forEach(el => {
        const deletes = bidNoti.filter((e) => {
          return el.bid_stat_cd === 'DEL' && e.paddle_num === scheduleCnt.my_paddle_num;
        });
        

        if (deletes.length > 0) {
          deletes.filter((e) => {
            const toast_01 =
              e.bid_noti_memo.length > 0 ? e.bid_noti_memo : formatMessage({ id: 'NOTI_CHG_INCREASE_PRC' });
            errorNotify(toast_01);
          });
        }

        const cancels = bidNoti.filter((e) => {
          return el.bid_stat_cd === 'CNL' && e.paddle_num === scheduleCnt.my_paddle_num;
        });
        if (cancels.length > 0) {
          cancels.filter((e) => {
            const toast_02 = e.bid_noti_memo.length > 0 ? e.bid_noti_memo : formatMessage({ id: 'NOTI_CHG_MY_BID' });
            errorNotify(toast_02);
          });
        }
      })
    }

  }, [bidListClaimDone, currentLot]);



  const modalOpenEvent = useCallback(() => {
    setActive((prev) => !prev);
    setShouldLockBody((prev) => !prev);
  }, [active]);



  return (

      <HomeWrap>
        <ToastComponent />

        {scheduleCnt && currentLotStat && (
          <>
            {active && <DimWrap onClick={modalOpenEvent} />}
            <Layout>
              <ListOpenBtn onClick={modalOpenEvent}>
                <ListIcon />
              </ListOpenBtn>
              {isTablet && (
                <TabletList className={active ? 'active' : ''}>
                  <CloseOutlined onClick={modalOpenEvent} />
                  <WorkListContainer currentLotStat={currentLotStat} />
                </TabletList>
              )}

              <Wrapper className="row">
                {isPc && (
                  <LeftWrapper>
                    <WorkListContainer 
                      currentLotStat={currentLotStat} 
                    />
                  </LeftWrapper>
                )}
                <div>
                  <AucStateContainer />
                  <VideoContainer />
                  <CurrentLotContainer 
                    currentLotInfo={currentLotInfo} 
                  />
                  <NoticeContainer />
                </div>
                <div>
                  <BiddingSelectContainer 
                    currentBidHst={currentBidHst} 
                    currentLotStat={currentLotStat} 
                    currentLotInfo={currentLotInfo}
                  />
                  <BiddingInsertContainer 
                    currentLotStat={currentLotStat} 
                  />
                </div>
              </Wrapper>
            </Layout>
          </>
        )}
      </HomeWrap>

  );
};

export default Home;
