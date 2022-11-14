var enabledFlash = function () {
    /*
    //크롬은 비디오(저지연)으로 재생 > 성능 이슈로 제거
    if (navigator.userAgent.toLowerCase().indexOf("chrome") >= 0 && navigator.userAgent.toLowerCase().indexOf("edge") < 0) {
        return false;
    }

    alert(navigator.userAgent.toLowerCase());

    if (navigator.userAgent.toLowerCase().indexOf("safari") != -1) {
        return true;
    }
    */

    var agent = navigator.userAgent.toLowerCase();

    if (agent.indexOf("firefox") >= 0) {
        return false;
    }

    //윈도우 기반 PC 재생은 플래시 뷰어로 재생
    if (navigator.platform) {
        if ("win16|win32|win64".indexOf(navigator.platform.toLowerCase()) >= 0) {
            //if (agent.indexOf("chrome") >= 0 || agent.indexOf("msie") >= 0 || agent.indexOf("trident") >= 0 || agent.indexOf("edge") >= 0 || agent.indexOf("opr") >= 0) {
            if (agent.indexOf("msie") >= 0 || agent.indexOf("trident") >= 0) {
                return true;
            }
        }
        /*
        else if ("mac|macintel".indexOf(navigator.platform.toLowerCase()) >= 0) {
            if (!(agent.indexOf("chrome") >= 0 || agent.indexOf("msie") >= 0 || agent.indexOf("trident") >= 0 || agent.indexOf("edge") >= 0 || agent.indexOf("opr") >= 0)
                && agent.indexOf("safari") >= 0) {
                alert("해당 부라우저는 동영상을 재생할 수 없습니다. 다른 브라우저를 이용해 주세요.");
            }
        }
        */
    }

    try {
        var shockwave = new ActiveXObject("ShockwaveFlash.ShockwaveFlash");

        return !!(shockwave);
    } catch (e) {
        var shockwave = navigator.mimeTypes && navigator.mimeTypes["application/x-shockwave-flash"];

        return !!(shockwave && shockwave.enabledPlugin);
    }

    return false;
};

var enabledVideo = function () {

    var video = document.createElement("video");
    var type = "application/vnd.apple.mpegurl";
    var agent = navigator.userAgent.toLowerCase();

    if (agent.indexOf("firefox") >= 0 || agent.indexOf("chrome") >= 0 || agent.indexOf("edge") >= 0 || agent.indexOf("opr") >= 0) {
        return true;
    }

    return !!(video && video.canPlayType && video.canPlayType(type));
};

var enabledUnrealHTML5VideoPlayer = function () {
    //OS 체크
    //if (/Windows NT 6.1/i.test(window.navigator.appVersion))
    // return 0;

    // iOS 아이폰, 아이패드, 아이팟
    // if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {    
    //     return 2;
    // //}
    // //// 안드로이드
    // //else if (/Android/i.test(navigator.userAgent)) {
    // //    return 2;
    // } else {
    //     return 0;
    // }
    return 2
}