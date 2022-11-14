import React, { useCallback, useState } from 'react';
import { useLocation } from 'react-router-dom';
import queryString from 'query-string';
import { useDispatch, useSelector } from 'react-redux';
import styled, { createGlobalStyle } from 'styled-components';
import { Link } from 'react-router-dom';
import { Select, Modal } from 'antd';
import { FormattedMessage, useIntl } from 'react-intl';
import palette from '../../styles/palette';
import { RootState } from '../../reducers';
import { currencyRequest } from '../../actions/auction';

const { Option } = Select;

const Wrapper = styled.div`
  position: relative;
  height: 60px;
  border: 1px solid ${palette.gray[1]};
  background-color: #fff;

  > div {
    height: 100%;
    max-width: 1920px;
    margin: 0 auto;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }
`;

const LogoWrap = styled.div`
  display: flex;
  align-items: center;

  > a {
    font-size: 0;
  }

  > span {
    font-size: 16px;
    font-weight: 600;
    margin-left: 10px;
  }
`;

const UtilMenuWrap = styled.div`
  display: flex;
  align-items: center;
  font-size: 12px;
  color: ${palette.gray[6]};

  > a {
    margin-right: 15px;
  }

  > div.paddle_no_wrap {
    margin-right: 15px;

    > strong {
      font-weight: 400;
      color: ${palette.orange[7]};
    }
  }

  > button {
    padding: 0 12px;
    height: 22px;
    color: ${palette.gray[6]};
    font-size: 12px;
    border: 1px solid ${palette.gray[3]};
    border-radius: 4px;
  }
`;

const ModalWrapper = styled(Modal)`
  .ant-modal {
    max-width: 350px;
  }
  .ant-modal-close {
    display: none;
  }
  .ant-modal-header {
    text-align: center;
  }
  .ant-modal-title {
    font-weight: 600;
  }
  .ant-modal-body {
    height: auto !important;
    display: flex;
    flex-direction: column;
    align-items: center;

    > p {
      font-size: 15px;
      margin-bottom: 20px;
      font-weight: 500;
    }
  }
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
  .btn_wrap {
    width: 100%;
    display: flex;
    align-items: flex-start;
    gap: 0 1%;

    > button {
      width: 49.5%;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 5px;
      color: #ffffff;

      &.cancel {
        background-color: ${palette.gray[7]};
      }

      &.confirm {
        background-color: ${palette.orange[5]};
      }
    }
  }
`;

const Global = createGlobalStyle`
    .ant-dropdown-menu {
        padding: 0;
    }
    .ant-dropdown-menu-title-content {
        line-height: 1em;
    }
    .ant-dropdown-menu-title-content > a {
        font-size: 12px;
    }
    .ant-dropdown-trigger {
        position:relative;
        width: 50px;
    }
    .ant-space > div {
        acolor: ${palette.gray[6]};
    }
    .ant-space > div:last-child {
        position: absolute;
        top: 50%; right:0px;
        transform: translateY(-50%);
    }

    .ant-select:not(.ant-select-customize-input) .ant-select-selector {
        height: auto;
        padding: 0;
    }
    .ant-select-selection-item {
        padding-right: 0 !important;
        font-size: 12px;
    }
    .ant-select.ant-select-borderless.ant-select-single.ant-select-show-arrow{
        margin-right: 15px;
    }
    .ant-select-arrow {
        right: 0;
    }

    .ant-select-item {
        padding: 5px 6px;
        font-size: 12px;
    }
    .ant-select-dropdown {
        padding: 0;
    }
`;

interface Props {
  langOptions: { opt_name: string; value: string }[];
  setCurrency: string;
}

const Header = () => {
  const location = useLocation();
  const { formatMessage } = useIntl();
  const query = queryString.parse(location.search);
  const auc_num = query.auc_num.toString();
  const lang_cd = query.lang_cd.toString();
  const returnUrl = `%2Flive%2Fmajor%2Ftest%2F${auc_num}`;
  const dispatch = useDispatch();

  const [value, setValue] = useState(lang_cd);
  // const [currency, setCurrency] = useState('KRW');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const scheduleCnt = useSelector((state: RootState) => state.auction.scheduleCnt);

  const langOptions: Props['langOptions'] | undefined = [
    {
      opt_name: 'KOR',
      value: 'kr',
    },
    {
      opt_name: 'ENG',
      value: 'en',
    },
  ];

  const handleChange = (value: string) => {
    console.log(`selected ${value}`);
    const langOption = langOptions.find((o) => o.value === value)?.opt_name;
    setValue(value);
    window.location.href = `/Home/SetLanguage?culture=${langOption}&returnUrl=${returnUrl}`;
  };

  const currencyChange = useCallback(
    (value: string) => {
      dispatch(currencyRequest(value));
    },
    [value],
  );

  const showModal = useCallback(() => {
    setIsModalOpen(true);
  }, []);

  const handleOk = useCallback(() => {
    setIsModalOpen(false);
    window.location.href = formatMessage({ id: 'EtcMsg.Logout_href' });
    alert(formatMessage({ id: 'EtcMsg.Mdl_Logout_End' }));
  }, []);

  const handleCancel = useCallback(() => {
    setIsModalOpen(false);
  }, []);

  return (
    <>
      <Global />
      <ModalWrapper
        title={formatMessage({ id: 'Popup_Notice' })}
        visible={isModalOpen}
        onOk={handleOk}
        onCancel={handleCancel}
        centered
        width={350}
      >
        <p>
          <FormattedMessage id="EtcMsg.Mdl_Logout" />
        </p>
        <div className="btn_wrap">
          <button onClick={handleCancel} className="cancel">
            <FormattedMessage id="Btn.CANCEL" />
          </button>
          <button onClick={handleOk} className="confirm">
            <FormattedMessage id="Btn.OK" />
          </button>
        </div>
      </ModalWrapper>

      <Wrapper>
        <div className="row">
          <LogoWrap>
            <Link to="/">
              <img src={require('../../assets/img/logo.png')} alt="" />
            </Link>
            <span>{scheduleCnt?.auc_title}</span>
          </LogoWrap>

          <UtilMenuWrap>
            <div className="paddle_no_wrap">
              <span>
                <FormattedMessage id="MyPaddleNum" />
              </span>
              <strong> # {scheduleCnt?.my_paddle_num}</strong>
            </div>

            <Select defaultValue={value} style={{ width: 40 }} bordered={false} onChange={handleChange}>
              <Option value="kr">KOR</Option>
              <Option value="en">ENG</Option>
            </Select>

            <Select defaultValue="KRW" style={{ width: 40 }} bordered={false} onChange={currencyChange}>
              <Option value="KRW">KRW</Option>
              <Option value="USD">USD</Option>
              <Option value="JPY">JPY</Option>
              <Option value="HKD">HKD</Option>
              <Option value="CNY">CNY</Option>
              <Option value="EUR">EUR</Option>
            </Select>

            <button onClick={showModal}>
              <FormattedMessage id="LOG_OUT" />
            </button>
          </UtilMenuWrap>
        </div>
      </Wrapper>
    </>
  );
};

export default Header;
