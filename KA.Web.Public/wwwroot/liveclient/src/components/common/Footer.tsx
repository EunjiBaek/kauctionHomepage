import React from "react";
import styled from 'styled-components';
import palette from '../../styles/palette';

const Wrapper = styled.div`
    min-height: 100px;
    display: flex;
    align-items: center;
    justify-content: center;

    > span {
        color: ${palette.gray[6]};
    }
`;



const Footer = () => {
    return (
        <Wrapper className="row">
            <span>Copyright ⓒ K Auction. All Rights Reserved</span>
        </Wrapper>
    )
}

export default Footer;