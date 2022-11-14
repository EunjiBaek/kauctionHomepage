var AucStatCd = {
}

var LotStatCd = {   
}

var BidTypeCd = {
    ONL: "" //온라인 텍스트 노출 안함.
    ,
    FLD: "Floor Bid"
    ,
    PRE: "Floor Bid" //서면
    ,
    TEL: "ARS" //전화  ARS
    ,
    MNG: "Floor Bid"
}

var BidStatCd = {
    BID: "Bid"
    ,
    CNL: "Cancelled"
    ,
    DEL: "Deleted"
}

var RsltCd = {
    ERR_LOT_NOTALLOW: "Lot Closed.",
    ERR_LOT_SUCCESSBID: "Lot Closed.",
    ERR_LOT_RESERVEDBID: "Reserve not met.<br/>Please check the amount and bid again.",
    ERR_LOT_LOWBID: "You have been outbidded.<br/>Please check the amount and bid again.",
    ERR_LOT_PREBID: "You have been outbidded.<br/>Please check the amount and bid again.",
    ERR_NOTPADDLE: "You have not paddle number.",
    ERR_LOT_BESTBIDPRC_SAMEASME: "You bid the best price",
    SUCC: "<p>Lot #{{lot_num}} Bid Submitted.</p><p>Bid Amount KRW {{bid_price}}</p><p>Received {{reg_date}}</p>",
    NOTI_CHG_INCREASE_PRC: "Asking price has been changed to {{bid_price}}. Please check the amount and bid again.",
    NOTI_CHG_MY_BID: "Asking price has been changed to {{bid_price}}. Please check the amount and bid again.",
    NOTI_CHG_SUCCBID_CANCEL: "Lot is reopened for bidding.",
}

var Btn = {
    OK: "Ok",
    CANCEL: "Cancel"
}

var EtcMsg = {
    NoBids: "There are no bids",
    FairWarning:"Fair Warning",
    Mdl_Currency_Terms_Msg1: "Please read",
    Mdl_Currency_Terms_Msg2: "The display of currencies other than Korean won (KRW) is for convenience of bidding only and all final bids and payment will be Korean Won (KRW). K Auction shall not be responsible for any error or omission, howsoever caused, arising from or in connection with, conversion of currencies.",
    Mdl_Currency_Terms_Msg3: "I have read and agree to the above",
    Mdl_Currency_Terms_Check: "Agree",
    Mdl_Currency_Terms_CheckBodx_Alert: "Please check the checkbox.",
    Mdl_Logout: "Do you want to logout?",
    Mdl_Logout_End: "Log out",
    Logout_href: "/Member/Logout",
    VideoWait: "<p>Please click the ▷ button to watch the live stream.</p><p>If you are unable to hear audio, please adjust your speaker volume.</p>",
    VideoPause: "The auction will begin shortly. Thank you for waiting.",
    VideoFinish: "The auction has ended. Thank you."
}