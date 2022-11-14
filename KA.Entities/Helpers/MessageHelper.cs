using System;
using System.Globalization;

public static class MessageHelper
{
    public static string Get(string code, bool isKor = true)
    {
        var message = string.Empty;
        switch (code)
        {
            case "00": // 정상
                message = "ka.msg.common.ok";
                break;

            #region # Auction / Online Auction #

            case "11":
                message = isKor ? "시작가 이하로 응찰이 불가능합니다." : "You cannot bid below the starting price.";
                break;
            case "12":
                message = isKor ? "경매 시작 전입니다." : "It's before the auction starts.";
                break;
            case "13":
                message = isKor ? "경매가 종료 되었습니다." : "The auction has ended.";
                break;
            case "14":
                // message = isKor ? "현재 최고 응찰자 입니다." : "You are currently the highest bidder.";
                message = isKor ? "회원님은 현재 최고가 응찰중이므로, 상위응찰이 접수될때까지 응찰이 제한됩니다." 
                    : "Since the current highest bidder is a member, your bids will be limited until another bidder submits a higher bid.";
                break;
            case "15":
                message = isKor ? "현재가 이하로 응찰할 수 없습니다." : "You cannot bid below the current price.";
                break;
            case "16":
                // message = isKor ? "현재 최고 예약 응찰자이십니다." : "You are currently the highest bidder.";
                message = isKor ? "이미 응찰하신 가격 이하의 응찰입니다." : "It's a bid below the price you've already bid.";
                break;
            case "17":
                message = isKor ? "만 19세 미만은 해당 작품에 응찰하실 수 없습니다." : "";
                break;
            case "18":
                message = isKor ? "해당 작품은 '임직원 등 매수금지 작품'으로 임직원 및 임직원 이해관계인의 응찰이 금지되어 있습니다. <br /><br />* 임직원 이해관계인 : 임직원/배우자 포함 4촌 이내, 기타 경제적 이익이 연관된 사람으로서 해당 임직원이 케이옥션에 등록한 분." : "";
                break;
            case "101":
                message = isKor ? "아직 온라인라이브응찰 신청 접수가 시작되기 전입니다.<br />담당자에게 문의 부탁 드립니다." 
                    : "Register for online live-bidding has not yet opened.<br />Please contact a K Auction specialist or our office (82-2-3479-8888).";
                break;
            case "102":
                message = isKor ? "온라인라이브응찰 신청 접수가 종료되었습니다.<br />담당자{MANAGER}에게 문의 부탁 드립니다."
                    : "Register for online live-bidding has closed.<br />Please contact a K Auction specialist or our office (82-2-3479-8888).";
                break;
            case "103":
                message = isKor ? "회원님은 이미 온라인라이브응찰 신청을 진행 하셨습니다.<br />담당자{MANAGER}에게 문의 부탁 드립니다."
                    : "You have already submitted your registration for online live-bidding.<br />Please contact a K Auction specialist or our office (82-2-3479-8888).";
                break;
            case "104":
                message = isKor ? "응찰 신청이 접수되었습니다.<br />당사의 검토 후 응찰 가능 여부를 회원님이 기재하신 이메일로 알려드립니다."
                    : "Your request has been submitted.<br />After review, we will notify you of the result by the registered email address.";
                break;

            #endregion

            #region # Util - Login / Join #

            case "21":
                message = isKor ? "이미 가입된 아이디가 있습니다." : "Sorry, the ID is already in use.";
                break;
            case "22":
                message = isKor ? "현재 비밀번호가 일치하지 않습니다." : "The current passwords do not match.";
                break;
            case "23":
                message = isKor ? "아이디를 입력해 주십시오." : "Please enter your ID";
                break;
            case "24":
                message = isKor ? "만 19세 미만의 경우 회원 가입이 불가능합니다. 대표전화로 문의 부탁 드립니다." : "If you are under 19 years old, you cannot sign up as a member. Please make an inquiry to the representative phone.";
                break;
            case "25":
                message = isKor ? "이미 가입된 휴대폰 정보가 있습니다." : "There is already a registered mobile phone information.";
                break;
            case "26":
                message = isKor ? "이미 가입된 정보입니다. 로그인 정보를 분실하셨다면 아이디 또는 비밀번호 찾기를 통해 확인하시기 바랍니다." : "Your ID already exists, you cannot sign up as a member. Please log in with your existing ID.";
                break;
            case "27":
                message = isKor ? "본인 인증 요청시간이 초과 하였습니다." : "The time required for authentication has exceeded.";
                break;
            case "28":
                message = isKor ? "미수금이 있어 탈퇴 처리가 불가능합니다." : "Withdrawal processing is impossible due to receivables.";
                break;
            case "29":
                message = isKor ? "아이디가 일치하지 않습니다." : "The ID does not match.";
                break;
            case "201":
                message = isKor ? "아이디 또는 비밀번호가 일치 하지 않습니다." : "Incorrect ID and/or password";
                break;
            case "202":
                message = isKor ? "아이디를 입력해 주십시오." : "Please enter your ID";
                break;
            case "203":
                message = isKor ? "비밀번호를 입력해 주십시오." : "Please enter your password";
                break;
            case "204":
                message = isKor ? "죄송합니다. 회원님은 아래 조건에 따라 즉시탈퇴는 불가합니다.<br />조건 충족 후 진행 부탁드리며, 관련 문의는 별도 문의 바랍니다.<br /><br />v 진행중인 경매에 응찰한 경우<br />v 낙찰 건에 대한 대금지불이 완료되지 않은 경우<br /><br />• 탈퇴문의: 02-3479-8888" 
                    : "I'm sorry. You are not allowed to leave immediately according to the conditions below.<br />Please proceed after meeting the conditions, and contact us separately for related inquiries.<br /><br />v In case of bidding for an ongoing auction<br />In case the payment for the winning bid is not completed<br /><br />• Withdrawal inquiry: +82-2-3479-8888";
                break;
            case "205":
                message = isKor ? "인증하신 이름과 휴대폰 번호로 가입된 회원 정보가 존재하지 않습니다." : "The member information registered with your authenticated name and mobile phone number does not exist.";
                break;
            case "206":
                message = isKor ? "회원 본인만 본인인증이 가능합니다. 가입된 정보를 확인 후 다시 이용해주세요." : "Please register as a member. Check the registration information.";
                break;

            #endregion

            #region # MyPage - Apply Book #

            case "31":
                message = "도록 서비스 신청중입니다.";
                break;
            case "32":
                message = "이미 도록 서비스를 이용하고 계십니다.";
                break;

            #endregion # MyPage

            #region # Admin #

            case "91":
                message = isKor ? "자동응찰 내역 취소후에 다시 시도해 주십시오." : "자동응찰 내역 취소후에 다시 시도해 주십시오.";
                break;
            case "92":
                message = isKor ? "서면응찰 건은 케이오피스를 통해서 취소 처리가 가능합니다." : "서면응찰 건은 케이오피스를 통해서 취소 처리가 가능합니다.";
                break;
            case "93":
                message = isKor ? "경매종료된 작품의 응찰건은 취소할 수 없습니다." : "경매종료된 작품의 응찰건은 취소할 수 없습니다.";
                break;
            case "96":
                message = isKor ? "존재하지 않는 회원고유번호(홈페이지) 입니다." : "존재하지 않는 회원고유번호(홈페이지) 입니다.";
                break;

            #endregion

            #region # Common Error #

            case "90":
            case "ERROR":
                message = isKor ? "처리중 오류가 발생하였습니다." : "An error occurred during processing.";
                break;
            case "99":
                message = "에러가 발생했습니다.<br />담당자에게 문의 부탁 드립니다.";
                break;
            case "ACCESSDENY":
                message = isKor ? "권한이 없거나 허용되지 않은 접근입니다. 다시 로그인 하시기 바랍니다." : "Unauthorized or unauthorized access. Please log in again.";
                break;

            #endregion
        }
        return string.IsNullOrWhiteSpace(message) ? code : message;
    }

    public static string GetTitleFromAucKind(string value, string lang = "") => value switch
    {
        "1" => lang.Equals("ENG") ? "Major Live" : "메이저 경매",
        "2" => lang.Equals("ENG") ? "Premium Online" : "프리미엄 온라인",
        "4" => lang.Equals("ENG") ? "Weekly Online" : "위클리 온라인",
        _ => string.Empty,
    };

    public static string GetShortTitleFromAucKind(string value, string lang = "") => value switch
    {
        "1" => lang.Equals("ENG") ? "Major" : "메이저",
        "2" => lang.Equals("ENG") ? "Premium" : "프리미엄",
        "4" => lang.Equals("ENG") ? "Weekly" : "위클리",
        _ => string.Empty,
    };

    public static string GetDisplayDate(DateTime value, string lang = "KOR") => lang.Equals("ENG") ? value.ToString("MMMM yyyy") : value.ToString("yyyy년 MM월 dd일");

    public static string GetDisplayStartDate(DateTime value, string lang = "KOR")
        => lang.Equals("ENG") ? value.ToString("f", DateTimeFormatInfo.InvariantInfo)
        : value.ToString(string.Format("yyyy-M-d(ddd) H시 경매시작")).Replace("-", "/");

    public static string GetDisplayEndDate(DateTime value, string lang = "KOR")
        => lang.Equals("ENG") ? value.ToString("f", DateTimeFormatInfo.InvariantInfo)
        : value.ToString(string.Format("yyyy/M/d(ddd) H시부터 마감")).Replace("-", "/");

    public static string GetCurrencyFormat(decimal value) => String.Format("KRW {0:#,###}", value);
}