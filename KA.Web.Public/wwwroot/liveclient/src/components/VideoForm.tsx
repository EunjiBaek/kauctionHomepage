import React, { useCallback, useRef, useState } from 'react';
import styled from 'styled-components';
import { PlayCircleFilled } from '@ant-design/icons';
import { FormattedMessage } from "react-intl";
import useScript from '../hooks/useInput';
// import '../utils/unreal_html5_player_script_v1.css';
import { UnrealWebRTCPlayer } from '../utils/unrealwebrtcplayer.js';

const Wrapper = styled.div`
    position: relative;
    width: 100%;
    height: 336px;
    margin: 12px 0;

    .react-player__shadow {
        border: 1px solid #ffffff;
    }

    > video {
        width: 100%;
        height: 336px;
    }
`;

const StartScreen = styled.div`
    position: absolute;
    top: 0; left: 0;
    width: 100%; height: 100%;
    background-size : cover;
    background-position : center;
    display: none;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    z-index: 5;

    > div {
        cursor: pointer;
    }

    &.on {
        display: flex;
    }

    span {
        font-size: 35px;
        color: #ffffff;
    }

    > p {
        color: #ffffff;

        &.desc01 {
            margin-top: 10px;
        }
    }
`;



const VideoForm = () => {

    const status = useScript(`/utils/UnrealWebRTCPlayer.js`);
    const domRef = useRef(null);
    const [state, setState] = useState(true);


    const videoOnload = useCallback(() => {
        setState(false);

        const el2 = domRef.current.id;
        // RunPlayer(el2, "600", "320", "stream.camtour.net", 448, true, "kalive", "", true, true, -1, "", false);
        const webrtcPlayer = new UnrealWebRTCPlayer(el2, "kaopus", "", "stream.camtour.net", "448", true, true, "tcp");
        webrtcPlayer.Play();

    }, [domRef]);


  
    return (
        <Wrapper>
            <StartScreen className={state === true ? "on" : ""} style={{backgroundImage: 'url(./stopimg.jpg)'}}>
                <div onClick={videoOnload}>
                    <PlayCircleFilled />
                </div>
                <p className="desc01"><FormattedMessage id='Video_desc_01'/></p>
                <p className="desc02"><FormattedMessage id='Video_desc_02'/></p>
            </StartScreen>
            <video id="RtcPlayer" style={{width: "600", height: "336", backgroundColor: "black"}} ref={domRef} autoPlay playsInline controls />
        </Wrapper>
    )
}

export default VideoForm;