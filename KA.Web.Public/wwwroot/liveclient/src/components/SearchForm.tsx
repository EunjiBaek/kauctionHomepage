import React from "react";
import styled from 'styled-components';
import { Input } from 'antd';
import { useIntl } from "react-intl";
import palette from '../styles/palette';

const { Search } = Input;


const Wrapper = styled.div`
    width: 100%;
    box-sizing: border-box;
    padding: 50px 20px 0;
    background-color: #ffffff;


    @media only screen and (max-width: 1250px) {
        position: absolute;
        top: 0; left: 0;
        width: 100%;
    }
`;

const SearchInput = styled(Search)`
    width: 100%;
    height: 40px;
    font-size: 12px;


    .ant-input {
        font-size: 12px;
        background-color: #f1f1f1;
    }

    .ant-input-group-addon {
        border: none;
        > button {
            width: 40px;
            height: 100%;
            background-color: ${palette.gray[2]} !important;

            svg {
                fill: black;
            }
        }
    }

    .ant-input-wrapper {
        height: 100%;
    }
    .ant-input-affix-wrapper {
        background-color: #f1f1f1;
        height: 100%;
        border: none;
    }
`;


const SearchDiv = styled.input`
    width: 100%;
    height: 36px;
    background-color: ${palette.gray[1]};
    border: none;
    outline: none;
    font-size: 12px;
    border-radius: 8px;
    box-shadow: inset 0px 1px 3px rgba(0, 0, 0, 0.1);
    padding: 0 0.8em;
`;



interface Props {
    onSearchChange: (e: any) => void;
};

const SearchForm = ({onSearchChange}: Props) => {

    const { formatMessage } = useIntl(); 

    return (
        <Wrapper>
            {/* <SearchInput
                placeholder={formatMessage({id: 'Search_Input'})}
                onSearch={onSearch}
            /> */}

            <SearchDiv placeholder={formatMessage({id: 'Search_Input'})} onChange={onSearchChange} />

        </Wrapper>
    )
}

export default SearchForm