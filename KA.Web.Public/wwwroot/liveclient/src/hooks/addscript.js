function addscript() {
    console.log("AddScript");

    // 필요한 파일들을 동적으로 생성해줍니다.
    let js = document.createElement("script");
    js.src = "https://stream.camtour.net/src/rtc/webrtcadapter.js";
    js.async = true;
    document.head.appendChild(js);

    js = document.createElement("script");
    js.src = "https://stream.camtour.net/src/rtc/unrealwebrtcplayer.js";
    js.async = true;
    document.head.appendChild(js);
  }
  
  export default addscript;