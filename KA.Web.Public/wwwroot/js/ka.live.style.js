//top 언어 버튼
//$(".kor > a").on(clickOrTouch, function(e){
//    //e.preventDefault();
//    //e.stopPropagation();

//    $(".kor .op").toggleClass("bt");
//    $(".kor").toggleClass("open");
//});


//$(".krw > a").on(clickOrTouch, function(e){
//    e.preventDefault();
//    e.stopPropagation();

//    $(".krw .op").toggleClass("bt");
//    $(".krw").toggleClass("open");
//});

window.addEventListener('beforeunload', (event) => {
  event.preventDefault();
  document.documentElement.scrollLeft = 0;
});


window.addEventListener('beforeunload', (event) => {
  event.preventDefault();
  document.documentElement.scrollLeft = 0;
});



window.addEventListener('beforeunload', (event) => {
  event.preventDefault();
  document.documentElement.scrollLeft = 0;
});



//mid tap버튼 9월11일수정
var tabBtn2 = $(".midbt-btn > ul > li"); 
var tabCont2 = $(".midbt-content > div");      

tabCont2.hide().eq(0).show();

tabBtn2.on(clickOrTouch, function(e){
    e.preventDefault();
    e.stopPropagation();
  var target = $(this);
  var index = target.index();
  tabBtn2.removeClass("active");
  target.addClass("active");
  tabCont2.css("display","none");
  tabCont2.eq(index).css("display","block");
});


////10월14일수정 추가
//$('.mid .mid-middle .notice ul > li').hover(function () {
//    $(".notice-tooltip").css("display", "block");
//    $(".notice-all .notice-tooltip > p").text($(this).attr("value"));
//}, function () {
//    $(".notice-tooltip").css("display", "none");
//});


// 수정

