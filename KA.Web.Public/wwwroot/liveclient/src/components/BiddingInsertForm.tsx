import React, { useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import styled from 'styled-components';
import { FormattedMessage } from 'react-intl';
import palette from '../styles/palette';
import { priceChange } from '../utils/priceChange';
import { AucStateResType } from '../type';
import { RootState } from '../reducers';
import { biddingInsertRequest } from '../actions/auction';
import 'react-toastify/dist/ReactToastify.min.css';

const Wrapper = styled.div`
  height: 172px;
  border: 1px solid ${palette.gray[2]};
  background-color: #fff;
  border-radius: 5px;
  padding: 0 20px;
  box-sizing: border-box;
  display: flex;
  align-items: center;
  flex-direction: column;
  justify-content: center;

  .current-price-wrap {
    width: 100%;
    margin-bottom: 5px;
    > div {
      width: 100%;
      line-height: 1.2em;
      display: flex;
      align-items: center;
      justify-content: center;

      > span {
        display: inline-block;
        line-height: 1.2em;
        font-weight: 700;
        font-size: 18px;
        margin-right: 10px;
      }

      > strong {
        font-size: 18px;
        line-height: 1.2em;

        &.success {
          color: ${palette.orange[7]};
        }
      }
    }

    > p {
      text-align: center;
      font-size: 18px;
      line-height: 1.2em;
    }
  }

  > .bidding-btn-wrap {
    width: 100%;
    height: 90px;
    background-color: ${palette.gray[9]};
    border-radius: 5px;
    box-sizing: border-box;
    color: #fff;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    padding: 10px;

    &.pre-bid {
      > span {
        font-size: 16px;
      }
    }

    &.preparing,
    &.processing {
      background-color: ${palette.gray[5]};
      > span {
        font-size: 14px;
        font-weight: 700;
        text-align: center;
      }
    }

    &.success {
      background-color: #fff;
      color: ${palette.orange[5]};
      border: 2px solid ${palette.orange[5]};
      cursor: unset;

      span {
        font-size: 22px;
        font-weight: 700;
        color: ${palette.orange[5]};
      }
    }

    > .price {
      font-size: 18px;
      line-height: 1.2em;
      font-weight: 500;
    }

    > .currency_price {
      font-size: 16px;
      line-height: 1.2em;
    }

    span {
      margin-top: 5px;
      font-size: 22px;
      line-height: 1.2em;
      font-weight: 500;
    }
  }
`;


interface Props {
  currentLotStat: AucStateResType;
  my_paddle_num: number;
  error: String | null;
  done: boolean;
}

const BiddingInsertForm = ({ currentLotStat, my_paddle_num, error, done }: Props) => {
  const dispatch = useDispatch();
  const {
    lot_num,
    bid_price,
    next_bid_price,
    usd_next_bid_price,
    jpy_next_bid_price,
    cny_next_bid_price,
    hkd_next_bid_price,
    eur_next_bid_price,
    lot_stat_cd,
    successful_bid,
    successful_bid_price,
    pre_bid_proc_yn,
    successful_paddle_num,
    usd_bid_price,
    jpy_bid_price,
    cny_bid_price,
    hkd_bid_price,
    eur_bid_price,
  } = currentLotStat;

  const { auc_kind, biddingInsert, currency } = useSelector(
    (state: RootState) => state.auction,
  );


  const onBidInsert = useCallback(() => {
    dispatch(
      biddingInsertRequest({
        auc_kind,
        auc_num: 139,
        lot_num,
        paddle_num: my_paddle_num,
        bid_price: next_bid_price,
      }),
    );
  }, [lot_num, my_paddle_num, next_bid_price]);

  return (
    <>
      <Wrapper>
        {lot_stat_cd === 'F' && successful_bid === false ? (
          <div className="current-price-wrap">
            <div>
              <span>낙찰가</span> <strong className="success">KRW {priceChange(successful_bid_price)}</strong>
            </div>
          </div>
        ) : (
          <div className="current-price-wrap">
            <div>
              <span>
                <FormattedMessage id="CurrentBid" />
              </span>{' '}
              <strong>KRW {priceChange(bid_price)}</strong>
            </div>
            {currency === 'USD' ? (
              <p>(USD {priceChange(usd_bid_price)})</p>
            ) : currency === 'JPY' ? (
              <p>(JPY {priceChange(jpy_bid_price)})</p>
            ) : currency === 'CNY' ? (
              <p>(CNY {priceChange(cny_bid_price)})</p>
            ) : currency === 'HKD' ? (
              <p>(HKD {priceChange(hkd_bid_price)})</p>
            ) : currency === 'EUR' ? (
              <p>(EUR {priceChange(eur_bid_price)})</p>
            ) : null}
          </div>
        )}
        <>
          {lot_stat_cd === 'F' ? (
            <div className="bidding-btn-wrap success">
              <span>
                낙찰
                {successful_paddle_num !== 0 ? `#${  successful_paddle_num}` : null}
              </span>
            </div>
          ) : lot_stat_cd === 'W' ? (
            <div className="bidding-btn-wrap preparing">
              <span>
                <FormattedMessage id="Preparing_Lot" />
              </span>
            </div>
          ) : lot_stat_cd === 'S' && pre_bid_proc_yn === false ? (
            <div className="bidding-btn-wrap pre-bid">
              <span>
                <FormattedMessage id="Absentee_bidding_Lot" />
              </span>
            </div>
          ) : lot_stat_cd === 'S' && my_paddle_num !== undefined ? (
            <div role="presentation" className="bidding-btn-wrap" onClick={onBidInsert}>
              <p className="price">KRW {priceChange(next_bid_price)}</p>

              {currency === 'USD' ? (
                <p className="currency_price">(USD {priceChange(usd_next_bid_price)})</p>
              ) : currency === 'JPY' ? (
                <p className="currency_price">(JPY {priceChange(jpy_next_bid_price)})</p>
              ) : currency === 'CNY' ? (
                <p className="currency_price">(CNY {priceChange(cny_next_bid_price)})</p>
              ) : currency === 'HKD' ? (
                <p className="currency_price">(HKD {priceChange(hkd_next_bid_price)})</p>
              ) : currency === 'EUR' ? (
                <p className="currency_price">(EUR {priceChange(eur_next_bid_price)})</p>
              ) : null}
              <span>
                <FormattedMessage id="PlaceBid" />
              </span>
            </div>
          ) : lot_stat_cd === 'N' ? (
            <div className="bidding-btn-wrap processing">
              <span>
                <FormattedMessage id="Confirming_Lot" />
              </span>
            </div>
          ) : lot_stat_cd === 'P' ? (
            <div className="bidding-btn-wrap processing">
              <span>조정중입니다. 잠시만 기다려주세요.</span>
            </div>
          ) : null}
        </>
      </Wrapper>
    </>
  );
};

export default BiddingInsertForm;
