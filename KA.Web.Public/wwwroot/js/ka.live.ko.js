var AucStatCd = {};

var LotStatCd = {};

var BidTypeCd = {
    ONL: '', //온라인 텍스트 노출 안함.
    FLD: '현장응찰',
    PRE: '현장응찰', //서면
    TEL: 'ARS', //전화  ARS
    MNG: '현장응찰',
};

var BidStatCd = {
    BID: '응찰',
    CNL: '응찰취소',
    DEL: '응찰삭제',
};

var RsltCd = {
    ERR_LOT_NOTALLOW: '응찰 제한된 LOT 입니다.',
    ERR_LOT_SUCCESSBID: '낙찰된 LOT 입니다.',
    ERR_LOT_RESERVEDBID: '내정가 이하입니다.<br/>응찰가 확인 후 다시 응찰 바랍니다.',
    ERR_LOT_LOWBID: '다른 응찰자가 이미 KRW {{bid_price}}에 응찰하였습니다.<br/>응찰가 확인 후 다시 응찰 바랍니다.',
    ERR_LOT_PREBID: '다른 응찰자가 이미 KRW {{bid_price}}에 응찰하였습니다.<br/>응찰가 확인 후 다시 응찰 바랍니다.',
    ERR_NOTPADDLE: '패들이 없습니다.',
    ERR_LOT_BESTBIDPRC_SAMEASME: '이미 최고가 응찰 하셨습니다.',
    SUCC: '<p>Lot #{{lot_num}} 응찰완료.</p><p>응찰가 KRW {{bid_price}}</p><p>접수시간 {{reg_date}}</p>',
    NOTI_CHG_INCREASE_PRC: '응찰가가 조정되었습니다. 응찰가 확인 후 다시 응찰 바랍니다. ',
    NOTI_CHG_MY_BID: '다른 응찰자기 이미 KRW {{bid_price}}에 응찰하였습니다. 응찰가 확인 후 다시 응찰 바랍니다. ',
    NOTI_CHG_SUCCBID_CANCEL: '응찰이 재개되었습니다.',
};

var Btn = {
    OK: '확인',
    CANCEL: '취소',
};

var EtcMsg = {
    NoBids: '응찰 내역이 없습니다.',
    FairWarning: '곧 응찰을 마감합니다',
    Mdl_Currency_Terms_Msg1: '읽어주세요',
    Mdl_Currency_Terms_Msg2:
        '한화(KRW)가 아닌 다른 통화의 표시는 응찰의 편의를 돕기 위한 것이며, 최종적인 금액은 낙찰대금으로서 케이옥션이 최종적으로 지급받는 한화(KRW)를  따릅니다. 케이옥션은 낙찰자가 낙찰시로부터 낙찰 대금 입금 시까지 적용되는 외국환 환율 차이에서 발생하는 어떤 차이 또는 손해에 대해서도 책임을 부담하지 않습니다.',
    Mdl_Currency_Terms_Msg3: '위 내용에 동의합니다.',
    Mdl_Currency_Terms_Check: '동의',
    Mdl_Currency_Terms_CheckBodx_Alert: '체크박스를 체크해주세요.',
    Mdl_Logout: '로그아웃 하시겠습니까?',
    Mdl_Logout_End: 'Log out',
    Logout_href: '/Member/Logout',
    VideoWait: '<p>경매 실황을 시청하시려면 시작 버튼(▷)을 눌러주세요.</p><p>소리가 들리지 않거나 작은 경우, 플레이어의 음량을 확인 후 조절해 주세요.</p>',
    VideoPause: '경매가 곧 시작됩니다. 잠시만 기다려주세요.',
    VideoFinish: '경매가 종료되었습니다. 감사합니다.',
};
