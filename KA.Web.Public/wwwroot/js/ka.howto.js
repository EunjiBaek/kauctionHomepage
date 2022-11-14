//상단탭버튼 스크립트
//11월
var howtotopbtn = $(".howto-top > ul > li");
var howtosubbtn = $(".aucbtn > div"); 
var howtoCont = $(".howto-bot > div");

howtosubbtn.hide().eq(0).show();
howtoCont.hide().eq(0).show();

howtotopbtn.click(function(){

  nav.removeClass("active1").eq(0).addClass("active1");
  a2btn.removeClass("active2").eq(0).addClass("active2");
  a3btn.removeClass("active3").eq(0).addClass("active3");

  var target = $(this);
  var index = target.index();

  howtotopbtn.removeClass("open");
  target.addClass("open");
  howtosubbtn.css("display","none");
  howtosubbtn.eq(index).css("display","block");
  howtoCont.css("display","none");
  howtoCont.eq(index).css("display","block");
  $('.slider').slick('setPosition');

});




//경매소개 스크립트
var nav = $('.a1 > ul > li');
// var contents = $(".intro-desc > div");
nav.removeClass("active1").eq(0).addClass("active1");

var nav1 = $('.a1 > ul > li:nth-child(1)');
var nav2 = $('.a1 > ul > li:nth-child(2)');
var contents = $(".intro-auc > div");


nav1.click(function(e){  
e.preventDefalt;
//alert(i);
var section = $(contents.eq(0));
var offset = section.offset().top*0.67;
//alert(offset);
$("html, body").animate({scrollTop:offset},400,"swing");
});


nav2.click(function(e){
e.preventDefalt;
//alert(i);
var section2 = $(contents.eq(1));
var offset2 = section2.offset().top*0.9;
//alert(offset);
$("html, body").animate({scrollTop:offset2},600,"swing");
});



$(window).scroll(function(){

var wScroll = $(this).scrollTop();

if( wScroll >= contents.eq(0).offset().top*0.5){
   nav.removeClass("active1");
   nav.eq(0).addClass("active1");
   }
if( wScroll >= contents.eq(1).offset().top*0.7){
   nav.removeClass("active1");
   nav.eq(1).addClass("active1");
   }

});


//11월23일 추가 howto 버튼 고정 스크립트
$(".howtobtn").each(function(){

var header = $(this);
var headerOffset = header.offset().top;

$(window).scroll(function(){
    var wScroll = $(this).scrollTop();
    if(wScroll > headerOffset){
        header.addClass("fixed");
    }else{
        header.removeClass("fixed");
    }
});
});


//11월23일 주석처리
//howto 버튼 고정 스크립트
// var lastScrollTop = 0,
// delta = 15;
// var howtobtnHeight = $('.howtobtn').outerHeight();

// $(window).scroll(function(event) {

//   var st = $(this).scrollTop();
//   var header = $(".howtobtn");
//   // var headerOffset = header.offset().top;

//   if (Math.abs(lastScrollTop - st) <= delta) return;
//   if ((st > lastScrollTop) && (lastScrollTop > howtobtnHeight)) {
//     header.addClass("fixed");
//   } else {
//     header.removeClass("fixed");
//   }
//   lastScrollTop = st;

// });



//응찰안내 스크립트
var a2btn = $('.a2 > ul > li');
a2btn.removeClass("active2").eq(0).addClass("active2");

var a2btn1 = $('.a2 > ul > li:nth-child(1)');
var a2btn2 = $('.a2 > ul > li:nth-child(2)');
var a2btn3 = $('.a2 > ul > li:nth-child(3)');
var a2btn4 = $('.a2 > ul > li:nth-child(4)');
var a2btn5 = $('.a2 > ul > li:nth-child(5)');

var a2contents = $(".bid-guide > div");

//응찰안내 step01
a2btn1.click(function(e){
e.preventDefalt;

var a2sec1 = $(a2contents.eq(0));
var a2offset1 = a2sec1.offset().top*0.7;
$("html, body").animate({scrollTop:a2offset1},500,"swing");

});

//응찰안내 step02
a2btn2.click(function(e){
e.preventDefalt;

var a2sec2 = $(a2contents.eq(1));
var a2offset2 = a2sec2.offset().top*0.93;
$("html, body").animate({scrollTop:a2offset2},750,"swing");

});

//응찰안내 step03
a2btn3.click(function(e){
e.preventDefalt;

var a2sec3 = $(a2contents.eq(2));
var a2offset3 = a2sec3.offset().top*0.95;
$("html, body").animate({scrollTop:a2offset3},650,"swing");
});

//응찰안내 step04
a2btn4.click(function(e){
e.preventDefalt;

var a2sec4 = $(a2contents.eq(3));
var a2offset4 = a2sec4.offset().top*0.97;
$("html, body").animate({scrollTop:a2offset4},780,"swing");
});


//응찰안내 step05
a2btn5.click(function(e){
e.preventDefalt;

var a2sec5 = $(a2contents.eq(4));
var a2offset5 = a2sec5.offset().top*0.98;
$("html, body").animate({scrollTop:a2offset5},750,"swing");
});




$(window).scroll(function(){


var wScroll = $(this).scrollTop();

if( wScroll >= a2contents.eq(0).offset().top*0.8){

  a2btn.removeClass("active2");
  a2btn.eq(0).addClass("active2");
}

if( wScroll >= a2contents.eq(1).offset().top*0.85){
  a2btn.removeClass("active2");
  a2btn.removeClass("open");

  a2btn.eq(0).addClass("open");
  a2btn.eq(1).addClass("active2");
}

if( wScroll >= a2contents.eq(2).offset().top*0.91){
  a2btn.removeClass("active2");
  a2btn.removeClass("open");

  a2btn.eq(1).addClass("open");
  a2btn.eq(2).addClass("active2");
}

if( wScroll >= a2contents.eq(3).offset().top*0.94){
  a2btn.removeClass("active2");
  a2btn.removeClass("open");

  a2btn.eq(2).addClass("open");
  a2btn.eq(3).addClass("active2");
} 

if( wScroll >= a2contents.eq(4).offset().top*0.96){
  a2btn.removeClass("active2");
  a2btn.removeClass("open");

  a2btn.eq(3).addClass("open");
  a2btn.eq(4).addClass("active2");
}


});





//위탁안내 스크립트
var a3btn = $('.a3 > ul > li');
a3btn.removeClass("active3").eq(0).addClass("active3");


var a3btn1 = $('.a3 > ul > li:first-child');
var a3btn2 = $('.a3 > ul > li:last-child');
var a3contents = $(".cos-guide > div");


a3btn1.click(function(e){
e.preventDefalt;

var a3sec1 = $(a3contents.eq(0));
var a3secoffset1 = a3sec1.offset().top*0.68;

$("html, body").animate({scrollTop:a3secoffset1},800,"swing");

});

a3btn2.click(function(e){
e.preventDefalt;

var a3sec2 = $(a3contents.eq(1));
var a3secoffset2 = a3sec2.offset().top*0.97;

$("html, body").animate({scrollTop:a3secoffset2},800,"swing");

});


$(window).scroll(function(e){

e.stopPropagation;

var wScroll = $(this).scrollTop();

if( wScroll >= a3contents.eq(0).offset().top*0.7){
   a3btn.removeClass("active3");
   a3btn.eq(0).addClass("active3");
   }
if( wScroll >= a3contents.eq(1).offset().top*0.94){
   a3btn.removeClass("active3");
   a3btn.eq(1).addClass("active3");
   }

});





//위탁안내 > 위탁절차탭버튼
var csproceBtn = $(".csproce-btn > ul > li"); 
var csproceCont = $(".csproce-cot > div");      

csproceCont.hide().eq(0).show();
csproceBtn.click(function(e){

  e.preventDefault();

  var target = $(this);
  var index = target.index();
  csproceBtn.removeClass("active");
  target.addClass("active");
  csproceCont.css("display","none");
  csproceCont.eq(index).css("display","block");


});



//세로로 길때 스크립트
// window.onresize = function(){
//   if (self.name != 'reload') {
//     self.name = 'reload';
//     self.location.reload(true);
//   }
//   else self.name = ''; 
// }

$(window).each(function(){

$(window).resize(function(){

  if (1920 <= $(window).height()){
    $(window).off('scroll');
  } 
  else {
    $(window).on('scroll');
  }  
});

});


