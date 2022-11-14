import React, { useCallback, useEffect, useState } from 'react';
import { UnorderedListOutlined, CloseOutlined } from '@ant-design/icons';
import { useLocation } from 'react-router-dom';
import queryString from 'query-string';
import { useDispatch, useSelector } from 'react-redux';
import styled from 'styled-components';
import Layout from '../components/common/Layout';
import palette from '../styles/palette';
import VideoContainer from '../containers/VideoContainer';
import WorkListContainer from '../containers/WorkListContainer';
import AucStateContainer from '../containers/AucStateContainer';
import NoticeContainer from '../containers/NoticeContainer';
import CurrentLotContainer from '../containers/CurrentLotContainer';
import BiddingInsertContainer from '../containers/BiddingInsertContainer';
import * as signalR from '@microsoft/signalr';
import { connection } from '../service/connection';
import { AucStateResType, bidsInfoResType, WorkListResType, currentBidClaimType } from '../type';
import {
  loadNoticeRequest,
  getScheduleRequest,
  bidsListClaimRequest,
  bidsListClaimRemoveSuccess,
} from '../actions/auction';
import BiddingSelectContainer from '../containers/BiddingSelectContainer';
import { RootState } from '../reducers';
import { useMediaQuery } from 'react-responsive';
import useScrollLock from 'react-use-scroll-lock';
import { toast, ToastContainer, Zoom } from 'react-toastify';
import { FormattedMessage, useIntl } from 'react-intl';
import 'react-toastify/dist/ReactToastify.min.css';

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

const StyledContainer = styled(ToastContainer)`
  .Toastify__close-button {
    display: none;
  }

  .Toastify__toast-icon {
    svg {
      fill: var(--toastify-color-light);
    }
  }

  .Toastify__toast--success {
    background-color: var(--toastify-color-success);

    .Toastify__toast-body {
      color: var(--toastify-color-light);
    }
  }

  .Toastify__toast--error {
    background-color: var(--toastify-color-error);
    .Toastify__toast-body {
      color: var(--toastify-color-light);
    }
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

  const errorNotify = (error: any) => {
    toast.error(error);
  };

  const [shouldLockBody, setShouldLockBody] = useState(false);
  useScrollLock(shouldLockBody);

  let location = useLocation();
  const [currentLotStat, setCurrentLotStat] = useState<HomeProps['currentLotStat']>();
  const [currentBidHst, setCurrentBidHst] = useState<HomeProps['currentBidHst']>();
  const [currentBidClaim, setCurrentBidClaim] = useState<HomeProps['currentBidClaim']>(null);
  // let currentBidClaim: HomeProps['currentBidClaim'] = [];
  const dispatch = useDispatch();
  const { workList, auc_kind, bidNoti, bidListClaimDone, bidsList } = useSelector((state: RootState) => state.auction);
  const [active, setActive] = useState(false);

  // 현재 진행중인 lot 번호
  const currentLotNum = currentLotStat?.lot_num;
  const currentLotInfo = workList?.find((v) => v.lot_num === currentLotNum);

  const query = queryString.parse(location.search);
  const auc_num = query.auc_num.toString();
  const returnUrl = `%2Flive%2Fmajor%2Ftest%2F${auc_num}`;
  const scheduleCnt = useSelector((state: RootState) => state.auction?.scheduleCnt);
  const auc_stat_cd = currentLotStat?.auc_stat_cd;
  const { formatMessage } = useIntl(); 


  useEffect(() => {

    connection.on('CurrentLotStat', function (message) {
      // console.log(message);
      //LiveFunc.fn_curr_lotinfo_complete(JSON.parse(message));
      var json = JSON.parse(message);
      if (!currentLotStat) {
        setCurrentLotStat(json.data.Table[0]);
      }

      if (currentBidClaim == null && json.data.Table1.length !== 0) {
        console.log(json.data.Table1)
        // currentBidClaim = json.data.Table1
        setCurrentBidClaim(json.data.Table1);
      }
    });

    connection.on('CurrentBidHst', function (message) {
      // console.log(JSON.parse(message));
      //LiveFunc.fn_curr_lotinfo_bidhst_complete(JSON.parse(message));
      setCurrentBidHst(JSON.parse(message).data.Table);
    });

    connection.on('CurrentUserInfo', function (message) {
      console.info('CurrentUserInfo', message);
      //toastr.error(message);
    });

    connection.on('ErrorNotification', function (message) {
      if (message === 'ERR_LOGOUT') {
        // eslint-disable-next-line no-restricted-globals
        window.location.href = `/Member/Login?returnUrl=${returnUrl}`;
      } else {
        console.error(message);
        //toastr.error(message);
      }
    });

    connection
      .start()
      .then(function () {
        try {
          connection.invoke('AddToGroup', query.auc_num);
        } catch (e: any) {
          console.error(e.message);
          //toastr.error(message);
        }
      })
      .catch(function (err) {
        console.error(err.toString());
        //toastr.error(err.toString());
        return;
      });

    connection.onreconnected(function (connectionId) {
      console.assert(connection.state === signalR.HubConnectionState.Connected);
      try {
        connection.invoke('AddToGroup', auc_num);
      } catch (e: any) {
        console.error(e.message);
      }
    });
  }, []);


  useEffect(() => {
    dispatch(
      loadNoticeRequest({
        auc_kind: auc_kind,
        auc_num: Number(auc_num),
      }),
    );
  }, [auc_num]);

  useEffect(() => {
    dispatch(
      getScheduleRequest({
        auc_kind: auc_kind,
        auc_num: Number(auc_num),
      }),
    );
  }, [auc_stat_cd, auc_num]);

  // useEffect(() => {
  //   if(bidListClaimDone === true) {
  //     // setCurrentBidClaim(null);
  //     dispatch(bidsListClaimRemoveSuccess());
  //   }
  // }, [bidListClaimDone]);

  useEffect(() => {
    if (currentBidClaim && currentBidClaim.length !== 0) {
      dispatch(
        bidsListClaimRequest({
          data: currentBidClaim,
        }),
      );
    }

    if (scheduleCnt?.my_paddle_num > 0 && bidListClaimDone) {
      currentBidClaim.forEach((el) => {
        let deletes = currentBidClaim.filter((e) => {
          return el.bid_stat_cd === 'DEL' && e.paddle_num === scheduleCnt.my_paddle_num;
        });
        if (deletes.length > 0) {
          deletes.filter((e) => {
            let toast_01 =
              e.bid_noti_memo.length > 0 ? e.bid_noti_memo : formatMessage({ id: 'NOTI_CHG_INCREASE_PRC' });

            errorNotify(toast_01);
          });
        }

        let cancels = currentBidClaim.filter((e) => {
          return el.bid_stat_cd === 'CNL' && e.paddle_num === scheduleCnt.my_paddle_num;
        });
        if (cancels.length > 0) {
          cancels.filter((e) => {
            let toast_02 = e.bid_noti_memo.length > 0 ? e.bid_noti_memo : formatMessage({ id: 'NOTI_CHG_MY_BID' });
            errorNotify(toast_02);
          });
        }
        
      });
    }
  }, [currentBidClaim, bidListClaimDone]);



  const modalOpenEvent = useCallback(() => {
    setActive((prev) => !prev);
    setShouldLockBody((prev) => !prev);
  }, [active]);

  return (
    <HomeWrap>
      {scheduleCnt && currentLotStat && (
        <>
          <StyledContainer
            position="bottom-center"
            autoClose={500}
            hideProgressBar={true}
            newestOnTop={false}
            closeOnClick={false}
            rtl={false}
            pauseOnFocusLoss={false}
            draggable={false}
            pauseOnHover={false}
            transition={Zoom}
          />
          {active && <DimWrap onClick={modalOpenEvent}></DimWrap>}
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
                  <WorkListContainer currentLotStat={currentLotStat} />
                </LeftWrapper>
              )}
              <div>
                <AucStateContainer />
                <VideoContainer />
                <CurrentLotContainer currentLotInfo={currentLotInfo} />
                <NoticeContainer />
              </div>
              <div>
                <BiddingSelectContainer currentBidHst={currentBidHst} currentLotStat={currentLotStat} />
                <BiddingInsertContainer currentLotStat={currentLotStat} />
              </div>
            </Wrapper>
          </Layout>
        </>
      )}
    </HomeWrap>
  );
};

export default Home;
